using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using static FaceDetectionTool_WPF.Properties.Settings;
using System.Windows;
using System.Collections.Generic;

namespace FaceDetectionTool_WPF
{
    [Serializable]
    public class ImagePath
    {
        public string Name { get; set; }

        /// <summary>
        /// Image Path
        /// </summary>
        public string FrmImagePath { get; set; }

        /// <summary>
        /// List File 
        /// </summary>
        public string FrmList { get; set; }

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string FrmDetectionFrPath { get; set; }

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        public string FrmGtFrPath { get; set; }

        /// <summary>
        /// Get number of image
        /// </summary>
        /// <returns></returns>

        public List<ImageInfo> GetImageInfoList()
        {
            var txt = File.ReadAllLines(FrmList, Encoding.Default);
            var count = int.Parse(txt[0]);
            var list = new List<ImageInfo>(count);
            foreach (var line in txt.Skip(1).Take(count))
            {
                var imageInfo = new ImageInfo();
                imageInfo.Path = Path.Combine(FrmImagePath, line);
                var name = line.Substring(0, line.LastIndexOf('.'));
                imageInfo.RelativeImgPath = name;
                imageInfo.PathFr = Path.Combine(FrmDetectionFrPath, name + ".fr");
                imageInfo.PathGt = Path.Combine(FrmGtFrPath, name + ".fr");
                list.Add(imageInfo);
            }
            return list;
        }
    }
}
