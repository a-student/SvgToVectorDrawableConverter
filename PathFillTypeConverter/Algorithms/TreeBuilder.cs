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
            // Due to the assumption just check one point
            // do not pick a point on the child border
            return PointInPolygonTest.Contains(parent.PolygonApproximation, child.PolygonApproximation.InsidePoint);
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
                (FindNode(roots, subpath)?.Children ?? roots).Add(new TreeNode<Subpath> { AssociatedObject = subpath });
            }
            return roots;
        }

        private static TreeNode<Subpath> FindNode(IEnumerable<TreeNode<Subpath>> nodes, Subpath subpath)
        {
            var node = nodes.FirstOrDefault(x => Contains(x.AssociatedObject, subpath));
            if (node == null)
            {
                return null;
            }
            return FindNode(node.Children, subpath) ?? node;
        }
    }
}
