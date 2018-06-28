// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create
{
    [TestFixture]
    public class StructuresCalculationCreateExtensionsTest
    {
        private static void AssertStructuresOutputEntity<T>(StructuresOutput output, T outputEntity)
            where T : IHasGeneralResultFaultTreeIllustrationPointEntity, IStructuresOutputEntity
        {
            Assert.AreEqual(output.Reliability, outputEntity.Reliability);

            Assert.IsNull(outputEntity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        #region CreateForHeightStructures

        [Test]
        public void CreateForHeightStructures_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            TestDelegate call = () => calculation.CreateForHeightStructures(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase("I have no comments", null, 0, 827364)]
        [TestCase("I have a comment", "I am comment", 98, 231)]
        public void CreateForHeightStructures_ValidCalculation_ReturnEntity(string name, string comments, int order, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = name,
                Comments =
                {
                    Body = comments
                },
                InputParameters =
                {
                    StructureNormalOrientation = random.NextRoundedDouble(0, 360),
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = random.NextRoundedDouble(-9999.9999, 9999.9999)
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    StorageStructureArea =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        CoefficientOfVariation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        CoefficientOfVariation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    FailureProbabilityStructureWithErosion = random.NextDouble(),
                    WidthFlowApertures =
                    {
                        Mean = random.NextRoundedDouble(-9999.9999, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    StormDuration =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        CoefficientOfVariation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    DeviationWaveDirection = random.NextRoundedDouble(),
                    LevelCrestStructure =
                    {
                        Mean = random.NextRoundedDouble(-9999.9999, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    UseBreakWater = true,
                    UseForeshore = false,
                    BreakWater =
                    {
                        Height = random.NextRoundedDouble(),
                        Type = BreakWaterType.Dam
                    },
                    ShouldIllustrationPointsBeCalculated = random.NextBoolean()
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, order);

            // Assert
            Assert.AreEqual(0, entity.HeightStructuresCalculationEntityId);
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(comments, entity.Comments);
            Assert.AreEqual(order, entity.Order);

            HeightStructuresInput input = calculation.InputParameters;
            Assert.AreEqual(input.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(input.ModelFactorSuperCriticalFlow.Mean.Value, entity.ModelFactorSuperCriticalFlowMean);
            Assert.AreEqual(input.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(input.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(input.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(input.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(input.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(input.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(input.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(input.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(input.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(input.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);
            Assert.AreEqual(input.StormDuration.Mean.Value, entity.StormDurationMean);

            Assert.AreEqual(input.LevelCrestStructure.Mean.Value, entity.LevelCrestStructureMean);
            Assert.AreEqual(input.LevelCrestStructure.StandardDeviation.Value, entity.LevelCrestStructureStandardDeviation);
            Assert.AreEqual(input.DeviationWaveDirection.Value, entity.DeviationWaveDirection);

            Assert.IsNull(entity.CalculationGroupEntity);
            Assert.IsNull(entity.ForeshoreProfileEntityId);
            Assert.IsNull(entity.HeightStructureEntityId);
            Assert.IsNull(entity.HydraulicLocationEntityId);

            Assert.AreEqual(input.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual((short) input.BreakWater.Type, entity.BreakWaterType);
            Assert.AreEqual(Convert.ToByte(input.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToByte(input.UseForeshore), entity.UseForeshore);

            Assert.AreEqual(Convert.ToByte(input.ShouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);

            CollectionAssert.IsEmpty(entity.HeightStructuresOutputEntities);
        }

        [Test]
        public void CreateForHeightStructures_NaNParameters_EntityWithNullFields()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    StructureNormalOrientation = RoundedDouble.NaN,
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    StorageStructureArea =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    WidthFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    StormDuration =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    LevelCrestStructure =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    DeviationWaveDirection = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Height = RoundedDouble.NaN
                    }
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.ModelFactorSuperCriticalFlowMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.DeviationWaveDirection);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);

            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void CreateForHeightStructures_CalculationWithAlreadySavedHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void CreateForHeightStructures_CalculationWithAlreadySavedHeightStructure_ReturnEntityWithHeightStructureEntity()
        {
            // Setup
            var heightStructure = new TestHeightStructure();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = heightStructure
                }
            };

            var heightStructureEntity = new HeightStructureEntity();
            var registry = new PersistenceRegistry();
            registry.Register(heightStructureEntity, heightStructure);

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.AreSame(heightStructureEntity, entity.HeightStructureEntity);
        }

        [Test]
        public void CreateForHeightStructures_CalculationWithAlreadySavedForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };

            var foreshoreProfileEntity = new ForeshoreProfileEntity();
            var registry = new PersistenceRegistry();
            registry.Register(foreshoreProfileEntity, foreshoreProfile);

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.AreSame(foreshoreProfileEntity, entity.ForeshoreProfileEntity);
        }

        [Test]
        public void CreateForHeightStructures_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            AssertStructuresOutputEntity(calculation.Output, entity.HeightStructuresOutputEntities.Single());
        }

        #endregion

        #region CreateForClosingStructures

        [Test]
        public void CreateForClosingStructures_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            TestDelegate call = () => calculation.CreateForClosingStructures(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void CreateForClosingStructures_ValidCalculation_ReturnClosingStructuresCalculationEntity()
        {
            // Setup
            var random = new Random(45);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = "A",
                Comments =
                {
                    Body = "B"
                },
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    StructureNormalOrientation = random.NextRoundedDouble(),
                    FailureProbabilityStructureWithErosion = random.NextRoundedDouble(),
                    UseForeshore = random.NextBoolean(),
                    UseBreakWater = random.NextBoolean(),
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = random.NextRoundedDouble()
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    StorageStructureArea =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    WidthFlowApertures =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    InflowModelType = ClosingStructureInflowModelType.VerticalWall,
                    InsideWaterLevel =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    DeviationWaveDirection = random.NextRoundedDouble(),
                    DrainCoefficient =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    FactorStormDurationOpenStructure = random.NextRoundedDouble(),
                    ThresholdHeightOpenWeir =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    AreaFlowApertures =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    FailureProbabilityOpenStructure = random.NextRoundedDouble(),
                    FailureProbabilityReparation = random.NextRoundedDouble(),
                    IdenticalApertures = random.Next(),
                    LevelCrestStructureNotClosing =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    ProbabilityOrFrequencyOpenStructureBeforeFlooding = random.NextRoundedDouble(),
                    ShouldIllustrationPointsBeCalculated = random.NextBoolean()
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(calculation.Name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(calculation.Comments.Body, entity.Comments);

            ClosingStructuresInput inputParameters = calculation.InputParameters;
            Assert.AreEqual(inputParameters.StormDuration.Mean.Value, entity.StormDurationMean);
            Assert.AreEqual(inputParameters.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(inputParameters.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.IsNull(entity.ClosingStructureEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            Assert.IsNull(entity.ForeshoreProfileEntity);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseForeshore), entity.UseForeshore);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToInt16(inputParameters.BreakWater.Type), entity.BreakWaterType);
            Assert.AreEqual(inputParameters.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(inputParameters.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(inputParameters.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ModelFactorSuperCriticalFlow.Mean.Value, entity.ModelFactorSuperCriticalFlowMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);
            Assert.AreEqual(Convert.ToByte(inputParameters.InflowModelType), entity.InflowModelType);
            Assert.AreEqual(inputParameters.InsideWaterLevel.Mean.Value, entity.InsideWaterLevelMean);
            Assert.AreEqual(inputParameters.InsideWaterLevel.StandardDeviation.Value, entity.InsideWaterLevelStandardDeviation);
            Assert.AreEqual(inputParameters.DeviationWaveDirection.Value, entity.DeviationWaveDirection);
            Assert.AreEqual(inputParameters.DrainCoefficient.Mean.Value, entity.DrainCoefficientMean);
            Assert.AreEqual(inputParameters.FactorStormDurationOpenStructure.Value, entity.FactorStormDurationOpenStructure);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.Mean.Value, entity.ThresholdHeightOpenWeirMean);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.StandardDeviation.Value, entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.AreEqual(inputParameters.AreaFlowApertures.Mean.Value, entity.AreaFlowAperturesMean);
            Assert.AreEqual(inputParameters.AreaFlowApertures.StandardDeviation.Value, entity.AreaFlowAperturesStandardDeviation);
            Assert.AreEqual(inputParameters.FailureProbabilityOpenStructure, entity.FailureProbabilityOpenStructure);
            Assert.AreEqual(inputParameters.FailureProbabilityReparation, entity.FailureProbabilityReparation);
            Assert.AreEqual(inputParameters.IdenticalApertures, entity.IdenticalApertures);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.Mean.Value, entity.LevelCrestStructureNotClosingMean);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.StandardDeviation.Value, entity.LevelCrestStructureNotClosingStandardDeviation);
            Assert.AreEqual(inputParameters.ProbabilityOrFrequencyOpenStructureBeforeFlooding, entity.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            Assert.AreEqual(Convert.ToByte(inputParameters.ShouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);
            Assert.AreEqual(order, entity.Order);

            CollectionAssert.IsEmpty(entity.ClosingStructuresOutputEntities);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithParametersNaN_ReturnEntityWithNullParameters()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    StructureNormalOrientation = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Height = RoundedDouble.NaN
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    StorageStructureArea =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    WidthFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    InsideWaterLevel =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    DeviationWaveDirection = RoundedDouble.NaN,
                    DrainCoefficient =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    FactorStormDurationOpenStructure = RoundedDouble.NaN,
                    ThresholdHeightOpenWeir =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    AreaFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    }
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, order);

            // Assert
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.BreakWaterHeight);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.ModelFactorSuperCriticalFlowMean);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);
            Assert.IsNull(entity.InsideWaterLevelMean);
            Assert.IsNull(entity.InsideWaterLevelStandardDeviation);
            Assert.IsNull(entity.DeviationWaveDirection);
            Assert.IsNull(entity.DrainCoefficientMean);
            Assert.IsNull(entity.FactorStormDurationOpenStructure);
            Assert.IsNull(entity.ThresholdHeightOpenWeirMean);
            Assert.IsNull(entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.IsNull(entity.AreaFlowAperturesMean);
            Assert.IsNull(entity.AreaFlowAperturesStandardDeviation);
            Assert.IsNull(entity.LevelCrestStructureNotClosingMean);
            Assert.IsNull(entity.LevelCrestStructureNotClosingStandardDeviation);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithStructure_ReturnEntityWithStructureEntity()
        {
            // Setup
            ClosingStructure alreadyRegisteredStructure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = alreadyRegisteredStructure
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructureEntity(), alreadyRegisteredStructure);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ClosingStructureEntity);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var alreadyRegisteredHydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = alreadyRegisteredHydraulicBoundaryLocation
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new HydraulicLocationEntity(), alreadyRegisteredHydraulicBoundaryLocation);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.HydraulicLocationEntity);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var alreadyRegisteredForeshoreProfile = new TestForeshoreProfile();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = alreadyRegisteredForeshoreProfile
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new ForeshoreProfileEntity(), alreadyRegisteredForeshoreProfile);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ForeshoreProfileEntity);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var registry = new PersistenceRegistry();

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, 0);

            // Assert
            AssertStructuresOutputEntity(calculation.Output, entity.ClosingStructuresOutputEntities.Single());
        }

        #endregion

        #region CreateForStabilityPointStructures

        [Test]
        public void CreateForStabilityPointStructures_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            // Call
            TestDelegate call = () => calculation.CreateForStabilityPointStructures(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void CreateForStabilityPointStructures_ValidCalculation_ReturnStabilityPointStructuresCalculationEntity()
        {
            // Setup
            var random = new Random(45);
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "A",
                Comments =
                {
                    Body = "B"
                },
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    StructureNormalOrientation = random.NextRoundedDouble(),
                    FailureProbabilityStructureWithErosion = random.NextDouble(),
                    UseForeshore = random.NextBoolean(),
                    UseBreakWater = random.NextBoolean(),
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = random.NextRoundedDouble()
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    StorageStructureArea =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    WidthFlowApertures =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    InsideWaterLevel =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    BankWidth =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    EvaluationLevel = random.NextRoundedDouble(),
                    LevelCrestStructure =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    VerticalDistance = random.NextRoundedDouble(),
                    FailureProbabilityRepairClosure = random.NextDouble(),
                    FailureCollisionEnergy =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ShipMass =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ShipVelocity =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    LevellingCount = random.Next(),
                    ProbabilityCollisionSecondaryStructure = random.NextDouble(),
                    FlowVelocityStructureClosable =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    AreaFlowApertures =
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic,
                    VolumicWeightWater = random.NextRoundedDouble(),
                    FactorStormDurationOpenStructure = random.NextRoundedDouble(),
                    DrainCoefficient =
                    {
                        Mean = random.NextRoundedDouble()
                    },
                    ShouldIllustrationPointsBeCalculated = random.NextBoolean()
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(calculation.Name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(calculation.Comments.Body, entity.Comments);

            StabilityPointStructuresInput inputParameters = calculation.InputParameters;
            Assert.AreEqual(inputParameters.StormDuration.Mean.Value, entity.StormDurationMean);
            Assert.AreEqual(inputParameters.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(inputParameters.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.IsNull(entity.StabilityPointStructureEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            Assert.IsNull(entity.ForeshoreProfileEntity);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseForeshore), entity.UseForeshore);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToInt16(inputParameters.BreakWater.Type), entity.BreakWaterType);
            Assert.AreEqual(inputParameters.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(inputParameters.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(inputParameters.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ModelFactorSuperCriticalFlow.Mean.Value, entity.ModelFactorSuperCriticalFlowMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);

            Assert.AreEqual(inputParameters.InsideWaterLevel.Mean.Value, entity.InsideWaterLevelMean);
            Assert.AreEqual(inputParameters.InsideWaterLevel.StandardDeviation.Value, entity.InsideWaterLevelStandardDeviation);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.Mean.Value, entity.ThresholdHeightOpenWeirMean);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.StandardDeviation.Value, entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.AreEqual(inputParameters.ConstructiveStrengthLinearLoadModel.Mean.Value, entity.ConstructiveStrengthLinearLoadModelMean);
            Assert.AreEqual(inputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value, entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ConstructiveStrengthQuadraticLoadModel.Mean.Value, entity.ConstructiveStrengthQuadraticLoadModelMean);
            Assert.AreEqual(inputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value, entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation);
            Assert.AreEqual(inputParameters.BankWidth.Mean.Value, entity.BankWidthMean);
            Assert.AreEqual(inputParameters.BankWidth.StandardDeviation.Value, entity.BankWidthStandardDeviation);
            Assert.AreEqual(inputParameters.InsideWaterLevelFailureConstruction.Mean.Value, entity.InsideWaterLevelFailureConstructionMean);
            Assert.AreEqual(inputParameters.InsideWaterLevelFailureConstruction.StandardDeviation.Value, entity.InsideWaterLevelFailureConstructionStandardDeviation);
            Assert.AreEqual(inputParameters.EvaluationLevel.Value, entity.EvaluationLevel);
            Assert.AreEqual(inputParameters.LevelCrestStructure.Mean.Value, entity.LevelCrestStructureMean);
            Assert.AreEqual(inputParameters.LevelCrestStructure.StandardDeviation.Value, entity.LevelCrestStructureStandardDeviation);
            Assert.AreEqual(inputParameters.VerticalDistance.Value, entity.VerticalDistance);
            Assert.AreEqual(inputParameters.FailureProbabilityRepairClosure, entity.FailureProbabilityRepairClosure);
            Assert.AreEqual(inputParameters.FailureCollisionEnergy.Mean.Value, entity.FailureCollisionEnergyMean);
            Assert.AreEqual(inputParameters.FailureCollisionEnergy.CoefficientOfVariation.Value, entity.FailureCollisionEnergyCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ShipMass.Mean.Value, entity.ShipMassMean);
            Assert.AreEqual(inputParameters.ShipMass.CoefficientOfVariation.Value, entity.ShipMassCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ShipVelocity.Mean.Value, entity.ShipVelocityMean);
            Assert.AreEqual(inputParameters.ShipVelocity.CoefficientOfVariation.Value, entity.ShipVelocityCoefficientOfVariation);
            Assert.AreEqual(inputParameters.LevellingCount, entity.LevellingCount);
            Assert.AreEqual(inputParameters.ProbabilityCollisionSecondaryStructure, entity.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(inputParameters.FlowVelocityStructureClosable.Mean.Value, entity.FlowVelocityStructureClosableMean);
            Assert.AreEqual(inputParameters.StabilityLinearLoadModel.Mean.Value, entity.StabilityLinearLoadModelMean);
            Assert.AreEqual(inputParameters.StabilityLinearLoadModel.CoefficientOfVariation.Value, entity.StabilityLinearLoadModelCoefficientOfVariation);
            Assert.AreEqual(inputParameters.StabilityQuadraticLoadModel.Mean.Value, entity.StabilityQuadraticLoadModelMean);
            Assert.AreEqual(inputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation.Value, entity.StabilityQuadraticLoadModelCoefficientOfVariation);
            Assert.AreEqual(inputParameters.AreaFlowApertures.Mean.Value, entity.AreaFlowAperturesMean);
            Assert.AreEqual(inputParameters.AreaFlowApertures.StandardDeviation.Value, entity.AreaFlowAperturesStandardDeviation);
            Assert.AreEqual(Convert.ToByte(inputParameters.InflowModelType), entity.InflowModelType);
            Assert.AreEqual(Convert.ToByte(inputParameters.LoadSchematizationType), entity.LoadSchematizationType);
            Assert.AreEqual(inputParameters.VolumicWeightWater.Value, entity.VolumicWeightWater);
            Assert.AreEqual(inputParameters.FactorStormDurationOpenStructure.Value, entity.FactorStormDurationOpenStructure);
            Assert.AreEqual(inputParameters.DrainCoefficient.Mean.Value, entity.DrainCoefficientMean);
            Assert.AreEqual(Convert.ToByte(inputParameters.ShouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);
            Assert.AreEqual(order, entity.Order);

            CollectionAssert.IsEmpty(entity.StabilityPointStructuresOutputEntities);
        }

        [Test]
        public void CreateForStabilityPointStructures_CalculationWithParametersNaN_ReturnEntityWithNullParameters()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    StructureNormalOrientation = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = RoundedDouble.NaN
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    StorageStructureArea =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    WidthFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    InsideWaterLevel =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    BankWidth =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    EvaluationLevel = RoundedDouble.NaN,
                    LevelCrestStructure =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    VerticalDistance = RoundedDouble.NaN,
                    FailureCollisionEnergy =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ShipMass =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ShipVelocity =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FlowVelocityStructureClosable =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    AreaFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    VolumicWeightWater = RoundedDouble.NaN,
                    FactorStormDurationOpenStructure = RoundedDouble.NaN,
                    DrainCoefficient =
                    {
                        Mean = RoundedDouble.NaN
                    }
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, order);

            // Assert
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.BreakWaterHeight);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.ModelFactorSuperCriticalFlowMean);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);

            Assert.IsNull(entity.InsideWaterLevelMean);
            Assert.IsNull(entity.InsideWaterLevelStandardDeviation);
            Assert.IsNull(entity.ThresholdHeightOpenWeirMean);
            Assert.IsNull(entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.IsNull(entity.ConstructiveStrengthLinearLoadModelMean);
            Assert.IsNull(entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.ConstructiveStrengthQuadraticLoadModelMean);
            Assert.IsNull(entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.BankWidthMean);
            Assert.IsNull(entity.BankWidthStandardDeviation);
            Assert.IsNull(entity.InsideWaterLevelFailureConstructionMean);
            Assert.IsNull(entity.InsideWaterLevelFailureConstructionStandardDeviation);
            Assert.IsNull(entity.EvaluationLevel);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);
            Assert.IsNull(entity.VerticalDistance);
            Assert.IsNull(entity.FailureCollisionEnergyMean);
            Assert.IsNull(entity.FailureCollisionEnergyCoefficientOfVariation);
            Assert.IsNull(entity.ShipMassMean);
            Assert.IsNull(entity.ShipMassCoefficientOfVariation);
            Assert.IsNull(entity.ShipVelocityMean);
            Assert.IsNull(entity.ShipVelocityCoefficientOfVariation);
            Assert.IsNull(entity.FlowVelocityStructureClosableMean);
            Assert.IsNull(entity.StabilityLinearLoadModelMean);
            Assert.IsNull(entity.StabilityLinearLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.StabilityQuadraticLoadModelMean);
            Assert.IsNull(entity.StabilityQuadraticLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.AreaFlowAperturesMean);
            Assert.IsNull(entity.AreaFlowAperturesStandardDeviation);
            Assert.IsNull(entity.VolumicWeightWater);
            Assert.IsNull(entity.FactorStormDurationOpenStructure);
            Assert.IsNull(entity.DrainCoefficientMean);
        }

        [Test]
        public void CreateForStabilityPointStructures_CalculationWithStructure_ReturnEntityWithStructureEntity()
        {
            // Setup
            StabilityPointStructure alreadyRegisteredStructure = new TestStabilityPointStructure();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = alreadyRegisteredStructure
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new StabilityPointStructureEntity(), alreadyRegisteredStructure);

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.StabilityPointStructureEntity);
        }

        [Test]
        public void CreateForStabilityPointStructures_CalculationWithHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var alreadyRegisteredHydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = alreadyRegisteredHydraulicBoundaryLocation
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new HydraulicLocationEntity(), alreadyRegisteredHydraulicBoundaryLocation);

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.HydraulicLocationEntity);
        }

        [Test]
        public void CreateForStabilityPointStructures_CalculationWithForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var alreadyRegisteredForeshoreProfile = new TestForeshoreProfile();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = alreadyRegisteredForeshoreProfile
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new ForeshoreProfileEntity(), alreadyRegisteredForeshoreProfile);

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ForeshoreProfileEntity);
        }

        [Test]
        public void CreateForStabilityPointStructures_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var registry = new PersistenceRegistry();

            // Call
            StabilityPointStructuresCalculationEntity entity = calculation.CreateForStabilityPointStructures(registry, 0);

            // Assert
            AssertStructuresOutputEntity(calculation.Output, entity.StabilityPointStructuresOutputEntities.Single());
        }

        #endregion
    }
}