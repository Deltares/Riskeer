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

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Core.Common.Gui.Converters;
using Core.Common.Gui.Test.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class PngToIconConverterTest
    {
        [Test]
        public void DefaultConcstructor_ExpectedValues()
        {
            // Call
            var converter = new PngToIconConverter();

            // Assert
            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_WithPngImage_ConvertToBitmapImage()
        {
            // Setup
            var converter = new PngToIconConverter();

            // Call
            object bitmap = converter.Convert(Resources.abacus, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

            // Assert
            var bitmapInstance = bitmap as BitmapImage;
            Assert.NotNull(bitmapInstance);
            TestHelper.AssertImagesAreEqual(new Bitmap(bitmapInstance.StreamSource), Resources.abacus);
        }

        [Test]
        public void ConvertBack_ThrowNotImplementedException()
        {
            // Setup
            var converter = new PngToIconConverter();

            // Call
            TestDelegate call = () => converter.ConvertBack(new BitmapImage(), typeof(Bitmap), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Throws<NotImplementedException>(call);
        }
    }
}