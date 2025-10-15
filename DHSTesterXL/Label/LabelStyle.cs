using System.ComponentModel;

namespace DHSTesterXL
{
    /// <summary>
    /// 라벨 스타일 SSOT (모든 수치 mm 기준)
    /// - 프리뷰 스케일/렌더링과 ZPL(인쇄) 모두 여기 값을 참조
    /// - 요소별 표시 플래그를 "미리보기/인쇄"로 분리(Is*PreviewEnabled / Is*PrintEnabled)
    /// </summary>
    public class LabelStyle
    {
        // ───────── 캔버스(mm) / 프리뷰 비주얼 ─────────
        public double LabelWidthMm { get; set; } = 60.0;
        public double LabelHeightMm { get; set; } = 15.0;

        public float CornerRadiusPx { get; set; } = 10f;  // 프리뷰용 라운드 모서리(px)

        // ───────── 텍스트/로고 SSOT ─────────
        // 로고(동적 이미지)
        public string LogoImagePath { get; set; } = "D:\\INFAC\\DHS_EOL_V3\\DHSTesterXL\\Images";      // 로고 파일 경로
        public double LogoXMm { get; set; } = 9.0;
        public double LogoYMm { get; set; } = 0.0;
        // public double LogoW { get; set; } = 10.0; // 로고 기본 가로(mm)
        public double LogoHeightMm { get; set; } = 7.0;  // 로고 size
        public double LogoScaleXRatio { get; set; } = 1.0;
        public double LogoScaleYRatio { get; set; } = 1.0;
        public bool IsLogoAspectRatioLocked { get; set; } = false;   // 비율 유지
        public string LogoZplFileName { get; set; } = "CE_logo.png"; // (옵션) ZPL 저장명

        // 고정 텍스트
        public string PartText { get; set; } = "82657-DC000";
        public string HardwareText { get; set; } = "1.00";
        public string SoftwareText { get; set; } = "2.52";
        public string LotText { get; set; } = "Lot NO : 240";
        public string SerialText { get; set; } = "S/N : 1234";

        // ───────── 요소 레이아웃(로고/브랜드/품번) ─────────
        public double BrandXMm { get; set; } = 17.0;
        public double BrandYMm { get; set; } = 2.0;
        public double BrandFontMm { get; set; } = 2.8;                // 글자 크기(mm)
        public string BrandText { get; set; } = "HYUNDAI KIA MOTORS";
        public double PartXMm { get; set; } = 18.0;
        public double PartYMm { get; set; } = 6.0;
        public double PartFontMm { get; set; } = 4.5;

        // ───────── 가변 요소 레이아웃(HW/SW/LOT) ─────────
        public double HardwareXMm { get; set; } = 2.0;
        public double HardwareYMm { get; set; } = 12.0;
        public double HardwareFontMm { get; set; } = 2.6;

        public double SoftwareXMm { get; set; } = 15.0;
        public double SoftwareYMm { get; set; } = 12.0;
        public double SoftwareFontMm { get; set; } = 2.6;

        public double LotXMm { get; set; } = 28.0;
        public double LotYMm { get; set; } = 12.0;
        public double LotFontMm { get; set; } = 2.6;

        public double SerialXMm { get; set; } = 45.0;
        public double SerialYMm { get; set; } = 12.0;
        public double SerialFontMm { get; set; } = 2.6;

        // DM (모듈 크기를 mm로 제어)
        public double DataMatrixXMm { get; set; } = 1.0;
        public double DataMatrixYMm { get; set; } = 1.0;
        public double DataMatrixModuleSizeMm { get; set; } = 0.5; // 모듈(셀) 한 변의 mm (203dpi 기준 0.5~1.0 권장)
        public int DataMatrixScaleFactor { get; set; } = 3; // 1~10, 기본 3

        // Pb 절대 좌표(좌상단 기준)
        public double BadgeXMm { get; set; } = 55.0;
        public double BadgeYMm { get; set; } = 1.0;
        public double BadgeDiameterMm { get; set; } = 4.0;

        // ───── Rating / FCC / IC ─────
        public string RatingText { get; set; } = "Rating:12V, 0.5A";
        public double RatingXMm { get; set; } = 24.0;
        public double RatingYMm { get; set; } = 0.8;
        public double RatingFontMm { get; set; } = 2.6;

        public string FccIdText { get; set; } = "FCC ID:";
        public double FccIdXMm { get; set; } = 2.0;
        public double FccIdYMm { get; set; } = 9.6;
        public double FccIdFontMm { get; set; } = 2.6;

