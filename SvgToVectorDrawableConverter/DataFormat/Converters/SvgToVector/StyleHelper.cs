using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    internal static class StyleHelper
    {
        [NotNull]
        public static readonly StringDictionary InitialStyles = new StringDictionary
        {
            { "fill", "#000000" },
            { "stroke-width", "1" },
            { "fill-rule", "nonzero" }
        };

        private static readonly HashSet<string> NotInheritedStyles = new HashSet<string>
        {
            "clip-path",
            "opacity",
            "display"
        };

        [NotNull]
        public static StringDictionary MergeStyles([NotNull] StringDictionary parentStyle, [NotNull] StringDictionary style)
        {
            const string inherit = "inherit";

            var result = new StringDictionary();
            foreach (string key in parentStyle.Keys)
            {
                if (style[key] == inherit || !NotInheritedStyles.Contains(key))
                {
                    result[key] = parentStyle[key];
                }
            }
            foreach (string key in style.Keys)
            {
                var value = style[key];
                if (value != inherit)
                {
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
