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
        BitmapImage img;

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_configuration = new Configuration();
            new_configuration.Accept = (s) =>
            {
                btnLast.IsEnabled = true;
                btnNext.IsEnabled = true;

                //get path
                io = s.IO;
                imageInfoList = io.GetImageInfoList();
                // image
                img = new BitmapImage(new Uri(imageInfoList[index].Path));
                image.Source = img;

                // show detection result
                io.showFR(image, imageInfoList[index], Brushes.Blue, TypeE.detection);
                // show ground truth
                io.showFR(image, imageInfoList[index], Brushes.Red, TypeE.gt);

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
            // process Last button
            if (e.Key == Key.A)
            {
                btnLast_Click(btnLast, null);
                return;
            }
            // process Net button
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
            index = index - 1;
            ShowImg();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (index == imageInfoList.Count - 1)
                index = -1;
            index = index + 1;
            ShowImg();
        }

        private void ShowImg()
        {
            img = new BitmapImage(new Uri(imageInfoList[index].Path));
            image.Source = null;
            image.Source = img;

            // show detection result
            io.showFR(image, imageInfoList[index], Brushes.Blue, TypeE.detection);
            // show ground truth
            io.showFR(image, imageInfoList[index], Brushes.Red, TypeE.gt);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var info_form = new Info(imageInfoList[index]);
            info_form.ImageInfo(img.Width, img.Height);
            info_form.Show();
        }
    }
}
