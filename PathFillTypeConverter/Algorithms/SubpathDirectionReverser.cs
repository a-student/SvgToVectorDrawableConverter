using System;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathDirectionReverser
    {
        public static Subpath ReverseDirection(Subpath subpath)
        {
            var segments = new SegmentBase[subpath.Segments.Count];
            var startPoint = subpath.StartPoint;
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = subpath.Segments[i];
                segments[i] = segment.Reverse(startPoint);
                startPoint = segment.EndPoint;
            }
            Array.Reverse(segments);
            return new Subpath(startPoint, segments, subpath.IsClosed);
        }
    }
}
