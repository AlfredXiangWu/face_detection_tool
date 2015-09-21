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

        public IO io = new IO();

        public Configuration()
        {
            InitializeComponent();  
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
        /// Select Detection FR Path
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

        public event EventHandler accept;
        /// <summary>
        /// OK
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            io.FrmImagePath = textBox2.Text;
            io.FrmList = textBox3.Text;
            io.FrmDetectionFrPath = textBox5.Text;
            io.FrmGtFrPath = textBox7.Text;
            io.FrmFaceProposalPath = textBox9.Text;

            if (accept != null)
            {
                accept(this, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Image File Path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = dialog.SelectedPath;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "AFW")
            {
                this.textBox2.Text = "Z:\\User\\wuxiang\\data\\AFW\\testimages";
                this.textBox3.Text = "Z:\\User\\wuxiang\\data\\AFW\\list_afw.txt";
                this.textBox5.Text = "Z:\\User\\wuxiang\\Result\\face_detection\\DeepDetector\\DeepDetector0.3\\DeepDetector0.3.11.1\\result\\AFW";
                this.textBox7.Text = "Z:\\User\\wuxiang\\data\\AFW\\gt";
            }
            else if (comboBox1.Text == "FDDB")
            {
                this.textBox2.Text = "Z:\\User\\wuxiang\\data\\FDDB\\originalPics";
                this.textBox3.Text = "Z:\\User\\wuxiang\\data\\FDDB\\FDDB_list.txt";
                this.textBox5.Text = "Z:\\User\\wuxiang\\Result\\face_detection\\DeepDetector\\DeepDetector0.3\\DeepDetector0.3.11.1\\result\\FDDB";
                this.textBox7.Text = "Z:\\User\\wuxiang\\data\\FDDB\\gt";
            }
            else
            {
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox5.Text = "";
                this.textBox7.Text = "";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Image File Path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox9.Text = dialog.SelectedPath;
            }
        }

    }
}
