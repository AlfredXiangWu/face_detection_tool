using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace face_detection_tool
{
    public partial class Form1 : Form
    {
        IO io = new IO();
        string[] img_path;
        string[] detection_fr_path;
        string[] gt_fr_path;
        int count = 0;
        int num_image = 0;
        Bitmap img;

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration new_configuration = new Configuration();
            new_configuration.Show();
            new_configuration.accept += new EventHandler(new_configuration_accept);
        }


        void new_configuration_accept(object sender, EventArgs e)
        {
            Configuration new_configuration = (Configuration)sender;

            //get path
            io.frmImagePath = new_configuration.frmImagePath;
            io.frmList = new_configuration.frmList;
            io.frmDetectionFrPath = new_configuration.frmDetectionFrPath;
            io.frmGtFrPath = new_configuration.frmGtFrPath;
            img_path = io.getPath("image");
            detection_fr_path = io.getPath("detection");
            gt_fr_path = io.getPath("gt");
            num_image = io.getNum();

            // image
            img = new Bitmap(img_path[count]);
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // detection fr
            int num_detect_face = 0;
            StreamReader sr_detect_fr = new StreamReader(detection_fr_path[count]);
            num_detect_face = Convert.ToInt32(sr_detect_fr.ReadLine().ToString());
            int xtl, ytl, xbr, ybr;
            double prob = 0;
            string str;
            for (int i = 0; i < num_detect_face; i++)
            {
                str = sr_detect_fr.ReadLine().ToString();
                string[] s = str.Split(' ');
                xtl = Convert.ToInt32(s[0]);
                ytl = Convert.ToInt32(s[1]);
                xbr = Convert.ToInt32(s[2]);
                ybr = Convert.ToInt32(s[3]);
                prob = Convert.ToDouble(s[4]);

                Graphics g = Graphics.FromImage(pictureBox1.Image);
                Pen pen = new Pen(Color.Blue, 2.0f);
                g.DrawRectangle(pen, new Rectangle(xtl, ytl, xbr - xtl + 1, ybr - ytl + 1));
                g.Dispose();
            }
            
            new_configuration.Hide();
        }


    }
}
