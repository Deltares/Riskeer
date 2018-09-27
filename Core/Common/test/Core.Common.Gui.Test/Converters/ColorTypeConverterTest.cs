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
using Core.Common.Gui.Converters;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class ColorTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var converter = new ColorTypeConverter();

            // Assert
            Assert.IsInstanceOf<ColorConverter>(converter);
        }

        [Test]
        public void GetStandardValuesSupported_Always_ReturnFalse()
        {
            // Setup
            var converter = new ColorTypeConverter();

            // Call
            bool standardValuesSupported = converter.GetStandardValuesSupported(null);

            // Assert
            Assert.IsFalse(standardValuesSupported);
        }
    }
}