using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLibrary.Model.HGA
{
    public class HGAPack : ObservableObject
    {
        public List<HGATray> HGATrays { get; set; }
        public string PackId { get; set; }

        public HGAPack(string packId)
        {
            HGATrays = new List<HGATray>();
            PackId = packId;
        }

        public void Add(HGATray hgaTray)
        {
            this.HGATrays.Add(hgaTray);
        }

        public void AddRange(IList<HGATray> hgaTrays)
        {
            foreach (HGATray ht in hgaTrays)
            {
                this.HGATrays.Add(ht);
            }
        }
    }
}
