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

namespace ARB_Host
{
    public partial class MainWindow : Form
    {
        public static readonly ILog Log = LogManager.GetLogger("MainWindow");

        List<hostController> ListOfHost = null;

        string Operations = string.Empty;
        public MainWindow()
        {
            InitializeComponent();

            ListOfHost = ToolModel.GetAllHostController();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Log.Info("MainWindow_Load");
            SubScribeAllTool();
            BindToolDetailToGrid();

            
            //MitecsFunction.M3 = null;
            //MitecsFunction.M3 = new Mitecs3Data();
            //string returnMsg = string.Empty;
            //Operations = AppConfig.GetString("appSettings", "Operations", string.Empty);
            //MitecsFunction.M3.Register(Operations, "D00", out returnMsg);
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

                                Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage secondaryMsgAreYouThere = new SCIMessage();
                                secondaryMsgAreYouThere.CommandID = "AreYouThereAck";
                                secondaryMsgAreYouThere.Item = new SCIItem();
                                secondaryMsgAreYouThere.Item.Format = SCIFormat.String;
                                secondaryMsgAreYouThere.Item.Value = e.Transaction.Primary.Item.Value;
                                secondaryMsgAreYouThere.Item.Items = new SCIItemCollection();
                                secondaryMsgAreYouThere.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SoftwareRevision", Value = "1.0.0" });

                                SCITransaction toReplyAreYouThere = new SCITransaction()
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


                            case "TrayCompleted":

                                Console.WriteLine(e.Transaction.XMLText);
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage secondaryMsgTrayCompletedAck = new SCIMessage();
                                secondaryMsgTrayCompletedAck.CommandID = "TrayCompletedAck";
                                secondaryMsgTrayCompletedAck.Item = new SCIItem();
                                secondaryMsgTrayCompletedAck.Item.Format = SCIFormat.String;
                                secondaryMsgTrayCompletedAck.Item.Value = e.Transaction.Primary.Item.Value;
                                secondaryMsgTrayCompletedAck.Item.Items = new SCIItemCollection();
                                secondaryMsgTrayCompletedAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });

                                SCITransaction toReplyTrayCompletedAck = new SCITransaction()
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


