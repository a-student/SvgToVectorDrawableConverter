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
            return new Path(path.Subpaths.SelectMany(SubpathSplitter.SplitByIntersections));
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
