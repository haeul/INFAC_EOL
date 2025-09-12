namespace DHSTesterXL.Forms
{
    partial class FormProductNew
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
            this.labelProductNoNew = new System.Windows.Forms.Label();
            this.labelProductNoRef = new System.Windows.Forms.Label();
            this.textProductNo = new System.Windows.Forms.TextBox();
            this.comboProductNo = new System.Windows.Forms.ComboBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelProductNoNew
            // 
            this.labelProductNoNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProductNoNew.Location = new System.Drawing.Point(14, 23);
            this.labelProductNoNew.Name = "labelProductNoNew";
            this.labelProductNoNew.Size = new System.Drawing.Size(80, 15);
            this.labelProductNoNew.TabIndex = 0;
            this.labelProductNoNew.Text = "새 품번";
            this.labelProductNoNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelProductNoRef
            // 
            this.labelProductNoRef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProductNoRef.Location = new System.Drawing.Point(14, 63);
            this.labelProductNoRef.Name = "labelProductNoRef";
            this.labelProductNoRef.Size = new System.Drawing.Size(80, 15);
            this.labelProductNoRef.TabIndex = 0;
            this.labelProductNoRef.Text = "참조 품번";
            this.labelProductNoRef.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textProductNo
            // 
            this.textProductNo.Location = new System.Drawing.Point(100, 20);
            this.textProductNo.Name = "textProductNo";
            this.textProductNo.Size = new System.Drawing.Size(200, 23);
            this.textProductNo.TabIndex = 1;
            // 
            // comboProductNo
            // 
            this.comboProductNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboProductNo.FormattingEnabled = true;
            this.comboProductNo.Location = new System.Drawing.Point(100, 60);
            this.comboProductNo.Name = "comboProductNo";
            this.comboProductNo.Size = new System.Drawing.Size(200, 23);
            this.comboProductNo.TabIndex = 2;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(102, 90);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(187, 15);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "(참조 품번을 신규 품번으로 복사)";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(72, 120);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 30);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "확인";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(178, 120);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormProductNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(334, 161);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboProductNo);
            this.Controls.Add(this.textProductNo);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelProductNoRef);
            this.Controls.Add(this.labelProductNoNew);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProductNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "품번 생성";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProductNew_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormProductNew_FormClosed);
            this.Load += new System.EventHandler(this.FormProductNew_Load);
            this.Shown += new System.EventHandler(this.FormProductNew_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProductNoNew;
        private System.Windows.Forms.Label labelProductNoRef;
        private System.Windows.Forms.TextBox textProductNo;
        private System.Windows.Forms.ComboBox comboProductNo;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}