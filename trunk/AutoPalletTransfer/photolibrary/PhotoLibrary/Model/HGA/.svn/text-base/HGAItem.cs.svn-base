using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoLibrary.Helpers;
using PhotoLibrary.Repository;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using System.ComponentModel;

namespace PhotoLibrary.Model.HGA
{
    public class HGAItem : ObservableObject, INotifyPropertyChanged
    {
        public long HGAId { get; set; }
        public long HGAId2 { get; set; }
        public string TrayId { get; set; }
        public int Position { get; set; }
        public int PalletOrder { get; set; }
        public int IsTrayComplete { get; set; }
        private string _serialNumber;
        public List<string> defectsList { get; set; }
        Dictionary<int, Defect> defectDict;
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                if (_serialNumber == value)
                    return;
                _serialNumber = value;
                RaisePropertyChanged("SerialNumber");
            }
        }
        public string SliderLotId { get; set; }
        public string PackId { get; set; }

        private bool _isDefect;
        public bool IsDefect
        {
            get
            {
                return _isDefect;
            }
            set
            {
                _isDefect = value;
                RaisePropertyChanged("IsDefect");
            }
        }

        private double _canvas_X;
        public double Canvas_X
        {
            get
            {
                return _canvas_X;
            }
            set
            {
                _canvas_X = value;
                RaisePropertyChanged("Canvas_X");
            }
        }
        private double _canvas_Y;
        public double Canvas_Y
        {
            get
            {
                return _canvas_Y;
            }
            set
            {
                _canvas_Y = value;
                RaisePropertyChanged("Canvas_Y");
            }
        }

        private double _hGAWidth;
        public double HGAWidth
        {
            get
            {
                return _hGAWidth;
            }
            set
            {
                _hGAWidth = value;
                RaisePropertyChanged("HGAWidth");
            }
        }
        private double _hGAHeight;
        public double HGAHeight
        {
            get
            {
                return _hGAHeight;
            }
            set
            {
                _hGAHeight = value;
                RaisePropertyChanged("HGAHeight");
            }
        }

        private int _vmiResult;
        /// <summary>
        /// 0:None, 1:Pending, 2:Good, 3:Reject
        /// </summary>
        public int VmiResult
        {
            get
            {
                return _vmiResult;
            }
            set
            {
                _vmiResult = value;
                defectDict = dataAccess.getDefectFromDB();
                VmiResultString = defectDict[_vmiResult].Name;
                RaisePropertyChanged("VmiResult");
            }
        }
        private string _vmiResultString;
        public string VmiResultString
        {
            get
            {
                return _vmiResultString;
            }
            set
            {
                _vmiResultString = value;
                RaisePropertyChanged("VmiResultString");
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public string ImagePath { get; set; }

        public List<HGAInspection> HGAInspections { get; set; }
        public List<HGAViewData> HGAViews { get; set; }

        public string DefectName { get; set; }

        IRepository dataAccess;
        public HGAItem(long hGAId, string trayId, int position,
            string serialNumber = null, string sliderLotId = null, string packId = null, int vmiResult = 1, string iaviResult = null, int isTrayComplete = 0, int palletOrder = 1)
        {
            //dataAccess = new DBUtil();
            dataAccess = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IRepository>();

            HGAId = hGAId;
            TrayId = trayId;
            Position = position + 10 * (palletOrder - 1);
            PalletOrder = palletOrder;
            SerialNumber = serialNumber;
            SliderLotId = sliderLotId;
            PackId = packId;

            DefectName = iaviResult;
            VmiResult = vmiResult;

            IsDefect = vmiResult == 1 ? ((iaviResult == null) ? false : !iaviResult.Equals("Good", StringComparison.CurrentCultureIgnoreCase)) : (vmiResult == 2 ? false : true);
            if (vmiResult > 2)
            {
                IsDefect = !DefectCollection.CheckSkipDefect(VmiResultString);
            }

            IsTrayComplete = isTrayComplete;
            double left_offset = HGATray.TRAY20_PHOTO_LEFTOFFSET;
            if (trayId[0] == '6')
            {
                left_offset = HGATray.TRAY60_PHOTO_LEFTOFFSET;
                int hga_in_row = 20;
                Canvas_X = 1.0 * ((Position - 1) % hga_in_row) / hga_in_row * (1 - left_offset * 2) + left_offset;
                Canvas_Y = 1.0 / 3 * ((Position - 1) / hga_in_row);
                HGAWidth = (1 - left_offset * 2) / hga_in_row;
                HGAHeight = 1.0/ 3 - 0.05;
            }
            else if (trayId[0] == '4')
            {
                left_offset = HGATray.TRAY40_PHOTO_LEFTOFFSET;
                int hga_in_row = 20;
                Canvas_X = 1.0 * ((Position - 1) % hga_in_row) / hga_in_row * (1 - left_offset * 2) + left_offset;
                Canvas_Y = Position > hga_in_row ? 0.75 - 1.0 / 12.0 - 1.0 / 17.0 : 0.25 - 1.0 / 12.0 - 1.0 / 17.0;
                HGAWidth = (1 - left_offset * 2) / hga_in_row;
                HGAHeight = 0.3 + 1.0 / 17.0;         //?????
            }
            else
            {
                left_offset = HGATray.TRAY20_PHOTO_LEFTOFFSET;
                int hga_in_row = 10;
                Canvas_X = 1.0 * (((Position - 1) % 10) * 2) / 20 * (1 - left_offset * 2) + left_offset;
                Canvas_Y = Position > hga_in_row ? 0.75 - 1.0 / 12.0 - 1.0 / 17.0 : 0.25 - 1.0 / 12.0 - 1.0 / 17.0;
                HGAWidth = (1 - left_offset * 2) / hga_in_row;
                HGAHeight = 0.3 + 1.0 / 17.0;
            }
            //HGAInspections = new List<HGAInspection>();
            HGAViews = new List<HGAViewData>();
            defectsList = new List<string>();
        }

        internal List<MachineView> Views { get; private set; }
        internal List<string> Paths { get; private set; }

        /// <summary>
        /// Deprecated.
        /// Return list of HGA paths in the form "datetime/sliderLot/..."
        /// Also return list of view of HGA associated with HGA path such as 'FrontView'.
        /// </summary>
        /// <param name="listView"></param>
        /// <returns></returns>
        [Obsolete("Path now is kept in database")]
        public List<string> GetPathHGA(out List<MachineView> listView)
        {
            listView = new List<MachineView>();
            Views = new List<MachineView>();
            try
            {
                Paths = new List<string>();
                foreach (HGAInspection hinspect in this.HGAInspections)
                {
                    foreach (MachineView view in MachineViewDictionary.Instance[hinspect.InspectionMachineId])
                    {
                        listView.Add(view);
                        Views.Add(view);

                        StringBuilder builder = new StringBuilder();
                        string formatDatetime = "yyyyMMdd";
                        string datetime = hinspect.Datetime.ToString(formatDatetime);
                        builder.Append(datetime).Append("/");

                        builder.Append(hinspect.Machine).Append("/");

                        builder.Append(this.SliderLotId).Append("/");

                        builder.Append(hinspect.Module).Append("/");

                        builder.Append(view.View).Append("/");

                        builder.Append(this.SerialNumber).Append("_");

                        // list defect
                        // pass

                        builder.Append("_");
                        builder.Append(this.TrayId).Append("_");
                        builder.Append(this.Position).Append("_");

                        builder.Append(view.View).Append(".");

                        builder.Append(view.FileType);

                        Paths.Add(builder.ToString());
                    }
                }
                return Paths;
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                LogHelper.AppendErrorFile(ex.ToString());
            }
            return null;
        }


        public List<string> GetPaths(out List<View> listView)
        {
            listView = new List<View>();
            //Views = new List<View>();
            try
            {
                Paths = new List<string>();
                foreach (HGAViewData viewData in this.HGAViews)
                {
                    listView.Add(new View()
                    {
                        Id = viewData.ViewId
                    });
                    Paths.Add(viewData.ImagePath);
                }
                return Paths;
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                LogHelper.AppendErrorFile(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// Deprecated!
        /// Return list of inspectionDataId of this HGA corresponding to view.
        /// So, inspectionDataId may appear more than one if there are many views.
        /// </summary>
        /// <returns></returns>
        internal List<long> GetInspectionDataIdFromHGA()
        {
            List<long> listInspectionDataId = new List<long>();
            foreach (HGAInspection hinspect in this.HGAInspections)
            {
                foreach (MachineView view in MachineViewDictionary.Instance[hinspect.InspectionMachineId])
                {
                    listInspectionDataId.Add(hinspect.InspectionDataId);
                }
            }
            return listInspectionDataId;
        }

        internal void SaveHGAItemToDatabase(IRepository dataAccess, SmartObservableCollection<ImageWithDefect> ItemsImage)
        {
            List<HGADefect> hgaoqa = new List<HGADefect>();
            bool hasDefect = false;
            foreach (ImageWithDefect imgDefect in ItemsImage)
            {
                foreach (DefectsArea da in imgDefect.ItemsDefects)
                {
                    string defect = string.Join("-", da.ListDefects.ToList().ConvertAll<string>(s => s.Name));
                    hgaoqa.Add(new HGADefect()
                    {
                        InspectionDataId = imgDefect.InspectionDataId,
                        ViewId = imgDefect.ViewId,
                        View = imgDefect.View,
                        Defect = defect,
                        PositionX = da.PositionRatio.X,
                        PositionY = da.PositionRatio.Y
                    });
                }
                if (imgDefect.ItemsDefects.Count > 0)
                {
                    this.HGAInspections.Where(x => x.InspectionDataId == imgDefect.InspectionDataId)
                        .First().StatusFromOQA = "Reject";
                    hasDefect = true;
                }
            }

            if (hasDefect)
            {
                //this.Status = "Reject";
            }
            else
            {
                //this.Status = "Good";
            }

            dataAccess.SaveViewDefect(hgaoqa);
            dataAccess.UpdateStatus(this);
        }

        /// <summary>
        /// Update status field in databse with status_defect
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="status_defect">true if have defect; otherwise, false.</param>
        internal void UpdateStatus(IRepository dataAccess, bool status_defect)
        {
            dataAccess.UpdateStatus(this);
        }

        internal void LoadHGADefectFromDatabaseToItemsImage(IRepository dataAccess, SmartObservableCollection<ImageWithDefect> ItemsImage)
        {
            List<HGADefect> hgaoqas = dataAccess.LoadViewDefect(this.HGAId);
            foreach (HGADefect oqa in hgaoqas)
            {
                //string[] strListDefect = oqa.Defect.Split('-');
                //List<string> list = strListDefect.ToList();
                //List<Defect> listDefect = list.ConvertAll<Defect>(d => new Defect(d));

                Point positionRatio = new Point(oqa.PositionX, oqa.PositionY);

                DefectsArea tempDefectsArea = new DefectsArea(positionRatio, new List<Defect>()
                {
                    new Defect(oqa.Defect)
                });

                foreach (ImageWithDefect ii in ItemsImage)
                {
                    if (ii.ViewId == oqa.ViewId)
                    {
                        tempDefectsArea.PositionRatio = new Point(tempDefectsArea.PositionRatio.X / ii.Image.Width, tempDefectsArea.PositionRatio.Y / ii.Image.Height);
                        ii.AddToItemsDefects(tempDefectsArea);
                    }
                    foreach (string defect in ii.ItemsDefectsStringUnique)
                    {
                        if (!(defectsList.Contains(defect)))
                        {
                            defectsList.Add(defect);
                        }
                    }
                }
            }
        }

        public string GetTopDefect()
        {
            Dictionary<string, Defect> dict2 = Defect.DefectPriority;

            if (defectsList.Count == 0) return null;
            Defect top = new Defect(defectsList[0], priority: dict2[defectsList[0]].Priority);
            foreach (string defect in defectsList)
            {
                if (dict2[defect].Priority < top.Priority)
                {
                    top = new Defect(defect, priority: dict2[defect].Priority);
                }
            }
            return top.Name;
        }
    }
}
