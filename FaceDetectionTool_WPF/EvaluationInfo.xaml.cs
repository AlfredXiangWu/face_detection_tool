using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Math;

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// EvaluationInfo.xaml 的交互逻辑
    /// </summary>
    public partial class EvaluationInfo : Window
    {
        public EvaluationInfo(MainWindow win)
        {
            InitializeComponent();
            this.win = win;
        }

        private MainWindow win;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var points = win.ImageInfoList.EvalPoints();
            lbCount.Content = "points: " + points.Length.ToString();

            var g = new StreamGeometry();
            using (StreamGeometryContext context = g.Open())
            {
                context.BeginFigure(points.First(), false, false);
                foreach (var p in points.Skip(1))
                    context.LineTo(p, true, false);
            }
            g.Transform = new ScaleTransform(canvas.Width, canvas.Height);
            var path = new Path() { Data = g, StrokeThickness = 1, Stroke = Brushes.Purple };
            canvas.Children.Add(path);

            sw.Stop();
            lbTime.Content = "time: " + sw.ElapsedMilliseconds.ToString() + "ms";
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            var x = canvas.ActualWidth;
            var y = canvas.ActualHeight;
            var dx = x / 10;
            var dy = y / 10;
            for (int i = 1; i <= 10; i++)
            {
                var vl = new Line() { X1 = 0, Y1 = i * dy, X2 = x, Y2 = i * dy };
                var hl = new Line() { X1 = i * dx, Y1 = 0, X2 = i * dx, Y2 = y };
                vl.Stroke = hl.Stroke = Brushes.LightBlue;
                vl.StrokeThickness = hl.StrokeThickness = 1;
                canvas.Children.Add(vl);
                canvas.Children.Add(hl);
            }
            canvas.Children.Add(new Line() { X1 = 0, Y1 = 0, X2 = 0, Y2 = y, Stroke = Brushes.Blue, StrokeThickness = 1 });
            canvas.Children.Add(new Line() { X1 = 0, Y1 = 0, X2 = x, Y2 = 0, Stroke = Brushes.Blue, StrokeThickness = 1 });
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            canvas_Loaded(null, null);
        }
    }
}
