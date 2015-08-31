using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
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

        public SvgToVectorDocumentConverter([NotNull] string blankVectorDrawablePath)
        {
            _blankVectorDrawablePath = blankVectorDrawablePath;
        }

        private bool _isFillTypeSupported;
        private bool _isStrokeDasharrayUsed;

        private void Reset()
        {
            _isFillTypeSupported = true;
            _isStrokeDasharrayUsed = false;
        }

        [NotNull]
        public IList<string> Warnings
        {
            get
            {
                var warnings = new List<string>();
                if (!_isFillTypeSupported)
                {
                    warnings.Add("SVG fill-rule and clip-rule are not properly supported on Android. Please, read https://github.com/a-student/SvgToVectorDrawableConverter#not-supported-svg-features");
                }
                if (_isStrokeDasharrayUsed)
                {
                    warnings.Add("The stroke-dasharray attribute is not supported.");
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
            if (viewBox.X != 0 || viewBox.Y != 0)
            {
                throw new UnsupportedFormatException("X and y coordinates of viewBox must be zeros.");
            }
            vectorDocument.Root.ViewportWidth = viewBox.Width;
            vectorDocument.Root.ViewportHeight = viewBox.Height;

            vectorDocument.Root.Width = ConvertToDp(svgDocument.Root.Width, viewBox.Width);
            vectorDocument.Root.Height = ConvertToDp(svgDocument.Root.Height, viewBox.Height);

            AppendAll(vectorDocument.Root.Children, svgDocument.Root.Children, StyleHelper.MergeStyles(StyleHelper.InitialStyles, svgDocument.Root.Style));

            VectorOptimizer.Optimize(vectorDocument.Root);
            return vectorDocument;
        }

        private static string ConvertToDp(string dimension, double viewBox)
        {
            Func<double, string> format = x => string.Format(CultureInfo.InvariantCulture, "{0}dp", x);

            if (string.IsNullOrEmpty(dimension))
            {
                return format(viewBox);
            }

            var value = double.Parse(Regex.Replace(dimension, "[^0-9.]", ""), CultureInfo.InvariantCulture);

            if (dimension.EndsWith("in"))
            {
                return format(value * 90);
            }
            if (dimension.EndsWith("cm"))
            {
                return format(value * 35.43307);
            }
            if (dimension.EndsWith("mm"))
            {
                return format(value * 3.543307);
            }
            if (dimension.EndsWith("pt"))
            {
                return format(value * 1.25);
            }
            if (dimension.EndsWith("pc"))
            {
                return format(value * 15);
            }
            if (dimension.EndsWith("%"))
            {
                return format(viewBox * value / 100);
            }
            return format(value);
        }

        private void InitRecursively(Group group, G g, StringDictionary parentStyle)
        {
            var style = StyleHelper.MergeStyles(parentStyle, g.Style);
            Init(group, g.Transform, style);
            AppendAll(group.Children, g.Children, style);
        }

        private void AppendAll(ElementCollection elements, ElementCollection children, StringDictionary parentStyle)
        {
            foreach (var child in children)
            {
                if (child is IStyleableElement && !IsDisplayed((IStyleableElement)child))
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
                    throw new UnsupportedFormatException($"Met unsupported element '{child}'.");
                }
            }
        }

        private static bool IsDisplayed(IStyleableElement element)
        {
            return element.Style["display"] != "none";
        }

        private void Init(Group group, SvgPath svgPath, StringDictionary parentStyle)
        {
            var style = StyleHelper.MergeStyles(parentStyle, svgPath.Style);
            Init(group, svgPath.Transform, style);
            var vdPath = group.Children.Append<VdPath>();
            vdPath.PathData = PathDataFixer.Fix(svgPath.D);

            foreach (string key in style.Keys)
            {
                var value = style[key];
                switch (key)
                {
                    case "fill":
                        if (value.StartsWith("#"))
                        {
                            vdPath.FillColor = value;
                        }
                        break;
                    case "stroke":
                        if (value.StartsWith("#"))
                        {
                            vdPath.StrokeColor = value;
                        }
                        break;
                    case "stroke-width":
                        vdPath.StrokeWidth = float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "stroke-opacity":
                        vdPath.StrokeAlpha = float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "fill-opacity":
                        vdPath.FillAlpha = float.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "stroke-linecap":
                        vdPath.StrokeLineCap = value;
                        break;
                    case "stroke-linejoin":
                        vdPath.StrokeLineJoin = value;
                        break;
                    case "stroke-miterlimit":
                        vdPath.StrokeMiterLimit = value;
                        break;
                    case "fill-rule":
                        SetFillType(vdPath, value);
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
                group.ScaleX = Math.Sign(matrix.A) * Math.Sqrt(matrix.A * matrix.A + matrix.C * matrix.C);
                group.ScaleY = Math.Sign(matrix.D) * Math.Sqrt(matrix.B * matrix.B + matrix.D * matrix.D);
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
            if (!string.IsNullOrEmpty(clipPath))
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
                    vdClipPath.PathData = PathDataFixer.Fix(x.Path.D);
                    SetFillType(vdClipPath, x.Style["clip-rule"]);
                }
            }
        }

        private void SetFillType(PathBase path, string rule)
        {
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
                path.FillType = fillType.Value;
                if (path.FillType != fillType.Value)
                {
                    _isFillTypeSupported = false;
                }
            }
        }
    }
}
