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
    public partial class GenValidForm : Form
    {
        public GenValidForm()
        {
            InitializeComponent();
        }
        public string image_save_path { get; set; }
        public string list_filename { get; set; }
        public event EventHandler accept;

        private void button1_Click(object sender, EventArgs e)
        {
            image_save_path = textBox1.Text;
            list_filename = textBox2.Text;

            if (accept != null)
            {
                accept(this, EventArgs.Empty);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Image File Path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
            }
        }
    }
}
