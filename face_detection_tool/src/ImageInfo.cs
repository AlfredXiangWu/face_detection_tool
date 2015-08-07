using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace face_detection_tool
{
    public class ImageInfo
    {
        /// <summary>
        /// Absolute Image Path
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// Relative Image Path
        /// </summary>
        public string RelativeImgPath { get; set; }

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string DetectFrPath { get; set; }

        /// <summary>
        /// Detection FR Result
        /// </summary>
        public List<double[]> DetectFrList { get; set; }

        /// <summary>
        /// IoU Scores for Detection Results
        /// </summary>
        public double[] Scores { get; set; }
        public void InitScores()
        {
            Scores = new double[DetectFrList.Count];
            for (int i = 0; i < DetectFrList.Count; i++)
            {
                Scores[i] = 0;
            }
        }

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
