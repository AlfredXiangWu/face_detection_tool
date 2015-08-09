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

        public string RelativeImgPath { get; set; }

        public string PathGt { get; set; }

        private List<double[]> gtList;
        public List<double[]> GtList => gtList = (gtList ?? GetInfoList(PathGt));

        public string PathFr { get; set; }

        private List<double[]> frList;
        public List<double[]> FrList => frList = (frList ?? GetInfoList(PathFr));

        public BitmapSource Bitmap => new BitmapImage(new Uri(Path));

        public List<Geometry> D_Geometries { get; set; }

        public List<Geometry> G_Geometries { get; set; }

        private List<ShapeMatch> matches;
        public List<ShapeMatch> Matches
        {
            get
            {
                if (matches == null)
                    matches = GetMatches();
                return matches;
            }
        }

        public int TP => Matches.Count;
        public int FP => FrList.Count - TP;

        public bool CreateGeometry(TypeE type)
        {
            try
            {
                double xtl, ytl, width, height;
                switch (type)
                {
                    case TypeE.Detection:
                        {
                            if (D_Geometries == null)
                                D_Geometries = new List<Geometry>();
                            foreach (var fr in FrList)
                            {
                                xtl = fr[0];
                                ytl = fr[1];
                                width = fr[2] - xtl + 1;
                                height = fr[3] - ytl + 1;

                                var g_rt = new RectangleGeometry(new Rect(xtl, ytl, width, height));
                                D_Geometries.Add(g_rt);
                            }
                        }
                        break;
                    case TypeE.Gt:
                        {
                            if (G_Geometries == null)
                                G_Geometries = new List<Geometry>();
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
                                    G_Geometries.Add(g_el);
                                }
                                else
                                {
                                    xtl = gt[0];
                                    ytl = gt[1];
                                    width = gt[2] - xtl + 1;
                                    height = gt[3] - ytl + 1;

                                    var g_rt = new RectangleGeometry(new Rect(xtl, ytl, width, height));
                                    G_Geometries.Add(g_rt);
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

        public void ClearGeometries()
        {
            D_Geometries = null;
            G_Geometries = null;
        }

        public void GeometriesPrepare()
        {
            if (D_Geometries == null)
                CreateGeometry(TypeE.Detection);
            if (G_Geometries == null)
                CreateGeometry(TypeE.Gt);
        }

        public List<ShapeMatch> GetMatches(double tolerance = 0, ToleranceType toleranceType = ToleranceType.Relative, double thr = 0.5)
        {
            GeometriesPrepare();
            var all = new List<ShapeMatch>();
            for (int i = 0; i < G_Geometries.Count; i++)
            {
                var gg = G_Geometries[i];
                for (int j = 0; j < D_Geometries.Count; j++)
                {
                    var dg = D_Geometries[j];
                    var gi = Geometry.Combine(dg, gg, GeometryCombineMode.Intersect, null, tolerance, toleranceType)
                        .GetArea(tolerance, toleranceType);
                    var gu = Geometry.Combine(dg, gg, GeometryCombineMode.Union, null, tolerance, toleranceType)
                        .GetArea(tolerance, toleranceType);
                    if (gi / gu > thr)
                        all.Add(new ShapeMatch() { IoU = gi / gu, FrIndex = j, GtIndex = i, Prob = FrList[j][4] });
                }
            }

            var list = new List<ShapeMatch>();
            var dss = new List<int>();
            var gss = new List<int>();
            var ordered = all.OrderByDescending(m => m.IoU);
            foreach (var m in ordered)
            {
                if (dss.Contains(m.FrIndex) || gss.Contains(m.GtIndex))
                    continue;
                list.Add(m);
                dss.Add(m.FrIndex);
                gss.Add(m.GtIndex);
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

        public IEnumerable<ShapePath> GetShapes(Brush ColorFr, Brush ColorGt, double Opacity = 0.5, double StrokeThickness = 4)
        {
            var m_fr = Matches.Select(m => m.FrIndex);
            var m_gt = Matches.Select(m => m.GtIndex);
            var shapes = new List<ShapePath>();

            for (int i = 0; i < D_Geometries.Count; i++)
            {
                var rt = new ShapePath()
                {
                    Data = D_Geometries[i],
                    Stroke = ColorFr,
                    StrokeThickness = StrokeThickness,
                };
                if (m_fr.Contains(i))
                {
                    rt.Opacity = Opacity;
                    rt.Fill = ColorFr;
                }
                shapes.Add(rt);
            }

            for (int i = 0; i < G_Geometries.Count; i++)
            {
                var rt = new ShapePath()
                {
                    Data = G_Geometries[i],
                    Stroke = ColorGt,
                    StrokeThickness = StrokeThickness,
                };
                if (m_gt.Contains(i))
                {
                    rt.Opacity = Opacity;
                    rt.Fill = ColorGt;
                }
                shapes.Add(rt);
            }
            return shapes;
        }
    }

    public class ShapeMatch
    {
        public double IoU;
        public double Prob;
        public int FrIndex;
        public int GtIndex;
    }
    public enum TypeE { Image, Detection, Gt }
}
