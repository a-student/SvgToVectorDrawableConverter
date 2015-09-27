using System;
using System.Globalization;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public /*immutable*/ struct Point : IEquatable<Point>
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X.ToString(CultureInfo.InvariantCulture)}, {Y.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator -(Point p)
        {
            return new Point(-p.X, -p.Y);
        }

        public static Point operator *(double d, Point p)
        {
            return p * d;
        }

        public static Point operator *(Point p, double d)
        {
            return new Point(p.X * d, p.Y * d);
        }

        public static Point operator /(Point p, double d)
        {
            return new Point(p.X / d, p.Y / d);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }
            return Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return X.IsEqualTo(other.X) && Y.IsEqualTo(other.Y);
        }

        public override int GetHashCode()
        {
            return unchecked(X.RoundToPrecision().GetHashCode() * 397) ^ Y.RoundToPrecision().GetHashCode();
        }
    }
}
