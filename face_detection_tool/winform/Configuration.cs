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
            _image_path_ = textBox2.Text;
            _list_ = textBox3.Text;
            _detection_fr_path_ = textBox5.Text;
            _gt_fr_path_ = textBox7.Text;

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

    }
}
