﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace AQCruncher
{
    public partial class AQTrayFileForm : Form
    {
        public AQTrayFileForm()
        {
            InitializeComponent();
            _init();
        }

        public AQTrayFileForm(AQTrayObj aq)
        {
            InitializeComponent();

            try
            {
                _aq = aq;
                _init();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                LoggerClass.Instance.ErrorLogInfo(ex.Message);
            }
        }

        /*1*/   private const string CONST_DATE = "DATE=";
        /*2*/   private const string CONST_TIME_START = "TIME START=";
        /*3*/   private const string CONST_TIME_END = "TIME END=";
        /*4*/   private const string CONST_USED_TIME = "USED TIME=";
        /*5*/   private const string CONST_TESTER_NUMBER = "TESTER NUMBER=";
        /*6*/   private const string CONST_CUSTOMER = "CUSTOMER=";
        /*7*/   private const string CONST_PRODUCT = "PRODUCT=";
        /*8*/   private const string CONST_USER = "USER=";

        /*9*/   private const string CONST_TRAY_ID = "TRAYID=";

        /*10*/  private const string CONST_LOTNUMBER = "LOTNUMBER=";
        /*11*/  private const string CONST_DOCCONTROL1 = "DOCCONTROL1=PN#";
        /*12*/  private const string CONST_DOCCONTROL2 = "DOCCONTROL2=STR#";
        /*13*/  private const string CONST_SUS = "SUS=";
        /*14*/  private const string CONST_ASSYLINE = "ASSYLINE=";

        
        private string[] arrInformation = new string[14];
        private string[] arrSerial = new string[60];

        private AQTrayObj _aq;
        private void _init()
        {
            arrInformation[0] = CONST_DATE;             //DATE=
            arrInformation[1] = CONST_TIME_START;       //TIME START=
            arrInformation[2] = CONST_TIME_END;         //TIME END=
            arrInformation[3] = CONST_USED_TIME;        //USED TIME=
            arrInformation[4] = CONST_TESTER_NUMBER;    //TESTER NUMBER=

            arrInformation[5] = CONST_CUSTOMER;         //CUSTOMER=WD
            arrInformation[6] = CONST_PRODUCT;          //PRODUCT=
            arrInformation[7] = CONST_USER;             //USER=

            arrInformation[8] = CONST_TRAY_ID;          //TRAYID=

            arrInformation[9] = CONST_LOTNUMBER;        //LOTNUMBER=
            arrInformation[10] = CONST_DOCCONTROL1;      //DOCCONTROL1=PN#
            arrInformation[11] = CONST_DOCCONTROL2;      //DOCCONTROL2=STR#
            arrInformation[12] = CONST_SUS;              //SUS=
            arrInformation[13] = CONST_ASSYLINE;         //ASSYLINE=

            for (int i = 0; i < 14; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = arrInformation[i];
                
                //item.SubItems.Add("xxx"+ i.ToString());
                switch (i)
                {
                    case 0:     
                        item.SubItems.Add(_aq.Date);
                        break;

                    case 1:    
                        item.SubItems.Add(_aq.TimeStart);
                        break;

                    case 2:     
                        item.SubItems.Add(_aq.TimeEnd);
                        break;

                    case 3:    
                        item.SubItems.Add(_aq.UsedTime);
                        break;

                    case 4:
                        item.SubItems.Add(_aq.TesterNumber);
                        break;

                    case 5:
                        item.SubItems.Add(_aq.Customer);
                        break;

                    case 6:     
                        item.SubItems.Add(_aq.Product);
                        break;

                    case 7:     
                        item.SubItems.Add(_aq.User);
                        break;

                    case 8:     
                        item.SubItems.Add(_aq.TrayID);
                        break;

                    case 9:     
                        item.SubItems.Add(_aq.LotNumber);
                        break;

                    case 10:     
                        item.SubItems.Add(_aq.DocControl1);
                        break;

                    case 11:
                        item.SubItems.Add(_aq.DocControl2);
                        break;

                    case 12:
                        item.SubItems.Add(_aq.Sus);
                        break;

                    case 13:
                        item.SubItems.Add(_aq.AssyLine);
                        break;

                    default:
                        break;
                }


                listViewInformation.Items.Add(item);
            }


            arrSerial[0] = _aq.HGAN1;
            arrSerial[1] = _aq.HGAN2;
            arrSerial[2] = _aq.HGAN3;
            arrSerial[3] = _aq.HGAN4;
            arrSerial[4] = _aq.HGAN5;

            arrSerial[5] = _aq.HGAN6;
            arrSerial[6] = _aq.HGAN7;
            arrSerial[7] = _aq.HGAN8;
            arrSerial[8] = _aq.HGAN9;
            arrSerial[9] = _aq.HGAN10;

            arrSerial[10] = _aq.HGAN11;
            arrSerial[11] = _aq.HGAN12;
            arrSerial[12] = _aq.HGAN13;
            arrSerial[13] = _aq.HGAN14;
            arrSerial[14] = _aq.HGAN15;

            arrSerial[15] = _aq.HGAN16;
            arrSerial[16] = _aq.HGAN17;
            arrSerial[17] = _aq.HGAN18;
            arrSerial[18] = _aq.HGAN19;
            arrSerial[19] = _aq.HGAN20;

            arrSerial[20] = _aq.HGAN21;
            arrSerial[21] = _aq.HGAN22;
            arrSerial[22] = _aq.HGAN23;
            arrSerial[23] = _aq.HGAN24;
            arrSerial[24] = _aq.HGAN25;

            arrSerial[25] = _aq.HGAN26;
            arrSerial[26] = _aq.HGAN27;
            arrSerial[27] = _aq.HGAN28;
            arrSerial[28] = _aq.HGAN29;
            arrSerial[29] = _aq.HGAN30;

            arrSerial[30] = _aq.HGAN31;
            arrSerial[31] = _aq.HGAN32;
            arrSerial[32] = _aq.HGAN33;
            arrSerial[33] = _aq.HGAN34;
            arrSerial[34] = _aq.HGAN35;

            arrSerial[35] = _aq.HGAN36;
            arrSerial[36] = _aq.HGAN37;
            arrSerial[37] = _aq.HGAN38;
            arrSerial[38] = _aq.HGAN39;
            arrSerial[39] = _aq.HGAN40;

            arrSerial[40] = _aq.HGAN41;
            arrSerial[41] = _aq.HGAN42;
            arrSerial[42] = _aq.HGAN43;
            arrSerial[43] = _aq.HGAN44;
            arrSerial[44] = _aq.HGAN45;

            arrSerial[45] = _aq.HGAN46;
            arrSerial[46] = _aq.HGAN47;
            arrSerial[47] = _aq.HGAN48;
            arrSerial[48] = _aq.HGAN49;
            arrSerial[49] = _aq.HGAN50;

            arrSerial[50] = _aq.HGAN51;
            arrSerial[51] = _aq.HGAN52;
            arrSerial[52] = _aq.HGAN53;
            arrSerial[53] = _aq.HGAN54;
            arrSerial[54] = _aq.HGAN55;

            arrSerial[55] = _aq.HGAN56;
            arrSerial[56] = _aq.HGAN57;
            arrSerial[57] = _aq.HGAN58;
            arrSerial[58] = _aq.HGAN59;
            arrSerial[59] = _aq.HGAN60;


            for (int i = 0; i < 60; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "HGAN" + (i + 1).ToString() + "=";
                item.SubItems.Add(arrSerial[i]);

                listViewSerial.Items.Add(item);
            }


            //AQTrayObj aTray = new AQTrayObj();
            //aTray.TrayID = "EA1231234Z";
            //aTray.Date = "27/02/2017";
            //aTray.SetTimeStart(DateTime.Now);
            //aTray.SetTimeEnd(DateTime.Parse(aTray.TimeStart).Add(new TimeSpan(0, 5, 0)));

            //aTray.HGAN1 = "NUMEEA";
            //aTray.HGAN2 = "NUMEEB";
            //aTray.HGAN10 = "NUMEEJ";
            //Console.WriteLine(aTray.ToXML());

            //AQTrayObj bTray = AQTrayObj.ToAQTrayObj(aTray.ToXML());
        }
    }
}
