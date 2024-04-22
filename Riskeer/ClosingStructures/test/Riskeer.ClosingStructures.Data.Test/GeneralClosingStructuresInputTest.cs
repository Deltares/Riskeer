// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Data.Test
{
    [TestFixture]
    public class GeneralClosingStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var modelFactorOvertoppingFlow = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.09,
                StandardDeviation = (RoundedDouble) 0.06
            };

            var modelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.2
            };

            var modelFactorLongThreshold = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.9,
                StandardDeviation = (RoundedDouble) 0.05
            };

            // Call
            var inputParameters = new GeneralClosingStructuresInput();

            // Assert
            Assert.IsFalse(inputParameters.ApplyLengthEffectInSection);

            Assert.AreEqual(2, inputParameters.GravitationalAcceleration.NumberOfDecimalPlaces);
            AssertAreEqual(9.81, inputParameters.GravitationalAcceleration);

            DistributionAssert.AreEqual(modelFactorOvertoppingFlow, inputParameters.ModelFactorOvertoppingFlow);
            DistributionAssert.AreEqual(modelFactorStorageVolume, inputParameters.ModelFactorStorageVolume);
            DistributionAssert.AreEqual(modelFactorLongThreshold, inputParameters.ModelFactorLongThreshold);

            Assert.AreEqual(2, inputParameters.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            AssertAreEqual(1.0, inputParameters.ModelFactorInflowVolume);
        }

        private static void AssertAreEqual(double expected, RoundedDouble actual)
        {
            Assert.AreEqual(expected, actual, actual.GetAccuracy());
        }
    }
}