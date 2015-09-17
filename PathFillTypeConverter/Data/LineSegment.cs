using System.Collections.Generic;

namespace PathFillTypeConverter.Data
{
    internal sealed class LineSegment : SegmentBase
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

        public override SegmentBase Reverse(Point startPoint)
        {
            return new LineSegment(startPoint);
        }
    }
}
