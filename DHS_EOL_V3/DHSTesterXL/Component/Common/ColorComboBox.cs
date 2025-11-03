using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSCommon
{
    public partial class ColorComboBox : ComboBox
    {
        private static readonly List<Color> webColor = new List<Color>() {
            Color.Transparent,
            Color.Black,
            Color.White,
            Color.DimGray,
            Color.Gray,
            Color.DarkGray,
            Color.Silver,
            Color.LightGray,
            Color.Gainsboro,
            Color.WhiteSmoke,
            Color.Maroon,
            Color.DarkRed,
            Color.Red,
            Color.Brown,
            Color.Firebrick,
            Color.IndianRed,
            Color.Snow,
            Color.LightCoral,
            Color.RosyBrown,
            Color.MistyRose,
            Color.Salmon,
            Color.Tomato,
            Color.DarkSalmon,
            Color.Coral,
            Color.OrangeRed,
            Color.LightSalmon,
            Color.Sienna,
            Color.SeaShell,
            Color.Chocolate,
            Color.SaddleBrown,
            Color.SandyBrown,
            Color.PeachPuff,
            Color.Peru,
            Color.Linen,
            Color.Bisque,
            Color.DarkOrange,
            Color.BurlyWood,
            Color.Tan,
            Color.AntiqueWhite,
            Color.NavajoWhite,
            Color.BlanchedAlmond,
            Color.PapayaWhip,
            Color.Moccasin,
            Color.Orange,
            Color.Wheat,
            Color.OldLace,
            Color.FloralWhite,
            Color.DarkGoldenrod,
            Color.Goldenrod,
            Color.Cornsilk,
            Color.Gold,
            Color.Khaki,
            Color.LemonChiffon,
            Color.PaleGoldenrod,
            Color.DarkKhaki,
            Color.Beige,
            Color.LightGoldenrodYellow,
            Color.Olive,
            Color.Yellow,
            Color.LightYellow,
            Color.Ivory,
            Color.OliveDrab,
            Color.YellowGreen,
            Color.DarkOliveGreen,
            Color.GreenYellow,
            Color.Chartreuse,
            Color.LawnGreen,
            Color.DarkSeaGreen,
            Color.LightGreen,
            Color.ForestGreen,
            Color.LimeGreen,
            Color.PaleGreen,
            Color.DarkGreen,
            Color.Green,
            Color.Lime,
            Color.Honeydew,
            Color.SeaGreen,
            Color.MediumSeaGreen,
            Color.SpringGreen,
            Color.MintCream,
            Color.MediumSpringGreen,
            Color.MediumAquamarine,
            Color.Aquamarine,
            Color.Turquoise,
            Color.LightSeaGreen,
            Color.MediumTurquoise,
            Color.DarkSlateGray,
            Color.PaleTurquoise,
            Color.Teal,
            Color.DarkCyan,
            Color.Aqua,
            Color.Cyan,
            Color.LightCyan,
            Color.Azure,
            Color.DarkTurquoise,
            Color.CadetBlue,
            Color.PowderBlue,
            Color.LightBlue,
            Color.DeepSkyBlue,
            Color.SkyBlue,
            Color.LightSkyBlue,
            Color.SteelBlue,
            Color.AliceBlue,
            Color.DodgerBlue,
            Color.SlateGray,
            Color.LightSlateGray,
            Color.LightSteelBlue,
            Color.CornflowerBlue,
            Color.RoyalBlue,
            Color.MidnightBlue,
            Color.Lavender,
            Color.Navy,
            Color.DarkBlue,
            Color.MediumBlue,
            Color.Blue,
            Color.GhostWhite,
            Color.SlateBlue,
            Color.DarkSlateBlue,
            Color.MediumSlateBlue,
            Color.MediumPurple,
            Color.BlueViolet,
            Color.Indigo,
            Color.DarkOrchid,
            Color.DarkViolet,
            Color.MediumOrchid,
            Color.Thistle,
            Color.Plum,
            Color.Violet,
            Color.Purple,
            Color.DarkMagenta,
            Color.Magenta,
            Color.Fuchsia,
            Color.Orchid,
            Color.MediumVioletRed,
            Color.DeepPink,
            Color.HotPink,
            Color.LavenderBlush,
            Color.PaleVioletRed,
            Color.Crimson,
            Color.Pink,
            Color.LightPink
        };

        private Color _selectedColor;
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set {
                _selectedColor = value;
                if (webColor != null && Items.Count > 0)
                    SelectedIndex = webColor.IndexOf(value);
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public ColorComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            Width = 150;
        }

        public void InitializeComponent()
        {
            // ToArray()는 반환값이 System.Array이기 때문에 object[]로 캐스팅하기 위해서 .OfType<object>().ToArray()를 사용한다.
            Items.AddRange(webColor.OfType<object>().ToArray());
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            _selectedColor = webColor[SelectedIndex];
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();
            if (e.Index >= 0)
            {
                string colorName = GetItemText(this.Items[e.Index]);
                Color color = (Color)this.Items[e.Index];
                Rectangle rectColor = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 2, 2 * (e.Bounds.Height - 5), e.Bounds.Height - 5);
                Rectangle rectName = Rectangle.FromLTRB(rectColor.Right + 2, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, rectColor);
                }
                e.Graphics.DrawRectangle(Pens.Black, rectColor);
                TextRenderer.DrawText(e.Graphics, colorName, this.Font, rectName, this.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}
