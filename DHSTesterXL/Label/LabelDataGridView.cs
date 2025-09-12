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
            LabelDataGridView.AllowUserToAddRows = false;
            LabelDataGridView.AllowUserToDeleteRows = false;
            LabelDataGridView.RowHeadersVisible = false;
            LabelDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            LabelDataGridView.MultiSelect = false;
            LabelDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

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

            // (회전 컬럼 완전 제거)

            // ── 행(9행) : 미리 생성 + Tag 지정
            int rLogo = LabelDataGridView.Rows.Add();
            int rBrand = LabelDataGridView.Rows.Add();
            int rPart = LabelDataGridView.Rows.Add();
            int rPb = LabelDataGridView.Rows.Add();
            int rHW = LabelDataGridView.Rows.Add();
            int rSW = LabelDataGridView.Rows.Add();
            int rLOT = LabelDataGridView.Rows.Add();
            int rSN = LabelDataGridView.Rows.Add();
            int rQR = LabelDataGridView.Rows.Add();

            LabelDataGridView.Rows[rLogo].Tag = RowKey.Logo;
            LabelDataGridView.Rows[rBrand].Tag = RowKey.Brand;
            LabelDataGridView.Rows[rPart].Tag = RowKey.Part;
            LabelDataGridView.Rows[rPb].Tag = RowKey.Pb;
            LabelDataGridView.Rows[rHW].Tag = RowKey.HW;
            LabelDataGridView.Rows[rSW].Tag = RowKey.SW;
            LabelDataGridView.Rows[rLOT].Tag = RowKey.LOT;
            LabelDataGridView.Rows[rSN].Tag = RowKey.SN;
            LabelDataGridView.Rows[rQR].Tag = RowKey.QR;

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

            var rowQR = LabelDataGridView.Rows[rQR];
            rowQR.Cells[COL_FIELD].Value = "QR";
            rowQR.Cells[COL_DATA].ReadOnly = true; // 자동 생성

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

            var qrData = BuildQrPayloadFromGrid();
            SetRow(RowKey.QR, "QR", qrData, _style.QRx, _style.QRy, Math.Max(0.1, _style.QRModuleMm), 1.0, 1.0);

            SetShow(RowKey.Logo, _style.ShowLogoPreview, _style.ShowLogoPrint);
            SetShow(RowKey.Brand, _style.ShowBrandPreview, _style.ShowBrandPrint);
            SetShow(RowKey.Part, _style.ShowPartPreview, _style.ShowPartPrint);
            SetShow(RowKey.Pb, _style.ShowPbPreview, _style.ShowPbPrint);
            SetShow(RowKey.HW, _style.ShowHWPreview, _style.ShowHWPrint);
            SetShow(RowKey.SW, _style.ShowSWPreview, _style.ShowSWPrint);
            SetShow(RowKey.LOT, _style.ShowLOTPreview, _style.ShowLOTPrint);
            SetShow(RowKey.SN, _style.ShowSNPreview, _style.ShowSNPrint);
            SetShow(RowKey.QR, _style.ShowQRPreview, _style.ShowQRPrint);

            void SetRow(RowKey key, string name, string data, double x, double y, double size, double xs, double ys)
            {
                var row = LabelDataGridView.Rows.Cast<DataGridViewRow>().First(r => (RowKey)r.Tag == key);
                row.Cells[COL_FIELD].Value = name;
                row.Cells[COL_DATA].Value = data;
                row.Cells[COL_X].Value = x.ToString("0.###");
                row.Cells[COL_Y].Value = y.ToString("0.###");
                row.Cells[COL_SIZE].Value = size.ToString("0.###");
                row.Cells[COL_XSCALE].Value = xs.ToString("0.###");
                row.Cells[COL_YSCALE].Value = ys.ToString("0.###");
            }
            void SetShow(RowKey key, bool showPreview, bool showPrint)
            {
                var row = GetRow(key);
                if (row == null) return;
                row.Cells[COL_SHOW_PREVIEW].Value = showPreview;
                row.Cells[COL_SHOW_PRINT].Value = showPrint;
            }
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
            UpdateFromRow(RowKey.QR, (x, y, f, data) =>
            {
                _style.QRx = x; _style.QRy = y; _style.QRModuleMm = Math.Max(0.1, f);
                _style.ShowQRPreview = B(GetRow(RowKey.QR)?.Cells[COL_SHOW_PREVIEW].Value);
                _style.ShowQRPrint = B(GetRow(RowKey.QR)?.Cells[COL_SHOW_PRINT].Value);
            });

            RefreshQrDataCell();
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

        private void RefreshQrDataCell()
        {
            var row = GetRow(RowKey.QR);
            if (row != null)
                row.Cells[COL_DATA].Value = BuildQrPayloadFromGrid();
        }
    }
}
