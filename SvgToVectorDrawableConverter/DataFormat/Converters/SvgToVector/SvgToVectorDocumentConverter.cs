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

        private bool _isFillRuleSupported;

        private void Reset()
        {
            _isFillRuleSupported = true;
        }

        [NotNull]
        public IList<string> Warnings
        {
            get
            {
                var warnings = new List<string>();
                if (!_isFillRuleSupported)
                {
                    warnings.Add("SVG fill-rule is not properly supported on Android. Please, read https://github.com/a-student/SvgToVectorDrawableConverter#not-supported-svg-features");
                }
                return warnings.AsReadOnly();
            }
        }

        [NotNull]
        public DocumentWrapper<Vector> Convert([NotNull] DocumentWrapper<Svg> svgDocument)
        {
            Reset();

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
            Init(group, g.Transform);

            AppendAll(group.Children, g.Children, StyleHelper.MergeStyles(parentStyle, g.Style));
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
                if (child is Metadata || child is Title || child is Desc || child is Defs)
                {
                    continue;
                }
                throw new UnsupportedFormatException($"Met unallowed element '{child}'.");
            }
        }

        private void Init(Group group, SvgPath svgPath, StringDictionary parentStyle)
        {
            Init(group, svgPath.Transform);

            var vdPath = group.Children.Append<VdPath>();

            vdPath.PathData = PathDataFixer.Fix(svgPath.D);

            var style = StyleHelper.MergeStyles(parentStyle, svgPath.Style);
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
                        FillType? fillType = null;
                        switch (value)
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
                            vdPath.FillType = fillType.Value;
                            if (vdPath.FillType != fillType.Value)
                            {
                                _isFillRuleSupported = false;
                            }
                        }
                        break;
                }
            }
        }

        private static void Init(Group group, Transform transform)
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
        }

        private static bool IsDisplayed(IStyleableElement element)
        {
            return element.Style["display"] != "none";
        }
    }
}
