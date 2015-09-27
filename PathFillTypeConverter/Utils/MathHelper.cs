using static System.Math;
using System.Runtime.CompilerServices;

namespace PathFillTypeConverter.Utils
{
    static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Square(double value)
        {
            return value * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cube(double value)
        {
            return value * value * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(double angle)
        {
            return angle * PI / 180;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(double angle)
        {
            return angle * 180 / PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cbrt(double value)
        {
            return Sign(value) * Pow(Abs(value), 1d / 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Asinh(double value)
        {
            return Log(value + Sqrt(Square(value) + 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Acosh(double value)
        {
            return Log(value + Sqrt(Square(value) - 1));
        }
    }
}
