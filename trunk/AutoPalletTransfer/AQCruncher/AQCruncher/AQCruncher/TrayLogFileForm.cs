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
    public partial class TrayLogFileForm : Form
    {
        public TrayLogFileForm()
        {
            InitializeComponent();
            _init();
        }

        public TrayLogFileForm(string filename)
        {
            InitializeComponent();

            _strTrayLogFileName = filename;

            TrayObj aTray = new TrayObj();
            aTray.ReadFile(_strTrayLogFileName);

            _trayObj = TrayObj.ToTrayObj(aTray.ToXML());
            _init();

            //_simulateTray1();
        }


        /*1*/        private const string CONST_DATE = "DATE=";
        /*2*/        private const string CONST_TIME_START = "TIME START=";
        /*3*/        private const string CONST_TIME_END = "TIME END=";
        /*4*/        private const string CONST_USED_TIME = "USED TIME=";
        /*5*/        private const string CONST_TRAY_ID = "TRAYID=";

        /*6*/        private const string CONST_PALLET_ID1 = "PALLETID1=";
        /*7*/        private const string CONST_PALLET_ID2 = "PALLETID2=";
        /*8*/        private const string CONST_PALLET_ID3 = "PALLETID3=";
        /*9*/        private const string CONST_PALLET_ID4 = "PALLETID4=";
        /*10*/       private const string CONST_PALLET_ID5 = "PALLETID5=";
        /*11*/       private const string CONST_PALLET_ID6 = "PALLETID6=";


        private string[] arrInformation = new string[11];
        private string[] arrSerial = new string[60];
        private string[] arrDefect = new string[60];

        private string _strTrayLogFileName = string.Empty;
        public string TrayLogFileName
        {
            get { return _strTrayLogFileName; }
            set { _strTrayLogFileName = value; }
        }
        
        private TrayObj _trayObj = new TrayObj();
        private List<string> _lstPalletFilesToSearch = new List<string>();
        private void _init()
        {
            arrInformation[0] = CONST_DATE;           //DATE=
            arrInformation[1] = CONST_TIME_START;     //TIME START=
            arrInformation[2] = CONST_TIME_END;       //TIME END=
            arrInformation[3] = CONST_USED_TIME;      //USED TIME=
            arrInformation[4] = CONST_TRAY_ID;        //TRAYID=
            arrInformation[5] = CONST_PALLET_ID1;     //PALLETID1=
            arrInformation[6] = CONST_PALLET_ID2;     //PALLETID2=
            arrInformation[7] = CONST_PALLET_ID3;     //PALLETID3=
            arrInformation[8] = CONST_PALLET_ID4;     //PALLETID4=
            arrInformation[9] = CONST_PALLET_ID5;     //PALLETID5=
            arrInformation[10] = CONST_PALLET_ID6;    //PALLETID6=

            for (int i = 0; i < 11; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = arrInformation[i];


                //item.SubItems.Add("xxx"+ i.ToString());
                switch (i)
                {
                    case 0:     //DATE=
                        item.SubItems.Add(_trayObj.Date);
                        break;

                    case 1:     //TIME START=
                        item.SubItems.Add(_trayObj.TimeStart);
                        break;

                    case 2:     //TIME END=
                        item.SubItems.Add(_trayObj.TimeEnd);
                        break;

                    case 3:     //USED TIME=
                        item.SubItems.Add(_trayObj.UsedTime);
                        break;

                    case 4:     //TRAYID=
                        item.SubItems.Add(_trayObj.TrayID);
                        break;

                    case 5:     //PALLETID1=
                        item.SubItems.Add(_trayObj.Pallet1);
                        break;

                    case 6:     //PALLETID2=
                        item.SubItems.Add(_trayObj.Pallet2);
                        break;

                    case 7:     //PALLETID3=
                        item.SubItems.Add(_trayObj.Pallet3);
                        break;

                    case 8:     //PALLETID4=
                        item.SubItems.Add(_trayObj.Pallet4);
                        break;

                    case 9:     //PALLETID5=
                        item.SubItems.Add(_trayObj.Pallet5);
                        break;

                    case 10:     //PALLETID6=
                        item.SubItems.Add(_trayObj.Pallet6);
                        break;

                    default:
                        break;
                }


                listViewInformation.Items.Add(item);
            }

            #region arrSerial
            arrSerial[0] = _trayObj.HGAN1;
            arrSerial[1] = _trayObj.HGAN2;
            arrSerial[2] = _trayObj.HGAN3;
            arrSerial[3] = _trayObj.HGAN4;
            arrSerial[4] = _trayObj.HGAN5;

            arrSerial[5] = _trayObj.HGAN6;
            arrSerial[6] = _trayObj.HGAN7;
            arrSerial[7] = _trayObj.HGAN8;
            arrSerial[8] = _trayObj.HGAN9;
            arrSerial[9] = _trayObj.HGAN10;

            arrSerial[10] = _trayObj.HGAN11;
            arrSerial[11] = _trayObj.HGAN12;
            arrSerial[12] = _trayObj.HGAN13;
            arrSerial[13] = _trayObj.HGAN14;
            arrSerial[14] = _trayObj.HGAN15;

            arrSerial[15] = _trayObj.HGAN16;
            arrSerial[16] = _trayObj.HGAN17;
            arrSerial[17] = _trayObj.HGAN18;
            arrSerial[18] = _trayObj.HGAN19;
            arrSerial[19] = _trayObj.HGAN20;

            arrSerial[20] = _trayObj.HGAN21;
            arrSerial[21] = _trayObj.HGAN22;
            arrSerial[22] = _trayObj.HGAN23;
            arrSerial[23] = _trayObj.HGAN24;
            arrSerial[24] = _trayObj.HGAN25;

            arrSerial[25] = _trayObj.HGAN26;
            arrSerial[26] = _trayObj.HGAN27;
            arrSerial[27] = _trayObj.HGAN28;
            arrSerial[28] = _trayObj.HGAN29;
            arrSerial[29] = _trayObj.HGAN30;

            arrSerial[30] = _trayObj.HGAN31;
            arrSerial[31] = _trayObj.HGAN32;
            arrSerial[32] = _trayObj.HGAN33;
            arrSerial[33] = _trayObj.HGAN34;
            arrSerial[34] = _trayObj.HGAN35;

            arrSerial[35] = _trayObj.HGAN36;
            arrSerial[36] = _trayObj.HGAN37;
            arrSerial[37] = _trayObj.HGAN38;
            arrSerial[38] = _trayObj.HGAN39;
            arrSerial[39] = _trayObj.HGAN40;

            arrSerial[40] = _trayObj.HGAN41;
            arrSerial[41] = _trayObj.HGAN42;
            arrSerial[42] = _trayObj.HGAN43;
            arrSerial[43] = _trayObj.HGAN44;
            arrSerial[44] = _trayObj.HGAN45;

            arrSerial[45] = _trayObj.HGAN46;
            arrSerial[46] = _trayObj.HGAN47;
            arrSerial[47] = _trayObj.HGAN48;
            arrSerial[48] = _trayObj.HGAN49;
            arrSerial[49] = _trayObj.HGAN50;

            arrSerial[50] = _trayObj.HGAN51;
            arrSerial[51] = _trayObj.HGAN52;
            arrSerial[52] = _trayObj.HGAN53;
            arrSerial[53] = _trayObj.HGAN54;
            arrSerial[54] = _trayObj.HGAN55;

            arrSerial[55] = _trayObj.HGAN56;
            arrSerial[56] = _trayObj.HGAN57;
            arrSerial[57] = _trayObj.HGAN58;
            arrSerial[58] = _trayObj.HGAN59;
            arrSerial[59] = _trayObj.HGAN60;

            #endregion

            #region defect
            arrDefect[0] = _trayObj.DefectN1;
            arrDefect[1] = _trayObj.DefectN2;
            arrDefect[2] = _trayObj.DefectN3;
            arrDefect[3] = _trayObj.DefectN4;
            arrDefect[4] = _trayObj.DefectN5;

            arrDefect[5] = _trayObj.DefectN6;
            arrDefect[6] = _trayObj.DefectN7;
            arrDefect[7] = _trayObj.DefectN8;
            arrDefect[8] = _trayObj.DefectN9;
            arrDefect[9] = _trayObj.DefectN10;

            arrDefect[10] = _trayObj.DefectN11;
            arrDefect[11] = _trayObj.DefectN12;
            arrDefect[12] = _trayObj.DefectN13;
            arrDefect[13] = _trayObj.DefectN14;
            arrDefect[14] = _trayObj.DefectN15;

            arrDefect[15] = _trayObj.DefectN16;
            arrDefect[16] = _trayObj.DefectN17;
            arrDefect[17] = _trayObj.DefectN18;
            arrDefect[18] = _trayObj.DefectN19;
            arrDefect[19] = _trayObj.DefectN20;

            arrDefect[20] = _trayObj.DefectN21;
            arrDefect[21] = _trayObj.DefectN22;
            arrDefect[22] = _trayObj.DefectN23;
            arrDefect[23] = _trayObj.DefectN24;
            arrDefect[24] = _trayObj.DefectN25;

            arrDefect[25] = _trayObj.DefectN26;
            arrDefect[26] = _trayObj.DefectN27;
            arrDefect[27] = _trayObj.DefectN28;
            arrDefect[28] = _trayObj.DefectN29;
            arrDefect[29] = _trayObj.DefectN30;

            arrDefect[30] = _trayObj.DefectN31;
            arrDefect[31] = _trayObj.DefectN32;
            arrDefect[32] = _trayObj.DefectN33;
            arrDefect[33] = _trayObj.DefectN34;
            arrDefect[34] = _trayObj.DefectN35;

            arrDefect[35] = _trayObj.DefectN36;
            arrDefect[36] = _trayObj.DefectN37;
            arrDefect[37] = _trayObj.DefectN38;
            arrDefect[38] = _trayObj.DefectN39;
            arrDefect[39] = _trayObj.DefectN40;

            arrDefect[40] = _trayObj.DefectN41;
            arrDefect[41] = _trayObj.DefectN42;
            arrDefect[42] = _trayObj.DefectN43;
            arrDefect[43] = _trayObj.DefectN44;
            arrDefect[44] = _trayObj.DefectN45;

            arrDefect[45] = _trayObj.DefectN46;
            arrDefect[46] = _trayObj.DefectN47;
            arrDefect[47] = _trayObj.DefectN48;
            arrDefect[48] = _trayObj.DefectN49;
            arrDefect[49] = _trayObj.DefectN50;

            arrDefect[50] = _trayObj.DefectN51;
            arrDefect[51] = _trayObj.DefectN52;
            arrDefect[52] = _trayObj.DefectN53;
            arrDefect[53] = _trayObj.DefectN54;
            arrDefect[54] = _trayObj.DefectN55;

            arrDefect[55] = _trayObj.DefectN56;
            arrDefect[56] = _trayObj.DefectN57;
            arrDefect[57] = _trayObj.DefectN58;
            arrDefect[58] = _trayObj.DefectN59;
            arrDefect[59] = _trayObj.DefectN60;

            #endregion


            for (int i = 0; i < 60; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "HGAN" + (i + 1).ToString() + "=";
                item.SubItems.Add(arrSerial[i]);
                item.SubItems.Add(arrDefect[i]);

                listViewSerial.Items.Add(item);
            }


            _lstPalletFilesToSearch.Clear();
            if (_trayObj.Pallet1.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet1);
            }

            if (_trayObj.Pallet2.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet2);
            }

            if (_trayObj.Pallet3.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet3);
            }

            if (_trayObj.Pallet4.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet4);
            }

            if (_trayObj.Pallet5.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet5);
            }

            if (_trayObj.Pallet6.Length > 0)
            {
                _lstPalletFilesToSearch.Add(_trayObj.Pallet6);
            }

        }

        private void _simulateTray1()
        {
            TrayObj tray = new TrayObj();

            tray.Date = DateTime.Today.ToString("dd/mm/yyyy");
            tray.TimeStart = "3:55:29 AM";
            tray.TimeEnd = "3:56:29 AM";
            //tray.UsedTime

            tray.TrayID = "EZ1677269M";
            tray.Pallet1 = "SF001";
            tray.Pallet2 = "SF002";


            //tray.HGAN1 = "HAS";
            //tray.HGAN2 = "HAS";
            //tray.HGAN3 = "HAS";
            //tray.HGAN4 = "HAS";
            //tray.HGAN5 = "HAS";
            //tray.HGAN6 = "HAS";
            //tray.HGAN7 = "HAS";
            //tray.HGAN8 = "HAS";
            //tray.HGAN9 = "HAS";
            //tray.HGAN10 = "HAS";

            //tray.HGAN11 = "HAS";
            //tray.HGAN12 = "HAS";
            //tray.HGAN13 = "HAS";
            //tray.HGAN14 = "HAS";
            //tray.HGAN15 = "HAS";

            //tray.HGAN16 = "HAS";
            //tray.HGAN17 = "HAS";
            //tray.HGAN18 = "HAS";
            //tray.HGAN19 = "HAS";
            //tray.HGAN20 = "HAS";


            tray.HGAN1 = "Y3DRGH2S";
            tray.HGAN2 = "Y3DRHH33";
            tray.HGAN3 = "Y3DRHH32";
            tray.HGAN4 = "Y3DRHH3Z";
            tray.HGAN5 = "Y3DRHH3Y";
            tray.HGAN6 = "Y3DRHH3W";
            tray.HGAN7 = "Y3DRHH3V";
            tray.HGAN8 = "Y3DRHH3N";
            tray.HGAN9 = "Y3DRHH3L";
            tray.HGAN10 = "Y3DRHH3K";

            tray.HGAN11 = "Y3DRHH24";
            tray.HGAN12 = "Y3DRHH23";
            tray.HGAN13 = "Y3DRHH22";
            tray.HGAN14 = "Y3DRHH21";
            tray.HGAN15 = "Y3DRHH3H";                                      
            tray.HGAN16 = "Y3DRHH3F";
            tray.HGAN17 = "Y3DRGH2Z";
            tray.HGAN18 = "Y3DRGH2X";
            tray.HGAN19 = "Y3DRGH2V";
            tray.HGAN20 = "Y3DRGH2T";


            tray.HGAN21 = "";
            tray.HGAN22 = "";
            tray.HGAN23 = "";
            tray.HGAN24 = "";
            tray.HGAN25 = "";

            tray.HGAN26 = "";
            tray.HGAN27 = "";
            tray.HGAN28 = "";
            tray.HGAN29 = "";
            tray.HGAN30 = "";

            tray.HGAN31 = "";
            tray.HGAN32 = "";
            tray.HGAN33 = "";
            tray.HGAN34 = "";
            tray.HGAN35 = "";

            tray.HGAN36 = "";
            tray.HGAN37 = "";
            tray.HGAN38 = "";
            tray.HGAN39 = "";
            tray.HGAN40 = "";

            tray.HGAN41 = "";
            tray.HGAN42 = "";
            tray.HGAN43 = "";
            tray.HGAN44 = "";
            tray.HGAN45 = "";

            tray.HGAN46 = "";
            tray.HGAN47 = "";
            tray.HGAN48 = "";
            tray.HGAN49 = "";
            tray.HGAN50 = "";

            tray.HGAN51 = "";
            tray.HGAN52 = "";
            tray.HGAN53 = "";
            tray.HGAN54 = "";
            tray.HGAN55 = "";

            tray.HGAN56 = "";
            tray.HGAN57 = "";
            tray.HGAN58 = "";
            tray.HGAN59 = "";
            tray.HGAN60 = "";

            tray.DefectN1 = "A1,J4B,T6,68";
            tray.DefectN12 = "J4A";

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.File.WriteAllText(exePath + @"\" + tray.TrayID + ".XML", tray.ToXML());
        }
    }
}
