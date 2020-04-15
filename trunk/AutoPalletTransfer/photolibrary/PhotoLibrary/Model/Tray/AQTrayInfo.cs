using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhotoLibrary.Helpers;

namespace PhotoLibrary.Model
{
    public class AQTrayInfo
    {
        public string TrayId { get; set; }
        public List<string> PalletIdList { get; set; }
        public List<string> HGAIdList { get; set; }
        public List<string> StatusList { get; set; }
        public string AQHeader { get; set; }
        public int NumHGA { get; set; }

        public const string NOHGA = "";
        public const string GOODHGA_CANNOTREAD_SERIAL_8OCR = "????????";
        public const string GOODHGA_CANNOTREAD_SERIAL_10OCR = "??????????";
        public const string NOHGA_SERIAL_8OCR = "--------";
        public const string NOHGA_SERIAL_10OCR = "----------";
        public string BasePathEasy { get; private set; }
        public string BasePathMt2 { get; private set; }
        public string BasePathMirror { get; private set; }
        public string BasePathBackup { get; private set; }

        public AQTrayInfo()
        {
            PalletIdList = new List<string>();
            HGAIdList = new List<string>();
            StatusList = new List<string>();

            ConfigXML config = new ConfigXML();
            BasePathEasy = config.AQTrayPath;
            BasePathMt2 = config.AQTrayPathMt2;
            BasePathMirror = config.AQTrayPathMirror;
            BasePathBackup = config.AQTrayPathBackup;
        }

        public static AQTrayInfo ReadAQTray(string data)
        {
            AQTrayInfo trayinfo = new AQTrayInfo();
            string[] lines = data.Split('\n', '\r');
            foreach (string line in lines)
            {
                if (line.StartsWith("TRAYID="))
                {
                    string trayId = line.Substring(line.IndexOf("=") + 1);
                    if (trayId.Length != 10)
                    {
                        LogHelper.AppendWarningFile(string.Format("Tray ID length is incorrect ({0})", line));
                    }
                    else
                    {
                        trayinfo.TrayId = trayId;
                    }
                }
                else if (line.StartsWith("HGAN"))
                {
                    char[] delimiter = new char[] { '=', ',' };
                    string[] result = line.Split(delimiter, 3, StringSplitOptions.None);

                    if (result.Length < 3)
                    {
                        LogHelper.AppendWarningFile(string.Format("HGAN and serial is incorrect format ({0})", line));
                    }
                    else
                    {
                        trayinfo.HGAIdList.Add(result[1]);
                        trayinfo.StatusList.Add(result[2]);
                    }
                }
            }

            int serial_position = data.IndexOf("[SERIAL]") + 8;
            trayinfo.AQHeader = data.Substring(0, serial_position) + Environment.NewLine;

            return trayinfo;
        }

        /// <summary>
        /// Write HGAIdList to AQ tray file. HGAIdList must have 20 HGAs or 40, 60, etc.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool WriteToAQTrayFile(string basePath, string filename)
        {
            string data = this.AQHeader;
            for (int i = 0; i < HGAIdList.Count; i++)
            {
                data += string.Format("HGAN{0}={1}", (i + 1), HGAIdList[i]) + Environment.NewLine;
            }
            string filepath = Path.Combine(basePath, filename);
            try
            {
                // Does folder exist?
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                using (var sw = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    sw.Write(data);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendErrorFile("File already exists (" + filepath + ") - " + ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write AQ tray file according to StatusList (match 1-1 to HGAIdList)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool WriteToAQTrayFileWithStatusList(string filename)
        {
            if (StatusList.Count != HGAIdList.Count) return false;

            string data = this.AQHeader;
            for (int i = 0; i < HGAIdList.Count; i++)
            {
                if (StatusList[i].ToUpper() == "GOOD")
                {
                    data += string.Format("HGAN{0}={1}", (i + 1), HGAIdList[i]) + Environment.NewLine;
                }
                else
                {
                    data += string.Format("HGAN{0}={1}", (i + 1), NOHGA) + Environment.NewLine;
                }
            }
            try
            {
                var fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write);
                using (var sw = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    sw.Write(data);
                }
            }
            catch (IOException ex)
            {
                LogHelper.AppendErrorFile("File already exists " + ex.ToString());
                return false;
            }
            return true;
        }

        [Obsolete("Not use", true)]
        public TrayObj ConvertToTrayObj()
        {
            TrayObj obj = new TrayObj();

            obj.Date = DateTime.Now.Date.ToString("dd/mm/yyyy");
            obj.SetTimeStart(DateTime.Now);
            obj.SetTimeEnd(DateTime.Now);
            obj.TrayID = this.TrayId;
            obj.Pallet1 = this.PalletIdList[0];
            obj.Pallet2 = this.PalletIdList[1];

            obj.HGAN1 = this.HGAIdList[0];
            obj.HGAN2 = this.HGAIdList[1];
            obj.HGAN3 = this.HGAIdList[2];
            obj.HGAN4 = this.HGAIdList[3];
            obj.HGAN5 = this.HGAIdList[4];
            obj.HGAN6 = this.HGAIdList[5];
            obj.HGAN7 = this.HGAIdList[6];
            obj.HGAN8 = this.HGAIdList[7];
            obj.HGAN9 = this.HGAIdList[8];
            obj.HGAN10 = this.HGAIdList[9];
            obj.HGAN11 = this.HGAIdList[10];
            obj.HGAN12 = this.HGAIdList[11];
            obj.HGAN13 = this.HGAIdList[12];
            obj.HGAN14 = this.HGAIdList[13];
            obj.HGAN15 = this.HGAIdList[14];
            obj.HGAN16 = this.HGAIdList[15];
            obj.HGAN17 = this.HGAIdList[16];
            obj.HGAN18 = this.HGAIdList[17];
            obj.HGAN19 = this.HGAIdList[18];
            obj.HGAN20 = this.HGAIdList[19];

            return obj;
        }
    }
}
