using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Data
{
    [Serializable]
    public class Polyline
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
    }
}
