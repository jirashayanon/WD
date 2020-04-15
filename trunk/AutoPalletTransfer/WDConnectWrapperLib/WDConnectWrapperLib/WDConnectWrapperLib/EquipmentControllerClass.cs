﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using System.IO;

using log4net;
using WDConnect.Application;
using WDConnect.Common;
using WDConnect.ComponentModel;

namespace WDConnectWrapperLib
{
    // ////////////////////////////////////////////////////////////////////////////////////
    public class EquipmentControllerClass : WDConnect.Application.WDConnectBase
    {
        public PROCESS_STATE _enmProcessState;
        public CONTROL_STATE _enmControlState;

        public EquipmentControllerClass()
        {
            _enmProcessState = PROCESS_STATE.INIT;
            _enmControlState = CONTROL_STATE.OFFLINE;
        }

        public override void Initialize(string equipmentModelPath)
        {
            base.Initialize(equipmentModelPath);
        }

        public string EquipmentID
        {
            get { return EquipmentModel.Nameable.id; }
        }

        public string DeviceID
        {
            get { return EquipmentModel.GemConnection.deviceId.ToString(); }
        }

        public string MachineName
        {
            get { return EquipmentModel.Nameable.alias; }
        }


        public string SoftwareRevision
        {
            get { return EquipmentModel.Nameable.softwareRev; }
        }

        public string ConnectionMode
        {
            get { return EquipmentModel.GemConnection.HSMS.connectionMode.ToString(); }
        }

        public string RemoteIPAddress
        {
            get { return EquipmentModel.GemConnection.HSMS.remoteIPAddress; }
        }

        public int RemotePortNumber
        {
            get { return EquipmentModel.GemConnection.HSMS.remotePortNumber; }
        }

        public int T3Timeout
        {
            get { return EquipmentModel.GemConnection.HSMS.T3Timeout; }
        }

        public int T5Timeout
        {
            get { return EquipmentModel.GemConnection.HSMS.T5Timeout; }
        }

        //public int T6Timeout
        //{
        //    get { return EquipmentModel.GemConnection.HSMS.T6Timeout; }
        //}

        //public int T7Timeout
        //{
        //    get { return EquipmentModel.GemConnection.HSMS.T7Timeout; }
        //}

        //public int T8Timeout
        //{
        //    get { return EquipmentModel.GemConnection.HSMS.T8Timeout; }
        //}

        public PROCESS_STATE ProcessState
        {
            get { return _enmProcessState; }
            set { _enmProcessState = value; }
        }

        public CONTROL_STATE ControlState
        {
            get { return _enmControlState; }
            set { _enmControlState = value; }
        }

        public void SendMessage(WDConnect.Common.SCITransaction trans)
        {
            try
            {
                LoggerClass.Instance.MainLogInfo("EquipmentController SendMessage");
                base.ProcessOutStream(trans);
            }
            catch (Exception ex)
            {
                LoggerClass.Instance.ErrorLogInfo("Exception, SendMessage: " + ex.Message);
            }
        }


        private EQUIPMENT_TYPE _enmEquipmentType = EQUIPMENT_TYPE.NA;
        public EQUIPMENT_TYPE EquipmentType
        {
            get { return _enmEquipmentType; }
            set { _enmEquipmentType = value; }
        }


        public WDConnect.ComponentModel.Alarms Alarms
        {
            //WDConnect.ComponentModel.Alarms  EquipmentModel.Alarms
            get { return EquipmentModel.Alarms; }
        }

    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public enum PROCESS_STATE
    {
        INIT        = 0,
        IDLE        = 1,
        HOME        = 2,
        EMERGENCY   = 3,
        READY       = 4,
        LOOP        = 5,
        OPERATOR    = 6,
        STEP        = 7,
        SETUP       = 8,
        ABORT       = 9

    }

    public enum CONTROL_STATE
    {
        NA                      = 0,
        OFFLINE                 = 1,
        OFFLINE_ATTEMPT_ONLINE  = 2,
        OFFLINE_HOST_OFFLINE    = 3,
        ONLINE_LOCAL            = 4,
        ONLINE_REMOTE           = 5
    }

    public enum PALLET_STATUS
    {
        NA          = 0,
        LOADING     = 1,
        LOADED      = 2,
        PROCESSING  = 3,
        PROCESSED   = 4,
        HOLDING     = 5,
        UNLOADING   = 6,
        UNLOADED    = 7
    }

    public enum EQUIPMENT_TYPE
    {
        //ASLV  , ACAM  , SAI   , APT   , ILC   , SJB   , AVI   , UNOCR
        //10    , 20    , 25    , 80    , 30    , 40    , 50    , 60
        NA      = 0,
        ASLV    = 10,
        ACAM    = 20,
        SAI     = 25,
        ILC     = 30,
        SJB     = 40,
        AVI     = 50,
        PGM     = 51,
        UNOCR   = 60,
        FVMI    = 70,
        APT     = 80
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

        public string Defects
        {
            get
            {
                return string.Join(",", _lstDefects);
            }
        }
    }


    // ////////////////////////////////////////////////////////////////////////////////////
    #region HGA
    [Serializable, XmlType(TypeName = "HGA")]
    public class HGA
    {
        [XmlElement(Type = typeof(string), ElementName = "SN")]
        private string _strSN = string.Empty;
        public string SN
        {
            get { return _strSN; }
            set { _strSN = value; }
        }

        [XmlElement("Defect")]
        private string _strDefect = string.Empty;
        public string Defect
        {
            get { return _strDefect; }
            set { _strDefect = value; }
        }

        [XmlElement("xlocACAM")]
        private double _dblxlocACAM = 0.0;
        public double xlocACAM
        {
            get { return _dblxlocACAM; }
            set { _dblxlocACAM = value; }
        }

        [XmlElement("ylocACAM")]
        private double _dblylocACAM = 0.0;
        public double ylocACAM
        {
            get { return _dblylocACAM; }
            set { _dblylocACAM = value; }
        }

        [XmlElement("skwACAM")]
        private double _dblskwACAM = 0.0;
        public double skwACAM
        {
            get { return _dblskwACAM; }
            set { _dblskwACAM = value; }
        }

        [XmlElement("xlocSAI")]
        private double _dblxlocSAI = 0.0;
        public double xlocSAI
        {
            get { return _dblxlocSAI; }
            set { _dblxlocSAI = value; }
        }

