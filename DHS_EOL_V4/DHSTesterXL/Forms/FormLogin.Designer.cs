namespace DHSTesterXL
{
    partial class FormLogin
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
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.comboUserSelect = new System.Windows.Forms.ComboBox();
            this.textPassword = new MetroFramework.Controls.MetroTextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.linkChangePassword = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureIcon
            // 
            this.pictureIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureIcon.Image = global::DHSTesterXL.Properties.Resources.lock_close_03_128x128;
            this.pictureIcon.Location = new System.Drawing.Point(12, 12);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(130, 130);
            this.pictureIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureIcon.TabIndex = 0;
            this.pictureIcon.TabStop = false;
            // 
            // comboUserSelect
            // 
            this.comboUserSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUserSelect.FormattingEnabled = true;
            this.comboUserSelect.Items.AddRange(new object[] {
            "작업자",
            "관리자"});
            this.comboUserSelect.Location = new System.Drawing.Point(150, 12);
            this.comboUserSelect.Name = "comboUserSelect";
            this.comboUserSelect.Size = new System.Drawing.Size(200, 23);
            this.comboUserSelect.TabIndex = 0;
            this.comboUserSelect.SelectionChangeCommitted += new System.EventHandler(this.comboUserSelect_SelectionChangeCommitted);
            // 
            // textPassword
            // 
            // 
            // 
            // 
            this.textPassword.CustomButton.Image = null;
            this.textPassword.CustomButton.Location = new System.Drawing.Point(178, 1);
            this.textPassword.CustomButton.Name = "";
            this.textPassword.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.textPassword.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textPassword.CustomButton.TabIndex = 1;
            this.textPassword.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textPassword.CustomButton.UseSelectable = true;
            this.textPassword.CustomButton.Visible = false;
            this.textPassword.Lines = new string[0];
            this.textPassword.Location = new System.Drawing.Point(150, 41);
            this.textPassword.MaxLength = 32767;
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '●';
            this.textPassword.PromptText = "Enter password";
            this.textPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textPassword.SelectedText = "";
            this.textPassword.SelectionLength = 0;
            this.textPassword.SelectionStart = 0;
            this.textPassword.ShortcutsEnabled = true;
            this.textPassword.Size = new System.Drawing.Size(200, 23);
            this.textPassword.TabIndex = 1;
            this.textPassword.UseSelectable = true;
            this.textPassword.WaterMark = "Enter password";
            this.textPassword.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textPassword.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(150, 112);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 30);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "확인";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(256, 112);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // linkChangePassword
            // 
            this.linkChangePassword.AutoSize = true;
            this.linkChangePassword.Location = new System.Drawing.Point(253, 77);
            this.linkChangePassword.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            this.linkChangePassword.Name = "linkChangePassword";
            this.linkChangePassword.Size = new System.Drawing.Size(92, 15);
            this.linkChangePassword.TabIndex = 4;
            this.linkChangePassword.TabStop = true;
            this.linkChangePassword.Text = "비밀번호 변경...";
            this.linkChangePassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkChangePassword_LinkClicked);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(364, 151);
            this.Controls.Add(this.linkChangePassword);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.comboUserSelect);
            this.Controls.Add(this.pictureIcon);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLogin_FormClosed);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.Shown += new System.EventHandler(this.FormLogin_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureIcon;
        private System.Windows.Forms.ComboBox comboUserSelect;
        private MetroFramework.Controls.MetroTextBox textPassword;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.LinkLabel linkChangePassword;
    }
}