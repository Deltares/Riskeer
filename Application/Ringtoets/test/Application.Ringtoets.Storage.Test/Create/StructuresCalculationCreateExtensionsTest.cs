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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class StructuresCalculationCreateExtensionsTest
    {
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
                Comments = comments,
                InputParameters =
                {
                    StructureNormalOrientation = (RoundedDouble) random.GetFromRange(0, 360),
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(-9999.9999, 9999.9999)
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    FailureProbabilityStructureWithErosion = random.NextDouble(),
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(-9999.9999, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    DeviationWaveDirection = (RoundedDouble) random.NextDouble(),
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(-9999.9999, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    UseBreakWater = true,
                    UseForeshore = false,
                    BreakWater =
                    {
                        Height = (RoundedDouble) random.NextDouble(),
                        Type = BreakWaterType.Dam
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, order);

            // Assert
            Assert.AreEqual(0, entity.HeightStructuresCalculationEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comments, entity.Comments);
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
            Assert.AreEqual(input.WidthFlowApertures.CoefficientOfVariation.Value, entity.WidthFlowAperturesCoefficientOfVariation);
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
                        CoefficientOfVariation = RoundedDouble.NaN
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
            Assert.IsNull(entity.WidthFlowAperturesCoefficientOfVariation);
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.DeviationWaveDirection);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);

            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void CreateForHeightStructures_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comment = "B";
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = name,
                Comments = comment
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);

            Assert.AreNotSame(comment, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(comment, entity.Comments);
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
            var random = new Random(159);
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(random.NextDouble(), random.NextDouble(),
                                                         random.NextDouble(), random.NextDouble(),
                                                         random.NextDouble())
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.CreateForHeightStructures(registry, 0);

            // Assert
            Assert.AreEqual(1, entity.HeightStructuresOutputEntities.Count);
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
                Comments = "B",
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                    },
                    StructureNormalOrientation = (RoundedDouble) random.NextDouble(),
                    FailureProbabilityStructureWithErosion = (RoundedDouble) random.NextDouble(),
                    UseForeshore = random.NextBoolean(),
                    UseBreakWater = random.NextBoolean(),
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = (RoundedDouble) random.NextDouble()
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) random.NextDouble()
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    InflowModelType = ClosingStructureInflowModelType.VerticalWall,
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    DeviationWaveDirection = (RoundedDouble) random.NextDouble(),
                    DrainCoefficient =
                    {
                        Mean = (RoundedDouble) random.NextDouble()
                    },
                    FactorStormDurationOpenStructure = (RoundedDouble) random.NextDouble(),
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    FailureProbabilityOpenStructure = (RoundedDouble) random.NextDouble(),
                    FailureProbabilityReparation = (RoundedDouble) random.NextDouble(),
                    IdenticalApertures = random.Next(),
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    ProbabilityOpenStructureBeforeFlooding = (RoundedDouble) random.NextDouble()
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, order);

            // Assert
            Assert.AreEqual(calculation.Name, entity.Name);
            Assert.AreEqual(calculation.Comments, entity.Comments);

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
            Assert.AreEqual(inputParameters.WidthFlowApertures.CoefficientOfVariation.Value, entity.WidthFlowAperturesCoefficientOfVariation);
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
            Assert.AreEqual(inputParameters.FailureProbabilityReparation, entity.FailureProbablityReparation);
            Assert.AreEqual(inputParameters.IdenticalApertures, entity.IdenticalApertures);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.Mean.Value, entity.LevelCrestStructureNotClosingMean);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.StandardDeviation.Value, entity.LevelCrestStructureNotClosingStandardDeviation);
            Assert.AreEqual(inputParameters.ProbabilityOpenStructureBeforeFlooding, entity.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(order, entity.Order);
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
                        Mean = RoundedDouble.NaN,
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
                        CoefficientOfVariation = RoundedDouble.NaN
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
            Assert.IsNull(entity.WidthFlowAperturesCoefficientOfVariation);
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
            var alreadyRegisteredHydroLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = alreadyRegisteredHydroLocation
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new HydraulicLocationEntity(), alreadyRegisteredHydroLocation);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.CreateForClosingStructures(registry, 0);

            // Assert
            Assert.IsNotNull(entity.HydraulicLocationEntity);
        }

        [Test]
        public void CreateForClosingStructures_CalculationWithForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var alreadyRegisteredForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                         new Point2D[0],
                                                                         null,
                                                                         new ForeshoreProfile.ConstructionProperties());
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

        #endregion
    }
}