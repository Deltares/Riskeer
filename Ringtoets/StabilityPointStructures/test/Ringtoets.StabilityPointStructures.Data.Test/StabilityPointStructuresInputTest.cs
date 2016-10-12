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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

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
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var insideWaterLevel = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            var modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            var drainCoefficient = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            var flowVelocityStructureClosable = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 1
            };

            var levelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            var thresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var areaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.01
            };

            var constructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var constructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var stabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var stabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var failureCollisionEnergy = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.3
            };

            var shipMass = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var shipVelocity = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.2
            };

            var allowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var storageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            var criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.15
            };

            var widthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.05
            };

            var bankWidth = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) double.NaN
            };

            // Call
            var input = new StabilityPointStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            Assert.IsNull(input.StabilityPointStructure);

            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            AssertEqualValue(0, input.BreakWater.Height);
            Assert.AreEqual(2, input.BreakWater.Height.NumberOfDecimalPlaces);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);

            Assert.IsNaN(input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            AssertEqualValue(9.81, input.VolumicWeightWater);
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            DistributionAssert.AreEqual(insideWaterLevelFailureConstruction, input.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(insideWaterLevel, input.InsideWaterLevel);
            DistributionAssert.AreEqual(stormDuration, input.StormDuration);

            DistributionAssert.AreEqual(modelFactorSuperCriticalFlow, input.ModelFactorSuperCriticalFlow);
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
            Assert.IsNaN(input.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(failureCollisionEnergy, input.FailureCollisionEnergy);
            DistributionAssert.AreEqual(shipMass, input.ShipMass);
            DistributionAssert.AreEqual(shipVelocity, input.ShipVelocity);
            Assert.AreEqual(0, input.LevellingCount);
            Assert.AreEqual(double.NaN, input.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(allowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(storageStructureArea, input.StorageStructureArea);
            DistributionAssert.AreEqual(flowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(criticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
            Assert.IsNaN(input.FailureProbabilityStructureWithErosion);
            DistributionAssert.AreEqual(widthFlowApertures, input.WidthFlowApertures);
            DistributionAssert.AreEqual(bankWidth, input.BankWidth);
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertEqualValue(0, input.EvaluationLevel);
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertEqualValue(double.NaN, input.VerticalDistance);
        }

        # region Calculation inputs

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        public void InflowModelType_SetValue_ReturnSetValue(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.InflowModelType = inflowModelType;

            // Assert
            Assert.AreEqual(inflowModelType, input.InflowModelType);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void LoadSchematizationType_SetValue_ReturnSetValue(LoadSchematizationType type)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.LoadSchematizationType = type;

            // Assert
            Assert.AreEqual(type, input.LoadSchematizationType);
        }

        #endregion

        #region Hydraulic data

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreSame(location, input.HydraulicBoundaryLocation);
        }

        [Test]
        [Combinatorial]
        public void ForeshoreProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreshoreGeometry.Add(new Point2D(4.4, 5.5));
            }

            BreakWater breakWater = null;
            if (withBreakWater)
            {
                var nonDefaultBreakWaterType = BreakWaterType.Wall;
                var nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                breakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            double orientation = 96;
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        foreshoreGeometry.ToArray(),
                                                        breakWater,
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = orientation
                                                        });

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Foreshore_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(3.3, 4.4),
                                                            new Point2D(5.5, 6.6)
                                                        },
                                                        new BreakWater(BreakWaterType.Caisson, 2.2),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = 96
                                                        });

            input.ForeshoreProfile = foreshoreProfile;

            // Precondition
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.ForeshoreProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Properties_VolumicWeightWater_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);

            RoundedDouble volumicWeightWater = new RoundedDouble(5, random.NextDouble());

            // Call
            input.VolumicWeightWater = volumicWeightWater;

            // Assert
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            AssertEqualValue(volumicWeightWater, input.VolumicWeightWater);
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
                StandardDeviation =  standardDeviation
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();
            
            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
            RoundedDouble initialVariation = input.StormDuration.CoefficientOfVariation;
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = initialVariation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StormDuration = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.StormDuration, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Model inputs

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
            RoundedDouble initialStd = input.ModelFactorSuperCriticalFlow.StandardDeviation;
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = initialStd
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.ModelFactorSuperCriticalFlow = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ModelFactorSuperCriticalFlow, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_FactorStormDurationOpenStructure_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);

            var factorStormDuration = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FactorStormDurationOpenStructure = factorStormDuration;

            // Assert
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            AssertEqualValue(factorStormDuration, input.FactorStormDurationOpenStructure);
        }

        [Test]
        public void Properties_DrainCoefficient_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
            RoundedDouble initialStd = input.DrainCoefficient.StandardDeviation;
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = initialStd
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void Properties_StructureNormalOrientationValidValues_NewValueSet(double orientation)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertEqualValue(orientation, input.StructureNormalOrientation);
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Properties_StructureNormalOrientationInvalidValues_ThrowArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik tussen [0, 360] graden liggen.");
        }

        [Test]
        public void Properties_ThresholdHeightOpenWeir_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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
        public void Properties_LevellingCount_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);

            int levellingCount = random.Next();

            // Call
            input.LevellingCount = levellingCount;

            // Assert
            Assert.AreEqual(levellingCount, input.LevellingCount);
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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "Kans moet in het bereik [0, 1] opgegeven worden.");
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
        public void Properties_AllowedLevelIncreaseStorage_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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
            input.AllowedLevelIncreaseStorage = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.AllowedLevelIncreaseStorage, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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
            input.StorageStructureArea = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.StorageStructureArea, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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
            input.FlowWidthAtBottomProtection = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.FlowWidthAtBottomProtection, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_CriticalOvertoppingDischarge_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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
            input.CriticalOvertoppingDischarge = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.CriticalOvertoppingDischarge, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_FailureProbabilityStructureWithErosion_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityStructureWithErosion_ExpectedValues(double probability)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call 
            input.FailureProbabilityStructureWithErosion = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        public void Properties_WidthFlowApertures_ExpectedValues()
        {
            //Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var variation = (RoundedDouble)(0.01 + random.NextDouble());
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
            input.WidthFlowApertures = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.WidthFlowApertures, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_BankWidth_ExpectedValues()
        {
            // Setup 
            var random = new Random(22);
            var input = new StabilityPointStructuresInput();

            var mean = (RoundedDouble)(0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble)(0.01 + random.NextDouble());
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
            var input = new StabilityPointStructuresInput();

            var random = new Random(22);
            var evaluationLevel = new RoundedDouble(5, random.NextDouble());

            // Call
            input.EvaluationLevel = evaluationLevel;

            // Assert
            Assert.AreEqual(2, input.EvaluationLevel.NumberOfDecimalPlaces);
            AssertEqualValue(evaluationLevel, input.EvaluationLevel);
        }

        [Test]
        public void Properties_VerticalDistance_ExpectedValues()
        {
            // Setup 
            var input = new StabilityPointStructuresInput();

            var random = new Random(22);
            var verticalDistance = new RoundedDouble(5, random.NextDouble());

            // Call
            input.VerticalDistance = verticalDistance;

            // Assert
            Assert.AreEqual(2, input.VerticalDistance.NumberOfDecimalPlaces);
            AssertEqualValue(verticalDistance, input.VerticalDistance);
        }

        #endregion

        #region Helpers

        private void AssertEqualValue(double expectedValue, RoundedDouble actualValue)
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