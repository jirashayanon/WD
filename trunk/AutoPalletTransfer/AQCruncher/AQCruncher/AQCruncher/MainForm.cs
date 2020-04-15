﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

using System.Diagnostics;

using log4net;

namespace AQCruncher
{
    // ////////////////////////////////////////////////////////////////////////
    public partial class MainForm : Form
    {
        private string _strFileNamePallet1 = string.Empty;
        private string _strFileNamePallet2 = string.Empty;
        private string _strFileNameTray = string.Empty;
        private string _strAfterCrunchPath = string.Empty;

        private string CONST_EXE_PATH = System.Windows.Forms.Application.StartupPath;

        private AQCruncherConfig _config = new AQCruncherConfig();

        public MainForm(string[] args)
        {
            InitializeComponent();

            _config = AQCruncherConfig.ReadConfig(@"AQCruncherConfig.xml");

            //_strTrayLogPath = System.Windows.Forms.Application.StartupPath;
            //_strPalletLogPath = System.Windows.Forms.Application.StartupPath;
            //_strAQTrayPath = System.Windows.Forms.Application.StartupPath;

            _strConfigUNLOADFilePath = _config.UNLOADFilePath;
            _strConfigIAVIFilePath = _config.IAVIFilePath;
            _strConfigHOSTFilePath = _config.HOSTFilePath;
            _strConfigVMIFilePath = _config.VMIFilePath;
            _strConfigAUTOOCRFilePath = _config.AUTOOCRFilePath;
            _strConfigProduct = _config.Product;
            _strConfigAfterCrunchPath = _config.AfterCrunchPath;


            switch (args.Length)
            {
                case 0:
                    break;

                case 1:
                    if ((args[0] == "/?") || (args[0] == "-h") || (args[0] == "-help"))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Generate an AQ Tray file from UNLOAD and iAVI log files to a specific folder");
                        Console.WriteLine();
                        Console.WriteLine("AQCruncher [drive:][path][filename]");
                        Console.WriteLine();
                        break;
                    }

                    txtboxFilename_Tray.Text = args[0];
                    _strFileNameTray = args[0];

                    AQCruncher.LoggerClass.Instance.MainLogInfo("args[0] " + _strFileNameTray);                   
                    try
                    {
                        string[] temp = _strFileNameTray.Split('\\');
                        string strVMIFilename = temp[temp.Length - 1];   //remove path, keep only filename in the list
                        strVMIFilename = strVMIFilename.Split('.')[0];

                        AQTrayObj aq = crunch(this, null);

                        string exePath = System.Windows.Forms.Application.StartupPath;
                        //System.IO.File.WriteAllText(exePath + @"\" + aq.TrayID + ".VMI", aq.ToXML());
                        //System.IO.File.WriteAllText(_strVMIFilePath + @"\" + aq.TrayID + ".VMI", aq.ToXML());
                        System.IO.File.WriteAllText(_strConfigVMIFilePath + @"\" + strVMIFilename + ".VMI", aq.ToXML());

                        Console.WriteLine(exePath + @"\" + aq.TrayID + ".VMI generated");
                    }
                    catch (Exception ex)
                    {
                        AQCruncher.LoggerClass.Instance.ErrorLogInfo("MainForm: case1:" + ex.Message);
                        //MessageBox.Show(ex.Message);
                    }

                    break;

                case 2:
                    txtboxFilename_Tray.Text = args[0];
                    _strFileNameTray = args[0];
                    _strAfterCrunchPath = args[1];

                    AQCruncher.LoggerClass.Instance.MainLogInfo("args[0] " + _strFileNameTray + ", args[1] " + _strAfterCrunchPath);
                    try
                    {
                        string[] temp = _strFileNameTray.Split('\\');
                        string strVMIFilename = temp[temp.Length - 1];   //remove path, keep only filename in the list
                        strVMIFilename = strVMIFilename.Split('.')[0];

                        AQTrayObj aq = crunch(this, null);

                        string exePath = System.Windows.Forms.Application.StartupPath;
                        //System.IO.File.WriteAllText(exePath + @"\" + aq.TrayID + ".VMI", aq.ToXML());
                        //System.IO.File.WriteAllText(_strVMIFilePath + @"\" + aq.TrayID + ".VMI", aq.ToXML());
                        System.IO.File.WriteAllText(_strConfigVMIFilePath + @"\" + strVMIFilename + ".VMI", aq.ToXML());
                        Console.WriteLine(exePath + @"\" + aq.TrayID + ".VMI generated");
                    }
                    catch (Exception ex)
                    {
                        AQCruncher.LoggerClass.Instance.ErrorLogInfo("MainForm: case2:" + ex.Message);
                        //MessageBox.Show(ex.Message);
                    }

                    break;


                default:
                    AQCruncher.LoggerClass.Instance.MainLogInfo("default:");
                    break;
            }
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBrowsePallet1_Click(object sender, EventArgs e)
        {
            DialogResult dlgret = filedialog_Pallet1.ShowDialog();
            if (dlgret == System.Windows.Forms.DialogResult.OK)
            {
                txtboxFilename_Pallet1.Text = _strFileNamePallet1 = filedialog_Pallet1.FileName;
            }
        }

        private void btnBrowsePallet2_Click(object sender, EventArgs e)
        {
            DialogResult dlgret = filedialog_Pallet2.ShowDialog();
            if (dlgret == System.Windows.Forms.DialogResult.OK)
            {
                txtboxFilename_Pallet2.Text = _strFileNamePallet2 = filedialog_Pallet2.FileName;
            }
        }

        private void btnBrowseTray_Click(object sender, EventArgs e)
        {
            DialogResult dlgret = filedialog_Tray.ShowDialog();
            if (dlgret == System.Windows.Forms.DialogResult.OK)
            {
                txtboxFilename_Tray.Text = _strFileNameTray = filedialog_Tray.FileName;
            }
        }

        private void btnShowInfoTray_Click(object sender, EventArgs e)
        {
            TrayLogFileForm trayFrm = new TrayLogFileForm(txtboxFilename_Tray.Text);
            //TrayLogFileForm trayFrm = new TrayLogFileForm();
            trayFrm.ShowDialog();
        }

        private void btnShowInfoPallet1_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet1.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }

        private void btnShowInfoPallet2_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet2.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }

        private void btnShowAQTray_Click(object sender, EventArgs e)
        {
            AQTrayFileForm aqtrayFrm = new AQTrayFileForm(_aq);
            aqtrayFrm.ShowDialog();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            AQTrayObj aqtray = new AQTrayObj();

            aqtray.Date = "2/11/2017";
            aqtray.TimeStart = "5:51:27 PM";
            aqtray.TimeEnd = "5:51:28 PM";

            aqtray.TesterNumber = "VOR001";
            aqtray.Customer = "WD";
            aqtray.Product = "TRAILS_C8_SD";

            aqtray.TrayID = "EZ132838AK";
            aqtray.LotNumber = "Y4RTN_A1";
            aqtray.DocControl1 = "PN#70929-15-SA5";
            aqtray.DocControl2 = "STR#70929-15-SA5";
            aqtray.Sus = "MPT-4G";
            aqtray.AssyLine = "B4A101";

            aqtray.HGAN1 = "Y4RTP71V";
            aqtray.HGAN2 = "Y4RTP71U";
            aqtray.HGAN3 = "Y4RTP71T";
            aqtray.HGAN4 = "Y4RTP71S";
            aqtray.HGAN5 = "Y4RTP71R";
            aqtray.HGAN6 = "Y4RTP71P";
            aqtray.HGAN7 = "Y4RTP71N";
            aqtray.HGAN8 = "Y4RTP71M";
            aqtray.HGAN9 = "Y4RTP71L";
            aqtray.HGAN10 = "Y4RTP71K";
            aqtray.HGAN11 = "";
            aqtray.HGAN12 = "Y4RTN72P";
            aqtray.HGAN13 = "Y4RTN71M";
            aqtray.HGAN14 = "Y4RTN71L";
            aqtray.HGAN15 = "Y4RTN71K";
            aqtray.HGAN16 = "Y4RTN71J";
            aqtray.HGAN17 = "Y4RTN71H";
            aqtray.HGAN18 = "Y4RTN71G";
            aqtray.HGAN19 = "Y4RTN71F";
            aqtray.HGAN20 = "Y4RTN71E";

            //aqtray.ToAQTrayFile(CONST_EXE_PATH + @"\" + aqtray.TrayID + @".LOG");
            aqtray.ToAQTrayFile(CONST_EXE_PATH + @"\" + aqtray.TrayID + @".mt2");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private string _strConfigUNLOADFilePath = string.Empty;
        public string UNLOADFilePath
        {
            get { return _strConfigUNLOADFilePath; }
            set { _strConfigUNLOADFilePath = value; }
        }

        private string _strConfigIAVIFilePath = "";
        public string IAVIFilePath
        {
            get { return _strConfigIAVIFilePath; }
            set { _strConfigIAVIFilePath = value; }
        }

        private string _strConfigHOSTFilePath = "";
        public string HOSTFilePath
        {
            get { return _strConfigHOSTFilePath; }
            set { _strConfigHOSTFilePath = value; }
        }

        private string _strConfigVMIFilePath = string.Empty;
        public string VMIFilePath
        {
            get { return _strConfigVMIFilePath; }
            set { _strConfigVMIFilePath = value; }
        }

        private string _strConfigAUTOOCRFilePath = string.Empty;
        public string AUTOOCRFilePath
        {
            get { return _strConfigAUTOOCRFilePath; }
            set { _strConfigAUTOOCRFilePath = value; }
        }

        private string _strConfigProduct = string.Empty;
        public string Product
        {
            get { return _strConfigProduct; }
            set { _strConfigProduct = value; }
        }


        private string _strConfigAfterCrunchPath = string.Empty;
        public string AfterCrunchPath
        {
            get { return _strConfigAfterCrunchPath; }
            set { _strConfigAfterCrunchPath = value; }
        }


        public AQTrayObj crunch(object sender, EventArgs e)
        {
            if (txtboxFilename_Tray.Text.Length < 1)
            {
                AQCruncher.LoggerClass.Instance.ErrorLogInfo("AQTrayObj crunch no filename specified");
                return new AQTrayObj();
            }

            TrayObj tray = new TrayObj();
            tray.ReadFile(txtboxFilename_Tray.Text);

            AQCruncher.LoggerClass.Instance.MainLogInfo("ReadFile: " + txtboxFilename_Tray.Text);
            if (_strAfterCrunchPath.Length > 0)     //_strAfterCrunchPath is set at command line, by CruncherAgent
            {
                //string[] onlyfilename = txtboxFilename_Tray.Text.Split('\\');
                //string xxx = onlyfilename[onlyfilename.Length - 1];
                //MessageBox.Show(xxx, "xxx");
                //MessageBox.Show(Path.GetFileName(txtboxFilename_Tray.Text), "getfilename");


                if (File.Exists(_strAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text)))
                {
                    File.Delete(_strAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
                }
                //File.Move(txtboxFilename_Tray.Text, _strAfterCrunchPath + @"\" + tray.TrayID + ".XML");
                File.Move(txtboxFilename_Tray.Text, _strAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
                AQCruncher.LoggerClass.Instance.MainLogInfo("Move file: " + txtboxFilename_Tray.Text + " to " + _strAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
            }
            else        //_strConfigAfterCrunchPath is set in config file
            {
                if (File.Exists(_strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text)))
                {
                    File.Delete(_strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
                }

                if (!Directory.Exists(_strConfigAfterCrunchPath))
                {
                    Directory.CreateDirectory(_strConfigAfterCrunchPath);
                }

                File.Move(txtboxFilename_Tray.Text, _strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
                AQCruncher.LoggerClass.Instance.MainLogInfo("Move file: " + txtboxFilename_Tray.Text + " to " + _strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtboxFilename_Tray.Text));
            }

            if (tray.PalletsCount == 0)
            {
                AQCruncher.LoggerClass.Instance.ErrorLogInfo("AQTrayObj crunch pallet count is 0");
                return new AQTrayObj();
            }

            PalletObj[] arrPallet = new PalletObj[tray.PalletsCount];
            for (int i = 0; i < tray.PalletsCount; i++)
            {
                arrPallet[i] = new PalletObj();
            }
            AQCruncher.LoggerClass.Instance.MainLogInfo("tray.PalletsCount: " + tray.PalletsCount.ToString());

            // 6 pallets

            if (tray.Pallet1.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet1 + @".IAVI"))
                {
                    arrPallet[0].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet1 + @".IAVI");
                    txtboxFilename_Pallet1.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet1 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet1 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet1 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet1 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet1 + @".IAVI");
                    }
                }
            }

            if (tray.Pallet2.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet2 + @".IAVI"))
                {
                    arrPallet[1].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet2 + @".IAVI");
                    txtboxFilename_Pallet2.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet2 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet2 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet2 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet2 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet2 + @".IAVI");
                    }
                }
            }

            if (tray.Pallet3.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet3 + @".IAVI"))
                {
                    arrPallet[2].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet3 + @".IAVI");
                    txtboxFilename_Pallet3.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet3 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet3 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet3 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet3 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet3 + @".IAVI");
                    }
                }
            }

            if (tray.Pallet4.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet4 + @".IAVI"))
                {
                    arrPallet[3].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet4 + @".IAVI");
                    txtboxFilename_Pallet4.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet4 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet4 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet4 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet4 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet4 + @".IAVI");
                    }
                }
            }

            if (tray.Pallet5.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet5 + @".IAVI"))
                {
                    arrPallet[4].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet5 + @".IAVI");
                    txtboxFilename_Pallet5.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet5 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet5 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet5 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet5 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet5 + @".IAVI");
                    }
                }
            }

            if (tray.Pallet6.Length > 0)
            {
                if (File.Exists(_strConfigIAVIFilePath + @"\" + tray.Pallet6 + @".IAVI"))
                {
                    arrPallet[5].ReadFile(_strConfigIAVIFilePath + @"\" + tray.Pallet6 + @".IAVI");
                    txtboxFilename_Pallet6.Text = _strConfigIAVIFilePath + @"\" + tray.Pallet6 + @".IAVI";

                    if (_strAfterCrunchPath.Length > 0)
                    {
                        if (!Directory.Exists(_strAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet6 + @".IAVI", _strAfterCrunchPath + @"\" + tray.Pallet6 + @".IAVI");
                    }
                    else
                    {
                        if (!Directory.Exists(_strConfigAfterCrunchPath))
                        {
                            Directory.CreateDirectory(_strConfigAfterCrunchPath);
                        }

                        File.Move(_strConfigIAVIFilePath + @"\" + tray.Pallet6 + @".IAVI", _strConfigAfterCrunchPath + @"\" + tray.Pallet6 + @".IAVI");
                    }
                }
            }

            int nCheckPallet = 0;
            for (int i = 0; i < arrPallet.Length; i++)
            {
                nCheckPallet = arrPallet[i].PalletID.Length | nCheckPallet; 
            }

            AQCruncher.LoggerClass.Instance.MainLogInfo("nCheckPallett: " + nCheckPallet.ToString());
            AQCruncher.LoggerClass.Instance.MainLogInfo("arrPallet.Length: " + arrPallet.Length.ToString());
            if((nCheckPallet == 0) && (arrPallet.Length >0))      
            {
                if(tray.Pallet1.Length > 0)
                    arrPallet[0].PalletID = tray.Pallet1;

                if(tray.Pallet2.Length > 0)
                    arrPallet[1].PalletID = tray.Pallet2;

                if (tray.Pallet3.Length > 0)
                    arrPallet[2].PalletID = tray.Pallet3;

                if (tray.Pallet4.Length > 0)
                    arrPallet[3].PalletID = tray.Pallet4;

                if (tray.Pallet5.Length > 0)
                    arrPallet[4].PalletID = tray.Pallet5;

                if (tray.Pallet6.Length > 0)
                    arrPallet[5].PalletID = tray.Pallet6;


                //in case, file name is not purely the barcode 
                DirectoryInfo configDirIAVIInfo = new DirectoryInfo(_strConfigIAVIFilePath);
                //FileInfo[] configIAVIfilesInfo = dirIAVIInfo.GetFiles("*.IAVI").OrderBy(p => p.LastWriteTime).ToArray();
                FileInfo[] configIAVIfilesInfo = configDirIAVIInfo.GetFiles("*.IAVI").OrderByDescending(p => p.LastWriteTime).ToArray();

                for (int j = 0; j < arrPallet.Length; j++)
                {                
                    foreach (FileInfo file in configIAVIfilesInfo)
                    {
                        if (file.FullName.Contains(arrPallet[j].PalletID))
                        {
                            AQCruncher.LoggerClass.Instance.MainLogInfo("file.FullName: " + file.FullName);
                            arrPallet[j].ReadFile(file.FullName);

                            TextBox txtbox = (TextBox)this.Controls.Find("txtboxFilename_Pallet" + (j + 1).ToString(), true).FirstOrDefault();
                            txtbox.Text = file.FullName;

                            if (_strAfterCrunchPath.Length > 0)
                            {
                                if (File.Exists(_strAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text)))
                                {
                                    File.Delete(_strAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text));
                                }
                                File.Move(txtbox.Text, _strAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text));
                            }
                            else
                            {
                                if (!Directory.Exists(_strConfigAfterCrunchPath))
                                {
                                    Directory.CreateDirectory(_strConfigAfterCrunchPath);
                                }

                                if (File.Exists(_strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text)))
                                {
                                    File.Delete(_strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text));
                                }
                                File.Move(txtbox.Text, _strConfigAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text));
                            }

                            AQCruncher.LoggerClass.Instance.MainLogInfo("Move file: " + txtbox.Text + " to " + _strAfterCrunchPath + @"\" + Path.GetFileName(txtbox.Text));
                            break;
                        }
                    }
                }
            }



            AQTrayObj aq = null;
            switch (tray.PalletsCount)
            {
                case 1:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0]);
                    break;

                case 2:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1]);
                    break;

                case 3:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID + ", " + arrPallet[2].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1], arrPallet[2]);
                    break;

                case 4:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID + ", " + arrPallet[2].PalletID + ", " + arrPallet[3].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1], arrPallet[2], arrPallet[3]);
                    break;

                case 5:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID + ", " + arrPallet[2].PalletID + ", " + arrPallet[3].PalletID + ", " + arrPallet[4].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1], arrPallet[2], arrPallet[3], arrPallet[4]);
                    break;

                case 6:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID + ", " + arrPallet[2].PalletID + ", " + arrPallet[3].PalletID + ", " + arrPallet[4].PalletID + ", " + arrPallet[5].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1], arrPallet[2], arrPallet[3], arrPallet[4], arrPallet[5]);
                    break;

                default:
                    AQCruncher.LoggerClass.Instance.MainLogInfo(arrPallet[0].PalletID + ", " + arrPallet[1].PalletID);
                    aq = new AQTrayObj(tray, arrPallet[0], arrPallet[1]);
                    break;
            }

            int nTrayType = 0;
            if (tray.TrayID.Substring(0, 1) == "4")
            {
                nTrayType = 40;
            }
            else if (tray.TrayID.Substring(0, 1) == "6")
            {
                nTrayType = 60;
            }
            else
            {
                nTrayType = 20;
            }

            //.LOG on EasyTray
            if (aq.Product.Length < 1)  //if no Product identified in IAVI's log
            {
                //aq.ToAQTrayFile(_strConfigAUTOOCRFilePath + @"\" + _strConfigProduct + @"\" + tray.TrayID + @".LOG", true);     //.LOG on EasyTray
                aq.ToAQTrayFile(_strConfigAUTOOCRFilePath + @"\" + _strConfigProduct + @"\" + tray.TrayID + @".LOG", true, nTrayType);     //.LOG on EasyTray
                AQCruncher.LoggerClass.Instance.MainLogInfo("toAQTrayFile: " + _strConfigAUTOOCRFilePath + @"\" + _strConfigProduct + @"\" + tray.TrayID + @".LOG, " + nTrayType.ToString());
            }
            else
            {
                //aq.ToAQTrayFile(_strConfigAUTOOCRFilePath + @"\" + aq.Product + @"\" + tray.TrayID + @".LOG", true);            //.LOG on EasyTray
                aq.ToAQTrayFile(_strConfigAUTOOCRFilePath + @"\" + aq.Product + @"\" + tray.TrayID + @".LOG", true, nTrayType);            //.LOG on EasyTray
                AQCruncher.LoggerClass.Instance.MainLogInfo("toAQTrayFile: " + _strConfigAUTOOCRFilePath + @"\" + aq.Product + @"\" + tray.TrayID + @".LOG, " + nTrayType.ToString());
            }


            //local backup
            string strLocalBackupFilePath = string.Empty;
            if (aq.Product.Length < 1)
            {
                strLocalBackupFilePath = CONST_EXE_PATH + @"\backup\" + _strConfigProduct + @"\" + DateTime.Today.ToString("MM-dd-yyyy");
            }
            else
            {
                strLocalBackupFilePath = CONST_EXE_PATH + @"\backup\" + aq.Product + @"\" + DateTime.Today.ToString("MM-dd-yyyy");
            }

            if (!System.IO.Directory.Exists(strLocalBackupFilePath))
            {
                System.IO.Directory.CreateDirectory(strLocalBackupFilePath);
            }
            //aq.ToAQTrayFile(strLocalBackupFilePath + @"\" + tray.TrayID + @".LOG", true);    //LOG on local backup
            aq.ToAQTrayFile(strLocalBackupFilePath + @"\" + tray.TrayID + @".LOG", true, nTrayType);    //LOG on local backup
            AQCruncher.LoggerClass.Instance.MainLogInfo("Backup: " + strLocalBackupFilePath + @"\" + tray.TrayID + @".LOG, " + nTrayType.ToString());


            //remote backup on WDTBHGAFS01
            string strRemoteBackupFilePath = @"\\wdtbhgafs01\ocr\AUTOOCR\Tray";
            if (System.IO.Directory.Exists(strRemoteBackupFilePath))
            {
                //aq.ToAQTrayFile(strRemoteBackupFilePath + @"\" + tray.TrayID + @".LOG", true);    //Backup LOG on \\WDTBHGAFS01\ocr\AUTOOCR\Tray
                aq.ToAQTrayFile(strRemoteBackupFilePath + @"\" + tray.TrayID + @".LOG", true, nTrayType);    //Backup LOG on \\WDTBHGAFS01\ocr\AUTOOCR\Tray
                AQCruncher.LoggerClass.Instance.MainLogInfo("Backup: " + strRemoteBackupFilePath + @"\" + tray.TrayID + @".LOG, " + nTrayType.ToString());                
            }


            //.mt2 on HOST
            //aq.ToAQTrayFile(_strConfigHOSTFilePath + @"\" + tray.TrayID + ".mt2", true);      //.mt2 on HOST
            aq.ToAQTrayFile(_strConfigHOSTFilePath + @"\" + tray.TrayID + ".mt2", true, nTrayType);      //.mt2 on HOST
            AQCruncher.LoggerClass.Instance.MainLogInfo("mt2: " + _strConfigHOSTFilePath + @"\" + tray.TrayID + ".mt2, " + nTrayType.ToString());
            AQCruncher.LoggerClass.Instance.MainLogInfo("-----");

            return aq;
        }

        private AQTrayObj _aq;
        private void btnCrunch_Click(object sender, EventArgs e)
        {
            //System.IO.File.WriteAllText("xxx.xml", _config.ToXML());
            //return;

            _aq = this.crunch(sender, e);

            string exePath = System.Windows.Forms.Application.StartupPath;
            //System.IO.File.WriteAllText(exePath + @"\" + _aq.TrayID + ".VMI", _aq.ToXML());
            System.IO.File.WriteAllText(_strConfigVMIFilePath + @"\" + _aq.TrayID + ".VMI", _aq.ToXML());
            Console.WriteLine(exePath + @"\" + _aq.TrayID + ".VMI generated");

            int nTrayType = 0;
            if(_aq.TrayID.Substring(0, 1) == "4")
            {
                nTrayType = 40;
            }
            else if(_aq.TrayID.Substring(0, 1) == "6")
            {
                nTrayType = 60;
            }
            else
            {
                nTrayType = 20;
            }

            _aq.ToAQTrayFile(_strConfigVMIFilePath + @"\" + _aq.TrayID + ".mt2", true, nTrayType);
        }

        private void btnShowInfoPallet3_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet3.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }

        private void btnShowInfoPallet4_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet4.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }

        private void btnShowInfoPallet5_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet5.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }

        private void btnShowInfoPallet6_Click(object sender, EventArgs e)
        {
            PalletLogFileForm palletFrm = new PalletLogFileForm(txtboxFilename_Pallet6.Text);
            //PalletLogFileForm palletFrm = new PalletLogFileForm();
            palletFrm.ShowDialog();
        }
    }


    // ////////////////////////////////////////////////////////////////////////
    public class PalletObj
    {
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

        private string _strPalletID = string.Empty;
        public string PalletID
        {
            get { return _strPalletID; }
            set {_strPalletID = value;}
        }

        private string _strTesterNumber = string.Empty;
        public string TesterNumber
        {
            get { return _strTesterNumber; }
            set { _strTesterNumber = value; }
        }

        private string _strCustomer = string.Empty;
        public string Customer
        {
            get { return _strCustomer; }
            set { _strCustomer = value; }
        }

        private string _strProduct = string.Empty;
        public string Product
        {
            get { return _strProduct; }
            set { _strProduct = value; }
        }

        private string _strUser = string.Empty;
        public string User
        {
            get { return _strUser; }
            set { _strUser = value; }
        }

        private string _strLotNumber = string.Empty;
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private string _strDocCtrl1 = string.Empty;
        public string DocControl1
        {
            get { return _strDocCtrl1; }
            set { _strDocCtrl1 = value; }
        }

        private string _strDocCtrl2 = string.Empty;
        public string DocControl2
        {
            get { return _strDocCtrl2; }
            set { _strDocCtrl2 = value; }
        }

        private string _strSus = string.Empty;
        public string Sus
        {
            get { return _strSus; }
            set { _strSus = value; }
        }

        private string _strAssyLine = string.Empty;
        public string AssyLine
        {
            get { return _strAssyLine; }
            set { _strAssyLine = value; }
        }


        #region HGAN
        private string _strHGAN1 = string.Empty;
        public string HGAN1
        {
            get { return _strHGAN1; }
            set { _strHGAN1 = value; }

        }

        private string _strHGAN2 = string.Empty;
        public string HGAN2
        {
            get { return _strHGAN2; }
            set { _strHGAN2 = value; }

        }

        private string _strHGAN3 = string.Empty;
        public string HGAN3
        {
            get { return _strHGAN3; }
            set { _strHGAN3 = value; }

        }

        private string _strHGAN4 = string.Empty;
        public string HGAN4
        {
            get { return _strHGAN4; }
            set { _strHGAN4 = value; }

        }

        private string _strHGAN5 = string.Empty;
        public string HGAN5
        {
            get { return _strHGAN5; }
            set { _strHGAN5 = value; }

        }

        private string _strHGAN6 = string.Empty;
        public string HGAN6
        {
            get { return _strHGAN6; }
            set { _strHGAN6 = value; }

        }

        private string _strHGAN7 = string.Empty;
        public string HGAN7
        {
            get { return _strHGAN7; }
            set { _strHGAN7 = value; }

        }

        private string _strHGAN8 = string.Empty;
        public string HGAN8
        {
            get { return _strHGAN8; }
            set { _strHGAN8 = value; }

        }

        private string _strHGAN9 = string.Empty;
        public string HGAN9
        {
            get { return _strHGAN9; }
            set { _strHGAN9 = value; }

        }

        private string _strHGAN10 = string.Empty;
        public string HGAN10
        {
            get { return _strHGAN10; }
            set { _strHGAN10 = value; }

        }

        #endregion


        #region DefectN
        //comma separated string format
        private string _strDefectN1 = string.Empty;
        public string DefectN1
        {
            get { return _strDefectN1; }
            set { _strDefectN1 = value; }

        }

        private string _strDefectN2 = string.Empty;
        public string DefectN2
        {
            get { return _strDefectN2; }
            set { _strDefectN2 = value; }

        }

        private string _strDefectN3 = string.Empty;
        public string DefectN3
        {
            get { return _strDefectN3; }
            set { _strDefectN3 = value; }

        }

        private string _strDefectN4 = string.Empty;
        public string DefectN4
        {
            get { return _strDefectN4; }
            set { _strDefectN4 = value; }

        }

        private string _strDefectN5 = string.Empty;
        public string DefectN5
        {
            get { return _strDefectN5; }
            set { _strDefectN5 = value; }

        }

        private string _strDefectN6 = string.Empty;
        public string DefectN6
        {
            get { return _strDefectN6; }
            set { _strDefectN6 = value; }

        }

        private string _strDefectN7 = string.Empty;
        public string DefectN7
        {
            get { return _strDefectN7; }
            set { _strDefectN7 = value; }

        }

        private string _strDefectN8 = string.Empty;
        public string DefectN8
        {
            get { return _strDefectN8; }
            set { _strDefectN8 = value; }

        }


        private string _strDefectN9 = string.Empty;
        public string DefectN9
        {
            get { return _strDefectN9; }
            set { _strDefectN9 = value; }

        }

        private string _strDefectN10 = string.Empty;
        public string DefectN10
        {
            get { return _strDefectN10; }
            set { _strDefectN10 = value; }

        }

        #endregion




        public void ReadFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return;
            }

            string text = System.IO.File.ReadAllText(strFilePath);
            PalletObj aPallet = PalletObj.ToPalletObj(text);

            this.PalletID = aPallet.PalletID;
            this.TesterNumber = aPallet.TesterNumber;
            this.Customer = aPallet.Customer;
            this.Product = aPallet.Product;
            this.User = aPallet.User;
            this.LotNumber = aPallet.LotNumber;
            this.DocControl1 = aPallet.DocControl1;
            this.DocControl2 = aPallet.DocControl2;
            this.Sus = aPallet.Sus;
            this.AssyLine = aPallet.AssyLine;


            this.HGAN1 = aPallet.HGAN1;
            this.HGAN2 = aPallet.HGAN2;
            this.HGAN3 = aPallet.HGAN3;
            this.HGAN4 = aPallet.HGAN4;
            this.HGAN5 = aPallet.HGAN5;

            this.HGAN6 = aPallet.HGAN6;
            this.HGAN7 = aPallet.HGAN7;
            this.HGAN8 = aPallet.HGAN8;
            this.HGAN9 = aPallet.HGAN9;
            this.HGAN10 = aPallet.HGAN10;


            this.DefectN1 = aPallet.DefectN1;
            this.DefectN2 = aPallet.DefectN2;
            this.DefectN3 = aPallet.DefectN3;
            this.DefectN4 = aPallet.DefectN4;
            this.DefectN5 = aPallet.DefectN5;

            this.DefectN6 = aPallet.DefectN6;
            this.DefectN7 = aPallet.DefectN7;
            this.DefectN8 = aPallet.DefectN8;
            this.DefectN9 = aPallet.DefectN9;
            this.DefectN10 = aPallet.DefectN10;
        }

        public string ToXML()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using(var writer = XmlWriter.Create(sb, settings))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

                writer.WriteStartDocument();
                x.Serialize(writer, this);
            }

            return sb.ToString();
        }

        public static PalletObj ToPalletObj(string strXML)
        {
            PalletObj aPallet;
            
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PalletObj));
            using (StringReader reader = new StringReader(strXML))
            {
                aPallet = (PalletObj)x.Deserialize(reader);
            }

            return aPallet;
        }

    }


    // ////////////////////////////////////////////////////////////////////////
    public class TrayObj
    {
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

        private const string CONST_DATETIME_FORMAT = "M/d/yyyy h:mm:ss tt";
        private const string CONST_TIMESTARTEND_FORMAT = "h:mm:ss tt";

        private DateTime _dtDate = new DateTime();
        public string Date
        {
            get { return _dtDate.ToString("M/d/yyyy"); }
            //set { _dtDate = DateTime.ParseExact(value, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { _dtDate = DateTime.ParseExact(value, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
        }

        private DateTime _dtTimeStart = new DateTime();
        public string TimeStart
        {
            get { return _dtTimeStart.ToString(CONST_TIMESTARTEND_FORMAT); }
            set 
            {
                try
                {
                    _dtTimeStart = DateTime.Parse(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void SetTimeStart(DateTime dtStart)
        {
            _dtTimeStart = dtStart;
        }


        private DateTime _dtTimeEnd = new DateTime();
        public string TimeEnd
        {
            get { return _dtTimeEnd.ToString(CONST_TIMESTARTEND_FORMAT); }
            set
            {
                try
                {
                    _dtTimeEnd = DateTime.Parse(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void SetTimeEnd(DateTime dtEnd)
        {
            _dtTimeEnd = dtEnd;
        }


        private string _strUsedTime = string.Empty;
        public string UsedTime
        {
            get
            {
                TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
                _strUsedTime = duration.ToString(@"mm\:ss");

                return _strUsedTime;
            }

            set
            {
                //Console.WriteLine((_dtTimeEnd.ToString(CONST_DATETIME_FORMAT)));
                //Console.WriteLine((_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));

                TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
                _strUsedTime = duration.ToString(@"mm\:ss");
            }
        }

        private string _strTrayID = string.Empty;
        public string TrayID
        {
            get { return _strTrayID; }
            set { _strTrayID = value; }
        }

        private string _strPallet1 = string.Empty;
        public string Pallet1
        {
            get { return _strPallet1; }
            set { _strPallet1 = value; }
        }

        private string _strPallet2 = string.Empty;
        public string Pallet2
        {
            get { return _strPallet2; }
            set { _strPallet2 = value; }
        }

        private string _strPallet3 = string.Empty;
        public string Pallet3
        {
            get { return _strPallet3; }
            set { _strPallet3 = value; }
        }

        private string _strPallet4 = string.Empty;
        public string Pallet4
        {
            get { return _strPallet4; }
            set { _strPallet4 = value; }
        }

        private string _strPallet5 = string.Empty;
        public string Pallet5
        {
            get { return _strPallet5; }
            set { _strPallet5 = value; }
        }

        private string _strPallet6 = string.Empty;
        public string Pallet6
        {
            get { return _strPallet6; }
            set { _strPallet6 = value; }
        }



        #region HGAN
        private string _strHGAN1 = string.Empty;
        public string HGAN1
        {
            get { return _strHGAN1; }
            set { _strHGAN1 = value; }

        }

        private string _strHGAN2 = string.Empty;
        public string HGAN2
        {
            get { return _strHGAN2; }
            set { _strHGAN2 = value; }

        }

        private string _strHGAN3 = string.Empty;
        public string HGAN3
        {
            get { return _strHGAN3; }
            set { _strHGAN3 = value; }

        }

        private string _strHGAN4 = string.Empty;
        public string HGAN4
        {
            get { return _strHGAN4; }
            set { _strHGAN4 = value; }

        }

        private string _strHGAN5 = string.Empty;
        public string HGAN5
        {
            get { return _strHGAN5; }
            set { _strHGAN5 = value; }

        }

        private string _strHGAN6 = string.Empty;
        public string HGAN6
        {
            get { return _strHGAN6; }
            set { _strHGAN6 = value; }

        }

        private string _strHGAN7 = string.Empty;
        public string HGAN7
        {
            get { return _strHGAN7; }
            set { _strHGAN7 = value; }

        }

        private string _strHGAN8 = string.Empty;
        public string HGAN8
        {
            get { return _strHGAN8; }
            set { _strHGAN8 = value; }

        }

        private string _strHGAN9 = string.Empty;
        public string HGAN9
        {
            get { return _strHGAN9; }
            set { _strHGAN9 = value; }

        }

        private string _strHGAN10 = string.Empty;
        public string HGAN10
        {
            get { return _strHGAN10; }
            set { _strHGAN10 = value; }

        }

        private string _strHGAN11 = string.Empty;
        public string HGAN11
        {
            get { return _strHGAN11; }
            set { _strHGAN11 = value; }

        }

        private string _strHGAN12 = string.Empty;
        public string HGAN12
        {
            get { return _strHGAN12; }
            set { _strHGAN12 = value; }

        }

        private string _strHGAN13 = string.Empty;
        public string HGAN13
        {
            get { return _strHGAN13; }
            set { _strHGAN13 = value; }

        }

        private string _strHGAN14 = string.Empty;
        public string HGAN14
        {
            get { return _strHGAN14; }
            set { _strHGAN14 = value; }

        }

        private string _strHGAN15 = string.Empty;
        public string HGAN15
        {
            get { return _strHGAN15; }
            set { _strHGAN15 = value; }

        }

        private string _strHGAN16 = string.Empty;
        public string HGAN16
        {
            get { return _strHGAN16; }
            set { _strHGAN16 = value; }

        }

        private string _strHGAN17 = string.Empty;
        public string HGAN17
        {
            get { return _strHGAN17; }
            set { _strHGAN17 = value; }

        }

        private string _strHGAN18 = string.Empty;
        public string HGAN18
        {
            get { return _strHGAN18; }
            set { _strHGAN18 = value; }

        }

        private string _strHGAN19 = string.Empty;
        public string HGAN19
        {
            get { return _strHGAN19; }
            set { _strHGAN19 = value; }

        }

        private string _strHGAN20 = string.Empty;
        public string HGAN20
        {
            get { return _strHGAN20; }
            set { _strHGAN20 = value; }

        }

        private string _strHGAN21 = string.Empty;
        public string HGAN21
        {
            get { return _strHGAN21; }
            set { _strHGAN21 = value; }

        }

        private string _strHGAN22 = string.Empty;
        public string HGAN22
        {
            get { return _strHGAN22; }
            set { _strHGAN22 = value; }

        }

        private string _strHGAN23 = string.Empty;
        public string HGAN23
        {
            get { return _strHGAN23; }
            set { _strHGAN23 = value; }

        }

        private string _strHGAN24 = string.Empty;
        public string HGAN24
        {
            get { return _strHGAN24; }
            set { _strHGAN24 = value; }

        }

        private string _strHGAN25 = string.Empty;
        public string HGAN25
        {
            get { return _strHGAN25; }
            set { _strHGAN25 = value; }

        }

        private string _strHGAN26 = string.Empty;
        public string HGAN26
        {
            get { return _strHGAN26; }
            set { _strHGAN26 = value; }

        }

        private string _strHGAN27 = string.Empty;
        public string HGAN27
        {
            get { return _strHGAN27; }
            set { _strHGAN27 = value; }

        }

        private string _strHGAN28 = string.Empty;
        public string HGAN28
        {
            get { return _strHGAN28; }
            set { _strHGAN28 = value; }

        }

        private string _strHGAN29 = string.Empty;
        public string HGAN29
        {
            get { return _strHGAN29; }
            set { _strHGAN29 = value; }

        }

        private string _strHGAN30 = string.Empty;
        public string HGAN30
        {
            get { return _strHGAN30; }
            set { _strHGAN30 = value; }

        }

        private string _strHGAN31 = string.Empty;
        public string HGAN31
        {
            get { return _strHGAN31; }
            set { _strHGAN31 = value; }

        }

        private string _strHGAN32 = string.Empty;
        public string HGAN32
        {
            get { return _strHGAN32; }
            set { _strHGAN32 = value; }

        }

        private string _strHGAN33 = string.Empty;
        public string HGAN33
        {
            get { return _strHGAN33; }
            set { _strHGAN33 = value; }

        }

        private string _strHGAN34 = string.Empty;
        public string HGAN34
        {
            get { return _strHGAN34; }
            set { _strHGAN34 = value; }

        }

        private string _strHGAN35 = string.Empty;
        public string HGAN35
        {
            get { return _strHGAN35; }
            set { _strHGAN35 = value; }

        }

        private string _strHGAN36 = string.Empty;
        public string HGAN36
        {
            get { return _strHGAN36; }
            set { _strHGAN36 = value; }

        }

        private string _strHGAN37 = string.Empty;
        public string HGAN37
        {
            get { return _strHGAN37; }
            set { _strHGAN37 = value; }

        }

        private string _strHGAN38 = string.Empty;
        public string HGAN38
        {
            get { return _strHGAN38; }
            set { _strHGAN38 = value; }

        }

        private string _strHGAN39 = string.Empty;
        public string HGAN39
        {
            get { return _strHGAN39; }
            set { _strHGAN39 = value; }

        }

        private string _strHGAN40 = string.Empty;
        public string HGAN40
        {
            get { return _strHGAN40; }
            set { _strHGAN40 = value; }

        }

        private string _strHGAN41 = string.Empty;
        public string HGAN41
        {
            get { return _strHGAN41; }
            set { _strHGAN41 = value; }

        }

        private string _strHGAN42 = string.Empty;
        public string HGAN42
        {
            get { return _strHGAN42; }
            set { _strHGAN42 = value; }

        }

        private string _strHGAN43 = string.Empty;
        public string HGAN43
        {
            get { return _strHGAN43; }
            set { _strHGAN43 = value; }

        }

        private string _strHGAN44 = string.Empty;
        public string HGAN44
        {
            get { return _strHGAN44; }
            set { _strHGAN44 = value; }

        }

        private string _strHGAN45 = string.Empty;
        public string HGAN45
        {
            get { return _strHGAN45; }
            set { _strHGAN45 = value; }

        }

        private string _strHGAN46 = string.Empty;
        public string HGAN46
        {
            get { return _strHGAN46; }
            set { _strHGAN46 = value; }

        }

        private string _strHGAN47 = string.Empty;
        public string HGAN47
        {
            get { return _strHGAN47; }
            set { _strHGAN47 = value; }

        }

        private string _strHGAN48 = string.Empty;
        public string HGAN48
        {
            get { return _strHGAN48; }
            set { _strHGAN48 = value; }

        }

        private string _strHGAN49 = string.Empty;
        public string HGAN49
        {
            get { return _strHGAN49; }
            set { _strHGAN49 = value; }

        }

        private string _strHGAN50 = string.Empty;
        public string HGAN50
        {
            get { return _strHGAN50; }
            set { _strHGAN50 = value; }

        }

        private string _strHGAN51 = string.Empty;
        public string HGAN51
        {
            get { return _strHGAN51; }
            set { _strHGAN51 = value; }

        }

        private string _strHGAN52 = string.Empty;
        public string HGAN52
        {
            get { return _strHGAN52; }
            set { _strHGAN52 = value; }

        }

        private string _strHGAN53 = string.Empty;
        public string HGAN53
        {
            get { return _strHGAN53; }
            set { _strHGAN53 = value; }

        }

        private string _strHGAN54 = string.Empty;
        public string HGAN54
        {
            get { return _strHGAN54; }
            set { _strHGAN54 = value; }

        }

        private string _strHGAN55 = string.Empty;
        public string HGAN55
        {
            get { return _strHGAN55; }
            set { _strHGAN55 = value; }

        }

        private string _strHGAN56 = string.Empty;
        public string HGAN56
        {
            get { return _strHGAN56; }
            set { _strHGAN56 = value; }

        }

        private string _strHGAN57 = string.Empty;
        public string HGAN57
        {
            get { return _strHGAN57; }
            set { _strHGAN57 = value; }

        }

        private string _strHGAN58 = string.Empty;
        public string HGAN58
        {
            get { return _strHGAN58; }
            set { _strHGAN58 = value; }

        }

        private string _strHGAN59 = string.Empty;
        public string HGAN59
        {
            get { return _strHGAN59; }
            set { _strHGAN59 = value; }

        }

        private string _strHGAN60 = string.Empty;
        public string HGAN60
        {
            get { return _strHGAN60; }
            set { _strHGAN60 = value; }

        }

        #endregion



        #region DefectN
        //comma separated string format
        private string _strDefectN1 = string.Empty;
        public string DefectN1
        {
            get { return _strDefectN1; }
            set { _strDefectN1 = value; }

        }

        private string _strDefectN2 = string.Empty;
        public string DefectN2
        {
            get { return _strDefectN2; }
            set { _strDefectN2 = value; }

        }

        private string _strDefectN3 = string.Empty;
        public string DefectN3
        {
            get { return _strDefectN3; }
            set { _strDefectN3 = value; }

        }

        private string _strDefectN4 = string.Empty;
        public string DefectN4
        {
            get { return _strDefectN4; }
            set { _strDefectN4 = value; }

        }

        private string _strDefectN5 = string.Empty;
        public string DefectN5
        {
            get { return _strDefectN5; }
            set { _strDefectN5 = value; }

        }

        private string _strDefectN6 = string.Empty;
        public string DefectN6
        {
            get { return _strDefectN6; }
            set { _strDefectN6 = value; }

        }

        private string _strDefectN7 = string.Empty;
        public string DefectN7
        {
            get { return _strDefectN7; }
            set { _strDefectN7 = value; }

        }

        private string _strDefectN8 = string.Empty;
        public string DefectN8
        {
            get { return _strDefectN8; }
            set { _strDefectN8 = value; }

        }

        private string _strDefectN9 = string.Empty;
        public string DefectN9
        {
            get { return _strDefectN9; }
            set { _strDefectN9 = value; }

        }

        private string _strDefectN10 = string.Empty;
        public string DefectN10
        {
            get { return _strDefectN10; }
            set { _strDefectN10 = value; }

        }

        private string _strDefectN11 = string.Empty;
        public string DefectN11
        {
            get { return _strDefectN11; }
            set { _strDefectN11 = value; }

        }

        private string _strDefectN12 = string.Empty;
        public string DefectN12
        {
            get { return _strDefectN12; }
            set { _strDefectN12 = value; }

        }

        private string _strDefectN13 = string.Empty;
        public string DefectN13
        {
            get { return _strDefectN13; }
            set { _strDefectN13 = value; }

        }

        private string _strDefectN14 = string.Empty;
        public string DefectN14
        {
            get { return _strDefectN14; }
            set { _strDefectN14 = value; }

        }

        private string _strDefectN15 = string.Empty;
        public string DefectN15
        {
            get { return _strDefectN15; }
            set { _strDefectN15 = value; }

        }

        private string _strDefectN16 = string.Empty;
        public string DefectN16
        {
            get { return _strDefectN16; }
            set { _strDefectN16 = value; }

        }

        private string _strDefectN17 = string.Empty;
        public string DefectN17
        {
            get { return _strDefectN17; }
            set { _strDefectN17 = value; }

        }

        private string _strDefectN18 = string.Empty;
        public string DefectN18
        {
            get { return _strDefectN18; }
            set { _strDefectN18 = value; }

        }

        private string _strDefectN19 = string.Empty;
        public string DefectN19
        {
            get { return _strDefectN19; }
            set { _strDefectN19 = value; }

        }

        private string _strDefectN20 = string.Empty;
        public string DefectN20
        {
            get { return _strDefectN20; }
            set { _strDefectN20 = value; }

        }

        private string _strDefectN21 = string.Empty;
        public string DefectN21
        {
            get { return _strDefectN21; }
            set { _strDefectN21 = value; }

        }

        private string _strDefectN22 = string.Empty;
        public string DefectN22
        {
            get { return _strDefectN22; }
            set { _strDefectN22 = value; }

        }

        private string _strDefectN23 = string.Empty;
        public string DefectN23
        {
            get { return _strDefectN23; }
            set { _strDefectN23 = value; }

        }

        private string _strDefectN24 = string.Empty;
        public string DefectN24
        {
            get { return _strDefectN24; }
            set { _strDefectN24 = value; }

        }

        private string _strDefectN25 = string.Empty;
        public string DefectN25
        {
            get { return _strDefectN25; }
            set { _strDefectN25 = value; }

        }

        private string _strDefectN26 = string.Empty;
        public string DefectN26
        {
            get { return _strDefectN26; }
            set { _strDefectN26 = value; }

        }

        private string _strDefectN27 = string.Empty;
        public string DefectN27
        {
            get { return _strDefectN27; }
            set { _strDefectN27 = value; }

        }

        private string _strDefectN28 = string.Empty;
        public string DefectN28
        {
            get { return _strDefectN28; }
            set { _strDefectN28 = value; }

        }

        private string _strDefectN29 = string.Empty;
        public string DefectN29
        {
            get { return _strDefectN29; }
            set { _strDefectN29 = value; }

        }

        private string _strDefectN30 = string.Empty;
        public string DefectN30
        {
            get { return _strDefectN30; }
            set { _strDefectN30 = value; }

        }

        private string _strDefectN31 = string.Empty;
        public string DefectN31
        {
            get { return _strDefectN31; }
            set { _strDefectN31 = value; }

        }

        private string _strDefectN32 = string.Empty;
        public string DefectN32
        {
            get { return _strDefectN32; }
            set { _strDefectN32 = value; }

        }

        private string _strDefectN33 = string.Empty;
        public string DefectN33
        {
            get { return _strDefectN33; }
            set { _strDefectN33 = value; }

        }

        private string _strDefectN34 = string.Empty;
        public string DefectN34
        {
            get { return _strDefectN34; }
            set { _strDefectN34 = value; }

        }

        private string _strDefectN35 = string.Empty;
        public string DefectN35
        {
            get { return _strDefectN35; }
            set { _strDefectN35 = value; }

        }

        private string _strDefectN36 = string.Empty;
        public string DefectN36
        {
            get { return _strDefectN36; }
            set { _strDefectN36 = value; }

        }

        private string _strDefectN37 = string.Empty;
        public string DefectN37
        {
            get { return _strDefectN37; }
            set { _strDefectN37 = value; }

        }

        private string _strDefectN38 = string.Empty;
        public string DefectN38
        {
            get { return _strDefectN38; }
            set { _strDefectN38 = value; }

        }

        private string _strDefectN39 = string.Empty;
        public string DefectN39
        {
            get { return _strDefectN39; }
            set { _strDefectN39 = value; }

        }

        private string _strDefectN40 = string.Empty;
        public string DefectN40
        {
            get { return _strDefectN40; }
            set { _strDefectN40 = value; }

        }

        private string _strDefectN41 = string.Empty;
        public string DefectN41
        {
            get { return _strDefectN41; }
            set { _strDefectN41 = value; }

        }

        private string _strDefectN42 = string.Empty;
        public string DefectN42
        {
            get { return _strDefectN42; }
            set { _strDefectN42 = value; }

        }

        private string _strDefectN43 = string.Empty;
        public string DefectN43
        {
            get { return _strDefectN43; }
            set { _strDefectN43 = value; }

        }

        private string _strDefectN44 = string.Empty;
        public string DefectN44
        {
            get { return _strDefectN44; }
            set { _strDefectN44 = value; }

        }

        private string _strDefectN45 = string.Empty;
        public string DefectN45
        {
            get { return _strDefectN45; }
            set { _strDefectN45 = value; }

        }

        private string _strDefectN46 = string.Empty;
        public string DefectN46
        {
            get { return _strDefectN46; }
            set { _strDefectN46 = value; }

        }

        private string _strDefectN47 = string.Empty;
        public string DefectN47
        {
            get { return _strDefectN47; }
            set { _strDefectN47 = value; }

        }

        private string _strDefectN48 = string.Empty;
        public string DefectN48
        {
            get { return _strDefectN48; }
            set { _strDefectN48 = value; }

        }

        private string _strDefectN49 = string.Empty;
        public string DefectN49
        {
            get { return _strDefectN49; }
            set { _strDefectN49 = value; }

        }

        private string _strDefectN50 = string.Empty;
        public string DefectN50
        {
            get { return _strDefectN50; }
            set { _strDefectN50 = value; }

        }

        private string _strDefectN51 = string.Empty;
        public string DefectN51
        {
            get { return _strDefectN51; }
            set { _strDefectN51 = value; }

        }

        private string _strDefectN52 = string.Empty;
        public string DefectN52
        {
            get { return _strDefectN52; }
            set { _strDefectN52 = value; }

        }

        private string _strDefectN53 = string.Empty;
        public string DefectN53
        {
            get { return _strDefectN53; }
            set { _strDefectN53 = value; }

        }

        private string _strDefectN54 = string.Empty;
        public string DefectN54
        {
            get { return _strDefectN54; }
            set { _strDefectN54 = value; }

        }

        private string _strDefectN55 = string.Empty;
        public string DefectN55
        {
            get { return _strDefectN55; }
            set { _strDefectN55 = value; }

        }

        private string _strDefectN56 = string.Empty;
        public string DefectN56
        {
            get { return _strDefectN56; }
            set { _strDefectN56 = value; }

        }

        private string _strDefectN57 = string.Empty;
        public string DefectN57
        {
            get { return _strDefectN57; }
            set { _strDefectN57 = value; }

        }

        private string _strDefectN58 = string.Empty;
        public string DefectN58
        {
            get { return _strDefectN58; }
            set { _strDefectN58 = value; }

        }

        private string _strDefectN59 = string.Empty;
        public string DefectN59
        {
            get { return _strDefectN59; }
            set { _strDefectN59 = value; }

        }

        private string _strDefectN60 = string.Empty;
        public string DefectN60
        {
            get { return _strDefectN60; }
            set { _strDefectN60 = value; }

        }

        #endregion


        public int PalletsCount
        {
            get
            {
                int nCount = 0;

                if (this.Pallet1.Length > 0)
                    nCount++;

                if (this.Pallet2.Length > 0)
                    nCount++;

                if (this.Pallet3.Length > 0)
                    nCount++;

                if (this.Pallet4.Length > 0)
                    nCount++;

                if (this.Pallet5.Length > 0)
                    nCount++;

                if (this.Pallet6.Length > 0)
                    nCount++;

                return nCount;
            }
        }

        public void ReadFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return;
            }

            string text = System.IO.File.ReadAllText(strFilePath);
            TrayObj aTray = TrayObj.ToTrayObj(text);

            this.Date = aTray.Date;
            this.TimeStart = aTray.TimeStart;
            this.TimeEnd = aTray.TimeEnd;
            this.UsedTime = aTray.UsedTime;
            this.TrayID = aTray.TrayID;
            this.Pallet1 = aTray.Pallet1;
            this.Pallet2 = aTray.Pallet2;
            this.Pallet3 = aTray.Pallet3;
            this.Pallet4 = aTray.Pallet4;
            this.Pallet5 = aTray.Pallet5;
            this.Pallet6 = aTray.Pallet6;


            this.HGAN1 = aTray.HGAN1;
            this.HGAN2 = aTray.HGAN2;
            this.HGAN3 = aTray.HGAN3;
            this.HGAN4 = aTray.HGAN4;
            this.HGAN5 = aTray.HGAN5;

            this.HGAN6 = aTray.HGAN6;
            this.HGAN7 = aTray.HGAN7;
            this.HGAN8 = aTray.HGAN8;
            this.HGAN9 = aTray.HGAN9;
            this.HGAN10 = aTray.HGAN10;

            this.HGAN11 = aTray.HGAN11;
            this.HGAN12 = aTray.HGAN12;
            this.HGAN13 = aTray.HGAN13;
            this.HGAN14 = aTray.HGAN14;
            this.HGAN15 = aTray.HGAN15;

            this.HGAN16 = aTray.HGAN16;
            this.HGAN17 = aTray.HGAN17;
            this.HGAN18 = aTray.HGAN18;
            this.HGAN19 = aTray.HGAN19;
            this.HGAN20 = aTray.HGAN20;

            this.HGAN21 = aTray.HGAN21;
            this.HGAN22 = aTray.HGAN22;
            this.HGAN23 = aTray.HGAN23;
            this.HGAN24 = aTray.HGAN24;
            this.HGAN25 = aTray.HGAN25;

            this.HGAN26 = aTray.HGAN26;
            this.HGAN27 = aTray.HGAN27;
            this.HGAN28 = aTray.HGAN28;
            this.HGAN29 = aTray.HGAN29;
            this.HGAN30 = aTray.HGAN30;

            this.HGAN31 = aTray.HGAN31;
            this.HGAN32 = aTray.HGAN32;
            this.HGAN33 = aTray.HGAN33;
            this.HGAN34 = aTray.HGAN34;
            this.HGAN35 = aTray.HGAN35;

            this.HGAN36 = aTray.HGAN36;
            this.HGAN37 = aTray.HGAN37;
            this.HGAN38 = aTray.HGAN38;
            this.HGAN39 = aTray.HGAN39;
            this.HGAN40 = aTray.HGAN40;

            this.HGAN41 = aTray.HGAN41;
            this.HGAN42 = aTray.HGAN42;
            this.HGAN43 = aTray.HGAN43;
            this.HGAN44 = aTray.HGAN44;
            this.HGAN45 = aTray.HGAN45;

            this.HGAN46 = aTray.HGAN46;
            this.HGAN47 = aTray.HGAN47;
            this.HGAN48 = aTray.HGAN48;
            this.HGAN49 = aTray.HGAN49;
            this.HGAN50 = aTray.HGAN50;

            this.HGAN51 = aTray.HGAN51;
            this.HGAN52 = aTray.HGAN52;
            this.HGAN53 = aTray.HGAN53;
            this.HGAN54 = aTray.HGAN54;
            this.HGAN55 = aTray.HGAN55;

            this.HGAN56 = aTray.HGAN56;
            this.HGAN57 = aTray.HGAN57;
            this.HGAN58 = aTray.HGAN58;
            this.HGAN59 = aTray.HGAN59;
            this.HGAN60 = aTray.HGAN60;


            this.DefectN1 = aTray.DefectN1;
            this.DefectN2 = aTray.DefectN2;
            this.DefectN3 = aTray.DefectN3;
            this.DefectN4 = aTray.DefectN4;
            this.DefectN5 = aTray.DefectN5;

            this.DefectN6 = aTray.DefectN6;
            this.DefectN7 = aTray.DefectN7;
            this.DefectN8 = aTray.DefectN8;
            this.DefectN9 = aTray.DefectN9;
            this.DefectN10 = aTray.DefectN10;

            this.DefectN11 = aTray.DefectN11;
            this.DefectN12 = aTray.DefectN12;
            this.DefectN13 = aTray.DefectN13;
            this.DefectN14 = aTray.DefectN14;
            this.DefectN15 = aTray.DefectN15;

            this.DefectN16 = aTray.DefectN16;
            this.DefectN17 = aTray.DefectN17;
            this.DefectN18 = aTray.DefectN18;
            this.DefectN19 = aTray.DefectN19;
            this.DefectN20 = aTray.DefectN20;

            this.DefectN21 = aTray.DefectN21;
            this.DefectN22 = aTray.DefectN22;
            this.DefectN23 = aTray.DefectN23;
            this.DefectN24 = aTray.DefectN24;
            this.DefectN25 = aTray.DefectN25;

            this.DefectN26 = aTray.DefectN26;
            this.DefectN27 = aTray.DefectN27;
            this.DefectN28 = aTray.DefectN28;
            this.DefectN29 = aTray.DefectN29;
            this.DefectN30 = aTray.DefectN30;

            this.DefectN31 = aTray.DefectN31;
            this.DefectN32 = aTray.DefectN32;
            this.DefectN33 = aTray.DefectN33;
            this.DefectN34 = aTray.DefectN34;
            this.DefectN35 = aTray.DefectN35;

            this.DefectN36 = aTray.DefectN36;
            this.DefectN37 = aTray.DefectN37;
            this.DefectN38 = aTray.DefectN38;
            this.DefectN39 = aTray.DefectN39;
            this.DefectN40 = aTray.DefectN40;

            this.DefectN41 = aTray.DefectN41;
            this.DefectN42 = aTray.DefectN42;
            this.DefectN43 = aTray.DefectN43;
            this.DefectN44 = aTray.DefectN44;
            this.DefectN45 = aTray.DefectN45;

            this.DefectN46 = aTray.DefectN46;
            this.DefectN47 = aTray.DefectN47;
            this.DefectN48 = aTray.DefectN48;
            this.DefectN49 = aTray.DefectN49;
            this.DefectN50 = aTray.DefectN50;

            this.DefectN51 = aTray.DefectN51;
            this.DefectN52 = aTray.DefectN52;
            this.DefectN53 = aTray.DefectN53;
            this.DefectN54 = aTray.DefectN54;
            this.DefectN55 = aTray.DefectN55;

            this.DefectN56 = aTray.DefectN56;
            this.DefectN57 = aTray.DefectN57;
            this.DefectN58 = aTray.DefectN58;
            this.DefectN59 = aTray.DefectN59;
            this.DefectN60 = aTray.DefectN60;

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

        public static TrayObj ToTrayObj(string strXML)
        {
            TrayObj aTray;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(TrayObj));
            using (StringReader reader = new StringReader(strXML))
            {
                aTray = (TrayObj)x.Deserialize(reader);
            }

            return aTray;
        }



    }


    // ////////////////////////////////////////////////////////////////////////
    public class AQTrayObj
    {
        /*1*/        private const string CONST_DATE = "DATE=";
        /*2*/        private const string CONST_TIME_START = "TIME START=";
        /*3*/        private const string CONST_TIME_END = "TIME END=";
        /*4*/        private const string CONST_USED_TIME = "USED TIME=";
        /*5*/        private const string CONST_TESTER_NUMBER = "TESTER NUMBER=";
        /*6*/        private const string CONST_CUSTOMER = "CUSTOMER=";
        /*7*/        private const string CONST_PRODUCT = "PRODUCT=";
        /*8*/        private const string CONST_USER = "USER=";

        /*9*/        private const string CONST_TRAY_ID = "TRAYID=";

        /*10*/       private const string CONST_LOTNUMBER = "LOTNUMBER=";
        /*11*/       private const string CONST_DOCCONTROL1 = "DOCCONTROL1=PN#";
        /*12*/       private const string CONST_DOCCONTROL2 = "DOCCONTROL2=STR#";
        /*13*/       private const string CONST_SUS = "SUS=";
        /*14*/       private const string CONST_ASSYLINE = "ASSYLINE=";


        private const string CONST_HGAN1 = "HGAN1=";
        private const string CONST_HGAN2 = "HGAN2=";
        private const string CONST_HGAN3 = "HGAN3=";
        private const string CONST_HGAN4 = "HGAN4=";
        private const string CONST_HGAN5 = "HGAN5=";
        private const string CONST_HGAN6 = "HGAN6=";
        private const string CONST_HGAN7 = "HGAN7=";
        private const string CONST_HGAN8 = "HGAN8=";
        private const string CONST_HGAN9 = "HGAN9=";
        private const string CONST_HGAN10 = "HGAN10=";

        private const string CONST_HGAN11 = "HGAN11=";
        private const string CONST_HGAN12 = "HGAN12=";
        private const string CONST_HGAN13 = "HGAN13=";
        private const string CONST_HGAN14 = "HGAN14=";
        private const string CONST_HGAN15 = "HGAN15=";
        private const string CONST_HGAN16 = "HGAN16=";
        private const string CONST_HGAN17 = "HGAN17=";
        private const string CONST_HGAN18 = "HGAN18=";
        private const string CONST_HGAN19 = "HGAN19=";
        private const string CONST_HGAN20 = "HGAN20=";

        private const string CONST_HGAN21 = "HGAN21=";
        private const string CONST_HGAN22 = "HGAN22=";
        private const string CONST_HGAN23 = "HGAN23=";
        private const string CONST_HGAN24 = "HGAN24=";
        private const string CONST_HGAN25 = "HGAN25=";
        private const string CONST_HGAN26 = "HGAN26=";
        private const string CONST_HGAN27 = "HGAN27=";
        private const string CONST_HGAN28 = "HGAN28=";
        private const string CONST_HGAN29 = "HGAN29=";
        private const string CONST_HGAN30 = "HGAN30=";

        private const string CONST_HGAN31 = "HGAN31=";
        private const string CONST_HGAN32 = "HGAN32=";
        private const string CONST_HGAN33 = "HGAN33=";
        private const string CONST_HGAN34 = "HGAN34=";
        private const string CONST_HGAN35 = "HGAN35=";
        private const string CONST_HGAN36 = "HGAN36=";
        private const string CONST_HGAN37 = "HGAN37=";
        private const string CONST_HGAN38 = "HGAN38=";
        private const string CONST_HGAN39 = "HGAN39=";
        private const string CONST_HGAN40 = "HGAN40=";

        private const string CONST_HGAN41 = "HGAN41=";
        private const string CONST_HGAN42 = "HGAN42=";
        private const string CONST_HGAN43 = "HGAN43=";
        private const string CONST_HGAN44 = "HGAN44=";
        private const string CONST_HGAN45 = "HGAN45=";
        private const string CONST_HGAN46 = "HGAN46=";
        private const string CONST_HGAN47 = "HGAN47=";
        private const string CONST_HGAN48 = "HGAN48=";
        private const string CONST_HGAN49 = "HGAN49=";
        private const string CONST_HGAN50 = "HGAN50=";

        private const string CONST_HGAN51 = "HGAN51=";
        private const string CONST_HGAN52 = "HGAN52=";
        private const string CONST_HGAN53 = "HGAN53=";
        private const string CONST_HGAN54 = "HGAN54=";
        private const string CONST_HGAN55 = "HGAN55=";
        private const string CONST_HGAN56 = "HGAN56=";
        private const string CONST_HGAN57 = "HGAN57=";
        private const string CONST_HGAN58 = "HGAN58=";
        private const string CONST_HGAN59 = "HGAN59=";
        private const string CONST_HGAN60 = "HGAN60=";


        private const string CONST_DATETIME_FORMAT = "M/d/yyyy h:mm:ss tt";
        private const string CONST_TIMESTARTEND_FORMAT = "h:mm:ss tt";

        private const string CONST_CANNOT_READ_8OCR = @"????????";
        private const string CONST_CANNOT_READ_10OCR = @"??????????";

        private const string CONST_EMPTY_SPOT = @"";           //obsolete, empty spot, in AQ tray file, contains blank
        private const string CONST_EMPTY_SPOT_8OCR = @"--------";
        private const string CONST_EMPTY_SPOT_10OCR = @"----------";

        #region ctor


        public AQTrayObj()
        {
        }


        public AQTrayObj(TrayObj tray, PalletObj pallet1)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);             //obsolete
            //this.HGAN1 = (tray.HGAN1 == "-") ? CONST_EMPTY_SPOT_10OCR : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);


            /*
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);
            */

            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;
       }


        public AQTrayObj(TrayObj tray, PalletObj pallet1, PalletObj pallet2)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;
            this.Pallet2 = pallet2.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);

            //this.HGAN11 = pallet2.HGAN1;
            this.HGAN11 = (tray.HGAN11 == "-") ? string.Empty : ((pallet2.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN1);
            //this.HGAN12 = pallet2.HGAN2;
            this.HGAN12 = (tray.HGAN12 == "-") ? string.Empty : ((pallet2.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN2);
            //this.HGAN13 = pallet2.HGAN3;
            this.HGAN13 = (tray.HGAN13 == "-") ? string.Empty : ((pallet2.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN3);
            //this.HGAN14 = pallet2.HGAN4;
            this.HGAN14 = (tray.HGAN14 == "-") ? string.Empty : ((pallet2.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN4);
            //this.HGAN15 = pallet2.HGAN5;
            this.HGAN15 = (tray.HGAN15 == "-") ? string.Empty : ((pallet2.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN5);
            //this.HGAN16 = pallet2.HGAN6;
            this.HGAN16 = (tray.HGAN16 == "-") ? string.Empty : ((pallet2.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN6);
            //this.HGAN17 = pallet2.HGAN7;
            this.HGAN17 = (tray.HGAN17 == "-") ? string.Empty : ((pallet2.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN7);
            //this.HGAN18 = pallet2.HGAN8;
            this.HGAN18 = (tray.HGAN18 == "-") ? string.Empty : ((pallet2.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN8);
            //this.HGAN19 = pallet2.HGAN9;
            this.HGAN19 = (tray.HGAN19 == "-") ? string.Empty : ((pallet2.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN9);
            //this.HGAN20 = pallet2.HGAN10;
            this.HGAN20 = (tray.HGAN20 == "-") ? string.Empty : ((pallet2.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN10);


            /*
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);

            this.HGAN11 = ((pallet2.DefectN1.Length > 0) ? "" : pallet2.HGAN1);
            this.HGAN12 = ((pallet2.DefectN2.Length > 0) ? "" : pallet2.HGAN2);
            this.HGAN13 = ((pallet2.DefectN3.Length > 0) ? "" : pallet2.HGAN3);
            this.HGAN14 = ((pallet2.DefectN4.Length > 0) ? "" : pallet2.HGAN4);
            this.HGAN15 = ((pallet2.DefectN5.Length > 0) ? "" : pallet2.HGAN5);
            this.HGAN16 = ((pallet2.DefectN6.Length > 0) ? "" : pallet2.HGAN6);
            this.HGAN17 = ((pallet2.DefectN7.Length > 0) ? "" : pallet2.HGAN7);
            this.HGAN18 = ((pallet2.DefectN8.Length > 0) ? "" : pallet2.HGAN8);
            this.HGAN19 = ((pallet2.DefectN9.Length > 0) ? "" : pallet2.HGAN9);
            this.HGAN20 = ((pallet2.DefectN10.Length > 0) ? "" : pallet2.HGAN10);
            */


            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;

            this.DefectN11 = pallet2.DefectN1;
            this.DefectN12 = pallet2.DefectN2;
            this.DefectN13 = pallet2.DefectN3;
            this.DefectN14 = pallet2.DefectN4;
            this.DefectN15 = pallet2.DefectN5;
            this.DefectN16 = pallet2.DefectN6;
            this.DefectN17 = pallet2.DefectN7;
            this.DefectN18 = pallet2.DefectN8;
            this.DefectN19 = pallet2.DefectN9;
            this.DefectN20 = pallet2.DefectN10;
        }


        public AQTrayObj(TrayObj tray, PalletObj pallet1, PalletObj pallet2, PalletObj pallet3)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;
            this.Pallet2 = pallet2.PalletID;
            this.Pallet3 = pallet3.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);

            //this.HGAN11 = pallet2.HGAN1;
            this.HGAN11 = (tray.HGAN11 == "-") ? string.Empty : ((pallet2.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN1);
            //this.HGAN12 = pallet2.HGAN2;
            this.HGAN12 = (tray.HGAN12 == "-") ? string.Empty : ((pallet2.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN2);
            //this.HGAN13 = pallet2.HGAN3;
            this.HGAN13 = (tray.HGAN13 == "-") ? string.Empty : ((pallet2.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN3);
            //this.HGAN14 = pallet2.HGAN4;
            this.HGAN14 = (tray.HGAN14 == "-") ? string.Empty : ((pallet2.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN4);
            //this.HGAN15 = pallet2.HGAN5;
            this.HGAN15 = (tray.HGAN15 == "-") ? string.Empty : ((pallet2.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN5);
            //this.HGAN16 = pallet2.HGAN6;
            this.HGAN16 = (tray.HGAN16 == "-") ? string.Empty : ((pallet2.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN6);
            //this.HGAN17 = pallet2.HGAN7;
            this.HGAN17 = (tray.HGAN17 == "-") ? string.Empty : ((pallet2.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN7);
            //this.HGAN18 = pallet2.HGAN8;
            this.HGAN18 = (tray.HGAN18 == "-") ? string.Empty : ((pallet2.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN8);
            //this.HGAN19 = pallet2.HGAN9;
            this.HGAN19 = (tray.HGAN19 == "-") ? string.Empty : ((pallet2.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN9);
            //this.HGAN20 = pallet2.HGAN10;
            this.HGAN20 = (tray.HGAN20 == "-") ? string.Empty : ((pallet2.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN10);

            //this.HGAN21 = pallet3.HGAN1;
            this.HGAN21 = (tray.HGAN21 == "-") ? string.Empty : ((pallet3.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN1);
            //this.HGAN22 = pallet3.HGAN2;
            this.HGAN22 = (tray.HGAN22 == "-") ? string.Empty : ((pallet3.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN2);
            //this.HGAN23 = pallet3.HGAN3;
            this.HGAN23 = (tray.HGAN23 == "-") ? string.Empty : ((pallet3.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN3);
            //this.HGAN24 = pallet3.HGAN4;
            this.HGAN24 = (tray.HGAN24 == "-") ? string.Empty : ((pallet3.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN4);
            //this.HGAN25 = pallet3.HGAN5;
            this.HGAN25 = (tray.HGAN25 == "-") ? string.Empty : ((pallet3.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN5);
            //this.HGAN26 = pallet3.HGAN6;
            this.HGAN26 = (tray.HGAN26 == "-") ? string.Empty : ((pallet3.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN6);
            //this.HGAN27 = pallet3.HGAN7;
            this.HGAN27 = (tray.HGAN27 == "-") ? string.Empty : ((pallet3.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN7);
            //this.HGAN28 = pallet3.HGAN8;
            this.HGAN28 = (tray.HGAN28 == "-") ? string.Empty : ((pallet3.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN8);
            //this.HGAN29 = pallet3.HGAN9;
            this.HGAN29 = (tray.HGAN29 == "-") ? string.Empty : ((pallet3.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN9);
            //this.HGAN30 = pallet3.HGAN10;
            this.HGAN30 = (tray.HGAN30 == "-") ? string.Empty : ((pallet3.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN10);


            /*
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);

            this.HGAN11 = ((pallet2.DefectN1.Length > 0) ? "" : pallet2.HGAN1);
            this.HGAN12 = ((pallet2.DefectN2.Length > 0) ? "" : pallet2.HGAN2);
            this.HGAN13 = ((pallet2.DefectN3.Length > 0) ? "" : pallet2.HGAN3);
            this.HGAN14 = ((pallet2.DefectN4.Length > 0) ? "" : pallet2.HGAN4);
            this.HGAN15 = ((pallet2.DefectN5.Length > 0) ? "" : pallet2.HGAN5);
            this.HGAN16 = ((pallet2.DefectN6.Length > 0) ? "" : pallet2.HGAN6);
            this.HGAN17 = ((pallet2.DefectN7.Length > 0) ? "" : pallet2.HGAN7);
            this.HGAN18 = ((pallet2.DefectN8.Length > 0) ? "" : pallet2.HGAN8);
            this.HGAN19 = ((pallet2.DefectN9.Length > 0) ? "" : pallet2.HGAN9);
            this.HGAN20 = ((pallet2.DefectN10.Length > 0) ? "" : pallet2.HGAN10);

            this.HGAN21 = ((pallet3.DefectN1.Length > 0) ? "" : pallet3.HGAN1);
            this.HGAN22 = ((pallet3.DefectN2.Length > 0) ? "" : pallet3.HGAN2);
            this.HGAN23 = ((pallet3.DefectN3.Length > 0) ? "" : pallet3.HGAN3);
            this.HGAN24 = ((pallet3.DefectN4.Length > 0) ? "" : pallet3.HGAN4);
            this.HGAN25 = ((pallet3.DefectN5.Length > 0) ? "" : pallet3.HGAN5);
            this.HGAN26 = ((pallet3.DefectN6.Length > 0) ? "" : pallet3.HGAN6);
            this.HGAN27 = ((pallet3.DefectN7.Length > 0) ? "" : pallet3.HGAN7);
            this.HGAN28 = ((pallet3.DefectN8.Length > 0) ? "" : pallet3.HGAN8);
            this.HGAN29 = ((pallet3.DefectN9.Length > 0) ? "" : pallet3.HGAN9);
            this.HGAN30 = ((pallet3.DefectN10.Length > 0) ? "" : pallet3.HGAN10);
            */


            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;

            this.DefectN11 = pallet2.DefectN1;
            this.DefectN12 = pallet2.DefectN2;
            this.DefectN13 = pallet2.DefectN3;
            this.DefectN14 = pallet2.DefectN4;
            this.DefectN15 = pallet2.DefectN5;
            this.DefectN16 = pallet2.DefectN6;
            this.DefectN17 = pallet2.DefectN7;
            this.DefectN18 = pallet2.DefectN8;
            this.DefectN19 = pallet2.DefectN9;
            this.DefectN20 = pallet2.DefectN10;

            this.DefectN21 = pallet3.DefectN1;
            this.DefectN22 = pallet3.DefectN2;
            this.DefectN23 = pallet3.DefectN3;
            this.DefectN24 = pallet3.DefectN4;
            this.DefectN25 = pallet3.DefectN5;
            this.DefectN26 = pallet3.DefectN6;
            this.DefectN27 = pallet3.DefectN7;
            this.DefectN28 = pallet3.DefectN8;
            this.DefectN29 = pallet3.DefectN9;
            this.DefectN30 = pallet3.DefectN10;
        }


        public AQTrayObj(TrayObj tray, PalletObj pallet1, PalletObj pallet2, PalletObj pallet3, PalletObj pallet4)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;
            this.Pallet2 = pallet2.PalletID;
            this.Pallet3 = pallet3.PalletID;
            this.Pallet4 = pallet4.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);

            //this.HGAN11 = pallet2.HGAN1;
            this.HGAN11 = (tray.HGAN11 == "-") ? string.Empty : ((pallet2.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN1);
            //this.HGAN12 = pallet2.HGAN2;
            this.HGAN12 = (tray.HGAN12 == "-") ? string.Empty : ((pallet2.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN2);
            //this.HGAN13 = pallet2.HGAN3;
            this.HGAN13 = (tray.HGAN13 == "-") ? string.Empty : ((pallet2.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN3);
            //this.HGAN14 = pallet2.HGAN4;
            this.HGAN14 = (tray.HGAN14 == "-") ? string.Empty : ((pallet2.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN4);
            //this.HGAN15 = pallet2.HGAN5;
            this.HGAN15 = (tray.HGAN15 == "-") ? string.Empty : ((pallet2.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN5);
            //this.HGAN16 = pallet2.HGAN6;
            this.HGAN16 = (tray.HGAN16 == "-") ? string.Empty : ((pallet2.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN6);
            //this.HGAN17 = pallet2.HGAN7;
            this.HGAN17 = (tray.HGAN17 == "-") ? string.Empty : ((pallet2.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN7);
            //this.HGAN18 = pallet2.HGAN8;
            this.HGAN18 = (tray.HGAN18 == "-") ? string.Empty : ((pallet2.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN8);
            //this.HGAN19 = pallet2.HGAN9;
            this.HGAN19 = (tray.HGAN19 == "-") ? string.Empty : ((pallet2.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN9);
            //this.HGAN20 = pallet2.HGAN10;
            this.HGAN20 = (tray.HGAN20 == "-") ? string.Empty : ((pallet2.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN10);

            //this.HGAN21 = pallet3.HGAN1;
            this.HGAN21 = (tray.HGAN21 == "-") ? string.Empty : ((pallet3.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN1);
            //this.HGAN22 = pallet3.HGAN2;
            this.HGAN22 = (tray.HGAN22 == "-") ? string.Empty : ((pallet3.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN2);
            //this.HGAN23 = pallet3.HGAN3;
            this.HGAN23 = (tray.HGAN23 == "-") ? string.Empty : ((pallet3.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN3);
            //this.HGAN24 = pallet3.HGAN4;
            this.HGAN24 = (tray.HGAN24 == "-") ? string.Empty : ((pallet3.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN4);
            //this.HGAN25 = pallet3.HGAN5;
            this.HGAN25 = (tray.HGAN25 == "-") ? string.Empty : ((pallet3.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN5);
            //this.HGAN26 = pallet3.HGAN6;
            this.HGAN26 = (tray.HGAN26 == "-") ? string.Empty : ((pallet3.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN6);
            //this.HGAN27 = pallet3.HGAN7;
            this.HGAN27 = (tray.HGAN27 == "-") ? string.Empty : ((pallet3.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN7);
            //this.HGAN28 = pallet3.HGAN8;
            this.HGAN28 = (tray.HGAN28 == "-") ? string.Empty : ((pallet3.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN8);
            //this.HGAN29 = pallet3.HGAN9;
            this.HGAN29 = (tray.HGAN29 == "-") ? string.Empty : ((pallet3.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN9);
            //this.HGAN30 = pallet3.HGAN10;
            this.HGAN30 = (tray.HGAN30 == "-") ? string.Empty : ((pallet3.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN10);

            //this.HGAN31 = pallet4.HGAN1;
            this.HGAN31 = (tray.HGAN31 == "-") ? string.Empty : ((pallet4.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN1);
            //this.HGAN32 = pallet4.HGAN2;
            this.HGAN32 = (tray.HGAN32 == "-") ? string.Empty : ((pallet4.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN2);
            //this.HGAN33 = pallet4.HGAN3;
            this.HGAN33 = (tray.HGAN33 == "-") ? string.Empty : ((pallet4.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN3);
            //this.HGAN34 = pallet4.HGAN4;
            this.HGAN34 = (tray.HGAN34 == "-") ? string.Empty : ((pallet4.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN4);
            //this.HGAN35 = pallet4.HGAN5;
            this.HGAN35 = (tray.HGAN35 == "-") ? string.Empty : ((pallet4.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN5);
            //this.HGAN36 = pallet4.HGAN6;
            this.HGAN36 = (tray.HGAN36 == "-") ? string.Empty : ((pallet4.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN6);
            //this.HGAN37 = pallet4.HGAN7;
            this.HGAN37 = (tray.HGAN37 == "-") ? string.Empty : ((pallet4.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN7);
            //this.HGAN38 = pallet4.HGAN8;
            this.HGAN38 = (tray.HGAN38 == "-") ? string.Empty : ((pallet4.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN8);
            //this.HGAN39 = pallet4.HGAN9;
            this.HGAN39 = (tray.HGAN39 == "-") ? string.Empty : ((pallet4.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN9);
            //this.HGAN40 = pallet4.HGAN10;
            this.HGAN40 = (tray.HGAN40 == "-") ? string.Empty : ((pallet4.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN10);


            /*
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);

            this.HGAN11 = ((pallet2.DefectN1.Length > 0) ? "" : pallet2.HGAN1);
            this.HGAN12 = ((pallet2.DefectN2.Length > 0) ? "" : pallet2.HGAN2);
            this.HGAN13 = ((pallet2.DefectN3.Length > 0) ? "" : pallet2.HGAN3);
            this.HGAN14 = ((pallet2.DefectN4.Length > 0) ? "" : pallet2.HGAN4);
            this.HGAN15 = ((pallet2.DefectN5.Length > 0) ? "" : pallet2.HGAN5);
            this.HGAN16 = ((pallet2.DefectN6.Length > 0) ? "" : pallet2.HGAN6);
            this.HGAN17 = ((pallet2.DefectN7.Length > 0) ? "" : pallet2.HGAN7);
            this.HGAN18 = ((pallet2.DefectN8.Length > 0) ? "" : pallet2.HGAN8);
            this.HGAN19 = ((pallet2.DefectN9.Length > 0) ? "" : pallet2.HGAN9);
            this.HGAN20 = ((pallet2.DefectN10.Length > 0) ? "" : pallet2.HGAN10);

            this.HGAN21 = ((pallet3.DefectN1.Length > 0) ? "" : pallet3.HGAN1);
            this.HGAN22 = ((pallet3.DefectN2.Length > 0) ? "" : pallet3.HGAN2);
            this.HGAN23 = ((pallet3.DefectN3.Length > 0) ? "" : pallet3.HGAN3);
            this.HGAN24 = ((pallet3.DefectN4.Length > 0) ? "" : pallet3.HGAN4);
            this.HGAN25 = ((pallet3.DefectN5.Length > 0) ? "" : pallet3.HGAN5);
            this.HGAN26 = ((pallet3.DefectN6.Length > 0) ? "" : pallet3.HGAN6);
            this.HGAN27 = ((pallet3.DefectN7.Length > 0) ? "" : pallet3.HGAN7);
            this.HGAN28 = ((pallet3.DefectN8.Length > 0) ? "" : pallet3.HGAN8);
            this.HGAN29 = ((pallet3.DefectN9.Length > 0) ? "" : pallet3.HGAN9);
            this.HGAN30 = ((pallet3.DefectN10.Length > 0) ? "" : pallet3.HGAN10);

            this.HGAN31 = ((pallet4.DefectN1.Length > 0) ? "" : pallet4.HGAN1);
            this.HGAN32 = ((pallet4.DefectN2.Length > 0) ? "" : pallet4.HGAN2);
            this.HGAN33 = ((pallet4.DefectN3.Length > 0) ? "" : pallet4.HGAN3);
            this.HGAN34 = ((pallet4.DefectN4.Length > 0) ? "" : pallet4.HGAN4);
            this.HGAN35 = ((pallet4.DefectN5.Length > 0) ? "" : pallet4.HGAN5);
            this.HGAN36 = ((pallet4.DefectN6.Length > 0) ? "" : pallet4.HGAN6);
            this.HGAN37 = ((pallet4.DefectN7.Length > 0) ? "" : pallet4.HGAN7);
            this.HGAN38 = ((pallet4.DefectN8.Length > 0) ? "" : pallet4.HGAN8);
            this.HGAN39 = ((pallet4.DefectN9.Length > 0) ? "" : pallet4.HGAN9);
            this.HGAN40 = ((pallet4.DefectN10.Length > 0) ? "" : pallet4.HGAN10);
            */


            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;

            this.DefectN11 = pallet2.DefectN1;
            this.DefectN12 = pallet2.DefectN2;
            this.DefectN13 = pallet2.DefectN3;
            this.DefectN14 = pallet2.DefectN4;
            this.DefectN15 = pallet2.DefectN5;
            this.DefectN16 = pallet2.DefectN6;
            this.DefectN17 = pallet2.DefectN7;
            this.DefectN18 = pallet2.DefectN8;
            this.DefectN19 = pallet2.DefectN9;
            this.DefectN20 = pallet2.DefectN10;

            this.DefectN21 = pallet3.DefectN1;
            this.DefectN22 = pallet3.DefectN2;
            this.DefectN23 = pallet3.DefectN3;
            this.DefectN24 = pallet3.DefectN4;
            this.DefectN25 = pallet3.DefectN5;
            this.DefectN26 = pallet3.DefectN6;
            this.DefectN27 = pallet3.DefectN7;
            this.DefectN28 = pallet3.DefectN8;
            this.DefectN29 = pallet3.DefectN9;
            this.DefectN30 = pallet3.DefectN10;

            this.DefectN31 = pallet4.DefectN1;
            this.DefectN32 = pallet4.DefectN2;
            this.DefectN33 = pallet4.DefectN3;
            this.DefectN34 = pallet4.DefectN4;
            this.DefectN35 = pallet4.DefectN5;
            this.DefectN36 = pallet4.DefectN6;
            this.DefectN37 = pallet4.DefectN7;
            this.DefectN38 = pallet4.DefectN8;
            this.DefectN39 = pallet4.DefectN9;
            this.DefectN40 = pallet4.DefectN10;
        }


        public AQTrayObj(TrayObj tray, PalletObj pallet1, PalletObj pallet2, PalletObj pallet3, PalletObj pallet4, PalletObj pallet5)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;
            this.Pallet2 = pallet2.PalletID;
            this.Pallet3 = pallet3.PalletID;
            this.Pallet4 = pallet4.PalletID;
            this.Pallet5 = pallet5.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);

            //this.HGAN11 = pallet2.HGAN1;
            this.HGAN11 = (tray.HGAN11 == "-") ? string.Empty : ((pallet2.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN1);
            //this.HGAN12 = pallet2.HGAN2;
            this.HGAN12 = (tray.HGAN12 == "-") ? string.Empty : ((pallet2.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN2);
            //this.HGAN13 = pallet2.HGAN3;
            this.HGAN13 = (tray.HGAN13 == "-") ? string.Empty : ((pallet2.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN3);
            //this.HGAN14 = pallet2.HGAN4;
            this.HGAN14 = (tray.HGAN14 == "-") ? string.Empty : ((pallet2.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN4);
            //this.HGAN15 = pallet2.HGAN5;
            this.HGAN15 = (tray.HGAN15 == "-") ? string.Empty : ((pallet2.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN5);
            //this.HGAN16 = pallet2.HGAN6;
            this.HGAN16 = (tray.HGAN16 == "-") ? string.Empty : ((pallet2.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN6);
            //this.HGAN17 = pallet2.HGAN7;
            this.HGAN17 = (tray.HGAN17 == "-") ? string.Empty : ((pallet2.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN7);
            //this.HGAN18 = pallet2.HGAN8;
            this.HGAN18 = (tray.HGAN18 == "-") ? string.Empty : ((pallet2.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN8);
            //this.HGAN19 = pallet2.HGAN9;
            this.HGAN19 = (tray.HGAN19 == "-") ? string.Empty : ((pallet2.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN9);
            //this.HGAN20 = pallet2.HGAN10;
            this.HGAN20 = (tray.HGAN20 == "-") ? string.Empty : ((pallet2.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN10);

            //this.HGAN21 = pallet3.HGAN1;
            this.HGAN21 = (tray.HGAN21 == "-") ? string.Empty : ((pallet3.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN1);
            //this.HGAN22 = pallet3.HGAN2;
            this.HGAN22 = (tray.HGAN22 == "-") ? string.Empty : ((pallet3.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN2);
            //this.HGAN23 = pallet3.HGAN3;
            this.HGAN23 = (tray.HGAN23 == "-") ? string.Empty : ((pallet3.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN3);
            //this.HGAN24 = pallet3.HGAN4;
            this.HGAN24 = (tray.HGAN24 == "-") ? string.Empty : ((pallet3.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN4);
            //this.HGAN25 = pallet3.HGAN5;
            this.HGAN25 = (tray.HGAN25 == "-") ? string.Empty : ((pallet3.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN5);
            //this.HGAN26 = pallet3.HGAN6;
            this.HGAN26 = (tray.HGAN26 == "-") ? string.Empty : ((pallet3.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN6);
            //this.HGAN27 = pallet3.HGAN7;
            this.HGAN27 = (tray.HGAN27 == "-") ? string.Empty : ((pallet3.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN7);
            //this.HGAN28 = pallet3.HGAN8;
            this.HGAN28 = (tray.HGAN28 == "-") ? string.Empty : ((pallet3.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN8);
            //this.HGAN29 = pallet3.HGAN9;
            this.HGAN29 = (tray.HGAN29 == "-") ? string.Empty : ((pallet3.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN9);
            //this.HGAN30 = pallet3.HGAN10;
            this.HGAN30 = (tray.HGAN30 == "-") ? string.Empty : ((pallet3.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN10);

            //this.HGAN31 = pallet4.HGAN1;
            this.HGAN31 = (tray.HGAN31 == "-") ? string.Empty : ((pallet4.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN1);
            //this.HGAN32 = pallet4.HGAN2;
            this.HGAN32 = (tray.HGAN32 == "-") ? string.Empty : ((pallet4.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN2);
            //this.HGAN33 = pallet4.HGAN3;
            this.HGAN33 = (tray.HGAN33 == "-") ? string.Empty : ((pallet4.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN3);
            //this.HGAN34 = pallet4.HGAN4;
            this.HGAN34 = (tray.HGAN34 == "-") ? string.Empty : ((pallet4.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN4);
            //this.HGAN35 = pallet4.HGAN5;
            this.HGAN35 = (tray.HGAN35 == "-") ? string.Empty : ((pallet4.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN5);
            //this.HGAN36 = pallet4.HGAN6;
            this.HGAN36 = (tray.HGAN36 == "-") ? string.Empty : ((pallet4.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN6);
            //this.HGAN37 = pallet4.HGAN7;
            this.HGAN37 = (tray.HGAN37 == "-") ? string.Empty : ((pallet4.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN7);
            //this.HGAN38 = pallet4.HGAN8;
            this.HGAN38 = (tray.HGAN38 == "-") ? string.Empty : ((pallet4.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN8);
            //this.HGAN39 = pallet4.HGAN9;
            this.HGAN39 = (tray.HGAN39 == "-") ? string.Empty : ((pallet4.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN9);
            //this.HGAN40 = pallet4.HGAN10;
            this.HGAN40 = (tray.HGAN40 == "-") ? string.Empty : ((pallet4.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN10);

            //this.HGAN41 = pallet5.HGAN1;
            this.HGAN41 = (tray.HGAN41 == "-") ? string.Empty : ((pallet5.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN1);
            //this.HGAN42 = pallet5.HGAN2;
            this.HGAN42 = (tray.HGAN42 == "-") ? string.Empty : ((pallet5.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN2);
            //this.HGAN43 = pallet5.HGAN3;
            this.HGAN43 = (tray.HGAN43 == "-") ? string.Empty : ((pallet5.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN3);
            //this.HGAN44 = pallet5.HGAN4;
            this.HGAN44 = (tray.HGAN44 == "-") ? string.Empty : ((pallet5.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN4);
            //this.HGAN45 = pallet5.HGAN5;
            this.HGAN45 = (tray.HGAN45 == "-") ? string.Empty : ((pallet5.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN5);
            //this.HGAN46 = pallet5.HGAN6;
            this.HGAN46 = (tray.HGAN46 == "-") ? string.Empty : ((pallet5.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN6);
            //this.HGAN47 = pallet5.HGAN7;
            this.HGAN47 = (tray.HGAN47 == "-") ? string.Empty : ((pallet5.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN7);
            //this.HGAN48 = pallet5.HGAN8;
            this.HGAN48 = (tray.HGAN48 == "-") ? string.Empty : ((pallet5.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN8);
            //this.HGAN49 = pallet5.HGAN9;
            this.HGAN49 = (tray.HGAN49 == "-") ? string.Empty : ((pallet5.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN9);
            //this.HGAN50 = pallet5.HGAN10;
            this.HGAN50 = (tray.HGAN50 == "-") ? string.Empty : ((pallet5.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN10);

            /*
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);

            this.HGAN11 = ((pallet2.DefectN1.Length > 0) ? "" : pallet2.HGAN1);
            this.HGAN12 = ((pallet2.DefectN2.Length > 0) ? "" : pallet2.HGAN2);
            this.HGAN13 = ((pallet2.DefectN3.Length > 0) ? "" : pallet2.HGAN3);
            this.HGAN14 = ((pallet2.DefectN4.Length > 0) ? "" : pallet2.HGAN4);
            this.HGAN15 = ((pallet2.DefectN5.Length > 0) ? "" : pallet2.HGAN5);
            this.HGAN16 = ((pallet2.DefectN6.Length > 0) ? "" : pallet2.HGAN6);
            this.HGAN17 = ((pallet2.DefectN7.Length > 0) ? "" : pallet2.HGAN7);
            this.HGAN18 = ((pallet2.DefectN8.Length > 0) ? "" : pallet2.HGAN8);
            this.HGAN19 = ((pallet2.DefectN9.Length > 0) ? "" : pallet2.HGAN9);
            this.HGAN20 = ((pallet2.DefectN10.Length > 0) ? "" : pallet2.HGAN10);

            this.HGAN21 = ((pallet3.DefectN1.Length > 0) ? "" : pallet3.HGAN1);
            this.HGAN22 = ((pallet3.DefectN2.Length > 0) ? "" : pallet3.HGAN2);
            this.HGAN23 = ((pallet3.DefectN3.Length > 0) ? "" : pallet3.HGAN3);
            this.HGAN24 = ((pallet3.DefectN4.Length > 0) ? "" : pallet3.HGAN4);
            this.HGAN25 = ((pallet3.DefectN5.Length > 0) ? "" : pallet3.HGAN5);
            this.HGAN26 = ((pallet3.DefectN6.Length > 0) ? "" : pallet3.HGAN6);
            this.HGAN27 = ((pallet3.DefectN7.Length > 0) ? "" : pallet3.HGAN7);
            this.HGAN28 = ((pallet3.DefectN8.Length > 0) ? "" : pallet3.HGAN8);
            this.HGAN29 = ((pallet3.DefectN9.Length > 0) ? "" : pallet3.HGAN9);
            this.HGAN30 = ((pallet3.DefectN10.Length > 0) ? "" : pallet3.HGAN10);

            this.HGAN31 = ((pallet4.DefectN1.Length > 0) ? "" : pallet4.HGAN1);
            this.HGAN32 = ((pallet4.DefectN2.Length > 0) ? "" : pallet4.HGAN2);
            this.HGAN33 = ((pallet4.DefectN3.Length > 0) ? "" : pallet4.HGAN3);
            this.HGAN34 = ((pallet4.DefectN4.Length > 0) ? "" : pallet4.HGAN4);
            this.HGAN35 = ((pallet4.DefectN5.Length > 0) ? "" : pallet4.HGAN5);
            this.HGAN36 = ((pallet4.DefectN6.Length > 0) ? "" : pallet4.HGAN6);
            this.HGAN37 = ((pallet4.DefectN7.Length > 0) ? "" : pallet4.HGAN7);
            this.HGAN38 = ((pallet4.DefectN8.Length > 0) ? "" : pallet4.HGAN8);
            this.HGAN39 = ((pallet4.DefectN9.Length > 0) ? "" : pallet4.HGAN9);
            this.HGAN40 = ((pallet4.DefectN10.Length > 0) ? "" : pallet4.HGAN10);

            this.HGAN41 = ((pallet5.DefectN1.Length > 0) ? "" : pallet5.HGAN1);
            this.HGAN42 = ((pallet5.DefectN2.Length > 0) ? "" : pallet5.HGAN2);
            this.HGAN43 = ((pallet5.DefectN3.Length > 0) ? "" : pallet5.HGAN3);
            this.HGAN44 = ((pallet5.DefectN4.Length > 0) ? "" : pallet5.HGAN4);
            this.HGAN45 = ((pallet5.DefectN5.Length > 0) ? "" : pallet5.HGAN5);
            this.HGAN46 = ((pallet5.DefectN6.Length > 0) ? "" : pallet5.HGAN6);
            this.HGAN47 = ((pallet5.DefectN7.Length > 0) ? "" : pallet5.HGAN7);
            this.HGAN48 = ((pallet5.DefectN8.Length > 0) ? "" : pallet5.HGAN8);
            this.HGAN49 = ((pallet5.DefectN9.Length > 0) ? "" : pallet5.HGAN9);
            this.HGAN50 = ((pallet5.DefectN10.Length > 0) ? "" : pallet5.HGAN10);
            */

            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;

            this.DefectN11 = pallet2.DefectN1;
            this.DefectN12 = pallet2.DefectN2;
            this.DefectN13 = pallet2.DefectN3;
            this.DefectN14 = pallet2.DefectN4;
            this.DefectN15 = pallet2.DefectN5;
            this.DefectN16 = pallet2.DefectN6;
            this.DefectN17 = pallet2.DefectN7;
            this.DefectN18 = pallet2.DefectN8;
            this.DefectN19 = pallet2.DefectN9;
            this.DefectN20 = pallet2.DefectN10;

            this.DefectN21 = pallet3.DefectN1;
            this.DefectN22 = pallet3.DefectN2;
            this.DefectN23 = pallet3.DefectN3;
            this.DefectN24 = pallet3.DefectN4;
            this.DefectN25 = pallet3.DefectN5;
            this.DefectN26 = pallet3.DefectN6;
            this.DefectN27 = pallet3.DefectN7;
            this.DefectN28 = pallet3.DefectN8;
            this.DefectN29 = pallet3.DefectN9;
            this.DefectN30 = pallet3.DefectN10;

            this.DefectN31 = pallet4.DefectN1;
            this.DefectN32 = pallet4.DefectN2;
            this.DefectN33 = pallet4.DefectN3;
            this.DefectN34 = pallet4.DefectN4;
            this.DefectN35 = pallet4.DefectN5;
            this.DefectN36 = pallet4.DefectN6;
            this.DefectN37 = pallet4.DefectN7;
            this.DefectN38 = pallet4.DefectN8;
            this.DefectN39 = pallet4.DefectN9;
            this.DefectN40 = pallet4.DefectN10;

            this.DefectN41 = pallet5.DefectN1;
            this.DefectN42 = pallet5.DefectN2;
            this.DefectN43 = pallet5.DefectN3;
            this.DefectN44 = pallet5.DefectN4;
            this.DefectN45 = pallet5.DefectN5;
            this.DefectN46 = pallet5.DefectN6;
            this.DefectN47 = pallet5.DefectN7;
            this.DefectN48 = pallet5.DefectN8;
            this.DefectN49 = pallet5.DefectN9;
            this.DefectN50 = pallet5.DefectN10;
        }


        public AQTrayObj(TrayObj tray, PalletObj pallet1, PalletObj pallet2, PalletObj pallet3, PalletObj pallet4, PalletObj pallet5, PalletObj pallet6)
        {
            this.Date = tray.Date;
            this.TimeStart = tray.TimeStart;
            this.TimeEnd = tray.TimeEnd;

            this.TesterNumber = pallet1.TesterNumber;
            this.Customer = pallet1.Customer;
            this.Product = pallet1.Product;

            this.TrayID = tray.TrayID;
            this.LotNumber = pallet1.LotNumber;
            this.DocControl1 = pallet1.DocControl1;
            this.DocControl2 = pallet1.DocControl2;
            this.Sus = pallet1.Sus;
            this.AssyLine = pallet1.AssyLine;

            this.Pallet1 = pallet1.PalletID;
            this.Pallet2 = pallet2.PalletID;
            this.Pallet3 = pallet3.PalletID;
            this.Pallet4 = pallet4.PalletID;
            this.Pallet5 = pallet5.PalletID;
            this.Pallet6 = pallet6.PalletID;

            //this.HGAN1 = pallet1.HGAN1;
            this.HGAN1 = (tray.HGAN1 == "-") ? string.Empty : ((pallet1.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN1);
            //this.HGAN2 = pallet1.HGAN2;
            this.HGAN2 = (tray.HGAN2 == "-") ? string.Empty : ((pallet1.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN2);
            //this.HGAN3 = pallet1.HGAN3;
            this.HGAN3 = (tray.HGAN3 == "-") ? string.Empty : ((pallet1.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN3);
            //this.HGAN4 = pallet1.HGAN4;
            this.HGAN4 = (tray.HGAN4 == "-") ? string.Empty : ((pallet1.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN4);
            //this.HGAN5 = pallet1.HGAN5;
            this.HGAN5 = (tray.HGAN5 == "-") ? string.Empty : ((pallet1.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN5);
            //this.HGAN6 = pallet1.HGAN6;
            this.HGAN6 = (tray.HGAN6 == "-") ? string.Empty : ((pallet1.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN6);
            //this.HGAN7 = pallet1.HGAN7;
            this.HGAN7 = (tray.HGAN7 == "-") ? string.Empty : ((pallet1.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN7);
            //this.HGAN8 = pallet1.HGAN8;
            this.HGAN8 = (tray.HGAN8 == "-") ? string.Empty : ((pallet1.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN8);
            //this.HGAN9 = pallet1.HGAN9;
            this.HGAN9 = (tray.HGAN9 == "-") ? string.Empty : ((pallet1.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN9);
            //this.HGAN10 = pallet1.HGAN10;
            this.HGAN10 = (tray.HGAN10 == "-") ? string.Empty : ((pallet1.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet1.HGAN10);

            //this.HGAN11 = pallet2.HGAN1;
            this.HGAN11 = (tray.HGAN11 == "-") ? string.Empty : ((pallet2.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN1);
            //this.HGAN12 = pallet2.HGAN2;
            this.HGAN12 = (tray.HGAN12 == "-") ? string.Empty : ((pallet2.HGAN2.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN2);
            //this.HGAN13 = pallet2.HGAN3;
            this.HGAN13 = (tray.HGAN13 == "-") ? string.Empty : ((pallet2.HGAN3.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN3);
            //this.HGAN14 = pallet2.HGAN4;
            this.HGAN14 = (tray.HGAN14 == "-") ? string.Empty : ((pallet2.HGAN4.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN4);
            //this.HGAN15 = pallet2.HGAN5;
            this.HGAN15 = (tray.HGAN15 == "-") ? string.Empty : ((pallet2.HGAN5.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN5);
            //this.HGAN16 = pallet2.HGAN6;
            this.HGAN16 = (tray.HGAN16 == "-") ? string.Empty : ((pallet2.HGAN6.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN6);
            //this.HGAN17 = pallet2.HGAN7;
            this.HGAN17 = (tray.HGAN17 == "-") ? string.Empty : ((pallet2.HGAN7.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN7);
            //this.HGAN18 = pallet2.HGAN8;
            this.HGAN18 = (tray.HGAN18 == "-") ? string.Empty : ((pallet2.HGAN8.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN8);
            //this.HGAN19 = pallet2.HGAN9;
            this.HGAN19 = (tray.HGAN19 == "-") ? string.Empty : ((pallet2.HGAN9.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN9);
            //this.HGAN20 = pallet2.HGAN10;
            this.HGAN20 = (tray.HGAN20 == "-") ? string.Empty : ((pallet2.HGAN10.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet2.HGAN10);

            //this.HGAN21 = pallet3.HGAN1;
            this.HGAN21 = (tray.HGAN21 == "-") ? string.Empty : ((pallet3.HGAN1.Length < 1) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN1);
            //this.HGAN22 = pallet3.HGAN2;
            this.HGAN22 = (tray.HGAN22 == "-") ? string.Empty : ((pallet3.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN2);
            //this.HGAN23 = pallet3.HGAN3;
            this.HGAN23 = (tray.HGAN23 == "-") ? string.Empty : ((pallet3.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN3);
            //this.HGAN24 = pallet3.HGAN4;
            this.HGAN24 = (tray.HGAN24 == "-") ? string.Empty : ((pallet3.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN4);
            //this.HGAN25 = pallet3.HGAN5;
            this.HGAN25 = (tray.HGAN25 == "-") ? string.Empty : ((pallet3.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN5);
            //this.HGAN26 = pallet3.HGAN6;
            this.HGAN26 = (tray.HGAN26 == "-") ? string.Empty : ((pallet3.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN6);
            //this.HGAN27 = pallet3.HGAN7;
            this.HGAN27 = (tray.HGAN27 == "-") ? string.Empty : ((pallet3.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN7);
            //this.HGAN28 = pallet3.HGAN8;
            this.HGAN28 = (tray.HGAN28 == "-") ? string.Empty : ((pallet3.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN8);
            //this.HGAN29 = pallet3.HGAN9;
            this.HGAN29 = (tray.HGAN29 == "-") ? string.Empty : ((pallet3.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN9);
            //this.HGAN30 = pallet3.HGAN10;
            this.HGAN30 = (tray.HGAN30 == "-") ? string.Empty : ((pallet3.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet3.HGAN10);

            //this.HGAN31 = pallet4.HGAN1;
            this.HGAN31 = (tray.HGAN31 == "-") ? string.Empty : ((pallet4.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN1);
            //this.HGAN32 = pallet4.HGAN2;
            this.HGAN32 = (tray.HGAN32 == "-") ? string.Empty : ((pallet4.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN2);
            //this.HGAN33 = pallet4.HGAN3;
            this.HGAN33 = (tray.HGAN33 == "-") ? string.Empty : ((pallet4.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN3);
            //this.HGAN34 = pallet4.HGAN4;
            this.HGAN34 = (tray.HGAN34 == "-") ? string.Empty : ((pallet4.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN4);
            //this.HGAN35 = pallet4.HGAN5;
            this.HGAN35 = (tray.HGAN35 == "-") ? string.Empty : ((pallet4.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN5);
            //this.HGAN36 = pallet4.HGAN6;
            this.HGAN36 = (tray.HGAN36 == "-") ? string.Empty : ((pallet4.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN6);
            //this.HGAN37 = pallet4.HGAN7;
            this.HGAN37 = (tray.HGAN37 == "-") ? string.Empty : ((pallet4.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN7);
            //this.HGAN38 = pallet4.HGAN8;
            this.HGAN38 = (tray.HGAN38 == "-") ? string.Empty : ((pallet4.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN8);
            //this.HGAN39 = pallet4.HGAN9;
            this.HGAN39 = (tray.HGAN39 == "-") ? string.Empty : ((pallet4.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN9);
            //this.HGAN40 = pallet4.HGAN10;
            this.HGAN40 = (tray.HGAN40 == "-") ? string.Empty : ((pallet4.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet4.HGAN10);

            //this.HGAN41 = pallet5.HGAN1;
            this.HGAN41 = (tray.HGAN41 == "-") ? string.Empty : ((pallet5.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN1);
            //this.HGAN42 = pallet5.HGAN2;
            this.HGAN42 = (tray.HGAN42 == "-") ? string.Empty : ((pallet5.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN2);
            //this.HGAN43 = pallet5.HGAN3;
            this.HGAN43 = (tray.HGAN43 == "-") ? string.Empty : ((pallet5.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN3);
            //this.HGAN44 = pallet5.HGAN4;
            this.HGAN44 = (tray.HGAN44 == "-") ? string.Empty : ((pallet5.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN4);
            //this.HGAN45 = pallet5.HGAN5;
            this.HGAN45 = (tray.HGAN45 == "-") ? string.Empty : ((pallet5.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN5);
            //this.HGAN46 = pallet5.HGAN6;
            this.HGAN46 = (tray.HGAN46 == "-") ? string.Empty : ((pallet5.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN6);
            //this.HGAN47 = pallet5.HGAN7;
            this.HGAN47 = (tray.HGAN47 == "-") ? string.Empty : ((pallet5.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN7);
            //this.HGAN48 = pallet5.HGAN8;
            this.HGAN48 = (tray.HGAN48 == "-") ? string.Empty : ((pallet5.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN8);
            //this.HGAN49 = pallet5.HGAN9;
            this.HGAN49 = (tray.HGAN49 == "-") ? string.Empty : ((pallet5.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN9);
            //this.HGAN50 = pallet5.HGAN10;
            this.HGAN50 = (tray.HGAN50 == "-") ? string.Empty : ((pallet5.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet5.HGAN10);

            //this.HGAN51 = pallet6.HGAN1;
            this.HGAN51 = (tray.HGAN51 == "-") ? string.Empty : ((pallet6.HGAN1.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN1);
            //this.HGAN52 = pallet6.HGAN2;
            this.HGAN52 = (tray.HGAN52 == "-") ? string.Empty : ((pallet6.HGAN2.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN2);
            //this.HGAN53 = pallet6.HGAN3;
            this.HGAN53 = (tray.HGAN53 == "-") ? string.Empty : ((pallet6.HGAN3.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN3);
            //this.HGAN54 = pallet6.HGAN4;
            this.HGAN54 = (tray.HGAN54 == "-") ? string.Empty : ((pallet6.HGAN4.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN4);
            //this.HGAN55 = pallet6.HGAN5;
            this.HGAN55 = (tray.HGAN55 == "-") ? string.Empty : ((pallet6.HGAN5.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN5);
            //this.HGAN56 = pallet6.HGAN6;
            this.HGAN56 = (tray.HGAN56 == "-") ? string.Empty : ((pallet6.HGAN6.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN6);
            //this.HGAN57 = pallet6.HGAN7;
            this.HGAN57 = (tray.HGAN57 == "-") ? string.Empty : ((pallet6.HGAN7.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN7);
            //this.HGAN58 = pallet6.HGAN8;
            this.HGAN58 = (tray.HGAN58 == "-") ? string.Empty : ((pallet6.HGAN8.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN8);
            //this.HGAN59 = pallet6.HGAN9;
            this.HGAN59 = (tray.HGAN59 == "-") ? string.Empty : ((pallet6.HGAN9.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN9);
            //this.HGAN60 = pallet6.HGAN10;
            this.HGAN60 = (tray.HGAN60 == "-") ? string.Empty : ((pallet6.HGAN10.Length < 2) ? CONST_CANNOT_READ_10OCR : pallet6.HGAN10);

            /* temporarily disabled
            this.HGAN1 = ((pallet1.DefectN1.Length > 0) ? "" : pallet1.HGAN1);
            this.HGAN2 = ((pallet1.DefectN2.Length > 0) ? "" : pallet1.HGAN2);
            this.HGAN3 = ((pallet1.DefectN3.Length > 0) ? "" : pallet1.HGAN3);
            this.HGAN4 = ((pallet1.DefectN4.Length > 0) ? "" : pallet1.HGAN4);
            this.HGAN5 = ((pallet1.DefectN5.Length > 0) ? "" : pallet1.HGAN5);
            this.HGAN6 = ((pallet1.DefectN6.Length > 0) ? "" : pallet1.HGAN6);
            this.HGAN7 = ((pallet1.DefectN7.Length > 0) ? "" : pallet1.HGAN7);
            this.HGAN8 = ((pallet1.DefectN8.Length > 0) ? "" : pallet1.HGAN8);
            this.HGAN9 = ((pallet1.DefectN9.Length > 0) ? "" : pallet1.HGAN9);
            this.HGAN10 = ((pallet1.DefectN10.Length > 0) ? "" : pallet1.HGAN10);

            this.HGAN11 = ((pallet2.DefectN1.Length > 0) ? "" : pallet2.HGAN1);
            this.HGAN12 = ((pallet2.DefectN2.Length > 0) ? "" : pallet2.HGAN2);
            this.HGAN13 = ((pallet2.DefectN3.Length > 0) ? "" : pallet2.HGAN3);
            this.HGAN14 = ((pallet2.DefectN4.Length > 0) ? "" : pallet2.HGAN4);
            this.HGAN15 = ((pallet2.DefectN5.Length > 0) ? "" : pallet2.HGAN5);
            this.HGAN16 = ((pallet2.DefectN6.Length > 0) ? "" : pallet2.HGAN6);
            this.HGAN17 = ((pallet2.DefectN7.Length > 0) ? "" : pallet2.HGAN7);
            this.HGAN18 = ((pallet2.DefectN8.Length > 0) ? "" : pallet2.HGAN8);
            this.HGAN19 = ((pallet2.DefectN9.Length > 0) ? "" : pallet2.HGAN9);
            this.HGAN20 = ((pallet2.DefectN10.Length > 0) ? "" : pallet2.HGAN10);

            this.HGAN21 = ((pallet3.DefectN1.Length > 0) ? "" : pallet3.HGAN1);
            this.HGAN22 = ((pallet3.DefectN2.Length > 0) ? "" : pallet3.HGAN2);
            this.HGAN23 = ((pallet3.DefectN3.Length > 0) ? "" : pallet3.HGAN3);
            this.HGAN24 = ((pallet3.DefectN4.Length > 0) ? "" : pallet3.HGAN4);
            this.HGAN25 = ((pallet3.DefectN5.Length > 0) ? "" : pallet3.HGAN5);
            this.HGAN26 = ((pallet3.DefectN6.Length > 0) ? "" : pallet3.HGAN6);
            this.HGAN27 = ((pallet3.DefectN7.Length > 0) ? "" : pallet3.HGAN7);
            this.HGAN28 = ((pallet3.DefectN8.Length > 0) ? "" : pallet3.HGAN8);
            this.HGAN29 = ((pallet3.DefectN9.Length > 0) ? "" : pallet3.HGAN9);
            this.HGAN30 = ((pallet3.DefectN10.Length > 0) ? "" : pallet3.HGAN10);

            this.HGAN31 = ((pallet4.DefectN1.Length > 0) ? "" : pallet4.HGAN1);
            this.HGAN32 = ((pallet4.DefectN2.Length > 0) ? "" : pallet4.HGAN2);
            this.HGAN33 = ((pallet4.DefectN3.Length > 0) ? "" : pallet4.HGAN3);
            this.HGAN34 = ((pallet4.DefectN4.Length > 0) ? "" : pallet4.HGAN4);
            this.HGAN35 = ((pallet4.DefectN5.Length > 0) ? "" : pallet4.HGAN5);
            this.HGAN36 = ((pallet4.DefectN6.Length > 0) ? "" : pallet4.HGAN6);
            this.HGAN37 = ((pallet4.DefectN7.Length > 0) ? "" : pallet4.HGAN7);
            this.HGAN38 = ((pallet4.DefectN8.Length > 0) ? "" : pallet4.HGAN8);
            this.HGAN39 = ((pallet4.DefectN9.Length > 0) ? "" : pallet4.HGAN9);
            this.HGAN40 = ((pallet4.DefectN10.Length > 0) ? "" : pallet4.HGAN10);

            this.HGAN41 = ((pallet5.DefectN1.Length > 0) ? "" : pallet5.HGAN1);
            this.HGAN42 = ((pallet5.DefectN2.Length > 0) ? "" : pallet5.HGAN2);
            this.HGAN43 = ((pallet5.DefectN3.Length > 0) ? "" : pallet5.HGAN3);
            this.HGAN44 = ((pallet5.DefectN4.Length > 0) ? "" : pallet5.HGAN4);
            this.HGAN45 = ((pallet5.DefectN5.Length > 0) ? "" : pallet5.HGAN5);
            this.HGAN46 = ((pallet5.DefectN6.Length > 0) ? "" : pallet5.HGAN6);
            this.HGAN47 = ((pallet5.DefectN7.Length > 0) ? "" : pallet5.HGAN7);
            this.HGAN48 = ((pallet5.DefectN8.Length > 0) ? "" : pallet5.HGAN8);
            this.HGAN49 = ((pallet5.DefectN9.Length > 0) ? "" : pallet5.HGAN9);
            this.HGAN50 = ((pallet5.DefectN10.Length > 0) ? "" : pallet5.HGAN10);

            this.HGAN51 = ((pallet6.DefectN1.Length > 0) ? "" : pallet6.HGAN1);
            this.HGAN52 = ((pallet6.DefectN2.Length > 0) ? "" : pallet6.HGAN2);
            this.HGAN53 = ((pallet6.DefectN3.Length > 0) ? "" : pallet6.HGAN3);
            this.HGAN54 = ((pallet6.DefectN4.Length > 0) ? "" : pallet6.HGAN4);
            this.HGAN55 = ((pallet6.DefectN5.Length > 0) ? "" : pallet6.HGAN5);
            this.HGAN56 = ((pallet6.DefectN6.Length > 0) ? "" : pallet6.HGAN6);
            this.HGAN57 = ((pallet6.DefectN7.Length > 0) ? "" : pallet6.HGAN7);
            this.HGAN58 = ((pallet6.DefectN8.Length > 0) ? "" : pallet6.HGAN8);
            this.HGAN59 = ((pallet6.DefectN9.Length > 0) ? "" : pallet6.HGAN9);
            this.HGAN60 = ((pallet6.DefectN10.Length > 0) ? "" : pallet6.HGAN10);
            */


            this.DefectN1 = pallet1.DefectN1;
            this.DefectN2 = pallet1.DefectN2;
            this.DefectN3 = pallet1.DefectN3;
            this.DefectN4 = pallet1.DefectN4;
            this.DefectN5 = pallet1.DefectN5;
            this.DefectN6 = pallet1.DefectN6;
            this.DefectN7 = pallet1.DefectN7;
            this.DefectN8 = pallet1.DefectN8;
            this.DefectN9 = pallet1.DefectN9;
            this.DefectN10 = pallet1.DefectN10;

            this.DefectN11 = pallet2.DefectN1;
            this.DefectN12 = pallet2.DefectN2;
            this.DefectN13 = pallet2.DefectN3;
            this.DefectN14 = pallet2.DefectN4;
            this.DefectN15 = pallet2.DefectN5;
            this.DefectN16 = pallet2.DefectN6;
            this.DefectN17 = pallet2.DefectN7;
            this.DefectN18 = pallet2.DefectN8;
            this.DefectN19 = pallet2.DefectN9;
            this.DefectN20 = pallet2.DefectN10;

            this.DefectN21 = pallet3.DefectN1;
            this.DefectN22 = pallet3.DefectN2;
            this.DefectN23 = pallet3.DefectN3;
            this.DefectN24 = pallet3.DefectN4;
            this.DefectN25 = pallet3.DefectN5;
            this.DefectN26 = pallet3.DefectN6;
            this.DefectN27 = pallet3.DefectN7;
            this.DefectN28 = pallet3.DefectN8;
            this.DefectN29 = pallet3.DefectN9;
            this.DefectN30 = pallet3.DefectN10;

            this.DefectN31 = pallet4.DefectN1;
            this.DefectN32 = pallet4.DefectN2;
            this.DefectN33 = pallet4.DefectN3;
            this.DefectN34 = pallet4.DefectN4;
            this.DefectN35 = pallet4.DefectN5;
            this.DefectN36 = pallet4.DefectN6;
            this.DefectN37 = pallet4.DefectN7;
            this.DefectN38 = pallet4.DefectN8;
            this.DefectN39 = pallet4.DefectN9;
            this.DefectN40 = pallet4.DefectN10;

            this.DefectN41 = pallet5.DefectN1;
            this.DefectN42 = pallet5.DefectN2;
            this.DefectN43 = pallet5.DefectN3;
            this.DefectN44 = pallet5.DefectN4;
            this.DefectN45 = pallet5.DefectN5;
            this.DefectN46 = pallet5.DefectN6;
            this.DefectN47 = pallet5.DefectN7;
            this.DefectN48 = pallet5.DefectN8;
            this.DefectN49 = pallet5.DefectN9;
            this.DefectN50 = pallet5.DefectN10;

            this.DefectN51 = pallet6.DefectN1;
            this.DefectN52 = pallet6.DefectN2;
            this.DefectN53 = pallet6.DefectN3;
            this.DefectN54 = pallet6.DefectN4;
            this.DefectN55 = pallet6.DefectN5;
            this.DefectN56 = pallet6.DefectN6;
            this.DefectN57 = pallet6.DefectN7;
            this.DefectN58 = pallet6.DefectN8;
            this.DefectN59 = pallet6.DefectN9;
            this.DefectN60 = pallet6.DefectN10;
        }



        #endregion



        private DateTime _dtDate = new DateTime();
        public string Date
        {
            get { return _dtDate.ToString("M/d/yyyy"); }
            //set { _dtDate = DateTime.ParseExact(value, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { _dtDate = DateTime.ParseExact(value, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
        }

        private DateTime _dtTimeStart = new DateTime();
        public string TimeStart
        {
            get { return _dtTimeStart.ToString(CONST_TIMESTARTEND_FORMAT); }
            set
            {
                try
                {
                    _dtTimeStart = DateTime.Parse(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void SetTimeStart(DateTime dtStart)
        {
            _dtTimeStart = dtStart;
        }


        private DateTime _dtTimeEnd = new DateTime();
        public string TimeEnd
        {
            get { return _dtTimeEnd.ToString(CONST_TIMESTARTEND_FORMAT); }
            set
            {
                try
                {
                    _dtTimeEnd = DateTime.Parse(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void SetTimeEnd(DateTime dtEnd)
        {
            _dtTimeEnd = dtEnd;
        }


        private string _strUsedTime = string.Empty;
        public string UsedTime
        {
            get
            {
                TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
                _strUsedTime = duration.ToString(@"mm\:ss");

                return _strUsedTime;
            }

            set
            {
                TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
                _strUsedTime = duration.ToString(@"mm\:ss");
            }
        }


        private string _strTesterNumber = string.Empty;
        public string TesterNumber
        {
            get { return _strTesterNumber; }
            set { _strTesterNumber = value; }
        }

        private string _strCustomer = string.Empty;
        public string Customer
        {
            get { return _strCustomer; }
            set { _strCustomer = value; }
        }

        private string _strProduct = string.Empty;
        public string Product
        {
            get { return _strProduct; }
            set { _strProduct = value; }
        }

        private string _strUser = string.Empty;
        public string User
        {
            get { return _strUser; }
            set { _strUser = value; }
        }


        private string _strTrayID = string.Empty;
        public string TrayID
        {
            get { return _strTrayID; }
            set { _strTrayID = value; }
        }


        private string _strLotNumber = string.Empty;
        public string LotNumber
        {
            get { return _strLotNumber; }
            set { _strLotNumber = value; }
        }

        private string _strDocCtrl1 = string.Empty;
        public string DocControl1
        {
            get { return _strDocCtrl1; }
            set { _strDocCtrl1 = value; }
        }

        private string _strDocCtrl2 = string.Empty;
        public string DocControl2
        {
            get { return _strDocCtrl2; }
            set { _strDocCtrl2 = value; }
        }

        private string _strSus = string.Empty;
        public string Sus
        {
            get { return _strSus; }
            set { _strSus = value; }
        }

        private string _strAssyLine = string.Empty;
        public string AssyLine
        {
            get { return _strAssyLine; }
            set { _strAssyLine = value; }
        }


        private string _strPallet1 = string.Empty;
        public string Pallet1
        {
            get { return _strPallet1; }
            set { _strPallet1 = value; }
        }

        private string _strPallet2 = string.Empty;
        public string Pallet2
        {
            get { return _strPallet2; }
            set { _strPallet2 = value; }
        }

        private string _strPallet3 = string.Empty;
        public string Pallet3
        {
            get { return _strPallet3; }
            set { _strPallet3 = value; }
        }

        private string _strPallet4 = string.Empty;
        public string Pallet4
        {
            get { return _strPallet4; }
            set { _strPallet4 = value; }
        }

        private string _strPallet5 = string.Empty;
        public string Pallet5
        {
            get { return _strPallet5; }
            set { _strPallet5 = value; }
        }

        private string _strPallet6 = string.Empty;
        public string Pallet6
        {
            get { return _strPallet6; }
            set { _strPallet6 = value; }
        }



        #region HGAN
        //private string _strHGAN1 = CONST_EMPTY_SPOT_10OCR;
        private string _strHGAN1 = string.Empty;
        public string HGAN1
        {
            get { return _strHGAN1; }
            set { _strHGAN1 = value; }

        }

        private string _strHGAN2 = string.Empty;
        public string HGAN2
        {
            get { return _strHGAN2; }
            set { _strHGAN2 = value; }

        }

        private string _strHGAN3 = string.Empty;
        public string HGAN3
        {
            get { return _strHGAN3; }
            set { _strHGAN3 = value; }

        }

        private string _strHGAN4 = string.Empty;
        public string HGAN4
        {
            get { return _strHGAN4; }
            set { _strHGAN4 = value; }

        }

        private string _strHGAN5 = string.Empty;
        public string HGAN5
        {
            get { return _strHGAN5; }
            set { _strHGAN5 = value; }

        }

        private string _strHGAN6 = string.Empty;
        public string HGAN6
        {
            get { return _strHGAN6; }
            set { _strHGAN6 = value; }

        }

        private string _strHGAN7 = string.Empty;
        public string HGAN7
        {
            get { return _strHGAN7; }
            set { _strHGAN7 = value; }

        }

        private string _strHGAN8 = string.Empty;
        public string HGAN8
        {
            get { return _strHGAN8; }
            set { _strHGAN8 = value; }

        }

        private string _strHGAN9 = string.Empty;
        public string HGAN9
        {
            get { return _strHGAN9; }
            set { _strHGAN9 = value; }

        }

        private string _strHGAN10 = string.Empty;
        public string HGAN10
        {
            get { return _strHGAN10; }
            set { _strHGAN10 = value; }

        }

        private string _strHGAN11 = string.Empty;
        public string HGAN11
        {
            get { return _strHGAN11; }
            set { _strHGAN11 = value; }

        }

        private string _strHGAN12 = string.Empty;
        public string HGAN12
        {
            get { return _strHGAN12; }
            set { _strHGAN12 = value; }

        }

        private string _strHGAN13 = string.Empty;
        public string HGAN13
        {
            get { return _strHGAN13; }
            set { _strHGAN13 = value; }

        }

        private string _strHGAN14 = string.Empty;
        public string HGAN14
        {
            get { return _strHGAN14; }
            set { _strHGAN14 = value; }

        }

        private string _strHGAN15 = string.Empty;
        public string HGAN15
        {
            get { return _strHGAN15; }
            set { _strHGAN15 = value; }

        }

        private string _strHGAN16 = string.Empty;
        public string HGAN16
        {
            get { return _strHGAN16; }
            set { _strHGAN16 = value; }

        }

        private string _strHGAN17 = string.Empty;
        public string HGAN17
        {
            get { return _strHGAN17; }
            set { _strHGAN17 = value; }

        }

        private string _strHGAN18 = string.Empty;
        public string HGAN18
        {
            get { return _strHGAN18; }
            set { _strHGAN18 = value; }

        }

        private string _strHGAN19 = string.Empty;
        public string HGAN19
        {
            get { return _strHGAN19; }
            set { _strHGAN19 = value; }

        }

        private string _strHGAN20 = string.Empty;
        public string HGAN20
        {
            get { return _strHGAN20; }
            set { _strHGAN20 = value; }

        }

        private string _strHGAN21 = string.Empty;
        public string HGAN21
        {
            get { return _strHGAN21; }
            set { _strHGAN21 = value; }

        }

        private string _strHGAN22 = string.Empty;
        public string HGAN22
        {
            get { return _strHGAN22; }
            set { _strHGAN22 = value; }

        }

        private string _strHGAN23 = string.Empty;
        public string HGAN23
        {
            get { return _strHGAN23; }
            set { _strHGAN23 = value; }

        }

        private string _strHGAN24 = string.Empty;
        public string HGAN24
        {
            get { return _strHGAN24; }
            set { _strHGAN24 = value; }

        }

        private string _strHGAN25 = string.Empty;
        public string HGAN25
        {
            get { return _strHGAN25; }
            set { _strHGAN25 = value; }

        }

        private string _strHGAN26 = string.Empty;
        public string HGAN26
        {
            get { return _strHGAN26; }
            set { _strHGAN26 = value; }

        }

        private string _strHGAN27 = string.Empty;
        public string HGAN27
        {
            get { return _strHGAN27; }
            set { _strHGAN27 = value; }

        }

        private string _strHGAN28 = string.Empty;
        public string HGAN28
        {
            get { return _strHGAN28; }
            set { _strHGAN28 = value; }

        }

        private string _strHGAN29 = string.Empty;
        public string HGAN29
        {
            get { return _strHGAN29; }
            set { _strHGAN29 = value; }

        }

        private string _strHGAN30 = string.Empty;
        public string HGAN30
        {
            get { return _strHGAN30; }
            set { _strHGAN30 = value; }

        }

        private string _strHGAN31 = string.Empty;
        public string HGAN31
        {
            get { return _strHGAN31; }
            set { _strHGAN31 = value; }

        }

        private string _strHGAN32 = string.Empty;
        public string HGAN32
        {
            get { return _strHGAN32; }
            set { _strHGAN32 = value; }

        }

        private string _strHGAN33 = string.Empty;
        public string HGAN33
        {
            get { return _strHGAN33; }
            set { _strHGAN33 = value; }

        }

        private string _strHGAN34 = string.Empty;
        public string HGAN34
        {
            get { return _strHGAN34; }
            set { _strHGAN34 = value; }

        }

        private string _strHGAN35 = string.Empty;
        public string HGAN35
        {
            get { return _strHGAN35; }
            set { _strHGAN35 = value; }

        }

        private string _strHGAN36 = string.Empty;
        public string HGAN36
        {
            get { return _strHGAN36; }
            set { _strHGAN36 = value; }

        }

        private string _strHGAN37 = string.Empty;
        public string HGAN37
        {
            get { return _strHGAN37; }
            set { _strHGAN37 = value; }

        }

        private string _strHGAN38 = string.Empty;
        public string HGAN38
        {
            get { return _strHGAN38; }
            set { _strHGAN38 = value; }

        }

        private string _strHGAN39 = string.Empty;
        public string HGAN39
        {
            get { return _strHGAN39; }
            set { _strHGAN39 = value; }

        }

        private string _strHGAN40 = string.Empty;
        public string HGAN40
        {
            get { return _strHGAN40; }
            set { _strHGAN40 = value; }

        }

        private string _strHGAN41 = string.Empty;
        public string HGAN41
        {
            get { return _strHGAN41; }
            set { _strHGAN41 = value; }

        }

        private string _strHGAN42 = string.Empty;
        public string HGAN42
        {
            get { return _strHGAN42; }
            set { _strHGAN42 = value; }

        }

        private string _strHGAN43 = string.Empty;
        public string HGAN43
        {
            get { return _strHGAN43; }
            set { _strHGAN43 = value; }

        }

        private string _strHGAN44 = string.Empty;
        public string HGAN44
        {
            get { return _strHGAN44; }
            set { _strHGAN44 = value; }

        }

        private string _strHGAN45 = string.Empty;
        public string HGAN45
        {
            get { return _strHGAN45; }
            set { _strHGAN45 = value; }

        }

        private string _strHGAN46 = string.Empty;
        public string HGAN46
        {
            get { return _strHGAN46; }
            set { _strHGAN46 = value; }

        }

        private string _strHGAN47 = string.Empty;
        public string HGAN47
        {
            get { return _strHGAN47; }
            set { _strHGAN47 = value; }

        }

        private string _strHGAN48 = string.Empty;
        public string HGAN48
        {
            get { return _strHGAN48; }
            set { _strHGAN48 = value; }

        }

        private string _strHGAN49 = string.Empty;
        public string HGAN49
        {
            get { return _strHGAN49; }
            set { _strHGAN49 = value; }

        }

        private string _strHGAN50 = string.Empty;
        public string HGAN50
        {
            get { return _strHGAN50; }
            set { _strHGAN50 = value; }

        }

        private string _strHGAN51 = string.Empty;
        public string HGAN51
        {
            get { return _strHGAN51; }
            set { _strHGAN51 = value; }

        }

        private string _strHGAN52 = string.Empty;
        public string HGAN52
        {
            get { return _strHGAN52; }
            set { _strHGAN52 = value; }

        }

        private string _strHGAN53 = string.Empty;
        public string HGAN53
        {
            get { return _strHGAN53; }
            set { _strHGAN53 = value; }

        }

        private string _strHGAN54 = string.Empty;
        public string HGAN54
        {
            get { return _strHGAN54; }
            set { _strHGAN54 = value; }

        }

        private string _strHGAN55 = string.Empty;
        public string HGAN55
        {
            get { return _strHGAN55; }
            set { _strHGAN55 = value; }

        }

        private string _strHGAN56 = string.Empty;
        public string HGAN56
        {
            get { return _strHGAN56; }
            set { _strHGAN56 = value; }

        }

        private string _strHGAN57 = string.Empty;
        public string HGAN57
        {
            get { return _strHGAN57; }
            set { _strHGAN57 = value; }

        }

        private string _strHGAN58 = string.Empty;
        public string HGAN58
        {
            get { return _strHGAN58; }
            set { _strHGAN58 = value; }

        }

        private string _strHGAN59 = string.Empty;
        public string HGAN59
        {
            get { return _strHGAN59; }
            set { _strHGAN59 = value; }

        }

        private string _strHGAN60 = string.Empty;
        public string HGAN60
        {
            get { return _strHGAN60; }
            set { _strHGAN60 = value; }

        }



        #endregion



        #region DefectN
        //comma separated string format
        private string _strDefectN1 = string.Empty;
        public string DefectN1
        {
            get { return _strDefectN1; }
            set { _strDefectN1 = value; }

        }

        private string _strDefectN2 = string.Empty;
        public string DefectN2
        {
            get { return _strDefectN2; }
            set { _strDefectN2 = value; }

        }

        private string _strDefectN3 = string.Empty;
        public string DefectN3
        {
            get { return _strDefectN3; }
            set { _strDefectN3 = value; }

        }

        private string _strDefectN4 = string.Empty;
        public string DefectN4
        {
            get { return _strDefectN4; }
            set { _strDefectN4 = value; }

        }

        private string _strDefectN5 = string.Empty;
        public string DefectN5
        {
            get { return _strDefectN5; }
            set { _strDefectN5 = value; }

        }

        private string _strDefectN6 = string.Empty;
        public string DefectN6
        {
            get { return _strDefectN6; }
            set { _strDefectN6 = value; }

        }

        private string _strDefectN7 = string.Empty;
        public string DefectN7
        {
            get { return _strDefectN7; }
            set { _strDefectN7 = value; }

        }

        private string _strDefectN8 = string.Empty;
        public string DefectN8
        {
            get { return _strDefectN8; }
            set { _strDefectN8 = value; }

        }

        private string _strDefectN9 = string.Empty;
        public string DefectN9
        {
            get { return _strDefectN9; }
            set { _strDefectN9 = value; }

        }

        private string _strDefectN10 = string.Empty;
        public string DefectN10
        {
            get { return _strDefectN10; }
            set { _strDefectN10 = value; }

        }

        private string _strDefectN11 = string.Empty;
        public string DefectN11
        {
            get { return _strDefectN11; }
            set { _strDefectN11 = value; }

        }

        private string _strDefectN12 = string.Empty;
        public string DefectN12
        {
            get { return _strDefectN12; }
            set { _strDefectN12 = value; }

        }

        private string _strDefectN13 = string.Empty;
        public string DefectN13
        {
            get { return _strDefectN13; }
            set { _strDefectN13 = value; }

        }

        private string _strDefectN14 = string.Empty;
        public string DefectN14
        {
            get { return _strDefectN14; }
            set { _strDefectN14 = value; }

        }

        private string _strDefectN15 = string.Empty;
        public string DefectN15
        {
            get { return _strDefectN15; }
            set { _strDefectN15 = value; }

        }

        private string _strDefectN16 = string.Empty;
        public string DefectN16
        {
            get { return _strDefectN16; }
            set { _strDefectN16 = value; }

        }

        private string _strDefectN17 = string.Empty;
        public string DefectN17
        {
            get { return _strDefectN17; }
            set { _strDefectN17 = value; }

        }

        private string _strDefectN18 = string.Empty;
        public string DefectN18
        {
            get { return _strDefectN18; }
            set { _strDefectN18 = value; }

        }

        private string _strDefectN19 = string.Empty;
        public string DefectN19
        {
            get { return _strDefectN19; }
            set { _strDefectN19 = value; }

        }

        private string _strDefectN20 = string.Empty;
        public string DefectN20
        {
            get { return _strDefectN20; }
            set { _strDefectN20 = value; }

        }

        private string _strDefectN21 = string.Empty;
        public string DefectN21
        {
            get { return _strDefectN21; }
            set { _strDefectN21 = value; }

        }

        private string _strDefectN22 = string.Empty;
        public string DefectN22
        {
            get { return _strDefectN22; }
            set { _strDefectN22 = value; }

        }

        private string _strDefectN23 = string.Empty;
        public string DefectN23
        {
            get { return _strDefectN23; }
            set { _strDefectN23 = value; }

        }

        private string _strDefectN24 = string.Empty;
        public string DefectN24
        {
            get { return _strDefectN24; }
            set { _strDefectN24 = value; }

        }

        private string _strDefectN25 = string.Empty;
        public string DefectN25
        {
            get { return _strDefectN25; }
            set { _strDefectN25 = value; }

        }

        private string _strDefectN26 = string.Empty;
        public string DefectN26
        {
            get { return _strDefectN26; }
            set { _strDefectN26 = value; }

        }

        private string _strDefectN27 = string.Empty;
        public string DefectN27
        {
            get { return _strDefectN27; }
            set { _strDefectN27 = value; }

        }

        private string _strDefectN28 = string.Empty;
        public string DefectN28
        {
            get { return _strDefectN28; }
            set { _strDefectN28 = value; }

        }

        private string _strDefectN29 = string.Empty;
        public string DefectN29
        {
            get { return _strDefectN29; }
            set { _strDefectN29 = value; }

        }

        private string _strDefectN30 = string.Empty;
        public string DefectN30
        {
            get { return _strDefectN30; }
            set { _strDefectN30 = value; }

        }

        private string _strDefectN31 = string.Empty;
        public string DefectN31
        {
            get { return _strDefectN31; }
            set { _strDefectN31 = value; }

        }

        private string _strDefectN32 = string.Empty;
        public string DefectN32
        {
            get { return _strDefectN32; }
            set { _strDefectN32 = value; }

        }

        private string _strDefectN33 = string.Empty;
        public string DefectN33
        {
            get { return _strDefectN33; }
            set { _strDefectN33 = value; }

        }

        private string _strDefectN34 = string.Empty;
        public string DefectN34
        {
            get { return _strDefectN34; }
            set { _strDefectN34 = value; }

        }

        private string _strDefectN35 = string.Empty;
        public string DefectN35
        {
            get { return _strDefectN35; }
            set { _strDefectN35 = value; }

        }

        private string _strDefectN36 = string.Empty;
        public string DefectN36
        {
            get { return _strDefectN36; }
            set { _strDefectN36 = value; }

        }

        private string _strDefectN37 = string.Empty;
        public string DefectN37
        {
            get { return _strDefectN37; }
            set { _strDefectN37 = value; }

        }

        private string _strDefectN38 = string.Empty;
        public string DefectN38
        {
            get { return _strDefectN38; }
            set { _strDefectN38 = value; }

        }

        private string _strDefectN39 = string.Empty;
        public string DefectN39
        {
            get { return _strDefectN39; }
            set { _strDefectN39 = value; }

        }

        private string _strDefectN40 = string.Empty;
        public string DefectN40
        {
            get { return _strDefectN40; }
            set { _strDefectN40 = value; }

        }

        private string _strDefectN41 = string.Empty;
        public string DefectN41
        {
            get { return _strDefectN41; }
            set { _strDefectN41 = value; }

        }

        private string _strDefectN42 = string.Empty;
        public string DefectN42
        {
            get { return _strDefectN42; }
            set { _strDefectN42 = value; }

        }

        private string _strDefectN43 = string.Empty;
        public string DefectN43
        {
            get { return _strDefectN43; }
            set { _strDefectN43 = value; }

        }

        private string _strDefectN44 = string.Empty;
        public string DefectN44
        {
            get { return _strDefectN44; }
            set { _strDefectN44 = value; }

        }

        private string _strDefectN45 = string.Empty;
        public string DefectN45
        {
            get { return _strDefectN45; }
            set { _strDefectN45 = value; }

        }

        private string _strDefectN46 = string.Empty;
        public string DefectN46
        {
            get { return _strDefectN46; }
            set { _strDefectN46 = value; }

        }

        private string _strDefectN47 = string.Empty;
        public string DefectN47
        {
            get { return _strDefectN47; }
            set { _strDefectN47 = value; }

        }

        private string _strDefectN48 = string.Empty;
        public string DefectN48
        {
            get { return _strDefectN48; }
            set { _strDefectN48 = value; }

        }

        private string _strDefectN49 = string.Empty;
        public string DefectN49
        {
            get { return _strDefectN49; }
            set { _strDefectN49 = value; }

        }

        private string _strDefectN50 = string.Empty;
        public string DefectN50
        {
            get { return _strDefectN50; }
            set { _strDefectN50 = value; }

        }

        private string _strDefectN51 = string.Empty;
        public string DefectN51
        {
            get { return _strDefectN51; }
            set { _strDefectN51 = value; }

        }

        private string _strDefectN52 = string.Empty;
        public string DefectN52
        {
            get { return _strDefectN52; }
            set { _strDefectN52 = value; }

        }

        private string _strDefectN53 = string.Empty;
        public string DefectN53
        {
            get { return _strDefectN53; }
            set { _strDefectN53 = value; }

        }

        private string _strDefectN54 = string.Empty;
        public string DefectN54
        {
            get { return _strDefectN54; }
            set { _strDefectN54 = value; }

        }

        private string _strDefectN55 = string.Empty;
        public string DefectN55
        {
            get { return _strDefectN55; }
            set { _strDefectN55 = value; }

        }

        private string _strDefectN56 = string.Empty;
        public string DefectN56
        {
            get { return _strDefectN56; }
            set { _strDefectN56 = value; }

        }

        private string _strDefectN57 = string.Empty;
        public string DefectN57
        {
            get { return _strDefectN57; }
            set { _strDefectN57 = value; }

        }

        private string _strDefectN58 = string.Empty;
        public string DefectN58
        {
            get { return _strDefectN58; }
            set { _strDefectN58 = value; }

        }

        private string _strDefectN59 = string.Empty;
        public string DefectN59
        {
            get { return _strDefectN59; }
            set { _strDefectN59 = value; }

        }

        private string _strDefectN60 = string.Empty;
        public string DefectN60
        {
            get { return _strDefectN60; }
            set { _strDefectN60 = value; }

        }

        #endregion


        public void ReadFile(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return;
            }

            string text = System.IO.File.ReadAllText(strFilePath);
            AQTrayObj aTray = AQTrayObj.ToAQTrayObj(text);

            this.Date = aTray.Date;
            this.TimeStart = aTray.TimeStart;
            this.TimeEnd = aTray.TimeEnd;
            this.UsedTime = aTray.UsedTime;

            this.TesterNumber = aTray.TesterNumber;

            this.Customer = aTray.Customer;
            this.Product = aTray.Product;
            this.User = aTray.User;

            this.TrayID = aTray.TrayID;

            this.LotNumber = aTray.LotNumber;
            this.DocControl1 = aTray.DocControl1;
            this.DocControl2 = aTray.DocControl2;
            this.Sus = aTray.Sus;
            this.AssyLine = aTray.AssyLine;


            this.HGAN1 = aTray.HGAN1;
            this.HGAN2 = aTray.HGAN2;
            this.HGAN3 = aTray.HGAN3;
            this.HGAN4 = aTray.HGAN4;
            this.HGAN5 = aTray.HGAN5;

            this.HGAN6 = aTray.HGAN6;
            this.HGAN7 = aTray.HGAN7;
            this.HGAN8 = aTray.HGAN8;
            this.HGAN9 = aTray.HGAN9;
            this.HGAN10 = aTray.HGAN10;

            this.HGAN11 = aTray.HGAN11;
            this.HGAN12 = aTray.HGAN12;
            this.HGAN13 = aTray.HGAN13;
            this.HGAN14 = aTray.HGAN14;
            this.HGAN15 = aTray.HGAN15;

            this.HGAN16 = aTray.HGAN16;
            this.HGAN17 = aTray.HGAN17;
            this.HGAN18 = aTray.HGAN18;
            this.HGAN19 = aTray.HGAN19;
            this.HGAN20 = aTray.HGAN20;

            this.HGAN21 = aTray.HGAN21;
            this.HGAN22 = aTray.HGAN22;
            this.HGAN23 = aTray.HGAN23;
            this.HGAN24 = aTray.HGAN24;
            this.HGAN25 = aTray.HGAN25;

            this.HGAN26 = aTray.HGAN26;
            this.HGAN27 = aTray.HGAN27;
            this.HGAN28 = aTray.HGAN28;
            this.HGAN29 = aTray.HGAN29;
            this.HGAN30 = aTray.HGAN30;

            this.HGAN31 = aTray.HGAN31;
            this.HGAN32 = aTray.HGAN32;
            this.HGAN33 = aTray.HGAN33;
            this.HGAN34 = aTray.HGAN34;
            this.HGAN35 = aTray.HGAN35;

            this.HGAN36 = aTray.HGAN36;
            this.HGAN37 = aTray.HGAN37;
            this.HGAN38 = aTray.HGAN38;
            this.HGAN39 = aTray.HGAN39;
            this.HGAN40 = aTray.HGAN40;

            this.HGAN41 = aTray.HGAN41;
            this.HGAN42 = aTray.HGAN42;
            this.HGAN43 = aTray.HGAN43;
            this.HGAN44 = aTray.HGAN44;
            this.HGAN45 = aTray.HGAN45;

            this.HGAN46 = aTray.HGAN46;
            this.HGAN47 = aTray.HGAN47;
            this.HGAN48 = aTray.HGAN48;
            this.HGAN49 = aTray.HGAN49;
            this.HGAN50 = aTray.HGAN50;

            this.HGAN51 = aTray.HGAN51;
            this.HGAN52 = aTray.HGAN52;
            this.HGAN53 = aTray.HGAN53;
            this.HGAN54 = aTray.HGAN54;
            this.HGAN55 = aTray.HGAN55;

            this.HGAN56 = aTray.HGAN56;
            this.HGAN57 = aTray.HGAN57;
            this.HGAN58 = aTray.HGAN58;
            this.HGAN59 = aTray.HGAN59;
            this.HGAN60 = aTray.HGAN60;


            this.DefectN1 = aTray.DefectN1;
            this.DefectN2 = aTray.DefectN2;
            this.DefectN3 = aTray.DefectN3;
            this.DefectN4 = aTray.DefectN4;
            this.DefectN5 = aTray.DefectN5;

            this.DefectN6 = aTray.DefectN6;
            this.DefectN7 = aTray.DefectN7;
            this.DefectN8 = aTray.DefectN8;
            this.DefectN9 = aTray.DefectN9;
            this.DefectN10 = aTray.DefectN10;

            this.DefectN11 = aTray.DefectN11;
            this.DefectN12 = aTray.DefectN12;
            this.DefectN13 = aTray.DefectN13;
            this.DefectN14 = aTray.DefectN14;
            this.DefectN15 = aTray.DefectN15;

            this.DefectN16 = aTray.DefectN16;
            this.DefectN17 = aTray.DefectN17;
            this.DefectN18 = aTray.DefectN18;
            this.DefectN19 = aTray.DefectN19;
            this.DefectN20 = aTray.DefectN20;

            this.DefectN21 = aTray.DefectN21;
            this.DefectN22 = aTray.DefectN22;
            this.DefectN23 = aTray.DefectN23;
            this.DefectN24 = aTray.DefectN24;
            this.DefectN25 = aTray.DefectN25;

            this.DefectN26 = aTray.DefectN26;
            this.DefectN27 = aTray.DefectN27;
            this.DefectN28 = aTray.DefectN28;
            this.DefectN29 = aTray.DefectN29;
            this.DefectN30 = aTray.DefectN30;

            this.DefectN31 = aTray.DefectN31;
            this.DefectN32 = aTray.DefectN32;
            this.DefectN33 = aTray.DefectN33;
            this.DefectN34 = aTray.DefectN34;
            this.DefectN35 = aTray.DefectN35;

            this.DefectN36 = aTray.DefectN36;
            this.DefectN37 = aTray.DefectN37;
            this.DefectN38 = aTray.DefectN38;
            this.DefectN39 = aTray.DefectN39;
            this.DefectN40 = aTray.DefectN40;

            this.DefectN41 = aTray.DefectN41;
            this.DefectN42 = aTray.DefectN42;
            this.DefectN43 = aTray.DefectN43;
            this.DefectN44 = aTray.DefectN44;
            this.DefectN45 = aTray.DefectN45;

            this.DefectN46 = aTray.DefectN46;
            this.DefectN47 = aTray.DefectN47;
            this.DefectN48 = aTray.DefectN48;
            this.DefectN49 = aTray.DefectN49;
            this.DefectN50 = aTray.DefectN50;

            this.DefectN51 = aTray.DefectN51;
            this.DefectN52 = aTray.DefectN52;
            this.DefectN53 = aTray.DefectN53;
            this.DefectN54 = aTray.DefectN54;
            this.DefectN55 = aTray.DefectN55;

            this.DefectN56 = aTray.DefectN56;
            this.DefectN57 = aTray.DefectN57;
            this.DefectN58 = aTray.DefectN58;
            this.DefectN59 = aTray.DefectN59;
            this.DefectN60 = aTray.DefectN60;

        }


        public void ToAQTrayFile(string strFilename)
        {
            StreamWriter sw = new StreamWriter(strFilename);

            sw.WriteLine("[INFORMATION]");
            //CONST_DATE
            sw.WriteLine(CONST_DATE + this.Date);
            //CONST_TIME_START
            sw.WriteLine(CONST_TIME_START + this.TimeStart);
            //CONST_TIME_END
            sw.WriteLine(CONST_TIME_END + this.TimeEnd);
            //CONST_USED_TIME
            sw.WriteLine(CONST_USED_TIME + this.UsedTime);
            //CONST_TESTER_NUMBER
            sw.WriteLine(CONST_TESTER_NUMBER + this.TesterNumber);
            //CONST_CUSTOMER
            sw.WriteLine(CONST_CUSTOMER + this.Customer);
            //CONST_PRODUCT
            sw.WriteLine(CONST_PRODUCT + this.Product);
            //CONST_USER
            sw.WriteLine(CONST_USER + this.User);            
            //CONST_TRAY_ID
            sw.WriteLine(CONST_TRAY_ID + this.TrayID);            
            //CONST_LOTNUMBER
            sw.WriteLine(CONST_LOTNUMBER + this.LotNumber);
            //CONST_DOCCONTROL1 
            sw.WriteLine(CONST_DOCCONTROL1 + this.DocControl1);
            //CONST_DOCCONTROL2 
            sw.WriteLine(CONST_DOCCONTROL2 + this.DocControl2);
            //CONST_SUS
            sw.WriteLine(CONST_SUS + this.Sus);
            //CONST_ASSYLINE = "
            sw.WriteLine(CONST_ASSYLINE + this.AssyLine);

            sw.WriteLine();
            sw.WriteLine();



            sw.WriteLine("[SERIAL]");
            sw.WriteLine((this.DefectN1.Length > 0) ? CONST_HGAN1 : CONST_HGAN1 + this.HGAN1);
            sw.WriteLine((this.DefectN2.Length > 0) ? CONST_HGAN2 : CONST_HGAN2 + this.HGAN2);
            sw.WriteLine((this.DefectN3.Length > 0) ? CONST_HGAN3 : CONST_HGAN3 + this.HGAN3);
            sw.WriteLine((this.DefectN4.Length > 0) ? CONST_HGAN4 : CONST_HGAN4 + this.HGAN4);
            sw.WriteLine((this.DefectN5.Length > 0) ? CONST_HGAN5 : CONST_HGAN5 + this.HGAN5);
            sw.WriteLine((this.DefectN6.Length > 0) ? CONST_HGAN6 : CONST_HGAN6 + this.HGAN6);
            sw.WriteLine((this.DefectN7.Length > 0) ? CONST_HGAN7 : CONST_HGAN7 + this.HGAN7);
            sw.WriteLine((this.DefectN8.Length > 0) ? CONST_HGAN8 : CONST_HGAN8 + this.HGAN8);
            sw.WriteLine((this.DefectN9.Length > 0) ? CONST_HGAN9 : CONST_HGAN9 + this.HGAN9);
            sw.WriteLine((this.DefectN10.Length > 0) ? CONST_HGAN10 : CONST_HGAN10 + this.HGAN10);

            sw.WriteLine((this.DefectN11.Length > 0) ? CONST_HGAN11 : CONST_HGAN11 + this.HGAN11);
            sw.WriteLine((this.DefectN12.Length > 0) ? CONST_HGAN12 : CONST_HGAN12 + this.HGAN12);
            sw.WriteLine((this.DefectN13.Length > 0) ? CONST_HGAN13 : CONST_HGAN13 + this.HGAN13);
            sw.WriteLine((this.DefectN14.Length > 0) ? CONST_HGAN14 : CONST_HGAN14 + this.HGAN14);
            sw.WriteLine((this.DefectN15.Length > 0) ? CONST_HGAN15 : CONST_HGAN15 + this.HGAN15);
            sw.WriteLine((this.DefectN16.Length > 0) ? CONST_HGAN16 : CONST_HGAN16 + this.HGAN16);
            sw.WriteLine((this.DefectN17.Length > 0) ? CONST_HGAN17 : CONST_HGAN17 + this.HGAN17);
            sw.WriteLine((this.DefectN18.Length > 0) ? CONST_HGAN18 : CONST_HGAN18 + this.HGAN18);
            sw.WriteLine((this.DefectN19.Length > 0) ? CONST_HGAN19 : CONST_HGAN19 + this.HGAN19);
            sw.WriteLine((this.DefectN20.Length > 0) ? CONST_HGAN20 : CONST_HGAN20 + this.HGAN20);

            sw.WriteLine((this.DefectN21.Length > 0) ? CONST_HGAN21 : CONST_HGAN21 + this.HGAN21);
            sw.WriteLine((this.DefectN22.Length > 0) ? CONST_HGAN22 : CONST_HGAN22 + this.HGAN22);
            sw.WriteLine((this.DefectN23.Length > 0) ? CONST_HGAN23 : CONST_HGAN23 + this.HGAN23);
            sw.WriteLine((this.DefectN24.Length > 0) ? CONST_HGAN24 : CONST_HGAN24 + this.HGAN24);
            sw.WriteLine((this.DefectN25.Length > 0) ? CONST_HGAN25 : CONST_HGAN25 + this.HGAN25);
            sw.WriteLine((this.DefectN26.Length > 0) ? CONST_HGAN26 : CONST_HGAN26 + this.HGAN26);
            sw.WriteLine((this.DefectN27.Length > 0) ? CONST_HGAN27 : CONST_HGAN27 + this.HGAN27);
            sw.WriteLine((this.DefectN28.Length > 0) ? CONST_HGAN28 : CONST_HGAN28 + this.HGAN28);
            sw.WriteLine((this.DefectN29.Length > 0) ? CONST_HGAN29 : CONST_HGAN29 + this.HGAN29);
            sw.WriteLine((this.DefectN30.Length > 0) ? CONST_HGAN30 : CONST_HGAN30 + this.HGAN30);

            sw.WriteLine((this.DefectN31.Length > 0) ? CONST_HGAN31 : CONST_HGAN31 + this.HGAN31);
            sw.WriteLine((this.DefectN32.Length > 0) ? CONST_HGAN32 : CONST_HGAN32 + this.HGAN32);
            sw.WriteLine((this.DefectN33.Length > 0) ? CONST_HGAN33 : CONST_HGAN33 + this.HGAN33);
            sw.WriteLine((this.DefectN34.Length > 0) ? CONST_HGAN34 : CONST_HGAN34 + this.HGAN34);
            sw.WriteLine((this.DefectN35.Length > 0) ? CONST_HGAN35 : CONST_HGAN35 + this.HGAN35);
            sw.WriteLine((this.DefectN36.Length > 0) ? CONST_HGAN36 : CONST_HGAN36 + this.HGAN36);
            sw.WriteLine((this.DefectN37.Length > 0) ? CONST_HGAN37 : CONST_HGAN37 + this.HGAN37);
            sw.WriteLine((this.DefectN38.Length > 0) ? CONST_HGAN38 : CONST_HGAN38 + this.HGAN38);
            sw.WriteLine((this.DefectN39.Length > 0) ? CONST_HGAN39 : CONST_HGAN39 + this.HGAN39);
            sw.WriteLine((this.DefectN40.Length > 0) ? CONST_HGAN40 : CONST_HGAN40 + this.HGAN40);

            sw.WriteLine((this.DefectN41.Length > 0) ? CONST_HGAN41 : CONST_HGAN41 + this.HGAN41);
            sw.WriteLine((this.DefectN42.Length > 0) ? CONST_HGAN42 : CONST_HGAN42 + this.HGAN42);
            sw.WriteLine((this.DefectN43.Length > 0) ? CONST_HGAN43 : CONST_HGAN43 + this.HGAN43);
            sw.WriteLine((this.DefectN44.Length > 0) ? CONST_HGAN44 : CONST_HGAN44 + this.HGAN44);
            sw.WriteLine((this.DefectN45.Length > 0) ? CONST_HGAN45 : CONST_HGAN45 + this.HGAN45);
            sw.WriteLine((this.DefectN46.Length > 0) ? CONST_HGAN46 : CONST_HGAN46 + this.HGAN46);
            sw.WriteLine((this.DefectN47.Length > 0) ? CONST_HGAN47 : CONST_HGAN47 + this.HGAN47);
            sw.WriteLine((this.DefectN48.Length > 0) ? CONST_HGAN48 : CONST_HGAN48 + this.HGAN48);
            sw.WriteLine((this.DefectN49.Length > 0) ? CONST_HGAN49 : CONST_HGAN49 + this.HGAN49);
            sw.WriteLine((this.DefectN50.Length > 0) ? CONST_HGAN50 : CONST_HGAN50 + this.HGAN50);

            sw.WriteLine((this.DefectN51.Length > 0) ? CONST_HGAN51 : CONST_HGAN51 + this.HGAN51);
            sw.WriteLine((this.DefectN52.Length > 0) ? CONST_HGAN52 : CONST_HGAN52 + this.HGAN52);
            sw.WriteLine((this.DefectN53.Length > 0) ? CONST_HGAN53 : CONST_HGAN53 + this.HGAN53);
            sw.WriteLine((this.DefectN54.Length > 0) ? CONST_HGAN54 : CONST_HGAN54 + this.HGAN54);
            sw.WriteLine((this.DefectN55.Length > 0) ? CONST_HGAN55 : CONST_HGAN55 + this.HGAN55);
            sw.WriteLine((this.DefectN56.Length > 0) ? CONST_HGAN56 : CONST_HGAN56 + this.HGAN56);
            sw.WriteLine((this.DefectN57.Length > 0) ? CONST_HGAN57 : CONST_HGAN57 + this.HGAN57);
            sw.WriteLine((this.DefectN58.Length > 0) ? CONST_HGAN58 : CONST_HGAN58 + this.HGAN58);
            sw.WriteLine((this.DefectN59.Length > 0) ? CONST_HGAN59 : CONST_HGAN59 + this.HGAN59);
            sw.WriteLine((this.DefectN60.Length > 0) ? CONST_HGAN60 : CONST_HGAN60 + this.HGAN60);

            sw.Close();
        }


        public void ToAQTrayFile(string strFilename, int nTrayType /*20, 40, 60*/)
        {
            StreamWriter sw = new StreamWriter(strFilename);

            sw.WriteLine("[INFORMATION]");
            //CONST_DATE
            sw.WriteLine(CONST_DATE + this.Date);
            //CONST_TIME_START
            sw.WriteLine(CONST_TIME_START + this.TimeStart);
            //CONST_TIME_END
            sw.WriteLine(CONST_TIME_END + this.TimeEnd);
            //CONST_USED_TIME
            sw.WriteLine(CONST_USED_TIME + this.UsedTime);
            //CONST_TESTER_NUMBER
            sw.WriteLine(CONST_TESTER_NUMBER + this.TesterNumber);
            //CONST_CUSTOMER
            sw.WriteLine(CONST_CUSTOMER + this.Customer);
            //CONST_PRODUCT
            sw.WriteLine(CONST_PRODUCT + this.Product);
            //CONST_USER
            sw.WriteLine(CONST_USER + this.User);
            //CONST_TRAY_ID
            sw.WriteLine(CONST_TRAY_ID + this.TrayID);
            //CONST_LOTNUMBER
            sw.WriteLine(CONST_LOTNUMBER + this.LotNumber);
            //CONST_DOCCONTROL1 
            sw.WriteLine(CONST_DOCCONTROL1 + this.DocControl1);
            //CONST_DOCCONTROL2 
            sw.WriteLine(CONST_DOCCONTROL2 + this.DocControl2);
            //CONST_SUS
            sw.WriteLine(CONST_SUS + this.Sus);
            //CONST_ASSYLINE = "
            sw.WriteLine(CONST_ASSYLINE + this.AssyLine);

            sw.WriteLine();
            sw.WriteLine();



            sw.WriteLine("[SERIAL]");
            sw.WriteLine((this.DefectN1.Length > 0) ? CONST_HGAN1 : CONST_HGAN1 + this.HGAN1);
            sw.WriteLine((this.DefectN2.Length > 0) ? CONST_HGAN2 : CONST_HGAN2 + this.HGAN2);
            sw.WriteLine((this.DefectN3.Length > 0) ? CONST_HGAN3 : CONST_HGAN3 + this.HGAN3);
            sw.WriteLine((this.DefectN4.Length > 0) ? CONST_HGAN4 : CONST_HGAN4 + this.HGAN4);
            sw.WriteLine((this.DefectN5.Length > 0) ? CONST_HGAN5 : CONST_HGAN5 + this.HGAN5);
            sw.WriteLine((this.DefectN6.Length > 0) ? CONST_HGAN6 : CONST_HGAN6 + this.HGAN6);
            sw.WriteLine((this.DefectN7.Length > 0) ? CONST_HGAN7 : CONST_HGAN7 + this.HGAN7);
            sw.WriteLine((this.DefectN8.Length > 0) ? CONST_HGAN8 : CONST_HGAN8 + this.HGAN8);
            sw.WriteLine((this.DefectN9.Length > 0) ? CONST_HGAN9 : CONST_HGAN9 + this.HGAN9);
            sw.WriteLine((this.DefectN10.Length > 0) ? CONST_HGAN10 : CONST_HGAN10 + this.HGAN10);

            sw.WriteLine((this.DefectN11.Length > 0) ? CONST_HGAN11 : CONST_HGAN11 + this.HGAN11);
            sw.WriteLine((this.DefectN12.Length > 0) ? CONST_HGAN12 : CONST_HGAN12 + this.HGAN12);
            sw.WriteLine((this.DefectN13.Length > 0) ? CONST_HGAN13 : CONST_HGAN13 + this.HGAN13);
            sw.WriteLine((this.DefectN14.Length > 0) ? CONST_HGAN14 : CONST_HGAN14 + this.HGAN14);
            sw.WriteLine((this.DefectN15.Length > 0) ? CONST_HGAN15 : CONST_HGAN15 + this.HGAN15);
            sw.WriteLine((this.DefectN16.Length > 0) ? CONST_HGAN16 : CONST_HGAN16 + this.HGAN16);
            sw.WriteLine((this.DefectN17.Length > 0) ? CONST_HGAN17 : CONST_HGAN17 + this.HGAN17);
            sw.WriteLine((this.DefectN18.Length > 0) ? CONST_HGAN18 : CONST_HGAN18 + this.HGAN18);
            sw.WriteLine((this.DefectN19.Length > 0) ? CONST_HGAN19 : CONST_HGAN19 + this.HGAN19);
            sw.WriteLine((this.DefectN20.Length > 0) ? CONST_HGAN20 : CONST_HGAN20 + this.HGAN20);


            if (nTrayType > 20) //for tray40
            {
                sw.WriteLine((this.DefectN21.Length > 0) ? CONST_HGAN21 : CONST_HGAN21 + this.HGAN21);
                sw.WriteLine((this.DefectN22.Length > 0) ? CONST_HGAN22 : CONST_HGAN22 + this.HGAN22);
                sw.WriteLine((this.DefectN23.Length > 0) ? CONST_HGAN23 : CONST_HGAN23 + this.HGAN23);
                sw.WriteLine((this.DefectN24.Length > 0) ? CONST_HGAN24 : CONST_HGAN24 + this.HGAN24);
                sw.WriteLine((this.DefectN25.Length > 0) ? CONST_HGAN25 : CONST_HGAN25 + this.HGAN25);
                sw.WriteLine((this.DefectN26.Length > 0) ? CONST_HGAN26 : CONST_HGAN26 + this.HGAN26);
                sw.WriteLine((this.DefectN27.Length > 0) ? CONST_HGAN27 : CONST_HGAN27 + this.HGAN27);
                sw.WriteLine((this.DefectN28.Length > 0) ? CONST_HGAN28 : CONST_HGAN28 + this.HGAN28);
                sw.WriteLine((this.DefectN29.Length > 0) ? CONST_HGAN29 : CONST_HGAN29 + this.HGAN29);
                sw.WriteLine((this.DefectN30.Length > 0) ? CONST_HGAN30 : CONST_HGAN30 + this.HGAN30);

                sw.WriteLine((this.DefectN31.Length > 0) ? CONST_HGAN31 : CONST_HGAN31 + this.HGAN31);
                sw.WriteLine((this.DefectN32.Length > 0) ? CONST_HGAN32 : CONST_HGAN32 + this.HGAN32);
                sw.WriteLine((this.DefectN33.Length > 0) ? CONST_HGAN33 : CONST_HGAN33 + this.HGAN33);
                sw.WriteLine((this.DefectN34.Length > 0) ? CONST_HGAN34 : CONST_HGAN34 + this.HGAN34);
                sw.WriteLine((this.DefectN35.Length > 0) ? CONST_HGAN35 : CONST_HGAN35 + this.HGAN35);
                sw.WriteLine((this.DefectN36.Length > 0) ? CONST_HGAN36 : CONST_HGAN36 + this.HGAN36);
                sw.WriteLine((this.DefectN37.Length > 0) ? CONST_HGAN37 : CONST_HGAN37 + this.HGAN37);
                sw.WriteLine((this.DefectN38.Length > 0) ? CONST_HGAN38 : CONST_HGAN38 + this.HGAN38);
                sw.WriteLine((this.DefectN39.Length > 0) ? CONST_HGAN39 : CONST_HGAN39 + this.HGAN39);
                sw.WriteLine((this.DefectN40.Length > 0) ? CONST_HGAN40 : CONST_HGAN40 + this.HGAN40);
            }

            if (nTrayType > 40) //for tray60
            {
                sw.WriteLine((this.DefectN41.Length > 0) ? CONST_HGAN41 : CONST_HGAN41 + this.HGAN41);
                sw.WriteLine((this.DefectN42.Length > 0) ? CONST_HGAN42 : CONST_HGAN42 + this.HGAN42);
                sw.WriteLine((this.DefectN43.Length > 0) ? CONST_HGAN43 : CONST_HGAN43 + this.HGAN43);
                sw.WriteLine((this.DefectN44.Length > 0) ? CONST_HGAN44 : CONST_HGAN44 + this.HGAN44);
                sw.WriteLine((this.DefectN45.Length > 0) ? CONST_HGAN45 : CONST_HGAN45 + this.HGAN45);
                sw.WriteLine((this.DefectN46.Length > 0) ? CONST_HGAN46 : CONST_HGAN46 + this.HGAN46);
                sw.WriteLine((this.DefectN47.Length > 0) ? CONST_HGAN47 : CONST_HGAN47 + this.HGAN47);
                sw.WriteLine((this.DefectN48.Length > 0) ? CONST_HGAN48 : CONST_HGAN48 + this.HGAN48);
                sw.WriteLine((this.DefectN49.Length > 0) ? CONST_HGAN49 : CONST_HGAN49 + this.HGAN49);
                sw.WriteLine((this.DefectN50.Length > 0) ? CONST_HGAN50 : CONST_HGAN50 + this.HGAN50);

                sw.WriteLine((this.DefectN51.Length > 0) ? CONST_HGAN51 : CONST_HGAN51 + this.HGAN51);
                sw.WriteLine((this.DefectN52.Length > 0) ? CONST_HGAN52 : CONST_HGAN52 + this.HGAN52);
                sw.WriteLine((this.DefectN53.Length > 0) ? CONST_HGAN53 : CONST_HGAN53 + this.HGAN53);
                sw.WriteLine((this.DefectN54.Length > 0) ? CONST_HGAN54 : CONST_HGAN54 + this.HGAN54);
                sw.WriteLine((this.DefectN55.Length > 0) ? CONST_HGAN55 : CONST_HGAN55 + this.HGAN55);
                sw.WriteLine((this.DefectN56.Length > 0) ? CONST_HGAN56 : CONST_HGAN56 + this.HGAN56);
                sw.WriteLine((this.DefectN57.Length > 0) ? CONST_HGAN57 : CONST_HGAN57 + this.HGAN57);
                sw.WriteLine((this.DefectN58.Length > 0) ? CONST_HGAN58 : CONST_HGAN58 + this.HGAN58);
                sw.WriteLine((this.DefectN59.Length > 0) ? CONST_HGAN59 : CONST_HGAN59 + this.HGAN59);
                sw.WriteLine((this.DefectN60.Length > 0) ? CONST_HGAN60 : CONST_HGAN60 + this.HGAN60);
            }

            sw.Close();
        }


        public void ToAQTrayFile(string strFilename, bool bIgnoreDefects)
        {
            StreamWriter sw = new StreamWriter(strFilename);

            sw.WriteLine("[INFORMATION]");
            //CONST_DATE
            sw.WriteLine(CONST_DATE + this.Date);
            //CONST_TIME_START
            sw.WriteLine(CONST_TIME_START + this.TimeStart);
            //CONST_TIME_END
            sw.WriteLine(CONST_TIME_END + this.TimeEnd);
            //CONST_USED_TIME
            sw.WriteLine(CONST_USED_TIME + this.UsedTime);
            //CONST_TESTER_NUMBER
            sw.WriteLine(CONST_TESTER_NUMBER + this.TesterNumber);
            //CONST_CUSTOMER
            sw.WriteLine(CONST_CUSTOMER + this.Customer);
            //CONST_PRODUCT
            sw.WriteLine(CONST_PRODUCT + this.Product);
            //CONST_USER
            sw.WriteLine(CONST_USER + this.User);
            //CONST_TRAY_ID
            sw.WriteLine(CONST_TRAY_ID + this.TrayID);
            //CONST_LOTNUMBER
            sw.WriteLine(CONST_LOTNUMBER + this.LotNumber);
            //CONST_DOCCONTROL1 
            sw.WriteLine(CONST_DOCCONTROL1 + this.DocControl1);
            //CONST_DOCCONTROL2 
            sw.WriteLine(CONST_DOCCONTROL2 + this.DocControl2);
            //CONST_SUS
            sw.WriteLine(CONST_SUS + this.Sus);
            //CONST_ASSYLINE = "
            sw.WriteLine(CONST_ASSYLINE + this.AssyLine);

            sw.WriteLine();
            sw.WriteLine();



            sw.WriteLine("[SERIAL]");
            if (bIgnoreDefects)
            {
                sw.WriteLine(CONST_HGAN1 + this.HGAN1);
                sw.WriteLine(CONST_HGAN2 + this.HGAN2);
                sw.WriteLine(CONST_HGAN3 + this.HGAN3);
                sw.WriteLine(CONST_HGAN4 + this.HGAN4);
                sw.WriteLine(CONST_HGAN5 + this.HGAN5);
                sw.WriteLine(CONST_HGAN6 + this.HGAN6);
                sw.WriteLine(CONST_HGAN7 + this.HGAN7);
                sw.WriteLine(CONST_HGAN8 + this.HGAN8);
                sw.WriteLine(CONST_HGAN9 + this.HGAN9);
                sw.WriteLine(CONST_HGAN10 + this.HGAN10);

                sw.WriteLine(CONST_HGAN11 + this.HGAN11);
                sw.WriteLine(CONST_HGAN12 + this.HGAN12);
                sw.WriteLine(CONST_HGAN13 + this.HGAN13);
                sw.WriteLine(CONST_HGAN14 + this.HGAN14);
                sw.WriteLine(CONST_HGAN15 + this.HGAN15);
                sw.WriteLine(CONST_HGAN16 + this.HGAN16);
                sw.WriteLine(CONST_HGAN17 + this.HGAN17);
                sw.WriteLine(CONST_HGAN18 + this.HGAN18);
                sw.WriteLine(CONST_HGAN19 + this.HGAN19);
                sw.WriteLine(CONST_HGAN20 + this.HGAN20);

                sw.WriteLine(CONST_HGAN21 + this.HGAN21);
                sw.WriteLine(CONST_HGAN22 + this.HGAN22);
                sw.WriteLine(CONST_HGAN23 + this.HGAN23);
                sw.WriteLine(CONST_HGAN24 + this.HGAN24);
                sw.WriteLine(CONST_HGAN25 + this.HGAN25);
                sw.WriteLine(CONST_HGAN26 + this.HGAN26);
                sw.WriteLine(CONST_HGAN27 + this.HGAN27);
                sw.WriteLine(CONST_HGAN28 + this.HGAN28);
                sw.WriteLine(CONST_HGAN29 + this.HGAN29);
                sw.WriteLine(CONST_HGAN30 + this.HGAN30);

                sw.WriteLine(CONST_HGAN31 + this.HGAN31);
                sw.WriteLine(CONST_HGAN32 + this.HGAN32);
                sw.WriteLine(CONST_HGAN33 + this.HGAN33);
                sw.WriteLine(CONST_HGAN34 + this.HGAN34);
                sw.WriteLine(CONST_HGAN35 + this.HGAN35);
                sw.WriteLine(CONST_HGAN36 + this.HGAN36);
                sw.WriteLine(CONST_HGAN37 + this.HGAN37);
                sw.WriteLine(CONST_HGAN38 + this.HGAN38);
                sw.WriteLine(CONST_HGAN39 + this.HGAN39);
                sw.WriteLine(CONST_HGAN40 + this.HGAN40);

                sw.WriteLine(CONST_HGAN41 + this.HGAN41);
                sw.WriteLine(CONST_HGAN42 + this.HGAN42);
                sw.WriteLine(CONST_HGAN43 + this.HGAN43);
                sw.WriteLine(CONST_HGAN44 + this.HGAN44);
                sw.WriteLine(CONST_HGAN45 + this.HGAN45);
                sw.WriteLine(CONST_HGAN46 + this.HGAN46);
                sw.WriteLine(CONST_HGAN47 + this.HGAN47);
                sw.WriteLine(CONST_HGAN48 + this.HGAN48);
                sw.WriteLine(CONST_HGAN49 + this.HGAN49);
                sw.WriteLine(CONST_HGAN50 + this.HGAN50);

                sw.WriteLine(CONST_HGAN51 + this.HGAN51);
                sw.WriteLine(CONST_HGAN52 + this.HGAN52);
                sw.WriteLine(CONST_HGAN53 + this.HGAN53);
                sw.WriteLine(CONST_HGAN54 + this.HGAN54);
                sw.WriteLine(CONST_HGAN55 + this.HGAN55);
                sw.WriteLine(CONST_HGAN56 + this.HGAN56);
                sw.WriteLine(CONST_HGAN57 + this.HGAN57);
                sw.WriteLine(CONST_HGAN58 + this.HGAN58);
                sw.WriteLine(CONST_HGAN59 + this.HGAN59);
                sw.WriteLine(CONST_HGAN60 + this.HGAN60);
            }
            else
            {
                sw.WriteLine((this.DefectN1.Length > 0) ? CONST_HGAN1 : CONST_HGAN1 + this.HGAN1);
                sw.WriteLine((this.DefectN2.Length > 0) ? CONST_HGAN2 : CONST_HGAN2 + this.HGAN2);
                sw.WriteLine((this.DefectN3.Length > 0) ? CONST_HGAN3 : CONST_HGAN3 + this.HGAN3);
                sw.WriteLine((this.DefectN4.Length > 0) ? CONST_HGAN4 : CONST_HGAN4 + this.HGAN4);
                sw.WriteLine((this.DefectN5.Length > 0) ? CONST_HGAN5 : CONST_HGAN5 + this.HGAN5);
                sw.WriteLine((this.DefectN6.Length > 0) ? CONST_HGAN6 : CONST_HGAN6 + this.HGAN6);
                sw.WriteLine((this.DefectN7.Length > 0) ? CONST_HGAN7 : CONST_HGAN7 + this.HGAN7);
                sw.WriteLine((this.DefectN8.Length > 0) ? CONST_HGAN8 : CONST_HGAN8 + this.HGAN8);
                sw.WriteLine((this.DefectN9.Length > 0) ? CONST_HGAN9 : CONST_HGAN9 + this.HGAN9);
                sw.WriteLine((this.DefectN10.Length > 0) ? CONST_HGAN10 : CONST_HGAN10 + this.HGAN10);

                sw.WriteLine((this.DefectN11.Length > 0) ? CONST_HGAN11 : CONST_HGAN11 + this.HGAN11);
                sw.WriteLine((this.DefectN12.Length > 0) ? CONST_HGAN12 : CONST_HGAN12 + this.HGAN12);
                sw.WriteLine((this.DefectN13.Length > 0) ? CONST_HGAN13 : CONST_HGAN13 + this.HGAN13);
                sw.WriteLine((this.DefectN14.Length > 0) ? CONST_HGAN14 : CONST_HGAN14 + this.HGAN14);
                sw.WriteLine((this.DefectN15.Length > 0) ? CONST_HGAN15 : CONST_HGAN15 + this.HGAN15);
                sw.WriteLine((this.DefectN16.Length > 0) ? CONST_HGAN16 : CONST_HGAN16 + this.HGAN16);
                sw.WriteLine((this.DefectN17.Length > 0) ? CONST_HGAN17 : CONST_HGAN17 + this.HGAN17);
                sw.WriteLine((this.DefectN18.Length > 0) ? CONST_HGAN18 : CONST_HGAN18 + this.HGAN18);
                sw.WriteLine((this.DefectN19.Length > 0) ? CONST_HGAN19 : CONST_HGAN19 + this.HGAN19);
                sw.WriteLine((this.DefectN20.Length > 0) ? CONST_HGAN20 : CONST_HGAN20 + this.HGAN20);

                sw.WriteLine((this.DefectN21.Length > 0) ? CONST_HGAN21 : CONST_HGAN21 + this.HGAN21);
                sw.WriteLine((this.DefectN22.Length > 0) ? CONST_HGAN22 : CONST_HGAN22 + this.HGAN22);
                sw.WriteLine((this.DefectN23.Length > 0) ? CONST_HGAN23 : CONST_HGAN23 + this.HGAN23);
                sw.WriteLine((this.DefectN24.Length > 0) ? CONST_HGAN24 : CONST_HGAN24 + this.HGAN24);
                sw.WriteLine((this.DefectN25.Length > 0) ? CONST_HGAN25 : CONST_HGAN25 + this.HGAN25);
                sw.WriteLine((this.DefectN26.Length > 0) ? CONST_HGAN26 : CONST_HGAN26 + this.HGAN26);
                sw.WriteLine((this.DefectN27.Length > 0) ? CONST_HGAN27 : CONST_HGAN27 + this.HGAN27);
                sw.WriteLine((this.DefectN28.Length > 0) ? CONST_HGAN28 : CONST_HGAN28 + this.HGAN28);
                sw.WriteLine((this.DefectN29.Length > 0) ? CONST_HGAN29 : CONST_HGAN29 + this.HGAN29);
                sw.WriteLine((this.DefectN30.Length > 0) ? CONST_HGAN30 : CONST_HGAN30 + this.HGAN30);

                sw.WriteLine((this.DefectN31.Length > 0) ? CONST_HGAN31 : CONST_HGAN31 + this.HGAN31);
                sw.WriteLine((this.DefectN32.Length > 0) ? CONST_HGAN32 : CONST_HGAN32 + this.HGAN32);
                sw.WriteLine((this.DefectN33.Length > 0) ? CONST_HGAN33 : CONST_HGAN33 + this.HGAN33);
                sw.WriteLine((this.DefectN34.Length > 0) ? CONST_HGAN34 : CONST_HGAN34 + this.HGAN34);
                sw.WriteLine((this.DefectN35.Length > 0) ? CONST_HGAN35 : CONST_HGAN35 + this.HGAN35);
                sw.WriteLine((this.DefectN36.Length > 0) ? CONST_HGAN36 : CONST_HGAN36 + this.HGAN36);
                sw.WriteLine((this.DefectN37.Length > 0) ? CONST_HGAN37 : CONST_HGAN37 + this.HGAN37);
                sw.WriteLine((this.DefectN38.Length > 0) ? CONST_HGAN38 : CONST_HGAN38 + this.HGAN38);
                sw.WriteLine((this.DefectN39.Length > 0) ? CONST_HGAN39 : CONST_HGAN39 + this.HGAN39);
                sw.WriteLine((this.DefectN40.Length > 0) ? CONST_HGAN40 : CONST_HGAN40 + this.HGAN40);

                sw.WriteLine((this.DefectN41.Length > 0) ? CONST_HGAN41 : CONST_HGAN41 + this.HGAN41);
                sw.WriteLine((this.DefectN42.Length > 0) ? CONST_HGAN42 : CONST_HGAN42 + this.HGAN42);
                sw.WriteLine((this.DefectN43.Length > 0) ? CONST_HGAN43 : CONST_HGAN43 + this.HGAN43);
                sw.WriteLine((this.DefectN44.Length > 0) ? CONST_HGAN44 : CONST_HGAN44 + this.HGAN44);
                sw.WriteLine((this.DefectN45.Length > 0) ? CONST_HGAN45 : CONST_HGAN45 + this.HGAN45);
                sw.WriteLine((this.DefectN46.Length > 0) ? CONST_HGAN46 : CONST_HGAN46 + this.HGAN46);
                sw.WriteLine((this.DefectN47.Length > 0) ? CONST_HGAN47 : CONST_HGAN47 + this.HGAN47);
                sw.WriteLine((this.DefectN48.Length > 0) ? CONST_HGAN48 : CONST_HGAN48 + this.HGAN48);
                sw.WriteLine((this.DefectN49.Length > 0) ? CONST_HGAN49 : CONST_HGAN49 + this.HGAN49);
                sw.WriteLine((this.DefectN50.Length > 0) ? CONST_HGAN50 : CONST_HGAN50 + this.HGAN50);

                sw.WriteLine((this.DefectN51.Length > 0) ? CONST_HGAN51 : CONST_HGAN51 + this.HGAN51);
                sw.WriteLine((this.DefectN52.Length > 0) ? CONST_HGAN52 : CONST_HGAN52 + this.HGAN52);
                sw.WriteLine((this.DefectN53.Length > 0) ? CONST_HGAN53 : CONST_HGAN53 + this.HGAN53);
                sw.WriteLine((this.DefectN54.Length > 0) ? CONST_HGAN54 : CONST_HGAN54 + this.HGAN54);
                sw.WriteLine((this.DefectN55.Length > 0) ? CONST_HGAN55 : CONST_HGAN55 + this.HGAN55);
                sw.WriteLine((this.DefectN56.Length > 0) ? CONST_HGAN56 : CONST_HGAN56 + this.HGAN56);
                sw.WriteLine((this.DefectN57.Length > 0) ? CONST_HGAN57 : CONST_HGAN57 + this.HGAN57);
                sw.WriteLine((this.DefectN58.Length > 0) ? CONST_HGAN58 : CONST_HGAN58 + this.HGAN58);
                sw.WriteLine((this.DefectN59.Length > 0) ? CONST_HGAN59 : CONST_HGAN59 + this.HGAN59);
                sw.WriteLine((this.DefectN60.Length > 0) ? CONST_HGAN60 : CONST_HGAN60 + this.HGAN60);
            }


            sw.Close();
        }


        public void ToAQTrayFile(string strFilename, bool bIgnoreDefects, int nTrayType /*20, 40, 60*/)
        {
            StreamWriter sw = new StreamWriter(strFilename);

            sw.WriteLine("[INFORMATION]");
            //CONST_DATE
            sw.WriteLine(CONST_DATE + this.Date);
            //CONST_TIME_START
            sw.WriteLine(CONST_TIME_START + this.TimeStart);
            //CONST_TIME_END
            sw.WriteLine(CONST_TIME_END + this.TimeEnd);
            //CONST_USED_TIME
            sw.WriteLine(CONST_USED_TIME + this.UsedTime);
            //CONST_TESTER_NUMBER
            sw.WriteLine(CONST_TESTER_NUMBER + this.TesterNumber);
            //CONST_CUSTOMER
            sw.WriteLine(CONST_CUSTOMER + this.Customer);
            //CONST_PRODUCT
            sw.WriteLine(CONST_PRODUCT + this.Product);
            //CONST_USER
            sw.WriteLine(CONST_USER + this.User);
            //CONST_TRAY_ID
            sw.WriteLine(CONST_TRAY_ID + this.TrayID);
            //CONST_LOTNUMBER
            sw.WriteLine(CONST_LOTNUMBER + this.LotNumber);
            //CONST_DOCCONTROL1 
            sw.WriteLine(CONST_DOCCONTROL1 + this.DocControl1);
            //CONST_DOCCONTROL2 
            sw.WriteLine(CONST_DOCCONTROL2 + this.DocControl2);
            //CONST_SUS
            sw.WriteLine(CONST_SUS + this.Sus);
            //CONST_ASSYLINE = "
            sw.WriteLine(CONST_ASSYLINE + this.AssyLine);

            sw.WriteLine();
            sw.WriteLine();



            sw.WriteLine("[SERIAL]");
            if (bIgnoreDefects)
            {
                sw.WriteLine(CONST_HGAN1 + this.HGAN1);
                sw.WriteLine(CONST_HGAN2 + this.HGAN2);
                sw.WriteLine(CONST_HGAN3 + this.HGAN3);
                sw.WriteLine(CONST_HGAN4 + this.HGAN4);
                sw.WriteLine(CONST_HGAN5 + this.HGAN5);
                sw.WriteLine(CONST_HGAN6 + this.HGAN6);
                sw.WriteLine(CONST_HGAN7 + this.HGAN7);
                sw.WriteLine(CONST_HGAN8 + this.HGAN8);
                sw.WriteLine(CONST_HGAN9 + this.HGAN9);
                sw.WriteLine(CONST_HGAN10 + this.HGAN10);

                sw.WriteLine(CONST_HGAN11 + this.HGAN11);
                sw.WriteLine(CONST_HGAN12 + this.HGAN12);
                sw.WriteLine(CONST_HGAN13 + this.HGAN13);
                sw.WriteLine(CONST_HGAN14 + this.HGAN14);
                sw.WriteLine(CONST_HGAN15 + this.HGAN15);
                sw.WriteLine(CONST_HGAN16 + this.HGAN16);
                sw.WriteLine(CONST_HGAN17 + this.HGAN17);
                sw.WriteLine(CONST_HGAN18 + this.HGAN18);
                sw.WriteLine(CONST_HGAN19 + this.HGAN19);
                sw.WriteLine(CONST_HGAN20 + this.HGAN20);

                if (nTrayType > 20)  //for tray40
                {
                    sw.WriteLine(CONST_HGAN21 + this.HGAN21);
                    sw.WriteLine(CONST_HGAN22 + this.HGAN22);
                    sw.WriteLine(CONST_HGAN23 + this.HGAN23);
                    sw.WriteLine(CONST_HGAN24 + this.HGAN24);
                    sw.WriteLine(CONST_HGAN25 + this.HGAN25);
                    sw.WriteLine(CONST_HGAN26 + this.HGAN26);
                    sw.WriteLine(CONST_HGAN27 + this.HGAN27);
                    sw.WriteLine(CONST_HGAN28 + this.HGAN28);
                    sw.WriteLine(CONST_HGAN29 + this.HGAN29);
                    sw.WriteLine(CONST_HGAN30 + this.HGAN30);

                    sw.WriteLine(CONST_HGAN31 + this.HGAN31);
                    sw.WriteLine(CONST_HGAN32 + this.HGAN32);
                    sw.WriteLine(CONST_HGAN33 + this.HGAN33);
                    sw.WriteLine(CONST_HGAN34 + this.HGAN34);
                    sw.WriteLine(CONST_HGAN35 + this.HGAN35);
                    sw.WriteLine(CONST_HGAN36 + this.HGAN36);
                    sw.WriteLine(CONST_HGAN37 + this.HGAN37);
                    sw.WriteLine(CONST_HGAN38 + this.HGAN38);
                    sw.WriteLine(CONST_HGAN39 + this.HGAN39);
                    sw.WriteLine(CONST_HGAN40 + this.HGAN40);
                }

                if (nTrayType > 40)  //for tray60
                {
                    sw.WriteLine(CONST_HGAN41 + this.HGAN41);
                    sw.WriteLine(CONST_HGAN42 + this.HGAN42);
                    sw.WriteLine(CONST_HGAN43 + this.HGAN43);
                    sw.WriteLine(CONST_HGAN44 + this.HGAN44);
                    sw.WriteLine(CONST_HGAN45 + this.HGAN45);
                    sw.WriteLine(CONST_HGAN46 + this.HGAN46);
                    sw.WriteLine(CONST_HGAN47 + this.HGAN47);
                    sw.WriteLine(CONST_HGAN48 + this.HGAN48);
                    sw.WriteLine(CONST_HGAN49 + this.HGAN49);
                    sw.WriteLine(CONST_HGAN50 + this.HGAN50);

                    sw.WriteLine(CONST_HGAN51 + this.HGAN51);
                    sw.WriteLine(CONST_HGAN52 + this.HGAN52);
                    sw.WriteLine(CONST_HGAN53 + this.HGAN53);
                    sw.WriteLine(CONST_HGAN54 + this.HGAN54);
                    sw.WriteLine(CONST_HGAN55 + this.HGAN55);
                    sw.WriteLine(CONST_HGAN56 + this.HGAN56);
                    sw.WriteLine(CONST_HGAN57 + this.HGAN57);
                    sw.WriteLine(CONST_HGAN58 + this.HGAN58);
                    sw.WriteLine(CONST_HGAN59 + this.HGAN59);
                    sw.WriteLine(CONST_HGAN60 + this.HGAN60);
                }
            }
            else
            {
                sw.WriteLine((this.DefectN1.Length > 0) ? CONST_HGAN1 : CONST_HGAN1 + this.HGAN1);
                sw.WriteLine((this.DefectN2.Length > 0) ? CONST_HGAN2 : CONST_HGAN2 + this.HGAN2);
                sw.WriteLine((this.DefectN3.Length > 0) ? CONST_HGAN3 : CONST_HGAN3 + this.HGAN3);
                sw.WriteLine((this.DefectN4.Length > 0) ? CONST_HGAN4 : CONST_HGAN4 + this.HGAN4);
                sw.WriteLine((this.DefectN5.Length > 0) ? CONST_HGAN5 : CONST_HGAN5 + this.HGAN5);
                sw.WriteLine((this.DefectN6.Length > 0) ? CONST_HGAN6 : CONST_HGAN6 + this.HGAN6);
                sw.WriteLine((this.DefectN7.Length > 0) ? CONST_HGAN7 : CONST_HGAN7 + this.HGAN7);
                sw.WriteLine((this.DefectN8.Length > 0) ? CONST_HGAN8 : CONST_HGAN8 + this.HGAN8);
                sw.WriteLine((this.DefectN9.Length > 0) ? CONST_HGAN9 : CONST_HGAN9 + this.HGAN9);
                sw.WriteLine((this.DefectN10.Length > 0) ? CONST_HGAN10 : CONST_HGAN10 + this.HGAN10);

                sw.WriteLine((this.DefectN11.Length > 0) ? CONST_HGAN11 : CONST_HGAN11 + this.HGAN11);
                sw.WriteLine((this.DefectN12.Length > 0) ? CONST_HGAN12 : CONST_HGAN12 + this.HGAN12);
                sw.WriteLine((this.DefectN13.Length > 0) ? CONST_HGAN13 : CONST_HGAN13 + this.HGAN13);
                sw.WriteLine((this.DefectN14.Length > 0) ? CONST_HGAN14 : CONST_HGAN14 + this.HGAN14);
                sw.WriteLine((this.DefectN15.Length > 0) ? CONST_HGAN15 : CONST_HGAN15 + this.HGAN15);
                sw.WriteLine((this.DefectN16.Length > 0) ? CONST_HGAN16 : CONST_HGAN16 + this.HGAN16);
                sw.WriteLine((this.DefectN17.Length > 0) ? CONST_HGAN17 : CONST_HGAN17 + this.HGAN17);
                sw.WriteLine((this.DefectN18.Length > 0) ? CONST_HGAN18 : CONST_HGAN18 + this.HGAN18);
                sw.WriteLine((this.DefectN19.Length > 0) ? CONST_HGAN19 : CONST_HGAN19 + this.HGAN19);
                sw.WriteLine((this.DefectN20.Length > 0) ? CONST_HGAN20 : CONST_HGAN20 + this.HGAN20);

                if(nTrayType > 20)  //for tray40
                {
                    sw.WriteLine((this.DefectN21.Length > 0) ? CONST_HGAN21 : CONST_HGAN21 + this.HGAN21);
                    sw.WriteLine((this.DefectN22.Length > 0) ? CONST_HGAN22 : CONST_HGAN22 + this.HGAN22);
                    sw.WriteLine((this.DefectN23.Length > 0) ? CONST_HGAN23 : CONST_HGAN23 + this.HGAN23);
                    sw.WriteLine((this.DefectN24.Length > 0) ? CONST_HGAN24 : CONST_HGAN24 + this.HGAN24);
                    sw.WriteLine((this.DefectN25.Length > 0) ? CONST_HGAN25 : CONST_HGAN25 + this.HGAN25);
                    sw.WriteLine((this.DefectN26.Length > 0) ? CONST_HGAN26 : CONST_HGAN26 + this.HGAN26);
                    sw.WriteLine((this.DefectN27.Length > 0) ? CONST_HGAN27 : CONST_HGAN27 + this.HGAN27);
                    sw.WriteLine((this.DefectN28.Length > 0) ? CONST_HGAN28 : CONST_HGAN28 + this.HGAN28);
                    sw.WriteLine((this.DefectN29.Length > 0) ? CONST_HGAN29 : CONST_HGAN29 + this.HGAN29);
                    sw.WriteLine((this.DefectN30.Length > 0) ? CONST_HGAN30 : CONST_HGAN30 + this.HGAN30);

                    sw.WriteLine((this.DefectN31.Length > 0) ? CONST_HGAN31 : CONST_HGAN31 + this.HGAN31);
                    sw.WriteLine((this.DefectN32.Length > 0) ? CONST_HGAN32 : CONST_HGAN32 + this.HGAN32);
                    sw.WriteLine((this.DefectN33.Length > 0) ? CONST_HGAN33 : CONST_HGAN33 + this.HGAN33);
                    sw.WriteLine((this.DefectN34.Length > 0) ? CONST_HGAN34 : CONST_HGAN34 + this.HGAN34);
                    sw.WriteLine((this.DefectN35.Length > 0) ? CONST_HGAN35 : CONST_HGAN35 + this.HGAN35);
                    sw.WriteLine((this.DefectN36.Length > 0) ? CONST_HGAN36 : CONST_HGAN36 + this.HGAN36);
                    sw.WriteLine((this.DefectN37.Length > 0) ? CONST_HGAN37 : CONST_HGAN37 + this.HGAN37);
                    sw.WriteLine((this.DefectN38.Length > 0) ? CONST_HGAN38 : CONST_HGAN38 + this.HGAN38);
                    sw.WriteLine((this.DefectN39.Length > 0) ? CONST_HGAN39 : CONST_HGAN39 + this.HGAN39);
                    sw.WriteLine((this.DefectN40.Length > 0) ? CONST_HGAN40 : CONST_HGAN40 + this.HGAN40);
                }

                if(nTrayType > 40)  //for tray60
                {
                    sw.WriteLine((this.DefectN41.Length > 0) ? CONST_HGAN41 : CONST_HGAN41 + this.HGAN41);
                    sw.WriteLine((this.DefectN42.Length > 0) ? CONST_HGAN42 : CONST_HGAN42 + this.HGAN42);
                    sw.WriteLine((this.DefectN43.Length > 0) ? CONST_HGAN43 : CONST_HGAN43 + this.HGAN43);
                    sw.WriteLine((this.DefectN44.Length > 0) ? CONST_HGAN44 : CONST_HGAN44 + this.HGAN44);
                    sw.WriteLine((this.DefectN45.Length > 0) ? CONST_HGAN45 : CONST_HGAN45 + this.HGAN45);
                    sw.WriteLine((this.DefectN46.Length > 0) ? CONST_HGAN46 : CONST_HGAN46 + this.HGAN46);
                    sw.WriteLine((this.DefectN47.Length > 0) ? CONST_HGAN47 : CONST_HGAN47 + this.HGAN47);
                    sw.WriteLine((this.DefectN48.Length > 0) ? CONST_HGAN48 : CONST_HGAN48 + this.HGAN48);
                    sw.WriteLine((this.DefectN49.Length > 0) ? CONST_HGAN49 : CONST_HGAN49 + this.HGAN49);
                    sw.WriteLine((this.DefectN50.Length > 0) ? CONST_HGAN50 : CONST_HGAN50 + this.HGAN50);

                    sw.WriteLine((this.DefectN51.Length > 0) ? CONST_HGAN51 : CONST_HGAN51 + this.HGAN51);
                    sw.WriteLine((this.DefectN52.Length > 0) ? CONST_HGAN52 : CONST_HGAN52 + this.HGAN52);
                    sw.WriteLine((this.DefectN53.Length > 0) ? CONST_HGAN53 : CONST_HGAN53 + this.HGAN53);
                    sw.WriteLine((this.DefectN54.Length > 0) ? CONST_HGAN54 : CONST_HGAN54 + this.HGAN54);
                    sw.WriteLine((this.DefectN55.Length > 0) ? CONST_HGAN55 : CONST_HGAN55 + this.HGAN55);
                    sw.WriteLine((this.DefectN56.Length > 0) ? CONST_HGAN56 : CONST_HGAN56 + this.HGAN56);
                    sw.WriteLine((this.DefectN57.Length > 0) ? CONST_HGAN57 : CONST_HGAN57 + this.HGAN57);
                    sw.WriteLine((this.DefectN58.Length > 0) ? CONST_HGAN58 : CONST_HGAN58 + this.HGAN58);
                    sw.WriteLine((this.DefectN59.Length > 0) ? CONST_HGAN59 : CONST_HGAN59 + this.HGAN59);
                    sw.WriteLine((this.DefectN60.Length > 0) ? CONST_HGAN60 : CONST_HGAN60 + this.HGAN60);
                }
            }


            sw.Close();
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


        public static AQTrayObj ToAQTrayObj(string strXML)
        {
            AQTrayObj aTray;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(AQTrayObj));
            using (StringReader reader = new StringReader(strXML))
            {
                aTray = (AQTrayObj)x.Deserialize(reader);
            }

            return aTray;
        }
    }


    // ////////////////////////////////////////////////////////////////////////
    public class AQCruncherConfig
    {
        //private string _strConfigFile = string.Empty;
        //public string ConfigFile
        //{
        //    get { return _strConfigFile; }
        //    set { _strConfigFile = value; }
        //}

        private string _strIAVIFilePath = string.Empty;
        public string IAVIFilePath
        {
            get { return _strIAVIFilePath; }
            set { _strIAVIFilePath = value; }
        }

        private string _strUNLOADFilePath = string.Empty;
        public string UNLOADFilePath
        {
            get { return _strUNLOADFilePath; }
            set { _strUNLOADFilePath = value; }
        }

        private string _strVMIFilePath = string.Empty;
        public string VMIFilePath
        {
            get { return _strVMIFilePath; }
            set { _strVMIFilePath = value; }
        }

        private string _strHOSTFilePath = string.Empty;
        public string HOSTFilePath
        {
            get { return _strHOSTFilePath; }
            set { _strHOSTFilePath = value; }
        }

        private string _strAUTOOCRFilePath = string.Empty;
        public string AUTOOCRFilePath
        {
            get { return _strAUTOOCRFilePath; }
            set { _strAUTOOCRFilePath = value; }
        }

        private string _strProduct = string.Empty;
        public string Product
        {
            get { return _strProduct; }
            set { _strProduct = value; }
        }

        private string _strAfterCrunchPath = string.Empty;
        public string AfterCrunchPath
        {
            get { return _strAfterCrunchPath; }
            set { _strAfterCrunchPath = value; }
        }


        #region ctor
        public AQCruncherConfig()
        {
        }

        #endregion

        // ////////////////////////////////////////////////////////////////////////
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

        public static AQCruncherConfig ReadConfig(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return new AQCruncherConfig();
            }

            AQCruncherConfig config;
            string strXML = System.IO.File.ReadAllText(strFilePath);

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(AQCruncherConfig));
            using (StringReader reader = new StringReader(strXML))
            {
                config = (AQCruncherConfig)x.Deserialize(reader);
            }

            return config;
        }
    }


    // ////////////////////////////////////////////////////////////////////////
    #region class LoggerClass

    public class LoggerClass
    {
        private readonly log4net.ILog MainLogger;
        private readonly log4net.ILog ErrorLogger;
        private readonly log4net.ILog TrayLogger;

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
            TrayLogger = log4net.LogManager.GetLogger("TrayLog");

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.FileInfo logconfig = new System.IO.FileInfo(exePath + @"\logger.config");
            log4net.Config.XmlConfigurator.Configure(logconfig);
        }

        public void MainLogInfo(string strLogMessage)
        {
            this.MainLogger.Info(strLogMessage);
        }

        public void ErrorLogInfo(string strLogMessage)
        {
            this.ErrorLogger.Info(getStackInfo() + ": " + strLogMessage);
        }

        public void TrayLogInfo(string strLogMesssage)
        {
            this.TrayLogger.Info(getStackInfo() + ": " + strLogMesssage);
        }


        private string getStackInfo()
        {
            StackTrace stackTrace = new StackTrace(0, true);                        // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();                      // get method calls (frames)
            string callerfilename = "[" + stackFrames[2].GetFileName() + "]";
            string caller = "[" + stackFrames[2].GetMethod().Name + ":line " + stackFrames[2].GetFileLineNumber().ToString() + "]:";
            caller += "[" + stackFrames[1].GetMethod().Name + ":line " + stackFrames[1].GetFileLineNumber().ToString() + "]:";

            return callerfilename + caller;
        }
    }

    #endregion

}

// ////////////////////////////////////////////////////////////////////////////////////
