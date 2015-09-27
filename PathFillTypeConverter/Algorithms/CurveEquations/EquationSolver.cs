using static System.Math;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    static class EquationSolver
    {
        public static void SolveLinear(double a, double b, out double x)
        {
            x = -b / a;
        }

        public static void SolveQuadratic(double a, double b, double c, out double x1, out double x2)
        {
            if (a == 0)
            {
                SolveLinear(b, c, out x1);
                x2 = double.NaN;
            }
            else
            {
                var d = Square(b) - 4 * a * c;
                d = Max(d, 0);
                d = Sqrt(d);
                x1 = (-b + d) / (2 * a);
                x2 = (-b - d) / (2 * a);
            }
        }

        public static void SolveCubic(double a, double b, double c, double d, out double x1, out double x2, out double x3)
        {
            if (a == 0)
            {
                SolveQuadratic(b, c, d, out x1, out x2);
                x3 = double.NaN;
            }
            else
            {
                var p = (3 * a * c - Square(b)) / (3 * Square(a));
                p /= 3;
                var q = (2 * Cube(b) - 9 * a * b * c + 27 * Square(a) * d) / (27 * Cube(a));
                q /= 2;
                var delta = Cube(p) + Square(q);
                var r = (q >= 0 ? 1 : -1) * Sqrt(Abs(p));
                if (p < 0)
                {
                    if (delta <= 0)
                    {
                        var phi = Acos(q / Cube(r));
                        x1 = -2 * r * Cos(phi / 3) - b / (3 * a);
                        x2 = 2 * r * Cos((PI - phi) / 3) - b / (3 * a);
                        x3 = 2 * r * Cos((PI + phi) / 3) - b / (3 * a);
                    }
                    else
                    {
                        var phi = Acosh(q / Cube(r));
                        x1 = -2 * r * Cosh(phi / 3) - b / (3 * a);
                        if (phi == 0)
                        {
                            x2 = r - b / (3 * a);
                        }
                        else
                        {
                            x2 = double.NaN;
                        }
                        x3 = double.NaN;
                    }
                }
                else if (p > 0)
                {
                    var phi = Asinh(q / Cube(r));
                    x1 = -2 * r * Sinh(phi / 3) - b / (3 * a);
                    x2 = x3 = double.NaN;
                }
                else
                {
                    x1 = Cbrt(-2 * q) - b / (3 * a);
                    x2 = x3 = double.NaN;
                }
            }
        }
    }
}
