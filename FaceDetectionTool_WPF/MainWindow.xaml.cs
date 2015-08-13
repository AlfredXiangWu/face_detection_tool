using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            DataContext = this;
        }

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
            set
            {
                if (index != value - 1)
                { index = value - 1; IndexChanged(); }
            }
        }

        private void IndexChanged()
        {
            OnPropertyChanged(nameof(Index));
            ShowImg();
        }

        private ImageInfo imageInfo;
        public ImageInfo CurrentImageInfo
        {
            get { return imageInfo; }
            set { imageInfo = value; OnPropertyChanged(nameof(CurrentImageInfo)); }
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
                ImageInfoList = s.ImagePath.GetImageInfoList();
                ShowImg();
                s.Close();
            };
            new_configuration.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A: Index--; break;
                case Key.D: Index++; break;
                case Key.E: Eval_Click(null, null); break;
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e) => Index--;

        private void btnNext_Click(object sender, RoutedEventArgs e) => Index++;

        private void ShowImg()
        {
            if (ImageInfoList.Count == 0)
                return;
            if (Index > ImageInfoList.Count)
                Index = ImageInfoList.Count;
            else if (Index < 1)
                Index = 1;
            CurrentImageInfo = ImageInfoList[index];
            canvas.Children.Clear();
            img = CurrentImageInfo.Bitmap;
            var image = new Image() { Source = img };
            canvas.Width = image.Width;
            canvas.Height = image.Height;
            canvas.Children.Add(image);

            CurrentImageInfo.GeometriesPrepare();
            foreach (var item in CurrentImageInfo.GetShapes(Brushes.Blue, Brushes.Red))
                canvas.Children.Add(item);

            bd_SizeChanged(null, null);
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
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
