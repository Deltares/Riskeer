// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            Assert.AreEqual(2, inputParameters.C.NumberOfDecimalPlaces);
            AssertAreEqual(0.5, inputParameters.C);
            Assert.AreEqual(1, inputParameters.N2A);

            Assert.AreEqual(2, inputParameters.GravitationalAcceleration.NumberOfDecimalPlaces);
            AssertAreEqual(9.81, inputParameters.GravitationalAcceleration);

            DistributionAssert.AreEqual(modelFactorOvertoppingFlow, inputParameters.ModelFactorOvertoppingFlow);
            DistributionAssert.AreEqual(modelFactorStorageVolume, inputParameters.ModelFactorStorageVolume);
            DistributionAssert.AreEqual(modelFactorLongThreshold, inputParameters.ModelFactorLongThreshold);

            Assert.AreEqual(2, inputParameters.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            AssertAreEqual(1.0, inputParameters.ModelFactorInflowVolume);
        }

        [Test]
        [TestCase(0, 1.0)]
        [TestCase(8, 4.0)]
        [TestCase(40, 20.0)]
        public void N_VariousN2AValues_ReturnsExpectedValue(int n2A, double expected)
        {
            // Setup
            var inputParameters = new GeneralClosingStructuresInput
            {
                N2A = n2A
            };

            // Call
            double n = inputParameters.N;

            // Assert
            Assert.AreEqual(expected, n);
        }

        [Test]
        [TestCase(-456)]
        [TestCase(-1)]
        [TestCase(41)]
        [TestCase(687)]
        public void N2A_ValuesOutOfRange_ThrowArgumentOutOfRangeException(int invalidValue)
        {
            // Setup
            var inputParameters = new GeneralClosingStructuresInput();

            // Call
            TestDelegate call = () => inputParameters.N2A = invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(
                call, "De waarde voor 'N2A' moet in het bereik [0, 40] liggen.");
        }

        private static void AssertAreEqual(double expected, RoundedDouble actual)
        {
            Assert.AreEqual(expected, actual, actual.GetAccuracy());
        }
    }
}