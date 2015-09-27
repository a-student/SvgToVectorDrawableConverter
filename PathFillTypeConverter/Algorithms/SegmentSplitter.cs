using System;
using System.Linq;
using PathFillTypeConverter.Algorithms.CurveEquations;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Diagnostics;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Algorithms
{
    static class SegmentSplitter
    {
        private static Point SplitByNextIntersection(Point startPoint, SegmentBase segment, out SegmentBase segment1, out SegmentBase segment2,
            CurveEquation equation,
            Func<double, Point, SegmentBase> factory1,
            Func<double, SegmentBase> factory2)
        {
            var intersection = segment.Intersections.Select(x => new { Point = x, T = equation.GetT(x) }).MinBy(x => x.T);
            var t = Math.Min(Math.Max(intersection.T, 0), 1);
            var point = equation.GetPoint(t);
            segment1 = point == startPoint ? null : factory1(t, intersection.Point);
            if (point == segment.EndPoint)
            {
                segment2 = null;
                Debugger.BreakWhen(segment.Intersections.Count > 1);
            }
            else
            {
                segment2 = factory2(t);
                segment2.Intersections.AddRange(segment.Intersections.Where(x => x != intersection.Point));
            }
            return intersection.Point;
        }

        public static Point SplitByNextIntersection(Point startPoint, LineSegment segment, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SplitByNextIntersection(startPoint, segment, out segment1, out segment2,
                new LineEquation(startPoint, segment),
                (t, p) => new LineSegment(p),
                t => new LineSegment(segment.EndPoint));
        }

        public static Point SplitByNextIntersection(Point startPoint, EllipticalArcSegment segment, out SegmentBase segment1, out SegmentBase segment2)
        {
            Func<Point, EllipticalArcSegment> create = p => new EllipticalArcSegment(segment.Radii, segment.XAxisRotation, segment.IsLargeArc, segment.IsSweep, p);
            return SplitByNextIntersection(startPoint, segment, out segment1, out segment2,
                new EllipticalArcEquation(startPoint, segment),
                (t, p) => create(p),
                t => create(segment.EndPoint));
        }

        public static Point SplitByNextIntersection(Point startPoint, QuadraticBezierSegment segment, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SplitByNextIntersection(startPoint, segment, out segment1, out segment2,
                new QuadraticBezierEquation(startPoint, segment),
                (t, p) => new QuadraticBezierSegment(Mid(startPoint, segment.ControlPoint, t), p),
                t => new QuadraticBezierSegment(Mid(segment.ControlPoint, segment.EndPoint, t), segment.EndPoint));
        }

        public static Point SplitByNextIntersection(Point startPoint, CubicBezierSegment segment, out SegmentBase segment1, out SegmentBase segment2)
        {
            return SplitByNextIntersection(startPoint, segment, out segment1, out segment2,
                new CubicBezierEquation(startPoint, segment),
                (t, p) =>
                {
                    var a = Mid(startPoint, segment.ControlPoint1, t);
                    var b = Mid(segment.ControlPoint1, segment.ControlPoint2, t);
                    return new CubicBezierSegment(a, Mid(a, b, t), p);
                },
                t =>
                {
                    var b = Mid(segment.ControlPoint1, segment.ControlPoint2, t);
                    var c = Mid(segment.ControlPoint2, segment.EndPoint, t);
                    return new CubicBezierSegment(Mid(b, c, t), c, segment.EndPoint);
                });
        }

        private static Point Mid(Point p1, Point p2, double t)
        {
            return (1 - t) * p1 + t * p2;
        }
    }
}
