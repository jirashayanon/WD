using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CruncherAgent
{
    public partial class ConfirmExitForm : Form
    {
        public ConfirmExitForm()
        {
            InitializeComponent();
        }


        private bool _bPasswordCorrect = false;
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (txtboxPassword.Text == "1234")
            {
                _bPasswordCorrect = true;
            }

            FormClosingEventArgs eArg = new FormClosingEventArgs(CloseReason.FormOwnerClosing, false);
            ConfirmExitForm_FormClosing(sender, eArg);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            FormClosingEventArgs eArg = new FormClosingEventArgs(CloseReason.FormOwnerClosing, false);
            ConfirmExitForm_FormClosing(sender, eArg);
        }

        new public DialogResult ShowDialog()
        {
            return base.ShowDialog();
        }

        private void ConfirmExitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_bPasswordCorrect)
            {
                this.DialogResult = DialogResult.Yes;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void ConfirmExitForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.buttonOK_Click(sender, e);
            }
        }

    }
}
