using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Exceptions;

namespace PathFillTypeConverter
{
    static class PathFormatter
    {
        [NotNull]
        public static string ToString([NotNull] Path path)
        {
            var builder = new StringBuilder();
            foreach (var subpath in path.Subpaths)
            {
                builder.Append('M');
                builder.Append(PointToString(subpath.StartPoint));
                builder.Append(' ');
                foreach (var segment in subpath.Segments)
                {
                    if (segment is LineSegment)
                    {
                        builder.Append('L');
                        builder.Append(PointToString(segment.EndPoint));
                        builder.Append(' ');
                    }
                    else if (segment is CubicBezierSegment)
                    {
                        var cubicBezierSegment = (CubicBezierSegment)segment;
                        builder.Append('C');
                        builder.Append(PointToString(cubicBezierSegment.ControlPoint1));
                        builder.Append(' ');
                        builder.Append(PointToString(cubicBezierSegment.ControlPoint2));
                        builder.Append(' ');
                        builder.Append(PointToString(cubicBezierSegment.EndPoint));
                        builder.Append(' ');
                    }
                    else if (segment is QuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = (QuadraticBezierSegment)segment;
                        builder.Append('Q');
                        builder.Append(PointToString(quadraticBezierSegment.ControlPoint));
                        builder.Append(' ');
                        builder.Append(PointToString(quadraticBezierSegment.EndPoint));
                        builder.Append(' ');
                    }
                    else if (segment is EllipticalArcSegment)
                    {
                        var ellipticalArcSegment = (EllipticalArcSegment)segment;
                        builder.Append('A');
                        builder.Append(PointToString(ellipticalArcSegment.Radii));
                        builder.Append(' ');
                        builder.Append(DoubleToString(ellipticalArcSegment.XAxisRotation));
                        builder.Append(' ');
                        builder.Append(ellipticalArcSegment.IsLargeArc ? '1' : '0');
                        builder.Append(' ');
                        builder.Append(ellipticalArcSegment.IsSweep ? '1' : '0');
                        builder.Append(' ');
                        builder.Append(PointToString(ellipticalArcSegment.EndPoint));
                        builder.Append(' ');
                    }
                    else
                    {
                        throw new PathDataConverterException("Unknown segment type.");
                    }
                }
                if (subpath.IsClosed)
                {
                    builder.Append('Z');
                    builder.Append(' ');
                }
            }
            return builder.ToString();
        }

        private static string PointToString(Point point)
        {
            return DoubleToString(point.X) + "," + DoubleToString(point.Y);
        }

        private static string DoubleToString(double d)
        {
            return Convert.ToSingle(d).ToString(CultureInfo.InvariantCulture);
        }
    }
}
