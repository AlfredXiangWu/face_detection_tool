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
    /// Info.xaml 的交互逻辑
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
        }

        internal void GetPath(string path)
        {
            tb_Path.Text = path;
        }

        internal void ImageInfo(double width, double height)
        {
            l_Width.Content = width;
            l_Height.Content = height;
        }

        internal void DetectionInfo(int detection, int gt)
        {
            l_d.Content = detection;
            l_gt.Content = gt;
        }
    }
}
