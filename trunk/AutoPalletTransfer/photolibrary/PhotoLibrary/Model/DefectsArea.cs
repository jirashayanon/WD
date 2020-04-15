using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoLibrary.Model
{
    public sealed class DefectsArea : ObservableObject
    {
        #region Properties

        private Point _positionRatio;
        public Point PositionRatio
        {
            get
            {
                return _positionRatio;
            }
            set
            {
                if (_positionRatio == value)
                    return;
                _positionRatio = value;
                RaisePropertyChanged("PositionRatio");
            }
        }

        private ObservableCollection<Defect> _listDefects;
        public ObservableCollection<Defect> ListDefects
        {
            get
            {
                return _listDefects;
            }
            set
            {
                if (_listDefects == value)
                    return;
                _listDefects = value;
                RaisePropertyChanged("ListDefects");
            }
        }

        private string _color;
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color == value)
                    return;
                _color = value;
                RaisePropertyChanged("Color");
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        #endregion

        static Random rnd = new Random();

        public DefectsArea(Point positionRatio, List<Defect> listDefects)
        {
            PositionRatio = positionRatio;
            ListDefects = new ObservableCollection<Defect>();
            foreach (Defect defect in listDefects)
            {
                ListDefects.Add(defect);
            }

            IsVisible = false;
            //Color = RandomColor();
            Color = "Red";
        }

        public string RandomColor()
        {
            List<string> colors = new List<string>() { "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", "#FA8072" };
            int r = rnd.Next(colors.Count);
            string s = colors[r];
            System.Diagnostics.Debug.WriteLine(s);
            return s;
        }
    }
}
