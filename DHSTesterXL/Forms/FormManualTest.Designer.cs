namespace DHSTesterXL.Forms
{
    partial class FormManualTest
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTestSettingsFrame = new System.Windows.Forms.Panel();
            this.panelTestSettings = new System.Windows.Forms.Panel();
            this.gridTestList = new System.Windows.Forms.DataGridView();
            this.labelTestSettingsTitle = new System.Windows.Forms.Label();
            this.panelXCPAddressFrame = new System.Windows.Forms.Panel();
            this.panelXCPAddress = new System.Windows.Forms.Panel();
            this.labelXCPAddressTitle = new System.Windows.Forms.Label();
            this.colTestNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTestFunction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOption = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonTestStart = new System.Windows.Forms.Button();
            this.buttonTestStop = new System.Windows.Forms.Button();
            this.panelTestSettingsFrame.SuspendLayout();
            this.panelTestSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTestList)).BeginInit();
            this.panelXCPAddressFrame.SuspendLayout();
            this.panelXCPAddress.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTestSettingsFrame
            // 
            this.panelTestSettingsFrame.BackColor = System.Drawing.Color.LightGray;
            this.panelTestSettingsFrame.Controls.Add(this.panelTestSettings);
            this.panelTestSettingsFrame.Location = new System.Drawing.Point(502, 20);
            this.panelTestSettingsFrame.Margin = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.panelTestSettingsFrame.Name = "panelTestSettingsFrame";
            this.panelTestSettingsFrame.Size = new System.Drawing.Size(702, 752);
            this.panelTestSettingsFrame.TabIndex = 7;
            // 
            // panelTestSettings
            // 
            this.panelTestSettings.BackColor = System.Drawing.SystemColors.Control;
            this.panelTestSettings.Controls.Add(this.gridTestList);
            this.panelTestSettings.Controls.Add(this.labelTestSettingsTitle);
            this.panelTestSettings.Location = new System.Drawing.Point(1, 1);
            this.panelTestSettings.Name = "panelTestSettings";
            this.panelTestSettings.Size = new System.Drawing.Size(700, 730);
            this.panelTestSettings.TabIndex = 0;
            // 
            // gridTestList
            // 
            this.gridTestList.AllowUserToAddRows = false;
            this.gridTestList.AllowUserToDeleteRows = false;
            this.gridTestList.AllowUserToResizeColumns = false;
            this.gridTestList.AllowUserToResizeRows = false;
            this.gridTestList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTestList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridTestList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridTestList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridTestList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTestNo,
            this.colTestUse,
            this.colTestFunction,
            this.colTestMin,
            this.colTestMax,
            this.colOption,
            this.colValue});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridTestList.DefaultCellStyle = dataGridViewCellStyle4;
            this.gridTestList.GridColor = System.Drawing.Color.LightGray;
            this.gridTestList.Location = new System.Drawing.Point(10, 30);
            this.gridTestList.Name = "gridTestList";
            this.gridTestList.RowHeadersVisible = false;
            this.gridTestList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridTestList.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridTestList.RowTemplate.Height = 23;
            this.gridTestList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTestList.Size = new System.Drawing.Size(680, 690);
            this.gridTestList.TabIndex = 5;
            this.gridTestList.Tag = "TestList";
            // 
            // labelTestSettingsTitle
            // 
            this.labelTestSettingsTitle.BackColor = System.Drawing.Color.LightGray;
            this.labelTestSettingsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTestSettingsTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTestSettingsTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTestSettingsTitle.Name = "labelTestSettingsTitle";
            this.labelTestSettingsTitle.Size = new System.Drawing.Size(700, 20);
            this.labelTestSettingsTitle.TabIndex = 4;
            this.labelTestSettingsTitle.Text = "검사 항목";
            this.labelTestSettingsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelXCPAddressFrame
            // 
            this.panelXCPAddressFrame.BackColor = System.Drawing.Color.LightGray;
            this.panelXCPAddressFrame.Controls.Add(this.panelXCPAddress);
            this.panelXCPAddressFrame.Location = new System.Drawing.Point(20, 20);
            this.panelXCPAddressFrame.Margin = new System.Windows.Forms.Padding(20, 20, 0, 0);
            this.panelXCPAddressFrame.Name = "panelXCPAddressFrame";
            this.panelXCPAddressFrame.Size = new System.Drawing.Size(462, 752);
            this.panelXCPAddressFrame.TabIndex = 9;
            // 
            // panelXCPAddress
            // 
            this.panelXCPAddress.BackColor = System.Drawing.SystemColors.Control;
            this.panelXCPAddress.Controls.Add(this.labelXCPAddressTitle);
            this.panelXCPAddress.Location = new System.Drawing.Point(1, 1);
            this.panelXCPAddress.Name = "panelXCPAddress";
            this.panelXCPAddress.Size = new System.Drawing.Size(460, 730);
            this.panelXCPAddress.TabIndex = 0;
            // 
            // labelXCPAddressTitle
            // 
            this.labelXCPAddressTitle.BackColor = System.Drawing.Color.LightGray;
            this.labelXCPAddressTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelXCPAddressTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelXCPAddressTitle.Location = new System.Drawing.Point(0, 0);
            this.labelXCPAddressTitle.Name = "labelXCPAddressTitle";
            this.labelXCPAddressTitle.Size = new System.Drawing.Size(460, 20);
            this.labelXCPAddressTitle.TabIndex = 5;
            this.labelXCPAddressTitle.Text = "LOG";
            this.labelXCPAddressTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // colTestNo
            // 
            this.colTestNo.HeaderText = "No";
            this.colTestNo.Name = "colTestNo";
            this.colTestNo.ReadOnly = true;
            this.colTestNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTestNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTestNo.Width = 50;
            // 
            // colTestUse
            // 
            this.colTestUse.HeaderText = "측정";
            this.colTestUse.Name = "colTestUse";
            this.colTestUse.Width = 50;
            // 
            // colTestFunction
            // 
            this.colTestFunction.HeaderText = "항목";
            this.colTestFunction.Name = "colTestFunction";
            this.colTestFunction.ReadOnly = true;
            this.colTestFunction.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTestFunction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTestFunction.Width = 120;
            // 
            // colTestMin
            // 
            this.colTestMin.HeaderText = "최소값";
            this.colTestMin.Name = "colTestMin";
            this.colTestMin.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTestMin.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTestMin.Width = 150;
            // 
            // colTestMax
            // 
            this.colTestMax.HeaderText = "최대값";
            this.colTestMax.Name = "colTestMax";
            this.colTestMax.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTestMax.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTestMax.Width = 150;
            // 
            // colOption
            // 
            this.colOption.HeaderText = "옵션";
            this.colOption.Name = "colOption";
            this.colOption.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colOption.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colOption.Width = 60;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.HeaderText = "측정값";
            this.colValue.Name = "colValue";
            this.colValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // buttonTestStart
            // 
            this.buttonTestStart.Location = new System.Drawing.Point(1263, 21);
            this.buttonTestStart.Name = "buttonTestStart";
            this.buttonTestStart.Size = new System.Drawing.Size(100, 30);
            this.buttonTestStart.TabIndex = 10;
            this.buttonTestStart.Text = "START";
            this.buttonTestStart.UseVisualStyleBackColor = true;
            this.buttonTestStart.Click += new System.EventHandler(this.buttonTestStart_Click);
            // 
            // buttonTestStop
            // 
            this.buttonTestStop.Location = new System.Drawing.Point(1369, 21);
            this.buttonTestStop.Name = "buttonTestStop";
            this.buttonTestStop.Size = new System.Drawing.Size(100, 30);
            this.buttonTestStop.TabIndex = 10;
            this.buttonTestStop.Text = "STOP";
            this.buttonTestStop.UseVisualStyleBackColor = true;
            this.buttonTestStop.Click += new System.EventHandler(this.buttonTestStop_Click);
            // 
            // FormManualTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1584, 811);
            this.Controls.Add(this.buttonTestStop);
            this.Controls.Add(this.buttonTestStart);
            this.Controls.Add(this.panelXCPAddressFrame);
            this.Controls.Add(this.panelTestSettingsFrame);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormManualTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormManualTest";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormManualTest_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormManualTest_FormClosed);
            this.Load += new System.EventHandler(this.FormManualTest_Load);
            this.Shown += new System.EventHandler(this.FormManualTest_Shown);
            this.panelTestSettingsFrame.ResumeLayout(false);
            this.panelTestSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTestList)).EndInit();
            this.panelXCPAddressFrame.ResumeLayout(false);
            this.panelXCPAddress.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTestSettingsFrame;
        private System.Windows.Forms.Panel panelTestSettings;
        private System.Windows.Forms.DataGridView gridTestList;
        private System.Windows.Forms.Label labelTestSettingsTitle;
        private System.Windows.Forms.Panel panelXCPAddressFrame;
        private System.Windows.Forms.Panel panelXCPAddress;
        private System.Windows.Forms.Label labelXCPAddressTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestNo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTestUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOption;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.Button buttonTestStart;
        private System.Windows.Forms.Button buttonTestStop;
    }
}