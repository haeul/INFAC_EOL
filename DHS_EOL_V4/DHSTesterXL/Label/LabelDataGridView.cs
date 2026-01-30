using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public partial class FormProduct
    {
        // ───────────────────── Label Grid 구성 ─────────────────────
        private void SetupLabelGrid()
        {
            if (LabelDataGridView == null) return;

            LabelDataGridView.SuspendLayout();

            // ── 기본 셋업
            LabelDataGridView.AutoGenerateColumns = false;
            LabelDataGridView.Columns.Clear();
            LabelDataGridView.Rows.Clear();
            LabelDataGridView.RowTemplate.Height = 24;
            LabelDataGridView.AllowUserToAddRows = false;
            LabelDataGridView.AllowUserToDeleteRows = false;
            LabelDataGridView.RowHeadersVisible = false;
            LabelDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            LabelDataGridView.MultiSelect = false;
            LabelDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            LabelDataGridView.EditingControlShowing += (s, e) =>
            {
                if (LabelDataGridView.CurrentCell == null) return;
                var row = LabelDataGridView.CurrentRow;
                var colName = LabelDataGridView.Columns[LabelDataGridView.CurrentCell.ColumnIndex].Name;

                if (row?.Tag is RowKey key && key == RowKey.DM && colName == COL_SIZE &&
                    e.Control is DataGridViewNumericUpDownEditingControl numericEditor)
                {
                    // "한 변 mm" 입력: 1mm 스텝 
                    numericEditor.DecimalPlaces = 0;
                    numericEditor.Increment = 1M;
                    numericEditor.Minimum = 1M;
                    numericEditor.Maximum = 100M;
                }
                else if (row?.Tag is RowKey key2 && key2 == RowKey.DM && colName == COL_XSCALE &&
                         e.Control is DataGridViewNumericUpDownEditingControl numericEditorX)
                {
                    numericEditorX.DecimalPlaces = 0;
                    numericEditorX.Increment = 1M;
                    numericEditorX.Minimum = 10M;
                    numericEditorX.Maximum = 144M;
                }
                else if (row?.Tag is RowKey key3 && key3 == RowKey.DM && colName == COL_YSCALE &&
                         e.Control is DataGridViewNumericUpDownEditingControl numericEditorY)
                {
                    numericEditorY.DecimalPlaces = 0;
                    numericEditorY.Increment = 1M;
                    numericEditorY.Minimum = 10M;
                    numericEditorY.Maximum = 144M;
                }
            };

            // 라벨 규격(좌표/크기 범위용)
            decimal W = (decimal)Math.Max(1.0, _style.LabelWmm);
            decimal H = (decimal)Math.Max(1.0, _style.LabelHmm);

            // ── 열 구성
            var colSeq = new DataGridViewTextBoxColumn
            {
                Name = COL_SEQ,
                HeaderText = "순번",
                ReadOnly = true,
                Width = 48,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colSeq.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelDataGridView.Columns.Add(colSeq);

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = COL_FIELD,
                HeaderText = "항목",
                ReadOnly = true,
                Width = 80
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = COL_SHOW_PREVIEW,
                HeaderText = "미리보기",
                Width = 72,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = COL_SHOW_PRINT,
                HeaderText = "인쇄",
                Width = 54,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = COL_DATA,
                HeaderText = "데이터",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // 숫자 컬럼 (NumericUpDown)
            var colX = new LabelNumericColumn
            {
                Name = COL_X,
                HeaderText = "X(mm)",
                Width = 80,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Minimum = 0M,
                Maximum = W
            };
            LabelDataGridView.Columns.Add(colX);

            var colY = new LabelNumericColumn
            {
                Name = COL_Y,
                HeaderText = "Y(mm)",
                Width = 80,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Minimum = 0M,
                Maximum = H
            };
            LabelDataGridView.Columns.Add(colY);

            var colSize = new LabelNumericColumn
            {
                Name = COL_SIZE,
                HeaderText = "Size(mm)",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = H
            };
            LabelDataGridView.Columns.Add(colSize);

            var colXs = new LabelNumericColumn
            {
                Name = COL_XSCALE,
                HeaderText = "X비율",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = 3.00M
            };
            LabelDataGridView.Columns.Add(colXs);

            var colYs = new LabelNumericColumn
            {
                Name = COL_YSCALE,
                HeaderText = "Y비율",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = 3.00M
            };
            LabelDataGridView.Columns.Add(colYs);

            // ── 행(9행) : 미리 생성 + Tag 지정
            int rLogo = LabelDataGridView.Rows.Add();
            int rBrand = LabelDataGridView.Rows.Add();
            int rPart = LabelDataGridView.Rows.Add();
            int rPb = LabelDataGridView.Rows.Add();
            int rHW = LabelDataGridView.Rows.Add();
            int rSW = LabelDataGridView.Rows.Add();
            int rLOT = LabelDataGridView.Rows.Add();
            int rSN = LabelDataGridView.Rows.Add();
            int rDM = LabelDataGridView.Rows.Add();
            int rRating = LabelDataGridView.Rows.Add();
            int rFCC = LabelDataGridView.Rows.Add();
            int rIC = LabelDataGridView.Rows.Add();
            int rItem1 = LabelDataGridView.Rows.Add();
            int rItem2 = LabelDataGridView.Rows.Add();
            int rItem3 = LabelDataGridView.Rows.Add();
            int rItem4 = LabelDataGridView.Rows.Add();
            int rItem5 = LabelDataGridView.Rows.Add();

            LabelDataGridView.Rows[rLogo].Tag = RowKey.Logo;
            LabelDataGridView.Rows[rBrand].Tag = RowKey.Brand;
            LabelDataGridView.Rows[rPart].Tag = RowKey.Part;
            LabelDataGridView.Rows[rPb].Tag = RowKey.Pb;
            LabelDataGridView.Rows[rHW].Tag = RowKey.HW;
            LabelDataGridView.Rows[rSW].Tag = RowKey.SW;
            LabelDataGridView.Rows[rLOT].Tag = RowKey.LOT;
            LabelDataGridView.Rows[rSN].Tag = RowKey.SN;
            LabelDataGridView.Rows[rDM].Tag = RowKey.DM;
            LabelDataGridView.Rows[rRating].Tag = RowKey.Rating;
            LabelDataGridView.Rows[rFCC].Tag = RowKey.FCCID;
            LabelDataGridView.Rows[rIC].Tag = RowKey.ICID;
            LabelDataGridView.Rows[rItem1].Tag = RowKey.Item1;
            LabelDataGridView.Rows[rItem2].Tag = RowKey.Item2;
            LabelDataGridView.Rows[rItem3].Tag = RowKey.Item3;
            LabelDataGridView.Rows[rItem4].Tag = RowKey.Item4;
            LabelDataGridView.Rows[rItem5].Tag = RowKey.Item5;

            // 표시명
            LabelDataGridView.Rows[rLogo].Cells[COL_FIELD].Value = "Logo";
            LabelDataGridView.Rows[rBrand].Cells[COL_FIELD].Value = "Brand";
            LabelDataGridView.Rows[rPart].Cells[COL_FIELD].Value = "Part";

            var rowPb = LabelDataGridView.Rows[rPb];
            rowPb.Cells[COL_FIELD].Value = "Pb";
            rowPb.Cells[COL_DATA].Value = "Pb";
            rowPb.Cells[COL_DATA].ReadOnly = true;

            LabelDataGridView.Rows[rHW].Cells[COL_FIELD].Value = "HW";
            LabelDataGridView.Rows[rSW].Cells[COL_FIELD].Value = "SW";
            LabelDataGridView.Rows[rLOT].Cells[COL_FIELD].Value = "LOT";
            LabelDataGridView.Rows[rSN].Cells[COL_FIELD].Value = "S/N";

            var rowDM = LabelDataGridView.Rows[rDM];
            rowDM.Cells[COL_FIELD].Value = "Data Matrix";
            rowDM.Cells[COL_DATA].ReadOnly = true; // 자동 생성

            LabelDataGridView.Rows[rRating].Cells[COL_FIELD].Value = "Rating";
            LabelDataGridView.Rows[rFCC].Cells[COL_FIELD].Value = "FCC ID";
            LabelDataGridView.Rows[rIC].Cells[COL_FIELD].Value = "IC ID";

            LabelDataGridView.Rows[rItem1].Cells[COL_FIELD].Value = "Item1";
            LabelDataGridView.Rows[rItem2].Cells[COL_FIELD].Value = "Item2";
            LabelDataGridView.Rows[rItem3].Cells[COL_FIELD].Value = "Item3";
            LabelDataGridView.Rows[rItem4].Cells[COL_FIELD].Value = "Item4";
            LabelDataGridView.Rows[rItem5].Cells[COL_FIELD].Value = "Item5";

            // 이벤트
            LabelDataGridView.CurrentCellDirtyStateChanged -= LabelGrid_CurrentCellDirtyStateChanged;
            LabelDataGridView.CellEndEdit -= LabelGrid_CellEndEdit;
            LabelDataGridView.DataError -= LabelGrid_DataError;

            LabelDataGridView.CurrentCellDirtyStateChanged += LabelGrid_CurrentCellDirtyStateChanged;
            LabelDataGridView.CellEndEdit += LabelGrid_CellEndEdit;
            LabelDataGridView.DataError += LabelGrid_DataError;

            RefreshSeqNumbers();
            LabelDataGridView.ResumeLayout();
        }

        private void UpdateGridLabel()
        {
            if (LabelDataGridView == null || LabelDataGridView.Rows.Count < 4) return;

            SetRow(RowKey.Logo, "Logo", _style.LogoImagePath ?? "", _style.LogoX, _style.LogoY, _style.LogoH, _style.LogoScaleX, _style.LogoScaleY);
            SetRow(RowKey.Brand, "회사명", _style.BrandText ?? "", _style.BrandX, _style.BrandY, _style.BrandFont, 1, 1);
            SetRow(RowKey.Part, "품번", _style.PartText ?? "", _style.PartX, _style.PartY, _style.PartFont, 1, 1);

            SetRow(RowKey.Pb, "Pb", "Pb", _style.BadgeX, _style.BadgeY, _style.BadgeDiameter, 1.0, 1.0);

            SetRow(RowKey.HW, "HW", _style.HWText, _style.HWx, _style.HWy, PositiveOr(_style.HWfont, 2.6), 1.0, 1.0);
            SetRow(RowKey.SW, "SW", _style.SWText, _style.SWx, _style.SWy, PositiveOr(_style.SWfont, 2.6), 1.0, 1.0);
            SetRow(RowKey.LOT, "LOT", _style.LOTText ?? "", _style.LOTx, _style.LOTy, PositiveOr(_style.LOTfont, 2.6), 1.0, 1.0);
            SetRow(RowKey.SN, "S/N", _style.SerialText ?? "", _style.SNx, _style.SNy, PositiveOr(_style.SNfont, 2.6), 1.0, 1.0);

            var dmData = BuildDmPayloadFromGrid();

            // 실제 인쇄 기준으로 환산: 모듈 도트(정수) × 모듈 수 × (25.4/DPI)
            int modules = GetCurrentDmModulesFromUiOrAuto();
            int moduleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), DEFAULT_DPI));
            double sideMmActual = modules * (moduleDots * 25.4 / (double)DEFAULT_DPI);

            // 그리드 'Size' 칸에 "한 변(mm)" 표시
            SetRow(RowKey.DM, "DM", dmData, _style.DMx, _style.DMy, Math.Round(sideMmActual), 1.0, 1.0);

            // 보기 포맷(원하면 0.0으로 소수 1자리)
            var dmRow = GetRow(RowKey.DM);
            if (dmRow != null) dmRow.Cells[COL_SIZE].Style.Format = "0";

            SetRow(RowKey.Rating, "Rating", _style.RatingText ?? "", _style.RatingX, _style.RatingY, PositiveOr(_style.RatingFont, 2.6), 1, 1);
            SetRow(RowKey.FCCID, "FCC ID", _style.FCCIDText ?? "", _style.FCCIDX, _style.FCCIDY, PositiveOr(_style.FCCIDFont, 2.6), 1, 1);
            SetRow(RowKey.ICID, "IC ID", _style.ICIDText ?? "", _style.ICIDX, _style.ICIDY, PositiveOr(_style.ICIDFont, 2.6), 1, 1);
            SetRow(RowKey.Item1, "Item1", _style.Item1Text ?? "", _style.Item1X, _style.Item1Y, PositiveOr(_style.Item1Font, 2.6), 1, 1);
            SetRow(RowKey.Item2, "Item2", _style.Item2Text ?? "", _style.Item2X, _style.Item2Y, PositiveOr(_style.Item2Font, 2.6), 1, 1);
            SetRow(RowKey.Item3, "Item3", _style.Item3Text ?? "", _style.Item3X, _style.Item3Y, PositiveOr(_style.Item3Font, 2.6), 1, 1);
            SetRow(RowKey.Item4, "Item4", _style.Item4Text ?? "", _style.Item4X, _style.Item4Y, PositiveOr(_style.Item4Font, 2.6), 1, 1);
            SetRow(RowKey.Item5, "Item5", _style.Item5Text ?? "", _style.Item5X, _style.Item5Y, PositiveOr(_style.Item5Font, 2.6), 1, 1);

            SetShow(RowKey.Logo, _style.ShowLogoPreview, _style.ShowLogoPrint);
            SetShow(RowKey.Brand, _style.ShowBrandPreview, _style.ShowBrandPrint);
            SetShow(RowKey.Part, _style.ShowPartPreview, _style.ShowPartPrint);
            SetShow(RowKey.Pb, _style.ShowPbPreview, _style.ShowPbPrint);
            SetShow(RowKey.HW, _style.ShowHWPreview, _style.ShowHWPrint);
            SetShow(RowKey.SW, _style.ShowSWPreview, _style.ShowSWPrint);
            SetShow(RowKey.LOT, _style.ShowLOTPreview, _style.ShowLOTPrint);
            SetShow(RowKey.SN, _style.ShowSNPreview, _style.ShowSNPrint);
            SetShow(RowKey.DM, _style.ShowDMPreview, _style.ShowDMPrint);
            SetShow(RowKey.Rating, _style.ShowRatingPreview, _style.ShowRatingPrint);
            SetShow(RowKey.FCCID, _style.ShowFCCIDPreview, _style.ShowFCCIDPrint);
            SetShow(RowKey.ICID, _style.ShowICIDPreview, _style.ShowICIDPrint);
            SetShow(RowKey.Item1, _style.ShowItem1Preview, _style.ShowItem1Print);
            SetShow(RowKey.Item2, _style.ShowItem2Preview, _style.ShowItem2Print);
            SetShow(RowKey.Item3, _style.ShowItem3Preview, _style.ShowItem3Print);
            SetShow(RowKey.Item4, _style.ShowItem4Preview, _style.ShowItem4Print);
            SetShow(RowKey.Item5, _style.ShowItem5Preview, _style.ShowItem5Print);

            void SetRow(RowKey key, string name, string data, double x, double y, double size, double xScale, double yScale)
            {
                var row = LabelDataGridView.Rows.Cast<DataGridViewRow>().First(r => (RowKey)r.Tag == key);
                row.Cells[COL_FIELD].Value = name;
                row.Cells[COL_DATA].Value = data;
                row.Cells[COL_X].Value = x.ToString("0.###");
                row.Cells[COL_Y].Value = y.ToString("0.###");
                row.Cells[COL_SIZE].Value = size.ToString("0.###");
                row.Cells[COL_XSCALE].Value = xScale.ToString("0.###");
                row.Cells[COL_YSCALE].Value = yScale.ToString("0.###");
            }
            void SetShow(RowKey key, bool showPreview, bool showPrint)
            {
                var row = GetRow(key);
                if (row == null) return;
                row.Cells[COL_SHOW_PREVIEW].Value = showPreview;
                row.Cells[COL_SHOW_PRINT].Value = showPrint;
            }
        }

        private void GetGridLabelValue()
        {
            if (LabelDataGridView == null || LabelDataGridView.Rows.Count < 4) return;

            bool B(object v) => v is bool b && b;

            UpdateFromRow(RowKey.Logo, (x, y, f, data) =>
            {
                _style.LogoX = x;
                _style.LogoY = y;
                _style.LogoH = f;
                _style.LogoImagePath = (data ?? "").Trim();

                var row = GetRow(RowKey.Logo);
                _style.LogoScaleX = ReadScaleCell(row, COL_XSCALE, 1.0);
                _style.LogoScaleY = ReadScaleCell(row, COL_YSCALE, 1.0);
                _style.ShowLogoPreview = B(GetRow(RowKey.Logo)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowLogoPrint = B(GetRow(RowKey.Logo)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Brand, (x, y, f, data) => {
                _style.BrandX = x; _style.BrandY = y; _style.BrandFont = f; _style.BrandText = data ?? "";
                _style.ShowBrandPreview = B(GetRow(RowKey.Brand)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowBrandPrint = B(GetRow(RowKey.Brand)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Part, (x, y, f, data) => {
                _style.PartX = x; _style.PartY = y; _style.PartFont = f; _style.PartText = data ?? "";
                _style.ShowPartPreview = B(GetRow(RowKey.Part)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowPartPrint = B(GetRow(RowKey.Part)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Pb, (x, y, f, data) => {
                _style.BadgeX = x; _style.BadgeY = y; _style.BadgeDiameter = f;
                _style.ShowPbPreview = B(GetRow(RowKey.Pb)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowPbPrint = B(GetRow(RowKey.Pb)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.HW, (x, y, f, data) =>
            {
                _style.HWx = x; _style.HWy = y; _style.HWfont = f; _style.HWText = data;
                _style.ShowHWPreview = B(GetRow(RowKey.HW)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowHWPrint = B(GetRow(RowKey.HW)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.SW, (x, y, f, data) =>
            {
                _style.SWx = x; _style.SWy = y; _style.SWfont = f; _style.SWText = data;
                _style.ShowSWPreview = B(GetRow(RowKey.SW)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowSWPrint = B(GetRow(RowKey.SW)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.LOT, (x, y, f, data) =>
            {
                _style.LOTx = x; _style.LOTy = y; _style.LOTfont = f; _style.LOTText = (data ?? "").Trim();
                _style.ShowLOTPreview = B(GetRow(RowKey.LOT)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowLOTPrint = B(GetRow(RowKey.LOT)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.SN, (x, y, f, data) =>
            {
                _style.SNx = x; _style.SNy = y; _style.SNfont = f;
                _style.SerialText = (data ?? "").Trim();
                _style.ShowSNPreview = B(GetRow(RowKey.SN)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowSNPrint = B(GetRow(RowKey.SN)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.DM, (x, y, sideMmTarget, data) =>
            {
                _style.DMx = x;
                _style.DMy = y;

                double targetMm = Math.Max(1.0, Math.Round(sideMmTarget));
                var pick = AutoPickDmByTarget(targetMm, DEFAULT_DPI);
                int M = pick.M;
                int h = pick.h;
                double sideMmActual = pick.sideMmActual;

                _style.DMModuleMm = h * 25.4 / (double)DEFAULT_DPI;

                var rowQR = GetRow(RowKey.DM);
                if (rowQR != null)
                {
                    rowQR.Cells[COL_XSCALE].Value = M.ToString("0");  // DM 열
                    rowQR.Cells[COL_YSCALE].Value = M.ToString("0");  // DM 행
                    rowQR.Cells[COL_SIZE].Value = Math.Round(sideMmActual).ToString("0"); // 정수 표시
                    rowQR.Cells[COL_XSCALE].Style.Format = "0";
                    rowQR.Cells[COL_YSCALE].Style.Format = "0";
                    rowQR.Cells[COL_SIZE].Style.Format = "0";       // 포맷 정수
                }

                _style.ShowDMPreview = B(GetRow(RowKey.DM)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowDMPrint = B(GetRow(RowKey.DM)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Rating, (x, y, f, data) => {
                _style.RatingX = x; _style.RatingY = y; _style.RatingFont = f;
                _style.RatingText = (data ?? "").Trim();
                _style.ShowRatingPreview = B(GetRow(RowKey.Rating)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowRatingPrint = B(GetRow(RowKey.Rating)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.FCCID, (x, y, f, data) => {
                _style.FCCIDX = x; _style.FCCIDY = y; _style.FCCIDFont = f;
                _style.FCCIDText = (data ?? "").Trim();
                _style.ShowFCCIDPreview = B(GetRow(RowKey.FCCID)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowFCCIDPrint = B(GetRow(RowKey.FCCID)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.ICID, (x, y, f, data) => {
                _style.ICIDX = x; _style.ICIDY = y; _style.ICIDFont = f;
                _style.ICIDText = (data ?? "").Trim();
                _style.ShowICIDPreview = B(GetRow(RowKey.ICID)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowICIDPrint = B(GetRow(RowKey.ICID)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Item1, (x, y, f, data) => {
                _style.Item1X = x; _style.Item1Y = y; _style.Item1Font = f;
                _style.Item1Text = (data ?? "").Trim();
                _style.ShowItem1Preview = B(GetRow(RowKey.Item1)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowItem1Print = B(GetRow(RowKey.Item1)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Item2, (x, y, f, data) => {
                _style.Item2X = x; _style.Item2Y = y; _style.Item2Font = f;
                _style.Item2Text = (data ?? "").Trim();
                _style.ShowItem2Preview = B(GetRow(RowKey.Item2)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowItem2Print = B(GetRow(RowKey.Item2)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Item3, (x, y, f, data) => {
                _style.Item3X = x; _style.Item3Y = y; _style.Item3Font = f;
                _style.Item3Text = (data ?? "").Trim();
                _style.ShowItem3Preview = B(GetRow(RowKey.Item3)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowItem3Print = B(GetRow(RowKey.Item3)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Item4, (x, y, f, data) => {
                _style.Item4X = x; _style.Item4Y = y; _style.Item4Font = f;
                _style.Item4Text = (data ?? "").Trim();
                _style.ShowItem4Preview = B(GetRow(RowKey.Item4)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowItem4Print = B(GetRow(RowKey.Item4)?.Cells[COL_SHOW_PRINT].Value);
            });
            UpdateFromRow(RowKey.Item5, (x, y, f, data) => {
                _style.Item5X = x; _style.Item5Y = y; _style.Item5Font = f;
                _style.Item5Text = (data ?? "").Trim();
                _style.ShowItem5Preview = B(GetRow(RowKey.Item5)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowItem5Print = B(GetRow(RowKey.Item5)?.Cells[COL_SHOW_PRINT].Value);
            });

            RefreshDmDataCell();
        }

        private void RefreshSeqNumbers()
        {
            if (LabelDataGridView == null) return;
            for (int i = 0; i < LabelDataGridView.Rows.Count; i++)
            {
                var row = LabelDataGridView.Rows[i];
                if (!row.IsNewRow)
                    row.Cells[COL_SEQ].Value = (i + 1).ToString();
            }
        }

        private void RefreshDmDataCell()
        {
            var row = GetRow(RowKey.DM);
            if (row != null)
                row.Cells[COL_DATA].Value = BuildDmPayloadFromGrid();
        }

        // 로고 셀 더블클릭
        private void LabelGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = LabelDataGridView.Rows[e.RowIndex];
            var colName = LabelDataGridView.Columns[e.ColumnIndex].Name;

            if ((RowKey)row.Tag == RowKey.Logo && colName == COL_DATA)
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = "로고 이미지 선택";
                    ofd.Filter = "이미지 파일|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                    if (!string.IsNullOrWhiteSpace(_lastLogoDir) && Directory.Exists(_lastLogoDir))
                        ofd.InitialDirectory = _lastLogoDir;
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog(this) == DialogResult.OK)
                    {
                        string full = ofd.FileName;
                        _lastLogoDir = Path.GetDirectoryName(full);

                        string toStore;
                        var baseDir = Path.GetFullPath(DEFAULT_LOGO_DIR).TrimEnd('\\') + "\\";
                        var fullNorm = Path.GetFullPath(full);
                        if (fullNorm.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                            toStore = Path.GetFileName(full);
                        else
                            toStore = full;

                        row.Cells[COL_DATA].Value = toStore;
                        _style.LogoImagePath = toStore;

                        LoadLogoBitmap();
                        _isModified = true;
                        Preview.Invalidate();
                    }
                }
            }
        }

        private void LabelGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (LabelDataGridView == null) return;

            if (LabelDataGridView.IsCurrentCellDirty)
            {
                LabelDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                var colName = LabelDataGridView.CurrentCell?.OwningColumn?.Name;
                if (!string.IsNullOrEmpty(colName) && IsImmediateApplyColumn(colName))
                    CommitAndRefreshPreview(colName);
            }
        }

        private void LabelGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_suppressPreview) return;
            if (LabelDataGridView == null || e.RowIndex < 0) return;

            _isModified = true;
            try
            {
                var row = LabelDataGridView.Rows[e.RowIndex];
                if ((RowKey)row.Tag == RowKey.Logo && LabelDataGridView.Columns[e.ColumnIndex].Name == COL_DATA)
                    LoadLogoBitmap();

                GetGridLabelValue();
            }
            catch { }
            Preview.Invalidate();
        }

        private void LabelGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        // 유틸: 행/셀 읽기
        private DataGridViewRow GetRow(RowKey key)
            => LabelDataGridView?.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => (RowKey)r.Tag == key);

        private double ReadDoubleCell(DataGridViewRow row, string colName, double fallback)
        {
            if (row == null) return fallback;
            var v = row.Cells[colName]?.Value?.ToString();
            if (double.TryParse(v, out var d)) return d;
            return fallback;
        }
        private string ReadStringCell(DataGridViewRow row, string colName, string fallback = "")
        {
            if (row == null) return fallback;
            var v = row.Cells[colName]?.Value?.ToString();
            return string.IsNullOrEmpty(v) ? fallback : v;
        }
        private double ReadScaleCell(DataGridViewRow row, string colName, double fallback = 1.0)
        {
            if (row == null) return fallback;
            var s = row.Cells[colName]?.Value?.ToString();
            return double.TryParse(s, out var d) ? d : fallback;
        }

        private void UpdateFromRow(RowKey key, Action<double, double, double, string> apply)
        {
            var row = GetRow(key);
            double x = ReadDoubleCell(row, COL_X, 0);
            double y = ReadDoubleCell(row, COL_Y, 0);
            double f = ReadDoubleCell(row, COL_SIZE, 2.6);
            string data = ReadStringCell(row, COL_DATA, "");
            apply(x, y, f, data);
        }

        private string GetGridText(RowKey key, string fallback)
        {
            var row = GetRow(key);
            var s = ReadStringCell(row, COL_DATA, null);
            return string.IsNullOrWhiteSpace(s) ? fallback : s;
        }

        private bool IsImmediateApplyColumn(string colName)
        {
            return colName == COL_X || colName == COL_Y || colName == COL_SIZE
                || colName == COL_XSCALE || colName == COL_YSCALE
                || colName == COL_DATA
                || colName == COL_SHOW_PREVIEW || colName == COL_SHOW_PRINT;
        }

        private void CommitAndRefreshPreview(string colName)
        {
            if (_suppressPreview) return;
            GetGridLabelValue();
            Preview.Invalidate();
        }

        /// <summary>
        /// 전체 라벨 요소의 X/Y 좌표에 전역 오프셋을 적용한다.
        /// (버튼에서 deltaX, deltaY를 받아서 호출)
        /// </summary>
        private void ApplyGlobalOffsetToGrid(double deltaX, double deltaY)
        {
            if (LabelDataGridView == null) return;

            _suppressPreview = true;
            try
            {
                // 전체 RowKey 순회하면서 X, Y 에 delta 더해주기
                foreach (RowKey key in Enum.GetValues(typeof(RowKey)))
                {
                    // 필요하면 실제 요소만 골라서 (Logo, Brand, Part, Pb, HW, SW, LOT, SN, Rating, FCCID, ICID, QR)
                    var row = GetRow(key);
                    if (row == null) continue;

                    double x = ReadDoubleCell(row, COL_X, 0.0);
                    double y = ReadDoubleCell(row, COL_Y, 0.0);

                    row.Cells[COL_X].Value = (x + deltaX).ToString("0.###");
                    row.Cells[COL_Y].Value = (y + deltaY).ToString("0.###");
                }

                // 그리드 → _style 동기화
                GetGridLabelValue();
                _isModified = true;
            }
            finally
            {
                _suppressPreview = false;
            }
        }

    }
}
