using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Exceptions;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathSplitter
    {
        public static IEnumerable<Subpath> SplitByIntersections(Subpath subpath)
        {
            var startPoint = subpath.StartPoint;
            var segments = subpath.ClosedSegments.ToList();

            while (true)
            {
                var i = segments.FindIndex(x => x.Intersections.Count > 0);
                if (i < 0)
                {
                    if (segments.Count > 0)
                    {
                        yield return new Subpath(startPoint, segments, false);
                    }
                    yield break;
                }
                SegmentBase segment1, segment2;
                var intersection = segments[i].SplitByNextIntersection(i > 0 ? segments[i - 1].EndPoint : startPoint, out segment1, out segment2);
                var segments1 = segments.GetRange(0, i);
                if (segment1 != null)
                {
                    if (segment1.Intersections.Count > 0)
                    {
                        throw new PathDataConverterException();
                    }
                    segments1.Add(segment1);
                }
                if (segments1.Count > 0)
                {
                    yield return new Subpath(startPoint, segments1, false);
                }
                i++;
                var segments2 = segments.GetRange(i, segments.Count - i);
                if (segment2 != null)
                {
                    segments2.Insert(0, segment2);
                }

                startPoint = intersection;
                segments = segments2;
            }
        }
    }
}
