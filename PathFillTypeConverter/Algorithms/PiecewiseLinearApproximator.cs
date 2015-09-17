using System;
using System.Collections.Generic;
using PathFillTypeConverter.Data;
using static System.Math;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms
{
    static class PiecewiseLinearApproximator
    {
        private const double MaxPieceLength = 0.02;

        private struct Piece
        {
            public double T0 { get; }
            public double T1 { get; }
            public Point P0 { get; }
            public Point P1 { get; }

            public Piece(double t0, double t1, Point p0, Point p1)
            {
                T0 = t0;
                T1 = t1;
                P0 = p0;
                P1 = p1;
            }

            public double SquareLength => Square(P0.X - P1.X) + Square(P0.Y - P1.Y);
        }

        private static List<Point> Approximate(Func<double, Point> curve, Point startPoint, Point endPoint)
        {
            var approximation = new List<Point> { startPoint };
            var stack = new Stack<Piece>();
            stack.Push(new Piece(0, 1, startPoint, endPoint));
            while (stack.Count > 0)
            {
                var piece = stack.Pop();
                if (piece.SquareLength <= Square(MaxPieceLength))
                {
                    approximation.Add(piece.P1);
                }
                else
                {
                    var t = (piece.T0 + piece.T1) / 2;
                    var pt = curve(t);
                    stack.Push(new Piece(t, piece.T1, pt, piece.P1));
                    stack.Push(new Piece(piece.T0, t, piece.P0, pt));
                }
            }
            return approximation;
        }

        public static List<Point> Approximate(Point startPoint, QuadraticBezierSegment segment)
        {
            return Approximate(t => Square(1 - t) * startPoint + 2 * (1 - t) * t * segment.ControlPoint + Square(t) * segment.EndPoint,
                startPoint, segment.EndPoint);
        }

        public static List<Point> Approximate(Point startPoint, CubicBezierSegment segment)
        {
            return Approximate(t => Cube(1 - t) * startPoint + 3 * Square(1 - t) * t * segment.ControlPoint1 + 3 * (1 - t) * Square(t) * segment.ControlPoint2 + Cube(t) * segment.EndPoint,
                startPoint, segment.EndPoint);
        }

        public static List<Point> Approximate(Point startPoint, EllipticalArcSegment segment)
        {
            // http://www.w3.org/TR/SVG/implnote.html#ArcImplementationNotes
            var phi = ToRadians(segment.XAxisRotation);
            var cos = Cos(phi);
            var sin = Sin(phi);
            var mid = (startPoint - segment.EndPoint) / 2;
            var x1 = cos * mid.X + sin * mid.Y;
            var y1 = -sin * mid.X + cos * mid.Y;
            var sqrt = Sqrt(Square(segment.Radii.X * segment.Radii.Y) / (Square(segment.Radii.X * y1) + Square(x1 * segment.Radii.Y)) - 1);
            var sign = segment.IsLargeArc != segment.IsSweep ? 1 : -1;
            var cx1 = sign * sqrt * segment.Radii.X * y1 / segment.Radii.Y;
            var cy1 = -sign * sqrt * segment.Radii.Y * x1 / segment.Radii.X;
            var avg = (startPoint + segment.EndPoint) / 2;
            var cx = cos * cx1 - sin * cy1 + avg.X;
            var cy = sin * cx1 + cos * cy1 + avg.Y;
            var theta1 = ToDegrees(Atan2((y1 - cy1) / segment.Radii.Y, (x1 - cx1) / segment.Radii.X));
            var theta2 = ToDegrees(Atan2((-y1 - cy1) / segment.Radii.Y, (-x1 - cx1) / segment.Radii.X));
            var deltaTheta = (theta2 - theta1) % 360;
            if (segment.IsSweep)
            {
                if (deltaTheta < 0)
                {
                    deltaTheta += 360;
                }
            }
            else
            {
                if (deltaTheta > 0)
                {
                    deltaTheta -= 360;
                }
            }
            theta1 = ToRadians(theta1);
            deltaTheta = ToRadians(deltaTheta);
            return Approximate(
                t =>
                {
                    var theta = theta1 + deltaTheta * t;
                    var x = segment.Radii.X * Cos(theta);
                    var y = segment.Radii.Y * Sin(theta);
                    return new Point(cos * x - sin * y + cx, sin * x + cos * y + cy);
                },
                startPoint, segment.EndPoint);
        }
    }
}
