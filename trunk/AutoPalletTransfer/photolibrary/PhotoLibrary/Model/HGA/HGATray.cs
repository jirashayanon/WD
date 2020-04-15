using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PhotoLibrary.Model.HGA
{
    public class HGATray : ObservableObject
    {
        public const double TRAY20_PHOTO_LEFTOFFSET = 0.068984;
        public const double TRAY40_PHOTO_LEFTOFFSET = 0.02;
        public const double TRAY60_PHOTO_LEFTOFFSET = 0.02;

        public int TrayInt { get; set; }
        public int TrayInt2 { get; set; }

        public int TraySize { get; set; }

        public List<HGAItem> HGAItems { get; set; }
        public string TrayId { get; set; }
        public string PackId { get; set; }
        public List<string> palletIds { get; set; }
        public bool HasAQHeader { get; set; }
        public int IsComplete { get; set; }

        /// <summary>
        /// Current row in tray. For tray 60, it will be 1-6. (Same as PalletOrder)
        /// For tray 20, it can show all and set this value to -1.
        /// </summary>
        public int CurrentRowColumnInTray { get; set; }

        public ICollectionView ItemsView
        {
            get
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(HGAItems);
                view.Filter = new Predicate<object>(o => FilterHGA(o as HGAItem));
                return view;
            }
        }

        public bool FilterHGA(HGAItem item)
        {
            return item != null && (
                CurrentRowColumnInTray == -1 ||
                item.PalletOrder == CurrentRowColumnInTray);
        }

        #region AQ Header
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }

        public string Date { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string UsedTime { get; set; }
        public string TesterNumber { get; set; }
        public string Customer { get; set; }

        private string _lotNumber;
        public string LotNumber
        {
            get
            {
                return _lotNumber;
            }
            set
            {
                if (_lotNumber == value)
                    return;
                _lotNumber = value;
                RaisePropertyChanged("LotNumber");
            }
        }

        public string User { get; set; }

        private string _product;
        public string Product
        {
            get
            {
                return _product;
            }
            set
            {
                if (_product == value)
                    return;
                _product = value;
                RaisePropertyChanged("Product");
            }
        }

        public string DocControl1 { get; set; }
        public string DocControl2 { get; set; }
        public string Sus { get; set; }
        public string AssyLine { get; set; }
        #endregion

        public HGATray(string trayId, string packId = null, int isComplete = 0, int traySize = 20)
        {
            HGAItems = new List<HGAItem>();
            palletIds = new List<string>();
            TrayId = trayId;
            PackId = packId;
            IsComplete = isComplete;
            LotNumber = "";
            Product = "";

            TraySize = traySize;
            if (TraySize == 20)
            {
                CurrentRowColumnInTray = -1;
            }
            else
            {
                CurrentRowColumnInTray = 1;
            }
        }

        public void Add(HGAItem hgaItem)
        {
            foreach (HGAItem hi in this.HGAItems)
            {
                if (hi.HGAId == hgaItem.HGAId) return;
            }
            this.HGAItems.Add(hgaItem);
        }

        public void AddRange(IList<HGAItem> hgaItems)
        {
            foreach (HGAItem hi in hgaItems)
            {
                this.HGAItems.Add(hi);
            }
        }

        /// <summary>
        /// Create header string from Tray property.
        /// </summary>
        /// <returns></returns>
        public string GetAQTrayHeader()
        {
            string aqtrayHeader;
            aqtrayHeader = string.Format("[INFORMATION]") + Environment.NewLine;
            //this.Date = DateTime.Now.ToString("M/d/yyyy");
            //this.TimeStart = DateTime.Now.ToString("h:mm:ss tt");
            //this.TimeEnd = DateTime.Now.ToString("h:mm:ss tt");
            string date = this.DateTimeEnd.ToString("M/d/yyyy");
            string timeEnd = this.DateTimeEnd.ToString("h:mm:ss tt");
            string timeStart = ""; //DateTimeStart.Date == DateTimeEnd.Date ? this.DateTimeStart.ToString("h:mm:ss tt") : this.DateTimeEnd.ToString("h:mm:ss tt");
            string usedTime = "";

            TimeSpan diffTime = DateTimeEnd - DateTimeStart;
            if (DateTimeStart.Date == DateTimeEnd.Date)
            {
                timeStart = this.DateTimeStart.ToString("h:mm:ss tt");
                usedTime = diffTime.Minutes.ToString("00") + ":" + diffTime.Seconds.ToString("00");
            }
            else
            {
                timeStart = this.DateTimeEnd.ToString("h:mm:ss tt");
                usedTime = "00:00";
            }
            aqtrayHeader += string.Format("DATE={0}", date) + Environment.NewLine;
            aqtrayHeader += string.Format("TIME START={0}", timeStart) + Environment.NewLine;
            aqtrayHeader += string.Format("TIME END={0}", timeEnd) + Environment.NewLine;
            aqtrayHeader += string.Format("USED TIME={0}", usedTime) + Environment.NewLine;
            aqtrayHeader += string.Format("TESTER NUMBER={0}", this.TesterNumber) + Environment.NewLine;
            aqtrayHeader += string.Format("CUSTOMER={0}", this.Customer) + Environment.NewLine;
            aqtrayHeader += string.Format("PRODUCT={0}", this.Product) + Environment.NewLine;
            aqtrayHeader += string.Format("USER={0}", this.User) + Environment.NewLine;
            aqtrayHeader += string.Format("TRAYID={0}", this.TrayId) + Environment.NewLine;
            aqtrayHeader += string.Format("LOTNUMBER={0}", this.LotNumber) + Environment.NewLine;
            aqtrayHeader += string.Format("DOCCONTROL1={0}", this.DocControl1) + Environment.NewLine;
            aqtrayHeader += string.Format("DOCCONTROL2={0}", this.DocControl2) + Environment.NewLine;
            aqtrayHeader += string.Format("SUS={0}", this.Sus) + Environment.NewLine;
            aqtrayHeader += string.Format("ASSYLINE={0}", this.AssyLine) + Environment.NewLine;
            aqtrayHeader += Environment.NewLine;
            aqtrayHeader += Environment.NewLine;
            aqtrayHeader += string.Format("[SERIAL]") + Environment.NewLine;

            return aqtrayHeader;
        }
    }
}
