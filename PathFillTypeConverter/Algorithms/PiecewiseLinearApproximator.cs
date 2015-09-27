using System;
using System.Collections.Generic;
using PathFillTypeConverter.Algorithms.CurveEquations;
using PathFillTypeConverter.Data;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms
{
    static class PiecewiseLinearApproximator
    {
        private const double MaxPieceLength = 0.05;

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
            return Approximate(new QuadraticBezierEquation(startPoint, segment).GetPoint, startPoint, segment.EndPoint);
        }

        public static List<Point> Approximate(Point startPoint, CubicBezierSegment segment)
        {
            return Approximate(new CubicBezierEquation(startPoint, segment).GetPoint, startPoint, segment.EndPoint);
        }

        public static List<Point> Approximate(Point startPoint, EllipticalArcSegment segment)
        {
            return Approximate(new EllipticalArcEquation(startPoint, segment).GetPoint, startPoint, segment.EndPoint);
        }
    }
}
