using System;
using System.Collections.Generic;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public sealed class EllipticalArcSegment : SegmentBase
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

        public override Point SplitByNextIntersection(Point startPoint, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SegmentSplitter.SplitByNextIntersection(startPoint, this, out segment1, out segment2);
        }

        public override SegmentBase Reverse(Point startPoint)
        {
            return new EllipticalArcSegment(Radii, XAxisRotation, IsLargeArc, !IsSweep, startPoint);
        }
    }
}
