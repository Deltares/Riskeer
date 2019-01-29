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
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class GeneralStabilityStoneCoverWaveConditionsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var generalInput = new GeneralStabilityStoneCoverWaveConditionsInput();

            // Assert
            Assert.AreEqual(2, generalInput.GeneralBlocksWaveConditionsInput.A.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, generalInput.GeneralBlocksWaveConditionsInput.A, generalInput.GeneralBlocksWaveConditionsInput.A.GetAccuracy());

            Assert.AreEqual(2, generalInput.GeneralBlocksWaveConditionsInput.B.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, generalInput.GeneralBlocksWaveConditionsInput.B, generalInput.GeneralBlocksWaveConditionsInput.B.GetAccuracy());

            Assert.AreEqual(2, generalInput.GeneralBlocksWaveConditionsInput.C.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, generalInput.GeneralBlocksWaveConditionsInput.C, generalInput.GeneralBlocksWaveConditionsInput.C.GetAccuracy());

            Assert.AreEqual(2, generalInput.GeneralColumnsWaveConditionsInput.A.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, generalInput.GeneralColumnsWaveConditionsInput.A, generalInput.GeneralColumnsWaveConditionsInput.A.GetAccuracy());

            Assert.AreEqual(2, generalInput.GeneralColumnsWaveConditionsInput.B.NumberOfDecimalPlaces);
            Assert.AreEqual(0.4, generalInput.GeneralColumnsWaveConditionsInput.B, generalInput.GeneralColumnsWaveConditionsInput.B.GetAccuracy());

            Assert.AreEqual(2, generalInput.GeneralColumnsWaveConditionsInput.C.NumberOfDecimalPlaces);
            Assert.AreEqual(0.8, generalInput.GeneralColumnsWaveConditionsInput.C, generalInput.GeneralColumnsWaveConditionsInput.C.GetAccuracy());

            Assert.AreEqual(2, generalInput.N.NumberOfDecimalPlaces);
            Assert.AreEqual(4.0, generalInput.N, generalInput.N.GetAccuracy());
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(20.0)]
        [TestCase(0.999)]
        [TestCase(20.001)]
        public void N_SetValidValue_UpdatesValue(double value)
        {
            // Setup
            var generalInput = new GeneralStabilityStoneCoverWaveConditionsInput();

            // Call
            generalInput.N = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(value, generalInput.N, generalInput.N.GetAccuracy());
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-10.0)]
        [TestCase(0.99)]
        [TestCase(20.01)]
        [TestCase(50.0)]
        public void N_SetValueOutsideValidRange_ThrowArgumentOutOfRangeException(double value)
        {
            // Setup
            var generalInput = new GeneralStabilityStoneCoverWaveConditionsInput();

            // Call
            TestDelegate call = () => generalInput.N = (RoundedDouble) value;

            // Assert
            var expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}