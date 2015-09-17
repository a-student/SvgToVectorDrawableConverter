using System;
using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathSplitter
    {
        public static IEnumerable<Subpath> SplitByIntersections(Subpath subpath)
        {
            if (subpath.ClosedSegments.Any(x => x.Intersections.Count > 0))
            {
                throw new NotImplementedException();
            }
            yield return subpath;
        }
    }
}
