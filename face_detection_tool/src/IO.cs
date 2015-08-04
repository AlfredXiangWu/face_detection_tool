using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace face_detection_tool
{
    public class IO
    {
        /// <summary>
        /// Image Path
        /// </summary>
        public string FrmImagePath { get; set; }

        /// <summary>
        /// List File 
        /// </summary>
        public string FrmList { get; set; }

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string FrmDetectionFrPath { get; set; }

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        public string FrmGtFrPath { get; set; }

        /// <summary>
        /// Get Image Information
        /// </summary>
        /// <returns></returns>
        public List<ImageInfo> getImageInfo()
        {
            StreamReader txt = new StreamReader(FrmList, Encoding.Default);
            int num = Convert.ToInt32(txt.ReadLine());
            List<ImageInfo> list = new List<ImageInfo>(num);
            for (int i = 0; i < num; i++)
            {
                ImageInfo image_info = new ImageInfo();
                var name = txt.ReadLine();
                image_info.ImgPath = Path.Combine(FrmImagePath, name);
                var tmp = name.Split('.')[0];
                image_info.DetectFrPath = Path.Combine(FrmDetectionFrPath, tmp + ".jpg.fr");
                image_info.GtFrPath = Path.Combine(FrmGtFrPath, tmp + ".fr");

                //image_info.DetecFrList = getFrList(image_info.DetectFrPath);
                //image_info.GtFrList = getFrList(image_info.GtFrPath);

                list.Add(image_info);
            }
            return list;
        }

        /// <summary>
        /// Get FR
        /// </summary>
        /// <param name="path">FR Path</param>
        /// <returns>List of FR</returns>
        public List<double[]> getFrList(string path)
        {
            StreamReader txt = new StreamReader(path, Encoding.Default);
            int num = Convert.ToInt32(txt.ReadLine());
            var list = new List<double[]>(num);
            for (int i = 0; i < num; i++)
            {
                var line = txt.ReadLine().Replace(' ', '\t').Split('\t').Select(s => double.Parse(s)).ToArray();
                list.Add(line);
            }
            return list;
        }
        
        /// <summary>
        /// Draw the bounding box on the detection image
        /// </summary>
        /// <param name="image">Detection Image</param>
        /// <param name="image_info">Image Information</param>
        public void showFR(Image image, ImageInfo image_info)
        {
            image_info.DetectFrList = getFrList(image_info.DetectFrPath);
            image_info.GtFrList = getFrList(image_info.GtFrPath);
            int num_detect_face = image_info.DetectFrList.Count;
            int num_gt_face = image_info.GtFrList.Count;

            double xtl, ytl, width, height;

            // draw detection fr (rectangle)
            for (int i = 0; i < num_detect_face; i++)
            {
                var s = image_info.DetectFrList[i];
                xtl = s[0];
                ytl = s[1];
                width = s[2] - xtl + 1;
                height = s[3] - ytl + 1;

                Graphics g = Graphics.FromImage(image);
                Pen pen = new Pen(Color.Blue, 4.0f);
                g.DrawRectangle(pen, Convert.ToSingle(xtl), Convert.ToSingle(ytl), Convert.ToSingle(width), Convert.ToSingle(height));
                g.Dispose();
            }

            // draw ground truth (rectangle or ellipse)
            for (int i = 0; i < num_gt_face; i++)
            {
                var s = image_info.GtFrList[i];
                if (s.Length == 5)
                {
                    xtl = -s[0];
                    ytl = -s[1];
                    width = s[0]*2;
                    height = s[1]*2;
                    float angle = Convert.ToSingle(Convert.ToDouble(s[2]) / Math.PI * 180);

                    Graphics g = Graphics.FromImage(image);
                    g.TranslateTransform(Convert.ToSingle(s[3]), Convert.ToSingle(s[4]));
                    g.RotateTransform(angle);
                    Pen pen = new Pen(Color.Red, 4.0f);
                    g.DrawEllipse(pen, Convert.ToSingle(xtl), Convert.ToSingle(ytl), Convert.ToSingle(width), Convert.ToSingle(height));
                    g.Dispose();
                }
                else
                {
                    xtl = s[0];
                    ytl = s[1];
                    width = s[2] - xtl + 1;
                    height = s[3] - ytl + 1;

                    Graphics g = Graphics.FromImage(image);
                    Pen pen = new Pen(Color.Red, 4.0f);
                    g.DrawRectangle(pen, Convert.ToSingle(xtl), Convert.ToSingle(ytl), Convert.ToSingle(width), Convert.ToSingle(height));
                    g.Dispose();
                }
            }
        }
    }
}