        public string IcIdText { get; set; } = "IC ID:";
        public double IcIdXMm { get; set; } = 2.0;
        public double IcIdYMm { get; set; } = 11.0;
        public double IcIdFontMm { get; set; } = 2.6;

        // ───── 고정 텍스트 항목 (Item1~5) ─────
        public string Item1Text { get; set; } = "Item1:";
        public double Item1XMm { get; set; } = 2.0;
        public double Item1YMm { get; set; } = 11.0;
        public double Item1FontMm { get; set; } = 2.6;

        public string Item2Text { get; set; } = "Item2:";
        public double Item2XMm { get; set; } = 2.0;
        public double Item2YMm { get; set; } = 11.0;
        public double Item2FontMm { get; set; } = 2.6;

        public string Item3Text { get; set; } = "Item3:";
        public double Item3XMm { get; set; } = 2.0;
        public double Item3YMm { get; set; } = 11.0;
        public double Item3FontMm { get; set; } = 2.6;

        public string Item4Text { get; set; } = "Item4:";
        public double Item4XMm { get; set; } = 2.0;
        public double Item4YMm { get; set; } = 11.0;
        public double Item4FontMm { get; set; } = 2.6;

        public string Item5Text { get; set; } = "Item5:";
        public double Item5XMm { get; set; } = 2.0;
        public double Item5YMm { get; set; } = 11.0;
        public double Item5FontMm { get; set; } = 2.6;


        // ───────── 요소별 표시 플래그(미리보기/인쇄 분리) ─────────
        // 기본값 true: 과거 JSON에도 안전하게 로드됨(미존재시 기본값)
        public bool IsLogoPreviewEnabled { get; set; } = true;
        public bool IsLogoPrintEnabled { get; set; } = true;

        public bool IsBrandPreviewEnabled { get; set; } = true;
        public bool IsBrandPrintEnabled { get; set; } = true;

        public bool IsPartPreviewEnabled { get; set; } = true;
        public bool IsPartPrintEnabled { get; set; } = true;

        public bool IsPbPreviewEnabled { get; set; } = true;
        public bool IsPbPrintEnabled { get; set; } = true;

        public bool IsHardwarePreviewEnabled { get; set; } = true;
        public bool IsHardwarePrintEnabled { get; set; } = true;

        public bool IsSoftwarePreviewEnabled { get; set; } = true;
        public bool IsSoftwarePrintEnabled { get; set; } = true;

        public bool IsLotPreviewEnabled { get; set; } = true;
        public bool IsLotPrintEnabled { get; set; } = true;

        public bool IsSerialPreviewEnabled { get; set; } = true;
        public bool IsSerialPrintEnabled { get; set; } = true;

        // ───────── DM ─────────
        public bool IsDataMatrixPreviewEnabled { get; set; } = true;
        public bool IsDataMatrixPrintEnabled { get; set; } = true;

        // ───────── Rating / FCC /IC ─────────
        public bool IsRatingPreviewEnabled { get; set; } = true;
        public bool IsRatingPrintEnabled { get; set; } = true;

        public bool IsFccIdPreviewEnabled { get; set; } = true;
        public bool IsFccIdPrintEnabled { get; set; } = true;

        public bool IsIcIdPreviewEnabled { get; set; } = true;
        public bool IsIcIdPrintEnabled { get; set; } = true;

        // ───── 고정 텍스트 항목 (Item1~5) ─────
        public bool IsItem1PreviewEnabled { get; set; } = true;
        public bool IsItem1PrintEnabled { get; set; } = true;

        public bool IsItem2PreviewEnabled { get; set; } = true;
        public bool IsItem2PrintEnabled { get; set; } = true;

        public bool IsItem3PreviewEnabled { get; set; } = true;
        public bool IsItem3PrintEnabled { get; set; } = true;

        public bool IsItem4PreviewEnabled { get; set; } = true;
        public bool IsItem4PrintEnabled { get; set; } = true;

        public bool IsItem5PreviewEnabled { get; set; } = true;
        public bool IsItem5PrintEnabled { get; set; } = true;

        // ───────── (선택) 그리드 바인딩용 아이템 ─────────
        public BindingList<LabelRow> Items { get; set; } = new BindingList<LabelRow>();

        // 얕은 복사(새 필드도 자동 포함)
        public LabelStyle Clone() => (LabelStyle)this.MemberwiseClone();
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
