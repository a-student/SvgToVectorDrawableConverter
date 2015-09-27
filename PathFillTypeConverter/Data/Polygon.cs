using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public class Polygon : Polyline
    {
        public Polygon([NotNull] IEnumerable<Point> points)
            : base(points)
        { }

        private Point? _insidePoint;

        public Point InsidePoint
        {
            get
            {
                if (!_insidePoint.HasValue)
                {
                    _insidePoint = PointInsidePolygonCalculator.GetInsidePoint(Points, BoundingBox);
                }
                return _insidePoint.Value;
            }
        }
    }
}
