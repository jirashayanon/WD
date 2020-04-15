using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PhotoLibrary.Helpers
{
    internal class ConfigXML : IConfigXML
    {
        private const string filename = "config.xml";
        private XElement config;
        private string serverIP;
        private string username;
        private string password;
        private string database;
        private string path;
        private string jpgpath;
        private bool _bJPG;
        private string pathEvaluation;
        private int maxPictureOrder;
        private int startPictureId;
        private int timeout;
        private int thumbnailWidth;
        private int imageWidth;
        private List<string> listView;
        private List<String> listMap;
        private List<string> filetype;
        private int numberOfHgaOnTray;

        public string AQTrayPath { get; set; }
        public string AQTrayPathMt2 { get; set; }
        public string AQTrayPathMirror { get; set; }
        public string AQTrayPathBackup { get; set; }
        public string PhotoFileType { get; set; }

        public int SamplingPlan;
        public int SamplingPlanTotal;

        public ConfigXML()
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), filename);
            if (File.Exists(filepath))
            {
                /*using (XmlReader reader = XmlReader.Create(filepath))
                {
                    config = XElement.Load(reader);
                    config = XElement.Load(reader);
                }
                 */
                using (StreamReader sr = new StreamReader(filepath))
                {
                    XmlDocument docXML = new XmlDocument();
                    docXML.LoadXml(sr.ReadToEnd().Trim());
                    XDocument xDoc = XDocument.Load(new XmlNodeReader(docXML));
                    config = xDoc.Root;
                }
            }
            else
            {
                config = new XElement("PhotoLibrary",
                                new XComment("Path for evaluating images"),
                                new XElement("PathEvaluation", @"D:\LocalFile\"),

                                new XComment("Path for real images on server"),
                                new XElement("Path", @"D:\Server\"),
                                new XElement("PathAQTray", @"TrayData"),
                                new XElement("PathAQTrayMt2", @"TrayDataMt2"),
                                new XElement("PathAQTrayMirror", @"TrayDataMirror"),
                                new XElement("PathAQTrayBackup", @"TrayData_Backup"),

                                new XComment("FileType for photo path from database"),
                                new XElement("PhotoFileType", @"bmp"),

                                new XElement("ServerConnection",
                                    new XElement("ServerIP", @"127.0.0.1"),
                                    new XElement("Username", @"iaviuser"),
                                    new XElement("Password", @"XuPCcdJJFHt8Dh8E"),
                                    new XElement("Database", @"iavi")
                                ),
                                new XElement("NumberOfHgaOnTray", "20"),
                                new XComment("Sampling"),
                                new XElement("Sampling",
                                    new XElement("Sampling-Plan", "3"),
                                    new XElement("Sampling-Total", "6")
                                ),
                                new XComment("View for evaluation only (this element is not used in production server)"),
                                new XElement("Views",
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Top View2"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Front View"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "45 deg View"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Top View3"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Top View4"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Top View5"),
                                    new XElement("View", new XAttribute("filetype", "bmp"), "Back View"),
                                    new XElement("View", new XAttribute("filetype", "jpeg"), "VOR Color/Picture Top")
                                ),

                                new XElement("Maps",
                                    new XComment("Type: 20, 60"),
                                    new XElement("Map", new XAttribute("type", "20"), "ImagePath1"),
                                    new XElement("Map", new XAttribute("type", "60"), "ImagePath2")
                                ),
                                new XElement("Picture",
                                    new XComment("Start picture number"),
                                    new XElement("StartPictureId", "5"),
                                    new XComment("How many would like to evaluate"),
                                    new XElement("Max", "5"),
                                    new XComment("Don't change these elements below"),
                                    new XElement("ThumbnailWidth", "150"),
                                    new XElement("ImageWidth", "1024")
                                ),
                                new XElement("Time",
                                    new XElement("Interval", "1000"),
                                    new XElement("Timeout", "60")
                                )
                            );
                config.Save(filepath);
            }

            serverIP = config.Element("ServerConnection").Element("ServerIP").Value;
            username = config.Element("ServerConnection").Element("Username").Value;
            password = config.Element("ServerConnection").Element("Password").Value;
            database = config.Element("ServerConnection").Element("Database").Value;
            PhotoFileType = config.Element("PhotoFileType").Value;

            AQTrayPath = config.Element("PathAQTray").Value;
            AQTrayPathMt2 = config.Element("PathAQTrayMt2").Value;
            AQTrayPathMirror = config.Element("PathAQTrayMirror").Value;
            AQTrayPathBackup = config.Element("PathAQTrayBackup").Value;

            string s = config.Element("NumberOfHgaOnTray").Value;
            if (!int.TryParse(config.Element("NumberOfHgaOnTray").Value, out numberOfHgaOnTray))
            {
                Debug.WriteLine("NumberOfHgaOnTray is wrong");
                LogHelper.AppendErrorFile("NumberOfHgaOnTray is wrong");
            }

            path = config.Element("Path").Value;
            pathEvaluation = config.Element("PathEvaluation").Value;
            jpgpath = config.Element("JPGPath").Value;
            _bJPG = bool.Parse(config.Element("JPG").Value);

            if (!int.TryParse(config.Element("Picture").Element("StartPictureId").Value, out startPictureId))
            {
                Debug.WriteLine("StartPictureId is wrong");
                LogHelper.AppendErrorFile("StartPictureId is wrong");
            }

            if (!int.TryParse(config.Element("Picture").Element("Max").Value, out maxPictureOrder))
            {
                Debug.WriteLine("Max View is wrong");
                LogHelper.AppendErrorFile("Max View is wrong");
            }

            if (!int.TryParse(config.Element("Time").Element("Timeout").Value, out timeout))
            {
                Debug.WriteLine("Timeout's wrong");
                LogHelper.AppendErrorFile("Timeout's wrong");
            }

            listView = new List<string>();
            listMap = new List<string>();
            filetype = new List<string>();
            foreach (XElement x in config.Elements("Views").Elements("View"))
            {
                if (x.Name.ToString() == "View")
                {
                    listView.Add(x.Value);
                    filetype.Add(x.Attribute("filetype").Value);
                }
            }
            foreach (XElement x in config.Elements("Maps").Elements("Map"))
            {
                if (x.Name.ToString() == "Map")
                {
                    listMap.Add(x.Value);
                    filetype.Add(x.Attribute("type").Value);
                }
            }

            if (!int.TryParse(config.Element("Picture").Element("ThumbnailWidth").Value, out thumbnailWidth))
            {
                Debug.WriteLine("ThumbnailWidth is wrong");
                LogHelper.AppendErrorFile("ThumbnailWidth is wrong");
            }

            if (!int.TryParse(config.Element("Picture").Element("ImageWidth").Value, out imageWidth))
            {
                Debug.WriteLine("ImageWidth is wrong");
                LogHelper.AppendErrorFile("ImageWidth is wrong");
            }


            if (!int.TryParse(config.Element("Sampling").Element("Sampling-Plan").Value, out SamplingPlan))
            {
                LogHelper.AppendErrorFile("Sampling-Plan is wrong");
            }

            if (!int.TryParse(config.Element("Sampling").Element("Sampling-Total").Value, out SamplingPlanTotal))
            {
                LogHelper.AppendErrorFile("Sampling-Total is wrong");
            }

            if (SamplingPlanTotal % SamplingPlan > 0)
            {
                LogHelper.AppendErrorFile("Sampling-Total is not multiple of Sampling-Plan");
                SamplingPlan = 3;
                SamplingPlanTotal = 6;
            }
        }

        public string ServerIP()
        {
            return serverIP;
        }
        public string UserName()
        {
            return username;
        }
        public string Password()
        {
            return password;
        }
        public string Database()
        {
            return database;
        }
        public string GetPath()
        {
            return path;
        }

        public string GetJPGPath()
        {
            return jpgpath;
        }

        public int GetNumberOfHgaOnTray()
        {
            return numberOfHgaOnTray;
        }

        public string GetPathEvaluation()
        {
            return pathEvaluation;
        }

        public int GetStartPictureId()
        {
            return startPictureId;
        }

        public int GetMaxPictureOrder()
        {
            return maxPictureOrder;
        }

        public List<string> GetListView()
        {
            return listView;
        }

        public List<string> GetListMap()
        {
            return listMap;
        }

        public int GetTimeout()
        {
            return timeout;
        }

        internal List<string> GetFiletype()
        {
            return filetype;
        }

        public int GetThumbnailWidth()
        {
            return thumbnailWidth;
        }

        public int GetImageWidth()
        {
            return imageWidth;
        }

        public bool IsUsingJPG()
        {
            return _bJPG;
        }
    }
}
