using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Data
{
    internal class Subpath
    {
        public Point StartPoint { get; }
        [NotNull]
        public IReadOnlyList<SegmentBase> Segments { get; }
        public bool IsClosed { get; }

        private readonly LineSegment _closingSegment;

        public Subpath(Point startPoint, [NotNull] IEnumerable<SegmentBase> segments, bool isClosed)
        {
            StartPoint = startPoint;
            Segments = segments.ToReadOnlyList();
            IsClosed = isClosed;

            if (Segments.Any(x => x == null))
            {
                throw new ArgumentException();
            }

            if (Segments.Count > 0 && Segments.Last().EndPoint != StartPoint)
            {
                _closingSegment = new LineSegment(StartPoint);
            }
        }

        private bool _builtPolylineApproximations;

        public void BuildPolylineApproximations()
        {
            if (_builtPolylineApproximations)
            {
                return;
            }
            _builtPolylineApproximations = true;

            var currentPoint = StartPoint;
            foreach (var segment in Segments)
            {
                segment.BuildPolylineApproximation(currentPoint);
                currentPoint = segment.EndPoint;
            }
            _closingSegment?.BuildPolylineApproximation(currentPoint);
        }

        private Polygon _polygonApproximation;

        public Polygon PolygonApproximation
        {
            get
            {
                if (_polygonApproximation == null)
                {
                    BuildPolylineApproximations();
                    _polygonApproximation = new Polygon(new[] { StartPoint }.Concat(Segments.SelectMany(x => x.PolylineApproximation.Points.Skip(1))));
                }
                return _polygonApproximation;
            }
        }

        public bool HasClosingSegment => _closingSegment != null;
        public IEnumerable<SegmentBase> ClosedSegments => HasClosingSegment ? Segments.Concat(new[] { _closingSegment }) : Segments;
    }
}
