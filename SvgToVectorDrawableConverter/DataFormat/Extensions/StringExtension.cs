using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Extensions
{
    static class StringExtension
    {
        public static string FirstCharToLower([NotNull] this string s)
        {
            if (s.Length == 0)
            {
                return s;
            }
            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}
