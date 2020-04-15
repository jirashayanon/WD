using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AQCruncher
{
    public partial class PalletLogFileForm : Form
    {
        public PalletLogFileForm()
        {
            InitializeComponent();
            _init();
        }

        public PalletLogFileForm(string filename)
        {
            InitializeComponent();

            _strPalletLogFileName = filename;

            PalletObj aPallet = new PalletObj();
            aPallet.ReadFile(_strPalletLogFileName);

            _palletObj = PalletObj.ToPalletObj(aPallet.ToXML());
            _init();

            //_simulatePallet1();
            //_simulatePallet2();
        }


        /*1*/        private const string CONST_PALLET_ID = "PALLETID=";
        /*2*/        private const string CONST_TESTER_NUMBER = "TESTER NUMBER=";
        /*3*/        private const string CONST_CUSTOMER = "CUSTOMER=";
        /*4*/        private const string CONST_PRODUCT = "PRODUCT=";
        /*5*/        private const string CONST_USER = "USER=";
        /*6*/        private const string CONST_LOTNUMBER = "LOTNUMBER=";
        /*7*/        private const string CONST_DOCCONTROL1 = "DOCCONTROL1=PN#";
        /*8*/        private const string CONST_DOCCONTROL2 = "DOCCONTROL2=STR#";
        /*9*/        private const string CONST_SUS = "SUS=";
        /*10*/       private const string CONST_ASSYLINE = "ASSYLINE=";


        private string[] arrInformation = new string[10];
        private string[] arrSerial = new string[10];
        private string[] arrDefect = new string[10];

        private string _strPalletLogFileName = string.Empty;
        public string PalletLogFileName
        {
            get { return _strPalletLogFileName; }
            set { _strPalletLogFileName = value; }
        }

        private PalletObj _palletObj = new PalletObj();
        private void _init()
        {
            arrInformation[0] = CONST_PALLET_ID;        //PALLETID=SF001
            arrInformation[1] = CONST_TESTER_NUMBER;    //TESTER NUMBER=AVI001
            arrInformation[2] = CONST_CUSTOMER;         //CUSTOMER=WD
            arrInformation[3] = CONST_PRODUCT;          //PRODUCT=TRAILS_C8_SD
            arrInformation[4] = CONST_USER;             //USER=
            arrInformation[5] = CONST_LOTNUMBER;        //LOTNUMBER=Y3DRG_CB2
            arrInformation[6] = CONST_DOCCONTROL1;      //DOCCONTROL1=PN#70632-15-SBB
            arrInformation[7] = CONST_DOCCONTROL2;      //DOCCONTROL2=STR#G24960SN
            arrInformation[8] = CONST_SUS;              //SUS=MPT-4G
            arrInformation[9] = CONST_ASSYLINE;         //ASSYLINE=B4A101

            for (int i = 0; i < 10; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = arrInformation[i];

                //item.SubItems.Add("xxx"+ i.ToString());
                switch (i)
                {
                    case 0:     //PALLETID=SF001
                        item.SubItems.Add(_palletObj.PalletID);
                        break;

                    case 1:     //TESTER NUMBER=AVI001
                        item.SubItems.Add(_palletObj.TesterNumber);
                        break;

                    case 2:     //CUSTOMER=WD
                        item.SubItems.Add(_palletObj.Customer);
                        break;

                    case 3:     //PRODUCT=TRAILS_C8_SD
                        item.SubItems.Add(_palletObj.Product);
                        break;

                    case 4:     //USER=
                        item.SubItems.Add(_palletObj.User);
                        break;

                    case 5:     //LOTNUMBER=Y3DRG_CB2
                        item.SubItems.Add(_palletObj.LotNumber);
                        break;

                    case 6:     //DOCCONTROL1=PN#70632-15-SBB
                        item.SubItems.Add(_palletObj.DocControl1);
                        break;

                    case 7:     //DOCCONTROL2=STR#G24960SN
                        item.SubItems.Add(_palletObj.DocControl2);
                        break;

                    case 8:     //SUS=MPT-4G
                        item.SubItems.Add(_palletObj.Sus);
                        break;

                    case 9:     //ASSYLINE=B4A101
                        item.SubItems.Add(_palletObj.AssyLine);
                        break;

                    default:
                        break;
                }

                listViewInformation.Items.Add(item);
            }


            arrSerial[0] = _palletObj.HGAN1;
            arrSerial[1] = _palletObj.HGAN2;
            arrSerial[2] = _palletObj.HGAN3;
            arrSerial[3] = _palletObj.HGAN4;
            arrSerial[4] = _palletObj.HGAN5;

            arrSerial[5] = _palletObj.HGAN6;
            arrSerial[6] = _palletObj.HGAN7;
            arrSerial[7] = _palletObj.HGAN8;
            arrSerial[8] = _palletObj.HGAN9;
            arrSerial[9] = _palletObj.HGAN10;


            arrDefect[0] = _palletObj.DefectN1;
            arrDefect[1] = _palletObj.DefectN2;
            arrDefect[2] = _palletObj.DefectN3;
            arrDefect[3] = _palletObj.DefectN4;
            arrDefect[4] = _palletObj.DefectN5;

            arrDefect[5] = _palletObj.DefectN6;
            arrDefect[6] = _palletObj.DefectN7;
            arrDefect[7] = _palletObj.DefectN8;
            arrDefect[8] = _palletObj.DefectN9;
            arrDefect[9] = _palletObj.DefectN10;


            for (int i = 0; i < 10; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "HGAN" + (i + 1).ToString() + "=";
                item.SubItems.Add(arrSerial[i]);
                item.SubItems.Add(arrDefect[i]);

                listViewSerial.Items.Add(item);
            }
        }

        private void _simulatePallet1()
        {
            PalletObj pallet = new PalletObj();

            pallet.PalletID = "SF001";
            
            pallet.TesterNumber = "AVI001";
            pallet.Customer = "WD";
            pallet.Product = "TRAILS_C8_SD";
            pallet.User = "";

            pallet.LotNumber = "Y3DRG_CB2";
            pallet.DocControl1 = "PN#70632-15-SBB";
            pallet.DocControl2 = "STR#G24960SN";
            pallet.Sus = "MPT-4G";
            pallet.AssyLine = "B4A101";

            pallet.HGAN1 = "Y3DRGH2S";
            pallet.HGAN2 = "Y3DRHH33";
            pallet.HGAN3 = "Y3DRHH32";
            pallet.HGAN4 = "Y3DRHH3Z";
            pallet.HGAN5 = "Y3DRHH3Y";
            pallet.HGAN6 = "Y3DRHH3W";
            pallet.HGAN7 = "Y3DRHH3V";
            pallet.HGAN8 = "Y3DRHH3N";
            pallet.HGAN9 = "Y3DRHH3L";
            pallet.HGAN10 = "Y3DRHH3K";

            pallet.DefectN1 = "A1,J4B,T6,68";

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.File.WriteAllText(exePath + @"\" + pallet.PalletID + ".IAVI", pallet.ToXML());
        }


        private void _simulatePallet2()
        {
            PalletObj pallet = new PalletObj();

            pallet.PalletID = "SF002";

            pallet.TesterNumber = "AVI001";
            pallet.Customer = "WD";
            pallet.Product = "TRAILS_C8_SD";
            pallet.User = "";

            pallet.LotNumber = "Y3DRG_CB2";
            pallet.DocControl1 = "PN#70632-15-SBB";
            pallet.DocControl2 = "STR#G24960SN";
            pallet.Sus = "MPT-4G";
            pallet.AssyLine = "B4A101";

            pallet.HGAN1 = "Y3DRHH24";
            pallet.HGAN2 = "Y3DRHH23";
            pallet.HGAN3 = "Y3DRHH22";
            pallet.HGAN4 = "Y3DRHH21";
            pallet.HGAN5 = "Y3DRHH3H";
            pallet.HGAN6 = "Y3DRHH3F";
            pallet.HGAN7 = "Y3DRGH2Z";
            pallet.HGAN8 = "Y3DRGH2X";
            pallet.HGAN9 = "Y3DRGH2V";
            pallet.HGAN10 = "Y3DRGH2T";

            pallet.DefectN2 = "J4A";

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.File.WriteAllText(exePath + @"\" + pallet.PalletID + ".IAVI", pallet.ToXML());
        }
    }
}