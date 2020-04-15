﻿using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using System.Threading;

using WDConnect.Application;
using WDConnect.Common;
using WDHelpers.ConfigHelper;
using WDHelpers.Mitecs3Helper;

using System.Collections;

using MITECS3.Data.SvcConn;
using ARB_Host.MITECSWebService;

using System.Data.SqlClient;

namespace ARB_Host
{
    public partial class MainWindow : Form
    {
        List<hostController> ListOfHost = null;
        private HostConfigHelper _hostConfig = new HostConfigHelper();
        private Thread _aslvCheckOnACAMRequest = null;

        public MainWindow()
        {
            InitializeComponent();

            ListOfHost = ToolModel.GetAllHostController();
            lblWebServiceStatus.Text = lblWebServiceStatus.Text + _init();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            LoggerClass.Instance.MainLogInfo("MainWindow_Load");
            SubScribeAllTool();
            BindToolDetailToGrid();

            PrimaryMessageTextBox.Text = string.Empty;
            SecondaryMessageTextBox.Text = string.Empty;
            ErrorMessageTextBox.Text = string.Empty;

            clearUITextData();

            _hostConfig = HostConfigHelper.ReadFromFile(exePath + @"\HostConfig.xml");

            //Read line id from config before registering to mitecs
            _hostConfig.LineNo = txtboxLine.Text = (_hostConfig.LineNo.Length > 0) ? _hostConfig.LineNo : @"CP4A2";

            //this.btnRegisterLine_Click(sender, e);
            try
            {
                if (!_hostConfig.RunWithNoDatabase)
                {
                    this.btnGetLots_Click(sender, e);
                }
                else
                {
                    this.btnGetLotsOffline_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
            }


            //_strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //_strConnectionString += ";Password=qwerty;";

            _strConnectionString = "Data Source=" + _hostConfig.HostDataSource + ((_hostConfig.HostPort != 0 ? ("," + _hostConfig.HostPort.ToString()) : ""));
            _strConnectionString += "; User ID=hostdev;database=" + _hostConfig.HostDatabase;
            _strConnectionString += ";Password=qwerty;";

            //System.IO.File.WriteAllText(exePath + @"\HostConfigxxxx.xml", _hostConfig.ToXML());
            //MessageBox.Show(_strConnectionString);

            this.Text = this.Text + " " + Application.ProductVersion.ToString();
            LoggerClass.Instance.MainLogInfo("MainWindow_Load: " + this.Text);

            this.btnReqSuspRefresh_Click(sender, e);
            this.btnPendingACAMRefresh_Click(sender, e);

        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfirmExitForm confirm = new ConfirmExitForm();
            if (confirm.ShowDialog() != DialogResult.Yes)
            {
                e.Cancel = true;
                LoggerClass.Instance.MainLogInfo("MainWindow_FormClosing: Cancel");
                return;
            }            
            
            foreach (hostController host in this.ListOfHost)
            {
                host.WDConnectPrimaryIn -= new WDConnectBase.SECsPrimaryInEventHandler(hostController_SECsPrimaryIn);
                host.WDConnectSecondaryIn -= new WDConnectBase.SECsSecondaryInEventHandler(hostController_SECsSecondaryIn);
                host.WDConnectHostError -= new WDConnectBase.SECsHostErrorHandler(hostController_SECsHostError);
            }

            //save config file
            System.IO.File.WriteAllText(exePath + @"\HostConfig.xml", _hostConfig.ToXML());

            LoggerClass.Instance.MainLogInfo("MainWindow_FormClosing: Exit");
            System.Environment.Exit(0);
        }


        private void SubScribeAllTool()
        {
            LoggerClass.Instance.MainLogInfo("SubScribeAllTool");
            foreach (hostController host in this.ListOfHost)
            {
                host.WDConnectPrimaryIn += new WDConnectBase.SECsPrimaryInEventHandler(hostController_SECsPrimaryIn);
                host.WDConnectSecondaryIn += new WDConnectBase.SECsSecondaryInEventHandler(hostController_SECsSecondaryIn);
                host.WDConnectHostError += new WDConnectBase.SECsHostErrorHandler(hostController_SECsHostError);
            }
        }

        private hostController GetHost(string remoteIPAddress, int remotePortNumber)
        {
            LoggerClass.Instance.MessageLogInfo("GetHost");
            foreach (hostController host in ListOfHost)
            {
                try
                {
                    if (host.localIPAddress == remoteIPAddress && host.localPortNumber == remotePortNumber)
                    {
                        return host;
                    }
                }
                catch (Exception ex)
                {
                    LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                    return null;
                }

            }
            return null;
        }

        private void BindToolDetailToGrid()
        {
            LoggerClass.Instance.MainLogInfo("BindToolDetailToGrid");
            int rowid = 0;

            List<string> ports = new List<string>();

            foreach (hostController host in this.ListOfHost)
            {
                MasterDataGridView.Rows.Add(host.hostConfiguration);

                if (host.ConnectionStatus == ConnectionStatus.NotConnected)
                {
                    MasterDataGridView.Rows[rowid].Cells[1].Style.BackColor = Color.Red;
                }

                bool flag = false;

                foreach (string port in ports)
                {
                    if (host.localPortNumber.ToString() == port)
                    {
                        MasterDataGridView.Rows[rowid].Cells[6].Style.BackColor = Color.Red;
                        flag = true;
                    }
                }

                if (!flag)
                {
                    ports.Add(host.localPortNumber.ToString());
                }

                rowid++;
            }
        }

        private Random rnd;
        private static object _lock = new object();
        private void hostController_SECsPrimaryIn(object sender, SECsPrimaryInEventArgs e)
        {
            DelegateSetErrorTextBoxMsg(""); //clear error message

            if (e.Transaction.Primary == null)
            {
                LoggerClass.Instance.MessageLogInfo("hostController_SECsPrimaryIn: null guard");
                return;
            }

            LoggerClass.Instance.MessageLogInfo("hostController_SECsPrimaryIn: " + e.Transaction.XMLText);
            DelegateSetPrimaryTextBoxMsg(e.Transaction.XMLText);

            hostController host = new hostController();
            //SCITransaction toReply;
            lock (_lock)
            {
                for (int i = 0; i < MasterDataGridView.Rows.Count; i++)
                {
                    if (e.remoteIPAddress == MasterDataGridView.Rows[i].Cells[3].Value.ToString() && e.remotePortNumber == int.Parse(MasterDataGridView.Rows[i].Cells[4].Value.ToString()))
                    {
                        switch (e.Transaction.Primary.CommandID)
                        {
                            case "Connected":
                                LoggerClass.Instance.MainLogInfo("Host << Connected");

                                this.Invoke(new Action(() =>
                                {
                                    MasterDataGridView.Rows[i].Cells[1].Value = ConnectionStatus.Connected;
                                    MasterDataGridView.Rows[i].Cells[1].Style.BackColor = Color.Green;

                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                    host.ConnectionStatus = ConnectionStatus.Connected;
                                }));
                                break;


                            case "Disconnected":
                                LoggerClass.Instance.MainLogInfo("Host << Disconnected");

                                this.Invoke(new Action(() =>
                                {
                                    MasterDataGridView.Rows[i].Cells[1].Value = ConnectionStatus.NotConnected;
                                    MasterDataGridView.Rows[i].Cells[1].Style.BackColor = Color.Red;

                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                    host.ConnectionStatus = ConnectionStatus.NotConnected;
                                }));
                                break;



                            #region case "AreYouThere"
                            case "AreYouThere":
                                LoggerClass.Instance.MainLogInfo("Host << AreYouThere");

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage secondaryMsgAreYouThere = new SCIMessage();
                                secondaryMsgAreYouThere.CommandID = "AreYouThereAck";
                                secondaryMsgAreYouThere.Item = new SCIItem();
                                secondaryMsgAreYouThere.Item.Format = SCIFormat.String;
                                secondaryMsgAreYouThere.Item.Value = e.Transaction.Primary.Item.Value;
                                secondaryMsgAreYouThere.Item.Items = new SCIItemCollection();
                                secondaryMsgAreYouThere.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SoftwareRevision", Value = "1.0.0" });

                                WDConnect.Common.SCITransaction toReplyAreYouThere = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "AreYouThereAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgAreYouThere
                                };

                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyAreYouThere.XMLText);
                                host.ReplyOutStream(toReplyAreYouThere);

                                break;

                            #endregion case "AreYouThere"



                            #region case "TrayCompleted"
                            case "TrayCompleted":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                TrayCompletedData traycompleted = TrayCompletedData.GetTrayCompletedDataFromXML_primary(e.Transaction.XMLText);
                                {
                                    string temp = "Host << TrayCompleted: " + traycompleted.TrayID + "," + traycompleted.TraySize.ToString();
                                    temp += "," + traycompleted.PalletIDRow11;
                                    temp += "," + traycompleted.PalletIDRow12;
                                    temp += "," + traycompleted.PalletIDRow21;
                                    temp += "," + traycompleted.PalletIDRow22;
                                    temp += "," + traycompleted.PalletIDRow31;
                                    temp += "," + traycompleted.PalletIDRow32;
                                    temp += "," + traycompleted.StartTime.ToString();
                                    temp += "," + traycompleted.FinishTime.ToString();

                                    LoggerClass.Instance.MainLogInfo(temp);
                                }



                                SCIMessage secondaryMsgTrayCompletedAck = new SCIMessage();
                                secondaryMsgTrayCompletedAck.CommandID = "TrayCompletedAck";
                                secondaryMsgTrayCompletedAck.Item = new SCIItem();
                                secondaryMsgTrayCompletedAck.Item.Format = SCIFormat.String;
                                secondaryMsgTrayCompletedAck.Item.Value = e.Transaction.Primary.Item.Value;
                                secondaryMsgTrayCompletedAck.Item.Items = new SCIItemCollection();


                                if (!_hostConfig.RunWithNoDatabase)
                                {

                                    System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                                    cnn.Open();

                                    string strStoredProcSQLCmd = "";
                                    int nTrayTransID = 0;

                                    if (cnn.State == ConnectionState.Open)
                                    {
                                        string strSqlcommand = "";
                                        strSqlcommand += "SELECT TOP(1) *"
                                                    + " FROM [dbo].[tblTrayTransaction] tblTray"
                                                    + " WHERE tblTray.TrayID = '" + /*"AW0162120F"*/ traycompleted.TrayID + "'"
                                                    + " ORDER BY tblTray.TrayTransID DESC, tblTray.CreatedDateTime DESC";


                                        System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(strSqlcommand, cnn);
                                        System.Data.DataSet dataset = new System.Data.DataSet();
                                        adpt.Fill(dataset);
                                        cnn.Close();

                                        System.Data.DataTableCollection tables = dataset.Tables;
                                        System.Data.DataTable table = tables[0];

                                        if (table.Rows.Count < 1)
                                        {
                                            //no data found; cannot close tray (traycompleted)
                                            LoggerClass.Instance.MainLogInfo("TrayCompleted: " + traycompleted.TraySize + " not found");

                                            secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                        }
                                        else
                                        {
                                            foreach (System.Data.DataRow row in table.Rows)
                                            {
                                                nTrayTransID = Int32.Parse(row[0].ToString());              //row[0] = TransID
                                                //row[1] = TrayID
                                                //row[2] = PalletID_Row12
                                                //...
                                                //row[8] = CompletedTray
                                                Console.WriteLine("CompletedTray: " + Boolean.Parse(row[8].ToString()));
                                                if (Boolean.Parse(row[8].ToString()))
                                                {
                                                    LoggerClass.Instance.MainLogInfo("TrayCompleted: " + traycompleted.TraySize + " already closed");
                                                }
                                            }


                                            //call [spUpdateTrayCompleted]
                                            //EXEC [spUpdateTrayCompleted] 1, 'AW0162120F', 1

                                            strStoredProcSQLCmd += "EXEC [spUpdateTrayCompleted] ";
                                            strStoredProcSQLCmd += nTrayTransID.ToString() + ", '" + traycompleted.TrayID + "', 1";

                                            System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strStoredProcSQLCmd, cnn);
                                            sqlcommand.Connection.Open();
                                            int nRet = sqlcommand.ExecuteNonQuery();
                                            sqlcommand.Connection.Close();


                                            secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                        }
                                    }
                                    else //cannot connect to database
                                    {
                                        LoggerClass.Instance.MainLogInfo("TrayCompleted: cannot connect to DB");

                                        secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                    }
                                }
                                else //run offline without database
                                {
                                    secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                }


                                WDConnect.Common.SCITransaction toReplyTrayCompletedAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "TrayCompletedAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgTrayCompletedAck
                                };

                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyTrayCompletedAck.XMLText);
                                host.ReplyOutStream(toReplyTrayCompletedAck);

                                break;

                            #endregion case "TrayCompleted"



                            #region case "OfflineRequest"
                            case "OfflineRequest":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                host.ReplyOutStream(e.Transaction);

                                break;

                            #endregion  case "OfflineRequest"



                            #region case "RequestPalletInfo"
                            case "RequestPalletInfo":
                                {
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    PalletToTrayData reqPallet = new PalletToTrayData();
                                    reqPallet = GetPalletFromXML_primary(e.Transaction.XMLText);


                                    //
                                    if (reqPallet.EquipmentType == "ASLV")
                                    {
                                        Console.WriteLine("ASLV");
                                        if(_aslvCheckOnACAMRequest == null || !_aslvCheckOnACAMRequest.IsAlive)
                                        //if (!_aslvCheckOnACAMRequest.IsAlive)
                                        {
                                            //_aslvCheckOnACAMRequest = new Thread(checkACAMRequestSusp);
                                            _aslvCheckOnACAMRequest = null;
                                            _aslvCheckOnACAMRequest = new Thread(() => checkACAMRequestSusp(ref host));
                                            _aslvCheckOnACAMRequest.Name = "checkACAMRequestSuspThread";
                                            _aslvCheckOnACAMRequest.Priority = ThreadPriority.Normal;
                                        }

                                        if (_aslvCheckOnACAMRequest.ThreadState == ThreadState.Unstarted)
                                        {
                                            _aslvCheckOnACAMRequest.Start();
                                        }
                                        else if (_aslvCheckOnACAMRequest.ThreadState == ThreadState.Stopped)
                                        {
                                            //_aslvCheckOnACAMRequest.Start();
                                        }


                                    }
                                    //


                                    Console.WriteLine("{0},{1},{2}", reqPallet.PalletID, reqPallet.EquipmentID, reqPallet.EquipmentType);
                                    LoggerClass.Instance.MainLogInfo("Host << RequestPalletInfo: " + reqPallet.PalletID + "," + reqPallet.EquipmentID + "," + reqPallet.EquipmentType);


                                    // suppose to get pallet info from database
                                    //RequestPalletInfoAckObj reqPalletObj = RequestPalletInfoAckObj.ReadFromFile(@".\Pallet\PT0001.xml");
                                    RequestPalletInfoAckObj reqPalletObj = new RequestPalletInfoAckObj();
                                    RequestSuspensionClass reqSuspObj = new RequestSuspensionClass();

                                    #region online connecting to database
                                    //if connect to database
                                    if (!_hostConfig.RunWithNoDatabase)
                                    {
                                        //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
                                        //strConnectionString += ";Password=qwerty;";

                                        System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                                        try
                                        {
                                            cnn.Open();

                                            if (cnn.State == ConnectionState.Open)
                                            {
                                                string sqlcommand = "";
                                                sqlcommand += "SELECT TOP(1) *"
                                                            + " FROM [dbo].[tblPalletTransaction] tblPallet"
                                                            + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ reqPallet.PalletID + "'"
                                                            //+ " ORDER BY tblPallet.CreatedDateTime DESC";
                                                            + " ORDER BY tblPallet.TransID DESC";


                                                System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                                                System.Data.DataSet dataset = new System.Data.DataSet();
                                                adpt.Fill(dataset);
                                                cnn.Close();


                                                System.Data.DataTableCollection tables = dataset.Tables;
                                                System.Data.DataTable table = tables[0];

                                                if (table.Rows.Count < 1)
                                                {
                                                    //no data found
                                                    reqPalletObj.COMMACK = 1;   //failed
                                                    reqPalletObj.EnabledPallet = false;
                                                    //break;
                                                }
                                                else
                                                {
                                                    #region foreach
                                                    foreach (System.Data.DataRow row in table.Rows)
                                                    {
                                                        //row[0] = TransID
                                                        //row[1] = PalletID
                                                        //row[2] = EquipmentType
                                                        //row[3] = NextEquipmentType
                                                        //row[4] = CreatedDateTime

                                                        //row[5] = LotNumber
                                                        //row6 = HGASN_1
                                                        //row7 = HGADefect_1
                                                        //row8 = HGASN_2
                                                        //row9 = HGADefect_2
                                                        //row10 = HGASN_3
                                                        //row11 = HGADefect_3
                                                        //row12 = HGASN_4
                                                        //row13 = HGADefect_4
                                                        //row14 = HGASN_5
                                                        //row15 = HGADefect_5

                                                        //row16 = HGASN_6
                                                        //row17 = HGADefect_6
                                                        //row18 = HGASN_7
                                                        //row19 = HGADefect_7
                                                        //row20 = HGASN_8
                                                        //row21 = HGADefect_8
                                                        //row22 = HGASN_9
                                                        //row23 = HGADefect_9
                                                        //row24 = HGASN_10
                                                        //row25 = HGADefect_10

                                                        //row26 = ACAMID

                                                        //row27 = ILC UVPower
                                                        //row28 = ILC CureTime
                                                        //row29 = ILC CureZone

                                                        //row30 = SJBStage


                                                        reqPalletObj.TransID = Int32.Parse(row[0].ToString());

                                                        reqPalletObj.COMMACK = 0;
                                                        reqPalletObj.ALMID = 0;
                                                        reqPalletObj.PalletID = reqPallet.PalletID;

                                                        reqPalletObj.EquipmentType = Int32.Parse(row[2].ToString());
                                                        reqPalletObj.NextEquipmentType = Int32.Parse(row[3].ToString());

                                                        //check equipment type
                                                        //if reqPallet.EquipmentType != row[3] = NextEquipmentType
                                                        //if not matched; enabled = false
                                                        EQUIPMENT_TYPE enCheckEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), reqPallet.EquipmentType);
                                                        if (enCheckEquipmentType == ((EQUIPMENT_TYPE)Int32.Parse(row[3].ToString()))) //check NextEquipmentType against tool's EquipmentType
                                                        {
                                                            reqPalletObj.EnabledPallet = true;
                                                        }
                                                        else
                                                        {
                                                            if (enCheckEquipmentType == EQUIPMENT_TYPE.PGM)
                                                            {
                                                                reqPalletObj.EnabledPallet = true;
                                                            }
                                                            else
                                                            {
                                                                reqPalletObj.EnabledPallet = false;
                                                            }
                                                        }


                                                        reqPalletObj.LotNumber = row[5].ToString();

                                                        // to remove non character strings
                                                        char[] arrTemp = reqPalletObj.LotNumber.ToCharArray();
                                                        arrTemp = Array.FindAll<char>(arrTemp, (c => (char.IsLetterOrDigit(c)
                                                                                                    || char.IsWhiteSpace(c)
                                                                                                    || c == '_')));
                                                        reqPalletObj.LotNumber = new string(arrTemp);
                                                        // to remove non character strings


                                                        LotInformation lotinfo = new LotInformation();
                                                        if (mappingLot_LotInfo.TryGetValue(reqPalletObj.LotNumber, out lotinfo))     //in case, no lot information; lot information will exist after ACAM
                                                        {
                                                            reqPalletObj.HGAType = lotinfo.HGAType;
                                                            reqPalletObj.PartNumber = lotinfo.PartNumber;
                                                            reqPalletObj.ProductName = lotinfo.Program;
                                                            reqPalletObj.Suspension = lotinfo.Suspension;
                                                        }
                                                        else
                                                        {
                                                            reqPalletObj.HGAType = "";
                                                            reqPalletObj.PartNumber = "";
                                                            reqPalletObj.ProductName = "";
                                                            reqPalletObj.Suspension = "";
                                                        }


                                                        reqPalletObj.ACAMID = row[26].ToString();



                                                        reqPalletObj.UVPower = row[27] == DBNull.Value ? 0 : Int32.Parse(row[27].ToString());         //row27 = ILC UVPower
                                                        reqPalletObj.CureTime = row[28] == DBNull.Value ? 0 : Int32.Parse(row[28].ToString());        //row28 = ILC CureTime
                                                        reqPalletObj.CureZone = row[29] == DBNull.Value ? 0 : Int32.Parse(row[29].ToString());        //row29 = ILC CureZone
                                                        reqPalletObj.SJBStage = row[30].ToString();                     //row30 = SJBStage

                                                        reqPalletObj.HGA.HGA1.SN = row[6].ToString();
                                                        reqPalletObj.HGA.HGA1.Defect = row[7].ToString();
                                                        reqPalletObj.HGA.HGA2.SN = row[8].ToString();
                                                        reqPalletObj.HGA.HGA2.Defect = row[9].ToString();
                                                        reqPalletObj.HGA.HGA3.SN = row[10].ToString();
                                                        reqPalletObj.HGA.HGA3.Defect = row[11].ToString();
                                                        reqPalletObj.HGA.HGA4.SN = row[12].ToString();
                                                        reqPalletObj.HGA.HGA4.Defect = row[13].ToString();
                                                        reqPalletObj.HGA.HGA5.SN = row[14].ToString();
                                                        reqPalletObj.HGA.HGA5.Defect = row[15].ToString();

                                                        reqPalletObj.HGA.HGA6.SN = row[16].ToString();
                                                        reqPalletObj.HGA.HGA6.Defect = row[17].ToString();
                                                        reqPalletObj.HGA.HGA7.SN = row[18].ToString();
                                                        reqPalletObj.HGA.HGA7.Defect = row[19].ToString();
                                                        reqPalletObj.HGA.HGA8.SN = row[20].ToString();
                                                        reqPalletObj.HGA.HGA8.Defect = row[21].ToString();
                                                        reqPalletObj.HGA.HGA9.SN = row[22].ToString();
                                                        reqPalletObj.HGA.HGA9.Defect = row[23].ToString();
                                                        reqPalletObj.HGA.HGA10.SN = row[24].ToString();
                                                        reqPalletObj.HGA.HGA10.Defect = row[25].ToString();

                                                        reqPalletObj.SJBFixture = row[92] == DBNull.Value ? reqPalletObj.SJBFixture : Int32.Parse(row[92].ToString());
                                                    }
                                                    #endregion foreach


                                                    //request suspension
                                                    string strId = string.Empty;
                                                    string strSqlReqSuspCommand = "";

                                                    switch (reqPallet.EquipmentType)
                                                    {
                                                        case "ASLV":
                                                            //for ASLV, PalletSN not yet assigned
                                                            strSqlReqSuspCommand += "SELECT TOP(1) *"
                                                                        + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                        + " WHERE tblReqSusp.PalletSN IS NULL"
                                                                        + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first

                                                            break;


                                                        case "APT":
                                                            //for APT, PalletSN already assigned
                                                            strSqlReqSuspCommand += "SELECT TOP(1) *"
                                                                        + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                        + " WHERE tblReqSusp.PalletSN = '" + reqPallet.PalletID + "'"
                                                                        //+ " AND tblReqSusp.ACAMID = '" + reqPallet.EquipmentID + "'"
                                                                        + " AND (tblReqSusp.IsProcessed = 0 OR tblReqSusp.IsProcessed = 3)"
                                                                        + " AND tblReqSusp.TransID IS NOT NULL"
                                                                        + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first

                                                            break;


                                                        default:
                                                            break;
                                                    }


                                                    //only ASLV, ACAM, APT will require to update the status on tblACAMRequestSusp
                                                    if (reqPallet.EquipmentType == "ASLV" || reqPallet.EquipmentType == "APT")
                                                    {
                                                        if (cnn.State == ConnectionState.Closed)
                                                        {
                                                            cnn.Open();
                                                        }

                                                        System.Data.SqlClient.SqlDataAdapter adptReqSusp = new SqlDataAdapter(strSqlReqSuspCommand, cnn);
                                                        System.Data.DataSet dsReqSusp = new System.Data.DataSet();
                                                        adptReqSusp.Fill(dsReqSusp);
                                                        cnn.Close();

                                                        System.Data.DataTableCollection dsTables = dsReqSusp.Tables;
                                                        System.Data.DataTable tbReqSusp = dsTables[0];
                                                        if (tbReqSusp.Rows.Count < 1)
                                                        {
                                                            //no ACAM is requesting suspensions
                                                            //no data found
                                                            //failed, pallet not found

                                                            //reqPalletObj.COMMACK = 1;
                                                            reqPalletObj.EnabledPallet = false;
                                                        }
                                                        else
                                                        {
                                                            reqSuspObj.IsRequesting = true;
                                                            foreach (System.Data.DataRow row in tbReqSusp.Rows)
                                                            {
                                                                //row[0]    Id
                                                                strId = row[0].ToString();

                                                                //row[1]    ACAMID
                                                                reqSuspObj.ACAMID = row[1].ToString();

                                                                //row[2]    SuspAmt
                                                                reqSuspObj.SuspAmt = Int32.Parse(row[2].ToString());

                                                                //row[3]    PalletSN
                                                                //row[4]    TransID

                                                                //row[5]    dbo_pallet_Id
                                                                //row[6]    CreatedDateTime
                                                                //row[7]    IsProcessed
                                                                //row[8]    UpdatedDateTime
                                                            }

                                                            if ((reqPallet.EquipmentType == "APT") && (reqPallet.EquipmentID != reqSuspObj.ACAMID))
                                                            {
                                                                reqPalletObj.EnabledPallet = false;
                                                                reqSuspObj.IsRequesting = false;
                                                            }

                                                        }

                                                        //for APT, set IsProcessed to 3
                                                        if (reqPallet.EquipmentType == "APT")
                                                        {
                                                            //-- EXEC [spUpdateACAMRequestSusp] Id, Pallet Barcode, IsProcessed, UpdatedDateTime,           TransID,                paleltID
                                                            //-- EXEC [spUpdateACAMRequestSusp] 3,  'PT0002',       1,           '2019-06-11 00:00:01.000', /*transID*/ nTransID
                                                            string strStoredProcSQLCmd = "EXEC [spUpdateACAMRequestSusp] " + strId + ", '" + reqPallet.PalletID + "',";
                                                            strStoredProcSQLCmd += "3, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "', " + reqPalletObj.TransID.ToString();

                                                            System.Data.SqlClient.SqlCommand sqlcmdUpdateACAMRequestSusp = new SqlCommand(strStoredProcSQLCmd, cnn);
                                                            sqlcmdUpdateACAMRequestSusp.Connection.Open();
                                                            int nRetReqSusp = sqlcmdUpdateACAMRequestSusp.ExecuteNonQuery();
                                                            sqlcmdUpdateACAMRequestSusp.Connection.Close();
                                                        }

                                                        //request suspension
                                                    }


                                                } //else

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerClass.Instance.ErrorLogInfo("!_hostConfig.RunWithNoDatabase: " + ex.Message);
                                        }
                                    }
                                    #endregion online connecting to database


                                    #region offline, run on files
                                    else
                                    {
                                        //offline pallet information
                                        lstReqPalletLookup.Clear();
                                        lstReqPalletLookup = GetLocalReqPalletInfoLookup();
                                        if (!mappingLocalPalletIDReqPalletInfoObj.TryGetValue(reqPallet.PalletID, out reqPalletObj))
                                        {
                                            //clearUITextData();
                                            //lbl_tab3_error.Text = txtbox_tab3_PalletID.Text + " not found in the system!";

                                            reqPalletObj = new RequestPalletInfoAckObj();

                                            reqPalletObj.COMMACK = 1;
                                            reqPalletObj.ALMID = 0;
                                            reqPalletObj.EnabledPallet = false;
                                            reqPalletObj.PalletID = reqPallet.PalletID; 
                                        }
                                    }
                                    #endregion offline, run on files


                                    SCIMessage scndMsgRequestPalletInfo = new SCIMessage();
                                    scndMsgRequestPalletInfo.CommandID = "RequestPalletInfoAck";
                                    scndMsgRequestPalletInfo.Item = new SCIItem();
                                    scndMsgRequestPalletInfo.Item.Format = SCIFormat.List;
                                    scndMsgRequestPalletInfo.Item.Items = new SCIItemCollection();

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = reqPalletObj.COMMACK });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = reqPalletObj.ALMID });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPalletObj.PalletID });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = reqPalletObj.LotNumber });

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumber", Value = reqPalletObj.PartNumber });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProductName", Value = reqPalletObj.ProductName });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = reqPalletObj.Suspension });


                                    //to support Pallet's dynamic lot information; STR info will be used to generate dynamic AQ Tray files on HOST
                                    LotInformation lotinfo1 = new LotInformation();
                                    if (mappingLot_LotInfo.TryGetValue(reqPalletObj.LotNumber, out lotinfo1))     //in case, no lot information; lot information will exist after ACAM
                                    {
                                        scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = lotinfo1.STR });
                                    }
                                    else
                                    {
                                        scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = "" });
                                    }


                                    //scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = reqPalletObj.Line });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = lotinfo1.Line });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = reqPalletObj.ACAMID });


                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCUVPower", Value = reqPalletObj.UVPower });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCCureTime", Value = reqPalletObj.CureTime });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCCureZone", Value = reqPalletObj.CureZone});

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SJBStage", Value = reqPalletObj.SJBStage });


                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EnabledPallet", Value = reqPalletObj.EnabledPallet });

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = reqPalletObj.EquipmentType });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "NextEquipmentType", Value = reqPalletObj.NextEquipmentType });
                                    
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EndLot", Value = reqPalletObj.EndLot });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "AllowedMix", Value = reqPalletObj.AllowedMix });

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAType", Value = reqPalletObj.HGAType });



                                    //request suspension
                                    SCIItem reqSuspListItem = new SCIItem();
                                    reqSuspListItem.Format = SCIFormat.List;
                                    reqSuspListItem.Name = "RequestSuspension";
                                    reqSuspListItem.Value = reqSuspObj.IsRequesting;
                                    reqSuspListItem.Items = new SCIItemCollection();

                                    scndMsgRequestPalletInfo.Item.Items.Add(reqSuspListItem);

                                    reqSuspListItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = reqSuspObj.ACAMID });
                                    reqSuspListItem.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = reqSuspObj.SuspAmt });
                                    //



                                    //hga data portion
                                    SCIItem hgaListItem = new SCIItem();
                                    hgaListItem.Format = SCIFormat.List;
                                    hgaListItem.Name = "HGA";
                                    hgaListItem.Items = new SCIItemCollection();

                                    scndMsgRequestPalletInfo.Item.Items.Add(hgaListItem);

                                    HGA[] hgas = new HGA[10];
                                    hgas[0] = new HGA();
                                    hgas[0] = reqPalletObj.HGA.HGA1;
                                    hgas[1] = new HGA();
                                    hgas[1] = reqPalletObj.HGA.HGA2;
                                    hgas[2] = new HGA();
                                    hgas[2] = reqPalletObj.HGA.HGA3;
                                    hgas[3] = new HGA();
                                    hgas[3] = reqPalletObj.HGA.HGA4;
                                    hgas[4] = new HGA();
                                    hgas[4] = reqPalletObj.HGA.HGA5;
                                    hgas[5] = new HGA();
                                    hgas[5] = reqPalletObj.HGA.HGA6;
                                    hgas[6] = new HGA();
                                    hgas[6] = reqPalletObj.HGA.HGA7;
                                    hgas[7] = new HGA();
                                    hgas[7] = reqPalletObj.HGA.HGA8;
                                    hgas[8] = new HGA();
                                    hgas[8] = reqPalletObj.HGA.HGA9;
                                    hgas[9] = new HGA();
                                    hgas[9] = reqPalletObj.HGA.HGA10;


                                    for (int j = 0; j < 10; j++)
                                    {
                                        SCIItem hgaItem = new SCIItem();
                                        hgaItem.Format = SCIFormat.List;
                                        hgaItem.Name = "HGA" + (j + 1).ToString();
                                        hgaItem.Value = "";
                                        hgaItem.Items = new SCIItemCollection();
                                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = hgas[j].SN });
                                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = hgas[j].Defect });

                                        //add HGA1 - HGA10
                                        hgaListItem.Items.Add(hgaItem);
                                    }

                                    //hga data portion

                                    //TransID
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "TransID", Value = reqPalletObj.TransID });

                                    //SJBFixture
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SJBFixture", Value = reqPalletObj.SJBFixture });


                                    LoggerClass.Instance.MainLogInfo("Host >> RequestPalletInfo: " + reqPalletObj.PalletID + "," + reqPalletObj.LotNumber + "," + reqPalletObj.EquipmentType.ToString() + "," + reqPalletObj.NextEquipmentType.ToString());


                                    WDConnect.Common.SCITransaction toReplyRequestPalletInfo = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "RequestPalletInfoAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = scndMsgRequestPalletInfo
                                    };

                                    //*****************
                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyRequestPalletInfo.XMLText);
                                    host.ReplyOutStream(toReplyRequestPalletInfo);

                                    DelegateRefreshReqSuspUI(sender, e);

                                    break;
                                }

                            #endregion case "RequestPalletInfo"



                            #region case "SendPalletInfo"
                            case "SendPalletInfo":
                                {
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    ProcessedPalletDataClass sendProcessedPalletInfoData = new ProcessedPalletDataClass();
                                    sendProcessedPalletInfoData = GetProcessedPalletFromXML_primary(e.Transaction.XMLText);


                                    {
                                        string temp = "Host << SendPalletInfo: " + sendProcessedPalletInfoData.PalletID + "," + sendProcessedPalletInfoData.EquipmentID + "," + sendProcessedPalletInfoData.LotNumber;
                                        foreach (HGADataClass hga in sendProcessedPalletInfoData._arrHGA)
                                        {
                                            temp += "#" + hga._strOCR + "%" + (hga._lstDefects.Count > 0 ? hga._lstDefects[0] : "");
                                        }
                                        LoggerClass.Instance.MainLogInfo(temp);
                                    }

                                    //
                                    WDConnect.Common.SCITransaction transObj;
                                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(WDConnect.Common.SCITransaction));
                                    using (StringReader reader = new StringReader(e.Transaction.XMLText))
                                    {
                                        transObj = (WDConnect.Common.SCITransaction)x.Deserialize(reader);
                                    }

                                    SendPalletInfoObj sndPalletObj = new SendPalletInfoObj();
                                    sndPalletObj = SendPalletInfoObj.ToObj(e.Transaction);
                                    //


                                    if (!_hostConfig.RunWithNoDatabase)
                                    {

                                        System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                                        cnn.Open();
                                        int nTransID = 0;
                                        int nEquipmentType = 0;
                                        int nNextEquipmentType = 0;

                                        if (cnn.State == ConnectionState.Open)
                                        {
                                            string strSqlcommand = "";
                                            strSqlcommand += "SELECT TOP(1) *"
                                                        + " FROM [dbo].[tblPalletTransaction] tblPallet"
                                                        + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ sendProcessedPalletInfoData.PalletID + "'"
                                                        //+ " ORDER BY tblPallet.CreatedDateTime DESC";
                                                        + " ORDER BY tblPallet.TransID DESC";


                                            System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(strSqlcommand, cnn);
                                            System.Data.DataSet dataset = new System.Data.DataSet();
                                            adpt.Fill(dataset);
                                            cnn.Close();


                                            System.Data.DataTableCollection tables = dataset.Tables;
                                            System.Data.DataTable table = tables[0];

                                            #region look in pallet transaction table
                                            if (table.Rows.Count < 1)
                                            {
                                                //no data found
                                                //failed, pallet not found
                                                SCIMessage scndMsgNOKPalletInfoAck = new SCIMessage();
                                                scndMsgNOKPalletInfoAck.CommandID = "SendPalletInfoAck";
                                                scndMsgNOKPalletInfoAck.Item = new SCIItem();
                                                scndMsgNOKPalletInfoAck.Item.Format = SCIFormat.List;
                                                scndMsgNOKPalletInfoAck.Item.Items = new SCIItemCollection();
                                                scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                                scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });

                                                WDConnect.Common.SCITransaction replyNOKPalletInfoAck = new WDConnect.Common.SCITransaction()
                                                {
                                                    DeviceId = e.Transaction.DeviceId,
                                                    MessageType = MessageType.Secondary,
                                                    Id = e.Transaction.Id,
                                                    Name = "SendPalletInfoAck",
                                                    NeedReply = false,
                                                    Primary = e.Transaction.Primary,
                                                    Secondary = scndMsgNOKPalletInfoAck
                                                };

                                                LoggerClass.Instance.MessageLogInfo("Reply: " + replyNOKPalletInfoAck.XMLText);
                                                LoggerClass.Instance.MainLogInfo("Host >> SendPalletInfoAck: " + sndPalletObj.PalletID + "," + sndPalletObj.EquipmentID + ", COMMACK 1");

                                                host.ReplyOutStream(replyNOKPalletInfoAck);

                                                break;
                                            }
                                            else
                                            {
                                                foreach (System.Data.DataRow row in table.Rows)
                                                {
                                                    nTransID = Int32.Parse(row[0].ToString());            //row[0] = TransID
                                                    nEquipmentType = Int32.Parse(row[2].ToString());      //row[2] = EquipmentType
                                                    nNextEquipmentType = Int32.Parse(row[3].ToString());  //row[3] = NextEqupimentType
                                                }

                                                //Check if NextEquipmentType not matched
                                                //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
                                                //10    , 80    , 30    , 40    , 50    , 60
                                                if (Int32.Parse(sndPalletObj.EquipmentType) != nNextEquipmentType)
                                                {
                                                    //MessageBox.Show("Please check NextEquipmentType");
                                                    string strErrorMsg = "Please check NextEquipmentType, ";
                                                    strErrorMsg += sndPalletObj.EquipmentType.ToString() + " != " + nNextEquipmentType.ToString();
                                                    DelegateSetErrorTextBoxMsg(strErrorMsg);


                                                    LoggerClass.Instance.MainLogInfo(strErrorMsg);


                                                    SCIMessage scndMsgNOKPalletInfoAck = new SCIMessage();
                                                    scndMsgNOKPalletInfoAck.CommandID = "SendPalletInfoAck";
                                                    scndMsgNOKPalletInfoAck.Item = new SCIItem();
                                                    scndMsgNOKPalletInfoAck.Item.Format = SCIFormat.List;
                                                    scndMsgNOKPalletInfoAck.Item.Items = new SCIItemCollection();
                                                    scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                                    scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });

                                                    WDConnect.Common.SCITransaction replyNOKPalletInfoAck = new WDConnect.Common.SCITransaction()
                                                    {
                                                        DeviceId = e.Transaction.DeviceId,
                                                        MessageType = MessageType.Secondary,
                                                        Id = e.Transaction.Id,
                                                        Name = "SendPalletInfoAck",
                                                        NeedReply = false,
                                                        Primary = e.Transaction.Primary,
                                                        Secondary = scndMsgNOKPalletInfoAck
                                                    };

                                                    LoggerClass.Instance.MessageLogInfo("Reply: " + replyNOKPalletInfoAck.XMLText);
                                                    LoggerClass.Instance.MainLogInfo("Host >> SendPalletInfoAck: " + sndPalletObj.PalletID + "," + sndPalletObj.EquipmentID + ", COMMACK 1");

                                                    host.ReplyOutStream(replyNOKPalletInfoAck);

                                                    break;
                                                }


                                                string strSQLCmd = "";


                                                #region region switch sndPalletObj.EquipmentType
                                                switch (sndPalletObj.EquipmentType)
                                                {
                                                    //if nextequipmenttype = 10; ASLV
                                                    //      call spNewPalletTransaction 
                                                    case "10":
                                                        //EXEC [spNewPalletTransaction] 'PT0001', 20, 30, '05-03-2017 16:40:00 PM', 'WYKU7_AB2',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'','',
                                                        //'',''
                                                        strSQLCmd += "EXEC [spNewPalletTransaction] " + "'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ",";
                                                        strSQLCmd += "'" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt") + "',";
                                                        strSQLCmd += "'',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'','',";
                                                        strSQLCmd += "'',''";

                                                        break;


                                                    case "25": //SAI
                                                        // EXEC [spUpdatePalletTransactionSAI] 1, 'PT0001', 25, 30,
                                                        // 1.1351, 0.0021, 89.4340, 1.1360, -0.0035, 89.3036,
                                                        // 1.1305, 0.0000, 89.6722, 1.1314, 0.0019, 89.7099,
                                                        // 1.1207, 0.0058, 90.0569, 1.1216, 0.0006, 89.9412,
                                                        // 1.1214, 0.0077, 90.4667, 1.1223, 0.0039, 90.3834,
                                                        // 1.1236, 0.0042, 90.3043, 1.1245, -0.0004, 90.2005,
                                                        // 1.1306, 0.0050, 90.4107, 1.1315, 0.0037, 90.3691,
                                                        // 1.1361, 0.0047, 89.4286, 1.1370, 0.0011, 89.3465,
                                                        // 1.1359, 0.0043, 90.5226, 1.1368, 0.0037, 90.5064,
                                                        // 1.1325, 0.0019, 90.1507, 1.1334, 0.0007, 90.1244,
                                                        // 1.1347, 0.0035, 90.2871, 1.1356, 0.0023, 90.2606

                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionSAI] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + ", 30,";     //hardcoded, after SAI, sent pallet to ILC

                                                        for (int j = 0; j < 10; j++)
                                                        {
                                                            strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblxlocACAM.ToString() + ", ";
                                                            strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblylocACAM.ToString() + ", ";
                                                            strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblskwACAM.ToString() + ", ";

                                                            strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblxlocSAI.ToString() + ", ";
                                                            strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblylocSAI.ToString() + ", ";

                                                            if (j < 9)
                                                            {
                                                                strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblskwSAI.ToString() + ", ";
                                                            }
                                                            else
                                                            {
                                                                strSQLCmd += sendProcessedPalletInfoData._arrHGA[j]._dblskwSAI.ToString();
                                                            }
                                                        }

                                                        break;


                                                    //if nextequpimenttype = 30 ILC
                                                    //      call spUpdatePalletTransactionNoLotNoSNILC
                                                    case "30":
                                                        //EXEC [spUpdatePalletTransactionNoLotNoSNILC] 3, 'PT0001', 30, 40, 0, 0.0, 0
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionNoLotNoSNILC] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ","; 
                                                        //strSQLCmd += sndPalletObj.EquipmentType + ", 50,";     //hardcoded, after ILC, sent pallet directly to IAVI
                                                        strSQLCmd += sndPalletObj.UVPower.ToString() + ",";
                                                        strSQLCmd += String.Format("{0:0.#####}", sndPalletObj.CureTime) + ",";
                                                        strSQLCmd += sndPalletObj.CureZone.ToString(); 

                                                        break;


                                                    //if nextequpimenttype = 40 SJB
                                                    //      call spUpdatePalletTransactionWithSJBFixture
                                                    case "40":
                                                        //EXEC [spUpdatePalletTransactionWithSJBFixture] 3, 'PT0001', 40, 50, 1
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionWithSJBFixture] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ",";
                                                        strSQLCmd += sndPalletObj.SJBFixture.ToString();

                                                        break;


                                                    //if nextequpimenttype = 60 UNLOAD
                                                    //      call spUpdatePalletTransactionNoLotNoSN
                                                    case "60":
                                                        //EXEC [spUpdatePalletTransactionNoLotNoSN] 3, 'PT0001', 20, 30
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionNoLotNoSN] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType);

                                                        break;


                                                    //if nextequipmenttype = 50; AVI
                                                    //      call spUpdatePalletTransactionSN
                                                    case "50":

                                                        //Clear given NULL PalletID before proceeding
                                                        string strRemoveNULLPalletSQLCmd = "EXEC [spRemoveGivenNullPalletID] '" + sndPalletObj.PalletID + "';";
                                                        System.Data.SqlClient.SqlCommand sqlcmdRemoveNULLPalletID = new SqlCommand(strRemoveNULLPalletSQLCmd, cnn);

                                                        sqlcmdRemoveNULLPalletID.Connection.Open();
                                                        int nRemovedNULLPalletRows = sqlcmdRemoveNULLPalletID.ExecuteNonQuery();
                                                        sqlcmdRemoveNULLPalletID.Connection.Close();

                                                        LoggerClass.Instance.MainLogInfo(string.Format("Clear given NULL PalletID: {0}, {1} rows affected", sndPalletObj.PalletID, nRemovedNULLPalletRows.ToString()));
                                                        System.Threading.Thread.Sleep(10);


                                                        //EXEC [spUpdatePalletTransactionSN] 3, 'PT0001', 20, 30,
                                                        //'WYKU871H','A1,WO', 
                                                        //'WYKU871C','A1,WO', 
                                                        //'WYKU7692','A1,WO', 
                                                        //'WYKU769X','A1,WO', 
                                                        //'WYKU769W','A1,WO', 
                                                        //'WYKU769V','A1,WO', 
                                                        //'WYKU769R','A1,WO', 
                                                        //'WYKU769P','A1,WO', 
                                                        //'WYKU771P','A1,WO', 
                                                        //'WYKU771K','A1,WO'
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionSN] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ",";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA1.SN + "'," + "'" + sndPalletObj.HGA.HGA1.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA2.SN + "'," + "'" + sndPalletObj.HGA.HGA2.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA3.SN + "'," + "'" + sndPalletObj.HGA.HGA3.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA4.SN + "'," + "'" + sndPalletObj.HGA.HGA4.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA5.SN + "'," + "'" + sndPalletObj.HGA.HGA5.Defect + "',";

                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA6.SN + "'," + "'" + sndPalletObj.HGA.HGA6.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA7.SN + "'," + "'" + sndPalletObj.HGA.HGA7.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA8.SN + "'," + "'" + sndPalletObj.HGA.HGA8.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA9.SN + "'," + "'" + sndPalletObj.HGA.HGA9.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA10.SN + "'," + "'" + sndPalletObj.HGA.HGA10.Defect + "',";

                                                        strSQLCmd += sndPalletObj.SJBLane.ToString();
                                                        break;


                                                    //if nextequipmenttype = 80; APT
                                                    //      call spUpdatePalletTransactionLot
                                                    case "80":
                                                        //EXEC [spUpdatePalletTransactionLot] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'APT001'
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionLot] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ",";
                                                        strSQLCmd += "'" + sndPalletObj.LotNumber + "',";
                                                        strSQLCmd += "'" + sndPalletObj.EquipmentID + "'";

                                                        break;


                                                    default:
                                                        //EXEC [spUpdatePalletTransaction] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO'
                                                        strSQLCmd += "EXEC [spUpdatePalletTransaction] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType) + ",";
                                                        strSQLCmd += "'" + sndPalletObj.LotNumber + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA1.SN + "'," + "'" + sndPalletObj.HGA.HGA1.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA2.SN + "'," + "'" + sndPalletObj.HGA.HGA2.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA3.SN + "'," + "'" + sndPalletObj.HGA.HGA3.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA4.SN + "'," + "'" + sndPalletObj.HGA.HGA4.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA5.SN + "'," + "'" + sndPalletObj.HGA.HGA5.Defect + "',";

                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA6.SN + "'," + "'" + sndPalletObj.HGA.HGA6.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA7.SN + "'," + "'" + sndPalletObj.HGA.HGA7.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA8.SN + "'," + "'" + sndPalletObj.HGA.HGA8.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA9.SN + "'," + "'" + sndPalletObj.HGA.HGA9.Defect + "',";
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA10.SN + "'," + "'" + sndPalletObj.HGA.HGA10.Defect + "'";

                                                        break;

                                                };
                                                #endregion switch

                                                if ((sndPalletObj.EquipmentType != "10") && (sndPalletObj.EquipmentType != "80"))
                                                {
                                                    System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                                                    sqlcommand.Connection.Open();
                                                    int nRet = sqlcommand.ExecuteNonQuery();
                                                    sqlcommand.Connection.Close();
                                                }

                                                //additional updates for ASLV
                                                #region ASLV
                                                else if (sndPalletObj.EquipmentType == "10")
                                                {

                                                    string strId = string.Empty;
                                                    string strSqlReqSuspCommand = "";
                                                    strSqlReqSuspCommand += "SELECT TOP(1) *"
                                                                + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                + " WHERE tblReqSusp.PalletSN IS NULL"
                                                                + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first

                                                    if (cnn.State == ConnectionState.Closed)
                                                    {
                                                        cnn.Open();
                                                    }

                                                    System.Data.DataSet dsReqSusp = new System.Data.DataSet();
                                                    System.Data.SqlClient.SqlDataAdapter adptReqSusp = new SqlDataAdapter(strSqlReqSuspCommand, cnn);
                                                    adptReqSusp.Fill(dsReqSusp);
                                                    cnn.Close();


                                                    System.Data.DataTableCollection dsTables = dsReqSusp.Tables;
                                                    System.Data.DataTable tbReqSusp = dsTables[0];
                                                    if (tbReqSusp.Rows.Count < 1)
                                                    {
                                                        //no ACAM is requesting suspensions
                                                        //no data found
                                                        //failed, pallet not found

                                                        SCIMessage scndMsgNOKPalletInfoAck = new SCIMessage();
                                                        scndMsgNOKPalletInfoAck.CommandID = "SendPalletInfoAck";
                                                        scndMsgNOKPalletInfoAck.Item = new SCIItem();
                                                        scndMsgNOKPalletInfoAck.Item.Format = SCIFormat.List;
                                                        scndMsgNOKPalletInfoAck.Item.Items = new SCIItemCollection();
                                                        scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                                        scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });

                                                        WDConnect.Common.SCITransaction replyNOKPalletInfoAck = new WDConnect.Common.SCITransaction()
                                                        {
                                                            DeviceId = e.Transaction.DeviceId,
                                                            MessageType = MessageType.Secondary,
                                                            Id = e.Transaction.Id,
                                                            Name = "SendPalletInfoAck",
                                                            NeedReply = false,
                                                            Primary = e.Transaction.Primary,
                                                            Secondary = scndMsgNOKPalletInfoAck
                                                        };

                                                        LoggerClass.Instance.MessageLogInfo("Reply: " + replyNOKPalletInfoAck.XMLText);
                                                        LoggerClass.Instance.MainLogInfo("Host >> SendPalletInfoAck: " + sndPalletObj.PalletID + "," + sndPalletObj.EquipmentID + ", COMMACK 1");

                                                        host.ReplyOutStream(replyNOKPalletInfoAck);

                                                        break;

                                                    }
                                                    else
                                                    {
                                                        //
                                                        System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                                                        sqlcommand.Connection.Open();
                                                        int nRet = sqlcommand.ExecuteNonQuery();
                                                        sqlcommand.Connection.Close();
                                                        //


                                                        //Get new nTransID after ASLV updates tblPalletTransaction
                                                        System.Data.SqlClient.SqlDataAdapter adptGetTransID = new SqlDataAdapter(strSqlcommand, cnn);
                                                        System.Data.DataSet dsGetTransID = new System.Data.DataSet();
                                                        adptGetTransID.Fill(dsGetTransID);
                                                        cnn.Close();

                                                        System.Data.DataTableCollection tbGetTransIDs = dsGetTransID.Tables;
                                                        System.Data.DataTable tbTransID = tbGetTransIDs[0];

                                                        foreach (System.Data.DataRow row in tbTransID.Rows)
                                                        {
                                                            nTransID = Int32.Parse(row[0].ToString());            //row[0] = TransID
                                                            //nEquipmentType = Int32.Parse(row[2].ToString());      //row[2] = EquipmentType
                                                            //nNextEquipmentType = Int32.Parse(row[3].ToString());  //row[3] = NextEqupimentType
                                                        }
                                                        //


                                                        foreach (System.Data.DataRow row in tbReqSusp.Rows)
                                                        {
                                                            //row[0]    Id
                                                            strId = row[0].ToString();

                                                            //row[1]    ACAMID
                                                            //row[2]    SuspAmt
                                                            //row[3]    PalletSN
                                                            //row[4]    TransID

                                                            //row[5]    dbo_pallet_Id
                                                            //row[6]    CreatedDateTime
                                                            //row[7]    IsProcessed
                                                            //row[8]    UpdatedDateTime
                                                        }
                                                    }
                                                    //

                                                    //-- EXEC [spUpdateACAMRequestSusp] Id, Pallet Barcode, IsProcessed, UpdatedDateTime,           TransID,                paleltID
                                                    //-- EXEC [spUpdateACAMRequestSusp] 3,  'PT0002',       1,           '2019-06-11 00:00:01.000', /*transID*/ nTransID
                                                    string strStoredProcSQLCmd = "EXEC [spUpdateACAMRequestSusp] " + strId + ", '" + sndPalletObj.PalletID + "',";
                                                    strStoredProcSQLCmd += "0, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "', " + nTransID.ToString();

                                                    System.Data.SqlClient.SqlCommand sqlcmdUpdateACAMRequestSusp = new SqlCommand(strStoredProcSQLCmd, cnn);
                                                    sqlcmdUpdateACAMRequestSusp.Connection.Open();
                                                    int nRetReqSusp = sqlcmdUpdateACAMRequestSusp.ExecuteNonQuery();
                                                    sqlcmdUpdateACAMRequestSusp.Connection.Close();

                                                }
                                                #endregion ASLV


                                                #region additional updates for APT
                                                else if (sndPalletObj.EquipmentType == "80")  //APT
                                                {
                                                    //
                                                    string strId = string.Empty;
                                                    string strSqlReqSuspCommand = "";
                                                    strSqlReqSuspCommand += "SELECT TOP(1) *"
                                                                    + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                    + " WHERE tblReqSusp.PalletSN = '" + sndPalletObj.PalletID + "'"
                                                                    + " AND tblReqSusp.ACAMID = '" + sndPalletObj.EquipmentID + "' AND (tblReqSusp.IsProcessed = 0 OR tblReqSusp.IsProcessed = 3)"
                                                                    + " AND tblReqSusp.TransID IS NOT NULL"
                                                                    + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first

                                                    if (cnn.State == ConnectionState.Closed)
                                                    {
                                                        cnn.Open();
                                                    }

                                                    System.Data.SqlClient.SqlDataAdapter adptReqSusp = new SqlDataAdapter(strSqlReqSuspCommand, cnn);
                                                    System.Data.DataSet dsReqSusp = new System.Data.DataSet();
                                                    adptReqSusp.Fill(dsReqSusp);
                                                    cnn.Close();


                                                    System.Data.DataTableCollection dsTables = dsReqSusp.Tables;
                                                    System.Data.DataTable tbReqSusp = dsTables[0];
                                                    if (tbReqSusp.Rows.Count < 1)
                                                    {
                                                        //no ACAM is requesting suspensions
                                                        //no data found
                                                        //failed, pallet not found

                                                        SCIMessage scndMsgNOKPalletInfoAck = new SCIMessage();
                                                        scndMsgNOKPalletInfoAck.CommandID = "SendPalletInfoAck";
                                                        scndMsgNOKPalletInfoAck.Item = new SCIItem();
                                                        scndMsgNOKPalletInfoAck.Item.Format = SCIFormat.List;
                                                        scndMsgNOKPalletInfoAck.Item.Items = new SCIItemCollection();
                                                        scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                                        scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });

                                                        WDConnect.Common.SCITransaction replyNOKPalletInfoAck = new WDConnect.Common.SCITransaction()
                                                        {
                                                            DeviceId = e.Transaction.DeviceId,
                                                            MessageType = MessageType.Secondary,
                                                            Id = e.Transaction.Id,
                                                            Name = "SendPalletInfoAck",
                                                            NeedReply = false,
                                                            Primary = e.Transaction.Primary,
                                                            Secondary = scndMsgNOKPalletInfoAck
                                                        };

                                                        LoggerClass.Instance.MessageLogInfo("Reply: " + replyNOKPalletInfoAck.XMLText);
                                                        LoggerClass.Instance.MainLogInfo("Host >> SendPalletInfoAck: " + sndPalletObj.PalletID + "," + sndPalletObj.EquipmentID + ", COMMACK 1");

                                                        host.ReplyOutStream(replyNOKPalletInfoAck);

                                                        break;

                                                    }
                                                    else
                                                    {
                                                        //
                                                        System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                                                        sqlcommand.Connection.Open();
                                                        int nRet = sqlcommand.ExecuteNonQuery();
                                                        sqlcommand.Connection.Close();
                                                        //


                                                        foreach (System.Data.DataRow row in tbReqSusp.Rows)
                                                        {
                                                            //row[0]    Id
                                                            strId = row[0].ToString();

                                                            //row[1]    ACAMID
                                                            //row[2]    SuspAmt
                                                            //row[3]    PalletSN
                                                            //row[4]    TransID

                                                            //row[5]    dbo_pallet_Id
                                                            //row[6]    CreatedDateTime
                                                            //row[7]    IsProcessed
                                                            //row[8]    UpdatedDateTime
                                                        }
                                                    }
                                                    //

                                                    //-- EXEC [spUpdateACAMRequestSusp] Id, Pallet Barcode, IsProcessed, UpdatedDateTime,           TransID,                paleltID
                                                    //-- EXEC [spUpdateACAMRequestSusp] 3,  'PT0002',       1,           '2019-06-11 00:00:01.000', /*transID*/ nTransID                                                    
                                                    string strStoredProcSQLCmd = "EXEC [spUpdateACAMRequestSusp] " + strId + ", '" + sndPalletObj.PalletID + "', ";
                                                    strStoredProcSQLCmd += "1, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "', " + nTransID.ToString();

                                                    System.Data.SqlClient.SqlCommand sqlcmdUpdateACAMRequestSusp = new SqlCommand(strStoredProcSQLCmd, cnn);
                                                    sqlcmdUpdateACAMRequestSusp.Connection.Open();
                                                    int nRetReqSusp = sqlcmdUpdateACAMRequestSusp.ExecuteNonQuery();
                                                    sqlcmdUpdateACAMRequestSusp.Connection.Close();
                                                    
                                                }
                                                #endregion additional updates for APT


                                            }
                                            #endregion

                                        }

                                    }//close if (!_hostConfig.RunWithNoDatabase)
                                    else
                                    {
                                        //write to file
                                        RequestPalletInfoAckObj reqPallet = new RequestPalletInfoAckObj();
                                        reqPallet.COMMACK = 0;
                                        reqPallet.ALMID = 0;

                                        reqPallet.PalletID = sndPalletObj.PalletID;
                                        reqPallet.LotNumber = sndPalletObj.LotNumber;


                                        EQUIPMENT_TYPE enEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), sndPalletObj.EquipmentType);
                                        reqPallet.EquipmentType = (int)enEquipmentType;

                                        if ((enEquipmentType == EQUIPMENT_TYPE.ACAM) || (enEquipmentType == EQUIPMENT_TYPE.APT))
                                        {
                                            reqPallet.ACAMID = sndPalletObj.EquipmentID;
                                        }

                                        try
                                        {
                                            reqPallet.Line          = mappingLot_LotInfo[sndPalletObj.LotNumber].Line;
                                            reqPallet.PartNumber    = mappingLot_LotInfo[sndPalletObj.LotNumber].PartNumber;
                                            reqPallet.ProductName   = mappingLot_LotInfo[sndPalletObj.LotNumber].Program;
                                            reqPallet.HGAType       = mappingLot_LotInfo[sndPalletObj.LotNumber].HGAType;
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                                        }

                                        string strNextEquipment = GetNextEquipmentType(sndPalletObj.EquipmentType);
                                        EQUIPMENT_TYPE enNextEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), strNextEquipment);
                                        reqPallet.NextEquipmentType = (int)enNextEquipmentType;

                                        reqPallet.HGA = sndPalletObj.HGA;

                                        string strPalletInfoPath = exePath + @"\Pallet";
                                        System.IO.File.WriteAllText(strPalletInfoPath + @"\" + reqPallet.PalletID + @".xml", reqPallet.ToXML());
                                    }


                                    //replay SCIMessage to tool
                                    //
                                    LoggerClass.Instance.MainLogInfo("Host >> SendPalletInfoAck: " + sendProcessedPalletInfoData.PalletID + "," + sendProcessedPalletInfoData.LotNumber + "," + sendProcessedPalletInfoData.EquipmentID);

                                    SCIMessage secondaryMsgSendProcessedPalletInfoAck = new SCIMessage();
                                    secondaryMsgSendProcessedPalletInfoAck.CommandID = "SendPalletInfoAck";
                                    secondaryMsgSendProcessedPalletInfoAck.Item = new SCIItem();
                                    secondaryMsgSendProcessedPalletInfoAck.Item.Format = SCIFormat.List;
                                    secondaryMsgSendProcessedPalletInfoAck.Item.Items = new SCIItemCollection();
                                    secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                    secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });

                                    WDConnect.Common.SCITransaction toReplySendProcessedPalletInfoAck = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "SendPalletInfoAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = secondaryMsgSendProcessedPalletInfoAck
                                    };

                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendProcessedPalletInfoAck.XMLText);
                                    host.ReplyOutStream(toReplySendProcessedPalletInfoAck);


                                    break;
                                }

                            #endregion case "SendPalletInfo"



                            #region SendStatus
                            /*
                            case "SendStatus":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendStatus = new SCIMessage();
                                scndMsgSendStatus.CommandID = "SendStatusAck";
                                scndMsgSendStatus.Item = new SCIItem();
                                scndMsgSendStatus.Item.Format = SCIFormat.List;
                                scndMsgSendStatus.Item.Items = new SCIItemCollection();
                                scndMsgSendStatus.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplySendStatus = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendStatusAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = scndMsgSendStatus
                                };

                                //*****************
                                Log.Info("Reply: " + toReplySendStatus.XMLText);
                                host.ReplyOutStream(toReplySendStatus);

                                break;
                            */
                            #endregion SendStatus



                            #region case "UnloadPalletToTray"
                            case "UnloadPalletToTray":
                                {
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    PalletToTrayData unloadPallet = new PalletToTrayData();
                                    unloadPallet = GetPalletFromXML_primary(e.Transaction.XMLText);

                                    string strMainLog = "Host << UnloadPalletToTray: " + unloadPallet.PalletID + "," + unloadPallet.TrayID + "," + unloadPallet.RowID.ToString() + "," + unloadPallet.HGALotNumber;
                                    for (int j = 0; j < unloadPallet._hga.Length; j++)
                                    {
                                        strMainLog += "," + unloadPallet._hga[j].SerialNum;
                                    }
                                    LoggerClass.Instance.MainLogInfo(strMainLog);



                                    //
                                    if (!_hostConfig.RunWithNoDatabase)
                                    {

                                        System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                                        cnn.Open();

                                        string strStoredProcSQLCmd = "";
                                        int nTrayTransID = 0;

                                        if (cnn.State == ConnectionState.Open)
                                        {
                                            string strSqlcommand = "";
                                            strSqlcommand += "SELECT TOP(1) *"
                                                        + " FROM [dbo].[tblTrayTransaction] tblTray"
                                                        + " WHERE tblTray.TrayID = '" + /*"AW0162120F"*/ unloadPallet.TrayID + "'"
                                                        + " ORDER BY tblTray.TrayTransID DESC, tblTray.CreatedDateTime DESC";


                                            System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(strSqlcommand, cnn);
                                            System.Data.DataSet dataset = new System.Data.DataSet();
                                            adpt.Fill(dataset);
                                            cnn.Close();

                                            System.Data.DataTableCollection tables = dataset.Tables;
                                            System.Data.DataTable table = tables[0];


                                            #region
                                            if (table.Rows.Count < 1)
                                            {
                                                //no data found; create a new tray

                                                //call [spNewTrayTransaction]
                                                //EXEC [spNewTrayTransaction] 'AW0162120F',
                                                //'PT0001',
                                                //'PT0001',
                                                //'PT0001',
                                                //'PT0001',
                                                //'PT0001',
                                                //'PT0001',
                                                //0,
                                                //'05-03-2017 16:40:00 PM'

                                                strStoredProcSQLCmd += "EXEC [spNewTrayTransaction] '" + unloadPallet.TrayID + "', ";

                                                switch (unloadPallet.RowID)
                                                {
                                                    case 11:
                                                        strStoredProcSQLCmd += "'" + unloadPallet.PalletID + "'," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'',";
                                                        break;

                                                    case 12:
                                                        strStoredProcSQLCmd += "''," +
                                                                               "'" + unloadPallet.PalletID + "'," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'',";
                                                        break;

                                                    case 21:
                                                        strStoredProcSQLCmd += "''," +
                                                                               "''," +
                                                                               "'" + unloadPallet.PalletID + "'," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'',";
                                                        break;

                                                    case 22:
                                                        strStoredProcSQLCmd += "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'" + unloadPallet.PalletID + "'," +
                                                                               "''," +
                                                                               "'',";
                                                        break;

                                                    case 31:
                                                        strStoredProcSQLCmd += "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'" + unloadPallet.PalletID + "'," +
                                                                               "'',";
                                                        break;

                                                    case 32:
                                                        strStoredProcSQLCmd += "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "''," +
                                                                               "'" + unloadPallet.PalletID + "',";
                                                        break;

                                                    default:
                                                        break;
                                                }


                                                strStoredProcSQLCmd += "0, '" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt") + "'";
                                            }
                                            else
                                            {

                                                foreach (System.Data.DataRow row in table.Rows)
                                                {
                                                    nTrayTransID = Int32.Parse(row[0].ToString());              //row[0] = TransID
                                                                                                                //row[1] = TrayID
                                                                                                                //row[2] = PalletID_Row12
                                                }


                                                //call [spUpdateTrayTransactionRowXX] 
                                                //EXEC [spUpdateTrayTransactionRowXX] 1, 'AW0162120F',
                                                //'PT0001'

                                                switch (unloadPallet.RowID)
                                                {
                                                    case 11:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow11] ";
                                                        break;

                                                    case 12:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow12] ";
                                                        break;

                                                    case 21:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow21] ";
                                                        break;

                                                    case 22:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow22] ";
                                                        break;

                                                    case 31:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow31] ";
                                                        break;

                                                    case 32:
                                                        strStoredProcSQLCmd += "EXEC [spUpdateTrayTransactionRow32] ";
                                                        break;

                                                    default:
                                                        break;

                                                }

                                                strStoredProcSQLCmd += nTrayTransID.ToString() + ", '" + unloadPallet.TrayID + "', '" + unloadPallet.PalletID + "'";

                                            }


                                            System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strStoredProcSQLCmd, cnn);
                                            sqlcommand.Connection.Open();
                                            int nRet = sqlcommand.ExecuteNonQuery();
                                            sqlcommand.Connection.Close();

                                            #endregion
                                        }
                                    }

                                    //


                                    SCIMessage secondaryMsgUnloadPalletAck = new SCIMessage();
                                    secondaryMsgUnloadPalletAck.CommandID = "UnloadPalletToTrayAck";
                                    secondaryMsgUnloadPalletAck.Item = new SCIItem();
                                    secondaryMsgUnloadPalletAck.Item.Format = SCIFormat.List;
                                    secondaryMsgUnloadPalletAck.Item.Items = new SCIItemCollection();
                                    secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                    secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = unloadPallet.PalletID });
                                    secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayID", Value = unloadPallet.TrayID });
                                    secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "RowID", Value = unloadPallet.RowID });


                                    WDConnect.Common.SCITransaction toReplyUnloadPalletAck = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "UnloadPalletToTrayAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = secondaryMsgUnloadPalletAck
                                    };

                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyUnloadPalletAck.XMLText);
                                    host.ReplyOutStream(toReplyUnloadPalletAck);

                                }
                                break;

                            #endregion case "UnloadPalletToTray"



                            #region GetTrayInfo
                            /*
                            case "GetTrayInfo":

                                //Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                GetTrayInfoObj gettrayinfo = new GetTrayInfoObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgGetTrayInfoAck = new SCIMessage();
                                secondaryMsgGetTrayInfoAck.CommandID = "GetTrayInfoAck";
                                secondaryMsgGetTrayInfoAck.Item = new SCIItem();
                                secondaryMsgGetTrayInfoAck.Item.Format = SCIFormat.List;
                                secondaryMsgGetTrayInfoAck.Item.Items = new SCIItemCollection();

                                //secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode", Value = gettrayinfo.TrayBarcode });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "IsInProcessLot", Value = false });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = "WWT8G_A" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#1", Value = gettrayinfo.TrayBarcode });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#2", Value = "EZ0001234A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#3", Value = "EZ0000002A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#4", Value = "EZ0000003A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#5", Value = "EZ0000004A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#6", Value = "EZ0000005A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#7", Value = "EZ0000006A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#8", Value = "EZ0000007A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#9", Value = "EZ0000008A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#10", Value = "EZ0000009A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#11", Value = "EZ0000010A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#12", Value = "EZ0000011A" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotStatus", Value = 0 });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "NetworkSpec", Value = "CL6A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProgramName", Value = "FIR-HTI-A" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Product", Value = "FIREBIRD_G8_SD" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Lot3250", Value = "WWT8G_A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "OperationName", Value = "3285" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAPN", Value = "70202-15-SAA" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "QtyIn", Value = "1122" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = "70202-15-SAA" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B408" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = "HTO-T5" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Type", Value = "A" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SuspInv", Value = "0298848" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SuspBatch", Value = "K9RF3" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "OSC", Value = "AC42" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotUsage", Value = "UsingByASAM045" });

                                }
                                else
                                {
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });


                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode", Value = gettrayinfo.TrayBarcode });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "IsInProcessLot", Value = false });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = "" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#1", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#2", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#3", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#4", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#5", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#6", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#7", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#8", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#9", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#10", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#11", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode#12", Value = "" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotStatus", Value = 0 });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "NetworkSpec", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProgramName", Value = "" });

                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Product", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Lot3250", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "OperationName", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAPN", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "QtyIn", Value = 0 });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Type", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SuspInv", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SuspBatch", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "OSC", Value = "" });
                                    secondaryMsgGetTrayInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotUsage", Value = "" });

                                }



                                WDConnect.Common.SCITransaction toReplyGetTrayInfoAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "GetTrayInfoAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgGetTrayInfoAck
                                };

                                Log.Info("Reply: " + toReplyGetTrayInfoAck.XMLText);
                                host.ReplyOutStream(toReplyGetTrayInfoAck);

                                break;

                            */
                            #endregion GetTrayInfo



                            #region UpdateTrayProcess
                            /*
                            case "UpdateTrayProcess":

                                //Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                UpdateTrayProcessObj updatetrayProcObj = new UpdateTrayProcessObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgUpdateTrayProcessAck = new SCIMessage();
                                secondaryMsgUpdateTrayProcessAck.CommandID = "UpdateTrayProcessAck";
                                secondaryMsgUpdateTrayProcessAck.Item = new SCIItem();
                                secondaryMsgUpdateTrayProcessAck.Item.Format = SCIFormat.List;
                                secondaryMsgUpdateTrayProcessAck.Item.Items = new SCIItemCollection();

                                secondaryMsgUpdateTrayProcessAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });                                
                                secondaryMsgUpdateTrayProcessAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode", Value = updatetrayProcObj.TrayBarcode });
                                secondaryMsgUpdateTrayProcessAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = updatetrayProcObj.LotNumber });

                                WDConnect.Common.SCITransaction toReplyUpdateTrayProcessAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "UpdateTrayProcessAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgUpdateTrayProcessAck
                                };

                                Log.Info("Reply: " + toReplyUpdateTrayProcessAck.XMLText);
                                host.ReplyOutStream(toReplyUpdateTrayProcessAck);


                                break;
                            */
                            #endregion UpdateTrayProcess



                            #region SendTrayStatus
                            /*
                            case "SendTrayStatus":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                UpdateTrayProcessObj sendtrayStatusObj = new UpdateTrayProcessObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgSendTrayStatusAck = new SCIMessage();
                                secondaryMsgSendTrayStatusAck.CommandID = "SendTrayStatusAck";
                                secondaryMsgSendTrayStatusAck.Item = new SCIItem();
                                secondaryMsgSendTrayStatusAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendTrayStatusAck.Item.Items = new SCIItemCollection();

                                secondaryMsgSendTrayStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                secondaryMsgSendTrayStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode", Value = sendtrayStatusObj.TrayBarcode });
                                secondaryMsgSendTrayStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = sendtrayStatusObj.LotNumber });

                                WDConnect.Common.SCITransaction toReplySendTrayStatusAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendTrayStatusAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgSendTrayStatusAck
                                };

                                Log.Info("Reply: " + toReplySendTrayStatusAck.XMLText);
                                host.ReplyOutStream(toReplySendTrayStatusAck);

                                break;

                            */
                            #endregion SendTrayStatus



                            #region RequestUnloadTray
                            /*
                            case "RequestUnloadTray":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                UpdateTrayProcessObj requestUnloadTrayObj = new UpdateTrayProcessObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgRequestUnloadTrayAck = new SCIMessage();
                                secondaryMsgRequestUnloadTrayAck.CommandID = "RequestUnloadTrayAck";
                                secondaryMsgRequestUnloadTrayAck.Item = new SCIItem();
                                secondaryMsgRequestUnloadTrayAck.Item.Format = SCIFormat.List;
                                secondaryMsgRequestUnloadTrayAck.Item.Items = new SCIItemCollection();

                                secondaryMsgRequestUnloadTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                secondaryMsgRequestUnloadTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayBarcode", Value = requestUnloadTrayObj.TrayBarcode });
                                
                                //secondaryMsgRequestUnloadTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "CanUnload", Value = true });                                
                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    secondaryMsgRequestUnloadTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "CanUnload", Value = true });
                                }
                                else
                                {
                                    secondaryMsgRequestUnloadTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "CanUnload", Value = false });
                                }


                                WDConnect.Common.SCITransaction toReplyRequestUnloadTrayAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "RequestUnloadTrayAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgRequestUnloadTrayAck
                                };

                                Log.Info("Reply: " + toReplyRequestUnloadTrayAck.XMLText);
                                host.ReplyOutStream(toReplyRequestUnloadTrayAck);

                                break;
                            */
                            #endregion RequestUnloadTray



                            #region RequestCancelProcessedLot
                            /*
                            case "RequestCancelProcessedLot":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                //UpdateTrayProcessObj requestCancelProcessedLotObj = new UpdateTrayProcessObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgRequestCancelProcessedLotAck = new SCIMessage();
                                secondaryMsgRequestCancelProcessedLotAck.CommandID = "RequestCancelProcessedLotAck";
                                secondaryMsgRequestCancelProcessedLotAck.Item = new SCIItem();
                                secondaryMsgRequestCancelProcessedLotAck.Item.Format = SCIFormat.List;
                                secondaryMsgRequestCancelProcessedLotAck.Item.Items = new SCIItemCollection();

                                //secondaryMsgRequestCancelProcessedLotAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    secondaryMsgRequestCancelProcessedLotAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                }
                                else
                                {
                                    secondaryMsgRequestCancelProcessedLotAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                }


                                WDConnect.Common.SCITransaction toReplyRequestCancelProcessedLotAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "RequestCancelProcessedLotAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgRequestCancelProcessedLotAck
                                };

                                Log.Info("Reply: " + toReplyRequestCancelProcessedLotAck.XMLText);
                                host.ReplyOutStream(toReplyRequestCancelProcessedLotAck);

                                break;
                            */
                            #endregion RequestCancelProcessedLot



                            #region RequestRemoveRemainTray
                            /*
                            case "RequestRemoveRemainTray":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                UpdateTrayProcessObj requestRemoveRemainTrayObj = new UpdateTrayProcessObj(e.Transaction.XMLText);


                                SCIMessage secondaryMsgRequestRemoveRemainTrayAck = new SCIMessage();
                                secondaryMsgRequestRemoveRemainTrayAck.CommandID = "RequestRemoveRemainTrayAck";
                                secondaryMsgRequestRemoveRemainTrayAck.Item = new SCIItem();
                                secondaryMsgRequestRemoveRemainTrayAck.Item.Format = SCIFormat.List;
                                secondaryMsgRequestRemoveRemainTrayAck.Item.Items = new SCIItemCollection();

                                //secondaryMsgRequestRemoveRemainTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                rnd = new Random(Environment.TickCount);
                                int nRnd = rnd.Next(0, 5);
                                if (nRnd < 3)
                                {
                                    secondaryMsgRequestRemoveRemainTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                }
                                else if (nRnd == 4)
                                {
                                    secondaryMsgRequestRemoveRemainTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                }
                                else
                                {
                                    secondaryMsgRequestRemoveRemainTrayAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 2 });
                                }


                                WDConnect.Common.SCITransaction toReplyRequestRemoveRemainTrayAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "RequestRemoveRemainTrayAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgRequestRemoveRemainTrayAck
                                };

                                Log.Info("Reply: " + toReplyRequestRemoveRemainTrayAck.XMLText);
                                host.ReplyOutStream(toReplyRequestRemoveRemainTrayAck);

                                break;
                            */
                            #endregion RequestRemoveRemainTray



                            #region SendPkgSetting
                            /*
                            case "SendPkgSetting":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendPkgSettingAck = new SCIMessage();
                                scndMsgSendPkgSettingAck.CommandID = "SendPkgSettingAck";
                                scndMsgSendPkgSettingAck.Item = new SCIItem();
                                scndMsgSendPkgSettingAck.Item.Format = SCIFormat.List;
                                scndMsgSendPkgSettingAck.Item.Items = new SCIItemCollection();
                                scndMsgSendPkgSettingAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplySendPkgSetting = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendPkgSettingAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = scndMsgSendPkgSettingAck
                                };

                                //*****************
                                Log.Info("Reply: " + toReplySendPkgSetting.XMLText);
                                host.ReplyOutStream(toReplySendPkgSetting);

                                break;
                            */
                            #endregion SendPkgSetting



                            #region case "AlarmReportSend"
                            //3.31	AlarmReportSend
                            case "AlarmReportSend":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgAlarmReportSendAck = new SCIMessage();
                                scndMsgAlarmReportSendAck.CommandID = "AlarmReportSendAck";
                                scndMsgAlarmReportSendAck.Item = new SCIItem();
                                scndMsgAlarmReportSendAck.Item.Format = SCIFormat.List;
                                scndMsgAlarmReportSendAck.Item.Items = new SCIItemCollection();
                                scndMsgAlarmReportSendAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplyAlarmReportSend = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "AlarmReportSendAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = scndMsgAlarmReportSendAck
                                };

                                //*****************
                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyAlarmReportSend.XMLText);
                                host.ReplyOutStream(toReplyAlarmReportSend);

                                break;

                            #endregion case "AlarmReportSend"



                            #region case "SendControlState"
                            case "SendControlState":

                               host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendControlStateAck = new SCIMessage();
                                scndMsgSendControlStateAck.CommandID = "SendControlStateAck";
                                scndMsgSendControlStateAck.Item = new SCIItem();
                                scndMsgSendControlStateAck.Item.Format = SCIFormat.List;
                                scndMsgSendControlStateAck.Item.Items = new SCIItemCollection();
                                scndMsgSendControlStateAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplySendControlStateAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendControlStateAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = scndMsgSendControlStateAck
                                };

                                //*****************
                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendControlStateAck.XMLText);
                                host.ReplyOutStream(toReplySendControlStateAck);
                                break;

                            #endregion case "SendControlState"



                            #region case "SendProcessState"
                            case "SendProcessState":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendProcessStateAck = new SCIMessage();
                                scndMsgSendProcessStateAck.CommandID = "SendProcessStateAck";
                                scndMsgSendProcessStateAck.Item = new SCIItem();
                                scndMsgSendProcessStateAck.Item.Format = SCIFormat.List;
                                scndMsgSendProcessStateAck.Item.Items = new SCIItemCollection();
                                scndMsgSendProcessStateAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplySendProcessStateAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendProcessStateAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = scndMsgSendProcessStateAck
                                };

                                //*****************
                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendProcessStateAck.XMLText);
                                host.ReplyOutStream(toReplySendProcessStateAck);
                                break;

                            #endregion case "SendProcessState"



                            #region case "RequestProcessRecipe"
                            case "RequestProcessRecipe":
                                {
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    RequestProcessRecipeStruct reqProcRecipeObj = GetRequestProcessRecipeFromXML_primary(e.Transaction.XMLText);
                                    LoggerClass.Instance.MainLogInfo("Host << RequestProcessRecipe: " + reqProcRecipeObj.EquipmentID + "," + reqProcRecipeObj.LotNumber);

                                    RequestProcessRecipeAckObj recipe = null;
                                    LotInformation lotinfo = new LotInformation();
                                    Tuple<string, string> tPartNumberSTR = null;
                                    try
                                    {
                                        //1* map lot number to lotinfo
                                        //2* map lotinfo (lotinfo.PartNumber and lotinfo.STR) to recipe

                                        lotinfo = mappingLot_LotInfo[reqProcRecipeObj.LotNumber];
                                        tPartNumberSTR = new Tuple<string, string>(lotinfo.PartNumber, lotinfo.STR);

                                        recipe = mappingPartNumberSTR_ReqRecipeObj[tPartNumberSTR];

                                        if (File.Exists(exePath + @"\Recipe\ILCRecipeObj.xml"))
                                        {
                                            ILCRecipeObj ilcRecipe1 = ILCRecipeObj.ReadFromFile(exePath + @"\Recipe\ILCRecipeObj.xml");

                                            recipe.PowerUV1 = ilcRecipe1.PowerUV1;
                                            recipe.CureTimeUV1 = ilcRecipe1.CureTimeUV1;
                                            recipe.EnabledUV1 = ilcRecipe1.EnabledUV1;

                                            recipe.FlowRateHeater1 = ilcRecipe1.FlowRateHeater1;
                                            recipe.TempHeater1 = ilcRecipe1.TempHeater1;
                                            recipe.EnabledHeater1 = ilcRecipe1.EnabledHeater1;
                                            recipe.EnabledN2Heater1 = ilcRecipe1.EnabledN2Heater1;

                                            recipe.FlowRateHeater2 = ilcRecipe1.FlowRateHeater2;
                                            recipe.TempHeater2 = ilcRecipe1.TempHeater2;
                                            recipe.EnabledHeater2 = ilcRecipe1.EnabledHeater2;
                                            recipe.EnabledN2Heater2 = ilcRecipe1.EnabledN2Heater2;

                                            recipe.FlowRateHeater3 = ilcRecipe1.FlowRateHeater3;
                                            recipe.TempHeater3 = ilcRecipe1.TempHeater3;
                                            recipe.EnabledHeater3 = ilcRecipe1.EnabledHeater3;
                                            recipe.EnabledN2Heater3 = ilcRecipe1.EnabledN2Heater3;

                                            recipe.FlowRateHeater4 = ilcRecipe1.FlowRateHeater4;
                                            recipe.TempHeater4 = ilcRecipe1.TempHeater4;
                                            recipe.EnabledHeater4 = ilcRecipe1.EnabledHeater4;
                                            recipe.EnabledN2Heater4 = ilcRecipe1.EnabledN2Heater4;

                                            recipe.FlowRateHeater5 = ilcRecipe1.FlowRateHeater5;
                                            recipe.TempHeater5 = ilcRecipe1.TempHeater5;
                                            recipe.EnabledHeater5 = ilcRecipe1.EnabledHeater5;
                                            recipe.EnabledN2Heater5 = ilcRecipe1.EnabledN2Heater5;

                                            recipe.PowerUV2 = ilcRecipe1.PowerUV2;
                                            recipe.CureTimeUV2 = ilcRecipe1.CureTimeUV2;
                                            recipe.EnabledUV2 = ilcRecipe1.EnabledUV2;

                                            recipe.FlowRateHeater6 = ilcRecipe1.FlowRateHeater6;
                                            recipe.TempHeater6 = ilcRecipe1.TempHeater6;
                                            recipe.EnabledHeater6 = ilcRecipe1.EnabledHeater6;
                                            recipe.EnabledN2Heater6 = ilcRecipe1.EnabledN2Heater6;

                                            recipe.FlowRateHeater7 = ilcRecipe1.FlowRateHeater7;
                                            recipe.TempHeater7 = ilcRecipe1.TempHeater7;
                                            recipe.EnabledHeater7 = ilcRecipe1.EnabledHeater7;
                                            recipe.EnabledN2Heater7 = ilcRecipe1.EnabledN2Heater7;

                                            recipe.FlowRateHeater8 = ilcRecipe1.FlowRateHeater8;
                                            recipe.TempHeater8 = ilcRecipe1.TempHeater8;
                                            recipe.EnabledHeater8 = ilcRecipe1.EnabledHeater8;
                                            recipe.EnabledN2Heater8 = ilcRecipe1.EnabledN2Heater8;

                                            recipe.FlowRateHeater9 = ilcRecipe1.FlowRateHeater9;
                                            recipe.TempHeater9 = ilcRecipe1.TempHeater9;
                                            recipe.EnabledHeater9 = ilcRecipe1.EnabledHeater9;
                                            recipe.EnabledN2Heater9 = ilcRecipe1.EnabledN2Heater9;

                                            recipe.FlowRateHeater10 = ilcRecipe1.FlowRateHeater10;
                                            recipe.TempHeater10 = ilcRecipe1.TempHeater10;
                                            recipe.EnabledHeater10 = ilcRecipe1.EnabledHeater10;
                                            recipe.EnabledN2Heater10 = ilcRecipe1.EnabledN2Heater10;

                                            recipe.Mode = ilcRecipe1.Mode;
                                            recipe.Bypass = ilcRecipe1.Bypass;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (recipe == null)
                                        {
                                            //MessageBox.Show("null");
                                            recipe = new RequestProcessRecipeAckObj();
                                            recipe.COMMACK = 1;
                                        }
                                    }

                                    SCIMessage scndMsgReqProcessRecipeAck = new SCIMessage();
                                    scndMsgReqProcessRecipeAck.CommandID = "RequestProcessRecipeAck";
                                    scndMsgReqProcessRecipeAck.Item = new SCIItem();
                                    scndMsgReqProcessRecipeAck.Item.Format = SCIFormat.List;
                                    scndMsgReqProcessRecipeAck.Item.Items = new SCIItemCollection();


                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = recipe.COMMACK });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = recipe.ALMID });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqProcRecipeObj.PalletID });
                                    //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = reqProcRecipeObj.LotNumber });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = lotinfo.LotNumber });
                                    //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotSize", Value = recipe.LotQty });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotSize", Value = lotinfo.Qty });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeID", Value = recipe.RecipeID });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeName", Value = recipe.ProductName });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = recipe.Line });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumber", Value = recipe.PartNumber });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProductName", Value = recipe.ProductName });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = recipe.STR });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = recipe.Suspension });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SuspPartNumber", Value = recipe.SuspPartNumber });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAType", Value = recipe.HGAType });


                                    //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "UVPower", Value = 200 });        //obsolete
                                    //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime", Value = 15.5 });        //obsolete
                                    //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "CureZone", Value = 0 });         //obsolete


                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV1", Value = recipe.PowerUV1 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV1", Value = recipe.CureTimeUV1 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV1", Value = recipe.EnabledUV1 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater1", Value = recipe.FlowRateHeater1 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater1", Value = recipe.TempHeater1 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater1", Value = recipe.EnabledHeater1 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater1", Value = recipe.EnabledN2Heater1 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater2", Value = recipe.FlowRateHeater2 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater2", Value = recipe.TempHeater2 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater2", Value = recipe.EnabledHeater2 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater2", Value = recipe.EnabledN2Heater2 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater3", Value = recipe.FlowRateHeater3 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater3", Value = recipe.TempHeater3 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater3", Value = recipe.EnabledHeater3 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater3", Value = recipe.EnabledN2Heater3 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater4", Value = recipe.FlowRateHeater4 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater4", Value = recipe.TempHeater4 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater4", Value = recipe.EnabledHeater4 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater4", Value = recipe.EnabledN2Heater4 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater5", Value = recipe.FlowRateHeater5 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater5", Value = recipe.TempHeater5 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater5", Value = recipe.EnabledHeater5 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater5", Value = recipe.EnabledN2Heater5 });


                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV2", Value = recipe.PowerUV2 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV2", Value = recipe.CureTimeUV2 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV2", Value = recipe.EnabledUV2 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater6", Value = recipe.FlowRateHeater6 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater6", Value = recipe.TempHeater6 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater6", Value = recipe.EnabledHeater6 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater6", Value = recipe.EnabledN2Heater6 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater7", Value = recipe.FlowRateHeater7 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater7", Value = recipe.TempHeater7 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater7", Value = recipe.EnabledHeater7 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater7", Value = recipe.EnabledN2Heater7 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater8", Value = recipe.FlowRateHeater8 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater8", Value = recipe.TempHeater8 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater8", Value = recipe.EnabledHeater8 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater8", Value = recipe.EnabledN2Heater8 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater9", Value = recipe.FlowRateHeater9 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater9", Value = recipe.TempHeater9 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater9", Value = recipe.EnabledHeater9 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater9", Value = recipe.EnabledN2Heater9 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater10", Value = recipe.FlowRateHeater10 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater10", Value = recipe.TempHeater10 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater10", Value = recipe.EnabledHeater10 });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater10", Value = recipe.EnabledN2Heater10 });

                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Mode", Value = recipe.Mode });
                                    scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Bypass", Value = recipe.Bypass });

                                    WDConnect.Common.SCITransaction toReplyReqProcessRecipe = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "RequestProcessRecipeAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = scndMsgReqProcessRecipeAck
                                    };

                                    //*****************
                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyReqProcessRecipe.XMLText);

                                    string strToReply = string.Format("{0},{1},{2},{3},{4}", reqProcRecipeObj.EquipmentID, 
                                                                                        recipe.RecipeID, 
                                                                                        recipe.ProductName,
                                                                                        lotinfo.LotNumber, 
                                                                                        lotinfo.Qty.ToString());
                                    LoggerClass.Instance.MainLogInfo("Host >> RequestProcessRecipeAck: " + strToReply);

                                    // lotinfo.LotNumber });
                                    //= recipe.LotQty });
                                    //lotinfo.Qty });
                                    
                                    //recipe.RecipeID });
                                    //= recipe.ProductName }

                                    host.ReplyOutStream(toReplyReqProcessRecipe);

                                }
                                break;

                            #endregion case "RequestProcessRecipe"



                            #region case "SendPalletStatus"
                            case "SendPalletStatus":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                PalletToTrayData sendPalletStatusData = new PalletToTrayData();
                                sendPalletStatusData = GetPalletFromXML_primary(e.Transaction.XMLText);


                                SCIMessage secondaryMsgSendPalletStatusAck = new SCIMessage();
                                secondaryMsgSendPalletStatusAck.CommandID = "SendPalletStatusAck";
                                secondaryMsgSendPalletStatusAck.Item = new SCIItem();
                                secondaryMsgSendPalletStatusAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendPalletStatusAck.Item.Items = new SCIItemCollection();
                                secondaryMsgSendPalletStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                secondaryMsgSendPalletStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendPalletStatusData.PalletID });

                                WDConnect.Common.SCITransaction toReplySendPalletStatusAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendPalletStatusAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgSendPalletStatusAck
                                };

                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendPalletStatusAck.XMLText);
                                host.ReplyOutStream(toReplySendPalletStatusAck);

                                break;

                            #endregion case "SendPalletStatus"



                            #region case "SendAlarmReport"
                            case "SendAlarmReport":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage secondaryMsgSendAlarmReportAck = new SCIMessage();
                                secondaryMsgSendAlarmReportAck.CommandID = "SendAlarmReportAck";
                                secondaryMsgSendAlarmReportAck.Item = new SCIItem();
                                secondaryMsgSendAlarmReportAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendAlarmReportAck.Item.Items = new SCIItemCollection();
                                secondaryMsgSendAlarmReportAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReplySendAlarmReportAck = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendAlarmReportAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgSendAlarmReportAck
                                };

                                LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendAlarmReportAck.XMLText);
                                host.ReplyOutStream(toReplySendAlarmReportAck);

                                break;

                            #endregion case "SendAlarmReport"



                            #region case "RequestSuspension"
                            case "RequestSuspension":
                                {
                                    RequestSuspensionClass reqSusp = new RequestSuspensionClass(e.Transaction.XMLText);
                                    LoggerClass.Instance.MainLogInfo("Host << RequestSuspension: " + reqSusp.ACAMID + "," + reqSusp.SuspAmt.ToString());

                                    int nCOMMACK = 1;
                                    //EXEC [spNewACAMRequestSusp] 'APT002', 10, null, null, null, '2019-06-11 00:00:01.000'
                                    string strReqSuspSQLCmd = "EXEC [spNewACAMRequestSusp] '" + reqSusp.ACAMID + "', " + reqSusp.SuspAmt.ToString() + ", null, null, null, '";
                                    strReqSuspSQLCmd += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "'";

                                    try
                                    {
                                        System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);


                                        //
                                        // check number of already in-system pallets for ACAM
                                        string strSqlCheckPendingACAMPalletNums = "";
                                        strSqlCheckPendingACAMPalletNums += "SELECT COUNT(Id)"
                                                    + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                    + " WHERE (tblReqSusp.IsProcessed = 0 OR tblReqSusp.IsProcessed = 3)"  /* pending ACAM, IsProcessed = 0 */
                                                    //+ " WHERE tblReqSusp.IsProcessed IS NULL"  /* pending ASLV, IsProcessed IS NULL */
                                                    + " AND tblReqSusp.ACAMID = '" + reqSusp.ACAMID + "'";


                                        if (cnn.State == ConnectionState.Closed)
                                        {
                                            cnn.Open();

                                            System.Data.SqlClient.SqlDataAdapter adptCheckPallets = new SqlDataAdapter(strSqlCheckPendingACAMPalletNums, cnn);
                                            System.Data.DataSet dsCheckPallets = new System.Data.DataSet();
                                            adptCheckPallets.Fill(dsCheckPallets);

                                            System.Data.DataTableCollection dsCheckPalletsTables = dsCheckPallets.Tables;
                                            System.Data.DataTable tbCheckPallets = dsCheckPalletsTables[0];

                                            cnn.Close();

                                            if (tbCheckPallets.Rows.Count < 1)
                                            {
                                                //error
                                                nCOMMACK = 1;
                                            }
                                            else
                                            {
                                                foreach (System.Data.DataRow r in tbCheckPallets.Rows)
                                                {
                                                    int nNumOfPallets = Int32.Parse(r[0].ToString());
                                                    if (nNumOfPallets > 8)  //limit number of requesting pallets to no MORE than 4 (in machine) + 3 extra 
                                                    {
                                                        nCOMMACK = 1;
                                                    }
                                                    else
                                                    {
                                                        System.Data.SqlClient.SqlCommand sqlcmdNewACAMRequestSusp = new SqlCommand(strReqSuspSQLCmd, cnn);
                                                        sqlcmdNewACAMRequestSusp.Connection.Open();
                                                        int nRetACAMReqSusp = sqlcmdNewACAMRequestSusp.ExecuteNonQuery();
                                                        sqlcmdNewACAMRequestSusp.Connection.Close();

                                                        nCOMMACK = 0;
                                                    }
                                                }
                                            }
                                        }

                                        //

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }


                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    SCIMessage secondaryMsgReqSuspensionAck = new SCIMessage();
                                    secondaryMsgReqSuspensionAck.CommandID = "RequestSuspensionAck";
                                    secondaryMsgReqSuspensionAck.Item = new SCIItem();
                                    secondaryMsgReqSuspensionAck.Item.Format = SCIFormat.List;
                                    secondaryMsgReqSuspensionAck.Item.Items = new SCIItemCollection();
                                    secondaryMsgReqSuspensionAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = nCOMMACK });


                                    WDConnect.Common.SCITransaction toReplyReqSuspensionAck = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "RequestSuspensionAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = secondaryMsgReqSuspensionAck
                                    };

                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyReqSuspensionAck.XMLText);
                                    LoggerClass.Instance.MainLogInfo("Host >> RequestSuspensionAck: " + reqSusp.ACAMID + "," + reqSusp.SuspAmt.ToString() + ", COMMACK " + nCOMMACK.ToString());
                                    host.ReplyOutStream(toReplyReqSuspensionAck);

                                }

                                break;
                            #endregion



                            #region case "RequestSwapPallet":
                            case "RequestSwapPallet":
                                RequestSwapPalletClass reqSwap = new RequestSwapPalletClass(e.Transaction.XMLText);
                                LoggerClass.Instance.MainLogInfo("Host << RequestSwapPallet: " + reqSwap.PalletID + "," + reqSwap.ACAMID);

                                if (!_hostConfig.RunWithNoDatabase)
                                {
                                    int nTransID = 0;
                                    int nEquipmentType = 0;
                                    int nNextEquipmentType = 0;
                                    //bool bEnabledPallet = false;

                                    System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);



                                    int nCOMMACK = 1;       //for acknowledge part
                                    //string strPalletToSwap = string.Empty;


                                    string strSwapFrom_Id = string.Empty;
                                    string strSwapFrom_ACAMID = string.Empty;
                                    string strSwapFrom_TransID = string.Empty;

                                    string strSwapTo_Id = string.Empty;
                                    string strSwapTo_ACAMID = string.Empty;
                                    string strSwapTo_PalletID = string.Empty;
                                    string strSwapTo_TransID = string.Empty;

                                    try
                                    {
                                        cnn.Open();

                                        #region check reqSwap.PalletID in tblPalletTransaction
                                        if (cnn.State == ConnectionState.Open)
                                        {
                                            string sqlcommand = "";
                                            sqlcommand += "SELECT TOP(1) *"
                                                        + " FROM [dbo].[tblPalletTransaction] tblPallet"
                                                        + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ reqSwap.PalletID + "'"
                                                        + " ORDER BY tblPallet.TransID DESC";


                                            System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                                            System.Data.DataSet dataset = new System.Data.DataSet();
                                            adpt.Fill(dataset);
                                            cnn.Close();


                                            System.Data.DataTableCollection tables = dataset.Tables;
                                            System.Data.DataTable table = tables[0];

                                            if (table.Rows.Count < 1)
                                            {
                                                //no such reqSwap.PalletID found in database
                                                nCOMMACK = 1;
                                            }
                                            else
                                            {
                                                #region foreach
                                                foreach (System.Data.DataRow row in table.Rows)
                                                {
                                                    //row[0] = TransID
                                                    //row[1] = PalletID
                                                    //row[2] = EquipmentType
                                                    //row[3] = NextEquipmentType
                                                    //row[4] = CreatedDateTime

                                                    //row[5] = LotNumber
                                                    //row6 = HGASN_1
                                                    //row7 = HGADefect_1
                                                    //row8 = HGASN_2
                                                    //row9 = HGADefect_2
                                                    //row10 = HGASN_3
                                                    //row11 = HGADefect_3
                                                    //row12 = HGASN_4
                                                    //row13 = HGADefect_4
                                                    //row14 = HGASN_5
                                                    //row15 = HGADefect_5

                                                    //row16 = HGASN_6
                                                    //row17 = HGADefect_6
                                                    //row18 = HGASN_7
                                                    //row19 = HGADefect_7
                                                    //row20 = HGASN_8
                                                    //row21 = HGADefect_8
                                                    //row22 = HGASN_9
                                                    //row23 = HGADefect_9
                                                    //row24 = HGASN_10
                                                    //row25 = HGADefect_10

                                                    //row26 = ACAMID

                                                    //row27 = ILC UVPower
                                                    //row28 = ILC CureTime
                                                    //row29 = ILC CureZone

                                                    //row30 = SJBStage


                                                    nTransID = Int32.Parse(row[0].ToString());

                                                    nEquipmentType = Int32.Parse(row[2].ToString());
                                                    nNextEquipmentType = Int32.Parse(row[3].ToString());

                                                    //check equipment type
                                                    //if reqPallet.EquipmentType != row[3] = NextEquipmentType
                                                    //if not matched; enabled = false

                                                    //the requesting equipment should be APT or 80
                                                    strSwapTo_ACAMID = reqSwap.ACAMID == "APT001" ? "APT002" : "APT001";


                                                    EQUIPMENT_TYPE enCheckEquipmentType = (EQUIPMENT_TYPE) reqSwap.EquipmentType;   
                                                    if (enCheckEquipmentType == ((EQUIPMENT_TYPE)Int32.Parse(row[3].ToString()))) //check NextEquipmentType against tool's EquipmentType
                                                    {
                                                        //bEnabledPallet = true;

                                                        #region check pallet to swap in tblACAMRequestSusp
                                                        // /////////////////////////////////////////////////////////////////
                                                        // check reqSwap.PalletID in tblACAMRequestSusp
                                                        string strSqlSwapFromCmd = "";
                                                        strSqlSwapFromCmd += "SELECT TOP(1) *"
                                                                    + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                    + " WHERE tblReqSusp.PalletSN = '" + reqSwap.PalletID + "'"
                                                                    + " AND tblReqSusp.TransID = " + nTransID.ToString()
                                                                    + " AND tblReqSusp.IsProcessed IS NOT NULL"
                                                                    + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first


                                                        // look for pallet to swap in tblACAMRequestSusp
                                                        string strSqlSwapToCmd = "";
                                                        strSqlSwapToCmd += "SELECT TOP(1) *"
                                                                    + " FROM [dbo].[tblACAMRequestSusp] tblReqSusp"
                                                                    + " WHERE tblReqSusp.ACAMID = '" + strSwapTo_ACAMID + "'"
                                                                    + " AND tblReqSusp.IsProcessed = 0"
                                                                    + " AND tblReqSusp.SuspAmt = 10"
                                                                    + " ORDER BY tblReqSusp.Id ASC";    //first come, first served; serve oldest first

                                                        if (cnn.State == ConnectionState.Closed)
                                                        {
                                                            cnn.Open();
                                                        }

                                                        System.Data.SqlClient.SqlDataAdapter adptSwapFrom = new SqlDataAdapter(strSqlSwapFromCmd, cnn);
                                                        System.Data.DataSet dsSwapFrom = new System.Data.DataSet();
                                                        adptSwapFrom.Fill(dsSwapFrom);

                                                        System.Data.DataTableCollection dsSwapFromTables = dsSwapFrom.Tables;
                                                        System.Data.DataTable tbSwapFrom = dsSwapFromTables[0];



                                                        System.Data.SqlClient.SqlDataAdapter adptSwapTo = new SqlDataAdapter(strSqlSwapToCmd, cnn);
                                                        System.Data.DataSet dsSwapTo = new System.Data.DataSet();
                                                        adptSwapTo.Fill(dsSwapTo);

                                                        System.Data.DataTableCollection dsSwapToTables = dsSwapTo.Tables;
                                                        System.Data.DataTable tbSwapTo = dsSwapToTables[0];

                                                        cnn.Close();

                                                        int nSwapFromACAMIDSuspAmt = 10;    //default value 10
                                                        int nSwapToACAMIDSuspAmt = 10;      //default value 10

                                                        if (tbSwapFrom.Rows.Count < 1)
                                                        {
                                                            //no ACAM is requesting suspensions
                                                            nCOMMACK = 1;
                                                        }
                                                        else
                                                        {
                                                            foreach (System.Data.DataRow r in tbSwapFrom.Rows)
                                                            {
                                                                //row[0]    Id
                                                                strSwapFrom_Id = r[0].ToString();

                                                                //row[1]    ACAMID
                                                                strSwapFrom_ACAMID = r[1].ToString();

                                                                //row[2]    SuspAmt
                                                                nSwapFromACAMIDSuspAmt = Int32.Parse(r[2].ToString());

                                                                //row[3]    PalletSN
                                                                //row[4]    TransID
                                                                strSwapFrom_TransID = r[4].ToString();

                                                                //row[5]    dbo_pallet_Id
                                                                //row[6]    CreatedDateTime
                                                                //row[7]    IsProcessed
                                                                //row[8]    UpdatedDateTime
                                                            }

                                                            if (nSwapFromACAMIDSuspAmt < 10)
                                                            {
                                                                //don't allow swapping
                                                                nCOMMACK = 1;
                                                            }

                                                        }


                                                        //string strSwapTo_Id = string.Empty;
                                                        //string strSwapTo_ACAMID = string.Empty;
                                                        //string strSwapTo_PalletID = string.Empty;

                                                        if (tbSwapTo.Rows.Count < 1)
                                                        {
                                                            //no pallet to swap
                                                            nCOMMACK = 1;
                                                        }
                                                        else
                                                        {
                                                            foreach (System.Data.DataRow r in tbSwapTo.Rows)
                                                            {
                                                                //row[0]    Id
                                                                strSwapTo_Id = r[0].ToString();

                                                                //row[1]    ACAMID
                                                                strSwapTo_ACAMID = r[1].ToString();

                                                                //row[2]    SuspAmt
                                                                nSwapToACAMIDSuspAmt = Int32.Parse(r[2].ToString());


                                                                //row[3]    PalletSN
                                                                strSwapTo_PalletID = r[3].ToString();

                                                                //row[4]    TransID
                                                                strSwapTo_TransID = r[4].ToString();

                                                                //row[5]    dbo_pallet_Id 
                                                                //row[6]    CreatedDateTime
                                                                //row[7]    IsProcessed
                                                                //row[8]    UpdatedDateTime
                                                            }


                                                            //strSwapFrom_ACAMID <=> strSwapToACAMID
                                                            if (cnn.State == ConnectionState.Closed)
                                                            {
                                                                cnn.Open();
                                                            }

                                                            SqlCommand cmdSqlSwapFromPallet = cnn.CreateCommand();
                                                            cmdSqlSwapFromPallet.CommandText += "UPDATE [dbo].[tblACAMRequestSusp]"
                                                                                                + " SET ACAMID = '" + strSwapTo_ACAMID + "'"
                                                                                                + " WHERE [dbo].[tblACAMRequestSusp].Id =" + strSwapFrom_Id;


                                                            SqlCommand cmdSqlSwapToPallet = cnn.CreateCommand();
                                                            cmdSqlSwapToPallet.CommandText += "UPDATE [dbo].[tblACAMRequestSusp]"
                                                                                            + " SET ACAMID = '" + strSwapFrom_ACAMID + "'"
                                                                                            + " WHERE [dbo].[tblACAMRequestSusp].Id =" + strSwapTo_Id;


                                                            cmdSqlSwapFromPallet.ExecuteNonQuery();
                                                            cmdSqlSwapToPallet.ExecuteNonQuery();
                                                            cnn.Close();

                                                            nCOMMACK = 0;
                                                        }

                                                        #endregion check pallet to swap in tblACAMRequestSusp

                                                    }
                                                    else
                                                    {
                                                        nCOMMACK = 1;
                                                    }

                                                }
                                                #endregion foreach

                                            }

                                        }
                                        #endregion

                                    }

                                    catch (Exception ex)
                                    {
                                        LoggerClass.Instance.ErrorLogInfo("!_hostConfig.RunWithNoDatabase: " + ex.Message);
                                        nCOMMACK = 1;
                                    }


                                    // scndMsgReqSwapPalletAck
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    SCIMessage scndMsgReqSwapPalletAck = new SCIMessage();
                                    scndMsgReqSwapPalletAck.CommandID = "RequestSwapPalletAck";
                                    scndMsgReqSwapPalletAck.Item = new SCIItem();
                                    scndMsgReqSwapPalletAck.Item.Format = SCIFormat.List;
                                    scndMsgReqSwapPalletAck.Item.Items = new SCIItemCollection();

                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = nCOMMACK });
                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedFrom_PalletID", Value = reqSwap.PalletID });
                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedFrom_ACAMID", Value = strSwapFrom_ACAMID });
                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedFrom_TransID", Value = strSwapFrom_TransID });

                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedTo_PalletID", Value = strSwapTo_PalletID });
                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedTo_ACAMID", Value = strSwapTo_ACAMID });
                                    scndMsgReqSwapPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SwappedTo_TransID", Value = strSwapTo_TransID });

                                    WDConnect.Common.SCITransaction toReplyRequestSwapPalletAck = new WDConnect.Common.SCITransaction()
                                    {
                                        DeviceId = e.Transaction.DeviceId,
                                        MessageType = MessageType.Secondary,
                                        Id = e.Transaction.Id,
                                        Name = "RequestSwapPalletAck",
                                        NeedReply = false,
                                        Primary = e.Transaction.Primary,
                                        Secondary = scndMsgReqSwapPalletAck
                                    };

                                    LoggerClass.Instance.MessageLogInfo("Reply: " + toReplyRequestSwapPalletAck.XMLText);

                                    string strToReply = string.Format("{0},{1},{2},{3},{4}", reqSwap.PalletID, 
                                                                                            reqSwap.ACAMID, 
                                                                                            strSwapTo_PalletID, 
                                                                                            strSwapTo_ACAMID, 
                                                                                            nCOMMACK.ToString());
                                    LoggerClass.Instance.MainLogInfo("Host >> RequestSwapPalletAck: " + strToReply);

                                    host.ReplyOutStream(toReplyRequestSwapPalletAck);
                                }

                                break;

                            #endregion 


                            default:
                                break;

                        }

                    }

                }

            }

            System.Threading.Thread.Sleep(500);
        }

        private void checkACAMRequestSusp(ref hostController h)
        {
            while (true)
            {
                if (h.ConnectionStatus != ConnectionStatus.Connected)
                {
                    Console.WriteLine("not connected");
                    return;
                }


                System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                string strSQLCmd = string.Empty;
                try
                {
                    cnn.Open();
                }
                catch (Exception ex)
                {
                    LoggerClass.Instance.ErrorLogInfo(ex.Message);
                }

                if (cnn.State == ConnectionState.Open)
                {
                    string strId = string.Empty;
                    string strACAMID = string.Empty;
                    string strSuspAmt = string.Empty;

                    string sqlcommand = "";
                    sqlcommand += "SELECT TOP(1) *"
                                + " FROM [dbo].[tblACAMRequestSusp] tblPallet"
                                + " WHERE tblPallet.PalletSN is null"
                                + " ORDER BY tblPallet.Id ASC";


                    System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                    System.Data.DataSet dataset = new System.Data.DataSet();
                    adpt.Fill(dataset);
                    cnn.Close();


                    System.Data.DataTableCollection tables = dataset.Tables;
                    System.Data.DataTable tbReqSusp = tables[0];

                    if (tbReqSusp.Rows.Count < 1)
                    {
                        //no request from ACAM yet
                    }
                    else
                    {

                        foreach (System.Data.DataRow row in tbReqSusp.Rows)
                        {
                            //row[0]    Id
                            strId = row[0].ToString();

                            //row[1]    ACAMID
                            strACAMID = row[1].ToString();

                            //row[2]    SuspAmt
                            strSuspAmt = row[2].ToString();

                            //row[3]    PalletSN
                            //row[4]    TransID

                            //row[5]    dbo_pallet_Id
                            //row[6]    CreatedDateTime
                            //row[7]    IsProcessed
                            //row[8]    UpdatedDateTime
                        }


                        hostController host = new hostController();
                        host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];


                        //********************************************************
                        SCIMessage primMsg = new SCIMessage();
                        primMsg.CommandID = "RequestSuspension";
                        primMsg.Item = new SCIItem();
                        primMsg.Item.Format = SCIFormat.List;
                        primMsg.Item.Items = new SCIItemCollection();

                        primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = strACAMID });
                        primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = Int32.Parse(strSuspAmt) });

                        WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
                        {
                            DeviceId = 1,
                            MessageType = MessageType.Primary,
                            Id = 1,
                            Name = "RequestSuspension",
                            //NeedReply = true,
                            NeedReply = false,
                            Primary = primMsg,
                            Secondary = null
                        };


                        if (host == null)
                        {
                            LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                            return;
                        }

                        try
                        {
                            LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                            host.SendMessage(trans);
                            DelegateSetErrorTextBoxMsg("");
                        }
                        catch (Exception ex)
                        {
                            DelegateSetErrorTextBoxMsg(ex.Message + "," + ex.StackTrace);
                            LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                        }

                    }
                }


                Application.DoEvents();
                Thread.Sleep(30000); //30 seconds
            }
        }
              


        private string GetNextEquipmentType(string EquipmentType)
        {
            //public enum EQUIPMENT_TYPE
            //{
            //    //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
            //    //10    , 80    , 30    , 40    , 50    , 60
            //    NA      = 0,
            //    ASLV    = 10,
            //    ACAM    = 20,
            //    ILC     = 30,
            //    SJB     = 40,
            //    AVI     = 50,
            //    UNOCR   = 60,
            //    FVMI    = 70,
            //    APT     = 80,
            //}

            string strNextEquipmentType = "0";
            switch (Int32.Parse(EquipmentType))
            {
                case 10:                            //ASLV
                    strNextEquipmentType = "80";    //to APT
                    break;

                case 20:                            //ACAM
                    strNextEquipmentType = "30";    //to ILC
                    break;

                case 30:                            //ILC
                    strNextEquipmentType = "40";    //to SJB
                    break;

                case 40:                            //SJB
                    strNextEquipmentType = "50";    //to AVI
                    break;

                case 50:                            //AVI
                    strNextEquipmentType = "60";    //to UNOCR
                    break;

                case 60:                            //UNOCR
                    strNextEquipmentType = "10";    //to ASLV
                    break;

                case 70:
                    break;

                case 80:                            //APT
                    strNextEquipmentType = "30";    //to ILC
                    break;

                case 0:
                default:
                    break;
            }

            return strNextEquipmentType;
        }

        private string GenSerialNumberFromLot(string strLotNumber)
        {
            System.Threading.Thread.Sleep(50);
            rnd = new Random(Environment.TickCount);
            string strSimulatedSerialNumber = strLotNumber.Substring(0, 4);

            for (int i = 0; i < 4; i++)
            {
                strSimulatedSerialNumber += ((char)rnd.Next(65, 91)).ToString();
            }

            Console.WriteLine(strSimulatedSerialNumber);
            return strSimulatedSerialNumber;
        }


        private delegate void delegateSetPrimaryTextBoxMsg(string strMsg);
        private void SetPrimaryTextBoxMsg(string strMsg)
        {
            PrimaryMessageTextBox.Text = strMsg;
        }
        private void DelegateSetPrimaryTextBoxMsg(string strMsg)
        {
            Invoke(new delegateSetPrimaryTextBoxMsg(SetPrimaryTextBoxMsg), strMsg);
        }


        private string CreateXMLText(Type ObjectType, Object obj)
        {
            LoggerClass.Instance.MessageLogInfo("CreateXMLText");
            string InnerXml = string.Empty;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer ser = new XmlSerializer(ObjectType);

                using (MemoryStream xmlStream = new MemoryStream())
                {
                    ser.Serialize(xmlStream, obj);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    InnerXml = xmlDoc.InnerXml;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
            return InnerXml;
        }

        private void hostController_SECsSecondaryIn(object sender, SECsSecondaryInEventArgs e)
        {
            DelegateSetErrorTextBoxMsg(""); //clear error message

            if (e.Transaction.Secondary == null)
            {
                LoggerClass.Instance.MessageLogInfo("hostController_SECsSecondaryIn: null guard");
                return;
            }

            LoggerClass.Instance.MessageLogInfo("hostController_SECsSecondaryIn: " + e.Transaction.XMLText);
            DelegateSetSecondaryTextBoxMsg(e.Transaction.XMLText);

            hostController host = GetHost(e.remoteIPAddress, e.remotePortNumber);
            for (int i = 0; i < MasterDataGridView.Rows.Count; i++)
            {
                if (e.remoteIPAddress == MasterDataGridView.Rows[i].Cells[3].Value.ToString() && e.remotePortNumber == int.Parse(MasterDataGridView.Rows[i].Cells[4].Value.ToString()))
                {

                    switch (e.Transaction.Secondary.CommandID)
                    {
                        case "Connected":
                            break;

                        case "SendStatus":
                            {
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsg = new SCIMessage();
                                scndMsg.CommandID = "SendStatusAck";
                                scndMsg.Item = new SCIItem();
                                scndMsg.Item.Format = SCIFormat.List;
                                scndMsg.Item.Items = new SCIItemCollection();
                                scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                WDConnect.Common.SCITransaction toReply = new WDConnect.Common.SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "SendStatusAck",
                                    NeedReply = false,
                                    Primary = null,
                                    Secondary = scndMsg
                                };

                                //*****************
                                host.ReplyOutStream(toReply);
                            }
                            
                            break;

                        case "SendPalletEnableAck":
                            break;
                            //simulate replying with SendPalletInfo
                            host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                            PalletToTrayData reqPallet = new PalletToTrayData();
                            //reqPallet = GetPalletFromXML(e.Transaction.XMLText);
                            reqPallet = GetPalletFromXML_secondary(e.Transaction.XMLText);

                            SCIMessage primMsgSendPalletInfo = new SCIMessage();
                            primMsgSendPalletInfo.CommandID = "SendPalletInfo";
                            primMsgSendPalletInfo.Item = new SCIItem();
                            primMsgSendPalletInfo.Item.Format = SCIFormat.List;
                            primMsgSendPalletInfo.Item.Items = new SCIItemCollection();
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPallet.PalletID });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumber", Value = "69885-15-SAA" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "WKU8A_A" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STRNumber", Value = "69885-15-SAA" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B4A1" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SliderAttachMachine", Value = "ACAM001" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = "Lot001" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAType", Value = "A" });
                            primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "AllowMix", Value = false });


                            WDConnect.Common.SCITransaction toReplySendPalletInfo = new WDConnect.Common.SCITransaction()
                            {
                                DeviceId = 1,
                                MessageType = MessageType.Primary,
                                Id = 1,
                                Name = "SendPalletInfo",
                                NeedReply = true,
                                Primary = primMsgSendPalletInfo,
                                Secondary = null
                            };

                            LoggerClass.Instance.MessageLogInfo("Reply: " + toReplySendPalletInfo.XMLText);
                            host.ReplyOutStream(toReplySendPalletInfo);

                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private delegate void delegateSetSecondaryTextBoxMsg(string strMsg);
        private void SetSecondaryTextBoxMsg(string strMsg)
        {
            SecondaryMessageTextBox.Text = strMsg;
        }
        private void DelegateSetSecondaryTextBoxMsg(string strMsg)
        {
            Invoke(new delegateSetSecondaryTextBoxMsg(SetSecondaryTextBoxMsg), strMsg);
        }


        private PalletToTrayData GetPalletFromXML(string xmlPallet)
        {
            PalletToTrayData palletObj = new PalletToTrayData();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;
            XmlNodeList palletIDNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (palletIDNodeList.Count > 0)
            {
                foreach (XmlNode node in palletIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.PalletID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.PartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGALotNo")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.HGALotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "STRNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.STRNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "SliderAttachMachine")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.SliderAttachMachine = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGAType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.HGAType = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "AllowMixed")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.AllowMixed = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnableCode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //0 = enabled
                                //1 = disabled
                                palletObj.PalletEnabled = !(Convert.ToInt32(valueChildNode.InnerText, 10) > 0);
                                LoggerClass.Instance.MessageLogInfo("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.PalletEnabled.ToString());
                            }
                        }
                    }

                }
            }

            return palletObj;
        }


        private PalletToTrayData GetPalletFromXML_primary(string xmlPallet)
        {
            PalletToTrayData palletToTrayObj = new PalletToTrayData();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;
            XmlNodeList palletIDNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (palletIDNodeList.Count > 0)
            {
                foreach (XmlNode node in palletIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.PalletID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EquipmentID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.EquipmentID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EquipmentType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.EquipmentType = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.PartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGALotNo")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.HGALotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "STRNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.STRNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "SliderAttachMachine")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.SliderAttachMachine = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGAType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.HGAType = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "AllowMixed")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.AllowMixed = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnableCode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //0 = enabled
                                //1 = disabled
                                palletToTrayObj.PalletEnabled = !(Convert.ToInt32(valueChildNode.InnerText, 10) > 0);
                                LoggerClass.Instance.MessageLogInfo("PalletID: " + palletToTrayObj.PalletID + ", Enabled: " + palletToTrayObj.PalletEnabled.ToString());
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "TrayID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.TrayID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "TraySize")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.TraySize = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "RowID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletToTrayObj.RowID = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGA")
                        {
                            XmlNodeList hgaItemsNodeList = node.SelectNodes("./Items/SCIItem");     //==HGA0, HGA1, HGA2, ...
                            foreach (XmlNode hgaChildNode in hgaItemsNodeList)
                            {
                                try
                                {
                                    int nIndex = Int32.Parse(hgaChildNode.SelectSingleNode("./Name").InnerText.Replace("HGA", ""));
                                    Console.WriteLine("nIndex: " + nIndex.ToString());

                                    foreach (XmlNode hgaItem in hgaChildNode.SelectNodes("./Items/SCIItem"))
                                    {
                                        if (hgaItem.SelectSingleNode("./Name").InnerText == "SN")
                                        {
                                            palletToTrayObj._hga[nIndex].SerialNum = hgaItem.SelectSingleNode("./Value").InnerText;
                                        }

                                        if (hgaItem.SelectSingleNode("./Name").InnerText == "Defect")
                                        {
                                            palletToTrayObj._hga[nIndex].AddDefects(hgaItem.SelectSingleNode("./Value").InnerText);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                                }

                                /*
                                switch (hgaChildNode.SelectSingleNode("./Name").InnerText)
                                {
                                    case "HGA0":
                                        foreach (XmlNode hgaItem in hgaChildNode.SelectNodes("./Items/SCIItem"))
                                        {
                                            if (hgaItem.SelectSingleNode("./Name").InnerText == "SN")
                                            {
                                                palletToTrayObj._hga[0].SerialNum = hgaItem.SelectSingleNode("./Value").InnerText;
                                            }

                                            if (hgaItem.SelectSingleNode("./Name").InnerText == "Defect")
                                            {
                                                palletToTrayObj._hga[0].AddDefects(hgaItem.SelectSingleNode("./Value").InnerText);
                                            }
                                        }
                                        break;

                                    case "HGA1":
                                        foreach (XmlNode hgaItem in hgaChildNode.SelectNodes("./Items/SCIItem"))
                                        {
                                            if (hgaItem.SelectSingleNode("./Name").InnerText == "SN")
                                            {
                                                palletToTrayObj._hga[1].SerialNum = hgaItem.SelectSingleNode("./Value").InnerText;
                                            }

                                            if (hgaItem.SelectSingleNode("./Name").InnerText == "Defect")
                                            {
                                                palletToTrayObj._hga[1].AddDefects(hgaItem.SelectSingleNode("./Value").InnerText);
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                                */
                            }
                        }
                    }

                }
            }

            return palletToTrayObj;
        }


        private PalletToTrayData GetPalletFromXML_secondary(string xmlPallet)
        {
            PalletToTrayData palletObj = new PalletToTrayData();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;
            XmlNodeList palletIDNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");
            if (palletIDNodeList.Count > 0)
            {
                foreach (XmlNode node in palletIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.PalletID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.PartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGALotNo")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.HGALotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "STRNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.STRNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "SliderAttachMachine")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.SliderAttachMachine = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGAType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.HGAType = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "AllowMixed")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.AllowMixed = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnableCode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //0 = enabled
                                //1 = disabled
                                palletObj.PalletEnabled = !(Convert.ToInt32(valueChildNode.InnerText, 10) > 0);
                                LoggerClass.Instance.MessageLogInfo("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.PalletEnabled.ToString());
                            }
                        }
                    }

                }
            }

            return palletObj;
        }


        private ProcessedPalletDataClass GetProcessedPalletFromXML_primary(string xmlPallet)
        {
            ProcessedPalletDataClass palletObj = new ProcessedPalletDataClass();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;
            XmlNodeList palletIDNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (palletIDNodeList.Count > 0)
            {
                foreach (XmlNode node in palletIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.PalletID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EquipmentID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.EquipmentID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.LotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "UVPower")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.UVPower = Convert.ToInt32(valueChildNode.InnerText , 10);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureTime")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.CureTime = Convert.ToDouble(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureZone")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.UVPower = Convert.ToInt32(valueChildNode.InnerText, 10);
                            }
                        }
                    }



                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "SJBLane")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.SJBLane = Convert.ToInt32(valueChildNode.InnerText , 10);
                            }
                        }
                    }

                }


                XmlNodeList itemSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
                //XmlNodeList hgalistSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem/Items/SCIItem");
                if (itemSCIItemsNodeList.Count > 0)  //nodes RequestSuspension and HGA nodes
                {
                    foreach (XmlNode node in itemSCIItemsNodeList)
                    {
                        XmlNodeList nameNodeList = node.SelectNodes("./Name");                                  //RequestSuspension, HGA
                        foreach (XmlNode childNode in nameNodeList)
                        {
                            if (childNode.InnerText == "RequestSuspension")
                            {
                                XmlNodeList reqSuspNodeList = node.SelectNodes("./Items/SCIItem/Name");         //ACAMID, SuspAmt
                                XmlNodeList reqSuspValueNodeList = node.SelectNodes("./Items/SCIItem/Value");   //APT001, 10
                                for (int i = 0; i < reqSuspNodeList.Count; i++)
                                {
                                    if (reqSuspNodeList[i].InnerText == "ACAMID")
                                    {
                                        //Console.WriteLine("ACAMID: " + reqSuspValueNodeList[i].InnerText);
                                        palletObj.ReqSusp.ACAMID = reqSuspValueNodeList[i].InnerText;
                                    }

                                    if (reqSuspNodeList[i].InnerText == "SuspAmt")
                                    {
                                        //Console.WriteLine("SuspAmt: " + reqSuspValueNodeList[i].InnerText);
                                        palletObj.ReqSusp.SuspAmt = Int32.Parse(reqSuspValueNodeList[i].InnerText);
                                    }

                                }
                            }

                            if (childNode.InnerText == "HGA")
                            {
                                XmlNodeList hgasNodeList = node.SelectNodes("./Items/SCIItem/Name");     //HGA1
                                foreach (XmlNode hgaChildNode in hgasNodeList)
                                {
                                    string strHGAPos = hgaChildNode.InnerText;
                                    strHGAPos = strHGAPos.Replace("HGA", "");
                                    int nHGAPos = Int32.Parse(strHGAPos);
                                    nHGAPos = nHGAPos - 1;


                                    XmlNodeList hgaNameNodeList = hgaChildNode.SelectNodes("../Items/SCIItem/Name");      //==SN
                                    XmlNodeList hgaValueNodeList = hgaChildNode.SelectNodes("../Items/SCIItem/Value");    //==SN
                                    for (int i = 0; i < hgaNameNodeList.Count; i++)
                                    {
                                        //if (hgaNameNodeList[i].InnerText == "SN")
                                        //{
                                        //    palletObj._arrHGA[nHGAPos]._strOCR = hgaValueNodeList[i].InnerText;
                                        //}
                                        //else if (hgaNameNodeList[i].InnerText == "Defect")
                                        //{
                                        //    palletObj._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                        //}

                                        switch (hgaNameNodeList[i].InnerText)
                                        {
                                            case "SN":
                                                palletObj._arrHGA[nHGAPos - 1]._strOCR = hgaValueNodeList[i].InnerText;
                                                break;

                                            case "Defect":
                                                if (hgaValueNodeList[i].InnerText.Length > 0)
                                                {
                                                    palletObj._arrHGA[nHGAPos - 1]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                                }
                                                break;

                                            case "xlocACAM":
                                                palletObj._arrHGA[nHGAPos - 1]._dblxlocACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            case "ylocACAM":
                                                palletObj._arrHGA[nHGAPos - 1]._dblylocACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            case "skwACAM":
                                                palletObj._arrHGA[nHGAPos - 1]._dblskwACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            case "xlocSAI":
                                                palletObj._arrHGA[nHGAPos - 1]._dblxlocSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            case "ylocSAI":
                                                palletObj._arrHGA[nHGAPos - 1]._dblylocSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            case "skwSAI":
                                                palletObj._arrHGA[nHGAPos - 1]._dblskwSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                                break;

                                            default:
                                                break;
                                        }

                                    }
                                }
                            }

                        }

                    }
                }


                /*
                XmlNodeList hgalistSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem/Items/SCIItem");
                if (hgalistSCIItemsNodeList.Count > 0)
                {
                    foreach (XmlNode node in hgalistSCIItemsNodeList)
                    {
                        XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==HGA0

                        foreach (XmlNode nameChildNode in nameNodeList)
                        {
                            string strHGAPos = nameChildNode.InnerText;
                            strHGAPos = strHGAPos.Replace("HGA", "");
                            int nHGAPos = Int32.Parse(strHGAPos);

                            XmlNodeList hgaNameNodeList = node.SelectNodes("./Items/SCIItem/Name");     //==SN
                            XmlNodeList hgaValueNodeList = node.SelectNodes("./Items/SCIItem/Value");   //==SN
                            for (int i = 0; i < hgaNameNodeList.Count; i++)
                            {
                                switch (hgaNameNodeList[i].InnerText)
                                {
                                    case "SN":
                                        palletObj._arrHGA[nHGAPos - 1]._strOCR = hgaValueNodeList[i].InnerText;
                                        break;

                                    case "Defect":
                                        if (hgaValueNodeList[i].InnerText.Length > 0)
                                        {
                                            palletObj._arrHGA[nHGAPos - 1]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                        }
                                        break;

                                    case "xlocACAM":
                                        palletObj._arrHGA[nHGAPos - 1]._dblxlocACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    case "ylocACAM":
                                        palletObj._arrHGA[nHGAPos - 1]._dblylocACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    case "skwACAM":
                                        palletObj._arrHGA[nHGAPos - 1]._dblskwACAM = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    case "xlocSAI":
                                        palletObj._arrHGA[nHGAPos - 1]._dblxlocSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    case "ylocSAI":
                                        palletObj._arrHGA[nHGAPos - 1]._dblylocSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    case "skwSAI":
                                        palletObj._arrHGA[nHGAPos - 1]._dblskwSAI = Convert.ToDouble(hgaValueNodeList[i].InnerText);
                                        break;

                                    default:
                                        break;
                                }

                            }

                        }


                    }

                }
                */

            }

            return palletObj;
        }


        private RequestProcessRecipeStruct GetRequestProcessRecipeFromXML_primary(string xmlReqProcessRecipe)
        {
            RequestProcessRecipeStruct reqProcRecipetObj = new RequestProcessRecipeStruct();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlReqProcessRecipe);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;
            XmlNodeList palletIDNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (palletIDNodeList.Count > 0)
            {
                foreach (XmlNode node in palletIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EquipmentID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                reqProcRecipetObj.EquipmentID = valueChildNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                reqProcRecipetObj.PalletID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                reqProcRecipetObj.LotNumber = valueChildNode.InnerText;
                            }
                        }
                    }



                }
            }

            return reqProcRecipetObj;
        }


        private void hostController_SECsHostError(object sender, SECsHostErrorEventArgs e)
        {
            LoggerClass.Instance.MainLogInfo("hostController_SECsHostError: " + e.Message);
            LoggerClass.Instance.MessageLogInfo("hostController_SECsHostError: " + e.Message);
            //ErrorMessageTextBox.Text = e.Message;
            DelegateSetErrorTextBoxMsg(e.Message);
        }


        private delegate void delegateSetErrorTextBoxMsg(string strMsg);
        private void SetErrorTextBoxMsg(string strMsg)
        {
            ErrorMessageTextBox.Text = strMsg;
        }
        private void DelegateSetErrorTextBoxMsg(string strMsg)
        {
            Invoke(new delegateSetErrorTextBoxMsg(SetErrorTextBoxMsg), strMsg);
        }
        

        private void MasterDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            LoggerClass.Instance.MessageLogInfo("MasterDataGridView_CellMouseClick");
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                PrimaryMessageTextBox.Text = MasterDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
                SecondaryMessageTextBox.Text = MasterDataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
        }


        private void AreYouThere_Click(object sender, EventArgs e)
        {
            /*  //obsolete as going to connect multiple tools to only 1 host; coding below returns the first connected tool, not the selected tool
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }
            */

            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            //Console.WriteLine(MasterDataGridView.CurrentCell.RowIndex.ToString());
            //Console.WriteLine(this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex].ConnectionStatus.ToString());      //selected tool on DataGridView
            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];

            


            //hostController host = this.GetHost("UNOCR46.tbpd.wdasia.com", 5001);
            //hostController host = this.GetHost("localhost", 5001);
            //hostController host = this.GetHost("WDBOA0185", 5001);

            //********************************************************
            //simulate AreYouThere from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "AreYouThere";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SoftwareRevision", Value = "x.x.x" });

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "AreYouThere",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);
                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message + "," + ex.StackTrace);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }
        }


        private void btnReqPalletASLV_Click(object sender, EventArgs e)
        {
            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
            }


            string strId = string.Empty;
            string strACAMID = string.Empty;
            string strSuspAmt = string.Empty;            
            if (cnn.State == ConnectionState.Open)
            {

                string sqlcommand = "";
                sqlcommand += "SELECT TOP(1) *"
                            + " FROM [dbo].[tblACAMRequestSusp] tblPallet"
                            + " WHERE tblPallet.PalletSN is null"
                            + " ORDER BY tblPallet.Id ASC";


                System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                System.Data.DataSet dataset = new System.Data.DataSet();
                adpt.Fill(dataset);
                cnn.Close();


                System.Data.DataTableCollection tables = dataset.Tables;
                System.Data.DataTable tbReqSusp = tables[0];

                if (tbReqSusp.Rows.Count < 1)
                {
                    //no request from ACAM yet
                    return;
                }
                else
                {

                    foreach (System.Data.DataRow row in tbReqSusp.Rows)
                    {
                        //row[0]    Id
                        strId = row[0].ToString();

                        //row[1]    ACAMID
                        strACAMID = row[1].ToString();

                        //row[2]    SuspAmt
                        strSuspAmt = row[2].ToString();

                        //row[3]    PalletSN
                        //row[4]    TransID

                        //row[5]    dbo_pallet_Id
                        //row[6]    CreatedDateTime
                        //row[7]    IsProcessed
                        //row[8]    UpdatedDateTime
                    }
                }
            }


            //********************************************************
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestSuspension";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = strACAMID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = Int32.Parse(strSuspAmt) });

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "RequestSuspension",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);
                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message + "," + ex.StackTrace);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }

        }

        private void RequestControlState_Click(object sender, EventArgs e)
        {
            /*  //obsolete as going to connect multiple tools to only 1 host; coding below returns the first connected tool, not the selected tool
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }
            */ 

            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView


            //hostController host = this.GetHost("UNOCR46.tbpd.wdasia.com", 5001);
            //hostController host = this.GetHost("localhost", 5001);
            //hostController host = this.GetHost("WDBOA0185", 5001);
            //hostController host = this.GetHost(host.localIPAddress, 5001);

            //********************************************************
            //simulate RequestEquipmentStatus from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestControlState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ControlState", Value = 0 });

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "RequestControlState",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }
        }


        private void RequestProcessState_Click(object sender, EventArgs e)
        {
            /*  //obsolete as going to connect multiple tools to only 1 host; coding below returns the first connected tool, not the selected tool
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }
            */

            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView


            //hostController host = this.GetHost("UNOCR46.tbpd.wdasia.com", 5001);
            //hostController host = this.GetHost("localhost", 5001);
            //hostController host = this.GetHost("WDBOA0185", 5001);
            //hostController host = this.GetHost(host.localIPAddress, 5001);

            //********************************************************
            //simulate RequestEquipmentStatus from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestProcessState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ProcessState", Value = 0 });

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "RequestProcessState",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }

        }
       

        private void SendAlarm_Click(object sender, EventArgs e)
        {
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }

            //hostController host = this.GetHost("UNOCR46.tbpd.wdasia.com", 5001);
            //hostController host = this.GetHost("localhost", 5001);
            //hostController host = this.GetHost("WDBOA0185", 5001);

            //********************************************************
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "AlarmReportSend";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALID", Value = 1002 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "AlarmText", Value = "Incorrect Offset" });

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "AlarmReportSend",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }
        }


        private void OnlineRequest_Click(object sender, EventArgs e)
        {
            /*  //obsolete as going to connect multiple tools to only 1 host; coding below returns the first connected tool, not the selected tool            
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }
            */

            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView

            //********************************************************
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "OnlineRequest";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "OnlineRequest",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }

        }


        private void OfflineRequest_Click(object sender, EventArgs e)
        {
            /*  //obsolete as going to connect multiple tools to only 1 host; coding below returns the first connected tool, not the selected tool
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }
            */

            //null guard
            if (MasterDataGridView.CurrentCell == null)
            {
                return;
            }

            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView

            //********************************************************
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "OfflineRequest";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            WDConnect.Common.SCITransaction trans = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 1,
                MessageType = MessageType.Primary,
                Id = 1,
                Name = "OfflineRequest",
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            if (host == null)
            {
                LoggerClass.Instance.MessageLogInfo("null guard: host == null");
                return;
            }

            try
            {
                LoggerClass.Instance.MessageLogInfo("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
            }

        }


        private string exePath = System.Windows.Forms.Application.StartupPath;
        private const string DATETIME_FORMAT = "M/d/yyyy h:mm:ss tt";

        string _strOperations = string.Empty;
        string _strConnectionString = string.Empty;

        private void _readPalletData()
        {
            string[] palletFiles = System.IO.Directory.GetFiles(exePath, "*.XML");
            foreach (string file in palletFiles)
            {

                ListViewItem lstItem = new ListViewItem();
                lstItem.Text = file.Replace(exePath, "");
                lstItem.Text = lstItem.Text.Replace(@"\", "");
                lstItem.SubItems.Add(DateTime.Now.ToString("M/d/yyyy h:mm:ss tt"));
            }

        }


        // mitecs /////////////////////////////////////////////////////////////////////////////////////////////////

        private ServiceConnModule mitecs3service;
        private string _init()
        {
            try
            {
                mitecs3service = new ServiceConnModule();

                //http://wdtbm32pws02.tb.asia.wdc.com/Mitecs3WebServices/service.asmx
                ARB_Host.MITECSWebService.ServiceSoapClient client = new ServiceSoapClient();

                string strReturn = client.getWebServiceVersion();
                LoggerClass.Instance.MainLogInfo("_init: " + strReturn);

                return strReturn;
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return ex.Message;
            }
        }



        private void btnRegisterLine_Click(object sender, EventArgs e)
        {
            if (txtboxLine.Text.Length < 1)
            {
                return;
            }

            _hostConfig.LineNo = txtboxLine.Text;

            string msg = String.Empty;
            bool m3status = mitecs3service.GetMitecsStatus(out msg);
            if (mitecs3service.RegisterLine(txtboxLine.Text, out msg))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(".\\config.xml");

                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList apikeyList = root.SelectNodes("/Configuration/Line/APIKey");
                if (apikeyList.Count > 0)
                {
                    foreach (XmlNode node in apikeyList)
                    {
                        txtboxAPIKey.Text = node.InnerText;
                    }
                }

                LoggerClass.Instance.MainLogInfo("RegisterLine: " + txtboxLine.Text + "," + txtboxAPIKey.Text  + "," + msg);
            }
            else
            {
                txtboxAPIKey.Text = msg;
                LoggerClass.Instance.MainLogInfo("RegisterLine: " + txtboxLine.Text + "," + txtboxAPIKey.Text);
            }
        }


        private void btnGetLots_Click(object sender, EventArgs e)
        {
            this.btnRegisterLine_Click(sender, e);
            this.btnClearRecipe_Click(sender, e);

            string msg = string.Empty;
            DataSet dsLotNumbers = null;
            try
            {
                if (txtboxHGAPartNumber.Text.Length > 0)
                {
                    dsLotNumbers = mitecs3service.GetLotNumbersByLinePartNo(txtboxLine.Text, txtboxHGAPartNumber.Text, out msg);
                }
                else
                {
                    dsLotNumbers = mitecs3service.GetLotNumbers(txtboxLine.Text, out msg);
                }
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
            }
            finally
            {
                if (dsLotNumbers == null)
                {
                    dsLotNumbers = new DataSet();
                }
            }

            lstviewLotDetails.Items.Clear();

            //1* //Already get lots from MITECS
            DataTable dtable = dsLotNumbers.Tables[0];
            //save datatable to file
            dtable.WriteXml("mitecs_lot.xml", XmlWriteMode.WriteSchema);

            //2* //Create recipe based on Part Number and STR;
            mappingPartNumberSTR_ReqRecipeObj.Clear();
            mappingLot_LotInfo.Clear();


            // Display items in the ListView control
            for (int i = 0; i < dtable.Rows.Count; i++)
            {
                DataRow drow = dtable.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    ListViewItem lstvItem = new ListViewItem();
                    lstvItem.Text = drow["LOT_NUMBER"].ToString();
                    lstvItem.SubItems.Add(drow["HGA_PART_NUMBER"].ToString());
                    lstvItem.SubItems.Add(drow["PROGRAM"].ToString());
                    lstvItem.SubItems.Add(drow["SUSPENSION"].ToString());
                    lstvItem.SubItems.Add(drow["SUSP_PART_NUMBER"].ToString());
                    lstvItem.SubItems.Add(drow["TYPE"].ToString());
                    lstvItem.SubItems.Add(drow["STR_NO"].ToString());
                    lstvItem.SubItems.Add(drow["LINE_NO"].ToString());
                    lstvItem.SubItems.Add(drow["QTY"].ToString());
                    lstvItem.SubItems.Add(drow["UPDATED_TIMESTAMP"].ToString());

                    // Add the list items to the ListView
                    lstviewLotDetails.Items.Add(lstvItem);

                    lstviewLotDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    lstviewLotDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    LotInformation lotinfo = new LotInformation();
                    RequestProcessRecipeAckObj recipe = new RequestProcessRecipeAckObj();
                    recipe.RecipeID = (mappingPartNumberSTR_ReqRecipeObj.Count + 1).ToString();

                    lotinfo.LotNumber       = recipe.LotNumber      = drow["LOT_NUMBER"].ToString();
                    lotinfo.PartNumber      = recipe.PartNumber     = drow["HGA_PART_NUMBER"].ToString();
                    lotinfo.Program         = recipe.ProductName    = drow["PROGRAM"].ToString();
                    lotinfo.Suspension      = recipe.Suspension     = drow["SUSPENSION"].ToString();
                    lotinfo.SuspPartNumber  = recipe.SuspPartNumber = drow["SUSP_PART_NUMBER"].ToString();
                    lotinfo.HGAType         = recipe.HGAType        = drow["TYPE"].ToString();
                    lotinfo.STR             = recipe.STR            = drow["STR_NO"].ToString();
                    lotinfo.Line            = recipe.Line           = drow["LINE_NO"].ToString();
                    lotinfo.Qty             = recipe.LotQty         = Int32.Parse(drow["QTY"].ToString());
                    lotinfo.UpdateTimeStamp = DateTime.Parse(drow["UPDATED_TIMESTAMP"].ToString());

                    #region local ilc recipe
                    if (File.Exists(exePath + @"\Recipe\ILCRecipeObj.xml"))
                    {
                        ILCRecipeObj ilcRecipe1 = ILCRecipeObj.ReadFromFile(exePath + @"\Recipe\ILCRecipeObj.xml");

                        recipe.PowerUV1 = ilcRecipe1.PowerUV1;
                        recipe.CureTimeUV1 = ilcRecipe1.CureTimeUV1;
                        recipe.EnabledUV1 = ilcRecipe1.EnabledUV1;

                        recipe.FlowRateHeater1 = ilcRecipe1.FlowRateHeater1;
                        recipe.TempHeater1 = ilcRecipe1.TempHeater1;
                        recipe.EnabledHeater1 = ilcRecipe1.EnabledHeater1;
                        recipe.EnabledN2Heater1 = ilcRecipe1.EnabledN2Heater1;

                        recipe.FlowRateHeater2 = ilcRecipe1.FlowRateHeater2;
                        recipe.TempHeater2 = ilcRecipe1.TempHeater2;
                        recipe.EnabledHeater2 = ilcRecipe1.EnabledHeater2;
                        recipe.EnabledN2Heater2 = ilcRecipe1.EnabledN2Heater2;

                        recipe.FlowRateHeater3 = ilcRecipe1.FlowRateHeater3;
                        recipe.TempHeater3 = ilcRecipe1.TempHeater3;
                        recipe.EnabledHeater3 = ilcRecipe1.EnabledHeater3;
                        recipe.EnabledN2Heater3 = ilcRecipe1.EnabledN2Heater3;

                        recipe.FlowRateHeater4 = ilcRecipe1.FlowRateHeater4;
                        recipe.TempHeater4 = ilcRecipe1.TempHeater4;
                        recipe.EnabledHeater4 = ilcRecipe1.EnabledHeater4;
                        recipe.EnabledN2Heater4 = ilcRecipe1.EnabledN2Heater4;

                        recipe.FlowRateHeater5 = ilcRecipe1.FlowRateHeater5;
                        recipe.TempHeater5 = ilcRecipe1.TempHeater5;
                        recipe.EnabledHeater5 = ilcRecipe1.EnabledHeater5;
                        recipe.EnabledN2Heater5 = ilcRecipe1.EnabledN2Heater5;

                        recipe.PowerUV2 = ilcRecipe1.PowerUV2;
                        recipe.CureTimeUV2 = ilcRecipe1.CureTimeUV2;
                        recipe.EnabledUV2 = ilcRecipe1.EnabledUV2;

                        recipe.FlowRateHeater6 = ilcRecipe1.FlowRateHeater6;
                        recipe.TempHeater6 = ilcRecipe1.TempHeater6;
                        recipe.EnabledHeater6 = ilcRecipe1.EnabledHeater6;
                        recipe.EnabledN2Heater6 = ilcRecipe1.EnabledN2Heater6;

                        recipe.FlowRateHeater7 = ilcRecipe1.FlowRateHeater7;
                        recipe.TempHeater7 = ilcRecipe1.TempHeater7;
                        recipe.EnabledHeater7 = ilcRecipe1.EnabledHeater7;
                        recipe.EnabledN2Heater7 = ilcRecipe1.EnabledN2Heater7;

                        recipe.FlowRateHeater8 = ilcRecipe1.FlowRateHeater8;
                        recipe.TempHeater8 = ilcRecipe1.TempHeater8;
                        recipe.EnabledHeater8 = ilcRecipe1.EnabledHeater8;
                        recipe.EnabledN2Heater8 = ilcRecipe1.EnabledN2Heater8;

                        recipe.FlowRateHeater9 = ilcRecipe1.FlowRateHeater9;
                        recipe.TempHeater9 = ilcRecipe1.TempHeater9;
                        recipe.EnabledHeater9 = ilcRecipe1.EnabledHeater9;
                        recipe.EnabledN2Heater9 = ilcRecipe1.EnabledN2Heater9;

                        recipe.FlowRateHeater10 = ilcRecipe1.FlowRateHeater10;
                        recipe.TempHeater10 = ilcRecipe1.TempHeater10;
                        recipe.EnabledHeater10 = ilcRecipe1.EnabledHeater10;
                        recipe.EnabledN2Heater10 = ilcRecipe1.EnabledN2Heater10;

                        recipe.Mode = ilcRecipe1.Mode;
                        recipe.Bypass = ilcRecipe1.Bypass;
                    }
                    #endregion


                    try
                    {
                        Tuple<string, string> tPartNumberSTR = new Tuple<string, string>(lotinfo.PartNumber, drow["STR_NO"].ToString());
                        mappingPartNumberSTR_ReqRecipeObj.Add(tPartNumberSTR, recipe);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("mappingPartNumberSTR_ReqRecipeObj: " + ex.Message);
                        LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        mappingLot_LotInfo.Add(recipe.LotNumber, lotinfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("mappingLot_LotInfo: " + ex.Message);
                        LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                    }
                }
            }


            //Generate recipe information
            //foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingLotNumberReqRecipeObj)
            //foreach(KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            foreach (KeyValuePair<Tuple<string, string>, RequestProcessRecipeAckObj> entry in mappingPartNumberSTR_ReqRecipeObj)
            {
                //Console.WriteLine(entry.Key.ToString());
                //Console.WriteLine(((RequestProcessRecipeAckObj)entry.Value).RecipeID);

                //Item1=Program; Item2=STR
                Console.WriteLine(entry.Key.Item1);
                Console.WriteLine(entry.Key.Item2);
                Console.WriteLine(entry.Value.RecipeID);


                ListViewItem lstvItem = new ListViewItem();
                RequestProcessRecipeAckObj recipe = (RequestProcessRecipeAckObj)entry.Value;
                lstvItem.Text = recipe.RecipeID;
                //lstvItem.SubItems.Add(recipe.LotNumber);
                lstvItem.SubItems.Add(recipe.PartNumber);
                lstvItem.SubItems.Add(recipe.ProductName);
                lstvItem.SubItems.Add(recipe.STR);
                lstvItem.SubItems.Add(recipe.Suspension);
                lstvItem.SubItems.Add(recipe.SuspPartNumber);
                lstvItem.SubItems.Add(recipe.HGAType);
                lstvItem.SubItems.Add(recipe.Line);
                lstvItem.SubItems.Add(recipe.PalletID);

                lstviewRecipe.Items.Add(lstvItem);

                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

        }

        private void lstviewLotDetails_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine what the last sort order was and change it.
            if (lstviewLotDetails.Sorting == System.Windows.Forms.SortOrder.Ascending)
            {
                lstviewLotDetails.Sorting = System.Windows.Forms.SortOrder.Descending;
            }
            else
            {
                lstviewLotDetails.Sorting = System.Windows.Forms.SortOrder.Ascending;
            }

            this.lstviewLotDetails.ListViewItemSorter = new ListViewItemComparer(e.Column, lstviewLotDetails.Sorting);
            lstviewLotDetails.Sort();
        }

        private void btnGenRecipe_Click(object sender, EventArgs e)
        {
            this.btnClearRecipe_Click(sender, e);            
            
            /*
            //Generate recipe information
            foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingLotNumberReqRecipeObj)
            {
                Console.WriteLine(entry.Key.ToString());
                Console.WriteLine(((RequestProcessRecipeAckObj)entry.Value).RecipeID);

                ListViewItem lstvItem = new ListViewItem();
                RequestProcessRecipeAckObj recipe = (RequestProcessRecipeAckObj)entry.Value;
                lstvItem.Text = recipe.RecipeID;
                lstvItem.SubItems.Add(recipe.LotNumber);
                lstvItem.SubItems.Add(recipe.PartNumber);
                lstvItem.SubItems.Add(recipe.ProductName);
                lstvItem.SubItems.Add(recipe.STR);
                lstvItem.SubItems.Add(recipe.Suspension);
                lstvItem.SubItems.Add(recipe.SuspPartNumber); 
                lstvItem.SubItems.Add(recipe.HGAType);
                lstvItem.SubItems.Add(recipe.Line);
                lstvItem.SubItems.Add(recipe.PalletID);

                lstviewRecipe.Items.Add(lstvItem);

                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            */


            /*
            ///
            //Generate recipe information
            foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            {
                Console.WriteLine(entry.Key.ToString());
                Console.WriteLine(((RequestProcessRecipeAckObj)entry.Value).RecipeID);

                ListViewItem lstvItem = new ListViewItem();
                RequestProcessRecipeAckObj recipe = (RequestProcessRecipeAckObj)entry.Value;
                lstvItem.Text = recipe.RecipeID;
                lstvItem.SubItems.Add(recipe.PartNumber);
                lstvItem.SubItems.Add(recipe.ProductName);
                lstvItem.SubItems.Add(recipe.STR);
                lstvItem.SubItems.Add(recipe.Suspension);
                lstvItem.SubItems.Add(recipe.SuspPartNumber);
                lstvItem.SubItems.Add(recipe.HGAType);
                lstvItem.SubItems.Add(recipe.Line);
                lstvItem.SubItems.Add(recipe.PalletID);

                lstviewRecipe.Items.Add(lstvItem);

                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                //save recipe to local
                string strRecipePath = exePath + @"\recipe";
                if (!System.IO.Directory.Exists(strRecipePath))
                {
                    System.IO.Directory.CreateDirectory(strRecipePath);
                }
                System.IO.File.WriteAllText(strRecipePath + @"\recipe" +  recipe.RecipeID + @".xml", recipe.ToXML());
            }
            */



            //Generate recipe information
            foreach (KeyValuePair<Tuple<string, string>, RequestProcessRecipeAckObj> entry in mappingPartNumberSTR_ReqRecipeObj)
            {
                //Item1=Program; Item2=STR
                Console.WriteLine(entry.Key.Item1);
                Console.WriteLine(entry.Key.Item2);
                Console.WriteLine(entry.Value.RecipeID);


                ListViewItem lstvItem = new ListViewItem();
                RequestProcessRecipeAckObj recipe = (RequestProcessRecipeAckObj)entry.Value;
                lstvItem.Text = recipe.RecipeID;
                lstvItem.SubItems.Add(recipe.PartNumber);
                lstvItem.SubItems.Add(recipe.ProductName);
                lstvItem.SubItems.Add(recipe.STR);
                lstvItem.SubItems.Add(recipe.Suspension);
                lstvItem.SubItems.Add(recipe.SuspPartNumber);
                lstvItem.SubItems.Add(recipe.HGAType);
                lstvItem.SubItems.Add(recipe.Line);
                lstvItem.SubItems.Add(recipe.PalletID);

                lstviewRecipe.Items.Add(lstvItem);

                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


                //save recipe to local
                string strRecipePath = exePath + @"\recipe";
                if (!System.IO.Directory.Exists(strRecipePath))
                {
                    System.IO.Directory.CreateDirectory(strRecipePath);
                }
                System.IO.File.WriteAllText(strRecipePath + @"\recipe" + recipe.RecipeID + @".xml", recipe.ToXML());                
            }
        }

        private void btnClearRecipe_Click(object sender, EventArgs e)
        {
            lstviewRecipe.Items.Clear();
        }


        // reqPalletInfo //////////////////////////////////////////////////////////////////////////////////////////
        private void btn_tab3_LoadLocal_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if(txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }

            lstReqPalletLookup.Clear();
            lstReqPalletLookup = GetLocalReqPalletInfoLookup();

            RequestPalletInfoAckObj reqPalletObj = new RequestPalletInfoAckObj();
            if (!mappingLocalPalletIDReqPalletInfoObj.TryGetValue(txtbox_tab3_PalletID.Text, out reqPalletObj))
            {
                clearUITextData();
                lbl_tab3_error.Text = txtbox_tab3_PalletID.Text + " not found in the system!";
                return;
            }

            txtbox_tab3_PartNumber.Text = reqPalletObj.PartNumber;
            txtbox_tab3_ProductName.Text = reqPalletObj.ProductName;
            txtbox_tab3_LotNumber.Text = reqPalletObj.LotNumber;
            txtbox_tab3_Line.Text = reqPalletObj.Line;
            txtbox_tab3_ACAMID.Text = reqPalletObj.ACAMID;

            int nACAM = combo_tab3_ACAMID.Items.IndexOf(reqPalletObj.ACAMID);
            combo_tab3_ACAMID.SelectedIndex = (nACAM < 0 ? 0 : nACAM);

            txtbox_tab3_Suspension.Text = reqPalletObj.Suspension;
            radioButton_DisabledPallet.Checked = !(radioButton_EnabledPallet.Checked = reqPalletObj.EnabledPallet);
            txtbox_tab3_HGAType.Text = reqPalletObj.HGAType;

            txtbox_tab3_SN1.Text = reqPalletObj.HGA.HGA1.SN;
            txtbox_tab3_Defect1.Text = reqPalletObj.HGA.HGA1.Defect;

            txtbox_tab3_SN2.Text = reqPalletObj.HGA.HGA2.SN;
            txtbox_tab3_Defect2.Text = reqPalletObj.HGA.HGA2.Defect;

            txtbox_tab3_SN3.Text = reqPalletObj.HGA.HGA3.SN;
            txtbox_tab3_Defect3.Text = reqPalletObj.HGA.HGA3.Defect;

            txtbox_tab3_SN4.Text = reqPalletObj.HGA.HGA4.SN;
            txtbox_tab3_Defect4.Text = reqPalletObj.HGA.HGA4.Defect;

            txtbox_tab3_SN5.Text = reqPalletObj.HGA.HGA5.SN;
            txtbox_tab3_Defect5.Text = reqPalletObj.HGA.HGA5.Defect;


            txtbox_tab3_SN6.Text = reqPalletObj.HGA.HGA6.SN;
            txtbox_tab3_Defect6.Text = reqPalletObj.HGA.HGA6.Defect;

            txtbox_tab3_SN7.Text = reqPalletObj.HGA.HGA7.SN;
            txtbox_tab3_Defect7.Text = reqPalletObj.HGA.HGA7.Defect;

            txtbox_tab3_SN8.Text = reqPalletObj.HGA.HGA8.SN;
            txtbox_tab3_Defect8.Text = reqPalletObj.HGA.HGA8.Defect;

            txtbox_tab3_SN9.Text = reqPalletObj.HGA.HGA9.SN;
            txtbox_tab3_Defect9.Text = reqPalletObj.HGA.HGA9.Defect;

            txtbox_tab3_SN10.Text = reqPalletObj.HGA.HGA10.SN;
            txtbox_tab3_Defect10.Text = reqPalletObj.HGA.HGA10.Defect;


            EQUIPMENT_TYPE enEquipmentType = (EQUIPMENT_TYPE)reqPalletObj.EquipmentType;
            EQUIPMENT_TYPE enNextEquipmentType = (EQUIPMENT_TYPE)reqPalletObj.NextEquipmentType;

            #region EquipmentType
            switch (enEquipmentType.ToString())
            {
                case "ASLV":
                    lstboxEquipmentType.SelectedIndex = 0;
                    break;

                case "ACAM":
                    lstboxEquipmentType.SelectedIndex = 1;
                    break;

                case "ILC":
                    lstboxEquipmentType.SelectedIndex = 2;
                    break;

                case "SJB":
                    lstboxEquipmentType.SelectedIndex = 3;
                    break;

                case "AVI":
                    lstboxEquipmentType.SelectedIndex = 4;
                    break;

                case "UNOCR":
                    lstboxEquipmentType.SelectedIndex = 5;
                    break;

                case "FVMI":
                    lstboxEquipmentType.SelectedIndex = 6;
                    break;

                case "APT":
                    lstboxEquipmentType.SelectedIndex = 7;
                    break;

                default:
                    break;
            };
            #endregion


            #region NextEquipmentType
            switch (enNextEquipmentType.ToString())
            {
                case "ASLV":
                    lstboxNextEquipmentType.SelectedIndex = 0;
                    break;

                case "ACAM":
                    lstboxNextEquipmentType.SelectedIndex = 1;
                    break;

                case "ILC":
                    lstboxNextEquipmentType.SelectedIndex = 2;
                    break;

                case "SJB":
                    lstboxNextEquipmentType.SelectedIndex = 3;
                    break;

                case "AVI":
                    lstboxNextEquipmentType.SelectedIndex = 4;
                    break;

                case "UNOCR":
                    lstboxNextEquipmentType.SelectedIndex = 5;
                    break;

                case "FVMI":
                    lstboxNextEquipmentType.SelectedIndex = 6;
                    break;

                case "APT":
                    lstboxNextEquipmentType.SelectedIndex = 7;
                    break;

                default:
                    break;
            };
            #endregion


            lbl_tab3_error.Text = "Executed successfully";
        }

        
        private void btn_tab3_SaveLocal_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if (txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }

            string strPalletInfoPath = exePath + @"\Pallet";
            RequestPalletInfoAckObj reqPallet = new RequestPalletInfoAckObj();

            reqPallet.PalletID = txtbox_tab3_PalletID.Text;
            reqPallet.PartNumber = txtbox_tab3_PartNumber.Text;
            reqPallet.ProductName = txtbox_tab3_ProductName.Text;
            reqPallet.LotNumber = txtbox_tab3_LotNumber.Text;
            reqPallet.Line = txtbox_tab3_Line.Text;
            reqPallet.ACAMID = txtbox_tab3_ACAMID.Text;
            reqPallet.Suspension = txtbox_tab3_Suspension.Text;
            reqPallet.EnabledPallet = radioButton_EnabledPallet.Checked;
            reqPallet.HGAType = txtbox_tab3_HGAType.Text;

            reqPallet.HGA.HGA1.SN = txtbox_tab3_SN1.Text;
            reqPallet.HGA.HGA1.Defect = txtbox_tab3_Defect1.Text;

            reqPallet.HGA.HGA2.SN = txtbox_tab3_SN2.Text;
            reqPallet.HGA.HGA2.Defect = txtbox_tab3_Defect2.Text;

            reqPallet.HGA.HGA3.SN = txtbox_tab3_SN3.Text;
            reqPallet.HGA.HGA3.Defect = txtbox_tab3_Defect3.Text;

            reqPallet.HGA.HGA4.SN = txtbox_tab3_SN4.Text;
            reqPallet.HGA.HGA4.Defect = txtbox_tab3_Defect4.Text;

            reqPallet.HGA.HGA5.SN = txtbox_tab3_SN5.Text;
            reqPallet.HGA.HGA5.Defect = txtbox_tab3_Defect5.Text;


            reqPallet.HGA.HGA6.SN = txtbox_tab3_SN6.Text;
            reqPallet.HGA.HGA6.Defect = txtbox_tab3_Defect6.Text;

            reqPallet.HGA.HGA7.SN = txtbox_tab3_SN7.Text;
            reqPallet.HGA.HGA7.Defect = txtbox_tab3_Defect7.Text;

            reqPallet.HGA.HGA8.SN = txtbox_tab3_SN8.Text;
            reqPallet.HGA.HGA8.Defect = txtbox_tab3_Defect8.Text;

            reqPallet.HGA.HGA9.SN = txtbox_tab3_SN9.Text;
            reqPallet.HGA.HGA9.Defect = txtbox_tab3_Defect9.Text;

            reqPallet.HGA.HGA10.SN = txtbox_tab3_SN10.Text;
            reqPallet.HGA.HGA10.Defect = txtbox_tab3_Defect10.Text;


            EQUIPMENT_TYPE  enEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), lstboxEquipmentType.SelectedItem.ToString());
            reqPallet.EquipmentType = (int)enEquipmentType;

            EQUIPMENT_TYPE enNextEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), lstboxNextEquipmentType.SelectedItem.ToString());
            reqPallet.NextEquipmentType = (int)enNextEquipmentType;


            System.IO.File.WriteAllText(strPalletInfoPath + @"\" + reqPallet.PalletID + @".xml", reqPallet.ToXML());

            lbl_tab3_error.Text = "Executed successfully";
        }


        private Dictionary<Tuple<string, string>, RequestProcessRecipeAckObj> mappingPartNumberSTR_ReqRecipeObj = new Dictionary<Tuple<string, string>, RequestProcessRecipeAckObj>();    //to generate recipe list, mapping
        private Dictionary<string, LotInformation> mappingLot_LotInfo = new Dictionary<string, LotInformation>();
        private void btnGetLotsOffline_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "XML|*.xml";
            if (fileDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            this.btnClearRecipe_Click(sender, e);
            lstviewLotDetails.Items.Clear();


            //1* //Already get lots from MITECS
            DataTable dtable = new DataTable("Table1");     //real scenario; this lot data table will contain informaiton from MITECS
            
            //dtable.ReadXml("mitecs_lot.xml");       //offline test, load lot information from file
            dtable.ReadXml(fileDlg.FileName);


            //2* //Create recipe based on Part Number and STR; 
            mappingPartNumberSTR_ReqRecipeObj.Clear();
            mappingLot_LotInfo.Clear();


            // Display items in the ListView control
            for (int i = 0; i < dtable.Rows.Count; i++)
            {
                DataRow drow = dtable.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    ListViewItem lstvItem = new ListViewItem();
                    lstvItem.Text = drow["LOT_NUMBER"].ToString();
                    lstvItem.SubItems.Add(drow["HGA_PART_NUMBER"].ToString());
                    lstvItem.SubItems.Add(drow["PROGRAM"].ToString());
                    lstvItem.SubItems.Add(drow["SUSPENSION"].ToString());
                    lstvItem.SubItems.Add(drow["SUSP_PART_NUMBER"].ToString());
                    lstvItem.SubItems.Add(drow["TYPE"].ToString());
                    lstvItem.SubItems.Add(drow["STR_NO"].ToString());
                    lstvItem.SubItems.Add(drow["LINE_NO"].ToString());
                    lstvItem.SubItems.Add(drow["QTY"].ToString());
                    lstvItem.SubItems.Add(drow["UPDATED_TIMESTAMP"].ToString());

                    // Add the list items to the ListView
                    lstviewLotDetails.Items.Add(lstvItem);

                    lstviewLotDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    lstviewLotDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    LotInformation lotinfo = new LotInformation();
                    RequestProcessRecipeAckObj recipe = new RequestProcessRecipeAckObj();
                    recipe.RecipeID = (mappingPartNumberSTR_ReqRecipeObj.Count + 1).ToString();

                    lotinfo.LotNumber       = recipe.LotNumber      = drow["LOT_NUMBER"].ToString();
                    lotinfo.PartNumber      = recipe.PartNumber     = drow["HGA_PART_NUMBER"].ToString();
                    lotinfo.Program         = recipe.ProductName    = drow["PROGRAM"].ToString();
                    lotinfo.Suspension      = recipe.Suspension     = drow["SUSPENSION"].ToString();
                    lotinfo.SuspPartNumber  = recipe.SuspPartNumber = drow["SUSP_PART_NUMBER"].ToString();
                    lotinfo.HGAType         = recipe.HGAType        = drow["TYPE"].ToString();
                    lotinfo.STR             = recipe.STR            = drow["STR_NO"].ToString();
                    lotinfo.Line            = recipe.Line           = drow["LINE_NO"].ToString();
                    lotinfo.Qty             = recipe.LotQty         = Int32.Parse(drow["QTY"].ToString());


                    #region local ilc recipe
                    if (File.Exists(exePath + @"\Recipe\ILCRecipeObj.xml"))
                    {
                        ILCRecipeObj ilcRecipe1 = ILCRecipeObj.ReadFromFile(exePath + @"\Recipe\ILCRecipeObj.xml");

                        recipe.PowerUV1 = ilcRecipe1.PowerUV1;
                        recipe.CureTimeUV1 = ilcRecipe1.CureTimeUV1;
                        recipe.EnabledUV1 = ilcRecipe1.EnabledUV1;

                        recipe.FlowRateHeater1 = ilcRecipe1.FlowRateHeater1;
                        recipe.TempHeater1 = ilcRecipe1.TempHeater1;
                        recipe.EnabledHeater1 = ilcRecipe1.EnabledHeater1;
                        recipe.EnabledN2Heater1 = ilcRecipe1.EnabledN2Heater1;

                        recipe.FlowRateHeater2 = ilcRecipe1.FlowRateHeater2;
                        recipe.TempHeater2 = ilcRecipe1.TempHeater2;
                        recipe.EnabledHeater2 = ilcRecipe1.EnabledHeater2;
                        recipe.EnabledN2Heater2 = ilcRecipe1.EnabledN2Heater2;

                        recipe.FlowRateHeater3 = ilcRecipe1.FlowRateHeater3;
                        recipe.TempHeater3 = ilcRecipe1.TempHeater3;
                        recipe.EnabledHeater3 = ilcRecipe1.EnabledHeater3;
                        recipe.EnabledN2Heater3 = ilcRecipe1.EnabledN2Heater3;

                        recipe.FlowRateHeater4 = ilcRecipe1.FlowRateHeater4;
                        recipe.TempHeater4 = ilcRecipe1.TempHeater4;
                        recipe.EnabledHeater4 = ilcRecipe1.EnabledHeater4;
                        recipe.EnabledN2Heater4 = ilcRecipe1.EnabledN2Heater4;

                        recipe.FlowRateHeater5 = ilcRecipe1.FlowRateHeater5;
                        recipe.TempHeater5 = ilcRecipe1.TempHeater5;
                        recipe.EnabledHeater5 = ilcRecipe1.EnabledHeater5;
                        recipe.EnabledN2Heater5 = ilcRecipe1.EnabledN2Heater5;

                        recipe.PowerUV2 = ilcRecipe1.PowerUV2;
                        recipe.CureTimeUV2 = ilcRecipe1.CureTimeUV2;
                        recipe.EnabledUV2 = ilcRecipe1.EnabledUV2;

                        recipe.FlowRateHeater6 = ilcRecipe1.FlowRateHeater6;
                        recipe.TempHeater6 = ilcRecipe1.TempHeater6;
                        recipe.EnabledHeater6 = ilcRecipe1.EnabledHeater6;
                        recipe.EnabledN2Heater6 = ilcRecipe1.EnabledN2Heater6;

                        recipe.FlowRateHeater7 = ilcRecipe1.FlowRateHeater7;
                        recipe.TempHeater7 = ilcRecipe1.TempHeater7;
                        recipe.EnabledHeater7 = ilcRecipe1.EnabledHeater7;
                        recipe.EnabledN2Heater7 = ilcRecipe1.EnabledN2Heater7;

                        recipe.FlowRateHeater8 = ilcRecipe1.FlowRateHeater8;
                        recipe.TempHeater8 = ilcRecipe1.TempHeater8;
                        recipe.EnabledHeater8 = ilcRecipe1.EnabledHeater8;
                        recipe.EnabledN2Heater8 = ilcRecipe1.EnabledN2Heater8;

                        recipe.FlowRateHeater9 = ilcRecipe1.FlowRateHeater9;
                        recipe.TempHeater9 = ilcRecipe1.TempHeater9;
                        recipe.EnabledHeater9 = ilcRecipe1.EnabledHeater9;
                        recipe.EnabledN2Heater9 = ilcRecipe1.EnabledN2Heater9;

                        recipe.FlowRateHeater10 = ilcRecipe1.FlowRateHeater10;
                        recipe.TempHeater10 = ilcRecipe1.TempHeater10;
                        recipe.EnabledHeater10 = ilcRecipe1.EnabledHeater10;
                        recipe.EnabledN2Heater10 = ilcRecipe1.EnabledN2Heater10;

                        recipe.Mode = ilcRecipe1.Mode;
                        recipe.Bypass = ilcRecipe1.Bypass;
                    }
                    #endregion


                    try
                    {
                        Tuple<string, string> tPartNumberSTR = new Tuple<string, string>(lotinfo.PartNumber, drow["STR_NO"].ToString());
                        mappingPartNumberSTR_ReqRecipeObj.Add(tPartNumberSTR, recipe);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("mappingPartNumberSTR_ReqRecipeObj: " + ex.Message);
                        LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        mappingLot_LotInfo.Add(recipe.LotNumber, lotinfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("mappingLot_LotInfo: " + ex.Message);
                        LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                    }
                }
            }


            //Tuple<string,string> tSearchRecipe = new Tuple<string,string>(mappingLotNumberProduct[@"WRXGC_A"],"A26003SN");
            //Console.WriteLine(mappingProgramSTRRecipe[tSearchRecipe].RecipeID);
            //Console.WriteLine(mappingProgramSTRRecipe[tSearchRecipe].ProductName);


            //Generate recipe information
            //foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingLotNumberReqRecipeObj)
            //foreach(KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            foreach(KeyValuePair<Tuple<string,string>, RequestProcessRecipeAckObj> entry in mappingPartNumberSTR_ReqRecipeObj)
            {
                //Console.WriteLine(entry.Key.ToString());
                //Console.WriteLine(((RequestProcessRecipeAckObj) entry.Value).RecipeID);

                //Item1=Program; Item2=STR
                Console.WriteLine(entry.Key.Item1);
                Console.WriteLine(entry.Key.Item2);
                Console.WriteLine(entry.Value.RecipeID);


                ListViewItem lstvItem = new ListViewItem();
                RequestProcessRecipeAckObj recipe = (RequestProcessRecipeAckObj)entry.Value;
                lstvItem.Text = recipe.RecipeID;
                //lstvItem.SubItems.Add(recipe.LotNumber);
                lstvItem.SubItems.Add(recipe.PartNumber);
                lstvItem.SubItems.Add(recipe.ProductName);
                lstvItem.SubItems.Add(recipe.STR);
                lstvItem.SubItems.Add(recipe.Suspension);
                lstvItem.SubItems.Add(recipe.SuspPartNumber);
                lstvItem.SubItems.Add(recipe.HGAType);
                lstvItem.SubItems.Add(recipe.Line);
                lstvItem.SubItems.Add(recipe.PalletID);
               
                lstviewRecipe.Items.Add(lstvItem);

                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstviewRecipe.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            this.btnGenRecipe_Click(sender, e);
        }


        private void btnExportLotInfo_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog savefileDlg = new SaveFileDialog() {Filter = "XML|*.xml", ValidateNames = true})
            {
                if (savefileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    #region fill data table
                    //lotinfo.LotNumber;        drow["LOT_NUMBER"].ToString();                  //0
                    //lotinfo.PartNumber;       drow["HGA_PART_NUMBER"].ToString();             //1
                    //lotinfo.Program;          drow["PROGRAM"].ToString();                     //2
                    //lotinfo.Suspension;       drow["SUSPENSION"].ToString();                  //3
                    //lotinfo.SuspPartNumber;   drow["SUSP_PART_NUMBER"].ToString();            //4
                    //lotinfo.HGAType;          drow["TYPE"].ToString();                        //5
                    //lotinfo.STR;              drow["STR_NO"].ToString();                      //6
                    //lotinfo.Line;             drow["LINE_NO"].ToString();                     //7
                    //lotinfo.Qty;              drow["QTY"].ToString();                         //8
                    //lotinfo.UpdateTimeStamp;  drow["UPDATED_TIMESTAMP"].ToString();           //9

                    DataTable dt = new DataTable();
                    dt.Columns.Add("LOT_NUMBER", typeof(string));
                    dt.Columns.Add("QTY", typeof(int));
                    dt.Columns.Add("SF_PART_NUMBER", typeof(string));
                    dt.Columns.Add("HGA_PART_NUMBER", typeof(string));
                    dt.Columns.Add("STR_NO", typeof(string));
                    dt.Columns.Add("LINE_NO", typeof(string));
                    dt.Columns.Add("PROGRAM", typeof(string));
                    dt.Columns.Add("SUSPENSION", typeof(string));
                    dt.Columns.Add("SUSP_PART_NUMBER", typeof(string));
                    dt.Columns.Add("TYPE", typeof(string));
                    dt.Columns.Add("UPDATED_TIMESTAMP", typeof(DateTime));

                    foreach (ListViewItem item in lstviewLotDetails.Items)
                    {
                        //int nColumns = lstviewLotDetails.Columns.Count;
                        //object[] row = new object[nColumns];
                        //for(int i =0; i < nColumns; i++)
                        //{
                        //    row[i] = item.SubItems[i].Text;
                        //}

                        object[] row = new object[11];
                        row[0] = item.SubItems[0].Text;     //lot
                        row[1] = item.SubItems[8].Text;     //qty
                        row[2] = "";                        //sf part number
                        row[3] = item.SubItems[1].Text;     //hga part number
                        row[4] = item.SubItems[6].Text;     //str

                        row[5] = item.SubItems[7].Text;     //line
                        row[6] = item.SubItems[2].Text;     //program
                        row[7] = item.SubItems[3].Text;     //suspension
                        row[8] = item.SubItems[4].Text;     //susp part number
                        row[9] = item.SubItems[5].Text;     //type

                        row[10] = item.SubItems[9].Text;    //update time stamp


                        dt.Rows.Add(row);
                    }
                    #endregion

                    DataSet ds = new DataSet("ExportLot" + DateTime.Now.ToString("MMddyyyyHHmmsstt"));
                    ds.Tables.Add(dt);
                    ds.WriteXml(savefileDlg.FileName, XmlWriteMode.WriteSchema);
                }
            }
        }


        private Dictionary<string, RequestPalletInfoAckObj> mappingLocalPalletIDReqPalletInfoObj = new Dictionary<string, RequestPalletInfoAckObj>();
        private List<RequestPalletInfoAckObj> GetLocalReqPalletInfoLookup()
        {
            mappingLocalPalletIDReqPalletInfoObj.Clear();

            List<RequestPalletInfoAckObj> lstReqPalletObjs = new List<RequestPalletInfoAckObj>();
            lstReqPalletObjs.Clear();

            string strPalletInfoPath = exePath + @"\Pallet";
            if (System.IO.Directory.Exists(strPalletInfoPath))
            {
                string[] reqPalletFiles = System.IO.Directory.GetFiles(strPalletInfoPath, "*.XML");

                if (reqPalletFiles.Length > 0)
                {
                    foreach (string file in reqPalletFiles)
                    {
                        RequestPalletInfoAckObj reqPallet = RequestPalletInfoAckObj.ReadFromFile(file);
                        lstReqPalletObjs.Add(reqPallet);

                        string temp = file.Replace(strPalletInfoPath, "");
                        temp = temp.Replace(@"\", "");
                        temp = temp.ToUpper().Replace(@".XML", "");
                        mappingLocalPalletIDReqPalletInfoObj.Add(temp, reqPallet);
                    }
                }
            }

            return lstReqPalletObjs;
        }



        List<RequestPalletInfoAckObj> lstReqPalletLookup = new List<RequestPalletInfoAckObj>();
        private void button3_Click(object sender, EventArgs e)
        {
            //string strPalletInfoPath = exePath + @"\Pallet";

            //RequestPalletInfoAckObj reqPallet = new RequestPalletInfoAckObj();
            //reqPallet.HGA.HGA1.SN = "DFALDDFA";
            //reqPallet.HGA.HGA1.Defect = "A1";
            //System.IO.File.WriteAllText(strPalletInfoPath + @"\xxx.xml", reqPallet.ToXML());


            //reqPallet = RequestPalletInfoAckObj.ReadFromFile(@"xxx.xml");
            //Console.Write("xxx");


            //RequestProcessRecipeAckObj reqRecipe = new RequestProcessRecipeAckObj();
            //System.IO.File.WriteAllText(@"recipe1.xml", reqRecipe.ToXML());

            lstReqPalletLookup.Clear();
            lstReqPalletLookup = GetLocalReqPalletInfoLookup();

            //Console.WriteLine(mappingReqPalletInfo["PT0001"].HGA.HGA1.SN);
        }

        private RequestPalletInfoAckObj RequestPalletInfo(string PalletID)
        {
            RequestPalletInfoAckObj reqPallet = new RequestPalletInfoAckObj();

            return reqPallet;
        }

        private void clearUITextData()
        {
            lbl_tab3_error.Text = "";
            lbl_tab4_error.Text = "";
            lbl_tab5_error.Text = "";

            txtbox_tab3_transID.Text = "";

            txtbox_tab3_COMMACK.Text = "";
            txtbox_tab3_ALMID.Text = "";
            radioButton_DisabledPallet.Checked = true;

            txtbox_tab3_PartNumber.Text = "";
            txtbox_tab3_ProductName.Text = "";
            txtbox_tab3_Line.Text = "";
            txtbox_tab3_ACAMID.Text = "";
            txtbox_tab3_Suspension.Text = "";
            txtbox_tab3_HGAType.Text = "";


            //reqPalletObj.EnabledPallet = true;
            //reqPalletObj.PalletID = reqPallet.PalletID;
            txtbox_tab3_LotNumber.Text = "";

            txtbox_tab3_SN1.Text = "";
            txtbox_tab3_Defect1.Text = "";
            txtbox_tab3_SN2.Text = "";
            txtbox_tab3_Defect2.Text = "";
            txtbox_tab3_SN3.Text = "";
            txtbox_tab3_Defect3.Text = "";
            txtbox_tab3_SN4.Text = "";
            txtbox_tab3_Defect4.Text = "";
            txtbox_tab3_SN5.Text = "";
            txtbox_tab3_Defect5.Text = "";

            txtbox_tab3_SN6.Text = "";
            txtbox_tab3_Defect6.Text = "";
            txtbox_tab3_SN7.Text = "";
            txtbox_tab3_Defect7.Text = "";
            txtbox_tab3_SN8.Text = "";
            txtbox_tab3_Defect8.Text = "";
            txtbox_tab3_SN9.Text = "";
            txtbox_tab3_Defect9.Text = "";
            txtbox_tab3_SN10.Text = "";
            txtbox_tab3_Defect10.Text = "";

            combo_tab3_ACAMID.SelectedIndex = 0;

            txtbox_tab3_UVPower.Text = "";
            txtbox_tab3_CureTime.Text = "";
            txtbox_tab3_CureZone.Text = "";

            txtbox_tab3_transID_Pallet1.Text = "";
            txtbox_tab3_transID_Pallet2.Text = "";

            lstboxEquipmentType.SelectedIndex = 0;
            lstboxNextEquipmentType.SelectedIndex = 0;
        }


        RequestPalletInfoAckObj _pallet1 = new RequestPalletInfoAckObj();
        RequestPalletInfoAckObj _pallet2 = new RequestPalletInfoAckObj();
        int _nPallet1_TransID = 0;
        int _nPallet2_TransID = 0;
        private void btn_tab3_ReqPalletInfo_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if (txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }

            int nPallet = 0;
            RequestPalletInfoAckObj reqPallet = new RequestPalletInfoAckObj();
            reqPallet.PalletID = txtbox_tab3_PalletID.Text;

            if (((Button)sender).Text == "Pallet1")
            {
                nPallet = 1;
                _pallet1.Clear();
                _nPallet1_TransID = 0;
            }
            else if (((Button)sender).Text == "Pallet2")
            {
                nPallet = 2;
                _pallet2.Clear();
                _nPallet2_TransID = 0;
            }




            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
            }

            if (cnn.State == ConnectionState.Open)
            {
                string sqlcommand = "";
                sqlcommand += "SELECT TOP(1) *"
                            + " FROM [dbo].[tblPalletTransaction] tblPallet"
                            + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ txtbox_tab3_PalletID.Text + "'"
                            //+ " ORDER BY tblPallet.CreatedDateTime DESC";
                            + " ORDER BY tblPallet.TransID DESC";


                System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                System.Data.DataSet dataset = new System.Data.DataSet();
                adpt.Fill(dataset);
                cnn.Close();


                System.Data.DataTableCollection tables = dataset.Tables;
                System.Data.DataTable table = tables[0];

                if (table.Rows.Count < 1)
                {
                    //no data found
                    clearUITextData();

                    lbl_tab3_error.Text = "Data not found for Pallet# " + txtbox_tab3_PalletID.Text;
                    radioButton_DisabledPallet.Checked = true;
                    return;
                }

                foreach (System.Data.DataRow row in table.Rows)
                {
                    //row[0] = TransID
                    //row[1] = PalletID
                    //row[2] = EquipmentType
                    //row[3] = NextEquipmentType
                    //row[4] = CreatedDateTime

                    //row[5] = LotNumber
                    //row6 = HGASN_1
                    //row7 = HGADefect_1
                    //row8 = HGASN_2
                    //row9 = HGADefect_2
                    //row10 = HGASN_3
                    //row11 = HGADefect_3
                    //row12 = HGASN_4
                    //row13 = HGADefect_4
                    //row14 = HGASN_5
                    //row15 = HGADefect_5

                    //row16 = HGASN_6
                    //row17 = HGADefect_6
                    //row18 = HGASN_7
                    //row19 = HGADefect_7
                    //row20 = HGASN_8
                    //row21 = HGADefect_8
                    //row22 = HGASN_9
                    //row23 = HGADefect_9
                    //row24 = HGASN_10
                    //row25 = HGADefect_10

                    //row26 = ACAMID

                    //row27 = ILC UVPower
                    //row28 = ILC CureTime
                    //row29 = ILC CureZone

                    //row30 = SJBStage

                    txtbox_tab3_COMMACK.Text = "0";                                             reqPallet.COMMACK = 0;
                    txtbox_tab3_ALMID.Text = "0";                                               reqPallet.ALMID = 0;
                    radioButton_EnabledPallet.Checked = true;                                   reqPallet.EnabledPallet = true;

                    txtbox_tab3_transID.Text = row[0].ToString();

                    int nEquipmentType = Int32.Parse(row[2].ToString());                        reqPallet.EquipmentType = nEquipmentType;
                    int nNextEquipmentType = Int32.Parse(row[3].ToString());                    reqPallet.NextEquipmentType = nNextEquipmentType;
                    EQUIPMENT_TYPE enEquipmentType = (EQUIPMENT_TYPE) nEquipmentType;
                    EQUIPMENT_TYPE enNextEquipmentType = (EQUIPMENT_TYPE)nNextEquipmentType;


                    #region EquipmentType
                    switch (enEquipmentType.ToString())
                    {
                        case "ASLV":
                            lstboxEquipmentType.SelectedIndex = 0;
                            break;

                        case "ACAM":
                            lstboxEquipmentType.SelectedIndex = 1;
                            break;

                        case "ILC":
                            lstboxEquipmentType.SelectedIndex = 2;
                            break;

                        case "SJB":
                            lstboxEquipmentType.SelectedIndex = 3;
                            break;

                        case "AVI":
                            lstboxEquipmentType.SelectedIndex = 4;
                            break;

                        case "UNOCR":
                            lstboxEquipmentType.SelectedIndex = 5;
                            break;

                        case "FVMI":
                            lstboxEquipmentType.SelectedIndex = 6;
                            break;

                        case "APT":
                            lstboxEquipmentType.SelectedIndex = 7;
                            break;

                        default:
                            break;
                    };
                    #endregion


                    #region NextEquipmentType
                    switch (enNextEquipmentType.ToString())
                    {
                        case "ASLV":
                            lstboxNextEquipmentType.SelectedIndex = 0;
                            break;

                        case "ACAM":
                            lstboxNextEquipmentType.SelectedIndex = 1;
                            break;

                        case "ILC":
                            lstboxNextEquipmentType.SelectedIndex = 2;
                            break;

                        case "SJB":
                            lstboxNextEquipmentType.SelectedIndex = 3;
                            break;

                        case "AVI":
                            lstboxNextEquipmentType.SelectedIndex = 4;
                            break;

                        case "UNOCR":
                            lstboxNextEquipmentType.SelectedIndex = 5;
                            break;

                        case "FVMI":
                            lstboxNextEquipmentType.SelectedIndex = 6;
                            break;

                        case "APT":
                            lstboxNextEquipmentType.SelectedIndex = 7;
                            break;

                        default:
                            break;
                    };
                    #endregion


                    try
                    {
                        string strFindInfoFromLot = row[5].ToString();

                        RequestProcessRecipeAckObj recipeObj = new RequestProcessRecipeAckObj();
                        string strFindProductFromLot = string.Empty;

                        LotInformation requestedLot = new LotInformation();
                        //(mappingLotNumber_Product.TryGetValue(strFindInfoFromLot, out strFindProductFromLot))
                        if (mappingLot_LotInfo.TryGetValue(strFindInfoFromLot, out requestedLot))
                        {

                            Tuple<string, string> tProgramSTR =  new Tuple<string, string>(requestedLot.PartNumber, requestedLot.STR);
                            if(mappingPartNumberSTR_ReqRecipeObj.TryGetValue(tProgramSTR, out recipeObj))
                            {
                                reqPallet.PartNumber    = txtbox_tab3_PartNumber.Text   = recipeObj.PartNumber;
                                reqPallet.ProductName   = txtbox_tab3_ProductName.Text  = recipeObj.ProductName;
                                reqPallet.Line          =  txtbox_tab3_Line.Text        = recipeObj.Line;
                                reqPallet.Suspension    = txtbox_tab3_Suspension.Text   = recipeObj.Suspension;
                                reqPallet.HGAType       =  txtbox_tab3_HGAType.Text     = recipeObj.HGAType;
                            }
                            else
                            {
                                reqPallet.PartNumber    = txtbox_tab3_PartNumber.Text   = "";
                                reqPallet.ProductName   = txtbox_tab3_ProductName.Text  = "";
                                reqPallet.Line          = txtbox_tab3_Line.Text         = "";
                                reqPallet.Suspension    = txtbox_tab3_Suspension.Text   = "";
                                reqPallet.HGAType       = txtbox_tab3_HGAType.Text      = "";
                            }
                        }
                        else
                        {
                            reqPallet.PartNumber    = txtbox_tab3_PartNumber.Text   = "";
                            reqPallet.ProductName   = txtbox_tab3_ProductName.Text  = "";
                            reqPallet.Line          = txtbox_tab3_Line.Text         = "";
                            reqPallet.Suspension    = txtbox_tab3_Suspension.Text   = "";
                            reqPallet.HGAType       = txtbox_tab3_HGAType.Text      = "";
                        }

                        reqPallet.ACAMID = txtbox_tab3_ACAMID.Text = row[26].ToString();
                        int nACAM = combo_tab3_ACAMID.Items.IndexOf(row[26].ToString());
                        combo_tab3_ACAMID.SelectedIndex = (nACAM < 0 ? 0 : nACAM);


                        //reqPalletObj.EnabledPallet = true;
                        //reqPalletObj.PalletID = reqPallet.PalletID;
                        //reqPallet.LotNumber     = txtbox_tab3_LotNumber.Text = row[5].ToString();


                        // to remove non character strings
                        //char[] arrLots = reqPallet.LotNumber.ToCharArray();
                        //Console.WriteLine(arrLots.Length.ToString());

                        char[] arrTemp = row[5].ToString().ToCharArray();
                        arrTemp = Array.FindAll<char>(arrTemp, (c => (char.IsLetterOrDigit(c)
                                                                    || char.IsWhiteSpace(c)
                                                                    || c == '_')));

                        reqPallet.LotNumber = txtbox_tab3_LotNumber.Text = new string(arrTemp);
                        // to remove non character strings



                        reqPallet.HGA.HGA1.SN       = txtbox_tab3_SN1.Text      = row[6].ToString();
                        reqPallet.HGA.HGA1.Defect   = txtbox_tab3_Defect1.Text  = row[7].ToString();

                        reqPallet.HGA.HGA2.SN       = txtbox_tab3_SN2.Text      = row[8].ToString();
                        reqPallet.HGA.HGA2.Defect   = txtbox_tab3_Defect2.Text  = row[9].ToString();

                        reqPallet.HGA.HGA3.SN       = txtbox_tab3_SN3.Text      = row[10].ToString();
                        reqPallet.HGA.HGA3.Defect   = txtbox_tab3_Defect3.Text  = row[11].ToString();

                        reqPallet.HGA.HGA4.SN       = txtbox_tab3_SN4.Text      = row[12].ToString();
                        reqPallet.HGA.HGA4.Defect   = txtbox_tab3_Defect4.Text  = row[13].ToString();

                        reqPallet.HGA.HGA5.SN       =  txtbox_tab3_SN5.Text     = row[14].ToString();
                        reqPallet.HGA.HGA5.Defect   = txtbox_tab3_Defect5.Text  = row[15].ToString();


                        reqPallet.HGA.HGA6.SN       = txtbox_tab3_SN6.Text      = row[16].ToString();
                        reqPallet.HGA.HGA6.Defect   = txtbox_tab3_Defect6.Text  = row[17].ToString();

                        reqPallet.HGA.HGA7.SN       = txtbox_tab3_SN7.Text      = row[18].ToString();
                        reqPallet.HGA.HGA7.Defect   = txtbox_tab3_Defect7.Text  = row[19].ToString();

                        reqPallet.HGA.HGA8.SN       = txtbox_tab3_SN8.Text      = row[20].ToString();
                        reqPallet.HGA.HGA8.Defect   = txtbox_tab3_Defect8.Text  = row[21].ToString();

                        reqPallet.HGA.HGA9.SN       = txtbox_tab3_SN9.Text      = row[22].ToString();
                        reqPallet.HGA.HGA9.Defect   = txtbox_tab3_Defect9.Text  = row[23].ToString();

                        reqPallet.HGA.HGA10.SN      = txtbox_tab3_SN10.Text     = row[24].ToString();
                        reqPallet.HGA.HGA10.Defect  = txtbox_tab3_Defect10.Text = row[25].ToString();


                        txtbox_tab3_UVPower.Text = row[27].ToString();
                        reqPallet.UVPower = (txtbox_tab3_UVPower.Text.Length < 1) ? 0 : Int32.Parse(txtbox_tab3_UVPower.Text);

                        txtbox_tab3_CureTime.Text = row[28].ToString();
                        reqPallet.CureTime = (txtbox_tab3_CureTime.Text.Length < 1) ? 0 : Int32.Parse(txtbox_tab3_CureTime.Text);

                        txtbox_tab3_CureZone.Text = row[29].ToString();
                        reqPallet.CureZone = (txtbox_tab3_CureZone.Text.Length < 1) ? 0 : Int32.Parse(txtbox_tab3_CureZone.Text);

                        reqPallet.SJBStage = row[30].ToString();


                        //row[32] = xLoc_ACAM_1,
                        //row[33] = yLoc_ACAM_1,
                        //row[34] = skwAngl_ACAM_1,
                        //row[35] = xLoc_SAI_1,
                        //row[36] = yLoc_SAI_1,
                        //row[37] = skwAngl_SAI_1
                        //...

                        //row[86] = xLoc_ACAM_10,
                        //row[87] = yLoc_ACAM_10,
                        //row[88] = skwAngl_ACAM_10,
                        //row[89] = xLoc_SAI_10,
                        //row[90] = yLoc_SAI_10,
                        //row[91] = skwAngl_SAI_10


                        //row[92] = SJBFixture
                        txtbox_tab3_SJBFixture.Text = row[92].ToString();

                    }
                    catch (Exception ex)
                    {
                        lbl_tab3_error.Text = ex.Message;
                        LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                        return;
                    }
                }
            }

            lbl_tab3_error.Text = "RequestPalletInfo executed successfully";

            if (nPallet == 1)
            {
                _pallet1 = RequestPalletInfoAckObj.ToObj(reqPallet.ToXML());
                _nPallet1_TransID = Int32.Parse(txtbox_tab3_transID.Text);
            }
            else if (nPallet == 2)
            {
                _pallet2 = RequestPalletInfoAckObj.ToObj(reqPallet.ToXML());
                _nPallet2_TransID = Int32.Parse(txtbox_tab3_transID.Text);
            }
        }

        private void btn_tab3_SendPalletInfo_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if (txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }


            SendPalletInfoObj sndPalletObj = new SendPalletInfoObj();

            EQUIPMENT_TYPE enEquipmentType;
            int nEquipmentType;

            EQUIPMENT_TYPE enNextEquipmentType;
            int nNextEquipmentType;

            try
            {
                sndPalletObj.PalletID = txtbox_tab3_PalletID.Text;
                sndPalletObj.LotNumber = txtbox_tab3_LotNumber.Text;


                enEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), lstboxEquipmentType.SelectedItem.ToString());
                nEquipmentType = (int)enEquipmentType;

                enNextEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), lstboxNextEquipmentType.SelectedItem.ToString());
                nNextEquipmentType = (int)enNextEquipmentType;

                sndPalletObj.HGA.HGA1.SN = txtbox_tab3_SN1.Text;
                sndPalletObj.HGA.HGA1.Defect = txtbox_tab3_Defect1.Text;

                sndPalletObj.HGA.HGA2.SN = txtbox_tab3_SN2.Text;
                sndPalletObj.HGA.HGA2.Defect = txtbox_tab3_Defect2.Text;

                sndPalletObj.HGA.HGA3.SN = txtbox_tab3_SN3.Text;
                sndPalletObj.HGA.HGA3.Defect = txtbox_tab3_Defect3.Text;

                sndPalletObj.HGA.HGA4.SN = txtbox_tab3_SN4.Text;
                sndPalletObj.HGA.HGA4.Defect = txtbox_tab3_Defect4.Text;

                sndPalletObj.HGA.HGA5.SN = txtbox_tab3_SN5.Text;
                sndPalletObj.HGA.HGA5.Defect = txtbox_tab3_Defect5.Text;

                sndPalletObj.HGA.HGA6.SN = txtbox_tab3_SN6.Text;
                sndPalletObj.HGA.HGA6.Defect = txtbox_tab3_Defect6.Text;

                sndPalletObj.HGA.HGA7.SN = txtbox_tab3_SN7.Text;
                sndPalletObj.HGA.HGA7.Defect = txtbox_tab3_Defect7.Text;

                sndPalletObj.HGA.HGA8.SN = txtbox_tab3_SN8.Text;
                sndPalletObj.HGA.HGA8.Defect = txtbox_tab3_Defect8.Text;

                sndPalletObj.HGA.HGA9.SN = txtbox_tab3_SN9.Text;
                sndPalletObj.HGA.HGA9.Defect = txtbox_tab3_Defect9.Text;

                sndPalletObj.HGA.HGA10.SN = txtbox_tab3_SN10.Text;
                sndPalletObj.HGA.HGA10.Defect = txtbox_tab3_Defect10.Text;


                if(txtbox_tab3_SJBFixture.Text.Length != 0)
                {
                    sndPalletObj.SJBFixture = Int32.Parse(txtbox_tab3_SJBFixture.Text);
                }

            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }


            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;

            //EXEC [spUpdatePalletTransaction] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO'
            //EXEC [spUpdatePalletTransactionACAM] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO','APT001'
            //EXEC [spUpdatePalletTransactionACAMILC] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO','APT001', 0, 0.0, 0
            //strSQLCmd += "EXEC [spUpdatePalletTransactionACAMILC] " + /*transID*/ txtbox_tab3_transID.Text + ",'" + sndPalletObj.PalletID + "',";
            strSQLCmd += "EXEC [spUpdatePalletTransactionACAM] " + /*transID*/ txtbox_tab3_transID.Text + ",'" + sndPalletObj.PalletID + "',";
            strSQLCmd += nEquipmentType.ToString() + "," + nNextEquipmentType.ToString() + ",";
            strSQLCmd += "'" + sndPalletObj.LotNumber + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA1.SN + "'," + "'" + sndPalletObj.HGA.HGA1.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA2.SN + "'," + "'" + sndPalletObj.HGA.HGA2.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA3.SN + "'," + "'" + sndPalletObj.HGA.HGA3.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA4.SN + "'," + "'" + sndPalletObj.HGA.HGA4.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA5.SN + "'," + "'" + sndPalletObj.HGA.HGA5.Defect + "',";

            strSQLCmd += "'" + sndPalletObj.HGA.HGA6.SN + "'," + "'" + sndPalletObj.HGA.HGA6.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA7.SN + "'," + "'" + sndPalletObj.HGA.HGA7.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA8.SN + "'," + "'" + sndPalletObj.HGA.HGA8.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA9.SN + "'," + "'" + sndPalletObj.HGA.HGA9.Defect + "',";
            strSQLCmd += "'" + sndPalletObj.HGA.HGA10.SN + "'," + "'" + sndPalletObj.HGA.HGA10.Defect + "'";

            strSQLCmd += ",'" + combo_tab3_ACAMID.Text + "'";   

            //// Commented out; to prevent accidently overwriting ILC Zone when manually editting data
            //strSQLCmd += "," + (txtbox_tab3_UVPower.Text.Length < 1 ? "0" : txtbox_tab3_UVPower.Text);
            //strSQLCmd += "," + (txtbox_tab3_CureTime.Text.Length < 1 ? "0.0" : txtbox_tab3_CureTime.Text);
            //strSQLCmd += "," + (txtbox_tab3_CureZone.Text.Length < 1 ? "0" : txtbox_tab3_CureZone.Text);  //for EXEC [spUpdatePalletTransactionACAMILC]


            try
            {
                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                sqlcommand.Connection.Open();
                int nRet = sqlcommand.ExecuteNonQuery();
                sqlcommand.Connection.Close();
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }


            //Update SJBFixture
            strSQLCmd = string.Empty;

            //EXEC [spUpdatePalletTransactionWithSJBFixture] 192574, 'PT0126', 40, 50, 2
            strSQLCmd += "EXEC [spUpdatePalletTransactionWithSJBFixture] " + /*transID*/ txtbox_tab3_transID.Text + ",'" + sndPalletObj.PalletID + "',";
            strSQLCmd += nEquipmentType.ToString() + "," + nNextEquipmentType.ToString() + ",";
            strSQLCmd += sndPalletObj.SJBFixture.ToString();

            try
            {
                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                sqlcommand.Connection.Open();
                int nRet = sqlcommand.ExecuteNonQuery();
                sqlcommand.Connection.Close();
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }



            lbl_tab3_error.Text = "SendPalletInfo executed successfully";
            LoggerClass.Instance.MainLogInfo("Edit SendPalletInfo: " + sndPalletObj.PalletID + "," + txtbox_tab3_transID.Text + "," + sndPalletObj.LotNumber + "," + sndPalletObj.EquipmentID);
        }

        private void btn_tab3_NewPallet_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if (txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }


            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";
            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;


            //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
            //10    , 80    , 30    , 40    , 50    , 60

            //EXEC [spNewPalletTransaction] 'PT0001', 20, 30, '05-03-2017 16:40:00 PM', 'WYKU7_AB2',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'',''
            strSQLCmd += "EXEC [spNewPalletTransaction] " + "'" + txtbox_tab3_PalletID.Text + "',";
            strSQLCmd += " 60, 10,";
            strSQLCmd += "'" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt") + "',";
            strSQLCmd += "'',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'','',";
            strSQLCmd += "'',''";


            try
            {
                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                sqlcommand.Connection.Open();
                int nRet = sqlcommand.ExecuteNonQuery();
                sqlcommand.Connection.Close();
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }

            lbl_tab3_error.Text = "New Pallet executed successfully";
            LoggerClass.Instance.MainLogInfo("NewPallet: " + txtbox_tab3_PalletID.Text);
        }


        private void PrimaryMessageTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                contextMenuStrip1.Show(this.Location.X + e.Location.X, this.Location.Y + e.Location.Y + 360);
                //contextMenuStrip1.Show( (Control)sender, ((TextBox)sender).Location.X + e.Location.X, ((TextBox)sender).Location.Y + e.Location.Y);
            }
        }


        private void SecondaryMessageTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                contextMenuStrip2.Show(this.Location.X + e.Location.X, this.Location.Y + e.Location.Y + 460);
            }
        }


        private void btn_tab3_RandomSN_Click(object sender, EventArgs e)
        {
            txtbox_tab3_SN1.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN2.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN3.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN4.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN5.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);

            txtbox_tab3_SN6.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN7.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN8.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN9.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
            txtbox_tab3_SN10.Text = GenSerialNumberFromLot(txtbox_tab3_LotNumber.Text);
        }



        private void copyTextMenuContext1_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(PrimaryMessageTextBox.Text);
            }
            catch (Exception ex)
            {
                Clipboard.Clear();
            }
        }



        private void copyTextMenuContext2_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(SecondaryMessageTextBox.Text);
            }
            catch (Exception ex)
            {
                Clipboard.Clear();
            }
        }



        public static String PrintXML(String XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            XML = "<root>" + XML + "</root>";
            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

        private void showXMLMenuContext1_Click(object sender, EventArgs e)
        {
            try
            {
                FormShowXML frm = new FormShowXML(PrintXML(PrimaryMessageTextBox.Text));
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }

        private void showXMLMenuContext2_Click(object sender, EventArgs e)
        {
            try
            {
                FormShowXML frm = new FormShowXML(PrintXML(SecondaryMessageTextBox.Text));
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }

        private void btn_tab3_ClearData_Click(object sender, EventArgs e)
        {
            clearUITextData();
        }

        private void btn_tab4_NewAllPallets_Click(object sender, EventArgs e)
        {
            lbl_tab4_error.Text = "";

            if (txtbox_tab4_PalletList.Text.Length < 1)
            {
                return;
            }

            string[] lstPallets = txtbox_tab4_PalletList.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";
            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;


            //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
            //10    , 80    , 30    , 40    , 50    , 60

            //EXEC [spNewPalletTransaction] 'PT0001', 20, 30, '05-03-2017 16:40:00 PM', 'WYKU7_AB2',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'','',
            //'',''

            foreach (string pallet in lstPallets)
            {
                strSQLCmd = "EXEC [spNewPalletTransaction] " + "'" + pallet.ToUpper() + "',";
                strSQLCmd += " 60, 10,";
                strSQLCmd += "'" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt") + "',";
                strSQLCmd += "'',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'','',";
                strSQLCmd += "'',''";


                try
                {
                    System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                    sqlcommand.Connection.Open();
                    int nRet = sqlcommand.ExecuteNonQuery();
                    sqlcommand.Connection.Close();
                }
                catch (Exception ex)
                {
                    lbl_tab4_error.Text = ex.Message;
                    LoggerClass.Instance.MainLogInfo("NewAllPallets: " + ex.Message + "," + ex.StackTrace);
                    return;
                }
            }

            LoggerClass.Instance.MainLogInfo("NewAllPallets: " + String.Join(",",lstPallets));

            lbl_tab4_error.Text = "New All Pallets successfully";
        }

        private void btn_tab3_Pallet1_Click(object sender, EventArgs e)
        {
            if(txtbox_tab3_Pallet1.Text.Length < 1)
            {
                return;
            }

            txtbox_tab3_PalletID.Text = txtbox_tab3_Pallet1.Text;
            btn_tab3_ReqPalletInfo_Click(sender, e);

            txtbox_tab3_transID_Pallet1.Text = txtbox_tab3_transID.Text;
        }

        private void btn_tab3_Pallet2_Click(object sender, EventArgs e)
        {
            if(txtbox_tab3_Pallet2.Text.Length < 1)
            {
                return;
            }

            txtbox_tab3_PalletID.Text = txtbox_tab3_Pallet2.Text;
            btn_tab3_ReqPalletInfo_Click(sender, e);

            txtbox_tab3_transID_Pallet2.Text = txtbox_tab3_transID.Text;
        }


        private void txtbox_tab3_Pallet1_TextChanged(object sender, EventArgs e)
        {
            _nPallet1_TransID = 0;
        }

        private void txtbox_tab3_Pallet2_TextChanged(object sender, EventArgs e)
        {
            _nPallet2_TransID = 0;
        }

        private void btn_tab3_MovePallet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to proceed? Pallet1's data will be erased.", "Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            if ((_nPallet2_TransID == 0) || (txtbox_tab3_transID_Pallet1.Text.Length < 1) || (txtbox_tab3_transID_Pallet2.Text.Length < 1))
            {
                return;
            }

            string temp = _pallet2.PalletID;
            _pallet2 = RequestPalletInfoAckObj.ToObj(_pallet1.ToXML());
            _pallet2.PalletID = temp;


            //---------
            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;

            //EXEC [spUpdatePalletTransactionACAM] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO','APT001'
            //EXEC [spUpdatePalletTransactionACAMILC] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO','APT001', 0, 0.0, 0
            strSQLCmd += "EXEC [spUpdatePalletTransactionACAMILC] " + /*transID*/ _nPallet2_TransID + ",'" + _pallet2.PalletID+ "',";
            strSQLCmd += _pallet2.EquipmentType.ToString() + "," + _pallet2.NextEquipmentType.ToString() + ",";
            strSQLCmd += "'" + _pallet2.LotNumber + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA1.SN + "'," + "'" + _pallet2.HGA.HGA1.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA2.SN + "'," + "'" + _pallet2.HGA.HGA2.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA3.SN + "'," + "'" + _pallet2.HGA.HGA3.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA4.SN + "'," + "'" + _pallet2.HGA.HGA4.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA5.SN + "'," + "'" + _pallet2.HGA.HGA5.Defect + "',";

            strSQLCmd += "'" + _pallet2.HGA.HGA6.SN + "'," + "'" + _pallet2.HGA.HGA6.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA7.SN + "'," + "'" + _pallet2.HGA.HGA7.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA8.SN + "'," + "'" + _pallet2.HGA.HGA8.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA9.SN + "'," + "'" + _pallet2.HGA.HGA9.Defect + "',";
            strSQLCmd += "'" + _pallet2.HGA.HGA10.SN + "'," + "'" + _pallet2.HGA.HGA10.Defect + "'";

            strSQLCmd += ",'" + _pallet2.ACAMID + "'";   

            strSQLCmd += "," + _pallet2.UVPower.ToString();
            strSQLCmd += "," + _pallet2.CureTime.ToString();
            strSQLCmd += "," + _pallet2.CureZone.ToString();  //for EXEC [spUpdatePalletTransactionACAMILC]

            //copy from pallet1 to pallet2
            try
            {
                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                sqlcommand.Connection.Open();
                int nRet = sqlcommand.ExecuteNonQuery();
                sqlcommand.Connection.Close();
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }


            strSQLCmd = string.Empty;
            strSQLCmd += "EXEC [spUpdatePalletTransactionACAMILC] " + /*transID*/ _nPallet1_TransID + ",'" + _pallet1.PalletID + "',";
            strSQLCmd += _pallet1.EquipmentType.ToString() + "," + _pallet1.NextEquipmentType.ToString() + ",";
            strSQLCmd += "'" + /*_pallet1.LotNumber*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA1.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA1.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA2.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA2.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA3.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA3.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA4.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA4.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA5.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA5.Defect*/ "" + "',";

            strSQLCmd += "'" + /*_pallet1.HGA.HGA6.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA6.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA7.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA7.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA8.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA8.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA9.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA9.Defect*/ "" + "',";
            strSQLCmd += "'" + /*_pallet1.HGA.HGA10.SN*/ "" + "'," + "'" + /*_pallet1.HGA.HGA10.Defect*/ "" + "'";

            strSQLCmd += ",'" + /*_pallet1.ACAMID*/ "" + "'";

            strSQLCmd += "," + /*_pallet1.UVPower.ToString()*/ "0";
            strSQLCmd += "," + /*_pallet1.CureTime.ToString()*/ "0.0";
            strSQLCmd += "," + /*_pallet1.CureZone.ToString()*/ "0";  //for EXEC [spUpdatePalletTransactionACAMILC]

            //remove all data of pallet1
            try
            {
                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                sqlcommand.Connection.Open();
                int nRet = sqlcommand.ExecuteNonQuery();
                sqlcommand.Connection.Close();
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                LoggerClass.Instance.ErrorLogInfo(ex.Message + "," + ex.StackTrace);
                return;
            }


            lbl_tab3_error.Text = "Move Pallet executed successfully";
        }

        private void btn_tab3_NoHGA1_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect1.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA2_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect2.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA3_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect3.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA4_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect4.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA5_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect5.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA6_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect6.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA7_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect7.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA8_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect8.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA9_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect9.Text = "NO_HGA";
        }

        private void btn_tab3_NoHGA10_Click(object sender, EventArgs e)
        {
            txtbox_tab3_Defect10.Text = "NO_HGA";
        }

        private void lstviewLotDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                //Console.WriteLine(sender.ToString() + " " +  DateTime.Now.Second.ToString());
                
                if (lstviewLotDetails.SelectedItems.Count <= 0)
                {
                    return;
                }
                /*
                foreach (ListViewItem lvi in lstviewLotDetails.SelectedItems)
                {
                    //Console.WriteLine(lvi.SubItems[0].Text);
                    StringBuilder sb = new StringBuilder();
                    sb.Clear();

                    sb.Append(lvi.Text);
                    if (sb.Length <= 0)
                    {
                        return;
                    }

                    Clipboard.Clear();
                    while (Clipboard.GetText().Length <= 0)
                    {
                        try
                        {
                            Clipboard.SetText(sb.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }                    

                }
                */

                /*
                if (sender == lstviewLotDetails)
                {
                    e.SuppressKeyPress = true;

                    //Console.WriteLine(lstviewLotDetails.SelectedItems[0].SubItems[0].Text);
                    Clipboard.SetText(lstviewLotDetails.SelectedItems[0].SubItems[0].Text);
                }
                */

                clipboardSetText(lstviewLotDetails.SelectedItems[0].SubItems[0].Text);

            }
        }

        protected void clipboardSetText(string inTextToCopy)
        {
            var clipboardThread = new Thread(() => clipBoardThreadWorker(inTextToCopy));
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.IsBackground = false;
            clipboardThread.Start();
        }
        private void clipBoardThreadWorker(string inTextToCopy)
        {
            Clipboard.SetText(inTextToCopy);
        }

        private void btnReqSusp_Click(object sender, EventArgs e)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestSuspension";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Value = true;

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = txtboxReqSuspACAMID.Text });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = Int32.Parse(txtboxReqSuspSuspAmt.Text) });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = /*Int32.Parse(_equipmentObj.DeviceID)*/ 1,
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };


            //
            hostController host = new hostController();
            host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView
            //

            SECsPrimaryInEventArgs sciEvnt = new SECsPrimaryInEventArgs(trans, host.localIPAddress, host.localPortNumber);
            sciEvnt.Transaction = trans;

            this.hostController_SECsPrimaryIn(this, sciEvnt);
            this.btnReqSuspRefresh_Click(this, e);
        }

        private void btnReqSuspRefresh_Click(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;

            lbl_tab5_error.Text = "";
            lstviewReqSusp.Items.Clear();

            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
                lbl_tab5_error.Text = ex.Message;
            }

            if (cnn.State == ConnectionState.Open)
            {
                string sqlcommand = "";
                sqlcommand += "SELECT TOP(100) *"
                            + " FROM [dbo].[tblACAMRequestSusp] tblPallet"
                            + " WHERE tblPallet.PalletSN is null"
                            + " ORDER BY tblPallet.Id ASC";


                System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                System.Data.DataSet dataset = new System.Data.DataSet();
                adpt.Fill(dataset);
                cnn.Close();


                System.Data.DataTableCollection tables = dataset.Tables;
                System.Data.DataTable dtable = tables[0];

                if (dtable.Rows.Count < 1)
                {
                    lbl_tab5_error.Text = "No requests from ACAM";
                    return;
                }


                // Display items in the ListView control
                for (int i = 0; i < dtable.Rows.Count; i++)
                {
                    DataRow drow = dtable.Rows[i];

                    // Only row that have not been deleted
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        ListViewItem lstvItem = new ListViewItem();
                        
                        lstvItem.Text = drow[0].ToString();
                        lstvItem.SubItems.Add(drow[1].ToString());
                        lstvItem.SubItems.Add(drow[2].ToString());
                        lstvItem.SubItems.Add(drow[3].ToString());
                        lstvItem.SubItems.Add(drow[4].ToString());
                        lstvItem.SubItems.Add(drow[5].ToString());
                        lstvItem.SubItems.Add(drow[6].ToString());
                        lstvItem.SubItems.Add(drow[7].ToString());
                        lstvItem.SubItems.Add(drow[8].ToString());

                        // Add the list items to the ListView
                        lstviewReqSusp.Items.Add(lstvItem);                    
                    }

                }
            }

        }

        private void btnMultiReqSusp10_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 10; i++)
            {
                SCIMessage primMsg = new SCIMessage();
                primMsg.CommandID = "RequestSuspension";
                primMsg.Item = new SCIItem();
                primMsg.Item.Format = SCIFormat.List;
                primMsg.Item.Items = new SCIItemCollection();
                primMsg.Item.Value = true;

                primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = txtboxReqSuspACAMID.Text });
                primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = Int32.Parse(txtboxReqSuspSuspAmt.Text) });

                SCITransaction trans = new SCITransaction()
                {
                    DeviceId = /*Int32.Parse(_equipmentObj.DeviceID)*/ 1,
                    MessageType = MessageType.Primary,
                    //Id = _transactionID + 1,
                    Name = primMsg.CommandID,
                    NeedReply = true,
                    Primary = primMsg,
                    Secondary = null
                };


                //
                hostController host = new hostController();
                host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView
                //

                SECsPrimaryInEventArgs sciEvnt = new SECsPrimaryInEventArgs(trans, host.localIPAddress, host.localPortNumber);
                sciEvnt.Transaction = trans;

                this.hostController_SECsPrimaryIn(this, sciEvnt);

                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }

            this.btnReqSuspRefresh_Click(this, e);
        }

        private void btnMultiReqSusp50_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                SCIMessage primMsg = new SCIMessage();
                primMsg.CommandID = "RequestSuspension";
                primMsg.Item = new SCIItem();
                primMsg.Item.Format = SCIFormat.List;
                primMsg.Item.Items = new SCIItemCollection();
                primMsg.Item.Value = true;

                primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = txtboxReqSuspACAMID.Text });
                primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = Int32.Parse(txtboxReqSuspSuspAmt.Text) });

                SCITransaction trans = new SCITransaction()
                {
                    DeviceId = /*Int32.Parse(_equipmentObj.DeviceID)*/ 1,
                    MessageType = MessageType.Primary,
                    //Id = _transactionID + 1,
                    Name = primMsg.CommandID,
                    NeedReply = true,
                    Primary = primMsg,
                    Secondary = null
                };


                //
                hostController host = new hostController();
                host = this.ListOfHost[MasterDataGridView.CurrentCell.RowIndex];        //selected tool on DataGridView
                //

                SECsPrimaryInEventArgs sciEvnt = new SECsPrimaryInEventArgs(trans, host.localIPAddress, host.localPortNumber);
                sciEvnt.Transaction = trans;

                this.hostController_SECsPrimaryIn(this, sciEvnt);

                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }

            this.btnReqSuspRefresh_Click(this, e);
        }

        private void btnDeleteReqSusp_Click(object sender, EventArgs e)
        {
            if (lstviewReqSusp.Items.Count == 0)
            {
                return;
            }

            Console.WriteLine(lstviewReqSusp.SelectedItems[0].SubItems[0].Text);
            string strId = lstviewReqSusp.SelectedItems[0].SubItems[0].Text;
            if (strId.Length < 1)
            {
                return;
            }

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            try
            {
                //-- DELETE FROM tblACAMRequestSusp WHERE Id = 1
                string strDelReqSuspSQLCmd = "DELETE FROM tblACAMRequestSusp WHERE Id = " + strId;
                System.Data.SqlClient.SqlCommand sqlcmdDelReqSusp = new SqlCommand(strDelReqSuspSQLCmd, cnn);
                sqlcmdDelReqSusp.Connection.Open();
                int nRet = sqlcmdDelReqSusp.ExecuteNonQuery();
                sqlcmdDelReqSusp.Connection.Close();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
                lbl_tab5_error.Text = ex.Message;
            }

            this.btnReqSuspRefresh_Click(this, e);
        }

        private void btnPendingACAMRefresh_Click(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;

            lbl_tab5_error.Text = "";
            lstviewPendingACAM.Items.Clear();

            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
                lbl_tab5_error.Text = ex.Message;
            }

            if (cnn.State == ConnectionState.Open)
            {
                string sqlcommand = "";
                sqlcommand += "SELECT TOP(100) *"
                            + " FROM [dbo].[tblACAMRequestSusp] tblPallet"
                            + " WHERE tblPallet.PalletSN IS NOT NULL"
                            + " AND (tblPallet.IsProcessed = 0 OR tblPallet.IsProcessed = 3)"
                            + " ORDER BY tblPallet.Id DESC";


                System.Data.SqlClient.SqlDataAdapter adpt = new SqlDataAdapter(sqlcommand, cnn);
                System.Data.DataSet dataset = new System.Data.DataSet();
                adpt.Fill(dataset);
                cnn.Close();


                System.Data.DataTableCollection tables = dataset.Tables;
                System.Data.DataTable dtable = tables[0];

                if (dtable.Rows.Count < 1)
                {
                    lbl_tab5_error.Text = "No requests from ACAM";
                    return;
                }


                // Display items in the ListView control
                for (int i = 0; i < dtable.Rows.Count; i++)
                {
                    DataRow drow = dtable.Rows[i];

                    // Only row that have not been deleted
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        ListViewItem lstvItem = new ListViewItem();

                        lstvItem.Text = drow[0].ToString();
                        lstvItem.SubItems.Add(drow[1].ToString());
                        lstvItem.SubItems.Add(drow[2].ToString());
                        lstvItem.SubItems.Add(drow[3].ToString());
                        lstvItem.SubItems.Add(drow[4].ToString());
                        lstvItem.SubItems.Add(drow[5].ToString());
                        lstvItem.SubItems.Add(drow[6].ToString());
                        lstvItem.SubItems.Add(drow[7].ToString());
                        lstvItem.SubItems.Add(drow[8].ToString());

                        // Add the list items to the ListView
                        if (lstvItem.SubItems[1].Text == "APT001")
                        {
                            lstvItem.BackColor = Color.AntiqueWhite;
                        }
                        else
                        {
                            lstvItem.BackColor = Color.LightSeaGreen;
                        }


                        lstviewPendingACAM.Items.Add(lstvItem);
                    }

                }
            }
        }

        private void btnPendingACAMCancel_Click(object sender, EventArgs e)
        {
            if (lstviewPendingACAM.Items.Count == 0)
            {
                return;
            }

            Console.WriteLine(lstviewPendingACAM.SelectedItems[0].SubItems[0].Text);
            string strId = lstviewPendingACAM.SelectedItems[0].SubItems[0].Text;
            if (strId.Length < 1)
            {
                return;
            }

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            try
            {
                //-- UPDATE tblACAMRequestSusp SET IsProcessed = 2 WHERE Id = 1
                string strCancelPendingReqSuspSQLCmd = "UPDATE tblACAMRequestSusp SET IsProcessed = 2 WHERE Id = " + strId;
                System.Data.SqlClient.SqlCommand sqlcmdCancelPendingReqSusp = new SqlCommand(strCancelPendingReqSuspSQLCmd, cnn);
                sqlcmdCancelPendingReqSusp.Connection.Open();
                int nRet = sqlcmdCancelPendingReqSusp.ExecuteNonQuery();
                sqlcmdCancelPendingReqSusp.Connection.Close();
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
                lbl_tab5_error.Text = ex.Message;
            }

            this.btnPendingACAMRefresh_Click(this, e);
        }


        private delegate void delegateRefreshReqSuspUI(object sender, EventArgs e);
        private void RefreshReqSuspUI(object sender, EventArgs e)
        {
            btnReqSuspRefresh_Click(sender, e);
            btnPendingACAMRefresh_Click(sender, e);
        }
        private void DelegateRefreshReqSuspUI(object sender, EventArgs e)
        {
            Invoke(new delegateRefreshReqSuspUI(RefreshReqSuspUI), sender, e);
        }


        private delegate void delegateRefreshPendingACAMPalletEdit(object sender, EventArgs e);
        private void RefreshPendingACAMPalletEdit(object sender, EventArgs e)
        {
            ListViewItemSelectionChangedEventArgs lstEvntArg = (ListViewItemSelectionChangedEventArgs) e;

            txtboxPendingACAMId.Text        = lstviewPendingACAM.SelectedItems[0].SubItems[0].Text;
            txtboxPendingACAMACAMId.Text    = lstviewPendingACAM.SelectedItems[0].SubItems[1].Text;
            txtboxPendingACAMSuspAmt.Text   = lstviewPendingACAM.SelectedItems[0].SubItems[2].Text;
            txtboxPendingACAMPalletSN.Text  = lstviewPendingACAM.SelectedItems[0].SubItems[3].Text;

        }
        private void DelegateRefreshPendingACAMPalletEdit(object sender, EventArgs e)
        {
            Invoke(new delegateRefreshPendingACAMPalletEdit(RefreshPendingACAMPalletEdit), sender, e);
        }

        private void lstviewPendingACAM_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                Console.WriteLine(e.ItemIndex.ToString());
                DelegateRefreshPendingACAMPalletEdit(sender, e);                
            }
        }

        private void btnPendingACAMIdSave_Click(object sender, EventArgs e)
        {
            try
            {
                System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);

                if (cnn.State != ConnectionState.Open)
                {
                    string strSqlcommand = "";
                    strSqlcommand += "UPDATE tblACAMRequestSusp"
                                + " SET ACAMID = '" + txtboxPendingACAMACAMId.Text + "', SuspAmt = " + txtboxPendingACAMSuspAmt.Text
                                + " WHERE Id = " + txtboxPendingACAMId.Text;


                    System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSqlcommand, cnn);
                    sqlcommand.Connection.Open();
                    int nRet = sqlcommand.ExecuteNonQuery();
                    sqlcommand.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
                lbl_tab5_error.Text = ex.Message;
            }

            this.btnPendingACAMRefresh_Click(sender, e);
        }

    }


    public class LotDetails
    {
        public string SectionNo;
        public string LOT_TYPE;

        public List<SliderSerialNumber> ListOfSliderSerialNumber = new List<SliderSerialNumber>();

    }


    public class SliderSerialNumber
    {
        public string BARLOT { get; set; }
        public string SLDSERIAL { get; set; }
        public string RefBar { get; set; }
        public string RealBar { get; set; }
    }


    public struct RequestProcessRecipeStruct
    {
        public string EquipmentID;
        public string PalletID;
        public string LotNumber;
    }


    public struct LotInformation
    {
        //lstvItem.Text = drow["LOT_NUMBER"].ToString();
        //lstvItem.SubItems.Add(drow["HGA_PART_NUMBER"].ToString());
        //lstvItem.SubItems.Add(drow["PROGRAM"].ToString());
        //lstvItem.SubItems.Add(drow["SUSPENSION"].ToString());
        //lstvItem.SubItems.Add(drow["SUSP_PART_NUMBER"].ToString());
        //lstvItem.SubItems.Add(drow["TYPE"].ToString());
        //lstvItem.SubItems.Add(drow["STR_NO"].ToString());
        //lstvItem.SubItems.Add(drow["LINE_NO"].ToString());
        //lstvItem.SubItems.Add(drow["QTY"].ToString());
        //lstvItem.SubItems.Add(drow["UPDATED_TIMESTAMP"].ToString());
        public string LotNumber;
        public string PartNumber;
        public string Program;
        public string Suspension;
        public string SuspPartNumber;
        public string HGAType;
        public string STR;
        public string Line;
        public int Qty;
        public DateTime UpdateTimeStamp;
    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public class ProcessedPalletDataClass
    {
        private const int SIZE = 10;
        public HGADataClass[] _arrHGA;

        #region ctor
        public ProcessedPalletDataClass()
        {
            _arrHGA = new HGADataClass[SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                _arrHGA[i] = new HGADataClass();
            }

        }
        #endregion

        private string _strPalletID = "";
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; ;}
        }

        private string _strEquipmentID = "";
        public string EquipmentID
        {
            get { return _strEquipmentID; }
            set { _strEquipmentID = value; }
        }

        private string _strLotNumber = "";
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private int _nUVPower = 0;
        public int UVPower
        {
            get { return _nUVPower; }
            set { _nUVPower = value; }
        }

        private double _dblCureTime = 0.0;
        public double CureTime
        {
            get { return _dblCureTime; }
            set { _dblCureTime = value; }
        }

        private int _nCureZone = 0;
        public int CureZone
        {
            get { return _nCureZone; }
            set { _nCureZone = value; }
        }

        private string _strSuspension = "";
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        private string _strSuspTrayID = "";
        public string SuspTrayID
        {
            get { return _strSuspTrayID; }
            set { _strSuspTrayID = value; }
        }

        private int _nSJBLane = 0;
        public int SJBLane
        {
            get { return _nSJBLane; }
            set { _nSJBLane = value; }
        }

        private RequestSuspensionClass _reqSuspObj = new RequestSuspensionClass();
        public RequestSuspensionClass ReqSusp
        {
            get { return _reqSuspObj; }
            set { _reqSuspObj = value; }
        }

    }
    

    // ////////////////////////////////////////////////////////////////////////////////////
    public class HGADataClass
    {
        public string _strOCR = "";
        public List<string> _lstDefects;

        public double _dblxlocACAM = 0.0;
        public double _dblylocACAM = 0.0;
        public double _dblskwACAM = 0.0;

        public double _dblxlocSAI = 0.0;
        public double _dblylocSAI = 0.0;
        public double _dblskwSAI = 0.0;

        #region ctor
        public HGADataClass()
        {
            _strOCR = "";
            _lstDefects = new List<string>();

            _dblxlocACAM = 0.0;
            _dblylocACAM = 0.0;
            _dblskwACAM = 0.0;

            _dblxlocSAI = 0.0;
            _dblylocSAI = 0.0;
            _dblskwSAI = 0.0;
        }
        #endregion

    }
    

    // ////////////////////////////////////////////////////////////////////////////////////
    public enum EQUIPMENT_TYPE
    {
        //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
        //10    , 80    , 30    , 40    , 50    , 60
        NA = 0,
        ASLV = 10,
        ACAM = 20,
        SAI = 25,
        ILC = 30,
        SJB = 40,
        AVI = 50,
        PGM = 51,
        UNOCR = 60,
        FVMI = 70,
        APT = 80,
    }    


    // ////////////////////////////////////////////////////////////////////////////////////
    public class ListViewItemComparer : IComparer
    {
        private int _nCol = 0;
        private System.Windows.Forms.SortOrder _order = System.Windows.Forms.SortOrder.Ascending;

        public ListViewItemComparer()
        {
            _nCol = 0;
        }

        public ListViewItemComparer(int column)
        {
            _nCol = column;
        }

        public ListViewItemComparer(int column, System.Windows.Forms.SortOrder order)
        {
            _nCol = column;
            this._order = order;
        }

        public int Compare(object x, object y)
        {
            int returnVal = -1;

            if (_nCol == 8)             //QTY, compare as integer, not string
            {
                int nX = System.Int32.Parse(((ListViewItem)x).SubItems[_nCol].Text);
                int nY = System.Int32.Parse(((ListViewItem)y).SubItems[_nCol].Text);

                returnVal = nX - nY;
            }
            else if (_nCol == 9)        //compare as datetime
            {
                // Parse the two objects passed as a parameter as a DateTime.
                System.DateTime firstDate = DateTime.Parse(((ListViewItem)x).SubItems[_nCol].Text);
                System.DateTime secondDate = DateTime.Parse(((ListViewItem)y).SubItems[_nCol].Text);
                // Compare the two dates.
                returnVal = DateTime.Compare(firstDate, secondDate);
            }
            else                        //compare as string
            {
                returnVal = String.Compare(((ListViewItem)x).SubItems[_nCol].Text, ((ListViewItem)y).SubItems[_nCol].Text);
            }

            // Determine whether the sort order is descending.
            if (_order == System.Windows.Forms.SortOrder.Descending)
            {
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            }

            return returnVal;
        }
    }


    // ////////////////////////////////////////////////////////////////////////////////////
    #region class LoggerClass

    public class LoggerClass
    {
        private readonly log4net.ILog MainLogger;
        private readonly log4net.ILog ErrorLogger;
        private readonly log4net.ILog MsgLogger;


        private static LoggerClass _instance;
        public static LoggerClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoggerClass();
                }

                return _instance;
            }
        }

        private LoggerClass()
        {
            MainLogger = log4net.LogManager.GetLogger("MainApp");
            ErrorLogger = log4net.LogManager.GetLogger("Error");
            MsgLogger = log4net.LogManager.GetLogger("Message");

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.FileInfo logconfig = new System.IO.FileInfo(exePath + @"\logger.config");
            log4net.Config.XmlConfigurator.Configure(logconfig);
        }

        public void MainLogInfo(string strLogMessage)
        {
            this.MainLogger.Info(strLogMessage);
        }

        public void ErrorLogInfo(string strLogMessage)
        {
            this.ErrorLogger.Info(strLogMessage);
        }

        public void MessageLogInfo(string strLogMessage)
        {
            this.MsgLogger.Info(strLogMessage);
        }

    }

    #endregion


    // ////////////////////////////////////////////////////////////////////////////////////
}