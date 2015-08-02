using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionTool_WPF
{
    public class ImageInfo
    {
        public string Path { get; set; }

        public string PathGt { get; set; }

        public List<double[]> GtList { get; set; }

        public string PathFr { get; set; }

        public List<double[]> FrList { get; set; }
    }
}
