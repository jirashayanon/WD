using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PhotoLibrary.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private string _version;
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version == value)
                    return;
                _version = value;
                RaisePropertyChanged("Version");
            }
        }

        private string _date;
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date == value)
                    return;
                _date = value;
                RaisePropertyChanged("Date");
            }
        }

        public AboutViewModel()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
                            TimeSpan.TicksPerDay * version.Build +      // days since 1 January 2000
                            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)
            Date = buildDateTime.ToString("dd MMMM yyyy HH:mm tt");
        }
    }
}