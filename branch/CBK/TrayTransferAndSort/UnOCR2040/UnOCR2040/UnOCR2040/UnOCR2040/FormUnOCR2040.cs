using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;

using System.Threading;
using System.Runtime.InteropServices;
using System.Xml;
using System.Diagnostics;

using log4net;
using CBKTransferCommLib;

namespace UnOCR2040
{
    public enum UnOCRState
    {
        UNKNOWN = 0,
        INIT = 1,
        HOMING = 2,
        SETUP = 3,
        READY = 4,
        IDLE = 5,
        STOP = 6,
        AUTORUN = 7
    }

    public partial class FormUnOCR2040 : Form
    {
        #region external mciw32VB apis
        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciErrCheckEnable(int boardNum, string errType, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciErrClear(int boardNum, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciErrFlagsGet(int boardNum, int nAxes, ref int axis, ref int errFlag);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciControllerInit(int boardNum, string iniFile, int logFlag);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciBlockingMode(int boardNum, int logFlag);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciControllerStart(int boardNum);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciControllerStop(int boardNum);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciReadFloat(int boardNum, string Variable, int N, ref float Data);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciVectorMoveAbs(int boardNum, int nAxes, double dest, int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciVectorMoveRel(int boardNum, int nAxes, double delta, int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciMoveAbs(int boardNum, int nAxes, ref double dest, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciMoveRel(int boardNum, int nAxes, ref double delta, ref int axis);

        //int mciMotionDoneWaitAll(int boardNum, int nAxes, int axis[], int wait);
        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciMotionDoneWaitAll(int boardNum, int nAxes, ref int axis, int millisecWait);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciPathStop(int boardNum, int nAxes, ref int axis, int flag);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciPathUnstop(int boardNum, int nAxes, ref int axis);


        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciAmpEnable(int boardNum, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciAmpEnableRS(int boardNum, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciAmpDisable(int boardNum, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciHomeToSwitch(int boardNum, int nAxes, ref int axis, ref int dirn, ref int homeFlags);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciHomeToIndex(int boardNum, int nAxes, ref int axis, ref int dirn, ref int homeFlags);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciHomeAbort(int boardNum);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciHomeAbortN(int boardNum, int nAxes, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciHomeDoneWaitAll(int boardNum, int nAxes, ref int axis, int wait);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciVectorSpeedSet(int boardNum, double speed);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciVectorAccelSet(int boardNum, double accel);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciVectorDecelSet(int boardNum, double decel);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciSpeedSet(int boardNum, int nAxes, ref double speed, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciAccelSet(int boardNum, int nAxes, ref double accel, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciDecelSet(int boardNum, int nAxes, ref double decel, ref int axis);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciDigitalWaitAll(int boardNum, int nAxes, ref int IOline, int wait);

        [DllImport(@"C:\Program Files\PMDI\MCIW32\lib\mciw32VB.dll")]
        public static extern int mciDigitalClear(int boardNum, int nAxes, ref int IOline);



        #endregion

        #region external Pci-Dask apis

        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int Register_Card(int cardType, int card_num);

        //Declare Function Release_Card Lib "Pci-Dask.dll" (ByVal CardNumber As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int Release_Card(int CardNumber);


        //Declare Function DO_WritePort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_WritePort(int CardNumber, int Port, int Value);

        //Declare Function DO_WriteLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, ByVal Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_WriteLine(int CardNumber, int Port, int Line, int Value);

        //Declare Function DO_ReadPort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_ReadPort(int CardNumber, int Port, ref int Value);

        //Declare Function DO_ReadLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_ReadLine(int CardNumber, int Port, int Line, int Value);


        //Declare Function DI_ReadPort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DI_ReadPort(int CardNumber, int Port, ref int Value);

        //Declare Function DI_ReadLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DI_ReadLine(int CardNumber, int Port, int Line, int Value);



        #endregion

        public readonly log4net.ILog MainLogger;
        public readonly log4net.ILog SetupLogger;
        public readonly log4net.ILog DowntimeLogger;

        private CBKTransferCommLibClass _cbkClient;

        #region singleton FormUnOCR2040
        private static FormUnOCR2040 _instance;
        public static FormUnOCR2040 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormUnOCR2040();
                }

                return _instance;
            }
        }

        #endregion

        #region ctor
        private FormUnOCR2040()
        {
            InitializeComponent();

            MainLogger = log4net.LogManager.GetLogger("MainUnOCR");
            SetupLogger = log4net.LogManager.GetLogger("Setup");
            DowntimeLogger = log4net.LogManager.GetLogger("Downtime");

            string exePath = System.Windows.Forms.Application.StartupPath;
            System.IO.FileInfo configFile = new System.IO.FileInfo(exePath + @"\UnOCR2040.config");

            log4net.Config.XmlConfigurator.Configure(configFile);
            MainLogger.Info("FormUnOCR2040 ctor");

            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = false;
            
            imgLoadGreen.Visible = false;
            imgLoadRed.Visible = true;

            LoadUnOCRConfig();
            txtMachineNo.Text = _strMachineNumber;
            txtLine.Text = _strLine;
            txtConfigFile.Text = _strPBAConfigFilename;

            LoadPBAConfig(_strPBAConfigFilename);
            txtProduct.Text = _strPBAConfigProduct;
            txtPartNumber.Text = _strPBAConfigPartNumber;
            _init();


            _cbkClient = new CBKTransferCommLibClass();
            //ListViewItem msgItem = new ListViewItem();
            //msgItem.Text = _cbkClient.Subscribe();
            //DelegateUpdateReplyList(msgItem);


            lstPalletPick = new List<int>();
            lstPalletPick.Clear();
            lstTrayPlace = new List<int>();
            lstTrayPlace.Clear();

            if (_bHardware)
            {
                IOs.Instance.PBAOn();

                //string iniFile = @"D:\Prasert\UnOCR2040\UNOCR2040.ini";
                string iniFile = exePath + "\\UNOCR2040.ini";
                
                int nRet = -1;
                nRet = mciControllerInit(0, iniFile, 1);
                Console.WriteLine("mciControllerInit: " + nRet.ToString());
                MainLogger.Info("mciControllerInit: " + nRet.ToString());

                nRet = mciControllerStart(0);
                Console.WriteLine("mciControllerStart: " + nRet.ToString());
                MainLogger.Info("mciControllerStart: " + nRet.ToString());


                string[] errType = new string[1];
                errType[0] = "following";

                nRet = mciErrCheckEnable(boardNum, errType[0], 2, ref _axis[0]);
                Console.WriteLine("mciErrCheckEnable: " + nRet.ToString());
                MainLogger.Info("mciErrCheckEnable: " + nRet.ToString());


                nRet = mciAmpEnable(0, 2, ref _axis[0]);
                Console.WriteLine("mciAmpEnableRS: " + nRet.ToString());
                MainLogger.Info("mciAmpEnableRS: " + nRet.ToString());

                int[] errFlag = new int[2];
                nRet = mciErrFlagsGet(boardNum, 2, ref _axis[0], ref errFlag[0]);
                if ((errFlag[0] == 32) || (errFlag[1] == 32))
                {
                    nRet = mciErrClear(boardNum, 2, ref _axis[0]);
                    Console.WriteLine("errFlag==32; mciErrClear: " + nRet.ToString());
                    MainLogger.Info("errFlag==32; mciErrClear: " + nRet.ToString());
                }


                nRet = mciAmpEnableRS(0, 2, ref _axis[0]);
                Console.WriteLine("mciAmpEnableRS: " + nRet.ToString());
                MainLogger.Info("mciAmpEnableRS: " + nRet.ToString());

            }

            ThreadStart start = delegate
            {
                CheckPosition();
            };

            motorpositionThread = new Thread(start);
            motorpositionThread.Name = "motorpositionThread";
            motorpositionThread.Priority = ThreadPriority.Normal;
            motorpositionThread.Start();


            //ThreadStart updateGUIStart = delegate
            //{
            //    updateGUIThreadProc();
            //};
            //updateGUIThread = new Thread(updateGUIStart);
            //updateGUIThread.Name = "updateGUIThread";
            //updateGUIThread.Priority = ThreadPriority.Normal;
            //updateGUIThread.Start();

            swStartUnOCR.Start();
            if (swUpTime.IsStopCount())
            {
                swUpTime.Start();
            }
        }

        private delegate void updateReplyList(ListViewItem lstMessage);
        private void UpdateReplyList(ListViewItem lstMessage)
        {
            listview_Reply.Items.Insert(0, lstMessage);
            listview_Reply.Items[0].Selected = true;

            //maintain only top 20 latest messages
            if (listview_Reply.Items.Count > 30)
            {
                listview_Reply.Items.Remove(listview_Reply.Items[30]);
            }

            listview_Reply.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listview_Reply.Refresh();
        }
        public void DelegateUpdateReplyList(ListViewItem lstMessage)
        {
            Invoke(new updateReplyList(UpdateReplyList), lstMessage);
        }


        private Thread updateGUIThread = null;
        private void updateGUIThreadProc()        
        {
            while (true)
            {
                FormUnOCR2040.Instance.DelegateUpdateGUI();

                Application.DoEvents();
                Thread.Sleep(10);
            }
        }



        ~FormUnOCR2040()
        {
            MainLogger.Info("FormUnOCR2040 destructor");
        }

        #endregion


        private Thread motorpositionThread = null;
        private Thread unocrAutoRunThread = null;

        private delegate void updateMachineState();
        private void UpdateMachineState()
        {
            this.tsRunningStatus.Text = _machineState.ToString();
        }
        public void DelegateUpdateMachineState()
        {
            Invoke(new updateMachineState(UpdateMachineState));
        }


        private UnOCRStopWatch swStartUnOCR = new UnOCRStopWatch();
        private UnOCRStopWatch swUpTime = new UnOCRStopWatch();
        private UnOCRStopWatch swDownTime = new UnOCRStopWatch();

        private delegate void updateRuntime();
        private void UpdateRuntime()
        {
            txtStartTime.Text = new DateTime(swStartUnOCR.Ticks()).ToString(Math.Floor(swStartUnOCR.TotalDays()).ToString() + ":HH:mm:ss");
            
            //txtUpTime.Text = new DateTime(swUpTime.CummTicks()).ToString("HH:mm:ss.fff");
            txtUpTime.Text = new DateTime(swUpTime.CummTicks()).ToString(Math.Floor(swUpTime.TotalDays()).ToString() + ":HH:mm:ss.fff");
            
            //txtDownTime.Text = new DateTime(swDownTime.CummTicks()).ToString("HH:mm:ss.fff");
            txtDownTime.Text = new DateTime(swDownTime.CummTicks()).ToString(Math.Floor(swDownTime.TotalDays()).ToString() + ":HH:mm:ss.fff");

            double dblUPH = 0.0;
            try
            {
                dblUPH = _lOutput / swUpTime.TotalHours();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                dblUPH = 0.0;

            }

            txtUPH.Text = Math.Floor(dblUPH).ToString();
            txtOutput.Text = _lOutput.ToString();
        }
        public void DelegateUpdateRuntime()
        {
            Invoke(new updateRuntime(UpdateRuntime));
        }



        public static void CheckPosition()
        {
            float[] pos = new float[2];
            pos[0] = 0;
            pos[1] = 0;

            while (true)
            {
                if(FormUnOCR2040.Instance._bHardware)
                {
                    int nRet = mciReadFloat(0, "pos", 3, ref pos[0]);
                    Console.WriteLine("mciReadFloat: " + nRet.ToString());

                    FormUnOCR2040.Instance.DelegateUpdateXPos(pos[0]);
                    FormUnOCR2040.Instance.DelegateUpdateYPos(pos[1]);

                    FormUnOCR2040.Instance.DelegateUpdateMachineState();
                    FormUnOCR2040.Instance.DelegateUpdateRuntime();
                }

                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

        #region delegate update position X
        private delegate void updateXPos(float nPos);
        private void UpdateXPos(float nPos)
        {
            float dblAdjust = (float)_dblX + nPos;
            //this.txtboxReadonlyX.Text = nPos.ToString();
            this.tsValueXPos.Text = nPos.ToString();
            //this.txtboxReadonlyX.Text = dblAdjust.ToString();
            Console.WriteLine("nPos: " + nPos.ToString());
            Console.WriteLine("_dblX: " + _dblX.ToString());
            //Console.WriteLine("XPos: " + (nPos - (float)_dblX).ToString());
        }
        public void DelegateUpdateXPos(float nPos)
        {
            Invoke(new updateXPos(UpdateXPos), nPos);
        }
        #endregion

        #region delegate update position Y
        private delegate void updateYPos(float nPos);
        private void UpdateYPos(float nPos)
        {
            float dblAdjust = (float)_dblY + nPos;
            //this.txtboxReadonlyY.Text = nPos.ToString();
            this.tsValueYPos.Text = nPos.ToString();
            //this.txtboxReadonlyY.Text = dblAdjust.ToString();
            Console.WriteLine("nPos: " + nPos.ToString());
            Console.WriteLine("_dblY: " + _dblY.ToString());
            //Console.WriteLine("YPos: " + (nPos - (float)_dblY).ToString());
        }
        public void DelegateUpdateYPos(float nPos)
        {
            Invoke(new updateYPos(UpdateYPos), nPos);
        }
        #endregion

        public string strAxisXPos
        {
            get
            {
                return tsValueXPos.Text;
            }
        }

        public string strAxisYPos
        {
            get
            {
                return tsValueYPos.Text;
            }
        }


        public UnOCRState _machineState = UnOCRState.UNKNOWN;

        public XmlDocument _pbaXmlConfigDoc = new XmlDocument();
        public XmlElement _rootPBAXmlConfigElem = null;

        public XmlElement _pbaSpeedElem = null;
        public XmlElement _pbaAccElem = null;
        public XmlElement _pbaDeclElem = null;
        public XmlElement _pbaStepElem = null;

        private double _dblSpeed = 0.0;
        private double _dblAcc = 0.0;
        private double _dblDecl = 0.0;

        public PBACoordinate _wdtrayCoord11 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord12 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord13 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord14 = new PBACoordinate();

        public PBACoordinate _wdtrayCoord21 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord22 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord23 = new PBACoordinate();
        public PBACoordinate _wdtrayCoord24 = new PBACoordinate();


        public PBACoordinate _hgsttrayCoord1 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord2 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord3 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord4 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord5 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord6 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord7 = new PBACoordinate();
        public PBACoordinate _hgsttrayCoord8 = new PBACoordinate();

        public string _strMachineNumber = "";
        public string _strLine = "";
        public string _strPBAConfigFilename = "";
        public bool _bHardware = true;
        public int _nSimulateWaitTray = 1;
        public void LoadUnOCRConfig()
        {
            XmlDocument unocrConfigDoc = new XmlDocument();
            try
            {
                string configPath = System.Windows.Forms.Application.StartupPath;
                unocrConfigDoc.Load(configPath + @"\UnOCR.xml");

                XmlNodeList unocrMachineNodeList = unocrConfigDoc.SelectNodes("/UnOCR2040");
                if (unocrMachineNodeList.Count > 0)
                {
                    foreach (XmlNode node in unocrMachineNodeList)
                    {
                        XmlElement elem = (XmlElement)node;
                        _strMachineNumber = node.Attributes["machinenumber"].Value;
                        Console.WriteLine(_strMachineNumber);
                        _strLine = node.Attributes["line"].Value;
                        Console.WriteLine(_strLine);
                    }
                }

                XmlNodeList unocrPBAConfigFileNodeList = unocrConfigDoc.SelectNodes("/UnOCR2040/PBAConfig");
                if (unocrPBAConfigFileNodeList.Count > 0)
                {
                    foreach (XmlNode node in unocrPBAConfigFileNodeList)
                    {
                        _strPBAConfigFilename = node.InnerText;
                    }
                }

                XmlNodeList unocrHardwareNodeList = unocrConfigDoc.SelectNodes("/UnOCR2040/Hardware");
                if (unocrHardwareNodeList.Count > 0)
                {
                    foreach (XmlNode node in unocrHardwareNodeList)
                    {
                        _bHardware = Convert.ToBoolean(node.InnerText);
                    }
                }  
            }
            catch
            {
            }
        }

        public void SaveUnOCRConfig()
        {
            try
            {
                XmlDocument unocrConfigXmlDoc = new XmlDocument();

                XmlElement rootConfigXmlElem = unocrConfigXmlDoc.CreateElement("UnOCR2040");
                unocrConfigXmlDoc.AppendChild(rootConfigXmlElem);
                rootConfigXmlElem = unocrConfigXmlDoc.DocumentElement;

                rootConfigXmlElem.SetAttribute("machinenumber", _strMachineNumber);
                rootConfigXmlElem.SetAttribute("line", _strLine);

                XmlElement enableSortXmlElem = unocrConfigXmlDoc.CreateElement("EnableSort");
                rootConfigXmlElem.AppendChild(enableSortXmlElem);
                bool bEnableSort = false;
                enableSortXmlElem.InnerText = bEnableSort.ToString();

                XmlElement pbaConfigXmlElem = unocrConfigXmlDoc.CreateElement("PBAConfig");
                rootConfigXmlElem.AppendChild(pbaConfigXmlElem);
                pbaConfigXmlElem.InnerText = _strPBAConfigFilename;

                XmlElement hardwareConfigXmlElem = unocrConfigXmlDoc.CreateElement("Hardware");
                rootConfigXmlElem.AppendChild(hardwareConfigXmlElem);
                hardwareConfigXmlElem.InnerText = _bHardware.ToString();


                string configPath = System.Windows.Forms.Application.StartupPath;
                unocrConfigXmlDoc.Save(configPath + @"\UnOCR.xml");
                return;
            }
            catch
            {
            }
        }


        public void LoadPBAConfig(string strXMLFile)
        {
            try
            {
                MainLogger.Info("LoadPBAConfig " + strXMLFile);

                _pbaXmlConfigDoc.Load(strXMLFile);
                _rootPBAXmlConfigElem = _pbaXmlConfigDoc.DocumentElement;

                XmlNodeList pbaProductList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA");
                if (pbaProductList.Count > 0)
                {
                    foreach (XmlNode node in pbaProductList)
                    {
                        XmlElement elem = (XmlElement)node;
                        _strPBAConfigProduct = node.Attributes["product"].Value;
                        Console.WriteLine(_strPBAConfigProduct);
                        this.txtProduct.Text = _strPBAConfigProduct;

                        _strPBAConfigPartNumber = node.Attributes["partnumber"].Value;
                        Console.WriteLine(_strPBAConfigPartNumber);
                        this.txtPartNumber.Text = _strPBAConfigPartNumber;
                    }
                }


                #region wdtray coordinates
                XmlNodeList wdtrayPosList;

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray1/pos1");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord11.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord11.Y = Convert.ToDouble(node.Attributes["y"].Value);                        
                    }

                    _wdtray1._unloadPos[0] = _wdtrayCoord11;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray1/pos2");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord12.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord12.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray1._unloadPos[1] = _wdtrayCoord12;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray1/pos3");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord13.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord13.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray1._unloadPos[2] = _wdtrayCoord13;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray1/pos4");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord14.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord14.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray1._unloadPos[3] = _wdtrayCoord14;
                }



                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray2/pos1");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord21.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord21.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray2._unloadPos[0] = _wdtrayCoord21;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray2/pos2");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord22.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord22.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray2._unloadPos[1] = _wdtrayCoord22;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray2/pos3");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord23.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord23.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray2._unloadPos[2] = _wdtrayCoord23;
                }

                wdtrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/WDTray2/pos4");
                if (wdtrayPosList.Count > 0)
                {
                    foreach (XmlNode node in wdtrayPosList)
                    {
                        _wdtrayCoord24.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _wdtrayCoord24.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _wdtray2._unloadPos[3] = _wdtrayCoord24;
                }


                #endregion


                #region hgsttray coordinates
                XmlNodeList hgsttrayPosList;

                hgsttrayPosList= _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos1");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord1.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord1.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[0] = _hgsttrayCoord1;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos2");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord2.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord2.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[1] = _hgsttrayCoord2;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos3");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord3.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord3.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[2] = _hgsttrayCoord3;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos4");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord4.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord4.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[3] = _hgsttrayCoord4;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos5");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord5.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord5.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[4] = _hgsttrayCoord5;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos6");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord6.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord6.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[5] = _hgsttrayCoord6;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos7");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord7.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord7.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[6] = _hgsttrayCoord7;
                }

                hgsttrayPosList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/HGSTTray/pos8");
                if (hgsttrayPosList.Count > 0)
                {
                    foreach (XmlNode node in hgsttrayPosList)
                    {
                        _hgsttrayCoord8.X = Convert.ToDouble(node.Attributes["x"].Value);
                        _hgsttrayCoord8.Y = Convert.ToDouble(node.Attributes["y"].Value);
                    }

                    _hgsttray1._unloadPos[7] = _hgsttrayCoord8;
                }

                #endregion


                #region PBA speed
                XmlNodeList pbaSpeedSettingList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/Speed");
                if (pbaSpeedSettingList.Count > 0)
                {
                    foreach (XmlNode node in pbaSpeedSettingList)
                    {
                        _pbaSpeedElem = (XmlElement)node;
                        _dblSpeed = Convert.ToDouble(_pbaSpeedElem.InnerText);
                    }
                }
                #endregion

                #region PBA acceleration
                XmlNodeList pbaAcclSettingList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/Acceleration");
                if (pbaAcclSettingList.Count > 0)
                {
                    foreach (XmlNode node in pbaAcclSettingList)
                    {
                        _pbaAccElem = (XmlElement)node;
                        _dblAcc = Convert.ToDouble(_pbaAccElem.InnerText);
                    }
                }
                #endregion

                #region PBA deceleration
                XmlNodeList pbaDeclSettingList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/Deceleration");
                if (pbaDeclSettingList.Count > 0)
                {
                    foreach (XmlNode node in pbaDeclSettingList)
                    {
                        _pbaDeclElem = (XmlElement)node;
                        _dblDecl = Convert.ToDouble(_pbaDeclElem.InnerText);
                    }
                }
                #endregion

                #region PBA step
                XmlNodeList pbaStepSettingList = _pbaXmlConfigDoc.SelectNodes("/UnOCR2040/PBA/Step");
                if (pbaStepSettingList.Count > 0)
                {
                    foreach (XmlNode node in pbaStepSettingList)
                    {
                        _pbaStepElem = (XmlElement)node;
                    }
                }
                #endregion

                _strPBAConfigFilename = strXMLFile;
                txtConfigFile.Text = _strPBAConfigFilename;
            }
            catch (Exception ex)
            {
                MainLogger.Info("LoadPBAConfig: exception " + ex.Message);
            }
        }


        private void _testGUI()
        {
            Random rd = new Random(Environment.TickCount);

            imgHGAd01.Visible = !(imgHGA01.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd01.Visible = imgHGA01.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd02.Visible = !(imgHGA02.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd02.Visible = imgHGA02.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd03.Visible = !(imgHGA03.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd03.Visible = imgHGA03.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd04.Visible = !(imgHGA04.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd04.Visible = imgHGA04.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd05.Visible = !(imgHGA05.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd05.Visible = imgHGA05.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd06.Visible = !(imgHGA06.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd06.Visible = imgHGA06.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd07.Visible = !(imgHGA07.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd07.Visible = imgHGA07.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd08.Visible = !(imgHGA08.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd08.Visible = imgHGA08.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd09.Visible = !(imgHGA09.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd09.Visible = imgHGA09.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd10.Visible = !(imgHGA10.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd10.Visible = imgHGA10.Visible = (rd.Next(0, 10) < 5 ? true : false);


            imgHGAd11.Visible = !(imgHGA11.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd11.Visible = imgHGA11.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd12.Visible = !(imgHGA12.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd12.Visible = imgHGA12.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd13.Visible = !(imgHGA13.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd13.Visible = imgHGA13.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd14.Visible = !(imgHGA14.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd14.Visible = imgHGA14.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd15.Visible = !(imgHGA15.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd15.Visible = imgHGA15.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd16.Visible = !(imgHGA16.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd16.Visible = imgHGA16.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd17.Visible = !(imgHGA17.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd17.Visible = imgHGA17.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd18.Visible = !(imgHGA18.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd18.Visible = imgHGA18.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd19.Visible = !(imgHGA19.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd19.Visible = imgHGA19.Visible = (rd.Next(0, 10) < 5 ? true : false);

            imgHGAd20.Visible = !(imgHGA20.Visible = rd.Next(0, 10) < 5 ? true : false);
            imgHGAd20.Visible = imgHGA20.Visible = (rd.Next(0, 10) < 5 ? true : false);


            imgHGST01.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST02.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST03.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST04.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST05.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST06.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST07.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST08.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST09.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST10.Visible = rd.Next(0, 10) < 5 ? true : false;

            imgHGST11.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST12.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST13.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST14.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST15.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST16.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST17.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST18.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST19.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST20.Visible = rd.Next(0, 10) < 5 ? true : false;


            imgHGST21.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST22.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST23.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST24.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST25.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST26.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST27.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST28.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST29.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST30.Visible = rd.Next(0, 10) < 5 ? true : false;

            imgHGST31.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST32.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST33.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST34.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST35.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST36.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST37.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST38.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST39.Visible = rd.Next(0, 10) < 5 ? true : false;
            imgHGST40.Visible = rd.Next(0, 10) < 5 ? true : false;
        }

        private HGAObj[] _simulateHGAData()
        {
            HGAObj[] arrHGA = new HGAObj[20];
            Random rd = new Random(Environment.TickCount);
            for (int i = 0; i < 20; i++)
            {
                arrHGA[i] = new HGAObj();
                arrHGA[i].HasPart = true;
                arrHGA[i].IsDefect = (rd.Next(0, 10) < 5 ? true : false);
            }

            return arrHGA;
        }

        private HGAObj[] getHGAArrayFromAQTrayData(AQTrayClass aqtray)
        {
            HGAObj[] arrHGA = new HGAObj[20];
            for (int i = 0; i < 20; i++)
            {
                arrHGA[i] = new HGAObj();
                arrHGA[i].HasPart = true;
                arrHGA[i].IsDefect = !aqtray._bCBKPassed[i];
            }

            return arrHGA;
        }

        public int boardNum = 0;
        private int[] _axis = new int[2];
        private int[] _io = new int[10];
        private int[] _homeFlag = new int[2];
        
        const int PCI_7432 = 16;
        private int _card_number = 0;
        private int _card = 0;

        private double _dblX = 0.0;
        private double _dblY = 0.0;


        private void _init()
        {
            for(int i = 1; i <= 20; i++)
            {
                //imgHGA01
                Control[] imgBoxs = this.Controls.Find("imgHGA" + i.ToString("00"), true);
                imgBoxs[0].Visible = false;

                Control[] imgBoxsd = this.Controls.Find("imgHGAd" + i.ToString("00"), true);
                imgBoxsd[0].Visible = false;
            }

            for (int i = 1; i <= 40; i++)
            {
                //imgHGST01
                Control[] imgBoxs = this.Controls.Find("imgHGST" + i.ToString("00"), true);
                imgBoxs[0].Visible = false;
            }


            if(_bHardware)
            {
                _card_number = 0;
                _card = 0;
                _card = Register_Card(PCI_7432, _card_number);
            }


            _axis[0] = 0;
            _axis[1] = 1;

            _machineState = UnOCRState.INIT;
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_bHardware)
            {
                int nRet = -1;
                nRet = mciAmpDisable(0, 2, ref _axis[0]);
                nRet = mciControllerStop(0);
            }

            if (motorpositionThread != null)
            {
                motorpositionThread.Abort();
            }

            Application.Exit();
        }

        System.IO.Ports.SerialPort coolMotor = new System.IO.Ports.SerialPort("COM1", 38400, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        private void button1_Click(object sender, EventArgs e)
        {

            //testGUI();
            //LoadPBAConfig(@"D:\Prasert\UnOCR2040_2\UnOCR2 040\UnOCR2040\UnOCR2040\bin\Debug\PBA.xml");
           
            /*
            coolMotor.ReadBufferSize = 1024;
            coolMotor.WriteBufferSize = 512;
            if (!coolMotor.IsOpen)
            {
                coolMotor.Open();
            }

            //string coolSetSpeed = "S=100\r\n";
            byte[] arrByteCoolSetSpeed = new byte[7]{83, 61, 49, 48, 48, 13, 10};
            //coolMotor.WriteLine("S=100");
            coolMotor.Write(arrByteCoolSetSpeed, 0, arrByteCoolSetSpeed.Length);

            //string coolSetAcc = "A=100\r\n";
            byte[] arrByteCoolSetAcc = new byte[7] { 65, 61, 49, 48, 48, 13, 10};
            //coolMotor.WriteLine("A=100");
            coolMotor.Write(arrByteCoolSetAcc, 0, arrByteCoolSetAcc.Length);

            byte[] arrByteCoolHome = new byte[3] {124, 13, 10 };
            //coolMotor.WriteLine("|");
            coolMotor.Write(arrByteCoolHome, 0, arrByteCoolHome.Length);

            return;
            */

            IOs.Instance.PalletClampsDown();

            _wdtray1.LoadPart();

            PBACoordinate tempWDtray = new PBACoordinate();
            PBACoordinate tempHGSTtray = new PBACoordinate();

            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload()/5];
            //this.PBAMoveAbs(_wdtrayCoord1.X, _wdtrayCoord1.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(0);
            _wdtray1.UnloadPart(1);
            _wdtray1.UnloadPart(2);
            _wdtray1.UnloadPart(3);
            _wdtray1.UnloadPart(4);

            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload()/5];
            //this.PBAMoveAbs(_hgsttrayCoord1.X, _hgsttrayCoord1.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(0);
            _hgsttray1.LoadPart(1);
            _hgsttray1.LoadPart(2);
            _hgsttray1.LoadPart(3);
            _hgsttray1.LoadPart(4);



            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord2.X, _wdtrayCoord2.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(5);
            _wdtray1.UnloadPart(6);
            _wdtray1.UnloadPart(7);
            _wdtray1.UnloadPart(8);
            _wdtray1.UnloadPart(9);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord2.X, _hgsttrayCoord2.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(5);
            _hgsttray1.LoadPart(6);
            _hgsttray1.LoadPart(7);
            _hgsttray1.LoadPart(8);
            _hgsttray1.LoadPart(9);


            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord3.X, _wdtrayCoord3.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(10);
            _wdtray1.UnloadPart(11);
            _wdtray1.UnloadPart(12);
            _wdtray1.UnloadPart(13);
            _wdtray1.UnloadPart(14);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord3.X, _hgsttrayCoord3.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(10);
            _hgsttray1.LoadPart(11);
            _hgsttray1.LoadPart(12);
            _hgsttray1.LoadPart(13);
            _hgsttray1.LoadPart(14);


            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord4.X, _wdtrayCoord4.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(15);
            _wdtray1.UnloadPart(16);
            _wdtray1.UnloadPart(17);
            _wdtray1.UnloadPart(18);
            _wdtray1.UnloadPart(19);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord4.X, _hgsttrayCoord4.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(15);
            _hgsttray1.LoadPart(16);
            _hgsttray1.LoadPart(17);
            _hgsttray1.LoadPart(18);
            _hgsttray1.LoadPart(19);



            _wdtray1.UnloadAll();
            _wdtray1.LoadPart();
            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord1.X, _wdtrayCoord1.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(0);
            _wdtray1.UnloadPart(1);
            _wdtray1.UnloadPart(2);
            _wdtray1.UnloadPart(3);
            _wdtray1.UnloadPart(4);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord5.X, _hgsttrayCoord5.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(20);
            _hgsttray1.LoadPart(21);
            _hgsttray1.LoadPart(22);
            _hgsttray1.LoadPart(23);
            _hgsttray1.LoadPart(24);


            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord2.X, _wdtrayCoord2.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(5);
            _wdtray1.UnloadPart(6);
            _wdtray1.UnloadPart(7);
            _wdtray1.UnloadPart(8);
            _wdtray1.UnloadPart(9);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord6.X, _hgsttrayCoord6.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(25);
            _hgsttray1.LoadPart(26);
            _hgsttray1.LoadPart(27);
            _hgsttray1.LoadPart(28);
            _hgsttray1.LoadPart(29);


            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord3.X, _wdtrayCoord3.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(10);
            _wdtray1.UnloadPart(11);
            _wdtray1.UnloadPart(12);
            _wdtray1.UnloadPart(13);
            _wdtray1.UnloadPart(14);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_hgsttrayCoord7.X, _hgsttrayCoord7.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(30);
            _hgsttray1.LoadPart(31);
            _hgsttray1.LoadPart(32);
            _hgsttray1.LoadPart(33);
            _hgsttray1.LoadPart(34);


            tempWDtray = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload() / 5];
            //this.PBAMoveAbs(_wdtrayCoord4.X, _wdtrayCoord4.Y);
            this.PBAMoveAbs(tempWDtray.X, tempWDtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _wdtray1.UnloadPart(15);
            _wdtray1.UnloadPart(16);
            _wdtray1.UnloadPart(17);
            _wdtray1.UnloadPart(18);
            _wdtray1.UnloadPart(19);


            tempHGSTtray = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];
            this.PBAMoveAbs(_hgsttrayCoord8.X, _hgsttrayCoord8.Y);
            this.PBAMoveAbs(tempHGSTtray.X, tempHGSTtray.Y);
            IOs.Instance.NozzleDown();
            Thread.Sleep(1000);
            IOs.Instance.NozzleUp();
            Thread.Sleep(1000);

            _hgsttray1.LoadPart(35);
            _hgsttray1.LoadPart(36);
            _hgsttray1.LoadPart(37);
            _hgsttray1.LoadPart(38);
            _hgsttray1.LoadPart(39);

        }


        public WDTrayObj _wdtray1 = new WDTrayObj();
        public WDTrayObj _wdtray2 = new WDTrayObj();
        public HGSTTrayObj _hgsttray1 = new HGSTTrayObj();
        public string _strPBAConfigProduct = "";
        public string _strPBAConfigPartNumber = "";


        #region delegate updateGUI
        private delegate void updateGUI();
        private void UpdateGUI()
        {
            if (_bHardware)
            {
                if (IOs.Instance.IsAreaSensorOn())
                {
                    if (_bAreaSensorsActivated == false)
                    {
                        _nPrevAutoRunStep = _nAutoRunStep;
                        _nAutoRunStep = -1;
                        lblAreaSensorAlert.Visible = true;
                        _bAreaSensorsActivated = true;
                    }
                }
                else
                {
                    if (_bAreaSensorsActivated == true)
                    {
                        _bAreaSensorsActivated = false;
                        _nAutoRunStep = _nPrevAutoRunStep;
                        lblAreaSensorAlert.Visible = false;
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                //imgHGAd01.Visible = !(imgHGA01.Visible = rd.Next(0, 10) < 5 ? true : false);
                Control[] imgBoxs = this.Controls.Find("imgHGA" + (i + 1).ToString("00"), true);
                Control[] imgBoxsd = this.Controls.Find("imgHGAd" + (i + 1).ToString("00"), true);
                if (_bHardware)
                {
                    if (IOs.Instance.IsTray1In())
                    {
                        //imgBoxs[0].Visible = _wdtray1._hga[i].HasPart;
                        //imgBoxsd[0].Visible = !(imgBoxs[0].Visible = _wdtray1._hga[i].HasPart);

                        if (_wdtray1._hga[i].HasPart)
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = _wdtray1._hga[i].HasPart;
                            imgBoxs[0].Visible = !(imgBoxsd[0].Visible = _wdtray1._hga[i].IsDefect);
                        }
                        else
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = false;
                        }
                    }
                    else
                    {
                        //imgBoxs[0].Visible = _wdtray2._hga[i].HasPart;
                        //imgBoxsd[0].Visible = !(imgBoxs[0].Visible = _wdtray2._hga[i].HasPart);

                        if (_wdtray2._hga[i].HasPart)
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = _wdtray2._hga[i].HasPart;
                            imgBoxs[0].Visible = !(imgBoxsd[0].Visible = _wdtray2._hga[i].IsDefect);
                        }
                        else
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = false;
                        }
                    }
                }
                else
                {
                    if (FormUnOCR2040.Instance._nSimulateWaitTray == 1)
                    {
                        if (_wdtray1._hga[i].HasPart)
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = _wdtray1._hga[i].HasPart;
                            imgBoxs[0].Visible = !(imgBoxsd[0].Visible = _wdtray1._hga[i].IsDefect);
                        }
                        else
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = false;
                        }

                    }
                    else
                    {
                        if (_wdtray2._hga[i].HasPart)
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = _wdtray2._hga[i].HasPart;
                            imgBoxs[0].Visible = !(imgBoxsd[0].Visible = _wdtray2._hga[i].IsDefect);
                        }
                        else
                        {
                            imgBoxsd[0].Visible = imgBoxs[0].Visible = false;
                        }
                    }
                }


            }

            for (int i = 0; i < 40; i++)
            {
                Control[] imgBoxs = this.Controls.Find("imgHGST" + (i + 1).ToString("00"), true);
                imgBoxs[0].Visible = _hgsttray1._hga[i].HasPart;
            }

            if (_machineState == UnOCRState.AUTORUN)
            {
                imgLoadGreen.Visible = true;
                imgLoadRed.Visible = !imgLoadGreen.Visible;
            }
            else
            {
                imgLoadGreen.Visible = false;
                imgLoadRed.Visible = !imgLoadGreen.Visible;
            }


        }
        public void DelegateUpdateGUI()
        {
            Invoke(new updateGUI(UpdateGUI));
        }
        #endregion

        #region delegate enableRun
        private delegate void enableRun();
        private void EnableRun()
        {
            if (this.HomeDone)
            {
                this._machineState = UnOCRState.READY;
                this.setUpToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.setUpToolStripMenuItem.Enabled = false;
            }

            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = this.HomeDone;
        }
        public void DelegateEnableRun()
        {
            Invoke(new enableRun(EnableRun));
        }
        #endregion

        private void iOsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FormIOs frmIOs = new FormIOs();
            //frmIOs.ShowDialog();
            FormIOs.Instance.ShowDialog();
        }

        private void FormUnOCR2040_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainLogger.Info("FormUnOCR2040 closing...");

            if (_bHardware)
            {
                int nRet = -1;
                nRet = mciAmpDisable(0, 2, ref _axis[0]);
                nRet = mciControllerStop(0);

                IOs.Instance.NozzleVacAllOff();
                IOs.Instance.NozzleUp();
            }


            if (motorpositionThread != null)
            {
                motorpositionThread.Abort();
            }

            if (unocrAutoRunThread != null)
            {
                unocrAutoRunThread.Abort();
            }


            this.SaveUnOCRConfig();
            Application.Exit();
        }


        #region Homing
        public static bool _bXHomeDone = false;
        public static bool _bYHomeDone = false;
        public bool HomeDone
        {
            get
            {
                return (_bXHomeDone && _bYHomeDone);
            }
        }

        ThreadStart startHome0 = delegate
        {
            //HomeAxis(0);
            Instance.HomeAxis0();
        };
        Thread threadHome0 = null;

        ThreadStart startHome1 = delegate
        {
            //HomeAxis(1);
            Instance.HomeAxis1();

            int nRet = 0;
            if (Instance.OnHomeYDoneCallBack != null)
            {
                nRet = Instance.OnHomeYDoneCallBack();
            }
            if (nRet == 1)
            {
                //success
                //check if axis X done homing
                if (Instance.OnHomeXDoneCallBack != null)
                {
                    nRet = Instance.OnHomeXDoneCallBack();
                }
                if (nRet == 1)
                {
                    //success
                    if (Instance.HomeDone)
                    {
                        Instance.DelegateEnableRun();

                        if(FormUnOCR2040.Instance._bHardware)
                        {
                            FormUnOCR2040.Instance.PBASetSpeed(FormUnOCR2040.Instance._dblSpeed);
                            FormUnOCR2040.Instance.PBASetAccl(FormUnOCR2040.Instance._dblAcc);
                            FormUnOCR2040.Instance.PBASetDecl(FormUnOCR2040.Instance._dblDecl);

                            PBACoordinate standbyPos = new PBACoordinate(120, 60);
                            FormUnOCR2040.Instance.PBAMoveAbs(standbyPos.X, standbyPos.Y);
                        }
                    }
                }
            }

        };
        Thread threadHome1 = null;


        private void HomeAxis0()        //nAxis = 0, 1
        {
            _bXHomeDone = false;

            this.OnHomeXDoneCallBack += HomeXDoneCallBackFunction;

            if(!this._bHardware)
            {
                Thread.Sleep(1000);
                _bXHomeDone = true;
                return;
            }

            Console.WriteLine("HomeAxis0:");
            int[] axis0 = new int[2];
            axis0[0] = 0;
            int[] io0 = new int[10];
            io0[0] = 34;

            int nRet = -1;
            nRet = mciHomeAbortN(0, 1, ref axis0[0]);
            Console.WriteLine("HomeAxis0 mciHomeAbortN: " + nRet.ToString());

            nRet = mciErrClear(0, 1, ref axis0[0]);
            Console.WriteLine("HomeAxis0 mciErrClear: " + nRet.ToString());

            nRet = mciDigitalWaitAll(0, 1, ref io0[0], 0);
            Console.WriteLine("HomeAxis0 mciDigitalWaitAll: " + nRet.ToString());

            int[] dir0 = new int[2];
            int[] homeFlag0 = new int[2];
            if (nRet == 1)
            {
                dir0[0] = 1;
                homeFlag0[0] = 3;

                int nHomeRet = 0;

                nRet = mciHomeToSwitch(0, 1, ref axis0[0], ref dir0[0], ref homeFlag0[0]);
                //nRet = mciHomeToIndex(boardNum, 1, ref axis[0], ref dirn[0], ref homeFlag[0]);
                Console.WriteLine("HomeAxis0 mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis0 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }

                //1 = home done on all axises
                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis0[0], 0);
                    Console.WriteLine("HomeAxis0 nHomeRet: " + nHomeRet.ToString());
                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        if (nHomeRet == 1) //home done
                        {
                            float[] pos = new float[2];
                            pos[0] = 0;
                            pos[1] = 0;

                            int nRetRead = mciReadFloat(0, "pos", 3, ref pos[0]);
                            //Console.WriteLine("mciReadFloat: " + nRet.ToString());
                            //Form1.Instance.DelegateUpdateXPos(pos[0]);
                            //Form1.Instance.DelegateUpdateYPos(pos[1]);
                            if (pos[0] < 0)
                            {
                                _dblX = -pos[0];
                            }
                            else
                            {
                                _dblX = pos[0];
                            }

                            _bXHomeDone = true;
                            Console.WriteLine("HomeX done: " + _dblX.ToString());
                            MainLogger.Info("HomeX done: " + _dblX.ToString());
                        }

                        break;
                    }
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis0 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }
                */
            }
            else if (nRet == 0)
            {
                dir0[0] = -1;
                homeFlag0[0] = 0;

                int nHomeRet = 0;

                nRet = mciHomeToSwitch(0, 1, ref axis0[0], ref dir0[0], ref homeFlag0[0]);
                //nRet = mciHomeToIndex(0, 1, ref axis0[0], ref dir0[0], ref homeFlag0[0]);
                Console.WriteLine("HomeAxis0 mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis0 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }

                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis0[0], 0);
                    Console.WriteLine("HomeAxis0 nHomeRet: " + nHomeRet.ToString());
                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        if (nHomeRet == 1) //home done
                        {
                            float[] pos = new float[2];
                            pos[0] = 0;
                            pos[1] = 0;

                            int nRetRead = mciReadFloat(0, "pos", 3, ref pos[0]);
                            //Console.WriteLine("mciReadFloat: " + nRet.ToString());
                            //Form1.Instance.DelegateUpdateXPos(pos[0]);
                            //Form1.Instance.DelegateUpdateYPos(pos[1]);
                            if (pos[0] < 0)
                            {
                                _dblX = -pos[0];
                            }
                            else
                            {
                                _dblX = pos[0];
                            }

                            _bXHomeDone = true;
                            Console.WriteLine("HomeX done: " + _dblX.ToString());
                            MainLogger.Info("HomeX done: " + _dblX.ToString());
                        }

                        break;
                    }

                    Thread.Sleep(10);
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis0 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }
                */


                dir0[0] = 1;
                homeFlag0[0] = 3;

                nRet = mciHomeToSwitch(0, 1, ref axis0[0], ref dir0[0], ref homeFlag0[0]);
                //nRet = mciHomeToIndex(boardNum, 1, ref axis[0], ref dirn[0], ref homeFlag[0]);
                Console.WriteLine("HomeAxis0 mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis0 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }

                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis0[0], 0);

                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        break;
                    }
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis0 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis0[0]);
                    nRet = mciErrClear(0, 1, ref axis0[0]);
                    return;
                }
                */
            }
            else //nRet == -1, error
            {
                return;
            }

        }

        private void HomeAxis1()        //nAxis = 0, 1
        {
            _bYHomeDone = false;

            this.OnHomeYDoneCallBack += HomeYDoneCallBackFunction;

            if (!this._bHardware)
            {
                Thread.Sleep(1000);
                _bYHomeDone = true;
                return;
            }

            Console.WriteLine("HomeAxis1:");
            int[] axis1 = new int[2];
            axis1[0] = 1;
            int[] io1 = new int[10];
            io1[0] = 40;

            int nRet = -1;
            nRet = mciHomeAbortN(0, 1, ref axis1[0]);
            Console.WriteLine("HomeAxis1 mciHomeAbortN: " + nRet.ToString());

            nRet = mciErrClear(0, 1, ref axis1[0]);
            Console.WriteLine("HomeAxis1 mciErrClear: " + nRet.ToString());

            nRet = mciDigitalWaitAll(0, 1, ref io1[0], 0);
            Console.WriteLine("HomeAxis1 mciDigitalWaitAll: " + nRet.ToString());

            int[] dir1 = new int[2];
            int[] homeFlag1 = new int[2];
            if (nRet == 1)
            {
                dir1[0] = 1;
                homeFlag1[0] = 3;

                int nHomeRet = 0;

                nRet = mciHomeToSwitch(0, 1, ref axis1[0], ref dir1[0], ref homeFlag1[0]);
                //nRet = mciHomeToIndex(0, 1, ref axis1[0], ref dir1[0], ref homeFlag1[0]);
                Console.WriteLine("HomeAxis1 mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis1 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }

                //1 = home done on all axises
                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis1[0], 0);
                    Console.WriteLine("HomeAxis1 nHomeRet: " + nHomeRet.ToString());
                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        if (nHomeRet == 1) //home done
                        {
                            float[] pos = new float[2];
                            pos[0] = 0;
                            pos[1] = 0;

                            int nRetRead = mciReadFloat(0, "pos", 3, ref pos[0]);
                            //Console.WriteLine("mciReadFloat: " + nRet.ToString());
                            //Form1.Instance.DelegateUpdateXPos(pos[0]);
                            //Form1.Instance.DelegateUpdateYPos(pos[1]);
                            if (pos[0] < 0)
                            {
                                _dblY = -pos[1];
                            }
                            else
                            {
                                _dblY = pos[1];
                            }

                            _bYHomeDone = true;
                            Console.WriteLine("HomeY done: " + _dblY.ToString());
                            MainLogger.Info("HomeY done: " + _dblY.ToString());
                        }

                        break;
                    }

                    Thread.Sleep(10);
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis1 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }
                */
            }
            else if (nRet == 0)
            {
                dir1[0] = -1;
                homeFlag1[0] = 0;

                int nHomeRet = 0;

                nRet = mciHomeToSwitch(0, 1, ref axis1[0], ref dir1[0], ref homeFlag1[0]);
                //nRet = mciHomeToIndex(0, 1, ref axis1[0], ref dir1[0], ref homeFlag1[0]);
                Console.WriteLine("mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis1 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }

                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis1[0], 0);
                    Console.WriteLine("HomeAxis1 nHomeRet: " + nHomeRet.ToString());
                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        if (nHomeRet == 1) //home done
                        {
                            float[] pos = new float[2];
                            pos[0] = 0;
                            pos[1] = 0;

                            int nRetRead = mciReadFloat(0, "pos", 3, ref pos[0]);
                            //Console.WriteLine("mciReadFloat: " + nRet.ToString());
                            //Form1.Instance.DelegateUpdateXPos(pos[0]);
                            //Form1.Instance.DelegateUpdateYPos(pos[1]);
                            if (pos[0] < 0)
                            {
                                _dblY = -pos[1];
                            }
                            else
                            {
                                _dblY = pos[1];
                            }

                            _bYHomeDone = true;
                            Console.WriteLine("HomeY done: " + _dblY.ToString());
                            MainLogger.Info("HomeY done: " + _dblY.ToString());
                        }

                        break;
                    }
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis1 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }
                */

                dir1[0] = 1;
                homeFlag1[0] = 3;

                nRet = mciHomeToSwitch(0, 1, ref axis1[0], ref dir1[0], ref homeFlag1[0]);
                //nRet = mciHomeToIndex(boardNum, 1, ref axis[0], ref dirn[0], ref homeFlag[0]);
                Console.WriteLine("HomeAxis1 mciHomeToSwitch: " + nRet.ToString());

                if (nRet == -1)
                {
                    Console.WriteLine("HomeAxis1 mciHomeToSwitch error");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }

                while ((nHomeRet != 1) /*|| (sw.ElapsedMilliseconds < 10000)*/)
                {
                    Application.DoEvents();
                    nHomeRet = mciHomeDoneWaitAll(0, 1, ref axis1[0], 0);

                    if ((nHomeRet == 1) || (nHomeRet == -1))
                    {
                        break;
                    }
                }

                /*
                if (nHomeRet == -1)      //timeout occurs
                {
                    Console.WriteLine("HomeAxis1 mciHomeDoneWaitAll timeout");

                    nRet = mciHomeAbortN(0, 1, ref axis1[0]);
                    nRet = mciErrClear(0, 1, ref axis1[0]);
                    return;
                }
                */
            }
            else //nRet == -1, error
            {
                return;
            }

        }

