using System;
using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public sealed class CubicBezierSegment : SegmentBase
    {
        public Point ControlPoint1 { get; }
        public Point ControlPoint2 { get; }
        public override Point EndPoint { get; }

        public CubicBezierSegment(Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;
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
            return new CubicBezierSegment(ControlPoint2, ControlPoint1, startPoint);
        }
    }
}
