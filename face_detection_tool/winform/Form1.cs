using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace face_detection_tool
{
    public partial class Form1 : Form
    {
        private IO io = new IO();
        private List<ImageInfo> image_info_list;
        private Evaluation eval = new Evaluation();
        private int index = 0;
        private Bitmap img;
        private Bitmap imgProposal;
        


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
            try
            {
                img = new Bitmap(image_info_list[index].ImgPath);
                pictureBox1.Image = img;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                imgProposal = new Bitmap(image_info_list[index].FaceProposalImgPath); 
                pictureBox2.Image = imgProposal;
                pictureBox2.Show();
                io.showFR(pictureBox1.Image, image_info_list[index]);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Configuration Form OK button trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void new_configuration_accept(object sender, EventArgs e)
        {
            Configuration new_configuration = (Configuration)sender;
            button1.Enabled = true;
            button2.Enabled = true;
            infoToolStripMenuItem.Enabled = true;
            evaluationToolStripMenuItem.Enabled = true;

            io = new_configuration.io;
            index = 0;
            image_info_list = io.getImageInfo();

            showImage();
            textBox4.Text = (index + 1).ToString();
            textBox5.Text = image_info_list.Count.ToString();

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

            textBox4.Text = (index + 1).ToString();
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

            textBox4.Text = (index + 1).ToString();
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
            double precision, recall;
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
            precision = Convert.ToDouble(TP) / Convert.ToDouble(P)*100;
            recall = Convert.ToDouble(TP) / Convert.ToDouble(T)*100;
            textBox1.Text = precision.ToString("0.00") + '%';
            textBox2.Text = recall.ToString("0.00") + '%';

            prToolStripMenuItem1.Enabled = true;
            printErrorLogToolStripMenuItem2.Enabled = true;
            textBox3.Text = "0.5";
        }

        private void prToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[] precision = new double[10001];
            double[] recall = new double[10001];
            int count = 0;
            PR pr_curve = new PR();
            for (double thr = 0.5; thr <= 1; thr = thr + (1 - 0.5) / 10000)
            {
                int TP = 0;
                int T = 0;
                int P = 0;
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
                    if (image_info_list[i].Scores == null)
                    {
                        eval.singleImageMatch(image_info_list[i], 0.5);
                    }

                    for (int j = 0; j < image_info_list[i].DetectFrList.Count; j++)
                    {
                        if (Convert.ToDouble(image_info_list[i].DetectFrList[j][4]) >= thr)
                        {
                            P++;
                            if (image_info_list[i].Scores[j] > 0.5)
                            {
                                TP++;
                            }
                        }
                    }

                    T += image_info_list[i].GtFrList.Count;
                }

                precision[count] = Convert.ToDouble(TP) / Convert.ToDouble(P);
                recall[count] = Convert.ToDouble(TP) / Convert.ToDouble(T);
                count++;
            }

            // draw Precision-Recall Curve
            Bitmap img = new Bitmap(pr_curve.pictureBox1.Width, pr_curve.pictureBox1.Height);
            pr_curve.pictureBox1.Image = img;
            pr_curve.Show();
            Graphics g = Graphics.FromImage(pr_curve.pictureBox1.Image);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, pr_curve.pictureBox1.Width, pr_curve.pictureBox1.Height));
            Pen pen = new Pen(Color.Blue, 2);
            for (int i = 0; i < precision.Length - 1; i++)
            {
                g = Graphics.FromImage(pr_curve.pictureBox1.Image);
                g.DrawLine(pen, Convert.ToSingle(recall[i]), Convert.ToSingle(precision[i]), Convert.ToSingle(recall[i + 1]), Convert.ToSingle(precision[i + 1]));
            }
        }

        private void printErrorLogToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int idx = trackBar1.Value;
            double thr = 0.5 + idx * 0.05;
            textBox3.Text = thr.ToString();

            double precision, recall;
            int TP = 0;
            int T = 0;
            int P = 0;
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
                if (image_info_list[i].Scores == null)
                {
                    eval.singleImageMatch(image_info_list[i], 0.5);
                }

                for (int j = 0; j < image_info_list[i].DetectFrList.Count; j++)
                {
                    if (Convert.ToDouble(image_info_list[i].DetectFrList[j][4]) >= thr)
                    {
                        P++;
                        if (image_info_list[i].Scores[j] > 0.5)
                        {
                            TP++;
                        }
                    }
                }

                T += image_info_list[i].GtFrList.Count;
            }

            precision = Convert.ToDouble(TP) / Convert.ToDouble(P)*100;
            recall = Convert.ToDouble(TP) / Convert.ToDouble(T)*100;

            textBox1.Text = precision.ToString("0.00") + '%';
            textBox2.Text = recall.ToString("0.00") + '%';
        }

        /// <summary>
        /// Generate images including face and non-face patch to construct the validation set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateValidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenValidForm gen_valid_form = new GenValidForm();
            gen_valid_form.Show();
            gen_valid_form.accept += new EventHandler(gen_valid_form_accept);
        }

        private void gen_valid_form_accept(object sender, EventArgs e)
        {
            // get information of validation set
            string valid_image_save_path;
            string valid_image_list;
            GenValidForm gen_valid_form = (GenValidForm)sender;
            valid_image_save_path = gen_valid_form.image_save_path;
            valid_image_list = gen_valid_form.list_filename;
            gen_valid_form.Close();

            // 
            process data_processing = new process();
            data_processing.Show();
            data_processing.progressBar1.Value = 0;
            data_processing.progressBar1.Maximum = image_info_list.Count;

            // crop and save images
            double xtl, ytl,  width, height;
            Bitmap original_image;
            string list_path = valid_image_save_path + "\\" + valid_image_list+".txt";
            FileStream list = new FileStream(list_path, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(list);

            for (int i = 0; i < image_info_list.Count; i++)
            {
                data_processing.progressBar1.Value++;
                if (image_info_list[i].DetectFrList == null)
                {
                    image_info_list[i].DetectFrList = io.getFrList(image_info_list[i].DetectFrPath);
                }
                if (image_info_list[i].GtFrList == null)
                {
                    image_info_list[i].GtFrList = io.getFrList(image_info_list[i].GtFrPath);
                }

                eval.singleImageMatch(image_info_list[i], 0.05);

                original_image = new Bitmap(image_info_list[i].ImgPath);
                string save_path;
                string save_path_name;

                // positive sample
                for (int j = 0; j < image_info_list[i].GtFrList.Count; j++)
                {
                    if (image_info_list[i].GtFrList[j].Length == 5)
                    {
                        double a, b, w;
                        a = image_info_list[i].GtFrList[j][0];
                        b = image_info_list[i].GtFrList[j][1];
                        w = image_info_list[i].GtFrList[j][2];
                        /*
                        xtl = image_info_list[i].GtFrList[j][3] - image_info_list[i].GtFrList[j][0] * 0.8;
                        ytl = image_info_list[i].GtFrList[j][4] - image_info_list[i].GtFrList[j][0] * 0.8;
                        width = image_info_list[i].GtFrList[j][0]  * 2*0.8;
                        height = image_info_list[i].GtFrList[j][0]  * 2*0.8;*/
                        width = 2 * Math.Sqrt(Math.Pow(a, 2) + (Math.Pow(b, 2) - Math.Pow(a, 2)) * Math.Pow(Math.Sin(w), 2));
                        height = 2 * Math.Sqrt(Math.Pow(a, 2) + (Math.Pow(b, 2) - Math.Pow(a, 2)) * Math.Pow(Math.Cos(w), 2));
                        xtl = image_info_list[i].GtFrList[j][3] - 0.5 * width;
                        ytl = image_info_list[i].GtFrList[j][4] - 0.5 * height;
                    }
                    else
                    {
                        xtl = image_info_list[i].GtFrList[j][0];
                        ytl = image_info_list[i].GtFrList[j][1];
                        width = image_info_list[i].GtFrList[j][2] - xtl + 1;
                        height = image_info_list[i].GtFrList[j][3] - ytl + 1;
                    }
                    Rectangle rect = new Rectangle(Convert.ToInt32(xtl), Convert.ToInt32(ytl), Convert.ToInt32(width), Convert.ToInt32(height));
                    Bitmap crop = crop_image(original_image, rect);
                    string line = "pos\\" + image_info_list[i].RelativeImgPath + '_' + j.ToString() + ".jpg";

                    save_path_name = Path.Combine(valid_image_save_path+"\\", line);
                    save_path = Path.GetDirectoryName(save_path_name).ToString();
                    if (Directory.Exists(save_path) == false)
                    {
                        Directory.CreateDirectory(save_path);
                    }
                    crop.Save(save_path_name, ImageFormat.Jpeg);
                    sw.WriteLine(line.Replace('\\', '/') + " 1");
                }

                // negative samples
                for (int j = 0; j < image_info_list[i].DetectFrList.Count; j++)
                {
                    if (image_info_list[i].Scores[j] < 0.05)
                    {
                        xtl = image_info_list[i].DetectFrList[j][0];
                        ytl = image_info_list[i].DetectFrList[j][1];
                        width = image_info_list[i].DetectFrList[j][2] - xtl + 1;
                        height = image_info_list[i].DetectFrList[j][3] - ytl + 1;

                        Rectangle rect = new Rectangle(Convert.ToInt32(xtl), Convert.ToInt32(ytl), Convert.ToInt32(width), Convert.ToInt32(height));
                        Bitmap crop = crop_image(original_image, rect);
                        string line = "neg\\" + image_info_list[i].RelativeImgPath + '_' + j.ToString() + ".jpg";

                        save_path_name = Path.Combine(valid_image_save_path + "\\", line);
                        save_path = Path.GetDirectoryName(save_path_name).ToString();
                        if (Directory.Exists(save_path) == false)
                        {
                            Directory.CreateDirectory(save_path);
                        }
                        crop.Save(save_path_name, ImageFormat.Jpeg);
                        sw.WriteLine(line.Replace('\\', '/') + " 0");
                    }
                }
                original_image.Dispose();
            }

            data_processing.Close();
            sw.Close();
            list.Close();
        }

        /// <summary>
        ///     Crop patch from image based on rectangle
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Bitmap crop_image(Image image, Rectangle rect)
        {
            Bitmap crop = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(crop);
            g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
            g.Dispose();
            return crop;
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            index = Convert.ToInt32(textBox4.Text);
            index = index - 1;
            showImage();
        }
        
    }


}
