// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo181IntegrationTest
    {
        /// <summary>
        /// Enum to indicate the normative norm type.
        /// </summary>
        private enum NormativeNormType
        {
            /// <summary>
            /// Represents the lower limit norm.
            /// </summary>
            LowerLimitNorm = 1,

            /// <summary>
            /// Represents the signaling norm.
            /// </summary>
            SignalingNorm = 2
        }

        private const string newVersion = "18.1";

        [Test]
        public void Given173Project_WhenUpgradedTo181_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject173.rtd");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given173Project_WhenUpgradedTo181_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given173Project_WhenUpgradedTo181_ThenProjectAsExpected), ".log"));
            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    AssertTablesContentMigrated(reader, sourceFilePath);

                    AssertVersions(reader);
                    AssertDatabase(reader);

                    AssertAssessmentSection(reader, sourceFilePath);
                    AssertHydraulicBoundaryLocationsProperties(reader, sourceFilePath);
                    AssertHydraulicBoundaryLocationsOnAssessmentSection(reader, sourceFilePath);
                    AssertHydraulicBoundaryLocationsOnGrassCoverErosionOutwardsFailureMechanism(reader, sourceFilePath);
                    AssertFailureMechanisms(reader, sourceFilePath);

                    AssertFailureMechanismRelatedOutput(reader);
                    AssertPipingOutput(reader, sourceFilePath);
                    AssertMacroStabilityInwardsOutput(reader, sourceFilePath);

                    AssertPipingSoilLayers(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertMacroStabilityOutwardsFailureMechanism(reader);
                    AssertPipingStructureFailureMechanism(reader);
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);

                    AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(reader, sourceFilePath);

                    AssertDuneErosionFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertDuneLocations(reader, sourceFilePath);

                    AssertHeightStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertClosingStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertStabilityPointStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsSectionResultEntity(reader, sourceFilePath);

                    AssertPipingSectionResultEntity(reader, sourceFilePath);
                    AssertMacroStabilityInwardsSectionResultEntity(reader, sourceFilePath);

                    AssertDuneErosionSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertStabilityStoneCoverSectionResultEntity(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverSectionResultEntity(reader, sourceFilePath);

                    AssertGrassCoverSlipOffInwardsSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverSlipOffOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertMacroStabilityOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertMicrostabilitySectionResultEntity(reader, sourceFilePath);
                    AssertPipingStructureSectionResultEntity(reader, sourceFilePath);
                    AssertStrengthStabilityLengthwiseConstructionSectionResultEntity(reader, sourceFilePath);
                    AssertTechnicalInnovationSectionResultEntity(reader, sourceFilePath);
                    AssertWaterPressureAsphaltCoverSectionResultEntity(reader, sourceFilePath);

                    AssertWaveConditionsCalculations(reader, sourceFilePath);

                    MigratedSerializedDataTestHelper.AssertSerializedMacroStabilityInwardsOutput(reader);
                    MigratedSerializedDataTestHelper.AssertSerializedDikeProfileRoughnessPoints(reader);
                    MigratedSerializedDataTestHelper.AssertSerializedSurfaceLine(reader);
                    MigratedSerializedDataTestHelper.AssertSerializedPoint2DCollection(reader);

                    AssertClosingStructure(reader, sourceFilePath);
                    AssertClosingStructureCalculation(reader, sourceFilePath);

                    AssertStabilityPointStructureCalculation(reader, sourceFilePath);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertClosingStructure(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateClosingStructureProbabilityOpenStructureBeforeFlooding =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "SUM([IsInvalid]) = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "CASE WHEN (NEW.[ProbabilityOpenStructureBeforeFlooding] IS NOT OLD.[ProbabilityOrFrequencyOpenStructureBeforeFlooding] " +
                "AND OLD.[ProbabilityOrFrequencyOpenStructureBeforeFlooding] <= 1)  " +
                "OR (NEW.[ProbabilityOpenStructureBeforeFlooding] IS NOT NULL AND OLD.[ProbabilityOrFrequencyOpenStructureBeforeFlooding] > 1) " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS [IsInvalid] " +
                "FROM ClosingStructureEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructureEntity OLD USING (ClosingStructureEntityId) " +
                "); " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateClosingStructureProbabilityOpenStructureBeforeFlooding);

            string validateIdenticalApertures =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "SUM([IsInvalid]) = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "CASE WHEN (NEW.[IdenticalApertures] != OLD.[IdenticalApertures] " +
                "AND OLD.[IdenticalApertures] >= 1)  " +
                "OR (NEW.[IdenticalApertures] != 1 AND OLD.[IdenticalApertures] < 1) " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS [IsInvalid] " +
                "FROM ClosingStructureEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructureEntity OLD USING (ClosingStructureEntityId) " +
                "); " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateIdenticalApertures);

            string validateClosingStructure =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructureEntity) " +
                "FROM ClosingStructureEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructureEntity OLD USING(ClosingStructureEntityId) " +
                "WHERE NEW.FailureMechanismEntityId = OLD.FailureMechanismEntityId " +
                "AND NEW.\"Order\" = OLD.\"Order\"; " +
                "AND NEW.Name = OLD.Name " +
                "AND NEW.X IS OLD.X " +
                "AND NEW.Y IS OLD.Y" +
                "AND NEW.StructureNormalOrientation IS OLD.StructureNormalOrientation" +
                "AND NEW.StorageStructureAreaMean IS OLD.StorageStructureAreaMean" +
                "AND NEW.StorageStructureAreaCoefficientOfVariation IS OLD.StorageStructureAreaCoefficientOfVariation" +
                "AND NEW.AllowedLevelIncreaseStorageMean IS OLD.AllowedLevelIncreaseStorageMean" +
                "AND NEW.AllowedLevelIncreaseStorageStandardDeviation IS OLD.AllowedLevelIncreaseStorageStandardDeviation" +
                "AND NEW.WidthFlowAperturesMean IS OLD.WidthFlowAperturesMean" +
                "AND NEW.WidthFlowAperturesStandardDeviation IS OLD.WidthFlowAperturesStandardDeviation" +
                "AND NEW.LevelCrestStructureNotClosingMean IS OLD.LevelCrestStructureNotClosingMean" +
                "AND NEW.LevelCrestStructureNotClosingStandardDeviation IS OLD.LevelCrestStructureNotClosingStandardDeviation" +
                "AND NEW.InsideWaterLevelMean IS OLD.InsideWaterLevelMean" +
                "AND NEW.InsideWaterLevelStandardDeviation IS OLD.InsideWaterLevelStandardDeviation" +
                "AND NEW.ThresholdHeightOpenWeirMean IS OLD.ThresholdHeightOpenWeirMean" +
                "AND NEW.ThresholdHeightOpenWeirStandardDeviation IS OLD.ThresholdHeightOpenWeirStandardDeviation" +
                "AND NEW.AreaFlowAperturesMean IS OLD.AreaFlowAperturesMean" +
                "AND NEW.AreaFlowAperturesStandardDeviation IS OLD.AreaFlowAperturesStandardDeviation" +
                "AND NEW.CriticalOvertoppingDischargeMean IS OLD.CriticalOvertoppingDischargeMean" +
                "AND NEW.CriticalOvertoppingDischargeCoefficientOfVariation IS OLD.CriticalOvertoppingDischargeCoefficientOfVariation" +
                "AND NEW.FlowWidthAtBottomProtectionMean IS OLD.FlowWidthAtBottomProtectionMean" +
                "AND NEW.FlowWidthAtBottomProtectionStandardDeviation IS OLD.FlowWidthAtBottomProtectionStandardDeviation" +
                "AND NEW.FailureProbabilityOpenStructure IS OLD.FailureProbabilityOpenStructure" +
                "AND NEW.FailureProbabilityReparation IS OLD.FailureProbabilityReparation" +
                "AND NEW.InflowModelType IS OLD.InflowModelType;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateClosingStructure);
        }

        private static void AssertClosingStructureCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateIdenticalApertures =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT " +
                "SUM([IsInvalid]) = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "CASE WHEN (NEW.[IdenticalApertures] != OLD.[IdenticalApertures] " +
                "AND OLD.[IdenticalApertures] >= 1)  " +
                "OR (NEW.[IdenticalApertures] != 1 AND OLD.[IdenticalApertures] < 1) " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS [IsInvalid] " +
                "FROM ClosingStructuresCalculationEntity NEW " +
                "JOIN [SOURCE].ClosingStructuresCalculationEntity OLD USING (ClosingStructuresCalculationEntityId) " +
                "); " +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateIdenticalApertures);

            string validateClosingStructureCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCE].ClosingStructuresCalculationEntity) " +
                "FROM ClosingStructuresCalculationEntity NEW " +
                "JOIN [SOURCE].ClosingStructuresCalculationEntity OLD USING(ClosingStructuresCalculationEntityId) " +
                "WHERE OLD.CalculationGroupEntityId = NEW.CalculationGroupEntityId " +
                "AND OLD.ForeshoreProfileEntityId IS NEW.ForeshoreProfileEntityId " +
                "AND OLD.HydraulicLocationEntityId IS NEW.HydraulicLocationEntityId " +
                "AND OLD.ClosingStructureEntityId IS NEW.ClosingStructureEntityId " +
                "AND OLD.\"Order\" = NEW.\"Order\" " +
                "AND OLD.Name IS NEW.Name " +
                "AND OLD.Comments IS NEW.Comments " +
                "AND OLD.UseBreakWater = NEW.UseBreakWater " +
                "AND OLD.BreakWaterType = NEW.BreakWaterType " +
                "AND OLD.BreakWaterHeight IS NEW.BreakWaterHeight " +
                "AND OLD.UseForeshore = NEW.UseForeshore " +
                "AND OLD.Orientation IS NEW.Orientation " +
                "AND OLD.StructureNormalOrientation IS NEW.StructureNormalOrientation " +
                "AND OLD.StorageStructureAreaMean IS NEW.StorageStructureAreaMean " +
                "AND OLD.StorageStructureAreaCoefficientOfVariation IS NEW.StorageStructureAreaCoefficientOfVariation " +
                "AND OLD.AllowedLevelIncreaseStorageMean IS NEW.AllowedLevelIncreaseStorageMean " +
                "AND OLD.AllowedLevelIncreaseStorageStandardDeviation IS NEW.AllowedLevelIncreaseStorageStandardDeviation " +
                "AND OLD.WidthFlowAperturesMean IS NEW.WidthFlowAperturesMean " +
                "AND OLD.WidthFlowAperturesStandardDeviation IS NEW.WidthFlowAperturesStandardDeviation " +
                "AND OLD.LevelCrestStructureNotClosingMean IS NEW.LevelCrestStructureNotClosingMean " +
                "AND OLD.LevelCrestStructureNotClosingStandardDeviation IS NEW.LevelCrestStructureNotClosingStandardDeviation " +
                "AND OLD.InsideWaterLevelMean IS NEW.InsideWaterLevelMean " +
                "AND OLD.InsideWaterLevelStandardDeviation IS NEW.InsideWaterLevelStandardDeviation " +
                "AND OLD.ThresholdHeightOpenWeirMean IS NEW.ThresholdHeightOpenWeirMean " +
                "AND OLD.ThresholdHeightOpenWeirStandardDeviation IS NEW.ThresholdHeightOpenWeirStandardDeviation " +
                "AND OLD.AreaFlowAperturesMean IS NEW.AreaFlowAperturesMean " +
                "AND OLD.AreaFlowAperturesStandardDeviation IS NEW.AreaFlowAperturesStandardDeviation " +
                "AND OLD.CriticalOvertoppingDischargeMean IS NEW.CriticalOvertoppingDischargeMean " +
                "AND OLD.CriticalOvertoppingDischargeCoefficientOfVariation IS NEW.CriticalOvertoppingDischargeCoefficientOfVariation " +
                "AND OLD.FlowWidthAtBottomProtectionMean IS NEW.FlowWidthAtBottomProtectionMean " +
                "AND OLD.FlowWidthAtBottomProtectionStandardDeviation IS NEW.FlowWidthAtBottomProtectionStandardDeviation " +
                "AND OLD.ProbabilityOrFrequencyOpenStructureBeforeFlooding = NEW.ProbabilityOpenStructureBeforeFlooding " +
                "AND OLD.FailureProbabilityOpenStructure = NEW.FailureProbabilityOpenStructure " +
                "AND OLD.FailureProbabilityReparation = NEW.FailureProbabilityReparation " +
                "AND OLD.InflowModelType = NEW.InflowModelType " +
                "AND OLD.FailureProbabilityStructureWithErosion = NEW.FailureProbabilityStructureWithErosion " +
                "AND OLD.DeviationWaveDirection IS NEW.DeviationWaveDirection " +
                "AND OLD.DrainCoefficientMean IS NEW.DrainCoefficientMean " +
                "AND OLD.ModelFactorSuperCriticalFlowMean IS NEW.ModelFactorSuperCriticalFlowMean " +
                "AND OLD.StormDurationMean IS NEW.StormDurationMean " +
                "AND OLD.FactorStormDurationOpenStructure IS NEW.FactorStormDurationOpenStructure " +
                "AND OLD.ShouldIllustrationPointsBeCalculated = NEW.ShouldIllustrationPointsBeCalculated;" +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateClosingStructureCalculation);
        }

        private static void AssertStabilityPointStructureCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateLevellingCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT " +
                "SUM([IsInvalid]) = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "CASE WHEN (NEW.[LevellingCount] != OLD.[LevellingCount] " +
                "AND OLD.[LevellingCount] >= 0)  " +
                "OR (NEW.[LevellingCount] != 0 AND OLD.[LevellingCount] < 0) " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS [IsInvalid] " +
                "FROM StabilityPointStructuresCalculationEntity NEW " +
                "JOIN [SOURCE].StabilityPointStructuresCalculationEntity OLD USING (StabilityPointStructuresCalculationEntityId) " +
                "); " +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateLevellingCount);

            string validateVerticalDistance =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT " +
                "SUM([IsInvalid]) = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "CASE WHEN (NEW.[VerticalDistance] IS NOT OLD.[VerticalDistance] " +
                "AND OLD.[VerticalDistance] >= 0)  " +
                "OR (NEW.[VerticalDistance] IS NOT NULL AND OLD.[VerticalDistance] < 0) " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS [IsInvalid] " +
                "FROM StabilityPointStructuresCalculationEntity NEW " +
                "JOIN [SOURCE].StabilityPointStructuresCalculationEntity OLD USING (StabilityPointStructuresCalculationEntityId) " +
                "); " +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateVerticalDistance);

            string validateStabilityPointStructureCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCE].StabilityPointStructuresCalculationEntity) " +
                "FROM StabilityPointStructuresCalculationEntity NEW " +
                "JOIN [SOURCE].StabilityPointStructuresCalculationEntity OLD USING(StabilityPointStructuresCalculationEntityId) " +
                "WHERE OLD.CalculationGroupEntityId = NEW.CalculationGroupEntityId " +
                "AND OLD.ForeshoreProfileEntityId IS NEW.ForeshoreProfileEntityId " +
                "AND OLD.HydraulicLocationEntityId IS NEW.HydraulicLocationEntityId " +
                "AND OLD.StabilityPointStructureEntityId IS NEW.StabilityPointStructureEntityId " +
                "AND OLD.\"Order\" = NEW.\"Order\" " +
                "AND OLD.Name IS NEW.Name " +
                "AND OLD.Comments IS NEW.Comments " +
                "AND OLD.UseBreakWater = NEW.UseBreakWater " +
                "AND OLD.BreakWaterType = NEW.BreakWaterType " +
                "AND OLD.BreakWaterHeight IS NEW.BreakWaterHeight " +
                "AND OLD.UseForeshore = NEW.UseForeshore " +
                "AND OLD.StructureNormalOrientation IS NEW.StructureNormalOrientation " +
                "AND OLD.StorageStructureAreaMean IS NEW.StorageStructureAreaMean " +
                "AND OLD.StorageStructureAreaCoefficientOfVariation IS NEW.StorageStructureAreaCoefficientOfVariation " +
                "AND OLD.AllowedLevelIncreaseStorageMean IS NEw.AllowedLevelIncreaseStorageMean " +
                "AND OLD.AllowedLevelIncreaseStorageStandardDeviation IS NEW.AllowedLevelIncreaseStorageStandardDeviation " +
                "AND OLD.WidthFlowAperturesMean IS NEW.WidthFlowAperturesMean " +
                "AND OLD.WidthFlowAperturesStandardDeviation IS NEW.WidthFlowAperturesStandardDeviation " +
                "AND OLD.InsideWaterLevelMean IS NEW.InsideWaterLevelMean " +
                "AND OLD.InsideWaterLevelStandardDeviation IS NEW.InsideWaterLevelStandardDeviation " +
                "AND OLD.ThresholdHeightOpenWeirMean IS NEW.ThresholdHeightOpenWeirMean " +
                "AND OLD.ThresholdHeightOpenWeirStandardDeviation IS NEW.ThresholdHeightOpenWeirStandardDeviation " +
                "AND OLD.CriticalOvertoppingDischargeMean IS NEW.CriticalOvertoppingDischargeMean " +
                "AND OLD.CriticalOvertoppingDischargeCoefficientOfVariation IS NEW.CriticalOvertoppingDischargeCoefficientOfVariation " +
                "AND OLD.FlowWidthAtBottomProtectionMean IS NEW.FlowWidthAtBottomProtectionMean " +
                "AND OLD.FlowWidthAtBottomProtectionStandardDeviation IS NEW.FlowWidthAtBottomProtectionStandardDeviation " +
                "AND OLD.ConstructiveStrengthLinearLoadModelMean IS NEW.ConstructiveStrengthLinearLoadModelMean " +
                "AND OLD.ConstructiveStrengthLinearLoadModelCoefficientOfVariation IS NEW.ConstructiveStrengthLinearLoadModelCoefficientOfVariation " +
                "AND OLD.ConstructiveStrengthQuadraticLoadModelMean IS NEW.ConstructiveStrengthQuadraticLoadModelMean " +
                "AND OLD.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation IS NEW.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation " +
                "AND OLD.BankWidthMean IS NEW.BankWidthMean " +
                "AND OLD.BankWidthStandardDeviation IS NEW.BankWidthStandardDeviation " +
                "AND OLD.InsideWaterLevelFailureConstructionMean IS NEW.InsideWaterLevelFailureConstructionMean " +
                "AND OLD.InsideWaterLevelFailureConstructionStandardDeviation IS NEW.InsideWaterLevelFailureConstructionStandardDeviation " +
                "AND OLD.EvaluationLevel IS NEW.EvaluationLevel " +
                "AND OLD.LevelCrestStructureMean IS NEW.LevelCrestStructureMean " +
                "AND OLD.LevelCrestStructureStandardDeviation IS NEW.LevelCrestStructureStandardDeviation " +
                "AND OLD.FailureProbabilityRepairClosure = NEW.FailureProbabilityRepairClosure " +
                "AND OLD.FailureCollisionEnergyMean IS NEW.FailureCollisionEnergyMean " +
                "AND OLD.FailureCollisionEnergyCoefficientOfVariation IS NEW.FailureCollisionEnergyCoefficientOfVariation " +
                "AND OLD.ShipMassMean IS NEW.ShipMassMean " +
                "AND OLD.ShipMassCoefficientOfVariation IS NEW.ShipMassCoefficientOfVariation " +
                "AND OLD.ShipVelocityMean IS NEW.ShipVelocityMean " +
                "AND OLD.ShipVelocityCoefficientOfVariation IS NEW.ShipVelocityCoefficientOfVariation " +
                "AND OLD.ProbabilityCollisionSecondaryStructure = NEW.ProbabilityCollisionSecondaryStructure " +
                "AND OLD.FlowVelocityStructureClosableMean IS NEW.FlowVelocityStructureClosableMean " +
                "AND OLD.StabilityLinearLoadModelMean IS NEW.StabilityLinearLoadModelMean " +
                "AND OLD.StabilityLinearLoadModelCoefficientOfVariation IS NEW.StabilityLinearLoadModelCoefficientOfVariation " +
                "AND OLD.StabilityQuadraticLoadModelMean IS NEW.StabilityQuadraticLoadModelMean " +
                "AND OLD.StabilityQuadraticLoadModelCoefficientOfVariation IS NEW.StabilityQuadraticLoadModelCoefficientOfVariation " +
                "AND OLD.AreaFlowAperturesMean IS NEW.AreaFlowAperturesMean " +
                "AND OLD.AreaFlowAperturesStandardDeviation IS NEW.AreaFlowAperturesStandardDeviation " +
                "AND OLD.InflowModelType = NEW.InflowModelType " +
                "AND OLD.LoadSchematizationType = NEW.LoadSchematizationType " +
                "AND OLD.VolumicWeightWater IS NEW.VolumicWeightWater " +
                "AND OLD.StormDurationMean IS NEW.StormDurationMean " +
                "AND OLD.FactorStormDurationOpenStructure IS NEW.FactorStormDurationOpenStructure " +
                "AND OLD.DrainCoefficientMean IS NEW.DrainCoefficientMean " +
                "AND OLD.FailureProbabilityStructureWithErosion = NEW.FailureProbabilityStructureWithErosion " +
                "AND OLD.ShouldIllustrationPointsBeCalculated = NEW.ShouldIllustrationPointsBeCalculated;" +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateStabilityPointStructureCalculation);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AssessmentSectionEntity",
                "BackgroundDataEntity",
                "BackgroundDataMetaEntity",
                "CalculationGroupEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresFailureMechanismMetaEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneErosionSectionResultEntity",
                "DuneLocationEntity",
                "FailureMechanismEntity",
                "FailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicLocationEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSectionResultEntity",
                "MacroStabilityInwardsSemiProbabilisticOutputEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "MacroStabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCalculationEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSemiProbabilisticOutputEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochasticSoilModelEntity",
                "StrengthStabilityLengthwiseConstructionSectionResultEntity",
                "SurfaceLineEntity",
                "TechnicalInnovationSectionResultEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverWaveConditionsCalculationEntity"
            };

            foreach (string table in tables)
            {
                string validateMigratedTable =
                    $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                    $"SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].{table}) " +
                    $"FROM {table};" +
                    "DETACH SOURCEPROJECT;";
                reader.AssertReturnedDataIsValid(validateMigratedTable);
            }
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(73, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "Gevolgen van de migratie van versie 17.3 naar versie 18.1:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Alle berekende resultaten zijn verwijderd, behalve die van het toetsspoor 'Piping' en 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'assessmentSectionResults'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit langsconstructies'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Technische innovaties'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Wateroverdruk bij asfaltbekleding'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Macrostabiliteit binnenwaarts'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Macrostabiliteit buitenwaarts'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Golfklappen op asfaltbekleding'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding erosie buitentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding afschuiven binnentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding afschuiven buitentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Microstabiliteit'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping bij kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Stabiliteit steenzetting'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Duinafslag'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'PipingSoilLayer'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-0.125' voor de variatiecoëfficiënt van parameter 'd70' van ondergrondlaag 'DiameterD70Variation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-1.0' voor het gemiddelde van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-1.0e-06' voor het gemiddelde van parameter 'd70' van ondergrondlaag 'DiameterD70Mean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-10.0' voor de standaardafwijking van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelDeviation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-10.0' voor de variatiecoëfficiënt van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityVariation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '0.0' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '15.0' voor de verschuiving van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelShift' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'Closing Structures Invalid Data'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '-11' van parameter 'Aantal identieke doorstroomopeningen' van berekening 'Invalid Identical Apertures - Negative Value' is ongeldig en is veranderd naar 1."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '-11' van parameter 'Aantal identieke doorstroomopeningen' van kunstwerk 'Gemaal Leemans (Negatieve doorstroomopeningen)' is ongeldig en is veranderd naar 1."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '0' van parameter 'Aantal identieke doorstroomopeningen' van berekening 'Invalid Identical Apertures' is ongeldig en is veranderd naar 1."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '0' van parameter 'Aantal identieke doorstroomopeningen' van kunstwerk 'Gemaal Leemans' is ongeldig en is veranderd naar 1."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '1.1' van parameter 'Kans op open staan bij naderend hoogwater' van kunstwerk 'Gemaal Leemans' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'Stability Point Structures Data'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '-0.11' van parameter 'Afstand onderkant wand en teen van de dijk/berm' van berekening 'Invalid Vertical Distance' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde van '-30' van parameter 'Aantal nivelleringen per jaar' van berekening 'Invalid Levelling Count' is ongeldig en is veranderd naar 0."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"18.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertFailureMechanisms(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string validateFailureMechanismSectionsSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (SELECT " +
                "CASE WHEN " +
                "COUNT([FailureMechanismSectionEntityId]) AND [FailureMechanismSectionCollectionSourcePath] IS NULL " +
                "OR " +
                "[FailureMechanismSectionCollectionSourcePath] IS NOT NULL AND NOT COUNT([FailureMechanismSectionEntityId]) " +
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [FailureMechanismEntity] " +
                "LEFT JOIN [FailureMechanismSectionEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionsSourcePath);

            string validateFailureMechanisms =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].FailureMechanismEntity) " +
                "FROM FailureMechanismEntity NEW " +
                "JOIN [SOURCEPROJECT].FailureMechanismEntity OLD USING (FailureMechanismEntityId) " +
                "WHERE NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId " +
                "AND NEW.CalculationGroupEntityId IS OLD.CalculationGroupEntityId " +
                "AND NEW.FailureMechanismType = OLD.FailureMechanismType " +
                "AND NEW.IsRelevant = OLD.IsRelevant " +
                "AND NEW.InputComments IS OLD.InputComments " +
                "AND NEW.OutputComments IS OLD.OutputComments " +
                "AND NEW.NotRelevantComments IS OLD.NotRelevantComments; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader)
        {
            const string validateBelowPhreaticLevel =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [BelowPhreaticLevelMean] < [BelowPhreaticLevelShift] " +
                "OR [BelowPhreaticLevelMean] <= 0 " +
                "OR [BelowPhreaticLevelDeviation] < 0;";
            reader.AssertReturnedDataIsValid(validateBelowPhreaticLevel);

            const string validateDiameter70 =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [DiameterD70Mean] <= 0 " +
                "OR [DiameterD70CoefficientOfVariation] < 0;";
            reader.AssertReturnedDataIsValid(validateDiameter70);

            const string validatePermeability =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [PermeabilityMean] <= 0 " +
                "OR [PermeabilityCoefficientOfVariation] < 0;";
            reader.AssertReturnedDataIsValid(validatePermeability);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateStabilityStoneCoverFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverFailureMechanismMetaEntity] " +
                "WHERE [N] IS NOT 4;";
            reader.AssertReturnedDataIsValid(validateStabilityStoneCoverFailureMechanisms);
        }

        private static void AssertMacroStabilityOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateMacroStabilityOutwardsFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 13) " +
                "FROM [MacroStabilityOutwardsFailureMechanismMetaEntity] " +
                "WHERE [A] = 0.033 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 13);";
            reader.AssertReturnedDataIsValid(validateMacroStabilityOutwardsFailureMechanisms);
        }

        private static void AssertPipingStructureFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validatePipingStructureFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 11) " +
                "FROM [PipingStructureFailureMechanismMetaEntity] " +
                "WHERE [N] = 1.0 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 11);";
            reader.AssertReturnedDataIsValid(validatePipingStructureFailureMechanisms);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateWaveImpactAsphaltCoverFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverFailureMechanismMetaEntity] " +
                "WHERE [DeltaL] IS NOT 1000;";
            reader.AssertReturnedDataIsValid(validateWaveImpactAsphaltCoverFailureMechanisms);
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSectionEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].AssessmentSectionEntity) " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity AS OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.ProjectEntityId = OLD.ProjectEntityId " +
                "AND NEW.Id = OLD.Id " +
                "AND NEW.Name IS OLD.Name " +
                "AND NEW.Comments IS OLD.Comments " +
                "AND NEW.LowerLimitNorm = OLD.LowerLimitNorm " +
                "AND NEW.SignalingNorm = OLD.SignalingNorm " +
                "AND NEW.NormativeNormType = OLD.NormativeNormType " +
                "AND NEW.HydraulicDatabaseVersion IS OLD.HydraulicDatabaseVersion " +
                "AND NEW.HydraulicDatabaseLocation IS OLD.HydraulicDatabaseLocation " +
                "AND NEW.Composition = OLD.Composition " +
                "AND NEW.\"Order\" = OLD.\"Order\"; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateAssessmentSectionEntities);
        }

        private static void AssertHydraulicBoundaryLocationsProperties(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMigratedHyraulicBoundaryLocations = $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                                                               "SELECT " +
                                                               "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                                                               "FROM HydraulicLocationEntity NEW " +
                                                               "JOIN [SourceProject].HydraulicLocationEntity AS OLD USING(HydraulicLocationEntityId) " +
                                                               "WHERE NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId " +
                                                               "AND NEW.LocationId = OLD.LocationId " +
                                                               "AND NEW.Name = OLD.Name " +
                                                               "AND NEW.LocationX = OLD.LocationX " +
                                                               "AND NEW.LocationY = OLD.LocationY " +
                                                               "AND NEW.\"Order\" = OLD.\"Order\"; " +
                                                               "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedHyraulicBoundaryLocations);

            string validateHydraulicLocationCalculationCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "COUNT(distinct HydraulicLocationCalculationEntityId) AS NrOfCalculationsPerLocation " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "LEFT JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationEntityId) " +
                "GROUP BY HydraulicLocationEntityId " +
                ") " +
                "WHERE NrOfCalculationsPerLocation IS NOT 14; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationCount);

            const string validateHydraulicBoundaryCalculationInputValues = "SELECT " +
                                                                           "COUNT() = 0 " +
                                                                           "FROM HydraulicLocationCalculationEntity " +
                                                                           "WHERE ShouldIllustrationPointsBeCalculated != 0 AND ShouldIllustrationPointsBeCalculated != 1";
            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryCalculationInputValues);

            string validateHydraulicLocationCalculationOutputCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = 0 " +
                "FROM HydraulicLocationOutputEntity; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationOutputCount);
        }

        private static void AssertHydraulicBoundaryLocationsOnAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator(sourceFilePath);
            AssertDesignWaterLevelCalculationEntitiesOnAssessmentSection(reader, queryGenerator);
            AssertWaveHeightCalculationEntitiesOnAssessmentSection(reader, queryGenerator);
        }

        private static void AssertHydraulicBoundaryLocationsOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator(sourceFilePath);

            AssertDesignWaterLevelCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(reader, queryGenerator);
            AssertWaveHeightCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(reader, queryGenerator);
        }

        private static void AssertDuneLocations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new DuneErosionFailureMechanismValidationQueryGenerator(sourceFilePath);

            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerFactorizedLimitNorm));

            string validateDuneLocationOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = 0 " +
                "FROM DuneLocationCalculationOutputEntity; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateDuneLocationOutput);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity) " +
                "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId) " +
                "WHERE new.FailureMechanismEntityId = OLD.FailureMechanismEntityId " +
                "AND NEW.N = OLD.N " +
                "AND NEW.ForeshoreProfileCollectionSourcePath IS OLD.ForeshoreProfileCollectionSourcePath; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertDuneErosionFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity) " +
                "FROM DuneErosionFailureMechanismMetaEntity NEW " +
                "JOIN [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity OLD USING(DuneErosionFailureMechanismMetaEntityId) " +
                "WHERE new.FailureMechanismEntityId = OLD.FailureMechanismEntityId " +
                "AND NEW.N = OLD.N; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertFailureMechanismRelatedOutput(MigratedDatabaseReader reader)
        {
            var tables = new[]
            {
                "ClosingStructuresOutputEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverErosionInwardsDikeHeightOutputEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsOvertoppingRateOutputEntity",
                "HeightStructuresOutputEntity",
                "StabilityPointStructuresOutputEntity",
                "StabilityStoneCoverWaveConditionsOutputEntity",
                "WaveImpactAsphaltCoverWaveConditionsOutputEntity",
                "TopLevelFaultTreeIllustrationPointEntity",
                "TopLevelSubMechanismIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointStochastEntity",
                "GeneralResultSubMechanismIllustrationPointEntity",
                "GeneralResultSubMechanismIllustrationPointStochastEntity",
                "SubMechanismIllustrationPointEntity",
                "SubMechanismIllustrationPointStochastEntity",
                "FaultTreeIllustrationPointEntity",
                "FaultTreeIllustrationPointStochastEntity",
                "FaultTreeSubmechanismIllustrationPointEntity",
                "StochastEntity"
            };

            foreach (string table in tables)
            {
                string validateMigratedTable =
                    "SELECT COUNT() = 0 " +
                    $"FROM {table};" +
                    "DETACH SOURCEPROJECT;";
                reader.AssertReturnedDataIsValid(validateMigratedTable);
            }
        }

        private static void AssertPipingOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateOutputCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].PipingCalculationOutputEntity " +
                "JOIN [SOURCEPROJECT].PipingCalculationEntity USING(PipingCalculationEntityId) " +
                "WHERE [UseAssessmentLevelManualInput] = 1 " +
                ") " +
                "FROM PipingCalculationOutputEntity;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateOutputCount);
        }

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateOutputCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity " +
                "JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId) " +
                "WHERE [UseAssessmentLevelManualInput] = 1 " +
                ") " +
                "FROM MacroStabilityInwardsCalculationOutputEntity;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateOutputCount);

            string validateOutputContent =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity " +
                "JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId) " +
                "WHERE [UseAssessmentLevelManualInput] = 1 " +
                ") " +
                "FROM MacroStabilityInwardsCalculationOutputEntity NEW " +
                "JOIN [SourceProject].MacroStabilityInwardsCalculationOutputEntity AS OLD USING(MacroStabilityInwardsCalculationOutputEntityId) " +
                "WHERE NEW.MacroStabilityInwardsCalculationEntityId = OLD.MacroStabilityInwardsCalculationEntityId " +
                "AND NEW.FactorOfStability IS OLD.FactorOfStability " +
                "AND NEW.ZValue IS OLD.ZValue " +
                "AND NEW.ForbiddenZonesXEntryMin IS OLD.ForbiddenZonesXEntryMin " +
                "AND NEW.ForbiddenZonesXEntryMax IS OLD.ForbiddenZonesXEntryMax " +
                "AND NEW.SlidingCurveLeftSlidingCircleCenterX IS OLD.SlidingCurveLeftSlidingCircleCenterX " +
                "AND NEW.SlidingCurveLeftSlidingCircleCenterY IS OLD.SlidingCurveLeftSlidingCircleCenterY " +
                "AND NEW.SlidingCurveLeftSlidingCircleRadius IS OLD.SlidingCurveLeftSlidingCircleRadius " +
                "AND NEW.SlidingCurveLeftSlidingCircleIsActive = OLD.SlidingCurveLeftSlidingCircleIsActive " +
                "AND NEW.SlidingCurveLeftSlidingCircleNonIteratedForce IS OLD.SlidingCurveLeftSlidingCircleNonIteratedForce " +
                "AND NEW.SlidingCurveLeftSlidingCircleIteratedForce IS OLD.SlidingCurveLeftSlidingCircleIteratedForce " +
                "AND NEW.SlidingCurveLeftSlidingCircleDrivingMoment IS OLD.SlidingCurveLeftSlidingCircleDrivingMoment " +
                "AND NEW.SlidingCurveLeftSlidingCircleResistingMoment IS OLD.SlidingCurveLeftSlidingCircleResistingMoment " +
                "AND NEW.SlidingCurveRightSlidingCircleCenterX IS OLD.SlidingCurveRightSlidingCircleCenterX " +
                "AND NEW.SlidingCurveRightSlidingCircleCenterY IS OLD.SlidingCurveRightSlidingCircleCenterY " +
                "AND NEW.SlidingCurveRightSlidingCircleRadius IS OLD.SlidingCurveRightSlidingCircleRadius " +
                "AND NEW.SlidingCurveRightSlidingCircleIsActive = OLD.SlidingCurveRightSlidingCircleIsActive " +
                "AND NEW.SlidingCurveRightSlidingCircleNonIteratedForce IS OLD.SlidingCurveRightSlidingCircleNonIteratedForce " +
                "AND NEW.SlidingCurveRightSlidingCircleIteratedForce IS OLD.SlidingCurveRightSlidingCircleIteratedForce " +
                "AND NEW.SlidingCurveRightSlidingCircleDrivingMoment IS OLD.SlidingCurveRightSlidingCircleDrivingMoment " +
                "AND NEW.SlidingCurveRightSlidingCircleResistingMoment IS OLD.SlidingCurveRightSlidingCircleResistingMoment " +
                "AND NEW.SlidingCurveNonIteratedHorizontalForce IS OLD.SlidingCurveNonIteratedHorizontalForce " +
                "AND NEW.SlidingCurveIteratedHorizontalForce IS OLD.SlidingCurveIteratedHorizontalForce " +
                "AND NEW.SlipPlaneLeftGridXLeft IS OLD.SlipPlaneLeftGridXLeft " +
                "AND NEW.SlipPlaneLeftGridXRight IS OLD.SlipPlaneLeftGridXRight " +
                "AND NEW.SlipPlaneLeftGridNrOfHorizontalPoints = OLD.SlipPlaneLeftGridNrOfHorizontalPoints " +
                "AND NEW.SlipPlaneLeftGridZTop IS OLD.SlipPlaneLeftGridZTop " +
                "AND NEW.SlipPlaneLeftGridZBottom IS OLD.SlipPlaneLeftGridZBottom " +
                "AND NEW.SlipPlaneLeftGridNrOfVerticalPoints = OLD.SlipPlaneLeftGridNrOfVerticalPoints " +
                "AND NEW.SlipPlaneRightGridXLeft IS OLD.SlipPlaneRightGridXLeft " +
                "AND NEW.SlipPlaneRightGridXRight IS OLD.SlipPlaneRightGridXRight " +
                "AND NEW.SlipPlaneRightGridNrOfHorizontalPoints = OLD.SlipPlaneRightGridNrOfHorizontalPoints " +
                "AND NEW.SlipPlaneRightGridZTop IS OLD.SlipPlaneRightGridZTop " +
                "AND NEW.SlipPlaneRightGridZBottom IS OLD.SlipPlaneRightGridZBottom " +
                "AND NEW.SlipPlaneRightGridNrOfVerticalPoints = OLD.SlipPlaneRightGridNrOfVerticalPoints;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateOutputContent);
        }

        #region Dune Locations

        /// <summary>
        /// Class to generate queries which can be used to assert if the dune locations are correctly migrated.
        /// </summary>
        private class DuneErosionFailureMechanismValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the dune location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                CalculationsForMechanismSpecificFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the calculations for the mechanism specific signaling norm.
                /// </summary>
                CalculationsForMechanismSpecificSignalingNorm = 2,

                /// <summary>
                /// Represents the calculations for the mechanism specific lower limit norm.
                /// </summary>
                CalculationsForMechanismSpecificLowerLimitNorm = 3,

                /// <summary>
                /// Represents the calculations for the lower limit norm.
                /// </summary>
                CalculationsForLowerLimitNorm = 4,

                /// <summary>
                /// Represents the calculations for the factorized lower limit norm.
                /// </summary>
                CalculationsForLowerFactorizedLimitNorm = 5
            }

            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="DuneErosionFailureMechanismValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public DuneErosionFailureMechanismValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created dune location calculations per failure mechanism.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of dune location calculations per calculation type.</returns>
            public string GetDuneLocationCalculationsCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetDuneLocationCalculationsQuery(calculationType) +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "COUNT() as OldCount " +
                       "FROM [SourceProject].DuneErosionFailureMechanismMetaEntity  " +
                       "JOIN [SourceProject].DuneLocationEntity USING(FailureMechanismEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") USING(DuneErosionFailureMechanismMetaEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       "UNION " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "NewCount, " +
                       "COUNT() AS OldCount " +
                       "FROM [SourceProject].DuneErosionFailureMechanismMetaEntity " +
                       "JOIN [SourceProject].DuneLocationEntity USING(FailureMechanismEntityId) " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId],  " +
                       "COUNT() as NewCount " +
                       GetDuneLocationCalculationsQuery(calculationType) +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") USING(DuneErosionFailureMechanismMetaEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetDuneLocationCalculationsQuery(CalculationType calculationType)
            {
                return "FROM DuneErosionFailureMechanismMetaEntity fme " +
                       "JOIN DuneLocationCalculationCollectionEntity ON  " +
                       $"DuneLocationCalculationCollectionEntityId = fme.DuneLocationCalculationCollectionEntity{(int) calculationType}Id " +
                       "JOIN DuneLocationCalculationEntity USING(DuneLocationCalculationCollectionEntityId) ";
            }
        }

        #endregion

        #region Serializers

        /// <summary>
        /// Test helper to assert the migrated values of serialized data.
        /// </summary>
        private static class MigratedSerializedDataTestHelper
        {
            private const string oldNamespace = "Application.Ringtoets.Storage.Serializers";

            /// <summary>
            /// Asserts the migrated serialized data related to macro stability inwards output.
            /// </summary>
            /// <param name="reader">The reader to read the migrated database.</param>
            /// <exception cref="AssertionException">Thrown when:
            /// <list type="bullet">
            /// <item>The namespace is still present.</item>
            /// <item>The class name of the serialized data is still present.</item>
            /// </list></exception>
            public static void AssertSerializedMacroStabilityInwardsOutput(MigratedDatabaseReader reader)
            {
                const string outputEntity = "MacroStabilityInwardsCalculationOutputEntity";

                string validateSlidingCurves =
                    "SELECT " +
                    "COUNT() = 0 " +
                    $"FROM {outputEntity} " +
                    "WHERE LIKE('%MacroStabilityInwardsSliceXmlSerializer%', SlidingCurveSliceXML) " +
                    $"OR LIKE('%{oldNamespace}%', SlidingCurveSliceXML)";

                reader.AssertReturnedDataIsValid(validateSlidingCurves);

                string validateTangentLines =
                    "SELECT " +
                    "COUNT() = 0 " +
                    $"FROM {outputEntity} " +
                    "WHERE LIKE('%TangentLinesXmlSerializer%', SlipPlaneTangentLinesXml) " +
                    $"OR LIKE('%{oldNamespace}%', SlipPlaneTangentLinesXml)";

                reader.AssertReturnedDataIsValid(validateTangentLines);
            }

            /// <summary>
            /// Asserts the migrated serialized data related to dike profiles.
            /// </summary>
            /// <param name="reader">The reader to read the migrated database.</param>
            /// <exception cref="AssertionException">Thrown when:
            /// <list type="bullet">
            /// <item>The namespace is still present.</item>
            /// <item>The class name of the serialized data is still present.</item>
            /// </list></exception>
            public static void AssertSerializedDikeProfileRoughnessPoints(MigratedDatabaseReader reader)
            {
                string validateDikeGeometry =
                    "SELECT " +
                    "COUNT() = 0 " +
                    "FROM DikeProfileEntity " +
                    "WHERE LIKE('%RoughnessPointXmlSerializer%', DikeGeometryXml) " +
                    $"OR LIKE('%{oldNamespace}%', DikeGeometryXml)";

                reader.AssertReturnedDataIsValid(validateDikeGeometry);
            }

            /// <summary>
            /// Asserts the migrated serialized data related to the surface lines.
            /// </summary>
            /// <param name="reader">The reader to read the migrated database.</param>
            /// <exception cref="AssertionException">Thrown when:
            /// <list type="bullet">
            /// <item>The namespace is still present.</item>
            /// <item>The class name of the serialized data is still present.</item>
            /// </list></exception>
            public static void AssertSerializedSurfaceLine(MigratedDatabaseReader reader)
            {
                string validateSurfaceLinePoints =
                    "SELECT " +
                    "COUNT() = 0 " +
                    "FROM SurfaceLineEntity " +
                    "WHERE LIKE('%Point3DXmlSerializer%', PointsXml) " +
                    $"OR LIKE('%{oldNamespace}%', PointsXml)";

                reader.AssertReturnedDataIsValid(validateSurfaceLinePoints);
            }

            /// <summary>
            /// Asserts the migrated serialized data related to serialized 2D point collections.
            /// </summary>
            /// <param name="reader">The reader to read the migrated database.</param>
            /// <exception cref="AssertionException">Thrown when:
            /// <list type="bullet">
            /// <item>The namespace is still present.</item>
            /// <item>The class name of the serialized data is still present.</item>
            /// </list></exception>
            public static void AssertSerializedPoint2DCollection(MigratedDatabaseReader reader)
            {
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("AssessmentSectionEntity", "ReferenceLinePointXml"));
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("FailureMechanismSectionEntity", "FailureMechanismSectionPointXml"));
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("StochasticSoilModelEntity", "StochasticSoilModelSegmentPointXml"));
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("DikeProfileEntity", "ForeshoreXml"));
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("ForeshoreProfileEntity", "GeometryXml"));
                reader.AssertReturnedDataIsValid(GenerateSerializedPoint2DValidationQuery("MacroStabilityInwardsSoilLayerTwoDEntity", "OuterRingXml"));
            }

            private static string GenerateSerializedPoint2DValidationQuery(string tableName,
                                                                           string columnName)
            {
                return "SELECT " +
                       "COUNT() = 0 " +
                       $"FROM {tableName} " +
                       $"WHERE LIKE('%Point2DXmlSerializer%', {columnName}) " +
                       $"OR LIKE('%{oldNamespace}%', {columnName})";
            }
        }

        #endregion

        #region Failure Mechanism Section Result Entities

        private static void AssertHeightStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity) " +
                "FROM HeightStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].HeightStructuresSectionResultEntity OLD USING(HeightStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.HeightStructuresCalculationEntityId IS OLD.HeightStructuresCalculationEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertClosingStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity) " +
                "FROM ClosingStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructuresSectionResultEntity OLD USING(ClosingStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.ClosingStructuresCalculationEntityId IS OLD.ClosingStructuresCalculationEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStabilityPointStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity) " +
                "FROM StabilityPointStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity OLD USING(StabilityPointStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.StabilityPointStructuresCalculationEntityId IS OLD.StabilityPointStructuresCalculationEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverErosionInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity) " +
                "FROM GrassCoverErosionInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity OLD USING(GrassCoverErosionInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.GrassCoverErosionInwardsCalculationEntityId IS OLD.GrassCoverErosionInwardsCalculationEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertPipingSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].PipingSectionResultEntity) " +
                "FROM PipingSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].PipingSectionResultEntity OLD USING(PipingSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMacroStabilityInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity) " +
                "FROM MacroStabilityInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity OLD USING(MacroStabilityInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertDuneErosionSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].DuneErosionSectionResultEntity) " +
                "FROM DuneErosionSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].DuneErosionSectionResultEntity OLD USING(DuneErosionSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverErosionOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity) " +
                "FROM GrassCoverErosionOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity OLD USING(GrassCoverErosionOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStabilityStoneCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity) " +
                "FROM StabilityStoneCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity OLD USING(StabilityStoneCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertWaveImpactAsphaltCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity) " +
                "FROM WaveImpactAsphaltCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity OLD USING(WaveImpactAsphaltCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverSlipOffInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity) " +
                "FROM GrassCoverSlipOffInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity OLD USING (GrassCoverSlipOffInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverSlipOffOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity) " +
                "FROM GrassCoverSlipOffOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity OLD USING (GrassCoverSlipOffOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMacroStabilityOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity) " +
                "FROM MacroStabilityOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity OLD USING (MacroStabilityOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.DetailedAssessmentProbability IS OLD.LayerTwoA " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentProbability IS NULL " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMicrostabilitySectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity) " +
                "FROM MicrostabilitySectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MicrostabilitySectionResultEntity OLD USING (MicrostabilitySectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertPipingStructureSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].PipingStructureSectionResultEntity) " +
                "FROM PipingStructureSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].PipingStructureSectionResultEntity OLD USING (PipingStructureSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStrengthStabilityLengthwiseConstructionSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity) " +
                "FROM StrengthStabilityLengthwiseConstructionSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity OLD USING (StrengthStabilityLengthwiseConstructionSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertTechnicalInnovationSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity) " +
                "FROM TechnicalInnovationSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].TechnicalInnovationSectionResultEntity OLD USING (TechnicalInnovationSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertWaterPressureAsphaltCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity) " +
                "FROM WaterPressureAsphaltCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity OLD USING (WaterPressureAsphaltCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssembly = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        #endregion

        #region  Migrated Hydraulic Boundary Locations on Assessment section

        private static void AssertWaveHeightCalculationEntitiesOnAssessmentSection(MigratedDatabaseReader reader,
                                                                                   HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
        }

        private static void AssertDesignWaterLevelCalculationEntitiesOnAssessmentSection(MigratedDatabaseReader reader,
                                                                                         HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
        }

        /// <summary>
        /// Class to generate queries which can be used to assert if the hydraulic boundary locations 
        /// are correctly migrated on the assessment section level.
        /// </summary>
        private class HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the hydraulic location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the water level calculations for the factorized signaling norm.
                /// </summary>
                WaterLevelCalculationsForFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the water level calculations for the signaling norm.
                /// </summary>
                WaterLevelCalculationsForSignalingNorm = 2,

                /// <summary>
                /// Represents the water level calculations for the lower limit norm.
                /// </summary>
                WaterLevelCalculationsForLowerLimitNorm = 3,

                /// <summary>
                /// Represents the water level calculations for the factorized lower limit norm.
                /// </summary>
                WaterLevelCalculationsForFactorizedLowerLimitNorm = 4,

                /// <summary>
                /// Represents the wave height calculations for the factorized signaling norm.
                /// </summary>
                WaveHeightCalculationsForFactorizedSignalingNorm = 5,

                /// <summary>
                /// Represents the wave height calculations for the signaling norm.
                /// </summary>
                WaveHeightCalculationsForSignalingNorm = 6,

                /// <summary>
                /// Represents the wave height calculations for the lower limit norm.
                /// </summary>
                WaveHeightCalculationsForLowerLimitNorm = 7,

                /// <summary>
                /// Represents the wave height calculations for the factorized lower limit norm.
                /// </summary>
                WaveHeightCalculationsForFactorizedLowerLimitNorm = 8
            }

            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrEmpty(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created hydraulic boundary location calculations per assessment section.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of hydraulic boundary location calculations per assessment section.</returns>
            public string GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "ase.AssessmentSectionEntityId, " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "LEFT JOIN  " +
                       "( " +
                       "SELECT   " +
                       "sourceAse.AssessmentSectionEntityId, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity sourceHle " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse ON sourceHle.AssessmentSectionEntityId = sourceAse.AssessmentSectionEntityId " +
                       "GROUP BY sourceAse.AssessmentSectionEntityId " +
                       ") USING(AssessmentSectionEntityId) " +
                       "GROUP BY ase.AssessmentSectionEntityId " +
                       "UNION " +
                       "SELECT " +
                       "sourceAse.AssessmentSectionEntityId, " +
                       "NewCount, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity sourceHle " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse ON sourceHle.AssessmentSectionEntityId = sourceAse.AssessmentSectionEntityId " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "ase.AssessmentSectionEntityId, " +
                       "COUNT() AS NewCount " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "GROUP BY ase.AssessmentSectionEntityId " +
                       ") USING(AssessmentSectionEntityId) " +
                       "GROUP BY sourceAse.AssessmentSectionEntityId " +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the design water level calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the design water level calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
                       $"WHERE OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the wave height calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the wave height calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
                       $"WHERE OLD.ShouldWaveHeightIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the input should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation input.</returns>
            public string GetNewCalculationsValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "WHERE ShouldIllustrationPointsBeCalculated = 0;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetHydraulicLocationCalculationsFromCollectionQuery(CalculationType calculationType)
            {
                return "FROM AssessmentSectionEntity ase " +
                       $"JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id " +
                       "= hlcce.HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId) ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding design water level calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static CalculationType ConvertToDesignWaterLevelCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaterLevelCalculationsForLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaterLevelCalculationsForSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding wave height calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static CalculationType ConvertToWaveHeightCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaveHeightCalculationsForLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaveHeightCalculationsForSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Migrated Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism

        private static void AssertDesignWaterLevelCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader,
                                                                                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
        }

        private static void AssertWaveHeightCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader,
                                                                                                           HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
        }

        /// <summary>
        /// Class to generate queries which can be used to assert if the hydraulic boundary locations 
        /// are correctly migrated on the grass cover erosion outwards failure mechanism level.
        /// </summary>
        private class HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the hydraulic location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the water level calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the water level calculations for the mechanism specific signaling norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificSignalingNorm = 2,

                /// <summary>
                /// Represents the water level calculations for the mechanism specific lower limit norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificLowerLimitNorm = 3,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm = 4,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific signaling norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificSignalingNorm = 5,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific lower limit norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificLowerLimitNorm = 6
            }

            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created hydraulic boundary location calculations per failure mechanism.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of hydraulic boundary location calculations per failure mechanism.</returns>
            public string GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "LEFT JOIN  " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT(distinct GrassCoverErosionOutwardsHydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId " +
                       ") USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId " +
                       "UNION " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "NewCount, " +
                       "COUNT(distinct GrassCoverErosionOutwardsHydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT() AS NewCount " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "GROUP BY FailureMechanismEntityId " +
                       ") USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId" +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the design water level calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the design water level calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId) " +
                       "JOIN( " +
                       "SELECT " +
                       "LocationId, " +
                       "AssessmentSectionEntityId, " +
                       "ShouldDesignWaterLevelIllustrationPointsBeCalculated " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD ON(OLD.LocationId = hl.LocationId AND OLD.AssessmentSectionEntityId = fm.AssessmentSectionEntityId) " +
                       $"WHERE OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the wave height calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the wave height calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId) " +
                       "JOIN( " +
                       "SELECT " +
                       "LocationId, " +
                       "AssessmentSectionEntityId, " +
                       "ShouldWaveHeightIllustrationPointsBeCalculated " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD ON(OLD.LocationId = hl.LocationId AND OLD.AssessmentSectionEntityId = fm.AssessmentSectionEntityId) " +
                       $"WHERE OLD.ShouldWaveHeightIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the input should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation output.</returns>
            public string GetNewCalculationsValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity) " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "WHERE ShouldIllustrationPointsBeCalculated = 0;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetHydraulicLocationCalculationsFromFailureMechanismQuery(CalculationType calculationType)
            {
                return "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmm " +
                       "JOIN HydraulicLocationCalculationCollectionEntity " +
                       $"ON gceofmm.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId) ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding design water level calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static CalculationType ConvertToDesignWaterLevelCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaterLevelCalculationsForMechanismSpecificSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding wave height calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static CalculationType ConvertToWaveHeightCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaveHeightCalculationsForMechanismSpecificSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Migrated Wave Condition Calculations

        private static void AssertWaveConditionsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new WaveConditionsCalculationValidationQueryGenerator(sourceFilePath);

            reader.AssertReturnedDataIsValid(queryGenerator.GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType.SignalingNorm));
        }

        /// <summary>
        /// Class to generate queries which can be used to assert if the wave conditions calculations
        /// are correctly migrated.
        /// </summary>
        private class WaveConditionsCalculationValidationQueryGenerator
        {
            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="WaveConditionsCalculationValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public WaveConditionsCalculationValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate if the grass cover erosion outwards calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the grass cover erosion outwards calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType normType)
            {
                FailureMechanismCategoryType categoryType = ConvertToFailureMechanismCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                       "JOIN CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       "LEFT JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN ( " +
                       "SELECT " +
                       "[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId], " +
                       "[LocationId], " +
                       "[AssessmentSectionEntityId], " +
                       "calc.CalculationGroupEntityId, " +
                       "[ForeshoreProfileEntityId], " +
                       "calc.'Order', " +
                       "calc.Name, " +
                       "[Comments], " +
                       "[UseBreakWater], " +
                       "[BreakWaterType], " +
                       "[BreakWaterHeight], " +
                       "[UseForeshore], " +
                       "[Orientation], " +
                       "[UpperBoundaryRevetment], " +
                       "[LowerBoundaryRevetment], " +
                       "[UpperBoundaryWaterLevels], " +
                       "[LowerBoundaryWaterLevels], " +
                       "[StepSize] " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity calc " +
                       "LEFT JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "LEFT JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND OLD.LocationId IS hl.LocationId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the stability stone cover calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the migrated stability stone cover calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType normType)
            {
                AssessmentSectionCategoryType categoryType = ConvertToAssessmentSectionCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM StabilityStoneCoverWaveConditionsCalculationEntity NEW " +
                       "JOIN [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity OLD USING(StabilityStoneCoverWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND NEW.HydraulicLocationEntityId IS OLD.HydraulicLocationEntityId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the wave impact asphalt cover calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the migrated wave impact asphalt cover calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType normType)
            {
                AssessmentSectionCategoryType categoryType = ConvertToAssessmentSectionCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM WaveImpactAsphaltCoverWaveConditionsCalculationEntity NEW " +
                       "JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity OLD USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND NEW.HydraulicLocationEntityId IS OLD.HydraulicLocationEntityId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetCommonWaveConditionsCalculationPropertiesValidationString(NormativeNormType normType)
            {
                return "JOIN CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId)" +
                       $"WHERE ase.NormativeNormType = {(int) normType} " +
                       "AND NEW.CalculationGroupEntityId = OLD.CalculationGroupEntityId " +
                       "AND NEW.ForeshoreProfileEntityId IS OLD.ForeshoreProfileEntityId " +
                       "AND NEW.\"Order\" = OLD.\"Order\" " +
                       "AND NEW.Name IS OLD.Name " +
                       "AND NEW.Comments IS OLD.Comments " +
                       "AND NEW.UseBreakWater = OLD.UseBreakWater " +
                       "AND NEW.BreakWaterType = OLD.BreakWaterType " +
                       "AND NEW.BreakWaterHeight IS OLD.BreakWaterHeight " +
                       "AND NEW.UseForeshore = OLD.UseForeshore " +
                       "AND NEW.Orientation IS OLD.Orientation " +
                       "AND NEW.UpperBoundaryRevetment IS OLD.UpperBoundaryRevetment " +
                       "AND NEW.LowerBoundaryRevetment IS OLD.LowerBoundaryRevetment " +
                       "AND NEW.UpperBoundaryWaterLevels IS OLD.UpperBoundaryWaterLevels " +
                       "AND NEW.LowerBoundaryWaterLevels IS OLD.LowerBoundaryWaterLevels " +
                       "AND NEW.StepSize = OLD.StepSize ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding category type from <see cref="FailureMechanismCategoryType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="FailureMechanismCategoryType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static FailureMechanismCategoryType ConvertToFailureMechanismCategoryType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.SignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    case NormativeNormType.LowerLimitNorm:
                        return FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding category type from <see cref="AssessmentSectionCategoryType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="AssessmentSectionCategoryType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static AssessmentSectionCategoryType ConvertToAssessmentSectionCategoryType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.SignalingNorm:
                        return AssessmentSectionCategoryType.SignalingNorm;
                    case NormativeNormType.LowerLimitNorm:
                        return AssessmentSectionCategoryType.LowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}