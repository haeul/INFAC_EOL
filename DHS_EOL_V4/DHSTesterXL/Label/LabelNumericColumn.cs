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
            var clonedColumn = (LabelNumericColumn)base.Clone();
            clonedColumn.DecimalPlaces = DecimalPlaces;
            clonedColumn.Increment = Increment;
            clonedColumn.Minimum = Minimum;
            clonedColumn.Maximum = Maximum;
            return clonedColumn;
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
            string formatString = (cellStyle?.Format) ?? "0.###";
            if (value is decimal decimalValue) return decimalValue.ToString(formatString);
            if (value is double doubleValue) return ((decimal)doubleValue).ToString(formatString);
            return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle cellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, cellStyle);

            var numericEditor = DataGridView.EditingControl as DataGridViewNumericUpDownEditingControl;
            var col = OwningColumn as LabelNumericColumn;
            if (numericEditor != null && col != null)
            {
                // 컬럼 설정 → 편집 컨트롤에 적용
                numericEditor.DecimalPlaces = col.DecimalPlaces;
                numericEditor.Increment = col.Increment;
                numericEditor.Minimum = col.Minimum;
                numericEditor.Maximum = col.Maximum;
            }

            // 현재 셀 값 → numericEditor
            decimal currentValue = 0M;
            var rawValue = this.Value ?? initialFormattedValue;
            if (rawValue is decimal decimalValue) currentValue = decimalValue;
            else if (rawValue is double doubleValue) currentValue = (decimal)doubleValue;
            else if (rawValue is string s && decimal.TryParse(s, out var parsed)) currentValue = parsed;

            if (numericEditor != null)
            {
                if (currentValue < numericEditor.Minimum) currentValue = numericEditor.Minimum;
                if (currentValue > numericEditor.Maximum) currentValue = numericEditor.Maximum;
                numericEditor.Value = currentValue;
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

        // 화살표/페이지키는 numericEditor가 처리
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

        private decimal Clamp(decimal value) => Math.Min(Math.Max(value, Minimum), Maximum);
    }
}
