using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace face_detection_tool
{
    class IO
    {
        /// <summary>
        /// Image Path
        /// </summary>
        private string _image_path_;
        public string frmImagePath
        {
            get { return _image_path_; }
            set { _image_path_ = value; }
        }

        /// <summary>
        /// List File 
        /// </summary>
        private string _list_;
        public string frmList
        {
            get { return _list_; }
            set { _list_ = value; }
        }

        /// <summary>
        /// Detection FR Path
        /// </summary>
        private string _detection_fr_path_;
        public string frmDetectionFrPath
        {
            get { return _detection_fr_path_; }
            set { _detection_fr_path_ = value; }
        }

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        private string _gt_fr_path_;
        public string frmGtFrPath
        {
            get { return _gt_fr_path_; }
            set { _gt_fr_path_ = value; }
        }

        /// <summary>
        /// Get number of image
        /// </summary>
        /// <returns></returns>
        public int getNum()
        {
            int num;
            StreamReader sr = new StreamReader(_list_, Encoding.Default);
            num = Convert.ToInt32(sr.ReadLine().ToString());
            return num;
        }

        /// <summary>
        /// Get each file path.
        /// </summary>
        /// <returns> </returns>
        public string[] getPath(string s)
        {
            int num = 0;
            num = this.getNum();
            StreamReader sr = new StreamReader(_list_, Encoding.Default);
            sr.ReadLine();          // skip num
            if (0 == num)
            {
                return null;
            }
            string[] path = new string[num];
            switch (s)
            {
                case "image":
                    {
                        for (int i = 0; i < num; i++)
                        {
                            path[i] = _image_path_ + "\\" + sr.ReadLine().ToString();
                        }
                        break;
                    }
                case "detection":
                    {
                        for (int i = 0; i < num; i++)
                        {
                            string[] temp = sr.ReadLine().ToString().Split('.');
                            path[i] = _detection_fr_path_ + "\\" + temp[0] + ".fr";
                        }
                        break;
                    }
                case "gt":
                    {
                        for (int i = 0; i < num; i++)
                        {
                            string[] temp = sr.ReadLine().ToString().Split('.');
                            path[i] = _gt_fr_path_ + "\\" + temp[0] + ".fr";
                        }
                        break;
                    }  
            }
            
            return path;
        }

        /// <summary>
        /// Draw the bounding box on the face detection image
        /// </summary>
        /// <param name="img">picturebox.Image</param>
        /// <param name="fr">The path of fr file</param>
        /// <returns>The probability of face box</returns>
        public double[] showFR(Image img, string fr, Color color)
        {
            int num_face = 0;
            StreamReader sr_detect_fr = new StreamReader(fr);
            num_face = Convert.ToInt32(sr_detect_fr.ReadLine().ToString());
            int xtl, ytl, xbr, ybr;
            double[] prob = new double[num_face];
            string str;
            for (int i = 0; i < num_face; i++)
            {
                str = sr_detect_fr.ReadLine().ToString();
                str = str.Replace(' ', '\t');
                string[] s = str.Split('\t');
                xtl = Convert.ToInt32(s[0]);
                ytl = Convert.ToInt32(s[1]);
                xbr = Convert.ToInt32(s[2]);
                ybr = Convert.ToInt32(s[3]);
                if (s.Length ==5)
                    prob[i] = Convert.ToDouble(s[4]);
                else 
                    prob[i] = 1;

                Graphics g = Graphics.FromImage(img);
                Pen pen = new Pen(color, 4.0f);
                g.DrawRectangle(pen, new Rectangle(xtl, ytl, xbr - xtl + 1, ybr - ytl + 1));
                g.Dispose();
            }
            return prob;
        }


    }
}
