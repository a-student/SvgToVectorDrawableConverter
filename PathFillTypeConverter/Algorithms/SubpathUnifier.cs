using System;
using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Diagnostics;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Algorithms
{
    static class SubpathUnifier
    {
        private class SubpathCollection
        {
            private readonly List<Subpath> _subpaths = new List<Subpath>();
            private readonly List<Subpath> _reverseSubpaths = new List<Subpath>();

            public void Add(Subpath subpath)
            {
                _subpaths.Add(subpath);
                _reverseSubpaths.Add(SubpathDirectionReverser.ReverseDirection(subpath));
            }

            public Subpath Dequeue()
            {
                if (_subpaths.Count > 0)
                {
                    var subpath = _subpaths[0];
                    _subpaths.RemoveAt(0);
                    _reverseSubpaths.RemoveAt(0);
                    return subpath;
                }
                return null;
            }

            public IEnumerable<Subpath> GetAll(Point startPoint)
            {
                return _subpaths.Concat(_reverseSubpaths).Where(x => x.StartPoint == startPoint);
            }

            public void Remove(Subpath subpath)
            {
                var i = _subpaths.IndexOf(subpath);
                if (i < 0)
                {
                    i = _reverseSubpaths.IndexOf(subpath);
                }
                _subpaths.RemoveAt(i);
                _reverseSubpaths.RemoveAt(i);
            }
        }

        public static IEnumerable<Subpath> Unify(IEnumerable<Subpath> subpaths)
        {
            var collection = new SubpathCollection();
            foreach (var subpath in subpaths)
            {
                if (subpath.AreSegmentsClosed)
                {
                    if (HasFillArea(subpath))
                    {
                        yield return subpath;
                    }
                }
                else
                {
                    collection.Add(subpath);
                }
            }
            var subpath1 = collection.Dequeue();
            while (subpath1 != null)
            {
                var concatenations = collection.GetAll(subpath1.EndPoint).ToArray();
                Debugger.BreakWhen(concatenations.Length == 0);
                if (concatenations.Length > 0)
                {
                    var subpath2 = ChooseConcatenation(subpath1, concatenations);
                    collection.Remove(subpath2);
                    subpath1 = new Subpath(subpath1.StartPoint, subpath1.Segments.Concat(subpath2.Segments), false);
                    if (!subpath1.AreSegmentsClosed)
                    {
                        continue;
                    }
                }
                if (HasFillArea(subpath1))
                {
                    yield return subpath1;
                }
                subpath1 = collection.Dequeue();
            }
        }

        private static bool HasFillArea(Subpath subpath)
        {
            // area is larger than 1/20 of a pixel
            return Math.Abs(SubpathFillAreaCalculator.CalculateSigned(subpath)) >= 0.05;
        }

        private static Subpath ChooseConcatenation(Subpath subpath, IReadOnlyList<Subpath> concatenations)
        {
            if (concatenations.Count == 1)
            {
                return concatenations[0];
            }
            return concatenations
                .MinBy(
                    x =>
                    {
                        var points1 = subpath.PolygonApproximation.Points;
                        var points2 = x.PolygonApproximation.Points;
                        return Angle(points1.Last() - points1.LastButOne(), points2.Second() - points2.First());
                    });
        }

        private static double Angle(Point p1, Point p2)
        {
            var angle = Math.Atan2(p2.Y, p2.X) - Math.Atan2(p1.Y, p1.X);
            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }
            if (angle >= Math.PI)
            {
                angle -= Math.PI;
            }
            else
            {
                angle += Math.PI;
            }
            return angle;
        }
    }
}
