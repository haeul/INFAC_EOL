using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GSCommon
{
    /// <summary>
    /// 비트맵 헬퍼
    /// </summary>
    public static class BitmapHelper
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 비트맵 구하기 - GetBitmap(sourceBitmap)

        /// <summary>
        /// 비트맵 구하기
        /// </summary>
        /// <param name="sourceBitmap">소스 비트맵</param>
        /// <returns>비트맵</returns>
        public static Bitmap GetBitmap(Bitmap sourceBitmap)
        {
            Bitmap targetBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb);
            
            using(Graphics graphics = Graphics.FromImage(targetBitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode    = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode      = SmoothingMode.HighQuality;
                
                graphics.DrawImage
                (
                    sourceBitmap,
                    new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                    new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                    GraphicsUnit.Pixel
                );
            }

            return targetBitmap;
        }

        #endregion
        #region 색상 대체하기 - SubstituteColor(sourceBitmap, colorSubstitutionFilter)

        /// <summary>
        /// 색상 대체하기
        /// </summary>
        /// <param name="sourceBitmap">소스 비트맵</param>
        /// <param name="colorSubstitutionFilter">색상 대체 필터</param>
        /// <returns>비트맵</returns>
        public static Bitmap SubstituteColor(Bitmap sourceBitmap, ColorSubstitutionFilter colorSubstitutionFilter)
        {
            BitmapData sourceBitmapData = sourceBitmap.LockBits
            (
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            Bitmap targetBitmap = new Bitmap
            (
                sourceBitmap.Width,
                sourceBitmap.Height,
                PixelFormat.Format32bppArgb
            );

            BitmapData targetBitmapData = targetBitmap.LockBits
            (
                new Rectangle(0, 0, targetBitmap.Width, targetBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            byte[] targetByteArray = new byte[targetBitmapData.Stride * targetBitmapData.Height];

            Marshal.Copy(sourceBitmapData.Scan0, targetByteArray, 0, targetByteArray.Length);

            sourceBitmap.UnlockBits(sourceBitmapData);

            byte sourceRed   = 0;
            byte sourceGreen = 0;
            byte sourceBlue  = 0;
            byte sourceAlpha = 0;

            int targetRed   = 0;
            int targetGreen = 0;
            int targetBlue  = 0;

            byte newRedValue   = colorSubstitutionFilter.TargetColor.R;
            byte newGreenValue = colorSubstitutionFilter.TargetColor.G;
            byte newBlueValue  = colorSubstitutionFilter.TargetColor.B;

            byte redFilter   = colorSubstitutionFilter.SourceColor.R;
            byte greenFilter = colorSubstitutionFilter.SourceColor.G;
            byte blueFilter  = colorSubstitutionFilter.SourceColor.B;

            byte minimumValue = 0;
            byte maximumValue = 255;

            for(int k = 0; k < targetByteArray.Length; k += 4)
            {
                sourceAlpha = targetByteArray[k + 3];

                if(sourceAlpha != 0)
                {
                    sourceBlue  = targetByteArray[k    ];
                    sourceGreen = targetByteArray[k + 1];
                    sourceRed   = targetByteArray[k + 2];

                    if
                    (
                        (sourceBlue  < blueFilter  + colorSubstitutionFilter.ThresholdValue && sourceBlue  > blueFilter  - colorSubstitutionFilter.ThresholdValue) &&
                        (sourceGreen < greenFilter + colorSubstitutionFilter.ThresholdValue && sourceGreen > greenFilter - colorSubstitutionFilter.ThresholdValue) &&
                        (sourceRed   < redFilter   + colorSubstitutionFilter.ThresholdValue && sourceRed   > redFilter   - colorSubstitutionFilter.ThresholdValue)
                    )
                    {
                        targetBlue = blueFilter - sourceBlue + newBlueValue;

                        if(targetBlue > maximumValue)
                        {
                            targetBlue = maximumValue;
                        }
                        else if(targetBlue < minimumValue)
                        {
                            targetBlue = minimumValue;
                        }

                        targetGreen = greenFilter - sourceGreen + newGreenValue;

                        if(targetGreen > maximumValue)
                        {
                            targetGreen = maximumValue;
                        }
                        else if(targetGreen < minimumValue)
                        {
                            targetGreen = minimumValue;
                        }

                        targetRed = redFilter - sourceRed + newRedValue;

                        if(targetRed > maximumValue)
                        {
                            targetRed = maximumValue;
                        }
                        else if(targetRed < minimumValue)
                        {
                            targetRed = minimumValue;
                        }

                        targetByteArray[k    ] = (byte)targetBlue;
                        targetByteArray[k + 1] = (byte)targetGreen;
                        targetByteArray[k + 2] = (byte)targetRed;
                        targetByteArray[k + 3] = sourceAlpha;
                    }
                }
            }

            Marshal.Copy(targetByteArray, 0, targetBitmapData.Scan0, targetByteArray.Length);

            targetBitmap.UnlockBits(targetBitmapData);

            return targetBitmap;
        }

        #endregion
    }
}