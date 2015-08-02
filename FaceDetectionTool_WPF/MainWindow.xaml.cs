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
        string[] img_path;
        string[] detection_fr_path;
        string[] gt_fr_path;
        int count = 0;
        int num_image = 0;
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
                img_path = io.GetPath(TypeE.image);
                detection_fr_path = io.GetPath(TypeE.detection);
                gt_fr_path = io.GetPath(TypeE.gt);
                num_image = io.GetNum();

                // image
                img = new BitmapImage(new Uri(img_path[count]));
                image.Source = img;

                // show detection result
                io.showFR(image, detection_fr_path[count], Brushes.Blue, TypeE.detection);
                // show ground truth
                io.showFR(image, gt_fr_path[count], Brushes.Red, TypeE.gt);

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
            if (count == 0)
                count = img_path.Length;
            count = count - 1;
            ShowImg();
        }      

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (count == img_path.Length - 1)
                count = -1;
            count = count + 1;
            ShowImg();
        }

        private void ShowImg()
        {
            img = new BitmapImage(new Uri(img_path[count]));
            image.Source = null;
            image.Source = img;

            // show detection result
            io.showFR(image, detection_fr_path[count], Brushes.Blue, TypeE.detection);
            // show ground truth
            io.showFR(image, gt_fr_path[count], Brushes.Red, TypeE.gt);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
