using System;

namespace PathFillTypeConverter.Extensions
{
    static class DoubleExtension
    {
        private const int PrecisionDigits = 5;
        private static readonly double Precision = Math.Pow(10, -PrecisionDigits);

        public static bool IsEqualTo(this double self, double d) => Math.Abs(self - d) < Precision;

        public static bool IsZero(this double self) => self.IsEqualTo(0);

        public static double RoundToPrecision(this double self) => Math.Round(self, PrecisionDigits);

        public static bool IsFiniteNumber(this double self)
        {
            return !double.IsInfinity(self) && !double.IsNaN(self);
        }
    }
}
