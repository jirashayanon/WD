using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using System.IO;

using WDConnect;
using WDConnect.Common;

namespace ARB_Host
{
    // /////////////////////////////////////////////////////////////////////
    //[Serializable]
    //public class SCITransaction
    //{
    //    [XmlElement("MessageType")]
    //    public string MessageType
    //    {
    //        get;
    //        set;
    //    }

    //    public string ToXML()
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        XmlWriterSettings settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

    //        using (var writer = XmlWriter.Create(sb, settings))
    //        {
    //            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

    //            writer.WriteStartDocument();
    //            x.Serialize(writer, this);
    //        }

    //        return sb.ToString();
    //    }

    //    public static ARB_Host.SCITransaction ReadFromFile(string strFilePath)
    //    {
    //        if (!System.IO.File.Exists(strFilePath))
    //        {
    //            return new ARB_Host.SCITransaction();
    //        }


    //        string strXML = System.IO.File.ReadAllText(strFilePath);

    //        return ARB_Host.SCITransaction.ToObj(strXML);
    //    }

    //    public static ARB_Host.SCITransaction ToObj(string strXML)
    //    {
    //        ARB_Host.SCITransaction obj;

    //        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ARB_Host.SCITransaction));
    //        using (StringReader reader = new StringReader(strXML))
    //        {
    //            obj = (ARB_Host.SCITransaction)x.Deserialize(reader);
    //        }

    //        return obj;
    //    }

    //}

    // /////////////////////////////////////////////////////////////////////
    [Serializable]
    public class SendPalletInfoObj
    {
        [XmlElement("EquipmentID")]
        private string _strEquipmentID = string.Empty;
        public string EquipmentID
        {
            get { return _strEquipmentID; }
            set { _strEquipmentID = value; }
        }

        [XmlElement("EquipmentType")]
        private string _strEquipmentType = string.Empty;
        public string EquipmentType
        {
            get { return _strEquipmentType; }
            set { _strEquipmentType = value; }
        }

        [XmlElement("PalletID")]
        private string _strPalletID = string.Empty;
        public string PalletID 
        {
            get { return _strPalletID; }
            set { _strPalletID = value; }
        }

        [XmlElement("Suspension")]
        private string _strSuspension = string.Empty;
        public string Suspension 
        {
            get { return _strSuspension; }
            set { _strSuspension = value; }
        }

        [XmlElement("SuspTrayID")]
        private string _strSuspTrayID = string.Empty;
        public string SuspTrayID 
        {
            get { return _strSuspTrayID; }
            set { _strSuspTrayID = value; }
        }

        [XmlElement("SliderTrayID")]
        private string _strSliderTrayID = string.Empty;
        public string SliderTrayID 
        {
            get { return _strSliderTrayID; }
            set { _strSliderTrayID = value; }
        }

        [XmlElement("LotNumber")]
        private string _strLotNumber = string.Empty;
        public string LotNumber 
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        [XmlElement("UVPower")]
        private int _nUVPower = 0;
        public int UVPower
        {
            get { return _nUVPower; }
            set { _nUVPower = value; }
        }

        [XmlElement("CureTime")]
        private double _dblCureTime = 0.0;
        public double CureTime
        {
            get { return _dblCureTime; }
            set { _dblCureTime = value; }
        }

        [XmlElement("CureZone")]
        private int _nCureZone = 0;
        public int CureZone
        {
            get { return _nCureZone; }
            set { _nCureZone = value; }
        }

        [XmlElement("SJBLane")]
        private int _nSJBLane = 0;
        public int SJBLane
        {
            get { return _nSJBLane; }
            set { _nSJBLane = value; }
        }

        [XmlElement("SJBFixture")]
        private int _nSJBFixture = 0;
        public int SJBFixture
        {
            get { return _nSJBFixture; }
            set { _nSJBFixture = value; }
        }

        [XmlElement("HGA")]
        private HGACollection _hgas = new HGACollection();
        public HGACollection HGA
        {
            get { return _hgas; }
            set { _hgas = value; }
        }

        [XmlElement("RequestSuspension")]
        private RequestSuspensionClass _reqSuspObj = new RequestSuspensionClass();
        public RequestSuspensionClass ReqSusp
        {
            get { return _reqSuspObj; }
            set { _reqSuspObj = value; }
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

        public static SendPalletInfoObj ReadFromFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new SendPalletInfoObj();
            }


            string strXML = System.IO.File.ReadAllText(strFilePath);

            return SendPalletInfoObj.ToObj(strXML);
        }

