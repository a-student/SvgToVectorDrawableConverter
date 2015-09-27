using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    class TreeNode<T>
    {
        public T AssociatedObject { get; set; }
        public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
    }

    static class TreeBuilder
    {
        /// <summary>
        /// Returns true if the child is entirely contained within the parent, inclusive the edges.
        /// Assumes that there is no intersection between the two subpaths, touchings allowed.
        /// </summary>
        private static bool Contains(Subpath parent, Subpath child)
        {
            if (!PointInPolygonTest.Contains(parent.PolygonApproximation, child.PolygonApproximation.InsidePoint))
            {
                return false;
            }
            return child.PolygonApproximation.Points.Any(x => PointInPolygonTest.Contains(parent.PolygonApproximation, x));
        }

        /// <summary>
        /// Tree building is based on the <see cref="Contains"/> method with its assumptions.
        /// </summary>
        /// <returns>List of tree roots.</returns>
        public static List<TreeNode<Subpath>> Build(Path path)
        {
            var roots = new List<TreeNode<Subpath>>();
            foreach (var subpath in path.Subpaths)
            {
                var node = new TreeNode<Subpath> { AssociatedObject = subpath };
                var nodes = FindParent(roots, subpath)?.Children ?? roots;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var child = nodes[i];
                    if (Contains(subpath, child.AssociatedObject))
                    {
                        nodes.RemoveAt(i--);
                        node.Children.Add(child);
                    }
                }
                nodes.Add(node);
            }
            return roots;
        }

        private static TreeNode<Subpath> FindParent(IEnumerable<TreeNode<Subpath>> nodes, Subpath subpath)
        {
            var parent = nodes.FirstOrDefault(x => Contains(x.AssociatedObject, subpath));
            if (parent == null)
            {
                return null;
            }
            return FindParent(parent.Children, subpath) ?? parent;
        }
    }
}
