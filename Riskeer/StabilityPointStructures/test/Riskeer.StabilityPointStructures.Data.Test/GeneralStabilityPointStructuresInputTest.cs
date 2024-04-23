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

namespace Riskeer.StabilityPointStructures.Data.Test
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
    }
}