// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo212IntegrationTest
    {
        private const string newVersion = "21.2";

        [Test]
        public void Given211Project_WhenUpgradedTo212_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject211.risk");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given211Project_WhenUpgradedTo212_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given211Project_WhenUpgradedTo212_ThenProjectAsExpected), ".log"));
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

                    AssertGrassCoverErosionInwardsCalculation(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsOutput(reader);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertGrassCoverErosionInwardsCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string getNormQuery =
                "JOIN ( " +
                "WITH CalculationGroups AS ( " +
                "SELECT " +
                "CalculationGroupEntityId, " +
                "ParentCalculationGroupEntityId AS OriginalParentId, " +
                "ParentCalculationGroupEntityId AS NextParentId, " +
                "NULL as RootId, " +
                "CASE " +
                "WHEN ParentCalculationGroupEntityId IS NULL " +
                "THEN 1 " +
                "END AS IsRoot " +
                "FROM CalculationGroupEntity " +
                "UNION ALL " +
                "SELECT " +
                "CalculationGroups.CalculationGroupEntityId, " +
                "CalculationGroups.OriginalParentId, " +
                "entity.ParentCalculationGroupEntityId, " +
                "CASE " +
                "WHEN entity.ParentCalculationGroupEntityId IS NULL " +
                "THEN CalculationGroups.NextParentId " +
                "ELSE " +
                "CalculationGroups.RootId " +
                "END, " +
                "NULL " +
                "FROM CalculationGroups " +
                "INNER JOIN CalculationGroupEntity entity " +
                "ON CalculationGroups.NextParentId = entity.CalculationGroupEntityId " +
                ") " +
                "SELECT " +
                "CalculationGroupEntityId as OriginalGroupId, " +
                "CASE " +
                "WHEN IsRoot = 1 " +
                "THEN CalculationGroupEntityId " +
                "ELSE RootId " +
                "END AS FinalGroupId " +
                "FROM CalculationGroups " +
                "WHERE RootId IS NOT NULL OR IsRoot = 1) " +
                "ON OLD.CalculationGroupEntityId = OriginalGroupId " +
                "JOIN ( " +
                "SELECT " +
                "AssessmentSectionEntityId AS failureMechanismAssessmentSectionId, " +
                "CalculationGroupEntityId AS failureMechanismCalculationGroupEntityId " +
                "FROM FailureMechanismEntity) " +
                "ON failureMechanismCalculationGroupEntityId = FinalGroupId " +
                "JOIN ( " +
                "SELECT " +
                "AssessmentSectionEntityId AS sectionId, " +
                "CASE " +
                "WHEN NormativeNormType IS 1 " +
                "THEN LowerLimitNorm " +
                "ELSE SignalingNorm " +
                "END AS Norm " +
                "FROM AssessmentSectionEntity) " +
                "ON sectionId = failureMechanismAssessmentSectionId";

            string validateCalculationWithoutDikeHeightAndOvertoppingRate =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "WHERE DikeHeightCalculationType = 1 " +
                "AND OvertoppingRateCalculationType = 1 " +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                $"{getNormQuery} " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldDikeHeightBeCalculated] = 0 " +
                "AND NEW.[DikeHeightTargetProbability] = Norm " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateBeCalculated] = 0 " +
                "AND NEW.[OvertoppingRateTargetProbability] = Norm " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[ScenarioContribution] = OLD.[ScenarioContribution]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationWithoutDikeHeightAndOvertoppingRate);

            string validateCalculationWithDikeHeightAndOvertoppingRate =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "WHERE DikeHeightCalculationType != 1 " +
                "AND OvertoppingRateCalculationType != 1 " +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                $"{getNormQuery} " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldDikeHeightBeCalculated] = 1 " +
                "AND NEW.[DikeHeightTargetProbability] = Norm " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateBeCalculated] = 1 " +
                "AND NEW.[OvertoppingRateTargetProbability] = Norm " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[ScenarioContribution] = OLD.[ScenarioContribution]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationWithDikeHeightAndOvertoppingRate);
        }

        private static void AssertGrassCoverErosionInwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateDikeHeightOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateDikeHeightOutput);

            const string validateOvertoppingRateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOvertoppingRateOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOvertoppingRateOutput);
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
                "ClosingStructuresOutputEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneErosionSectionResultEntity",
                "DuneLocationCalculationCollectionEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationOutputEntity",
                "DuneLocationEntity",
                "FailureMechanismEntity",
                "FailureMechanismSectionEntity",
                "FaultTreeIllustrationPointEntity",
                "FaultTreeIllustrationPointStochastEntity",
                "FaultTreeSubmechanismIllustrationPointEntity",
                "ForeshoreProfileEntity",
                "GeneralResultFaultTreeIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointStochastEntity",
                "GeneralResultSubMechanismIllustrationPointEntity",
                "GeneralResultSubMechanismIllustrationPointStochastEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresOutputEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicBoundaryDatabaseEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationEntity",
                "HydraulicLocationOutputEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCalculationOutputEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSectionResultEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "MacroStabilityOutwardsFailureMechanismMetaEntity",
                "MacroStabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "PipingStructureSectionResultEntity",
                "ProbabilisticPipingCalculationEntity",
                "ProbabilisticPipingCalculationOutputEntity",
                "ProjectEntity",
                "SemiProbabilisticPipingCalculationEntity",
                "SemiProbabilisticPipingCalculationOutputEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresOutputEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StabilityStoneCoverWaveConditionsOutputEntity",
                "StochastEntity",
                "StochasticSoilModelEntity",
                "StrengthStabilityLengthwiseConstructionSectionResultEntity",
                "SubMechanismIllustrationPointEntity",
                "SubMechanismIllustrationPointStochastEntity",
                "SurfaceLineEntity",
                "TechnicalInnovationSectionResultEntity",
                "TopLevelFaultTreeIllustrationPointEntity",
                "TopLevelSubMechanismIllustrationPointEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverWaveConditionsCalculationEntity",
                "WaveImpactAsphaltCoverWaveConditionsOutputEntity"
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

                Assert.AreEqual(2, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "Gevolgen van de migratie van versie 21.1 naar versie 21.2:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "* Geen aanpassingen."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"21.2\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }
    }
}