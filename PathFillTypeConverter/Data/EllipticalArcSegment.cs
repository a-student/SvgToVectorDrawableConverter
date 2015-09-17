using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    internal sealed class EllipticalArcSegment : SegmentBase
    {
        public Point Radii { get; }
        public double XAxisRotation { get; }
        public bool IsLargeArc { get; }
        public bool IsSweep { get; }
        public override Point EndPoint { get; }

        public EllipticalArcSegment(Point radii, double xAxisRotation, bool isLargeArc, bool isSweep, Point endPoint)
        {
            Radii = radii;
            XAxisRotation = xAxisRotation;
            IsLargeArc = isLargeArc;
            IsSweep = isSweep;
            EndPoint = endPoint;
        }

        protected override IEnumerable<Point> GetPiecewiseLinearApproximation(Point startPoint)
        {
            return PiecewiseLinearApproximator.Approximate(startPoint, this);
        }

        public override SegmentBase Reverse(Point startPoint)
        {
            return new EllipticalArcSegment(Radii, XAxisRotation, IsLargeArc, !IsSweep, startPoint);
        }
    }
}
