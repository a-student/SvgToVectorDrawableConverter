using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.DebuggerVisualizers;
using PathDebuggerVisualizer;
using PathFillTypeConverter.Data;
using ShapePath = System.Windows.Shapes.Path;

[assembly: DebuggerVisualizer(typeof(DebuggerSide), Target = typeof(Path), Description = "Path Visualizer")]
[assembly: DebuggerVisualizer(typeof(DebuggerSide), Target = typeof(Subpath), Description = "Subpath Visualizer")]
[assembly: DebuggerVisualizer(typeof(DebuggerSide), Target = typeof(Polyline), Description = "Polyline Visualizer")]

namespace PathDebuggerVisualizer
{
    public class DebuggerSide : DialogDebuggerVisualizer
    {
        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(DebuggerSide));
            visualizerHost.ShowVisualizer();
        }

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            IEnumerable<ShapePath> paths = null;
            var obj = objectProvider.GetObject();
            if (obj is Path)
            {
                paths = PathVisualizer.CreatePaths((Path)obj);
            }
            if (obj is Subpath)
            {
                paths = SubpathVisualizer.CreatePaths((Subpath)obj);
            }
            if (obj is Polyline)
            {
                paths = PolylineVisualizer.CreatePaths((Polyline)obj);
            }
            new VisualizerWindow { Paths = paths }.ShowDialog();
        }
    }
}
