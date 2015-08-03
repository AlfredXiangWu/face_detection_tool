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
        private IO io = new IO();
        private List<ImageInfo> image_info_list;
        private int index = 0;
        private Bitmap img;

        public Form1()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        /// <summary>
        /// New Configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration new_configuration = new Configuration();
            new_configuration.Show();
            new_configuration.accept += new EventHandler(new_configuration_accept);
        }

        /// <summary>
        /// Show image in picturebox1
        /// </summary>
        private void showImage()
        {
            img = new Bitmap(image_info_list[index].ImgPath);
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            io.showFR(pictureBox1.Image, image_info_list[index]);
        }


        void new_configuration_accept(object sender, EventArgs e)
        {
            Configuration new_configuration = (Configuration)sender;
            button1.Enabled = true;
            button2.Enabled = true;
            infoToolStripMenuItem.Enabled = true;

            io = new_configuration.io;
            image_info_list = io.getImageInfo();

            showImage();

             new_configuration.Hide();
        }

        /// <summary>
        /// Next button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (index == 0)
            {
                index = image_info_list.Count;
            }
            index = index - 1;

            showImage();
        }

        /// <summary>
        /// Last button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (index == image_info_list.Count - 1)
            {
                index = -1;
            }
            index = index + 1;

            showImage();
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

        /// <summary>
        /// detials of face detection image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            info info_form = new info();
            info_form.Show();
            info_form.getPath(image_info_list[index].ImgPath);
            info_form.imageInfo(img.Width, img.Height);
            info_form.detectionInfo(image_info_list[index].DetecFrList.Count, image_info_list[index].GtFrList.Count);
        }
    }
}
