using System.Collections.Generic;
using JetBrains.Annotations;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    internal class Polygon : Polyline
    {
        public Point InsidePoint { get; }

        public Polygon([NotNull] IEnumerable<Point> points)
            : base(points)
        {
            InsidePoint = PointInsidePolygonCalculator.GetInsidePoint(Points, BoundingBox);
        }
    }
}
