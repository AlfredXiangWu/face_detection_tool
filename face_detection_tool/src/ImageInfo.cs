using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace face_detection_tool
{
    public class ImageInfo
    {
        /// <summary>
        /// Image Path
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string DetectFrPath { get; set; }

        /// <summary>
        /// Detection FR Result
        /// </summary>
        public List<double[]> DetecFrList { get; set; }

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        public string GtFrPath { get; set; }

        /// <summary>
        /// Ground Truth Fr Result
        /// </summary>
        public List<double[]> GtFrList { get; set; }

    }
}
