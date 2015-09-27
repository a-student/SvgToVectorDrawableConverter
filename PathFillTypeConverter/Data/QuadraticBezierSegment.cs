using System;
using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public sealed class QuadraticBezierSegment : SegmentBase
    {
        public Point ControlPoint { get; }
        public override Point EndPoint { get; }

        public QuadraticBezierSegment(Point controlPoint, Point endPoint)
        {
            ControlPoint = controlPoint;
            EndPoint = endPoint;
        }

        protected override IEnumerable<Point> GetPiecewiseLinearApproximation(Point startPoint)
        {
            return PiecewiseLinearApproximator.Approximate(startPoint, this);
        }

        public override Point SplitByNextIntersection(Point startPoint, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SegmentSplitter.SplitByNextIntersection(startPoint, this, out segment1, out segment2);
        }

        public override SegmentBase Reverse(Point startPoint)
        {
            return new QuadraticBezierSegment(ControlPoint, startPoint);
        }
    }
}
