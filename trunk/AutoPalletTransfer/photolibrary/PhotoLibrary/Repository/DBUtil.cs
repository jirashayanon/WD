using MySql.Data.MySqlClient;
using PhotoLibrary.Model;
using PhotoLibrary.Model.HGA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PhotoLibrary.Repository
{
    class DBUtil : IRepository
    {
        public DBUtil()
        {
        }

        /// <summary>
        /// Test Function
        /// </summary>
        /// <returns></returns>
        public bool ReadData()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "SELECT * FROM HGAInformation";
            MySqlCommand filmsCommand = new MySqlCommand(mySelectQuery, dbcredential.Conn);

            MySqlDataReader reader = filmsCommand.ExecuteReader();

            while (reader.Read())
            {
                long Numero = reader.GetInt64("HGAId");
                string name = reader.GetString("SliderLotId");
                Debug.WriteLine(Numero.ToString() + "  " + name);
            }

            filmsCommand.Connection.Close();

            return true;
        }

        public bool WriteData()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "INSERT INTO Patient ( Nom , Prenom , Adresse , Telephone , Date_Naissance , Date_Ouverte) " +
                                    "VALUES('Bouhlel', 'BHL', 'Sfax', '22108069', '29/08/1991', '" +
                                DateTime.Now.Date.ToShortDateString() + "')";

            MySqlCommand filmsCommand = new MySqlCommand(mySelectQuery, dbcredential.Conn);

            filmsCommand.ExecuteNonQuery();
            filmsCommand.Connection.Close();

            return true;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public HGAPack SearchPackId(string packId)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "spSearchPackId";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?packIdparam", packId); // cast if a constant
            cmd.Parameters["?packIdparam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();

            HGAPack hp = new HGAPack(packId);
            Dictionary<string, HGATray> dictionaryTray = new Dictionary<string, HGATray>();
            Dictionary<string, HGAItem> dictionaryItem = new Dictionary<string, HGAItem>();
            while (reader.Read())
            {
                long hGAId = reader.GetInt64("HGAId");
                string name = reader.GetString("SliderLotId");
                string trayId = reader.GetString("ProcessTrayId");
                int position = reader.GetInt32("Position");
                string serialNumber = reader.GetString("SerialNumber");
                string sliderLotId = reader.GetString("SliderLotId");
                string iaviResult = reader.GetString("iaviResult");
                int vmiResult = reader.GetInt32("vmiResult");
                int isTrayComplete = reader.GetInt32("isCompleted");
                Debug.WriteLine(serialNumber);

                long inspectionDataId = reader.GetInt64("InspectionDataId");
                int machineId = reader.GetInt32("InspectionMachineId");
                string machine = reader.GetString("Machine");
                string module = reader.GetString("Module");
                DateTime datetime = reader.GetDateTime("DateTime");
                string statusFromMachine = reader.GetString("StatusFromMachine");
                string statusFromOQA = reader.GetString("StatusFromOQA");
                HGAInspection hinspect = new HGAInspection()
                {
                    InspectionDataId = inspectionDataId,
                    InspectionMachineId = machineId,
                    Machine = machine,
                    Module = module,
                    Datetime = datetime,
                    StatusFromMachine = statusFromMachine,
                    StatusFromOQA = statusFromOQA
                };

                if (dictionaryItem.ContainsKey(serialNumber))
                {
                    dictionaryItem[serialNumber].HGAInspections.Add(hinspect);
                }
                else
                {
                    //HGAItem hi = new HGAItem(hGAId, trayId, position, serialNumber, sliderLotId,
                    //    packId, status);
                    //hi.HGAInspections.Add(hinspect);
                    //dictionaryItem.Add(serialNumber, hi);
                }
                //Debug.WriteLine(hGAId.ToString() + "  " + name);
            }

            foreach (KeyValuePair<string, HGAItem> pair in dictionaryItem)
            {
                if (dictionaryTray.ContainsKey(pair.Value.TrayId))
                {
                    dictionaryTray[pair.Value.TrayId].Add(pair.Value);
                }
                else
                {
                    HGATray ht = new HGATray(pair.Value.TrayId, packId, pair.Value.IsTrayComplete);
                    ht.Add(pair.Value);
                    dictionaryTray.Add(pair.Value.TrayId, ht);
                }
            }

            foreach (KeyValuePair<string, HGATray> pair in dictionaryTray)
            {
                hp.Add(pair.Value);
            }

            cmd.Connection.Close();

            return hp;
        }

        public HGAPack SearchTrayId(string trayId)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "sp_searchTrayId";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?trayIdParam", trayId); // cast if a constant
            cmd.Parameters["?trayIdParam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;

            HGAPack hp = new HGAPack("P0");
            Dictionary<string, HGATray> dictionaryTray = new Dictionary<string, HGATray>();
            Dictionary<long, HGAItem> dictionaryItem = new Dictionary<long, HGAItem>();
            HGATray ht = new HGATray(trayId);
            bool has_data = false;

            while (reader.Read())
            {
                if (!has_data)
                {
                    int trayIndex = reader.GetInt32("trayId");
                    ht.TrayInt = trayIndex;
                    ht.HasAQHeader = SetAQtrayHeader(trayIndex, ht);
                    ht.DateTimeStart = DateTime.Now;
                }
                has_data = true;
                long hGAId = reader.GetInt64("hgainfoId");
                string lineNo = reader.GetString("lineNo");
                //string trayId = reader.GetString("palletId");
                int position = reader.GetInt32("position");
                DateTime datetime = reader.GetDateTime("processdatetime");
                string iaviResult = reader.GetString("iaviResult");
                int vmiResult = reader.GetInt32("vmiResult");
                //string statusS = reader.GetString("statusS");
                string serialNumber = reader.GetString("serialNumber");
                int isTrayComplete = reader.GetInt32("isCompleted");
                int palletIdInt = reader.GetInt32("palletId");
                string palletSN = reader.GetString("palletSN");
                //string palletId = getPalletId(palletIdInt);
                int palletOrder = reader.GetInt32("palletOrder");
                //int palletOrder = getPalletorder(palletIdInt);
                int viewId = 0;
                string imagePath = "";
                try
                {
                    viewId = reader.GetInt32("viewId");
                    imagePath = reader.GetString("imagePath");
                    int resultInt = reader.GetInt32("result");
                }
                catch (Exception ex)
                {
                    viewId = 1;
                    imagePath = "dummy.bmp";
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

            cmd.Connection.Close();

            return has_data ? hp : null;
        }

        public HGAPack SearchPalletId(string palletId)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "sp_searchPalletId";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?palletIdParam", palletId); // cast if a constant
            cmd.Parameters["?palletIdParam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();

            HGAPack hp = new HGAPack("P0");
            Dictionary<string, HGATray> dictionaryPallet = new Dictionary<string, HGATray>();
            Dictionary<long, HGAItem> dictionaryItem = new Dictionary<long, HGAItem>();
            HGATray ht = new HGATray(palletId);
            bool has_data = false;

            while (reader.Read())
            {

                has_data = true;
                long hGAId = reader.GetInt64("hgainfoId");
                string lineNo = reader.GetString("lineNo");
                //string trayId = reader.GetString("palletId");
                int position = reader.GetInt32("position");
                //int palletOrder = reader.GetInt32("palletOrder");
                DateTime datetime = reader.GetDateTime("processdatetime");
                string iaviResult = reader.GetString("iaviResult");
                int vmiResult = reader.GetInt32("vmiResult");
                //string statusS = reader.GetString("statusS");
                string serialNumber = reader.GetString("serialNumber");
                int isTrayComplete = reader.GetInt32("isCompleted");
                int viewId = 0;
                string imagePath = "";
                try
                {
                    viewId = reader.GetInt32("viewId");
                    imagePath = reader.GetString("imagePath");
                    int resultInt = reader.GetInt32("result");
                }
                catch (Exception ex)
                {
                    viewId = 1;
                    imagePath = "dummy.bmp";
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
                //Debug.WriteLine(hGAId.ToString() + "  " + name);
            }

            hp.Add(ht);

            cmd.Connection.Close();

            return has_data ? hp : null;
        }

        public HGATray GetDefectsOnTray(HGATray trayObj, string trayId)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "sp_getDefectsOnTray";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?trayIdParam", trayId); // cast if a constant
            cmd.Parameters["?trayIdParam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;

            Dictionary<long, HGAItem> dictionaryItem = new Dictionary<long, HGAItem>();

            while (reader.Read())
            {
                long hGAId = reader.GetInt64("hgainfoId");
                string lineNo = reader.GetString("lineNo");
                //string trayId = reader.GetString("palletId");
                int position = reader.GetInt32("position");
                int palletOrder = reader.GetInt32("palletOrder");
                string iaviResult = reader.GetString("iaviResult");
                int vmiResult = reader.GetInt32("vmiResult");

                string serialNumber = reader.GetString("serialNumber");
                string palletSN = reader.GetString("palletSN");
                string defectString = reader.GetString("defectString");

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

            cmd.Connection.Close();

            return trayObj;
        }

        /// <summary>
        /// Deprecated!
        /// Get Inspection Machine and View
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<MachineView>> InspectionView()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = String.Format(
                                    @"SELECT *
                                    FROM vw_machineview");
            MySqlCommand filmsCommand = new MySqlCommand(mySelectQuery, dbcredential.Conn);

            MySqlDataReader reader = filmsCommand.ExecuteReader();

            Dictionary<int, List<MachineView>> machineview = new Dictionary<int, List<MachineView>>();

            while (reader.Read())
            {
                int machineId = reader.GetInt32("InspectionMachineId");
                int viewId = reader.GetInt32("ViewId");
                string view = reader.GetString("View");
                string fileType = reader.GetString("FileType");

                if (machineview.ContainsKey(machineId))
                {
                    machineview[machineId].Add(new MachineView()
                    {
                        ViewId = viewId,
                        View = view,
                        FileType = fileType
                    });
                }
                else
                {
                    List<MachineView> oneView = new List<MachineView> { new MachineView() {
                        ViewId = viewId,
                        View = view,
                        FileType = fileType
                     }};
                    machineview.Add(machineId, oneView);
                }
            }

            filmsCommand.Connection.Close();

            return machineview;
        }

        public List<View> GetView()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = String.Format(
                                    @"SELECT *
                                    FROM view");
            MySqlCommand filmsCommand = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            MySqlDataReader reader = filmsCommand.ExecuteReader();

            List<View> views = new List<View>();
            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string view = reader.GetString("name");

                views.Add(new View()
                {
                    Id = id,
                    Name = view
                });
            }

            filmsCommand.Connection.Close();

            return views;
        }

        /// <summary>
        /// Deprecated. Old database structure.
        /// </summary>
        /// <param name="viewDefects"></param>
        /// <returns></returns>
        public bool SaveViewDefect(List<HGADefect> viewDefects)
        {
            throw new InvalidOperationException();
            // TODO: Delete fail when don't have any defect. Should pass inspectionDataId list to this method
            if (viewDefects.Count == 0)
            {
                return true;
            }

            DBCredential dbcredential = new DBCredential();
            StringBuilder insert = new StringBuilder();
            Dictionary<int, Defect> defectNames = getDefectFromDB();
            foreach (HGADefect viewDefect in viewDefects)
            {
                string deleteOld = String.Format(
                                    @"DELETE FROM hgaoqa
                                    WHERE InspectionDataId = {0};", viewDefect.InspectionDataId);
                insert.Append(deleteOld);
                string viewDefectString = viewDefect.Defect;
                int viewDefectInt = defectNames.FirstOrDefault(x => x.Value.Name == viewDefectString).Key;
                string myQuery = String.Format(
                                        @"INSERT INTO hgaoqa (InspectionDataId, ViewId, Defect, PositionRatioX, PositionRatioY) 
                                        VALUES ({0}, {1}, '{2}', {3}, {4});", viewDefect.InspectionDataId,
                                        viewDefect.ViewId, viewDefectInt, viewDefect.PositionX, viewDefect.PositionY);
                insert.Append(myQuery);
            }

            MySqlCommand filmsCommand = new MySqlCommand(insert.ToString(), dbcredential.Conn);

            filmsCommand.ExecuteNonQuery();

            filmsCommand.Connection.Close();

            return true;
        }

        public bool UpdateStatus(HGAItem hi)
        {
            DBCredential dbcredential = new DBCredential();
            StringBuilder update = new StringBuilder();

            string myUpdateInfo = String.Format(
                                    @"UPDATE hgainfo
                                    SET vmiResult='{0}'
                                    WHERE hgainfoId = {1};", hi.VmiResult, hi.HGAId);
            update.Append(myUpdateInfo);

            MySqlCommand filmsCommand = new MySqlCommand(update.ToString(), dbcredential.Conn);
            filmsCommand.ExecuteNonQuery();
            filmsCommand.Connection.Close();

            return true;
        }

        public bool UpdateTrayComplete(HGATray t,int sampling)
        {
            DBCredential dbcredential = new DBCredential();
            StringBuilder update = new StringBuilder();
            string myUpdateInfo = String.Format(
                                    @"UPDATE trays
                                    SET isCompleted={0},isSampling={1} WHERE traySN='{2}'
                                    ORDER BY id DESC
                                    LIMIT 1;", 1,sampling, t.TrayId);
            update.Append(myUpdateInfo);
            MySqlCommand filmsCommand = new MySqlCommand(update.ToString(), dbcredential.Conn);
            filmsCommand.ExecuteNonQuery();
            filmsCommand.Connection.Close();

            return true;
        }

        public bool UpdateSerial(HGAItem hi, string serialnumber)
        {
            DBCredential dbcredential = new DBCredential();
            StringBuilder update = new StringBuilder();
            string myUpdateInfo = String.Format(
                                    @"UPDATE hgainfo
                                    SET serialNumber='{1}'
                                    WHERE hgainfoId={0};", hi.HGAId, serialnumber);
            update.Append(myUpdateInfo);
            MySqlCommand filmsCommand = new MySqlCommand(update.ToString(), dbcredential.Conn);
            filmsCommand.ExecuteNonQuery();
            filmsCommand.Connection.Close();

            return true;
        }


        public List<HGADefect> LoadViewDefect(long hgaId)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "spLoadDefectHGA";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?hgaIdParam", hgaId); // cast if a constant
            cmd.Parameters["?hgaIdParam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();
            Dictionary<int, Defect> defectNames = getDefectFromDB();
            List<HGADefect> hgaoqa = new List<HGADefect>();

            while (reader.Read())
            {
                //long inspectionDataId = reader.GetInt64("Id");
                int viewId = reader.GetInt32("viewId");
                //string view = reader.GetString("View");
                int defectInt = reader.GetInt32("defectId");
                string defect = defectNames[defectInt].Name;
                double positionRatioX = reader.GetDouble("coordinateX");
                double positionRatioY = reader.GetDouble("coordinateY");

                hgaoqa.Add(new HGADefect()
                {
                    //InspectionDataId = inspectionDataId,
                    ViewId = viewId,
                    //View = view,
                    Defect = defect,
                    PositionX = positionRatioX,
                    PositionY = positionRatioY
                });
            }

            cmd.Connection.Close();

            return hgaoqa;
        }

        [Obsolete("Example of calling stored procedure", true)]
        public bool CallStoredProcedure()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = "SearchPackId";
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("?packIdparam", "4"); // cast if a constant
            cmd.Parameters["?packIdparam"].Direction = System.Data.ParameterDirection.Input;

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                long Numero = reader.GetInt64("HGAId");
                string name = reader.GetString("SliderLotId");
                Debug.WriteLine(Numero.ToString() + "  " + name);
            }

            cmd.Connection.Close();

            return true;
        }

        /// <summary>
        /// Insert new defect to `defect` table. Return inserted id.
        /// </summary>
        /// <param name="defectName"></param>
        /// <returns></returns>
        public long InsertDefect(string defectName)
        {
            DBCredential dbcredential = new DBCredential();

            string insertViewCommand = @"INSERT INTO defect (defectName)" +
                                        "VALUES (@param_defectName);";

            MySqlCommand mCommand = new MySqlCommand(insertViewCommand, dbcredential.Conn);
            mCommand.Parameters.AddWithValue("@param_defectName", defectName);
            mCommand.ExecuteNonQuery();
            long defect_id = mCommand.LastInsertedId;
            return defect_id;
        }

        public Dictionary<int, Defect> getDefectFromDB()
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = String.Format(
                                    @"SELECT *
                                    FROM defect");
            MySqlCommand filmsCommand = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            MySqlDataReader reader = filmsCommand.ExecuteReader();
            Dictionary<int, Defect> defectNames = new Dictionary<int, Defect>();
            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string defectName = reader.GetString("defectName");
                int priority = reader.GetInt32("priority");

                defectNames.Add(id, new Defect(defectName, priority: priority));
            }
            filmsCommand.Connection.Close();
            return defectNames;
        }

        /// <summary>
        /// Search by trayId (int). Set all header information to Tray object.
        /// </summary>
        /// <param name="trayId"></param>
        /// <param name="tray">Target tray object to keep all data</param>
        /// <returns></returns>
        public bool SetAQtrayHeader(int trayId, HGATray tray)
        {
            DBCredential dbcredential = new DBCredential();

            string mySelectQuery = String.Format(
                                    @"SELECT *
                                    FROM aqtraydata where TrayId={0}", trayId);
            MySqlCommand cmd = new MySqlCommand(mySelectQuery, dbcredential.Conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool hasAQHeader = false;
            while (reader.Read())
            {
                //long inspectionDataId = reader.GetInt64("Id");
                string date = reader.GetString("Date");
                string timeStart = reader.GetString("TimeStart");
                string timeEnd = reader.GetString("TimeEnd");
                string usedTime = reader.GetString("UsedTime");
                string testerNumber = reader.GetString("TesterNumber");
                string customer = reader.GetString("Customer");
                string product = reader.GetString("Product");
                string user = reader.GetString("User");
                string lotNumber = reader.GetString("Lotnumber");
                string docControl1 = reader.GetString("DocControl1");
                string docControl2 = reader.GetString("DocControl2");
                string sus = reader.GetString("Sus");
                string assyLine = reader.GetString("AssyLine");

                tray.Date = date;
                tray.TimeStart = timeStart;
                tray.TimeEnd = timeEnd;
                tray.UsedTime = usedTime;
                tray.TesterNumber = testerNumber;
                tray.Customer = customer;
                tray.Product = product;
                tray.User = user;
                tray.LotNumber = lotNumber;
                tray.DocControl1 = docControl1;
                tray.DocControl2 = docControl2;
                tray.Sus = sus;
                tray.AssyLine = assyLine;

                hasAQHeader = true;
            }

            cmd.Connection.Close();

            return hasAQHeader;
        }
    }
}
