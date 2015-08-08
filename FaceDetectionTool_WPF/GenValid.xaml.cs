using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FDialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;


namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// GenValid.xaml 的交互逻辑
    /// </summary>
    public partial class GenValid : Window
    {
        public GenValid(MainWindow win)
        {
            InitializeComponent();
            this.win = win;
        }

        private MainWindow win;

        private void btnDir_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Path";
            if (fbd.ShowDialog() == FDialogResult.OK)
                tbDir.Text = fbd.SelectedPath;
        }

        private async void btnDo_Click(object sender, RoutedEventArgs e)
        {
            var il = win.imageInfoList;
            pb.Maximum = il.Count;
            pb.Value = 0;
            var dir = tbDir.Text;
            var file = tbFile.Text;
            var indexList = il.Select(ii => ii.GetAllCouples(thr: 0.05).Select(m => m.FrIndex).ToArray()).ToArray();
            await Task.Run(() =>
            {
                double xtl, ytl, width, height;
                var list_path = Path.Combine(dir, file + ".txt");
                var sw = File.CreateText(list_path);

                for (int i = 0; i < il.Count; i++)
                {
                    var ii = il[i];
                    var bitmap = new Bitmap(ii.Path);
                    // positive sample
                    for (int j = 0; j < ii.GtList.Count; j++)
                    {
                        if (ii.GtList[j].Length == 5)
                        {
                            xtl = ii.GtList[j][3] - ii.GtList[j][0];
                            ytl = ii.GtList[j][4] - ii.GtList[j][0];
                            width = ii.GtList[j][0] * 2;
                            height = ii.GtList[j][0] * 2;
                        }
                        else
                        {
                            xtl = ii.GtList[j][0];
                            ytl = ii.GtList[j][1];
                            width = ii.GtList[j][2] - xtl + 1;
                            height = ii.GtList[j][3] - ytl + 1;
                        }
                        var rect = new Rectangle((int)xtl, (int)ytl, (int)width, (int)height);
                        Bitmap crop = crop_image(bitmap, rect);
                        string line = $"pos\\{ii.RelativeImgPath }_{j}.jpg";

                        var save_path_name = Path.Combine(dir, line);
                        var save_path = Path.GetDirectoryName(save_path_name);
                        if (!Directory.Exists(save_path))
                            Directory.CreateDirectory(save_path);

                        crop.Save(save_path_name, ImageFormat.Jpeg);
                        crop.Dispose();
                        sw.WriteLine(line.Replace('\\', '/') + " 1");
                    }

                    // negative samples
                    for (int j = 0; j < ii.FrList.Count; j++)
                    {
                        if (indexList[i].Contains(j))
                            continue;
                        xtl = ii.FrList[j][0];
                        ytl = ii.FrList[j][1];
                        width = ii.FrList[j][2] - xtl + 1;
                        height = ii.FrList[j][3] - ytl + 1;

                        var rect = new Rectangle((int)xtl, (int)ytl, (int)width, (int)height);
                        Bitmap crop = crop_image(bitmap, rect);
                        string line = $"neg\\{ii.RelativeImgPath }_{j}.jpg";
                        var save_path_name = Path.Combine(dir, line);
                        var save_path = Path.GetDirectoryName(save_path_name).ToString();
                        if (!Directory.Exists(save_path))
                            Directory.CreateDirectory(save_path);
                        crop.Save(save_path_name, ImageFormat.Jpeg);
                        crop.Dispose();
                        sw.WriteLine(line.Replace('\\', '/') + " 0");
                    }
                    bitmap.Dispose();
                    Dispatcher.Invoke(() => pb.Value = i + 1);
                }
                sw.Close();
            });
            MessageBox.Show("Done");
        }

        private Bitmap crop_image(Image image, Rectangle rect)
        {
            Bitmap crop = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(crop);
            g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
            g.Dispose();
            return crop;
        }
    }
}
