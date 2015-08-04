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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        IO io = new IO();
        private List<ImageInfo> imageInfoList;
        private int index = 0;

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_configuration = new Configuration();
            new_configuration.Accept = (s) =>
            {
                btnLast.IsEnabled = true;
                btnNext.IsEnabled = true;

                io = s.IO;
                imageInfoList = io.GetImageInfoList();
                ShowImg();
                this.Activate();
            };
            new_configuration.Show();
        }

        /// <summary>
        /// shortcut keys configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                btnLast_Click(btnLast, null);
                return;
            }
            if (e.Key == Key.D)
            {
                btnNext_Click(btnNext, null);
                return;
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (index == 0)
                index = imageInfoList.Count;
            index--;
            ShowImg();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (index == imageInfoList.Count - 1)
                index = -1;
            index++;
            ShowImg();
        }

        private void ShowImg()
        {
            var ii = imageInfoList[index];

            canvas.Children.Clear();
            var image = new Image() { Source = ii.Bitmap };
            canvas.Width = image.Width;
            canvas.Height = image.Height;
            canvas.Children.Add(image);

            if (ii.Rectangles.Count + ii.Ellipses.Count == 0)
            {
                ii.AddShapes(Brushes.Blue, TypeE.Detection);
                ii.AddShapes(Brushes.Red, TypeE.Gt);
            }
            foreach (var item in ii.Rectangles)
                canvas.Children.Add(item);
            foreach (var item in ii.Ellipses)
                canvas.Children.Add(item);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var info_form = new Info(imageInfoList[index]);
            info_form.Show();
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //List<Shape> list = new List<Shape>();
            //var p = e.GetPosition(sender as UIElement);
            //VisualTreeHelper.HitTest(this, null, f =>
            //{
            //    var s = f.VisualHit as Shape;
            //    if (s != null)
            //        list.Add(s);
            //    return HitTestResultBehavior.Continue;
            //}, new PointHitTestParameters(p));
            //AreaCalc(list);
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            var query = from rt in imageInfoList[index].Rectangles
                        from el in imageInfoList[index].Ellipses
                        select new { rt, el };

            var sb = new StringBuilder();
            var sw = new Stopwatch();
            var tolerance = 0;
            var ttype = ToleranceType.Relative;

            foreach (var item in query)
            {
                sw.Start();
                var rt = item.rt;
                var x1 = Canvas.GetLeft(rt);
                var y1 = Canvas.GetTop(rt);
                var geo1 = rt.RenderedGeometry;
                geo1.Transform = new TranslateTransform(x1, y1);

                var el = item.el;
                var x2 = Canvas.GetLeft(el);
                var y2 = Canvas.GetTop(el);
                var geo2 = el.RenderedGeometry;
                var group = new TransformGroup();
                group.Children.Add((RotateTransform)el.Tag);
                group.Children.Add(new TranslateTransform(x2, y2));
                geo2.Transform = group;

                var ci = Geometry.Combine(geo1, geo2, GeometryCombineMode.Intersect, null, tolerance, ttype);
                var cu = Geometry.Combine(geo1, geo2, GeometryCombineMode.Union, null, tolerance, ttype);
                sb.AppendLine($"交集面积：\n{ci.GetArea(tolerance, ttype)}\n并集面积：\n{cu.GetArea(tolerance, ttype)}");
                sw.Stop();
                sb.AppendLine(sw.ElapsedMilliseconds.ToString()).AppendLine();
            }

            tb_Result.Text = sb.ToString();
        }
    }
}
