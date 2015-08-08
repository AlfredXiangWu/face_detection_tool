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
using static FaceDetectionTool_WPF.Properties.Settings;

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
            Width = Default.WinWidth;
            Height = Default.WinHeight;
        }

        ImagePath imagePath = new ImagePath();
        private List<ImageInfo> imageInfoList;
        private int index = 0;
        private ImageSource img;

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_configuration = new Configuration();
            new_configuration.Accept = (s) =>
            {
                imagePath = s.ImagePath;
                imageInfoList = imagePath.GetImageInfoList();
                ShowImg();
                this.Activate();
            };
            new_configuration.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A: btnLast_Click(btnLast, null); break;
                case Key.D: btnNext_Click(btnNext, null); break;
                case Key.E:Eval_Click(null, null);break;
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            index--;
            if (index == -1)
                index = imageInfoList.Count - 1;
            ShowImg();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            index++;
            if (index == imageInfoList.Count)
                index = 0;
            ShowImg();
        }

        private void ShowImg()
        {
            if (imageInfoList.Count == 0)
                return;
            var ii = imageInfoList[index];
            canvas.Children.Clear();
            img = ii.Bitmap;
            var image = new Image() { Source = img };
            canvas.Width = image.Width;
            canvas.Height = image.Height;
            canvas.Children.Add(image);
            ii.ShapesPrepare();
            foreach (var item in ii.D_Shapes)
                canvas.Children.Add(item);
            foreach (var item in ii.G_Shapes)
                canvas.Children.Add(item);
            foreach (var m in ii.Matches)
            {
                m.D_Shape.Fill = Brushes.Blue;
                m.D_Shape.Opacity = 0.5;
                m.G_Shape.Fill = Brushes.Red;
                m.G_Shape.Opacity = 0.5;
            }
            bd_SizeChanged(null, null);
            spInfo.DataContext = ii;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Default.WinWidth = ActualWidth;
            Default.WinHeight = ActualHeight;
            Default.Save();
            Application.Current.Shutdown();
        }

        private void Eval_Click(object sender, RoutedEventArgs e)
        {
            var win = new EvaluationInfo(imageInfoList);
            win.Show();
        }

        private void sliderThr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (imageInfoList == null)
                return;
            var p = imageInfoList.EvalRecallAndPrecision(sliderThr.Value);
            tbRecall.Text = p.X.ToString("0.0000%");
            tbPrecision.Text = p.Y.ToString("0.0000%");
        }

        private void bd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (img == null)
                return;
            var ir = img.Width / img.Height;
            var sr = bd.ActualWidth / bd.ActualHeight;
            var r = ir > sr ? bd.ActualWidth / img.Width : bd.ActualHeight / img.Height;
            canvas.RenderTransform = new ScaleTransform(r, r);
        }
    }
}
