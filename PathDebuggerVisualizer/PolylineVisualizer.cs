using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using PathFillTypeConverter.Data;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;
using DataPoint = PathFillTypeConverter.Data.Point;

namespace PathDebuggerVisualizer
{
    static class PolylineVisualizer
    {
        public static IEnumerable<Path> CreatePaths(Polyline data)
        {
            var start = data.Points[0].ToPoint();
            var isPolygon = data is Polygon;
            var geometry = new PathGeometry(new[] { new PathFigure(start, new[] { new PolyLineSegment(data.Points.Skip(1).Select(ToPoint), true) }, isPolygon) });
            if (isPolygon)
            {
                yield return Visualizer.CreateFillPath(geometry);
            }
            var group = new GeometryGroup();
            group.Children.Add(geometry);
            group.Children.Add(new EllipseGeometry(start, 1, 1));
            yield return new Path
            {
                Data = group,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
        }

        private static Point ToPoint(this DataPoint point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
