using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace AQCruncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(args));
            }
            else
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                MainForm frm = new MainForm(args);
            }

        }
    }
}
