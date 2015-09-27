using System.Collections.Generic;
using System.Linq;
using PathFillTypeConverter;
using DataPath = PathFillTypeConverter.Data.Path;
using Path = System.Windows.Shapes.Path;

namespace PathDebuggerVisualizer
{
    static class PathVisualizer
    {
        public static IEnumerable<Path> CreatePaths(DataPath data)
        {
            yield return Visualizer.CreateFillPath(PathFormatter.ToString(data));
            foreach (var x in data.Subpaths.SelectMany(Visualizer.CreatePathsForSubpath))
            {
                yield return x;
            }
        }
    }
}
