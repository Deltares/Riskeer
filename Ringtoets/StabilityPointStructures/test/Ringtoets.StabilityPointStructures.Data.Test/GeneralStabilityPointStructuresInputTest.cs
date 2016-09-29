﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class GeneralStabilityPointStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralStabilityPointStructuresInput();

            // Assert
            Assert.AreEqual(3, inputParameters.N);

            Assert.AreEqual(2, inputParameters.GravitationalAcceleration.NumberOfDecimalPlaces);
            Assert.AreEqual(9.81, inputParameters.GravitationalAcceleration, inputParameters.GravitationalAcceleration.GetAccuracy());

            var modelFactorForStorageVolume = new NormalDistribution(1)
            {
                Mean = new RoundedDouble(1, 1),
                StandardDeviation = new RoundedDouble(1, 0.2)
            };
            Assert.AreEqual(modelFactorForStorageVolume.Mean, inputParameters.ModelFactorForStorageVolume.Mean);
            Assert.AreEqual(modelFactorForStorageVolume.StandardDeviation, inputParameters.ModelFactorForStorageVolume.StandardDeviation);

            var modelFactorForSubCriticalFlow = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 0.1)
            };
            Assert.AreEqual(modelFactorForSubCriticalFlow.Mean, inputParameters.ModelFactorForSubCriticalFlow.Mean);
            Assert.AreEqual(modelFactorForSubCriticalFlow.StandardDeviation, inputParameters.ModelFactorForSubCriticalFlow.StandardDeviation);

            var modelFactorForCollisionLoad = new NormalDistribution(1)
            {
                Mean = new RoundedDouble(1, 1),
                StandardDeviation = new RoundedDouble(1, 0.2)
            };
            Assert.AreEqual(modelFactorForCollisionLoad.Mean, inputParameters.ModelFactorForCollisionLoad.Mean);
            Assert.AreEqual(modelFactorForCollisionLoad.StandardDeviation, inputParameters.ModelFactorForCollisionLoad.StandardDeviation);

            var modelFactorLoadEffectMs = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 0.05)
            };
            Assert.AreEqual(modelFactorLoadEffectMs.Mean, inputParameters.ModelFactorLoadEffectMs.Mean);
            Assert.AreEqual(modelFactorLoadEffectMs.StandardDeviation, inputParameters.ModelFactorLoadEffectMs.StandardDeviation);

            Assert.AreEqual(1, inputParameters.ModelFactorForIncomingFlowVolume.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModelFactorForIncomingFlowVolume, inputParameters.ModelFactorForIncomingFlowVolume.GetAccuracy());

            Assert.AreEqual(1, inputParameters.ModificationFactor1.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModificationFactor1, inputParameters.ModificationFactor1.GetAccuracy());

            Assert.AreEqual(1, inputParameters.ModificationFactor2.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModificationFactor2, inputParameters.ModificationFactor2.GetAccuracy());
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N_ValueInsideValidRegion_DoesNotThrow(int value)
        {
            // Setup
            var generalHeightStructuresInput = new GeneralStabilityPointStructuresInput();

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
            var generalHeightStructuresInput = new GeneralStabilityPointStructuresInput();

            // Call
            TestDelegate test = () => generalHeightStructuresInput.N = value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in interval [1, 20] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}