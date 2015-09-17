using System.Collections.Generic;
using JetBrains.Annotations;

namespace PathFillTypeConverter.Data
{
    internal abstract class SegmentBase
    {
        public abstract Point EndPoint { get; }

        public Polyline PolylineApproximation { get; private set; }

        public void BuildPolylineApproximation(Point startPoint)
        {
            if (PolylineApproximation == null)
            {
                PolylineApproximation = new Polyline(GetPiecewiseLinearApproximation(startPoint));
            }
        }

        [NotNull]
        protected abstract IEnumerable<Point> GetPiecewiseLinearApproximation(Point startPoint);

        public IList<Point> Intersections { get; } = new List<Point>();

        [NotNull]
        public abstract SegmentBase Reverse(Point startPoint);
    }
}
