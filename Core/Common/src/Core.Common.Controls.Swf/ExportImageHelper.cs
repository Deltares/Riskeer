﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf
{
    public static class ExportImageHelper
    {
        private static readonly Dictionary<string, ImageFormat> ImageFormats = new Dictionary<string, ImageFormat>
        {
            {
                "png", ImageFormat.Png
            },
            {
                "jpg", ImageFormat.Jpeg
            },
            {
                "bmp", ImageFormat.Bmp
            },
            {
                "gif", ImageFormat.Gif
            },
            {
                "emf", ImageFormat.Emf
            },
            {
                "tiff", ImageFormat.Tiff
            },
        };

        public static void Export(Image image, string filePath, string imageType, double factor = 1)
        {
            if (image == null)
            {
                return;
            }

            var imageToWrite = CreateResizedImage(image, (int) (image.Width*factor), (int) (image.Height*factor));
            if (imageToWrite == null)
            {
                return;
            }

            imageToWrite.Save(filePath, ImageFormats[imageType.ToLower()]);
        }

        public static void ExportWithDialog(Image image)
        {
            if (image == null)
            {
                return;
            }

            var filter = GetFileFormatFilter(ImageFormats);

            var saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = Resources.ExportImageHelper_ExportWithDialog_Export_as_image
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var imageResolutionDialog = new ImageResolutionDialog
            {
                BaseImage = image
            };

            if (imageResolutionDialog.ShowModal() != DialogResult.OK)
            {
                return;
            }

            var factor = imageResolutionDialog.Resolution/100;
            Export(image, saveFileDialog.FileName, ImageFormats.Keys.ElementAt(saveFileDialog.FilterIndex), factor);
        }

        public static Bitmap CreateResizedImage(Image image, int width, int height)
        {
            //a holder for the result
            var result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (var graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        private static string GetFileFormatFilter(Dictionary<string, ImageFormat> imageFormats)
        {
            var filter = new StringBuilder();

            foreach (var imageFormatExt in imageFormats.Keys)
            {
                filter.AppendFormat("{0} files (*.{0})|", imageFormatExt);
                filter.AppendFormat("*.{0}|", imageFormatExt);
            }
            filter.Append("All files (*.*)|*.*");
            return filter.ToString();
        }
    }
}