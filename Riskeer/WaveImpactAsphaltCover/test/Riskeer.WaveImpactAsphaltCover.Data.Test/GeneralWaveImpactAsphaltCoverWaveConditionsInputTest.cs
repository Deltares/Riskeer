// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class GeneralWaveImpactAsphaltCoverWaveConditionsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var generalInput = new GeneralWaveImpactAsphaltCoverWaveConditionsInput();

            // Assert
            Assert.AreEqual(1.0, generalInput.A, generalInput.A.GetAccuracy());
            Assert.AreEqual(2, generalInput.A.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, generalInput.B, generalInput.B.GetAccuracy());
            Assert.AreEqual(2, generalInput.B.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, generalInput.C, generalInput.C.GetAccuracy());
            Assert.AreEqual(2, generalInput.C.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1.69)]
        [TestCase(-0.004)]
        [TestCase(2.004)]
        public void C_SetValidValue_ValueSet(double newValue)
        {
            // Setup
            var generalInput = new GeneralWaveImpactAsphaltCoverWaveConditionsInput();

            // Call
            generalInput.C = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, generalInput.C, generalInput.C.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(-0.005)]
        [TestCase(2.005)]
        public void C_SetInvalidValue_ThrowArgumentException(double newValue)
        {
            // Setup
            var generalInput = new GeneralWaveImpactAsphaltCoverWaveConditionsInput();

            // Call
            TestDelegate test = () => generalInput.C = (RoundedDouble) newValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test,
                "De waarde van parameter 'c' moet binnen het bereik [0,00, 2,00] liggen.");
        }
    }
}