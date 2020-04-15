using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using System.IO;

using WDConnect;
using WDConnect.Common;

namespace ARB_Host
{
    // /////////////////////////////////////////////////////////////////////

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

        [XmlElement("HGA")]
        private HGACollection _hgas = new HGACollection();
        public HGACollection HGA
        {
            get { return _hgas; }
            set { _hgas = value; }
        }


        #region ctor
        public RequestPalletInfoAckObj()
        {
        }


        #endregion


        public SCITransaction ToSCITrans()
        {
            SCIMessage scndMsgRequestPalletInfo = new SCIMessage();
            scndMsgRequestPalletInfo.CommandID = "RequestPalletInfoAck";
            scndMsgRequestPalletInfo.Item = new SCIItem();
            scndMsgRequestPalletInfo.Item.Format = SCIFormat.List;
            scndMsgRequestPalletInfo.Item.Items = new SCIItemCollection();

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "COMMACK", Value = 0 });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "ALMID", Value = 0 });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.String, Name = "PalletID", Value = "" /*reqPallet.PalletID*/ });


            string strLot = "";
            Random rnd = new Random(Environment.TickCount);
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

    }

    #endregion


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

        private string _strDefect = string.Empty;
        public string Defect
        {
            get { return _strDefect; }
            set { _strDefect = value; }
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



    // /////////////////////////////////////////////////////////////////////

    [Serializable]
    public class RequestProcessRecipeAckObj
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

        [XmlElement("LotNumber")]
        private string _strLotNumber = string.Empty;
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        [XmlElement("RecipeID")]
        private string _strRecipeID = string.Empty;
        public string RecipeID
        {
            get { return _strRecipeID; }
            set { _strRecipeID = value; }
        }

        [XmlElement("Line")]
        private string _strLine = string.Empty;
        public string Line
        {
            get { return _strLine; }
            set { _strLine = value; }
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

        [XmlElement("Suspension")]
        private string _strSuspension = string.Empty;
        public string Suspension
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        [XmlElement("HGAType")]
        private string _strHGAType = string.Empty;
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }




        #region ctor
        public RequestProcessRecipeAckObj()
        {
        }
        #endregion



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


        public static RequestProcessRecipeAckObj ReadFromFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new RequestProcessRecipeAckObj();
            }


            string strXML = System.IO.File.ReadAllText(strFilePath);

            return RequestProcessRecipeAckObj.ToObj(strXML);
        }


        public static RequestProcessRecipeAckObj ToObj(string strXML)
        {
            RequestProcessRecipeAckObj obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(RequestProcessRecipeAckObj));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (RequestProcessRecipeAckObj)x.Deserialize(reader);
            }

            return obj;
        }

    }

    // /////////////////////////////////////////////////////////////////////
}
