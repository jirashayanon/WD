using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace PhotoLibrary.Model
{
    public class Images
    {
        public Images(string id)
        {
            Path = new List<string>();

            foreach (var key in ConfigurationManager.AppSettings)
            {
                Path.Add(key.ToString());
            }
        }

        public List<string> Path
        {
            get;
            private set;
        }

        public string ReadAppSettings(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];            // Not found returns null
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return null;
        }
    }
}
