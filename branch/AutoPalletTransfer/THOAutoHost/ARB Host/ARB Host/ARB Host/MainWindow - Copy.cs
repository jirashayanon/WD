using log4net;
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
        public static readonly ILog Log = LogManager.GetLogger("MainWindow");
        List<hostController> ListOfHost = null;
        private HostConfigHelper _hostConfig = new HostConfigHelper();


        public MainWindow()
        {
            InitializeComponent();

            ListOfHost = ToolModel.GetAllHostController();

            lblWebServiceStatus.Text = lblWebServiceStatus.Text + _init();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Log.Info("MainWindow_Load");
            SubScribeAllTool();
            BindToolDetailToGrid();

            PrimaryMessageTextBox.Text = string.Empty;
            SecondaryMessageTextBox.Text = string.Empty;
            ErrorMessageTextBox.Text = string.Empty;

            clearUITextData();

            this.btnRegisterLine_Click(sender, e);
            this.btnGetLots_Click(sender, e);

            _hostConfig = HostConfigHelper.ReadFromFile(exePath + @"\HostConfig.xml");

            //_strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //_strConnectionString += ";Password=qwerty;";

            _strConnectionString = "Data Source=" + _hostConfig.HostDataSource + ((_hostConfig.HostPort != 0 ? ("," + _hostConfig.HostPort.ToString()) : ""));
            _strConnectionString += "; User ID=hostdev;database=" + _hostConfig.HostDatabase;
            _strConnectionString += ";Password=qwerty;";

            //System.IO.File.WriteAllText(exePath + @"\HostConfigxxxx.xml", _hostConfig.ToXML());
            //MessageBox.Show(_strConnectionString);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Info("MainWindow_FormClosing");
            foreach (hostController host in this.ListOfHost)
            {
                host.WDConnectPrimaryIn -= new WDConnectBase.SECsPrimaryInEventHandler(hostController_SECsPrimaryIn);
                host.WDConnectSecondaryIn -= new WDConnectBase.SECsSecondaryInEventHandler(hostController_SECsSecondaryIn);
                host.WDConnectHostError -= new WDConnectBase.SECsHostErrorHandler(hostController_SECsHostError);
            }
        }


        private void SubScribeAllTool()
        {
            Log.Info("SubScribeAllTool");
            foreach (hostController host in this.ListOfHost)
            {
                host.WDConnectPrimaryIn += new WDConnectBase.SECsPrimaryInEventHandler(hostController_SECsPrimaryIn);
                host.WDConnectSecondaryIn += new WDConnectBase.SECsSecondaryInEventHandler(hostController_SECsSecondaryIn);
                host.WDConnectHostError += new WDConnectBase.SECsHostErrorHandler(hostController_SECsHostError);
            }
        }

        private hostController GetHost(string remoteIPAddress, int remotePortNumber)
        {
            Log.Info("GetHost");
            foreach (hostController host in ListOfHost)
            {
                if (host.localIPAddress == remoteIPAddress && host.localPortNumber == remotePortNumber)
                {
                    return host;
                }
            }
            return null;
        }

        private void BindToolDetailToGrid()
        {
            Log.Info("BindToolDetailToGrid");
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
            if (e.Transaction.Primary == null)
            {
                Log.Info("hostController_SECsPrimaryIn: null guard");
                return;
            }

            Log.Info("hostController_SECsPrimaryIn: " + e.Transaction.XMLText);
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
                                this.Invoke(new Action(() =>
                                {
                                    MasterDataGridView.Rows[i].Cells[1].Value = ConnectionStatus.Connected;
                                    MasterDataGridView.Rows[i].Cells[1].Style.BackColor = Color.Green;

                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                    host.ConnectionStatus = ConnectionStatus.Connected;
                                }));
                                break;

                            case "Disconnected":
                                this.Invoke(new Action(() =>
                                {
                                    MasterDataGridView.Rows[i].Cells[1].Value = ConnectionStatus.NotConnected;
                                    MasterDataGridView.Rows[i].Cells[1].Style.BackColor = Color.Red;

                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                    host.ConnectionStatus = ConnectionStatus.NotConnected;
                                }));
                                break;

                      
                            case "AreYouThere":

                                //Console.WriteLine(e.Transaction.XMLText);
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

                                Log.Info("Reply: " + toReplyAreYouThere.XMLText);
                                host.ReplyOutStream(toReplyAreYouThere);

                                break;

                            /*
                            case "TrayCompleted":

                                //Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage secondaryMsgTrayCompletedAck = new SCIMessage();
                                secondaryMsgTrayCompletedAck.CommandID = "TrayCompletedAck";
                                secondaryMsgTrayCompletedAck.Item = new SCIItem();
                                secondaryMsgTrayCompletedAck.Item.Format = SCIFormat.String;
                                secondaryMsgTrayCompletedAck.Item.Value = e.Transaction.Primary.Item.Value;
                                secondaryMsgTrayCompletedAck.Item.Items = new SCIItemCollection();
                                secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });

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

                                Log.Info("Reply: " + toReplyTrayCompletedAck.XMLText);
                                host.ReplyOutStream(toReplyTrayCompletedAck);

                                break;
                             */


                            case "OfflineRequest":

                                //Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                host.ReplyOutStream(e.Transaction);

                                break;


                            case "RequestPalletInfo":
                                {
                                    //Console.WriteLine(e.Transaction.XMLText);
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    PalletToTrayData reqPallet = new PalletToTrayData();
                                    //reqPallet = GetPalletFromXML(e.Transaction.XMLText);
                                    reqPallet = GetPalletFromXML_primary(e.Transaction.XMLText);
                                    Console.WriteLine("{0},{1},{2}", reqPallet.PalletID, reqPallet.EquipmentID, reqPallet.EquipmentType);


                                    // suppose to get pallet info from database
                                    //RequestPalletInfoAckObj reqPalletObj = RequestPalletInfoAckObj.ReadFromFile(@".\Pallet\PT0001.xml");
                                    RequestPalletInfoAckObj reqPalletObj = new RequestPalletInfoAckObj();

                                    if (!_hostConfig.RunWithNoDatabase)
                                    {
                                        //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
                                        //strConnectionString += ";Password=qwerty;";

                                        System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
                                        cnn.Open();

                                        if (cnn.State == ConnectionState.Open)
                                        {
                                            string sqlcommand = "";
                                            sqlcommand += "SELECT TOP(1) *"
                                                        + " FROM [dbo].[tblPalletTransaction] tblPallet"
                                                        + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ reqPallet.PalletID + "'"
                                                        + " ORDER BY tblPallet.CreatedDateTime DESC";


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
                                                break;
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

                                                reqPalletObj.COMMACK = 0;
                                                reqPalletObj.ALMID = 0;
                                                reqPalletObj.EnabledPallet = true;
                                                reqPalletObj.PalletID = reqPallet.PalletID;
                                                reqPalletObj.LotNumber = row[5].ToString();

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
                                            }
                                        }
                                    }
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

                                            //return;
                                        }
                                    }



                                    SCIMessage scndMsgRequestPalletInfo = new SCIMessage();
                                    scndMsgRequestPalletInfo.CommandID = "RequestPalletInfoAck";
                                    scndMsgRequestPalletInfo.Item = new SCIItem();
                                    scndMsgRequestPalletInfo.Item.Format = SCIFormat.List;
                                    scndMsgRequestPalletInfo.Item.Items = new SCIItemCollection();

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = reqPalletObj.COMMACK });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = reqPalletObj.ALMID });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPalletObj.PalletID });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = reqPalletObj.LotNumber });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = reqPalletObj.Line });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = reqPalletObj.ACAMID });

                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = reqPalletObj.EquipmentType });
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "NextEquipmentType", Value = reqPalletObj.NextEquipmentType });


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
                                        hgaItem.Name = "HGA" + j.ToString();
                                        hgaItem.Value = "";
                                        hgaItem.Items = new SCIItemCollection();
                                        //hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = GenSerialNumberFromLot(strLot) });
                                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = hgas[j].SN });
                                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = hgas[j].Defect });

                                        //add HGA1 - HGA10
                                        hgaListItem.Items.Add(hgaItem);
                                    }


                                    //hga data portion


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
                                    Log.Info("Reply: " + toReplyRequestPalletInfo.XMLText);
                                    host.ReplyOutStream(toReplyRequestPalletInfo);

                                    break;
                                }


                            case "SendPalletInfo":
                                {
                                    //Console.WriteLine(e.Transaction.XMLText);
                                    host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                    ProcessedPalletDataClass sendProcessedPalletInfoData = new ProcessedPalletDataClass();
                                    sendProcessedPalletInfoData = GetProcessedPalletFromXML_primary(e.Transaction.XMLText);

                                    WDConnect.Common.SCITransaction transObj;
                                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(WDConnect.Common.SCITransaction));
                                    using (StringReader reader = new StringReader(e.Transaction.XMLText))
                                    {
                                        transObj = (WDConnect.Common.SCITransaction)x.Deserialize(reader);
                                    }

                                    SendPalletInfoObj sndPalletObj = new SendPalletInfoObj();
                                    sndPalletObj = SendPalletInfoObj.ToObj(e.Transaction);




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
                                                        + " ORDER BY tblPallet.CreatedDateTime DESC";


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
                                                scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });
                                                scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });


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

                                                Log.Info("Reply: " + replyNOKPalletInfoAck.XMLText);
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
                                                    MessageBox.Show("Please check NextEquipmentType");

                                                    SCIMessage scndMsgNOKPalletInfoAck = new SCIMessage();
                                                    scndMsgNOKPalletInfoAck.CommandID = "SendPalletInfoAck";
                                                    scndMsgNOKPalletInfoAck.Item = new SCIItem();
                                                    scndMsgNOKPalletInfoAck.Item.Format = SCIFormat.List;
                                                    scndMsgNOKPalletInfoAck.Item.Items = new SCIItemCollection();
                                                    scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });
                                                    scndMsgNOKPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });


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

                                                    Log.Info("Reply: " + replyNOKPalletInfoAck.XMLText);
                                                    host.ReplyOutStream(replyNOKPalletInfoAck);

                                                    break;
                                                }


                                                string strSQLCmd = "";


                                                #region region switch
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


                                                    //if nextequpimenttype = 30, 60; ILC, UNLOAD
                                                    //      call spUpdatePalletTransactionNoLotNoSN
                                                    case "30":
                                                    case "60":
                                                        //EXEC [spUpdatePalletTransactionNoLotNoSN] 3, 'PT0001', 20, 30
                                                        strSQLCmd += "EXEC [spUpdatePalletTransactionNoLotNoSN] " + /*transID*/ nTransID.ToString() + ",'" + sndPalletObj.PalletID + "',";
                                                        strSQLCmd += sndPalletObj.EquipmentType + "," + GetNextEquipmentType(sndPalletObj.EquipmentType);

                                                        break;


                                                    //if nextequipmenttype = 50; AVI
                                                    //      call spUpdatePalletTransactionSN
                                                    case "50":
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
                                                        strSQLCmd += "'" + sndPalletObj.HGA.HGA10.SN + "'," + "'" + sndPalletObj.HGA.HGA10.Defect + "'";
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


                                                System.Data.SqlClient.SqlCommand sqlcommand = new SqlCommand(strSQLCmd, cnn);
                                                sqlcommand.Connection.Open();
                                                int nRet = sqlcommand.ExecuteNonQuery();
                                                sqlcommand.Connection.Close();
                                            
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                        }


                                    //





                                        //replay SCIMessage to tool
                                        //
                                        SCIMessage secondaryMsgSendProcessedPalletInfoAck = new SCIMessage();
                                        secondaryMsgSendProcessedPalletInfoAck.CommandID = "SendPalletInfoAck";
                                        secondaryMsgSendProcessedPalletInfoAck.Item = new SCIItem();
                                        secondaryMsgSendProcessedPalletInfoAck.Item.Format = SCIFormat.List;
                                        secondaryMsgSendProcessedPalletInfoAck.Item.Items = new SCIItemCollection();
                                        secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });
                                        secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


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

                                        Log.Info("Reply: " + toReplySendProcessedPalletInfoAck.XMLText);
                                        host.ReplyOutStream(toReplySendProcessedPalletInfoAck);
                                    } 


                                    break;
                                }


                            /*
                            PalletToTrayData sendPalletInfoData = new PalletToTrayData();
                            sendPalletInfoData = GetPalletFromXML_primary(e.Transaction.XMLText);


                            SCIMessage secondaryMsgSendPalletInfoAck = new SCIMessage();
                            secondaryMsgSendPalletInfoAck.CommandID = "SendPalletInfoAck";
                            secondaryMsgSendPalletInfoAck.Item = new SCIItem();
                            secondaryMsgSendPalletInfoAck.Item.Format = SCIFormat.List;
                            secondaryMsgSendPalletInfoAck.Item.Items = new SCIItemCollection();
                            secondaryMsgSendPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendPalletInfoData.PalletID });
                            secondaryMsgSendPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                            WDConnect.Common.SCITransaction toReplySendPalletInfoAck = new WDConnect.Common.SCITransaction()
                            {
                                DeviceId = e.Transaction.DeviceId,
                                MessageType = MessageType.Secondary,
                                Id = e.Transaction.Id,
                                Name = "SendPalletInfoAck",
                                NeedReply = false,
                                Primary = e.Transaction.Primary,
                                Secondary = secondaryMsgSendPalletInfoAck
                            };

                            Log.Info("Reply: " + toReplySendPalletInfoAck.XMLText);
                            host.ReplyOutStream(toReplySendPalletInfoAck);

                            break;
                            */



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


                            /*
                            case "UnloadPalletToTray":

                                //Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                PalletToTrayData unloadPallet = new PalletToTrayData();
                                unloadPallet = GetPalletFromXML_primary(e.Transaction.XMLText);


                                SCIMessage secondaryMsgUnloadPalletAck = new SCIMessage();
                                secondaryMsgUnloadPalletAck.CommandID = "UnloadPalletToTrayAck";
                                secondaryMsgUnloadPalletAck.Item = new SCIItem();
                                secondaryMsgUnloadPalletAck.Item.Format = SCIFormat.List;
                                secondaryMsgUnloadPalletAck.Item.Items = new SCIItemCollection();
                                secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = unloadPallet.PalletID });
                                secondaryMsgUnloadPalletAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


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

                                Log.Info("Reply: " + toReplyUnloadPalletAck.XMLText);
                                host.ReplyOutStream(toReplyUnloadPalletAck);

                                break;
                            */


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
                                Log.Info("Reply: " + toReplyAlarmReportSend.XMLText);
                                host.ReplyOutStream(toReplyAlarmReportSend);

                                break;



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
                                Log.Info("Reply: " + toReplySendControlStateAck.XMLText);
                                host.ReplyOutStream(toReplySendControlStateAck);
                                break;


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
                                Log.Info("Reply: " + toReplySendProcessStateAck.XMLText);
                                host.ReplyOutStream(toReplySendProcessStateAck);
                                break;



                            case "RequestProcessRecipe":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                RequestProcessRecipeStruct reqProcRecipeObj = GetRequestProcessRecipeFromXML_primary(e.Transaction.XMLText);

                                RequestProcessRecipeAckObj recipe = null;
                                try
                                {
                                    recipe = mappingProductReqRecipeObj[mappingLotNumberProduct[reqProcRecipeObj.LotNumber]];
                                }
                                catch (Exception ex)
                                {
                                    if (recipe == null)
                                    {
                                        MessageBox.Show("null");
                                        recipe = new RequestProcessRecipeAckObj();
                                        recipe.COMMACK = 1;
                                    }
                                }

                                SCIMessage scndMsgReqProcessRecipeAck = new SCIMessage();
                                scndMsgReqProcessRecipeAck.CommandID = "RequestProcessRecipeAck";
                                scndMsgReqProcessRecipeAck.Item = new SCIItem();
                                scndMsgReqProcessRecipeAck.Item.Format = SCIFormat.List;
                                scndMsgReqProcessRecipeAck.Item.Items = new SCIItemCollection();

                                //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = recipe.COMMACK });
                                //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = recipe.ALMID });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqProcRecipeObj.PalletID });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = reqProcRecipeObj.LotNumber });
                                //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeID", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeID", Value = recipe.RecipeID });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeName", Value = "PALMER_8_SD" });

                                //scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B401" });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = recipe.Line });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "UVPower", Value = 200 });        //obsolete
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime", Value = 15.5 });        //obsolete
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "CureZone", Value = 0 });         //obsolete


                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV1", Value =         recipe.PowerUV1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV1", Value =        recipe.CureTimeUV1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV1", Value =       recipe.EnabledUV1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater1", Value =    recipe.FlowRateHeater1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater1", Value =      recipe.TempHeater1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater1", Value =   recipe.EnabledHeater1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater1", Value = recipe.EnabledN2Heater1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater2", Value =    recipe.FlowRateHeater2 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater2", Value =      recipe.TempHeater2 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater2", Value =   recipe.EnabledHeater2 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater2", Value = recipe.EnabledN2Heater2 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater3", Value =    recipe.FlowRateHeater3 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater3", Value =      recipe.TempHeater3 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater3", Value =   recipe.EnabledHeater3 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater3", Value = recipe.EnabledN2Heater3 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater4", Value =    recipe.FlowRateHeater4 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater4", Value =      recipe.TempHeater4 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater4", Value =   recipe.EnabledHeater4 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater4", Value = recipe.EnabledN2Heater4 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater5", Value =    recipe.FlowRateHeater5 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater5", Value =      recipe.TempHeater5 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater5", Value =   recipe.EnabledHeater5 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater5", Value = recipe.EnabledN2Heater5 });


                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV2", Value =         recipe.PowerUV2 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV2", Value =        recipe.CureTimeUV2 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV2", Value =       recipe.EnabledUV2 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater6", Value =    recipe.FlowRateHeater6 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater6", Value =      recipe.TempHeater6 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater6", Value =   recipe.EnabledHeater6 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater6", Value = recipe.EnabledN2Heater6 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater7", Value =    recipe.FlowRateHeater7 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater7", Value =      recipe.TempHeater7 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater7", Value =   recipe.EnabledHeater7 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater7", Value = recipe.EnabledN2Heater7 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater8", Value =    recipe.FlowRateHeater8 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater8", Value =      recipe.TempHeater8 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater8", Value =   recipe.EnabledHeater8 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater8", Value = recipe.EnabledN2Heater8 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater9", Value =    recipe.FlowRateHeater9 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater9", Value =      recipe.TempHeater9 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater9", Value =   recipe.EnabledHeater9 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater9", Value = recipe.EnabledN2Heater9 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater10", Value =       recipe.FlowRateHeater10 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater10", Value =         recipe.TempHeater10 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater10", Value =      recipe.EnabledHeater10 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater10", Value =    recipe.EnabledN2Heater10 });


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
                                Log.Info("Reply: " + toReplyReqProcessRecipe.XMLText);
                                host.ReplyOutStream(toReplyReqProcessRecipe);

                                break;
                                



                            case "SendPalletStatus":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                PalletToTrayData sendPalletStatusData = new PalletToTrayData();
                                sendPalletStatusData = GetPalletFromXML_primary(e.Transaction.XMLText);


                                SCIMessage secondaryMsgSendPalletStatusAck = new SCIMessage();
                                secondaryMsgSendPalletStatusAck.CommandID = "SendPalletStatusAck";
                                secondaryMsgSendPalletStatusAck.Item = new SCIItem();
                                secondaryMsgSendPalletStatusAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendPalletStatusAck.Item.Items = new SCIItemCollection();
                                secondaryMsgSendPalletStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendPalletStatusData.PalletID });
                                secondaryMsgSendPalletStatusAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


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

                                Log.Info("Reply: " + toReplySendPalletStatusAck.XMLText);
                                host.ReplyOutStream(toReplySendPalletStatusAck);

                                break;


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

                                Log.Info("Reply: " + toReplySendAlarmReportAck.XMLText);
                                host.ReplyOutStream(toReplySendAlarmReportAck);

                                break;


                            default:
                                break;

                        }

                    }

                }

            }

            System.Threading.Thread.Sleep(500);
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
                case 10:
                    strNextEquipmentType = "80";
                    break;

                case 20:
                    strNextEquipmentType = "30";
                    break;

                case 30:
                    strNextEquipmentType = "40";
                    break;

                case 40:
                    strNextEquipmentType = "50";
                    break;

                case 50:
                    strNextEquipmentType = "60";
                    break;

                case 60:
                    strNextEquipmentType = "10";
                    break;

                case 70:
                    break;

                case 80:
                    strNextEquipmentType = "30";
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
            Log.Info("CreateXMLText");
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
            if (e.Transaction.Secondary == null)
            {
                Log.Info("hostController_SECsSecondaryIn: null guard");
                return;
            }

            Log.Info("hostController_SECsSecondaryIn: " + e.Transaction.XMLText);
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

                            Log.Info("Reply: " + toReplySendPalletInfo.XMLText);
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
                                Log.Info("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.PalletEnabled.ToString());
                            }
                        }
                    }

                }
            }

            return palletObj;
        }


        private PalletToTrayData GetPalletFromXML_primary(string xmlPallet)
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
                        if (nameChildNode.InnerText == "EquipmentType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.EquipmentType = valueChildNode.InnerText;
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
                                Log.Info("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.PalletEnabled.ToString());
                            }
                        }
                    }

                }
            }

            return palletObj;
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
                                Log.Info("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.PalletEnabled.ToString());
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

                }


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
                                if (hgaNameNodeList[i].InnerText == "SN")
                                {
                                    palletObj._arrHGA[nHGAPos - 1]._strOCR = hgaValueNodeList[i].InnerText;
                                }
                                else if (hgaNameNodeList[i].InnerText == "Defect")
                                {
                                    if (hgaValueNodeList[i].InnerText.Length > 0)
                                    {
                                        palletObj._arrHGA[nHGAPos - 1]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                    }
                                }
                            }

                        }


                    }

                }
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
            Log.Info("hostController_SECsHostError: " + e.Message);
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
            Log.Info("MasterDataGridView_CellMouseClick");
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                PrimaryMessageTextBox.Text = MasterDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
                SecondaryMessageTextBox.Text = MasterDataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
        }


        private void AreYouThere_Click(object sender, EventArgs e)
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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);
                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message + "," + ex.StackTrace);
            }
        }


        private void RequestControlState_Click(object sender, EventArgs e)
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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
            }
        }


        private void RequestProcessState_Click(object sender, EventArgs e)
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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
            }
        }


        private void OnlineRequest_Click(object sender, EventArgs e)
        {
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }


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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
            }

        }


        private void OfflineRequest_Click(object sender, EventArgs e)
        {
            hostController host = new hostController();
            foreach (hostController host1 in this.ListOfHost)
            {
                if (host1.ConnectionStatus == ConnectionStatus.Connected)
                {
                    host = host1;
                }
            }


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
                Log.Info("null guard: host == null");
                return;
            }

            try
            {
                Log.Info("host.SendMessage: " + trans.XMLText);
                host.SendMessage(trans);

                DelegateSetErrorTextBoxMsg("");
            }
            catch (Exception ex)
            {
                DelegateSetErrorTextBoxMsg(ex.Message);
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
                //DelegateUpdateTrayHistory(lstItem);
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

                return client.getWebServiceVersion();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        private void btnRegisterLine_Click(object sender, EventArgs e)
        {
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
            }
            else
            {
                txtboxAPIKey.Text = msg;
            }
        }

        private void btnGetLots_Click(object sender, EventArgs e)
        {
            this.btnClearRecipe_Click(sender, e);

            string msg = string.Empty;
            DataSet dsLotNumbers;
            if (txtboxHGAPartNumber.Text.Length > 0)
            {
                dsLotNumbers = mitecs3service.GetLotNumbersByLinePartNo(txtboxLine.Text, txtboxHGAPartNumber.Text, out msg);
            }
            else
            {
                dsLotNumbers = mitecs3service.GetLotNumbers(txtboxLine.Text, out msg);
            }

            lstviewLotDetails.Items.Clear();

            //1* //Already get lots from MITECS
            DataTable dtable = dsLotNumbers.Tables[0];

            //save datatable to file
            dtable.WriteXml("mitecs_lot.xml", XmlWriteMode.WriteSchema);

            //2* //Create recipe based on products, assume 1 product 1 recipe
            mappingProductReqRecipeObj.Clear();
            mappingLotNumberProduct.Clear();

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


                    RequestProcessRecipeAckObj recipe = new RequestProcessRecipeAckObj();
                    recipe.RecipeID = (mappingLotNumberReqRecipeObj.Count + 1).ToString();

                    recipe.LotNumber = drow["LOT_NUMBER"].ToString();
                    recipe.PartNumber = drow["HGA_PART_NUMBER"].ToString();
                    recipe.ProductName = drow["PROGRAM"].ToString();
                    recipe.Suspension = drow["SUSPENSION"].ToString();
                    recipe.SuspPartNumber = drow["SUSP_PART_NUMBER"].ToString();
                    recipe.HGAType = drow["TYPE"].ToString();
                    recipe.STR = drow["STR_NO"].ToString();
                    recipe.Line = drow["LINE_NO"].ToString();
                    recipe.LotQty = Int32.Parse(drow["QTY"].ToString());


                    try
                    {
                        //mappingLotNumberReqRecipeObj.Add(drow["LOT_NUMBER"].ToString(), recipe);
                        mappingProductReqRecipeObj.Add(drow["PROGRAM"].ToString(), recipe);             //create recipe list based on products
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        mappingLotNumberProduct.Add(recipe.LotNumber, recipe.ProductName);              //get product from lot: mappingLotNumberProduct[recipe.LotNumber]
                        //get recipe from lot: mappingProductReqRecipeObj[ mappingLotNumberProduct[recipe.LotNumber] ]
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }



            //Generate recipe information
            //foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingLotNumberReqRecipeObj)
            foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            {
                Console.WriteLine(entry.Key.ToString());
                Console.WriteLine(((RequestProcessRecipeAckObj)entry.Value).RecipeID);

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



            ///
            //Generate recipe information
            foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            {
                Console.WriteLine(entry.Key.ToString());
                Console.WriteLine(((RequestProcessRecipeAckObj)entry.Value).RecipeID);

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

                //save recipe to local
                string strRecipePath = exePath + @"\recipe";
                if (!System.IO.Directory.Exists(strRecipePath))
                {
                    System.IO.Directory.CreateDirectory(strRecipePath);
                }
                System.IO.File.WriteAllText(strRecipePath + @"\recipe" +  recipe.RecipeID + @".xml", recipe.ToXML());
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
        }

        
        private void btn_tab3_SaveLocal_Click(object sender, EventArgs e)
        {
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
        }

        private Dictionary<string, RequestProcessRecipeAckObj> mappingProductReqRecipeObj = new Dictionary<string, RequestProcessRecipeAckObj>();
        private Dictionary<string, string> mappingLotNumberProduct = new Dictionary<string, string>();
        private Dictionary<string, RequestProcessRecipeAckObj> mappingLotNumberReqRecipeObj = new Dictionary<string, RequestProcessRecipeAckObj>();
        private void btnGetLotsOffline_Click(object sender, EventArgs e)
        {
            this.btnClearRecipe_Click(sender, e);

            lstviewLotDetails.Items.Clear();
            mappingLotNumberReqRecipeObj.Clear();

            //1* //Already get lots from MITECS
            DataTable dtable = new DataTable();     //real scenario; this lot data table will contain informaiton from MITECS
            dtable.ReadXml("mitecs_lot.xml");       //offline test, load lot information from file


            //2* //Create recipe based on products, assume 1 product 1 recipe
            mappingProductReqRecipeObj.Clear();
            mappingLotNumberProduct.Clear();

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


                    RequestProcessRecipeAckObj recipe = new RequestProcessRecipeAckObj();
                    recipe.RecipeID = (mappingLotNumberReqRecipeObj.Count + 1).ToString();

                    recipe.LotNumber = drow["LOT_NUMBER"].ToString();
                    recipe.PartNumber = drow["HGA_PART_NUMBER"].ToString();
                    recipe.ProductName = drow["PROGRAM"].ToString();
                    recipe.Suspension = drow["SUSPENSION"].ToString();
                    recipe.SuspPartNumber = drow["SUSP_PART_NUMBER"].ToString();
                    recipe.HGAType = drow["TYPE"].ToString();
                    recipe.STR = drow["STR_NO"].ToString();
                    recipe.Line = drow["LINE_NO"].ToString();
                    recipe.LotQty = Int32.Parse(drow["QTY"].ToString());


                    try
                    {
                        //mappingLotNumberReqRecipeObj.Add(drow["LOT_NUMBER"].ToString(), recipe);
                        mappingProductReqRecipeObj.Add(drow["PROGRAM"].ToString(), recipe);             //create recipe list based on products
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        mappingLotNumberProduct.Add(recipe.LotNumber, recipe.ProductName);              //get product from lot: mappingLotNumberProduct[recipe.LotNumber]
                        //get recipe from lot: mappingProductReqRecipeObj[ mappingLotNumberProduct[recipe.LotNumber] ]
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }



            //Generate recipe information
            //foreach (KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingLotNumberReqRecipeObj)
            foreach(KeyValuePair<string, RequestProcessRecipeAckObj> entry in mappingProductReqRecipeObj)
            {
                Console.WriteLine(entry.Key.ToString());
                Console.WriteLine(((RequestProcessRecipeAckObj) entry.Value).RecipeID);

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

        private void button7_Click(object sender, EventArgs e)
        {
            string temp =   mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].RecipeID + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].ProductName + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].PalletID + ";\r\n" +
                            //mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].LotNumber + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].LotQty + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].Line + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].PartNumber + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].Suspension + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].SuspPartNumber + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].HGAType + ";\r\n" +
                            mappingProductReqRecipeObj[mappingLotNumberProduct[@"WRXGC_A"]].PowerUV1 + ";\r\n";


            MessageBox.Show(temp);
        }


        private void clearUITextData()
        {
            lbl_tab3_error.Text = "";

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
        }

        private void btn_tab3_ReqPalletInfo_Click(object sender, EventArgs e)
        {
            lbl_tab3_error.Text = "";

            if (txtbox_tab3_PalletID.Text.Length < 1)
            {
                return;
            }

            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            cnn.Open();

            if (cnn.State == ConnectionState.Open)
            {
                string sqlcommand = "";
                sqlcommand += "SELECT TOP(1) *"
                            + " FROM [dbo].[tblPalletTransaction] tblPallet"
                            + " WHERE tblPallet.PalletID = '" + /*"PT0001"*/ txtbox_tab3_PalletID.Text + "'"
                            + " ORDER BY tblPallet.CreatedDateTime DESC";


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

                    txtbox_tab3_COMMACK.Text = "0";
                    txtbox_tab3_ALMID.Text = "0";
                    radioButton_EnabledPallet.Checked = true;

                    txtbox_tab3_transID.Text = row[0].ToString();

                    int nEquipmentType = Int32.Parse(row[2].ToString());
                    int nNextEquipmentType = Int32.Parse(row[3].ToString());
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
                        txtbox_tab3_PartNumber.Text = mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].PartNumber;
                        txtbox_tab3_ProductName.Text = mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].ProductName;
                        txtbox_tab3_Line.Text = mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].Line;
                        txtbox_tab3_ACAMID.Text = row[26].ToString();
                        txtbox_tab3_Suspension.Text = mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].Suspension;
                        //mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].SuspPartNumber;
                        txtbox_tab3_HGAType.Text = mappingProductReqRecipeObj[mappingLotNumberProduct[strFindInfoFromLot]].HGAType;


                        //reqPalletObj.EnabledPallet = true;
                        //reqPalletObj.PalletID = reqPallet.PalletID;
                        txtbox_tab3_LotNumber.Text = row[5].ToString();

                        txtbox_tab3_SN1.Text = row[6].ToString();
                        txtbox_tab3_Defect1.Text = row[7].ToString();

                        txtbox_tab3_SN2.Text = row[8].ToString();
                        txtbox_tab3_Defect2.Text = row[9].ToString();

                        txtbox_tab3_SN3.Text = row[10].ToString();
                        txtbox_tab3_Defect3.Text = row[11].ToString();

                        txtbox_tab3_SN4.Text = row[12].ToString();
                        txtbox_tab3_Defect4.Text = row[13].ToString();

                        txtbox_tab3_SN5.Text = row[14].ToString();
                        txtbox_tab3_Defect5.Text = row[15].ToString();


                        txtbox_tab3_SN6.Text = row[16].ToString();
                        txtbox_tab3_Defect6.Text = row[17].ToString();

                        txtbox_tab3_SN7.Text = row[18].ToString();
                        txtbox_tab3_Defect7.Text = row[19].ToString();

                        txtbox_tab3_SN8.Text = row[20].ToString();
                        txtbox_tab3_Defect8.Text = row[21].ToString();

                        txtbox_tab3_SN9.Text = row[22].ToString();
                        txtbox_tab3_Defect9.Text = row[23].ToString();

                        txtbox_tab3_SN10.Text = row[24].ToString();
                        txtbox_tab3_Defect10.Text = row[25].ToString();
                    }
                    catch (Exception ex)
                    {
                        lbl_tab3_error.Text = ex.Message;
                        return;
                    }
                }
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
            }
            catch (Exception ex)
            {
                lbl_tab3_error.Text = ex.Message;
                return;
            }


            //string strConnectionString = "Data Source=172.16.160.122\\SQLEXPRESS; User ID=hostdev;database=TempPalletTrackingDB";
            //strConnectionString += ";Password=qwerty;";

            System.Data.SqlClient.SqlConnection cnn = new SqlConnection(_strConnectionString);
            string strSQLCmd = string.Empty;

            //EXEC [spUpdatePalletTransaction] 3, 'PT0001', 20, 30, 'WYKU7_AB2', 'WYKU871H','A1,WO', 'WYKU871C','A1,WO', 'WYKU7692','A1,WO', 'WYKU769X','A1,WO', 'WYKU769W','A1,WO', 'WYKU769V','A1,WO', 'WYKU769R','A1,WO', 'WYKU769P','A1,WO', 'WYKU771P','A1,WO', 'WYKU771K','A1,WO'
            strSQLCmd += "EXEC [spUpdatePalletTransaction] " + /*transID*/ txtbox_tab3_transID.Text + ",'" + sndPalletObj.PalletID + "',";
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
            }

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

    public class BARLOT_NO
    {
        public string barlot_no { get; set; }
        public int index { get; set; }
    }


    public struct RequestProcessRecipeStruct
    {
        public string EquipmentID;
        public string PalletID;
        public string LotNumber;
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

    }
    

    // ////////////////////////////////////////////////////////////////////////////////////
    public class HGADataClass
    {
        public string _strOCR = "";
        public List<string> _lstDefects;

        #region ctor
        public HGADataClass()
        {
            _strOCR = "";
            _lstDefects = new List<string>();
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
        ILC = 30,
        SJB = 40,
        AVI = 50,
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
}
