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
        private Evaluation eval = new Evaluation();
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

            try
            {
                io.showFR(pictureBox1.Image, image_info_list[index]);
            }
            catch
            {
                return;
            }
        }


        void new_configuration_accept(object sender, EventArgs e)
        {
            Configuration new_configuration = (Configuration)sender;
            button1.Enabled = true;
            button2.Enabled = true;
            infoToolStripMenuItem.Enabled = true;
            evaluationToolStripMenuItem.Enabled = true;

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
            if (e.KeyCode.Equals(Keys.I))
            {
                infoToolStripMenuItem.PerformClick();
                return;
            }
        }

        /// <summary>
        /// details of face detection image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            info info_form = new info();
            int tp, fp;
            info_form.Show();
            info_form.getPath(image_info_list[index].ImgPath);
            info_form.imageInfo(img.Width, img.Height);
            info_form.detectionInfo(image_info_list[index].DetectFrList.Count, image_info_list[index].GtFrList.Count);

            tp = eval.singleImageMatch(image_info_list[index], 0.5);
            fp = image_info_list[index].DetectFrList.Count - tp;
            info_form.evaluationInfo(tp, fp);
        }

        /// <summary>
        /// evaluation for dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evaluateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int TP = 0;
            int T = 0;
            int P = 0;
            double percision, recall;
            process data_processing = new process();
            data_processing.Show();
            data_processing.progressBar1.Value = 0;
            data_processing.progressBar1.Maximum = image_info_list.Count;

            for (int i = 0; i < image_info_list.Count; i++)
            {
                if (image_info_list[i].DetectFrList == null)
                {
                    image_info_list[i].DetectFrList = io.getFrList(image_info_list[i].DetectFrPath);
                }
                if (image_info_list[i].GtFrList == null)
                {
                    image_info_list[i].GtFrList = io.getFrList(image_info_list[i].GtFrPath);
                }
                int tp = eval.singleImageMatch(image_info_list[i], 0.5);
                TP += tp;
                T += image_info_list[i].GtFrList.Count;
                P += image_info_list[i].DetectFrList.Count;
                data_processing.progressBar1.Value++;
            }
            data_processing.Close();
            percision = Convert.ToDouble(TP) / Convert.ToDouble(P)*100;
            recall = Convert.ToDouble(TP) / Convert.ToDouble(T)*100;
            textBox1.Text = percision.ToString("0.00") + '%';
            textBox2.Text = recall.ToString("0.00") + '%';
        }
    }
}