        public static SendPalletInfoObj ToObj(string strXML)
        {
            SendPalletInfoObj obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SendPalletInfoObj));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (SendPalletInfoObj)x.Deserialize(reader);
            }

            return obj;
        }

        public static SendPalletInfoObj ToObj(WDConnect.Common.SCITransaction trans)
        {
            SendPalletInfoObj obj = new SendPalletInfoObj();
            foreach (SCIItem item in trans.Primary.Item.Items)
            {
                Console.WriteLine("{0},{1}", item.Name, item.Value);

                if(Helpers.PropertyName(() => obj.EquipmentID) == item.Name)
                {
                    obj.EquipmentID = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.EquipmentType) == item.Name)
                {
                    obj.EquipmentType = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.PalletID) == item.Name)
                {
                    obj.PalletID = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.Suspension) == item.Name)
                {
                    obj.Suspension = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.SuspTrayID) == item.Name)
                {
                    obj.SuspTrayID = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.SliderTrayID) == item.Name)
                {
                    obj.SliderTrayID = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.LotNumber) == item.Name)
                {
                    obj.LotNumber = item.Value.ToString();
                }

                if (Helpers.PropertyName(() => obj.UVPower) == item.Name)
                {
                    obj.UVPower = Int32.Parse(item.Value.ToString());
                }

                if (Helpers.PropertyName(() => obj.CureTime) == item.Name)
                {
                    obj.CureTime = Double.Parse(item.Value.ToString());
                }

                if (Helpers.PropertyName(() => obj.CureZone) == item.Name)
                {
                    obj.CureZone = Int32.Parse(item.Value.ToString());
                }

                if (Helpers.PropertyName(() => obj.SJBLane) == item.Name)
                {
                    obj.SJBLane = Int32.Parse(item.Value.ToString());
                }

                if (Helpers.PropertyName(() => obj.SJBFixture) == item.Name)
                {
                    obj.SJBFixture = Int32.Parse(item.Value.ToString());
                }

                if (Helpers.PropertyName(() => obj.HGA) == item.Name)
                {
                    obj.HGA = new HGACollection();
                    
                    obj.HGA.HGA1 = new HGA();
                    obj.HGA.HGA1.SN = item.Items[0].Items[0].Value.ToString();
                    obj.HGA.HGA1.Defect = item.Items[0].Items[1].Value.ToString();

                    obj.HGA.HGA2 = new HGA();
                    obj.HGA.HGA2.SN = item.Items[1].Items[0].Value.ToString();
                    obj.HGA.HGA2.Defect = item.Items[1].Items[1].Value.ToString();

                    obj.HGA.HGA3 = new HGA();
                    obj.HGA.HGA3.SN = item.Items[2].Items[0].Value.ToString();
                    obj.HGA.HGA3.Defect = item.Items[2].Items[1].Value.ToString();

                    obj.HGA.HGA4 = new HGA();
                    obj.HGA.HGA4.SN = item.Items[3].Items[0].Value.ToString();
                    obj.HGA.HGA4.Defect = item.Items[3].Items[1].Value.ToString();

                    obj.HGA.HGA5 = new HGA();
                    obj.HGA.HGA5.SN = item.Items[4].Items[0].Value.ToString();
                    obj.HGA.HGA5.Defect = item.Items[4].Items[1].Value.ToString();


                    obj.HGA.HGA6 = new HGA();
                    obj.HGA.HGA6.SN = item.Items[5].Items[0].Value.ToString();
                    obj.HGA.HGA6.Defect = item.Items[5].Items[1].Value.ToString();

                    obj.HGA.HGA7 = new HGA();
                    obj.HGA.HGA7.SN = item.Items[6].Items[0].Value.ToString();
                    obj.HGA.HGA7.Defect = item.Items[6].Items[1].Value.ToString();

                    obj.HGA.HGA8 = new HGA();
                    obj.HGA.HGA8.SN = item.Items[7].Items[0].Value.ToString();
                    obj.HGA.HGA8.Defect = item.Items[7].Items[1].Value.ToString();

                    obj.HGA.HGA9 = new HGA();
                    obj.HGA.HGA9.SN = item.Items[8].Items[0].Value.ToString();
                    obj.HGA.HGA9.Defect = item.Items[8].Items[1].Value.ToString();

                    obj.HGA.HGA10 = new HGA();
                    obj.HGA.HGA10.SN = item.Items[9].Items[0].Value.ToString();
                    obj.HGA.HGA10.Defect = item.Items[9].Items[1].Value.ToString();
                }


                if (item.Name == "RequestSuspension")
                {
                    obj.ReqSusp.IsRequesting = Boolean.Parse(item.Value.ToString());
                    Console.WriteLine(item.Value.ToString());
                    if (item.Items[0].Name == "ACAMID")
                    {
                        obj.ReqSusp.ACAMID = item.Items[0].Value.ToString();
                    }
                    if (item.Items[1].Name == "SuspAmt")
                    {
                        obj.ReqSusp.SuspAmt = Int32.Parse(item.Items[1].Value.ToString());
                    }
                }
            }

            return obj;
        }
    }

    // /////////////////////////////////////////////////////////////////////
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
            #region reqSuspSCIItemsNodeList
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
                                this.IsRequesting = this.SuspAmt > 0 ? true : false;
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


    // /////////////////////////////////////////////////////////////////////
    public class RequestSwapPalletClass
    {
        #region ctor
        public RequestSwapPalletClass()
        {
        }


        public RequestSwapPalletClass(string strRequestSwapPalletClassXML)
        {
            XmlDocument reqSwapPalletXMLDoc = new XmlDocument();
            reqSwapPalletXMLDoc.LoadXml(strRequestSwapPalletClassXML);

            XmlElement root_reqSwapPalletXMLDoc = reqSwapPalletXMLDoc.DocumentElement;

            XmlNodeList reqSwapPalletSCIItemsNodeList = root_reqSwapPalletXMLDoc.SelectNodes("/SCITransaction/Primary/Item/Items/SCIItem");
            #region reqSwapPalletSCIItemsNodeList
            if (reqSwapPalletSCIItemsNodeList.Count > 0)
            {
                foreach (XmlNode node in reqSwapPalletSCIItemsNodeList)
                {
                    XmlNodeList nameNodeList = node.SelectNodes("./Name");   //==PalletID
                    XmlNodeList valueNodeList = node.SelectNodes("./Value");  //==PT0001
                    foreach (XmlNode nameChildNode in nameNodeList)
                    {
                        if (nameChildNode.InnerText == "PalletID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.PalletID = valueChildNode.InnerText;
                            }
                        }

                        if (nameChildNode.InnerText == "ACAMID")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.ACAMID = valueChildNode.InnerText;
                            }
                        }

                        if (nameChildNode.InnerText == "EquipmentType")
                        {
                            foreach (XmlNode valueChildNode in valueNodeList)
                            {
                                this.EquipmentType = Int32.Parse(valueChildNode.InnerText);
                            }
                        }
                    }


                }
            }

            #endregion
        }

        #endregion 


        private string _strACAMID = string.Empty;
        public string ACAMID
        {
            get { return _strACAMID; }
            set { _strACAMID = value; }
        }


        private string _strPalletID = string.Empty;
        public string PalletID
        {
            get { return _strPalletID; }
            set { _strPalletID = value; }
        }


        private int _nEquipmentType = 0;
        public int EquipmentType
        {
            get { return _nEquipmentType; }
            set { _nEquipmentType = value; }
        }
    }

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


        [XmlElement("SJBFixture")]
        private int _nSJBFixture = 0;
        public int SJBFixture
        {
            get { return _nSJBFixture; }
            set { _nSJBFixture = value; }
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

            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "TransID", Value = this.TransID });
            scndMsgRequestPalletInfo.Item.Items.Add(new SCIItem { Format = SCIFormat.Integer, Name = "SJBFixture", Value = this.SJBFixture });


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
                hgaItem.Name = "HGA" + (j+1).ToString();
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

        public void Clear()
        {
            this.COMMACK = 1;
            this.ALMID = 0;
            
            this.PalletID = "";
            this.PartNumber = "";
            this.ProductName = "";
            this.LotNumber = "";
            this.STR = "";
            this.Line = "";
            this.ACAMID = "";
            this.Suspension = "";

            this.EnabledPallet = false;

            this.HGAType = "A";
            this.EquipmentType = 0;
            this.NextEquipmentType = 0;

            this.EndLot = false;
            this.AllowedMix = false;

            this.UVPower = 0;
            this.CureTime = 0;
            this.CureZone = 0;

            this.SJBStage = "";


            this.HGA.HGA1.SN = "";
            this.HGA.HGA1.Defect = "";
            this.HGA.HGA2.SN = "";
            this.HGA.HGA2.Defect = "";
            this.HGA.HGA3.SN = "";
            this.HGA.HGA3.Defect = "";
            this.HGA.HGA4.SN = "";
            this.HGA.HGA4.Defect = "";
            this.HGA.HGA5.SN = "";
            this.HGA.HGA5.Defect = "";

            this.HGA.HGA6.SN = "";
            this.HGA.HGA6.Defect = "";
            this.HGA.HGA7.SN = "";
            this.HGA.HGA7.Defect = "";
            this.HGA.HGA8.SN = "";
            this.HGA.HGA8.Defect = "";
            this.HGA.HGA9.SN = "";
            this.HGA.HGA9.Defect = "";
            this.HGA.HGA10.SN = "";
            this.HGA.HGA10.Defect = "";

            this.TransID = 0;
            this.SJBFixture = 0;
        }
    }

    #endregion


    // /////////////////////////////////////////////////////////////////////
    #region HGA
    [Serializable, XmlType(TypeName = "HGA")]
    public class HGA
    {
        [XmlElement("SN")]
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


    // /////////////////////////////////////////////////////////////////////
    #region RequestProcessRecipeAckObj
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

        [XmlElement("LotQty")]
        private int _nLotQty = 0;
        public int LotQty
        {
            get { return _nLotQty; }
            set { _nLotQty = value; }
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

        [XmlElement("SuspPartNumber")]
        private string _strSuspPartNumber = string.Empty;
        public string SuspPartNumber
        {
            get { return _strSuspPartNumber; }
            set { _strSuspPartNumber = value; }
        }


        [XmlElement("HGAType")]
        private string _strHGAType = string.Empty;
        public string HGAType
        {
            get { return _strHGAType; }
            set { _strHGAType = value; }
        }

        [XmlElement("STR")]
        private string _strSTR = string.Empty;
        public string STR
        {
            get { return _strSTR; }
            set { _strSTR = value; }
        }


        // ////////////////////////////////////////////////////////////////////////
        //recipe attributes for ILC
        #region UV1
        [XmlElement("Power_UV1")]
        private int _nPowerUV1 = 0;
        public int PowerUV1
        {
            get { return _nPowerUV1; }
            set { _nPowerUV1 = value; }
        }

        [XmlElement("CureTime_UV1")]
        private double _dblCureTimeUV1 = 0.0;
        public double CureTimeUV1
        {
            get { return _dblCureTimeUV1; }
            set { _dblCureTimeUV1 = value; }
        }

        [XmlElement("Enabled_UV1")]
        private int _nEnabledUV1 = 0;
        public int EnabledUV1
        {
            get { return _nEnabledUV1; }
            set { _nEnabledUV1 = value; }
        }

        #endregion 

        // ////////////////////////////////////////////
        #region Heater1

        [XmlElement("FlowRate_Heater1")]
        private double _dblFlowRateHeater1 = 0.0;
        public double FlowRateHeater1
        {
            get { return _dblFlowRateHeater1; }
            set { _dblFlowRateHeater1 = value; }
        }

        [XmlElement("Temp_Heater1")]
        private int _nTempHeater1 = 0;
        public int TempHeater1
        {
            get { return _nTempHeater1; }
            set { _nTempHeater1 = value; }
        }

        [XmlElement("Enabled_Heater1")]
        private int _nEnabledHeater1 = 0;
        public int EnabledHeater1
        {
            get { return _nEnabledHeater1; }
            set { _nEnabledHeater1 = value; }
        }

        [XmlElement("EnabledN2_Heater1")]
        private int _nEnabledN2Heater1 = 0;
        public int EnabledN2Heater1
        {
            get { return _nEnabledN2Heater1; }
            set { _nEnabledN2Heater1 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater2
        [XmlElement("FlowRate_Heater2")]
        private double _dblFlowRateHeater2 = 0.0;
        public double FlowRateHeater2
        {
            get { return _dblFlowRateHeater2; }
            set { _dblFlowRateHeater2 = value; }
        }

        [XmlElement("Temp_Heater2")]
        private int _nTempHeater2 = 0;
        public int TempHeater2
        {
            get { return _nTempHeater2; }
            set { _nTempHeater2 = value; }
        }

        [XmlElement("Enabled_Heater2")]
        private int _nEnabledHeater2 = 0;
        public int EnabledHeater2
        {
            get { return _nEnabledHeater2; }
            set { _nEnabledHeater2 = value; }
        }

        [XmlElement("EnabledN2_Heater2")]
        private int _nEnabledN2Heater2 = 0;
        public int EnabledN2Heater2
        {
            get { return _nEnabledN2Heater2; }
            set { _nEnabledN2Heater2 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater3
        [XmlElement("FlowRate_Heater3")]
        private double _dblFlowRateHeater3 = 0.0;
        public double FlowRateHeater3
        {
            get { return _dblFlowRateHeater3; }
            set { _dblFlowRateHeater3 = value; }
        }

        [XmlElement("Temp_Heater3")]
        private int _nTempHeater3 = 0;
        public int TempHeater3
        {
            get { return _nTempHeater3; }
            set { _nTempHeater3 = value; }
        }

        [XmlElement("Enabled_Heater3")]
        private int _nEnabledHeater3 = 0;
        public int EnabledHeater3
        {
            get { return _nEnabledHeater3; }
            set { _nEnabledHeater3 = value; }
        }

        [XmlElement("EnabledN2_Heater3")]
        private int _nEnabledN2Heater3 = 0;
        public int EnabledN2Heater3
        {
            get { return _nEnabledN2Heater3; }
            set { _nEnabledN2Heater3 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater4
        [XmlElement("FlowRate_Heater4")]
        private double _dblFlowRateHeater4 = 0.0;
        public double FlowRateHeater4
        {
            get { return _dblFlowRateHeater4; }
            set { _dblFlowRateHeater4 = value; }
        }

        [XmlElement("Temp_Heater4")]
        private int _nTempHeater4 = 0;
        public int TempHeater4
        {
            get { return _nTempHeater4; }
            set { _nTempHeater4 = value; }
        }

        [XmlElement("Enabled_Heater4")]
        private int _nEnabledHeater4 = 0;
        public int EnabledHeater4
        {
            get { return _nEnabledHeater4; }
            set { _nEnabledHeater4 = value; }
        }

        [XmlElement("EnabledN2_Heater4")]
        private int _nEnabledN2Heater4 = 0;
        public int EnabledN2Heater4
        {
            get { return _nEnabledN2Heater4; }
            set { _nEnabledN2Heater4 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater5
        [XmlElement("FlowRate_Heater5")]
        private double _dblFlowRateHeater5 = 0.0;
        public double FlowRateHeater5
        {
            get { return _dblFlowRateHeater5; }
            set { _dblFlowRateHeater5 = value; }
        }

        [XmlElement("Temp_Heater5")]
        private int _nTempHeater5 = 0;
        public int TempHeater5
        {
            get { return _nTempHeater5; }
            set { _nTempHeater5 = value; }
        }

        [XmlElement("Enabled_Heater5")]
        private int _nEnabledHeater5 = 0;
        public int EnabledHeater5
        {
            get { return _nEnabledHeater5; }
            set { _nEnabledHeater5 = value; }
        }

        [XmlElement("EnabledN2_Heater5")]
        private int _nEnabledN2Heater5 = 0;
        public int EnabledN2Heater5
        {
            get { return _nEnabledN2Heater5; }
            set { _nEnabledN2Heater5 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region UV2
        [XmlElement("Power_UV2")]
        private int _nPowerUV2 = 0;
        public int PowerUV2
        {
            get { return _nPowerUV2; }
            set { _nPowerUV2 = value; }
        }

        [XmlElement("CureTime_UV2")]
        private double _dblCureTimeUV2 = 0.0;
        public double CureTimeUV2
        {
            get { return _dblCureTimeUV2; }
            set { _dblCureTimeUV2 = value; }
        }

        [XmlElement("Enabled_UV2")]
        private int _nEnabledUV2 = 0;
        public int EnabledUV2
        {
            get { return _nEnabledUV2; }
            set { _nEnabledUV2 = value; }
        }

        #endregion


        // ////////////////////////////////////////////
        #region Heater6
        [XmlElement("FlowRate_Heater6")]
        private double _dblFlowRateHeater6 = 0.0;
        public double FlowRateHeater6
        {
            get { return _dblFlowRateHeater6; }
            set { _dblFlowRateHeater6 = value; }
        }

        [XmlElement("Temp_Heater6")]
        private int _nTempHeater6 = 0;
        public int TempHeater6
        {
            get { return _nTempHeater6; }
            set { _nTempHeater6 = value; }
        }

        [XmlElement("Enabled_Heater6")]
        private int _nEnabledHeater6 = 0;
        public int EnabledHeater6
        {
            get { return _nEnabledHeater6; }
            set { _nEnabledHeater6 = value; }
        }

        [XmlElement("EnabledN2_Heater6")]
        private int _nEnabledN2Heater6 = 0;
        public int EnabledN2Heater6
        {
            get { return _nEnabledN2Heater6; }
            set { _nEnabledN2Heater6 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater7
        [XmlElement("FlowRate_Heater7")]
        private double _dblFlowRateHeater7 = 0.0;
        public double FlowRateHeater7
        {
            get { return _dblFlowRateHeater7; }
            set { _dblFlowRateHeater7 = value; }
        }

        [XmlElement("Temp_Heater7")]
        private int _nTempHeater7 = 0;
        public int TempHeater7
        {
            get { return _nTempHeater7; }
            set { _nTempHeater7 = value; }
        }

        [XmlElement("Enabled_Heater7")]
        private int _nEnabledHeater7 = 0;
        public int EnabledHeater7
        {
            get { return _nEnabledHeater7; }
            set { _nEnabledHeater7 = value; }
        }

        [XmlElement("EnabledN2_Heater7")]
        private int _nEnabledN2Heater7 = 0;
        public int EnabledN2Heater7
        {
            get { return _nEnabledN2Heater7; }
            set { _nEnabledN2Heater7 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater8
        [XmlElement("FlowRate_Heater8")]
        private double _dblFlowRateHeater8 = 0.0;
        public double FlowRateHeater8
        {
            get { return _dblFlowRateHeater8; }
            set { _dblFlowRateHeater8 = value; }
        }

        [XmlElement("Temp_Heater8")]
        private int _nTempHeater8 = 0;
        public int TempHeater8
        {
            get { return _nTempHeater8; }
            set { _nTempHeater8 = value; }
        }

        [XmlElement("Enabled_Heater8")]
        private int _nEnabledHeater8 = 0;
        public int EnabledHeater8
        {
            get { return _nEnabledHeater8; }
            set { _nEnabledHeater8 = value; }
        }

        [XmlElement("EnabledN2_Heater8")]
        private int _nEnabledN2Heater8 = 0;
        public int EnabledN2Heater8
        {
            get { return _nEnabledN2Heater8; }
            set { _nEnabledN2Heater8 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater9
        [XmlElement("FlowRate_Heater9")]
        private double _dblFlowRateHeater9 = 0.0;
        public double FlowRateHeater9
        {
            get { return _dblFlowRateHeater9; }
            set { _dblFlowRateHeater9 = value; }
        }

        [XmlElement("Temp_Heater9")]
        private int _nTempHeater9 = 0;
        public int TempHeater9
        {
            get { return _nTempHeater9; }
            set { _nTempHeater9 = value; }
        }

        [XmlElement("Enabled_Heater9")]
        private int _nEnabledHeater9 = 0;
        public int EnabledHeater9
        {
            get { return _nEnabledHeater9; }
            set { _nEnabledHeater9 = value; }
        }

        [XmlElement("EnabledN2_Heater9")]
        private int _nEnabledN2Heater9 = 0;
        public int EnabledN2Heater9
        {
            get { return _nEnabledN2Heater9; }
            set { _nEnabledN2Heater9 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater10
        [XmlElement("FlowRate_Heater10")]
        private double _dblFlowRateHeater10 = 0.0;
        public double FlowRateHeater10
        {
            get { return _dblFlowRateHeater10; }
            set { _dblFlowRateHeater10 = value; }
        }

        [XmlElement("Temp_Heater10")]
        private int _nTempHeater10 = 0;
        public int TempHeater10
        {
            get { return _nTempHeater10; }
            set { _nTempHeater10 = value; }
        }

        [XmlElement("Enabled_Heater10")]
        private int _nEnabledHeater10 = 0;
        public int EnabledHeater10
        {
            get { return _nEnabledHeater10; }
            set { _nEnabledHeater10 = value; }
        }

        [XmlElement("EnabledN2_Heater10")]
        private int _nEnabledN2Heater10 = 0;
        public int EnabledN2Heater10
        {
            get { return _nEnabledN2Heater10; }
            set { _nEnabledN2Heater10 = value; }
        }

        #endregion

        [XmlElement("Mode")]
        private int _nMode = 0;
        public int Mode
        {
            get { return _nMode; }
            set { _nMode = value; }
        }

        [XmlElement("Bypass")]
        private int _nBypass = 0;
        public int Bypass
        {
            get { return _nBypass; }
            set { _nBypass = value; }
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


    #endregion



    // /////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ILCRecipeObj
    {
        #region UV1
        [XmlElement("Power_UV1")]
        private int _nPowerUV1 = 0;
        public int PowerUV1
        {
            get { return _nPowerUV1; }
            set { _nPowerUV1 = value; }
        }

        [XmlElement("CureTime_UV1")]
        private double _dblCureTimeUV1 = 0.0;
        public double CureTimeUV1
        {
            get { return _dblCureTimeUV1; }
            set { _dblCureTimeUV1 = value; }
        }

        [XmlElement("Enabled_UV1")]
        private int _nEnabledUV1 = 0;
        public int EnabledUV1
        {
            get { return _nEnabledUV1; }
            set { _nEnabledUV1 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater1

        [XmlElement("FlowRate_Heater1")]
        private double _dblFlowRateHeater1 = 0.0;
        public double FlowRateHeater1
        {
            get { return _dblFlowRateHeater1; }
            set { _dblFlowRateHeater1 = value; }
        }

        [XmlElement("Temp_Heater1")]
        private int _nTempHeater1 = 0;
        public int TempHeater1
        {
            get { return _nTempHeater1; }
            set { _nTempHeater1 = value; }
        }

        [XmlElement("Enabled_Heater1")]
        private int _nEnabledHeater1 = 0;
        public int EnabledHeater1
        {
            get { return _nEnabledHeater1; }
            set { _nEnabledHeater1 = value; }
        }

        [XmlElement("EnabledN2_Heater1")]
        private int _nEnabledN2Heater1 = 0;
        public int EnabledN2Heater1
        {
            get { return _nEnabledN2Heater1; }
            set { _nEnabledN2Heater1 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater2
        [XmlElement("FlowRate_Heater2")]
        private double _dblFlowRateHeater2 = 0.0;
        public double FlowRateHeater2
        {
            get { return _dblFlowRateHeater2; }
            set { _dblFlowRateHeater2 = value; }
        }

        [XmlElement("Temp_Heater2")]
        private int _nTempHeater2 = 0;
        public int TempHeater2
        {
            get { return _nTempHeater2; }
            set { _nTempHeater2 = value; }
        }

        [XmlElement("Enabled_Heater2")]
        private int _nEnabledHeater2 = 0;
        public int EnabledHeater2
        {
            get { return _nEnabledHeater2; }
            set { _nEnabledHeater2 = value; }
        }

        [XmlElement("EnabledN2_Heater2")]
        private int _nEnabledN2Heater2 = 0;
        public int EnabledN2Heater2
        {
            get { return _nEnabledN2Heater2; }
            set { _nEnabledN2Heater2 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater3
        [XmlElement("FlowRate_Heater3")]
        private double _dblFlowRateHeater3 = 0.0;
        public double FlowRateHeater3
        {
            get { return _dblFlowRateHeater3; }
            set { _dblFlowRateHeater3 = value; }
        }

        [XmlElement("Temp_Heater3")]
        private int _nTempHeater3 = 0;
        public int TempHeater3
        {
            get { return _nTempHeater3; }
            set { _nTempHeater3 = value; }
        }

        [XmlElement("Enabled_Heater3")]
        private int _nEnabledHeater3 = 0;
        public int EnabledHeater3
        {
            get { return _nEnabledHeater3; }
            set { _nEnabledHeater3 = value; }
        }

        [XmlElement("EnabledN2_Heater3")]
        private int _nEnabledN2Heater3 = 0;
        public int EnabledN2Heater3
        {
            get { return _nEnabledN2Heater3; }
            set { _nEnabledN2Heater3 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater4
        [XmlElement("FlowRate_Heater4")]
        private double _dblFlowRateHeater4 = 0.0;
        public double FlowRateHeater4
        {
            get { return _dblFlowRateHeater4; }
            set { _dblFlowRateHeater4 = value; }
        }

        [XmlElement("Temp_Heater4")]
        private int _nTempHeater4 = 0;
        public int TempHeater4
        {
            get { return _nTempHeater4; }
            set { _nTempHeater4 = value; }
        }

        [XmlElement("Enabled_Heater4")]
        private int _nEnabledHeater4 = 0;
        public int EnabledHeater4
        {
            get { return _nEnabledHeater4; }
            set { _nEnabledHeater4 = value; }
        }

        [XmlElement("EnabledN2_Heater4")]
        private int _nEnabledN2Heater4 = 0;
        public int EnabledN2Heater4
        {
            get { return _nEnabledN2Heater4; }
            set { _nEnabledN2Heater4 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater5
        [XmlElement("FlowRate_Heater5")]
        private double _dblFlowRateHeater5 = 0.0;
        public double FlowRateHeater5
        {
            get { return _dblFlowRateHeater5; }
            set { _dblFlowRateHeater5 = value; }
        }

        [XmlElement("Temp_Heater5")]
        private int _nTempHeater5 = 0;
        public int TempHeater5
        {
            get { return _nTempHeater5; }
            set { _nTempHeater5 = value; }
        }

        [XmlElement("Enabled_Heater5")]
        private int _nEnabledHeater5 = 0;
        public int EnabledHeater5
        {
            get { return _nEnabledHeater5; }
            set { _nEnabledHeater5 = value; }
        }

        [XmlElement("EnabledN2_Heater5")]
        private int _nEnabledN2Heater5 = 0;
        public int EnabledN2Heater5
        {
            get { return _nEnabledN2Heater5; }
            set { _nEnabledN2Heater5 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region UV2
        [XmlElement("Power_UV2")]
        private int _nPowerUV2 = 0;
        public int PowerUV2
        {
            get { return _nPowerUV2; }
            set { _nPowerUV2 = value; }
        }

        [XmlElement("CureTime_UV2")]
        private double _dblCureTimeUV2 = 0.0;
        public double CureTimeUV2
        {
            get { return _dblCureTimeUV2; }
            set { _dblCureTimeUV2 = value; }
        }

        [XmlElement("Enabled_UV2")]
        private int _nEnabledUV2 = 0;
        public int EnabledUV2
        {
            get { return _nEnabledUV2; }
            set { _nEnabledUV2 = value; }
        }

        #endregion


        // ////////////////////////////////////////////
        #region Heater6
        [XmlElement("FlowRate_Heater6")]
        private double _dblFlowRateHeater6 = 0.0;
        public double FlowRateHeater6
        {
            get { return _dblFlowRateHeater6; }
            set { _dblFlowRateHeater6 = value; }
        }

        [XmlElement("Temp_Heater6")]
        private int _nTempHeater6 = 0;
        public int TempHeater6
        {
            get { return _nTempHeater6; }
            set { _nTempHeater6 = value; }
        }

        [XmlElement("Enabled_Heater6")]
        private int _nEnabledHeater6 = 0;
        public int EnabledHeater6
        {
            get { return _nEnabledHeater6; }
            set { _nEnabledHeater6 = value; }
        }

        [XmlElement("EnabledN2_Heater6")]
        private int _nEnabledN2Heater6 = 0;
        public int EnabledN2Heater6
        {
            get { return _nEnabledN2Heater6; }
            set { _nEnabledN2Heater6 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater7
        [XmlElement("FlowRate_Heater7")]
        private double _dblFlowRateHeater7 = 0.0;
        public double FlowRateHeater7
        {
            get { return _dblFlowRateHeater7; }
            set { _dblFlowRateHeater7 = value; }
        }

        [XmlElement("Temp_Heater7")]
        private int _nTempHeater7 = 0;
        public int TempHeater7
        {
            get { return _nTempHeater7; }
            set { _nTempHeater7 = value; }
        }

        [XmlElement("Enabled_Heater7")]
        private int _nEnabledHeater7 = 0;
        public int EnabledHeater7
        {
            get { return _nEnabledHeater7; }
            set { _nEnabledHeater7 = value; }
        }

        [XmlElement("EnabledN2_Heater7")]
        private int _nEnabledN2Heater7 = 0;
        public int EnabledN2Heater7
        {
            get { return _nEnabledN2Heater7; }
            set { _nEnabledN2Heater7 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater8
        [XmlElement("FlowRate_Heater8")]
        private double _dblFlowRateHeater8 = 0.0;
        public double FlowRateHeater8
        {
            get { return _dblFlowRateHeater8; }
            set { _dblFlowRateHeater8 = value; }
        }

        [XmlElement("Temp_Heater8")]
        private int _nTempHeater8 = 0;
        public int TempHeater8
        {
            get { return _nTempHeater8; }
            set { _nTempHeater8 = value; }
        }

        [XmlElement("Enabled_Heater8")]
        private int _nEnabledHeater8 = 0;
        public int EnabledHeater8
        {
            get { return _nEnabledHeater8; }
            set { _nEnabledHeater8 = value; }
        }

        [XmlElement("EnabledN2_Heater8")]
        private int _nEnabledN2Heater8 = 0;
        public int EnabledN2Heater8
        {
            get { return _nEnabledN2Heater8; }
            set { _nEnabledN2Heater8 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater9
        [XmlElement("FlowRate_Heater9")]
        private double _dblFlowRateHeater9 = 0.0;
        public double FlowRateHeater9
        {
            get { return _dblFlowRateHeater9; }
            set { _dblFlowRateHeater9 = value; }
        }

        [XmlElement("Temp_Heater9")]
        private int _nTempHeater9 = 0;
        public int TempHeater9
        {
            get { return _nTempHeater9; }
            set { _nTempHeater9 = value; }
        }

        [XmlElement("Enabled_Heater9")]
        private int _nEnabledHeater9 = 0;
        public int EnabledHeater9
        {
            get { return _nEnabledHeater9; }
            set { _nEnabledHeater9 = value; }
        }

        [XmlElement("EnabledN2_Heater9")]
        private int _nEnabledN2Heater9 = 0;
        public int EnabledN2Heater9
        {
            get { return _nEnabledN2Heater9; }
            set { _nEnabledN2Heater9 = value; }
        }

        #endregion

        // ////////////////////////////////////////////
        #region Heater10
        [XmlElement("FlowRate_Heater10")]
        private double _dblFlowRateHeater10 = 0.0;
        public double FlowRateHeater10
        {
            get { return _dblFlowRateHeater10; }
            set { _dblFlowRateHeater10 = value; }
        }

        [XmlElement("Temp_Heater10")]
        private int _nTempHeater10 = 0;
        public int TempHeater10
        {
            get { return _nTempHeater10; }
            set { _nTempHeater10 = value; }
        }

        [XmlElement("Enabled_Heater10")]
        private int _nEnabledHeater10 = 0;
        public int EnabledHeater10
        {
            get { return _nEnabledHeater10; }
            set { _nEnabledHeater10 = value; }
        }

        [XmlElement("EnabledN2_Heater10")]
        private int _nEnabledN2Heater10 = 0;
        public int EnabledN2Heater10
        {
            get { return _nEnabledN2Heater10; }
            set { _nEnabledN2Heater10 = value; }
        }

        #endregion

        [XmlElement("Mode")]
        private int _nMode = 0;
        public int Mode
        {
            get { return _nMode; }
            set { _nMode = value; }
        }

        [XmlElement("Bypass")]
        private int _nBypass = 0;
        public int Bypass
        {
            get { return _nBypass; }
            set { _nBypass = value; }
        }


        public ILCRecipeObj()
        {
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

        public static ILCRecipeObj ReadFromFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new ILCRecipeObj();
            }


            string strXML = System.IO.File.ReadAllText(strFilePath);

            return ILCRecipeObj.ToObj(strXML);
        }

        public static ILCRecipeObj ToObj(string strXML)
        {
            ILCRecipeObj obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ILCRecipeObj));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (ILCRecipeObj)x.Deserialize(reader);
            }

            return obj;
        }
    }


    // /////////////////////////////////////////////////////////////////////
    public static class Helpers
    {
        public static string PropertyName<T>(Expression<Func<T>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null && member.Member is PropertyInfo)
            {
                return member.Member.Name;
            }

            throw new ArgumentException("Expression is not a property!");
        }
    }


    // /////////////////////////////////////////////////////////////////////
    #region HostConfigHelper
    [Serializable]
    public class HostConfigHelper
    {
        [XmlElement("HostDataSource")]
        private string _strHostDataSource = string.Empty;
        public string HostDataSource
        {
            get { return _strHostDataSource; }
            set { _strHostDataSource = value; }
        }

        [XmlElement("HostPort")]
        private int _strHostPort = 0;
        public int HostPort
        {
            get { return _strHostPort; }
            set { _strHostPort = value; }
        }

        [XmlElement("HostDatabase")]
        private string _strHostDatabase = string.Empty;
        public string HostDatabase
        {
            get { return _strHostDatabase; }
            set { _strHostDatabase = value; }
        }

        [XmlElement("RunWithNoDatabase")]
        private bool _bRunWithNoDatabase = false;
        public bool RunWithNoDatabase
        {
            get { return _bRunWithNoDatabase; }
            set { _bRunWithNoDatabase = value; }
        }

        [XmlElement("LineNo")]
        private string _strLineNo = string.Empty;
        public string LineNo
        {
            get { return _strLineNo; }
            set { _strLineNo = value; }
        }

        [XmlElement("ASLVHost")]
        private bool _bASLVHost = false;
        public bool ASLVHost
        {
            get { return _bASLVHost; }
            set { _bASLVHost = value; }
        }

        public HostConfigHelper()
        {
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


        public static HostConfigHelper ReadFromFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new HostConfigHelper();
            }


            string strXML = System.IO.File.ReadAllText(strFilePath);

            return HostConfigHelper.ToObj(strXML);
        }


        public static HostConfigHelper ToObj(string strXML)
        {
            HostConfigHelper obj;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(HostConfigHelper));
            using (StringReader reader = new StringReader(strXML))
            {
                obj = (HostConfigHelper)x.Deserialize(reader);
            }

            return obj;
        }

    }

    #endregion

}
