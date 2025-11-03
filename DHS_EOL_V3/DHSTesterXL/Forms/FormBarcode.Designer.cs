namespace DHSTesterXL
{
    partial class FormBarcode
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
            this.label1 = new System.Windows.Forms.Label();
            this.panelProductBarcode = new System.Windows.Forms.Panel();
            this.textProductBarcode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelTrayBarcode = new System.Windows.Forms.Panel();
            this.textTrayBarcode = new System.Windows.Forms.TextBox();
            this.numericTrayCount = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.numericProductCount = new System.Windows.Forms.NumericUpDown();
            this.checkRetry = new System.Windows.Forms.CheckBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.panelProductBarcode.SuspendLayout();
            this.panelTrayBarcode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrayCount)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericProductCount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(3, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 45);
            this.label1.TabIndex = 1;
            this.label1.Text = "품목";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelProductBarcode
            // 
            this.panelProductBarcode.BackColor = System.Drawing.Color.White;
            this.panelProductBarcode.Controls.Add(this.textProductBarcode);
            this.panelProductBarcode.Location = new System.Drawing.Point(74, 48);
            this.panelProductBarcode.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panelProductBarcode.Name = "panelProductBarcode";
            this.panelProductBarcode.Size = new System.Drawing.Size(440, 45);
            this.panelProductBarcode.TabIndex = 3;
            // 
            // textProductBarcode
            // 
            this.textProductBarcode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textProductBarcode.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textProductBarcode.Location = new System.Drawing.Point(5, 5);
            this.textProductBarcode.Name = "textProductBarcode";
            this.textProductBarcode.Size = new System.Drawing.Size(430, 32);
            this.textProductBarcode.TabIndex = 1;
            this.textProductBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textProductBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProductBarcode_KeyDown);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(3, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 45);
            this.label2.TabIndex = 1;
            this.label2.Text = "트레이";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelTrayBarcode
            // 
            this.panelTrayBarcode.BackColor = System.Drawing.Color.White;
            this.panelTrayBarcode.Controls.Add(this.textTrayBarcode);
            this.panelTrayBarcode.Location = new System.Drawing.Point(74, 2);
            this.panelTrayBarcode.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panelTrayBarcode.Name = "panelTrayBarcode";
            this.panelTrayBarcode.Size = new System.Drawing.Size(440, 45);
            this.panelTrayBarcode.TabIndex = 3;
            // 
            // textTrayBarcode
            // 
            this.textTrayBarcode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textTrayBarcode.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textTrayBarcode.Location = new System.Drawing.Point(5, 5);
            this.textTrayBarcode.Name = "textTrayBarcode";
            this.textTrayBarcode.Size = new System.Drawing.Size(430, 32);
            this.textTrayBarcode.TabIndex = 1;
            this.textTrayBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textTrayBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textTrayBarcode_KeyDown);
            // 
            // numericTrayCount
            // 
            this.numericTrayCount.BackColor = System.Drawing.SystemColors.Window;
            this.numericTrayCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericTrayCount.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numericTrayCount.Location = new System.Drawing.Point(5, 5);
            this.numericTrayCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericTrayCount.Name = "numericTrayCount";
            this.numericTrayCount.Size = new System.Drawing.Size(60, 35);
            this.numericTrayCount.TabIndex = 4;
            this.numericTrayCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericTrayCount.ValueChanged += new System.EventHandler(this.numericTrayCount_ValueChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.numericTrayCount);
            this.panel3.Location = new System.Drawing.Point(515, 2);
            this.panel3.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(70, 45);
            this.panel3.TabIndex = 5;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.numericProductCount);
            this.panel4.Location = new System.Drawing.Point(515, 48);
            this.panel4.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(70, 45);
            this.panel4.TabIndex = 5;
            // 
            // numericProductCount
            // 
            this.numericProductCount.BackColor = System.Drawing.SystemColors.Window;
            this.numericProductCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericProductCount.Enabled = false;
            this.numericProductCount.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numericProductCount.Location = new System.Drawing.Point(5, 5);
            this.numericProductCount.Name = "numericProductCount";
            this.numericProductCount.Size = new System.Drawing.Size(60, 35);
            this.numericProductCount.TabIndex = 4;
            this.numericProductCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkRetry
            // 
            this.checkRetry.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkRetry.BackColor = System.Drawing.SystemColors.Control;
            this.checkRetry.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gold;
            this.checkRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkRetry.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkRetry.Location = new System.Drawing.Point(587, 2);
            this.checkRetry.Margin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.checkRetry.Name = "checkRetry";
            this.checkRetry.Size = new System.Drawing.Size(70, 91);
            this.checkRetry.TabIndex = 6;
            this.checkRetry.Text = "재시도";
            this.checkRetry.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkRetry.UseVisualStyleBackColor = false;
            this.checkRetry.CheckedChanged += new System.EventHandler(this.checkRetry_CheckedChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(547, 105);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 30);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "닫기";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(441, 105);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(100, 30);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "확인";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FormBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(664, 146);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.checkRetry);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panelTrayBarcode);
            this.Controls.Add(this.panelProductBarcode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBarcode";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "바코드 스캔 [CH.1]";
            this.Load += new System.EventHandler(this.FormBarcode_Load);
            this.Shown += new System.EventHandler(this.FormBarcode_Shown);
            this.panelProductBarcode.ResumeLayout(false);
            this.panelProductBarcode.PerformLayout();
            this.panelTrayBarcode.ResumeLayout(false);
            this.panelTrayBarcode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrayCount)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericProductCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelProductBarcode;
        private System.Windows.Forms.TextBox textProductBarcode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelTrayBarcode;
        private System.Windows.Forms.TextBox textTrayBarcode;
        private System.Windows.Forms.NumericUpDown numericTrayCount;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.NumericUpDown numericProductCount;
        private System.Windows.Forms.CheckBox checkRetry;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOk;
    }
}