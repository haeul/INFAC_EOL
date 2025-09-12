using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL.Forms
{
    public partial class FormSelectProduct : Form
    {
        public string SelectedProductNo { get; set; }

        public FormSelectProduct()
        {
            InitializeComponent();
        }

        private void FormSelectProduct_Load(object sender, EventArgs e)
        {

        }

        private void FormSelectProduct_Shown(object sender, EventArgs e)
        {
            SetupComboBox();
        }

        private void FormSelectProduct_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormSelectProduct_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void SetupComboBox()
        {
            List<string> productNoList = GSystem.ProductSettings.GetProductList(GSystem.SystemData.GeneralSettings.ProductFolder);
            comboProductNo.Items.Clear();
            foreach (string productNo in productNoList)
            {
                comboProductNo.Items.Add($"{productNo}");
            }
            comboProductNo.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SelectedProductNo = comboProductNo.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedProductNo = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
