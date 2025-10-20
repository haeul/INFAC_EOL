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
    public partial class FormMasterTest : Form
    {
        private bool PrevMasterTestStatus { get; set; } = false;

        public FormMasterTest()
        {
            InitializeComponent();
        }

        private void FormMasterTest_Load(object sender, EventArgs e)
        {
            SetupMasterTestStatus();
        }

        private void FormMasterTest_Shown(object sender, EventArgs e)
        {
            PrevMasterTestStatus = false;
            timerUpdate.Start();
        }

        private void FormMasterTest_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormMasterTest_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void SetupMasterTestStatus()
        {
            labelMasterType1.Text = GSystem.ProductSettings.MasterSampleCh1.MasterType1;
            labelMasterType2.Text = GSystem.ProductSettings.MasterSampleCh1.MasterType2;
            labelMasterType3.Text = GSystem.ProductSettings.MasterSampleCh1.MasterType3;
            labelMasterType4.Text = GSystem.ProductSettings.MasterSampleCh1.MasterType4;
            labelMasterType5.Text = GSystem.ProductSettings.MasterSampleCh1.MasterType5;
            labelMasterBarcode1.Text = GSystem.ProductSettings.MasterSampleCh1.MasterBarcode1;
            labelMasterBarcode2.Text = GSystem.ProductSettings.MasterSampleCh1.MasterBarcode2;
            labelMasterBarcode3.Text = GSystem.ProductSettings.MasterSampleCh1.MasterBarcode3;
            labelMasterBarcode4.Text = GSystem.ProductSettings.MasterSampleCh1.MasterBarcode4;
            labelMasterBarcode5.Text = GSystem.ProductSettings.MasterSampleCh1.MasterBarcode5;
            labelMasterOk1Ch1.Text = (labelMasterType1.Text != "" && labelMasterBarcode1.Text != "") ? "완료" : "";
            labelMasterOk2Ch1.Text = (labelMasterType2.Text != "" && labelMasterBarcode2.Text != "") ? "완료" : "";
            labelMasterOk3Ch1.Text = (labelMasterType3.Text != "" && labelMasterBarcode3.Text != "") ? "완료" : "";
            labelMasterOk4Ch1.Text = (labelMasterType4.Text != "" && labelMasterBarcode4.Text != "") ? "완료" : "";
            labelMasterOk5Ch1.Text = (labelMasterType5.Text != "" && labelMasterBarcode5.Text != "") ? "완료" : "";
            labelMasterOk1Ch2.Text = (labelMasterType1.Text != "" && labelMasterBarcode1.Text != "") ? "완료" : "";
            labelMasterOk2Ch2.Text = (labelMasterType2.Text != "" && labelMasterBarcode2.Text != "") ? "완료" : "";
            labelMasterOk3Ch2.Text = (labelMasterType3.Text != "" && labelMasterBarcode3.Text != "") ? "완료" : "";
            labelMasterOk4Ch2.Text = (labelMasterType4.Text != "" && labelMasterBarcode4.Text != "") ? "완료" : "";
            labelMasterOk5Ch2.Text = (labelMasterType5.Text != "" && labelMasterBarcode5.Text != "") ? "완료" : "";
        }

        private void UpdateMasterTestStatus()
        {
            if (labelMasterType1.Text != "" && labelMasterBarcode1.Text != "")
            {
                // Ch1
                if (GSystem.MasterTestOkCh1[0])
                {
                    if (labelMasterOk1Ch1.BackColor != Color.Lime)
                        labelMasterOk1Ch1.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk1Ch1.BackColor != SystemColors.Control)
                        labelMasterOk1Ch1.BackColor = SystemColors.Control;
                }
                // Ch2
                if (GSystem.MasterTestOkCh2[0])
                {
                    if (labelMasterOk1Ch2.BackColor != Color.Lime)
                        labelMasterOk1Ch2.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk1Ch2.BackColor != SystemColors.Control)
                        labelMasterOk1Ch2.BackColor = SystemColors.Control;
                }
            }
            else
            {
                if (labelMasterOk1Ch1.BackColor != SystemColors.Control)
                    labelMasterOk1Ch1.BackColor = SystemColors.Control;
                if (labelMasterOk1Ch2.BackColor != SystemColors.Control)
                    labelMasterOk1Ch2.BackColor = SystemColors.Control;
            }

            if (labelMasterType2.Text != "" && labelMasterBarcode2.Text != "")
            {
                // Ch1
                if (GSystem.MasterTestOkCh1[1])
                {
                    if (labelMasterOk2Ch1.BackColor != Color.Lime)
                        labelMasterOk2Ch1.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk2Ch1.BackColor != SystemColors.Control)
                        labelMasterOk2Ch1.BackColor = SystemColors.Control;
                }
                // Ch2
                if (GSystem.MasterTestOkCh2[1])
                {
                    if (labelMasterOk2Ch2.BackColor != Color.Lime)
                        labelMasterOk2Ch2.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk2Ch2.BackColor != SystemColors.Control)
                        labelMasterOk2Ch2.BackColor = SystemColors.Control;
                }
            }
            else
            {
                if (labelMasterOk2Ch1.BackColor != SystemColors.Control)
                    labelMasterOk2Ch1.BackColor = SystemColors.Control;
                if (labelMasterOk2Ch2.BackColor != SystemColors.Control)
                    labelMasterOk2Ch2.BackColor = SystemColors.Control;
            }

            if (labelMasterType3.Text != "" && labelMasterBarcode3.Text != "")
            {
                // Ch1
                if (GSystem.MasterTestOkCh1[2])
                {
                    if (labelMasterOk3Ch1.BackColor != Color.Lime)
                        labelMasterOk3Ch1.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk3Ch1.BackColor != SystemColors.Control)
                        labelMasterOk3Ch1.BackColor = SystemColors.Control;
                }
                // Ch2
                if (GSystem.MasterTestOkCh2[2])
                {
                    if (labelMasterOk3Ch2.BackColor != Color.Lime)
                        labelMasterOk3Ch2.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk3Ch2.BackColor != SystemColors.Control)
                        labelMasterOk3Ch2.BackColor = SystemColors.Control;
                }
            }
            else
            {
                if (labelMasterOk3Ch1.BackColor != SystemColors.Control)
                    labelMasterOk3Ch1.BackColor = SystemColors.Control;
                if (labelMasterOk3Ch2.BackColor != SystemColors.Control)
                    labelMasterOk3Ch2.BackColor = SystemColors.Control;
            }

            if (labelMasterType4.Text != "" && labelMasterBarcode4.Text != "")
            {
                // Ch1
                if (GSystem.MasterTestOkCh1[3])
                {
                    if (labelMasterOk4Ch1.BackColor != Color.Lime)
                        labelMasterOk4Ch1.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk4Ch1.BackColor != SystemColors.Control)
                        labelMasterOk4Ch1.BackColor = SystemColors.Control;
                }
                // Ch2
                if (GSystem.MasterTestOkCh2[3])
                {
                    if (labelMasterOk4Ch2.BackColor != Color.Lime)
                        labelMasterOk4Ch2.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk4Ch2.BackColor != SystemColors.Control)
                        labelMasterOk4Ch2.BackColor = SystemColors.Control;
                }
            }
            else
            {
                if (labelMasterOk4Ch1.BackColor != SystemColors.Control)
                    labelMasterOk4Ch1.BackColor = SystemColors.Control;
                if (labelMasterOk4Ch2.BackColor != SystemColors.Control)
                    labelMasterOk4Ch2.BackColor = SystemColors.Control;
            }

            if (labelMasterType5.Text != "" && labelMasterBarcode5.Text != "")
            {
                // Ch1
                if (GSystem.MasterTestOkCh1[4])
                {
                    if (labelMasterOk5Ch1.BackColor != Color.Lime)
                        labelMasterOk5Ch1.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk5Ch1.BackColor != SystemColors.Control)
                        labelMasterOk5Ch1.BackColor = SystemColors.Control;
                }
                // Ch2
                if (GSystem.MasterTestOkCh2[4])
                {
                    if (labelMasterOk5Ch2.BackColor != Color.Lime)
                        labelMasterOk5Ch2.BackColor = Color.Lime;
                }
                else
                {
                    if (labelMasterOk5Ch2.BackColor != SystemColors.Control)
                        labelMasterOk5Ch2.BackColor = SystemColors.Control;
                }
            }
            else
            {
                if (labelMasterOk5Ch1.BackColor != SystemColors.Control)
                    labelMasterOk5Ch1.BackColor = SystemColors.Control;
                if (labelMasterOk5Ch2.BackColor != SystemColors.Control)
                    labelMasterOk5Ch2.BackColor = SystemColors.Control;
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateMasterTestStatus();
            bool masterOkCh1 = CheckMasterSampleTest(GSystem.CH1);
            bool masterOkCh2 = CheckMasterSampleTest(GSystem.CH2);
            if (masterOkCh1 && masterOkCh2)
            {
                if (!PrevMasterTestStatus)
                {
                    PrevMasterTestStatus = true;
                    timerUpdate.Stop();
                    this.Hide();
                }
            }
        }

        private bool CheckMasterSampleTest(int channel)
        {
            // 마스터샘플 테스트 진행 여부에 따라 메시지 표시
            if (GSystem.ProductSettings.ProductInfo.UseMasterSample)
            {
                if (GSystem.ProductSettings.MasterSampleCh1.MasterCount > 0)
                {
                    bool masterSampleOK = false;

                    if (channel == GSystem.CH1)
                    {
                        int masterCount = 0;
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh1.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh1[0];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh1.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh1[1];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh1.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh1[2];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh1.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh1[3];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh1.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh1[4];
                            }
                        }
                    }
                    else
                    {
                        int masterCount = 0;
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh2.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType1 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh2[0];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh2.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType2 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh2[1];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh2.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType3 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh2[2];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh2.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType4 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh2[3];
                            }
                        }
                        if (masterCount < GSystem.ProductSettings.MasterSampleCh2.MasterCount)
                        {
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType5 != "")
                            {
                                masterCount++;
                                masterSampleOK = GSystem.MasterTestOkCh2[4];
                            }
                        }
                    }
                    return masterSampleOK;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private void labelMasterOkCh1_DoubleClick(object sender, EventArgs e)
        {
            if (GSystem.AdminMode)
            {
                if (sender is Label label)
                {
                    int index = Convert.ToInt16(label.Tag.ToString());
                    switch (index)
                    {
                        case 0:
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 != "")
                                GSystem.MasterTestOkCh1[index] = true;
                            break;
                        case 1:
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 != "")
                                GSystem.MasterTestOkCh1[index] = true;
                            break;
                        case 2:
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 != "")
                                GSystem.MasterTestOkCh1[index] = true;
                            break;
                        case 3:
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 != "")
                                GSystem.MasterTestOkCh1[index] = true;
                            break;
                        case 4:
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 != "")
                                GSystem.MasterTestOkCh1[index] = true;
                            break;
                    }
                }
            }
        }

        private void labelMasterOkCh2_DoubleClick(object sender, EventArgs e)
        {
            if (GSystem.AdminMode)
            {
                if (sender is Label label)
                {
                    int index = Convert.ToInt16(label.Tag.ToString());
                    switch (index)
                    {
                        case 0:
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType1 != "")
                                GSystem.MasterTestOkCh2[index] = true;
                            break;
                        case 1:
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType2 != "")
                                GSystem.MasterTestOkCh2[index] = true;
                            break;
                        case 2:
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType3 != "")
                                GSystem.MasterTestOkCh2[index] = true;
                            break;
                        case 3:
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType4 != "")
                                GSystem.MasterTestOkCh2[index] = true;
                            break;
                        case 4:
                            if (GSystem.ProductSettings.MasterSampleCh2.MasterType5 != "")
                                GSystem.MasterTestOkCh2[index] = true;
                            break;
                    }
                }
            }
        }
    }
}