        #endregion


        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainLogger.Info("Home Axises");
            _bXHomeDone = false;
            _bYHomeDone = false;
            DelegateEnableRun();


            threadHome0 = new Thread(startHome0);
            threadHome0.Name = "threadHome0";
            threadHome0.Priority = ThreadPriority.Normal;
            threadHome0.Start();

            threadHome1 = new Thread(startHome1);
            threadHome1.Name = "threadHome1";
            threadHome1.Priority = ThreadPriority.Normal;
            threadHome1.Start();

            _machineState = UnOCRState.HOMING;
        }

        private void powerOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainLogger.Info("Power On");
            this.PBAPowerOn();
        }

        private void powerOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainLogger.Info("Power Off");
            this.PBAPowerOff();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = false;
            this.btnPauseAutoRun.Enabled = true;
            this.btnRunToFinish.Enabled = true;
            this.btnStopAutoRun.Enabled = true;

            this.btnUnload.Visible = false;

            if(_bHardware)
            {
                IOs.Instance.PalletClampsDown();
            }

            _bStopAutoRun = false;
            this._bRunToFinish = false;

            if (swUpTime.IsStopCount())
            {
                swDownTime.Stop();
                swUpTime.Start();
            }


            ThreadStart startAutoRun = delegate
            {
                unocrAutoRun();
            };

            unocrAutoRunThread = new Thread(startAutoRun);
            unocrAutoRunThread.Name = "unocrAutoRunThread";
            unocrAutoRunThread.Priority = ThreadPriority.Normal;
            unocrAutoRunThread.Start();

            _machineState = UnOCRState.AUTORUN;
        }

        private bool _bUnOCRDryRun = false;
        private void dryrunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bUnOCRDryRun = !_bUnOCRDryRun;
            dryrunToolStripMenuItem.Checked = _bUnOCRDryRun;
        }

        private bool _bStopAutoRun = true;
        private long _lOutput = 0;
        private int _nAutoRunStep = 0;
        private bool _bNoHGSTTray = true;
        private int nWaitTray = 1;
        private bool _bRunToFinish = false;

        private int _nPrevAutoRunStep = 0;  //to store previous step when area sensors are activated
        private bool _bAreaSensorsActivated = false;

        List<int> lstPalletPick = null;
        List<int> lstTrayPlace = null;

        private void unocrAutoRun()
        {
            PBACoordinate tempWDtrayCoord = new PBACoordinate();
            PBACoordinate tempHGSTtrayCoord = new PBACoordinate();
            //int nAutoRunStep = 0;
            //int nWaitTray = 1;

            int nPnPGrouping = 0;

            while (true)
            {
                FormUnOCR2040.Instance.DelegateUpdateGUI();
                if (_bStopAutoRun)
                {
                    if (unocrAutoRunThread != null)
                    {
                        DelegateEnableRunMenu(true);
                        unocrAutoRunThread.Abort();
                    }
                    break;
                }

                if (_bRunToFinish && (_nAutoRunStep == 0))
                {
                    if (unocrAutoRunThread != null)
                    {
                        DelegateEnableRunMenu(true);
                        unocrAutoRunThread.Abort();
                    }
                    break;
                }

                //FormUnOCR2040.Instance.DelegateUpdateGUI();
                switch (_nAutoRunStep)
                {
                    case -1: //area sensors are activated
                        System.Threading.Thread.Sleep(50);
                        break;


                    case 0: //wait for user to turn new tray in //wait for signal from switch

                        string strTotalRuntime = new DateTime(swStartUnOCR.Ticks()).ToString(Math.Floor(swStartUnOCR.TotalDays()).ToString() + ":HH:mm:ss");
                        string strUpTime = new DateTime(swUpTime.CummTicks()).ToString(Math.Floor(swUpTime.TotalDays()).ToString() + ":HH:mm:ss.fff");
                        string strDownTime = new DateTime(swDownTime.CummTicks()).ToString(Math.Floor(swDownTime.TotalDays()).ToString() + ":HH:mm:ss.fff");

                        double dblUPH = 0.0;
                        try
                        {
                            dblUPH = _lOutput / swUpTime.TotalHours();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            dblUPH = 0.0;
                        }

                        string strUPH = Math.Floor(dblUPH).ToString();
                        string strOutput = _lOutput.ToString();
                        DowntimeLogger.Info("Total run time: " + strTotalRuntime);
                        DowntimeLogger.Info("Total up time: " + strUpTime);
                        DowntimeLogger.Info("Total down time: " + strDownTime);
                        DowntimeLogger.Info("Output: " + _lOutput.ToString());
                        DowntimeLogger.Info("UPH: " + dblUPH.ToString());
                        
                        
                        if (_bUnOCRDryRun)
                        {
                            _nAutoRunStep = 1;
                        }
                        else
                        {
                            //not dryrun
                            if(_bHardware)
                            {
                                this._bWaitWDTray = true;
                                DelegateWaitWDTrayGUI();
                                Thread.Sleep(500);

                                if (IOs.Instance.IsTurnTraySWOn() && IOs.Instance.IsPush2Activated())
                                {
                                    IOs.Instance.TurnTraySWLightOn();
                                    _nAutoRunStep = 1;
                                    break;
                                }
                            }
                            else
                            {
                                _nAutoRunStep = 1;
                            }

                        }

                        break;

                    case 1:
                        DelegateEnableUnloadVisibility(false);
                        if (_bHardware)
                        {
                            IOs.Instance.TurnTraySWLightOn();

                            if (IOs.Instance.IsTray1In())
                            {
                                IOs.Instance.Tray2In();
                                nWaitTray = 2;
                            }
                            else if (IOs.Instance.IsTray2In())
                            {
                                IOs.Instance.Tray1In();
                                nWaitTray = 1;
                            }

                            //nAutoRunStep = 2;
                            _nAutoRunStep = 2;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            nWaitTray = (nWaitTray == 1) ? 2 : 1;
                            _nSimulateWaitTray = nWaitTray;
                            _nAutoRunStep = 2;
                            Thread.Sleep(100);
                        }

                        break;


                    case 2:
                        if (nWaitTray == 1)
                        {
                            if(!_bHardware)
                            {
                                _wdtray1 = _wdtray2;
                                _nAutoRunStep = 3;
                                break;
                            }

                            if (IOs.Instance.IsTray1In())
                            {
                                _nAutoRunStep = 3;
                            }
                        }
                        else
                        {
                            if (!_bHardware)
                            {
                                _wdtray2 = _wdtray1;
                                _nAutoRunStep = 3;
                                break;
                            }

                            if (IOs.Instance.IsTray2In())
                            {
                                _nAutoRunStep = 3;
                            }
                        }

                        break;

                    
                    case 3:
                        if (_bHardware)
                        {
                            IOs.Instance.TurnTraySWLightOff();
                            Thread.Sleep(500);
                        }

                        //nAutoRunStep = 4;
                        _nAutoRunStep = 4;
                        break;


                    case 4:
                        if (_bHardware)
                        {
                            //_wdtray1.LoadPart();
                            if (nWaitTray == 1)
                            {
                                //_wdtray1.LoadPart(_simulateHGAData());
                                _wdtray1.LoadPart();    //this should load wdtray data after getting data from host
                                if (IOs.Instance.IsTray1In())
                                {
                                    //nAutoRunStep = 10;
                                    _nAutoRunStep = 5;      //scan tray barcode
                                }
                            }
                            else
                            {
                                _wdtray2.LoadPart();
                                if (IOs.Instance.IsTray2In())
                                {
                                    //nAutoRunStep = 10;
                                    _nAutoRunStep = 5;      //scan tray barcode
                                }
                            }
                        }

                        if (!_bHardware)
                        {
                            this._bWaitWDTray = true;
                            DelegateWaitWDTrayGUI();
                            Thread.Sleep(500);

                            if(IOs.Instance.IsCtrlPressd())
                            {
                                if (nWaitTray == 1)
                                {
                                    _wdtray1.LoadPart(_simulateHGAData());
                                }
                                else
                                {
                                    _wdtray2.LoadPart(_simulateHGAData());
                                }

                                _nAutoRunStep = 10;
                                break;
                            }
                        }

                        break;

                    case 5: //scan wdtray barcode and get info from host
                        SICKScannerObj.Instance.COMPort = "COM4";
                        string strTrayID = SICKScannerObj.Instance.ReadBarcode();
                        //MessageBox.Show(strTrayID);

                        ListViewItem msgItem = new ListViewItem();

                        if (strTrayID.Length != 10)
                        {
                            msgItem.Text = "Invalid tray; please reload";
                            DelegateUpdateReplyList(msgItem);

                            _nAutoRunStep = 0;  //invalid tray
                            break;
                        }


                        AQTrayClass aqtrayObj = new AQTrayClass();
                        string strReply = String.Empty;

                        if (_cbkClient.GetTrayData(strTrayID, out strReply, out aqtrayObj) == -1)
                        {
                            msgItem.Text = "Invalid tray; please reload"; ;
                            DelegateUpdateReplyList(msgItem);

                            _nAutoRunStep = 0;  //invalid tray
                            break;
                        }

                        msgItem.Text = strReply;
                        DelegateUpdateReplyList(msgItem);

                        if (nWaitTray == 1)
                        {
                            _wdtray1.LoadPart(getHGAArrayFromAQTrayData(aqtrayObj));
                        }
                        else
                        {
                            _wdtray2.LoadPart(getHGAArrayFromAQTrayData(aqtrayObj));
                        }

                        _nAutoRunStep = 6;
                        break;


                    case 6:
                        if (nWaitTray == 1)
                        {
                            nPnPGrouping = _wdtray1.NextHGAToUnload(true) / 5;
                        }
                        else
                        {
                            nPnPGrouping = _wdtray2.NextHGAToUnload(true) / 5;
                        }

                        _nAutoRunStep = 10;

                        break;


                    case 7: //invalid tray, pop up
                        break;


                    case 8:
                        break;


                    case 9:
                        break;


                    case 10:
                        this._bWaitWDTray = false;
                        DelegateWaitWDTrayGUI();



                        //lstPalletPick.Clear();

                        //unload part from wd tray
                        //go to wdtray
                        if (nWaitTray == 1)
                        {
                            //if (_wdtray1.CountHGA() > 0)
                            if (_wdtray1.CountHGA(true) > 0)
                            {
                                if (_bHardware)
                                {
                                    Console.WriteLine(_wdtray1.NextHGAToUnload(true).ToString());
                                    MainLogger.Info("_wdtray1.NextHGAToUnload(true): " + _wdtray1.NextHGAToUnload(true).ToString());
                                    tempWDtrayCoord = _wdtray1._unloadPos[_wdtray1.NextHGAToUnload(true) / 5];


                                    // //////////////////////////////////////////////
                                    //#pnp1
                                    //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());                            
                                    //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                                    //if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                                    //{
                                    //    lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                    //    _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                                    //}

                                    //#pnp2
                                    //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                                    //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                                    //if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                                    //{
                                    //    lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                    //    _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                                    //}

                                    //#pnp3
                                    //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                                    //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                                    //if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                                    //{
                                    //    lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                    //    _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                                    //}

                                    //#pnp4
                                    //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                                    //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                                    //if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                                    //{
                                    //    lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                    //    _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                                    //}

                                    //#pnp5
                                    //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                                    //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                                    //if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                                    //{
                                    //    lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                    //    _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                                    //}

                                    // //////////////////////////////////////////////


                                    MainLogger.Info("PBAMoveAbs: " + tempWDtrayCoord.X.ToString() + "," + tempWDtrayCoord.Y.ToString());
                                    this.PBAMoveAbs(tempWDtrayCoord.X, tempWDtrayCoord.Y);
                                }
                                else
                                {
                                    Thread.Sleep(500);
                                }

                                _nAutoRunStep = 20;
                            }
                            else
                            {
                                Console.WriteLine("wd tray empty");

                                _nAutoRunStep = 0;
                            }
                        }
                        else
                        {
                            if (_wdtray2.CountHGA(true) > 0)
                            {
                                if (_bHardware)
                                {
                                    Console.WriteLine(_wdtray2.NextHGAToUnload(true).ToString());
                                    MainLogger.Info("_wdtray2.NextHGAToUnload(true): " + _wdtray2.NextHGAToUnload(true).ToString());
                                    tempWDtrayCoord = _wdtray2._unloadPos[_wdtray2.NextHGAToUnload(true) / 5];

                                    MainLogger.Info("PBAMoveAbs: " + tempWDtrayCoord.X.ToString() + "," + tempWDtrayCoord.Y.ToString());
                                    this.PBAMoveAbs(tempWDtrayCoord.X, tempWDtrayCoord.Y);
                                }
                                else
                                {
                                    Thread.Sleep(500);
                                }

                                _nAutoRunStep = 20;
                            }
                            else
                            {
                                Console.WriteLine("wd tray empty");

                                _nAutoRunStep = 0;
                            }
                        }


                        break;

                    case 20:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleExtend())
                            {
                                IOs.Instance.NozzleExtend();
                                Thread.Sleep(50);
                            }
                            else
                            {
                                _nAutoRunStep = 21;
                            }
                        }
                        else
                        {
                            _nAutoRunStep = 21;
                        }

                        break;


                    case 21:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleDown())
                            {
                                IOs.Instance.NozzleDown();
                                Thread.Sleep(50);
                            }
                            else
                            {
                                _nAutoRunStep = 22;
                            }
                        }
                        else
                        {
                            _nAutoRunStep = 22;
                        }

                        break;


                    case 22:
                        if (_bHardware)
                        {
                            //IOs.Instance.NozzleVacAllOn();
                            //for (int i = 0; i < lstPalletPick.Count; i++)
                            //{
                            //    IOs.Instance.NozzleVacuumOn(lstPalletPick[i]);
                            //}
                            Thread.Sleep(200);
                        }
                        //else
                        //{
                        //}

                        _nAutoRunStep = 30;
                        break;


                    case 23:
                        break;


                    case 30:
                        //unload part
                        if (nWaitTray == 1)
                        {
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload());
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload());
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload());
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload());
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload());

                            //nPnPGrouping = 0;
                            lstPalletPick.Clear();


                            //#pnp1
                            //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());                            
                            //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                            //nPnPGrouping = _wdtray1.NextHGAToUnload(true)/5;
                            //lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                            //_wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            if (nPnPGrouping == _wdtray1.NextHGAToUnload(true) / 5)
                            {
                                lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            }

                            //#pnp2
                            //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray1.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            }

                            //#pnp3
                            //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray1.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            }

                            //#pnp4
                            //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray1.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            }

                            //#pnp5
                            //Console.WriteLine("wdtray count: " + _wdtray1.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray1.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray1.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray1.NextHGAToUnload(true));
                                _wdtray1.UnloadPart(_wdtray1.NextHGAToUnload(true));
                            }
                        }
                        else
                        {
                            //_wdtray2.UnloadPart(_wdtray2.NextHGAToUnload());
                            //_wdtray2.UnloadPart(_wdtray2.NextHGAToUnload());
                            //_wdtray2.UnloadPart(_wdtray2.NextHGAToUnload());
                            //_wdtray2.UnloadPart(_wdtray2.NextHGAToUnload());
                            //_wdtray2.UnloadPart(_wdtray2.NextHGAToUnload());

                            //nPnPGrouping = 0;

                            //#pnp1
                            //Console.WriteLine("wdtray count: " + _wdtray2.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray2.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray2.NextHGAToUnload(true) / 5)
                            {
                                lstPalletPick.Add(_wdtray2.NextHGAToUnload(true));
                                _wdtray2.UnloadPart(_wdtray2.NextHGAToUnload(true));
                            }

                            //#pnp2
                            //Console.WriteLine("wdtray count: " + _wdtray2.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray2.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray2.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray2.NextHGAToUnload(true));
                                _wdtray2.UnloadPart(_wdtray2.NextHGAToUnload(true));
                            }

                            //#pnp3
                            //Console.WriteLine("wdtray count: " + _wdtray2.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray2.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray2.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray2.NextHGAToUnload(true));
                                _wdtray2.UnloadPart(_wdtray2.NextHGAToUnload(true));
                            }

                            //#pnp4
                            //Console.WriteLine("wdtray count: " + _wdtray2.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray2.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray2.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray2.NextHGAToUnload(true));
                                _wdtray2.UnloadPart(_wdtray2.NextHGAToUnload(true));
                            }

                            //#pnp5
                            //Console.WriteLine("wdtray count: " + _wdtray2.CountHGA().ToString());
                            //Console.WriteLine("NextToUnload: " + _wdtray2.NextHGAToUnload(true).ToString());
                            if (nPnPGrouping == _wdtray2.NextHGAToUnload(true)/5)
                            {
                                lstPalletPick.Add(_wdtray2.NextHGAToUnload(true));
                                _wdtray2.UnloadPart(_wdtray2.NextHGAToUnload(true));
                            }
                        }


                        //_nAutoRunStep = 40;
                        _nAutoRunStep = 31;
                        break;


                    case 31:
                        if (_bHardware)
                        {
                            for (int i = 0; i < lstPalletPick.Count; i++)
                            {
                                IOs.Instance.NozzleVacuumOn(lstPalletPick[i]%5);
                            }
                            Thread.Sleep(200);
                        }

                        _nAutoRunStep = 40;
                        break;


                    case 40:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleUp())
                            {
                                IOs.Instance.NozzleUp();
                                Thread.Sleep(50);
                            }
                            else
                            {
                                _nAutoRunStep = 50;
                            }
                        }
                        else
                        {
                            _nAutoRunStep = 50;
                        }

                        break;


                    case 41:
                        if (_bUnOCRDryRun)
                        {
                            _nAutoRunStep = 50;
                            _hgsttray1.UnloadAll();
                        }
                        else
                        {   //
                            if(_bHardware)
                            {
                                this._bWaitHGSTTray = true;
                                DelegateWaitHGSTTrayGUI();
                                Thread.Sleep(200);

                                if (IOs.Instance.IsPush1Activated() && IOs.Instance.IsPush2Activated())
                                {
                                    _nAutoRunStep = 50;
                                    _hgsttray1.UnloadAll();
                                }
                            }
                            else
                            {
                                this._bWaitHGSTTray = true;
                                DelegateWaitHGSTTrayGUI();
                                Thread.Sleep(500);

                                if(IOs.Instance.IsCtrlPressd())
                                {
                                    _nAutoRunStep = 50;
                                    _hgsttray1.UnloadAll();
                                }
                            }
                        }
                        break;


                    case 50:
                        if (_bNoHGSTTray)
                        {
                            _bNoHGSTTray = false;
                            _nAutoRunStep = 41;
                            break;
                        }

                        //unload part to hgst tray
                        //go to hgst tray
                        if ((_hgsttray1.CountHGA() < 40) && (lstPalletPick.Count > 0))
                        {
                            if(_bHardware)
                            {
                                Console.WriteLine(_hgsttray1.NextHGAToUnload().ToString());
                                MainLogger.Info("_hgsttray1.NextHGAToUnload(): " + _hgsttray1.NextHGAToUnload().ToString());
                                tempHGSTtrayCoord = _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5];

                                MainLogger.Info("PBAMoveAbs: " + tempHGSTtrayCoord.X.ToString() + "," + tempHGSTtrayCoord.Y.ToString());
                                this.PBAMoveAbs(tempHGSTtrayCoord.X, tempHGSTtrayCoord.Y);
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }

                            //nAutoRunStep = 60;
                            _nAutoRunStep = 60;
                        }
                        else
                        {
                            if (_hgsttray1.CountHGA() < 40)
                            {
                                _nAutoRunStep = 6; //hgst tray not full yet
                                break;
                            }

                            Console.WriteLine("hgst tray full");
                            //_hgsttray1.UnloadAll();
                            _nAutoRunStep = 41;
                            //DelegateEnableUnloadVisibility(true);
                        }

                        break;


                    case 60:
                        this._bWaitHGSTTray = false;
                        DelegateWaitHGSTTrayGUI();
                        Thread.Sleep(100);

                        if(_bHardware)
                        {
                            //IOs.Instance.NozzleDown();
                            //Thread.Sleep(1000);
                            if (IOs.Instance.IsNozzleExtend())
                            {
                                IOs.Instance.NozzleRetract();
                                Thread.Sleep(50);
                            }
                            else
                            {
                                //_nAutoRunStep = 61;
                                _nAutoRunStep = 70;
                            }
                        }
                        else
                        {
                            //_nAutoRunStep = 61;
                            _nAutoRunStep = 70;
                        }

                        break;


                    case 61:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleDown())
                            {
                                IOs.Instance.NozzleDown();
                                Thread.Sleep(200);
                            }
                            else
                            {
                                _nAutoRunStep = 62;
                            }

                        }
                        else
                        {
                            _nAutoRunStep = 62;
                        }

                        break;


                    case 62:
                        if (_bHardware)
                        {
                            //IOs.Instance.NozzleVacAllOff();
                            //Thread.Sleep(200);
                        }
                        //else
                        //{
                        //}

                        lstTrayPlace.Clear();
                        _nAutoRunStep = 70;
                        break;

                    case 65:    //nozzle up
                        if(_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleUp())
                            {
                                IOs.Instance.NozzleUp();
                                Thread.Sleep(50);
                                break;
                            }
                        }

                        _nAutoRunStep = 66;
                        break;

                    case 66:    //index
                        //double dblPitch = 1.8;
                        //double diff = (lstPalletPick[0] % 5) - (_hgsttray1.NextHGAToUnload() % 5);
                        //this.PBAMoveRel(diff * dblPitch, 0.0);

                        Thread.Sleep(100);
                        _nAutoRunStep = 61;
                        break;
                    

                    case 70:
                        //unload part
                        //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                        //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                        //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                        //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                        //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());

                        //_lOutput += 5;

                        //while((lstPalletPick.Count > 0) && (_hgsttray1.CountHGA() < 40))
                        if ((lstPalletPick.Count > 0) && (_hgsttray1.CountHGA() < 40))
                        {
                            if (tempHGSTtrayCoord != _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5])
                            {
                                _nAutoRunStep = 50;
                                break; //break while loop
                            }

                            //MessageBox.Show("Pick: " + lstPalletPick[0].ToString() + " Place: " + _hgsttray1.NextHGAToUnload().ToString());
                            Console.WriteLine("Pick: " + lstPalletPick[0].ToString() + " Place: " + _hgsttray1.NextHGAToUnload().ToString());

                            //MessageBox.Show("(lstPalletPick[0] % 5):" + (lstPalletPick[0] % 5).ToString());
                            //MessageBox.Show("(_hgsttray1.NextHGAToUnload() % 5):" + (_hgsttray1.NextHGAToUnload() % 5).ToString());
                            //MessageBox.Show("(_hgsttray1.NextHGAToUnload() % 10):" + (_hgsttray1.NextHGAToUnload() % 10).ToString());

                            if ((lstPalletPick[0] % 5) == (_hgsttray1.NextHGAToUnload() % 5))
                            {
                                //IOs.Instance.NozzleVacuumOff(lstPalletPick[0] % 5);
                                _nAutoRunStep = 71;
                                break; //break while loop
                            }
                            else
                            {
                                if (_bHardware)
                                {
                                    //while(!IOs.Instance.IsNozzleUp())
                                    //{
                                    //    IOs.Instance.NozzleUp();
                                    //    Thread.Sleep(1000);
                                    //    break;
                                    //}

                                    double dblPitchX = 8.0;
                                    //double dblPitchY = 59.68;
                                    Console.WriteLine((lstPalletPick[0] % 5).ToString());
                                    Console.WriteLine((_hgsttray1.NextHGAToUnload() % 10).ToString());
                                    //double diff = (lstPalletPick[0] % 5) - (_hgsttray1.NextHGAToUnload() % 10);
                                    double diff = (lstPalletPick[0] % 5) - (_hgsttray1.NextHGAToUnload() % 5);

                                    this.PBAMoveRel(diff * dblPitchX, 0.0);
                                    Thread.Sleep(500);

                                    //while (!IOs.Instance.IsNozzleDown())
                                    //{
                                    //    IOs.Instance.NozzleDown();
                                    //    Thread.Sleep(1000);
                                    //    break;
                                    //}

                                    //IOs.Instance.NozzleVacuumOff(lstPalletPick[0] % 5);
                                }

                                _nAutoRunStep = 71;
                                break; //break while loop
                            }

                            //_hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                            //_lOutput++;

                            //FormUnOCR2040.Instance.DelegateUpdateGUI();

                            //lstPalletPick.RemoveAt(0);
                            //Console.WriteLine("lstPalletPick.count:" + lstPalletPick.Count.ToString());
                            Console.WriteLine("_hgsttray1.CountHGA:" + _hgsttray1.CountHGA().ToString());
                        }

                        if (tempHGSTtrayCoord != _hgsttray1._unloadPos[_hgsttray1.NextHGAToUnload() / 5])
                        {
                            _nAutoRunStep = 50;
                            //while (!IOs.Instance.IsNozzleUp())
                            //{
                            //    IOs.Instance.NozzleUp();
                            //    Thread.Sleep(1000);
                            //    break;
                            //}

                            break; 
                        }

                        //nAutoRunStep = 80;

                        //_nAutoRunStep = 80;  //new logic for sorting
                        _nAutoRunStep = 80;  //new logic for sorting
                        break;


                    case 71:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleDown())
                            {
                                IOs.Instance.NozzleDown();
                                Thread.Sleep(100);
                            }
                            else
                            {
                                _nAutoRunStep = 72;
                            }

                        }

                        break;


                    case 72:
                        if ((lstPalletPick[0] % 5) == (_hgsttray1.NextHGAToUnload() % 5))
                        {
                            IOs.Instance.NozzleVacuumOff(lstPalletPick[0] % 5);
                        }
                        else
                        {
                            if (_bHardware)
                            {
                                IOs.Instance.NozzleVacuumOff(lstPalletPick[0] % 5);
                            }
                        }

                        _hgsttray1.LoadPart(_hgsttray1.NextHGAToUnload());
                        _lOutput++;

                        FormUnOCR2040.Instance.DelegateUpdateGUI();
                        lstPalletPick.RemoveAt(0); 

                        _nAutoRunStep = 73;
                        Thread.Sleep(1500);
                        break;


                    case 73:
                        if (_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleUp())
                            {
                                IOs.Instance.NozzleUp();
                                Thread.Sleep(100);
                            }

                            else
                            {
                                _nAutoRunStep = 50;
                            }
                        }

                        break;


                    case 80:
                        if(_bHardware)
                        {
                            if (!IOs.Instance.IsNozzleUp())
                            {
                                IOs.Instance.NozzleUp();

                                if (_hgsttray1.CountHGA() >= 40)
                                {
                                    DelegateEnableUnloadVisibility(true);
                                }

                                Thread.Sleep(50);
                                break;
                            }
                        }

                        if (nWaitTray == 1)
                        {
                            //if (_wdtray1.CountHGA() > 0)
                            if (_wdtray1.CountHGA(true) > 0)
                            {
                                //_nAutoRunStep = 10;
                                _nAutoRunStep = 6;
                            }
                            else
                            {
                                _nAutoRunStep = 0;
                            }
                        }
                        else
                        {
                            //if (_wdtray2.CountHGA() > 0)
                            if (_wdtray2.CountHGA(true) > 0)
                            {
                                _nAutoRunStep = 6;
                            }
                            else
                            {
                                _nAutoRunStep = 0;
                            }
                        }


                        break;

                    default:
                        break;
                }


                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

        private delegate void enableRunMenu(bool bEnabled);
        private void EnableRunMenu(bool bEnabled)
        {
            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = bEnabled;
            this.btnUnload.Visible = true;
        }
        public void DelegateEnableRunMenu(bool bEnabled)
        {
            Invoke(new enableRunMenu(EnableRunMenu), bEnabled);
        }

        private delegate void enableUnloadVisibility(bool bVisibled);
        private void EnableUnloadVisibility(bool bVisibled)
        {
            this.btnUnload.Visible = bVisibled;
        }
        public void DelegateEnableUnloadVisibility(bool bVisibled)
        {
            Invoke(new enableUnloadVisibility(EnableUnloadVisibility), bVisibled);
        }





        #region delegate waitWDTrayGUI
        private delegate void waitWDTrayGUI();
        private bool _bWaitWDTray = false;
        private void WaitWDTrayGUI()
        {
            if(_bWaitWDTray)
            {
                this.panelWDTray.Visible = !this.panelWDTray.Visible;
            }
            else
            {
                this.panelWDTray.Visible = false;
            }
        }
        public void DelegateWaitWDTrayGUI()
        {
            Invoke(new waitWDTrayGUI(WaitWDTrayGUI));
        }
        #endregion

        #region delegate waitHGSTTrayGUI
        private delegate void waitHGSTTrayGUI();
        private bool _bWaitHGSTTray = false;
        private void WaitHGSTTrayGUI()
        {
            if (_bWaitHGSTTray)
            {
                this.panelHGSTTray.Visible = !this.panelHGSTTray.Visible;
            }
            else
            {
                this.panelHGSTTray.Visible = false;
            }
        }
        public void DelegateWaitHGSTTrayGUI()
        {
            Invoke(new waitHGSTTrayGUI(WaitHGSTTrayGUI));
        }
        #endregion

        #region delegate resetWaitWDnHGSTTrayGUI
        private delegate void resetWaitWDnHGSTTrayGUI();
        private void ResetWaitWDnHGSTTrayGUI()
        {
            this.panelWDTray.Visible = false;
            this.panelHGSTTray.Visible = false;
        }
        public void DelegateResetWaitWDnHGSTTrayGUI()
        {
            Invoke(new resetWaitWDnHGSTTrayGUI(ResetWaitWDnHGSTTrayGUI));
        }
        #endregion


        private void setUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLinearSetup.Instance.ShowDialog();
        }


        #region PBA Control

        public void PBAPowerOn()
        {
            int nRet = -1;
            string[] errType = new string[1];
            errType[0] = "following";

            nRet = mciControllerStart(0);
            Console.WriteLine("btnPowerON_Click: mciControllerStart " + nRet.ToString());

            _axis[0] = 0;
            int[] errFlag = new int[2];
            nRet = mciErrFlagsGet(boardNum, 2, ref _axis[0], ref errFlag[0]);
            if ((errFlag[0] == 32) || (errFlag[1] == 32))
            {
                nRet = mciErrClear(boardNum, 2, ref _axis[0]);
                Console.WriteLine("errFlag==32; mciErrClear: " + nRet.ToString());
            }

            nRet = mciErrCheckEnable(boardNum, errType[0], 2, ref _axis[0]);
            Console.WriteLine("mciErrCheckEnable: " + nRet.ToString());

            nRet = mciAmpEnable(0, 2, ref _axis[0]);
            Console.WriteLine("mciAmpEnable: " + nRet.ToString());
        }

        public void PBAPowerOff()
        {
            int nRet = -1;
            nRet = mciAmpDisable(0, 2, ref _axis[0]);
        }

        public void PBAClearError()
        {
            int nRet = -1;
            nRet = mciErrClear(boardNum, 2, ref _axis[0]);
            MainLogger.Info("mciErrClear: " + nRet.ToString());
        }

        public void PBAAxisXStepPositive(double dblStep)
        {
            _axis[0] = 0;
            int nRet = -1;

            double[] dblVel = new double[2];
            dblVel[0] = 10;
            dblVel[1] = 10;
            nRet = mciSpeedSet(0, 2, ref dblVel[0], ref _axis[0]);

            double[] delta = new double[2];
            delta[0] = 1 * dblStep;

            nRet = mciMoveRel(0, 1, ref delta[0], ref _axis[0]);
            Console.WriteLine("mciMoveRel: " + nRet.ToString());
        }

        public void PBAAxisXStepNegative(double dblStep)
        {
            _axis[0] = 0;
            int nRet = -1;

            double[] dblVel = new double[2];
            dblVel[0] = 10;
            dblVel[1] = 10;
            nRet = mciSpeedSet(0, 2, ref dblVel[0], ref _axis[0]);

            double[] delta = new double[2];
            delta[0] = -1 * dblStep;

            nRet = mciMoveRel(0, 1, ref delta[0], ref _axis[0]);
            Console.WriteLine("mciMoveRel: " + nRet.ToString());
        }

        public void PBAAxisYStepPositive(double dblStep)
        {
            _axis[1] = 1;
            int nRet = -1;

            double[] dblVel = new double[2];
            dblVel[0] = 10;
            dblVel[1] = 10;
            nRet = mciSpeedSet(0, 2, ref dblVel[0], ref _axis[0]);

            double[] delta = new double[2];
            delta[0] = 1 * dblStep;

            nRet = mciMoveRel(0, 1, ref delta[0], ref _axis[1]);
            Console.WriteLine("mciMoveRel: " + nRet.ToString());
        }

        public void PBAAxisYStepNegative(double dblStep)
        {
            _axis[1] = 1;
            int nRet = -1;

            double[] dblVel = new double[2];
            dblVel[0] = 10;
            dblVel[1] = 10;
            nRet = mciSpeedSet(0, 2, ref dblVel[0], ref _axis[0]);

            double[] delta = new double[2];
            delta[0] = -1 * dblStep;

            nRet = mciMoveRel(0, 1, ref delta[0], ref _axis[1]);
            Console.WriteLine("mciMoveRel: " + nRet.ToString());
        }


        public delegate int HomeXDoneCallback();
        public HomeXDoneCallback OnHomeXDoneCallBack
        {
            get;
            set;
        }
        public int HomeXDoneCallBackFunction()
        {
            this.OnHomeXDoneCallBack -= HomeXDoneCallBackFunction;

            if(!_bHardware)
            {
                return 1;  //home done
            }

            int nRet = 0;
            while (nRet == 0)
            {
                nRet = mciHomeDoneWaitAll(0, 1, ref _axis[0], 0);
                Thread.Sleep(10);
            }

            return nRet;
        }

        public delegate int HomeYDoneCallback();
        public HomeYDoneCallback OnHomeYDoneCallBack
        {
            get;
            set;
        }
        public int HomeYDoneCallBackFunction()
        {
            this.OnHomeYDoneCallBack -= HomeYDoneCallBackFunction;

            if (!_bHardware)
            {
                return 1;  //home done
            }

            int nRet = 0;
            while (nRet == 0)
            {
                nRet = mciHomeDoneWaitAll(0, 1, ref _axis[1], 0);
                Thread.Sleep(10);
            }

            return nRet;
        }




        public delegate int MotionDoneCallback();
        public MotionDoneCallback OnMotionDoneCallBack
        {
            get;
            set;
        }
        public int MotionDoneCallBackFunction()
        {
            this.OnMotionDoneCallBack -= MotionDoneCallBackFunction;

            int nRet = 0;
            while (nRet == 0)
            {
                nRet = mciMotionDoneWaitAll(0, 2, ref _axis[0], 0);
                Thread.Sleep(10);
            }
            return nRet;
        }
        public void PBAMoveAbs(double dblX, double dblY)
        {
            _axis[0] = 0;
            _axis[1] = 1;
            int nRet = -1;

            double[] dest = new double[2];
            dest[0] = dblX;
            dest[1] = dblY;
            nRet = mciMoveAbs(0, 2, ref dest[0], ref _axis[0]);    
        
            this.OnMotionDoneCallBack += MotionDoneCallBackFunction;
            nRet = 0;

            if (OnMotionDoneCallBack != null)
            {
                nRet = OnMotionDoneCallBack();
            }

            if (nRet == 1)
            {
                //all motions completely done
                //MessageBox.Show("move done");
                MainLogger.Info("PBAMoveAbs motion done: mciMoveAbs nRet " + nRet.ToString());
            }
            else
            {
                //0 = timeout, -1 = error
                MainLogger.Info("PBAMoveAbs : nRet " + nRet.ToString());
            }
        }


        public void PBAMoveRel(double dblOffsetX, double dblOffsetY)
        {
            _axis[0] = 0;
            _axis[1] = 1;
            int nRet = -1;

            double[] dest = new double[2];
            dest[0] = dblOffsetX;
            dest[1] = dblOffsetY;
            nRet = mciMoveRel(0, 2, ref dest[0], ref _axis[0]);

            this.OnMotionDoneCallBack += MotionDoneCallBackFunction;
            nRet = 0;

            if (OnMotionDoneCallBack != null)
            {
                nRet = OnMotionDoneCallBack();
            }

            if (nRet == 1)
            {
                //all motions completely done
                //MessageBox.Show("move done");
                MainLogger.Info("PBAMoveRel motion done: mciMoveRel nRet " + nRet.ToString());
            }
            else
            {
                //0 = timeout, -1 = error
                MainLogger.Info("PBAMoveRel : nRet " + nRet.ToString());
            }
        }



        public void PBASetSpeed(double dblVel)
        {
            int nRet = -1;
            double[] dblVelArr = new double[2];
            dblVelArr[0] = dblVel;
            dblVelArr[1] = dblVel;
            nRet = mciSpeedSet(0, 2, ref dblVelArr[0], ref _axis[0]); 
        }

        public void PBASetAccl(double dblAccl)
        {
            int nRet = -1;
            double[] dblAccArr = new double[2];
            dblAccArr[0] = dblAccl;
            dblAccArr[1] = dblAccl;
            nRet = mciAccelSet(0, 2, ref dblAccArr[0], ref _axis[0]);
        }

        public void PBASetDecl(double dblDecl)
        {
            int nRet = -1;
            double[] dblDeclArr = new double[2];
            dblDeclArr[0] = dblDecl;
            dblDeclArr[1] = dblDecl;
            nRet = mciDecelSet(0, 2, ref dblDeclArr[0], ref _axis[0]);
        }

        public PBACoordinate PBAGetPosition()
        {
            float[] pos = new float[2];
            pos[0] = 0;
            pos[1] = 0;

            int nRet = mciReadFloat(0, "pos", 3, ref pos[0]);
            Console.WriteLine("mciReadFloat: " + nRet.ToString());

            PBACoordinate currPos = new PBACoordinate();
            if (nRet == 0)   //no error
            {
                currPos.X = pos[0];
                currPos.Y = pos[1];
            }
            else
            {
            }

            return currPos;
        }

        #endregion

        private void btnPauseAutoRun_Click(object sender, EventArgs e)
        {
            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = true;

            _machineState = UnOCRState.STOP;
            _bStopAutoRun = true;

            if (swDownTime.IsStopCount())
            {
                swDownTime.Start();
                swUpTime.Stop();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDiag = new OpenFileDialog();
            openFileDiag.Filter = "xml files|*.xml";
            string configPath = System.Windows.Forms.Application.StartupPath;
            openFileDiag.InitialDirectory = configPath;
            
            DialogResult result = openFileDiag.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.LoadPBAConfig(openFileDiag.FileName);
            }
        }

        private void btnResetStat_Click(object sender, EventArgs e)
        {
            swUpTime.Reset();
            swDownTime.Reset();
            _lOutput = 0;
        }

        private void FormUnOCR2040_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void FormUnOCR2040_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.ControlKey)
            {
                IOs.Instance._bCtrlPressed = true;
            }
        }

        private void FormUnOCR2040_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                IOs.Instance._bCtrlPressed = false;
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAPowerOff();
            FormUnOCR2040.Instance.PBAClearError();
            FormUnOCR2040.Instance.PBAPowerOn();
        }

        private void btnStopAutoRun_Click(object sender, EventArgs e)
        {
            _machineState = UnOCRState.STOP;
            _bStopAutoRun = true;
            _nAutoRunStep = 0;
            _hgsttray1.LoadAll();   //full tray

            FormUnOCR2040.Instance.DelegateUpdateGUI();

            if (_bHardware)
            {
                IOs.Instance.NozzleUp();
                IOs.Instance.NozzleRetract();
            }

            this.btnStartAutoRun.Enabled = this.runToolStripMenuItem.Enabled = true;
            this.btnUnload.Visible = true;

            if (swDownTime.IsStopCount())
            {
                swDownTime.Start();
                swUpTime.Stop();
            }
        }

        private void btnRunToFinish_Click(object sender, EventArgs e)
        {
            _bRunToFinish = true;
            btnPauseAutoRun.Enabled = false;
            btnStopAutoRun.Enabled = false;
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            if (_bHardware)
            {
                PBACoordinate coord = new PBACoordinate();
                coord = _wdtray1._unloadPos[0];
                FormUnOCR2040.Instance.PBAMoveAbs(coord.X, coord.Y);
            }

            _hgsttray1.LoadAll();   //full tray
        }

        private void btnStartAutoRun_Click(object sender, EventArgs e)
        {
            runToolStripMenuItem_Click(sender, e);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _testGUI();
        }

        //one time subscribe when the form gets loaded
        private void FormUnOCR2040_Shown(object sender, EventArgs e)
        {
            ListViewItem msgItem = new ListViewItem();
            msgItem.Text = _cbkClient.Subscribe();
            DelegateUpdateReplyList(msgItem);
        }


  




    }

    // //////////////////////////////////////////////////////////////
    #region class HGAObj
    public class HGAObj
    {
        public HGAObj()
        {
        }

        private bool _bHasPart = false;
        public bool HasPart
        {
            get{ return _bHasPart; }
            set { _bHasPart = value;}
        }

        private bool _bDefect = false;
        public bool IsDefect
        {
            get { return _bDefect; }
            set { _bDefect = value; }
        }
    }
    #endregion


    // //////////////////////////////////////////////////////////////
    #region class WDTrayObj
    public class WDTrayObj
    {
        public HGAObj[] _hga = new HGAObj[20];
        public WDTrayObj()
        {
            for (int i = 0; i < 20; i++)
            {
                _hga[i] = new HGAObj();
                _hga[i].HasPart = false;
                _hga[i].IsDefect = false;
            }
        }

        public void LoadPart()
        {
            for (int i = 0; i < 20; i++)
            {
                _hga[i].HasPart = true;
                _hga[i].IsDefect = false;
            }
        }

        //public void LoadPart(int pos)
        //{
        //    _hga[pos].HasPart = true;
        //}

        public void LoadPart(HGAObj[] arrHGAs)
        {
            if (arrHGAs.Length != 20)
            {
                return;
            }

            for(int i = 0; i < 20; i++)
            {
                _hga[i].HasPart = arrHGAs[i].HasPart;
                _hga[i].IsDefect = arrHGAs[i].IsDefect;
            }
        }


        public void UnloadPart(int pos)
        {
            _hga[pos].HasPart = false;
            _hga[pos].IsDefect = false;
        }

        public void UnloadAll()
        {
            for (int i = 0; i < 20; i++)
            {
                _hga[i].HasPart = false;
                _hga[i].IsDefect = false;
            }
        }

        public int CountHGA()
        {
            int count = 0;
            for (int i = 0; i < 20; i++)
            {
                if (_hga[i].HasPart)
                {
                    count++;
                }
            }

            return count;
        }

        public int CountHGA(bool bExcludeDefect)
        {
            int count = 0;
            for (int i = 0; i < 20; i++)
            {
                if (bExcludeDefect)
                {
                    if (_hga[i].HasPart && (!_hga[i].IsDefect))
                    {
                        count++;
                    }
                }
                else
                {
                    if (_hga[i].HasPart)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public int NextHGAToUnload()
        {
            for (int i = 0; i < 20; i++)
            {
                if (_hga[i].HasPart)
                {
                    return i;
                }
            }

            return 0;
        }


        public int NextHGAToUnload(bool bSort)
        {
            for (int i = 0; i < 20; i++)
            {
                if (bSort)
                {
                    if (_hga[i].HasPart && (!_hga[i].IsDefect))
                    {
                        return i;
                    }
                }
                else
                {
                    if (_hga[i].HasPart)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }


        public PBACoordinate[] _unloadPos = new PBACoordinate[4];

    }
    #endregion


    // //////////////////////////////////////////////////////////////
    #region class HGSTTrayObj
    public class HGSTTrayObj
    {
        public HGAObj[] _hga = new HGAObj[40];
        public HGSTTrayObj()
        {
            for (int i = 0; i < 40; i++)
            {
                _hga[i] = new HGAObj();
                _hga[i].HasPart = false;
            }
        }

        public void LoadPart()
        {
            for (int i = 0; i < 40; i++)
            {
                _hga[i].HasPart = true;
            }
        }

        public void LoadPart(int pos)
        {
            _hga[pos].HasPart = true;
        }

        public void LoadAll()
        {
            for (int i = 0; i < 40; i++)
            {
                _hga[i].HasPart = true;
            }
        }

        public void UnloadPart(int pos)
        {
            _hga[pos].HasPart = false;
        }

        public void UnloadAll()
        {
            for (int i = 0; i < 40; i++)
            {
                _hga[i].HasPart = false;
            }
        }

        public int CountHGA()
        {
            int count = 0;
            for (int i = 0; i < 40; i++)
            {
                if (_hga[i].HasPart)
                {
                    count++;
                }
            }

            return count;
        }


        public int NextHGAToUnload()
        {
            for (int i = 0; i < 40; i++)
            {
                if (!_hga[i].HasPart)
                {
                    return i;
                }
            }

            return 0;
        }

        public PBACoordinate[] _unloadPos = new PBACoordinate[8];
    }
    #endregion


    // //////////////////////////////////////////////////////////////
    #region class PBACoordinate
    public class PBACoordinate
    {
        private double _x = 0.0;
        private double _y = 0.0;

        public PBACoordinate()
        {
        }

        public PBACoordinate(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public PBACoordinate(string strX, string strY)
        {
            _x = Convert.ToDouble(strX);
            _y = Convert.ToDouble(strY);
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
    }

    #endregion


    // //////////////////////////////////////////////////////////////
    #region class Nozzle
    public class Nozzle
    {
        bool _bUp = true;
        bool _bDown = false;

        bool _bExtend = false;
        bool _bRetract = true;

        public Nozzle()
        {
        }

        public bool MoveUp()
        {
            return IOs.Instance.NozzleUp();
        }

        public bool MoveDown()
        {
            return IOs.Instance.NozzleDown();
        }

        public bool Extend()
        {
            return IOs.Instance.NozzleExtend();
        }

        public bool Retract()
        {
            return IOs.Instance.NozzleRetract();
        }

        public bool VacAllOn()
        {
            return IOs.Instance.NozzleVacAllOn();
        }

        public bool VacAllOff()
        {
            return IOs.Instance.NozzleVacAllOff();
        }

    }

    #endregion


    // //////////////////////////////////////////////////////////////
    #region class IOs
    public class IOs
    {
        #region external Pci-Dask apis

        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int Register_Card(int cardType, int card_num);

        //Declare Function Release_Card Lib "Pci-Dask.dll" (ByVal CardNumber As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int Release_Card(int CardNumber);


        //Declare Function DO_WritePort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_WritePort(int CardNumber, int Port, int Value);

        //Declare Function DO_WriteLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, ByVal Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_WriteLine(int CardNumber, int Port, int Line, int Value);

        //Declare Function DO_ReadPort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_ReadPort(int CardNumber, int Port, ref int Value);

        //Declare Function DO_ReadLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DO_ReadLine(int CardNumber, int Port, int Line, int Value);


        //Declare Function DI_ReadPort Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, Value As Long) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DI_ReadPort(int CardNumber, int Port, ref int Value);

        //Declare Function DI_ReadLine Lib "Pci-Dask.dll" (ByVal CardNumber As Integer, ByVal Port As Integer, ByVal Line As Integer, Value As Integer) As Integer
        [DllImport(@"C:\Program Files\Common Files\ADLINK\PCI-DASK\WIN2K_XP\PCI-Dask.dll")]
        public static extern int DI_ReadLine(int CardNumber, int Port, int Line, int Value);



        #endregion

        #region singleton
        private static IOs _instance;
        public static IOs Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IOs();
                }
                return _instance;
            }
        }

        #endregion

        #region ctor
        private IOs()
        {
            for (int i = 0; i < 32; i++)
            {
                _oBits[i] = false;
            }


            _iBits[0] = 0x1;
            _iBits[1] = 0x2;
            _iBits[2] = 0x4;
            _iBits[3] = 0x8;
            _iBits[4] = 0x10;
            _iBits[5] = 0x20;
            _iBits[6] = 0x40;
            _iBits[7] = 0x80;
            _iBits[8] = 0x100;
            _iBits[9] = 0x200;

            _iBits[10] = 0x400;
            _iBits[11] = 0x800;
            _iBits[12] = 0x1000;
            _iBits[13] = 0x2000;
            _iBits[14] = 0x4000;
            _iBits[15] = 0x8000;
            _iBits[16] = 0x10000;
            _iBits[17] = 0x20000;
            _iBits[18] = 0x40000;
            _iBits[19] = 0x80000;

            _iBits[20] = 0x100000;
            _iBits[21] = 0x200000;
            _iBits[22] = 0x400000;
            _iBits[23] = 0x800000;
            _iBits[24] = 0x1000000;
            _iBits[25] = 0x2000000;
            _iBits[26] = 0x4000000;
            _iBits[27] = 0x8000000;
            _iBits[28] = 0x10000000;
            _iBits[29] = 0x20000000;

            _iBits[30] = 0x40000000;
            _iBits[31] = 0x80000000;

        }

        #endregion

        public static uint[] _iBits = new uint[32];


        public static bool[] _oBits = new bool[32];
        private int _driveOutput()
        {
            int nOutput = 0;
            for (int i = 0; i < 32; i++)
            {
                if (_oBits[i] == true)
                {
                    nOutput += (int)System.Math.Pow(2, i);
                }
            }

            return nOutput;
        }

        public bool PBAOn()
        {
            _oBits[22] = true;      //PBA on
            DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool PBAOff()
        {
            _oBits[22] = false;      //PBA on
            DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacAllOn()
        {
            _oBits[5] = true;   //nozzle#5
            _oBits[6] = true;   //nozzle#4
            _oBits[7] = true;   //nozzle#3
            _oBits[8] = true;   //nozzle#2
            _oBits[9] = true;   //nozzle#1

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOnOff_1(bool bOn)
        {
            _oBits[9] = bOn;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOnOff_2(bool bOn)
        {
            _oBits[8] = bOn;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOnOff_3(bool bOn)
        {
            _oBits[7] = bOn;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOnOff_4(bool bOn)
        {
            _oBits[6] = bOn;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOnOff_5(bool bOn)
        {
            _oBits[5] = bOn;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleVacuumOn(int nPosition)
        {
            //0 = position1 = bit9, 1 = position2 = bit8, etc.
            switch (nPosition)
            {
                case 0:
                    _oBits[9] = true;
                    break;

                case 1:
                    _oBits[8] = true;
                    break;

                case 2:
                    _oBits[7] = true;
                    break;

                case 3:
                    _oBits[6] = true;
                    break;

                case 4:
                    _oBits[5] = true;
                    break;

                default:
                    break;
            }
            
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }


        public bool NozzleVacuumOff(int nPosition)
        {
            //0 = position1 = bit9, 1 = position2 = bit8, etc.
            switch (nPosition)
            {
                case 0:
                    _oBits[9] = false;
                    break;

                case 1:
                    _oBits[8] = false;
                    break;

                case 2:
                    _oBits[7] = false;
                    break;

                case 3:
                    _oBits[6] = false;
                    break;

                case 4:
                    _oBits[5] = false;
                    break;

                default:
                    break;
            }

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }


        public bool NozzleVacAllOff()
        {
            _oBits[5] = false;
            _oBits[6] = false;
            _oBits[7] = false;
            _oBits[8] = false;
            _oBits[9] = false;

            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleExtend()
        {
            _oBits[0] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsNozzleExtend()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) /*& _iBits[1]*/ & _iBits[2];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool NozzleRetract()
        {
            _oBits[0] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool NozzleUp()
        {
            _oBits[15] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsNozzleUp()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[10];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool NozzleDown()
        {
            _oBits[15] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsNozzleDown()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[11];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool PalletClampsDown()
        {
            _oBits[27] = true;
            _oBits[28] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool Tray1In()
        {
            _oBits[17] = true;
            _oBits[16] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsTray1In()
        {
            //If DIn(12) = True Then
            //ImgIntray1.Picture = ImgOn.Picture
            //Else
            //ImgIntray1.Picture = ImgOff.Picture
            //End If

            //If DIn(13) = True Then
            //ImgIntray2.Picture = ImgOn.Picture
            //Else
            //ImgIntray2.Picture = ImgOff.Picture
            //End If

            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[12];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool Tray2In()
        {
            _oBits[17] = false;
            _oBits[16] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsTray2In()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[13];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool IsTurnTraySWOn()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[14];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool TurnTraySWLightOn()
        {
            _oBits[21] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool TurnTraySWLightOff()
        {
            _oBits[21] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool IsPush1Activated()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[16];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool Push1SWLightOn()
        {
            _oBits[19] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool Push1SWLightOff()
        {
            _oBits[19] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }
        
        public bool IsPush2Activated()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[17];
            if (P != 0)
            {
                return true;
            }

            return false;
        }

        public bool Push2SWLightOn()
        {
            _oBits[20] = true;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool Push2SWLightOff()
        {
            _oBits[20] = false;
            int nRet = DO_WritePort(0, 0, _driveOutput());
            return true;
        }

        public bool _bCtrlPressed = false;
        public bool IsCtrlPressd()
        {
            return _bCtrlPressed;
        }


        public bool IsAreaSensorOn()
        {
            int nDIData = 0;
            int nRet = DI_ReadPort(0, 0, ref nDIData);
            uint P = 0;
            P = ((uint)nDIData) & _iBits[3];
            if (P != 0)
            {
                return true;
            }

            return false;
        }
    }

    #endregion


    // //////////////////////////////////////////////////////////////
    public class UnOCRStopWatch
    {
        private DateTime _dt = new DateTime();
        public UnOCRStopWatch()
        {
            _bStopCount = false;
            this.Start();
            this.Stop();
        }

        public void Start()
        {
            _dt = System.DateTime.Now;
            _bStopCount = false;
        }

        private bool _bStopCount = false;
        public void Stop()
        {
            if (!_bStopCount)
            {
                _bStopCount = true;

                TimeSpan sp = System.DateTime.Now - _dt;

                _lCummTicks = sp.Ticks + _lCummTicks;
                _lCummTotalDays = sp.TotalDays + _lCummTotalDays;
                _lCummTotalHous = sp.TotalHours + _lCummTotalHous;
            }
        }

        public bool IsStopCount()
        {
            return _bStopCount;
        }
        
        public long Ticks()
        {
            TimeSpan sp = System.DateTime.Now - _dt;
            return sp.Ticks;
        }


        private long _lCummTicks = 0;
        public long CummTicks()
        {
            if (!_bStopCount)
            {
                TimeSpan sp = System.DateTime.Now - _dt;

                return sp.Ticks + _lCummTicks;
            }

            return _lCummTicks;
        }


        private double _lCummTotalDays = 0.0;
        public double TotalDays()
        {
            if (!_bStopCount)
            {
                TimeSpan sp = System.DateTime.Now - _dt;

                Console.WriteLine((sp.TotalDays + _lCummTotalDays).ToString());
                return sp.TotalDays + _lCummTotalDays;
            }

            Console.WriteLine(_lCummTotalHous.ToString());
            return _lCummTotalDays;
        }


        private double _lCummTotalHous = 0.0;
        public double TotalHours()
        {
            if (!_bStopCount)
            {
                TimeSpan sp = System.DateTime.Now - _dt;

                Console.WriteLine((sp.TotalHours + _lCummTotalHous).ToString());
                return sp.TotalHours + _lCummTotalHous;
            }

            Console.WriteLine(_lCummTotalHous.ToString());
            return _lCummTotalHous;
        }

        public void Reset()
        {
            _dt = System.DateTime.Now;
            _lCummTicks = 0;
            _lCummTotalDays = 0;
            _lCummTotalHous = 0;
        }
    }


    // //////////////////////////////////////////////////////////////
    #region class SICKScannerObj
    public class SICKScannerObj
    {
        #region singleton
        private static SICKScannerObj _instance;
        public static SICKScannerObj Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SICKScannerObj();
                }
                return _instance;
            }
        }

        #endregion


        private System.IO.Ports.SerialPort _serialPort;

        #region ctor
        private SICKScannerObj()
        {
            _serialPort = new SerialPort();

            _serialPort.PortName = "COM4";   //default
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;

            _serialPort.ReadBufferSize = 16;
        }

        #endregion

        public string COMPort
        {
            get { return _serialPort.PortName; }
            set { _serialPort.PortName = value; }
        }

        public string ReadBarcode()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                string strRead = "";

                //this region is no use
                //set the scanner to read mode, STX,11,ETX
                //byte[] arrReadMode = new byte[] { 0x02, 0x31, 0x31, 0x03 };
                //serialPort1.Write(arrReadMode, 0, arrReadMode.Length);
                //serialPort1.ReadExisting();
                //serialPort1.DiscardInBuffer();
                //-------------------------------------


                //send trigger to scanner, STX,21,ETX
                byte[] arrReadBarcode = new byte[] { 0x02, 0x32, 0x31, 0x03 };
                _serialPort.Write(arrReadBarcode, 0, arrReadBarcode.Length);


                System.Threading.Thread.Sleep(200);
                strRead = _serialPort.ReadExisting();           //read barcode

                string[] arrBarcode1 = strRead.Split((char)2);   //split STX or 0x02
                string[] arrBarcode2 = arrBarcode1[arrBarcode1.Length - 1].Split((char)3);   //split ETX or 0x03


                _serialPort.Close();
                return arrBarcode2[0];
            }
            catch (Exception ex)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                Console.WriteLine("Exception: " + ex.Message);
                return "Error";
            }
        }

    }

    #endregion
}
