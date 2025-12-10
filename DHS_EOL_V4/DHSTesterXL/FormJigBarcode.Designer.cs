namespace DHSTesterXL
{
    partial class FormJigBarcode
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
            this.panelProductBarcode = new System.Windows.Forms.Panel();
            this.textJigBarcode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelProductBarcode.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelProductBarcode
            // 
            this.panelProductBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelProductBarcode.BackColor = System.Drawing.Color.White;
            this.panelProductBarcode.Controls.Add(this.textJigBarcode);
            this.panelProductBarcode.Location = new System.Drawing.Point(140, 62);
            this.panelProductBarcode.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.panelProductBarcode.Name = "panelProductBarcode";
            this.panelProductBarcode.Size = new System.Drawing.Size(440, 45);
            this.panelProductBarcode.TabIndex = 5;
            // 
            // textJigBarcode
            // 
            this.textJigBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textJigBarcode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textJigBarcode.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textJigBarcode.Location = new System.Drawing.Point(5, 5);
            this.textJigBarcode.Name = "textJigBarcode";
            this.textJigBarcode.Size = new System.Drawing.Size(430, 32);
            this.textJigBarcode.TabIndex = 1;
            this.textJigBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textJigBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textJigBarcode_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(5, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 45);
            this.label1.TabIndex = 4;
            this.label1.Text = "지그 바코드";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(10, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(565, 45);
            this.label2.TabIndex = 4;
            this.label2.Text = "검사할 제품의 지그 바코드를 스캔하세요";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormJigBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(587, 114);
            this.Controls.Add(this.panelProductBarcode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormJigBarcode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "지그 바코드 스캔";
            this.panelProductBarcode.ResumeLayout(false);
            this.panelProductBarcode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelProductBarcode;
        private System.Windows.Forms.TextBox textJigBarcode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}