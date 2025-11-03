namespace DHSTesterXL.Forms
{
    partial class FormChangePassword
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textCurrent = new MetroFramework.Controls.MetroTextBox();
            this.textNew = new MetroFramework.Controls.MetroTextBox();
            this.textConfirm = new MetroFramework.Controls.MetroTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::DHSTesterXL.Properties.Resources.key_01_128x128;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(130, 130);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "현재 비밀번호";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "신규 비밀번호";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "비밀번호 확인";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(169, 112);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(100, 30);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "확인";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(272, 112);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // textCurrent
            // 
            // 
            // 
            // 
            this.textCurrent.CustomButton.Image = null;
            this.textCurrent.CustomButton.Location = new System.Drawing.Point(115, 1);
            this.textCurrent.CustomButton.Name = "";
            this.textCurrent.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.textCurrent.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textCurrent.CustomButton.TabIndex = 1;
            this.textCurrent.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textCurrent.CustomButton.UseSelectable = true;
            this.textCurrent.CustomButton.Visible = false;
            this.textCurrent.Lines = new string[0];
            this.textCurrent.Location = new System.Drawing.Point(235, 17);
            this.textCurrent.MaxLength = 32767;
            this.textCurrent.Name = "textCurrent";
            this.textCurrent.PasswordChar = '●';
            this.textCurrent.PromptText = "Enter current password";
            this.textCurrent.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textCurrent.SelectedText = "";
            this.textCurrent.SelectionLength = 0;
            this.textCurrent.SelectionStart = 0;
            this.textCurrent.ShortcutsEnabled = true;
            this.textCurrent.Size = new System.Drawing.Size(137, 23);
            this.textCurrent.TabIndex = 0;
            this.textCurrent.UseSelectable = true;
            this.textCurrent.WaterMark = "Enter current password";
            this.textCurrent.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textCurrent.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // textNew
            // 
            // 
            // 
            // 
            this.textNew.CustomButton.Image = null;
            this.textNew.CustomButton.Location = new System.Drawing.Point(115, 1);
            this.textNew.CustomButton.Name = "";
            this.textNew.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.textNew.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textNew.CustomButton.TabIndex = 1;
            this.textNew.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textNew.CustomButton.UseSelectable = true;
            this.textNew.CustomButton.Visible = false;
            this.textNew.Lines = new string[0];
            this.textNew.Location = new System.Drawing.Point(235, 46);
            this.textNew.MaxLength = 32767;
            this.textNew.Name = "textNew";
            this.textNew.PasswordChar = '●';
            this.textNew.PromptText = "Enter new password";
            this.textNew.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textNew.SelectedText = "";
            this.textNew.SelectionLength = 0;
            this.textNew.SelectionStart = 0;
            this.textNew.ShortcutsEnabled = true;
            this.textNew.Size = new System.Drawing.Size(137, 23);
            this.textNew.TabIndex = 1;
            this.textNew.UseSelectable = true;
            this.textNew.WaterMark = "Enter new password";
            this.textNew.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textNew.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // textConfirm
            // 
            // 
            // 
            // 
            this.textConfirm.CustomButton.Image = null;
            this.textConfirm.CustomButton.Location = new System.Drawing.Point(115, 1);
            this.textConfirm.CustomButton.Name = "";
            this.textConfirm.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.textConfirm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textConfirm.CustomButton.TabIndex = 1;
            this.textConfirm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textConfirm.CustomButton.UseSelectable = true;
            this.textConfirm.CustomButton.Visible = false;
            this.textConfirm.Lines = new string[0];
            this.textConfirm.Location = new System.Drawing.Point(235, 75);
            this.textConfirm.MaxLength = 32767;
            this.textConfirm.Name = "textConfirm";
            this.textConfirm.PasswordChar = '●';
            this.textConfirm.PromptText = "Confirm new password";
            this.textConfirm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textConfirm.SelectedText = "";
            this.textConfirm.SelectionLength = 0;
            this.textConfirm.SelectionStart = 0;
            this.textConfirm.ShortcutsEnabled = true;
            this.textConfirm.Size = new System.Drawing.Size(137, 23);
            this.textConfirm.TabIndex = 2;
            this.textConfirm.UseSelectable = true;
            this.textConfirm.WaterMark = "Confirm new password";
            this.textConfirm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textConfirm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // FormChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.textConfirm);
            this.Controls.Add(this.textNew);
            this.Controls.Add(this.textCurrent);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChangePassword";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Password";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChangePassword_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormChangePassword_FormClosed);
            this.Load += new System.EventHandler(this.FormChangePassword_Load);
            this.Shown += new System.EventHandler(this.FormChangePassword_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private MetroFramework.Controls.MetroTextBox textCurrent;
        private MetroFramework.Controls.MetroTextBox textNew;
        private MetroFramework.Controls.MetroTextBox textConfirm;
    }
}