using PhotoLibrary.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Repository
{
    class BothDB : IRepository
    {
        SQLServer sqlserver;
        DBUtil mysql;
        public BothDB()
        {
            sqlserver = new SQLServer();
            mysql = new DBUtil();
        }


        public Model.HGA.HGAPack SearchTrayId(string trayId)
        {
            PhotoLibrary.Model.HGA.HGAPack pack_sqlserver = sqlserver.SearchTrayId(trayId);
            PhotoLibrary.Model.HGA.HGAPack pack_mysql = mysql.SearchTrayId(trayId);
            pack_mysql.HGATrays[0].TrayInt2 = pack_sqlserver.HGATrays[0].TrayInt;
            for (int i = 0; i < pack_mysql.HGATrays[0].HGAItems.Count; i++)
            {
                pack_mysql.HGATrays[0].HGAItems[i].HGAId2 = pack_sqlserver.HGATrays[0].HGAItems[i].HGAId;
            }
            
            return pack_mysql;  // return with TrayInt2 and HGAId2 are from sql server
        }

        public Model.HGA.HGAPack SearchPalletId(string palletId)
        {
            sqlserver.SearchPalletId(palletId);
            return mysql.SearchPalletId(palletId);
        }

        public Model.HGA.HGATray GetDefectsOnTray(Model.HGA.HGATray trayObj, string trayId)
        {
            sqlserver.GetDefectsOnTray(trayObj, trayId);
            return mysql.GetDefectsOnTray(trayObj, trayId);
        }

        [Obsolete("Old Database Design", true)]
        public Dictionary<int, List<Model.HGA.MachineView>> InspectionView()
        {
            throw new NotImplementedException();
        }

        public List<Model.HGA.View> GetView()
        {
            sqlserver.GetView();
            return mysql.GetView();
        }

        [Obsolete("Old Database Design", true)]
        public bool SaveViewDefect(List<Model.HGA.HGADefect> viewDefects)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStatus(Model.HGA.HGAItem hi)
        {
            sqlserver.UpdateStatus(hi);
            return mysql.UpdateStatus(hi);
        }

        public bool UpdateTrayComplete(Model.HGA.HGATray t,int sampling)
        {
            sqlserver.UpdateTrayComplete(t,sampling);
            return mysql.UpdateTrayComplete(t,sampling);
        }

        public List<Model.HGA.HGADefect> LoadViewDefect(long hgaId)
        {
            sqlserver.LoadViewDefect(hgaId);
            return mysql.LoadViewDefect(hgaId);
        }

        public Dictionary<int, Defect> getDefectFromDB()
        {
            sqlserver.getDefectFromDB();
            return mysql.getDefectFromDB();
        }

        public long InsertDefect(string defectName)
        {
            sqlserver.InsertDefect(defectName);
            return mysql.InsertDefect(defectName);
        }

        public bool UpdateSerial(PhotoLibrary.Model.HGA.HGAItem hi, string serialnumber)
        {
            sqlserver.UpdateSerial(hi, serialnumber);
            return mysql.UpdateSerial(hi, serialnumber);
        }

        [Obsolete("No use", true)]
        public Model.HGA.HGAPack SearchPackId(string packId)
        {
            throw new NotImplementedException();
        }
    }
}
