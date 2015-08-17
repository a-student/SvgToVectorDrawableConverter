using System.Collections.Specialized;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    internal static class StyleHelper
    {
        public static readonly StringDictionary InitialStyles = new StringDictionary
        {
            { "fill", "#000000" },
            { "stroke-width", "1" },
            { "fill-rule", "nonzero" }
        };

        public static StringDictionary MergeStyles(StringDictionary parentStyle, StringDictionary style)
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
