using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Data
{
    internal class Polyline
    {
        public IReadOnlyList<Point> Points { get; }
        public Box BoundingBox { get; }

        public Polyline([NotNull] IEnumerable<Point> points)
        {
            Points = points.ToReadOnlyList();
            BoundingBox = new Box(
                Points.Select(x => x.X).Min(),
                Points.Select(x => x.X).Max(),
                Points.Select(x => x.Y).Min(),
                Points.Select(x => x.Y).Max());
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var point in Points)
            {
                builder.Append($"{point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)} ");
            }
            return builder.ToString().TrimEnd();
        }
    }
}
