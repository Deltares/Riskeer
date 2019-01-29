// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerIsAquiferConverterTest
    {
        [Test]
        [TestCase(1.0, true)]
        [TestCase(1 + 1e-7, true)]
        [TestCase(1 - 1e-7, true)]
        [TestCase(0, false)]
        [TestCase(0.0 + 1e-7, false)]
        [TestCase(0.0 - 1e-7, false)]
        public void Convert_ValidValues_ReturnsExpectedValues(double isAquifer,
                                                              bool expectedResult)
        {
            // Call
            bool result = SoilLayerIsAquiferConverter.Convert(isAquifer);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(null)]
        [TestCase(0.5)]
        [TestCase(1.1)]
        [TestCase(0.1)]
        [TestCase(1 - 1e-5)]
        [TestCase(1 + 1e-5)]
        [TestCase(0 - 1e-5)]
        [TestCase(0 + 1e-5)]
        [TestCase(double.NaN)]
        public void Convert_InvalidValues_ThrowsImportedDataTransformException(double? isAquifer)
        {
            // Call
            TestDelegate call = () => SoilLayerIsAquiferConverter.Convert(isAquifer);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            string expectedMessage = $"A value of {isAquifer} for isAquifer cannot be converted to a valid boolean.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}