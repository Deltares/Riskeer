// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Core.Common.TestUtil;
using Core.Common.Util.Drawing;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Drawing
{
    [TestFixture]
    public class ImageExtensionsTest
    {
        [Test]
        public void AddOverlayImage_SourceDrawnCompletelyOverTarget_TargetShouldBeTheSameAsSource()
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap image = Resources.TestImage2x2.Clone(rect2By2, imageFormat))
            using (Bitmap target = Resources.Black2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                using (Bitmap result = target.AddOverlayImage(image, 0, 0, 2, 2))
                {
                    // Assert
                    TestHelper.AssertImagesAreEqual(image, result);
                }
            }
        }

        [Test]
        public void AddOverlayImage_SourceDrawnPartiallyOverTarget_TargetPartiallyChanged()
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap target = Resources.TestImage2x2.Clone(rect2By2, imageFormat))
            using (Bitmap image = Resources.Black2x2.Clone(rect2By2, imageFormat))
            using (Bitmap expectedImage = Resources.TestImageWithBlack2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                using (Bitmap result = target.AddOverlayImage(image, 1, 1, 2, 2))
                {
                    // Assert
                    TestHelper.AssertImagesAreEqual(expectedImage, result);
                }

                // 'target' should be unchanged! A new image should have been created.
                TestHelper.AssertImagesAreEqual(Resources.TestImage2x2, target);
            }
        }

        [Test]
        public void AsBitmapImage_ImageFromResources_BitmapImageCreated()
        {
            BitmapImage bitmapImage = Resources.TestImage2x2.AsBitmapImage();

            TestHelper.AssertImagesAreEqual(Resources.TestImage2x2, ConvertBitmapSourceToBitmap(bitmapImage));
        }

        private static Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                var bmpBitmapEncoder = new BmpBitmapEncoder();

                bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                bmpBitmapEncoder.Save(memoryStream);

                return new Bitmap(memoryStream);
            }
        }
    }
}