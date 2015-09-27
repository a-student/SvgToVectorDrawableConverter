using System;
using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public sealed class LineSegment : SegmentBase
    {
        public override Point EndPoint { get; }

        public LineSegment(Point point)
        {
            EndPoint = point;
        }

        protected override IEnumerable<Point> GetPiecewiseLinearApproximation(Point startPoint)
        {
            yield return startPoint;
            yield return EndPoint;
        }

        public override Point SplitByNextIntersection(Point startPoint, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SegmentSplitter.SplitByNextIntersection(startPoint, this, out segment1, out segment2);
        }

        public override SegmentBase Reverse(Point startPoint)
        {
            return new LineSegment(startPoint);
        }
    }
}
