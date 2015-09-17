using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    enum SubpathDirection
    {
        Clockwise,
        Counterclockwise
    }

    static class SubpathDirectionCalculator
    {
        public static SubpathDirection CalculateDirection(Subpath subpath)
        {
            var points = subpath.PolygonApproximation.Points;

            double doubleArea = 0;
            var point1 = points.Last();
            foreach (var point2 in points)
            {
                doubleArea += (point2.X - point1.X) * (point1.Y + point2.Y);
                point1 = point2;
            }
            return doubleArea <= 0 ? SubpathDirection.Clockwise : SubpathDirection.Counterclockwise;
        }
    }
}
