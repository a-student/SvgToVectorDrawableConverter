using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public abstract class SegmentBase
    {
        public abstract Point EndPoint { get; }

        public Polyline PolylineApproximation { get; private set; }

        public void BuildPolylineApproximation(Point startPoint)
        {
            if (PolylineApproximation == null)
            {
                PolylineApproximation = new Polyline(GetPiecewiseLinearApproximation(startPoint));
            }
        }

        [NotNull]
        protected abstract IEnumerable<Point> GetPiecewiseLinearApproximation(Point startPoint);

        public ISet<Point> Intersections { get; } = new HashSet<Point>();

        public abstract Point SplitByNextIntersection(Point startPoint, out SegmentBase segment1, out SegmentBase segment2);

        [NotNull]
        public abstract SegmentBase Reverse(Point startPoint);
    }
}
