using MySql.Data.MySqlClient;
using PhotoLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLibrary.Repository
{
    internal class DBCredential
    {
        internal MySqlConnection Conn { get; private set; }

        public DBCredential()
        {
            ConfigXML xml = new ConfigXML();
            string cn = string.Format("server={0}; user id=abc; password='def'; database=iavi", xml.ServerIP());
            Conn = new MySqlConnection(cn);
            Conn.Open();
        }

        ~DBCredential()
        {
            Conn.Close();
            Debug.WriteLine("DB closed");
        }
    }
}
