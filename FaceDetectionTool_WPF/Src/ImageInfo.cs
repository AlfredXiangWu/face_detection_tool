using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FaceDetectionTool_WPF
{
    public class ImageInfo
    {
        public string Path { get; set; }

        public string PathGt { get; set; }

        private List<double[]> gtList;
        public List<double[]> GtList => gtList = (gtList ?? GetInfoList(PathGt));

        public string PathFr { get; set; }

        private List<double[]> frList;
        public List<double[]> FrList => frList = (frList ?? GetInfoList(PathFr));

        private BitmapSource bitmap;
        public BitmapSource Bitmap => bitmap = (bitmap ?? new BitmapImage(new Uri(Path)));

        public List<Rectangle> Rectangles { get; set; } = new List<Rectangle>();
        public List<Ellipse> Ellipses { get; set; } = new List<Ellipse>();

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

        public double[] AddShapes(Brush color, TypeE type, Canvas canvas)
        {
            try
            {                
                var opacity = 0.5;
                double[] prob = null;
                double xtl, ytl, width, height;
                var children = canvas.Children;
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

                                var rt = new Rectangle()
                                {
                                    Width = width,
                                    Height = height,
                                    Fill = color,
                                    Opacity = opacity,
                                };
                                children.Add(rt);
                                Rectangles.Add(rt);
                                Canvas.SetTop(rt, ytl);
                                Canvas.SetLeft(rt, xtl);
                                // drawingContext.DrawRectangle(null, pen, new Rect(xtl, ytl, width, height));
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

                                    var t = new RotateTransform(angle, width, height);
                                    var el = new Ellipse()
                                    {
                                        Width = width * 2,
                                        Height = height * 2,
                                        Fill = color,
                                        RenderTransform = t,
                                        Opacity = opacity,
                                        Tag = angle,
                                    };
                                    children.Add(el);
                                    Ellipses.Add(el);
                                    Canvas.SetTop(el, ytl - height);
                                    Canvas.SetLeft(el, xtl - width);
                                }
                                else
                                {
                                    xtl = s[0];
                                    ytl = s[1];
                                    width = s[2] - xtl + 1;
                                    height = s[3] - ytl + 1;
                                    prob[i] = 1;

                                    var rt = new Rectangle()
                                    {
                                        Width = width,
                                        Height = height,
                                        Fill = color,
                                        Opacity = opacity,
                                    };
                                    children.Add(rt);
                                    Rectangles.Add(rt);
                                    Canvas.SetTop(rt, ytl);
                                    Canvas.SetLeft(rt, xtl);
                                }
                            }
                        }
                        break;
                }
                return prob;
            }
            catch
            { return null; }
        }

        private static List<double[]> GetInfoList(string path)
        {
            var txt = File.ReadAllLines(path);
            var count = int.Parse(txt[0]);
            var lines = txt.Skip(1).Take(count);
            return lines.Select(l =>
                l.Replace(' ', '\t').Split('\t')
                .Select(s => double.Parse(s)).ToArray()).ToList();
        }
    }

    public enum TypeE { Image, Detection, Gt }
}