        [XmlElement("ylocSAI")]
        private double _dblylocSAI = 0.0;
        public double ylocSAI
        {
            get { return _dblylocSAI; }
            set { _dblylocSAI = value; }
        }

        [XmlElement("skwSAI")]
        private double _dblskwSAI = 0.0;
        public double skwSAI
        {
            get { return _dblskwSAI; }
            set { _dblskwSAI = value; }
        }

        #region ctor
        public HGA()
        {
        }
        #endregion

    }

    #endregion


    #region HGACollection
    [Serializable]
    public class HGACollection
    {
        private HGA[] _hga = new HGA[10];

        [XmlElement("HGA1")]
        public HGA HGA1
        {
            get { return _hga[0]; }
            set { _hga[0] = value; }
        }

        [XmlElement("HGA2")]
        public HGA HGA2
        {
            get { return _hga[1]; }
            set { _hga[1] = value; }
        }

        [XmlElement("HGA3")]
        public HGA HGA3
        {
            get { return _hga[2]; }
            set { _hga[2] = value; }
        }

        [XmlElement("HGA4")]
        public HGA HGA4
        {
            get { return _hga[3]; }
            set { _hga[3] = value; }
        }

        [XmlElement("HGA5")]
        public HGA HGA5
        {
            get { return _hga[4]; }
            set { _hga[4] = value; }
        }

        [XmlElement("HGA6")]
        public HGA HGA6
        {
            get { return _hga[5]; }
            set { _hga[5] = value; }
        }

        [XmlElement("HGA7")]
        public HGA HGA7
        {
            get { return _hga[6]; }
            set { _hga[6] = value; }
        }

        [XmlElement("HGA8")]
        public HGA HGA8
        {
            get { return _hga[7]; }
            set { _hga[7] = value; }
        }

        [XmlElement("HGA9")]
        public HGA HGA9
        {
            get { return _hga[8]; }
            set { _hga[8] = value; }
        }

        [XmlElement("HGA10")]
        public HGA HGA10
        {
            get { return _hga[9]; }
            set { _hga[9] = value; }
        }




        #region ctor
        public HGACollection()
        {
            _hga = new HGA[10];
            for (int i = 0; i < 10; i++)
            {
                _hga[i] = new HGA();
            }
        }
        #endregion
    }

    #endregion


    // ////////////////////////////////////////////////////////////////////////////////////
    public class PalletDataClass
    {
        private const int SIZE = 10;
        public HGADataClass[] _arrHGA;


        #region ctor
        public PalletDataClass()
        {
            _arrHGA = new HGADataClass[SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                _arrHGA[i] = new HGADataClass();
            }
        }


        public PalletDataClass(string strReqestPalletInfoAckXML)
        {
            _arrHGA = new HGADataClass[SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                _arrHGA[i] = new HGADataClass();
            }


            XmlDocument palletXMLDoc = new XmlDocument();
            palletXMLDoc.LoadXml(strReqestPalletInfoAckXML);

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
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PalletID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ProductName")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ProductName = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.LotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "STR")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.STR = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ACAMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ACAMID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ILCCureZone")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ILCCureZone = Convert.ToInt32(valueChildNode.InnerText, 10);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGAType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.HGAType = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "AllowedMix")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.AllowedMix = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledPallet")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }


