using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using System.Xml;
using System.Xml.Serialization;

using WDConnectWrapperLib;

namespace TestWrapperLib
{
    public partial class TestWrapperLib : Form
    {
        public TestWrapperLib()
        {
            InitializeComponent();
            _init();
        }

        //function to initialize the instance of WDConnectWrapperLib before making use
        private void _init()
        {
            //create a callback function when Host sends a primary message to equipment
            WDConnectWrapperLibClass.Instance.primaryInEvntHandler += new PrimaryMessageInEventHandler(PrimaryInCallBack);

            //create a callback function when Host replies a secondary message for the primary message request from equipment
            WDConnectWrapperLibClass.Instance.secondaryInEvntHandler += new SecondaryMessageInEventHandler(SecondaryInCallBack);

            //create a callback function when error occurs during communications
            WDConnectWrapperLibClass.Instance.hostErrorEvntHandler += new HostErrorEventHandler(HostErrorCallBack);


            //list CONTROL STATE values in a combo box for user to select
            cmbControlState.Items.Add(WDConnectWrapperLib.CONTROL_STATE.OFFLINE.ToString());
            cmbControlState.Items.Add(WDConnectWrapperLib.CONTROL_STATE.ONLINE_LOCAL.ToString());
            cmbControlState.Items.Add(WDConnectWrapperLib.CONTROL_STATE.ONLINE_REMOTE.ToString());
            cmbControlState.SelectedIndex = 0;

            //this control state value will be equipment's control state
            WDConnectWrapperLibClass.Instance.ControlState = (WDConnectWrapperLib.CONTROL_STATE)Enum.Parse(typeof(WDConnectWrapperLib.CONTROL_STATE), cmbControlState.Text);


            //list PROCESS STATE values in a combo box for user to select
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.INIT.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.IDLE.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.HOME.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.EMERGENCY.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.READY.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.LOOP.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.OPERATOR.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.STEP.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.SETUP.ToString());
            cmbProcessState.Items.Add(WDConnectWrapperLib.PROCESS_STATE.ABORT.ToString());
            cmbProcessState.SelectedIndex = 0;

            //this process state value will be equipment's process state
            WDConnectWrapperLibClass.Instance.ProcessState = (WDConnectWrapperLib.PROCESS_STATE)Enum.Parse(typeof(WDConnectWrapperLib.PROCESS_STATE), cmbProcessState.Text);

            WDConnectWrapperLibClass.Instance.ControlState = CONTROL_STATE.ONLINE_REMOTE;
        }


