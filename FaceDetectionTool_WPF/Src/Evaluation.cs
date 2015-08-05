using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FaceDetectionTool_WPF
{
    public static class Evaluation
    {
        public static Point EvalPrecisionAndRecall(this IEnumerable<ImageInfo> list, double P, double T, double thr = 0.5)
        {
            double fp = list.Sum(i => { i.InitShapes(false); return i.GetMatches(thr).Count; });
            return new Point(fp / T * 100, fp / P * 100);
        }

        public static Point[] EvalPoints(this IEnumerable<ImageInfo> list, int count, double thrS = 0.5, double thrE = 1)
        {
            double p = list.Sum(i => i.FrList.Count);
            double t = list.Sum(i => i.GtList.Count);
            var points = new Point[count];
            var delta = (thrE - thrS) / count;
            //Parallel.For(0, count, (i) =>
            // { points[i] = list.EvalPrecisionAndRecall(p, t, thrS + i * delta); });
            for (int i = 0; i < count; i++)
                points[i] = list.EvalPrecisionAndRecall(p, t, thrS + i * delta);
            return points;
        }
    }
}
