// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using NUnit.Framework;
using Riskeer.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerColorConverterTest
    {
        [Test]
        public void Convert_Null_ReturnsEmptyColor()
        {
            // Call
            Color color = SoilLayerColorConverter.Convert(null);

            // Assert
            Assert.AreEqual(Color.Empty, color);
        }

        [Test]
        [TestCase(-12156236, 70, 130, 180)]
        [TestCase(-8372160, 128, 64, 64)]
        [TestCase(-65536, 255, 0, 0)]
        [TestCase(-14634326, 32, 178, 170)]
        [TestCase(-6632142, 154, 205, 50)]
        [TestCase(-12566528, 64, 64, 0)]
        [TestCase(-7278960, 144, 238, 144)]
        [TestCase(-8323200, 128, 255, 128)]
        [TestCase(-65281, 255, 0, 255)]
        [TestCase(-8372224, 128, 64, 0)]
        public void Convert_DifferentDoubleValues_ReturnsExpectedColor(double colorValue, int r, int g, int b)
        {
            // Call
            Color color = SoilLayerColorConverter.Convert(colorValue);

            // Assert
            Assert.AreEqual(r, color.R);
            Assert.AreEqual(g, color.G);
            Assert.AreEqual(b, color.B);
            Assert.AreEqual(255, color.A);
        }
    }
}