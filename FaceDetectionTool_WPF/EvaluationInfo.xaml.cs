using System;
using System.Collections.Generic;
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

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// EvaluationInfo.xaml 的交互逻辑
    /// </summary>
    public partial class EvaluationInfo : Window
    {
        public EvaluationInfo(List<ImageInfo> list)
        {
            InitializeComponent();
            this.list = list;
        }

        private List<ImageInfo> list;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var count = 1000;
            var points = list.EvalPoints(count);
            var y = canvas.Height;
            var g = new StreamGeometry();
            using (StreamGeometryContext context = g.Open())
            {
                context.BeginFigure(points.First(), false, false);
                foreach (var p in points.Skip(1))
                    context.LineTo(p, true, false);
            }
            var path = new Path() { Data = g, StrokeThickness = 1, Stroke = Brushes.Black };
            canvas.Children.Add(path);
        }
    }
}
