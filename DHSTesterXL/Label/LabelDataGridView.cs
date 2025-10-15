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
                var columnName = LabelDataGridView.Columns[LabelDataGridView.CurrentCell.ColumnIndex].Name;

                if (row?.Tag is RowKey rowKey && rowKey == RowKey.DM && columnName == ColumnFontMm &&
                    e.Control is DataGridViewNumericUpDownEditingControl sizeEditor)
                {
                    // "한 변 mm" 입력: 1mm 스텝
                    sizeEditor.DecimalPlaces = 0;
                    sizeEditor.Increment = 1M;
                    sizeEditor.Minimum = 1M;
                    sizeEditor.Maximum = 100M;
                }
                else if (row?.Tag is RowKey rowKeyForXScale && rowKeyForXScale == RowKey.DM && columnName == ColumnScaleX &&
                         e.Control is DataGridViewNumericUpDownEditingControl xScaleEditor)
                {
                    xScaleEditor.DecimalPlaces = 0;
                    xScaleEditor.Increment = 1M;
                    xScaleEditor.Minimum = 10M;
                    xScaleEditor.Maximum = 144M;
                }
                else if (row?.Tag is RowKey rowKeyForYScale && rowKeyForYScale == RowKey.DM && columnName == ColumnScaleY &&
                         e.Control is DataGridViewNumericUpDownEditingControl yScaleEditor)
                {
                    yScaleEditor.DecimalPlaces = 0;
                    yScaleEditor.Increment = 1M;
                    yScaleEditor.Minimum = 10M;
                    yScaleEditor.Maximum = 144M;
                }
            };

            // 라벨 규격(좌표/크기 범위용)
            decimal labelWidthLimitMm = (decimal)Math.Max(1.0, _style.LabelWmm);
            decimal labelHeightLimitMm = (decimal)Math.Max(1.0, _style.LabelHmm);

            // ── 열 구성
            var sequenceColumn = new DataGridViewTextBoxColumn
            {
                Name = ColumnSequence,
                HeaderText = "순번",
                ReadOnly = true,
                Width = 48,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            sequenceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelDataGridView.Columns.Add(sequenceColumn);

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ColumnField,
                HeaderText = "항목",
                ReadOnly = true,
                Width = 80
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ColumnShowPreview,
                HeaderText = "미리보기",
                Width = 72,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ColumnShowPrint,
                HeaderText = "인쇄",
                Width = 54,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ColumnData,
                HeaderText = "데이터",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // 숫자 컬럼 (NumericUpDown)
            var xPositionColumn = new LabelNumericColumn
            {
                Name = ColumnXMm,
                HeaderText = "X(mm)",
                Width = 80,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Minimum = 0M,
                Maximum = labelWidthLimitMm
            };
            LabelDataGridView.Columns.Add(xPositionColumn);

            var yPositionColumn = new LabelNumericColumn
            {
                Name = ColumnYMm,
                HeaderText = "Y(mm)",
                Width = 80,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Minimum = 0M,
                Maximum = labelHeightLimitMm
            };
            LabelDataGridView.Columns.Add(yPositionColumn);

            var sizeColumn = new LabelNumericColumn
            {
                Name = ColumnFontMm,
                HeaderText = "Size(mm)",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = labelHeightLimitMm
            };
            LabelDataGridView.Columns.Add(sizeColumn);

            var xScaleColumn = new LabelNumericColumn
            {
                Name = ColumnScaleX,
                HeaderText = "X비율",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = 3.00M
            };
            LabelDataGridView.Columns.Add(xScaleColumn);

            var yScaleColumn = new LabelNumericColumn
            {
                Name = ColumnScaleY,
                HeaderText = "Y비율",
                Width = 80,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Minimum = 0.10M,
                Maximum = 3.00M
            };
            LabelDataGridView.Columns.Add(yScaleColumn);

            // ── 행(9행) : 미리 생성 + Tag 지정
            int logoRowIndex = LabelDataGridView.Rows.Add();
            int brandRowIndex = LabelDataGridView.Rows.Add();
            int partRowIndex = LabelDataGridView.Rows.Add();
            int leadFreeRowIndex = LabelDataGridView.Rows.Add();
            int hardwareRowIndex = LabelDataGridView.Rows.Add();
            int softwareRowIndex = LabelDataGridView.Rows.Add();
            int lotRowIndex = LabelDataGridView.Rows.Add();
            int serialRowIndex = LabelDataGridView.Rows.Add();
            int dataMatrixRowIndex = LabelDataGridView.Rows.Add();
            int ratingRowIndex = LabelDataGridView.Rows.Add();
            int fccIdRowIndex = LabelDataGridView.Rows.Add();
            int icIdRowIndex = LabelDataGridView.Rows.Add();
            int item1RowIndex = LabelDataGridView.Rows.Add();
            int item2RowIndex = LabelDataGridView.Rows.Add();
            int item3RowIndex = LabelDataGridView.Rows.Add();
            int item4RowIndex = LabelDataGridView.Rows.Add();
            int item5RowIndex = LabelDataGridView.Rows.Add();

            LabelDataGridView.Rows[logoRowIndex].Tag = RowKey.Logo;
            LabelDataGridView.Rows[brandRowIndex].Tag = RowKey.Brand;
            LabelDataGridView.Rows[partRowIndex].Tag = RowKey.Part;
            LabelDataGridView.Rows[leadFreeRowIndex].Tag = RowKey.Pb;
            LabelDataGridView.Rows[hardwareRowIndex].Tag = RowKey.HW;
            LabelDataGridView.Rows[softwareRowIndex].Tag = RowKey.SW;
            LabelDataGridView.Rows[lotRowIndex].Tag = RowKey.LOT;
            LabelDataGridView.Rows[serialRowIndex].Tag = RowKey.SN;
            LabelDataGridView.Rows[dataMatrixRowIndex].Tag = RowKey.DM;
            LabelDataGridView.Rows[ratingRowIndex].Tag = RowKey.Rating;
            LabelDataGridView.Rows[fccIdRowIndex].Tag = RowKey.FCCID;
            LabelDataGridView.Rows[icIdRowIndex].Tag = RowKey.ICID;
            LabelDataGridView.Rows[item1RowIndex].Tag = RowKey.Item1;
            LabelDataGridView.Rows[item2RowIndex].Tag = RowKey.Item2;
            LabelDataGridView.Rows[item3RowIndex].Tag = RowKey.Item3;
            LabelDataGridView.Rows[item4RowIndex].Tag = RowKey.Item4;
            LabelDataGridView.Rows[item5RowIndex].Tag = RowKey.Item5;

            // 표시명
            LabelDataGridView.Rows[logoRowIndex].Cells[ColumnField].Value = "Logo";
            LabelDataGridView.Rows[brandRowIndex].Cells[ColumnField].Value = "Brand";
            LabelDataGridView.Rows[partRowIndex].Cells[ColumnField].Value = "Part";

            var leadFreeRow = LabelDataGridView.Rows[leadFreeRowIndex];
            leadFreeRow.Cells[ColumnField].Value = "Pb";
            leadFreeRow.Cells[ColumnData].Value = "Pb";
            leadFreeRow.Cells[ColumnData].ReadOnly = true;

            LabelDataGridView.Rows[hardwareRowIndex].Cells[ColumnField].Value = "HW";
            LabelDataGridView.Rows[softwareRowIndex].Cells[ColumnField].Value = "SW";
            LabelDataGridView.Rows[lotRowIndex].Cells[ColumnField].Value = "LOT";
            LabelDataGridView.Rows[serialRowIndex].Cells[ColumnField].Value = "S/N";

            var dataMatrixRow = LabelDataGridView.Rows[dataMatrixRowIndex];
            dataMatrixRow.Cells[ColumnField].Value = "QR";
            dataMatrixRow.Cells[ColumnData].ReadOnly = true; // 자동 생성

            LabelDataGridView.Rows[ratingRowIndex].Cells[ColumnField].Value = "Rating";
            LabelDataGridView.Rows[fccIdRowIndex].Cells[ColumnField].Value = "FCC ID";
            LabelDataGridView.Rows[icIdRowIndex].Cells[ColumnField].Value = "IC ID";

            LabelDataGridView.Rows[item1RowIndex].Cells[ColumnField].Value = "Item1";
            LabelDataGridView.Rows[item2RowIndex].Cells[ColumnField].Value = "Item2";
            LabelDataGridView.Rows[item3RowIndex].Cells[ColumnField].Value = "Item3";
            LabelDataGridView.Rows[item4RowIndex].Cells[ColumnField].Value = "Item4";
            LabelDataGridView.Rows[item5RowIndex].Cells[ColumnField].Value = "Item5";

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

            var dataMatrixPayload = BuildQrPayloadFromGrid();

            // 실제 인쇄 기준으로 환산: 모듈 도트(정수) × 모듈 수 × (25.4/DPI)
            int dataMatrixModuleCount = GetCurrentDmModulesFromUiOrAuto();
            int dataMatrixModuleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), DefaultDpi));
            double dataMatrixSideLengthMm = dataMatrixModuleCount * (dataMatrixModuleDots * 25.4 / (double)DefaultDpi);

            // 그리드 'Size' 칸에 "한 변(mm)" 표시
            SetRow(RowKey.DM, "DM", dataMatrixPayload, _style.DMx, _style.DMy, Math.Round(dataMatrixSideLengthMm), 1.0, 1.0);

            // 보기 포맷(원하면 0.0으로 소수 1자리)
            var dataMatrixRow = GetRow(RowKey.DM);
            if (dataMatrixRow != null) dataMatrixRow.Cells[ColumnFontMm].Style.Format = "0";

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

            void SetRow(RowKey key, string fieldLabel, string cellValue, double positionX, double positionY, double sizeValue, double scaleXValue, double scaleYValue)
            {
                var row = LabelDataGridView.Rows.Cast<DataGridViewRow>().First(gridRow => (RowKey)gridRow.Tag == key);
                row.Cells[ColumnField].Value = fieldLabel;
                row.Cells[ColumnData].Value = cellValue;
                row.Cells[ColumnXMm].Value = positionX.ToString("0.###");
                row.Cells[ColumnYMm].Value = positionY.ToString("0.###");
                row.Cells[ColumnFontMm].Value = sizeValue.ToString("0.###");
                row.Cells[ColumnScaleX].Value = scaleXValue.ToString("0.###");
                row.Cells[ColumnScaleY].Value = scaleYValue.ToString("0.###");
            }
            void SetShow(RowKey key, bool showPreview, bool showPrint)
            {
                var row = GetRow(key);
                if (row == null) return;
                row.Cells[ColumnShowPreview].Value = showPreview;
                row.Cells[ColumnShowPrint].Value = showPrint;
            }
        }

        private void GetGridLabelValue()
        {
            if (LabelDataGridView == null || LabelDataGridView.Rows.Count < 4) return;

            bool AsBoolean(object value) => value is bool boolValue && boolValue;

            UpdateFromRow(RowKey.Logo, (x, y, f, data) =>
            {
                _style.LogoX = x;
                _style.LogoY = y;
                _style.LogoH = f;
                _style.LogoImagePath = (data ?? "").Trim();

                var logoRow = GetRow(RowKey.Logo);
                _style.LogoScaleX = ReadScaleCell(logoRow, ColumnScaleX, 1.0);
                _style.LogoScaleY = ReadScaleCell(logoRow, ColumnScaleY, 1.0);
                _style.ShowLogoPreview = AsBoolean(logoRow?.Cells[ColumnShowPreview].Value);
                _style.ShowLogoPrint = AsBoolean(logoRow?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Brand, (x, y, f, data) => {
                _style.BrandX = x; _style.BrandY = y; _style.BrandFont = f; _style.BrandText = data ?? "";
                var brandRow = GetRow(RowKey.Brand);
                _style.ShowBrandPreview = AsBoolean(brandRow?.Cells[ColumnShowPreview].Value);
                _style.ShowBrandPrint = AsBoolean(brandRow?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Part, (x, y, f, data) => {
                _style.PartX = x; _style.PartY = y; _style.PartFont = f; _style.PartText = data ?? "";
                var partRow = GetRow(RowKey.Part);
                _style.ShowPartPreview = AsBoolean(partRow?.Cells[ColumnShowPreview].Value);
                _style.ShowPartPrint = AsBoolean(partRow?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Pb, (x, y, f, data) => {
                _style.BadgeX = x; _style.BadgeY = y; _style.BadgeDiameter = f;
                var leadFreeRow = GetRow(RowKey.Pb);
                _style.ShowPbPreview = AsBoolean(leadFreeRow?.Cells[ColumnShowPreview].Value);
                _style.ShowPbPrint = AsBoolean(leadFreeRow?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.HW, (x, y, f, data) =>
            {
                _style.HWx = x; _style.HWy = y; _style.HWfont = f; _style.HWText = data;
                _style.ShowHWPreview = AsBoolean(GetRow(RowKey.HW)?.Cells[ColumnShowPreview].Value);
                _style.ShowHWPrint = AsBoolean(GetRow(RowKey.HW)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.SW, (x, y, f, data) =>
            {
                _style.SWx = x; _style.SWy = y; _style.SWfont = f; _style.SWText = data;
                _style.ShowSWPreview = AsBoolean(GetRow(RowKey.SW)?.Cells[ColumnShowPreview].Value);
                _style.ShowSWPrint = AsBoolean(GetRow(RowKey.SW)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.LOT, (x, y, f, data) =>
            {
                _style.LOTx = x; _style.LOTy = y; _style.LOTfont = f; _style.LOTText = (data ?? "").Trim();
                _style.ShowLOTPreview = AsBoolean(GetRow(RowKey.LOT)?.Cells[ColumnShowPreview].Value);
                _style.ShowLOTPrint = AsBoolean(GetRow(RowKey.LOT)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.SN, (x, y, f, data) =>
            {
                _style.SNx = x; _style.SNy = y; _style.SNfont = f;
                _style.SerialText = (data ?? "").Trim();
                _style.ShowSNPreview = AsBoolean(GetRow(RowKey.SN)?.Cells[ColumnShowPreview].Value);
                _style.ShowSNPrint = AsBoolean(GetRow(RowKey.SN)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.DM, (x, y, sideMmTarget, data) =>
            {
                _style.DMx = x;
                _style.DMy = y;

                double targetSideLengthMm = Math.Max(1.0, Math.Round(sideMmTarget));
                var autoPickResult = AutoPickDmByTarget(targetSideLengthMm, DefaultDpi);
                int autoPickModuleCount = autoPickResult.moduleCount;
                int autoPickModuleHeightDots = autoPickResult.moduleHeightDots;
                double dataMatrixSideLengthMm = autoPickResult.sideMmActual;

                _style.DMModuleMm = autoPickModuleHeightDots * 25.4 / (double)DefaultDpi;

                var dataMatrixRow = GetRow(RowKey.DM);
                if (dataMatrixRow != null)
                {
                    dataMatrixRow.Cells[ColumnScaleX].Value = autoPickModuleCount.ToString("0");  // DM 열
                    dataMatrixRow.Cells[ColumnScaleY].Value = autoPickModuleCount.ToString("0");  // DM 행
                    dataMatrixRow.Cells[ColumnFontMm].Value = Math.Round(dataMatrixSideLengthMm).ToString("0"); // 정수 표시
                    dataMatrixRow.Cells[ColumnScaleX].Style.Format = "0";
                    dataMatrixRow.Cells[ColumnScaleY].Style.Format = "0";
                    dataMatrixRow.Cells[ColumnFontMm].Style.Format = "0";       // 포맷 정수
                }

                _style.ShowDMPreview = AsBoolean(GetRow(RowKey.DM)?.Cells[ColumnShowPreview].Value);
                _style.ShowDMPrint = AsBoolean(GetRow(RowKey.DM)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Rating, (x, y, f, data) => {
                _style.RatingX = x; _style.RatingY = y; _style.RatingFont = f;
                _style.RatingText = (data ?? "").Trim();
                _style.ShowRatingPreview = AsBoolean(GetRow(RowKey.Rating)?.Cells[ColumnShowPreview].Value);
                _style.ShowRatingPrint = AsBoolean(GetRow(RowKey.Rating)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.FCCID, (x, y, f, data) => {
                _style.FCCIDX = x; _style.FCCIDY = y; _style.FCCIDFont = f;
                _style.FCCIDText = (data ?? "").Trim();
                _style.ShowFCCIDPreview = AsBoolean(GetRow(RowKey.FCCID)?.Cells[ColumnShowPreview].Value);
                _style.ShowFCCIDPrint = AsBoolean(GetRow(RowKey.FCCID)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.ICID, (x, y, f, data) => {
                _style.ICIDX = x; _style.ICIDY = y; _style.ICIDFont = f;
                _style.ICIDText = (data ?? "").Trim();
                _style.ShowICIDPreview = AsBoolean(GetRow(RowKey.ICID)?.Cells[ColumnShowPreview].Value);
                _style.ShowICIDPrint = AsBoolean(GetRow(RowKey.ICID)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Item1, (x, y, f, data) => {
                _style.Item1X = x; _style.Item1Y = y; _style.Item1Font = f;
                _style.Item1Text = (data ?? "").Trim();
                _style.ShowItem1Preview = AsBoolean(GetRow(RowKey.Item1)?.Cells[ColumnShowPreview].Value);
                _style.ShowItem1Print = AsBoolean(GetRow(RowKey.Item1)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Item2, (x, y, f, data) => {
                _style.Item2X = x; _style.Item2Y = y; _style.Item2Font = f;
                _style.Item2Text = (data ?? "").Trim();
                _style.ShowItem2Preview = AsBoolean(GetRow(RowKey.Item2)?.Cells[ColumnShowPreview].Value);
                _style.ShowItem2Print = AsBoolean(GetRow(RowKey.Item2)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Item3, (x, y, f, data) => {
                _style.Item3X = x; _style.Item3Y = y; _style.Item3Font = f;
                _style.Item3Text = (data ?? "").Trim();
                _style.ShowItem3Preview = AsBoolean(GetRow(RowKey.Item3)?.Cells[ColumnShowPreview].Value);
                _style.ShowItem3Print = AsBoolean(GetRow(RowKey.Item3)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Item4, (x, y, f, data) => {
                _style.Item4X = x; _style.Item4Y = y; _style.Item4Font = f;
                _style.Item4Text = (data ?? "").Trim();
                _style.ShowItem4Preview = AsBoolean(GetRow(RowKey.Item4)?.Cells[ColumnShowPreview].Value);
                _style.ShowItem4Print = AsBoolean(GetRow(RowKey.Item4)?.Cells[ColumnShowPrint].Value);
            });
            UpdateFromRow(RowKey.Item5, (x, y, f, data) => {
                _style.Item5X = x; _style.Item5Y = y; _style.Item5Font = f;
                _style.Item5Text = (data ?? "").Trim();
                _style.ShowItem5Preview = AsBoolean(GetRow(RowKey.Item5)?.Cells[ColumnShowPreview].Value);
                _style.ShowItem5Print = AsBoolean(GetRow(RowKey.Item5)?.Cells[ColumnShowPrint].Value);
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
                    row.Cells[ColumnSequence].Value = (i + 1).ToString();
            }
        }

        private void RefreshDmDataCell()
        {
            var row = GetRow(RowKey.DM);
            if (row != null)
                row.Cells[ColumnData].Value = BuildQrPayloadFromGrid();
        }

        // 로고 셀 더블클릭
        private void LabelGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = LabelDataGridView.Rows[e.RowIndex];
            var colName = LabelDataGridView.Columns[e.ColumnIndex].Name;

            if ((RowKey)row.Tag == RowKey.Logo && colName == ColumnData)
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
                        var baseDir = Path.GetFullPath(DefaultLogoDirectory).TrimEnd('\\') + "\\";
                        var fullNorm = Path.GetFullPath(full);
                        if (fullNorm.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                            toStore = Path.GetFileName(full);
                        else
                            toStore = full;

                        row.Cells[ColumnData].Value = toStore;
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
                if ((RowKey)row.Tag == RowKey.Logo && LabelDataGridView.Columns[e.ColumnIndex].Name == ColumnData)
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
            double x = ReadDoubleCell(row, ColumnXMm, 0);
            double y = ReadDoubleCell(row, ColumnYMm, 0);
            double f = ReadDoubleCell(row, ColumnFontMm, 2.6);
            string data = ReadStringCell(row, ColumnData, "");
            apply(x, y, f, data);
        }

        private string GetGridText(RowKey key, string fallback)
        {
            var row = GetRow(key);
            var s = ReadStringCell(row, ColumnData, null);
            return string.IsNullOrWhiteSpace(s) ? fallback : s;
        }

        private bool IsImmediateApplyColumn(string colName)
        {
            return colName == ColumnXMm || colName == ColumnYMm || colName == ColumnFontMm
                || colName == ColumnScaleX || colName == ColumnScaleY
                || colName == ColumnData
                || colName == ColumnShowPreview || colName == ColumnShowPrint;
        }

        private void CommitAndRefreshPreview(string colName)
        {
            if (_suppressPreview) return;
            GetGridLabelValue();
            Preview.Invalidate();
        }
    }
}
