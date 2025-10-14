using System.ComponentModel;

namespace DHSTesterXL
{
    /// <summary>
    /// 라벨 스타일 SSOT (모든 수치 mm 기준)
    /// - 프리뷰 스케일/렌더링과 ZPL(인쇄) 모두 여기 값을 참조
    /// - 요소별 표시 플래그를 "미리보기/인쇄"로 분리(Show*Preview / Show*Print)
    /// </summary>
    public class LabelStyle
    {
        // ───────── 캔버스(mm) / 프리뷰 비주얼 ─────────
        public double LabelWmm { get; set; } = 60.0;
        public double LabelHmm { get; set; } = 15.0;

        public float CornerRadiusPx { get; set; } = 10f;  // 프리뷰용 라운드 모서리(px)

        // ───────── 텍스트/로고 SSOT ─────────
        // 로고(동적 이미지)
        public string LogoImagePath { get; set; } = "D:\\INFAC\\DHS_EOL_V3\\DHSTesterXL\\Images";      // 로고 파일 경로
        public double LogoX { get; set; } = 9.0;
        public double LogoY { get; set; } = 0.0;
        // public double LogoW { get; set; } = 10.0; // 로고 기본 가로(mm)
        public double LogoH { get; set; } = 7.0;  // 로고 size
        public double LogoScaleX { get; set; } = 1.0;
        public double LogoScaleY { get; set; } = 1.0;
        public bool LogoKeepAspect { get; set; } = false;   // 비율 유지
        public string LogoZplName { get; set; } = "CE_logo.png"; // (옵션) ZPL 저장명

        // 고정 텍스트
        public string PartText { get; set; } = "82657-DC000";
        public string HWText { get; set; } = "1.00";
        public string SWText { get; set; } = "2.52";
        public string LOTText { get; set; } = "Lot NO : 240";
        public string SerialText { get; set; } = "S/N : 1234";

        // ───────── 요소 레이아웃(로고/브랜드/품번) ─────────
        public double BrandX { get; set; } = 17.0;
        public double BrandY { get; set; } = 2.0;
        public double BrandFont { get; set; } = 2.8;                // 글자 크기(mm)
        public string BrandText { get; set; } = "HYUNDAI KIA MOTORS";
        public double PartX { get; set; } = 18.0;
        public double PartY { get; set; } = 6.0;
        public double PartFont { get; set; } = 4.5;

        // ───────── 가변 요소 레이아웃(HW/SW/LOT) ─────────
        public double HWx { get; set; } = 2.0;
        public double HWy { get; set; } = 12.0;
        public double HWfont { get; set; } = 2.6;

        public double SWx { get; set; } = 15.0;
        public double SWy { get; set; } = 12.0;
        public double SWfont { get; set; } = 2.6;

        public double LOTx { get; set; } = 28.0;
        public double LOTy { get; set; } = 12.0;
        public double LOTfont { get; set; } = 2.6;

        public double SNx { get; set; } = 45.0;
        public double SNy { get; set; } = 12.0;
        public double SNfont { get; set; } = 2.6;

        // DM (모듈 크기를 mm로 제어)
        public double DMx { get; set; } = 1.0;
        public double DMy { get; set; } = 1.0;
        public double DMModuleMm { get; set; } = 0.5; // 모듈(셀) 한 변의 mm (203dpi 기준 0.5~1.0 권장)
        public int DMScale { get; set; } = 3; // 1~10, 기본 3

        // Pb 절대 좌표(좌상단 기준)
        public double BadgeX { get; set; } = 55.0;
        public double BadgeY { get; set; } = 1.0;
        public double BadgeDiameter { get; set; } = 4.0;

        // ───── Rating / FCC / IC ─────
        public string RatingText { get; set; } = "Rating:12V, 0.5A";
        public double RatingX { get; set; } = 24.0;
        public double RatingY { get; set; } = 0.8;
        public double RatingFont { get; set; } = 2.6;

        public string FCCIDText { get; set; } = "FCC ID:";
        public double FCCIDX { get; set; } = 2.0;
        public double FCCIDY { get; set; } = 9.6;
        public double FCCIDFont { get; set; } = 2.6;

        public string ICIDText { get; set; } = "IC ID:";
        public double ICIDX { get; set; } = 2.0;
        public double ICIDY { get; set; } = 11.0;
        public double ICIDFont { get; set; } = 2.6;

