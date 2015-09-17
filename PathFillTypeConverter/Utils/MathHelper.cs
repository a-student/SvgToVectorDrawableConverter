using System;
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
            return angle * Math.PI / 180;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(double angle)
        {
            return angle * 180 / Math.PI;
        }
    }
}
