namespace ARB_Host
{
    partial class MainWindow
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.MasterDataGridView = new System.Windows.Forms.DataGridView();
            this.ToolIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToolConnectionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionModeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocalIPAddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localPortNumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrimaryMessageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecondaryMessageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorMessageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.T3TimeoutColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.T5TimeoutColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ErrorMessageGroupBox = new System.Windows.Forms.GroupBox();
            this.ErrorMessageTextBox = new System.Windows.Forms.TextBox();
            this.SecondaryGroupBox = new System.Windows.Forms.GroupBox();
            this.SecondaryMessageTextBox = new System.Windows.Forms.TextBox();
            this.PrimaryGroupBox = new System.Windows.Forms.GroupBox();
            this.PrimaryMessageTextBox = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MasterDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.ErrorMessageGroupBox.SuspendLayout();
            this.SecondaryGroupBox.SuspendLayout();
            this.PrimaryGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MasterDataGridView);
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(985, 302);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuration";
            // 
            // MasterDataGridView
            // 
            this.MasterDataGridView.AllowUserToAddRows = false;
            this.MasterDataGridView.AllowUserToDeleteRows = false;
            this.MasterDataGridView.AllowUserToResizeColumns = false;
            this.MasterDataGridView.AllowUserToResizeRows = false;
            this.MasterDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MasterDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ToolIdColumn,
            this.ToolConnectionColumn,
            this.ConnectionModeColumn,
            this.LocalIPAddressColumn,
            this.localPortNumberColumn,
            this.PrimaryMessageColumn,
            this.SecondaryMessageColumn,
            this.ErrorMessageColumn,
            this.T3TimeoutColumn,
            this.T5TimeoutColumn});
            this.MasterDataGridView.Location = new System.Drawing.Point(3, 16);
            this.MasterDataGridView.Name = "MasterDataGridView";
            this.MasterDataGridView.ReadOnly = true;
            this.MasterDataGridView.RowHeadersVisible = false;
            this.MasterDataGridView.Size = new System.Drawing.Size(976, 286);
            this.MasterDataGridView.TabIndex = 0;
            this.MasterDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MasterDataGridView_CellMouseClick);
            // 
            // ToolIdColumn
            // 
            this.ToolIdColumn.HeaderText = "Tool Id";
            this.ToolIdColumn.Name = "ToolIdColumn";
            this.ToolIdColumn.ReadOnly = true;
            // 
            // ToolConnectionColumn
            // 
            this.ToolConnectionColumn.HeaderText = "Connection Status";
            this.ToolConnectionColumn.Name = "ToolConnectionColumn";
            this.ToolConnectionColumn.ReadOnly = true;
            this.ToolConnectionColumn.Width = 120;
            // 
            // ConnectionModeColumn
            // 
            this.ConnectionModeColumn.HeaderText = "Connection Mode";
            this.ConnectionModeColumn.Name = "ConnectionModeColumn";
            this.ConnectionModeColumn.ReadOnly = true;
            this.ConnectionModeColumn.Width = 120;
            // 
            // LocalIPAddressColumn
            // 
            this.LocalIPAddressColumn.HeaderText = "Local IPAddress";
            this.LocalIPAddressColumn.Name = "LocalIPAddressColumn";
            this.LocalIPAddressColumn.ReadOnly = true;
            this.LocalIPAddressColumn.Width = 120;
            // 
            // localPortNumberColumn
            // 
            this.localPortNumberColumn.HeaderText = "Local Port";
            this.localPortNumberColumn.Name = "localPortNumberColumn";
            this.localPortNumberColumn.ReadOnly = true;
            this.localPortNumberColumn.Width = 120;
            // 
            // PrimaryMessageColumn
            // 
            this.PrimaryMessageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PrimaryMessageColumn.HeaderText = "Primary Message";
            this.PrimaryMessageColumn.Name = "PrimaryMessageColumn";
            this.PrimaryMessageColumn.ReadOnly = true;
            this.PrimaryMessageColumn.Width = 240;
            // 
            // SecondaryMessageColumn
            // 
            this.SecondaryMessageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SecondaryMessageColumn.HeaderText = "Secondary Message";
            this.SecondaryMessageColumn.Name = "SecondaryMessageColumn";
            this.SecondaryMessageColumn.ReadOnly = true;
            this.SecondaryMessageColumn.Width = 240;
            // 
            // ErrorMessageColumn
            // 
            this.ErrorMessageColumn.HeaderText = "Error Message";
            this.ErrorMessageColumn.Name = "ErrorMessageColumn";
            this.ErrorMessageColumn.ReadOnly = true;
            this.ErrorMessageColumn.Width = 120;
            // 
            // T3TimeoutColumn
            // 
            this.T3TimeoutColumn.HeaderText = "T3";
            this.T3TimeoutColumn.Name = "T3TimeoutColumn";
            this.T3TimeoutColumn.ReadOnly = true;
            this.T3TimeoutColumn.Width = 40;
            // 
            // T5TimeoutColumn
            // 
            this.T5TimeoutColumn.HeaderText = "T5";
            this.T5TimeoutColumn.Name = "T5TimeoutColumn";
            this.T5TimeoutColumn.ReadOnly = true;
            this.T5TimeoutColumn.Width = 40;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ErrorMessageGroupBox);
            this.groupBox4.Controls.Add(this.SecondaryGroupBox);
            this.groupBox4.Controls.Add(this.PrimaryGroupBox);
            this.groupBox4.Location = new System.Drawing.Point(6, 330);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(985, 279);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Message";
            // 
            // ErrorMessageGroupBox
            // 
            this.ErrorMessageGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorMessageGroupBox.Controls.Add(this.ErrorMessageTextBox);
            this.ErrorMessageGroupBox.Location = new System.Drawing.Point(6, 225);
            this.ErrorMessageGroupBox.Name = "ErrorMessageGroupBox";
            this.ErrorMessageGroupBox.Size = new System.Drawing.Size(973, 48);
            this.ErrorMessageGroupBox.TabIndex = 11;
            this.ErrorMessageGroupBox.TabStop = false;
            this.ErrorMessageGroupBox.Text = "Error Message";
            // 
            // ErrorMessageTextBox
            // 
            this.ErrorMessageTextBox.Location = new System.Drawing.Point(3, 16);
            this.ErrorMessageTextBox.Multiline = true;
            this.ErrorMessageTextBox.Name = "ErrorMessageTextBox";
            this.ErrorMessageTextBox.ReadOnly = true;
            this.ErrorMessageTextBox.Size = new System.Drawing.Size(964, 26);
            this.ErrorMessageTextBox.TabIndex = 7;
            // 
            // SecondaryGroupBox
            // 
            this.SecondaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SecondaryGroupBox.Controls.Add(this.SecondaryMessageTextBox);
            this.SecondaryGroupBox.Location = new System.Drawing.Point(6, 122);
            this.SecondaryGroupBox.Name = "SecondaryGroupBox";
            this.SecondaryGroupBox.Size = new System.Drawing.Size(973, 100);
            this.SecondaryGroupBox.TabIndex = 10;
            this.SecondaryGroupBox.TabStop = false;
            this.SecondaryGroupBox.Text = "Secondary Message";
            // 
            // SecondaryMessageTextBox
            // 
            this.SecondaryMessageTextBox.Location = new System.Drawing.Point(3, 16);
            this.SecondaryMessageTextBox.Multiline = true;
            this.SecondaryMessageTextBox.Name = "SecondaryMessageTextBox";
            this.SecondaryMessageTextBox.ReadOnly = true;
            this.SecondaryMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SecondaryMessageTextBox.Size = new System.Drawing.Size(964, 81);
            this.SecondaryMessageTextBox.TabIndex = 8;
            // 
            // PrimaryGroupBox
            // 
            this.PrimaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PrimaryGroupBox.Controls.Add(this.PrimaryMessageTextBox);
            this.PrimaryGroupBox.Location = new System.Drawing.Point(6, 19);
            this.PrimaryGroupBox.Name = "PrimaryGroupBox";
            this.PrimaryGroupBox.Size = new System.Drawing.Size(973, 100);
            this.PrimaryGroupBox.TabIndex = 9;
            this.PrimaryGroupBox.TabStop = false;
            this.PrimaryGroupBox.Text = "Primary Message";
            // 
            // PrimaryMessageTextBox
            // 
            this.PrimaryMessageTextBox.Location = new System.Drawing.Point(3, 16);
            this.PrimaryMessageTextBox.Multiline = true;
            this.PrimaryMessageTextBox.Name = "PrimaryMessageTextBox";
            this.PrimaryMessageTextBox.ReadOnly = true;
            this.PrimaryMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PrimaryMessageTextBox.Size = new System.Drawing.Size(964, 81);
            this.PrimaryMessageTextBox.TabIndex = 5;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(127, 283);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(133, 32);
            this.button5.TabIndex = 14;
            this.button5.Text = "RequestProcessState";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.RequestProcessState_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(266, 283);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 32);
            this.button1.TabIndex = 9;
            this.button1.Text = "RUDare";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AreYouThere_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1006, 618);
            this.panel1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 283);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 32);
            this.button2.TabIndex = 14;
            this.button2.Text = "RequestControlState";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.RequestControlState_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(841, 283);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(133, 32);
            this.button6.TabIndex = 14;
            this.button6.Text = "OfflineRequest to Tool";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OfflineRequest_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(702, 283);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(133, 32);
            this.button4.TabIndex = 14;
            this.button4.Text = "OnlineRequest to Tool";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnlineRequest_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(985, 274);
            this.panel2.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 621);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 649);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1022, 649);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MasterDataGridView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.ErrorMessageGroupBox.ResumeLayout(false);
            this.ErrorMessageGroupBox.PerformLayout();
            this.SecondaryGroupBox.ResumeLayout(false);
            this.SecondaryGroupBox.PerformLayout();
            this.PrimaryGroupBox.ResumeLayout(false);
            this.PrimaryGroupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView MasterDataGridView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox SecondaryMessageTextBox;
        private System.Windows.Forms.TextBox ErrorMessageTextBox;
        private System.Windows.Forms.TextBox PrimaryMessageTextBox;
        private System.Windows.Forms.GroupBox ErrorMessageGroupBox;
        private System.Windows.Forms.GroupBox SecondaryGroupBox;
        private System.Windows.Forms.GroupBox PrimaryGroupBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToolIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToolConnectionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionModeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocalIPAddressColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localPortNumberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrimaryMessageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecondaryMessageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrorMessageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn T3TimeoutColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn T5TimeoutColumn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
    }
}