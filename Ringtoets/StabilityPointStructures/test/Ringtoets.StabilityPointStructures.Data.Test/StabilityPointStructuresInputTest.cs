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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var insideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

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

            var flowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 1
            };

            var levelCrestStructure = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.05
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

            var constructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var constructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var stabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var stabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var failureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.3
            };

            var shipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var shipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var bankWidth = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            // Call
            var input = new StabilityPointStructuresInput();

            // Assert
            Assert.IsInstanceOf<StructuresInputBase<StabilityPointStructure>>(input);

            AssertAreEqual(9.81, input.VolumicWeightWater);
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            DistributionAssert.AreEqual(insideWaterLevelFailureConstruction, input.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(insideWaterLevel, input.InsideWaterLevel);

            Assert.IsNaN(input.FactorStormDurationOpenStructure);
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            DistributionAssert.AreEqual(drainCoefficient, input.DrainCoefficient);
            DistributionAssert.AreEqual(flowVelocityStructureClosable, input.FlowVelocityStructureClosable);

            DistributionAssert.AreEqual(levelCrestStructure, input.LevelCrestStructure);
            DistributionAssert.AreEqual(thresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(areaFlowApertures, input.AreaFlowApertures);
            DistributionAssert.AreEqual(constructiveStrengthLinearLoadModel, input.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(constructiveStrengthQuadraticLoadModel, input.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(stabilityLinearLoadModel, input.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(stabilityQuadraticLoadModel, input.StabilityQuadraticLoadModel);
            Assert.AreEqual(0, input.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(failureCollisionEnergy, input.FailureCollisionEnergy);
            DistributionAssert.AreEqual(shipMass, input.ShipMass);
            DistributionAssert.AreEqual(shipVelocity, input.ShipVelocity);
            Assert.AreEqual(0, input.LevellingCount);
            Assert.AreEqual(0, input.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(bankWidth, input.BankWidth);
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertAreEqual(0, input.EvaluationLevel);
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertAreEqual(double.NaN, input.VerticalDistance);
        }

        [Test]
        public void Input_StructureNull_DoesNotChangeValues()
        {
            var input = new StabilityPointStructuresInput();

            // Call
            input.Structure = null;

            // Assert
            AssertStabilityPointStructure(null, input);
        }

        [Test]
        public void Input_Structure_UpdateValuesAccordingly()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            TestStabilityPointStructure structure = new TestStabilityPointStructure();

            // Call
            input.Structure = structure;

            // Assert
            AssertStabilityPointStructure(structure, input);
        }

        #region Hydraulic data

        [Test]
        public void Properties_VolumicWeightWater_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);
            var volumicWeightWater = new RoundedDouble(5, random.NextDouble());

            // Call
            input.VolumicWeightWater = volumicWeightWater;

            // Assert
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            AssertAreEqual(volumicWeightWater, input.VolumicWeightWater);
        }

        [Test]
        public void Properties_InsideWaterLevelFailureConstruction_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
            input.InsideWaterLevelFailureConstruction = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.InsideWaterLevelFailureConstruction, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_InsideWaterLevel_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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

        #endregion

        #region Model factors and critical values

        [Test]
        public void Properties_FactorStormDurationOpenStructure_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);
            var factorStormDurationOpenStructure = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FactorStormDurationOpenStructure = factorStormDurationOpenStructure;

            // Assert
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            AssertAreEqual(factorStormDurationOpenStructure, input.FactorStormDurationOpenStructure);
        }

        [Test]
        public void Properties_DrainCoefficient_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = input.DrainCoefficient.StandardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.DrainCoefficient = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.DrainCoefficient, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_FlowVelocityStructureClosable_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
            input.FlowVelocityStructureClosable = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.FlowVelocityStructureClosable, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Schematization

        [Test]
        public void Properties_LevelCrestStructure_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
            input.LevelCrestStructure = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.LevelCrestStructure, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_ThresholdHeightOpenWeir_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
        public void Properties_AreaFlowApertures_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
        public void Properties_ConstructiveStrengthLinearLoadModel_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ConstructiveStrengthLinearLoadModel = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ConstructiveStrengthLinearLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_ConstructiveStrengthQuadraticLoadModel_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ConstructiveStrengthQuadraticLoadModel = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ConstructiveStrengthQuadraticLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_StabilityLinearLoadModel_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StabilityLinearLoadModel = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.StabilityLinearLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_StabilityQuadraticLoadModel()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StabilityQuadraticLoadModel = distributionToSet;

            // Assert 
            AssertDistributionCorrectlySet(input.StabilityQuadraticLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_FailureProbabilityRepairClosure_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityRepairClosure = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityRepairClosure_ExpectedValues(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call 
            input.FailureProbabilityRepairClosure = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityRepairClosure);
        }

        [Test]
        public void Properties_FailureCollisionEnergy_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.FailureCollisionEnergy = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.FailureCollisionEnergy, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_ShipMass_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ShipMass = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ShipMass, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_ShipVelocity_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ShipVelocity = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ShipVelocity, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_ProbabilityCollisionSecondaryStructure_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            TestDelegate call = () => input.ProbabilityCollisionSecondaryStructure = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "Kans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_ProbabilityCollisionSecondaryStructure_ExpectedValues(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call 
            input.ProbabilityCollisionSecondaryStructure = probability;

            // Assert
            Assert.AreEqual(probability, input.ProbabilityCollisionSecondaryStructure);
        }

        [Test]
        public void Properties_BankWidth_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
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
            input.BankWidth = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.BankWidth, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_EvaluationLevel_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var evaluationLevel = new RoundedDouble(5, random.NextDouble());

            // Call
            input.EvaluationLevel = evaluationLevel;

            // Assert
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertAreEqual(evaluationLevel, input.EvaluationLevel);
        }

        [Test]
        public void Properties_VerticalDistance_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var verticalDistance = new RoundedDouble(5, random.NextDouble());

            // Call
            input.VerticalDistance = verticalDistance;

            // Assert
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertAreEqual(verticalDistance, input.VerticalDistance);
        }

        #endregion

        #region Helpers

        private static void AssertStabilityPointStructure(StabilityPointStructure expectedStructure, StabilityPointStructuresInput input)
        {
            if (expectedStructure == null)
            {
                Assert.IsNull(input.Structure);
                var defaultInput = new StabilityPointStructuresInput();
                AssertAreEqual(defaultInput.StructureNormalOrientation, input.StructureNormalOrientation);

                DistributionAssert.AreEqual(defaultInput.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
                DistributionAssert.AreEqual(defaultInput.AreaFlowApertures, input.AreaFlowApertures);
                DistributionAssert.AreEqual(defaultInput.BankWidth, input.BankWidth);
                DistributionAssert.AreEqual(defaultInput.ConstructiveStrengthLinearLoadModel, input.ConstructiveStrengthLinearLoadModel);
                DistributionAssert.AreEqual(defaultInput.ConstructiveStrengthQuadraticLoadModel, input.ConstructiveStrengthQuadraticLoadModel);
                DistributionAssert.AreEqual(defaultInput.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                AssertAreEqual(defaultInput.EvaluationLevel, input.EvaluationLevel);
                DistributionAssert.AreEqual(defaultInput.FailureCollisionEnergy, input.FailureCollisionEnergy);
                Assert.AreEqual(defaultInput.FailureProbabilityRepairClosure, input.FailureProbabilityRepairClosure);
                DistributionAssert.AreEqual(defaultInput.FlowVelocityStructureClosable, input.FlowVelocityStructureClosable);
                DistributionAssert.AreEqual(defaultInput.FlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
                Assert.AreEqual(defaultInput.InflowModelType, input.InflowModelType);
                DistributionAssert.AreEqual(defaultInput.InsideWaterLevel, input.InsideWaterLevel);
                DistributionAssert.AreEqual(defaultInput.InsideWaterLevelFailureConstruction, input.InsideWaterLevelFailureConstruction);
                DistributionAssert.AreEqual(defaultInput.LevelCrestStructure, input.LevelCrestStructure);
                Assert.AreEqual(defaultInput.LevellingCount, input.LevellingCount);
                Assert.AreEqual(defaultInput.ProbabilityCollisionSecondaryStructure, input.ProbabilityCollisionSecondaryStructure);
                DistributionAssert.AreEqual(defaultInput.ShipMass, input.ShipMass);
                DistributionAssert.AreEqual(defaultInput.ShipVelocity, input.ShipVelocity);
                DistributionAssert.AreEqual(defaultInput.StabilityLinearLoadModel, input.StabilityLinearLoadModel);
                DistributionAssert.AreEqual(defaultInput.StabilityQuadraticLoadModel, input.StabilityQuadraticLoadModel);
                DistributionAssert.AreEqual(defaultInput.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(defaultInput.ThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
                AssertAreEqual(defaultInput.VerticalDistance, input.VerticalDistance);
                DistributionAssert.AreEqual(defaultInput.WidthFlowApertures, input.WidthFlowApertures);
            }
            else
            {
                DistributionAssert.AreEqual(expectedStructure.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
                DistributionAssert.AreEqual(expectedStructure.AreaFlowApertures, input.AreaFlowApertures);
                DistributionAssert.AreEqual(expectedStructure.BankWidth, input.BankWidth);
                DistributionAssert.AreEqual(expectedStructure.ConstructiveStrengthLinearLoadModel, input.ConstructiveStrengthLinearLoadModel);
                DistributionAssert.AreEqual(expectedStructure.ConstructiveStrengthQuadraticLoadModel, input.ConstructiveStrengthQuadraticLoadModel);
                DistributionAssert.AreEqual(expectedStructure.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                AssertAreEqual(expectedStructure.EvaluationLevel, input.EvaluationLevel);
                DistributionAssert.AreEqual(expectedStructure.FailureCollisionEnergy, input.FailureCollisionEnergy);
                Assert.AreEqual(expectedStructure.FailureProbabilityRepairClosure, input.FailureProbabilityRepairClosure);
                DistributionAssert.AreEqual(expectedStructure.FlowVelocityStructureClosable, input.FlowVelocityStructureClosable);
                DistributionAssert.AreEqual(expectedStructure.FlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
                Assert.AreEqual(expectedStructure.InflowModelType, input.InflowModelType);
                DistributionAssert.AreEqual(expectedStructure.InsideWaterLevel, input.InsideWaterLevel);
                DistributionAssert.AreEqual(expectedStructure.InsideWaterLevelFailureConstruction, input.InsideWaterLevelFailureConstruction);
                DistributionAssert.AreEqual(expectedStructure.LevelCrestStructure, input.LevelCrestStructure);
                Assert.AreEqual(expectedStructure.LevellingCount, input.LevellingCount);
                Assert.AreEqual(expectedStructure.ProbabilityCollisionSecondaryStructure, input.ProbabilityCollisionSecondaryStructure);
                DistributionAssert.AreEqual(expectedStructure.ShipMass, input.ShipMass);
                DistributionAssert.AreEqual(expectedStructure.ShipVelocity, input.ShipVelocity);
                DistributionAssert.AreEqual(expectedStructure.StabilityLinearLoadModel, input.StabilityLinearLoadModel);
                DistributionAssert.AreEqual(expectedStructure.StabilityQuadraticLoadModel, input.StabilityQuadraticLoadModel);
                DistributionAssert.AreEqual(expectedStructure.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(expectedStructure.ThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
                AssertAreEqual(expectedStructure.VerticalDistance, input.VerticalDistance);
                DistributionAssert.AreEqual(expectedStructure.WidthFlowApertures, input.WidthFlowApertures);
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

        private static void AssertDistributionCorrectlySet(IVariationCoefficientDistribution distributionToAssert, IVariationCoefficientDistribution setDistribution, IVariationCoefficientDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }

        #endregion
    }
}