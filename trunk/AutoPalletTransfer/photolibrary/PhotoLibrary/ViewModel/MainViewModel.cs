using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PhotoLibrary.Repository;
using PhotoLibrary.Helpers;
using PhotoLibrary.Model;
using PhotoLibrary.Model.HGA;
using ReadWriteCsv;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using PhotoLibrary.Views;
using System.Windows.Controls;

namespace PhotoLibrary.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public sealed class MainViewModel : ViewModelBase
    {
        public event EventHandler ErrorProcess;
        public event EventHandler FileUsedByAnotherProcess;
        public event EventHandler FinishProcess;
        public event EventHandler ChangeToolDefectEvent;
        public event EventHandler DBConnectionFail;
        public event EventHandler ImageNotFound;
        public event EventHandler NoCurrentOrderIdInCsv;

        public event EventHandler ChangeToTray20Event;
        public event EventHandler ChangeToTray40_60Event;

        private readonly BackgroundWorker worker = new BackgroundWorker();
        private bool _isNextImagePrompted = false;
        public bool IsNextImagePrompted
        {
            get
            {
                return _isNextImagePrompted;
            }
            set
            {
                if (_isNextImagePrompted == value)
                    return;
                _isNextImagePrompted = value;
                RaisePropertyChanged("IsNextImagePrompted");
            }
        }


        private bool _waitImagePrompted = false;
        public bool WaitImagePrompted
        {
            get
            {
                return _waitImagePrompted;
            }
            set
            {
                if (_waitImagePrompted == value)
                    return;
                _waitImagePrompted = value;
                RaisePropertyChanged("WaitImagePrompted");
            }
        }

        private bool loadImageComplete = true;

        private bool isTray = true;

        private readonly IDataService _dataService;

        private ENUM.Mode _currentMode;
        public ENUM.Mode CurrentMode
        {
            get
            {
                return _currentMode;
            }
            set
            {
                if (_currentMode == value)
                    return;
                _currentMode = value;
                RaisePropertyChanged("CurrentMode");
            }
        }

        #region Defect Data

        private int startPictureId;
        private int _pictureNoId;
        public int PictureNoId
        {
            get
            {
                return _pictureNoId;
            }
            set
            {
                if (_pictureNoId == value)
                    return;
                _pictureNoId = value;
                RaisePropertyChanged("PictureNoId");
            }
        }
        private int pictureNextNoId;

        private string _pictureFilename;
        public string PictureFilename
        {
            get
            {
                return _pictureFilename;
            }
            set
            {
                if (_pictureFilename == value)
                    return;
                _pictureFilename = value;
                RaisePropertyChanged("PictureFilename");
            }
        }

        private List<DefectDataOnImage> defectDataOnImage;

        #endregion

        #region ICommand

        public ICommand StartCommand { get; private set; }
        public ICommand NextImageCommand { get; private set; }
        public ICommand ClearDefectsCommand { get; private set; }
        public ICommand SaveDefectsCommand { get; private set; }
        public ICommand ResetCalibrateCommand { get; private set; }
        public ICommand ConfirmGoodCommand { get; private set; }
        public ICommand ConfirmRejectCommand { get; private set; }
        public ICommand HGAImageCommand { get; private set; }
        public ICommand TrayMapCommand { get; private set; }
        public ICommand ConfirmMapCommand { get; private set; }
        public ICommand UpdateSerialCommand { get; private set; }
        public ICommand ConfirmMultipleGoodCommand { get; private set; }
        public ICommand ShowPalletFolderCommand { get; private set; }


        #endregion

        #region Properties

        private int _maxRound;
        public int MaxRound
        {
            get
            {
                return _maxRound;
            }
            set
            {
                if (_maxRound == value)
                    return;
                _maxRound = value;
                RaisePropertyChanged("MaxRound");
            }
        }

        private int _maxOrder;
        public int MaxOrder
        {
            get
            {
                return _maxOrder;
            }
            set
            {
                if (_maxOrder == value)
                    return;
                _maxOrder = value;
                RaisePropertyChanged("MaxOrder");
            }
        }

        private string _currentPath;
        public string CurrentPath
        {
            get
            {
                return _currentPath;
            }
            set
            {
                if(configXML.IsUsingJPG())
                {
                    _currentPath = configXML.GetJPGPath();
                    RaisePropertyChanged("CurrentPath");
                    return;
                }
                else if (!String.IsNullOrEmpty(_currentPath))
                {
                    return;
                }

                _currentPath = value;
                RaisePropertyChanged("CurrentPath");
            }
        }

        private string _strJPGPath;
        public string JPGPath
        {
            get { return _strJPGPath; }
            set { _strJPGPath = value; }
        }


        private string _mapImage;
        public string MapImage
        {
            get
            {
                return _mapImage;
            }
            set
            {
                if (_mapImage == value)
                    return;
                _mapImage = value;
                RaisePropertyChanged("MapImage");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _currentRoundId;
        public int CurrentRoundId
        {
            get
            {
                return _currentRoundId;
            }
            set
            {
                if (_currentRoundId == value)
                    return;
                _currentRoundId = value;
                RaisePropertyChanged("CurrentRoundId");
            }
        }

        private int _currentOrderId;
        public int CurrentOrderId
        {
            get
            {
                return _currentOrderId;
            }
            set
            {
                if (_currentOrderId == value)
                    return;
                _currentOrderId = value;
                //PictureNoId = _currentOrderId + startPictureId - 1;
                if (IsRandomImage)
                {
                    PictureNoId = _currentOrderId > 0 ? idShuffle[_currentOrderId - 1] : 0;
                }
                else
                {
                    PictureNoId = _currentOrderId - 1;
                }
                if (pathfilesInDirectory != null)
                {
                    PictureFilename = pathfilesInDirectory[pictureNextNoId].Substring(pathfilesInDirectory[pictureNextNoId].LastIndexOf("\\") + 1);
                }
                RaisePropertyChanged("CurrentOrderId");
            }
        }

        private bool _isRandomImage;
        public bool IsRandomImage
        {
            get
            {
                return _isRandomImage;
            }
            set
            {
                if (_isRandomImage == value)
                    return;
                _isRandomImage = value;
                RaisePropertyChanged("IsRandomImage");
            }
        }
        private bool _isProduction;
        public bool IsProduction
        {
            get
            {
                return _isProduction;
            }
            set
            {
                if (_isProduction == value)
                    return;
                _isProduction = value;
                RaisePropertyChanged("IsProduction");
            }
        }

        private bool _isPanning;
        public bool IsPanning
        {
            get
            {
                return _isPanning;
            }
            set
            {
                if (_isPanning == value)
                    return;
                _isPanning = value;
                RaisePropertyChanged("IsPanning");
            }
        }

        private SmartObservableCollection<ImageWithDefect> _itemsImage;
        public SmartObservableCollection<ImageWithDefect> ItemsImage
        {
            get
            {
                return _itemsImage;
            }
            set
            {
                if (_itemsImage == value)
                    return;
                _itemsImage = value;
                RaisePropertyChanged("ItemsImage");
            }
        }
        private List<ImageWithDefect> itemsImageNext;
        private ImageWithDefect _selectedImage;
        public ImageWithDefect SelectedImage
        {
            get
            {
                return _selectedImage;
            }
            set
            {
                if (_selectedImage == value)
                    return;
                _selectedImage = value;
                RaisePropertyChanged("SelectedImage");
            }
        }


        private string _percentZoomString = "100%";
        public string PercentZoomString
        {
            get
            {
                return _percentZoomString;
            }
            set
            {
                if (_percentZoomString == value)
                    return;
                _percentZoomString = value;
                RaisePropertyChanged("PercentZoomString");
            }
        }

        private double _scaleFactorSlider = 1;
        public double ScaleFactorSlider
        {
            get
            {
                return _scaleFactorSlider;
            }
            set
            {
                if (_scaleFactorSlider == value)
                    return;
                _scaleFactorSlider = value;
                PercentZoomString = (_scaleFactorSlider * 100).ToString("0") + "%";
                RaisePropertyChanged("ScaleFactorSlider");

                if (ScaleFactor == value)
                    return;

                double oldScale = ScaleFactor;
                ScaleFactor = value;

                // move image so that it will focus on center of the current ImagePanel
                double width = ImagePanelSize.X;
                double height = ImagePanelSize.Y;
                ImageTranslateX = (ImageTranslateX - width / 2) / oldScale * ScaleFactor + width / 2;
                ImageTranslateY = (ImageTranslateY - height / 2) / oldScale * ScaleFactor + height / 2;
            }
        }

        private double _scaleFactor = 1;
        public double ScaleFactor
        {
            get
            {
                return _scaleFactor;
            }
            set
            {
                if (_scaleFactor == value)
                    return;
                else if (value < 1)
                {
                    _scaleFactor = 1;
                }
                else if (value > 40)
                {
                    _scaleFactor = 40;
                }
                else
                {
                    _scaleFactor = value;
                }
                ScaleFactorSlider = value;
                RaisePropertyChanged("ScaleFactor");
            }
        }

        private double _imageTranslateX = 0;
        public double ImageTranslateX
        {
            get
            {
                return _imageTranslateX;
            }
            set
            {
                if (_imageTranslateX == value)
                    return;
                _imageTranslateX = value;

                RaisePropertyChanged("ImageTranslateX");
            }
        }

        private double _imageTranslateY = 0;
        public double ImageTranslateY
        {
            get
            {
                return _imageTranslateY;
            }
            set
            {
                if (_imageTranslateY == value)
                    return;
                _imageTranslateY = value;

                RaisePropertyChanged("ImageTranslateY");
            }
        }

        private Point _imagePanelSize;
        public Point ImagePanelSize
        {
            get
            {
                return _imagePanelSize;
            }
            set
            {
                if (_imagePanelSize == value)
                    return;
                _imagePanelSize = value;
            }
        }

        private bool _isTrayMap;
        public bool IsTrayMap
        {
            get
            {
                return _isTrayMap;
            }
            set
            {
                if (_isTrayMap == value)
                    return;
                _isTrayMap = value;
                RaisePropertyChanged("IsTrayMap");
            }
        }
        private bool _isComplete;
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
            set
            {
                if (_isComplete == value)
                    return;
                _isComplete = value;
                RaisePropertyChanged("IsComplete");
            }
        }

        public string _palletPath;
        public string PalletPath
        {
            get
            {
                return _palletPath;
            }
            set
            {
                if (_palletPath == value)
                    return;
                _palletPath = value;
                RaisePropertyChanged("PalletPath");
            }
        }

        private bool _isShowDefectPosition;
        public bool IsShowDefectPosition
        {
            get
            {
                return _isShowDefectPosition;
            }
            set
            {
                if (_isShowDefectPosition == value)
                    return;
                _isShowDefectPosition = value;
                RaisePropertyChanged("IsShowDefectPosition");

                if (SelectedImage != null && SelectedImage.ItemsDefects != null)
                {
                    foreach (var x in SelectedImage.ItemsDefects)
                    {
                        x.IsVisible = value;
                    }
                }
            }
        }

        private bool _isInspectingTray;
        public bool IsInspectingTray
        {
            get
            {
                return _isInspectingTray;
            }
            set
            {
                if (_isInspectingTray == value)
                    return;
                _isInspectingTray = value;
                RaisePropertyChanged("IsInspectingTray");
            }
        }

        private bool _isShowSerialNumber;
        public bool IsShowSerialNumber
        {
            get
            {
                return _isShowSerialNumber;
            }
            set
            {
                if (_isShowSerialNumber == value)
                    return;
                _isShowSerialNumber = value;
                RaisePropertyChanged("IsShowSerialNumber");
            }
        }

        #endregion

        #region Properties Defect

        private bool _checkDefect;
        public bool CheckDefect
        {
            get
            {
                return _checkDefect;
            }
            set
            {
                if (_checkDefect == value)
                    return;
                _checkDefect = value;
                RaisePropertyChanged("CheckDefect");
            }
        }

        private ENUM.ToolDefectEnum _toolDefect;
        public ENUM.ToolDefectEnum ToolDefect
        {
            get
            {
                return _toolDefect;
            }
            set
            {
                if (_toolDefect == value)
                    return;
                _toolDefect = value;
                RaisePropertyChanged("ToolDefect");

                if (_toolDefect == ENUM.ToolDefectEnum.None)
                {
                    CheckDefect = false;
                }
                else
                {
                    CheckDefect = true;
                }

                if (ChangeToolDefectEvent != null)
                {
                    ChangeToolDefectEvent(this, EventArgs.Empty);
                }
            }
        }

        private bool _moveT6;
        public bool MoveT6
        {
            get
            {
                return _moveT6;
            }
            set
            {
                if (_moveT6 == value)
                    return;
                _moveT6 = value;
                RaisePropertyChanged("MoveT6");
            }
        }

        #endregion

        private ConfigXML configXML;

        private SmartObservableCollection<HGAPack> _hGAList;
        public SmartObservableCollection<HGAPack> HGAList
        {
            get
            {
                return _hGAList;
            }
            set
            {
                if (_hGAList == value)
                    return;
                _hGAList = value;
                RaisePropertyChanged("HGAList");
            }
        }
        private HGATray _currentHGATray;
        public HGATray CurrentHGATray
        {
            get
            {
                return _currentHGATray;
            }
            set
            {
                if (_currentHGATray == value)
                    return;
                //_hGATray = HGAList[0].HGATrays[0];
                _currentHGATray = value;
                RaisePropertyChanged("CurrentHGATray");
            }
        }

        private List<int> idShuffle = new List<int>();
        int idShuffleIndex;

        private IRepository dataAccess;
        private ICsvDefectsData csvDefectsData;

        public ISampling Sampling { get; set; }

        Stopwatch stopWatch = new Stopwatch();

        private List<string> pathfilesInDirectory;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService, IRepository dataAccess, ICsvDefectsData csvDefectsData, ISampling sampling)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                    //WelcomeTitle = item.Title;
                });

            this.dataAccess = dataAccess;
            this.csvDefectsData = csvDefectsData;
            this.Sampling = sampling;

            ItemsImage = new SmartObservableCollection<ImageWithDefect>();
            itemsImageNext = new List<ImageWithDefect>();
            defectDataOnImage = new List<DefectDataOnImage>();
            HGAList = new SmartObservableCollection<HGAPack>();
            StartCommand = new RelayCommand(() => ExecuteStartCommand());
            NextImageCommand = new RelayCommand(() => NextImage());
            ClearDefectsCommand = new RelayCommand(() => ExecuteClearDefectsCommand());
            //CheckDefectCommand = new RelayCommand(() => ExecuteCheckDefectCommand());
            ResetCalibrateCommand = new RelayCommand(() => ExecuteResetCalibrateCommand());
            ConfirmGoodCommand = new RelayCommand(() => ExecuteConfirmGoodCommand());
            ConfirmRejectCommand = new RelayCommand(() => ExecuteConfirmRejectCommand());
            ConfirmMultipleGoodCommand = new RelayCommand(() => ExecuteConfirmMultipleGoodCommand());
            HGAImageCommand = new RelayCommand(() => SwitchToImageCommand());
            TrayMapCommand = new RelayCommand(() => SwitchToMapCommand());
            ConfirmMapCommand = new RelayCommand(() => ExecuteConfirmMapCommand());
            ShowPalletFolderCommand = new RelayCommand(() => ShowPalletFolderCommandCommand());

            configXML = new ConfigXML();

            startPictureId = configXML.GetStartPictureId();
            MaxRound = 0;
            MaxOrder = 0;
            CurrentPath = configXML.GetPath();
            JPGPath = configXML.GetJPGPath();
            if(configXML.IsUsingJPG())
            {
                CurrentPath = JPGPath;
            }

            MapImage = "Assets/TrayCrop.jpg";
            CurrentRoundId = 0;
            CurrentOrderId = 0;
            CurrentMode = ENUM.Mode.Stop;
            IsProduction = false;
            ToolDefect = ENUM.ToolDefectEnum.None;

            IsShowDefectPosition = true;

            IsRandomImage = true;
            IsComplete = false;
            worker.DoWork += Worker_DoWork;
            //worker.ProgressChanged += Worker_ProgressChanged;
            //worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            //worker.RunWorkerAsync();

        }

        private void ShowPalletFolderCommandCommand()
        {
            if (PalletPath == null || PalletPath.Length == 0) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                string path = Path.Combine(CurrentPath, PalletPath);
                try
                {
                    Process.Start(path);
                }
                catch (Win32Exception)
                {
                    MessageBox.Show("Cannot access this path.", "Pallet Path Error");
                }
            }
        }

        public void UpdateSerial(string serialnumber)
        {
            LogHelper.AppendLogFile(string.Format("Change S/N from '{0}' to '{1}' on trayId: {2}", SelectedHGAItem.SerialNumber, serialnumber, SelectedHGAItem.TrayId));
            dataAccess.UpdateSerial(SelectedHGAItem, serialnumber);
            SelectedHGAItem.SerialNumber = serialnumber;
        }

        /// <summary>
        /// Update status
        /// </summary>
        /// <param name="status_defect">true if have defect; otherwise, false.</param>
        private void UpdateGoodReject(bool status_defect)
        {
            if (CurrentMode == ENUM.Mode.Production)
            {
                if (WaitImagePrompted) return;
                if (IsTrayMap) return;
                if (SelectedHGAItem != null)
                {
                    SelectedHGAItem.IsDefect = status_defect;
                    SelectedHGAItem.UpdateStatus(dataAccess, status_defect);
                }
                //HGAItem hi = NextHGA();
                //if (hi == null)
                //{
                //}
                //else // calculate path from information
                //{
                //    if (IsNextImagePrompted)
                //    {
                //        UpdateImageUI();
                //    }
                //    else
                //    {
                //        WaitImagePrompted = true;
                //        LoadImageNextOQABackground(hi);
                //    }
                //}
            }
        }

        private void ExecuteConfirmRejectCommand()
        {
            if (SelectedHGAItem == null) return;
            if (CurrentMode == ENUM.Mode.Stop) return;
            DefectChooseOne choose = new DefectChooseOne();
            choose.SetDefect(SelectedHGAItem.GetTopDefect());
            choose.ShowDialog();
            if (choose.DialogResult == true)
            {
                Defect defect = choose.CurrentDefect;
                if (!(SelectedHGAItem.defectsList.Contains(defect.Name)))
                {
                    SelectedHGAItem.defectsList.Add(defect.Name);
                }

                Dictionary<int, Defect> defectNames = dataAccess.getDefectFromDB();
                SelectedHGAItem.VmiResult = defectNames.FirstOrDefault(x => x.Value.Name == defect.Name).Key;
                if (DefectCollection.CheckSkipDefect(defect.Name))
                {
                    UpdateGoodReject(false);
                }
                else
                {
                    UpdateGoodReject(true);
                }
            }
        }

        private void ExecuteConfirmGoodCommand()
        {
            if (SelectedHGAItem == null) return;
            SelectedHGAItem.VmiResult = 2;
            UpdateGoodReject(false);
        }
        private void ExecuteConfirmMultipleGoodCommand()
        {
            UpdateMultipleGood("A1");
        }

        private void UpdateMultipleGood(string defectName)
        {
            if (CurrentMode == ENUM.Mode.Production)
            {
                foreach (HGAPack hp in HGAList)
                {
                    foreach (HGATray ht in hp.HGATrays)
                    {
                        if (ht.IsComplete == 1) return;
                        foreach (HGAItem hi in ht.HGAItems)
                        {
                            if (hi != null && hi.IsDefect && hi.DefectName.Equals("REJECT", StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (hi.defectsList.Contains(defectName))
                                {
                                    hi.defectsList.RemoveAll(item => item.Equals(defectName, StringComparison.CurrentCultureIgnoreCase));
                                }
                                if (hi.defectsList.Count == 0)
                                {
                                    Dictionary<int, Defect> defectNames = dataAccess.getDefectFromDB();
                                    hi.VmiResult = defectNames.FirstOrDefault(x => x.Value.Name.Equals("A1_Autowipe", StringComparison.CurrentCultureIgnoreCase)).Key;
                                    hi.IsDefect = false;
                                    hi.UpdateStatus(dataAccess, false);
                                }
                            }
                        }
                    }
                }

            }
        }

        private void SwitchToMapCommand()
        {
            IsTrayMap = true;
        }

        private void SwitchToImageCommand()
        {
            IsTrayMap = false;
        }

        private void ExecuteConfirmMapCommand()
        {
            if (CurrentMode == ENUM.Mode.Stop)
            {
                return;
            }
            if (!isTray)
            {
                return;
            }
            int positionSN = ValidateSerialOnTray(HGAList[0].HGATrays[0]);
            if (positionSN >= 0)
            {
                MessageBox.Show("Please correct HGA serial number at position " + (positionSN + 1) + ".");
                return;
            }
            int positionDefect = ValidateAllDefectIsConfirmed(HGAList[0].HGATrays[0]);
            if (positionDefect >= 0)
            {
                MessageBox.Show("Please confirm reject HGA at position " + (positionDefect + 1) + ".");
                return;
            }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure to confirm tray data?", "Submit Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                int changed_reject_count = ValidateAllHGAChangedToDefect(HGAList[0].HGATrays[0]);

                CurrentMode = ENUM.Mode.Stop;
                IsProduction = false;
                ItemsImage.Clear();
                MapImage = null;
                SelectedHGAItem = null;
                ItemsImage = new SmartObservableCollection<ImageWithDefect>();
                HGATray tray = HGAList[0].HGATrays[0];
                //HGAList[0].HGATrays[0].HGAItems = new List<HGAItem>();
                if (tray.IsComplete == 1)
                {
                    LogHelper.AppendLogFile(string.Format("Reconfirm tray barcode: {0} (Key: {1}).", tray.TrayId, tray.TrayInt));
                }
                dataAccess.UpdateTrayComplete(tray,Sampling.SamplingNow()?1:0);
                SubmitTray();


                if (changed_reject_count > 0)
                {
                    Sampling.Next();

                    Sampling.SamplingRate = Sampling.TotalSampling;

                    //Sampling.Current = 1;

                    SampingList.Clear();
                }
                else
                {
                    if (Sampling.Current == 1)
                    {
                        SampingList.Clear();
                    }

                    SampingList.Add(true);

                    Sampling.Next();

                    if (Sampling.Current == 1 && SampingList.Count == Sampling.TotalSampling)
                    {
                        Sampling.SamplingRate = Sampling.SamplingStartRate;
                    }
                }

                StartClick();
            }
            else return;
        }

        private List<bool> SampingList = new List<bool>();

        private bool ValidateSerialNumber(string serialNumber)
        {
            return ((serialNumber.Length == 8) || (serialNumber.Length == 10)) && !serialNumber.Contains("?");
        }

        private int ValidateSerialOnTray(HGATray tray) {
            for (int i = 0; i < tray.HGAItems.Count; i++) 
            {
                var item = tray.HGAItems[i];
                if (!item.IsDefect && !ValidateSerialNumber(item.SerialNumber))
                {
                    return i;
                }
            }
            return -1;
        }

        private int ValidateAllDefectIsConfirmed(HGATray tray)
        {
            for (int i = 0; i < tray.HGAItems.Count; i++)
            {
                if (tray.HGAItems[i].IsDefect && tray.HGAItems[i].VmiResultString.Equals("None", StringComparison.InvariantCultureIgnoreCase) &&
                    !tray.HGAItems[i].DefectName.Equals("NOHGA", StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private int ValidateAllHGAChangedToDefect(HGATray tray)
        {
            int count=0;
            for (int i = 0; i < tray.HGAItems.Count; i++)
            {
                if (tray.HGAItems[i].VmiResult != 1 && tray.HGAItems[i].VmiResult != 2 && tray.HGAItems[i].DefectName.ToUpper().Equals("GOOD"))
                    count++;                                   
            }
            return count;
        }


        public void NoTrayData(string trayId, HGATray tray = null)
        {
            MessageBoxResult messageBoxResult;
            if (tray == null)
            {
                LogHelper.AppendErrorFile(string.Format("Tray ({0}) does not have data from unload.", trayId));
                messageBoxResult = System.Windows.MessageBox.Show("Tray Does Not Have Data from Unload. Please try again", "Tray Loading Error", System.Windows.MessageBoxButton.YesNo);
            }
            else // if (tray.AQTrayHeader == null)
            {

                LogHelper.AppendErrorFile(string.Format("Tray ({0}, id={1}) does not have header information from Unload.", trayId, tray.TrayInt));
                messageBoxResult = System.Windows.MessageBox.Show("Tray Does Not Have Header Information from Unload. Please try again", "Tray Header Error", System.Windows.MessageBoxButton.YesNo);
            }
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                CurrentMode = ENUM.Mode.Stop;
                IsProduction = false;
                ItemsImage.Clear();
                MapImage = null;
                SelectedHGAItem = null;
                ItemsImage = new SmartObservableCollection<ImageWithDefect>();
                //HGAList[0].HGATrays[0].HGAItems = new List<HGAItem>();         
                StartClick();
            }
            else
            {
                return;
            }
        }

        internal ConfigXML SetConfigXML()
        {
            ConfigXML configXML = new ConfigXML();

            startPictureId = configXML.GetStartPictureId();

            MaxOrder = configXML.GetMaxPictureOrder();
            if (CurrentMode == ENUM.Mode.Test || CurrentMode == ENUM.Mode.PlayBack)
            {
                CurrentPath = configXML.GetPathEvaluation();
            }
            else
            {
                CurrentPath = configXML.GetPath();
            }

            return configXML;
        }

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
            }
        }

        void Reset()
        {
            // reset
            Name = "";
            CurrentRoundId = 0;
            CurrentOrderId = 0;
            PictureNoId = 0;
            ItemsImage.Clear();

            MapImage = null;
            SelectedHGAItem = null;
            IsProduction = false;
            CurrentHGATray = null;
            PalletPath = null;

            CurrentMode = ENUM.Mode.Stop;
        }

        internal void StartOQA(ENUM.OPTIONS option, ENUM.DETAILS detail, string input)
        {
            try
            {
                if (detail == ENUM.DETAILS.Auto)
                {
                    HGAPack hp = dataAccess.SearchTrayId("P002");
                    if (hp == null)
                    {
                        // TODO: add some event to clear all screen and image
                        Reset();
                        NoTrayData(input);
                        return;
                    }
                    HGAList.Clear();
                    HGAList.Add(hp);
                    if (hp.HGATrays[0].palletIds.Count == 0)
                    {

                        LogHelper.AppendLogFile(string.Format("Checking TrayId {0}({1})", hp.HGATrays[0].TrayId, hp.HGATrays[0].TrayInt));
                    }
                    else if (hp.HGATrays[0].palletIds.Count >= 1)
                    {
                        string str_pallet = string.Join(", ", hp.HGATrays[0].palletIds);
                        LogHelper.AppendLogFile(string.Format("Checking TrayId {0}({1}): Including Pallet {2}", 
                                hp.HGATrays[0].TrayId, hp.HGATrays[0].TrayInt, str_pallet));
                    }
                    IsComplete = hp.HGATrays[0].IsComplete == 0 ? false : true;
                }
                else if (detail == ENUM.DETAILS.Pack)
                {
                    //HGAPack hp = dataAccess.SearchPackId(input);
                    HGAPack hp = dataAccess.SearchTrayId(input);
                    if (hp == null)
                    {
                        // TODO: add some event to clear all screen and image
                        Reset();
                        NoTrayData(input);
                        return;
                    }
                    HGAList.Clear();
                    HGAList.Add(hp);
                    CurrentHGATray = null;
                    IsComplete = hp.HGATrays[0].IsComplete == 0 ? false : true;
                }
                else if (detail == ENUM.DETAILS.Tray)
                {
                    HGAPack hp = dataAccess.SearchTrayId(input);
                    if (input == null || input == "" || hp == null)
                    {
                        // TODO: add some event to clear all screen and image
                        Reset();
                        NoTrayData(input);
                        return;
                    }
                    else if (!hp.HGATrays[0].HasAQHeader)
                    {
                        Reset();
                        NoTrayData(input, hp.HGATrays[0]);
                        return;
                    }

                    IsShowSerialNumber = false;
                    if (input[0] == '4')
                    {
                        MapImage = "Assets/tray40.png";
                        hp.HGATrays[0].TraySize = 40;
                        //hp.HGATrays[0].CurrentRowColumnInTray = 1;
                        //for (int i = 0; i < hp.HGATrays[0].HGAItems.Count; i++)
                        //{
                        //    hp.HGATrays[0].HGAItems[i].HGAHeight *= 2;
                        //    if (i >= 10) hp.HGATrays[0].HGAItems[i].Canvas_Y = hp.HGATrays[0].HGAItems[0].Canvas_Y;
                        //}

                        //if (ChangeToTray40_60Event != null)
                        //{
                        //    ChangeToTray40_60Event(this, EventArgs.Empty);
                        //}
                    }
                    else if (input[0] == '6')
                    {
                        MapImage = "Assets/tray60.png";
                        hp.HGATrays[0].TraySize = 60;
                    }
                    else
                    {
                        MapImage = "Assets/TrayCrop.jpg";
                        IsShowSerialNumber = true;
                        //if (ChangeToTray20Event != null)
                        //{
                        //    ChangeToTray20Event(this, EventArgs.Empty);
                        //}
                    }

                    HGAList.Clear();
                    HGAList.Add(hp);

                    dataAccess.GetDefectsOnTray(hp.HGATrays[0], input);
                    CurrentHGATray = hp.HGATrays[0];

                    if (hp.HGATrays[0].palletIds.Count == 0)
                    {
                        LogHelper.AppendLogFile("Checking TrayId " + hp.HGATrays[0].TrayId);
                    }
                    else if (hp.HGATrays[0].palletIds.Count >= 1)
                    {
                        string str_pallet = string.Join(", ", hp.HGATrays[0].palletIds);
                        LogHelper.AppendLogFile("Checking TrayId " + hp.HGATrays[0].TrayId + ": " + "Including Pallet " + str_pallet);
                    }
                    isTray = true;
                    IsComplete = hp.HGATrays[0].IsComplete == 0 ? false : true;
                }
                else if (detail == ENUM.DETAILS.Pallet)
                {
                    MapImage = "Assets/Pallet.jpg";
                    HGAPack hp = dataAccess.SearchPalletId(input);
                    HGAList.Clear();
                    if (hp == null)
                    {
                        // TODO: add some event to clear all screen and image
                        Reset();
                        return;
                    }
                    HGAList.Add(hp);
                    LogHelper.AppendLogFile("Checking Pallet: " + hp.HGATrays[0].TrayId);
                    isTray = false;
                    IsComplete = hp.HGATrays[0].IsComplete == 0 ? false : true;
                }

                Sampling.SamplingNow();
                Option = option;
                IsNextImagePrompted = false;    // don't pick auto load image.
                if (option == ENUM.OPTIONS.OQA)
                {
                    CurrentMode = ENUM.Mode.Production;
                    IsProduction = true;
                    //initial
                    SelectedHGAItem = null;
                    WaitImagePrompted = false;
                    ItemsImage.Clear();
                    configXML = SetConfigXML();
                    pictureNextNoId = 1;
                    NextImage();
                }
                else if (option == ENUM.OPTIONS.Rescreen)
                {

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LogHelper.AppendErrorFile(ex.ToString());
                if (DBConnectionFail != null)
                {
                    DBConnectionFail(this, EventArgs.Empty);
                }
                return;
            }

        }

        private void ExecuteResetCalibrateCommand()
        {
            ImageTranslateX = 0;
            ImageTranslateY = 0;
            ScaleFactor = 1;
            ScaleFactorSlider = 1;
        }

        private void ExecuteCheckDefectCommand()
        {
            CheckDefect = true;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("RunWorkerCompleted");


            if (!loadImageComplete)
            {
                return;
            }

            if (WaitImagePrompted)
            {
                WaitImagePrompted = false;
                UpdateImageUI();
            }
            else
            {
                IsNextImagePrompted = true;
            }
        }

        /// <summary>
        /// Load Images to ItemsImage in background. Define the image by pictureNextNoId.
        /// </summary>
        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadImageComplete = true;
            List<string> listView = configXML.GetListView();
            List<String> listMap = configXML.GetListMap();
            List<string> filetype = configXML.GetFiletype();

            if (pathfilesInDirectory == null)
            {
                if (ImageNotFound != null)
                {
                    ImageNotFound(this, EventArgs.Empty);
                }
                return;
            }

            List<string> path = new List<string>();
            for (int i = 0; i < listView.Count; i++)
            {
                //path.Add(Path.Combine(CurrentPath, listView[i], pictureNextNoId.ToString() + "." + filetype[i]));
                path.Add(pathfilesInDirectory[pictureNextNoId]);
            }

            List<View> dummyListView = new List<View>();
            foreach (string x in listView)
            {
                dummyListView.Add(new View()
                {
                    Id = 0,
                    Name = x
                });
            }

            LoadImageList(path, dummyListView, ref loadImageComplete);



        }

        /// <summary>
        /// Load image in provided path. (Old - Throw System.IO.FileNotFoundException if file not found.)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listView"></param>
        private void LoadImageList(List<string> path, List<View> listView, ref bool loadImageComplete, List<long> listInspectionDataId = null)
        {
            try
            {
                lock (_objectItemsImage)
                {
                    itemsImageNext.Clear();

                    int thumbnailWidth = configXML.GetThumbnailWidth();
                    int imageWidth = configXML.GetImageWidth();

                    List<BitmapImage> bmpImageList = new List<BitmapImage>();
                    List<BitmapImage> bmpImageThumbnailList = new List<BitmapImage>();
                    Random rnd = new Random();
                    for (int i = 0; i < listView.Count; i++)
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bi.DecodePixelWidth = imageWidth;     // At least, should be 1024
                        bi.EndInit();
                        bi.Freeze();
                        bmpImageList.Add(bi);

                        BitmapImage bmpImageThumbnail = new BitmapImage();
                        bmpImageThumbnail.BeginInit();
                        bmpImageThumbnail.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bmpImageThumbnail.DecodePixelWidth = thumbnailWidth;   // Default, 150px
                        bmpImageThumbnail.CacheOption = BitmapCacheOption.OnLoad;
                        bmpImageThumbnail.EndInit();
                        //RenderOptions.SetBitmapScalingMode(bmpImageThumbnail, BitmapScalingMode.LowQuality);
                        bmpImageThumbnail.Freeze();
                        bmpImageThumbnailList.Add(bmpImageThumbnail);
                    }

                    if (listInspectionDataId == null)
                    {
                        for (int i = 0; i < listView.Count; i++)
                        {
                            ImageWithDefect imgNext = new ImageWithDefect()
                            {
                                Id = i,
                                ImageThumbnail = bmpImageThumbnailList[i],
                                Image = bmpImageList[i],
                                View = listView[i].Name,
                                ViewId = listView[i].Id
                            };
                            Match match = Regex.Match(path[i], ".*#(.*)\\.(bmp|jpg|jpeg)");

                            if (match.Success)
                            {
                                // Finally, we get the Group value and display it.
                                string defect = match.Groups[1].Value;
                                Console.WriteLine(defect);
                                imgNext.AddToItemsDefects(new DefectsArea(new Point(0, 0), new List<Defect>() {
                                    new Defect(defect, true)
                                }));
                            }
                            itemsImageNext.Add(imgNext);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listView.Count; i++)
                        {
                            ImageWithDefect imgNext = new ImageWithDefect()
                            {
                                Id = i,
                                ImageThumbnail = bmpImageThumbnailList[i],
                                Image = bmpImageList[i],
                                View = listView[i].Name,
                                ViewId = listView[i].Id,
                                //InspectionDataId = listInspectionDataId[i]
                            };
                            //imgNext.AddToItemsDefects(new DefectsArea(new Point(0, 0), new List<Defect>() {
                            //    new Defect("A1", true)
                            //}));
                            itemsImageNext.Add(imgNext);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is System.Net.WebException) // WebException - The device is not ready.
                {
                    Debug.WriteLine(ex);
                    LogHelper.AppendErrorFile(ex.ToString());

                    loadImageComplete = false;
                    if (ImageNotFound != null)
                    {
                        ImageNotFound(this, EventArgs.Empty);
                    }
                }
                else
                {
                    LogHelper.AppendErrorFile(ex.ToString(), true);
                    throw;
                }
            }
        }

        private void LoadImageListFromDatabase(List<string> path, List<HGAViewData> listView, ref bool loadImageComplete, List<long> listInspectionDataId = null)
        {
            try
            {
                lock (_objectItemsImage)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    itemsImageNext.Clear();

                    int thumbnailWidth = configXML.GetThumbnailWidth();
                    int imageWidth = configXML.GetImageWidth();

                    List<BitmapImage> bmpImageList = new List<BitmapImage>();
                    List<BitmapImage> bmpImageThumbnailList = new List<BitmapImage>();
                    Random rnd = new Random();
                    for (int i = 0; i < path.Count; i++)
                    {
                        
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bi.DecodePixelWidth = imageWidth;     // At least, should be 1024     //commented out, due to different views/cameras having different resolution; cannot fix with 1024 pixels
                        bi.EndInit();
                        bi.Freeze();
                        bmpImageList.Add(bi);
                        

                        BitmapImage bmpImageThumbnail = new BitmapImage();
                        bmpImageThumbnail.BeginInit();
                        bmpImageThumbnail.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bmpImageThumbnail.DecodePixelWidth = thumbnailWidth;   // Default, 150px
                        bmpImageThumbnail.CacheOption = BitmapCacheOption.OnLoad;
                        bmpImageThumbnail.EndInit();
                        //RenderOptions.SetBitmapScalingMode(bmpImageThumbnail, BitmapScalingMode.LowQuality);
                        bmpImageThumbnail.Freeze();
                        bmpImageThumbnailList.Add(bmpImageThumbnail);
                    }
                    
                    for (int i = 0; i < listView.Count; i++)
                    {
                        ImageWithDefect imgNext = new ImageWithDefect()
                        {
                            Id = i,
                            ImageThumbnail = bmpImageThumbnailList[i],
                            Image = bmpImageList[i],
                            //Image = null,
                            ImagePath = path[i],
                            //View = listView[i].Name,
                            ViewId = listView[i].ViewId,
                            //InspectionDataId = listInspectionDataId[i]
                        };
                        //imgNext.AddToItemsDefects(new DefectsArea(new Point(0, 0), new List<Defect>() {
                        //    new Defect("A1", true)
                        //}));
                        itemsImageNext.Add(imgNext);
                    }
                    
                    sw.Stop();
                    Debug.WriteLine("Stop Loading image2: " + sw.ElapsedMilliseconds);
                    LogHelper.AppendDebugFile(string.Format("Loading photo time: {0} ms", sw.ElapsedMilliseconds));
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is System.Net.WebException) // WebException - The device is not ready.
                {
                    Debug.WriteLine(ex);
                    LogHelper.AppendErrorFile(ex.ToString());

                    loadImageComplete = false;
                    if (ImageNotFound != null)
                    {
                        ImageNotFound(this, EventArgs.Empty);
                    }
                }
                else
                {
                    LogHelper.AppendErrorFile(ex.ToString(), true);
                    throw;
                }
            }
        }



        /// <summary>
        /// Load image in provided path. (Old - Throw System.IO.FileNotFoundException if file not found.)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listView"></param>
        private void LoadImageListFromDatabase2(List<string> path, List<HGAViewData> listView, ref bool loadImageComplete, List<long> listInspectionDataId = null)
        {
            try
            {
                lock (_objectItemsImage)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    itemsImageNext.Clear();

                    int thumbnailWidth = configXML.GetThumbnailWidth();
                    int imageWidth = configXML.GetImageWidth();

                    List<BitmapImage> bmpImageList = new List<BitmapImage>();
                    List<BitmapImage> bmpImageThumbnailList = new List<BitmapImage>();
                    Random rnd = new Random();
                    for (int i = 0; i < path.Count; i++)
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bi.DecodePixelWidth = imageWidth;     // At least, should be 1024
                        bi.EndInit();
                        bi.Freeze();
                        bmpImageList.Add(bi);

                        BitmapImage bmpImageThumbnail = new BitmapImage();
                        bmpImageThumbnail.BeginInit();
                        bmpImageThumbnail.UriSource = new Uri(path[i], UriKind.RelativeOrAbsolute);
                        //bmpImageThumbnail.DecodePixelWidth = thumbnailWidth;   // Default, 150px
                        bmpImageThumbnail.CacheOption = BitmapCacheOption.OnLoad;
                        bmpImageThumbnail.EndInit();
                        //RenderOptions.SetBitmapScalingMode(bmpImageThumbnail, BitmapScalingMode.LowQuality);
                        bmpImageThumbnail.Freeze();
                        bmpImageThumbnailList.Add(bmpImageThumbnail);
                    }

                    for (int i = 0; i < listView.Count; i++)
                    {
                        ImageWithDefect imgNext = new ImageWithDefect()
                        {
                            Id = i,
                            ImageThumbnail = bmpImageThumbnailList[i],
                            Image = bmpImageList[i],
                            //Image = null,
                            ImagePath = path[i],
                            //View = listView[i].Name,
                            ViewId = listView[i].ViewId,
                            //InspectionDataId = listInspectionDataId[i]
                        };
                        //imgNext.AddToItemsDefects(new DefectsArea(new Point(0, 0), new List<Defect>() {
                        //    new Defect("A1", true)
                        //}));
                        itemsImageNext.Add(imgNext);
                    }
                    sw.Stop();
                    Debug.WriteLine("Stop Loading image2: " + sw.ElapsedMilliseconds);
                    LogHelper.AppendDebugFile(string.Format("Loading photo time: {0} ms", sw.ElapsedMilliseconds));
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is System.Net.WebException) // WebException - The device is not ready.
                {
                    Debug.WriteLine(ex);
                    LogHelper.AppendErrorFile(ex.ToString());

                    loadImageComplete = false;
                    if (ImageNotFound != null)
                    {
                        ImageNotFound(this, EventArgs.Empty);
                    }
                }
                else
                {
                    LogHelper.AppendErrorFile(ex.ToString(), true);
                    throw;
                }
            }
        }

        private void InitialDefectData()
        {
            defectDataOnImage.Clear();
        }

        private List<string> ListAllFilesInFolder(string directory, string filetype)
        {
            if (Directory.Exists(directory))
            {
                string[] arr = Directory.GetFiles(directory, "*." + filetype, SearchOption.TopDirectoryOnly);
                foreach (string a in arr)
                {
                    Debug.WriteLine(a);
                }
                return arr.ToList();
            }
            else return null;
        }

        private void ExecuteStartCommand()
        {
            CurrentMode = ENUM.Mode.Test;

            configXML = SetConfigXML();

            // list all files in directory with certain type
            List<string> listView = configXML.GetListView();
            List<string> filetype = configXML.GetFiletype();
            pathfilesInDirectory = ListAllFilesInFolder(Path.Combine(CurrentPath, listView[0]), filetype[0]);

            try
            {
                MaxOrder = pathfilesInDirectory.Count;
            }
            catch (System.NullReferenceException)
            {


            }
            // initialize Defect Data
            InitialDefectData();

            CurrentRoundId = 0;

            IsNextImagePrompted = false;
            NextRound();

            //Timer counter = new Timer(5000);
            //counter.Elapsed += counter_Elapsed;
            //counter.Start();
        }

        void counter_Elapsed(object sender, ElapsedEventArgs e)
        {
            MessageBox.Show("Hello");
        }

        private void SetPictureNextNoId()
        {
            if (CurrentMode == ENUM.Mode.PlayBack)
            {
                try
                {
                    pictureNextNoId = defectDataOnImage.Where(d => d.Round == CurrentRoundId).Where(d => d.Order == CurrentOrderId + 1).First().PictureNo;
                    //pictureNextNoId = defectDataOnImage.Where(d => d.Round == CurrentRoundId).Where(d => d.Order == CurrentOrderId + 1).First().Order - 1;
                    idShuffle.Add(pictureNextNoId);
                }
                catch (Exception)
                {
                    if (NoCurrentOrderIdInCsv != null)
                    {
                        NoCurrentOrderIdInCsv(this, EventArgs.Empty);
                    }
                    CurrentMode = ENUM.Mode.Stop;
                }
            }
            else if (CurrentMode == ENUM.Mode.Test)
            {
                //pictureNextNoId = (startPictureId - 1) + CurrentOrderId + 1;
                pictureNextNoId = idShuffle[idShuffleIndex];        // enable to random image for GR&R
                //pictureNextNoId = idShuffleIndex;
                idShuffleIndex++;
            }
            //else if (CurrentMode == Mode.OQA)
            //{
            //    pictureNextNoId++;
            //}
        }

        /// <summary>
        /// For test(evaluation). It may be multiple round to do for repeatability.
        /// </summary>
        private void NextRound()
        {
            CurrentRoundId++;
            if (CurrentRoundId <= MaxRound)
            {
                CurrentOrderId = 0;

                // generate shuffle array
                idShuffle.Clear();
                if (CurrentMode == ENUM.Mode.Test)
                {
                    // set to 0 for using the value in idShuffle as an index
                    startPictureId = 0;
                    for (int i = startPictureId; i < startPictureId + MaxOrder; i++)
                    {
                        idShuffle.Add(i);
                    }
                    idShuffleIndex = 0;
                    Random rnd = new Random();
                    idShuffle = idShuffle.OrderBy(x => rnd.Next()).ToList();
                }

                SetPictureNextNoId();
                worker.RunWorkerAsync();
                WaitImagePrompted = false;
                stopWatch.Restart();
                NextImage();
            }
            else            // FinishProcess
            {
                if (CurrentMode == ENUM.Mode.Test)
                {
                    csvDefectsData.SaveDefectsToCsv(Name, defectDataOnImage, CurrentPath);
                }
                CurrentMode = ENUM.Mode.Stop;

                // reset
                Name = "";
                CurrentRoundId = 0;
                CurrentOrderId = 0;
                PictureNoId = 0;
                ItemsImage.Clear();

                if (FinishProcess != null)
                {
                    FinishProcess(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Load next image 
        /// If there is image before and in Test Mode, add info to DefectData fields.
        /// </summary>
        private void NextImage()
        {
            if (CurrentMode == ENUM.Mode.Test || CurrentMode == ENUM.Mode.PlayBack)
            {
                if (CurrentRoundId == 0)
                {
                    LogHelper.AppendErrorFile("NextImage with RoundId = 0, something's wrong");
                    return;
                }
                if (WaitImagePrompted)
                {
                    return;
                }

                if (CurrentOrderId > 0 && CurrentMode == ENUM.Mode.Test)
                {
                    stopWatch.Stop();
                    long elapsedMs = stopWatch.ElapsedMilliseconds;
                    SaveToDefectDataOnImageObject((int)elapsedMs);
                    stopWatch.Restart();
                }

                if (CurrentOrderId + 1 <= MaxOrder)
                {
                    CurrentOrderId++;
                    if (IsNextImagePrompted)
                    {
                        UpdateImageUI();
                    }
                    else
                    {
                        WaitImagePrompted = true;
                    }
                }
                else
                {
                    NextRound();
                }
            }
            else if (CurrentMode == ENUM.Mode.Production)
            {
                if (WaitImagePrompted) return;

                if (SelectedHGAItem != null)
                {
                    //SelectedHGAItem.SaveHGAItemToDatabase(dataAccess, ItemsImage);
                    //TODO: handle confirm good or reject
                    bool status_defect = false;
                    foreach (var image in ItemsImage)
                    {
                        if (image.ItemsDefectsStringUnique.Count > 0)
                        {
                            status_defect = true;
                            break;
                        }
                    }
                    SelectedHGAItem.UpdateStatus(dataAccess, status_defect);
                }

                HGAItem hi = NextHGA();
                if (hi == null)
                {
                    //if (FinishProcess != null)
                    //{
                    //    FinishProcess(this, EventArgs.Empty);
                    //}
                }
                else // calculate path from information
                {
                    if (IsNextImagePrompted)
                    {
                        UpdateImageUI();
                    }
                    else
                    {
                        WaitImagePrompted = true;
                        LoadImageNextOQABackground(hi);
                    }
                }
            }
        }

        /// <summary>
        /// ChangeImage in OQA without save.
        /// </summary>
        internal void ChangeImage()
        {
            if (CurrentMode == ENUM.Mode.Production)
            {
                if (WaitImagePrompted) return;

                HGAItem hi = NextHGAItem;
                if (hi == null)
                {
                    LogHelper.AppendErrorFile("NextHGAItem is null but treeview_selectedChanged is raised.");
                    if (ErrorProcess != null)
                    {
                        ErrorProcess(this, EventArgs.Empty);
                    }
                }
                else // calculate path from information
                {
                    Debug.WriteLine("ChangeImage " + hi.SerialNumber);
                    IsNextImagePrompted = false;    // don't pick auto load image.
                    if (IsNextImagePrompted)
                    {
                        UpdateImageUI();
                    }
                    else
                    {
                        WaitImagePrompted = true;
                        LoadImageNextOQABackground(hi);
                    }
                }
            }
        }
        public ManipulationModes ManipulationMode { get; set; }

        private void SaveToDefectDataOnImageObject(int elapsedMs)
        {
            foreach (ImageWithDefect imgDefect in ItemsImage)
            {
                List<string> strDefectEachArea = imgDefect.GetListDefectEachArea();
                List<string> strDefectData = imgDefect.GetListDefectData();

                DefectDataOnImage tempDefectDataOnImage = new DefectDataOnImage()
                {
                    Round = CurrentRoundId,
                    Order = CurrentOrderId,
                    PictureNo = PictureNoId, //CurrentOrderId - 1, 
                    View = imgDefect.View,
                    Defect = string.Join("-", strDefectEachArea.Distinct().ToList()),
                    Time = elapsedMs,
                    DefectData = string.Join("/", strDefectData),
                    filename = PictureFilename
                };

                defectDataOnImage.Add(tempDefectDataOnImage);
            }
        }

        static readonly object _objectItemsImage = new object();
        private void UpdateImageUI()
        {
            IsNextImagePrompted = false;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                lock (_objectItemsImage)
                {
                    ItemsImage.Clear();
                    ItemsImage.AddRange(itemsImageNext);
                    if (SelectedHGAItem != null)
                    {
                        LastSelectedHGAItem = SelectedHGAItem;
                    }
                    SelectedHGAItem = NextHGAItem;
                    if (SelectedHGAItem != null)
                    {
                        SelectedHGAItem.IsSelected = true;
                        if (LastSelectedHGAItem != null && LastSelectedHGAItem != SelectedHGAItem)
                        {
                            LastSelectedHGAItem.IsSelected = false;
                        }
                        SelectedHGAItem.LoadHGADefectFromDatabaseToItemsImage(dataAccess, ItemsImage);
                    }

                    try
                    {
                        Defect top = new Defect("", priority: Defect.LEAST_PRIORITY);
                        int top_index = 0;
                        for (int i = 0; i < ItemsImage.Count; i++)
                        {
                            foreach (string str in ItemsImage[i].ItemsDefectsStringUnique)
                            {
                                int priority = Defect.GetDefectPriority(str);
                                if (priority < top.Priority)
                                {
                                    top = new Defect(str, priority: priority);
                                    top_index = i;
                                }
                            }
                        }
                        SelectedImage = ItemsImage[top_index];
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        LogHelper.AppendErrorFile("[No Next Image] - " + ex.ToString());
                        if (ErrorProcess != null)
                        {
                            ErrorProcess(this, EventArgs.Empty);
                        }
                    }
                }
            }));

            if (CurrentMode == ENUM.Mode.PlayBack)
            {
                LoadDefectPlayback();
            }

            // Load next image in background
            if (CurrentMode == ENUM.Mode.Test || CurrentMode == ENUM.Mode.PlayBack)
            {
                if (CurrentOrderId + 1 <= MaxOrder)
                {
                    SetPictureNextNoId();
                    worker.RunWorkerAsync();
                }
            }
            else if (CurrentMode == ENUM.Mode.Production)
            {
                HGAItem hi = NextHGA();
                //GetPathHGA(hi);
                if (hi != null)
                {
                    //SetPictureNextNoId();
                    LoadImageNextOQABackground(hi);
                }
            }
        }

        #region OQA
        private HGAItem _selectedHGAItem;
        public HGAItem SelectedHGAItem
        {
            get
            {
                return _selectedHGAItem;
            }
            set
            {
                if (_selectedHGAItem == value)
                    return;
                _selectedHGAItem = value;
                RaisePropertyChanged("SelectedHGAItem");
            }
        }

        private HGAItem LastSelectedHGAItem { get; set; }
        public HGAItem NextHGAItem { get; set; }

        internal HGAItem NextHGA()
        {
            foreach (HGAPack hp in HGAList)
            {
                foreach (HGATray ht in hp.HGATrays)
                {
                    //int _counter = 0; //added for temporarily debugging while in the loop
                    foreach (HGAItem hi in ht.HGAItems)
                    {
                        //_counter++;
                        if (hi.VmiResult == 1 && hi.IsDefect)
                        {
                            NextHGAItem = hi;
                            return hi;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Load Images to ItemsImage in background. Define the image by HGAPath.
        /// </summary>
        private void LoadImageNextOQABackground(HGAItem hi)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            //List<MachineView> listView = new List<MachineView>();
            //List<string> paths = hi.GetPathHGA(out listView);
            List<View> listView = new List<View>();
            List<string> paths = hi.GetPaths(out listView);
            //List<string> paths = new List<string>()
            //{
            //    "ABCD/1481101193.Tulips.jpg",
            //    "ABCD/1481101193.Tulips_front.jpg"
            //};
            //List<string> paths = hi.ImagePath;
            //List<long> listInspectionDataId = hi.GetInspectionDataIdFromHGA();
            List<long> listInspectionDataId = new List<long>() { 1 };
            if (paths != null)
            {
                foreach (string x in paths)
                {
                    Debug.WriteLine(x);
                }
                foreach (string x in paths)
                {
                    if(!WaitImagePrompted)
                    {
                        break;
                    }

                    int pos1 = x.LastIndexOf("\\Images");
                    //int pos2 = x.LastIndexOf('/');
                    //pos1 = Math.Max(pos1, pos2);
                    if (pos1 >= 0)
                    {
                        PalletPath = x.Substring(0, pos1);
                    }
                    else
                    {
                        PalletPath = x;
                    }
                    break;
                }
            }

            loadImageComplete = true;
            //List<string> filetype = configXML.GetFiletype();
            List<View> listView2 = View.Instance;

            List<string> fullpaths = new List<string>();
            for (int i = 0; i < paths.Count; i++)
            {
                fullpaths.Add(Path.Combine(CurrentPath, paths[i]));
            }

            List<string> fullJPGpaths = new List<string>();
            for (int i = 0; i < paths.Count; i++)
            {
                fullJPGpaths.Add(Path.Combine(JPGPath, paths[i].Replace(".bmp", ".jpg")));
            }


            Task.Factory.StartNew(() =>
            {
                if(configXML.IsUsingJPG())
                {
                    LoadImageListFromDatabase(fullJPGpaths, hi.HGAViews, ref loadImageComplete, listInspectionDataId);
                }
                else
                {
                    LoadImageListFromDatabase(fullpaths, hi.HGAViews, ref loadImageComplete, listInspectionDataId);
                }
            }).ContinueWith(t =>
            {
                //if (!loadImageComplete)
                //{
                //    return;
                //}

                if (WaitImagePrompted)
                {
                    WaitImagePrompted = false;
                    UpdateImageUI();
                }
                else
                {
                    IsNextImagePrompted = true;
                }
                sw.Stop();
                Debug.WriteLine("Stop Loading image: " + sw.ElapsedMilliseconds);
            });

        }

        #endregion

        /// <summary>
        /// Load defect information to display into ItemsImage.
        /// </summary>
        private void LoadDefectPlayback()
        {
            if (CurrentMode == ENUM.Mode.PlayBack)
            {
                foreach (DefectDataOnImage x in defectDataOnImage.Where(d => d.Round == CurrentRoundId).Where(d => d.Order == CurrentOrderId))
                {
                    if (x.DefectData != null && x.DefectData.Length > 0)
                    {
                        for (int i = 0; i < ItemsImage.Count; i++)
                        {
                            if (x.View == ItemsImage[i].View)
                            {
                                // Cut string
                                string data = x.DefectData;
                                string[] dataItem = data.Split('/');

                                for (int j = 0; j < dataItem.Count(); j++)
                                {
                                    string[] tempData = dataItem[j].Split('+');        // tempData[0] = list defect, tempData[1] = Position

                                    // Get List Defect
                                    string[] strListDefect = tempData[0].Split('-');
                                    List<string> list = strListDefect.ToList();
                                    List<Defect> listDefect = list.ConvertAll<Defect>(d => new Defect(d));

                                    // Get Position Ratio
                                    string[] strPosition = tempData[1].Split(':');
                                    Point positionRatio = new Point(double.Parse(strPosition[0]), double.Parse(strPosition[1]));

                                    DefectsArea tempDefectsArea = new DefectsArea(positionRatio, listDefect);

                                    //ItemsImage[i].ItemsDefects.Add(tempDefectsArea);
                                    ItemsImage[i].AddToItemsDefects(tempDefectsArea);
                                }

                            }
                        }
                    }
                }
            }
        }

        private void ExecuteClearDefectsCommand()
        {
            if (SelectedImage != null)
            {
                SelectedImage.ClearItemsDefects();
            }
        }

        public void AddDefect(Point positionRatio, List<Defect> listDefect)
        {
            if (SelectedImage != null)
            {
                //SelectedImage.AddToItemsDefects(new DefectsArea(positionRatio, listDefect));
                SelectedImage.AddToItemsDefects(new DefectsArea(positionRatio, listDefect));
            }
        }

        /// <summary>
        /// Load Defects from csv file. 
        /// </summary>
        internal void StartLoadDefectsFromCsv(string filepath)
        {
            int maxRound = 0;
            int maxOrder = 0;
            int minPictureNo = 1000000000;
            try
            {
                using (CsvFileReader reader = new CsvFileReader(filepath))
                {
                    CsvRow row = new CsvRow();
                    reader.ReadRow(row);            // Read Image Path
                    string imagePath = row[1];

                    List<string> listView = configXML.GetListView();
                    List<string> filetype = configXML.GetFiletype();
                    pathfilesInDirectory = ListAllFilesInFolder(Path.Combine(row[1], listView[0]), filetype[0]);

                    reader.ReadRow(row);            // Read Header

                    InitialDefectData();
                    while (reader.ReadRow(row))
                    {
                        DefectDataOnImage tempDefectDataOnImage = new DefectDataOnImage();
                        tempDefectDataOnImage.Round = int.Parse(row[0]);
                        tempDefectDataOnImage.Order = int.Parse(row[1]);
                        tempDefectDataOnImage.PictureNo = int.Parse(row[2]);
                        tempDefectDataOnImage.View = row[3];
                        tempDefectDataOnImage.Defect = row[4];
                        tempDefectDataOnImage.Time = int.Parse(row[5]);
                        if (row.Count >= 7)
                        {
                            tempDefectDataOnImage.DefectData = row[6];
                        }
                        if (row.Count >= 8)
                        {
                            tempDefectDataOnImage.filename = row[7];
                        }

                        defectDataOnImage.Add(tempDefectDataOnImage);

                        maxRound = Math.Max(maxRound, int.Parse(row[0]));
                        maxOrder = Math.Max(maxRound, tempDefectDataOnImage.Order);
                        minPictureNo = Math.Min(minPictureNo, tempDefectDataOnImage.PictureNo);
                    }
                }
            }
            catch (IOException)
            {
                if (FileUsedByAnotherProcess != null)
                {
                    FileUsedByAnotherProcess(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendErrorFile(ex.ToString());
                if (ErrorProcess != null)
                {
                    ErrorProcess(this, EventArgs.Empty);
                }
            }

            Name = Path.GetFileNameWithoutExtension(filepath);
            CurrentRoundId = 0;
            CurrentMode = ENUM.Mode.PlayBack;
            configXML = SetConfigXML();

            MaxOrder = maxOrder;
            MaxRound = maxRound;
            startPictureId = minPictureNo;

            IsNextImagePrompted = false;
            NextRound();
        }

        Point originalPoint = new Point(0, 0);

        public void SetOriginalPoint()
        {
            originalPoint = new Point(ImageTranslateX, ImageTranslateY);
        }

        public void ImageTranslate(Vector moveVector)
        {
            ImageTranslateX = originalPoint.X + moveVector.X;
            ImageTranslateY = originalPoint.Y + moveVector.Y;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void GetPositionOfHGA(Point location, double panel_height, double panel_width)
        {
            if (HGAList.Count == 0 || HGAList[0].HGATrays.Count == 0) return;
            int HGAIDOnTray = -1;
            if (isTray)
            {
                if (HGAList[0].HGATrays[0].TraySize == 20)
                {
                    double tray_offset_left = panel_width * HGATray.TRAY20_PHOTO_LEFTOFFSET;
                    HGAIDOnTray = (int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 10)) + 10 * Math.Floor(location.Y / (panel_height / 2)));
                }
                else if (HGAList[0].HGATrays[0].TraySize == 40)
                {
                    double tray_offset_left = panel_width * HGATray.TRAY40_PHOTO_LEFTOFFSET;
                    //HGAIDOnTray = (int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 10))) + 10 * (HGAList[0].HGATrays[0].CurrentRowColumnInTray - 1);
                    HGAIDOnTray = (int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 20)) + 20 * Math.Floor(location.Y / (panel_height / 2)));
                }
                else if (HGAList[0].HGATrays[0].TraySize == 60)
                {
                    double tray_offset_left = panel_width * HGATray.TRAY60_PHOTO_LEFTOFFSET;
                    //HGAIDOnTray = (int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 10))) + 10 * (HGAList[0].HGATrays[0].CurrentRowColumnInTray - 1);
                    HGAIDOnTray = (int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 20)) + 20 * Math.Floor(location.Y / (panel_height / 3)));
                }
            }
            else
            {
                HGAIDOnTray = (int)(Math.Ceiling(((location.X / panel_width - 0.004) * 992 - 120 - 14) / 72.2));
            }
            //int HGAIDOnTray = isTray ? ((int)(Math.Ceiling((location.X - tray_offset_left) / ((panel_width - 2 * tray_offset_left) / 10)) + 10 * Math.Floor(location.Y / (panel_height / 2)))) : (int)(Math.Ceiling(((location.X / panel_width - 0.004) * 992 - 120 - 14) / 72.2));
            foreach (HGAItem hga in HGAList[0].HGATrays[0].HGAItems)
            {
                if (hga.Position == HGAIDOnTray)
                {
                    //hga.IsDefect = hga.IsDefect == true ? false : true;
                    NextHGAItem = hga;
                    ChangeImage();
                    //hga.UpdateStatus(dataAccess, hga.IsDefect);
                }
            }
        }

        public void AddDefect(string defectName)
        {
            Dictionary<int, Defect> defectNames = dataAccess.getDefectFromDB();

            int id = defectNames.FirstOrDefault(x => x.Value.Name == defectName).Key;
            if (id == 0)
            {
                dataAccess.InsertDefect(defectName);
                DefectsXML xml = new DefectsXML();
                xml.AppendDefect(defectName);
            }
        }

        public void StartClick()
        {
            StartOQA startOQA = new StartOQA();
            startOQA.ShowDialog();

            if (startOQA.DialogResult == true)
            {
                string input = startOQA.GetDataContext().Id;
                ENUM.OPTIONS option = startOQA.GetDataContext().Option;
                ENUM.DETAILS detail = startOQA.GetDataContext().Detail;
                StartOQA(option, detail, input);
            }
        }

        public void SubmitTray()
        {
            CurrentMode = ENUM.Mode.Stop;
            if (HGAList.Count == 0 || HGAList[0].HGATrays.Count == 0) return;
            HGATray tray = HGAList[0].HGATrays[0];
            //foreach (HGAItem hga in tray.HGAItems)
            //{
            //    hga.UpdateStatus(dataAccess, hga.IsDefect);

            //}

            tray.DateTimeEnd = DateTime.Now;

            AQTrayInfo aqtray = new AQTrayInfo();
            aqtray.TrayId = tray.TrayId;
            aqtray.AQHeader = tray.GetAQTrayHeader();
            aqtray.HGAIdList = Enumerable.Repeat(AQTrayInfo.NOHGA, configXML.GetNumberOfHgaOnTray()).ToList();            //obsoleted by prasert, prepare list of hga serial in aq file with "----------" and then replace with real serials
            //aqtray.HGAIdList = Enumerable.Repeat(AQTrayInfo.NOHGA_SERIAL_10OCR, configXML.GetNumberOfHgaOnTray()).ToList(); 
            aqtray.PalletIdList = tray.palletIds;

            Dictionary<int, Defect> defectNames = dataAccess.getDefectFromDB();
            for (int i = 0; i < tray.HGAItems.Count; i++)
            {
                if (!tray.HGAItems[i].IsDefect || DefectCollection.CheckSkipDefect(defectNames[tray.HGAItems[i].VmiResult].Name))
                {
                    int position = tray.HGAItems[i].Position;
                    aqtray.HGAIdList[position - 1] = tray.HGAItems[i].SerialNumber;
                    if ((aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA) || (aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA_SERIAL_10OCR))      //modified, consequnce of line#2405
                    {
                        //aqtray.HGAIdList[position - 1] = AQTrayInfo.GOODHGA_NOSERIAL;
                        aqtray.HGAIdList[position - 1] = AQTrayInfo.GOODHGA_CANNOTREAD_SERIAL_10OCR;
                    }
                }
                else
                {
                    //if reject
                    int position = tray.HGAItems[i].Position;
                    aqtray.HGAIdList[position - 1] = tray.HGAItems[i].SerialNumber;
                    if ((aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA) || (aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA_SERIAL_10OCR))       //modified, consequnce of line#2405
                    {
                        //aqtray.HGAIdList[position - 1] = "--------";
                        aqtray.HGAIdList[position - 1] = AQTrayInfo.NOHGA_SERIAL_10OCR;
                    }
                }
            }

            string path_product = Path.Combine(aqtray.BasePathEasy, tray.Product);
            aqtray.WriteToAQTrayFile(path_product, aqtray.TrayId + ".LOG");
            aqtray.WriteToAQTrayFile(aqtray.BasePathMt2, aqtray.TrayId + ".mt2");
            aqtray.WriteToAQTrayFile(aqtray.BasePathMirror, aqtray.TrayId + ".LOG");
            aqtray.WriteToAQTrayFile(aqtray.BasePathBackup, aqtray.TrayId + "-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".LOG");
        }

        /// <summary>
        /// Script for running by list of tray. NOT for normal use in production.
        /// </summary>
        /// <param name="list_trays"></param>
        public void SubmitMultiplesTrayByListTrayId(List<string> list_trays)
        {
            ConfigXML configXML = new ConfigXML();

            Dictionary<int, Defect> defectNames = dataAccess.getDefectFromDB();
            foreach (var trayId in list_trays)
            {
                HGAPack hp = dataAccess.SearchTrayId(trayId);
                HGATray tray = hp.HGATrays[0];

                tray.DateTimeStart = DateTime.Now;
                tray.DateTimeEnd = DateTime.Now;

                AQTrayInfo aqtray = new AQTrayInfo();
                aqtray.TrayId = tray.TrayId;
                aqtray.AQHeader = tray.GetAQTrayHeader();
                aqtray.HGAIdList = Enumerable.Repeat(AQTrayInfo.NOHGA, configXML.GetNumberOfHgaOnTray()).ToList();                    //obsoleted by prasert
                //aqtray.HGAIdList = Enumerable.Repeat(AQTrayInfo.NOHGA_SERIAL_10OCR, configXML.GetNumberOfHgaOnTray()).ToList(); 
                aqtray.PalletIdList = tray.palletIds;
                for (int i = 0; i < tray.HGAItems.Count; i++)
                {
                    if (!tray.HGAItems[i].IsDefect || DefectCollection.CheckSkipDefect(defectNames[tray.HGAItems[i].VmiResult].Name))
                    {
                        int position = tray.HGAItems[i].Position;
                        aqtray.HGAIdList[position - 1] = tray.HGAItems[i].SerialNumber;
                        if ((aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA) || (aqtray.HGAIdList[position - 1] == AQTrayInfo.NOHGA_SERIAL_10OCR)) 
                        {
                            //aqtray.HGAIdList[position - 1] = AQTrayInfo.GOODHGA_NOSERIAL;
                            aqtray.HGAIdList[position - 1] = AQTrayInfo.GOODHGA_CANNOTREAD_SERIAL_10OCR;
                        }
                    }
                }

                string path_product = Path.Combine(aqtray.BasePathEasy, tray.Product);
                aqtray.WriteToAQTrayFile(path_product, aqtray.TrayId + ".LOG");
                aqtray.WriteToAQTrayFile(aqtray.BasePathMt2, aqtray.TrayId + ".mt2");
                aqtray.WriteToAQTrayFile(aqtray.BasePathMirror, aqtray.TrayId + ".LOG");
                aqtray.WriteToAQTrayFile(aqtray.BasePathBackup, aqtray.TrayId + "-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".LOG");
            }
        }

        public void ChangeViewPalletOrder(int palletOrder)
        {
            if (palletOrder < 1 || palletOrder > 6) return;
            if (HGAList == null || HGAList.Count == 0 || HGAList[0].HGATrays[0] == null) return;
            HGAList[0].HGATrays[0].CurrentRowColumnInTray = palletOrder;
            HGAList[0].HGATrays[0].ItemsView.Refresh();
        }
    }

}
