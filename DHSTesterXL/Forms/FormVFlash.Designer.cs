/*
namespace DHSTesterXL.Forms
{
    partial class FormVFlash
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
            this.textFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelPercent = new System.Windows.Forms.Label();
            this.labelRemainTime = new System.Windows.Forms.Label();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonFlash = new System.Windows.Forms.Button();
            this.checkAutoClose = new System.Windows.Forms.CheckBox();
            this.labelElapsedTime = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.labelMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textFileName
            // 
            this.textFileName.Location = new System.Drawing.Point(89, 27);
            this.textFileName.Name = "textFileName";
            this.textFileName.Size = new System.Drawing.Size(500, 23);
            this.textFileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "vFlash file:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(89, 83);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(500, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // labelPercent
            // 
            this.labelPercent.Location = new System.Drawing.Point(308, 109);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(63, 15);
            this.labelPercent.TabIndex = 2;
            this.labelPercent.Text = "0 %";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRemainTime
            // 
            this.labelRemainTime.Location = new System.Drawing.Point(86, 109);
            this.labelRemainTime.Name = "labelRemainTime";
            this.labelRemainTime.Size = new System.Drawing.Size(63, 15);
            this.labelRemainTime.TabIndex = 2;
            this.labelRemainTime.Text = "0.0 sec";
            this.labelRemainTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(600, 24);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(100, 30);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.Text = "Open...";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.ButtonOpen_Click);
            // 
            // buttonFlash
            // 
            this.buttonFlash.Location = new System.Drawing.Point(600, 80);
            this.buttonFlash.Name = "buttonFlash";
            this.buttonFlash.Size = new System.Drawing.Size(100, 30);
            this.buttonFlash.TabIndex = 2;
            this.buttonFlash.Text = "Flash";
            this.buttonFlash.UseVisualStyleBackColor = true;
            this.buttonFlash.Click += new System.EventHandler(this.ButtonFlash_Click);
            // 
            // checkAutoClose
            // 
            this.checkAutoClose.AutoSize = true;
            this.checkAutoClose.Location = new System.Drawing.Point(605, 130);
            this.checkAutoClose.Name = "checkAutoClose";
            this.checkAutoClose.Size = new System.Drawing.Size(78, 19);
            this.checkAutoClose.TabIndex = 4;
            this.checkAutoClose.Text = "자동 종료";
            this.checkAutoClose.UseVisualStyleBackColor = true;
            // 
            // labelElapsedTime
            // 
            this.labelElapsedTime.Location = new System.Drawing.Point(526, 109);
            this.labelElapsedTime.Name = "labelElapsedTime";
            this.labelElapsedTime.Size = new System.Drawing.Size(63, 15);
            this.labelElapsedTime.TabIndex = 2;
            this.labelElapsedTime.Text = "0.0 sec";
            this.labelElapsedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // labelMessage
            // 
            this.labelMessage.Location = new System.Drawing.Point(86, 65);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(500, 15);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "Ready";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormVFlash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(734, 161);
            this.Controls.Add(this.checkAutoClose);
            this.Controls.Add(this.buttonFlash);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelElapsedTime);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelRemainTime);
            this.Controls.Add(this.labelPercent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textFileName);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormVFlash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VFlash ROM Writing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVFlash_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormVFlash_FormClosed);
            this.Load += new System.EventHandler(this.FormVFlash_Load);
            this.Shown += new System.EventHandler(this.FormVFlash_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.Label labelRemainTime;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonFlash;
        private System.Windows.Forms.CheckBox checkAutoClose;
        private System.Windows.Forms.Label labelElapsedTime;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label labelMessage;
    }
}
*/