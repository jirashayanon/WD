namespace ARB_Host
{
    partial class FormShowXML
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxXML = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxXML
            // 
            this.textBoxXML.Location = new System.Drawing.Point(13, 26);
            this.textBoxXML.Multiline = true;
            this.textBoxXML.Name = "textBoxXML";
            this.textBoxXML.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxXML.Size = new System.Drawing.Size(696, 526);
            this.textBoxXML.TabIndex = 1;
            // 
            // FormShowXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 579);
            this.Controls.Add(this.textBoxXML);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormShowXML";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShowXML";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxXML;
    }
}