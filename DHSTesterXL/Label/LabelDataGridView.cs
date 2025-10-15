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

                if (row?.Tag is RowKey rowKey && rowKey == RowKey.DM && columnName == ColumnNameSizeMm &&
                    e.Control is DataGridViewNumericUpDownEditingControl sizeEditor)
                {
                    // "한 변 mm" 입력: 1mm 스텝
                    sizeEditor.DecimalPlaces = 0;
                    sizeEditor.Increment = 1M;
                    sizeEditor.Minimum = 1M;
                    sizeEditor.Maximum = 100M;
                }
                else if (row?.Tag is RowKey rowKeyForXScale && rowKeyForXScale == RowKey.DM && columnName == ColumnNameScaleX &&
                         e.Control is DataGridViewNumericUpDownEditingControl xScaleEditor)
                {
                    xScaleEditor.DecimalPlaces = 0;
                    xScaleEditor.Increment = 1M;
                    xScaleEditor.Minimum = 10M;
                    xScaleEditor.Maximum = 144M;
                }
                else if (row?.Tag is RowKey rowKeyForYScale && rowKeyForYScale == RowKey.DM && columnName == ColumnNameScaleY &&
                         e.Control is DataGridViewNumericUpDownEditingControl yScaleEditor)
                {
                    yScaleEditor.DecimalPlaces = 0;
                    yScaleEditor.Increment = 1M;
                    yScaleEditor.Minimum = 10M;
                    yScaleEditor.Maximum = 144M;
                }
            };

            // 라벨 규격(좌표/크기 범위용)
            decimal labelWidthLimitMm = (decimal)Math.Max(1.0, _style.LabelWidthMm);
            decimal labelHeightLimitMm = (decimal)Math.Max(1.0, _style.LabelHeightMm);

            // ── 열 구성
            var sequenceColumn = new DataGridViewTextBoxColumn
            {
                Name = ColumnNameSequence,
                HeaderText = "순번",
                ReadOnly = true,
                Width = 48,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            sequenceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelDataGridView.Columns.Add(sequenceColumn);

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ColumnNameField,
                HeaderText = "항목",
                ReadOnly = true,
                Width = 80
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ColumnNamePreviewEnabled,
                HeaderText = "미리보기",
                Width = 72,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ColumnNamePrintEnabled,
                HeaderText = "인쇄",
                Width = 54,
                ThreeState = false
            });

            LabelDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ColumnNameData,
                HeaderText = "데이터",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // 숫자 컬럼 (NumericUpDown)
            var xPositionColumn = new LabelNumericColumn
            {
                Name = ColumnNamePositionXMm,
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
                Name = ColumnNamePositionYMm,
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
                Name = ColumnNameSizeMm,
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
                Name = ColumnNameScaleX,
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
                Name = ColumnNameScaleY,
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
            LabelDataGridView.Rows[logoRowIndex].Cells[ColumnNameField].Value = "Logo";
            LabelDataGridView.Rows[brandRowIndex].Cells[ColumnNameField].Value = "Brand";
            LabelDataGridView.Rows[partRowIndex].Cells[ColumnNameField].Value = "Part";

            var leadFreeRow = LabelDataGridView.Rows[leadFreeRowIndex];
            leadFreeRow.Cells[ColumnNameField].Value = "Pb";
            leadFreeRow.Cells[ColumnNameData].Value = "Pb";
            leadFreeRow.Cells[ColumnNameData].ReadOnly = true;

            LabelDataGridView.Rows[hardwareRowIndex].Cells[ColumnNameField].Value = "HW";
            LabelDataGridView.Rows[softwareRowIndex].Cells[ColumnNameField].Value = "SW";
            LabelDataGridView.Rows[lotRowIndex].Cells[ColumnNameField].Value = "LOT";
            LabelDataGridView.Rows[serialRowIndex].Cells[ColumnNameField].Value = "S/N";

            var dataMatrixRow = LabelDataGridView.Rows[dataMatrixRowIndex];
            dataMatrixRow.Cells[ColumnNameField].Value = "QR";
            dataMatrixRow.Cells[ColumnNameData].ReadOnly = true; // 자동 생성

            LabelDataGridView.Rows[ratingRowIndex].Cells[ColumnNameField].Value = "Rating";
            LabelDataGridView.Rows[fccIdRowIndex].Cells[ColumnNameField].Value = "FCC ID";
            LabelDataGridView.Rows[icIdRowIndex].Cells[ColumnNameField].Value = "IC ID";

            LabelDataGridView.Rows[item1RowIndex].Cells[ColumnNameField].Value = "Item1";
            LabelDataGridView.Rows[item2RowIndex].Cells[ColumnNameField].Value = "Item2";
            LabelDataGridView.Rows[item3RowIndex].Cells[ColumnNameField].Value = "Item3";
            LabelDataGridView.Rows[item4RowIndex].Cells[ColumnNameField].Value = "Item4";
            LabelDataGridView.Rows[item5RowIndex].Cells[ColumnNameField].Value = "Item5";

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

            SetRow(RowKey.Logo, "Logo", _style.LogoImagePath ?? "", _style.LogoXMm, _style.LogoYMm, _style.LogoHeightMm, _style.LogoScaleXRatio, _style.LogoScaleYRatio);
            SetRow(RowKey.Brand, "회사명", _style.BrandText ?? "", _style.BrandXMm, _style.BrandYMm, _style.BrandFontMm, 1, 1);
            SetRow(RowKey.Part, "품번", _style.PartText ?? "", _style.PartXMm, _style.PartYMm, _style.PartFontMm, 1, 1);

            SetRow(RowKey.Pb, "Pb", "Pb", _style.BadgeXMm, _style.BadgeYMm, _style.BadgeDiameterMm, 1.0, 1.0);

            SetRow(RowKey.HW, "HW", _style.HardwareText, _style.HardwareXMm, _style.HardwareYMm, PositiveOr(_style.HardwareFontMm, 2.6), 1.0, 1.0);
            SetRow(RowKey.SW, "SW", _style.SoftwareText, _style.SoftwareXMm, _style.SoftwareYMm, PositiveOr(_style.SoftwareFontMm, 2.6), 1.0, 1.0);
            SetRow(RowKey.LOT, "LOT", _style.LotText ?? "", _style.LotXMm, _style.LotYMm, PositiveOr(_style.LotFontMm, 2.6), 1.0, 1.0);
            SetRow(RowKey.SN, "S/N", _style.SerialText ?? "", _style.SerialXMm, _style.SerialYMm, PositiveOr(_style.SerialFontMm, 2.6), 1.0, 1.0);

            var dataMatrixPayload = BuildQrPayloadFromGrid();

            // 실제 인쇄 기준으로 환산: 모듈 도트(정수) × 모듈 수 × (25.4/DPI)
            int dataMatrixModuleCount = GetCurrentDmModulesFromUiOrAuto();
            int dataMatrixModuleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DataMatrixModuleSizeMm), DefaultDpi));
            double dataMatrixSideLengthMm = dataMatrixModuleCount * (dataMatrixModuleDots * 25.4 / (double)DefaultDpi);

            // 그리드 'Size' 칸에 "한 변(mm)" 표시
            SetRow(RowKey.DM, "DM", dataMatrixPayload, _style.DataMatrixXMm, _style.DataMatrixYMm, Math.Round(dataMatrixSideLengthMm), 1.0, 1.0);

            // 보기 포맷(원하면 0.0으로 소수 1자리)
            var dataMatrixRow = GetRow(RowKey.DM);
            if (dataMatrixRow != null) dataMatrixRow.Cells[ColumnNameSizeMm].Style.Format = "0";

            SetRow(RowKey.Rating, "Rating", _style.RatingText ?? "", _style.RatingXMm, _style.RatingYMm, PositiveOr(_style.RatingFontMm, 2.6), 1, 1);
            SetRow(RowKey.FCCID, "FCC ID", _style.FccIdText ?? "", _style.FccIdXMm, _style.FccIdYMm, PositiveOr(_style.FccIdFontMm, 2.6), 1, 1);
            SetRow(RowKey.ICID, "IC ID", _style.IcIdText ?? "", _style.IcIdXMm, _style.IcIdYMm, PositiveOr(_style.IcIdFontMm, 2.6), 1, 1);
            SetRow(RowKey.Item1, "Item1", _style.Item1Text ?? "", _style.Item1XMm, _style.Item1YMm, PositiveOr(_style.Item1FontMm, 2.6), 1, 1);
            SetRow(RowKey.Item2, "Item2", _style.Item2Text ?? "", _style.Item2XMm, _style.Item2YMm, PositiveOr(_style.Item2FontMm, 2.6), 1, 1);
            SetRow(RowKey.Item3, "Item3", _style.Item3Text ?? "", _style.Item3XMm, _style.Item3YMm, PositiveOr(_style.Item3FontMm, 2.6), 1, 1);
            SetRow(RowKey.Item4, "Item4", _style.Item4Text ?? "", _style.Item4XMm, _style.Item4YMm, PositiveOr(_style.Item4FontMm, 2.6), 1, 1);
            SetRow(RowKey.Item5, "Item5", _style.Item5Text ?? "", _style.Item5XMm, _style.Item5YMm, PositiveOr(_style.Item5FontMm, 2.6), 1, 1);

            SetShow(RowKey.Logo, _style.IsLogoPreviewEnabled, _style.IsLogoPrintEnabled);
            SetShow(RowKey.Brand, _style.IsBrandPreviewEnabled, _style.IsBrandPrintEnabled);
            SetShow(RowKey.Part, _style.IsPartPreviewEnabled, _style.IsPartPrintEnabled);
            SetShow(RowKey.Pb, _style.IsPbPreviewEnabled, _style.IsPbPrintEnabled);
            SetShow(RowKey.HW, _style.IsHardwarePreviewEnabled, _style.IsHardwarePrintEnabled);
            SetShow(RowKey.SW, _style.IsSoftwarePreviewEnabled, _style.IsSoftwarePrintEnabled);
            SetShow(RowKey.LOT, _style.IsLotPreviewEnabled, _style.IsLotPrintEnabled);
            SetShow(RowKey.SN, _style.IsSerialPreviewEnabled, _style.IsSerialPrintEnabled);
            SetShow(RowKey.DM, _style.IsDataMatrixPreviewEnabled, _style.IsDataMatrixPrintEnabled);
            SetShow(RowKey.Rating, _style.IsRatingPreviewEnabled, _style.IsRatingPrintEnabled);
            SetShow(RowKey.FCCID, _style.IsFccIdPreviewEnabled, _style.IsFccIdPrintEnabled);
            SetShow(RowKey.ICID, _style.IsIcIdPreviewEnabled, _style.IsIcIdPrintEnabled);
            SetShow(RowKey.Item1, _style.IsItem1PreviewEnabled, _style.IsItem1PrintEnabled);
            SetShow(RowKey.Item2, _style.IsItem2PreviewEnabled, _style.IsItem2PrintEnabled);
            SetShow(RowKey.Item3, _style.IsItem3PreviewEnabled, _style.IsItem3PrintEnabled);
            SetShow(RowKey.Item4, _style.IsItem4PreviewEnabled, _style.IsItem4PrintEnabled);
            SetShow(RowKey.Item5, _style.IsItem5PreviewEnabled, _style.IsItem5PrintEnabled);

            void SetRow(RowKey key, string fieldLabel, string cellValue, double positionX, double positionY, double sizeValue, double scaleXValue, double scaleYValue)
            {
                var row = LabelDataGridView.Rows.Cast<DataGridViewRow>().First(gridRow => (RowKey)gridRow.Tag == key);
                row.Cells[ColumnNameField].Value = fieldLabel;
                row.Cells[ColumnNameData].Value = cellValue;
                row.Cells[ColumnNamePositionXMm].Value = positionX.ToString("0.###");
                row.Cells[ColumnNamePositionYMm].Value = positionY.ToString("0.###");
                row.Cells[ColumnNameSizeMm].Value = sizeValue.ToString("0.###");
                row.Cells[ColumnNameScaleX].Value = scaleXValue.ToString("0.###");
                row.Cells[ColumnNameScaleY].Value = scaleYValue.ToString("0.###");
            }
            void SetShow(RowKey key, bool isPreviewEnabled, bool isPrintEnabled)
            {
                var row = GetRow(key);
                if (row == null) return;
                row.Cells[ColumnNamePreviewEnabled].Value = isPreviewEnabled;
                row.Cells[ColumnNamePrintEnabled].Value = isPrintEnabled;
            }
        }

        private void GetGridLabelValue()
        {
            if (LabelDataGridView == null || LabelDataGridView.Rows.Count < 4) return;

            bool AsBoolean(object value) => value is bool boolValue && boolValue;

            UpdateFromRow(RowKey.Logo, (x, y, f, data) =>
            {
                _style.LogoXMm = x;
                _style.LogoYMm = y;
                _style.LogoHeightMm = f;
                _style.LogoImagePath = (data ?? "").Trim();

                var logoRow = GetRow(RowKey.Logo);
                _style.LogoScaleXRatio = ReadScaleCell(logoRow, ColumnNameScaleX, 1.0);
                _style.LogoScaleYRatio = ReadScaleCell(logoRow, ColumnNameScaleY, 1.0);
                _style.IsLogoPreviewEnabled = AsBoolean(logoRow?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsLogoPrintEnabled = AsBoolean(logoRow?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Brand, (x, y, f, data) => {
                _style.BrandXMm = x; _style.BrandYMm = y; _style.BrandFontMm = f; _style.BrandText = data ?? "";
                var brandRow = GetRow(RowKey.Brand);
                _style.IsBrandPreviewEnabled = AsBoolean(brandRow?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsBrandPrintEnabled = AsBoolean(brandRow?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Part, (x, y, f, data) => {
                _style.PartXMm = x; _style.PartYMm = y; _style.PartFontMm = f; _style.PartText = data ?? "";
                var partRow = GetRow(RowKey.Part);
                _style.IsPartPreviewEnabled = AsBoolean(partRow?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsPartPrintEnabled = AsBoolean(partRow?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Pb, (x, y, f, data) => {
                _style.BadgeXMm = x; _style.BadgeYMm = y; _style.BadgeDiameterMm = f;
                var leadFreeRow = GetRow(RowKey.Pb);
                _style.IsPbPreviewEnabled = AsBoolean(leadFreeRow?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsPbPrintEnabled = AsBoolean(leadFreeRow?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.HW, (x, y, f, data) =>
            {
                _style.HardwareXMm = x; _style.HardwareYMm = y; _style.HardwareFontMm = f; _style.HardwareText = data;
                _style.IsHardwarePreviewEnabled = AsBoolean(GetRow(RowKey.HW)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsHardwarePrintEnabled = AsBoolean(GetRow(RowKey.HW)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.SW, (x, y, f, data) =>
            {
                _style.SoftwareXMm = x; _style.SoftwareYMm = y; _style.SoftwareFontMm = f; _style.SoftwareText = data;
                _style.IsSoftwarePreviewEnabled = AsBoolean(GetRow(RowKey.SW)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsSoftwarePrintEnabled = AsBoolean(GetRow(RowKey.SW)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.LOT, (x, y, f, data) =>
            {
                _style.LotXMm = x; _style.LotYMm = y; _style.LotFontMm = f; _style.LotText = (data ?? "").Trim();
                _style.IsLotPreviewEnabled = AsBoolean(GetRow(RowKey.LOT)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsLotPrintEnabled = AsBoolean(GetRow(RowKey.LOT)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.SN, (x, y, f, data) =>
            {
                _style.SerialXMm = x; _style.SerialYMm = y; _style.SerialFontMm = f;
                _style.SerialText = (data ?? "").Trim();
                _style.IsSerialPreviewEnabled = AsBoolean(GetRow(RowKey.SN)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsSerialPrintEnabled = AsBoolean(GetRow(RowKey.SN)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.DM, (x, y, sideMmTarget, data) =>
            {
                _style.DataMatrixXMm = x;
                _style.DataMatrixYMm = y;

                double targetSideLengthMm = Math.Max(1.0, Math.Round(sideMmTarget));
                var autoPickResult = AutoPickDmByTarget(targetSideLengthMm, DefaultDpi);
                int autoPickModuleCount = autoPickResult.M;
                int autoPickModuleHeightDots = autoPickResult.h;
                double dataMatrixSideLengthMm = autoPickResult.dataMatrixSideLengthMm;

                _style.DataMatrixModuleSizeMm = autoPickModuleHeightDots * 25.4 / (double)DefaultDpi;

                var dataMatrixRow = GetRow(RowKey.DM);
                if (dataMatrixRow != null)
                {
                    dataMatrixRow.Cells[ColumnNameScaleX].Value = autoPickModuleCount.ToString("0");  // DM 열
                    dataMatrixRow.Cells[ColumnNameScaleY].Value = autoPickModuleCount.ToString("0");  // DM 행
                    dataMatrixRow.Cells[ColumnNameSizeMm].Value = Math.Round(dataMatrixSideLengthMm).ToString("0"); // 정수 표시
                    dataMatrixRow.Cells[ColumnNameScaleX].Style.Format = "0";
                    dataMatrixRow.Cells[ColumnNameScaleY].Style.Format = "0";
                    dataMatrixRow.Cells[ColumnNameSizeMm].Style.Format = "0";       // 포맷 정수
                }

                _style.IsDataMatrixPreviewEnabled = AsBoolean(GetRow(RowKey.DM)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsDataMatrixPrintEnabled = AsBoolean(GetRow(RowKey.DM)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Rating, (x, y, f, data) => {
                _style.RatingXMm = x; _style.RatingYMm = y; _style.RatingFontMm = f;
                _style.RatingText = (data ?? "").Trim();
                _style.IsRatingPreviewEnabled = AsBoolean(GetRow(RowKey.Rating)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsRatingPrintEnabled = AsBoolean(GetRow(RowKey.Rating)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.FCCID, (x, y, f, data) => {
                _style.FccIdXMm = x; _style.FccIdYMm = y; _style.FccIdFontMm = f;
                _style.FccIdText = (data ?? "").Trim();
                _style.IsFccIdPreviewEnabled = AsBoolean(GetRow(RowKey.FCCID)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsFccIdPrintEnabled = AsBoolean(GetRow(RowKey.FCCID)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.ICID, (x, y, f, data) => {
                _style.IcIdXMm = x; _style.IcIdYMm = y; _style.IcIdFontMm = f;
                _style.IcIdText = (data ?? "").Trim();
                _style.IsIcIdPreviewEnabled = AsBoolean(GetRow(RowKey.ICID)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsIcIdPrintEnabled = AsBoolean(GetRow(RowKey.ICID)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Item1, (x, y, f, data) => {
                _style.Item1XMm = x; _style.Item1YMm = y; _style.Item1FontMm = f;
                _style.Item1Text = (data ?? "").Trim();
                _style.IsItem1PreviewEnabled = AsBoolean(GetRow(RowKey.Item1)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsItem1PrintEnabled = AsBoolean(GetRow(RowKey.Item1)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Item2, (x, y, f, data) => {
                _style.Item2XMm = x; _style.Item2YMm = y; _style.Item2FontMm = f;
                _style.Item2Text = (data ?? "").Trim();
                _style.IsItem2PreviewEnabled = AsBoolean(GetRow(RowKey.Item2)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsItem2PrintEnabled = AsBoolean(GetRow(RowKey.Item2)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Item3, (x, y, f, data) => {
                _style.Item3XMm = x; _style.Item3YMm = y; _style.Item3FontMm = f;
                _style.Item3Text = (data ?? "").Trim();
                _style.IsItem3PreviewEnabled = AsBoolean(GetRow(RowKey.Item3)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsItem3PrintEnabled = AsBoolean(GetRow(RowKey.Item3)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Item4, (x, y, f, data) => {
                _style.Item4XMm = x; _style.Item4YMm = y; _style.Item4FontMm = f;
                _style.Item4Text = (data ?? "").Trim();
                _style.IsItem4PreviewEnabled = AsBoolean(GetRow(RowKey.Item4)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsItem4PrintEnabled = AsBoolean(GetRow(RowKey.Item4)?.Cells[ColumnNamePrintEnabled].Value);
            });
            UpdateFromRow(RowKey.Item5, (x, y, f, data) => {
                _style.Item5XMm = x; _style.Item5YMm = y; _style.Item5FontMm = f;
                _style.Item5Text = (data ?? "").Trim();
                _style.IsItem5PreviewEnabled = AsBoolean(GetRow(RowKey.Item5)?.Cells[ColumnNamePreviewEnabled].Value);
                _style.IsItem5PrintEnabled = AsBoolean(GetRow(RowKey.Item5)?.Cells[ColumnNamePrintEnabled].Value);
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
                    row.Cells[ColumnNameSequence].Value = (i + 1).ToString();
            }
        }

        private void RefreshDmDataCell()
        {
            var row = GetRow(RowKey.DM);
            if (row != null)
                row.Cells[ColumnNameData].Value = BuildQrPayloadFromGrid();
        }

        // 로고 셀 더블클릭
        private void LabelGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = LabelDataGridView.Rows[e.RowIndex];
            var colName = LabelDataGridView.Columns[e.ColumnIndex].Name;

            if ((RowKey)row.Tag == RowKey.Logo && colName == ColumnNameData)
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

                        row.Cells[ColumnNameData].Value = toStore;
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
                if ((RowKey)row.Tag == RowKey.Logo && LabelDataGridView.Columns[e.ColumnIndex].Name == ColumnNameData)
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
            double x = ReadDoubleCell(row, ColumnNamePositionXMm, 0);
            double y = ReadDoubleCell(row, ColumnNamePositionYMm, 0);
            double f = ReadDoubleCell(row, ColumnNameSizeMm, 2.6);
            string data = ReadStringCell(row, ColumnNameData, "");
            apply(x, y, f, data);
        }

        private string GetGridText(RowKey key, string fallback)
        {
            var row = GetRow(key);
            var s = ReadStringCell(row, ColumnNameData, null);
            return string.IsNullOrWhiteSpace(s) ? fallback : s;
        }

        private bool IsImmediateApplyColumn(string colName)
        {
            return colName == ColumnNamePositionXMm || colName == ColumnNamePositionYMm || colName == ColumnNameSizeMm
                || colName == ColumnNameScaleX || colName == ColumnNameScaleY
                || colName == ColumnNameData
                || colName == ColumnNamePreviewEnabled || colName == ColumnNamePrintEnabled;
        }

        private void CommitAndRefreshPreview(string colName)
        {
            if (_suppressPreview) return;
            GetGridLabelValue();
            Preview.Invalidate();
        }
    }
}
