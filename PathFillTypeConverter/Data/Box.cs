using System;
using System.Globalization;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public /*immutable*/ struct Box
    {
        public double MinX { get; }
        public double MaxX { get; }
        public double MinY { get; }
        public double MaxY { get; }

        public Box(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public Box(Point p1, Point p2)
        {
            MinX = Math.Min(p1.X, p2.X);
            MaxX = Math.Max(p1.X, p2.X);
            MinY = Math.Min(p1.Y, p2.Y);
            MaxY = Math.Max(p1.Y, p2.Y);
        }

        public override string ToString()
        {
            return $"{MinX.ToString(CultureInfo.InvariantCulture)} - {MaxX.ToString(CultureInfo.InvariantCulture)}, {MinY.ToString(CultureInfo.InvariantCulture)} - {MaxY.ToString(CultureInfo.InvariantCulture)}";
        }

        public bool Contains(Point point)
        {
            return
                point.X >= MinX &&
                point.X <= MaxX &&
                point.Y >= MinY &&
                point.Y <= MaxY;
        }

        public bool IntersectsWith(Box box)
        {
            return
                box.MinX <= MaxX &&
                box.MaxX >= MinX &&
                box.MinY <= MaxY &&
                box.MaxY >= MinY;
        }
    }
}
