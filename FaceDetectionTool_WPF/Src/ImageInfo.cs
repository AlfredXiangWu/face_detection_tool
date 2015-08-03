using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FaceDetectionTool_WPF
{
    public class ImageInfo
    {
        public string Path { get; set; }

        public string PathGt { get; set; }

        public List<double[]> GtList { get; set; }

        public string PathFr { get; set; }

        public List<double[]> FrList { get; set; }

        private BitmapSource bitmap;
        public BitmapSource Bitmap
        {
            get
            {
                if (bitmap == null)
                    bitmap = new BitmapImage(new Uri(Path));
                return bitmap;
            }
        }

        public BitmapSource Drawing(Brush color, TypeE type, BitmapSource img = null, double[] prob = null)
        {
            img = img ?? Bitmap;
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(img, new Rect(0, 0, img.Width, img.Height));
            var pen = new Pen(color, 4.0f);
            try
            {
                double xtl, ytl, width, height;
                switch (type)
                {
                    case TypeE.Detection:
                        {
                            prob = new double[FrList.Count];
                            for (int i = 0; i < prob.Length; i++)
                            {
                                var s = FrList[i];
                                xtl = s[0];
                                ytl = s[1];
                                width = s[2] - xtl + 1;
                                height = s[3] - ytl + 1;
                                prob[i] = s[4];

                                drawingContext.DrawRectangle(null, pen, new Rect(xtl, ytl, width, height));
                            }
                        }
                        break;
                    case TypeE.Gt:
                        {
                            prob = new double[GtList.Count];
                            for (int i = 0; i < prob.Length; i++)
                            {
                                var s = GtList[i];
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
                        break;
                }

                drawingContext.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap(img.PixelWidth, img.PixelHeight, img.DpiX, img.DpiY, PixelFormats.Pbgra32);
                bmp.Render(drawingVisual);
                return bmp;
            }
            catch
            { return null; }
        }
    }

    public enum TypeE { Image, Detection, Gt }
}
