using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathFillAreaCalculator
    {
        public static double CalculateSigned(Subpath subpath)
        {
            var points = subpath.PolygonApproximation.Points;

            double doubleArea = 0;
            var point1 = points.Last();
            foreach (var point2 in points)
            {
                doubleArea += (point2.X - point1.X) * (point1.Y + point2.Y);
                point1 = point2;
            }
            return doubleArea / 2;
        }
    }
}
