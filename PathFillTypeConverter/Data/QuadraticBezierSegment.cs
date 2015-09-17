using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    internal sealed class QuadraticBezierSegment : SegmentBase
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

        public override SegmentBase Reverse(Point startPoint)
        {
            return new QuadraticBezierSegment(ControlPoint, startPoint);
        }
    }
}
