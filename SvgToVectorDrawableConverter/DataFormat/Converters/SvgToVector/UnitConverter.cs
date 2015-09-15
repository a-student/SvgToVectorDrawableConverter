using System.Globalization;
using System.Text.RegularExpressions;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    internal static class UnitConverter
    {
        public static double ConvertToPx(string length, double reference)
        {
            length = length?.ToLowerInvariant() ?? "";

            var number = Regex.Replace(length, "[^0-9.]", "");
            if (number.Length == 0)
            {
                return reference;
            }
            var value = double.Parse(number, CultureInfo.InvariantCulture);

            if (length.EndsWith("in"))
            {
                return value * 90;
            }
            if (length.EndsWith("cm"))
            {
                return value * 35.43307;
            }
            if (length.EndsWith("mm"))
            {
                return value * 3.543307;
            }
            if (length.EndsWith("pt"))
            {
                return value * 1.25;
            }
            if (length.EndsWith("pc"))
            {
                return value * 15;
            }
            if (length.EndsWith("%"))
            {
                return reference * value / 100;
            }
            return value;
        }
    }
}
