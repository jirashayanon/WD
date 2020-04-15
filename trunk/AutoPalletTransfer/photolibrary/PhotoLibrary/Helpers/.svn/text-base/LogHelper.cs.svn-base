using System;
using System.IO;

namespace PhotoLibrary.Helpers
{
    public class LogHelper
    {

        /// <summary>
        /// Write a message to log file.
        /// </summary>
        /// <param name="message"></param>
        public static void AppendLogFile(string message)
        {
            try
            {
                // This text is added only once to the file.

                string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "History_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                if (!File.Exists(destFile))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(destFile))
                    {
                        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + "Scanning Records Start");
                    }
                }

                // This text is always added, making the file longer over time 
                // if it is not deleted. 
                using (StreamWriter sw = File.AppendText(destFile))
                {
                    sw.WriteLine(DateTime.Now + " - " + message);
                }

                /*
                // Open the file to read from. 
                using (StreamReader sr = File.OpenText(destFile))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
                */
            }
            catch (Exception Ex)
            {
                AppendErrorFile(Ex.ToString());
            }
        }

        /// <summary>
        /// Write an error to Error file.
        /// </summary>
        /// <param name="message"></param>
        public static void AppendErrorFile(string message, bool critical = false)
        {
            try
            {
                string errorFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Errors_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                // This text is added only once to the file. 
                if (!File.Exists(errorFile))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(errorFile))
                    {
                        sw.WriteLine("Error Log File:");
                    }
                }

                // This text is always added, making the file longer over time 
                // if it is not deleted. 
                using (StreamWriter sw = File.AppendText(errorFile))
                {
                    if (critical)
                    {
                        sw.WriteLine(DateTime.Now + " - Critical! - " + message);
                    }
                    else
                    {
                        sw.WriteLine(DateTime.Now + " - " + message);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public static void AppendWarningFile(string message)
        {
            try
            {
                string warningFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Warnings_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                // This text is added only once to the file. 
                if (!File.Exists(warningFile))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(warningFile))
                    {
                        sw.WriteLine("Warning Log File.");
                    }
                }

                // This text is always added, making the file longer over time 
                // if it is not deleted. 
                using (StreamWriter sw = File.AppendText(warningFile))
                {
                    sw.WriteLine(DateTime.Now + " - " + message);
                }
            }
            catch (Exception Ex)
            {
                AppendErrorFile(Ex.ToString());
            }
        }

        /// <summary>
        /// Write a message to debug log file.
        /// </summary>
        /// <param name="message"></param>
        public static void AppendDebugFile(string message)
        {
            try
            {
                string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Debug_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                if (!File.Exists(destFile))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(destFile))
                    {
                    }
                }

                // This text is always added, making the file longer over time 
                // if it is not deleted. 
                using (StreamWriter sw = File.AppendText(destFile))
                {
                    sw.WriteLine(DateTime.Now + " - " + message);
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + " - " + message);
                }
            }
            catch (Exception Ex)
            {
                AppendErrorFile(Ex.ToString());
            }
        }

        // use AppendLogFile instead
        //public static void CreateLogFile()
        //{
        //    try
        //    {
        //        // Delete the file if it exists. 
        //        if (File.Exists(destFile))
        //        {
        //            File.Delete(destFile);
        //        }

        //        // Create the file. 
        //        using (FileStream fs = File.Create(destFile))
        //        {
        //            Byte[] info = new UTF8Encoding(true).GetBytes("History File: " + path + "\n");
        //            fs.Write(info, 0, info.Length);
        //        }

        //        // Open the stream and read it back. 
        //        using (StreamReader sr = File.OpenText(destFile))
        //        {
        //            string s = "";
        //            while ((s = sr.ReadLine()) != null)
        //            {
        //                Console.WriteLine(s);
        //            }
        //        }
        //        Console.WriteLine("Finish ahah");
        //    }
        //    catch (Exception Ex)
        //    {
        //        Console.WriteLine(Ex.ToString());
        //    }

        //}
    }
}