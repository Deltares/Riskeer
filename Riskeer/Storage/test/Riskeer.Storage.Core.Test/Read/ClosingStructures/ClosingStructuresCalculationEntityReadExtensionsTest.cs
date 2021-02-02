﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.ClosingStructures;

namespace Riskeer.Storage.Core.Test.Read.ClosingStructures
{
    [TestFixture]
    public class ClosingStructuresCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new ClosingStructuresCalculationEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnClosingStructuresCalculation()
        {
            // Setup
            var entity = new ClosingStructuresCalculationEntity
            {
                Name = "name",
                RelevantForScenario = Convert.ToByte(true),
                ScenarioContribution = 0.0,
                Comments = "comments",
                StructureNormalOrientation = 1.1,
                ModelFactorSuperCriticalFlowMean = 2.2,
                AllowedLevelIncreaseStorageMean = 3.3,
                AllowedLevelIncreaseStorageStandardDeviation = 4.4,
                StorageStructureAreaMean = 5.5,
                StorageStructureAreaCoefficientOfVariation = 6.6,
                FlowWidthAtBottomProtectionMean = 7.7,
                FlowWidthAtBottomProtectionStandardDeviation = 8.8,
                CriticalOvertoppingDischargeMean = 9.9,
                CriticalOvertoppingDischargeCoefficientOfVariation = 10.10,
                FailureProbabilityStructureWithErosion = 0.11,
                WidthFlowAperturesMean = 12.12,
                WidthFlowAperturesStandardDeviation = 13.13,
                StormDurationMean = 14.14,
                UseBreakWater = Convert.ToByte(true),
                BreakWaterType = Convert.ToByte(BreakWaterType.Wall),
                BreakWaterHeight = 15.15,
                UseForeshore = Convert.ToByte(true),
                InflowModelType = Convert.ToByte(ClosingStructureInflowModelType.LowSill),
                InsideWaterLevelMean = 16.16,
                InsideWaterLevelStandardDeviation = 17.17,
                DeviationWaveDirection = 18.18,
                DrainCoefficientMean = 19.19,
                DrainCoefficientStandardDeviation = 20.20,
                FactorStormDurationOpenStructure = 21.21,
                ThresholdHeightOpenWeirMean = 22.22,
                ThresholdHeightOpenWeirStandardDeviation = 23.23,
                AreaFlowAperturesMean = 24.24,
                AreaFlowAperturesStandardDeviation = 25.25,
                FailureProbabilityOpenStructure = 0.26,
                FailureProbabilityReparation = 0.27,
                IdenticalApertures = 28,
                LevelCrestStructureNotClosingMean = 29.29,
                LevelCrestStructureNotClosingStandardDeviation = 30.30,
                ProbabilityOpenStructureBeforeFlooding = 0.31
            };
            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(Convert.ToBoolean(entity.RelevantForScenario), calculation.IsRelevant);
            Assert.AreEqual(entity.ScenarioContribution, calculation.Contribution);
            Assert.AreEqual(entity.Comments, calculation.Comments.Body);

            ClosingStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            Assert.IsNull(inputParameters.Structure);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(entity.StructureNormalOrientation, inputParameters.StructureNormalOrientation.Value);
            Assert.AreEqual(entity.ModelFactorSuperCriticalFlowMean, inputParameters.ModelFactorSuperCriticalFlow.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageMean, inputParameters.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageStandardDeviation, inputParameters.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(entity.StorageStructureAreaMean, inputParameters.StorageStructureArea.Mean.Value);
            Assert.AreEqual(entity.StorageStructureAreaCoefficientOfVariation, inputParameters.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionMean, inputParameters.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionStandardDeviation, inputParameters.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeMean, inputParameters.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeCoefficientOfVariation, inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FailureProbabilityStructureWithErosion, inputParameters.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(entity.WidthFlowAperturesMean, inputParameters.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(entity.WidthFlowAperturesStandardDeviation, inputParameters.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.StormDurationMean, inputParameters.StormDuration.Mean.Value);
            Assert.AreEqual(Convert.ToBoolean(entity.UseBreakWater), inputParameters.UseBreakWater);
            Assert.AreEqual((BreakWaterType) entity.BreakWaterType, inputParameters.BreakWater.Type);
            Assert.AreEqual(entity.BreakWaterHeight, inputParameters.BreakWater.Height.Value);
            Assert.AreEqual(Convert.ToBoolean(entity.UseForeshore), inputParameters.UseForeshore);

            Assert.AreEqual((ClosingStructureInflowModelType) entity.InflowModelType, inputParameters.InflowModelType);
            Assert.AreEqual(entity.InsideWaterLevelMean, inputParameters.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelStandardDeviation, inputParameters.InsideWaterLevel.StandardDeviation.Value);
            Assert.AreEqual(entity.DeviationWaveDirection, inputParameters.DeviationWaveDirection.Value);
            Assert.AreEqual(entity.DrainCoefficientMean, inputParameters.DrainCoefficient.Mean.Value);
            Assert.AreEqual(entity.DrainCoefficientStandardDeviation, inputParameters.DrainCoefficient.StandardDeviation.Value);
            Assert.AreEqual(entity.FactorStormDurationOpenStructure, inputParameters.FactorStormDurationOpenStructure.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirMean, inputParameters.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirStandardDeviation, inputParameters.ThresholdHeightOpenWeir.StandardDeviation.Value);
            Assert.AreEqual(entity.AreaFlowAperturesMean, inputParameters.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(entity.AreaFlowAperturesStandardDeviation, inputParameters.AreaFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.FailureProbabilityOpenStructure, inputParameters.FailureProbabilityOpenStructure);
            Assert.AreEqual(entity.FailureProbabilityReparation, inputParameters.FailureProbabilityReparation);
            Assert.AreEqual(entity.IdenticalApertures, inputParameters.IdenticalApertures);
            Assert.AreEqual(entity.LevelCrestStructureNotClosingMean, inputParameters.LevelCrestStructureNotClosing.Mean.Value);
            Assert.AreEqual(entity.LevelCrestStructureNotClosingStandardDeviation, inputParameters.LevelCrestStructureNotClosing.StandardDeviation.Value);
            Assert.AreEqual(entity.ProbabilityOpenStructureBeforeFlooding, inputParameters.ProbabilityOpenStructureBeforeFlooding);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Read_EntityWithNullParameters_ReturnClosingStructuresCalculationWithInputParametersNaN()
        {
            // Setup
            var entity = new ClosingStructuresCalculationEntity
            {
                ScenarioContribution = null,
                StructureNormalOrientation = null,
                ModelFactorSuperCriticalFlowMean = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null,
                StorageStructureAreaMean = null,
                StorageStructureAreaCoefficientOfVariation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                StormDurationMean = null,
                BreakWaterHeight = null,
                InsideWaterLevelMean = null,
                InsideWaterLevelStandardDeviation = null,
                DeviationWaveDirection = null,
                DrainCoefficientMean = null,
                DrainCoefficientStandardDeviation = null,
                FactorStormDurationOpenStructure = null,
                ThresholdHeightOpenWeirMean = null,
                ThresholdHeightOpenWeirStandardDeviation = null,
                AreaFlowAperturesMean = null,
                AreaFlowAperturesStandardDeviation = null,
                LevelCrestStructureNotClosingMean = null,
                LevelCrestStructureNotClosingStandardDeviation = null,
                IdenticalApertures = 1
            };
            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.IsNaN(calculation.Contribution);

            ClosingStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNaN(inputParameters.StructureNormalOrientation);
            Assert.IsNaN(inputParameters.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(inputParameters.StorageStructureArea.Mean);
            Assert.IsNaN(inputParameters.StorageStructureArea.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.WidthFlowApertures.Mean);
            Assert.IsNaN(inputParameters.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(inputParameters.StormDuration.Mean);
            Assert.IsNaN(inputParameters.BreakWater.Height);

            Assert.IsNaN(inputParameters.InsideWaterLevel.Mean);
            Assert.IsNaN(inputParameters.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(inputParameters.DeviationWaveDirection);
            Assert.IsNaN(inputParameters.DrainCoefficient.Mean);
            Assert.IsNaN(inputParameters.DrainCoefficient.StandardDeviation);
            Assert.IsNaN(inputParameters.FactorStormDurationOpenStructure);
            Assert.IsNaN(inputParameters.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(inputParameters.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNaN(inputParameters.AreaFlowApertures.Mean);
            Assert.IsNaN(inputParameters.AreaFlowApertures.StandardDeviation);
            Assert.IsNaN(inputParameters.LevelCrestStructureNotClosing.Mean);
            Assert.IsNaN(inputParameters.LevelCrestStructureNotClosing.StandardDeviation);
        }

        [Test]
        public void Read_EntityWithStructureEntity_ReturnCalculationWithStructure()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();
            var structureEntity = new ClosingStructureEntity();
            var entity = new ClosingStructuresCalculationEntity
            {
                ClosingStructureEntity = structureEntity,
                IdenticalApertures = 1
            };
            var collector = new ReadConversionCollector();
            collector.Read(structureEntity, structure);

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(structure, calculation.InputParameters.Structure);
        }

        [Test]
        public void Read_EntityWithHydraulicLocationEntity_ReturnCalculationWithHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new ClosingStructuresCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                IdenticalApertures = 1
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileEntity_ReturnCalculationWithForeshoreProfile()
        {
            // Setup
            var profile = new TestForeshoreProfile();
            var profileEntity = new ForeshoreProfileEntity();
            var entity = new ClosingStructuresCalculationEntity
            {
                ForeshoreProfileEntity = profileEntity,
                IdenticalApertures = 1
            };

            var collector = new ReadConversionCollector();
            collector.Read(profileEntity, profile);

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(profile, calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void Read_ValidEntityWithOutputEntity_ReturnCalculationWithOutput()
        {
            // Setup
            var entity = new ClosingStructuresCalculationEntity
            {
                ClosingStructuresOutputEntities =
                {
                    new ClosingStructuresOutputEntity()
                },
                IdenticalApertures = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculationScenario<ClosingStructuresInput> calculation = entity.Read(collector);

            // Assert
            StructuresOutput calculationOutput = calculation.Output;
            Assert.IsNaN(calculationOutput.Reliability);
            Assert.IsFalse(calculationOutput.HasGeneralResult);
        }
    }
}