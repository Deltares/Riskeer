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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class GeneralHeightStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var modelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.00,
                StandardDeviation = (RoundedDouble) 0.20
            };

            var modelFactorOvertoppingFlow = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.09,
                StandardDeviation = (RoundedDouble) 0.06
            };

            // Call
            var generalHeightStructuresInput = new GeneralHeightStructuresInput();

            // Assert
            Assert.AreEqual(2, generalHeightStructuresInput.N);

            Assert.AreEqual(2, generalHeightStructuresInput.GravitationalAcceleration.NumberOfDecimalPlaces);
            Assert.AreEqual(9.81, generalHeightStructuresInput.GravitationalAcceleration, generalHeightStructuresInput.GravitationalAcceleration.GetAccuracy());

            DistributionAssert.AreEqual(modelFactorOvertoppingFlow, generalHeightStructuresInput.ModelFactorOvertoppingFlow);
            DistributionAssert.AreEqual(modelFactorStorageVolume, generalHeightStructuresInput.ModelFactorStorageVolume);

            Assert.AreEqual(2, generalHeightStructuresInput.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            Assert.AreEqual(1, generalHeightStructuresInput.ModelFactorInflowVolume, generalHeightStructuresInput.ModelFactorInflowVolume.GetAccuracy());
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N_ValueInsideValidRegion_DoesNotThrow(int value)
        {
            // Setup
            var generalHeightStructuresInput = new GeneralHeightStructuresInput();

            // Call
            TestDelegate test = () => generalHeightStructuresInput.N = value;

            // Assert
            Assert.DoesNotThrow(test);
            Assert.AreEqual(value, generalHeightStructuresInput.N);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(21)]
        [TestCase(50)]
        public void N_ValueOutsideValidRegion_ThrowsArgumentOutOfRangeException(int value)
        {
            // Setup
            var generalHeightStructuresInput = new GeneralHeightStructuresInput();

            // Call
            TestDelegate test = () => generalHeightStructuresInput.N = value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1, 20] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}