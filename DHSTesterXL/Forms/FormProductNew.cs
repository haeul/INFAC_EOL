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
    public partial class FormProductNew : Form
    {
        private readonly ProductConfig _productSettings = new ProductConfig();

        public bool CopyMode { get; set; }
        public string CurrentProductNo { get; set; }
        public string NewProductNo { get; set; }

        public FormProductNew()
        {
            InitializeComponent();
            _productSettings = GSystem.ProductSettings;
        }

        private void FormProductNew_Load(object sender, EventArgs e)
        {
            if (CopyMode)
            {
                this.Text = "품번 복사";
                Point curLabelProductNoNew = labelProductNoNew.Location;
                Point curLabelProductNoRef = labelProductNoRef.Location;
                Point curTextProductNo = textProductNo.Location;
                Point curComboProductNo = comboProductNo.Location;

                labelProductNoNew.Location = curLabelProductNoRef;
                labelProductNoRef.Location = curLabelProductNoNew;
                textProductNo.Location = curComboProductNo;
                comboProductNo.Location = curTextProductNo;
                labelMessage.Text = "";
            }
        }

        private void FormProductNew_Shown(object sender, EventArgs e)
        {
            buttonCancel.Focus();
            SetupComboBox();
            textProductNo.Focus();
        }

        private void FormProductNew_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormProductNew_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void SetupComboBox()
        {
            List<string> productNoList = _productSettings.GetProductList(GSystem.SystemData.GeneralSettings.ProductFolder);
            comboProductNo.Items.Clear();
            if (!CopyMode)
                comboProductNo.Items.Add("선택");
            foreach (string productNo in productNoList)
            {
                comboProductNo.Items.Add($"{productNo}");
            }
            if (CopyMode)
                comboProductNo.SelectedItem = CurrentProductNo;
            else
                comboProductNo.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string productNo = textProductNo.Text;
            string productFileName = textProductNo.Text + GSystem.JSON_EXT;
            string selectedNo = comboProductNo.SelectedItem.ToString();
            string selectedFileName = comboProductNo.SelectedItem.ToString() + GSystem.JSON_EXT;

            if (CopyMode)
            {
                // 파일 복사
                string caption = $"파일 복사";
                string message = $"품번 [{selectedNo}]를 [{productNo}]로 복사하시겠습니까?";
                if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                try
                {
                    string productPath = GSystem.SystemData.GeneralSettings.ProductFolder;
                    string sourceFilePath = Path.Combine(productPath, selectedFileName);
                    string targetFilePath = Path.Combine(productPath, productFileName);

                    File.Copy(sourceFilePath, targetFilePath, true);

                    // 신규 품번 파일의 PartNo를 신규 품번으로 수정한다.
                    ProductConfig productSettings = new ProductConfig();
                    productSettings.Load(productFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                    productSettings.ProductInfo.PartNo = productNo;
                    productSettings.Save(productFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (comboProductNo.SelectedIndex > 0)
                {
                    // 새 파일 (복사)
                    string caption = $"파일 복사";
                    string message = $"신규 품번 [{productNo}]를 생성하시겠습니까?\n(기본 품번 [{selectedNo}]를 복사)";
                    if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    try
                    {
                        string productPath = GSystem.SystemData.GeneralSettings.ProductFolder;
                        string sourceFilePath = Path.Combine(productPath, selectedFileName);
                        string targetFilePath = Path.Combine(productPath, productFileName);

                        File.Copy(sourceFilePath, targetFilePath, true);

                        // 신규 품번 파일의 PartNo를 신규 품번으로 수정한다.
                        ProductConfig productSettings = new ProductConfig();
                        productSettings.Load(productFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                        productSettings.ProductInfo.PartNo = productNo;
                        productSettings.Save(productFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 새 파일 (생성)
                    string caption = $"파일 생성";
                    string message = $"신규 품번 [{productNo}]를 생성하시겠습니까?";
                    if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    try
                    {
                        // 신규 파일 생성
                        GSystem.ProductSettings.Load(productFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            NewProductNo = productNo;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
