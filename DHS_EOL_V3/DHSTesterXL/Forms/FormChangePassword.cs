using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL.Forms
{
    public partial class FormChangePassword : Form
    {
        public FormChangePassword()
        {
            InitializeComponent();
        }

        private void FormChangePassword_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
        }

        private void FormChangePassword_Shown(object sender, EventArgs e)
        {

        }

        private void FormChangePassword_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormChangePassword_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (textCurrent.Text != GSystem.SystemData.GeneralSettings.Password)
            {
                // 현재 비밀번호가 일치하지 않음
                string message = "현재 비밀 번호가 일치하지 않습니다. 확인하시고 다시 시도해 주세요.";
                string caption = "비밀 번호 오류";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textCurrent.Text = "";
                textCurrent.Focus();
                return;
            }
            if (textNew.Text != textConfirm.Text)
            {
                // 신규 비밀번호가 일치하지 않음
                string message = "신규 비밀 번호와 비밀 번호 확인이 일치하지 않습니다. 확인하시고 다시 시도해 주세요.";
                string caption = "비밀 번호 오류";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textConfirm.Text = "";
                textConfirm.Focus();
                return;
            }
            if (textCurrent.Text == textNew.Text)
            {
                // 신규 비밀번호가 현재 비밀번호와 동일함
                string message = "신규 비밀 번호가 현재 비밀 번호와 동일합니다. 다른 비밀번호를 입력해 주십시오.";
                string caption = "비밀 번호 오류";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textNew.Text = "";
                textConfirm.Text = "";
                textNew.Focus();
                return;
            }
            GSystem.SystemData.GeneralSettings.Password = textNew.Text;
            GSystem.SystemDataSave();
            // 비밀번호 변경 완료
            GSystem.Logger.Info("비밀번호 변경 완료");

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
