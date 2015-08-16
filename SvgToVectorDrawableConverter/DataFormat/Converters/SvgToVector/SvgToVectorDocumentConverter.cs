using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;
using SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics;
using SvgToVectorDrawableConverter.DataFormat.VectorDrawable;
using Group = SvgToVectorDrawableConverter.DataFormat.VectorDrawable.Group;
using VdPath = SvgToVectorDrawableConverter.DataFormat.VectorDrawable.Path;
using SvgPath = SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics.Path;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    static class SvgToVectorDocumentConverter
    {
        public static DocumentWrapper<Vector> Convert(DocumentWrapper<Svg> svgDocument, string blankVectorDrawablePath)
        {
            var vectorDocument = VectorDocumentWrapper.CreateFromFile(blankVectorDrawablePath);

            var viewBox = svgDocument.Root.ViewBox;
            if (viewBox.X != 0 || viewBox.Y != 0)
            {
                throw new UnsupportedFormatException("X and y coordinates of viewBox must be zeros.");
            }
            vectorDocument.Root.ViewportWidth = viewBox.Width;
            vectorDocument.Root.ViewportHeight = viewBox.Height;

            vectorDocument.Root.Width = ConvertToDp(svgDocument.Root.Width, viewBox.Width);
            vectorDocument.Root.Height = ConvertToDp(svgDocument.Root.Height, viewBox.Height);

            foreach (var child in svgDocument.Root.Children)
            {
                if (child is G)
                {
                    InitRecursively(vectorDocument.Root.Children.Append<Group>(), (G)child, svgDocument.Root.Style);
                }
                if (child is SvgPath)
                {
                    Init(vectorDocument.Root.Children.Append<Group>(), (SvgPath)child, svgDocument.Root.Style);
                }
            }

            VectorOptimizer.Optimize(vectorDocument.Root);
            return vectorDocument;
        }

        private static string ConvertToDp(string dimension, double viewBox)
        {
            var value = double.Parse(Regex.Replace(dimension, "[^0-9.]", ""), CultureInfo.InvariantCulture);
            Func<double, string> format = x => string.Format(CultureInfo.InvariantCulture, "{0}dp", x);

            if (dimension.EndsWith("in"))
            {
                return format(value * 45);
            }
            if (dimension.EndsWith("cm"))
            {
                return format(value * 17.7);
            }
            if (dimension.EndsWith("mm"))
            {
                return format(value * 1.77);
            }
            if (dimension.EndsWith("pt"))
            {
                return format(value * 0.625);
            }
            if (dimension.EndsWith("pc"))
            {
                return format(value * 7.5);
            }
            if (dimension.EndsWith("%"))
            {
                return format(viewBox * value / 200);
            }
            return format(value / 2);
        }

        private static void InitRecursively(Group group, G g, StringDictionary parentStyle)
        {
            Init(group, g.Transform);

            var style = MergeStyles(parentStyle, g.Style);

            foreach (var child in g.Children)
            {
                if (child is G)
                {
                    InitRecursively(group.Children.Append<Group>(), (G)child, style);
                    continue;
                }
                if (child is SvgPath)
                {
                    Init(group.Children.Append<Group>(), (SvgPath)child, style);
                    continue;
                }
                throw new UnsupportedFormatException(string.Format("Met unallowed element '{0}'.", child));
            }
        }

        private static void Init(Group group, SvgPath svgPath, StringDictionary parentStyle)
        {
            Init(group, svgPath.Transform);

            var vdPath = group.Children.Append<VdPath>();

            vdPath.PathData = PathDataFixer.Fix(svgPath.D);

            var style = MergeStyles(parentStyle, svgPath.Style);
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
                        switch (value)
                        {
                            case "nonzero":
                                vdPath.FillType = FillType.winding;
                                break;
                            case "evenodd":
                                vdPath.FillType = FillType.even_odd;
                                break;
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

        private static StringDictionary MergeStyles(StringDictionary parentStyle, StringDictionary style)
        {
            var result = new StringDictionary();
            foreach (string key in parentStyle.Keys)
            {
                result[key] = parentStyle[key];
            }
            foreach (string key in style.Keys)
            {
                var value = style[key];
                if (value != "inherit")
                {
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
