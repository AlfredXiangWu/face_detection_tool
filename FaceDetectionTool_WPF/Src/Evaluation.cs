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
        /// <summary>
        /// 计算Recall(对应X)和Precision(对应Y)。
        /// </summary>
        /// <param name="list">ImageInfo列表</param>
        /// <param name="thr">概率阈值</param>
        /// <returns></returns>
        public static Point EvalRecallAndPrecision(this IEnumerable<ImageInfo> list, double thr = 0.5)
        {
            double p = list.Sum(i => i.FrList.Where(f => f[4] >= thr).Count());
            double t = list.Sum(i => i.GtList.Count);
            double tp = list.Sum(i => i.Matches.Where(m => m.Prob >= thr).Count());
            return new Point(tp / t, tp / p);
        }

        public static Point[] EvalPoints(this IEnumerable<ImageInfo> list, int count, double thrS = 0.5, double thrE = 1)
        {
            var points = new Point[count + 1];
            var delta = (thrE - thrS) / count;

            double t = list.Sum(i => i.GtList.Count);
            var frlist = list.SelectMany(ii => ii.FrList.Select(f => f[4])).Where(d => d >= 0.5).ToArray();
            var tplist = list.SelectMany(ii => ii.Matches.Select(m => m.Prob)).Where(d => d >= 0.5).ToArray();
            Array.Sort(frlist);
            Array.Sort(tplist);

            double frCount = frlist.Length;
            double tpCount = tplist.Length;
            int frIndex = 0;
            int tpIndex = 0;

            for (int i = 0; i <= count; i++)
            {
                var thr = thrS + i * delta;
                for (; frIndex < frCount; frIndex++)
                    if (frlist[frIndex] >= thr)
                        break;
                for (; tpIndex < tpCount; tpIndex++)
                    if (tplist[tpIndex] >= thr)
                        break;
                points[i] = new Point((tpCount - tpIndex) / t, (tpCount - tpIndex) / (frCount - frIndex));
            };
            return points;
        }

        public static Point[] TestPoints(int count, Func<double, double> func)
        {
            var points = new Point[count + 1];
            var delta = 1.0 / count;
            for (int i = 0; i <= count; i++)
            {
                var x = 0 + i * delta;
                var y = func(x);
                if (double.IsNaN(y))
                    continue;
                points[i].X = x;
                points[i].Y = y;
            }
            return points;
        }
    }
}
