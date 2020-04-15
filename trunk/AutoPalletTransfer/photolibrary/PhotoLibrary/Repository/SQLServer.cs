using PhotoLibrary.Helpers;
using PhotoLibrary.Model.HGA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Repository
{
    public class SQLServer : IRepository
    {
        public Model.HGA.HGAPack SearchTrayId(string trayId)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                //var parameter = new System.Data.SqlClient.SqlParameter("@trayIdParam", trayId);
                //var results = db.Database.SqlQuery<spSearchTrayId_Result>("spSearchTrayId @trayIdParam", parameter);
                var searchTrayParam = new System.Data.SqlClient.SqlParameter("@trayIdParam", trayId);
                var results = db.Database.SqlQuery<spSearchTrayId_Result>("spSearchTrayId @trayIdParam", searchTrayParam);


                HGAPack hp = new HGAPack("P0");
                Dictionary<string, HGATray> dictionaryTray = new Dictionary<string, HGATray>();
                Dictionary<long, HGAItem> dictionaryItem = new Dictionary<long, HGAItem>();

                HGATray ht = new HGATray(trayId);
                bool has_data = false;

                foreach (var row in results)
                {
                    if (!has_data)
                    {
                        ht.TrayInt = row.TrayId;
                        ht.HasAQHeader = GetAQtrayHeader(row.TrayId, ht);
                        ht.DateTimeStart = DateTime.Now;
                    }
                    has_data = true;
                    long hGAId = row.Id;
                    string lineNo = row.LineNo;
                    //string trayId = reader.GetString("palletId");
                    int position = row.Position;
                    DateTime datetime = row.Processdatetime;
                    string iaviResult = row.IaviResult;
                    int vmiResult = row.VmiResult;
                    //string statusS = reader.GetString("statusS");
                    string serialNumber = row.SerialNumber;
                    int isTrayComplete = row.IsCompleted;
                    int palletIdInt = row.PalletId;
                    string palletSN = row.PalletSN;
                    //string palletId = getPalletId(palletIdInt);
                    int palletOrder = row.PalletOrder ?? 0;
                    //int palletOrder = getPalletorder(palletIdInt);
                    int viewId = 0;
                    string imagePath = "";
                    if (row.ViewId != null)
                    {
                        viewId = (int)row.ViewId;
                        imagePath = row.ImagePath;
                    }
                    else
                    {
                        viewId = 1;
                        imagePath = "dummy.bmp";
                    }

                    Helpers.ConfigXML config = new Helpers.ConfigXML();
                    imagePath = imagePath.Replace(".bmp", "." + config.PhotoFileType);

                    //Debug.WriteLine(serialNumber);

                    HGAViewData viewData = new HGAViewData()
                    {
                        HgainfoId = hGAId,
                        ImagePath = imagePath,
                        Result = "GOOD",
                        ViewId = viewId
                    };
                    if (dictionaryItem.ContainsKey(hGAId))
                    {
                        //dictionaryItem[serialNumber].HGAInspections.Add(hinspect);
                        dictionaryItem[hGAId].HGAViews.Add(viewData);
                    }
                    else if (serialNumber != "--------")
                    {
                        HGAItem hi = new HGAItem(hGAId, trayId, position, serialNumber: serialNumber, vmiResult: vmiResult, iaviResult: iaviResult, isTrayComplete: isTrayComplete, palletOrder: palletOrder);
                        //hi.HGAInspections.Add(hinspect);
                        hi.HGAViews.Add(viewData);
                        dictionaryItem.Add(hGAId, hi);
                        ht.Add(hi);
                        if (!ht.palletIds.Contains(palletSN))
                        {
                            ht.palletIds.Add(palletSN);
                        }
                        ht.IsComplete = hi.IsTrayComplete;
                    }
                    //Debug.WriteLine(hGAId.ToString() + "  " + name);
                }

                hp.Add(ht);
                return has_data ? hp : null;
            }
        }

        public Model.HGA.HGAPack SearchPalletId(string palletId)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var parameter = new System.Data.SqlClient.SqlParameter("@palletIdParam", palletId);
                var results = db.Database.SqlQuery<spSearchPalletId_Result>("spSearchPalletId @palletIdParam", parameter);

                HGAPack hp = new HGAPack("P0");
                Dictionary<long, HGAItem> dictionaryItem = new Dictionary<long, HGAItem>();
                HGATray ht = new HGATray(palletId);

                bool has_data = false;
                foreach (var row in results)
                {
                    has_data = true;
                    long hGAId = row.Id;
                    string lineNo = row.LineNo;
                    //string trayId = reader.GetString("palletId");
                    int position = row.Position;
                    //int palletOrder = reader.GetInt32("palletOrder");
                    DateTime datetime = row.Processdatetime;
                    string iaviResult = row.IaviResult;
                    int vmiResult = row.VmiResult;
                    //string statusS = reader.GetString("statusS");
                    string serialNumber = row.SerialNumber;
                    int isTrayComplete = row.isCompleted;

                    int viewId = 0;
                    string imagePath = "";
                    try
                    {
                        // TODO: Check this condition
                        viewId = row.viewId ?? 0;
                        imagePath = row.imagePath;
                    }
                    catch (Exception ex)
                    {
                        viewId = 1;
                        imagePath = "dummy.bmp";
                        LogHelper.AppendErrorFile("" + ex.ToString());
                    }
                    Helpers.ConfigXML config = new Helpers.ConfigXML();
                    imagePath = imagePath.Replace(".bmp", "." + config.PhotoFileType);

                    Debug.WriteLine(serialNumber);

                    HGAViewData viewData = new HGAViewData()
                    {
                        HgainfoId = hGAId,
                        ImagePath = imagePath,
                        Result = "GOOD",
                        ViewId = viewId
                    };
                    if (dictionaryItem.ContainsKey(hGAId))
                    {
                        //dictionaryItem[serialNumber].HGAInspections.Add(hinspect);
                        dictionaryItem[hGAId].HGAViews.Add(viewData);
                    }
                    else if (serialNumber != "--------")
                    {
                        HGAItem hi = new HGAItem(hGAId, palletId, position, serialNumber: serialNumber, vmiResult: vmiResult, iaviResult: iaviResult, isTrayComplete: isTrayComplete);
                        //hi.HGAInspections.Add(hinspect);
                        hi.Position = (hi.Position > 10 && hi.Position != 20) ? hi.Position % 10 : hi.Position;
                        hi.Position = hi.Position == 20 ? 10 : hi.Position;
                        hi.Canvas_Y = 1.0 / 6;
                        hi.Canvas_X = (120 + 14 + (hi.Position - 1) * 71.75) / 992 + 0.0033;
                        hi.HGAWidth = 71.5 / 992;
                        hi.HGAHeight = hi.HGAHeight / 0.5;
                        hi.HGAViews.Add(viewData);
                        dictionaryItem.Add(hGAId, hi);
                        ht.Add(hi);
                    }
                }
                hp.Add(ht);

                return has_data ? hp : null;
            }
        }

        public Model.HGA.HGATray GetDefectsOnTray(Model.HGA.HGATray trayObj, string trayId)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var parameter = new System.Data.SqlClient.SqlParameter("@trayIdParam", trayObj.TrayId);
                var results = db.Database.SqlQuery<spGetDefectsOnTray_Result>("spGetDefectsOnTray @trayIdParam", parameter);

                foreach (var row in results)
                {
                    long hGAId = row.Id;
                    int position = row.Position;
                    int? palletOrder = row.PalletOrder;
                    string defectString = row.DefectString;

                    if (row.PalletOrder == null)
                    {
                        LogHelper.AppendErrorFile("PalletOrder is null at TrayInt: " + trayObj.TrayInt, true);
                    }

                    HGAItem item = trayObj.HGAItems.FirstOrDefault(x => x.Position == position + 10 * (palletOrder - 1));
                    if (item != null)
                    {
                        string[] defects = defectString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string defectName in defects)
                        {
                            if (!(item.defectsList.Contains(defectName)))
                            {
                                item.defectsList.Add(defectName);
                            }
                        }
                    }
                }

                return trayObj;
            }
        }

        [Obsolete("Old Database Design", true)]
        public Dictionary<int, List<Model.HGA.MachineView>> InspectionView()
        {
            throw new NotImplementedException();
        }

        public List<Model.HGA.View> GetView()
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var query = from b in db.views
                            select b;
                return query
                    .Select(x => new Model.HGA.View() { Id = x.Id, Name = x.Name })
                    .ToList();
            }
        }

        [Obsolete("Old Database Design", true)]
        public bool SaveViewDefect(List<Model.HGA.HGADefect> viewDefects)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStatus(Model.HGA.HGAItem hi)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var result = db.hgainfoes.SingleOrDefault(b => b.Id == hi.HGAId);
                if (result != null)
                {
                    result.VmiResult = hi.VmiResult;
                    db.SaveChanges();
                    return true;
                }
                else return false;
            }
        }

        public bool UpdateTrayComplete(Model.HGA.HGATray t,int sampling)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var result = db.trays.SingleOrDefault(b => b.Id == t.TrayInt);
                if (result != null)
                {
                    result.IsCompleted = 1;
                    result.IsSampling = sampling;
                    db.SaveChanges();
                    return true;
                }
                else return false;
            }
        }

        public List<Model.HGA.HGADefect> LoadViewDefect(long hgaId)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var parameter = new System.Data.SqlClient.SqlParameter("@hgaIdParam", hgaId);
                var results = db.Database.SqlQuery<spLoadDefectHGA_Result>("spLoadDefectHGA @hgaIdParam", parameter);

                Dictionary<int, PhotoLibrary.Model.Defect> defectNames = getDefectFromDB();
                List<HGADefect> hgaoqa = new List<HGADefect>();

                foreach (var row in results)
                {
                    int defectInt = row.defectId;
                    string defect = defectNames[defectInt].Name;

                    hgaoqa.Add(new HGADefect()
                    {
                        ViewId = row.viewId,
                        Defect = defect,
                        PositionX = row.coordinateX,
                        PositionY = row.coordinateY
                    });
                }

                return hgaoqa;
            }
        }

        public Dictionary<int, PhotoLibrary.Model.Defect> getDefectFromDB()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var query = from b in db.defects
                            select b;
                //var y = query.ToDictionary(x => x.Id, x => x.DefectName);
                Dictionary<int, PhotoLibrary.Model.Defect> result = new Dictionary<int, PhotoLibrary.Model.Defect>();
                foreach (var x in query)
                {
                    //result.Add(x.Id, x.DefectName);
                    result.Add(x.Id, new PhotoLibrary.Model.Defect(x.DefectName, priority: x.Priority));
                    Debug.WriteLine(x.Id + " " + x.DefectName);
                }

                sw.Stop();
                LogHelper.AppendDebugFile("GetDefectFromDB SQL: " + sw.ElapsedMilliseconds + "ms.");
                return result;
            }

        }

        /// <summary>
        /// Insert new defect to `defect` table. Return inserted id.
        /// </summary>
        /// <param name="defectName"></param>
        /// <returns></returns>
        public long InsertDefect(string defectName)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var obj = new defect { DefectName = defectName };
                db.defects.Add(obj);
                db.SaveChanges();
                return obj.Id;
            }
        }

        public bool UpdateSerial(HGAItem hi, string serialnumber)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var result = db.hgainfoes.SingleOrDefault(b => b.Id == hi.HGAId);
                if (result != null)
                {
                    result.SerialNumber = serialnumber;
                    db.SaveChanges();
                    return true;
                }
                else return false;
            }
        }

        [Obsolete("No use", true)]
        public Model.HGA.HGAPack SearchPackId(string packId)
        {
            throw new NotImplementedException();
        }


        public bool GetAQtrayHeader(int trayId, HGATray tray)
        {
            using (var db = new TempPalletTrackingDB_2Entities())
            {
                var query = from b in db.aqtraydatas
                            where b.TrayId == trayId
                            select b;
                bool hasAQHeader = false;
                foreach (var item in query)
                {
                    tray.Date = item.Date;
                    tray.TimeStart = item.TimeStart;
                    tray.TimeEnd = item.TimeEnd;
                    tray.UsedTime = item.UsedTime;
                    tray.TesterNumber = item.TesterNumber;
                    tray.Customer = item.Customer;
                    tray.Product = item.Product;
                    tray.User = item.User;
                    tray.LotNumber = item.LotNumber;
                    tray.DocControl1 = item.DocControl1;
                    tray.DocControl2 = item.DocControl2;
                    tray.Sus = item.Sus;
                    tray.AssyLine = item.AssyLine;

                    hasAQHeader = true;
                }
                return hasAQHeader;
            }
        }
    }
}
