using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PathDebuggerVisualizer.Utils;
using PathFillTypeConverter;
using PathFillTypeConverter.Data;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;

namespace PathDebuggerVisualizer
{
    static class Visualizer
    {
        public static Path CreateFillPath(string data)
        {
            return CreateFillPath(PathGeometry.CreateFromGeometry(Geometry.Parse(data)));
        }

        public static Path CreateFillPath(PathGeometry geometry)
        {
            return new Path
            {
                Data = geometry,
                Fill = new SolidColorBrush(Colors.Gray),
                Opacity = 0.2
            };
        }

        public static IEnumerable<Path> CreatePathsForSubpath(Subpath subpath)
        {
            var source = PathFormatter.ToString(subpath);
            var hash = BitConverter.GetBytes(source.GetHashCode());
            var color = ColorHelper.FromHsb(360f * hash[0] / 255, 0.5f * hash[1] / 255 + 0.5f, 0.5f * hash[2] / 255);
            var group1 = new GeometryGroup();
            group1.Children.Add(Geometry.Parse(source));
            group1.Children.Add(new RectangleGeometry(new Rect(subpath.StartPoint.X - 2.5, subpath.StartPoint.Y - 2.5, 5, 5)));
            foreach (var segment in subpath.Segments)
            {
                group1.Children.Add(new EllipseGeometry(new Point(segment.EndPoint.X, segment.EndPoint.Y), 1.5, 1.5));
            }
            yield return new Path
            {
                Data = group1,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 3,
                Opacity = 0.5
            };

            var intersections = subpath.ClosedSegments.SelectMany(x => x.Intersections).ToArray();
            if (intersections.Length == 0)
            {
                yield break;
            }
            var group2 = new GeometryGroup();
            foreach (var point in intersections)
            {
                group2.Children.Add(new EllipseGeometry(new Point(point.X, point.Y), 1, 1));
            }
            yield return new Path
            {
                Data = group2,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2,
                Opacity = 0.3
            };
        }
    }
}
