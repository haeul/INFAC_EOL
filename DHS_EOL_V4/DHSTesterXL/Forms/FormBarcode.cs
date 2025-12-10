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
        public event EventHandler BarcodeDataChanged;

        public int Channel { get; set; }
        public string TrayBarcode { get; set; }
        public string ProductBarcode { get; set; }

        public FormBarcode()
        {
            InitializeComponent();
        }

        private void FormBarcode_Load(object sender, EventArgs e)
        {

        }

        private void FormBarcode_Shown(object sender, EventArgs e)
        {
            this.Text = $"바코드 스캔 [CH.{Channel + 1}]";
            textTrayBarcode.Text = TrayBarcode;
            textProductBarcode.Text = string.Empty;
            numericTrayCount.Value = GSystem.TrayInterlockCount;
            numericProductCount.Value = GSystem.ProductInterlockCount;

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
                textTrayBarcode.Text = textTrayBarcode.Text.ToUpper();
                textProductBarcode.Text = "";
                textProductBarcode.Focus();
            }
        }

        private void textProductBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textProductBarcode.Text = textProductBarcode.Text.ToUpper();
                if (GSystem.ProductSettings.ProductInfo.UseTrayInterlock)
                {
                    //int productCount = (int)numericProductCount.Value;
                    if (GSystem.ProductInterlockCount < GSystem.TrayInterlockCount)
                    {
                        // 입력 바코드가 마스터샘플인 경우 카운트 하지 않는다
                        if (textProductBarcode.Text != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode1 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode2 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode3 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode4 &&
                            textProductBarcode.Text != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode5)
                        {
                            if (!checkRetry.Checked)
                                GSystem.ProductInterlockCount++;
                            numericProductCount.Value = GSystem.ProductInterlockCount;
                        }
                        TrayBarcode = textTrayBarcode.Text;
                        ProductBarcode = textProductBarcode.Text;
                        OnBarcodeDataChanged(Channel, TrayBarcode, ProductBarcode);
                        //DialogResult = DialogResult.OK;
                        //Close();
                        Hide();
                    }
                    else
                    {
                        //GSystem.MiPLC.SetErrorTowerLamp(Channel, true);
                        string message = $"트레이 당 제품 바코드 인식 수량을 초과하였습니다. 트레이 바코드를 다시 인식하시기 바랍니다.";
                        string caption = $"트레이 바코드 인터락 에러";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textProductBarcode.Text = string.Empty;
                        textTrayBarcode.Focus();
                        //GSystem.MiPLC.SetErrorTowerLamp(Channel, false);
                    }
                }
                else
                {
                    TrayBarcode = textTrayBarcode.Text;
                    ProductBarcode = textProductBarcode.Text;
                    OnBarcodeDataChanged(Channel, TrayBarcode, ProductBarcode);
                    //DialogResult = DialogResult.OK;
                    //Close();
                    Hide();
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
        private void OnBarcodeDataChanged(int channel, string trayBarcode, string productBarcode)
        {
            BarcodeEventArgs barcodeEventArgs = new BarcodeEventArgs
            {
                Channel = channel,
                TrayBarcode = trayBarcode,
                ProductBarcode = productBarcode
            };
            BarcodeDataChanged?.Invoke(this, barcodeEventArgs);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            TrayBarcode = textTrayBarcode.Text;
            ProductBarcode = textProductBarcode.Text;
            OnBarcodeDataChanged(Channel, TrayBarcode, ProductBarcode);
            Hide();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public void ResetProductBarcodeData()
        {
            textProductBarcode.Text = string.Empty;
            textProductBarcode.Focus();
        }

        private void FormBarcode_Activated(object sender, EventArgs e)
        {
            TrayBarcode = GSystem.TrayBarcode;
            textTrayBarcode.Text = GSystem.TrayBarcode;
            if (string.IsNullOrEmpty(textTrayBarcode.Text))
                textTrayBarcode.Focus();
            else
                textProductBarcode.Focus();
            numericTrayCount.Value = GSystem.TrayInterlockCount;
            numericProductCount.Value = GSystem.ProductInterlockCount;
        }
    }
}
