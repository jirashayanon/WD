using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace PhotoLibrary.Model
{
    public class ImageWithDefect : ObservableObject, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public BitmapImage ImageThumbnail { get; set; }
        private bool _isDefect;
        public bool IsDefect
        {
            get
            {
                return _isDefect;
            }
            set
            {
                if (_isDefect == value)
                    return;
                _isDefect = value;
                OnPropertyChanged("IsDefect");
            }
        }
        public string ImagePath { get; set; }
        private BitmapImage _image = null;
        public BitmapImage Image
        {
            get
            {
                //if (_image == null)
                //{
                //    _image = new BitmapImage();
                //    _image.BeginInit();
                //    _image.UriSource = new Uri(ImagePath, UriKind.RelativeOrAbsolute);
                //    //bi.DecodePixelWidth = imageWidth;     // At least, should be 1024
                //    _image.EndInit();
                //    _image.Freeze();
                //}
                return _image;
            }
            set
            {
                if (_image == value)
                    return;
                _image = value;
            }
        }

        public int ViewId { get; set; }
        public string View { get; set; }
        public ObservableCollection<DefectsArea> ItemsDefects { get; set; }
        public ObservableCollection<string> ItemsDefectsStringUnique { get; set; }

        public long InspectionDataId { get; set; }

        public ImageWithDefect()
        {
            ItemsDefects = new ObservableCollection<DefectsArea>();
            ItemsDefectsStringUnique = new ObservableCollection<string>();
        }

        public void AddToItemsDefects(DefectsArea defect)
        {
            if (defect.ListDefects.Count == 0)
            {
                return;
            }
            IsDefect = true;
            ItemsDefects.Add(defect);
            foreach (var xx in defect.ListDefects.Where(d => !ItemsDefectsStringUnique.Contains(d.Name)))
            {
                ItemsDefectsStringUnique.Add(xx.Name);
            }
        }

        public void ClearItemsDefects()
        {
            IsDefect = false;
            ItemsDefects.Clear();
            ItemsDefectsStringUnique.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public List<string> GetListDefectEachArea()
        {
            List<string> strDefectEachArea = new List<string>();
            foreach (DefectsArea da in this.ItemsDefects)
            {
                strDefectEachArea.AddRange(da.ListDefects.ToList().ConvertAll<string>(s => s.Name));
            }
            return strDefectEachArea;
        }

        public List<string> GetListDefectData()
        {
            List<string> strDefectData = new List<string>();
            foreach (DefectsArea da in this.ItemsDefects)
            {
                string tempStr = string.Join("-", da.ListDefects.ToList().ConvertAll<string>(s => s.Name));
                tempStr += "+" + da.PositionRatio.X.ToString("0.####") + ":" + da.PositionRatio.Y.ToString("0.####");

                strDefectData.Add(tempStr);
            }
            return strDefectData;
        }
    }
}
