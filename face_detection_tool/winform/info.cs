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
            textBox5.Text = detection.ToString();
            textBox6.Text = gt.ToString();
        }
    }
}
