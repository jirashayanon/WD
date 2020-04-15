using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WDConnect.Common;
using WDConnect.ComponentModel;


namespace ARB_Host
{
    public static class ToolModel
    {
       
        public static List<hostController> GetAllHostController()
        {
            string ToolModelPath = ConfigurationManager.AppSettings["ToolModelPath"].ToString();
            DirectoryInfo d = new DirectoryInfo(ToolModelPath);
            FileInfo[] Files = d.GetFiles("*.xml");

            List<hostController> ListOfHost = new List<hostController>();

            foreach (FileInfo _file in Files)
            {
                hostController host = new hostController();
                host.Initialize(_file.FullName);

                ListOfHost.Add(host);
            }


            return ListOfHost;
        }


        public static SCIMessage GetMessage(string messageName)
        {
            string MessageTemplatePath = ConfigurationManager.AppSettings["MessageTemplate"].ToString();
            string filePath = string.Format("{0}\\{1}.xml", MessageTemplatePath, messageName);
            SCIMessage scimessage = null;
            if (File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SCIMessage));

                StreamReader reader = new StreamReader(filePath);
                scimessage = (SCIMessage)serializer.Deserialize(reader);
                reader.Close();
            }
            return scimessage;
        }

     

    }

    public class hostController : WDConnect.Application.WDConnectBase
    {
        public override void Initialize(string equipmentModelPath)
        {
            base.Initialize(equipmentModelPath);
            this.ConnectionStatus = ConnectionStatus.NotConnected;
        }

        public void SendMessage(SCITransaction transaction)
        {
            base.ProcessOutStream(transaction);
        }

        public void ReplyOutStream(SCITransaction transaction)
        {
            base.ReplyOutSteam(transaction);
        }

        public string ToolId
        {
            get
            {
                return EquipmentModel.Nameable.id;
            }
        }

        public string connectionMode
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.connectionMode.ToString();
            }
        }

        public string localIPAddress
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.localIPAddress;
            }
        }

        public int localPortNumber
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.localPortNumber;
            }
        }

        public int T3Timeout
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.T3Timeout;
            }
        }

        public int T5Timeout
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.T5Timeout;
            }
        }

        public string[] hostConfiguration
        {
            get
            {
                string[] config = {  EquipmentModel.Nameable.id,
                                      ConnectionStatus.NotConnected.ToString(),
                                      EquipmentModel.GemConnection.HSMS.connectionMode.ToString(),
                                      EquipmentModel.GemConnection.HSMS.localIPAddress,
                                      EquipmentModel.GemConnection.HSMS.localPortNumber.ToString(),
                                      string.Empty ,
                                      string.Empty,
                                      string.Empty ,
                                      EquipmentModel.GemConnection.HSMS.T3Timeout.ToString(),
                                      EquipmentModel.GemConnection.HSMS.T5Timeout.ToString()
                                   };
                return config;
            }
        }
        private ConnectionStatus _connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
            }
        }
    }

    public enum ConnectionStatus
    {
        Connected,
        NotConnected
    }


    public class HGAData
    {
        private string _sn = "";
        private bool _bMixedWafer = false;
        private HSC _nHSC = HSC.NoPart_NotRecognized_NotManualSN;

        public HGAData()
        {
        }

        public string SerialNum
        {
            get { return _sn; }
            set { _sn = value; }
        }

        public bool bMixedWafer
        {
            get { return _bMixedWafer; }
            set { _bMixedWafer = value; }
        }

        public HSC HSCCode
        {
            get { return _nHSC; }
            set { _nHSC = value; }
        }
    }

    public enum HSC
    {
        HasPart_Recogonized_NotManualSN = 0,
        HasPart_NotRecognized_ManualSN = 1,
        NoPart_NotRecognized_NotManualSN = 2,
        HasPart_NotRecognized_NotManualSN = 3
    }

    public class PalletToTrayData
    {
        public PalletToTrayData()
        {
            _hga = new HGAData[10];
            for (int i = 0; i < 10; i++)
            {
                _hga[i] = new HGAData();
            }
        }

        private string _strPalletID = "";
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; }
        }

        private int _nRowID = 0;
        public int RowID
        {
            get { return _nRowID; }
            set { _nRowID = value; }
        }

        private string _strTrayID = "";
        public string TrayID
        {
            get { return _strTrayID; }
            set { _strTrayID = value; }
        }

        private bool _bMixedBin = false;
        public bool bMixedBin
        {
            get { return _bMixedBin; }
            set { _bMixedBin = value; }
        }

        public HGAData[] _hga;

        private int _nPalletLoaderID = 0;
        public int PalletLoaderID
        {
            get { return _nPalletLoaderID; }
            set { _nPalletLoaderID = value; }
        }

        private int _nTrayFixtureID = 0;
        public int TrayFixtureID
        {
            get { return _nTrayFixtureID; }
            set { _nTrayFixtureID = value; }
        }

        public bool bMixedWafer
        {
            get
            {
                for (int i = 0; i < 10; i++)
                {
                    if (_hga[i].bMixedWafer)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // /////////////////////////////////////////////////////////////////////
        //for SendPalletInfo
        private string _strPartNumber = "";
        public string PartNumber
        {
            get { return _strPartNumber; }
            set { _strPartNumber = value; }
        }

        private string _strHGALotNumber = "";
        public string HGALotNumber
        {
            get { return _strHGALotNumber; }
            set { _strHGALotNumber = value; }
        }

        private string _strSTRNumber = "";
        public string STRNumber
        {
            get { return _strSTRNumber; }
            set { _strSTRNumber = value; }
        }

        private string _strLine = "";
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
        }

        private string _strSliderAttachMachine = "";
        public string SliderAttachMachine
        {
            get { return _strSliderAttachMachine; }
            set { _strSliderAttachMachine = value; }
        }

        private string _strSuspension = "";
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        private string _strHGAType = "";
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }

        private bool _bAllowMixed = false;
        public bool AllowMixed
        {
            get { return _bAllowMixed; }
            set { _bAllowMixed = value; }
        }

        private bool _bPalletEnabled = true;
        public bool PalletEnabled
        {
            get { return _bPalletEnabled; }
            set { _bPalletEnabled = value; }
        }

        public void Clear()
        {
            _strPartNumber = "";
            _strHGALotNumber = "";
            _strSTRNumber = "";
            _strLine = "";
            _strSliderAttachMachine = "";
            _strSuspension = "";
            _strHGAType = "";
            _bAllowMixed = false;
            _bPalletEnabled = true;
        }
    }

    //equipment status
    //0 = init
    //1 = idle
    //2 = home
    //3 = emergency - fault state
    //4 = ready
    //5 = loop - production processing state
    //6 = operator - processing state by operator
    //7 = step - step processing state
    //8 = setup - setup/repair
    //9 = abort - stop
    public enum EQUIPMENT_STATUS
    {
        Init = 0,
        Idle = 1,
        Home = 2,
        Emergency = 3,
        Ready = 4,
        Loop = 5,
        Operator = 6,
        Step = 7,
        Setup = 8,
        Abort = 9
    }



    public class UpdateTrayProcessObj
    {
        public UpdateTrayProcessObj()
        {
            _trayBarcode = "";
            _lotnumber = "";
        }

        public UpdateTrayProcessObj(string strXML)
        {
            InitUpdateTrayProcessFromXML(strXML);
        }

        private string _trayBarcode = "";
        public string TrayBarcode
        {
            get { return _trayBarcode; }
            set { _trayBarcode = value; }
        }

        private string _lotnumber = "";
        public string LotNumber
        {
            get { return _lotnumber; }
            set { _lotnumber = value; }
        }




        private void InitUpdateTrayProcessFromXML(string xmlPallet)
        {
            XmlDocument updatetrayProcXMLDoc = new XmlDocument();
            updatetrayProcXMLDoc.LoadXml(xmlPallet);

            XmlElement root_updatetrayProcXMLDoc = updatetrayProcXMLDoc.DocumentElement;
            XmlNodeList updatetrayProcIDNodeList = root_updatetrayProcXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (updatetrayProcIDNodeList.Count > 0)
            {
                foreach (XmlNode node in updatetrayProcIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==TrayBarcode
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==EA0001234Z
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "TrayBarcode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.TrayBarcode = valueChildNode.InnerText;
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

                }
            }

            return;
        }


    }

    public class GetTrayInfoObj
    {
        public GetTrayInfoObj()
        {
        }

        public GetTrayInfoObj(string strGetTrayInfoXML)
        {
            this.InitGetTrayInfoFromXML(strGetTrayInfoXML);
        }

        private string _traybarcode = "";
        public string TrayBarcode
        {
            get { return _traybarcode; }
            set { _traybarcode = value; }
        }

        private string _lotnumber = "";
        public string LotNumber
        {
            get { return _lotnumber; }
            set { _lotnumber = value; }
        }


        private void InitGetTrayInfoFromXML(string xmlPallet)
        {
            XmlDocument updatetrayProcXMLDoc = new XmlDocument();
            updatetrayProcXMLDoc.LoadXml(xmlPallet);

            XmlElement root_updatetrayProcXMLDoc = updatetrayProcXMLDoc.DocumentElement;
            XmlNodeList updatetrayProcIDNodeList = root_updatetrayProcXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            if (updatetrayProcIDNodeList.Count > 0)
            {
                foreach (XmlNode node in updatetrayProcIDNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==TrayBarcode
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==EA0001234Z
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "TrayBarcode")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.TrayBarcode = valueChildNode.InnerText;
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

                }
            }

            return;
        }


    }

}
