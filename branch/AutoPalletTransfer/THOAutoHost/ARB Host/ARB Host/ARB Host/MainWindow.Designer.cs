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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.button2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnReqPalletASLV = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnExportLotInfo = new System.Windows.Forms.Button();
            this.btnGetLotsOffline = new System.Windows.Forms.Button();
            this.lstviewRecipe = new System.Windows.Forms.ListView();
            this.colRecipe_ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_PartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_ProductName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_STR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_Suspension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_SuspPartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_HGAType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_Line = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecipe_PalletID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblWebServiceStatus = new System.Windows.Forms.Label();
            this.txtboxHGAPartNumber = new System.Windows.Forms.TextBox();
            this.lblHGAPartNumber = new System.Windows.Forms.Label();
            this.lstviewLotDetails = new System.Windows.Forms.ListView();
            this.colLotNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHGAPartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProgram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSuspension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSuspPartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSTR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLineNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colQTY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUpdatedTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblAPIKey = new System.Windows.Forms.Label();
            this.txtboxAPIKey = new System.Windows.Forms.TextBox();
            this.lblLine = new System.Windows.Forms.Label();
            this.txtboxLine = new System.Windows.Forms.TextBox();
            this.btnClearRecipe = new System.Windows.Forms.Button();
            this.btnGenRecipe = new System.Windows.Forms.Button();
            this.btnGetLots = new System.Windows.Forms.Button();
            this.btnRegisterLine = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtbox_tab3_transID_Pallet2 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_transID_Pallet1 = new System.Windows.Forms.TextBox();
            this.combo_tab3_ACAMID = new System.Windows.Forms.ComboBox();
            this.btn_tab3_NoHGA10 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA9 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA8 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA7 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA6 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA5 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA4 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA3 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA2 = new System.Windows.Forms.Button();
            this.btn_tab3_NoHGA1 = new System.Windows.Forms.Button();
            this.btn_tab3_ClearData = new System.Windows.Forms.Button();
            this.btn_tab3_NewPallet = new System.Windows.Forms.Button();
            this.lbl_tab3_transID_Pallet2 = new System.Windows.Forms.Label();
            this.lbl_tab3_transID_Pallet1 = new System.Windows.Forms.Label();
            this.lbl_tab3_CureZone = new System.Windows.Forms.Label();
            this.lbl_tab3_CureTime = new System.Windows.Forms.Label();
            this.lbl_tab3_UVPower = new System.Windows.Forms.Label();
            this.lbl_tab3_SJBFixture = new System.Windows.Forms.Label();
            this.lbl_tab3_transID = new System.Windows.Forms.Label();
            this.lstboxNextEquipmentType = new System.Windows.Forms.ListBox();
            this.lstboxEquipmentType = new System.Windows.Forms.ListBox();
            this.lblNextEquipmentType = new System.Windows.Forms.Label();
            this.lblEquipmentType = new System.Windows.Forms.Label();
            this.lbl_tab3_error = new System.Windows.Forms.Label();
            this.btn_tab3_Pallet2 = new System.Windows.Forms.Button();
            this.btn_tab3_SaveLocal = new System.Windows.Forms.Button();
            this.btn_tab3_MovePallet = new System.Windows.Forms.Button();
            this.btn_tab3_Pallet1 = new System.Windows.Forms.Button();
            this.btn_tab3_RandomSN = new System.Windows.Forms.Button();
            this.btn_tab3_LoadLocal = new System.Windows.Forms.Button();
            this.btn_tab3_SendPalletInfo = new System.Windows.Forms.Button();
            this.btn_tab3_ReqPalletInfo = new System.Windows.Forms.Button();
            this.radioButton_DisabledPallet = new System.Windows.Forms.RadioButton();
            this.radioButton_EnabledPallet = new System.Windows.Forms.RadioButton();
            this.lbl_tab3_EnabledPallet = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect10 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect9 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect8 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect7 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect6 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect5 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect4 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect3 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect2 = new System.Windows.Forms.Label();
            this.lbl_tab3_Defect1 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN10 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN9 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN8 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN7 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN6 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN5 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN4 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN3 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN2 = new System.Windows.Forms.Label();
            this.lbl_tab3_SN1 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA10 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA9 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA8 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA7 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA6 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA5 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA4 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA3 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA2 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGA1 = new System.Windows.Forms.Label();
            this.lbl_tab3_HGAType = new System.Windows.Forms.Label();
            this.lbl_tab3_ALMID = new System.Windows.Forms.Label();
            this.lbl_tab3_Suspension = new System.Windows.Forms.Label();
            this.lbl_tab3_COMMACK = new System.Windows.Forms.Label();
            this.lbl_tab3_ACAMID = new System.Windows.Forms.Label();
            this.lbl_tab3_Line = new System.Windows.Forms.Label();
            this.lbl_tab3_LotNumber = new System.Windows.Forms.Label();
            this.lbl_tab3_ProductName = new System.Windows.Forms.Label();
            this.lbl_tab3_PartNumber = new System.Windows.Forms.Label();
            this.lbl_tab3_PalletID = new System.Windows.Forms.Label();
            this.txtbox_tab3_Defect10 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN10 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect9 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN9 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect8 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN8 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect7 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN7 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect6 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN6 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect5 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN5 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect4 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN4 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect3 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN3 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect2 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN2 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Defect1 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SN1 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_HGAType = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_ALMID = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_COMMACK = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Suspension = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_CureZone = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_CureTime = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_UVPower = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_ACAMID = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Line = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_LotNumber = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_ProductName = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_PartNumber = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_SJBFixture = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_transID = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Pallet2 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_Pallet1 = new System.Windows.Forms.TextBox();
            this.txtbox_tab3_PalletID = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lbl_tab4_error = new System.Windows.Forms.Label();
            this.btn_tab4_NewAllPallets = new System.Windows.Forms.Button();
            this.lbl_tab4_PalletList = new System.Windows.Forms.Label();
            this.txtbox_tab4_PalletList = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.txtboxPendingACAMPalletSN = new System.Windows.Forms.TextBox();
            this.txtboxPendingACAMSuspAmt = new System.Windows.Forms.TextBox();
            this.txtboxPendingACAMId = new System.Windows.Forms.TextBox();
            this.txtboxPendingACAMACAMId = new System.Windows.Forms.TextBox();
            this.lbl_tab5_pendingACAM = new System.Windows.Forms.Label();
            this.lbl_tab5_reqsusp = new System.Windows.Forms.Label();
            this.lbl_tab5_error = new System.Windows.Forms.Label();
            this.btnPendingACAMRefresh = new System.Windows.Forms.Button();
            this.btnReqSuspRefresh = new System.Windows.Forms.Button();
            this.lblReqSuspSuspAmt = new System.Windows.Forms.Label();
            this.lblReqSuspACAMID = new System.Windows.Forms.Label();
            this.btnPendingACAMIdSave = new System.Windows.Forms.Button();
            this.btnPendingACAMCancel = new System.Windows.Forms.Button();
            this.btnDeleteReqSusp = new System.Windows.Forms.Button();
            this.btnMultiReqSusp50 = new System.Windows.Forms.Button();
            this.btnMultiReqSusp10 = new System.Windows.Forms.Button();
            this.btnReqSusp = new System.Windows.Forms.Button();
            this.txtboxReqSuspSuspAmt = new System.Windows.Forms.TextBox();
            this.txtboxReqSuspACAMID = new System.Windows.Forms.TextBox();
            this.lstviewPendingACAM = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstviewReqSusp = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTextMenuContext1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showXMLMenuContext1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTextMenuContext2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showXMLMenuContext2 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MasterDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.ErrorMessageGroupBox.SuspendLayout();
            this.SecondaryGroupBox.SuspendLayout();
            this.PrimaryGroupBox.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MasterDataGridView);
            this.groupBox3.Location = new System.Drawing.Point(9, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(756, 194);
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
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MasterDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.MasterDataGridView.DefaultCellStyle = dataGridViewCellStyle5;
            this.MasterDataGridView.Location = new System.Drawing.Point(5, 16);
            this.MasterDataGridView.Name = "MasterDataGridView";
            this.MasterDataGridView.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MasterDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.MasterDataGridView.RowHeadersVisible = false;
            this.MasterDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MasterDataGridView.Size = new System.Drawing.Size(745, 170);
            this.MasterDataGridView.TabIndex = 0;
            this.MasterDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MasterDataGridView_CellMouseClick);
            // 
            // ToolIdColumn
            // 
            this.ToolIdColumn.HeaderText = "Tool Id";
            this.ToolIdColumn.Name = "ToolIdColumn";
            this.ToolIdColumn.ReadOnly = true;
            this.ToolIdColumn.Width = 70;
            // 
            // ToolConnectionColumn
            // 
            this.ToolConnectionColumn.HeaderText = "Connection Status";
            this.ToolConnectionColumn.Name = "ToolConnectionColumn";
            this.ToolConnectionColumn.ReadOnly = true;
            // 
            // ConnectionModeColumn
            // 
            this.ConnectionModeColumn.HeaderText = "Connection Mode";
            this.ConnectionModeColumn.Name = "ConnectionModeColumn";
            this.ConnectionModeColumn.ReadOnly = true;
            // 
            // LocalIPAddressColumn
            // 
            this.LocalIPAddressColumn.HeaderText = "Local IPAddress";
            this.LocalIPAddressColumn.Name = "LocalIPAddressColumn";
            this.LocalIPAddressColumn.ReadOnly = true;
            // 
            // localPortNumberColumn
            // 
            this.localPortNumberColumn.HeaderText = "Local Port";
            this.localPortNumberColumn.Name = "localPortNumberColumn";
            this.localPortNumberColumn.ReadOnly = true;
            this.localPortNumberColumn.Width = 90;
            // 
            // PrimaryMessageColumn
            // 
            this.PrimaryMessageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PrimaryMessageColumn.HeaderText = "Primary Message";
            this.PrimaryMessageColumn.Name = "PrimaryMessageColumn";
            this.PrimaryMessageColumn.ReadOnly = true;
            this.PrimaryMessageColumn.Width = 50;
            // 
            // SecondaryMessageColumn
            // 
            this.SecondaryMessageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SecondaryMessageColumn.HeaderText = "Secondary Message";
            this.SecondaryMessageColumn.Name = "SecondaryMessageColumn";
            this.SecondaryMessageColumn.ReadOnly = true;
            this.SecondaryMessageColumn.Width = 50;
            // 
            // ErrorMessageColumn
            // 
            this.ErrorMessageColumn.HeaderText = "Error Message";
            this.ErrorMessageColumn.Name = "ErrorMessageColumn";
            this.ErrorMessageColumn.ReadOnly = true;
            this.ErrorMessageColumn.Width = 50;
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
            this.groupBox4.Location = new System.Drawing.Point(8, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(757, 279);
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
            this.ErrorMessageGroupBox.Size = new System.Drawing.Size(743, 48);
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
            this.ErrorMessageTextBox.Size = new System.Drawing.Size(740, 26);
            this.ErrorMessageTextBox.TabIndex = 7;
            // 
            // SecondaryGroupBox
            // 
            this.SecondaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SecondaryGroupBox.Controls.Add(this.SecondaryMessageTextBox);
            this.SecondaryGroupBox.Location = new System.Drawing.Point(6, 122);
            this.SecondaryGroupBox.Name = "SecondaryGroupBox";
            this.SecondaryGroupBox.Size = new System.Drawing.Size(743, 100);
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
            this.SecondaryMessageTextBox.Size = new System.Drawing.Size(736, 81);
            this.SecondaryMessageTextBox.TabIndex = 8;
            this.SecondaryMessageTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SecondaryMessageTextBox_MouseUp);
            // 
            // PrimaryGroupBox
            // 
            this.PrimaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PrimaryGroupBox.Controls.Add(this.PrimaryMessageTextBox);
            this.PrimaryGroupBox.Location = new System.Drawing.Point(6, 19);
            this.PrimaryGroupBox.Name = "PrimaryGroupBox";
            this.PrimaryGroupBox.Size = new System.Drawing.Size(745, 100);
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
            this.PrimaryMessageTextBox.Size = new System.Drawing.Size(736, 81);
            this.PrimaryMessageTextBox.TabIndex = 5;
            this.PrimaryMessageTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PrimaryMessageTextBox_MouseUp);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(134, 207);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(120, 32);
            this.button5.TabIndex = 14;
            this.button5.Text = "RequestProcessState";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.RequestProcessState_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(260, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 32);
            this.button1.TabIndex = 9;
            this.button1.Text = "AreYouThere";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AreYouThere_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(8, 207);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 32);
            this.button2.TabIndex = 14;
            this.button2.Text = "RequestControlState";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.RequestControlState_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(630, 207);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(127, 32);
            this.button6.TabIndex = 14;
            this.button6.Text = "OfflineRequest to Tool";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OfflineRequest_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(497, 207);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(127, 32);
            this.button4.TabIndex = 14;
            this.button4.Text = "OnlineRequest to Tool";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnlineRequest_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(4, 9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(782, 561);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.btnReqPalletASLV);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(774, 535);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnReqPalletASLV
            // 
            this.btnReqPalletASLV.Location = new System.Drawing.Point(360, 207);
            this.btnReqPalletASLV.Name = "btnReqPalletASLV";
            this.btnReqPalletASLV.Size = new System.Drawing.Size(94, 32);
            this.btnReqPalletASLV.TabIndex = 9;
            this.btnReqPalletASLV.Text = "ReqPallet ASLV";
            this.btnReqPalletASLV.UseVisualStyleBackColor = true;
            this.btnReqPalletASLV.Click += new System.EventHandler(this.btnReqPalletASLV_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnExportLotInfo);
            this.tabPage2.Controls.Add(this.btnGetLotsOffline);
            this.tabPage2.Controls.Add(this.lstviewRecipe);
            this.tabPage2.Controls.Add(this.lblWebServiceStatus);
            this.tabPage2.Controls.Add(this.txtboxHGAPartNumber);
            this.tabPage2.Controls.Add(this.lblHGAPartNumber);
            this.tabPage2.Controls.Add(this.lstviewLotDetails);
            this.tabPage2.Controls.Add(this.lblAPIKey);
            this.tabPage2.Controls.Add(this.txtboxAPIKey);
            this.tabPage2.Controls.Add(this.lblLine);
            this.tabPage2.Controls.Add(this.txtboxLine);
            this.tabPage2.Controls.Add(this.btnClearRecipe);
            this.tabPage2.Controls.Add(this.btnGenRecipe);
            this.tabPage2.Controls.Add(this.btnGetLots);
            this.tabPage2.Controls.Add(this.btnRegisterLine);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(774, 535);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "MITECS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnExportLotInfo
            // 
            this.btnExportLotInfo.Location = new System.Drawing.Point(413, 127);
            this.btnExportLotInfo.Name = "btnExportLotInfo";
            this.btnExportLotInfo.Size = new System.Drawing.Size(105, 34);
            this.btnExportLotInfo.TabIndex = 20;
            this.btnExportLotInfo.Text = "Export";
            this.btnExportLotInfo.UseVisualStyleBackColor = true;
            this.btnExportLotInfo.Click += new System.EventHandler(this.btnExportLotInfo_Click);
            // 
            // btnGetLotsOffline
            // 
            this.btnGetLotsOffline.Location = new System.Drawing.Point(524, 127);
            this.btnGetLotsOffline.Name = "btnGetLotsOffline";
            this.btnGetLotsOffline.Size = new System.Drawing.Size(105, 34);
            this.btnGetLotsOffline.TabIndex = 19;
            this.btnGetLotsOffline.Text = "Get Lots Offline";
            this.btnGetLotsOffline.UseVisualStyleBackColor = true;
            this.btnGetLotsOffline.Click += new System.EventHandler(this.btnGetLotsOffline_Click);
            // 
            // lstviewRecipe
            // 
            this.lstviewRecipe.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRecipe_ID,
            this.colRecipe_PartNumber,
            this.colRecipe_ProductName,
            this.colRecipe_STR,
            this.colRecipe_Suspension,
            this.colRecipe_SuspPartNumber,
            this.colRecipe_HGAType,
            this.colRecipe_Line,
            this.colRecipe_PalletID});
            this.lstviewRecipe.FullRowSelect = true;
            this.lstviewRecipe.HideSelection = false;
            this.lstviewRecipe.Location = new System.Drawing.Point(9, 374);
            this.lstviewRecipe.MultiSelect = false;
            this.lstviewRecipe.Name = "lstviewRecipe";
            this.lstviewRecipe.Size = new System.Drawing.Size(731, 118);
            this.lstviewRecipe.TabIndex = 18;
            this.lstviewRecipe.UseCompatibleStateImageBehavior = false;
            this.lstviewRecipe.View = System.Windows.Forms.View.Details;
            // 
            // colRecipe_ID
            // 
            this.colRecipe_ID.Text = "RecipeID";
            this.colRecipe_ID.Width = 72;
            // 
            // colRecipe_PartNumber
            // 
            this.colRecipe_PartNumber.Text = "PartNumber";
            this.colRecipe_PartNumber.Width = 116;
            // 
            // colRecipe_ProductName
            // 
            this.colRecipe_ProductName.Text = "ProductName";
            this.colRecipe_ProductName.Width = 90;
            // 
            // colRecipe_STR
            // 
            this.colRecipe_STR.Text = "STR";
            // 
            // colRecipe_Suspension
            // 
            this.colRecipe_Suspension.Text = "Suspension";
            this.colRecipe_Suspension.Width = 77;
            // 
            // colRecipe_SuspPartNumber
            // 
            this.colRecipe_SuspPartNumber.Text = "SuspPartNumber";
            // 
            // colRecipe_HGAType
            // 
            this.colRecipe_HGAType.Text = "HGAType";
            this.colRecipe_HGAType.Width = 84;
            // 
            // colRecipe_Line
            // 
            this.colRecipe_Line.Text = "Line";
            // 
            // colRecipe_PalletID
            // 
            this.colRecipe_PalletID.Text = "PalletID";
            // 
            // lblWebServiceStatus
            // 
            this.lblWebServiceStatus.AutoSize = true;
            this.lblWebServiceStatus.Location = new System.Drawing.Point(6, 17);
            this.lblWebServiceStatus.Name = "lblWebServiceStatus";
            this.lblWebServiceStatus.Size = new System.Drawing.Size(110, 13);
            this.lblWebServiceStatus.TabIndex = 17;
            this.lblWebServiceStatus.Text = "MITECS Webservice:";
            // 
            // txtboxHGAPartNumber
            // 
            this.txtboxHGAPartNumber.Location = new System.Drawing.Point(9, 135);
            this.txtboxHGAPartNumber.Name = "txtboxHGAPartNumber";
            this.txtboxHGAPartNumber.Size = new System.Drawing.Size(180, 20);
            this.txtboxHGAPartNumber.TabIndex = 16;
            // 
            // lblHGAPartNumber
            // 
            this.lblHGAPartNumber.AutoSize = true;
            this.lblHGAPartNumber.Location = new System.Drawing.Point(6, 119);
            this.lblHGAPartNumber.Name = "lblHGAPartNumber";
            this.lblHGAPartNumber.Size = new System.Drawing.Size(95, 13);
            this.lblHGAPartNumber.TabIndex = 15;
            this.lblHGAPartNumber.Text = "HGA Part Number:";
            // 
            // lstviewLotDetails
            // 
            this.lstviewLotDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLotNumber,
            this.colHGAPartNumber,
            this.colProgram,
            this.colSuspension,
            this.colSuspPartNumber,
            this.colType,
            this.colSTR,
            this.colLineNo,
            this.colQTY,
            this.colUpdatedTimestamp});
            this.lstviewLotDetails.FullRowSelect = true;
            this.lstviewLotDetails.GridLines = true;
            this.lstviewLotDetails.HideSelection = false;
            this.lstviewLotDetails.Location = new System.Drawing.Point(9, 173);
            this.lstviewLotDetails.MultiSelect = false;
            this.lstviewLotDetails.Name = "lstviewLotDetails";
            this.lstviewLotDetails.Size = new System.Drawing.Size(731, 137);
            this.lstviewLotDetails.TabIndex = 14;
            this.lstviewLotDetails.UseCompatibleStateImageBehavior = false;
            this.lstviewLotDetails.View = System.Windows.Forms.View.Details;
            this.lstviewLotDetails.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstviewLotDetails_ColumnClick);
            this.lstviewLotDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstviewLotDetails_KeyDown);
            // 
            // colLotNumber
            // 
            this.colLotNumber.Text = "LOT_NUMBER";
            this.colLotNumber.Width = 131;
            // 
            // colHGAPartNumber
            // 
            this.colHGAPartNumber.Text = "HGA_PART_NUMBER";
            this.colHGAPartNumber.Width = 131;
            // 
            // colProgram
            // 
            this.colProgram.Text = "Program/Product";
            // 
            // colSuspension
            // 
            this.colSuspension.Text = "SUSPENSION";
            // 
            // colSuspPartNumber
            // 
            this.colSuspPartNumber.Text = "SuspPartNumber";
            // 
            // colType
            // 
            this.colType.Text = "TYPE";
            // 
            // colSTR
            // 
            this.colSTR.Text = "STR_NO";
            // 
            // colLineNo
            // 
            this.colLineNo.Text = "LINE_NO";
            // 
            // colQTY
            // 
            this.colQTY.Text = "QTY";
            // 
            // colUpdatedTimestamp
            // 
            this.colUpdatedTimestamp.Text = "UPDATED_TIMESTAMP";
            // 
            // lblAPIKey
            // 
            this.lblAPIKey.AutoSize = true;
            this.lblAPIKey.Location = new System.Drawing.Point(6, 73);
            this.lblAPIKey.Name = "lblAPIKey";
            this.lblAPIKey.Size = new System.Drawing.Size(45, 13);
            this.lblAPIKey.TabIndex = 13;
            this.lblAPIKey.Text = "API Key";
            // 
            // txtboxAPIKey
            // 
            this.txtboxAPIKey.Location = new System.Drawing.Point(6, 89);
            this.txtboxAPIKey.Name = "txtboxAPIKey";
            this.txtboxAPIKey.ReadOnly = true;
            this.txtboxAPIKey.Size = new System.Drawing.Size(734, 20);
            this.txtboxAPIKey.TabIndex = 12;
            // 
            // lblLine
            // 
            this.lblLine.AutoSize = true;
            this.lblLine.Location = new System.Drawing.Point(6, 35);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(30, 13);
            this.lblLine.TabIndex = 11;
            this.lblLine.Text = "Line:";
            // 
            // txtboxLine
            // 
            this.txtboxLine.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtboxLine.Location = new System.Drawing.Point(6, 51);
            this.txtboxLine.Name = "txtboxLine";
            this.txtboxLine.Size = new System.Drawing.Size(183, 20);
            this.txtboxLine.TabIndex = 10;
            this.txtboxLine.Text = "B4A2";
            // 
            // btnClearRecipe
            // 
            this.btnClearRecipe.Location = new System.Drawing.Point(635, 316);
            this.btnClearRecipe.Name = "btnClearRecipe";
            this.btnClearRecipe.Size = new System.Drawing.Size(105, 34);
            this.btnClearRecipe.TabIndex = 8;
            this.btnClearRecipe.Text = "Clear Recipe";
            this.btnClearRecipe.UseVisualStyleBackColor = true;
            this.btnClearRecipe.Click += new System.EventHandler(this.btnClearRecipe_Click);
            // 
            // btnGenRecipe
            // 
            this.btnGenRecipe.Location = new System.Drawing.Point(524, 316);
            this.btnGenRecipe.Name = "btnGenRecipe";
            this.btnGenRecipe.Size = new System.Drawing.Size(105, 34);
            this.btnGenRecipe.TabIndex = 8;
            this.btnGenRecipe.Text = "Gen Recipe";
            this.btnGenRecipe.UseVisualStyleBackColor = true;
            this.btnGenRecipe.Click += new System.EventHandler(this.btnGenRecipe_Click);
            // 
            // btnGetLots
            // 
            this.btnGetLots.Location = new System.Drawing.Point(635, 127);
            this.btnGetLots.Name = "btnGetLots";
            this.btnGetLots.Size = new System.Drawing.Size(105, 34);
            this.btnGetLots.TabIndex = 8;
            this.btnGetLots.Text = "Get Lots";
            this.btnGetLots.UseVisualStyleBackColor = true;
            this.btnGetLots.Click += new System.EventHandler(this.btnGetLots_Click);
            // 
            // btnRegisterLine
            // 
            this.btnRegisterLine.Location = new System.Drawing.Point(635, 43);
            this.btnRegisterLine.Name = "btnRegisterLine";
            this.btnRegisterLine.Size = new System.Drawing.Size(105, 34);
            this.btnRegisterLine.TabIndex = 9;
            this.btnRegisterLine.Text = "Register Line";
            this.btnRegisterLine.UseVisualStyleBackColor = true;
            this.btnRegisterLine.Click += new System.EventHandler(this.btnRegisterLine_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtbox_tab3_transID_Pallet2);
            this.tabPage3.Controls.Add(this.txtbox_tab3_transID_Pallet1);
            this.tabPage3.Controls.Add(this.combo_tab3_ACAMID);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA10);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA9);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA8);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA7);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA6);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA5);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA4);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA3);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA2);
            this.tabPage3.Controls.Add(this.btn_tab3_NoHGA1);
            this.tabPage3.Controls.Add(this.btn_tab3_ClearData);
            this.tabPage3.Controls.Add(this.btn_tab3_NewPallet);
            this.tabPage3.Controls.Add(this.lbl_tab3_transID_Pallet2);
            this.tabPage3.Controls.Add(this.lbl_tab3_transID_Pallet1);
            this.tabPage3.Controls.Add(this.lbl_tab3_CureZone);
            this.tabPage3.Controls.Add(this.lbl_tab3_CureTime);
            this.tabPage3.Controls.Add(this.lbl_tab3_UVPower);
            this.tabPage3.Controls.Add(this.lbl_tab3_SJBFixture);
            this.tabPage3.Controls.Add(this.lbl_tab3_transID);
            this.tabPage3.Controls.Add(this.lstboxNextEquipmentType);
            this.tabPage3.Controls.Add(this.lstboxEquipmentType);
            this.tabPage3.Controls.Add(this.lblNextEquipmentType);
            this.tabPage3.Controls.Add(this.lblEquipmentType);
            this.tabPage3.Controls.Add(this.lbl_tab3_error);
            this.tabPage3.Controls.Add(this.btn_tab3_Pallet2);
            this.tabPage3.Controls.Add(this.btn_tab3_SaveLocal);
            this.tabPage3.Controls.Add(this.btn_tab3_MovePallet);
            this.tabPage3.Controls.Add(this.btn_tab3_Pallet1);
            this.tabPage3.Controls.Add(this.btn_tab3_RandomSN);
            this.tabPage3.Controls.Add(this.btn_tab3_LoadLocal);
            this.tabPage3.Controls.Add(this.btn_tab3_SendPalletInfo);
            this.tabPage3.Controls.Add(this.btn_tab3_ReqPalletInfo);
            this.tabPage3.Controls.Add(this.radioButton_DisabledPallet);
            this.tabPage3.Controls.Add(this.radioButton_EnabledPallet);
            this.tabPage3.Controls.Add(this.lbl_tab3_EnabledPallet);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect10);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect9);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect8);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect7);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect6);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect5);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect4);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect3);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect2);
            this.tabPage3.Controls.Add(this.lbl_tab3_Defect1);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN10);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN9);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN8);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN7);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN6);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN5);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN4);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN3);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN2);
            this.tabPage3.Controls.Add(this.lbl_tab3_SN1);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA10);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA9);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA8);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA7);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA6);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA5);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA4);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA3);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA2);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGA1);
            this.tabPage3.Controls.Add(this.lbl_tab3_HGAType);
            this.tabPage3.Controls.Add(this.lbl_tab3_ALMID);
            this.tabPage3.Controls.Add(this.lbl_tab3_Suspension);
            this.tabPage3.Controls.Add(this.lbl_tab3_COMMACK);
            this.tabPage3.Controls.Add(this.lbl_tab3_ACAMID);
            this.tabPage3.Controls.Add(this.lbl_tab3_Line);
            this.tabPage3.Controls.Add(this.lbl_tab3_LotNumber);
            this.tabPage3.Controls.Add(this.lbl_tab3_ProductName);
            this.tabPage3.Controls.Add(this.lbl_tab3_PartNumber);
            this.tabPage3.Controls.Add(this.lbl_tab3_PalletID);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect10);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN10);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect9);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN9);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect8);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN8);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect7);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN7);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect6);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN6);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect5);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN5);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect4);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN4);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect3);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN3);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect2);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN2);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Defect1);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SN1);
            this.tabPage3.Controls.Add(this.txtbox_tab3_HGAType);
            this.tabPage3.Controls.Add(this.txtbox_tab3_ALMID);
            this.tabPage3.Controls.Add(this.txtbox_tab3_COMMACK);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Suspension);
            this.tabPage3.Controls.Add(this.txtbox_tab3_CureZone);
            this.tabPage3.Controls.Add(this.txtbox_tab3_CureTime);
            this.tabPage3.Controls.Add(this.txtbox_tab3_UVPower);
            this.tabPage3.Controls.Add(this.txtbox_tab3_ACAMID);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Line);
            this.tabPage3.Controls.Add(this.txtbox_tab3_LotNumber);
            this.tabPage3.Controls.Add(this.txtbox_tab3_ProductName);
            this.tabPage3.Controls.Add(this.txtbox_tab3_PartNumber);
            this.tabPage3.Controls.Add(this.txtbox_tab3_SJBFixture);
            this.tabPage3.Controls.Add(this.txtbox_tab3_transID);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Pallet2);
            this.tabPage3.Controls.Add(this.txtbox_tab3_Pallet1);
            this.tabPage3.Controls.Add(this.txtbox_tab3_PalletID);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(774, 535);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Pallet Info";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtbox_tab3_transID_Pallet2
            // 
            this.txtbox_tab3_transID_Pallet2.Location = new System.Drawing.Point(656, 369);
            this.txtbox_tab3_transID_Pallet2.Name = "txtbox_tab3_transID_Pallet2";
            this.txtbox_tab3_transID_Pallet2.ReadOnly = true;
            this.txtbox_tab3_transID_Pallet2.Size = new System.Drawing.Size(88, 20);
            this.txtbox_tab3_transID_Pallet2.TabIndex = 29;
            // 
            // txtbox_tab3_transID_Pallet1
            // 
            this.txtbox_tab3_transID_Pallet1.Location = new System.Drawing.Point(545, 369);
            this.txtbox_tab3_transID_Pallet1.Name = "txtbox_tab3_transID_Pallet1";
            this.txtbox_tab3_transID_Pallet1.ReadOnly = true;
            this.txtbox_tab3_transID_Pallet1.Size = new System.Drawing.Size(88, 20);
            this.txtbox_tab3_transID_Pallet1.TabIndex = 29;
            // 
            // combo_tab3_ACAMID
            // 
            this.combo_tab3_ACAMID.FormattingEnabled = true;
            this.combo_tab3_ACAMID.Items.AddRange(new object[] {
            "",
            "APT001",
            "APT002",
            "ACAM22",
            "ACAM34"});
            this.combo_tab3_ACAMID.Location = new System.Drawing.Point(85, 137);
            this.combo_tab3_ACAMID.Name = "combo_tab3_ACAMID";
            this.combo_tab3_ACAMID.Size = new System.Drawing.Size(112, 21);
            this.combo_tab3_ACAMID.TabIndex = 28;
            // 
            // btn_tab3_NoHGA10
            // 
            this.btn_tab3_NoHGA10.Location = new System.Drawing.Point(460, 497);
            this.btn_tab3_NoHGA10.Name = "btn_tab3_NoHGA10";
            this.btn_tab3_NoHGA10.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA10.TabIndex = 26;
            this.btn_tab3_NoHGA10.Text = "NO HGA";
            this.btn_tab3_NoHGA10.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA10.Click += new System.EventHandler(this.btn_tab3_NoHGA10_Click);
            // 
            // btn_tab3_NoHGA9
            // 
            this.btn_tab3_NoHGA9.Location = new System.Drawing.Point(460, 471);
            this.btn_tab3_NoHGA9.Name = "btn_tab3_NoHGA9";
            this.btn_tab3_NoHGA9.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA9.TabIndex = 26;
            this.btn_tab3_NoHGA9.Text = "NO HGA";
            this.btn_tab3_NoHGA9.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA9.Click += new System.EventHandler(this.btn_tab3_NoHGA9_Click);
            // 
            // btn_tab3_NoHGA8
            // 
            this.btn_tab3_NoHGA8.Location = new System.Drawing.Point(460, 445);
            this.btn_tab3_NoHGA8.Name = "btn_tab3_NoHGA8";
            this.btn_tab3_NoHGA8.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA8.TabIndex = 26;
            this.btn_tab3_NoHGA8.Text = "NO HGA";
            this.btn_tab3_NoHGA8.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA8.Click += new System.EventHandler(this.btn_tab3_NoHGA8_Click);
            // 
            // btn_tab3_NoHGA7
            // 
            this.btn_tab3_NoHGA7.Location = new System.Drawing.Point(460, 419);
            this.btn_tab3_NoHGA7.Name = "btn_tab3_NoHGA7";
            this.btn_tab3_NoHGA7.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA7.TabIndex = 26;
            this.btn_tab3_NoHGA7.Text = "NO HGA";
            this.btn_tab3_NoHGA7.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA7.Click += new System.EventHandler(this.btn_tab3_NoHGA7_Click);
            // 
            // btn_tab3_NoHGA6
            // 
            this.btn_tab3_NoHGA6.Location = new System.Drawing.Point(460, 393);
            this.btn_tab3_NoHGA6.Name = "btn_tab3_NoHGA6";
            this.btn_tab3_NoHGA6.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA6.TabIndex = 26;
            this.btn_tab3_NoHGA6.Text = "NO HGA";
            this.btn_tab3_NoHGA6.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA6.Click += new System.EventHandler(this.btn_tab3_NoHGA6_Click);
            // 
            // btn_tab3_NoHGA5
            // 
            this.btn_tab3_NoHGA5.Location = new System.Drawing.Point(460, 354);
            this.btn_tab3_NoHGA5.Name = "btn_tab3_NoHGA5";
            this.btn_tab3_NoHGA5.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA5.TabIndex = 26;
            this.btn_tab3_NoHGA5.Text = "NO HGA";
            this.btn_tab3_NoHGA5.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA5.Click += new System.EventHandler(this.btn_tab3_NoHGA5_Click);
            // 
            // btn_tab3_NoHGA4
            // 
            this.btn_tab3_NoHGA4.Location = new System.Drawing.Point(460, 328);
            this.btn_tab3_NoHGA4.Name = "btn_tab3_NoHGA4";
            this.btn_tab3_NoHGA4.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA4.TabIndex = 26;
            this.btn_tab3_NoHGA4.Text = "NO HGA";
            this.btn_tab3_NoHGA4.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA4.Click += new System.EventHandler(this.btn_tab3_NoHGA4_Click);
            // 
            // btn_tab3_NoHGA3
            // 
            this.btn_tab3_NoHGA3.Location = new System.Drawing.Point(460, 302);
            this.btn_tab3_NoHGA3.Name = "btn_tab3_NoHGA3";
            this.btn_tab3_NoHGA3.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA3.TabIndex = 26;
            this.btn_tab3_NoHGA3.Text = "NO HGA";
            this.btn_tab3_NoHGA3.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA3.Click += new System.EventHandler(this.btn_tab3_NoHGA3_Click);
            // 
            // btn_tab3_NoHGA2
            // 
            this.btn_tab3_NoHGA2.Location = new System.Drawing.Point(460, 276);
            this.btn_tab3_NoHGA2.Name = "btn_tab3_NoHGA2";
            this.btn_tab3_NoHGA2.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA2.TabIndex = 26;
            this.btn_tab3_NoHGA2.Text = "NO HGA";
            this.btn_tab3_NoHGA2.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA2.Click += new System.EventHandler(this.btn_tab3_NoHGA2_Click);
            // 
            // btn_tab3_NoHGA1
            // 
            this.btn_tab3_NoHGA1.Location = new System.Drawing.Point(460, 250);
            this.btn_tab3_NoHGA1.Name = "btn_tab3_NoHGA1";
            this.btn_tab3_NoHGA1.Size = new System.Drawing.Size(66, 23);
            this.btn_tab3_NoHGA1.TabIndex = 26;
            this.btn_tab3_NoHGA1.Text = "NO HGA";
            this.btn_tab3_NoHGA1.UseVisualStyleBackColor = true;
            this.btn_tab3_NoHGA1.Click += new System.EventHandler(this.btn_tab3_NoHGA1_Click);
            // 
            // btn_tab3_ClearData
            // 
            this.btn_tab3_ClearData.Location = new System.Drawing.Point(656, 166);
            this.btn_tab3_ClearData.Name = "btn_tab3_ClearData";
            this.btn_tab3_ClearData.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_ClearData.TabIndex = 25;
            this.btn_tab3_ClearData.Text = "ClearData";
            this.btn_tab3_ClearData.UseVisualStyleBackColor = true;
            this.btn_tab3_ClearData.Click += new System.EventHandler(this.btn_tab3_ClearData_Click);
            // 
            // btn_tab3_NewPallet
            // 
            this.btn_tab3_NewPallet.Location = new System.Drawing.Point(545, 166);
            this.btn_tab3_NewPallet.Name = "btn_tab3_NewPallet";
            this.btn_tab3_NewPallet.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_NewPallet.TabIndex = 24;
            this.btn_tab3_NewPallet.Text = "New Pallet";
            this.btn_tab3_NewPallet.UseVisualStyleBackColor = true;
            this.btn_tab3_NewPallet.Click += new System.EventHandler(this.btn_tab3_NewPallet_Click);
            // 
            // lbl_tab3_transID_Pallet2
            // 
            this.lbl_tab3_transID_Pallet2.AutoSize = true;
            this.lbl_tab3_transID_Pallet2.Location = new System.Drawing.Point(653, 353);
            this.lbl_tab3_transID_Pallet2.Name = "lbl_tab3_transID_Pallet2";
            this.lbl_tab3_transID_Pallet2.Size = new System.Drawing.Size(83, 13);
            this.lbl_tab3_transID_Pallet2.TabIndex = 23;
            this.lbl_tab3_transID_Pallet2.Text = "Pallet2 TransID:";
            // 
            // lbl_tab3_transID_Pallet1
            // 
            this.lbl_tab3_transID_Pallet1.AutoSize = true;
            this.lbl_tab3_transID_Pallet1.Location = new System.Drawing.Point(542, 353);
            this.lbl_tab3_transID_Pallet1.Name = "lbl_tab3_transID_Pallet1";
            this.lbl_tab3_transID_Pallet1.Size = new System.Drawing.Size(83, 13);
            this.lbl_tab3_transID_Pallet1.TabIndex = 23;
            this.lbl_tab3_transID_Pallet1.Text = "Pallet1 TransID:";
            // 
            // lbl_tab3_CureZone
            // 
            this.lbl_tab3_CureZone.AutoSize = true;
            this.lbl_tab3_CureZone.Location = new System.Drawing.Point(575, 62);
            this.lbl_tab3_CureZone.Name = "lbl_tab3_CureZone";
            this.lbl_tab3_CureZone.Size = new System.Drawing.Size(76, 13);
            this.lbl_tab3_CureZone.TabIndex = 23;
            this.lbl_tab3_CureZone.Text = "ILC CureZone:";
            // 
            // lbl_tab3_CureTime
            // 
            this.lbl_tab3_CureTime.AutoSize = true;
            this.lbl_tab3_CureTime.Location = new System.Drawing.Point(575, 36);
            this.lbl_tab3_CureTime.Name = "lbl_tab3_CureTime";
            this.lbl_tab3_CureTime.Size = new System.Drawing.Size(74, 13);
            this.lbl_tab3_CureTime.TabIndex = 23;
            this.lbl_tab3_CureTime.Text = "ILC CureTime:";
            // 
            // lbl_tab3_UVPower
            // 
            this.lbl_tab3_UVPower.AutoSize = true;
            this.lbl_tab3_UVPower.Location = new System.Drawing.Point(575, 10);
            this.lbl_tab3_UVPower.Name = "lbl_tab3_UVPower";
            this.lbl_tab3_UVPower.Size = new System.Drawing.Size(74, 13);
            this.lbl_tab3_UVPower.TabIndex = 23;
            this.lbl_tab3_UVPower.Text = "ILC UVPower:";
            // 
            // lbl_tab3_SJBFixture
            // 
            this.lbl_tab3_SJBFixture.AutoSize = true;
            this.lbl_tab3_SJBFixture.Location = new System.Drawing.Point(383, 36);
            this.lbl_tab3_SJBFixture.Name = "lbl_tab3_SJBFixture";
            this.lbl_tab3_SJBFixture.Size = new System.Drawing.Size(60, 13);
            this.lbl_tab3_SJBFixture.TabIndex = 23;
            this.lbl_tab3_SJBFixture.Text = "SJBFixture:";
            // 
            // lbl_tab3_transID
            // 
            this.lbl_tab3_transID.AutoSize = true;
            this.lbl_tab3_transID.Location = new System.Drawing.Point(218, 10);
            this.lbl_tab3_transID.Name = "lbl_tab3_transID";
            this.lbl_tab3_transID.Size = new System.Drawing.Size(48, 13);
            this.lbl_tab3_transID.TabIndex = 23;
            this.lbl_tab3_transID.Text = "TransID:";
            // 
            // lstboxNextEquipmentType
            // 
            this.lstboxNextEquipmentType.FormattingEnabled = true;
            this.lstboxNextEquipmentType.Items.AddRange(new object[] {
            "ASLV",
            "ACAM",
            "ILC",
            "SJB",
            "AVI",
            "UNOCR",
            "FVMI",
            "APT"});
            this.lstboxNextEquipmentType.Location = new System.Drawing.Point(369, 124);
            this.lstboxNextEquipmentType.Name = "lstboxNextEquipmentType";
            this.lstboxNextEquipmentType.Size = new System.Drawing.Size(85, 108);
            this.lstboxNextEquipmentType.TabIndex = 22;
            // 
            // lstboxEquipmentType
            // 
            this.lstboxEquipmentType.FormattingEnabled = true;
            this.lstboxEquipmentType.Items.AddRange(new object[] {
            "ASLV",
            "ACAM",
            "ILC",
            "SJB",
            "AVI",
            "UNOCR",
            "FVMI",
            "APT"});
            this.lstboxEquipmentType.Location = new System.Drawing.Point(266, 124);
            this.lstboxEquipmentType.Name = "lstboxEquipmentType";
            this.lstboxEquipmentType.Size = new System.Drawing.Size(89, 108);
            this.lstboxEquipmentType.TabIndex = 22;
            // 
            // lblNextEquipmentType
            // 
            this.lblNextEquipmentType.AutoSize = true;
            this.lblNextEquipmentType.Location = new System.Drawing.Point(369, 108);
            this.lblNextEquipmentType.Name = "lblNextEquipmentType";
            this.lblNextEquipmentType.Size = new System.Drawing.Size(103, 13);
            this.lblNextEquipmentType.TabIndex = 21;
            this.lblNextEquipmentType.Text = "NextEquipmentType";
            // 
            // lblEquipmentType
            // 
            this.lblEquipmentType.AutoSize = true;
            this.lblEquipmentType.Location = new System.Drawing.Point(263, 108);
            this.lblEquipmentType.Name = "lblEquipmentType";
            this.lblEquipmentType.Size = new System.Drawing.Size(81, 13);
            this.lblEquipmentType.TabIndex = 21;
            this.lblEquipmentType.Text = "EquipmentType";
            // 
            // lbl_tab3_error
            // 
            this.lbl_tab3_error.AutoSize = true;
            this.lbl_tab3_error.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_tab3_error.ForeColor = System.Drawing.Color.Red;
            this.lbl_tab3_error.Location = new System.Drawing.Point(506, 92);
            this.lbl_tab3_error.Name = "lbl_tab3_error";
            this.lbl_tab3_error.Size = new System.Drawing.Size(85, 13);
            this.lbl_tab3_error.TabIndex = 20;
            this.lbl_tab3_error.Text = "lbl_tab3_error";
            // 
            // btn_tab3_Pallet2
            // 
            this.btn_tab3_Pallet2.Location = new System.Drawing.Point(656, 424);
            this.btn_tab3_Pallet2.Name = "btn_tab3_Pallet2";
            this.btn_tab3_Pallet2.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_Pallet2.TabIndex = 19;
            this.btn_tab3_Pallet2.Text = "Pallet2";
            this.btn_tab3_Pallet2.UseVisualStyleBackColor = true;
            this.btn_tab3_Pallet2.Click += new System.EventHandler(this.btn_tab3_Pallet2_Click);
            // 
            // btn_tab3_SaveLocal
            // 
            this.btn_tab3_SaveLocal.Location = new System.Drawing.Point(656, 247);
            this.btn_tab3_SaveLocal.Name = "btn_tab3_SaveLocal";
            this.btn_tab3_SaveLocal.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_SaveLocal.TabIndex = 19;
            this.btn_tab3_SaveLocal.Text = "Save Local";
            this.btn_tab3_SaveLocal.UseVisualStyleBackColor = true;
            this.btn_tab3_SaveLocal.Click += new System.EventHandler(this.btn_tab3_SaveLocal_Click);
            // 
            // btn_tab3_MovePallet
            // 
            this.btn_tab3_MovePallet.Location = new System.Drawing.Point(545, 468);
            this.btn_tab3_MovePallet.Name = "btn_tab3_MovePallet";
            this.btn_tab3_MovePallet.Size = new System.Drawing.Size(199, 28);
            this.btn_tab3_MovePallet.TabIndex = 19;
            this.btn_tab3_MovePallet.Text = "Move  Pallet1  -->  Pallet2";
            this.btn_tab3_MovePallet.UseVisualStyleBackColor = true;
            this.btn_tab3_MovePallet.Click += new System.EventHandler(this.btn_tab3_MovePallet_Click);
            // 
            // btn_tab3_Pallet1
            // 
            this.btn_tab3_Pallet1.Location = new System.Drawing.Point(545, 424);
            this.btn_tab3_Pallet1.Name = "btn_tab3_Pallet1";
            this.btn_tab3_Pallet1.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_Pallet1.TabIndex = 19;
            this.btn_tab3_Pallet1.Text = "Pallet1";
            this.btn_tab3_Pallet1.UseVisualStyleBackColor = true;
            this.btn_tab3_Pallet1.Click += new System.EventHandler(this.btn_tab3_Pallet1_Click);
            // 
            // btn_tab3_RandomSN
            // 
            this.btn_tab3_RandomSN.Location = new System.Drawing.Point(545, 288);
            this.btn_tab3_RandomSN.Name = "btn_tab3_RandomSN";
            this.btn_tab3_RandomSN.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_RandomSN.TabIndex = 19;
            this.btn_tab3_RandomSN.Text = "Random SN";
            this.btn_tab3_RandomSN.UseVisualStyleBackColor = true;
            this.btn_tab3_RandomSN.Click += new System.EventHandler(this.btn_tab3_RandomSN_Click);
            // 
            // btn_tab3_LoadLocal
            // 
            this.btn_tab3_LoadLocal.Location = new System.Drawing.Point(545, 247);
            this.btn_tab3_LoadLocal.Name = "btn_tab3_LoadLocal";
            this.btn_tab3_LoadLocal.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_LoadLocal.TabIndex = 19;
            this.btn_tab3_LoadLocal.Text = "Load Local";
            this.btn_tab3_LoadLocal.UseVisualStyleBackColor = true;
            this.btn_tab3_LoadLocal.Click += new System.EventHandler(this.btn_tab3_LoadLocal_Click);
            // 
            // btn_tab3_SendPalletInfo
            // 
            this.btn_tab3_SendPalletInfo.Location = new System.Drawing.Point(656, 125);
            this.btn_tab3_SendPalletInfo.Name = "btn_tab3_SendPalletInfo";
            this.btn_tab3_SendPalletInfo.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_SendPalletInfo.TabIndex = 19;
            this.btn_tab3_SendPalletInfo.Text = "SendPalletInfo";
            this.btn_tab3_SendPalletInfo.UseVisualStyleBackColor = true;
            this.btn_tab3_SendPalletInfo.Click += new System.EventHandler(this.btn_tab3_SendPalletInfo_Click);
            // 
            // btn_tab3_ReqPalletInfo
            // 
            this.btn_tab3_ReqPalletInfo.Location = new System.Drawing.Point(545, 125);
            this.btn_tab3_ReqPalletInfo.Name = "btn_tab3_ReqPalletInfo";
            this.btn_tab3_ReqPalletInfo.Size = new System.Drawing.Size(88, 28);
            this.btn_tab3_ReqPalletInfo.TabIndex = 19;
            this.btn_tab3_ReqPalletInfo.Text = "ReqPalletInfo";
            this.btn_tab3_ReqPalletInfo.UseVisualStyleBackColor = true;
            this.btn_tab3_ReqPalletInfo.Click += new System.EventHandler(this.btn_tab3_ReqPalletInfo_Click);
            // 
            // radioButton_DisabledPallet
            // 
            this.radioButton_DisabledPallet.AutoSize = true;
            this.radioButton_DisabledPallet.Location = new System.Drawing.Point(157, 188);
            this.radioButton_DisabledPallet.Name = "radioButton_DisabledPallet";
            this.radioButton_DisabledPallet.Size = new System.Drawing.Size(66, 17);
            this.radioButton_DisabledPallet.TabIndex = 18;
            this.radioButton_DisabledPallet.TabStop = true;
            this.radioButton_DisabledPallet.Text = "Disabled";
            this.radioButton_DisabledPallet.UseVisualStyleBackColor = true;
            // 
            // radioButton_EnabledPallet
            // 
            this.radioButton_EnabledPallet.AutoSize = true;
            this.radioButton_EnabledPallet.Location = new System.Drawing.Point(87, 189);
            this.radioButton_EnabledPallet.Name = "radioButton_EnabledPallet";
            this.radioButton_EnabledPallet.Size = new System.Drawing.Size(64, 17);
            this.radioButton_EnabledPallet.TabIndex = 18;
            this.radioButton_EnabledPallet.TabStop = true;
            this.radioButton_EnabledPallet.Text = "Enabled";
            this.radioButton_EnabledPallet.UseVisualStyleBackColor = true;
            // 
            // lbl_tab3_EnabledPallet
            // 
            this.lbl_tab3_EnabledPallet.AutoSize = true;
            this.lbl_tab3_EnabledPallet.Location = new System.Drawing.Point(6, 192);
            this.lbl_tab3_EnabledPallet.Name = "lbl_tab3_EnabledPallet";
            this.lbl_tab3_EnabledPallet.Size = new System.Drawing.Size(75, 13);
            this.lbl_tab3_EnabledPallet.TabIndex = 17;
            this.lbl_tab3_EnabledPallet.Text = "EnabledPallet:";
            // 
            // lbl_tab3_Defect10
            // 
            this.lbl_tab3_Defect10.AutoSize = true;
            this.lbl_tab3_Defect10.Location = new System.Drawing.Point(218, 502);
            this.lbl_tab3_Defect10.Name = "lbl_tab3_Defect10";
            this.lbl_tab3_Defect10.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect10.TabIndex = 17;
            this.lbl_tab3_Defect10.Text = "Defect:";
            // 
            // lbl_tab3_Defect9
            // 
            this.lbl_tab3_Defect9.AutoSize = true;
            this.lbl_tab3_Defect9.Location = new System.Drawing.Point(218, 476);
            this.lbl_tab3_Defect9.Name = "lbl_tab3_Defect9";
            this.lbl_tab3_Defect9.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect9.TabIndex = 17;
            this.lbl_tab3_Defect9.Text = "Defect:";
            // 
            // lbl_tab3_Defect8
            // 
            this.lbl_tab3_Defect8.AutoSize = true;
            this.lbl_tab3_Defect8.Location = new System.Drawing.Point(218, 450);
            this.lbl_tab3_Defect8.Name = "lbl_tab3_Defect8";
            this.lbl_tab3_Defect8.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect8.TabIndex = 17;
            this.lbl_tab3_Defect8.Text = "Defect:";
            // 
            // lbl_tab3_Defect7
            // 
            this.lbl_tab3_Defect7.AutoSize = true;
            this.lbl_tab3_Defect7.Location = new System.Drawing.Point(218, 424);
            this.lbl_tab3_Defect7.Name = "lbl_tab3_Defect7";
            this.lbl_tab3_Defect7.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect7.TabIndex = 17;
            this.lbl_tab3_Defect7.Text = "Defect:";
            // 
            // lbl_tab3_Defect6
            // 
            this.lbl_tab3_Defect6.AutoSize = true;
            this.lbl_tab3_Defect6.Location = new System.Drawing.Point(218, 398);
            this.lbl_tab3_Defect6.Name = "lbl_tab3_Defect6";
            this.lbl_tab3_Defect6.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect6.TabIndex = 17;
            this.lbl_tab3_Defect6.Text = "Defect:";
            // 
            // lbl_tab3_Defect5
            // 
            this.lbl_tab3_Defect5.AutoSize = true;
            this.lbl_tab3_Defect5.Location = new System.Drawing.Point(218, 359);
            this.lbl_tab3_Defect5.Name = "lbl_tab3_Defect5";
            this.lbl_tab3_Defect5.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect5.TabIndex = 17;
            this.lbl_tab3_Defect5.Text = "Defect:";
            // 
            // lbl_tab3_Defect4
            // 
            this.lbl_tab3_Defect4.AutoSize = true;
            this.lbl_tab3_Defect4.Location = new System.Drawing.Point(218, 333);
            this.lbl_tab3_Defect4.Name = "lbl_tab3_Defect4";
            this.lbl_tab3_Defect4.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect4.TabIndex = 17;
            this.lbl_tab3_Defect4.Text = "Defect:";
            // 
            // lbl_tab3_Defect3
            // 
            this.lbl_tab3_Defect3.AutoSize = true;
            this.lbl_tab3_Defect3.Location = new System.Drawing.Point(218, 307);
            this.lbl_tab3_Defect3.Name = "lbl_tab3_Defect3";
            this.lbl_tab3_Defect3.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect3.TabIndex = 17;
            this.lbl_tab3_Defect3.Text = "Defect:";
            // 
            // lbl_tab3_Defect2
            // 
            this.lbl_tab3_Defect2.AutoSize = true;
            this.lbl_tab3_Defect2.Location = new System.Drawing.Point(218, 281);
            this.lbl_tab3_Defect2.Name = "lbl_tab3_Defect2";
            this.lbl_tab3_Defect2.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect2.TabIndex = 17;
            this.lbl_tab3_Defect2.Text = "Defect:";
            // 
            // lbl_tab3_Defect1
            // 
            this.lbl_tab3_Defect1.AutoSize = true;
            this.lbl_tab3_Defect1.Location = new System.Drawing.Point(218, 255);
            this.lbl_tab3_Defect1.Name = "lbl_tab3_Defect1";
            this.lbl_tab3_Defect1.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_Defect1.TabIndex = 17;
            this.lbl_tab3_Defect1.Text = "Defect:";
            // 
            // lbl_tab3_SN10
            // 
            this.lbl_tab3_SN10.AutoSize = true;
            this.lbl_tab3_SN10.Location = new System.Drawing.Point(54, 502);
            this.lbl_tab3_SN10.Name = "lbl_tab3_SN10";
            this.lbl_tab3_SN10.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN10.TabIndex = 17;
            this.lbl_tab3_SN10.Text = "SN:";
            // 
            // lbl_tab3_SN9
            // 
            this.lbl_tab3_SN9.AutoSize = true;
            this.lbl_tab3_SN9.Location = new System.Drawing.Point(54, 476);
            this.lbl_tab3_SN9.Name = "lbl_tab3_SN9";
            this.lbl_tab3_SN9.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN9.TabIndex = 17;
            this.lbl_tab3_SN9.Text = "SN:";
            // 
            // lbl_tab3_SN8
            // 
            this.lbl_tab3_SN8.AutoSize = true;
            this.lbl_tab3_SN8.Location = new System.Drawing.Point(54, 450);
            this.lbl_tab3_SN8.Name = "lbl_tab3_SN8";
            this.lbl_tab3_SN8.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN8.TabIndex = 17;
            this.lbl_tab3_SN8.Text = "SN:";
            // 
            // lbl_tab3_SN7
            // 
            this.lbl_tab3_SN7.AutoSize = true;
            this.lbl_tab3_SN7.Location = new System.Drawing.Point(54, 424);
            this.lbl_tab3_SN7.Name = "lbl_tab3_SN7";
            this.lbl_tab3_SN7.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN7.TabIndex = 17;
            this.lbl_tab3_SN7.Text = "SN:";
            // 
            // lbl_tab3_SN6
            // 
            this.lbl_tab3_SN6.AutoSize = true;
            this.lbl_tab3_SN6.Location = new System.Drawing.Point(54, 398);
            this.lbl_tab3_SN6.Name = "lbl_tab3_SN6";
            this.lbl_tab3_SN6.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN6.TabIndex = 17;
            this.lbl_tab3_SN6.Text = "SN:";
            // 
            // lbl_tab3_SN5
            // 
            this.lbl_tab3_SN5.AutoSize = true;
            this.lbl_tab3_SN5.Location = new System.Drawing.Point(54, 359);
            this.lbl_tab3_SN5.Name = "lbl_tab3_SN5";
            this.lbl_tab3_SN5.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN5.TabIndex = 17;
            this.lbl_tab3_SN5.Text = "SN:";
            // 
            // lbl_tab3_SN4
            // 
            this.lbl_tab3_SN4.AutoSize = true;
            this.lbl_tab3_SN4.Location = new System.Drawing.Point(54, 333);
            this.lbl_tab3_SN4.Name = "lbl_tab3_SN4";
            this.lbl_tab3_SN4.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN4.TabIndex = 17;
            this.lbl_tab3_SN4.Text = "SN:";
            // 
            // lbl_tab3_SN3
            // 
            this.lbl_tab3_SN3.AutoSize = true;
            this.lbl_tab3_SN3.Location = new System.Drawing.Point(54, 307);
            this.lbl_tab3_SN3.Name = "lbl_tab3_SN3";
            this.lbl_tab3_SN3.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN3.TabIndex = 17;
            this.lbl_tab3_SN3.Text = "SN:";
            // 
            // lbl_tab3_SN2
            // 
            this.lbl_tab3_SN2.AutoSize = true;
            this.lbl_tab3_SN2.Location = new System.Drawing.Point(54, 281);
            this.lbl_tab3_SN2.Name = "lbl_tab3_SN2";
            this.lbl_tab3_SN2.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN2.TabIndex = 17;
            this.lbl_tab3_SN2.Text = "SN:";
            // 
            // lbl_tab3_SN1
            // 
            this.lbl_tab3_SN1.AutoSize = true;
            this.lbl_tab3_SN1.Location = new System.Drawing.Point(54, 255);
            this.lbl_tab3_SN1.Name = "lbl_tab3_SN1";
            this.lbl_tab3_SN1.Size = new System.Drawing.Size(25, 13);
            this.lbl_tab3_SN1.TabIndex = 17;
            this.lbl_tab3_SN1.Text = "SN:";
            // 
            // lbl_tab3_HGA10
            // 
            this.lbl_tab3_HGA10.AutoSize = true;
            this.lbl_tab3_HGA10.Location = new System.Drawing.Point(6, 502);
            this.lbl_tab3_HGA10.Name = "lbl_tab3_HGA10";
            this.lbl_tab3_HGA10.Size = new System.Drawing.Size(42, 13);
            this.lbl_tab3_HGA10.TabIndex = 17;
            this.lbl_tab3_HGA10.Text = "HGA10";
            // 
            // lbl_tab3_HGA9
            // 
            this.lbl_tab3_HGA9.AutoSize = true;
            this.lbl_tab3_HGA9.Location = new System.Drawing.Point(6, 476);
            this.lbl_tab3_HGA9.Name = "lbl_tab3_HGA9";
            this.lbl_tab3_HGA9.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA9.TabIndex = 17;
            this.lbl_tab3_HGA9.Text = "HGA9";
            // 
            // lbl_tab3_HGA8
            // 
            this.lbl_tab3_HGA8.AutoSize = true;
            this.lbl_tab3_HGA8.Location = new System.Drawing.Point(6, 450);
            this.lbl_tab3_HGA8.Name = "lbl_tab3_HGA8";
            this.lbl_tab3_HGA8.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA8.TabIndex = 17;
            this.lbl_tab3_HGA8.Text = "HGA8";
            // 
            // lbl_tab3_HGA7
            // 
            this.lbl_tab3_HGA7.AutoSize = true;
            this.lbl_tab3_HGA7.Location = new System.Drawing.Point(6, 424);
            this.lbl_tab3_HGA7.Name = "lbl_tab3_HGA7";
            this.lbl_tab3_HGA7.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA7.TabIndex = 17;
            this.lbl_tab3_HGA7.Text = "HGA7";
            // 
            // lbl_tab3_HGA6
            // 
            this.lbl_tab3_HGA6.AutoSize = true;
            this.lbl_tab3_HGA6.Location = new System.Drawing.Point(6, 398);
            this.lbl_tab3_HGA6.Name = "lbl_tab3_HGA6";
            this.lbl_tab3_HGA6.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA6.TabIndex = 17;
            this.lbl_tab3_HGA6.Text = "HGA6";
            // 
            // lbl_tab3_HGA5
            // 
            this.lbl_tab3_HGA5.AutoSize = true;
            this.lbl_tab3_HGA5.Location = new System.Drawing.Point(6, 359);
            this.lbl_tab3_HGA5.Name = "lbl_tab3_HGA5";
            this.lbl_tab3_HGA5.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA5.TabIndex = 17;
            this.lbl_tab3_HGA5.Text = "HGA5";
            // 
            // lbl_tab3_HGA4
            // 
            this.lbl_tab3_HGA4.AutoSize = true;
            this.lbl_tab3_HGA4.Location = new System.Drawing.Point(6, 333);
            this.lbl_tab3_HGA4.Name = "lbl_tab3_HGA4";
            this.lbl_tab3_HGA4.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA4.TabIndex = 17;
            this.lbl_tab3_HGA4.Text = "HGA4";
            // 
            // lbl_tab3_HGA3
            // 
            this.lbl_tab3_HGA3.AutoSize = true;
            this.lbl_tab3_HGA3.Location = new System.Drawing.Point(6, 307);
            this.lbl_tab3_HGA3.Name = "lbl_tab3_HGA3";
            this.lbl_tab3_HGA3.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA3.TabIndex = 17;
            this.lbl_tab3_HGA3.Text = "HGA3";
            // 
            // lbl_tab3_HGA2
            // 
            this.lbl_tab3_HGA2.AutoSize = true;
            this.lbl_tab3_HGA2.Location = new System.Drawing.Point(6, 281);
            this.lbl_tab3_HGA2.Name = "lbl_tab3_HGA2";
            this.lbl_tab3_HGA2.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA2.TabIndex = 17;
            this.lbl_tab3_HGA2.Text = "HGA2";
            // 
            // lbl_tab3_HGA1
            // 
            this.lbl_tab3_HGA1.AutoSize = true;
            this.lbl_tab3_HGA1.Location = new System.Drawing.Point(6, 255);
            this.lbl_tab3_HGA1.Name = "lbl_tab3_HGA1";
            this.lbl_tab3_HGA1.Size = new System.Drawing.Size(36, 13);
            this.lbl_tab3_HGA1.TabIndex = 17;
            this.lbl_tab3_HGA1.Text = "HGA1";
            // 
            // lbl_tab3_HGAType
            // 
            this.lbl_tab3_HGAType.AutoSize = true;
            this.lbl_tab3_HGAType.Location = new System.Drawing.Point(6, 215);
            this.lbl_tab3_HGAType.Name = "lbl_tab3_HGAType";
            this.lbl_tab3_HGAType.Size = new System.Drawing.Size(57, 13);
            this.lbl_tab3_HGAType.TabIndex = 17;
            this.lbl_tab3_HGAType.Text = "HGAType:";
            // 
            // lbl_tab3_ALMID
            // 
            this.lbl_tab3_ALMID.AutoSize = true;
            this.lbl_tab3_ALMID.Location = new System.Drawing.Point(218, 62);
            this.lbl_tab3_ALMID.Name = "lbl_tab3_ALMID";
            this.lbl_tab3_ALMID.Size = new System.Drawing.Size(43, 13);
            this.lbl_tab3_ALMID.TabIndex = 17;
            this.lbl_tab3_ALMID.Text = "ALMID:";
            // 
            // lbl_tab3_Suspension
            // 
            this.lbl_tab3_Suspension.AutoSize = true;
            this.lbl_tab3_Suspension.Location = new System.Drawing.Point(6, 166);
            this.lbl_tab3_Suspension.Name = "lbl_tab3_Suspension";
            this.lbl_tab3_Suspension.Size = new System.Drawing.Size(65, 13);
            this.lbl_tab3_Suspension.TabIndex = 17;
            this.lbl_tab3_Suspension.Text = "Suspension:";
            // 
            // lbl_tab3_COMMACK
            // 
            this.lbl_tab3_COMMACK.AutoSize = true;
            this.lbl_tab3_COMMACK.Location = new System.Drawing.Point(218, 36);
            this.lbl_tab3_COMMACK.Name = "lbl_tab3_COMMACK";
            this.lbl_tab3_COMMACK.Size = new System.Drawing.Size(64, 13);
            this.lbl_tab3_COMMACK.TabIndex = 17;
            this.lbl_tab3_COMMACK.Text = "COMMACK:";
            // 
            // lbl_tab3_ACAMID
            // 
            this.lbl_tab3_ACAMID.AutoSize = true;
            this.lbl_tab3_ACAMID.Location = new System.Drawing.Point(6, 140);
            this.lbl_tab3_ACAMID.Name = "lbl_tab3_ACAMID";
            this.lbl_tab3_ACAMID.Size = new System.Drawing.Size(51, 13);
            this.lbl_tab3_ACAMID.TabIndex = 17;
            this.lbl_tab3_ACAMID.Text = "ACAMID:";
            // 
            // lbl_tab3_Line
            // 
            this.lbl_tab3_Line.AutoSize = true;
            this.lbl_tab3_Line.Location = new System.Drawing.Point(6, 114);
            this.lbl_tab3_Line.Name = "lbl_tab3_Line";
            this.lbl_tab3_Line.Size = new System.Drawing.Size(30, 13);
            this.lbl_tab3_Line.TabIndex = 17;
            this.lbl_tab3_Line.Text = "Line:";
            // 
            // lbl_tab3_LotNumber
            // 
            this.lbl_tab3_LotNumber.AutoSize = true;
            this.lbl_tab3_LotNumber.Location = new System.Drawing.Point(6, 88);
            this.lbl_tab3_LotNumber.Name = "lbl_tab3_LotNumber";
            this.lbl_tab3_LotNumber.Size = new System.Drawing.Size(62, 13);
            this.lbl_tab3_LotNumber.TabIndex = 17;
            this.lbl_tab3_LotNumber.Text = "LotNumber:";
            // 
            // lbl_tab3_ProductName
            // 
            this.lbl_tab3_ProductName.AutoSize = true;
            this.lbl_tab3_ProductName.Location = new System.Drawing.Point(6, 62);
            this.lbl_tab3_ProductName.Name = "lbl_tab3_ProductName";
            this.lbl_tab3_ProductName.Size = new System.Drawing.Size(75, 13);
            this.lbl_tab3_ProductName.TabIndex = 17;
            this.lbl_tab3_ProductName.Text = "ProductName:";
            // 
            // lbl_tab3_PartNumber
            // 
            this.lbl_tab3_PartNumber.AutoSize = true;
            this.lbl_tab3_PartNumber.Location = new System.Drawing.Point(6, 36);
            this.lbl_tab3_PartNumber.Name = "lbl_tab3_PartNumber";
            this.lbl_tab3_PartNumber.Size = new System.Drawing.Size(66, 13);
            this.lbl_tab3_PartNumber.TabIndex = 17;
            this.lbl_tab3_PartNumber.Text = "PartNumber:";
            // 
            // lbl_tab3_PalletID
            // 
            this.lbl_tab3_PalletID.AutoSize = true;
            this.lbl_tab3_PalletID.Location = new System.Drawing.Point(6, 10);
            this.lbl_tab3_PalletID.Name = "lbl_tab3_PalletID";
            this.lbl_tab3_PalletID.Size = new System.Drawing.Size(47, 13);
            this.lbl_tab3_PalletID.TabIndex = 17;
            this.lbl_tab3_PalletID.Text = "PalletID:";
            // 
            // txtbox_tab3_Defect10
            // 
            this.txtbox_tab3_Defect10.Location = new System.Drawing.Point(266, 499);
            this.txtbox_tab3_Defect10.Name = "txtbox_tab3_Defect10";
            this.txtbox_tab3_Defect10.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect10.TabIndex = 0;
            // 
            // txtbox_tab3_SN10
            // 
            this.txtbox_tab3_SN10.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN10.Location = new System.Drawing.Point(85, 499);
            this.txtbox_tab3_SN10.Name = "txtbox_tab3_SN10";
            this.txtbox_tab3_SN10.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN10.TabIndex = 0;
            // 
            // txtbox_tab3_Defect9
            // 
            this.txtbox_tab3_Defect9.Location = new System.Drawing.Point(266, 473);
            this.txtbox_tab3_Defect9.Name = "txtbox_tab3_Defect9";
            this.txtbox_tab3_Defect9.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect9.TabIndex = 0;
            // 
            // txtbox_tab3_SN9
            // 
            this.txtbox_tab3_SN9.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN9.Location = new System.Drawing.Point(85, 473);
            this.txtbox_tab3_SN9.Name = "txtbox_tab3_SN9";
            this.txtbox_tab3_SN9.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN9.TabIndex = 0;
            // 
            // txtbox_tab3_Defect8
            // 
            this.txtbox_tab3_Defect8.Location = new System.Drawing.Point(266, 447);
            this.txtbox_tab3_Defect8.Name = "txtbox_tab3_Defect8";
            this.txtbox_tab3_Defect8.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect8.TabIndex = 0;
            // 
            // txtbox_tab3_SN8
            // 
            this.txtbox_tab3_SN8.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN8.Location = new System.Drawing.Point(85, 447);
            this.txtbox_tab3_SN8.Name = "txtbox_tab3_SN8";
            this.txtbox_tab3_SN8.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN8.TabIndex = 0;
            // 
            // txtbox_tab3_Defect7
            // 
            this.txtbox_tab3_Defect7.Location = new System.Drawing.Point(266, 421);
            this.txtbox_tab3_Defect7.Name = "txtbox_tab3_Defect7";
            this.txtbox_tab3_Defect7.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect7.TabIndex = 0;
            // 
            // txtbox_tab3_SN7
            // 
            this.txtbox_tab3_SN7.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN7.Location = new System.Drawing.Point(85, 421);
            this.txtbox_tab3_SN7.Name = "txtbox_tab3_SN7";
            this.txtbox_tab3_SN7.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN7.TabIndex = 0;
            // 
            // txtbox_tab3_Defect6
            // 
            this.txtbox_tab3_Defect6.Location = new System.Drawing.Point(266, 395);
            this.txtbox_tab3_Defect6.Name = "txtbox_tab3_Defect6";
            this.txtbox_tab3_Defect6.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect6.TabIndex = 0;
            // 
            // txtbox_tab3_SN6
            // 
            this.txtbox_tab3_SN6.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN6.Location = new System.Drawing.Point(85, 395);
            this.txtbox_tab3_SN6.Name = "txtbox_tab3_SN6";
            this.txtbox_tab3_SN6.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN6.TabIndex = 0;
            // 
            // txtbox_tab3_Defect5
            // 
            this.txtbox_tab3_Defect5.Location = new System.Drawing.Point(266, 356);
            this.txtbox_tab3_Defect5.Name = "txtbox_tab3_Defect5";
            this.txtbox_tab3_Defect5.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect5.TabIndex = 0;
            // 
            // txtbox_tab3_SN5
            // 
            this.txtbox_tab3_SN5.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN5.Location = new System.Drawing.Point(85, 356);
            this.txtbox_tab3_SN5.Name = "txtbox_tab3_SN5";
            this.txtbox_tab3_SN5.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN5.TabIndex = 0;
            // 
            // txtbox_tab3_Defect4
            // 
            this.txtbox_tab3_Defect4.Location = new System.Drawing.Point(266, 330);
            this.txtbox_tab3_Defect4.Name = "txtbox_tab3_Defect4";
            this.txtbox_tab3_Defect4.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect4.TabIndex = 0;
            // 
            // txtbox_tab3_SN4
            // 
            this.txtbox_tab3_SN4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN4.Location = new System.Drawing.Point(85, 330);
            this.txtbox_tab3_SN4.Name = "txtbox_tab3_SN4";
            this.txtbox_tab3_SN4.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN4.TabIndex = 0;
            // 
            // txtbox_tab3_Defect3
            // 
            this.txtbox_tab3_Defect3.Location = new System.Drawing.Point(266, 304);
            this.txtbox_tab3_Defect3.Name = "txtbox_tab3_Defect3";
            this.txtbox_tab3_Defect3.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect3.TabIndex = 0;
            // 
            // txtbox_tab3_SN3
            // 
            this.txtbox_tab3_SN3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN3.Location = new System.Drawing.Point(85, 304);
            this.txtbox_tab3_SN3.Name = "txtbox_tab3_SN3";
            this.txtbox_tab3_SN3.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN3.TabIndex = 0;
            // 
            // txtbox_tab3_Defect2
            // 
            this.txtbox_tab3_Defect2.Location = new System.Drawing.Point(266, 278);
            this.txtbox_tab3_Defect2.Name = "txtbox_tab3_Defect2";
            this.txtbox_tab3_Defect2.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect2.TabIndex = 0;
            // 
            // txtbox_tab3_SN2
            // 
            this.txtbox_tab3_SN2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN2.Location = new System.Drawing.Point(85, 278);
            this.txtbox_tab3_SN2.Name = "txtbox_tab3_SN2";
            this.txtbox_tab3_SN2.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN2.TabIndex = 0;
            // 
            // txtbox_tab3_Defect1
            // 
            this.txtbox_tab3_Defect1.Location = new System.Drawing.Point(266, 252);
            this.txtbox_tab3_Defect1.Name = "txtbox_tab3_Defect1";
            this.txtbox_tab3_Defect1.Size = new System.Drawing.Size(188, 20);
            this.txtbox_tab3_Defect1.TabIndex = 0;
            // 
            // txtbox_tab3_SN1
            // 
            this.txtbox_tab3_SN1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SN1.Location = new System.Drawing.Point(85, 252);
            this.txtbox_tab3_SN1.Name = "txtbox_tab3_SN1";
            this.txtbox_tab3_SN1.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_SN1.TabIndex = 0;
            // 
            // txtbox_tab3_HGAType
            // 
            this.txtbox_tab3_HGAType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_HGAType.Location = new System.Drawing.Point(85, 212);
            this.txtbox_tab3_HGAType.Name = "txtbox_tab3_HGAType";
            this.txtbox_tab3_HGAType.ReadOnly = true;
            this.txtbox_tab3_HGAType.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_HGAType.TabIndex = 0;
            // 
            // txtbox_tab3_ALMID
            // 
            this.txtbox_tab3_ALMID.Location = new System.Drawing.Point(288, 59);
            this.txtbox_tab3_ALMID.Name = "txtbox_tab3_ALMID";
            this.txtbox_tab3_ALMID.ReadOnly = true;
            this.txtbox_tab3_ALMID.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_ALMID.TabIndex = 0;
            this.txtbox_tab3_ALMID.Text = "0";
            // 
            // txtbox_tab3_COMMACK
            // 
            this.txtbox_tab3_COMMACK.Location = new System.Drawing.Point(288, 33);
            this.txtbox_tab3_COMMACK.Name = "txtbox_tab3_COMMACK";
            this.txtbox_tab3_COMMACK.ReadOnly = true;
            this.txtbox_tab3_COMMACK.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_COMMACK.TabIndex = 0;
            this.txtbox_tab3_COMMACK.Text = "0";
            // 
            // txtbox_tab3_Suspension
            // 
            this.txtbox_tab3_Suspension.Location = new System.Drawing.Point(85, 163);
            this.txtbox_tab3_Suspension.Name = "txtbox_tab3_Suspension";
            this.txtbox_tab3_Suspension.ReadOnly = true;
            this.txtbox_tab3_Suspension.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_Suspension.TabIndex = 0;
            // 
            // txtbox_tab3_CureZone
            // 
            this.txtbox_tab3_CureZone.Location = new System.Drawing.Point(655, 59);
            this.txtbox_tab3_CureZone.Name = "txtbox_tab3_CureZone";
            this.txtbox_tab3_CureZone.ReadOnly = true;
            this.txtbox_tab3_CureZone.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_CureZone.TabIndex = 0;
            // 
            // txtbox_tab3_CureTime
            // 
            this.txtbox_tab3_CureTime.Location = new System.Drawing.Point(655, 33);
            this.txtbox_tab3_CureTime.Name = "txtbox_tab3_CureTime";
            this.txtbox_tab3_CureTime.ReadOnly = true;
            this.txtbox_tab3_CureTime.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_CureTime.TabIndex = 0;
            // 
            // txtbox_tab3_UVPower
            // 
            this.txtbox_tab3_UVPower.Location = new System.Drawing.Point(655, 7);
            this.txtbox_tab3_UVPower.Name = "txtbox_tab3_UVPower";
            this.txtbox_tab3_UVPower.ReadOnly = true;
            this.txtbox_tab3_UVPower.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_UVPower.TabIndex = 0;
            // 
            // txtbox_tab3_ACAMID
            // 
            this.txtbox_tab3_ACAMID.Location = new System.Drawing.Point(383, 7);
            this.txtbox_tab3_ACAMID.Name = "txtbox_tab3_ACAMID";
            this.txtbox_tab3_ACAMID.ReadOnly = true;
            this.txtbox_tab3_ACAMID.Size = new System.Drawing.Size(101, 20);
            this.txtbox_tab3_ACAMID.TabIndex = 0;
            // 
            // txtbox_tab3_Line
            // 
            this.txtbox_tab3_Line.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_Line.Location = new System.Drawing.Point(85, 111);
            this.txtbox_tab3_Line.Name = "txtbox_tab3_Line";
            this.txtbox_tab3_Line.ReadOnly = true;
            this.txtbox_tab3_Line.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_Line.TabIndex = 0;
            // 
            // txtbox_tab3_LotNumber
            // 
            this.txtbox_tab3_LotNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_LotNumber.Location = new System.Drawing.Point(85, 85);
            this.txtbox_tab3_LotNumber.Name = "txtbox_tab3_LotNumber";
            this.txtbox_tab3_LotNumber.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_LotNumber.TabIndex = 0;
            // 
            // txtbox_tab3_ProductName
            // 
            this.txtbox_tab3_ProductName.Location = new System.Drawing.Point(85, 59);
            this.txtbox_tab3_ProductName.Name = "txtbox_tab3_ProductName";
            this.txtbox_tab3_ProductName.ReadOnly = true;
            this.txtbox_tab3_ProductName.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_ProductName.TabIndex = 0;
            // 
            // txtbox_tab3_PartNumber
            // 
            this.txtbox_tab3_PartNumber.Location = new System.Drawing.Point(85, 33);
            this.txtbox_tab3_PartNumber.Name = "txtbox_tab3_PartNumber";
            this.txtbox_tab3_PartNumber.ReadOnly = true;
            this.txtbox_tab3_PartNumber.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_PartNumber.TabIndex = 0;
            // 
            // txtbox_tab3_SJBFixture
            // 
            this.txtbox_tab3_SJBFixture.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_SJBFixture.Location = new System.Drawing.Point(449, 33);
            this.txtbox_tab3_SJBFixture.Name = "txtbox_tab3_SJBFixture";
            this.txtbox_tab3_SJBFixture.Size = new System.Drawing.Size(35, 20);
            this.txtbox_tab3_SJBFixture.TabIndex = 0;
            // 
            // txtbox_tab3_transID
            // 
            this.txtbox_tab3_transID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_transID.Location = new System.Drawing.Point(288, 7);
            this.txtbox_tab3_transID.Name = "txtbox_tab3_transID";
            this.txtbox_tab3_transID.ReadOnly = true;
            this.txtbox_tab3_transID.Size = new System.Drawing.Size(89, 20);
            this.txtbox_tab3_transID.TabIndex = 0;
            // 
            // txtbox_tab3_Pallet2
            // 
            this.txtbox_tab3_Pallet2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_Pallet2.Location = new System.Drawing.Point(656, 395);
            this.txtbox_tab3_Pallet2.Name = "txtbox_tab3_Pallet2";
            this.txtbox_tab3_Pallet2.Size = new System.Drawing.Size(88, 20);
            this.txtbox_tab3_Pallet2.TabIndex = 0;
            this.txtbox_tab3_Pallet2.TextChanged += new System.EventHandler(this.txtbox_tab3_Pallet2_TextChanged);
            // 
            // txtbox_tab3_Pallet1
            // 
            this.txtbox_tab3_Pallet1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_Pallet1.Location = new System.Drawing.Point(545, 395);
            this.txtbox_tab3_Pallet1.Name = "txtbox_tab3_Pallet1";
            this.txtbox_tab3_Pallet1.Size = new System.Drawing.Size(88, 20);
            this.txtbox_tab3_Pallet1.TabIndex = 0;
            this.txtbox_tab3_Pallet1.TextChanged += new System.EventHandler(this.txtbox_tab3_Pallet1_TextChanged);
            // 
            // txtbox_tab3_PalletID
            // 
            this.txtbox_tab3_PalletID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbox_tab3_PalletID.Location = new System.Drawing.Point(85, 7);
            this.txtbox_tab3_PalletID.Name = "txtbox_tab3_PalletID";
            this.txtbox_tab3_PalletID.Size = new System.Drawing.Size(112, 20);
            this.txtbox_tab3_PalletID.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lbl_tab4_error);
            this.tabPage4.Controls.Add(this.btn_tab4_NewAllPallets);
            this.tabPage4.Controls.Add(this.lbl_tab4_PalletList);
            this.tabPage4.Controls.Add(this.txtbox_tab4_PalletList);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(774, 535);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "NewPallet";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lbl_tab4_error
            // 
            this.lbl_tab4_error.AutoSize = true;
            this.lbl_tab4_error.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_tab4_error.ForeColor = System.Drawing.Color.Red;
            this.lbl_tab4_error.Location = new System.Drawing.Point(6, 21);
            this.lbl_tab4_error.Name = "lbl_tab4_error";
            this.lbl_tab4_error.Size = new System.Drawing.Size(85, 13);
            this.lbl_tab4_error.TabIndex = 27;
            this.lbl_tab4_error.Text = "lbl_tab4_error";
            // 
            // btn_tab4_NewAllPallets
            // 
            this.btn_tab4_NewAllPallets.Location = new System.Drawing.Point(6, 264);
            this.btn_tab4_NewAllPallets.Name = "btn_tab4_NewAllPallets";
            this.btn_tab4_NewAllPallets.Size = new System.Drawing.Size(88, 28);
            this.btn_tab4_NewAllPallets.TabIndex = 25;
            this.btn_tab4_NewAllPallets.Text = "New All Pallets";
            this.btn_tab4_NewAllPallets.UseVisualStyleBackColor = true;
            this.btn_tab4_NewAllPallets.Click += new System.EventHandler(this.btn_tab4_NewAllPallets_Click);
            // 
            // lbl_tab4_PalletList
            // 
            this.lbl_tab4_PalletList.AutoSize = true;
            this.lbl_tab4_PalletList.Location = new System.Drawing.Point(6, 45);
            this.lbl_tab4_PalletList.Name = "lbl_tab4_PalletList";
            this.lbl_tab4_PalletList.Size = new System.Drawing.Size(118, 13);
            this.lbl_tab4_PalletList.TabIndex = 1;
            this.lbl_tab4_PalletList.Text = "List of Pallets (Barcode)";
            // 
            // txtbox_tab4_PalletList
            // 
            this.txtbox_tab4_PalletList.Location = new System.Drawing.Point(6, 61);
            this.txtbox_tab4_PalletList.Multiline = true;
            this.txtbox_tab4_PalletList.Name = "txtbox_tab4_PalletList";
            this.txtbox_tab4_PalletList.Size = new System.Drawing.Size(435, 197);
            this.txtbox_tab4_PalletList.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.txtboxPendingACAMPalletSN);
            this.tabPage5.Controls.Add(this.txtboxPendingACAMSuspAmt);
            this.tabPage5.Controls.Add(this.txtboxPendingACAMId);
            this.tabPage5.Controls.Add(this.txtboxPendingACAMACAMId);
            this.tabPage5.Controls.Add(this.lbl_tab5_pendingACAM);
            this.tabPage5.Controls.Add(this.lbl_tab5_reqsusp);
            this.tabPage5.Controls.Add(this.lbl_tab5_error);
            this.tabPage5.Controls.Add(this.btnPendingACAMRefresh);
            this.tabPage5.Controls.Add(this.btnReqSuspRefresh);
            this.tabPage5.Controls.Add(this.lblReqSuspSuspAmt);
            this.tabPage5.Controls.Add(this.lblReqSuspACAMID);
            this.tabPage5.Controls.Add(this.btnPendingACAMIdSave);
            this.tabPage5.Controls.Add(this.btnPendingACAMCancel);
            this.tabPage5.Controls.Add(this.btnDeleteReqSusp);
            this.tabPage5.Controls.Add(this.btnMultiReqSusp50);
            this.tabPage5.Controls.Add(this.btnMultiReqSusp10);
            this.tabPage5.Controls.Add(this.btnReqSusp);
            this.tabPage5.Controls.Add(this.txtboxReqSuspSuspAmt);
            this.tabPage5.Controls.Add(this.txtboxReqSuspACAMID);
            this.tabPage5.Controls.Add(this.lstviewPendingACAM);
            this.tabPage5.Controls.Add(this.lstviewReqSusp);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(774, 535);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "ReqSusp";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // txtboxPendingACAMPalletSN
            // 
            this.txtboxPendingACAMPalletSN.Location = new System.Drawing.Point(200, 468);
            this.txtboxPendingACAMPalletSN.Name = "txtboxPendingACAMPalletSN";
            this.txtboxPendingACAMPalletSN.ReadOnly = true;
            this.txtboxPendingACAMPalletSN.Size = new System.Drawing.Size(96, 20);
            this.txtboxPendingACAMPalletSN.TabIndex = 40;
            // 
            // txtboxPendingACAMSuspAmt
            // 
            this.txtboxPendingACAMSuspAmt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtboxPendingACAMSuspAmt.Location = new System.Drawing.Point(137, 468);
            this.txtboxPendingACAMSuspAmt.Name = "txtboxPendingACAMSuspAmt";
            this.txtboxPendingACAMSuspAmt.Size = new System.Drawing.Size(60, 20);
            this.txtboxPendingACAMSuspAmt.TabIndex = 40;
            // 
            // txtboxPendingACAMId
            // 
            this.txtboxPendingACAMId.Location = new System.Drawing.Point(7, 468);
            this.txtboxPendingACAMId.Name = "txtboxPendingACAMId";
            this.txtboxPendingACAMId.ReadOnly = true;
            this.txtboxPendingACAMId.Size = new System.Drawing.Size(29, 20);
            this.txtboxPendingACAMId.TabIndex = 40;
            // 
            // txtboxPendingACAMACAMId
            // 
            this.txtboxPendingACAMACAMId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtboxPendingACAMACAMId.Location = new System.Drawing.Point(39, 468);
            this.txtboxPendingACAMACAMId.Name = "txtboxPendingACAMACAMId";
            this.txtboxPendingACAMACAMId.Size = new System.Drawing.Size(95, 20);
            this.txtboxPendingACAMACAMId.TabIndex = 40;
            // 
            // lbl_tab5_pendingACAM
            // 
            this.lbl_tab5_pendingACAM.AutoSize = true;
            this.lbl_tab5_pendingACAM.Location = new System.Drawing.Point(6, 309);
            this.lbl_tab5_pendingACAM.Name = "lbl_tab5_pendingACAM";
            this.lbl_tab5_pendingACAM.Size = new System.Drawing.Size(132, 13);
            this.lbl_tab5_pendingACAM.TabIndex = 39;
            this.lbl_tab5_pendingACAM.Text = "Pallet List Pending ACAMs";
            // 
            // lbl_tab5_reqsusp
            // 
            this.lbl_tab5_reqsusp.AutoSize = true;
            this.lbl_tab5_reqsusp.Location = new System.Drawing.Point(6, 14);
            this.lbl_tab5_reqsusp.Name = "lbl_tab5_reqsusp";
            this.lbl_tab5_reqsusp.Size = new System.Drawing.Size(166, 13);
            this.lbl_tab5_reqsusp.TabIndex = 39;
            this.lbl_tab5_reqsusp.Text = "Request Suspension by ACAM ID";
            // 
            // lbl_tab5_error
            // 
            this.lbl_tab5_error.AutoSize = true;
            this.lbl_tab5_error.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_tab5_error.ForeColor = System.Drawing.Color.Red;
            this.lbl_tab5_error.Location = new System.Drawing.Point(6, 263);
            this.lbl_tab5_error.Name = "lbl_tab5_error";
            this.lbl_tab5_error.Size = new System.Drawing.Size(85, 13);
            this.lbl_tab5_error.TabIndex = 38;
            this.lbl_tab5_error.Text = "lbl_tab5_error";
            // 
            // btnPendingACAMRefresh
            // 
            this.btnPendingACAMRefresh.Location = new System.Drawing.Point(680, 468);
            this.btnPendingACAMRefresh.Name = "btnPendingACAMRefresh";
            this.btnPendingACAMRefresh.Size = new System.Drawing.Size(88, 28);
            this.btnPendingACAMRefresh.TabIndex = 37;
            this.btnPendingACAMRefresh.Text = "Refresh";
            this.btnPendingACAMRefresh.UseVisualStyleBackColor = true;
            this.btnPendingACAMRefresh.Click += new System.EventHandler(this.btnPendingACAMRefresh_Click);
            // 
            // btnReqSuspRefresh
            // 
            this.btnReqSuspRefresh.Location = new System.Drawing.Point(680, 222);
            this.btnReqSuspRefresh.Name = "btnReqSuspRefresh";
            this.btnReqSuspRefresh.Size = new System.Drawing.Size(88, 28);
            this.btnReqSuspRefresh.TabIndex = 37;
            this.btnReqSuspRefresh.Text = "Refresh";
            this.btnReqSuspRefresh.UseVisualStyleBackColor = true;
            this.btnReqSuspRefresh.Click += new System.EventHandler(this.btnReqSuspRefresh_Click);
            // 
            // lblReqSuspSuspAmt
            // 
            this.lblReqSuspSuspAmt.AutoSize = true;
            this.lblReqSuspSuspAmt.Location = new System.Drawing.Point(166, 180);
            this.lblReqSuspSuspAmt.Name = "lblReqSuspSuspAmt";
            this.lblReqSuspSuspAmt.Size = new System.Drawing.Size(49, 13);
            this.lblReqSuspSuspAmt.TabIndex = 35;
            this.lblReqSuspSuspAmt.Text = "SuspAmt";
            // 
            // lblReqSuspACAMID
            // 
            this.lblReqSuspACAMID.AutoSize = true;
            this.lblReqSuspACAMID.Location = new System.Drawing.Point(6, 180);
            this.lblReqSuspACAMID.Name = "lblReqSuspACAMID";
            this.lblReqSuspACAMID.Size = new System.Drawing.Size(48, 13);
            this.lblReqSuspACAMID.TabIndex = 36;
            this.lblReqSuspACAMID.Text = "ACAMID";
            // 
            // btnPendingACAMIdSave
            // 
            this.btnPendingACAMIdSave.Location = new System.Drawing.Point(300, 468);
            this.btnPendingACAMIdSave.Name = "btnPendingACAMIdSave";
            this.btnPendingACAMIdSave.Size = new System.Drawing.Size(88, 28);
            this.btnPendingACAMIdSave.TabIndex = 34;
            this.btnPendingACAMIdSave.Text = "Edit/Save";
            this.btnPendingACAMIdSave.UseVisualStyleBackColor = true;
            this.btnPendingACAMIdSave.Click += new System.EventHandler(this.btnPendingACAMIdSave_Click);
            // 
            // btnPendingACAMCancel
            // 
            this.btnPendingACAMCancel.Location = new System.Drawing.Point(586, 468);
            this.btnPendingACAMCancel.Name = "btnPendingACAMCancel";
            this.btnPendingACAMCancel.Size = new System.Drawing.Size(88, 28);
            this.btnPendingACAMCancel.TabIndex = 34;
            this.btnPendingACAMCancel.Text = "Cancel Pallet";
            this.btnPendingACAMCancel.UseVisualStyleBackColor = true;
            this.btnPendingACAMCancel.Click += new System.EventHandler(this.btnPendingACAMCancel_Click);
            // 
            // btnDeleteReqSusp
            // 
            this.btnDeleteReqSusp.Location = new System.Drawing.Point(586, 222);
            this.btnDeleteReqSusp.Name = "btnDeleteReqSusp";
            this.btnDeleteReqSusp.Size = new System.Drawing.Size(88, 28);
            this.btnDeleteReqSusp.TabIndex = 34;
            this.btnDeleteReqSusp.Text = "Del Req";
            this.btnDeleteReqSusp.UseVisualStyleBackColor = true;
            this.btnDeleteReqSusp.Click += new System.EventHandler(this.btnDeleteReqSusp_Click);
            // 
            // btnMultiReqSusp50
            // 
            this.btnMultiReqSusp50.Location = new System.Drawing.Point(197, 222);
            this.btnMultiReqSusp50.Name = "btnMultiReqSusp50";
            this.btnMultiReqSusp50.Size = new System.Drawing.Size(88, 28);
            this.btnMultiReqSusp50.TabIndex = 34;
            this.btnMultiReqSusp50.Text = "Req x50";
            this.btnMultiReqSusp50.UseVisualStyleBackColor = true;
            this.btnMultiReqSusp50.Click += new System.EventHandler(this.btnMultiReqSusp50_Click);
            // 
            // btnMultiReqSusp10
            // 
            this.btnMultiReqSusp10.Location = new System.Drawing.Point(103, 222);
            this.btnMultiReqSusp10.Name = "btnMultiReqSusp10";
            this.btnMultiReqSusp10.Size = new System.Drawing.Size(88, 28);
            this.btnMultiReqSusp10.TabIndex = 34;
            this.btnMultiReqSusp10.Text = "Req x10";
            this.btnMultiReqSusp10.UseVisualStyleBackColor = true;
            this.btnMultiReqSusp10.Click += new System.EventHandler(this.btnMultiReqSusp10_Click);
            // 
            // btnReqSusp
            // 
            this.btnReqSusp.Location = new System.Drawing.Point(9, 222);
            this.btnReqSusp.Name = "btnReqSusp";
            this.btnReqSusp.Size = new System.Drawing.Size(88, 28);
            this.btnReqSusp.TabIndex = 34;
            this.btnReqSusp.Text = "Req Susp";
            this.btnReqSusp.UseVisualStyleBackColor = true;
            this.btnReqSusp.Click += new System.EventHandler(this.btnReqSusp_Click);
            // 
            // txtboxReqSuspSuspAmt
            // 
            this.txtboxReqSuspSuspAmt.Location = new System.Drawing.Point(169, 196);
            this.txtboxReqSuspSuspAmt.Name = "txtboxReqSuspSuspAmt";
            this.txtboxReqSuspSuspAmt.Size = new System.Drawing.Size(90, 20);
            this.txtboxReqSuspSuspAmt.TabIndex = 33;
            this.txtboxReqSuspSuspAmt.Text = "10";
            // 
            // txtboxReqSuspACAMID
            // 
            this.txtboxReqSuspACAMID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtboxReqSuspACAMID.Location = new System.Drawing.Point(9, 196);
            this.txtboxReqSuspACAMID.Name = "txtboxReqSuspACAMID";
            this.txtboxReqSuspACAMID.Size = new System.Drawing.Size(154, 20);
            this.txtboxReqSuspACAMID.TabIndex = 32;
            this.txtboxReqSuspACAMID.Text = "APT001";
            // 
            // lstviewPendingACAM
            // 
            this.lstviewPendingACAM.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18});
            this.lstviewPendingACAM.FullRowSelect = true;
            this.lstviewPendingACAM.GridLines = true;
            this.lstviewPendingACAM.HideSelection = false;
            this.lstviewPendingACAM.Location = new System.Drawing.Point(6, 325);
            this.lstviewPendingACAM.MultiSelect = false;
            this.lstviewPendingACAM.Name = "lstviewPendingACAM";
            this.lstviewPendingACAM.Size = new System.Drawing.Size(762, 137);
            this.lstviewPendingACAM.TabIndex = 15;
            this.lstviewPendingACAM.UseCompatibleStateImageBehavior = false;
            this.lstviewPendingACAM.View = System.Windows.Forms.View.Details;
            this.lstviewPendingACAM.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstviewPendingACAM_ItemSelectionChanged);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Id";
            this.columnHeader10.Width = 30;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "ACAMID";
            this.columnHeader11.Width = 100;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "SuspAmt";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "PalletSN";
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "TransID";
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "dbo_pallet_Id";
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "CreatedDateTime";
            this.columnHeader16.Width = 120;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "IsProcessed";
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "UpdatedDateTime";
            this.columnHeader18.Width = 120;
            // 
            // lstviewReqSusp
            // 
            this.lstviewReqSusp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lstviewReqSusp.FullRowSelect = true;
            this.lstviewReqSusp.GridLines = true;
            this.lstviewReqSusp.HideSelection = false;
            this.lstviewReqSusp.Location = new System.Drawing.Point(6, 30);
            this.lstviewReqSusp.MultiSelect = false;
            this.lstviewReqSusp.Name = "lstviewReqSusp";
            this.lstviewReqSusp.Size = new System.Drawing.Size(762, 137);
            this.lstviewReqSusp.TabIndex = 15;
            this.lstviewReqSusp.UseCompatibleStateImageBehavior = false;
            this.lstviewReqSusp.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Id";
            this.columnHeader1.Width = 30;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ACAMID";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "SuspAmt";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "PalletSN";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "TransID";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "dbo_pallet_Id";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "CreatedDateTime";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "IsProcessed";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "UpdatedDateTime";
            this.columnHeader9.Width = 120;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTextMenuContext1,
            this.showXMLMenuContext1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(122, 48);
            this.contextMenuStrip1.Text = "PrimaryMessage";
            // 
            // copyTextMenuContext1
            // 
            this.copyTextMenuContext1.Name = "copyTextMenuContext1";
            this.copyTextMenuContext1.Size = new System.Drawing.Size(121, 22);
            this.copyTextMenuContext1.Text = "CopyText";
            this.copyTextMenuContext1.Click += new System.EventHandler(this.copyTextMenuContext1_Click);
            // 
            // showXMLMenuContext1
            // 
            this.showXMLMenuContext1.Name = "showXMLMenuContext1";
            this.showXMLMenuContext1.Size = new System.Drawing.Size(121, 22);
            this.showXMLMenuContext1.Text = "ShowXML";
            this.showXMLMenuContext1.Click += new System.EventHandler(this.showXMLMenuContext1_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTextMenuContext2,
            this.showXMLMenuContext2});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(122, 48);
            // 
            // copyTextMenuContext2
            // 
            this.copyTextMenuContext2.Name = "copyTextMenuContext2";
            this.copyTextMenuContext2.Size = new System.Drawing.Size(121, 22);
            this.copyTextMenuContext2.Text = "CopyText";
            this.copyTextMenuContext2.Click += new System.EventHandler(this.copyTextMenuContext2_Click);
            // 
            // showXMLMenuContext2
            // 
            this.showXMLMenuContext2.Name = "showXMLMenuContext2";
            this.showXMLMenuContext2.Size = new System.Drawing.Size(121, 22);
            this.showXMLMenuContext2.Text = "ShowXML";
            this.showXMLMenuContext2.Click += new System.EventHandler(this.showXMLMenuContext2_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 570);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Host - Dev";
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView MasterDataGridView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox SecondaryMessageTextBox;
        private System.Windows.Forms.TextBox ErrorMessageTextBox;
        private System.Windows.Forms.TextBox PrimaryMessageTextBox;
        private System.Windows.Forms.GroupBox ErrorMessageGroupBox;
        private System.Windows.Forms.GroupBox SecondaryGroupBox;
        private System.Windows.Forms.GroupBox PrimaryGroupBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtboxHGAPartNumber;
        private System.Windows.Forms.Label lblHGAPartNumber;
        private System.Windows.Forms.ListView lstviewLotDetails;
        private System.Windows.Forms.ColumnHeader colLotNumber;
        private System.Windows.Forms.ColumnHeader colHGAPartNumber;
        private System.Windows.Forms.ColumnHeader colSuspension;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colSTR;
        private System.Windows.Forms.ColumnHeader colLineNo;
        private System.Windows.Forms.ColumnHeader colQTY;
        private System.Windows.Forms.ColumnHeader colUpdatedTimestamp;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox txtboxAPIKey;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.TextBox txtboxLine;
        private System.Windows.Forms.Button btnGetLots;
        private System.Windows.Forms.Button btnRegisterLine;
        private System.Windows.Forms.Label lblWebServiceStatus;
        private System.Windows.Forms.Button btnGenRecipe;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RadioButton radioButton_DisabledPallet;
        private System.Windows.Forms.RadioButton radioButton_EnabledPallet;
        private System.Windows.Forms.Label lbl_tab3_EnabledPallet;
        private System.Windows.Forms.Label lbl_tab3_Suspension;
        private System.Windows.Forms.Label lbl_tab3_ACAMID;
        private System.Windows.Forms.Label lbl_tab3_Line;
        private System.Windows.Forms.Label lbl_tab3_LotNumber;
        private System.Windows.Forms.Label lbl_tab3_ProductName;
        private System.Windows.Forms.Label lbl_tab3_PartNumber;
        private System.Windows.Forms.Label lbl_tab3_PalletID;
        private System.Windows.Forms.TextBox txtbox_tab3_Suspension;
        private System.Windows.Forms.TextBox txtbox_tab3_ACAMID;
        private System.Windows.Forms.TextBox txtbox_tab3_Line;
        private System.Windows.Forms.TextBox txtbox_tab3_LotNumber;
        private System.Windows.Forms.TextBox txtbox_tab3_ProductName;
        private System.Windows.Forms.TextBox txtbox_tab3_PartNumber;
        private System.Windows.Forms.TextBox txtbox_tab3_PalletID;
        private System.Windows.Forms.Button btn_tab3_SendPalletInfo;
        private System.Windows.Forms.Button btn_tab3_ReqPalletInfo;
        private System.Windows.Forms.Label lbl_tab3_Defect10;
        private System.Windows.Forms.Label lbl_tab3_Defect9;
        private System.Windows.Forms.Label lbl_tab3_Defect8;
        private System.Windows.Forms.Label lbl_tab3_Defect7;
        private System.Windows.Forms.Label lbl_tab3_Defect6;
        private System.Windows.Forms.Label lbl_tab3_Defect5;
        private System.Windows.Forms.Label lbl_tab3_Defect4;
        private System.Windows.Forms.Label lbl_tab3_Defect3;
        private System.Windows.Forms.Label lbl_tab3_Defect2;
        private System.Windows.Forms.Label lbl_tab3_Defect1;
        private System.Windows.Forms.Label lbl_tab3_SN10;
        private System.Windows.Forms.Label lbl_tab3_SN9;
        private System.Windows.Forms.Label lbl_tab3_SN8;
        private System.Windows.Forms.Label lbl_tab3_SN7;
        private System.Windows.Forms.Label lbl_tab3_SN6;
        private System.Windows.Forms.Label lbl_tab3_SN5;
        private System.Windows.Forms.Label lbl_tab3_SN4;
        private System.Windows.Forms.Label lbl_tab3_SN3;
        private System.Windows.Forms.Label lbl_tab3_SN2;
        private System.Windows.Forms.Label lbl_tab3_SN1;
        private System.Windows.Forms.Label lbl_tab3_HGA10;
        private System.Windows.Forms.Label lbl_tab3_HGA9;
        private System.Windows.Forms.Label lbl_tab3_HGA8;
        private System.Windows.Forms.Label lbl_tab3_HGA7;
        private System.Windows.Forms.Label lbl_tab3_HGA6;
        private System.Windows.Forms.Label lbl_tab3_HGA5;
        private System.Windows.Forms.Label lbl_tab3_HGA4;
        private System.Windows.Forms.Label lbl_tab3_HGA3;
        private System.Windows.Forms.Label lbl_tab3_HGA2;
        private System.Windows.Forms.Label lbl_tab3_HGA1;
        private System.Windows.Forms.Label lbl_tab3_HGAType;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect10;
        private System.Windows.Forms.TextBox txtbox_tab3_SN10;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect9;
        private System.Windows.Forms.TextBox txtbox_tab3_SN9;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect8;
        private System.Windows.Forms.TextBox txtbox_tab3_SN8;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect7;
        private System.Windows.Forms.TextBox txtbox_tab3_SN7;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect6;
        private System.Windows.Forms.TextBox txtbox_tab3_SN6;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect5;
        private System.Windows.Forms.TextBox txtbox_tab3_SN5;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect4;
        private System.Windows.Forms.TextBox txtbox_tab3_SN4;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect3;
        private System.Windows.Forms.TextBox txtbox_tab3_SN3;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect2;
        private System.Windows.Forms.TextBox txtbox_tab3_SN2;
        private System.Windows.Forms.TextBox txtbox_tab3_Defect1;
        private System.Windows.Forms.TextBox txtbox_tab3_SN1;
        private System.Windows.Forms.TextBox txtbox_tab3_HGAType;
        private System.Windows.Forms.Button btn_tab3_SaveLocal;
        private System.Windows.Forms.Button btn_tab3_LoadLocal;
        private System.Windows.Forms.Label lbl_tab3_ALMID;
        private System.Windows.Forms.Label lbl_tab3_COMMACK;
        private System.Windows.Forms.TextBox txtbox_tab3_ALMID;
        private System.Windows.Forms.TextBox txtbox_tab3_COMMACK;
        private System.Windows.Forms.ListView lstviewRecipe;
        private System.Windows.Forms.ColumnHeader colRecipe_ID;
        private System.Windows.Forms.ColumnHeader colRecipe_PartNumber;
        private System.Windows.Forms.ColumnHeader colRecipe_ProductName;
        private System.Windows.Forms.ColumnHeader colRecipe_Suspension;
        private System.Windows.Forms.ColumnHeader colRecipe_HGAType;
        private System.Windows.Forms.ColumnHeader colRecipe_Line;
        private System.Windows.Forms.ColumnHeader colRecipe_PalletID;
        private System.Windows.Forms.Button btnClearRecipe;
        private System.Windows.Forms.Button btnGetLotsOffline;
        private System.Windows.Forms.ColumnHeader colProgram;
        private System.Windows.Forms.ColumnHeader colRecipe_STR;
        private System.Windows.Forms.ColumnHeader colSuspPartNumber;
        private System.Windows.Forms.ColumnHeader colRecipe_SuspPartNumber;
        private System.Windows.Forms.Label lbl_tab3_error;
        private System.Windows.Forms.ListBox lstboxEquipmentType;
        private System.Windows.Forms.Label lblEquipmentType;
        private System.Windows.Forms.ListBox lstboxNextEquipmentType;
        private System.Windows.Forms.Label lblNextEquipmentType;
        private System.Windows.Forms.Label lbl_tab3_transID;
        private System.Windows.Forms.TextBox txtbox_tab3_transID;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyTextMenuContext1;
        private System.Windows.Forms.Button btn_tab3_RandomSN;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem copyTextMenuContext2;
        private System.Windows.Forms.ToolStripMenuItem showXMLMenuContext1;
        private System.Windows.Forms.ToolStripMenuItem showXMLMenuContext2;
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
        private System.Windows.Forms.Button btn_tab3_NewPallet;
        private System.Windows.Forms.Button btn_tab3_ClearData;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btn_tab4_NewAllPallets;
        private System.Windows.Forms.Label lbl_tab4_PalletList;
        private System.Windows.Forms.TextBox txtbox_tab4_PalletList;
        private System.Windows.Forms.Label lbl_tab4_error;
        private System.Windows.Forms.Button btn_tab3_Pallet2;
        private System.Windows.Forms.Button btn_tab3_MovePallet;
        private System.Windows.Forms.Button btn_tab3_Pallet1;
        private System.Windows.Forms.TextBox txtbox_tab3_Pallet2;
        private System.Windows.Forms.TextBox txtbox_tab3_Pallet1;
        private System.Windows.Forms.Button btn_tab3_NoHGA1;
        private System.Windows.Forms.Button btn_tab3_NoHGA2;
        private System.Windows.Forms.Button btn_tab3_NoHGA10;
        private System.Windows.Forms.Button btn_tab3_NoHGA9;
        private System.Windows.Forms.Button btn_tab3_NoHGA8;
        private System.Windows.Forms.Button btn_tab3_NoHGA7;
        private System.Windows.Forms.Button btn_tab3_NoHGA6;
        private System.Windows.Forms.Button btn_tab3_NoHGA5;
        private System.Windows.Forms.Button btn_tab3_NoHGA4;
        private System.Windows.Forms.Button btn_tab3_NoHGA3;
        private System.Windows.Forms.ComboBox combo_tab3_ACAMID;
        private System.Windows.Forms.TextBox txtbox_tab3_transID_Pallet2;
        private System.Windows.Forms.TextBox txtbox_tab3_transID_Pallet1;
        private System.Windows.Forms.Label lbl_tab3_transID_Pallet2;
        private System.Windows.Forms.Label lbl_tab3_transID_Pallet1;
        private System.Windows.Forms.Button btnExportLotInfo;
        private System.Windows.Forms.Label lbl_tab3_CureZone;
        private System.Windows.Forms.Label lbl_tab3_CureTime;
        private System.Windows.Forms.Label lbl_tab3_UVPower;
        private System.Windows.Forms.TextBox txtbox_tab3_CureZone;
        private System.Windows.Forms.TextBox txtbox_tab3_CureTime;
        private System.Windows.Forms.TextBox txtbox_tab3_UVPower;
        private System.Windows.Forms.Label lbl_tab3_SJBFixture;
        private System.Windows.Forms.TextBox txtbox_tab3_SJBFixture;
        private System.Windows.Forms.Button btnReqPalletASLV;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnReqSuspRefresh;
        private System.Windows.Forms.Label lblReqSuspSuspAmt;
        private System.Windows.Forms.Label lblReqSuspACAMID;
        private System.Windows.Forms.Button btnReqSusp;
        private System.Windows.Forms.TextBox txtboxReqSuspSuspAmt;
        private System.Windows.Forms.TextBox txtboxReqSuspACAMID;
        private System.Windows.Forms.ListView lstviewReqSusp;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Label lbl_tab5_error;
        private System.Windows.Forms.Button btnMultiReqSusp10;
        private System.Windows.Forms.Button btnMultiReqSusp50;
        private System.Windows.Forms.Button btnDeleteReqSusp;
        private System.Windows.Forms.Label lbl_tab5_pendingACAM;
        private System.Windows.Forms.Label lbl_tab5_reqsusp;
        private System.Windows.Forms.Button btnPendingACAMRefresh;
        private System.Windows.Forms.ListView lstviewPendingACAM;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.Button btnPendingACAMCancel;
        private System.Windows.Forms.TextBox txtboxPendingACAMPalletSN;
        private System.Windows.Forms.TextBox txtboxPendingACAMSuspAmt;
        private System.Windows.Forms.TextBox txtboxPendingACAMId;
        private System.Windows.Forms.TextBox txtboxPendingACAMACAMId;
        private System.Windows.Forms.Button btnPendingACAMIdSave;
    }
}