                            case "OfflineRequest":
                                Console.WriteLine(e.Transaction.XMLText);

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);
                                host.ReplyOutStream(e.Transaction);

                                break;


                            case "RequestPalletInfo":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                PalletToTrayData reqPallet = new PalletToTrayData();
                                //reqPallet = GetPalletFromXML(e.Transaction.XMLText);
                                reqPallet = GetPalletFromXML_primary(e.Transaction.XMLText);

                                SCIMessage scndMsgRequestPalletInfo = new SCIMessage();
                                scndMsgRequestPalletInfo.CommandID = "RequestPalletInfoAck";
                                scndMsgRequestPalletInfo.Item = new SCIItem();
                                scndMsgRequestPalletInfo.Item.Format = SCIFormat.List;
                                scndMsgRequestPalletInfo.Item.Items = new SCIItemCollection();

                                scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
                                scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPallet.PalletID });


                                string strLot = "";
                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = "WD84J_K" });
                                    Console.WriteLine(DateTime.Now.ToString() + ": WD84J_K");
                                    strLot = "WD84J_K";
                                }
                                else
                                {
                                    scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = "Y13X5_A" });
                                    Console.WriteLine(DateTime.Now.ToString() + ": WD84Z_A");
                                    strLot = "Y13X5_A";
                                }

                                scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B403" });
                                scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = "ACAM001" });


                                //hga data portion
                                SCIItem hgaListItem = new SCIItem();
                                hgaListItem.Format = SCIFormat.List;
                                hgaListItem.Name = "HGA";
                                hgaListItem.Items = new SCIItemCollection();

                                scndMsgRequestPalletInfo.Item.Items.Add(hgaListItem);

                                for (int j = 0; j < 10; j++)
                                {
                                    //if ((i == 2) || (i == 5) || (i == 6) || (i == 8))
                                    //{
                                    //    continue;
                                    //}

                                    SCIItem hgaItem = new SCIItem();
                                    hgaItem.Format = SCIFormat.List;
                                    hgaItem.Name = "HGA" + j.ToString();
                                    hgaItem.Value = "";
                                    hgaItem.Items = new SCIItemCollection();
                                    hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = GenSerialNumberFromLot(strLot) });
                                    hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = "" });

                                    //add HGA1 - HGA10
                                    hgaListItem.Items.Add(hgaItem);
                                }


                                //hga data portion


                                SCITransaction toReplyRequestPalletInfo = new SCITransaction()
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

                                //-------------------------------------------------------------------
                                System.Threading.Thread.Sleep(100);

                                /*  //obsolete
                                SCIMessage primMsgSendPalletEnable = new SCIMessage();
                                primMsgSendPalletEnable.CommandID = "SendPalletEnable";
                                primMsgSendPalletEnable.Item = new SCIItem();
                                primMsgSendPalletEnable.Item.Format = SCIFormat.List;
                                primMsgSendPalletEnable.Item.Items = new SCIItemCollection();
                                primMsgSendPalletEnable.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPallet.PalletID });
                                primMsgSendPalletEnable.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnableCode", Value = 0 }); //0=enabled, 1=disabled

                                SCITransaction toReplySendPalletEnable = new SCITransaction()
                                {
                                    DeviceId = 1,
                                    MessageType = MessageType.Primary,
                                    Id = 1,
                                    Name = "SendPalletEnable",
                                    NeedReply = true,
                                    Primary = primMsgSendPalletEnable,
                                    Secondary = null
                                };
                                */


                                SCIMessage primMsgSendPalletInfo = new SCIMessage();
                                primMsgSendPalletInfo.CommandID = "SendPalletInfo";
                                primMsgSendPalletInfo.Item = new SCIItem();
                                primMsgSendPalletInfo.Item.Format = SCIFormat.List;
                                primMsgSendPalletInfo.Item.Items = new SCIItemCollection();
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqPallet.PalletID });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumber", Value = "69885-15-SAA" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProductName", Value = "FIREBIRD(E8)" });
                                //primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "WKU8A_A" });

                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    //primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "T1ELA_D" });
                                    //Console.WriteLine(DateTime.Now.ToString() + ": T1ELA_D");
                                    primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "WD84J_K" });
                                    Console.WriteLine(DateTime.Now.ToString() + ": WD84J_K");
                                }
                                else
                                {
                                    //primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "T39JE_A" });
                                    //Console.WriteLine(DateTime.Now.ToString() + ": T39JE_A");

                                    primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGALotNo", Value = "WD84Z_A" });
                                    Console.WriteLine(DateTime.Now.ToString() + ": WD84Z_A");
                                }

                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STRNumber", Value = "69885-15-SAA" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B403" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SliderAttachMachine", Value = "ACAM001" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = "HTO-T5" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAType", Value = "A" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "AllowMix", Value = false });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnableCode", Value = 0 });  //0 = enabled, 1 = disabled

                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProductName", Value = "FIREBIRD(E8)" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumberForSHOST", Value = "PN#69885-15-SAA" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STRNumberForSHOST", Value = "STR#69885-15-SAA" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LineForSHOST", Value = "B40309" });

                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP1", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP2", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP3", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP4", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP5", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP6", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP7", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP8", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP9", Value = "" });
                                primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "DefectP10", Value = "" });


                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 2) == 0)
                                {
                                    primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EndLotStatus", Value = false });
                                }
                                else
                                {
                                    primMsgSendPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EndLotStatus", Value = false });
                                }



                                SCITransaction toReplySendPalletInfo = new SCITransaction()
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


                                //*****************
                                //Log.Info("Reply: " + toReplySendPalletEnable.XMLText);
                                //host.ReplyOutStream(toReplySendPalletEnable);

                                break;


                            case "SendStatus":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendStatus = new SCIMessage();
                                scndMsgSendStatus.CommandID = "SendStatusAck";
                                scndMsgSendStatus.Item = new SCIItem();
                                scndMsgSendStatus.Item.Format = SCIFormat.List;
                                scndMsgSendStatus.Item.Items = new SCIItemCollection();
                                scndMsgSendStatus.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                SCITransaction toReplySendStatus = new SCITransaction()
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


                            case "UnloadPalletToTray":

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


                                SCITransaction toReplyUnloadPalletAck = new SCITransaction()
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


                            case "GetTrayInfo":

                                //System.Threading.Thread.Sleep(3000);

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



                                SCITransaction toReplyGetTrayInfoAck = new SCITransaction()
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



                            case "UpdateTrayProcess":

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

                                SCITransaction toReplyUpdateTrayProcessAck = new SCITransaction()
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


                            case "SendTrayStatus":
                                for(int ii = 0; ii < 5 ; ii++)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    Console.WriteLine("SendTrayStatus: sleep");
                                }


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

                                SCITransaction toReplySendTrayStatusAck = new SCITransaction()
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


                            case "RequestUnloadTray":

                                Console.WriteLine("RequestUnloadTray: sleep");

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


                                SCITransaction toReplyRequestUnloadTrayAck = new SCITransaction()
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


                            case "ClosePack":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);


                                SCIMessage secondaryMsgClosePackAck = new SCIMessage();
                                secondaryMsgClosePackAck.CommandID = "ClosePackAck";
                                secondaryMsgClosePackAck.Item = new SCIItem();
                                secondaryMsgClosePackAck.Item.Format = SCIFormat.List;
                                secondaryMsgClosePackAck.Item.Items = new SCIItemCollection();


                                rnd = new Random(Environment.TickCount);
                                if (rnd.Next(0, 3) == 0)
                                {
                                    secondaryMsgClosePackAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                }
                                else
                                {
                                    secondaryMsgClosePackAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 1 });
                                }


                                SCITransaction toReplyClosePackAck = new SCITransaction()
                                {
                                    DeviceId = e.Transaction.DeviceId,
                                    MessageType = MessageType.Secondary,
                                    Id = e.Transaction.Id,
                                    Name = "ClosePackAck",
                                    NeedReply = false,
                                    Primary = e.Transaction.Primary,
                                    Secondary = secondaryMsgClosePackAck
                                };

                                Log.Info("Reply: " + toReplyClosePackAck.XMLText);
                                host.ReplyOutStream(toReplyClosePackAck);

                                break;



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


                                SCITransaction toReplyRequestCancelProcessedLotAck = new SCITransaction()
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


                                SCITransaction toReplyRequestRemoveRemainTrayAck = new SCITransaction()
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


                            case "SendPkgSetting":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgSendPkgSettingAck = new SCIMessage();
                                scndMsgSendPkgSettingAck.CommandID = "SendPkgSettingAck";
                                scndMsgSendPkgSettingAck.Item = new SCIItem();
                                scndMsgSendPkgSettingAck.Item.Format = SCIFormat.List;
                                scndMsgSendPkgSettingAck.Item.Items = new SCIItemCollection();
                                scndMsgSendPkgSettingAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                SCITransaction toReplySendPkgSetting = new SCITransaction()
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



                                //3.31	AlarmReportSend
                            case "AlarmReportSend":

                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                SCIMessage scndMsgAlarmReportSendAck = new SCIMessage();
                                scndMsgAlarmReportSendAck.CommandID = "AlarmReportSendAck";
                                scndMsgAlarmReportSendAck.Item = new SCIItem();
                                scndMsgAlarmReportSendAck.Item.Format = SCIFormat.List;
                                scndMsgAlarmReportSendAck.Item.Items = new SCIItemCollection();
                                scndMsgAlarmReportSendAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                SCITransaction toReplyAlarmReportSend = new SCITransaction()
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


                                SCITransaction toReplySendControlStateAck = new SCITransaction()
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


                                SCITransaction toReplySendProcessStateAck = new SCITransaction()
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

                                SCIMessage scndMsgReqProcessRecipeAck = new SCIMessage();
                                scndMsgReqProcessRecipeAck.CommandID = "RequestProcessRecipeAck";
                                scndMsgReqProcessRecipeAck.Item = new SCIItem();
                                scndMsgReqProcessRecipeAck.Item.Format = SCIFormat.List;
                                scndMsgReqProcessRecipeAck.Item.Items = new SCIItemCollection();

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = reqProcRecipeObj.PalletID });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = reqProcRecipeObj.LotNumber });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeID", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeName", Value = "PALMER1" });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B401" });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "UVPower", Value = 200 });   //obsolete
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime", Value = 15.5 });   //obsolete
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "CureZone", Value = 0 });    //obsolete


                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV1", Value = 30 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV1", Value = 5.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV1", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater1", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater1", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater1", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater1", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater2", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater2", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater2", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater2", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater3", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater3", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater3", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater3", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater4", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater4", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater4", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater4", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater5", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater5", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater5", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater5", Value = 1 });


                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Power_UV2", Value = 30 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime_UV2", Value = 5.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_UV2", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater6", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater6", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater6", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater6", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater7", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater7", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater7", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater7", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater8", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater8", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater8", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater8", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater9", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater9", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater9", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater9", Value = 1 });

                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "FlowRate_Heater10", Value = 3.0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Temp_Heater10", Value = 0 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Enabled_Heater10", Value = 1 });
                                scndMsgReqProcessRecipeAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EnabledN2_Heater10", Value = 1 });


                                SCITransaction toReplyReqProcessRecipe = new SCITransaction()
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



                            case "SendPalletInfo":
                                host = GetHost(e.remoteIPAddress, e.remotePortNumber);

                                ProcessedPalletDataClass sendProcessedPalletInfoData = new ProcessedPalletDataClass();
                                sendProcessedPalletInfoData = GetProcessedPalletFromXML_primary(e.Transaction.XMLText);


                                SCIMessage secondaryMsgSendProcessedPalletInfoAck = new SCIMessage();
                                secondaryMsgSendProcessedPalletInfoAck.CommandID = "SendPalletInfoAck";
                                secondaryMsgSendProcessedPalletInfoAck.Item = new SCIItem();
                                secondaryMsgSendProcessedPalletInfoAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendProcessedPalletInfoAck.Item.Items = new SCIItemCollection();
                                secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendProcessedPalletInfoData.PalletID });
                                secondaryMsgSendProcessedPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                SCITransaction toReplySendProcessedPalletInfoAck = new SCITransaction()
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

                                break;




                                PalletToTrayData sendPalletInfoData = new PalletToTrayData();
                                sendPalletInfoData = GetPalletFromXML_primary(e.Transaction.XMLText);


                                SCIMessage secondaryMsgSendPalletInfoAck = new SCIMessage();
                                secondaryMsgSendPalletInfoAck.CommandID = "SendPalletInfoAck";
                                secondaryMsgSendPalletInfoAck.Item = new SCIItem();
                                secondaryMsgSendPalletInfoAck.Item.Format = SCIFormat.List;
                                secondaryMsgSendPalletInfoAck.Item.Items = new SCIItemCollection();
                                secondaryMsgSendPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = sendPalletInfoData.PalletID });
                                secondaryMsgSendPalletInfoAck.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });


                                SCITransaction toReplySendPalletInfoAck = new SCITransaction()
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


                                SCITransaction toReplySendPalletStatusAck = new SCITransaction()
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


                                SCITransaction toReplySendAlarmReportAck = new SCITransaction()
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


                                SCITransaction toReply = new SCITransaction()
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


                            SCITransaction toReplySendPalletInfo = new SCITransaction()
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
                                    palletObj._arrHGA[nHGAPos]._strOCR = hgaValueNodeList[i].InnerText;
                                }
                                else if (hgaNameNodeList[i].InnerText == "Defect")
                                {
                                    if (hgaValueNodeList[i].InnerText.Length > 0)
                                    {
                                        palletObj._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
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

        private BarInfo GetBarInfo(string Operation, string OCR)
        {
            Log.Info("GetBarInfo");
            string SLDSERIAL_OCR = OCR;

            LotDetails lotDetail1 = GetLotDetailsXML();

            SliderSerialNumber serial = lotDetail1.ListOfSliderSerialNumber.Find(f => f.SLDSERIAL == SLDSERIAL_OCR);

            if (serial == null)
            {
                string errorMsg = string.Empty;
                DataSet ds = MitecsFunction.GetLotDataWithLotID(Operation, OCR, out errorMsg);
                if (ds == null) return null;
                string SectionLot = ds.Tables[0].Rows[0]["SectionNo"].ToString();

                DataSet ds1 = MitecsFunction.GetSliderSerialNumberByGroup(Operation, SectionLot, out errorMsg);
                if (ds1 == null) return null;

                LotDetails lotDetail = new LotDetails();
                lotDetail.SectionNo = ds.Tables[0].Rows[0]["SectionNo"].ToString();
                lotDetail.LOT_TYPE = ds.Tables[0].Rows[0]["LOT_TYPE"].ToString();


                DataSet ds2 = null;
                if (ds.Tables[0].Rows[0]["LOT_TYPE"].ToString() == "XLOT")
                {
                    ds2 = MitecsFunction.GetBarGroupInfoByGroupAll(Operation, SectionLot, out errorMsg);


                    foreach (DataRow row in ds1.Tables[0].Rows)
                    {

                        foreach (DataRow row1 in ds2.Tables[0].Rows)
                        {

                            if (row1["LotNo"].ToString() == row["BARLOT"].ToString())
                            {
                                lotDetail.ListOfSliderSerialNumber.Add(new SliderSerialNumber()
                                {
                                    BARLOT = row["BARLOT"].ToString(),
                                    RealBar = SLDSERIAL_OCR.Substring(5, 2),
                                    
                                    RefBar = int.Parse(row1["BarLotSeq"].ToString()).ToString("00"),

                                    SLDSERIAL = row["SLDSERIAL"].ToString()
                                });


                            }

                        }
                    }



                }
                else
                {
                    ds2 = MitecsFunction.GetBarGroupInfo(Operation, SectionLot, out errorMsg);
                    if (ds2 == null) return null;

                    List<BARLOT_NO> LotSequence = new List<BARLOT_NO>();

                    foreach (DataRow row in ds2.Tables[0].Rows)
                    {
                        LotSequence.Add(new BARLOT_NO { barlot_no = row["LotNo"].ToString() });
                    }

                    LotSequence = LotSequence.OrderBy(f => f.barlot_no).ToList();

                    int index = 1;
                    foreach (BARLOT_NO BAR in LotSequence)
                    {
                        LotSequence[index - 1].index = index;
                        index++;
                    }



                    lotDetail.ListOfSliderSerialNumber = new List<SliderSerialNumber>();

                    foreach (DataRow row in ds1.Tables[0].Rows)
                    {
                        foreach (BARLOT_NO bar in LotSequence)
                        {
                            if (bar.barlot_no == row["BARLOT"].ToString())
                            {
                                lotDetail.ListOfSliderSerialNumber.Add(new SliderSerialNumber()
                                {
                                    BARLOT = row["BARLOT"].ToString(),
                                    RealBar = SLDSERIAL_OCR.Substring(5, 2),
                                    RefBar = bar.index.ToString("00"),
                                    SLDSERIAL = row["SLDSERIAL"].ToString()
                                });
                            }
                        }
                    }


                }
                //////////////



                SaveLotDetailsXML(lotDetail);

                lotDetail1 = GetLotDetailsXML();

                serial = lotDetail1.ListOfSliderSerialNumber.Find(f => f.SLDSERIAL == SLDSERIAL_OCR);
            }

            string m_SectionLot = lotDetail1.SectionNo;
            string m_LOT_TYPE = lotDetail1.LOT_TYPE;
            string m_RefBar = int.Parse(serial.RefBar).ToString("00");
            string m_RealBar = serial.RealBar;

            BarInfo barinfo = new BarInfo() { LOT_TYPE = m_LOT_TYPE, RealBar = m_RealBar, RefBar = m_RefBar, SectionLot = m_SectionLot };
            return barinfo;
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

            //GetBarInfo("1045", textBox1.Text);
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

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
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

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
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

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
        }


        public void SaveLotDetailsXML(LotDetails lotDetail)
        {
            Log.Info("SaveLotDetailsXML");
            XmlSerializer ser = new XmlSerializer(typeof(LotDetails));

            string LotNoPath = AppConfig.GetString("appSettings", "LotNo", string.Empty);
            string FullTrayPath = string.Format("{0}\\LotNo.xml", LotNoPath);

            TextWriter writer = new StreamWriter(FullTrayPath);
            ser.Serialize(writer, lotDetail);
            writer.Close();
        }


        public LotDetails GetLotDetailsXML()
        {
            Log.Info("GetLotDetailsXML");
            string LotNoPath = AppConfig.GetString("appSettings", "LotNo", string.Empty);
            string FullTrayPath = string.Format("{0}\\LotNo.xml", LotNoPath);
            LotDetails lotdetail = null;
            if (File.Exists(FullTrayPath))
            {
                System.Xml.Serialization.XmlSerializer reader =
                                           new System.Xml.Serialization.XmlSerializer(typeof(LotDetails));
                System.IO.StreamReader file = new System.IO.StreamReader(FullTrayPath);

                lotdetail = new LotDetails();

                lotdetail = (LotDetails)reader.Deserialize(file);
                file.Close();

                return lotdetail;
            }
            else
            {
                return null;
            }
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

            //GetBarInfo("1045", textBox1.Text);
            //hostController host = this.GetHost("UNOCR46.tbpd.wdasia.com", 5001);
            //hostController host = this.GetHost("localhost", 5001);
            //hostController host = this.GetHost("WDBOA0185", 5001);

            //********************************************************
            //simulate AreYouThere from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "AlarmReportSend";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALID", Value = 1002 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "AlarmText", Value = "Incorrect Offset" });

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
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
            //simulate AreYouThere from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "OnlineRequest";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
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
            //simulate AreYouThere from Host
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "OfflineRequest";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            SCITransaction trans = new SCITransaction()
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

            Log.Info("host.SendMessage: " + trans.XMLText);
            host.SendMessage(trans);
        }




    }


    public class BarInfo
    {
        public string SectionLot { get; set; }
        public string LOT_TYPE { get; set; }
        public string RefBar { get; set; }
        public string RealBar { get; set; }
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


}
