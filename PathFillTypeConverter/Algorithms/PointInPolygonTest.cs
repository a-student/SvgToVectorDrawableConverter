using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class PointInPolygonTest
    {
        public static bool Contains(Polygon polygon, Point point)
        {
            return polygon.BoundingBox.Contains(point) && Contains(polygon.Points, point);
        }

        public static bool Contains(IReadOnlyList<Point> points, Point point)
        {
            var contains = false;
            var point1 = points.Last();
            foreach (var point2 in points)
            {
                if ((point.Y > point1.Y) != (point.Y > point2.Y) &&
                    (point.X < point1.X || point.X < point2.X) &&
                    point.X < (point.Y - point2.Y) * (point1.X - point2.X) / (point1.Y - point2.Y) + point2.X)
                {
                    contains ^= true;
                }
                point1 = point2;
            }
            return contains;
        }
    }
}
