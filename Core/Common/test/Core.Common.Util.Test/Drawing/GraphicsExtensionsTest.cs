// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common.TestUtil;
using Core.Common.Util.Drawing;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Drawing
{
    [TestFixture]
    public class GraphicsExtensionsTest
    {
        [Test]
        public void DrawImageTransparent_GraphicsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => GraphicsExtensions.DrawImageTransparent(null, Resources.Black2x2, 1, 1, 0.4f);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("g", paramName);
        }

        [Test]
        public void DrawImageTransparent_ImageNull_ThrowArgumentNullException()
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap target = Resources.Black2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                TestDelegate call = () => Graphics.FromImage(target).DrawImageTransparent(null, 0, 0, 0.4f);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("image", paramName);
            }
        }

        [Test]
        [TestCase(1.0f)]
        [TestCase(3.0f)]
        public void DrawImageTransparent_DrawWithOpacity1OrGreater_TargetShouldBeIdenticalToSource(float opacity)
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap image = Resources.TestImage2x2.Clone(rect2By2, imageFormat))
            using (Bitmap target = Resources.Black2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, opacity);

                // Assert
                TestHelper.AssertImagesAreEqual(image, target);
            }
        }

        [Test]
        [TestCase(0f)]
        [TestCase(-3f)]
        public void DrawImageTransparent_DrawWithOpacity0OrLess_TargetShouldBeUnaffected(float opacity)
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap image = Resources.TestImage2x2.Clone(rect2By2, imageFormat))
            using (Bitmap target = Resources.Black2x2.Clone(rect2By2, imageFormat))
            using (Bitmap expectedtarget = Resources.Black2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, opacity);

                // Assert
                TestHelper.AssertImagesAreEqual(expectedtarget, target);
            }
        }

        [Test]
        public void DrawImageTransparent_DrawWithOpacity50Percent_TargetShouldBeUpdated()
        {
            // Setup
            var rect2By2 = new RectangleF(0f, 0f, 2f, 2f);
            const PixelFormat imageFormat = PixelFormat.Format32bppArgb;

            using (Bitmap image = Resources.TestImage2x2.Clone(rect2By2, imageFormat))
            using (Bitmap target = Resources.Black2x2.Clone(rect2By2, imageFormat))
            using (Bitmap expectedtarget = Resources.TestImageHalfOpacityOnBlack2x2.Clone(rect2By2, imageFormat))
            {
                // Call
                Graphics.FromImage(target).DrawImageTransparent(image, 0, 0, 0.5f);

                // Assert
                TestHelper.AssertImagesAreEqual(expectedtarget, target);
            }
        }
    }
}