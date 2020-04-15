using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLibrary.Model.HGA
{
    public class HGADefect
    {
        public long InspectionDataId { get; set; }
        public int ViewId { get; set; }
        public string View { get; set; }
        public int Hgainfo_view_id { get; set; }
        public string Defect { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }
}
