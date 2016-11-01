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
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var insideWaterLevel = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            var thresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var areaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            var levelCrestStructureNotClosing = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            // Call
            var input = new ClosingStructuresInput();

            // Assert
            Assert.IsInstanceOf<StructuresInputBase<ClosingStructure>>(input);

            Assert.AreEqual(0, input.FailureProbabilityOpenStructure);
            Assert.AreEqual(0, input.FailureProbabilityReparation);

            Assert.IsNaN(input.FactorStormDurationOpenStructure);
            Assert.IsNaN(input.DeviationWaveDirection);

            DistributionAssert.AreEqual(insideWaterLevel, input.InsideWaterLevel);
            DistributionAssert.AreEqual(drainCoefficient, input.DrainCoefficient);
            DistributionAssert.AreEqual(thresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(areaFlowApertures, input.AreaFlowApertures);
            DistributionAssert.AreEqual(levelCrestStructureNotClosing, input.LevelCrestStructureNotClosing);

            Assert.AreEqual(1.0, input.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(0, input.IdenticalApertures);
        }

        [Test]
        public void Structure_Null_ExpectedValues()
        {
            var input = new ClosingStructuresInput();

            // Call
            input.Structure = null;

            // Assert
            AssertClosingStructure(null, input);
        }

        [Test]
        public void Structure_NotNull_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            TestClosingStructure structure = new TestClosingStructure();

            // Call
            input.Structure = structure;

            // Assert
            AssertClosingStructure(structure, input);
        }

        #region Hydraulic data

        [Test]
        public void InsideWaterLevel_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.InsideWaterLevel = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.InsideWaterLevel, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-360.005)]
        [TestCase(-400)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void DeviationWaveDirection_InvalidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.DeviationWaveDirection = (RoundedDouble) invalidValue;

            // Assert
            const string expectedMessage = "De waarde voor de afwijking van de golfrichting moet in het bereik [-360, 360] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call,
                                                                                                expectedMessage);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-360.004)]
        [TestCase(double.NaN)]
        public void DeviationWaveDirection_ValidValues_ExpectedValues(double validValue)
        {
            // Setup
            var input = new ClosingStructuresInput();
            var deviationWaveDirection = new RoundedDouble(5, validValue);

            // Call
            input.DeviationWaveDirection = deviationWaveDirection;

            // Assert
            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            AssertAreEqual(deviationWaveDirection, input.DeviationWaveDirection);
        }

        #endregion

        #region Model factors

        [Test]
        public void DrainCoefficient_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = input.DrainCoefficient.StandardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.DrainCoefficient = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.DrainCoefficient, distributionToSet, expectedDistribution);
        }

        [Test]
        public void FactorStormDurationOpenStructure_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var factorStormDurationOpenStructure = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FactorStormDurationOpenStructure = factorStormDurationOpenStructure;

            // Assert
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            AssertAreEqual(factorStormDurationOpenStructure, input.FactorStormDurationOpenStructure);
        }

        #endregion

        #region Schematization

        [Test]
        public void ThresholdHeightOpenWeir_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.ThresholdHeightOpenWeir = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ThresholdHeightOpenWeir, distributionToSet, expectedDistribution);
        }

        [Test]
        public void AreaFlowApertures_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.AreaFlowApertures = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.AreaFlowApertures, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void FailureProbabilityOpenStructure_InvalidValues_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityOpenStructure = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void FailureProbabilityOpenStructure_ValidValues_ExpectedValues(double probability)
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
        [TestCase(double.NaN)]
        public void FailureProbabilityReparation_InvalidValues_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityReparation = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void FailureProbabilityReparation_ValidValues_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityReparation = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityReparation);
        }

        [Test]
        public void LevelCrestStructureNotClosing_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new ClosingStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.LevelCrestStructureNotClosing = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.LevelCrestStructureNotClosing, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void ProbabilityOpenStructureBeforeFlooding_InvalidValues_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void ProbabilityOpenStructureBeforeFlooding_ValidValues_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            Assert.AreEqual(probability, input.ProbabilityOpenStructureBeforeFlooding);
        }

        #endregion

        #region Helpers

        private static void AssertClosingStructure(ClosingStructure expectedClosingStructure, ClosingStructuresInput input)
        {
            if (expectedClosingStructure == null)
            {
                Assert.IsNull(input.Structure);
                var defaultInput = new ClosingStructuresInput();
                AssertAreEqual(defaultInput.StructureNormalOrientation, input.StructureNormalOrientation);

                DistributionAssert.AreEqual(defaultInput.LevelCrestStructureNotClosing, input.LevelCrestStructureNotClosing);
                DistributionAssert.AreEqual(defaultInput.FlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
                DistributionAssert.AreEqual(defaultInput.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                DistributionAssert.AreEqual(defaultInput.WidthFlowApertures, input.WidthFlowApertures);
                DistributionAssert.AreEqual(defaultInput.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(defaultInput.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
                Assert.AreEqual(defaultInput.InflowModelType, input.InflowModelType);
                DistributionAssert.AreEqual(defaultInput.AreaFlowApertures, input.AreaFlowApertures);
                Assert.AreEqual(defaultInput.FailureProbabilityOpenStructure, input.FailureProbabilityOpenStructure);
                Assert.AreEqual(defaultInput.FailureProbabilityReparation, input.FailureProbabilityReparation);
                Assert.AreEqual(defaultInput.IdenticalApertures, input.IdenticalApertures);
                DistributionAssert.AreEqual(defaultInput.InsideWaterLevel, input.InsideWaterLevel);
                Assert.AreEqual(defaultInput.ProbabilityOpenStructureBeforeFlooding, input.ProbabilityOpenStructureBeforeFlooding);
                DistributionAssert.AreEqual(defaultInput.ThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            }
            else
            {
                AssertAreEqual(expectedClosingStructure.StructureNormalOrientation, input.StructureNormalOrientation);

                DistributionAssert.AreEqual(expectedClosingStructure.LevelCrestStructureNotClosing, input.LevelCrestStructureNotClosing);
                DistributionAssert.AreEqual(expectedClosingStructure.FlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
                DistributionAssert.AreEqual(expectedClosingStructure.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                DistributionAssert.AreEqual(expectedClosingStructure.WidthFlowApertures, input.WidthFlowApertures);
                DistributionAssert.AreEqual(expectedClosingStructure.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(expectedClosingStructure.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
                Assert.AreEqual(expectedClosingStructure.InflowModelType, input.InflowModelType);
                DistributionAssert.AreEqual(expectedClosingStructure.AreaFlowApertures, input.AreaFlowApertures);
                Assert.AreEqual(expectedClosingStructure.FailureProbabilityOpenStructure, input.FailureProbabilityOpenStructure);
                Assert.AreEqual(expectedClosingStructure.FailureProbabilityReparation, input.FailureProbabilityReparation);
                Assert.AreEqual(expectedClosingStructure.IdenticalApertures, input.IdenticalApertures);
                DistributionAssert.AreEqual(expectedClosingStructure.InsideWaterLevel, input.InsideWaterLevel);
                Assert.AreEqual(expectedClosingStructure.ProbabilityOpenStructureBeforeFlooding, input.ProbabilityOpenStructureBeforeFlooding);
                DistributionAssert.AreEqual(expectedClosingStructure.ThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            }
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static void AssertDistributionCorrectlySet(IDistribution distributionToAssert, IDistribution setDistribution, IDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }

        #endregion
    }
}