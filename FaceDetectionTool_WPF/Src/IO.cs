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
    public class IO
    {
        /// <summary>
        /// Image Path
        /// </summary>
        public string FrmImagePath { get; set; } = Default.ImagePath;

        /// <summary>
        /// List File 
        /// </summary>
        public string FrmList { get; set; } = Default.List;

        /// <summary>
        /// Detection FR Path
        /// </summary>
        public string FrmDetectionFrPath { get; set; } = Default.DetectionFrPath;

        /// <summary>
        /// Ground Truth FR Path
        /// </summary>
        public string FrmGtFrPath { get; set; } = Default.GtFrPath;

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
                var name = line.Split('.')[0];
                imageInfo.PathFr = Path.Combine(FrmDetectionFrPath, name + ".jpg.fr");
                imageInfo.PathGt = Path.Combine(FrmGtFrPath, name + ".fr");

                imageInfo.FrList = GetInfoList(imageInfo.PathFr);
                imageInfo.GtList = GetInfoList(imageInfo.PathGt);

                list.Add(imageInfo);
            }
            return list;
        }

        private List<double[]> GetInfoList(string path)
        {
            var txt = File.ReadAllLines(path);
            var count = int.Parse(txt[0]);
            var lines = txt.Skip(1).Take(count);
            return lines.Select(l =>
                l.Replace(' ', '\t').Split('\t')
                .Select(s => double.Parse(s)).ToArray()).ToList();
        }
    }
}
