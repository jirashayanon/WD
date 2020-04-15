using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoLibrary.Model.HGA;

namespace PhotoLibrary.Repository
{
    public interface IRepository
    {
        HGAPack SearchTrayId(string trayId);
        HGAPack SearchPalletId(string palletId);

        HGATray GetDefectsOnTray(HGATray trayObj, string trayId);
        
        Dictionary<int, List<MachineView>> InspectionView();
        List<View> GetView();

        bool SaveViewDefect(List<HGADefect> viewDefects);
        bool UpdateStatus(HGAItem hi);
        bool UpdateTrayComplete(HGATray t,int sampling);
        List<HGADefect> LoadViewDefect(long hgaId);
        Dictionary<int, PhotoLibrary.Model.Defect> getDefectFromDB();
        long InsertDefect(string defectName);

        bool UpdateSerial(HGAItem hi, string serialnumber);

        #region Deprecated

        HGAPack SearchPackId(string packId);

        #endregion
    }
}
