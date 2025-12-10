using System.Drawing;

namespace GSCommon
{
    /// <summary>
    /// 색상 대체 필터
    /// </summary>
    public class ColorSubstitutionFilter
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 임계치 값
        /// </summary>
        private int thresholdValue = 10;

        /// <summary>
        /// 소스 색상
        /// </summary>
        private Color sourceColor = Color.White;

        /// <summary>
        /// 타겟 색상
        /// </summary>
        private Color targetColor = Color.White;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 임계치 값 - ThresholdValue

        /// <summary>
        /// 임계치 값
        /// </summary>
        public int ThresholdValue
        {
            get
            {
                return this.thresholdValue;
            }
            set
            {
                this.thresholdValue = value;
            }
        }

        #endregion
        #region 소스 색상 - SourceColor

        /// <summary>
        /// 소스 색상
        /// </summary>
        public Color SourceColor
        {
            get
            {
                return this.sourceColor;
            }
            set
            {
                this.sourceColor = value;
            }
        }

        #endregion
        #region 타겟 색상 - TargetColor

        /// <summary>
        /// 타겟 색상
        /// </summary>
        public Color TargetColor
        {
            get
            {
                return this.targetColor;
            }
            set
            {
                this.targetColor = value;
            }
        }

        #endregion
    }
}