namespace CruncherAgent
{
    partial class CruncherAgentForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CruncherAgentForm));
            this.listView_TrayHistory = new System.Windows.Forms.ListView();
            this.col_TrayList = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_CrunchedTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblTrayHistory = new System.Windows.Forms.Label();
            this.btnClearTrayList = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconCruncherAgent = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_TrayHistory
            // 
            this.listView_TrayHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_TrayList,
            this.col_CrunchedTime});
            this.listView_TrayHistory.Location = new System.Drawing.Point(12, 81);
            this.listView_TrayHistory.Name = "listView_TrayHistory";
            this.listView_TrayHistory.Size = new System.Drawing.Size(592, 155);
            this.listView_TrayHistory.TabIndex = 0;
            this.listView_TrayHistory.UseCompatibleStateImageBehavior = false;
            this.listView_TrayHistory.View = System.Windows.Forms.View.Details;
            // 
            // col_TrayList
            // 
            this.col_TrayList.Text = "TrayID";
            this.col_TrayList.Width = 184;
            // 
            // col_CrunchedTime
            // 
            this.col_CrunchedTime.Text = "Crunched Time";
            this.col_CrunchedTime.Width = 207;
            // 
            // lblTrayHistory
            // 
            this.lblTrayHistory.AutoSize = true;
            this.lblTrayHistory.Location = new System.Drawing.Point(12, 65);
            this.lblTrayHistory.Name = "lblTrayHistory";
            this.lblTrayHistory.Size = new System.Drawing.Size(66, 13);
            this.lblTrayHistory.TabIndex = 1;
            this.lblTrayHistory.Text = "Tray History:";
            // 
            // btnClearTrayList
            // 
            this.btnClearTrayList.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnClearTrayList.Location = new System.Drawing.Point(84, 52);
            this.btnClearTrayList.Name = "btnClearTrayList";
            this.btnClearTrayList.Size = new System.Drawing.Size(75, 26);
            this.btnClearTrayList.TabIndex = 5;
            this.btnClearTrayList.Text = "Clear list";
            this.btnClearTrayList.UseVisualStyleBackColor = true;
            this.btnClearTrayList.Click += new System.EventHandler(this.btnClearTrayList_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(616, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyIconCruncherAgent
            // 
            this.notifyIconCruncherAgent.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconCruncherAgent.Icon")));
            this.notifyIconCruncherAgent.Text = "CruncherAgent";
            this.notifyIconCruncherAgent.Visible = true;
            this.notifyIconCruncherAgent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // CruncherAgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 289);
            this.Controls.Add(this.btnClearTrayList);
            this.Controls.Add(this.lblTrayHistory);
            this.Controls.Add(this.listView_TrayHistory);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "CruncherAgentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CruncherAgent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CruncherAgentForm_FormClosing);
            this.Load += new System.EventHandler(this.CruncherAgentForm_Load);
            this.Resize += new System.EventHandler(this.CruncherAgentForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_TrayHistory;
        private System.Windows.Forms.ColumnHeader col_TrayList;
        private System.Windows.Forms.ColumnHeader col_CrunchedTime;
        private System.Windows.Forms.Label lblTrayHistory;
        private System.Windows.Forms.Button btnClearTrayList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.NotifyIcon notifyIconCruncherAgent;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

