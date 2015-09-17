using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Algorithms
{
    static class PathPreprocessor
    {
        [NotNull]
        public static Path Preprocess([NotNull] Path path)
        {
            return new Path(path.Subpaths.Select(Preprocess).Where(x => x != null));
        }

        [CanBeNull]
        private static Subpath Preprocess([NotNull] Subpath subpath)
        {
            var segments = new List<SegmentBase>();
            var startPoint = subpath.StartPoint;
            foreach (var segment in subpath.Segments)
            {
                var x = Simplify(startPoint, segment);
                if (!IsDegenerate(startPoint, x))
                {
                    segments.Add(x);
                }
                startPoint = segment.EndPoint;
            }
            if (segments.Count > 0)
            {
                return new Subpath(subpath.StartPoint, segments, subpath.IsClosed);
            }
            return null;
        }

        private static SegmentBase Simplify(Point startPoint, SegmentBase segment)
        {
            var endPoint = segment.EndPoint;
            var simplifyToLine = false;
            if (segment is EllipticalArcSegment)
            {
                var x = (EllipticalArcSegment)segment;
                simplifyToLine = x.Radii.X.IsZero() || x.Radii.Y.IsZero();
            }
            if (segment is QuadraticBezierSegment)
            {
                var x = (QuadraticBezierSegment)segment;
                simplifyToLine = x.ControlPoint == startPoint || x.ControlPoint == endPoint;
            }
            if (segment is CubicBezierSegment)
            {
                var x = (CubicBezierSegment)segment;
                simplifyToLine = (x.ControlPoint1 == startPoint || x.ControlPoint1 == endPoint) &&
                    (x.ControlPoint2 == startPoint || x.ControlPoint2 == endPoint);
            }
            return simplifyToLine ? new LineSegment(endPoint) : segment;
        }

        private static bool IsDegenerate(Point startPoint, SegmentBase segment)
        {
            if (segment is LineSegment || segment is EllipticalArcSegment)
            {
                return segment.EndPoint == startPoint;
            }
            return false;
        }
    }
}
