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
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Drawing;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Drawing
{
    [TestFixture]
    public class ColorAccessTest
    {
        [Test]
        public void Create_BitmapNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ColorAccess.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("bitmap", paramName);
        }

        [Test]
        [TestCase(0, 0, 3, 2)]
        [TestCase(0, 0, 2, 3)]
        [TestCase(-1, 0, 2, 2)]
        [TestCase(0, -1, 2, 2)]
        public void Create_RectangleNotFullyWithinImage_ThrowArgumentException(int leftX, int topY, int rightX, int bottomY)
        {
            // Setup
            Rectangle rect = Rectangle.FromLTRB(leftX, topY, rightX, bottomY);

            // Call
            TestDelegate call = () => ColorAccess.Create(Resources.Black2x2, rect);

            // Assert
            const string message = "Toegankelijk gebied moet geheel binnen de afbeelding vallen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("accessibleArea", paramName);
        }

        [Test]
        public void Index_ValidIndices_ReturnColor()
        {
            // Setup
            ColorAccess colorAccess = ColorAccess.Create(Resources.acorn);

            // Call
            Color color1 = colorAccess[0, 0];
            Color color2 = colorAccess[8, 8];
            Color color3 = colorAccess[12, 6];

            // Assert
            Color expectedColor1 = Color.FromArgb(0, 110, 55, 2);
            Color expectedColor2 = Color.FromArgb(255, 137, 69, 18);
            Color expectedColor3 = Color.FromArgb(255, 197, 171, 146);

            Assert.AreEqual(expectedColor1, color1);
            Assert.AreEqual(expectedColor2, color2);
            Assert.AreEqual(expectedColor3, color3);
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(3, 2)]
        [TestCase(2, 3)]
        public void Index_GetAtInvalidIndices_ThrowIndexOutOfRangeException(int x, int y)
        {
            // Setup
            ColorAccess colorAccess = ColorAccess.Create(Resources.Black2x2);

            // Call
            TestDelegate call = () =>
            {
                Color c = colorAccess[x, y];
            };

            // Assert
            var exception = Assert.Throws<IndexOutOfRangeException>(call);
            Assert.AreEqual("Index must be in range x:[0,1], y:[0,1].", exception.Message);
        }

        [Test]
        public void Index_SetAtValidIndices_BufferUpdated()
        {
            // Setup
            using (var image = (Bitmap) Resources.acorn.Clone())
            {
                ColorAccess colorAccess = ColorAccess.Create(image);

                // Call
                colorAccess[8, 8] = Color.AliceBlue;

                // Assert
                const int expectedBufferStartIndex = 544;
                Assert.AreEqual(Color.AliceBlue.B, colorAccess.Buffer.ElementAt(expectedBufferStartIndex));
                Assert.AreEqual(Color.AliceBlue.G, colorAccess.Buffer.ElementAt(expectedBufferStartIndex + 1));
                Assert.AreEqual(Color.AliceBlue.R, colorAccess.Buffer.ElementAt(expectedBufferStartIndex + 2));
                Assert.AreEqual(Color.AliceBlue.A, colorAccess.Buffer.ElementAt(expectedBufferStartIndex + 3));

                // ColorAccess should not be capable of changing the source image:
                TestHelper.AssertImagesAreEqual(Resources.acorn, image);
            }
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(16, 15)]
        [TestCase(15, 16)]
        public void Index_SetAtInvalidIndices_ThrowIndexOutOfRangeException(int x, int y)
        {
            // Setup
            ColorAccess colorAccess = ColorAccess.Create(Resources.acorn);

            // Call
            TestDelegate call = () => colorAccess[x, y] = Color.AliceBlue;

            // Assert
            var exception = Assert.Throws<IndexOutOfRangeException>(call);
            Assert.AreEqual("Index must be in range x:[0,15], y:[0,15].", exception.Message);
        }

        [Test]
        public void SetBufferToImageAtOriginalLocation_AfterModificationsToBuffer_TargetImageUpdated()
        {
            // Setup
            ColorAccess colorAccess = ColorAccess.Create(Resources.acorn);

            for (var i = 0; i < 16; i++)
            {
                colorAccess[i, i] = Color.Red;
                colorAccess[i, 15 - i] = Color.Red;
            }

            using (var targetImage = (Bitmap) Resources.acorn.Clone())
            {
                // Call
                colorAccess.SetBufferToImageAtOriginalLocation(targetImage);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.acornWithCross, targetImage);
            }
        }

        [Test]
        public void SetBufferToImageAtOriginalLocation_BitmapNull_ThrowArgumentNullException()
        {
            // Setup
            ColorAccess colorAccess = ColorAccess.Create(Resources.acorn);

            // Call
            TestDelegate call = () => colorAccess.SetBufferToImageAtOriginalLocation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("bitmap", paramName);
        }
    }
}