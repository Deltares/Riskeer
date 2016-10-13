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

using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
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

            var modelFactorSubCriticalFlow = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var modelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.2
            };

            // Call
            var inputParameters = new GeneralClosingStructuresInput();

            // Assert
            Assert.AreEqual(2, inputParameters.C.NumberOfDecimalPlaces);
            AssertEqualValue(0.5, inputParameters.C);

            Assert.AreEqual(1, inputParameters.N2A);

            Assert.AreEqual(2, inputParameters.N.NumberOfDecimalPlaces);
            AssertEqualValue(1, inputParameters.N);

            Assert.AreEqual(2, inputParameters.GravitationalAcceleration.NumberOfDecimalPlaces);
            AssertEqualValue(9.81, inputParameters.GravitationalAcceleration);

            DistributionAssert.AreEqual(modelFactorOvertoppingFlow, inputParameters.ModelFactorOvertoppingFlow);
            DistributionAssert.AreEqual(modelFactorSubCriticalFlow, inputParameters.ModelFactorSubCriticalFlow);
            DistributionAssert.AreEqual(modelFactorStorageVolume, inputParameters.ModelFactorStorageVolume);

            Assert.AreEqual(2, inputParameters.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            AssertEqualValue(1.0, inputParameters.ModelFactorInflowVolume);
        }

        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 4)]
        public void N_VariousValues_ReturnsExpectedValue(double c, int n2A, double expected)
        {
            // Setup
            var inputParameters = new GeneralClosingStructuresInput
            {
                C = (RoundedDouble) c,
                N2A = n2A
            };

            // Call
            RoundedDouble n = inputParameters.N;

            // Assert
            Assert.AreEqual(expected, n, n.GetAccuracy());
        }

        private void AssertEqualValue(double expected, RoundedDouble actual)
        {
            Assert.AreEqual(expected, actual, actual.GetAccuracy());
        }
    }
}