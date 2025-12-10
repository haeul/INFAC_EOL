using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DHSTesterXL.FormProduct;

namespace DHSTesterXL
{
    public class LabelStyle
    {
        // 라벨 캔버스(mm) ─ 프리뷰 스케일의 기준
        public double LabelWmm { get; set; } = 60.0;
        public double LabelHmm { get; set; } = 15.0;

        // 프리뷰 전용 비주얼 옵션
        public float CornerRadiusPx { get; set; } = 10f;  // 모서리 라운드(픽셀)

        // ★ 텍스트 SSOT (기본값은 원하는 초기값으로)
        public string PartText { get; set; } = "82657-DC000";
        public string HWText { get; set; } = "1.00";
        public string SWText { get; set; } = "2.52";
        public string LOTText { get; set; } = "240";
        public string SerialText { get; set; } = "1234";

        public LabelStyle Clone() => (LabelStyle)this.MemberwiseClone();

        // 고정 요소(로고/브랜드/품번)
        public double LogoX { get; set; } = 2.0;
        public double LogoY { get; set; } = 2.0;
        public double LogoW { get; set; } = 14.0;
        public double LogoH { get; set; } = 5.0;

        public double BrandX { get; set; } = 17.0;
        public double BrandY { get; set; } = 2.0;
        public double BrandFont { get; set; } = 2.8; // 글자 크기
        public string BrandText { get; set; } = "HYUNDAI KIA MOTORS";

        public double PartY { get; set; } = 6.0;
        public double PartFont { get; set; } = 4.5;

        // 가변 요소(HW/SW/LOT) — UI로 조정
        public double HWx { get; set; } = 4.0;
        public double HWy { get; set; } = 12.0;
        public double HWfont { get; set; } = 2.6;

        public double SWx { get; set; } = 18.0;
        public double SWy { get; set; } = 12.0;
        public double SWfont { get; set; } = 2.6;

        public double LOTx { get; set; } = 34.0;
        public double LOTy { get; set; } = 12.0;
        public double LOTfont { get; set; } = 2.6;

        // Pb 배지(프리뷰에만 표시)
        public double BadgeDiameter { get; set; } = 5.0;
        public double BadgeMargin { get; set; } = 1.0;

        // Grid
        public BindingList<LabelRow> Items { get; set; } = new BindingList<LabelRow>();
    }
    // 라벨 편집용 그리드 행 타입
    public enum LabelDataType { Text, DataMatrix }

    public class LabelRow
    {
        public int No { get; set; }              // 순번(표시용)
        public LabelDataType Type { get; set; } = LabelDataType.Text;
        public double Xmm { get; set; }              // X 좌표(mm)
        public double Ymm { get; set; }              // Y 좌표(mm)
        public int RotDeg { get; set; } = 0;         // 회전(0/90/180/270)
        public double SizeMm { get; set; } = 2.6;       // 텍스트: 폰트높이(mm), DM: 모듈(mm)
        public double ScaleX { get; set; } = 1.0;       // X 비율
        public double ScaleY { get; set; } = 1.0;       // Y 비율
        public string Data { get; set; } = "";
    }
}

