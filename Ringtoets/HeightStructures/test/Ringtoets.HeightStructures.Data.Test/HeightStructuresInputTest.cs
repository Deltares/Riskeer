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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresInputTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var input = new HeightStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            AssertAreEqual(0, input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            AssertAreEqual(0.05, input.LevelCrestStructure.StandardDeviation);
            AssertAreEqual(1.1, input.ModelFactorSuperCriticalFlow.Mean);
            AssertAreEqual(0.03, input.ModelFactorSuperCriticalFlow.StandardDeviation);
            AssertAreEqual(0.1, input.AllowedLevelIncreaseStorage.StandardDeviation);
            AssertAreEqual(0.1, input.StorageStructureArea.CoefficientOfVariation);
            AssertAreEqual(0.05, input.FlowWidthAtBottomProtection.StandardDeviation);
            AssertAreEqual(0.15, input.CriticalOvertoppingDischarge.CoefficientOfVariation);
            AssertAreEqual(0.05, input.WidthFlowApertures.CoefficientOfVariation);
            AssertAreEqual(6.0, input.StormDuration.Mean);
            AssertAreEqual(0.25, input.StormDuration.CoefficientOfVariation);
        }

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble defaultModelFactorSuperCriticalFlowStandardDeviation = input.ModelFactorSuperCriticalFlow.StandardDeviation;
            NormalDistribution modelFactorSuperCriticalFlow = new NormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.ModelFactorSuperCriticalFlow = modelFactorSuperCriticalFlow;

            // Assert
            AssertAreEqual(modelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.Mean);
            AssertAreEqual(defaultModelFactorSuperCriticalFlowStandardDeviation, input.ModelFactorSuperCriticalFlow.StandardDeviation);
        }

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreEqual(location, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Properties_DeviationWaveDirection_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble deviationWaveDirection = new RoundedDouble(5, random.NextDouble());

            // Call
            input.DeviationWaveDirection = deviationWaveDirection;

            // Assert
            AssertAreEqual(deviationWaveDirection, input.DeviationWaveDirection);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble defaultStormDurationCoefficientOfVariation = input.StormDuration.CoefficientOfVariation;
            var stormDuration = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.StormDuration = stormDuration;

            // Assert
            AssertAreEqual(stormDuration.Mean, input.StormDuration.Mean);
            AssertAreEqual(defaultStormDurationCoefficientOfVariation, input.StormDuration.CoefficientOfVariation);
        }

        [Test]
        public void Properties_LevelCrestStructure_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            NormalDistribution levelCrestStructure = new NormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.LevelCrestStructure = levelCrestStructure;

            // Assert
            AssertAreEqual(levelCrestStructure.Mean, input.LevelCrestStructure.Mean);
            AssertAreEqual(levelCrestStructure.StandardDeviation, input.LevelCrestStructure.StandardDeviation);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void Properties_StructureNormalOrientationValidValues_NewValueSet(double orientation)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertAreEqual(orientation, input.StructureNormalOrientation);
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Properties_StructureNormalOrientationInValidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik tussen [0, 360] graden liggen.");
        }

        [Test]
        public void Properties_AllowedLevelIncreaseStorage_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            LogNormalDistribution allowedLevelIncreaseStorage = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.AllowedLevelIncreaseStorage = allowedLevelIncreaseStorage;

            // Assert
            AssertAreEqual(allowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.Mean);
            AssertAreEqual(allowedLevelIncreaseStorage.StandardDeviation, input.AllowedLevelIncreaseStorage.StandardDeviation);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            var storageStructureArea = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.StorageStructureArea = storageStructureArea;

            // Assert
            AssertAreEqual(storageStructureArea.Mean, input.StorageStructureArea.Mean);
            AssertAreEqual(storageStructureArea.CoefficientOfVariation, input.StorageStructureArea.CoefficientOfVariation);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            LogNormalDistribution flowWidthAtBottomProtection = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;

            // Assert
            AssertAreEqual(flowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.Mean);
            AssertAreEqual(flowWidthAtBottomProtection.StandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation);
        }

        [Test]
        public void Properties_CriticalOvertoppingDischarge_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            var criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;

            // Assert
            AssertAreEqual(criticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.Mean);
            AssertAreEqual(criticalOvertoppingDischarge.CoefficientOfVariation, input.CriticalOvertoppingDischarge.CoefficientOfVariation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1)]
        public void Properties_ValidFailureProbabilityStructureWithErosion_ExpectedValues(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.FailureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;

            // Assert
            Assert.AreEqual(failureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(double.NaN)]
        public void Properties_InvalidFailureProbabilityStructureWithErosion_ThrowArgumentOutOfRangeException(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbabilityStructureWithErosion;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        public void Properties_WidthFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            var widthFlowApertures = new VariationCoefficientNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.WidthFlowApertures = widthFlowApertures;

            // Assert
            AssertAreEqual(widthFlowApertures.Mean, input.WidthFlowApertures.Mean);
            AssertAreEqual(widthFlowApertures.CoefficientOfVariation, input.WidthFlowApertures.CoefficientOfVariation);
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}