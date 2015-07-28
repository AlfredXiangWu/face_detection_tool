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
        /// <summary>
        /// Image Path
        /// </summary>
        private string _image_path_;
        public string frmImagePath
        {
            get {return _image_path_;}
            set {_image_path_ = value;}
        }

        /// <summary>
        /// List File 
        /// </summary>
        private string _list_;
        public string frmList
        {
            get { return _list_; }
            set { _list_ = value;}
        }

        /// <summary>
        /// FR Path
        /// </summary>
        private string _fr_path_;
        public string frmFrPath
        {
            get { return _fr_path_; }
            set { _fr_path_ = value; }
        }


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
            _image_path_ = textBox2.Text;
            _list_ = textBox3.Text;
            _fr_path_ = textBox5.Text;
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
