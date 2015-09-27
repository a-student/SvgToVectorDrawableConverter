using System.Collections.Generic;
using PathFillTypeConverter;
using PathFillTypeConverter.Data;
using Path = System.Windows.Shapes.Path;

namespace PathDebuggerVisualizer
{
    static class SubpathVisualizer
    {
        public static IEnumerable<Path> CreatePaths(Subpath data)
        {
            yield return Visualizer.CreateFillPath(PathFormatter.ToString(data));
            foreach (var x in Visualizer.CreatePathsForSubpath(data))
            {
                yield return x;
            }
        }
    }
}
