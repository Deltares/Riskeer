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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class GeneralWaveImpactAsphaltCoverInputTest
    {
        [Test]
        public void Constructor_Always_PropertiesSet()
        {
            // Call
            var generalInput = new GeneralWaveImpactAsphaltCoverInput();

            // Assert
            Assert.AreEqual(2, generalInput.DeltaL.NumberOfDecimalPlaces);
            Assert.AreEqual(1000.0, generalInput.DeltaL, generalInput.DeltaL.GetAccuracy());
        }

        [Test]
        public void GetN_SectionLengthNaN_ReturnsNaN()
        {
            // Setup
            var random = new Random(39);
            var generalInput = new GeneralWaveImpactAsphaltCoverInput
            {
                DeltaL = random.NextRoundedDouble()
            };

            // Call
            double n = generalInput.GetN(double.NaN);

            // Assert
            Assert.IsNaN(n);
        }

        [Test]
        public void GetN_DeltaLBiggerThanSectionLength_ReturnsCorrectValue()
        {
            // Setup
            var random = new Random(39);

            var generalInput = new GeneralWaveImpactAsphaltCoverInput
            {
                DeltaL = random.NextRoundedDouble(1001, 99999)
            };

            // Call
            double n = generalInput.GetN(random.NextDouble(0, 1000));

            // Assert
            Assert.AreEqual(1, n);
        }

        [Test]
        public void GetN_DeltaLSmallerThanSectionLength_ReturnsCorrectValue()
        {
            // Setup
            var random = new Random(39);
            double sectionLength = random.NextDouble(1001, 99999);
            RoundedDouble deltaL = random.NextRoundedDouble(0, 1000);

            var generalInput = new GeneralWaveImpactAsphaltCoverInput
            {
                DeltaL = deltaL
            };

            // Call
            double n = generalInput.GetN(sectionLength);

            // Assert
            Assert.AreEqual(sectionLength / deltaL, n, 1e-2);
        }

        [Test]
        [TestCase(0.005)]
        [TestCase(42)]
        [TestCase(1000)]
        [TestCase(double.MaxValue)]
        public void DeltaL_SetValidValue_UpdatesValue(double value)
        {
            // Setup
            var generalInput = new GeneralWaveImpactAsphaltCoverInput();

            // Call
            generalInput.DeltaL = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(2, generalInput.DeltaL.NumberOfDecimalPlaces);
            Assert.AreEqual(value, generalInput.DeltaL, generalInput.DeltaL.GetAccuracy());
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(0.004)]
        [TestCase(-1)]
        [TestCase(double.MinValue)]
        [TestCase(double.NaN)]
        public void DeltaL_SetValueOutsideValidRange_ThrowArgumentOutOfRangeException(double value)
        {
            // Setup
            var generalInput = new GeneralWaveImpactAsphaltCoverInput();

            // Call
            TestDelegate test = () => generalInput.DeltaL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'ΔL' moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}