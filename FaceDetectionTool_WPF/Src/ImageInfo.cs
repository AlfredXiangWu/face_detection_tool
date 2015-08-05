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
using ShapePath = System.Windows.Shapes.Path;

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

        public BitmapSource Bitmap => new BitmapImage(new Uri(Path));

        public List<Shape> D_Shapes { get; set; }

        public List<Shape> G_Shapes { get; set; }

        private List<ShapeMatch> macthes;
        public List<ShapeMatch> Matches
        {
            get
            {
                if (macthes == null)
                    macthes = GetMatches();
                return macthes;
            }
        }

        public bool AddShapes(Brush color, TypeE type, double opacity = 0.5)
        {
            try
            {
                if (D_Shapes == null)
                    D_Shapes = new List<Shape>();
                if (G_Shapes == null)
                    G_Shapes = new List<Shape>();

                double xtl, ytl, width, height;
                switch (type)
                {
                    case TypeE.Detection:
                        {
                            foreach (var fr in FrList)
                            {
                                xtl = fr[0];
                                ytl = fr[1];
                                width = fr[2] - xtl + 1;
                                height = fr[3] - ytl + 1;

                                var g_rt = new RectangleGeometry(new Rect(xtl, ytl, width, height));
                                var rt = new ShapePath()
                                {
                                    Data = g_rt,
                                    Fill = color,
                                    Opacity = opacity,
                                    Tag = g_rt,
                                };
                                D_Shapes.Add(rt);
                            }
                        }
                        break;
                    case TypeE.Gt:
                        {
                            foreach (var gt in GtList)
                            {
                                if (gt.Length == 5)
                                {
                                    width = gt[0];
                                    height = gt[1];
                                    var angle = gt[2] / Math.PI * 180;
                                    xtl = gt[3];
                                    ytl = gt[4];

                                    var rt = new RotateTransform(angle, xtl, ytl);
                                    var g_el = new EllipseGeometry()
                                    {
                                        Center = new Point(xtl, ytl),
                                        RadiusX = width,
                                        RadiusY = height,
                                        Transform = rt,
                                    };
                                    var el = new ShapePath()
                                    {
                                        Data = g_el,
                                        Fill = color,
                                        Opacity = opacity,
                                        Tag = g_el,
                                    };
                                    G_Shapes.Add(el);
                                }
                                else
                                {
                                    xtl = gt[0];
                                    ytl = gt[1];
                                    width = gt[2] - xtl + 1;
                                    height = gt[3] - ytl + 1;

                                    var g_rt = new RectangleGeometry(new Rect(xtl, ytl, width, height));
                                    var rt = new ShapePath()
                                    {
                                        Data = g_rt,
                                        Fill = color,
                                        Opacity = opacity,
                                        Tag = g_rt,
                                    };
                                    G_Shapes.Add(rt);
                                }
                            }
                        }
                        break;
                }
                return true;
            }
            catch
            { return false; }
        }

        public void ClearShapes()
        {
            D_Shapes?.Clear();
            G_Shapes?.Clear();
        }

        public IEnumerable<ShapeMatch> GetIUAreas(double tolerance = 0, ToleranceType toleranceType = ToleranceType.Relative)
        {
            foreach (var gss in G_Shapes)
            {
                foreach (var dss in D_Shapes)
                {
                    var ds = (Geometry)dss.Tag;
                    var gs = (Geometry)gss.Tag;
                    var gi = Geometry.Combine(ds, gs, GeometryCombineMode.Intersect, null, tolerance, toleranceType)
                        .GetArea(tolerance, toleranceType);
                    var gu = Geometry.Combine(ds, gs, GeometryCombineMode.Union, null, tolerance, toleranceType)
                        .GetArea(tolerance, toleranceType);
                    yield return new ShapeMatch() { IoU = gi / gu, D_Shape = dss, G_Shape = gss };
                }
            }
        }

        public List<ShapeMatch> GetMatches(double value = 0.5)
        {
            var list = new List<ShapeMatch>();
            var dss = new List<Shape>();
            var gss = new List<Shape>();
            var ordered = GetIUAreas().Where(m => m.IoU > value).OrderByDescending(m => m.IoU);
            foreach (var m in ordered)
            {
                if (dss.Contains(m.D_Shape) || gss.Contains(m.G_Shape))
                    continue;
                list.Add(m);
                dss.Add(m.D_Shape);
                gss.Add(m.G_Shape);
            }
            return list;
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

    public class ShapeMatch
    {
        public double IoU;
        public Shape G_Shape;
        public Shape D_Shape;
    }
    public enum TypeE { Image, Detection, Gt }
}
