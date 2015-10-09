using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DelftTools.Utils
{
    public static class GraphicsUtils
    {
        public static void DrawImageTransparent(Graphics g, Image image, float opacity)
        {
            var width = image.Width;
            var height = image.Height;
            g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0,
                        width, height, GraphicsUnit.Pixel,
                        CalculateOpacityImageAttributes(opacity));
        }

        private static ImageAttributes CalculateOpacityImageAttributes(float opacity)
        {
            var clippedOpacity = (float) Math.Min(1.0, Math.Max(0.0, opacity));
            float[][] ptsArray =
            {
                new float[]
                {
                    1,
                    0,
                    0,
                    0,
                    0
                },
                new float[]
                {
                    0,
                    1,
                    0,
                    0,
                    0
                },
                new float[]
                {
                    0,
                    0,
                    1,
                    0,
                    0
                },
                new float[]
                {
                    0,
                    0,
                    0,
                    clippedOpacity,
                    0
                },
                new float[]
                {
                    0,
                    0,
                    0,
                    0,
                    1
                }
            };
            var clrMatrix = new ColorMatrix(ptsArray);
            var imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return imgAttributes;
        }
    }
}