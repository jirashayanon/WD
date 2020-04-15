using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PhotoLibrary.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections;
using PhotoLibrary.Helpers;

namespace PhotoLibrary.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DefectsViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCollection<Defect> Suspension { get; set; }
        public ObservableCollection<Defect> Attach { get; set; }
        public ObservableCollection<Defect> SJB { get; set; }
        public ObservableCollection<Defect> OTH { get; set; }

        #endregion

        #region ICommand

        public ICommand ClearCheckBoxCommand { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the DefectsViewModel class.
        /// </summary>
        public DefectsViewModel()
        {
            ClearCheckBoxCommand = new RelayCommand(() => ExecuteClearCheckBoxCommand());

            Suspension = new ObservableCollection<Defect>();
            Attach = new ObservableCollection<Defect>();
            SJB = new ObservableCollection<Defect>();
            OTH = new ObservableCollection<Defect>();
            LoadDefectList();
        }

        private void LoadDefectList()
        {
            DefectsXML defectsXML = new DefectsXML();
            foreach (string name in defectsXML.GetSuspension())
            {
                Suspension.Add(new Defect(name));
            }
            foreach (string name in defectsXML.GetAttach())
            {
                Attach.Add(new Defect(name));
            }
            foreach (string name in defectsXML.GetSJB())
            {
                SJB.Add(new Defect(name));
            }
            foreach (string name in defectsXML.GetOTH())
            {
                OTH.Add(new Defect(name));
            }
        }

        private void ExecuteClearCheckBoxCommand()
        {
            foreach (ObservableCollection<Defect> group in new[] { Suspension, OTH, Attach, SJB })
            {
                foreach (Defect defect in group)
                {
                    defect.IsSelected = false;
                }
            }
        }

        #region Public Method
        public List<Defect> GetSelectedDefect()
        {
            List<Defect> list = new List<Defect>();
            list.AddRange(Suspension.Where(d => d.IsSelected).ToList());
            list.AddRange(Attach.Where(d => d.IsSelected).ToList());
            list.AddRange(SJB.Where(d => d.IsSelected).ToList());
            list.AddRange(OTH.Where(d => d.IsSelected).ToList());
            return list;
        }

        public void SetDefectSelected(string defectName)
        {
            if (defectName == null) return;
            ExecuteClearCheckBoxCommand();

            foreach (ObservableCollection<Defect> group in new[] { Suspension, OTH, Attach, SJB })
            {
                foreach (Defect defect in group)
                {
                    if (defect.Name.Equals(defectName, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        defect.IsSelected = true;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}