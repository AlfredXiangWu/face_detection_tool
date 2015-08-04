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
    public partial class info : Form
    {
        public info()
        {
            InitializeComponent();
        }

        public void getPath(string path)
        {
            textBox1.Text = path;
        }

        public void imageInfo(int width, int height)
        {
            textBox2.Text = width.ToString();
            textBox3.Text = height.ToString();
        }

        public void detectionInfo(int detection, int gt)
        {
            textBox6.Text = detection.ToString();
            textBox5.Text = gt.ToString();
        }

        public void evaluationInfo(int tp, int fp)
        {
            textBox7.Text = tp.ToString();
            textBox4.Text = fp.ToString();
        }

        private void info_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Escape))
            {
                this.Close();
            }
        }

    }
}
