using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathComparer
    {
        public static bool EqualsIgnoreDirection(this Subpath subpath1, Subpath subpath2)
        {
            if (subpath1.Segments.Count != subpath2.Segments.Count ||
                subpath1.IsClosed != subpath2.IsClosed)
            {
                return false;
            }
            if (subpath1.StartPoint == subpath2.StartPoint &&
                subpath1.EndPoint == subpath2.EndPoint &&
                Equals(subpath1, subpath2))
            {
                return true;
            }
            if (subpath1.StartPoint == subpath2.EndPoint &&
                subpath1.EndPoint == subpath2.StartPoint &&
                Equals(subpath1, SubpathDirectionReverser.ReverseDirection(subpath2)))
            {
                return true;
            }
            return false;
        }

        private static bool Equals(Subpath subpath1, Subpath subpath2)
        {
            return PathFormatter.ToString(subpath1) == PathFormatter.ToString(subpath2);
        }
    }
}
