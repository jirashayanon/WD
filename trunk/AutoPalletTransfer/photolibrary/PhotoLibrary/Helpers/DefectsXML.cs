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
    internal class DefectsXML : IDefectsXML
    {
        private const string filename = "Defects.xml";
        private XElement config;

        private List<string> Suspension;
        private List<string> Attach;
        private List<string> SJB;
        private List<string> OTH;

        public DefectsXML()
        {
            string filepath = Directory.GetCurrentDirectory() + "/" + filename;
            if (File.Exists(filepath))
            {
                //XmlReader reader = XmlReader.Create(filepath);
                //config = XElement.Load(reader);
                using (Stream s = File.OpenRead(filepath))
                {
                    config = XElement.Load(s);
                }
            }
            else
            {
                config = new XElement("Defects",
                                new XElement("Suspension",
                                    new XElement("PB2"),
                                    new XElement("PB3"),
                                    new XElement("S2"),
                                    new XElement("S3"),
                                    new XElement("S3T"),
                                    new XElement("S4"),
                                    new XElement("T2"),
                                    new XElement("T2A"),
                                    new XElement("F2"),
                                    new XElement("PS0")
                                ),
                                new XElement("Attach",
                                    new XElement("A1"),
                                    new XElement("A3"),
                                    new XElement("A5"),
                                    new XElement("D1"),
                                    new XElement("E1S"),
                                    new XElement("E2"),
                                    new XElement("E2A"),
                                    new XElement("E2F"),
                                    new XElement("E4"),
                                    new XElement("E5"),
                                    new XElement("E6"),
                                    new XElement("E6S"),
                                    new XElement("S1"),
                                    new XElement("S2D"),
                                    new XElement("SB1"),
                                    new XElement("SB2"),
                                    new XElement("T1"),
                                    new XElement("T6"),
                                    new XElement("T8")
                                ),
                                new XElement("SJB",
                                    new XElement("J1O"),
                                    new XElement("J2"),
                                    new XElement("J4A"),
                                    new XElement("J4B"),
                                    new XElement("J4O"),
                                    new XElement("J4P"),
                                    new XElement("J4T"),
                                    new XElement("J7"),
                                    new XElement("J4S")
                                ),
                                new XElement("OTH", 
                                    new XElement("MIX"),
                                    new XElement("SLD"),
                                    new XElement("LOS"),
                                    new XElement("AD3"),
                                    new XElement("AD4"),
                                    new XElement("DotSize"),
                                    new XElement("OTH")
                                )
                            );
                config.Save(filepath);
            }


            Suspension = new List<string>();
            Attach = new List<string>();
            SJB = new List<string>();
            OTH = new List<string>();

            foreach (XElement x in config.Elements("Suspension").Descendants())
            {
                Suspension.Add(x.Name.ToString());
            }
            foreach (XElement x in config.Elements("Attach").Descendants())
            {
                Attach.Add(x.Name.ToString());
            }
            foreach (XElement x in config.Elements("SJB").Descendants())
            {
                SJB.Add(x.Name.ToString());
            }
            foreach (XElement x in config.Elements("OTH").Descendants())
            {
                OTH.Add(x.Name.ToString());
            }
        }

        public void AppendDefect(string defect)
        {
            string filepath = Directory.GetCurrentDirectory() + "/" + filename;
            config.Element("OTH").Add(new XElement(defect));
            config.Save(filepath);
        }

        public List<string> GetSuspension()
        {
            return Suspension;
        }

        public List<string> GetAttach()
        {
            return Attach;
        }

        public List<string> GetSJB()
        {
            return SJB;
        }

        public List<string> GetOTH()
        {
            return OTH;
        }
    }
}
