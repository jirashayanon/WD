namespace COVID19TempLogApp
{
    partial class Form1
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
            this.txtboxID = new System.Windows.Forms.TextBox();
            this.txtboxTemp = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblExceptionMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtboxID
            // 
            this.txtboxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxID.Location = new System.Drawing.Point(120, 162);
            this.txtboxID.Name = "txtboxID";
            this.txtboxID.Size = new System.Drawing.Size(223, 38);
            this.txtboxID.TabIndex = 0;
            this.txtboxID.Text = "1000132781";
            this.txtboxID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtboxID.WordWrap = false;
            this.txtboxID.Validated += new System.EventHandler(this.txtboxID_Validated);
            // 
            // txtboxTemp
            // 
            this.txtboxTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxTemp.Location = new System.Drawing.Point(386, 162);
            this.txtboxTemp.Name = "txtboxTemp";
            this.txtboxTemp.Size = new System.Drawing.Size(148, 38);
            this.txtboxTemp.TabIndex = 1;
            this.txtboxTemp.Text = "36.5";
            this.txtboxTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtboxTemp.WordWrap = false;
            this.txtboxTemp.Validated += new System.EventHandler(this.txtboxTemp_Validated);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Location = new System.Drawing.Point(254, 249);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(141, 60);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Location = new System.Drawing.Point(84, 39);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(481, 38);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(115, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 29);
            this.label1.TabIndex = 4;
            this.label1.Text = "Employee ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(381, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = "Temperature";
            // 
            // lblExceptionMessage
            // 
            this.lblExceptionMessage.AutoSize = true;
            this.lblExceptionMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExceptionMessage.Location = new System.Drawing.Point(81, 335);
            this.lblExceptionMessage.Name = "lblExceptionMessage";
            this.lblExceptionMessage.Size = new System.Drawing.Size(130, 17);
            this.lblExceptionMessage.TabIndex = 5;
            this.lblExceptionMessage.Text = "Exception message";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 388);
            this.Controls.Add(this.lblExceptionMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.txtboxTemp);
            this.Controls.Add(this.txtboxID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "COVID19 - Temperature Record";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtboxID;
        private System.Windows.Forms.TextBox txtboxTemp;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblExceptionMessage;
    }
}

