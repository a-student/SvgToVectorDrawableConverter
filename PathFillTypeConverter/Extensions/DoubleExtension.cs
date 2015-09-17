using System;

namespace PathFillTypeConverter.Extensions
{
    static class DoubleExtension
    {
        private const double Precision = 0.00001;

        public static bool IsEqualTo(this double self, double d) => Math.Abs(self - d) < Precision;

        public static bool IsZero(this double self) => self.IsEqualTo(0);
    }
}