        // ───── 고정 텍스트 항목 (Item1~5) ─────
        public string Item1Text { get; set; } = "Item1:";
        public double Item1X { get; set; } = 2.0;
        public double Item1Y { get; set; } = 11.0;
        public double Item1Font { get; set; } = 2.6;

        public string Item2Text { get; set; } = "Item2:";
        public double Item2X { get; set; } = 2.0;
        public double Item2Y { get; set; } = 11.0;
        public double Item2Font { get; set; } = 2.6;

        public string Item3Text { get; set; } = "Item3:";
        public double Item3X { get; set; } = 2.0;
        public double Item3Y { get; set; } = 11.0;
        public double Item3Font { get; set; } = 2.6;

        public string Item4Text { get; set; } = "Item4:";
        public double Item4X { get; set; } = 2.0;
        public double Item4Y { get; set; } = 11.0;
        public double Item4Font { get; set; } = 2.6;

        public string Item5Text { get; set; } = "Item5:";
        public double Item5X { get; set; } = 2.0;
        public double Item5Y { get; set; } = 11.0;
        public double Item5Font { get; set; } = 2.6;


        // ───────── 요소별 표시 플래그(미리보기/인쇄 분리) ─────────
        // 기본값 true: 과거 JSON에도 안전하게 로드됨(미존재시 기본값)
        public bool ShowLogoPreview { get; set; } = true;
        public bool ShowLogoPrint { get; set; } = true;

        public bool ShowBrandPreview { get; set; } = true;
        public bool ShowBrandPrint { get; set; } = true;

        public bool ShowPartPreview { get; set; } = true;
        public bool ShowPartPrint { get; set; } = true;

        public bool ShowPbPreview { get; set; } = true;
        public bool ShowPbPrint { get; set; } = true;

        public bool ShowHWPreview { get; set; } = true;
        public bool ShowHWPrint { get; set; } = true;

        public bool ShowSWPreview { get; set; } = true;
        public bool ShowSWPrint { get; set; } = true;

        public bool ShowLOTPreview { get; set; } = true;
        public bool ShowLOTPrint { get; set; } = true;

        public bool ShowSNPreview { get; set; } = true;
        public bool ShowSNPrint { get; set; } = true;

        // ───────── DM ─────────
        public bool ShowDMPreview { get; set; } = true;
        public bool ShowDMPrint { get; set; } = true;

        // ───────── Rating / FCC /IC ─────────
        public bool ShowRatingPreview { get; set; } = true;
        public bool ShowRatingPrint { get; set; } = true;

        public bool ShowFCCIDPreview { get; set; } = true;
        public bool ShowFCCIDPrint { get; set; } = true;

        public bool ShowICIDPreview { get; set; } = true;
        public bool ShowICIDPrint { get; set; } = true;

        // ───── 고정 텍스트 항목 (Item1~5) ─────
        public bool ShowItem1Preview { get; set; } = true;
        public bool ShowItem1Print { get; set; } = true;

        public bool ShowItem2Preview { get; set; } = true;
        public bool ShowItem2Print { get; set; } = true;

        public bool ShowItem3Preview { get; set; } = true;
        public bool ShowItem3Print { get; set; } = true;

        public bool ShowItem4Preview { get; set; } = true;
        public bool ShowItem4Print { get; set; } = true;

        public bool ShowItem5Preview { get; set; } = true;
        public bool ShowItem5Print { get; set; } = true;

        // 얕은 복사(새 필드도 자동 포함)
        public LabelStyle Clone() => (LabelStyle)this.MemberwiseClone();

        // ───────── (선택) 그리드 바인딩용 아이템 ─────────
        public BindingList<LabelRow> Items { get; set; } = new BindingList<LabelRow>();
    }

    // 라벨 편집용 그리드 행 타입(현재 구조 유지)
    public enum LabelDataType { Text, DataMatrix }

    public class LabelRow
    {
        public int No { get; set; }                   // 순번(표시용)
        public LabelDataType Type { get; set; } = LabelDataType.Text;

        public double Xmm { get; set; }              // X 좌표(mm)
        public double Ymm { get; set; }              // Y 좌표(mm)
        // public int RotDeg { get; set; } = 0;         // 회전(0/90/180/270)

        public double SizeMm { get; set; } = 2.6;    // 텍스트: 폰트높이(mm), DM: 모듈(mm)
        public double ScaleX { get; set; } = 1.0;    // X 비율
        public double ScaleY { get; set; } = 1.0;    // Y 비율

        public string Data { get; set; } = "";       // 텍스트/데이터
    }
}
