using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathDebuggerVisualizer
{
    public partial class VisualizerWindow
    {
        public VisualizerWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<Path> Paths
        {
            get { return PathGrid?.Children.Cast<Path>() ?? Enumerable.Empty<Path>(); }
            set
            {
                var bounds = Rect.Empty;
                foreach (var path in value)
                {
                    PathGrid.Children.Add(path);
                    bounds.Union(path.Data.Bounds);
                }
                PathGrid.Margin = new Thickness(-Math.Floor(bounds.Left), -Math.Floor(bounds.Top), 0, 0);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var data in Paths.Select(x => x.Data).OfType<PathGeometry>())
            {
                data.FillRule = (FillRule)ComboBox.SelectedIndex;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(PathGrid);
            TextBlock.Text = $"{Math.Round(position.X)} {Math.Round(position.Y)}";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Paths.All(x => x.Visibility == Visibility.Visible))
            {
                foreach (var path in Paths.Skip(1))
                {
                    path.Visibility = Visibility.Hidden;
                }
                return;
            }
            if (Paths.Last().Visibility == Visibility.Visible)
            {
                foreach (var path in Paths)
                {
                    path.Visibility = Visibility.Visible;
                }
                return;
            }
            var current = Paths.Single(x => x.Visibility == Visibility.Visible);
            var next = Paths.SkipWhile(x => x != current).ElementAt(1);
            current.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
