using GalaSoft.MvvmLight;
using PhotoLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Model
{
    public class Sampling : ObservableObject, PhotoLibrary.Model.ISampling
    {
        #region Properties Sampling

        private int _totalSampling;
        public int TotalSampling
        {
            get
            {
                return _totalSampling;
            }
            set
            {
                if (_totalSampling == value)
                    return;
                _totalSampling = value;
                RaisePropertyChanged("TotalSampling");
            }
        }

        private int _samplingRate;
        public int SamplingRate
        {
            get
            {
                return _samplingRate;
            }
            set
            {
                if (_samplingRate == value)
                    return;
                _samplingRate = value;
                RaisePropertyChanged("SamplingRate");
            }
        }

        private int _samplingStartRate;
        public int SamplingStartRate
        {
            get
            {
                return _samplingStartRate;
            }
            set
            {
                if (_samplingStartRate == value)
                    return;
                _samplingStartRate = value;
                //RaisePropertyChanged("SamplingStartRate");
            }
        }


        private string _samplingText;
        public string SamplingText
        {
            get
            {
                return _samplingText;
            }
            set
            {
                if (_samplingText == value)
                    return;
                _samplingText = value;
                RaisePropertyChanged("SamplingText");
            }
        }

        private bool _isSampling;
        public bool IsSampling
        {
            get
            {
                return _isSampling;
            }
            set
            {
                if (_isSampling == value)
                    return;
                _isSampling = value;

                SamplingText = _isSampling ? "Yes" : "No";
                RaisePropertyChanged("IsSampling");
            }
        }

        private int _current;
        public int Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current == value)
                    return;
                _current = value;
                RaisePropertyChanged("Current");
                RaisePropertyChanged("CountTray");
            }
        }

        public string Plan
        {
            get
            {
                //return this.SamplingRate + " / " + this.TotalSampling;
                return this.SamplingStartRate + " / " + this.TotalSampling;
            }
        }

        public string CountTray
        {
            get
            {
                return this.Current + " / " + this.TotalSampling;
            }
        }
        #endregion

        public Sampling()
        {
            ConfigXML config = new ConfigXML();
            TotalSampling = config.SamplingPlanTotal;
            SamplingStartRate = config.SamplingPlan;
            //SamplingRate = config.SamplingPlan;
            SamplingRate = TotalSampling;

            Current = 1;

            SamplingText = "No";
            SamplingNow();
        }

        public bool SamplingNow()
        {
            if (Current % (TotalSampling / SamplingRate) == 0)
            {
                IsSampling = true;
            }
            else IsSampling = false;
            return IsSampling;
        }

        public void Next()
        {
            Current += 1;
            if (Current > TotalSampling)
            {
                Current = 1;
            }

            IsSampling = false;
        }
    }
}
