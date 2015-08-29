using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    internal static class Parser
    {
        public static Rect ParseViewBox([NotNull] string source)
        {
            // do not use Rect.Parse() - it is not implemented in Mono

            if (source == null)
            {
                throw new UnsupportedFormatException("The viewBox attribute must be set.");
            }

            var split = source.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new Rect(
                double.Parse(split[0], CultureInfo.InvariantCulture),
                double.Parse(split[1], CultureInfo.InvariantCulture),
                double.Parse(split[2], CultureInfo.InvariantCulture),
                double.Parse(split[3], CultureInfo.InvariantCulture)
            );
        }

        [NotNull]
        public static StringDictionary ParseStyle([CanBeNull] string source)
        {
            var result = new StringDictionary();
            if (source != null)
            {
                var matches = Regex.Matches(source, "(?<key>[^:;]+):(?<value>[^:;]+)");
                foreach (Match match in matches)
                {
                    result[match.Groups["key"].Value] = match.Groups["value"].Value;
                }
            }
            return result;
        }

        [CanBeNull]
        public static Transform ParseTransform([CanBeNull] string source)
        {
            if (source == null)
            {
                return null;
            }

            var match = Regex.Match(source, @"^(?<definition>[a-zA-Z]+)\((?<values>[^()]+)\)$");
            if (!match.Success)
            {
                throw new UnsupportedFormatException("Only one definition is supported in a transform list.");
            }
            var definition = match.Groups["definition"].Value;
            var values = match.Groups["values"].Value
                .Split(',', ' ')
                .Select(x => double.Parse(x, CultureInfo.InvariantCulture))
                .ToArray();
            switch (definition.ToLower())
            {
                case "matrix":
                    return new Transform.Matrix
                    {
                        A = values[0],
                        B = values[1],
                        C = values[2],
                        D = values[3],
                        E = values[4],
                        F = values[5]
                    };
                case "translate":
                    return new Transform.Translate
                    {
                        Tx = values[0],
                        Ty = values.Length > 1 ? values[1] : 0
                    };
                case "scale":
                    return new Transform.Scale
                    {
                        Sx = values[0],
                        Sy = values.Length > 1 ? values[1] : values[0]
                    };
                case "rotate":
                    return new Transform.Rotate
                    {
                        Angle = values[0],
                        Cx = values.Length >= 3 ? values[1] : 0,
                        Cy = values.Length >= 3 ? values[2] : 0
                    };
                default:
                    throw new UnsupportedFormatException(string.Format("Transform definition '{0}' is not supported.", definition));
            }
        }
    }
}
