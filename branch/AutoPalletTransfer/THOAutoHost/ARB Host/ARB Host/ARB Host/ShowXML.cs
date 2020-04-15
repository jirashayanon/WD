using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARB_Host
{
    public partial class FormShowXML : Form
    {
        public FormShowXML()
        {
            InitializeComponent();
        }

        public FormShowXML(string strXML)
        {
            InitializeComponent();

            strXML = strXML.Replace("<root>", "");
            strXML = strXML.Replace("</root>", "");

            string strNewLine = "\r\n";
            int nNewLineIndex = 0;
            if ((nNewLineIndex = strXML.IndexOf(strNewLine)) >= 0)
            {
                Console.WriteLine(nNewLineIndex.ToString());
                strXML = strXML.Remove(nNewLineIndex, strNewLine.Length);
            }

            textBoxXML.Text = strXML;
        }
    }
}
