using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    static class PathConverter
    {
        [NotNull]
        public static Path EliminateIntersections([NotNull] Path path)
        {
            IntersectionsCalculator.Calculate(path);
            if (path.Subpaths.SelectMany(x => x.ClosedSegments).All(x => x.Intersections.Count == 0))
            {
                // nothing to do
                return path;
            }
            var subpaths = path.Subpaths.SelectMany(SubpathSplitter.SplitByIntersections).ToList();
            // path creation could be avoided here – do it only for debugging
            path = new Path(subpaths);
            RemoveDuplicatePairs(subpaths);
            path = new Path(subpaths);
            path = new Path(SubpathUnifier.Unify(subpaths));
            return path;
        }

        private static void RemoveDuplicatePairs(List<Subpath> subpaths)
        {
            for (var i = 0; i < subpaths.Count; i++)
            {
                var j = subpaths.FindIndex(i + 1, x => x.EqualsIgnoreDirection(subpaths[i]));
                if (j < 0)
                {
                    continue;
                }
                subpaths.RemoveAt(j);
                subpaths.RemoveAt(i--);
            }
        }

        [NotNull]
        public static Path FixDirections([NotNull] Path path)
        {
            var subpaths = new List<Subpath>(path.Subpaths);
            FixDirectionRecursively(subpaths, TreeBuilder.Build(path), null);
            return new Path(subpaths);
        }

        private static void FixDirectionRecursively(ICollection<Subpath> subpaths, IEnumerable<TreeNode<Subpath>> children, SubpathDirection? parentDirection)
        {
            foreach (var node in children)
            {
                var child = node.AssociatedObject;
                SubpathDirection? childDirection = SubpathDirectionCalculator.CalculateDirection(child);
                if (parentDirection != null)
                {
                    if (childDirection == parentDirection)
                    {
                        subpaths.Remove(child);
                        subpaths.Add(SubpathDirectionReverser.ReverseDirection(child));
                    }
                    childDirection = null;
                }
                FixDirectionRecursively(subpaths, node.Children, childDirection);
            }
        }
    }
}
