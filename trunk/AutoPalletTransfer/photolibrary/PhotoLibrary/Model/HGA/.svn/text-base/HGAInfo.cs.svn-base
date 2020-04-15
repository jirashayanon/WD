using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Model.HGA
{
    public class HGAInfo
    {
        public long HgainfoId { get; set; }
        public string lineNo { get; set; }
        public string PalletId { get; set; }
        public int Position { get; set; }
        public DateTime ProcessDateTime { get; set; }
        public string defect { get; set; }
        public string SerialNumber { get; set; }

        public List<HGAViewData> HGAViews { get; set; }

        public HGAInfo(long hGAId, string trayId, int position, string serialNumber)
        {
            HgainfoId = hGAId;
            PalletId = trayId;
            Position = position;
            SerialNumber = serialNumber;
            //Status = status;
            
            HGAViews = new List<HGAViewData>();
        }
    }
}
