using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class IntersectionsCalculator
    {
        public static void Calculate(Path path)
        {
            foreach (var subpath in path.Subpaths)
            {
                subpath.BuildPolylineApproximations();
                foreach (var segment in subpath.ClosedSegments)
                {
                    segment.Intersections.Clear();
                }
            }

            for (var i = 0; i < path.Subpaths.Count; i++)
            {
                var subpath1 = path.Subpaths[i];
                var segments1 = subpath1.ClosedSegments.ToArray();
                for (var j = 0; j < segments1.Length; j++)
                {
                    var segment1 = segments1[j];
                    CalculateSelfIntersections(segment1);
                    for (var k = j + 1; k < segments1.Length; k++)
                    {
                        CalculateIntersections(segment1, segments1[k], k == j + 1, j == 0 && k == segments1.Length - 1);
                    }
                    foreach (var subpath2 in path.Subpaths.Skip(i + 1))
                    {
                        if (!segment1.PolylineApproximation.BoundingBox.IntersectsWith(subpath2.PolygonApproximation.BoundingBox))
                        {
                            continue;
                        }
                        foreach (var segment2 in subpath2.ClosedSegments)
                        {
                            CalculateIntersections(segment1, segment2, false, false);
                        }
                    }
                }
            }
        }

        private static void CalculateSelfIntersections(SegmentBase segment)
        {
            if (!(segment is CubicBezierSegment))
            {
                // only cubic bezier segment can self-intersect
                return;
            }
            var points = segment.PolylineApproximation.Points;
            for (var i = 1; i < points.Count - 2; i++)
            {
                var p11 = points[i - 1];
                var p12 = points[i];
                var box1 = new Box(p11, p12);
                for (var j = i + 2; j < points.Count; j++)
                {
                    var p21 = points[j - 1];
                    var p22 = points[j];
                    var box2 = new Box(p21, p22);
                    if (!box1.IntersectsWith(box2))
                    {
                        continue;
                    }
                    var intersection = CalculateIntersection(p11, p12, p21, p22);
                    if (intersection.HasValue)
                    {
                        segment.Intersections.Add(intersection.Value);
                    }
                }
            }
        }

        private static void CalculateIntersections(SegmentBase segment1, SegmentBase segment2, bool skipInner, bool skipOuter)
        {
            var boundingBox1 = segment1.PolylineApproximation.BoundingBox;
            var boundingBox2 = segment2.PolylineApproximation.BoundingBox;
            if (!boundingBox1.IntersectsWith(boundingBox2))
            {
                return;
            }
            var points1 = segment1.PolylineApproximation.Points;
            var points2 = segment2.PolylineApproximation.Points;
            for (var i = 1; i < points1.Count; i++)
            {
                var p11 = points1[i - 1];
                var p12 = points1[i];
                var box1 = new Box(p11, p12);
                if (!box1.IntersectsWith(boundingBox2))
                {
                    continue;
                }
                for (var j = 1; j < points2.Count; j++)
                {
                    var p21 = points2[j - 1];
                    var p22 = points2[j];
                    var box2 = new Box(p21, p22);
                    if (!box1.IntersectsWith(box2))
                    {
                        continue;
                    }
                    if ((skipInner && i == points1.Count - 1 && j == 1) ||
                        (skipOuter && i == 1 && j == points2.Count - 1))
                    {
                        continue;
                    }
                    var intersection = CalculateIntersection(p11, p12, p21, p22);
                    if (intersection.HasValue)
                    {
                        segment1.Intersections.Add(intersection.Value);
                        segment2.Intersections.Add(intersection.Value);
                    }
                }
            }
        }

        private static Point? CalculateIntersection(Point p11, Point p12, Point p21, Point p22)
        {
            var s1 = p12 - p11;
            var s2 = p22 - p21;
            var a = p11 - p21;
            var b = s1.X * s2.Y - s2.X * s1.Y;
            var s = (s1.X * a.Y - s1.Y * a.X) / b;
            var t = (s2.X * a.Y - s2.Y * a.X) / b;
            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return new Point(p11.X + t * s1.X, p11.Y + t * s1.Y);
            }
            return null;
        }
    }
}
