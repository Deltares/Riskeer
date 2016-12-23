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
            // Call
            var input = new StabilityPointStructuresInput();

            // Assert
            Assert.IsInstanceOf<StructuresInputBase<StabilityPointStructure>>(input);

            Assert.AreEqual(LoadSchematizationType.Linear, input.LoadSchematizationType);

            AssertAreEqual(9.81, input.VolumicWeightWater);
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);

            var expectedInsideWaterLevelFailureConstruction = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)0.1
            };

            var expectedInsideWaterLevel = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)0.1
            };

            var expectedDrainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble)1,
                StandardDeviation = (RoundedDouble)0.2
            };

            var expectedFlowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)1
            };

            var expectedLevelCrestStructure = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)0.05
            };

            var expectedThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)0.1
            };

            var expectedAreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble)0.01
            };

            var expectedConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.1
            };

            var expectedConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.1
            };

            var expectedStabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.1
            };

            var expectedStabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.1
            };

            var expectedFailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.3
            };

            var expectedShipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.2
            };

            var expectedShipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble)0.2
            };

            var expectedBankWidth = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            DistributionAssert.AreEqual(expectedInsideWaterLevelFailureConstruction, input.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(expectedInsideWaterLevel, input.InsideWaterLevel);

            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, input.FactorStormDurationOpenStructure, input.FactorStormDurationOpenStructure.GetAccuracy());
            DistributionAssert.AreEqual(expectedDrainCoefficient, input.DrainCoefficient);
            DistributionAssert.AreEqual(expectedFlowVelocityStructureClosable, input.FlowVelocityStructureClosable);

            DistributionAssert.AreEqual(expectedLevelCrestStructure, input.LevelCrestStructure);
            DistributionAssert.AreEqual(expectedThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedAreaFlowApertures, input.AreaFlowApertures);
            DistributionAssert.AreEqual(expectedConstructiveStrengthLinearLoadModel, input.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedConstructiveStrengthQuadraticLoadModel, input.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityLinearLoadModel, input.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityQuadraticLoadModel, input.StabilityQuadraticLoadModel);
            Assert.AreEqual(0, input.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(expectedFailureCollisionEnergy, input.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedShipMass, input.ShipMass);
            DistributionAssert.AreEqual(expectedShipVelocity, input.ShipVelocity);
            Assert.AreEqual(0, input.LevellingCount);
            Assert.AreEqual(0, input.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(expectedBankWidth, input.BankWidth);
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertAreEqual(0, input.EvaluationLevel);
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertAreEqual(double.NaN, input.VerticalDistance);
        }

        [Test]
        public void Structure_Null_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.Structure = null;

            // Assert
            AssertStabilityPointStructure(null, input);
        }

        [Test]
        public void GivenInputWithStructure_WhenStructureNull_ThenSchematizationPropertiesSynedToDefaults()
        {
            // Given
            var structure = new TestStabilityPointStructure();
            var input = new StabilityPointStructuresInput
            {
                Structure = structure
            };

            RoundedDouble expectedVolumicWeightWater = input.VolumicWeightWater;
            NormalDistribution expectedDrainCoefficient = input.DrainCoefficient;
            RoundedDouble expectedFactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure;

            // Pre-condition
            AssertStabilityPointStructure(structure, input);

            // When
            input.Structure = null;

            // Then
            AssertAreEqual(expectedVolumicWeightWater, input.VolumicWeightWater);
            DistributionAssert.AreEqual(structure.InsideWaterLevelFailureConstruction,
                                        input.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(structure.InsideWaterLevel, input.InsideWaterLevel);

            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedFactorStormDurationOpenStructure, input.FactorStormDurationOpenStructure,
                            input.FactorStormDurationOpenStructure.GetAccuracy());
            DistributionAssert.AreEqual(structure.FlowVelocityStructureClosable, input.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedDrainCoefficient, input.DrainCoefficient);

            var expectedLevelCrestStructure = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            var expectedThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var expectedAreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            var expectedConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var expectedConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var expectedStabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var expectedStabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var expectedFailureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.3
            };

            var expectedShipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var expectedShipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var expectedBankWidth = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            DistributionAssert.AreEqual(expectedLevelCrestStructure, input.LevelCrestStructure);
            DistributionAssert.AreEqual(expectedThresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedAreaFlowApertures, input.AreaFlowApertures);
            DistributionAssert.AreEqual(expectedConstructiveStrengthLinearLoadModel, input.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedConstructiveStrengthQuadraticLoadModel, input.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityLinearLoadModel, input.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityQuadraticLoadModel, input.StabilityQuadraticLoadModel);
            Assert.AreEqual(0, input.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(expectedFailureCollisionEnergy, input.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedShipMass, input.ShipMass);
            DistributionAssert.AreEqual(expectedShipVelocity, input.ShipVelocity);
            Assert.AreEqual(0, input.LevellingCount);
            Assert.AreEqual(0, input.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(expectedBankWidth, input.BankWidth);
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertAreEqual(0, input.EvaluationLevel);
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertAreEqual(double.NaN, input.VerticalDistance);
        }

        [Test]
        public void Structure_NotNull_ExpectedValues()
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
        public void VolumicWeightWater_Always_ExpectedValues()
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
        public void InsideWaterLevelFailureConstruction_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.InsideWaterLevelFailureConstruction = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.InsideWaterLevelFailureConstruction, distributionToSet, expectedDistribution);
        }

        [Test]
        public void InsideWaterLevel_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.InsideWaterLevel = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.InsideWaterLevel, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Model factors and critical values

        [Test]
        public void FactorStormDurationOpenStructure_Always_ExpectedValues()
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
        public void DrainCoefficient_Always_ExpectedValues()
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
        public void FlowVelocityStructureClosable_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.FlowVelocityStructureClosable = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.FlowVelocityStructureClosable, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Schematization

        [Test]
        public void LevelCrestStructure_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.LevelCrestStructure = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.LevelCrestStructure, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ThresholdHeightOpenWeir_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.ThresholdHeightOpenWeir = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.ThresholdHeightOpenWeir, distributionToSet, expectedDistribution);
        }

        [Test]
        public void AreaFlowApertures_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.AreaFlowApertures = distributionToSet;

            // Assert
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.AreaFlowApertures, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ConstructiveStrengthLinearLoadModel_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ConstructiveStrengthLinearLoadModel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.ConstructiveStrengthLinearLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ConstructiveStrengthQuadraticLoadModel_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ConstructiveStrengthQuadraticLoadModel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.ConstructiveStrengthQuadraticLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void StabilityLinearLoadModel_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StabilityLinearLoadModel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.StabilityLinearLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void StabilityQuadraticLoadModel_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StabilityQuadraticLoadModel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.StabilityQuadraticLoadModel, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void FailureProbabilityRepairClosure_InvalidValues_ThrowsArgumentOutOfRangeException(double probability)
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
        public void FailureProbabilityRepairClosure_ValidValues_ExpectedValues(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call 
            input.FailureProbabilityRepairClosure = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityRepairClosure);
        }

        [Test]
        public void FailureCollisionEnergy_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.FailureCollisionEnergy = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.FailureCollisionEnergy, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ShipMass_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ShipMass = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.ShipMass, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ShipVelocity_Always_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.ShipVelocity = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            AssertDistributionCorrectlySet(input.ShipVelocity, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void ProbabilityCollisionSecondaryStructure_InvalidValues_ThrowsArgumentOutOfRangeException(double probability)
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
        public void ProbabilityCollisionSecondaryStructure_ValidValues_ExpectedValues(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call 
            input.ProbabilityCollisionSecondaryStructure = probability;

            // Assert
            Assert.AreEqual(probability, input.ProbabilityCollisionSecondaryStructure);
        }

        [Test]
        public void BankWidth_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.BankWidth = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.BankWidth, distributionToSet, expectedDistribution);
        }

        [Test]
        public void EvaluationLevel_Always_ExpectedValues()
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
        public void VerticalDistance_Always_ExpectedValues()
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
                Assert.AreEqual(defaultInput.StructureNormalOrientation, input.StructureNormalOrientation);
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
                AssertAreEqual(expectedStructure.StructureNormalOrientation, input.StructureNormalOrientation);
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