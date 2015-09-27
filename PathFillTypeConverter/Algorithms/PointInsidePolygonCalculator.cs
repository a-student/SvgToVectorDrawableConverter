using System;
using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Diagnostics;

namespace PathFillTypeConverter.Algorithms
{
    static class PointInsidePolygonCalculator
    {
        public static Point GetInsidePoint(IReadOnlyList<Point> points, Box boundingBox)
        {
            var y = (boundingBox.MinY + boundingBox.MaxY) / 2;
            var xIntersections = new List<double>();
            var point1 = points.Last();
            foreach (var point2 in points)
            {
                if ((y > point1.Y) != (y > point2.Y))
                {
                    xIntersections.Add((y - point2.Y) * (point1.X - point2.X) / (point1.Y - point2.Y) + point2.X);
                }
                point1 = point2;
            }
            xIntersections.Sort();
            Debugger.BreakWhen(xIntersections.Count == 0 || xIntersections.Count % 2 != 0);
            var x = (boundingBox.MinX + boundingBox.MaxX) / 2;
            var maxDelta = double.NegativeInfinity;
            for (var i = 0; i < xIntersections.Count - 1; i += 2)
            {
                var delta = Math.Abs(xIntersections[i] - xIntersections[i + 1]);
                if (delta > maxDelta)
                {
                    x = (xIntersections[i] + xIntersections[i + 1]) / 2;
                    maxDelta = delta;
                }
            }
            var point = new Point(x, y);
#if DEBUG
            Debugger.BreakWhen(!PointInPolygonTest.Contains(points, point));
#endif
            return point;
        }
    }
}