        #region Primary In Message
        private delegate void updatePrimaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e);
        private void UpdatePrimaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            txtboxPrimaryMsg.Text = e.MessageIn;
            txtboxPrimaryCommandID.Text = e.CommandID;


            #region AreYouThere
            if (e.CommandID == "AreYouThere")
            {
                //this.DoSomeTasksWhenReceivingAreYouThereFromHost();
            }
            #endregion


            #region OnlineRequest
            if (e.CommandID == "OnlineRequest")
            {
                //probably verify some conditions
                //probably inform user about the request from Host
                //probably do some tasks
                //and reply

                if (WDConnectWrapperLibClass.Instance.ControlState == CONTROL_STATE.OFFLINE)
                {
                    DialogResult ret = MessageBox.Show("Get OnlineReqeust. Do you want to go Online?", "OnlineRequest", MessageBoxButtons.YesNo);
                    if (ret == System.Windows.Forms.DialogResult.Yes)
                    {
                        WDConnectWrapperLibClass.Instance.ControlState = CONTROL_STATE.ONLINE_REMOTE;
                        WDConnectWrapperLibClass.Instance.OnlineRequestAck(e.Transaction, 0);   //COMMACK == 0, ok
                    }
                    else
                    {
                        //Still offline
                        //WDConnectWrapperLibClass.Instance.ControlState = CONTROL_STATE.OFFLINE;
                        WDConnectWrapperLibClass.Instance.OnlineRequestAck(e.Transaction, 1);   //COMMACK == 1, not ok
                    }
                }
                else
                {
                    //already online
                }

            }
            #endregion


            #region OfflineRequest
            if (e.CommandID == "OfflineRequest")
            {
                //probably verify some conditions
                //probably inform user about the request from Host
                //probably do some tasks
                //and reply

                if (WDConnectWrapperLibClass.Instance.ControlState == CONTROL_STATE.OFFLINE)
                {
                    //already offline
                    WDConnectWrapperLibClass.Instance.RejectCommandWhileOffline(e.Transaction);
                }
                else //online
                {
                    DialogResult ret = MessageBox.Show("Get OfflineReqeust. Do you want to go Offline?", "OfflineRequest", MessageBoxButtons.YesNo);
                    if (ret == System.Windows.Forms.DialogResult.Yes)
                    {
                        WDConnectWrapperLibClass.Instance.ControlState = CONTROL_STATE.OFFLINE;
                        WDConnectWrapperLibClass.Instance.OfflineRequestAck(e.Transaction, 0);   //COMMACK == 0, ok
                    }
                    else
                    {
                        WDConnectWrapperLibClass.Instance.OfflineRequestAck(e.Transaction, 1);   //COMMACK == 1, not ok
                    }
                }



            }
            #endregion


            #region RequestSuspension
            if (e.CommandID == "RequestSuspension")
            {
                //this.DoSomeTasks();

                RequestSuspensionClass rqSusp = new RequestSuspensionClass(e.Transaction.XMLText);
                WDConnectWrapperLibClass.Instance.SendRequestSuspensionAck(e.Transaction);

                if (rqSusp.IsRequesting)
                {
                    Console.WriteLine(string.Format("ACAM {0} is requesting {1} suspension", rqSusp.ACAMID, rqSusp.SuspAmt.ToString()));
                }
            }
            #endregion

        }
        public void DelegateUpdatePrimaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Invoke(new updatePrimaryMessageInTextbox(UpdatePrimaryMessageInTextbox), e);
        }


        //callback function when Host sends a primary message to equipment
        public void PrimaryInCallBack(object sender, WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Console.WriteLine("PrimaryInCallBack");
            DelegateUpdatePrimaryMessageInTextbox(e);

            if (e.CommandID == "RequestSuspension")
            {
            }
        }


        public void DoSomeTasksWhenReceivingAreYouThereFromHost()
        {
            //MessageBox.Show("DoSomeTasks");
        }


        #endregion


        #region Secondary In Message
        private delegate void updateSecondaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e);
        private void UpdateSecondaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            txtboxSecondaryMsg.Text = e.MessageIn;
            txtboxSecondaryCommandID.Text = e.CommandID;

            if (e.CommandID == "RequestPalletInfoAck")
            {
                Console.WriteLine(e.MessageIn);

                RequestSuspensionClass reqSusp = new RequestSuspensionClass(e.Transaction.XMLText);
                if (reqSusp.IsRequesting)
                {
                    Console.WriteLine(string.Format("{0} request {1}", reqSusp.ACAMID, reqSusp.SuspAmt.ToString()));
                }
                else
                {
                    Console.WriteLine("no ACAM is requesting suspension");
                }

            }
        }
        public void DelegateUpdateSecondaryMessageInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Invoke(new updatePrimaryMessageInTextbox(UpdateSecondaryMessageInTextbox), e);
        }


        //callback function when Host replies a secondary message for the primary message request from equipment
        ProcessRecipeDataClass _recipe = new ProcessRecipeDataClass();
        private RequestPalletInfoAckObj _reqPalletObj = new RequestPalletInfoAckObj();
        public void SecondaryInCallBack(object sender, WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Console.WriteLine("SecondaryInCallBack");
            DelegateUpdateSecondaryMessageInTextbox(e);


            if (e.CommandID == "RequestPalletInfoAck")
            {
                //PalletDataClass retPallet;
                //retPallet = new PalletDataClass(e.MessageIn);
                //Console.WriteLine(retPallet.PalletID);


                _reqPalletObj = RequestPalletInfoAckObj.ToObj(e.Transaction.Secondary);
                Console.WriteLine(string.Format("{0}, {1}, {2}", _reqPalletObj.ReqSusp.ACAMID, _reqPalletObj.ReqSusp.IsRequesting.ToString(), _reqPalletObj.ReqSusp.SuspAmt.ToString()));
                Console.WriteLine(_reqPalletObj.PalletID);
            }

            if (e.CommandID == "RequestProcessRecipeAck")
            {
                ProcessRecipeDataClass procRecipeObj = new ProcessRecipeDataClass(e.MessageIn);
                _recipe = procRecipeObj;
                //Console.WriteLine(procRecipeObj.RecipeID);
                
                Console.WriteLine(procRecipeObj.LotNumber);
                Console.WriteLine(procRecipeObj.LotSize);
                Console.WriteLine(procRecipeObj.ProductName);
                Console.WriteLine(procRecipeObj.HGAType);
                Console.WriteLine(procRecipeObj.STR);
                Console.WriteLine(procRecipeObj.Line);
                
            }
        }

        #endregion


        #region Host Error
        private delegate void updateHostErrorTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e);
        private void UpdateHostErrorInTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            txtboxErrorMsg.Text = e.MessageIn;
        }
        public void DelegateUpdateHostErrorTextbox(WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Invoke(new updateHostErrorTextbox(UpdateHostErrorInTextbox), e);
        }


        //callback function when error occurs during communications
        public void HostErrorCallBack(object sender, WDConnectWrapperLib.WDConnectCallbackEventArgs e)
        {
            Console.WriteLine("HostErrorCallBack");
            DelegateUpdateHostErrorTextbox(e);
        }

        #endregion


        //sample code to update equipment's control state to WD Host
        private void btnSendControlState_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.ControlState = (WDConnectWrapperLib.CONTROL_STATE) Enum.Parse(typeof( WDConnectWrapperLib.CONTROL_STATE), cmbControlState.Text);
            WDConnectWrapperLibClass.Instance.SendControlState();
        }

        //sample code to send AreYouThere to check if WD Host is still alive
        private void btnRUThere_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.SendAreYouThere();
        }

        //sample code to update equipment's process state to WD Host
        private void btnSendProcessState_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.ProcessState = (WDConnectWrapperLib.PROCESS_STATE)Enum.Parse(typeof(WDConnectWrapperLib.PROCESS_STATE), cmbProcessState.Text);
            WDConnectWrapperLibClass.Instance.SendProcessState();
        }

        //sample code to request for a process recipe
        private void btnRequestProcessRecipe_Click(object sender, EventArgs e)
        {
            //WDConnectWrapperLibClass.Instance.RequestProcessRecipe("PT0001", "WRXGC_A");
            WDConnectWrapperLibClass.Instance.RequestProcessRecipe("", txtboxLotNumber.Text);
            //WDConnectWrapperLibClass.Instance.RequestProcessRecipe("PT0001", "Y5TFE_DD/PS0");            
        }

        //sample code to request for a pallet information for pallet barcode id# SF0001
        private void btnRequestPalletInfo_Click(object sender, EventArgs e)
        {
            //WDConnectWrapperLibClass.Instance.RequestPalletInfo("PTXXXX");
            WDConnectWrapperLibClass.Instance.RequestPalletInfo(txtboxPalletBarcode.Text);
        }


        private ProcessRecipeDataClass _inlineCureInternalProcessRecipe = new ProcessRecipeDataClass();
        private void btnSendPallet_Click(object sender, EventArgs e)
        {
   

            WDConnectWrapperLib.ProcessedPalletDataClass palletDataAfterProcessing = new ProcessedPalletDataClass();
            palletDataAfterProcessing.EquipmentID = WDConnectWrapperLibClass.Instance.EquipmentID;
            //palletDataAfterProcessing.PalletID = _reqPalletObj.PalletID;
            palletDataAfterProcessing.PalletID = txtboxPalletBarcode.Text;
            palletDataAfterProcessing.LotNumber = _reqPalletObj.LotNumber;
            palletDataAfterProcessing.SJBLane = 21;




            //1.1351, 0.0021, 89.4340, 1.1360, -0.0035, 89.3036,
            palletDataAfterProcessing._arrHGA[0]._dblxlocACAM = 1.1351;
            palletDataAfterProcessing._arrHGA[0]._dblylocACAM = 0.0021;
            palletDataAfterProcessing._arrHGA[0]._dblskwACAM = 89.4340;

            palletDataAfterProcessing._arrHGA[0]._dblxlocSAI = 1.1360;
            palletDataAfterProcessing._arrHGA[0]._dblylocSAI = -0.0035;
            palletDataAfterProcessing._arrHGA[0]._dblskwSAI = 89.3036;


            // 1.1305, 0.0000, 89.6722, 1.1314, 0.0019, 89.7099,
            palletDataAfterProcessing._arrHGA[1]._dblxlocACAM = 1.1305;
            palletDataAfterProcessing._arrHGA[1]._dblylocACAM = 0.0000;
            palletDataAfterProcessing._arrHGA[1]._dblskwACAM = 89.6722;

            palletDataAfterProcessing._arrHGA[1]._dblxlocSAI = 1.1314;
            palletDataAfterProcessing._arrHGA[1]._dblylocSAI = 0.0019;
            palletDataAfterProcessing._arrHGA[1]._dblskwSAI = 89.7099;

            // 1.1207, 0.0058, 90.0569, 1.1216, 0.0006, 89.9412,
            // 1.1214, 0.0077, 90.4667, 1.1223, 0.0039, 90.3834,
            // 1.1236, 0.0042, 90.3043, 1.1245, -0.0004, 90.2005,
            // 1.1306, 0.0050, 90.4107, 1.1315, 0.0037, 90.3691,
            // 1.1361, 0.0047, 89.4286, 1.1370, 0.0011, 89.3465,
            // 1.1359, 0.0043, 90.5226, 1.1368, 0.0037, 90.5064,
            // 1.1325, 0.0019, 90.1507, 1.1334, 0.0007, 90.1244,
            // 1.1347, 0.0035, 90.2871, 1.1356, 0.0023, 90.2606


            //e.g. ILC may be configured to use some pre-config recipe data during setup
            //palletDataAfterProcessing.LotNumber = _inlineCureInternalProcessRecipe.LotNumber;

            palletDataAfterProcessing.ReqSusp.IsRequesting  = _reqPalletObj.ReqSusp.IsRequesting;
            palletDataAfterProcessing.ReqSusp.ACAMID        = _reqPalletObj.ReqSusp.ACAMID;
            palletDataAfterProcessing.ReqSusp.SuspAmt       = _reqPalletObj.ReqSusp.SuspAmt;

            WDConnectWrapperLibClass.Instance.SendPalletInfo(palletDataAfterProcessing);
        }

        private void btnSendPalletStatus_Click(object sender, EventArgs e)
        {
            //after verifying pallet's data from Host, then proceed to the loading step.
            //here to inform WD's Host that ILC is about to load in the pallet into the cure zone#1
            WDConnectWrapperLibClass.Instance.SendPalletStatus("PT0001", PALLET_STATUS.LOADING, 1);

        }

        private void btnSendAlarmReport_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.SendAlarmReport("10000");
        }

        private void copyTextPrimaryMsg_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtboxPrimaryMsg.Text);
            }
            catch (Exception ex)
            {
                Clipboard.Clear();
            }
        }

        private void copyTextSecondaryMsg_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtboxSecondaryMsg.Text);
            }
            catch (Exception ex)
            {
                Clipboard.Clear();
            }
        }

        private void txtboxPrimaryMsg_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                contextMenuStrip1.Show(this.Location.X + e.Location.X, this.Location.Y + e.Location.Y + 230);
            }
        }

        private void txtboxSecondaryMsg_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                contextMenuStrip2.Show(this.Location.X + e.Location.X, this.Location.Y + e.Location.Y + 350);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WDConnectWrapperLibClass.Instance.VerifySuspensionPartNumber("UH04382-E2-30T-J9H65043", _recipe))
            {
                MessageBox.Show("Suspension Part Number is correct.");
            }
            else
            {
                string suspPartNumberFromTray = WDConnectWrapperLibClass.Instance.ExtractSuspPartNumberFromTrayID("UH04382-E2-30T-J9H65043");
                string suspPartNumberFromRecipe = _recipe.SuspPartNumber;

                MessageBox.Show("Please verify Suspension Part Number. " + suspPartNumberFromTray + " : " + suspPartNumberFromRecipe);
            }

        }

        private void btnReqSusp_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.SendRequestSuspension(txtboxReqSuspACAMID.Text, Int32.Parse(txtboxReqSuspSuspAmt.Text));
        }

        private void showXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormShowXML frm = new FormShowXML(PrintXML(txtboxPrimaryMsg.Text));
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }

        private static String PrintXML(String XML)
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

        private void showXMLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                FormShowXML frm = new FormShowXML(PrintXML(txtboxSecondaryMsg.Text));
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnRequestSwapPallet_Click(object sender, EventArgs e)
        {
            WDConnectWrapperLibClass.Instance.RequestSwapPallet(txtboxPalletBarcode.Text);
        }

    }
}
