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
using System.ComponentModel;
using static FaceDetectionTool_WPF.Properties.Settings;

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            Width = Default.WinWidth;
            Height = Default.WinHeight;
            spIndex.DataContext = this;
        }

        ImagePath imagePath = new ImagePath();

        private List<ImageInfo> imageInfoList;
        public List<ImageInfo> ImageInfoList
        {
            get { return imageInfoList; }
            set { imageInfoList = value; OnPropertyChanged(nameof(ImageInfoList)); }
        }
        private int index = 0;
        public int Index
        {
            get { return index + 1; }
            set { index = value - 1; OnPropertyChanged(nameof(Index)); }
        }

        private ImageSource img;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_configuration = new Configuration();
            new_configuration.Accept = (s) =>
            {
                imagePath = s.ImagePath;
                ImageInfoList = imagePath.GetImageInfoList();
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
                case Key.E: Eval_Click(null, null); break;
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            Index--;
            ShowImg();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Index++;
            ShowImg();
        }

        private void ShowImg()
        {
            if (ImageInfoList.Count == 0)
                return;
            if (Index > ImageInfoList.Count)
                Index = ImageInfoList.Count;
            else if (Index < 1)
                Index = 1;
            var ii = ImageInfoList[index];
            canvas.Children.Clear();
            img = ii.Bitmap;
            var image = new Image() { Source = img };
            canvas.Width = image.Width;
            canvas.Height = image.Height;
            canvas.Children.Add(image);

            ii.GeometriesPrepare();
            foreach (var item in ii.GetShapes(Brushes.Blue, Brushes.Red))
                canvas.Children.Add(item);

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
            var win = new EvaluationInfo(this);
            win.Show();
        }

        private void sliderThr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageInfoList == null)
                return;
            var p = ImageInfoList.EvalRecallAndPrecision(sliderThr.Value);
            tbRecall.Text = p.X.ToString("0.0000%");
            tbPrecision.Text = p.Y.ToString("0.0000%");
        }

        private void bd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (img == null)
                return;
            var bw = bd.ActualWidth - 2;
            var bh = bd.ActualHeight - 2;
            var ir = img.Width / img.Height;
            var sr = bw / bh;
            var r = ir > sr ? bw / img.Width : bh / img.Height;
            canvas.RenderTransform = new ScaleTransform(r, r);
        }

        private void GenValid_Click(object sender, RoutedEventArgs e)
        {
            var win = new GenValid(this);
            win.Show();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                ShowImg();
            }
        }
    }
}
