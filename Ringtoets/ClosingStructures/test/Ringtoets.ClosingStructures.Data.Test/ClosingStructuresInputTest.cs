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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new ClosingStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);

            AssertEqualValues(1.1, input.ModelFactorSuperCriticalFlow.Mean);
            AssertEqualValues(0.03, input.ModelFactorSuperCriticalFlow.StandardDeviation);

            AssertEqualValues(0.1, input.ThresholdLowWeirHeight.StandardDeviation);

            AssertEqualValues(1, input.DrainCoefficient.Mean);
            AssertEqualValues(0.2, input.DrainCoefficient.StandardDeviation);

            AssertEqualValues(0.01, input.AreaFlowApertures.StandardDeviation);
            AssertEqualValues(0.05, input.LevelCrestOfStructureNotClosing.StandardDeviation);
            AssertEqualValues(0.1, input.WaterLevelInside.StandardDeviation);

            AssertEqualValues(0.1, input.AllowableIncreaseOfLevelForStorage.StandardDeviation);
            AssertEqualValues(0.1, input.StorageStructureArea.StandardDeviation);
            AssertEqualValues(0.05, input.FlowWidthAtBottomProtection.StandardDeviation);

            AssertEqualValues(7.5, input.StormDuration.Mean);
            AssertEqualValues(0.25, input.StormDuration.GetVariationCoefficient());
            Assert.AreEqual(1, input.ProbabilityOpenStructureBeforeFlooding);
        }

        [Test]
        [TestCase(ClosingStructureType.VerticalWall)]
        [TestCase(ClosingStructureType.LowSill)]
        [TestCase(ClosingStructureType.FloodedCulvert)]
        public void Properties_Type_ExpectedValues(ClosingStructureType type)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            input.ClosingStructureType = type;

            // Assert
            Assert.AreEqual(type, input.ClosingStructureType);
        }

        [Test]
        public void Properties_OrientationOfTheNormalStructure_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            var orientationOfTheNormalStructure = new RoundedDouble(5, random.NextDouble());

            // Call
            input.OrientationOfTheNormalOfTheStructure = orientationOfTheNormalStructure;

            // Assert
            Assert.AreEqual(2, input.OrientationOfTheNormalOfTheStructure.NumberOfDecimalPlaces);
            AssertEqualValues(orientationOfTheNormalStructure, input.OrientationOfTheNormalOfTheStructure);
        }

        [Test]
        public void Properties_FactorStormDurationOpenStructure_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            var factorStormDuration = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FactorStormDurationOpenStructure = factorStormDuration;

            // Assert
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            AssertEqualValues(factorStormDuration, input.FactorStormDurationOpenStructure);
        }

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution modelFactorSuperCriticalFlow = GenerateNormalDistribution();

            RoundedDouble initialStd = input.ModelFactorSuperCriticalFlow.StandardDeviation;

            //Call
            input.ModelFactorSuperCriticalFlow = modelFactorSuperCriticalFlow;

            //Assert
            Assert.AreEqual(modelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.Mean);
            AssertEqualValues(initialStd, input.ModelFactorSuperCriticalFlow.StandardDeviation);
        }

        [Test]
        public void Properties_ThresholdLowWeirHeight_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution thresholdLowWeirHeight = GenerateNormalDistribution();

            RoundedDouble initialStd = input.ThresholdLowWeirHeight.StandardDeviation;

            //Call
            input.ThresholdLowWeirHeight = thresholdLowWeirHeight;

            //Assert
            Assert.AreEqual(thresholdLowWeirHeight.Mean, input.ThresholdLowWeirHeight.Mean);
            AssertEqualValues(initialStd, input.ThresholdLowWeirHeight.StandardDeviation);
        }

        [Test]
        public void Properties_DrainCoefficient_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution drainCoefficient = GenerateNormalDistribution();

            RoundedDouble initialStd = input.DrainCoefficient.StandardDeviation;

            //Call
            input.DrainCoefficient = drainCoefficient;

            //Assert
            Assert.AreEqual(drainCoefficient.Mean, input.DrainCoefficient.Mean);
            AssertEqualValues(initialStd, input.DrainCoefficient.StandardDeviation);
        }

        [Test]
        public void Properties_AreaFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution areaFlowApertures = GenerateLogNormalDistribution();

            //Call
            input.AreaFlowApertures = areaFlowApertures;

            //Assert
            Assert.AreEqual(areaFlowApertures.Mean, input.AreaFlowApertures.Mean);
            AssertEqualValues(areaFlowApertures.StandardDeviation, input.AreaFlowApertures.StandardDeviation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        public void Properties_FailureProbablityOpenStructure_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityOpenStructure = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityOpenStructure_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityOpenStructure = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityOpenStructure);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        public void Properties_FailureProbablityReparation_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbablityReparation = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityReparation_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbablityReparation = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbablityReparation);
        }

        [Test]
        public void Properties_IdenticalAperture_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            int identicalAperture = random.Next();

            // Call
            input.IdenticalAperture = identicalAperture;

            // Assert
            Assert.AreEqual(identicalAperture, input.IdenticalAperture);
        }

        [Test]
        public void Properties_LevelCrestOfStructureNotClosing_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution levelCrestOfStructureNotClosing = GenerateNormalDistribution();

            //Call
            input.LevelCrestOfStructureNotClosing = levelCrestOfStructureNotClosing;

            //Assert
            Assert.AreEqual(levelCrestOfStructureNotClosing.Mean, input.LevelCrestOfStructureNotClosing.Mean);
            Assert.AreEqual(levelCrestOfStructureNotClosing.StandardDeviation, input.LevelCrestOfStructureNotClosing.StandardDeviation);
        }

        [Test]
        public void Properties_WaterLevelInside_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution waterLevelInside = GenerateNormalDistribution();

            //Call
            input.WaterLevelInside = waterLevelInside;

            //Assert
            Assert.AreEqual(waterLevelInside.Mean, input.WaterLevelInside.Mean);
            Assert.AreEqual(waterLevelInside.StandardDeviation, input.WaterLevelInside.StandardDeviation);
        }

        [Test]
        public void Properties_AllowableIncreaseOfLevelForStorage_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution allowableIncreaseOfLevelForStorage = GenerateLogNormalDistribution();

            //Call
            input.AllowableIncreaseOfLevelForStorage = allowableIncreaseOfLevelForStorage;

            //Assert
            Assert.AreEqual(allowableIncreaseOfLevelForStorage.Mean, input.AllowableIncreaseOfLevelForStorage.Mean);
            Assert.AreEqual(allowableIncreaseOfLevelForStorage.StandardDeviation, input.AllowableIncreaseOfLevelForStorage.StandardDeviation);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution storageStructureArea = GenerateLogNormalDistribution();

            //Call
            input.StorageStructureArea = storageStructureArea;

            //Assert
            Assert.AreEqual(storageStructureArea.Mean, input.StorageStructureArea.Mean);
            Assert.AreEqual(storageStructureArea.StandardDeviation, input.StorageStructureArea.StandardDeviation);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution flowWidthAtBottomProtection = GenerateLogNormalDistribution();

            //Call
            input.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;

            //Assert
            Assert.AreEqual(flowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(flowWidthAtBottomProtection.StandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        public void Properties_FailureProbabilityOfStructureGivenErosion_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityOfStructureGivenErosion = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityOfStructureGivenErosion_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityOfStructureGivenErosion = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityOfStructureGivenErosion);
        }

        [Test]
        public void Properties_WidthOfFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution widthOfFlowApertures = GenerateNormalDistribution();

            //Call
            input.WidthOfFlowApertures = widthOfFlowApertures;

            //Assert
            Assert.AreEqual(widthOfFlowApertures.Mean, input.WidthOfFlowApertures.Mean);
            Assert.AreEqual(widthOfFlowApertures.StandardDeviation, input.WidthOfFlowApertures.StandardDeviation);
        }

        [Test]
        public void Properties_CriticalOvertoppingDischarge_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution criticalOverToppingDischarge = GenerateLogNormalDistribution();

            //Call
            input.CriticalOverToppingDischarge = criticalOverToppingDischarge;

            //Assert
            Assert.AreEqual(criticalOverToppingDischarge.Mean, input.CriticalOverToppingDischarge.Mean);
            AssertEqualValues(criticalOverToppingDischarge.StandardDeviation, input.CriticalOverToppingDischarge.StandardDeviation);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution stormDuration = GenerateLogNormalDistribution();

            RoundedDouble initialStd = input.StormDuration.StandardDeviation;

            //Call
            input.StormDuration = stormDuration;

            //Assert
            Assert.AreEqual(stormDuration.Mean, input.StormDuration.Mean);
            AssertEqualValues(initialStd, input.StormDuration.StandardDeviation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        public void Properties_ProbabilityOpenStructureBeforeFlooding_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        public void Properties_WaveDirectionDeviation_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            var wavedirectionDeviation = new RoundedDouble(5, random.NextDouble());

            // Call
            input.WavedirectionDeviation = wavedirectionDeviation;

            // Assert
            Assert.AreEqual(2, input.WavedirectionDeviation.NumberOfDecimalPlaces);
            AssertEqualValues(wavedirectionDeviation, input.WavedirectionDeviation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_ProbabilityOpenStructureBeforeFlooding_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            Assert.AreEqual(probability, input.ProbabilityOpenStructureBeforeFlooding);
        }

        private void AssertEqualValues(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static LogNormalDistribution GenerateLogNormalDistribution()
        {
            var random = new Random(22);
            return new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }

        private static NormalDistribution GenerateNormalDistribution()
        {
            var random = new Random(22);
            return new NormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }
    }
}