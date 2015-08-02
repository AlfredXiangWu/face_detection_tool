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
            KeyDown += new KeyEventHandler(Form1_KeyDown);
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
            button1.Enabled = true;
            button2.Enabled = true;
            infoToolStripMenuItem.Enabled = true;

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

            // show detection result
            io.showFR(pictureBox1.Image, detection_fr_path[count], Color.Blue, "detection");
            // show ground truth
            io.showFR(pictureBox1.Image, gt_fr_path[count], Color.Red, "gt");

             new_configuration.Hide();
        }

        /// <summary>
        /// Next button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (count == 0)
            {
                count = io.getNum();
            }
            count = count - 1;

            img = new Bitmap(img_path[count]);
            pictureBox1.Image = null;
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // show detection result
            io.showFR(pictureBox1.Image, detection_fr_path[count], Color.Blue, "detection");
            // show ground truth
            io.showFR(pictureBox1.Image, gt_fr_path[count], Color.Red, "gt");
        }

        /// <summary>
        /// Last button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (count == io.getNum() - 1)
            {
                count = -1;
            }
            count = count + 1;
            img = new Bitmap(img_path[count]);
            pictureBox1.Image = null;
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // show detection result
            io.showFR(pictureBox1.Image, detection_fr_path[count], Color.Blue, "detection");
            // show ground truth
            io.showFR(pictureBox1.Image, gt_fr_path[count], Color.Red, "gt");
        }

        /// <summary>
        /// shortcut keys configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // process Last button
            if (e.KeyCode.Equals(Keys.A))
            {
                button1.PerformClick();
                return;
            }
            // process Net button
            if (e.KeyCode.Equals(Keys.D))
            {
                button2.PerformClick();
                return;
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            info info_form = new info();
            info_form.Show();
            info_form.getPath(img_path[count]);
            info_form.imageInfo(img.Width, img.Height);
            //info_form.detectionInfo()

        }

    }
}
