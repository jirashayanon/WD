﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using log4net;

using WDConnect.Application;
using WDConnect.Common;
using WDConnect.ComponentModel;

using MetrologyHost;
using MetrologyHost.Classes;
using MetrologyHost.Classes.Communication.WDConnect;
using MetrologyHost.Classes.Utilities;


namespace WDConnectWrapperLib
{
    public delegate void PrimaryMessageInEventHandler(object sender, WDConnectCallbackEventArgs e);
    public delegate void SecondaryMessageInEventHandler(object sender, WDConnectCallbackEventArgs e);
    public delegate void HostErrorEventHandler(object sender, WDConnectCallbackEventArgs e);

    public class WDConnectCallbackEventArgs : EventArgs
    {
        public WDConnectCallbackEventArgs():base()
        {
        }

        private string _strCommandID = "";
        public string CommandID
        {
            get { return _strCommandID; }
            set { _strCommandID = value; }
        }

        private string _strMessageIn = "";
        public string MessageIn
        {
            get { return _strMessageIn; }
            set { _strMessageIn = value; }
        }

        private SCITransaction _trans = new SCITransaction();
        public SCITransaction Transaction
        {
            get { return _trans; }
            set { _trans = value; }
        }

        private int _nCOMMACK = 1;
        public int COMMACK
        {
            get { return _nCOMMACK; }
            set { _nCOMMACK = value; }
        }

    }

    public class WDConnectWrapperLibClass
    {
        #region event handler
        public event PrimaryMessageInEventHandler primaryInEvntHandler;
        protected virtual void OnSECsPrimaryIn(WDConnectCallbackEventArgs e)
        {
            if (primaryInEvntHandler != null)
            {
                primaryInEvntHandler(this, e);
            }
        }

        public event SecondaryMessageInEventHandler secondaryInEvntHandler;
        protected virtual void OnSECsSecondaryIn(WDConnectCallbackEventArgs e)
        {
            if (secondaryInEvntHandler != null)
            {
                secondaryInEvntHandler(this, e);
            }
        }

        public event HostErrorEventHandler hostErrorEvntHandler;
        protected virtual void OnSECsHostError(WDConnectCallbackEventArgs e)
        {
            if (hostErrorEvntHandler != null)
            {
                hostErrorEvntHandler(this, e);
            }
        }

        #endregion

        private static WDConnectWrapperLibClass _instance;
        public static WDConnectWrapperLibClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WDConnectWrapperLibClass();
                }

