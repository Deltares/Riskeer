// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Data;
using System.Windows.Media;
using Core.Components.GraphSharp.Forms.Converters;
using NUnit.Framework;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Components.GraphSharp.Forms.Test.Converters
{
    [TestFixture]
    public class IconToImageSourceConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new IconToImageSourceConverter();

            // Assert
            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_ValidIcon_ReturnsImageSource()
        {
            // Setup
            var converter = new IconToImageSourceConverter();

            // Call
            object imageSource = converter.Convert(CoreCommonGuiResources.warning, null, null, null);

            // Assert
            Assert.IsInstanceOf<ImageSource>(imageSource);
        }

        [Test]
        public void ConvertBack_Always_ThrowsNotSupportedException()
        {
            // Setup
            var converter = new IconToImageSourceConverter();

            // Call
            void Call() => converter.ConvertBack(null, null, null, null);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }
    }
}