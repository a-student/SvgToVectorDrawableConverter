using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using PathFillTypeConverter;
using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;
using SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics;
using SvgToVectorDrawableConverter.DataFormat.VectorDrawable;
using Group = SvgToVectorDrawableConverter.DataFormat.VectorDrawable.Group;
using VdPath = SvgToVectorDrawableConverter.DataFormat.VectorDrawable.Path;
using SvgPath = SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics.Path;
using VdClipPath = SvgToVectorDrawableConverter.DataFormat.VectorDrawable.ClipPath;
using SvgClipPath = SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics.ClipPath;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    class SvgToVectorDocumentConverter
    {
        [NotNull]
        private readonly string _blankVectorDrawablePath;
        private readonly bool _fixFillType;

        public SvgToVectorDocumentConverter([NotNull] string blankVectorDrawablePath, bool fixFillType)
        {
            _blankVectorDrawablePath = blankVectorDrawablePath;
            _fixFillType = fixFillType;
        }

        private bool _isFillTypeSupported;
        private bool _isStrokeDasharrayUsed;
        private bool _isGroupOpacityUsed;
        private readonly HashSet<string> _unsupportedElements = new HashSet<string>();

        private void Reset()
        {
            _isFillTypeSupported = true;
            _isStrokeDasharrayUsed = false;
            _isGroupOpacityUsed = false;
            _unsupportedElements.Clear();
        }

        [NotNull]
        public IList<string> Warnings
        {
            get
            {
                var warnings = new List<string>();
                if (!_isFillTypeSupported)
                {
                    warnings.Add("SVG fill-rule and clip-rule are not properly supported on Android. Please, read https://github.com/a-student/SvgToVectorDrawableConverter#not-supported-svg-features. Try specifying the --fix-fill-type option.");
                }
                if (_isStrokeDasharrayUsed)
                {
                    warnings.Add("The stroke-dasharray attribute is not supported.");
                }
                if (_isGroupOpacityUsed)
                {
                    warnings.Add("Group opacity is not supported on Android. Please, apply opacity to path elements instead of a group.");
                }
                if (_unsupportedElements.Count > 0)
                {
                    warnings.Add($"Met unsupported element(s): {string.Join(", ", _unsupportedElements)}.");
                }
                return warnings.AsReadOnly();
            }
        }

        private Dictionary<string, Element> _map;

        private static void FillMap(IDictionary<string, Element> map, Element root)
        {
            if (!string.IsNullOrEmpty(root.Id))
            {
                map[root.Id] = root;
            }
            if (!(root is ElementWithChildren))
            {
                return;
            }
            foreach (var child in ((ElementWithChildren)root).Children)
            {
                FillMap(map, child);
            }
        }

        [NotNull]
        public DocumentWrapper<Vector> Convert([NotNull] DocumentWrapper<Svg> svgDocument)
        {
            Reset();

            _map = new Dictionary<string, Element>();
            FillMap(_map, svgDocument.Root);

            var vectorDocument = VectorDocumentWrapper.CreateFromFile(_blankVectorDrawablePath);

            var viewBox = svgDocument.Root.ViewBox;

            vectorDocument.Root.ViewportWidth = viewBox.Width;
            vectorDocument.Root.ViewportHeight = viewBox.Height;

            vectorDocument.Root.Width = ConvertToDp(svgDocument.Root.Width, viewBox.Width);
            vectorDocument.Root.Height = ConvertToDp(svgDocument.Root.Height, viewBox.Height);

            var style = StyleHelper.MergeStyles(StyleHelper.InitialStyles, svgDocument.Root.Style);

            vectorDocument.Root.Alpha = float.Parse(style["opacity"] ?? "1", CultureInfo.InvariantCulture);

            var group = vectorDocument.Root.Children.Append<Group>();
            group.TranslateX = -viewBox.X;
            group.TranslateY = -viewBox.Y;
            AppendAll(group.Children, svgDocument.Root.Children, style);

            VectorOptimizer.Optimize(vectorDocument.Root);
            return vectorDocument;
        }

        private static string ConvertToDp(string length, double reference)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}dp", UnitConverter.ConvertToPx(length, reference));
        }

        private void InitRecursively(Group group, G g, StringDictionary parentStyle)
        {
            var style = StyleHelper.MergeStyles(parentStyle, g.Style);
            Init(group, g.Transform, style);
            AppendAll(group.Children, g.Children, style);

            _isGroupOpacityUsed |= style["opacity"] != null;
        }

        private void AppendAll(ElementCollection elements, ElementCollection children, StringDictionary parentStyle)
        {
            foreach (var child in children)
            {
                if (!IsDisplayed(child))
                {
                    continue;
                }
                if (child is G)
                {
                    InitRecursively(elements.Append<Group>(), (G)child, parentStyle);
                    continue;
                }
                if (child is SvgPath)
                {
                    Init(elements.Append<Group>(), (SvgPath)child, parentStyle);
                    continue;
                }
                if (child is UnsupportedElement)
                {
                    _unsupportedElements.Add(child.ToString());
                }
            }
        }

        private static bool IsDisplayed(Element element)
        {
            return Styler.GetStyle(element)["display"] != "none";
        }

        private void Init(Group group, SvgPath svgPath, StringDictionary parentStyle)
        {
            var style = StyleHelper.MergeStyles(parentStyle, svgPath.Style);
            Init(group, svgPath.Transform, style);
            var fillPath = group.Children.Append<VdPath>();
            var strokePath = fillPath;

            fillPath.PathData = svgPath.D;
            if (style.ContainsKey("fill") && SetFillType(fillPath, style["fill-rule"]))
            {
                strokePath = group.Children.Append<VdPath>();
                strokePath.PathData = PathDataFixer.Fix(svgPath.D);
            }
            fillPath.PathData = PathDataFixer.Fix(fillPath.PathData);

            foreach (string key in style.Keys)
            {
                var value = style[key];
                switch (key)
                {
                    case "fill":
                        if (value.StartsWith("#"))
                        {
                            fillPath.FillColor = value;
                        }
                        break;
                    case "stroke":
                        if (value.StartsWith("#"))
                        {
                            strokePath.StrokeColor = value;
                        }
                        break;
                    case "stroke-width":
                        strokePath.StrokeWidth = (float)UnitConverter.ConvertToPx(value, 0);
                        break;
                    case "stroke-opacity":
                        strokePath.StrokeAlpha *= float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "fill-opacity":
                        fillPath.FillAlpha *= float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "opacity":
                        strokePath.StrokeAlpha *= float.Parse(value, CultureInfo.InvariantCulture);
                        fillPath.FillAlpha *= float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "stroke-linecap":
                        strokePath.StrokeLineCap = value;
                        break;
                    case "stroke-linejoin":
                        strokePath.StrokeLineJoin = value;
                        break;
                    case "stroke-miterlimit":
                        strokePath.StrokeMiterLimit = value;
                        break;
                    case "stroke-dasharray":
                        _isStrokeDasharrayUsed |= value != "none";
                        break;
                }
            }
        }

        private void Init(Group group, Transform transform, StringDictionary style)
        {
            if (transform is Transform.Matrix)
            {
                var matrix = (Transform.Matrix)transform;
                group.ScaleX = (matrix.A >= 0 ? 1 : -1) * Math.Sqrt(matrix.A * matrix.A + matrix.C * matrix.C);
                group.ScaleY = (matrix.D >= 0 ? 1 : -1) * Math.Sqrt(matrix.B * matrix.B + matrix.D * matrix.D);
                group.Rotation = Math.Atan(matrix.B / matrix.D) * 180 / Math.PI;
                group.TranslateX = matrix.E;
                group.TranslateY = matrix.F;
            }
            if (transform is Transform.Translate)
            {
                var translate = (Transform.Translate)transform;
                group.TranslateX = translate.Tx;
                group.TranslateY = translate.Ty;
            }
            if (transform is Transform.Scale)
            {
                var scale = (Transform.Scale)transform;
                group.ScaleX = scale.Sx;
                group.ScaleY = scale.Sy;
            }
            if (transform is Transform.Rotate)
            {
                var rotate = (Transform.Rotate)transform;
                group.Rotation = rotate.Angle;
                group.PivotX = rotate.Cx;
                group.PivotY = rotate.Cy;
            }

            var clipPath = style["clip-path"];
            if (!string.IsNullOrEmpty(clipPath) && clipPath != "none")
            {
                var match = Regex.Match(clipPath, @"^url\(#(?<key>.+)\)$");
                if (!match.Success)
                {
                    throw new UnsupportedFormatException("Wrong clip-path attribute value.");
                }
                var key = match.Groups["key"].Value;
                foreach (var x in ClipPathHelper.ExtractPaths((SvgClipPath)_map[key]))
                {
                    var vdClipPath = group.Children.Append<VdClipPath>();
                    vdClipPath.PathData = x.Path.D;
                    SetFillType(vdClipPath, x.Style["clip-rule"]);
                    vdClipPath.PathData = PathDataFixer.Fix(vdClipPath.PathData);
                }
            }
        }

        private bool SetFillType(PathBase path, string rule)
        {
            var separatePathForStroke = false;
            FillType? fillType = null;
            switch (rule)
            {
                case "nonzero":
                    fillType = FillType.winding;
                    break;
                case "evenodd":
                    fillType = FillType.even_odd;
                    break;
            }
            if (fillType.HasValue)
            {
                if (fillType.Value == FillType.even_odd && _fixFillType)
                {
                    try
                    {
                        path.PathData = PathDataConverter.ConvertFillTypeFromEvenOddToWinding(path.PathData, out separatePathForStroke);
                        fillType = FillType.winding;
                    }
                    catch (Exception e)
                    {
                        throw new FixFillTypeException(e);
                    }
                }

                path.FillType = fillType.Value;
                if (path.FillType != fillType.Value)
                {
                    _isFillTypeSupported = false;
                }
            }
            return separatePathForStroke;
        }
    }
}
