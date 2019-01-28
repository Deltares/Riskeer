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

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class GeneralStabilityPointStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup 
            var modelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            var modelFactorCollisionLoad = new VariationCoefficientNormalDistribution(1)
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var modelFactorLoadEffect = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.05
            };

            var modelFactorLongThreshold = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.9,
                StandardDeviation = (RoundedDouble) 0.05
            };

            // Call
            var inputParameters = new GeneralStabilityPointStructuresInput();

            // Assert
            Assert.AreEqual(2, inputParameters.N.NumberOfDecimalPlaces);
            Assert.AreEqual(3.0, inputParameters.N, inputParameters.N.GetAccuracy());

            Assert.AreEqual(2, inputParameters.GravitationalAcceleration.NumberOfDecimalPlaces);
            Assert.AreEqual(9.81, inputParameters.GravitationalAcceleration, inputParameters.GravitationalAcceleration.GetAccuracy());

            DistributionAssert.AreEqual(modelFactorStorageVolume, inputParameters.ModelFactorStorageVolume);
            DistributionAssert.AreEqual(modelFactorCollisionLoad, inputParameters.ModelFactorCollisionLoad);
            DistributionAssert.AreEqual(modelFactorLoadEffect, inputParameters.ModelFactorLoadEffect);
            DistributionAssert.AreEqual(modelFactorLongThreshold, inputParameters.ModelFactorLongThreshold);

            Assert.AreEqual(2, inputParameters.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModelFactorInflowVolume, inputParameters.ModelFactorInflowVolume.GetAccuracy());

            Assert.AreEqual(0, inputParameters.ModificationFactorWavesSlowlyVaryingPressureComponent.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModificationFactorWavesSlowlyVaryingPressureComponent, inputParameters.ModificationFactorWavesSlowlyVaryingPressureComponent.GetAccuracy());

            Assert.AreEqual(0, inputParameters.ModificationFactorDynamicOrImpulsivePressureComponent.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.ModificationFactorDynamicOrImpulsivePressureComponent, inputParameters.ModificationFactorDynamicOrImpulsivePressureComponent.GetAccuracy());

            Assert.AreEqual(2, inputParameters.WaveRatioMaxHN.NumberOfDecimalPlaces);
            Assert.AreEqual(5000, inputParameters.WaveRatioMaxHN, inputParameters.WaveRatioMaxHN.GetAccuracy());

            Assert.AreEqual(2, inputParameters.WaveRatioMaxHStandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.WaveRatioMaxHStandardDeviation, inputParameters.WaveRatioMaxHStandardDeviation.GetAccuracy());
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
            var generalStabilityPointStructuresInput = new GeneralStabilityPointStructuresInput();

            // Call
            generalStabilityPointStructuresInput.N = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(2, generalStabilityPointStructuresInput.N.NumberOfDecimalPlaces);
            Assert.AreEqual(value, generalStabilityPointStructuresInput.N, generalStabilityPointStructuresInput.N.GetAccuracy());
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
            var generalStabilityPointStructuresInput = new GeneralStabilityPointStructuresInput();

            // Call
            TestDelegate test = () => generalStabilityPointStructuresInput.N = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }
    }
}