                return _instance;
            }
        }

        private EquipmentControllerClass _equipmentObj;
        //private int _nTransactionID;
        private string _strExePath = "";
        private string _strToolModelPath = "";

        private CONNECTION_RESULT _enmConnected = CONNECTION_RESULT.DISCONNECTED;

        #region ctor
        private WDConnectWrapperLibClass()
        {
            LoggerClass.Instance.MainLogInfo("WDConnectWrapperLibClass ctor");

            _strExePath = System.Windows.Forms.Application.StartupPath;



            // find toolmodel xml file path
            string strToolModelFileName = "";
            EQUIPMENT_TYPE enmEquipmentType = EQUIPMENT_TYPE.NA;
            try
            {
                string strWDConnectWrapperToolModelFileName = _strExePath + @"\WDConnectWrapperLib.xml";
                XmlDocument wdconntToolModelDoc = new XmlDocument();
                wdconntToolModelDoc.Load(strWDConnectWrapperToolModelFileName);

                XmlNodeList toolmodelDNodeList = wdconntToolModelDoc.SelectNodes("/config/ToolModel");
                if (toolmodelDNodeList.Count > 0)
                {
                    foreach (XmlNode node in toolmodelDNodeList)
                    {
                        XmlElement elem = (XmlElement)node;
                        strToolModelFileName = node.InnerText;
                        Console.WriteLine(strToolModelFileName);
                    }
                }

                XmlNodeList equipmentTypeDNodeList = wdconntToolModelDoc.SelectNodes("/config/EquipmentType");
                if (equipmentTypeDNodeList.Count > 0)
                {
                    foreach (XmlNode node in equipmentTypeDNodeList)
                    {
                        XmlElement elem = (XmlElement)node;
                        enmEquipmentType = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), node.InnerText);
                        Console.WriteLine(enmEquipmentType.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LoggerClass.Instance.ErrorLogInfo("Exception, WDConnectWrapperLibClass ctor: " + ex.Message);
            }

            // find toolmodel xml file path


            try
            {
                _strToolModelPath = _strExePath + @"\Local\ToolModel\" + strToolModelFileName;

                _equipmentObj = new EquipmentControllerClass();
                _equipmentObj.Initialize(_strToolModelPath);
                _enmConnected = CONNECTION_RESULT.DISCONNECTED;

                _equipmentObj.EquipmentType = enmEquipmentType;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LoggerClass.Instance.ErrorLogInfo("Exception, WDConnectWrapperLibClass ctor: " + ex.Message);
            }



            UnregisterHOST();
            RegisterHOST();

            _equipmentObj.Connect();
        }
        #endregion


        #region dtor
        ~WDConnectWrapperLibClass()
         {
             //if (_enmConnected == CONNECTION_RESULT.CONNECTED)
             //{
             //    UnregisterHOST();
             //    _equipmentObj.Disconnect();
             //}
         }
        #endregion


        // *************************************************************************
        public CONTROL_STATE ControlState
        {
            get { return _equipmentObj.ControlState; }
            set { _equipmentObj.ControlState = value; }
        }


        public PROCESS_STATE ProcessState
        {
            get { return _equipmentObj.ProcessState; }
            set { _equipmentObj.ProcessState = value; }
        }


        public string EquipmentID
        {
            get { return _equipmentObj.EquipmentID; }
        }



        #region message in/out event handler
        private void equipmentController_SECsPrimaryIn(object sender, SECsPrimaryInEventArgs e)
        {
            LoggerClass.Instance.MainLogInfo("equipmentController_SECsPrimaryIn: " + e.Transaction.XMLText);
            //_nTransactionID = e.Transaction.Id;

            WDConnectCallbackEventArgs wdconnectEvntArgs = new WDConnectCallbackEventArgs();
            wdconnectEvntArgs.MessageIn = e.Transaction.XMLText;
            wdconnectEvntArgs.Transaction = e.Transaction;


            if (e.Transaction.Primary != null)
            {
                wdconnectEvntArgs.Transaction.Primary = e.Transaction.Primary;
                wdconnectEvntArgs.CommandID = e.Transaction.Primary.CommandID;

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

                        SendMessage(trans);

                        this._enmConnected = CONNECTION_RESULT.CONNECTED;
                        break;


                    case "Disconnected":
                        this._enmConnected = CONNECTION_RESULT.DISCONNECTED;
                        break;


                    case "AreYouThere":
                        wdconnectEvntArgs.CommandID = "AreYouThere";
                        this.SendAreYouThereAck(e.Transaction);
                        break;


                    case "RequestProcessState":
                        wdconnectEvntArgs.CommandID = "RequestProcessState";
                        RequestProcessStateAck(e.Transaction, _equipmentObj.ProcessState);
                        break;


                    case "RequestControlState":
                        wdconnectEvntArgs.CommandID = "RequestControlState";
                        RequestControlStateAck(e.Transaction, _equipmentObj.ControlState);
                        break;


                    //case "SendPalletInfo":
                    //    //need to fill pallet object here
                    //    if (this.ControlState == CONTROL_STATE.OFFLINE)
                    //    {
                    //        this.RejectCommandWhileOffline(e.Transaction);
                    //    }
                    //    else
                    //    {
                    //        //PalletToTrayData pallet = GetPalletFromXML(e.Transaction.XMLText);
                    //        //_returnPalletInfo.Clear();
                    //        //_returnPalletInfo = pallet;
                    //        //_returnPalletInfo.WaitForPalletInfo = false;

                    //        //this.SendPalletInfoAck(pallet.PalletID, e.Transaction);
                    //    }

                    //    break;


                    case "OfflineRequest":
                        wdconnectEvntArgs.CommandID = "OfflineRequest";
                        if (this.ControlState == CONTROL_STATE.OFFLINE)
                        {
                            this.RejectCommandWhileOffline(e.Transaction);

                        }
                        else
                        {
                            //this.ControlState = CONTROL_STATE.ONLINE_LOCAL;
                            //this.OfflineRequestAck(e.Transaction);
                        }

                        break;


                    case "OnlineRequest":
                        wdconnectEvntArgs.CommandID = "OnlineRequest";
                        //this.ControlState = CONTROL_STATE.ONLINE_LOCAL;
                        //this.OnlineRequestAck(e.Transaction);

                        break;


                    //case "SendAlarmReport":
                    //    break;


                    case "RequestSuspension":
                        wdconnectEvntArgs.CommandID = "RequestSuspension";
                        break;

                    default:
                        break;
                }
            }
            else
            {
                if (e.Transaction.Secondary != null)
                {
                    LoggerClass.Instance.MainLogInfo("equipmentController_SECsPrimaryIn, Primary null, Secondary not null");
                    LoggerClass.Instance.MainLogInfo("secondary not null: " + e.Transaction.XMLText);
                }
            }


            this.OnSECsPrimaryIn(wdconnectEvntArgs);

        }

        private void equipmentController_SECsSecondaryIn(object sender, SECsSecondaryInEventArgs e)
        {
            LoggerClass.Instance.MainLogInfo("equipmentController_SECsSecondaryIn: " + e.Transaction.XMLText);
            //_transactionID = e.Transaction.Id;

            WDConnectCallbackEventArgs wdconnectEvntArgs = new WDConnectCallbackEventArgs();
            wdconnectEvntArgs.MessageIn = e.Transaction.XMLText;
            wdconnectEvntArgs.COMMACK = this.GetCOMMACKFromSecondaryReply(e.Transaction.XMLText);

            if (e.Transaction.Secondary != null)
            {
                wdconnectEvntArgs.Transaction.Secondary = e.Transaction.Secondary;
                wdconnectEvntArgs.CommandID = e.Transaction.Secondary.CommandID;

                switch (e.Transaction.Secondary.CommandID)
                {
                    case "AreYouThereAck":
                        wdconnectEvntArgs.CommandID = "AreYouThereAck";
                        //do nothing
                        break;


                    case "SendPalletStatusAck":
                        wdconnectEvntArgs.CommandID = "SendPalletStatusAck";
                        //do nothing
                        break;


                    case "SendControlStateAck":
                        wdconnectEvntArgs.CommandID = "SendControlStateAck";
                        //do nothing
                        break;


                    case "SendProcessStateAck":
                        wdconnectEvntArgs.CommandID = "SendProcessStateAck";
                        //do nothing
                        break;


                    case "RequestPalletInfoAck":
                        wdconnectEvntArgs.CommandID = "RequestPalletInfoAck";
                        PalletDataClass palletObj = GetPalletObjFromReqPalletInfoAckXML(e.Transaction.XMLText);

                        //_returnPalletInfo.Clear();
                        //_returnPalletInfo = pallet;
                        //_returnPalletInfo.WaitForPalletInfo = false;

                        //Log.Info("RequestPalletInfoAck: " + pallet.PalletID + "," + pallet.HGALotNumber + "," + pallet.PalletEnabled.ToString());
                        break;


                    case "SendPalletInfoAck":
                        wdconnectEvntArgs.CommandID = "SendPalletInfoAck";
                        //do nothing
                        break;
                    
     
                    case "TrayCompletedAck":
                        wdconnectEvntArgs.CommandID = "TrayCompletedAck";
                        //do nothing
                        break;


                    case "UnloadPalletToTrayAck":
                        wdconnectEvntArgs.CommandID = "UnloadPalletToTrayAck";
                        //do nothing
                        break;


                    case "RemovePalletAck":
                        wdconnectEvntArgs.CommandID = "RemovePalletAck";
                        //do nothing
                        break;


                    case "RequestProcessRecipeAck":
                        wdconnectEvntArgs.CommandID = "RequestProcessRecipeAck";
                        ProcessRecipeDataClass recipeObj = GetProcessRecipeObjFromReqProcessRecipeAckXML(e.Transaction.XMLText);

                        break;


                    case "SendAlarmReportAck":
                        wdconnectEvntArgs.CommandID = "SendAlarmReportAck";
                        break;


                    default:
                        break;
                }
            }
            else
            {
                if (e.Transaction.Primary != null)
                {
                    LoggerClass.Instance.MainLogInfo("equipmentController_SECsSecondaryIn, Secondary null, Primary not null");
                    LoggerClass.Instance.MainLogInfo("primary not null: " + e.Transaction.XMLText);
                }
            }


            this.OnSECsSecondaryIn(wdconnectEvntArgs);
        }

        private string _connectionError = "";
        private void equipmentController_SECsHostError(object sender, SECsHostErrorEventArgs e)
        {
            LoggerClass.Instance.MainLogInfo("equipmentController_SECsHostError: " + e.Message);
            _connectionError = e.Message;

            WDConnectCallbackEventArgs wdconnectEvntArgs = new WDConnectCallbackEventArgs();
            wdconnectEvntArgs.MessageIn = e.Message;
            this.OnSECsHostError(wdconnectEvntArgs);
        }

        #endregion

        // *************************************************************************
        private void RegisterHOST()
        {
            _equipmentObj.WDConnectPrimaryIn += new WDConnectBase.SECsPrimaryInEventHandler(equipmentController_SECsPrimaryIn);
            _equipmentObj.WDConnectSecondaryIn += new WDConnectBase.SECsSecondaryInEventHandler(equipmentController_SECsSecondaryIn);
            _equipmentObj.WDConnectHostError += new WDConnectBase.SECsHostErrorHandler(equipmentController_SECsHostError);
        }

        private void UnregisterHOST()
        {
            if (_equipmentObj == null)
            {
                return;
            }

            _equipmentObj.ClearEventInvocations("WDConnectPrimaryIn");
            _equipmentObj.ClearEventInvocations("WDConnectHostError");
            _equipmentObj.ClearEventInvocations("WDConnectSecondaryIn");
        }


        // *************************************************************************
        #region Communicaitons Messages 

        public void SendMessage(SCITransaction trans)
        {
            LoggerClass.Instance.MainLogInfo("SendMessage: " + trans.XMLText);
            _equipmentObj.SendMessage(trans);
        }


        public void RejectCommandWhileOffline(SCITransaction priTrans)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "RejectCommandWhileOffline";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem {Format = SCIFormat.Integer, Name = "COMMACK", Value = 1});

            SCITransaction replyTrans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(replyTrans);
        }

        
        public void SendAreYouThere()
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "AreYouThere";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Items.Add(new SCIItem {Format = SCIFormat.String, Name = "SoftwareRevision", Value = _equipmentObj.SoftwareRevision});

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendAreYouThereAck()
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "AreYouThereAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem {Format = SCIFormat.String, Name = "SoftwareRevision", Value = _equipmentObj.SoftwareRevision});

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                //Id = _transactionID,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = null,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }

        
        public void SendAreYouThereAck(SCITransaction priTrans)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "AreYouThereAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem {Format = SCIFormat.String, Name = "SoftwareRevision", Value = _equipmentObj.SoftwareRevision});

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        public void OnlineRequestAck(SCITransaction priTrans)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "OnlineRequestAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem {Format = SCIFormat.Integer, Name = "COMMACK", Value = 0});

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        public void OnlineRequestAck(SCITransaction priTrans, int nCOMMACK)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "OnlineRequestAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = nCOMMACK });

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        public void OfflineRequestAck(SCITransaction priTrans)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "OfflineRequestAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        public void OfflineRequestAck(SCITransaction priTrans, int nCOMMACK)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "OfflineRequestAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = nCOMMACK });

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        private void RequestProcessStateAck(SCITransaction priTrans, PROCESS_STATE enmProcessState)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "RequestProcessStateAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ProcessState", Value = (int)enmProcessState });
            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        private void RequestControlStateAck(SCITransaction priTrans, CONTROL_STATE enmControlState)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "RequestControlStateAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ControlState", Value = (int)enmControlState });
            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }


        public void RequestPalletInfo(string strPalletID)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestPalletInfo";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentType", Value = _equipmentObj.EquipmentType.ToString() });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendProcessState(PROCESS_STATE enmProcessState)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendProcessState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ProcessState", Value = (int)enmProcessState });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendProcessState()
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendProcessState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ProcessState", Value = (int)this.ProcessState });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendControlState(CONTROL_STATE enmControlState)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendControlState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ControlState", Value = (int)enmControlState });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendControlState()
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendControlState";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ControlState", Value = (int)this.ControlState });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void RequestProcessRecipe(string strPalletID, string strLotNumber)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestProcessRecipe";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = strLotNumber });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendPalletStatus(string strPalletID, PALLET_STATUS enmStatus)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendPalletStatus";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletStatus", Value = (int)enmStatus });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "WorkZone", Value = 0 });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendPalletStatus(string strPalletID, PALLET_STATUS enmStatus, int nWorkingZone)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendPalletStatus";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletStatus", Value = (int)enmStatus });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "WorkZone", Value = nWorkingZone });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void SendPalletInfo(ProcessedPalletDataClass processedPalletData)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "SendPalletInfo";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();


            SCIItem hgaListItem = new SCIItem();
            hgaListItem.Format = SCIFormat.List;
            hgaListItem.Name = "HGA";
            hgaListItem.Items = new SCIItemCollection();

            switch (_equipmentObj.EquipmentType)
            {
                case EQUIPMENT_TYPE.ASLV:   //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "Suspension", Value = processedPalletData.Suspension });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspTrayID", Value = processedPalletData.SuspTrayID });

                    //request suspension
                    SCIItem reqSuspListItem = new SCIItem();
                    reqSuspListItem.Format = SCIFormat.List;
                    reqSuspListItem.Name = "RequestSuspension";
                    reqSuspListItem.Value = processedPalletData.ReqSusp.IsRequesting;
                    reqSuspListItem.Items = new SCIItemCollection();

                    reqSuspListItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = processedPalletData.ReqSusp.ACAMID });
                    reqSuspListItem.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = processedPalletData.ReqSusp.SuspAmt });

                    primMsg.Item.Items.Add(reqSuspListItem);

                    break;


                case EQUIPMENT_TYPE.APT:    //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SliderTrayID", Value = processedPalletData.SliderTrayID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotNumber", Value = processedPalletData.LotNumber });

                    break;


                case EQUIPMENT_TYPE.ILC:    //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = processedPalletData.LotNumber });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "UVPower", Value = processedPalletData.UVPower });        //obsolete
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "CureTime", Value = (float) processedPalletData.CureTime });       //obsolete
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "CureZone", Value = processedPalletData.CureZone });       //obsolete

                    break;


                case EQUIPMENT_TYPE.SJB:    //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = processedPalletData.LotNumber });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SJBFixture", Value = processedPalletData.SJBFixture });

                    break;

                case EQUIPMENT_TYPE.AVI:    //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = processedPalletData.LotNumber });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SJBLane", Value = processedPalletData.SJBLane});

                    primMsg.Item.Items.Add(hgaListItem);
                    for (int i = 0; i < 10; i++)
                    {
                        SCIItem hgaItem = new SCIItem();
                        hgaItem.Format = SCIFormat.List;
                        hgaItem.Name = "HGA" + (i + 1).ToString();
                        hgaItem.Value = "";
                        hgaItem.Items = new SCIItemCollection();
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = processedPalletData._arrHGA[i]._strOCR });

                        if (processedPalletData._arrHGA[i]._lstDefects.Count > 0)
                        {
                            //hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = processedPalletData._arrHGA[i]._lstDefects[0] });
                            hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = processedPalletData._arrHGA[i].Defects });
                        }
                        else
                        {
                            hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = "" });
                        }

                        //add HGA1 - HGA10
                        hgaListItem.Items.Add(hgaItem);
                    }

                    break;


                case EQUIPMENT_TYPE.UNOCR:  //
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "LotNumber", Value = processedPalletData.LotNumber });

                    primMsg.Item.Items.Add(hgaListItem);
                    for (int i = 0; i < 10; i++)
                    {
                        SCIItem hgaItem = new SCIItem();
                        hgaItem.Format = SCIFormat.List;
                        hgaItem.Name = "HGA" + (i + 1).ToString();
                        hgaItem.Value = "";
                        hgaItem.Items = new SCIItemCollection();
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = processedPalletData._arrHGA[i]._strOCR});

                        if (processedPalletData._arrHGA[i]._lstDefects.Count > 0)
                        {
                            //hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = processedPalletData._arrHGA[i]._lstDefects[0] });
                            hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = processedPalletData._arrHGA[i].Defects });
                        }
                        else
                        {
                            hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = "" });
                        }

                        //add HGA1 - HGA10
                        hgaListItem.Items.Add(hgaItem);
                    }

                    break;


                case EQUIPMENT_TYPE.SAI:
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = (int) _equipmentObj.EquipmentType });
                    primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = processedPalletData.PalletID });

                    primMsg.Item.Items.Add(hgaListItem);
                    for (int i = 0; i < 10; i++)
                    {
                        SCIItem hgaItem = new SCIItem();
                        hgaItem.Format = SCIFormat.List;
                        hgaItem.Name = "HGA" + (i + 1).ToString();
                        hgaItem.Value = "";
                        hgaItem.Items = new SCIItemCollection();

                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "xlocACAM", Value = (float)processedPalletData._arrHGA[i]._dblxlocACAM });
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "ylocACAM", Value = (float)processedPalletData._arrHGA[i]._dblylocACAM });
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "skwACAM", Value = (float)processedPalletData._arrHGA[i]._dblskwACAM });

                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "xlocSAI", Value = (float)processedPalletData._arrHGA[i]._dblxlocSAI });
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "ylocSAI", Value = (float)processedPalletData._arrHGA[i]._dblylocSAI });
                        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.Float, Name = "skwSAI", Value = (float)processedPalletData._arrHGA[i]._dblskwSAI });

                        //add HGA1 - HGA10
                        hgaListItem.Items.Add(hgaItem);
                    }


                    break;


                case EQUIPMENT_TYPE.NA:
                    break;


                default:
                    break;
            }


            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void RemovePallet(string strPalletID, int nWorkingZone)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RemovePallet";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "WorkZone", Value = nWorkingZone });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);

        }


        public void SendAlarmReport(string strAlarmID)
        {
            SCIMessage almMsg = new SCIMessage();
            almMsg = GetAlarmLogicalName(strAlarmID);

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = almMsg.CommandID,
                NeedReply = true,
                Primary = almMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void UnloadPalletToTray(PalletDataClass pallet, string strTrayID, int nTraySize, int nRowID)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "UnloadPalletToTray";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = pallet.PalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayID", Value = strTrayID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "TraySize", Value = nTraySize });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "RowID", Value = nRowID });


            SCIItem hgaListItem = new SCIItem();
            hgaListItem.Format = SCIFormat.List;
            hgaListItem.Name = "HGA";
            hgaListItem.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(hgaListItem);

            for (int i = 0; i < 10; i++)
            {
                SCIItem hgaItem = new SCIItem();
                hgaItem.Format = SCIFormat.List;
                hgaItem.Name = "HGA" + i.ToString();
                hgaItem.Value = "";
                hgaItem.Items = new SCIItemCollection();
                hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = pallet._arrHGA[i]._strOCR});
                
                if (pallet._arrHGA[i]._lstDefects.Count > 0)
                {
                    //hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = pallet._arrHGA[i]._lstDefects[0] });
                    hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = pallet._arrHGA[i].Defects });
                }
                else
                {
                    hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = "" });
                }

                //add HGA1 - HGA10
                hgaListItem.Items.Add(hgaItem);
            }


            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        public void TrayCompleted(TrayDataClass tray)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "TrayCompleted";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "TrayID", Value = tray.TrayID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "TraySize", Value = tray.TraySize });

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID11", Value = tray.PalletID_RowID11 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID12", Value = tray.PalletID_RowID12 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID21", Value = tray.PalletID_RowID21 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID22", Value = tray.PalletID_RowID22 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID31", Value = tray.PalletID_RowID31 });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID_RowID32", Value = tray.PalletID_RowID32 });

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "StartTime", Value = tray.UnloadStartTime.ToString() });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "FinishTime", Value = tray.UnloadFinishTime.ToString() });


            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        private int GetCOMMACKFromSecondaryReply(string strSecondaryXML)
        {
            int nCOMMACK = 1;

            if (strSecondaryXML.Length < 1)
            {
                return 1;   //error, no reply
            }

            try
            {
                XmlDocument getCOMMACKXMLDoc = new XmlDocument();
                getCOMMACKXMLDoc.LoadXml(strSecondaryXML);

                XmlElement root_getCOMMACKXMLDoc = getCOMMACKXMLDoc.DocumentElement;
                XmlNodeList getCOMMACKNodeList = root_getCOMMACKXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");

                if (getCOMMACKNodeList.Count > 0)
                {
                    foreach (XmlNode node in getCOMMACKNodeList)
                    {
                        XmlNodeList nameNodeList = node.SelectNodes("./Name");
                        XmlNodeList valueNodeList = node.SelectNodes("./Value");

                        foreach (XmlNode nameChildNode in nameNodeList)
                        {
                            if (nameChildNode.InnerText == "COMMACK")
                            {
                                foreach (XmlNode valueChildNode in valueNodeList)
                                {
                                    nCOMMACK = Int32.Parse(valueChildNode.InnerText);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception, " + ex.Message);
                return 1;
            }


            return nCOMMACK;
        }


        private SCIMessage GetAlarmLogicalName(string strAlarmID)
        {
            for (int i = 0; i < _equipmentObj.Alarms.Count; i++)
            {
                if (_equipmentObj.Alarms[i].id.ToString() == strAlarmID)
                {
                    SCIMessage message = new SCIMessage();
                    //message.CommandID = "Alarm:" + _equipmentObj.Alarms[i].id;
                    message.CommandID = "SendAlarmReport";
                    message.Item = new SCIItem();

                    message.Item.Items = new SCIItemCollection();
                    message.Item.Format = SCIFormat.List;

                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "logicalName", Value = _equipmentObj.Alarms[i].logicalName });
                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "id", Value = _equipmentObj.Alarms[i].id });
                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "description", Value = _equipmentObj.Alarms[i].description });

                    return message;
                }
            }

            return null;
        }


        private SCIMessage GetAlarmLogicalName(string strAlarmID, out string strLogicalName, out string strDescription)
        {
            for (int i = 0; i < _equipmentObj.Alarms.Count; i++)
            {
                if (_equipmentObj.Alarms[i].id.ToString() == strAlarmID)
                {
                    SCIMessage message = new SCIMessage();
                    //message.CommandID = "Alarm:" + _equipmentObj.Alarms[i].id;
                    message.CommandID = "SendAlarmReport";
                    message.Item = new SCIItem();

                    message.Item.Items = new SCIItemCollection();
                    message.Item.Format = SCIFormat.List;

                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "logicalName", Value = _equipmentObj.Alarms[i].logicalName });
                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "id", Value = _equipmentObj.Alarms[i].id });
                    message.Item.Items.Add(new SCIItem() { Format = SCIFormat.String, Name = "description", Value = _equipmentObj.Alarms[i].description });

                    strLogicalName = _equipmentObj.Alarms[i].logicalName;
                    strDescription = _equipmentObj.Alarms[i].description;

                    return message;
                }
            }

            strLogicalName = "";
            strDescription = "";
            return null;
        }


        public void SendRequestSuspensionAck()
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "RequestSuspensionAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                //Id = _transactionID,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = null,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }

        public void SendRequestSuspensionAck(SCITransaction priTrans)
        {
            SCIMessage scndMsg = new SCIMessage();
            scndMsg.CommandID = "RequestSuspensionAck";
            scndMsg.Item = new SCIItem();
            scndMsg.Item.Format = SCIFormat.List;
            scndMsg.Item.Items = new SCIItemCollection();

            scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "EquipmentID", Value = _equipmentObj.EquipmentID });

            SCITransaction trans = new SCITransaction
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Secondary,
                Id = priTrans.Id,
                Name = scndMsg.CommandID,
                NeedReply = false,
                Primary = priTrans.Primary,
                Secondary = scndMsg
            };

            this.SendMessage(trans);
        }



        public void SendRequestSuspension(string strACAMID, int nSuspAmt)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestSuspension";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Value = true;

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = strACAMID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SuspAmt", Value = nSuspAmt });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }


        //This SCIMessage is for APT /or ACAM only
        public void RequestSwapPallet(string strPalletID)
        {
            SCIMessage primMsg = new SCIMessage();
            primMsg.CommandID = "RequestSwapPallet";
            primMsg.Item = new SCIItem();
            primMsg.Item.Format = SCIFormat.List;
            primMsg.Item.Items = new SCIItemCollection();
            primMsg.Item.Value = true;

            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = strPalletID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = _equipmentObj.EquipmentID });
            primMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = _equipmentObj.EquipmentType });

            SCITransaction trans = new SCITransaction()
            {
                DeviceId = Int32.Parse(_equipmentObj.DeviceID),
                MessageType = MessageType.Primary,
                //Id = _transactionID + 1,
                Name = primMsg.CommandID,
                NeedReply = true,
                Primary = primMsg,
                Secondary = null
            };

            this.SendMessage(trans);
        }

        #endregion


        // *************************************************************************
        #region test message
        //public void TestSendPalletObjXML()
        //{
        //    SCIMessage scndMsg = new SCIMessage();
        //    scndMsg.CommandID = "TestSendPalletObjXML";
        //    scndMsg.Item = new SCIItem();
        //    scndMsg.Item.Format = SCIFormat.List;
        //    scndMsg.Item.Items = new SCIItemCollection();


        //    SCIItem hgaListItem = new SCIItem();
        //    hgaListItem.Format = SCIFormat.List;
        //    hgaListItem.Name = "HGA";
        //    hgaListItem.Items = new SCIItemCollection();


        //    scndMsg.Item.Items.Add(hgaListItem);


        //    for (int i = 0; i < 10; i++)
        //    {
        //        if ((i == 2) || (i==5) || (i==6) || (i==8))
        //        {
        //            continue;
        //        }

        //        SCIItem hgaItem = new SCIItem();
        //        hgaItem.Format = SCIFormat.List;
        //        hgaItem.Name = "HGA" + i.ToString();
        //        hgaItem.Value = "";
        //        hgaItem.Items = new SCIItemCollection();
        //        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = i.ToString() });
        //        hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = "A" + i.ToString() });

        //        //add HGA1 - HGA10
        //        hgaListItem.Items.Add(hgaItem);
        //    }

        //    SCITransaction trans = new SCITransaction
        //    {
        //        DeviceId = Int32.Parse(_equipmentObj.DeviceID),
        //        MessageType = MessageType.Primary,
        //        //Id = _transactionID + 1,
        //        Name = "TestSendPalletObjXML",
        //        NeedReply = false,
        //        Primary = null,
        //        Secondary = scndMsg
        //    };


        //    //PalletDataClass aPallet = GetPalletObjFromSecndXML(trans.XMLText);
        //    this.SendMessage(trans);
        //}

        //public void TestSendRecipeObjXML()
        //{
        //    SCIMessage scndMsg = new SCIMessage();
        //    scndMsg.CommandID = "RequestProcessRecipeAck";
        //    scndMsg.Item = new SCIItem();
        //    scndMsg.Item.Format = SCIFormat.List;
        //    scndMsg.Item.Items = new SCIItemCollection();

        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = "SF0001" });

        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = "NUMEE_A1" });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "RecipeID", Value = "r0001" });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = "B401" });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "UVPower", Value = 100.99 });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "CureTime", Value = 20 });
        //    scndMsg.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "CureZone", Value = 0 });
        //    SCITransaction trans = new SCITransaction()
        //    {
        //        DeviceId = Int32.Parse(_equipmentObj.DeviceID),
        //        MessageType = MessageType.Primary,
        //        //Id = priTrans.Id,
        //        Name = "RequestProcessRecipeAck",
        //        NeedReply = false,
        //        Primary = null,
        //        Secondary = scndMsg
        //    };

        //    ProcessRecipeDataClass aRecipe = GetProcessRecipeObjFromSecndXML(trans.XMLText);
        //    this.SendMessage(trans);
        //}

        #endregion


        // *************************************************************************
        private PalletDataClass GetPalletObjFromReqPalletInfoAckXML(string xmlPallet)
        {
            PalletDataClass palletObj = new PalletDataClass();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;

            XmlNodeList palletInfoSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");
            #region palletInfoSCIItemsNodeList
            if (palletInfoSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in palletInfoSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "COMMACK")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.COMMACK = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ALMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                string strAlarmName = "";
                                string strAlarmDescription = "";

                                SCIMessage almSciMsg = this.GetAlarmLogicalName(valueChildNode.InnerText, out strAlarmName, out strAlarmDescription);                                
                                
                                palletObj.Alarm.AlarmID = valueChildNode.InnerText;
                                palletObj.Alarm.AlarmName = strAlarmName;
                                palletObj.Alarm.AlarmDescription = strAlarmDescription;
                            }
                        }
                    } 
                    
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
                        if (nameChildNode.InnerText == "ProductName")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.ProductName = valueChildNode.InnerText;
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
                        if (nameChildNode.InnerText == "STR")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.STR = valueChildNode.InnerText;
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
                        if (nameChildNode.InnerText == "ACAMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.ACAMID = valueChildNode.InnerText;
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
                        if (nameChildNode.InnerText == "AllowedMix")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.AllowedMix = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledPallet")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //0 = enabled
                                //1 = disabled
                                //palletObj.Enabled = !(Convert.ToInt32(valueChildNode.InnerText, 10) > 0);
                                //Log.Info("PalletID: " + palletObj.PalletID + ", Enabled: " + palletObj.Enabled.ToString());

                                palletObj.Enabled = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }



                    /*
                    //DefectP1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[0].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[1].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP3
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP3")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[2].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP4
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP4")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[3].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP5
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP5")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[4].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP6
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP6")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[5].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP7
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP7")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[6].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP8
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP8")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[7].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP9
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP9")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[8].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }

                    //DefectP10
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "DefectP10")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                //palletObj._arrHGA[9].DefectCode = valueChildNode.InnerText;
                            }
                        }
                    }
                    */

                    //EndLot
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EndLot")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.EndLot = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "TransID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                palletObj.TransID = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }
                }
            }

            #endregion

            XmlNodeList itemSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");
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
                            for(int i = 0; i < reqSuspNodeList.Count; i++)
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
                                    if (hgaNameNodeList[i].InnerText == "SN")
                                    {
                                        palletObj._arrHGA[nHGAPos]._strOCR = hgaValueNodeList[i].InnerText;
                                    }
                                    else if (hgaNameNodeList[i].InnerText == "Defect")
                                    {
                                        palletObj._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                    }
                                }

                            }
                        }
                    }

                }

            }

            /*
            XmlNodeList hgalistSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem/Items/SCIItem");
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
                        nHGAPos = nHGAPos - 1;

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
                                palletObj._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                            }
                        }

                    }


                }

            }
            */

            return palletObj;
        }


        private ProcessRecipeDataClass GetProcessRecipeObjFromReqProcessRecipeAckXML(string xmlPallet)
        {
            ProcessRecipeDataClass processRecipeObj = new ProcessRecipeDataClass();

            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(xmlPallet);

            XmlElement root_palletXMLDoc = palletXMLDoc.DocumentElement;

            XmlNodeList recipeInfoSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");
            #region recipeInfoSCIItemsNodeList
            if (recipeInfoSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in recipeInfoSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "COMMACK")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.COMMACK = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ALMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                string strAlarmName = "";
                                string strAlarmDescription = "";

                                SCIMessage almSciMsg = this.GetAlarmLogicalName(valueChildNode.InnerText, out strAlarmName, out strAlarmDescription);

                                processRecipeObj.Alarm.AlarmID = valueChildNode.InnerText;
                                processRecipeObj.Alarm.AlarmName = strAlarmName;
                                processRecipeObj.Alarm.AlarmDescription = strAlarmDescription;
                            }
                        }
                    } 
                    
                    
                    
                    
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.PalletID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.LotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "RecipeID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.RecipeID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "RecipeName")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.RecipeName = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "UVPower")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.UVPower = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureTime")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.CureTime = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureZone")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                processRecipeObj.CureZone = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                }
            }

            #endregion

            /*
            XmlNodeList hgalistSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem/Items/SCIItem");
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
                        nHGAPos = nHGAPos - 1;

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
                                palletObj._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                            }
                        }

                    }


                }

            }
            */

            return processRecipeObj;
        }


        // *************************************************************************
        //Digit# 3-10
        public bool VerifySuspensionPartNumber(string suspensionTrayID, ProcessRecipeDataClass recipe)
        {
            return (this.ExtractSuspPartNumberFromTrayID(suspensionTrayID) == recipe.SuspPartNumber) ? true: false;
        }

        public string ExtractSuspPartNumberFromTrayID(string suspensionTrayID)
        {
            string suspPartNumber = suspensionTrayID.Replace("\r\n", "");
            suspPartNumber = suspPartNumber.Trim();

            return suspPartNumber.Substring(2, 8);
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

            MainLogger.Info("CBKTransferCommLibClass ctor");
            ErrorLogger.Info("CBKTransferCommLibClass ErrorLogger init");
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
    public enum CONNECTION_RESULT
    {
        DISCONNECTED = 0,
        CONNECTED = 1
    }

    // ////////////////////////////////////////////////////////////////////////////////////
}
