using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using static FaceDetectionTool_WPF.Properties.Settings;
using System.Windows;

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
        public int GetNum()
        {
            int num = 0;
            using (var sr = new StreamReader(FrmList, Encoding.Default))
                int.TryParse(sr.ReadLine(), out num);
            return num;
        }

        /// <summary>
        /// Get each file path.
        /// </summary>
        /// <returns> </returns>
        public string[] GetPath(TypeE s)
        {
            try
            {
                int num = GetNum();
                if (num == 0)
                    return null;
                var lines = File.ReadAllLines(FrmList, Encoding.Default).Skip(1).Take(num);
                Func<string, string> func = l => l;
                switch (s)
                {
                    case TypeE.image: func = l => Path.Combine(FrmImagePath, l); break;
                    case TypeE.detection: func = l => Path.Combine(FrmDetectionFrPath, l.Split('.')[0] + ".jpg.fr"); break;
                    case TypeE.gt: func = l => Path.Combine(FrmGtFrPath, l.Split('.')[0] + ".fr"); break;
                }
                return lines.Select(func).ToArray();
            }
            catch
            { return null; }
        }

        /// <summary>
        /// Draw the bounding box on the detection image
        /// </summary>
        /// <param name="image">The detection image</param>
        /// <param name="fr">Bounding box(rectangle or ellipse)</param>
        /// <param name="color">Color of bounding box</param>
        /// <param name="type">FR type("detection" or "gt")</param>
        /// <returns>The probability of face bounding box</returns>
        public double[] showFR(Image image, string fr, Brush color, TypeE type)
        {
            var img = (BitmapSource)image.Source;
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(image.Source, new Rect(0, 0, img.Width, img.Height));

            var lines = File.ReadAllLines(fr);
            int num_face = int.Parse(lines.First());
            var strsList = lines.Skip(1).Take(num_face).Select(l => l.Replace(' ', '\t').Split('\t')).ToArray();
            try
            {
                double xtl, ytl, width, height;
                double[] prob = new double[num_face];

                if (type == TypeE.detection)
                {
                    for (int i = 0; i < num_face; i++)
                    {
                        var s = strsList[i];
                        xtl = Convert.ToDouble(s[0]);
                        ytl = Convert.ToDouble(s[1]);
                        width = Convert.ToDouble(s[2]) - xtl + 1;
                        height = Convert.ToDouble(s[3]) - ytl + 1;
                        prob[i] = Convert.ToDouble(s[4]);

                        var pen = new Pen(color, 4.0f);
                        drawingContext.DrawRectangle(null, pen, new Rect(xtl, ytl, width, height));
                    }
                }
                else if (type == TypeE.gt)
                {
                    for (int i = 0; i < num_face; i++)
                    {
                        var s = strsList[i];
                        if (s.Length == 5)
                        {
                            width = Convert.ToDouble(s[0]);
                            height = Convert.ToDouble(s[1]);
                            xtl = Convert.ToDouble(s[3]);
                            ytl = Convert.ToDouble(s[4]);
                            var angle = Convert.ToDouble(Convert.ToDouble(s[2]) / Math.PI * 180);
                            prob[i] = 1;

                            var rt = new RotateTransform(angle, xtl, ytl);
                            drawingContext.PushTransform(rt);
                            Pen pen = new Pen(color, 4.0f);
                            drawingContext.DrawEllipse(null, pen, new Point(xtl, ytl), width, height);
                            drawingContext.Pop();
                        }
                        else
                        {
                            xtl = Convert.ToDouble(s[0]);
                            ytl = Convert.ToDouble(s[1]);
                            width = Convert.ToDouble(s[2]) - xtl + 1;
                            height = Convert.ToDouble(s[3]) - ytl + 1;
                            prob[i] = 1;

                            Pen pen = new Pen(color, 4.0f);
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
}