                    //EndLot
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EndLot")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EndLot = Convert.ToBoolean(valueChildNode.InnerText);
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EquipmentType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EquipmentType = Convert.ToInt32(valueChildNode.InnerText, 10);
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "NextEquipmentType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.NextEquipmentType = Convert.ToInt32(valueChildNode.InnerText, 10);
                            }
                        }
                    }
                }
            }

            #endregion


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
                                this._arrHGA[nHGAPos]._strOCR = hgaValueNodeList[i].InnerText;
                            }
                            else if (hgaNameNodeList[i].InnerText == "Defect")
                            {
                                if (hgaValueNodeList[i].InnerText.Length > 0)
                                {
                                    //this._arrHGA[nHGAPos]._lstDefects.Add(hgaValueNodeList[i].InnerText);
                                    this._arrHGA[nHGAPos]._lstDefects.InsertRange(this._arrHGA[nHGAPos]._lstDefects.Count, hgaValueNodeList[i].InnerText.Split(','));
                                }
                            }
                        }

                    }


                }

            }


        }

        #endregion


        private int _nCOMMACK = 0;
        public int COMMACK
        {
            get { return _nCOMMACK; }
            set { _nCOMMACK = value; }
        }

        private AlarmClass _alrmObj = new AlarmClass();
        public AlarmClass Alarm
        {
            get { return _alrmObj; }
            set { _alrmObj = value; }
        }

        private string _strPalletID = "";
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; ;}
        }

        private string _strPartNumber = "";
        public string PartNumber
        {
            get { return _strPartNumber; }
            set { _strPartNumber = value; }
        }

        private string _strLotNumber = "";
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private string _strProductName = "";
        public string ProductName
        {
            get { return _strProductName; }
            set { _strProductName = value; }
        }

        private string _strLine = "";
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
        }

        private string _strACAMID = "";
        public string ACAMID
        {
            get { return _strACAMID; }
            set { _strACAMID = value; }
        }

        private string _strHGAType = "";
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }

        private string _strSTR = "";
        public string STR
        {
            get { return _strSTR; }
            set { _strSTR = value; }
        }

        private string _strSuspension = "";
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        private bool _bAllowedMix = false;
        public bool AllowedMix
        {
            get { return _bAllowedMix; }
            set { _bAllowedMix = value; }
        }

        private bool _bEnabled = true;
        public bool Enabled
        {
            get { return _bEnabled; }
            set { _bEnabled = value; }
        }

        private bool _bEndLot = false;
        public bool EndLot
        {
            get { return _bEndLot; }
            set { _bEndLot = value; }
        }

        private int _nILCCureZone = 0;
        public int ILCCureZone
        {
            get { return _nILCCureZone; }
            set { _nILCCureZone = value; }
        }

        private int _nEquipmentType = 0;
        public int EquipmentType
        {
            get { return _nEquipmentType; }
            set { _nEquipmentType = value; }
        }

        private int _nNextEquipmentType = 0;
        public int NextEquipmentType
        {
            get { return _nNextEquipmentType; }
            set { _nNextEquipmentType = value; }
        }

        private int _nTransID = 0;
        public int TransID
        {
            get { return _nTransID; }
            set { _nTransID = value; }
        }

        private RequestSuspensionClass _reqSuspObj = new RequestSuspensionClass();
        public RequestSuspensionClass ReqSusp
        {
            get { return _reqSuspObj; }
            set { _reqSuspObj = value; }
        }

    }


    // ////////////////////////////////////////////////////////////////////////////////////
    #region RequestPalletInfoAckObj
    [Serializable]
    public class RequestPalletInfoAckObj
    {
        [XmlElement("COMMACK")]
        private int _nCOMMACK = 0;
        public int COMMACK
        {
            get { return _nCOMMACK; }
            set { _nCOMMACK = value; }
        }

        [XmlElement("ALMID")]
        private int _nALMID = 0;
        public int ALMID
        {
            get { return _nALMID; }
            set { _nALMID = value; }
        }

        [XmlElement("PalletID")]
        private string _strPalletID = string.Empty;
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; }
        }

        [XmlElement("PartNumber")]
        private string _strPartNumber = string.Empty;
        public string PartNumber
        {
            get { return _strPartNumber; }
            set { _strPartNumber = value; }
        }

        [XmlElement("ProductName")]
        private string _strProductName = string.Empty;
        public string ProductName
        {
            get { return _strProductName; }
            set { _strProductName = value; }
        }

        [XmlElement("LotNumber")]
        private string _strLotNumber = string.Empty;
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        [XmlElement("STR")]
        private string _strSTR = string.Empty;
        public string STR
        {
            get { return _strSTR; }
            set { _strSTR = value; }
        }

        [XmlElement("Line")]
        private string _strLine = string.Empty;
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
        }

        [XmlElement("ACAMID")]
        private string _strACAMID = string.Empty;
        public string ACAMID
        {
            get { return _strACAMID; }
            set { _strACAMID = value; }
        }

        [XmlElement("Suspension")]
        private string _strSuspension = string.Empty;
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        [XmlElement("EnabledPallet")]
        private bool _bEnabledPallet = false;
        public bool EnabledPallet
        {
            get { return _bEnabledPallet; }
            set { _bEnabledPallet = value; }
        }

        [XmlElement("HGAType")]
        private string _strHGAType = string.Empty;
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }

        [XmlElement("EquipmentType")]
        private int _nEquipmentType = 0;
        public int EquipmentType
        {
            get { return _nEquipmentType; }
            set { _nEquipmentType = value; }
        }

        [XmlElement("NextEquipmentType")]
        private int _nNextEquipmentType = 0;
        public int NextEquipmentType
        {
            get { return _nNextEquipmentType; }
            set { _nNextEquipmentType = value; }
        }

        [XmlElement("EndLot")]
        private bool _bEndLot = false;
        public bool EndLot
        {
            get { return _bEndLot; }
            set { _bEndLot = value; }
        }

        [XmlElement("AllowedMix")]
        private bool _bAllowedMix = false;
        public bool AllowedMix
        {
            get { return _bAllowedMix; }
            set { _bAllowedMix = value; }
        }


        [XmlElement("UVPower")]
        private int _nUVPower = 0;
        public int UVPower
        {
            get { return _nUVPower; }
            set { _nUVPower = value; }
        }

        [XmlElement("CureTime")]
        private int _nCureTime = 0;
        public int CureTime
        {
            get { return _nCureTime; }
            set { _nCureTime = value; }
        }

        [XmlElement("CureZone")]
        private int _nCureZone = 0;
        public int CureZone
        {
            get { return _nCureZone; }
            set { _nCureZone = value; }
        }

        [XmlElement("SJBStage")]
        private string _strSJBStage = string.Empty;
        public string SJBStage
        {
            get { return _strSJBStage; }
            set { _strSJBStage = value; }
        }

        [XmlElement("HGA")]
        private HGACollection _hgas = new HGACollection();
        public HGACollection HGA
        {
            get { return _hgas; }
            set { _hgas = value; }
        }

        [XmlElement("TransID")]
        private int _nTransID = 0;
        public int TransID
        {
            get { return _nTransID; }
            set { _nTransID = value; }
        }

        private RequestSuspensionClass _reqSuspObj = new RequestSuspensionClass();
        public RequestSuspensionClass ReqSusp
        {
            get { return _reqSuspObj; }
            set { _reqSuspObj = value; }
        }


        #region ctor
        public RequestPalletInfoAckObj()
        {
        }


        #endregion

        public WDConnect.Common.SCITransaction ToSCITrans()
        {
            SCIMessage scndMsgRequestPalletInfo = new SCIMessage();
            scndMsgRequestPalletInfo.CommandID = "RequestPalletInfoAck";
            scndMsgRequestPalletInfo.Item = new SCIItem();
            scndMsgRequestPalletInfo.Item.Format = SCIFormat.List;
            scndMsgRequestPalletInfo.Item.Items = new SCIItemCollection();

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = this.PalletID /*reqPallet.PalletID*/ });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "LotNumber", Value = this.LotNumber });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PartNumber", Value = this.PartNumber });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ProductName", Value = this.ProductName });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Suspension", Value = this.Suspension });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "STR", Value = this.STR });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Line", Value = this.Line });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "ACAMID", Value = this.ACAMID });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCUVPower", Value = this.UVPower });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCCureTime", Value = this.CureTime });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ILCCureZone", Value = this.CureZone });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SJBStage", Value = this.SJBStage });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EnabledPallet", Value = this.EnabledPallet });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "EquipmentType", Value = this.EquipmentType });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "NextEquipmentType", Value = this.NextEquipmentType });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "EndLot", Value = this.EndLot });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Boolean, Name = "AllowedMix", Value = this.AllowedMix });

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "HGAType", Value = this.HGAType });


            //hga data portion
            SCIItem hgaListItem = new SCIItem();
            hgaListItem.Format = SCIFormat.List;
            hgaListItem.Name = "HGA";
            hgaListItem.Items = new SCIItemCollection();

            scndMsgRequestPalletInfo.Item.Items.Add(hgaListItem);


            HGA[] hgas = new HGA[10];
            hgas[0] = new HGA();
            hgas[0] = this.HGA.HGA1;
            hgas[1] = new HGA();
            hgas[1] = this.HGA.HGA2;
            hgas[2] = new HGA();
            hgas[2] = this.HGA.HGA3;
            hgas[3] = new HGA();
            hgas[3] = this.HGA.HGA4;
            hgas[4] = new HGA();
            hgas[4] = this.HGA.HGA5;
            hgas[5] = new HGA();
            hgas[5] = this.HGA.HGA6;
            hgas[6] = new HGA();
            hgas[6] = this.HGA.HGA7;
            hgas[7] = new HGA();
            hgas[7] = this.HGA.HGA8;
            hgas[8] = new HGA();
            hgas[8] = this.HGA.HGA9;
            hgas[9] = new HGA();
            hgas[9] = this.HGA.HGA10;

            for (int j = 0; j < 10; j++)
            {
                SCIItem hgaItem = new SCIItem();
                hgaItem.Format = SCIFormat.List;
                hgaItem.Name = "HGA" + j.ToString();
                hgaItem.Value = "";
                hgaItem.Items = new SCIItemCollection();
                hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "SN", Value = hgas[j].SN });
                hgaItem.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "Defect", Value = hgas[j].Defect });

                //add HGA1 - HGA10
                hgaListItem.Items.Add(hgaItem);
            }
            //hga data portion

            WDConnect.Common.SCITransaction toReplyRequestPalletInfo = new WDConnect.Common.SCITransaction()
            {
                DeviceId = 0, //e.Transaction.DeviceId,
                MessageType = MessageType.Secondary,
                Id = 0, //e.Transaction.Id,
                Name = "RequestPalletInfoAck",
                NeedReply = false,
                Primary = null, //e.Transaction.Primary,
                Secondary = scndMsgRequestPalletInfo
            };

            return toReplyRequestPalletInfo;
        }

        private string GenSerialNumberFromLot(string strLotNumber)
        {
            System.Threading.Thread.Sleep(50);
            Random rnd = new Random(Environment.TickCount);
            string strSimulatedSerialNumber = strLotNumber.Substring(0, 4);

            for (int i = 0; i < 4; i++)
            {
                strSimulatedSerialNumber += ((char)rnd.Next(65, 91)).ToString();
            }

            Console.WriteLine(strSimulatedSerialNumber);
            return strSimulatedSerialNumber;
        }

        public string ToXML()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

                writer.WriteStartDocument();
                x.Serialize(writer, this);
            }

            return sb.ToString();
        }

        public static RequestPalletInfoAckObj ReadFromFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new RequestPalletInfoAckObj();
            }


            string strXML = System.IO.File.ReadAllText(strFilePath);

            return RequestPalletInfoAckObj.ToObj(strXML);
        }

        public static RequestPalletInfoAckObj ToObj(string strXML)
        {
            RequestPalletInfoAckObj obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(RequestPalletInfoAckObj));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (RequestPalletInfoAckObj)x.Deserialize(reader);
            }

            return obj;
        }

        public static RequestPalletInfoAckObj ToObj(SCIMessage sciMsgRequestPalletInfoAck)
        {
            RequestPalletInfoAckObj tempObj = new RequestPalletInfoAckObj();

            if (sciMsgRequestPalletInfoAck.CommandID == "RequestPalletInfoAck")
            { 
                foreach (WDConnect.Common.SCIItem itm in sciMsgRequestPalletInfoAck.Item.Items)
                {
                    switch (itm.Name)
                    {
                        case "RequestSuspension":

                            //Console.WriteLine(Boolean.Parse(itm.Value.ToString()).ToString());
                            tempObj.ReqSusp.IsRequesting = Boolean.Parse(itm.Value.ToString());
                            foreach (WDConnect.Common.SCIItem reqSuspItem in itm.Items)
                            {
                                switch (reqSuspItem.Name)
                                {
                                    case "ACAMID":
                                        //Console.WriteLine(reqSuspItem.Value);
                                        tempObj.ReqSusp.ACAMID = reqSuspItem.Value.ToString();
                                        break;

                                    case "SuspAmt":
                                        //Console.WriteLine(reqSuspItem.Value);
                                        tempObj.ReqSusp.SuspAmt = Int32.Parse(reqSuspItem.Value.ToString());
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        case "COMMACK":
                            tempObj.COMMACK = (int)itm.Value;
                            break;

                        case "ALMID":
                            tempObj.ALMID = (int)itm.Value;
                            break;

                        case "PalletID":
                            tempObj.PalletID = (string)itm.Value;
                            break;

                        case "LotNumber":
                            tempObj.LotNumber = (string)itm.Value;
                            break;

                        case "PartNumber":
                            tempObj.PartNumber = (string)itm.Value;
                            break;

                        case "ProductName":
                            tempObj.ProductName = (string)itm.Value;
                            break;

                        case "Suspension":
                            tempObj.Suspension = (string)itm.Value;
                            break;

                        case "STR":
                            tempObj.STR = (string)itm.Value;
                            break;

                        case "Line":
                            tempObj.Line = (string)itm.Value;
                            break;

                        case "ACAMID":
                            tempObj.ACAMID = (string)itm.Value;
                            break;

                        case "ILCUVPower":
                            tempObj.UVPower = (int)itm.Value;
                            break;

                        case "ILCCureTime":
                            tempObj.CureTime = (int)itm.Value;
                            break;

                        case "ILCCureZone":
                            tempObj.CureZone = (int)itm.Value;
                            break;

                        case "SJBStage":
                            tempObj.SJBStage = (string)itm.Value;
                            break;

                        case "EnabledPallet":
                            tempObj.EnabledPallet = (bool)itm.Value;
                            break;

                        case "EquipmentType":
                            tempObj.EquipmentType = (int)itm.Value;
                            break;

                        case "NextEquipmentType":
                            tempObj.NextEquipmentType = (int)itm.Value;
                            break;

                        case "EndLot":
                            tempObj.EndLot = (bool)itm.Value;
                            break;

                        case "AllowedMix":
                            tempObj.AllowedMix = (bool)itm.Value;
                            break;

                        case "HGAType":
                            tempObj.HGAType = (string)itm.Value;
                            break;

                        case "TransID":
                            tempObj.TransID = (int)itm.Value;
                            break;

                        case "HGA":
                            foreach (WDConnect.Common.SCIItem hga in itm.Items)
                            {
                                switch (hga.Name)
                                {
                                    case "HGA1":
                                        tempObj.HGA.HGA1.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA1.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA2":
                                        tempObj.HGA.HGA2.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA2.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA3":
                                        tempObj.HGA.HGA3.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA3.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA4":
                                        tempObj.HGA.HGA4.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA4.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA5":
                                        tempObj.HGA.HGA5.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA5.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA6":
                                        tempObj.HGA.HGA6.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA6.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA7":
                                        tempObj.HGA.HGA7.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA7.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA8":
                                        tempObj.HGA.HGA8.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA8.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA9":
                                        tempObj.HGA.HGA9.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA9.Defect = (string)hga.Items[1].Value;
                                        break;

                                    case "HGA10":
                                        tempObj.HGA.HGA10.SN = (string)hga.Items[0].Value;
                                        tempObj.HGA.HGA10.Defect = (string)hga.Items[1].Value;
                                        break;


                                    default:
                                        break;
                                }
                            }

                            break;


                        default:
                            break;
                    }

                }

            }

            return tempObj;
        }

    }

    #endregion


    // ////////////////////////////////////////////////////////////////////////////////////
    public enum SJB_FIXTURE
    {
        UNDEFINED   = 0,
        LEFT        = 1,
        RIGHT       = 2
    };

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

        private string _strSliderTrayID = "";
        public string SliderTrayID
        {
            get { return _strSliderTrayID; }
            set { _strSliderTrayID = value; }
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

        private SJB_FIXTURE _nFixture = SJB_FIXTURE.UNDEFINED;
        public SJB_FIXTURE SJBFixture
        {
            get { return _nFixture; }
            set { _nFixture = value; }
        }

        private RequestSuspensionClass _reqSuspObj = new RequestSuspensionClass();
        public RequestSuspensionClass ReqSusp
        {
            get { return _reqSuspObj; }
            set { _reqSuspObj = value; }
        }

    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public class RequestSuspensionClass
    {

        #region ctor
        public RequestSuspensionClass()
        {
        }

        public RequestSuspensionClass(string strRequestSuspensionClassXML)
        {
            XmlDocument reqSuspXMLDoc = new XmlDocument();
            reqSuspXMLDoc.LoadXml(strRequestSuspensionClassXML);

            XmlElement root_reqSuspXMLDoc = reqSuspXMLDoc.DocumentElement;

            XmlNodeList reqSuspSCIItemsNodeList = root_reqSuspXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            #region reqSuspSCIItemsNodeList Primary SCIMessage
            if (reqSuspSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in reqSuspSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==ACAMID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==APT001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ACAMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ACAMID = valueChildNode.InnerText;
                            }
                        }

                        if (nameChildNode.InnerText == "SuspAmt")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.SuspAmt = Int32.Parse(valueChildNode.InnerText);

                                if (SuspAmt > 0)
                                {
                                    this.IsRequesting = true;
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region reqSuspSCIItemsNodeList Secondary SCIMessage
            reqSuspSCIItemsNodeList = root_reqSuspXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem/Items/SCIItem");
            if (reqSuspSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in reqSuspSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==ACAMID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==APT001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ACAMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ACAMID = valueChildNode.InnerText;
                            }
                        }

                        if (nameChildNode.InnerText == "SuspAmt")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.SuspAmt = Int32.Parse(valueChildNode.InnerText);

                                if (this.SuspAmt > 0)
                                {
                                    this.IsRequesting = true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

        }

        #endregion


        private int _nSuspAmt = 0;
        public int SuspAmt
        {
            get { return _nSuspAmt; }
            set { _nSuspAmt = value; }
        }

        private string _strACAMID = string.Empty;
        public string ACAMID
        {
            get { return _strACAMID; }
            set { _strACAMID = value; }
        }

        private bool _bIsRequesting = false;
        public bool IsRequesting
        {
            get { return _bIsRequesting; }
            set { _bIsRequesting = value; }
        }

    }


    public class RequestSuspensionAckClass
    {

        #region ctor
        public RequestSuspensionAckClass()
        {
        }


        #endregion

        private int _nCOMMACK = 0;
        public int COMMACK
        {
            get { return _nCOMMACK; }
            set { _nCOMMACK = value; }
        }



        public static RequestSuspensionAckClass ToObj(string strXML)
        {
            RequestSuspensionAckClass obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(RequestSuspensionAckClass));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (RequestSuspensionAckClass)x.Deserialize(reader);
            }

            return obj;
        }

    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public class TrayDataClass
    {
        private int _nTraySize = 0;
        public int TraySize
        {
            get {return _nTraySize; }
            set { _nTraySize = value; }
        }
        

        public HGADataClass[] _arrHGA;


        #region ctor
        public TrayDataClass()
        {
            _nTraySize = 20;
            _arrHGA = new HGADataClass[_nTraySize];

            for (int i = 0; i < _nTraySize; i++)
            {
                _arrHGA[i] = new HGADataClass();
            }
        }

        public TrayDataClass(int nTraySize)
        {
            _nTraySize = nTraySize;
            _arrHGA = new HGADataClass[_nTraySize];

            for (int i = 0; i < _nTraySize; i++)
            {
                _arrHGA[i] = new HGADataClass();
            }
        }

        #endregion


        private string _strTrayID = "";
        public string TrayID
        {
            get { return _strTrayID; }
            set { _strTrayID = value; }
        }

        private string _strLotNumber = "";
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private string _strLine = "";
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
        }

        private string _strSuspension = "";
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        private string _strProduct = "";
        public string Product
        {
            get { return _strProduct; }
            set { _strProduct = value; }
        }

        private string _strPartNumber = "";
        public string PartNumber
        {
            get { return _strPartNumber; }
            set { _strPartNumber = value; }
        }

        private string _strSTR = "";
        public string STR
        {
            get { return _strSTR; }
            set { _strSTR = value; }
        }

        private string _strPalletID11 = "";
        public string PalletID_RowID11
        {
            get { return _strPalletID11; }
            set { _strPalletID11 = value; }
        }

        private string _strPalletID12 = "";
        public string PalletID_RowID12
        {
            get { return _strPalletID12; }
            set { _strPalletID12 = value; }
        }

        private string _strPalletID21 = "";
        public string PalletID_RowID21
        {
            get { return _strPalletID21; }
            set { _strPalletID21 = value; }
        }

        private string _strPalletID22 = "";
        public string PalletID_RowID22
        {
            get { return _strPalletID22; }
            set { _strPalletID22 = value; }
        }

        private string _strPalletID31 = "";
        public string PalletID_RowID31
        {
            get { return _strPalletID31; }
            set { _strPalletID31 = value; }
        }

        private string _strPalletID32 = "";
        public string PalletID_RowID32
        {
            get { return _strPalletID32; }
            set { _strPalletID32 = value; }
        }

        
        private DateTime _dtUnloadingStartTime = new DateTime();
        public DateTime UnloadStartTime
        {
            get { return _dtUnloadingStartTime; }
            set { _dtUnloadingStartTime = value; }
        }


        private DateTime _dtUnloadFinishTime = new DateTime();
        public DateTime UnloadFinishTime
        {
            get { return _dtUnloadFinishTime; }
            set { _dtUnloadFinishTime = value; }
        }


        public void UnloadPalletToRowID(int nRowID, PalletDataClass pallet)
        {
            //if(nRowID > _nTraySize)
            //{
            //    Console.WriteLine("Error, please check tray size: " + _nTraySize.ToString());
            //    return;
            //}


            //for traysize 40
            //nRowID 11, HGA 0-9
            //nRowID 12, HGA 10-19
            //nRowID 21, HGA 20-29
            //nRowID 22, HGA 30-39

            //for traysize 60
            //nRowID 11, HGA 0-9
            //nRowID 12, HGA 10-19
            //nRowID 21, HGA 20-29
            //nRowID 22, HGA 30-39
            //nRowID 31, HGA 40-49
            //nRowID 32, HGA 50-59

            switch (nRowID)
            {
                case 11:    // 1-10                                         //for traysize 20
                    for (int i = 0; i < 10; i++)                            //nRowID 11, HGA 0-9
                    {                                                       //nRowID 21, HGA 10-19
                        _arrHGA[i] = pallet._arrHGA[i];
                    }

                    this.PalletID_RowID11 = pallet.PalletID;

                    this.Line = pallet.Line;
                    this.LotNumber = pallet.LotNumber;
                    this.PartNumber = pallet.PartNumber;
                    this.Product = pallet.ProductName;
                    this.STR = pallet.STR;

                    break;


                case 12:
                    if (this.TraySize > 20)     //10-19
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            _arrHGA[i + 10] = pallet._arrHGA[i];
                        }

                        this.PalletID_RowID12 = pallet.PalletID;
                    }
                    else                        //n/a, not defined for tray 20
                    {
                    }

                    break;


                case 21:    
                    if (this.TraySize > 20)     //20-29, for tray 40, 60
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            _arrHGA[i + 20] = pallet._arrHGA[i];
                        }
                    }
                    else                        //10-19, for tray 20
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            _arrHGA[i + 10] = pallet._arrHGA[i];
                        }
                    }

                    this.PalletID_RowID21 = pallet.PalletID;
                    break;


                case 22:                        //30-39
                    for (int i = 0; i < 10; i++)
                    {
                        _arrHGA[i + 30] = pallet._arrHGA[i];
                    }

                    this.PalletID_RowID22 = pallet.PalletID;
                    break;


                case 31:                        //40-49
                    for (int i = 0; i < 10; i++)
                    {
                        _arrHGA[i + 40] = pallet._arrHGA[i];
                    }

                    this.PalletID_RowID31 = pallet.PalletID;
                    break;


                case 32:                        //50-59
                    for (int i = 0; i < 10; i++)
                    {
                        _arrHGA[i + 50] = pallet._arrHGA[i];
                    }

                    this.PalletID_RowID32 = pallet.PalletID;
                    break;


                default:
                    break;
            }
        }
    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public class ProcessRecipeDataClass
    {
        #region ctor
        public ProcessRecipeDataClass()
        {
        }

        public ProcessRecipeDataClass(string strRequestProcessRecipeAckXML)
        {
            XmlDocument procRecipeXMLDoc = new XmlDocument();
            procRecipeXMLDoc.LoadXml(strRequestProcessRecipeAckXML);

            XmlElement root_palletXMLDoc = procRecipeXMLDoc.DocumentElement;

            XmlNodeList procRecipetInfoSCIItemsNodeList = root_palletXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem");
            #region palletInfoSCIItemsNodeList
            if (procRecipetInfoSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in procRecipetInfoSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==SF0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PalletID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.LotNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "LotSize")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.LotSize = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "RecipeID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.RecipeID = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "RecipeName")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.RecipeName = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Line")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Line = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "UVPower")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.UVPower = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureTime")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.CureTime = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureZone")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.CureZone = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "ProductName")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ProductName = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Suspension")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Suspension = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "SuspPartNumber")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.SuspPartNumber = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "STR")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.STR = valueChildNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "HGAType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.HGAType = valueChildNode.InnerText;
                            }
                        }
                    }


                    // for ILC
                    // UV1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Power_UV1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PowerUV1 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureTime_UV1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.CureTimeUV1 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_UV1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledUV1 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                    //Heater1
                    //public double FlowRate_Heater1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater1 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater1 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater1 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater1
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater1")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater1 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater2
                    //public double FlowRate_Heater2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater2 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater2 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater2 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater2 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater3
                    //public double FlowRate_Heater3
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater3")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater3 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater3
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater3")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater3 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater3
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater3")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater3 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater3
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater3")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater3 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater4
                    //public double FlowRate_Heater4
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater4")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater4 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater4
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater4")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater4 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater4
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater4")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater4 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater4
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater4")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater4 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater5
                    //public double FlowRate_Heater5
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater5")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater5 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater5
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater5")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater5 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater5
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater5")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater5 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater5
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater5")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater5 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    // UV2
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Power_UV2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PowerUV2 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "CureTime_UV2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.CureTimeUV2 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_UV2")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledUV2 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater6
                    //public double FlowRate_Heater6
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater6")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater6 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater6
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater6")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater6 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater6
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater6")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater6 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater6
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater6")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater6 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater7
                    //public double FlowRate_Heater7
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater7")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater7 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater7
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater7")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater7 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater7
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater7")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater7 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater7
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater7")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater7 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater8
                    //public double FlowRate_Heater8
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater8")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater8 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater8
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater8")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater8 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater8
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater8")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater8 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater8
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater8")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater8 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater9
                    //public double FlowRate_Heater9
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater9")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater9 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater9
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater9")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater9 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater9
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater9")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater9 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater9
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater9")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater9 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    //Heater10
                    //public double FlowRate_Heater10
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "FlowRate_Heater10")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.FlowRate_Heater10 = Double.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Temp_Heater10
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Temp_Heater10")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Temp_Heater10 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int Enabled_Heater10
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Enabled_Heater10")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Enabled_Heater10 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }

                    //public int EnabledN2_Heater10
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "EnabledN2_Heater10")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EnabledN2_Heater10 = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }



                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Mode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Mode = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "Bypass")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.Bypass = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                }
            }

            #endregion
        }

        #endregion

        private int _nCOMMACK = 0;
        public int COMMACK
        {
            get { return _nCOMMACK; }
            set { _nCOMMACK = value; }
        }

        private AlarmClass _alrmObj = new AlarmClass();
        public AlarmClass Alarm
        {
            get { return _alrmObj; }
            set { _alrmObj = value; }
        }

        private string _strRecipeID = "";
        public string RecipeID
        {
            get { return _strRecipeID; }
            set { _strRecipeID = value; }
        }

        private string _strRecipeName = "";
        public string RecipeName
        {
            get { return _strRecipeName; }
            set { _strRecipeName = value; }
        }

        private string _strLotNumber = "";
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private int _nLotSize = 0;
        public int LotSize
        {
            get { return _nLotSize; }
            set { _nLotSize = value; }
        }

        private string _strLine = "";
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
        }

        private string _strPalletID = "";
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; }
        }

        private string _strSuspension = "";
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        private string _strSuspPartNumber = "";
        public string SuspPartNumber
        {
            get { return _strSuspPartNumber; }
            set { _strSuspPartNumber = value; }
        }

        private string _strPartNumber = "";
        public string PartNumber
        {
            get { return _strPartNumber; }
            set { _strPartNumber = value; }
        }

        private string _strProductName = "";
        public string ProductName
        {
            get { return _strProductName; }
            set { _strProductName = value; }
        }

        private string _strSTR = string.Empty;
        public string STR
        {
            get { return _strSTR; }
            set { _strSTR = value; }
        }

        private string _strHGAType = string.Empty;
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }

        // for ILC //////////////////////////////////
        #region for ILC
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


        #region UV1
        // UV1
        private int _nPowerUV1 = 0;
        public int PowerUV1
        {
            get { return _nPowerUV1; }
            set { _nPowerUV1 = value; }
        }

        private double _dblCureTimeUV1 = 0.0;
        public double CureTimeUV1
        {
            get { return _dblCureTimeUV1; }
            set { _dblCureTimeUV1 = value; }
        }

        private int _nEnabledUV1 = 0;
        public int EnabledUV1
        {
            get { return _nEnabledUV1; }
            set { _nEnabledUV1 = value; }
        }
        #endregion


        #region Heater1
        // Heater1
        private double _dblFlowRate_Heater1 = 0.0;
        public double FlowRate_Heater1
        {
            get { return _dblFlowRate_Heater1; }
            set { _dblFlowRate_Heater1 = value; }
        }
        
        private int _nTemp_Heater1 = 0;
        public int Temp_Heater1
        {
            get { return _nTemp_Heater1; }
            set { _nTemp_Heater1 = value; }
        }

        private int _nEnabled_Heater1 = 0;
        public int Enabled_Heater1
        {
            get { return _nEnabled_Heater1; }
            set { _nEnabled_Heater1 = value; }
        }

        private int _nEnabledN2_Heater1 = 0;
        public int EnabledN2_Heater1
        {
            get { return _nEnabledN2_Heater1; }
            set { _nEnabledN2_Heater1 = value; }
        }
        #endregion


        #region Heater2
        // Heater2
        private double _dblFlowRate_Heater2 = 0.0;
        public double FlowRate_Heater2
        {
            get { return _dblFlowRate_Heater2; }
            set { _dblFlowRate_Heater2 = value; }
        }

        private int _nTemp_Heater2 = 0;
        public int Temp_Heater2
        {
            get { return _nTemp_Heater2; }
            set { _nTemp_Heater2 = value; }
        }

        private int _nEnabled_Heater2 = 0;
        public int Enabled_Heater2
        {
            get { return _nEnabled_Heater2; }
            set { _nEnabled_Heater2 = value; }
        }

        private int _nEnabledN2_Heater2 = 0;
        public int EnabledN2_Heater2
        {
            get { return _nEnabledN2_Heater2; }
            set { _nEnabledN2_Heater2 = value; }
        }
        #endregion


        #region Heater3
        // Heater3
        private double _dblFlowRate_Heater3 = 0.0;
        public double FlowRate_Heater3
        {
            get { return _dblFlowRate_Heater3; }
            set { _dblFlowRate_Heater3 = value; }
        }

        private int _nTemp_Heater3 = 0;
        public int Temp_Heater3
        {
            get { return _nTemp_Heater3; }
            set { _nTemp_Heater3 = value; }
        }

        private int _nEnabled_Heater3 = 0;
        public int Enabled_Heater3
        {
            get { return _nEnabled_Heater3; }
            set { _nEnabled_Heater3 = value; }
        }

        private int _nEnabledN2_Heater3 = 0;
        public int EnabledN2_Heater3
        {
            get { return _nEnabledN2_Heater3; }
            set { _nEnabledN2_Heater3 = value; }
        }
        #endregion


        #region Heater4
        // Heater4
        private double _dblFlowRate_Heater4 = 0.0;
        public double FlowRate_Heater4
        {
            get { return _dblFlowRate_Heater4; }
            set { _dblFlowRate_Heater4 = value; }
        }

        private int _nTemp_Heater4 = 0;
        public int Temp_Heater4
        {
            get { return _nTemp_Heater4; }
            set { _nTemp_Heater4 = value; }
        }

        private int _nEnabled_Heater4 = 0;
        public int Enabled_Heater4
        {
            get { return _nEnabled_Heater4; }
            set { _nEnabled_Heater4 = value; }
        }

        private int _nEnabledN2_Heater4 = 0;
        public int EnabledN2_Heater4
        {
            get { return _nEnabledN2_Heater4; }
            set { _nEnabledN2_Heater4 = value; }
        }
        #endregion


        #region Heater5
        // Heater5
        private double _dblFlowRate_Heater5 = 0.0;
        public double FlowRate_Heater5
        {
            get { return _dblFlowRate_Heater5; }
            set { _dblFlowRate_Heater5 = value; }
        }

        private int _nTemp_Heater5 = 0;
        public int Temp_Heater5
        {
            get { return _nTemp_Heater5; }
            set { _nTemp_Heater5 = value; }
        }

        private int _nEnabled_Heater5 = 0;
        public int Enabled_Heater5
        {
            get { return _nEnabled_Heater5; }
            set { _nEnabled_Heater5 = value; }
        }

        private int _nEnabledN2_Heater5 = 0;
        public int EnabledN2_Heater5
        {
            get { return _nEnabledN2_Heater5; }
            set { _nEnabledN2_Heater5 = value; }
        }
        #endregion



        #region UV2
        // UV2
        private int _nPowerUV2 = 0;
        public int PowerUV2
        {
            get { return _nPowerUV2; }
            set { _nPowerUV2 = value; }
        }

        private double _dblCureTimeUV2 = 0.0;
        public double CureTimeUV2
        {
            get { return _dblCureTimeUV2; }
            set { _dblCureTimeUV2 = value; }
        }

        private int _nEnabledUV2 = 0;
        public int EnabledUV2
        {
            get { return _nEnabledUV2; }
            set { _nEnabledUV2 = value; }
        }
        #endregion


        #region Heater6
        // Heater6
        private double _dblFlowRate_Heater6 = 0.0;
        public double FlowRate_Heater6
        {
            get { return _dblFlowRate_Heater6; }
            set { _dblFlowRate_Heater6 = value; }
        }

        private int _nTemp_Heater6 = 0;
        public int Temp_Heater6
        {
            get { return _nTemp_Heater6; }
            set { _nTemp_Heater6 = value; }
        }

        private int _nEnabled_Heater6 = 0;
        public int Enabled_Heater6
        {
            get { return _nEnabled_Heater6; }
            set { _nEnabled_Heater6 = value; }
        }

        private int _nEnabledN2_Heater6 = 0;
        public int EnabledN2_Heater6
        {
            get { return _nEnabledN2_Heater6; }
            set { _nEnabledN2_Heater6 = value; }
        }
        #endregion


        #region Heater7
        // Heater7
        private double _dblFlowRate_Heater7 = 0.0;
        public double FlowRate_Heater7
        {
            get { return _dblFlowRate_Heater7; }
            set { _dblFlowRate_Heater7 = value; }
        }

        private int _nTemp_Heater7 = 0;
        public int Temp_Heater7
        {
            get { return _nTemp_Heater7; }
            set { _nTemp_Heater7 = value; }
        }

        private int _nEnabled_Heater7 = 0;
        public int Enabled_Heater7
        {
            get { return _nEnabled_Heater7; }
            set { _nEnabled_Heater7 = value; }
        }

        private int _nEnabledN2_Heater7 = 0;
        public int EnabledN2_Heater7
        {
            get { return _nEnabledN2_Heater7; }
            set { _nEnabledN2_Heater7 = value; }
        }
        #endregion


        #region Heater8
        // Heater8
        private double _dblFlowRate_Heater8 = 0.0;
        public double FlowRate_Heater8
        {
            get { return _dblFlowRate_Heater8; }
            set { _dblFlowRate_Heater8 = value; }
        }

        private int _nTemp_Heater8 = 0;
        public int Temp_Heater8
        {
            get { return _nTemp_Heater8; }
            set { _nTemp_Heater8 = value; }
        }

        private int _nEnabled_Heater8 = 0;
        public int Enabled_Heater8
        {
            get { return _nEnabled_Heater8; }
            set { _nEnabled_Heater8 = value; }
        }

        private int _nEnabledN2_Heater8 = 0;
        public int EnabledN2_Heater8
        {
            get { return _nEnabledN2_Heater8; }
            set { _nEnabledN2_Heater8 = value; }
        }
        #endregion


        #region Heater9
        // Heater9
        private double _dblFlowRate_Heater9 = 0.0;
        public double FlowRate_Heater9
        {
            get { return _dblFlowRate_Heater9; }
            set { _dblFlowRate_Heater9 = value; }
        }

        private int _nTemp_Heater9 = 0;
        public int Temp_Heater9
        {
            get { return _nTemp_Heater9; }
            set { _nTemp_Heater9 = value; }
        }

        private int _nEnabled_Heater9 = 0;
        public int Enabled_Heater9
        {
            get { return _nEnabled_Heater9; }
            set { _nEnabled_Heater9 = value; }
        }

        private int _nEnabledN2_Heater9 = 0;
        public int EnabledN2_Heater9
        {
            get { return _nEnabledN2_Heater9; }
            set { _nEnabledN2_Heater9 = value; }
        }
        #endregion


        #region Heater10
        // Heater10
        private double _dblFlowRate_Heater10 = 0.0;
        public double FlowRate_Heater10
        {
            get { return _dblFlowRate_Heater10; }
            set { _dblFlowRate_Heater10 = value; }
        }

        private int _nTemp_Heater10 = 0;
        public int Temp_Heater10
        {
            get { return _nTemp_Heater10; }
            set { _nTemp_Heater10 = value; }
        }

        private int _nEnabled_Heater10 = 0;
        public int Enabled_Heater10
        {
            get { return _nEnabled_Heater10; }
            set { _nEnabled_Heater10 = value; }
        }

        private int _nEnabledN2_Heater10 = 0;
        public int EnabledN2_Heater10
        {
            get { return _nEnabledN2_Heater10; }
            set { _nEnabledN2_Heater10 = value; }
        }
        #endregion


        private int _nMode = 0;
        public int Mode
        {
            get { return _nMode; }
            set { _nMode = value; }
        }


        private int _nBypass = 0;
        public int Bypass
        {
            get { return _nBypass; }
            set { _nBypass = value; }
        }

        #endregion for ILC
        // /////////////////////////////////////////
    }


    // ////////////////////////////////////////////////////////////////////////////////////
    public class AlarmClass
    {
        private string _strAlarmID = "";
        private string _strAlarmName = "";
        private string _strAlarmDescritption = "";

        #region ctor
        public AlarmClass()
        {
        }
        #endregion

        public string AlarmID
        {
            get { return _strAlarmID; }
            set { _strAlarmID = value; }
        }

        public string AlarmName
        {
            get { return _strAlarmName; }
            set { _strAlarmName = value; }
        }

        public string AlarmDescription
        {
            get { return _strAlarmDescritption; }
            set { _strAlarmDescritption = value; }
        }
    }


    // ////////////////////////////////////////////////////////////////////////////////////

}