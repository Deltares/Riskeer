// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.TestUtil;
using Core.Common.Utils.Drawing;
using Core.Common.Utils.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Drawing
{
    [TestFixture]
    public class ImageExtensionsTest
    {
        [Test]
        public void AddOverlayImage_SourceDrawnCompletelyOverTarget_TargetShouldBeTheSameAsSource()
        {
            // Setup
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var image = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var target = Resources.Black2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                using (var result = target.AddOverlayImage(image, 0, 0, 2, 2))
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
            var rect2x2 = new RectangleF(0f, 0f, 2f, 2f);
            var imageFormat = PixelFormat.Format32bppArgb;

            using (var target = Resources.TestImage2x2.Clone(rect2x2, imageFormat))
            using (var image = Resources.Black2x2.Clone(rect2x2, imageFormat))
            using (var expectedImage = Resources.TestImageWithBlack2x2.Clone(rect2x2, imageFormat))
            {
                // Call
                using (var result = target.AddOverlayImage(image, 1, 1, 2, 2))
                {
                    // Assert
                    TestHelper.AssertImagesAreEqual(expectedImage, result);
                }

                // 'target' should be unchanged! A new image should have been created.
                TestHelper.AssertImagesAreEqual(Resources.TestImage2x2, target);
            }
        }
    }
}