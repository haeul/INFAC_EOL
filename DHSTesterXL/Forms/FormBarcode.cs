using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace DHSTesterXL
{
    public partial class FormBarcode : Form
    {
        public int Channel { get; set; }
        //public int TrayInterlockCount { get; set; } = 10;
        //public int ProductCount { get; set; }
        public string TrayBarcode { get; set; }
        public string ProductBarcode { get; set; }

        public FormBarcode()
        {
            InitializeComponent();
        }

        private void FormBarcode_Load(object sender, EventArgs e)
        {
            this.Text = $"바코드 스캔 [CH.{Channel + 1}]";
            textTrayBarcode.Text = TrayBarcode;
            textProductBarcode.Text = string.Empty;
            numericTrayCount.Value = GSystem.TrayInterlockCount;
            numericProductCount.Value = GSystem.ProductInterlockCount;
        }

        private void FormBarcode_Shown(object sender, EventArgs e)
        {
            if (textTrayBarcode.Text == string.Empty)
                textTrayBarcode.Focus();
            else
                textProductBarcode.Focus();
        }

        private void textTrayBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GSystem.ProductInterlockCount = 0;
                numericProductCount.Value = 0;
                textProductBarcode.Text = "";
                textProductBarcode.Focus();
            }
        }

        private void textProductBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (GSystem.ProductSettings.ProductInfo.UseTrayInterlock)
                {
                    //int productCount = (int)numericProductCount.Value;
                    if (GSystem.ProductInterlockCount < GSystem.TrayInterlockCount)
                    {
                        // 입력 바코드가 마스터샘플인 경우 카운트 하지 않는다
                        if (textProductBarcode.Text != GSystem.ProductSettings.MasterSample.MasterBarcode1 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSample.MasterBarcode2 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSample.MasterBarcode3 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSample.MasterBarcode4 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSample.MasterBarcode5)
                        {
                            if (!checkRetry.Checked)
                                GSystem.ProductInterlockCount++;
                            numericProductCount.Value = GSystem.ProductInterlockCount;
                        }
                        TrayBarcode = textTrayBarcode.Text;
                        ProductBarcode = textProductBarcode.Text;
                        DialogResult = DialogResult.OK;
                        Close();
                        //Hide();
                    }
                    else
                    {
                        string message = $"트레이 당 제품 바코드 인식 수량을 초과하였습니다. 트레이 바코드를 다시 인식하시기 바랍니다.";
                        string caption = $"트레이 바코드 인터락 에러";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textTrayBarcode.Focus();
                    }
                }
                else
                {
                    TrayBarcode = textTrayBarcode.Text;
                    ProductBarcode = textProductBarcode.Text;
                    DialogResult = DialogResult.OK;
                    Close();
                    //Hide();
                }
            }
        }

        private void numericTrayCount_ValueChanged(object sender, EventArgs e)
        {
            GSystem.TrayInterlockCount = (int)numericTrayCount.Value;
        }

        private void checkRetry_CheckedChanged(object sender, EventArgs e)
        {
            if (checkRetry.Checked)
                panelTrayBarcode.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            else
                panelTrayBarcode.BackColor = System.Drawing.Color.White;
            textTrayBarcode.Enabled = !checkRetry.Checked;
            numericTrayCount.Enabled = !checkRetry.Checked;
            textProductBarcode.Focus();
        }
    }
}
