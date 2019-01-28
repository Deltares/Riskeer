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

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GeneralGrassCoverErosionOutwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralGrassCoverErosionOutwardsInput();

            // Assert
            Assert.AreEqual(2, inputParameters.N.NumberOfDecimalPlaces);
            Assert.AreEqual(2.0, inputParameters.N, inputParameters.N.GetAccuracy());
            Assert.AreEqual(1.0, inputParameters.GeneralWaveConditionsInput.A, inputParameters.GeneralWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(0.67, inputParameters.GeneralWaveConditionsInput.B, inputParameters.GeneralWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(0.0, inputParameters.GeneralWaveConditionsInput.C, inputParameters.GeneralWaveConditionsInput.C.GetAccuracy());
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
            var generalGrassCoverErosionOutwardsInput = new GeneralGrassCoverErosionOutwardsInput();

            // Call
            generalGrassCoverErosionOutwardsInput.N = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(2, generalGrassCoverErosionOutwardsInput.N.NumberOfDecimalPlaces);
            Assert.AreEqual(value, generalGrassCoverErosionOutwardsInput.N, generalGrassCoverErosionOutwardsInput.N.GetAccuracy());
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
            var generalGrassCoverErosionOutwardsInput = new GeneralGrassCoverErosionOutwardsInput();

            // Call
            TestDelegate test = () => generalGrassCoverErosionOutwardsInput.N = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}