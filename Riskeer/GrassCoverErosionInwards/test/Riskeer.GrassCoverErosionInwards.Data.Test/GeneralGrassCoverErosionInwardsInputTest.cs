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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GeneralGrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var fbFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 4.75,
                StandardDeviation = (RoundedDouble) 0.5,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var fnFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 2.6,
                StandardDeviation = (RoundedDouble) 0.35,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var fshallow = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.92,
                StandardDeviation = (RoundedDouble) 0.24,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var frunupModelFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.07,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            // Call
            var inputParameters = new GeneralGrassCoverErosionInwardsInput();

            // Assert
            Assert.AreEqual(2, inputParameters.N.NumberOfDecimalPlaces);
            Assert.AreEqual(2.0, inputParameters.N, inputParameters.N.GetAccuracy());

            DistributionAssert.AreEqual(fbFactor, inputParameters.FbFactor);
            DistributionAssert.AreEqual(fnFactor, inputParameters.FnFactor);
            DistributionAssert.AreEqual(fshallow, inputParameters.FshallowModelFactor);
            DistributionAssert.AreEqual(frunupModelFactor, inputParameters.FrunupModelFactor);

            Assert.AreEqual(1, inputParameters.CriticalOvertoppingModelFactor);
            Assert.AreEqual(1, inputParameters.OvertoppingModelFactor);
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
            var generalGrassCoverErosionInwardsInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            generalGrassCoverErosionInwardsInput.N = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(2, generalGrassCoverErosionInwardsInput.N.NumberOfDecimalPlaces);
            Assert.AreEqual(value, generalGrassCoverErosionInwardsInput.N, generalGrassCoverErosionInwardsInput.N.GetAccuracy());
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
            var generalGrassCoverErosionInwardsInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => generalGrassCoverErosionInwardsInput.N = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}