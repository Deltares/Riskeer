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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
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
            Assert.IsNull(input.HydraulicBoundaryLocation);

            AssertAreEqual(0.05, input.LevelOfCrestOfStructure.StandardDeviation);
            AssertAreEqual(1.1, input.ModelFactorOvertoppingSuperCriticalFlow.Mean);
            AssertAreEqual(0.03, input.ModelFactorOvertoppingSuperCriticalFlow.StandardDeviation);
            AssertAreEqual(0.1, input.AllowableIncreaseOfLevelForStorage.StandardDeviation);
            AssertAreEqual(0.1, input.StorageStructureArea.StandardDeviation);
            AssertAreEqual(0.05, input.FlowWidthAtBottomProtection.StandardDeviation);
            AssertAreEqual(0.15, input.CriticalOvertoppingDischarge.StandardDeviation);
            AssertAreEqual(0.05, input.WidthOfFlowApertures.StandardDeviation);
            AssertAreEqual(7.5, input.StormDuration.Mean);
            AssertAreEqual(0.25, input.StormDuration.StandardDeviation);
        }

        [Test]
        public void Properties_ModelFactorOvertoppingSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble defaultModelFactorOvertoppingSupercriticalFlowStandardDeviation = input.ModelFactorOvertoppingSuperCriticalFlow.StandardDeviation;
            NormalDistribution modelFactorOvertoppingSuperCriticalFlow = new NormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.ModelFactorOvertoppingSuperCriticalFlow = modelFactorOvertoppingSuperCriticalFlow;

            // Assert
            AssertAreEqual(modelFactorOvertoppingSuperCriticalFlow.Mean, input.ModelFactorOvertoppingSuperCriticalFlow.Mean);
            AssertAreEqual(defaultModelFactorOvertoppingSupercriticalFlowStandardDeviation, input.ModelFactorOvertoppingSuperCriticalFlow.StandardDeviation);
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
        public void Properties_DeviationOfTheWaveDirection_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble deviationOfTheWaveDirection = new RoundedDouble(5, random.NextDouble());

            // Call
            input.DeviationOfTheWaveDirection = deviationOfTheWaveDirection;

            // Assert
            AssertAreEqual(deviationOfTheWaveDirection, input.DeviationOfTheWaveDirection);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble defaultStormDurationStandardDeviation = input.StormDuration.StandardDeviation;
            LogNormalDistribution stormDuration = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.StormDuration = stormDuration;

            // Assert
            AssertAreEqual(stormDuration.Mean, input.StormDuration.Mean);
            AssertAreEqual(defaultStormDurationStandardDeviation, input.StormDuration.StandardDeviation);
        }

        [Test]
        public void Properties_LevelOfCrestOfStructure_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            NormalDistribution levelOfCrestOfStructure = new NormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.LevelOfCrestOfStructure = levelOfCrestOfStructure;

            // Assert
            AssertAreEqual(levelOfCrestOfStructure.Mean, input.LevelOfCrestOfStructure.Mean);
            AssertAreEqual(levelOfCrestOfStructure.StandardDeviation, input.LevelOfCrestOfStructure.StandardDeviation);
        }

        [Test]
        public void Properties_OrientationOfTheNormalOfTheStructure_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble orientationOfTheNormalOfTheStructure = new RoundedDouble(5, random.NextDouble());

            // Call
            input.OrientationOfTheNormalOfTheStructure = orientationOfTheNormalOfTheStructure;

            // Assert
            AssertAreEqual(orientationOfTheNormalOfTheStructure, input.OrientationOfTheNormalOfTheStructure);
        }

        [Test]
        public void Properties_AllowableIncreaseOfLevelForStorage_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            LogNormalDistribution allowableIncreaseOfLevelForStorage = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.AllowableIncreaseOfLevelForStorage = allowableIncreaseOfLevelForStorage;

            // Assert
            AssertAreEqual(allowableIncreaseOfLevelForStorage.Mean, input.AllowableIncreaseOfLevelForStorage.Mean);
            AssertAreEqual(allowableIncreaseOfLevelForStorage.StandardDeviation, input.AllowableIncreaseOfLevelForStorage.StandardDeviation);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            LogNormalDistribution storageStructureArea = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.StorageStructureArea = storageStructureArea;

            // Assert
            AssertAreEqual(storageStructureArea.Mean, input.StorageStructureArea.Mean);
            AssertAreEqual(storageStructureArea.StandardDeviation, input.StorageStructureArea.StandardDeviation);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            LogNormalDistribution flowWidthAtBottomProtection = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
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

            LogNormalDistribution criticalOvertoppingDischarge = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;

            // Assert
            AssertAreEqual(criticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.Mean);
            AssertAreEqual(criticalOvertoppingDischarge.StandardDeviation, input.CriticalOvertoppingDischarge.StandardDeviation);
        }

        [Test]
        public void Properties_FailureProbabilityOfStructureGivenErosion_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble failureProbabilityOfStructureGivenErosion = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FailureProbabilityOfStructureGivenErosion = failureProbabilityOfStructureGivenErosion;

            // Assert
            AssertAreEqual(failureProbabilityOfStructureGivenErosion, input.FailureProbabilityOfStructureGivenErosion);
        }

        [Test]
        public void Properties_WidthOfFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            NormalDistribution widthOfFlowApertures = new NormalDistribution(5)
            {
                Mean = (RoundedDouble) random.NextDouble(),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.WidthOfFlowApertures = widthOfFlowApertures;

            // Assert
            AssertAreEqual(widthOfFlowApertures.Mean, input.WidthOfFlowApertures.Mean);
            AssertAreEqual(widthOfFlowApertures.StandardDeviation, input.WidthOfFlowApertures.StandardDeviation);
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}