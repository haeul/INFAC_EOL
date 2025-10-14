using DHSTesterXL.Forms;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public partial class FormLogin : Form
    {
        public bool AdminMode { get; set; }

        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            AcceptButton = buttonOK;
        }

        private void FormLogin_Shown(object sender, EventArgs e)
        {
            // 현재 사용자가 작업자면 관리자로 변경하기 위한 것이니까 관리자를 표시
            // 현재 사용자가 관리자면 작업자로 변경하기 위한 것이니까 작업자를 표시
            if (!AdminMode)
            {
                comboUserSelect.SelectedIndex = 1; // 관리자
                textPassword.Enabled = true;
                textPassword.Focus();
            }
            else
            {
                comboUserSelect.SelectedIndex = 0; // 작업자
                textPassword.Enabled = false;
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboUserSelect.SelectedIndex != 0)
            {
                // 작업자가 아닐 경우 비밀번호 확인
                if (textPassword.Text != GSystem.SystemData.GeneralSettings.Password)
                {
                    // 비밀번호가 일치하지 않음
                    string msg = "비밀 번호가 일치하지 않습니다. 확인하시고 다시 시도해 주세요.";
                    string cap = "비밀 번호 오류";
                    MessageBox.Show(msg, cap, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textPassword.Focus();
                    return;
                }
                AdminMode = true;
            }
            else
            {
                AdminMode = false;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void LinkChangePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormChangePassword formChangePassword = new FormChangePassword();
            formChangePassword.ShowDialog(this);
        }

        private void comboUserSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboUserSelect.SelectedIndex == 0)
            {
                textPassword.Enabled = false;
            }
            else
            {
                textPassword.Enabled = true;
                textPassword.Focus();
            }
        }
    }
}
