using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace face_detection_tool
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
            textBox2.Text = "Z:\\User\\wuxiang\\data\\AFW\\testimages";
            textBox3.Text = "Z:\\User\\wuxiang\\data\\AFW\\list_afw.txt";
            textBox5.Text = "Z:\\User\\wuxiang\\Result\\face_detection\\Deep_Detector0.1.11.1\\result\\AFW";
        }

        /// <summary>
        /// Select Image File Path
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Image File Path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Select Image List File
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Select FR Path
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Image File Path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// OK
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        



    }
}
