// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Properties;
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
            Assert.AreEqual(2, inputParameters.N);

            DistributionAssert.AreEqual(fbFactor, inputParameters.FbFactor);
            DistributionAssert.AreEqual(fnFactor, inputParameters.FnFactor);
            DistributionAssert.AreEqual(fshallow, inputParameters.FshallowModelFactor);
            DistributionAssert.AreEqual(frunupModelFactor, inputParameters.FrunupModelFactor);

            Assert.AreEqual(1, inputParameters.CriticalOvertoppingModelFactor);
            Assert.AreEqual(1, inputParameters.OvertoppingModelFactor);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N_ValueInsideValidRegion_DoesNotThrow(int value)
        {
            // Setup
            var generalGrassCoverErosionInwardsInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => generalGrassCoverErosionInwardsInput.N = value;

            // Assert
            Assert.DoesNotThrow(test);
            Assert.AreEqual(value, generalGrassCoverErosionInwardsInput.N);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(21)]
        [TestCase(50)]
        public void N_ValueOutsideValidRegion_ThrowsArgumentOutOfRangeException(int value)
        {
            // Setup
            var generalGrassCoverErosionInwardsInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => generalGrassCoverErosionInwardsInput.N = value;

            // Assert
            var expectedMessage = Resources.N_Value_should_be_in_interval_1_20;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}