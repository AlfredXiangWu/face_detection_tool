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
            try
            {
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
                                path[i] = _detection_fr_path_ + "\\" + temp[0] + ".jpg.fr";
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
            catch
            {
                return null;
            }
        }


        
       /// <summary>
       /// Draw the bounding box on the detection image
       /// </summary>
       /// <param name="img">The detection image</param>
       /// <param name="fr">Bounding box(rectangle or ellipse)</param>
       /// <param name="color">Color of bounding box</param>
       /// <param name="type">FR type("detection" or "gt")</param>
       /// <returns>The probability of face bounding box</returns>
        public double[] showFR(Image img, string fr, Color color, string type)
        {
            int num_face = 0;
            StreamReader sr_fr = new StreamReader(fr);
            try
            {
                num_face = Convert.ToInt32(sr_fr.ReadLine().ToString());

                float xtl, ytl, width, height;
                double[] prob = new double[num_face];
                string str;

                if (String.Equals(type, "detection"))
                {
                    
                    for (int i = 0; i < num_face; i++)
                    {
                        str = sr_fr.ReadLine().ToString();
                        str = str.Replace(' ', '\t');
                        string[] s = str.Split('\t');
                        xtl = Convert.ToSingle(s[0]);
                        ytl = Convert.ToSingle(s[1]);
                        width = Convert.ToSingle(s[2]) - xtl + 1;
                        height = Convert.ToSingle(s[3]) - ytl + 1;
                        prob[i] = Convert.ToDouble(s[4]);

                        Graphics g = Graphics.FromImage(img);
                        Pen pen = new Pen(color, 4.0f);
                        g.DrawRectangle(pen, xtl, ytl, width, height);
                        g.Dispose();
                    }
                }
                else if (String.Equals(type, "gt"))
                {
                    for (int i = 0; i < num_face; i++)
                    {
                        str = sr_fr.ReadLine().ToString();
                        str = str.Replace(' ', '\t');
                        string[] s = str.Split('\t');
                        if (s.Length == 5)
                        {
                            xtl = -Convert.ToSingle(s[0]);
                            ytl =  -Convert.ToSingle(s[1]);
                            width = Convert.ToSingle(s[0])*2;
                            height = Convert.ToSingle(s[1])*2;
                            float angle = Convert.ToSingle(Convert.ToDouble(s[2])/Math.PI*180);
                            prob[i] = 1;

                            Graphics g = Graphics.FromImage(img);
                            g.TranslateTransform(Convert.ToSingle(s[3]), Convert.ToSingle(s[4]));
                            g.RotateTransform(angle);
                            Pen pen = new Pen(color, 4.0f);
                            g.DrawEllipse(pen, xtl, ytl, width, height);
                            g.Dispose();
                        }
                        else
                        {
                            xtl = Convert.ToSingle(s[0]);
                            ytl = Convert.ToSingle(s[1]);
                            width = Convert.ToSingle(s[2]) - xtl + 1;
                            height = Convert.ToSingle(s[3]) - ytl + 1;
                            prob[i] = 1;

                            Graphics g = Graphics.FromImage(img);
                            Pen pen = new Pen(color, 4.0f);
                            g.DrawRectangle(pen, xtl, ytl, width, height);
                            g.Dispose();
                        }
                    }
                }
                return prob;
            }
            catch
            {
                return null;
            }  
        }
    }
}
