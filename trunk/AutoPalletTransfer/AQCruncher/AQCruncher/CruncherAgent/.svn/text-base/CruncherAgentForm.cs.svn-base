using System;
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

using System.Threading;
using log4net;


namespace CruncherAgent
{
    public partial class CruncherAgentForm : Form
    {
        private Thread _agentThread = null;
        private int _nCruncherTimer = 30;    //140 seconds

        private AQCruncherConfig _config = new AQCruncherConfig();

        //public AQCruncher.LoggerClass logger;

        #region ctor
        private static CruncherAgentForm _instance;
        public static CruncherAgentForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CruncherAgentForm();
                }

                return _instance;
            }
        }

        //public override DialogResult ShowDialog()
        //{
        //    return base.ShowDialog();
        //}

        private CruncherAgentForm()
        {
            InitializeComponent();

            var _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += @" " + _version;
        }

        #endregion


        private void _init()
        {
            _config = AQCruncherConfig.ReadConfig(@"AQCruncherConfig.xml");

            _agentThread = new Thread(StartMonitor);
            _agentThread.Name = "agentThread";
            _agentThread.Priority = ThreadPriority.Normal;
            _agentThread.Start();

            //logger = AQCruncher.LoggerClass.Instance;
        }
        
        
        private void CruncherAgentForm_Load(object sender, EventArgs e)
        {
            _init();
        }


        private void StartMonitor()
        {
            while (true)
            {
                string exePath = System.Windows.Forms.Application.StartupPath;
                string crunchProg = exePath + @"\AQCruncher.exe";

                string afterCrunchFolder = exePath + @"\Crunched";

                //string[] unloadfiles = System.IO.Directory.GetFiles(exePath, "*.XML");
                string[] unloadfiles = System.IO.Directory.GetFiles(_config.UNLOADFilePath, "*.UNL");
                try
                {
                    foreach (string file in unloadfiles)
                    {
                        //System.Diagnostics.Process.Start(crunchProg, file);
                        System.Diagnostics.Process.Start(crunchProg, file + " " + afterCrunchFolder);

                        ListViewItem lstItem = new ListViewItem();
                        //lstItem.Text = file.Replace(exePath,"");
                        //lstItem.Text = lstItem.Text.Replace(@"\", "");

                        string[] temp = file.Split('\\');
                        lstItem.Text = temp[temp.Length - 1];   //remove path, keep only filename in the list

                        lstItem.SubItems.Add(DateTime.Now.ToString("M/d/yyyy h:mm:ss tt"));
                        DelegateUpdateTrayHistory(lstItem);

                        AQCruncher.LoggerClass.Instance.TrayLogInfo(lstItem.Text + "," + lstItem.SubItems[1].Text);
                        //logger.TrayLogInfo(lstItem.Text + "," + lstItem.SubItems[1].Text);
                    }
                }
                catch (Exception ex)
                {
                    AQCruncher.LoggerClass.Instance.ErrorLogInfo(ex.Message);
                    //logger.ErrorLogInfo(ex.Message);
                }


                Application.DoEvents();
                Thread.Sleep(_nCruncherTimer * 1000);
            }
        }

        private void CruncherAgentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfirmExitForm confirm = new ConfirmExitForm();
            if (confirm.ShowDialog() != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }            
            
            if (_agentThread != null)
            {
                _agentThread.Abort();
            }

            System.Environment.Exit(0);
        }


        #region delegate UpdateTrayHistory
        private delegate void updateTrayHistory(ListViewItem lstTray);
        private void UpdateTrayHistory(ListViewItem lstTray)
        {
            listView_TrayHistory.Items.Insert(0, lstTray);
            listView_TrayHistory.Items[0].Selected = true;

            //maintain only top 20 latest messages
            if (listView_TrayHistory.Items.Count > 100)
            {
                listView_TrayHistory.Items.Remove(listView_TrayHistory.Items[100]);
            }

            listView_TrayHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView_TrayHistory.Refresh();
        }
        public void DelegateUpdateTrayHistory(ListViewItem lstTray)
        {
            if (this.IsHandleCreated)
            {
                Invoke(new updateTrayHistory(UpdateTrayHistory), lstTray);
            }
            else
            {
                UpdateTrayHistory(lstTray);
            }
        }
        #endregion

        private void btnClearTrayList_Click(object sender, EventArgs e)
        {
            if (listView_TrayHistory.Items.Count > 0)
            {
                listView_TrayHistory.Items.Clear();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIconCruncherAgent.Visible = false;

            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void CruncherAgentForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIconCruncherAgent.BalloonTipText = "CruncherAgent still running...";
                notifyIconCruncherAgent.BalloonTipTitle = "CruncherAgent";

                notifyIconCruncherAgent.Visible = true;
                notifyIconCruncherAgent.ShowBalloonTip(500);
                this.Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CruncherAgentForm_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, true));
        }

    }


    // ////////////////////////////////////////////////////////////////////////////////////
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


    // ////////////////////////////////////////////////////////////////////////////////////
    //#region class LoggerClass

    //public class LoggerClass
    //{
    //    private readonly log4net.ILog MainLogger;
    //    private readonly log4net.ILog ErrorLogger;
    //    private readonly log4net.ILog TrayLogger;

    //    private static LoggerClass _instance;
    //    public static LoggerClass Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                _instance = new LoggerClass();
    //            }

    //            return _instance;
    //        }
    //    }

    //    private LoggerClass()
    //    {
    //        MainLogger = log4net.LogManager.GetLogger("MainApp");
    //        ErrorLogger = log4net.LogManager.GetLogger("Error");
    //        TrayLogger = log4net.LogManager.GetLogger("TrayLog");

    //        string exePath = System.Windows.Forms.Application.StartupPath;
    //        System.IO.FileInfo logconfig = new System.IO.FileInfo(exePath + @"\logger.config");
    //        log4net.Config.XmlConfigurator.Configure(logconfig);

    //        MainLogger.Info("Logger ctor");
    //        ErrorLogger.Info("ErrorLogger init");
    //        TrayLogger.Info("TrayLogger init");
    //    }

    //    public void MainLogInfo(string strLogMessage)
    //    {
    //        this.MainLogger.Info(strLogMessage);
    //    }

    //    public void ErrorLogInfo(string strLogMessage)
    //    {
    //        this.ErrorLogger.Info(getStackInfo() + ": " + strLogMessage);
    //    }

    //    public void TrayLogInfo(string strLogMesssage)
    //    {
    //        this.TrayLogger.Info(getStackInfo() + ": " + strLogMesssage);
    //    }


    //    private string getStackInfo()
    //    {
    //        StackTrace stackTrace = new StackTrace(0, true);                        // get call stack
    //        StackFrame[] stackFrames = stackTrace.GetFrames();                      // get method calls (frames)
    //        string callerfilename = "[" + stackFrames[2].GetFileName() + "]";
    //        string caller = "[" + stackFrames[2].GetMethod().Name + ":line " + stackFrames[2].GetFileLineNumber().ToString() + "]:";
    //        caller += "[" + stackFrames[1].GetMethod().Name + ":line " + stackFrames[1].GetFileLineNumber().ToString() + "]:";

    //        return callerfilename + caller;
    //    }
    //}

    //#endregion


}
