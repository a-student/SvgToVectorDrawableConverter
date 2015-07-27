using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.DataFormat.VectorDrawable;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    internal static class VectorOptimizer
    {
        public static void Optimize(Vector root)
        {
            RemoveInvisiblePaths(root.Children);
            RemoveEmptyGroups(root.Children);
            EliminateUselessGroupNesting(root.Children);
        }

        private static void RemoveInvisiblePaths(ElementCollection elements)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element is Path)
                {
                    if (!IsPathVisible((Path)element))
                    {
                        elements.RemoveAt(i--);
                    }
                }
                else if (element is ElementWithChildren)
                {
                    RemoveInvisiblePaths(((ElementWithChildren)element).Children);
                }
            }
        }

        private static bool IsPathVisible(Path path)
        {
            if (!string.IsNullOrEmpty(path.FillColor) && path.FillAlpha > 0)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(path.StrokeColor) && path.StrokeAlpha > 0 && path.StrokeWidth > 0)
            {
                return true;
            }
            return false;
        }

        private static void RemoveEmptyGroups(ElementCollection elements)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i] as ElementWithChildren;
                if (element == null)
                {
                    continue;
                }
                RemoveEmptyGroups(element.Children);
                if (element.Children.Count == 0)
                {
                    elements.RemoveAt(i--);
                }
            }
        }

        private static void EliminateUselessGroupNesting(ElementCollection elements)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                var group = elements[i] as Group;
                if (group == null)
                {
                    continue;
                }
                EliminateUselessGroupNesting(group.Children);
                if (IsUselessGroup(group))
                {
                    elements.RemoveAt(i);
                    var count = group.Children.Count;
                    while (group.Children.Count > 0)
                    {
                        elements.MoveTo(i, group.Children[group.Children.Count - 1]);
                    }
                    i += count - 1;
                }
            }
        }

        private static bool IsUselessGroup(Group group)
        {
            if (group.Rotation != 0)
            {
                return false;
            }
            if (group.PivotX != 0 || group.PivotY != 0)
            {
                return false;
            }
            if (group.ScaleX != 1 || group.ScaleY != 1)
            {
                return false;
            }
            if (group.TranslateX != 0 || group.TranslateY != 0)
            {
                return false;
            }
            return true;
        }
    }
}
