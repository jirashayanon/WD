using GalaSoft.MvvmLight;
using PhotoLibrary.Helpers;
using PhotoLibrary.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoLibrary.Model
{
    public sealed class Defect : ObservableObject
    {
        public const int LEAST_PRIORITY = 1000000;
        private static readonly object padlock = new object();

        #region Properties

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

        private int _priority;
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                if (_priority == value)
                    return;
                _priority = value;
                RaisePropertyChanged("Priority");
            }
        }

        #endregion

        private static Dictionary<string, Defect> _defectPriority;
        public static Dictionary<string, Defect> DefectPriority
        {
            get
            {
                lock (padlock)
                {
                    if (_defectPriority == null)
                    {
                        dataAccess = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IRepository>();
                        Dictionary<int, Defect> dict = dataAccess.getDefectFromDB();
                        _defectPriority = new Dictionary<string, Defect>();
                        foreach (var x in dict)
                        {
                            try
                            {
                                _defectPriority.Add(x.Value.Name, x.Value);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.AppendErrorFile("Duplicate defect name (" + x.Value.Name + ") in database: " + ex.ToString(), true);
                            }
                        }
                    }
                    return _defectPriority;
                }
            }
        }
        static IRepository dataAccess;

        public Defect(string name, bool isSelected = false, int priority = 9999)
        {
            Name = name;
            IsSelected = isSelected;
            Priority = priority;
        }

        public static int GetDefectPriority(string defectName)
        {
            if (DefectPriority.ContainsKey(defectName))
            {
                return DefectPriority[defectName].Priority;
            }
            return LEAST_PRIORITY;
        }
    }
}
