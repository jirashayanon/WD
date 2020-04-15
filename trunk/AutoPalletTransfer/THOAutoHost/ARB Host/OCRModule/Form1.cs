using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WDConnect.Application;
using WDConnect.Common;

namespace OCRModule
{
    public partial class Form1 : Form
    {
        public static readonly ILog Log = LogManager.GetLogger("MainWindow");
        public Form1()
        {
            InitializeComponent();
            toolController = new toolController();

        }

        toolController toolController;
        private void Form1_Load(object sender, EventArgs e)
        {
            Log.Info("Form1_Load");
            string ToolModelPath = ConfigurationManager.AppSettings["ToolModelPath"].ToString();
            toolController.Initialize(ToolModelPath + "\\ToolModel.xml");

            toolController.WDConnectPrimaryIn += new WDConnectBase.SECsPrimaryInEventHandler(hostController_SECsPrimaryIn);
            toolController.WDConnectSecondaryIn += new WDConnectBase.SECsSecondaryInEventHandler(hostController_SECsSecondaryIn);
            toolController.WDConnectHostError += new WDConnectBase.SECsHostErrorHandler(hostController_SECsHostError);

            toolController.Connect();

            ConnectionTextBox.Text = "NotConnecting";
            ConnectionTextBox.BackColor = Color.Red;

            ToolIDTextBox.Text = toolController.ToolId;
            RemoteIPAddressTextBox.Text = toolController.remoteIPAddress;
            RemotePortTextBox.Text = toolController.remotePortNumber;

        }

        private void hostController_SECsPrimaryIn(object sender, SECsPrimaryInEventArgs e)
        {
            Log.Info("hostController_SECsPrimaryIn");
            switch (e.Transaction.Primary.CommandID)
            {
                case "Connected":
                    SCITransaction trans = new SCITransaction();
                    trans.MessageType = MessageType.Primary;
                    trans.Name = "Event:FinishedInitialize";
                    trans.NeedReply = false;

                    SCIMessage mes = new SCIMessage();
                    mes.CommandID = "Event:FinishedInitialize";
                    trans.Primary = mes;

                    toolController.SendMessage(trans);

                    this.Invoke(new Action(() =>
                    {
                        this.ConnectionTextBox.Text = "Connected";
                        ConnectionTextBox.BackColor = Color.Green;
                    }));
                    break;
                case "Disconnected":
                    this.Invoke(new Action(() =>
                    {
                        ConnectionTextBox.Text = "NotConnecting";
                        ConnectionTextBox.BackColor = Color.Red;
                    }));

                    break;
            }
        }

        private void hostController_SECsSecondaryIn(object sender, SECsSecondaryInEventArgs e)
        {
            switch (e.Transaction.Secondary.CommandID)
            {
                case "MES_Data":
                    this.Invoke(new Action(() =>
                    {
                        SCIItemCollection items3 = e.Transaction.Secondary.Item.Items;
                        LotNoTextBox.Text = items3[0].Value.ToString();
                        LotTypeTextBox.Text = items3[1].Value.ToString();
                        RefBarTextBox.Text = items3[2].Value.ToString();
                        RealBarTextBox.Text = items3[3].Value.ToString();

                        Log.Debug("Receive <-- " + LotNoTextBox.Text);
                    }));
                    break;
            }
        }

        private void hostController_SECsHostError(object sender, SECsHostErrorEventArgs e)
        {
            Log.Info("hostController_SECsHostError");
            this.Invoke(new Action(() =>
            {
                ErrorMessageTextBox.Text = e.Message;
            }));
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Log.Info("SendButton_Click");
            SCITransaction trans = CreatePrimaryTransaction("Mitecs GetLotDetails by Slider Serial number.", true, "MES_OCR");

            trans.Primary.Item = new SCIItem();
            trans.Primary.Item.Format = SCIFormat.List;
            trans.Primary.Item.Items = new SCIItemCollection();

            trans.Primary.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Operation", Value = OperationNoTextBox.Text });
            trans.Primary.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "OCRNumber", Value = OCRTextBox.Text });


            toolController.SendMessage(trans);
            Log.Debug("Send --> " + OCRTextBox.Text);
        }


        private SCITransaction CreatePrimaryTransaction(string Name, bool NeedReply, string CommandID)
        {
            Log.Info("CreatePrimaryTransaction");
            SCITransaction trans = new SCITransaction();
            trans.MessageType = MessageType.Primary;

            trans.Name = Name;
            trans.NeedReply = NeedReply;

            SCIMessage mes = new SCIMessage();
            mes.CommandID = CommandID;
            trans.Primary = mes;

            return trans;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            //ToolModel.
            SCIMessage msg = new SCIMessage();
            //msg.CommandID;
            //msg.Item
            SCIItem itm = new SCIItem()
            itm.Items.Add
            */

            //Test Send AreYouThere
            Log.Info("Test Send AreYouThere");
            SCITransaction trans = CreatePrimaryTransaction("AreYouThere", true, "AreYouThere");

            trans.Primary.Item = new SCIItem();
            trans.Primary.Item.Format = SCIFormat.List;
            trans.Primary.Item.Items = new SCIItemCollection();

            trans.Primary.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SoftwareRevision", Value = "1.1.0.2F"});


            toolController.SendMessage(trans);
            Log.Debug("Send --> AreYouThere");
        }
    }
}
