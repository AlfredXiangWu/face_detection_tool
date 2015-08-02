using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using static FaceDetectionTool_WPF.Properties.Settings;
using System.Windows;
using System.Collections.Generic;

namespace FaceDetectionTool_WPF
{
    public class IO
    {
        /// <summary>
        /// Image Path
        /// </summary>
        public string FrmImagePath { get; set; } = Default.ImagePath;

        /// <summary>
        /// List File 
        /// </summary>
        public string FrmList { get; set; } = Default.List;

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string FrmDetectionFrPath { get; set; } = Default.DetectionFrPath;

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        public string FrmGtFrPath { get; set; } = Default.GtFrPath;

        /// <summary>
        /// Get number of image
        /// </summary>
        /// <returns></returns>

        public List<ImageInfo> GetImageInfoList()
        {
            var txt = File.ReadAllLines(FrmList, Encoding.Default);
            var count = int.Parse(txt[0]);
            var list = new List<ImageInfo>(count);
            foreach (var line in txt.Skip(1).Take(count))
            {
                var imageInfo = new ImageInfo();
                imageInfo.Path = Path.Combine(FrmImagePath, line);
                var name = line.Split('.')[0];
                imageInfo.PathFr = Path.Combine(FrmDetectionFrPath, name + ".jpg.fr");
                imageInfo.PathGt = Path.Combine(FrmGtFrPath, name + ".fr");

                imageInfo.FrList = GetInfoList(imageInfo.PathFr);
                imageInfo.GtList = GetInfoList(imageInfo.PathGt);

                list.Add(imageInfo);
            }
            return list;
        }

        private List<double[]> GetInfoList(string path)
        {
            var txt = File.ReadAllLines(path);
            var count = int.Parse(txt[0]);
            var lines = txt.Skip(1).Take(count);
            return lines.Select(l =>
                l.Replace(' ', '\t').Split('\t')
                .Select(s => double.Parse(s)).ToArray()).ToList();
        }

        public double[] showFR(Image image, ImageInfo imageInfo, Brush color, TypeE type)
        {
            var img = (BitmapSource)image.Source;
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(image.Source, new Rect(0, 0, img.Width, img.Height));
            var pen = new Pen(color, 4.0f);
            try
            {
                double xtl, ytl, width, height;
                double[] prob = null;

                if (type == TypeE.detection)
                {
                    prob = new double[imageInfo.FrList.Count];
                    for (int i = 0; i < prob.Length; i++)
                    {
                        var s = imageInfo.FrList[i];
                        xtl = s[0];
                        ytl = s[1];
                        width = s[2] - xtl + 1;
                        height = s[3] - ytl + 1;
                        prob[i] = s[4];

                        drawingContext.DrawRectangle(null, pen, new Rect(xtl, ytl, width, height));
                    }
                }
                else if (type == TypeE.gt)
                {
                    prob = new double[imageInfo.GtList.Count];
                    for (int i = 0; i < prob.Length; i++)
                    {
                        var s = imageInfo.GtList[i];
                        if (s.Length == 5)
                        {
                            width = s[0];
                            height = s[1];
                            var angle = s[2] / Math.PI * 180;
                            xtl = s[3];
                            ytl = s[4];
                            prob[i] = 1;

                            var rt = new RotateTransform(angle, xtl, ytl);
                            drawingContext.PushTransform(rt);
                            drawingContext.DrawEllipse(null, pen, new Point(xtl, ytl), width, height);
                            drawingContext.Pop();
                        }
                        else
                        {
                            xtl = s[0];
                            ytl = s[1];
                            width = s[2] - xtl + 1;
                            height = s[3] - ytl + 1;
                            prob[i] = 1;

                            drawingContext.DrawRectangle(null, pen, new Rect(xtl, ytl, width, height));
                        }
                    }
                }
                drawingContext.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap(img.PixelWidth, img.PixelHeight, img.DpiX, img.DpiY, PixelFormats.Pbgra32);
                bmp.Render(drawingVisual);
                image.Source = bmp;
                return prob;
            }
            catch
            { return null; }
        }
    }

    public enum TypeE { image, detection, gt }

    public static class Extend
    {
        public static double[] ToDouble(this string[] strs) =>
             strs.Select(s => double.Parse(s)).ToArray();
    }
}
