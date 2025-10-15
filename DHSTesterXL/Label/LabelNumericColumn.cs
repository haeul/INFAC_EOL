using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DHSTesterXL
{
    // ───────────────── Column ─────────────────
    public class LabelNumericColumn : DataGridViewColumn
    {
        // 컬럼 단위 기본 설정(편집 컨트롤에 그대로 적용)
        public int DecimalPlaces { get; set; } = 1;
        public decimal Increment { get; set; } = 0.1M;
        public decimal Minimum { get; set; } = 0M;
        public decimal Maximum { get; set; } = 100M;

        public LabelNumericColumn()
            : base(new DataGridViewNumericUpDownCell()) { }

        public override object Clone()
        {
            var c = (LabelNumericColumn)base.Clone();
            c.DecimalPlaces = DecimalPlaces;
            c.Increment = Increment;
            c.Minimum = Minimum;
            c.Maximum = Maximum;
            return c;
        }
    }

    // ───────────────── Cell ─────────────────
    public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell
    {
        public DataGridViewNumericUpDownCell()
        {
            Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public override Type EditType => typeof(DataGridViewNumericUpDownEditingControl);
        public override Type ValueType => typeof(decimal);
        public override object DefaultNewRowValue => 0M;

        // 보기용 문자열 포맷
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
            TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            string fmt = (cellStyle?.Format) ?? "0.###";
            if (value is decimal d) return d.ToString(fmt);
            if (value is double db) return ((decimal)db).ToString(fmt);
            return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle cellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, cellStyle);

            var nud = DataGridView.EditingControl as DataGridViewNumericUpDownEditingControl;
            var col = OwningColumn as LabelNumericColumn;
            if (nud != null && col != null)
            {
                // 컬럼 설정 → 편집 컨트롤에 적용
                nud.DecimalPlaces = col.DecimalPlaces;
                nud.Increment = col.Increment;
                nud.Minimum = col.Minimum;
                nud.Maximum = col.Maximum;
            }

            // 현재 셀 값 → NUD
            decimal cur = 0M;
            var raw = this.Value ?? initialFormattedValue;
            if (raw is decimal d) cur = d;
            else if (raw is double dd) cur = (decimal)dd;
            else if (raw is string s && decimal.TryParse(s, out var parsed)) cur = parsed;

            if (nud != null)
            {
                if (cur < nud.Minimum) cur = nud.Minimum;
                if (cur > nud.Maximum) cur = nud.Maximum;
                nud.Value = cur;
            }
        }
    }

    // ───────────────── EditingControl ─────────────────
    public class DataGridViewNumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        public DataGridViewNumericUpDownEditingControl()
        {
            BorderStyle = BorderStyle.FixedSingle;
            ThousandsSeparator = false;
            ValueChanged += (_, __) =>
            {
                EditingControlValueChanged = true;
                EditingControlDataGridView?.NotifyCurrentCellDirty(true);
            };
        }

        public DataGridView EditingControlDataGridView { get; set; }
        public object EditingControlFormattedValue
        {
            get => Value.ToString();
            set { if (value is string s && decimal.TryParse(s, out var d)) Value = Clamp(d); }
        }
        public int EditingControlRowIndex { get; set; }
        public bool EditingControlValueChanged { get; set; }
        public Cursor EditingPanelCursor => Cursors.IBeam;
        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle style)
        {
            Font = style.Font;
            ForeColor = style.ForeColor;
            BackColor = style.BackColor;
        }

        public void PrepareEditingControlForEdit(bool selectAll) { /* no-op */ }
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => Value.ToString();

        // 화살표/페이지키는 NUD가 처리
        public bool EditingControlWantsInputKey(Keys keyData, bool gridWants)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.PageDown:
                    return true;
                default:
                    return !gridWants;
            }
        }

        private decimal Clamp(decimal v) => Math.Min(Math.Max(v, Minimum), Maximum);
    }
}
