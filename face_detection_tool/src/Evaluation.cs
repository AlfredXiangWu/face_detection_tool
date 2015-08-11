using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;



namespace face_detection_tool
{
    public class Evaluation
    {
        /// <summary>
        /// Evaluate single image to compute true positive samples and false positive samples
        /// </summary>
        /// <param name="image_info"></param>
        /// <returns>Number of true positive samples</returns>
        public int singleImageMatch(ImageInfo image_info, double thr)
        {
            List<double[]> detect_face_fr = image_info.DetectFrList;
            List<double[]> gt_face_fr = image_info.GtFrList;
            int count = 0;
            bool[] flag = new bool[detect_face_fr.Count];
            for (int i = 0; i < flag.Length; i++)
            {
                flag[i] = false;
            }

            image_info.InitScores();

            for (int i = 0; i < gt_face_fr.Count; i++)
            {
                int idx = -1;
                double max_score = 0;
                for (int j = 0; j < detect_face_fr.Count; j++)
                {
                    double temp = computeIoU(gt_face_fr[i], detect_face_fr[j]);
                    if (max_score < temp)
                    {
                        max_score = temp;
                        idx = j;
                    }
                }
                if ((idx != -1) && (flag[idx] == false) && (max_score > thr))
                {
                    image_info.Scores[idx] = max_score;
                    flag[idx] = true;
                    count = count + 1;
                }
            }

            return count;
        }
        
        /// <summary>
        /// Compute the IoU score of two bounding box
        /// </summary>
        /// <param name="gt_face_fr"></param>
        /// <param name="detect_face_fr"></param>
        /// <returns></returns>
        private double computeIoU(double[] gt_face_fr, double[] detect_face_fr)
        {
            double xtl1, ytl1, xbr1, ybr1, area1;
            double xtl2, ytl2, xbr2, ybr2, area2;
            double area_intersect, area_union;

            if (gt_face_fr.Length == 5)
            {
                double a, b, w, width, height;
                a = gt_face_fr[0];
                b = gt_face_fr[1];
                w = gt_face_fr[2];

                width = 2 * Math.Sqrt(Math.Pow(a, 2) + (Math.Pow(b, 2) - Math.Pow(a, 2)) * Math.Pow(Math.Sin(w), 2));
                height = 2 * Math.Sqrt(Math.Pow(a, 2) + (Math.Pow(b, 2) - Math.Pow(a, 2)) * Math.Pow(Math.Cos(w), 2));
                xtl1 = gt_face_fr[3] - 0.5 * width;
                ytl1 = gt_face_fr[4] - 0.5 * height;
                xbr1 = xtl1 + width - 1;
                ybr1 = ytl1 + height - 1;
            }
            else
            {
                xtl1 = gt_face_fr[0];
                ytl1 = gt_face_fr[1];
                xbr1 = gt_face_fr[2];
                ybr1 = gt_face_fr[3];
            }
            area1 = computeArea(xtl1, ytl1, xbr1, ybr1);

            xtl2 = detect_face_fr[0];
            ytl2 = detect_face_fr[1];
            xbr2 = detect_face_fr[2];
            ybr2 = detect_face_fr[3];
            area2 = computeArea(xtl2, ytl2, xbr2, ybr2);

            // compute score
            double xx1, xx2, yy1, yy2;
            xx1 = Math.Max(xtl1, xtl2);
            xx2 = Math.Min(xbr1, xbr2);
            yy1 = Math.Max(ytl1, ytl2);
            yy2 = Math.Min(ybr1, ybr2);
            area_intersect = Math.Max(0, xx2 - xx1 + 1) * Math.Max(0, yy2 - yy1 + 1);
            area_union = area1 + area2 - area_intersect;

            return (area_intersect / area_union);
        }
        
        /// <summary>
        /// Compute rectangle area
        /// </summary>
        /// <param name="xtl"></param>
        /// <param name="ytl"></param>
        /// <param name="xbr"></param>
        /// <param name="ybr"></param>
        /// <returns></returns>
        private double computeArea(double xtl, double ytl, double xbr, double ybr)
        {
            return (xbr - xtl + 1) * (ybr - ytl + 1);
        }




    }
}
