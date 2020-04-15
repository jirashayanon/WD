using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoLibrary.ViewModel
{
    public class StartOQAViewModel : ViewModelBase
    {
        private ENUM.OPTIONS _option;
        public ENUM.OPTIONS Option
        {
            get
            {
                return _option;
            }
            set
            {
                if (_option == value)
                    return;
                _option = value;

                RaisePropertyChanged("Option");
                if (_option == ENUM.OPTIONS.OQA)
                {
                    Detail = ENUM.DETAILS.Auto;

                    IsAutoEnabled = true;
                    IsPackEnabled = true;
                    IsTrayEnabled = false;
                    IsPalletEnabled = false;
                    IsHGAEnabled = false;
                }
                else if (_option == ENUM.OPTIONS.Rescreen)
                {
                    Detail = ENUM.DETAILS.Auto;

                    IsAutoEnabled = true;
                    IsPackEnabled = true;
                    IsTrayEnabled = true;
                    IsPalletEnabled = true;
                    IsHGAEnabled = true;
                }
                else
                {
                    Detail = ENUM.DETAILS.Pack;

                    IsAutoEnabled = false;
                    IsPackEnabled = true;
                    IsTrayEnabled = true;
                    IsPalletEnabled = true;
                    IsHGAEnabled = true;
                }
            }
        }

        private ENUM.DETAILS _detail { get; set; }
        public ENUM.DETAILS Detail
        {
            get
            {
                return _detail;
            }
            set
            {
                if (_detail == value)
                    return;
                _detail = value;
                RaisePropertyChanged("Detail");
            }
        }

        public string PackId { get; set; }
        public string TrayId { get; set; }
        public string HGAId { get; set; }
        public string PalletId { get; set; }

        private bool _isAutoEnabled;
        private bool _isPackEnabled;
        private bool _isTrayEnabled;
        private bool _isPalletEnabled;
        private bool _isHGAEnabled;

        public bool IsAutoEnabled
        {
            get
            {
                return _isAutoEnabled;
            }
            set
            {
                if (_isAutoEnabled == value)
                    return;
                _isAutoEnabled = value;
                RaisePropertyChanged("IsAutoEnabled");
            }
        }
        public bool IsPackEnabled
        {
            get
            {
                return _isPackEnabled;
            }
            set
            {
                if (_isPackEnabled == value)
                    return;
                _isPackEnabled = value;
                RaisePropertyChanged("IsPackEnabled");
            }
        }
        public bool IsTrayEnabled
        {
            get
            {
                return _isTrayEnabled;
            }
            set
            {
                if (_isTrayEnabled == value)
                    return;
                _isTrayEnabled = value;
                RaisePropertyChanged("IsTrayEnabled");
            }
        }

        public bool IsPalletEnabled
        {
            get
            {
                return _isPalletEnabled;
            }
            set
            {
                if (_isPalletEnabled == value)
                    return;
                _isPalletEnabled = value;
                RaisePropertyChanged("IsPalletEnabled");
            }
        }
        public bool IsHGAEnabled
        {
            get
            {
                return _isHGAEnabled;
            }
            set
            {
                if (_isHGAEnabled == value)
                    return;
                _isHGAEnabled = value;
                RaisePropertyChanged("IsHGAEnabled");
            }
        }

        public ICommand SubmitCommand { get; private set; }

        public event EventHandler FinishProcess;
        public event EventHandler ErrorInputMissing;

        public StartOQAViewModel()
        {
            Option = ENUM.OPTIONS.OQA;
            Detail = ENUM.DETAILS.Tray;

            //IsAutoEnabled = true;
            //IsPackEnabled = true;
            //IsTrayEnabled = false;

            IsAutoEnabled = false;
            IsPackEnabled = false;
            IsTrayEnabled = true;
            IsPalletEnabled = true;
            IsHGAEnabled = false;

            SubmitCommand = new RelayCommand(() => ExecuteSubmitCommand());
        }

        public string Id;

        public void ExecuteSubmitCommand()
        {
            // TODO: Do something
            if (Detail == ENUM.DETAILS.Auto)
            {
                Id = "";
            }
            else if (Detail == ENUM.DETAILS.Pack)
            {
                Id = PackId;
            }
            else if (Detail == ENUM.DETAILS.Tray)
            {
                Id = TrayId;
            }
            else if (Detail == ENUM.DETAILS.Pallet)
            {
                Id = PalletId;
            }
            else // DETAILS.HGA
            {
                Id = HGAId;
            }

            if (Detail == ENUM.DETAILS.Auto || (Id != null && Id.Length > 0))
            {
                if (FinishProcess != null)
                {
                    FinishProcess(this, EventArgs.Empty);
                }
            }
            else
            {
                if (ErrorInputMissing != null)
                {
                    ErrorInputMissing(this, EventArgs.Empty);
                }
            }

        }
    }
}
