using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using System.Runtime.InteropServices;

using System.IO;
using System.Globalization;

namespace IAVICleanUpService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };  


    // ////////////////////////////////////////////////////////////////////////////////////////
    public partial class IAVICleanUpService : ServiceBase
    {
        private System.Diagnostics.EventLog _srvcEvntLog;
        private int _eventId = 1;

        public IAVICleanUpService()
        {
            InitializeComponent();

            _srvcEvntLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("IAVICleanUpServiceSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("IAVICleanUpServiceSource", "IAVICleanUpServiceLog");
            }
            _srvcEvntLog.Source = "IAVICleanUpServiceSource";
            _srvcEvntLog.Log = "IAVICleanUpServiceLog";
        }


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);  


            //The OnStart method must return to the operating system after the service's operation has begun.
            //It must not loop forever or block. 

            _srvcEvntLog.WriteEntry("In OnStart");

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 600000; // 600 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();


            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus); 
        }

        protected override void OnStop()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus); 

            _srvcEvntLog.WriteEntry("In onStop.");

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus); 
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            bool bNeedToClean = false;
            _srvcEvntLog.WriteEntry(@"Monitoring Hard Drive Space.\r\n" + this.CheckDrives(out bNeedToClean), EventLogEntryType.Information, _eventId++);

            if (bNeedToClean)
            {
                //Clean Drive F:\
                _srvcEvntLog.WriteEntry(@"Space in drive F:\ is less 50%.\r\n" + this.CleanDriveF(), EventLogEntryType.Warning, _eventId++);
            }
        }
  
        public string CheckDrives(out bool bNeedToClean)
        {
            StringBuilder sb = new StringBuilder();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            bNeedToClean = false;

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady)
                {
                    sb.AppendLine("Drive " + d.Name);                                                                                                  
                    sb.AppendLine("Drive Type " + d.DriveType);                                                                                        

                    sb.AppendLine("Available free space: " + d.AvailableFreeSpace.ToString("##,#", new System.Globalization.CultureInfo("en-US")));    
                    sb.AppendLine("Total available space: " + d.TotalFreeSpace.ToString("##,#", new System.Globalization.CultureInfo("en-US")));       
                    sb.AppendLine("Total size: " + d.TotalSize.ToString("##,#", new System.Globalization.CultureInfo("en-US")));                       

                    double dblRatio = (double) d.AvailableFreeSpace / d.TotalSize;
                    sb.AppendLine("Free to Total Ratio: " + dblRatio.ToString("#0.0##", new System.Globalization.CultureInfo("en-US")));
                    sb.AppendLine("");

                    if ((d.Name == @"F:\") && (dblRatio < 0.5)) //less than 50% of total space
                    {
                        bNeedToClean = true;
                    }
                }
            }

            return sb.ToString();
        }


        public string CleanDriveF()
        {
            StringBuilder sb = new StringBuilder();
            DateTime dtOlderThan24Hours = DateTime.Now.Subtract(new TimeSpan(24, 0, 0));
            DateTime dtOlderThanOneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));


            #region F:\ImagesBMP
            DirectoryInfo baseDirBMP = new DirectoryInfo(@"F:\ImagesBMP");
            //DirectoryInfo baseDirBMP = new DirectoryInfo(@"D:\temp\F\ImagesBMP");
            //DirectoryInfo baseDirBMP = new DirectoryInfo(@"D:\temp");
            if (baseDirBMP.Exists)
            {
                sb.AppendLine(@"Check F:\ImagesBMP");
                sb.AppendLine("Delete if older than " + dtOlderThan24Hours.ToString("yyyyMMdd HH:mm:ss.fff"));
                DirectoryInfo[] subDirImagesBMP = baseDirBMP.GetDirectories();
                foreach (DirectoryInfo d in subDirImagesBMP)
                {
                    if (d.CreationTime < dtOlderThan24Hours)
                    {
                        sb.AppendLine("Delete " + d.FullName);
                        d.Delete(true);
                    }
                    else
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Delete if older than " + dtOlderThanOneHour.ToString("yyyyMMdd HH:mm:ss.fff"));

                        DirectoryInfo[] baseSubDirs = d.GetDirectories();
                        foreach (DirectoryInfo subdir in baseSubDirs)
                        {
                            //Console.WriteLine(dtOlderThanOneHour.ToString("yyyyMMdd HH:mm:ss.fff"));
                            if (subdir.CreationTime < dtOlderThanOneHour)        //older than one hour
                            {
                                sb.AppendLine("Delete " + subdir.FullName);
                                subdir.Delete(true);
                            }
                        }
                    }
                }
            }

            #endregion



            #region F:\ImagesJPG
            sb.AppendLine("");

            DirectoryInfo baseDirJPG = new DirectoryInfo(@"F:\ImagesJPG");
            if (baseDirJPG.Exists)
            {
                sb.AppendLine(@"Check F:\ImagesJPG");
                sb.AppendLine("Delete if older than " + dtOlderThan24Hours.ToString("yyyyMMdd HH:mm:ss.fff"));
                DirectoryInfo[] subDirImagesJPG = baseDirJPG.GetDirectories();
                foreach (DirectoryInfo d in subDirImagesJPG)
                {
                    if (d.CreationTime < dtOlderThan24Hours)
                    {
                        sb.AppendLine("Delete " + d.FullName);
                        d.Delete(true);
                    }
                    else
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Delete if older than " + dtOlderThanOneHour.ToString("yyyyMMdd HH:mm:ss.fff"));

                        DirectoryInfo[] baseSubDirs = d.GetDirectories();
                        foreach (DirectoryInfo subdir in baseSubDirs)
                        {
                            //Console.WriteLine(dtOlderThanOneHour.ToString("yyyyMMdd HH:mm:ss.fff"));
                            if (subdir.CreationTime < dtOlderThanOneHour)        //older than one hour
                            {
                                sb.AppendLine("Delete " + subdir.FullName);
                                subdir.Delete(true);
                            }
                        }
                    }
                }
            }

            #endregion

            return sb.ToString();
        }
    }
}
