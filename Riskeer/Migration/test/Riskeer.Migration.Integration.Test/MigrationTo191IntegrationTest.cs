// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    public class MigrationTo191IntegrationTest
    {
        private const string newVersion = "19.1";

        [Test]
        public void Given181Project_WhenUpgradedTo191_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject181.rtd");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given181Project_WhenUpgradedTo191_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given181Project_WhenUpgradedTo191_ThenProjectAsExpected), ".log"));
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
                    AssertBackgroundData(reader, sourceFilePath);

                    AssertForeshoreProfile(reader, sourceFilePath);

                    AssertPipingSoilLayers(reader);

                    AssertGrassCoverErosionOutwardsWaveConditionsCalculations(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsWaveConditionsOutput(reader, sourceFilePath);

                    AssertStabilityStoneCoverWaveConditionsCalculations(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverWaveConditionsCalculations(reader, sourceFilePath);

                    AssertHeightStructuresCalculation(reader, sourceFilePath);
                    AssertClosingStructuresCalculation(reader, sourceFilePath);
                    AssertStabilityPointStructuresCalculations(reader, sourceFilePath);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSection =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM SOURCEPROJECT.AssessmentSectionEntity) " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[ProjectEntityId] = OLD.[ProjectEntityId] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity1Id] = OLD.[HydraulicLocationCalculationCollectionEntity1Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity2Id] = OLD.[HydraulicLocationCalculationCollectionEntity2Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity3Id] = OLD.[HydraulicLocationCalculationCollectionEntity3Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity4Id] = OLD.[HydraulicLocationCalculationCollectionEntity4Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity5Id] = OLD.[HydraulicLocationCalculationCollectionEntity5Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity6Id] = OLD.[HydraulicLocationCalculationCollectionEntity6Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity7Id] = OLD.[HydraulicLocationCalculationCollectionEntity7Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity8Id] = OLD.[HydraulicLocationCalculationCollectionEntity8Id] " +
                "AND NEW.[Id] IS OLD.[Id] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[LowerLimitNorm] = OLD.[LowerLimitNorm] " +
                "AND NEW.[SignalingNorm] = OLD.[SignalingNorm] " +
                "AND NEW.[NormativeNormType] = OLD.[NormativeNormType] " +
                "AND NEW.[Composition] = OLD.[Composition] " +
                "AND NEW.[ReferenceLinePointXml] IS OLD.[ReferenceLinePointXml] " +
                "AND NEW.\"Order\" =  OLD.\"Order\"; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateAssessmentSection);

            string validateHydraulicDatabase =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AssessmentSectionEntity " +
                "WHERE HydraulicDatabaseLocation IS NOT NULL " +
                ") " +
                "FROM HydraulicBoundaryDatabaseEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[Version] = OLD.[HydraulicDatabaseVersion] " +
                "AND NEW.[FilePath] = OLD.[HydraulicDatabaseLocation] " +
                "AND NEW.[HydraulicLocationConfigurationSettingsFilePath] = rtrim(OLD.[HydraulicDatabaseLocation], replace(OLD.[HydraulicDatabaseLocation], '\\', '')) || 'hlcd.sqlite' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsUsePreprocessorClosure] = 0 " +
                "AND NEW.[HydraulicLocationConfigurationSettingsScenarioName] = 'WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsYear] = 2023 " +
                "AND NEW.[HydraulicLocationConfigurationSettingsScope] = 'WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsSeaLevel] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsRiverDischarge] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsLakeLevel] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsWindDirection] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsWindSpeed] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsComment] IS 'Gegenereerd door Riskeer (conform WBI2017)'; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicDatabase);
        }

        private static void AssertBackgroundData(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateTransparency =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM " +
                "(" +
                "SELECT " +
                "CASE WHEN (NEW.[Transparency] = OLD.[Transparency] AND OLD.[Transparency] > 0) " +
                "OR (NEW.[Transparency] = 0.6 AND OLD.[Transparency] = 0) " +
                "THEN 0 " +
                "ELSE 1 " +
                "END AS [IsInvalid] " +
                "FROM BackgroundDataEntity NEW " +
                "JOIN [SOURCEPROJECT].BackgroundDataEntity OLD USING(BackgroundDataEntityId) " +
                "); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateTransparency);

            string validateBackgroundData =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].BackgroundDataEntity) " +
                "FROM BackgroundDataEntity NEW " +
                "JOIN [SOURCEPROJECT].BackgroundDataEntity OLD USING(BackgroundDataEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[Name] = OLD.[Name] " +
                "AND NEW.[IsVisible] = OLD.[IsVisible] " +
                "AND NEW.[BackgroundDataType] = OLD.[BackgroundDataType]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateBackgroundData);
        }

        private static void AssertGrassCoverErosionOutwardsWaveConditionsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculationsWithoutForeshoreProfile =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                "WHERE ForeshoreProfileEntityId IS NULL " +
                ") " +
                "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                "JOIN ( " +
                "SELECT " +
                "[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId], " +
                "[CalculationGroupEntityId], " +
                "[ForeshoreProfileEntityId], " +
                "[HydraulicLocationEntityId], " +
                "CALC.\"Order\", " +
                "[Name], " +
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
                "[StepSize], " +
                "[CategoryType], " +
                "CASE WHEN GrassCoverErosionOutwardsWaveConditionsOutputEntityId IS NOT NULL " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS HasOutput " +
                "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity CALC " +
                "LEFT JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "GROUP BY GrassCoverErosionOutwardsWaveConditionsCalculationEntityId " +
                ") AS OLD USING (GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "WHERE OLD.ForeshoreProfileEntityId IS NULL " +
                "AND NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] = OLD.[CategoryType] " +
                "AND ((HasOutput = 1 AND NEW.[CalculationType] = 2) OR (HasOutput = 0 AND NEW.[CalculationType] = 3)); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationsWithoutForeshoreProfile);

            string validateCalculationsWithForeshoreProfile =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                "WHERE ForeshoreProfileEntityId IS NOT NULL " +
                ") " +
                "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                "JOIN ( " +
                "SELECT " +
                "[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId], " +
                "[CalculationGroupEntityId], " +
                "[ForeshoreProfileEntityId], " +
                "[HydraulicLocationEntityId], " +
                "CALC.\"Order\", " +
                "CALC.[Name], " +
                "[Comments], " +
                "CALC.[UseBreakWater], " +
                "CALC.[BreakWaterType], " +
                "CALC.[BreakWaterHeight], " +
                "CALC.[UseForeshore], " +
                "CALC.[Orientation], " +
                "[UpperBoundaryRevetment], " +
                "[LowerBoundaryRevetment], " +
                "[UpperBoundaryWaterLevels], " +
                "[LowerBoundaryWaterLevels], " +
                "[StepSize], " +
                "[CategoryType], " +
                "CASE WHEN GrassCoverErosionOutwardsWaveConditionsOutputEntityId IS NOT NULL " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS HasOutput," +
                "CASE WHEN " +
                "(LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1 " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS HasValidForeshore " +
                "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity CALC " +
                "LEFT JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId) " +
                "GROUP BY GrassCoverErosionOutwardsWaveConditionsCalculationEntityId " +
                ") OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "WHERE OLD.ForeshoreProfileEntityId IS NOT NULL " +
                "AND NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" =  OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND (HasValidForeshore = 1 AND NEW.[UseBreakWater] = OLD.[UseBreakWater]) OR (HasValidForeshore = 0 AND NEW.[UseBreakWater] = 0) " +
                "AND (HasValidForeshore = 1 AND NEW.[BreakWaterType] = OLD.[BreakWaterType]) OR (HasValidForeshore = 0 AND NEW.[BreakWaterType] = 3) " +
                "AND (HasValidForeshore = 1 AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight]) OR (HasValidForeshore = 0 AND NEW.[BreakWaterHeight] IS NULL) " +
                "AND (HasValidForeshore = 1 AND NEW.[UseForeshore] = OLD.[UseForeshore]) OR (HasValidForeshore = 0 AND NEW.[UseForeshore] = 0) " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] = OLD.[CategoryType] " +
                "AND ((HasOutput = 1 AND NEW.[CalculationType] = 2) OR (HasOutput = 0 AND NEW.[CalculationType] = 3)); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationsWithForeshoreProfile);
        }

        private static void AssertGrassCoverErosionOutwardsWaveConditionsOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateOutputs =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity) " +
                "FROM GrassCoverErosionOutwardsWaveConditionsOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity OLD USING(GrassCoverErosionOutwardsWaveConditionsOutputEntityId) " +
                "WHERE NEW.[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId] = OLD.[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[OutputType] = 2 " +
                "AND NEW.[WaterLevel] IS OLD.[WaterLevel] " +
                "AND NEW.[WaveHeight] IS OLD.[WaveHeight] " +
                "AND NEW.[WavePeakPeriod] IS OLD.[WavePeakPeriod] " +
                "AND NEW.[WaveAngle] IS OLD.[WaveAngle] " +
                "AND NEW.[WaveDirection] IS OLD.[WaveDirection] " +
                "AND NEW.[TargetProbability] IS OLD.[TargetProbability] " +
                "AND NEW.[TargetReliability] IS OLD.[TargetReliability] " +
                "AND NEW.[CalculatedProbability] IS OLD.[CalculatedProbability] " +
                "AND NEW.[CalculatedReliability] IS OLD.[CalculatedReliability] " +
                "AND NEW.[CalculationConvergence] = OLD.[CalculationConvergence]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateOutputs);
        }

        private static void AssertForeshoreProfile(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateForeshoreProfile =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.ForeshoreProfileEntity " +
                "WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1 " +
                ") " +
                "FROM ForeshoreProfileEntity NEW " +
                "JOIN SOURCEPROJECT.ForeshoreProfileEntity OLD USING(ForeshoreProfileEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[Id] = OLD.[Id] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[GeometryXml] = OLD.[GeometryXml] " +
                "AND NEW.[X] IS OLD.[X] " +
                "AND NEW.[Y] IS OLD.[Y] " +
                "AND NEW.[X0] IS OLD.[X0] " +
                "AND NEW.\"Order\" = OLD.\"Order\";" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateForeshoreProfile);
        }

        private static void AssertStabilityStoneCoverWaveConditionsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string calculationEntityName = "StabilityStoneCoverWaveConditionsCalculationEntity";
            const string validForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] =  OLD.[CategoryType] " +
                "AND NEW.[CalculationType] = 3; ";

            AssertCalculationsWithValidForeshoreProfile(reader, sourceFilePath, calculationEntityName, validForeshoreProfileCriteria);

            const string invalidForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] =  OLD.[CategoryType] " +
                "AND NEW.[CalculationType] = 3; ";

            AssertCalculationsWithInvalidForeshoreProfile(reader, sourceFilePath, calculationEntityName, invalidForeshoreProfileCriteria);
        }

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string calculationEntityName = "StabilityStoneCoverWaveConditionsCalculationEntity";
            const string validForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] =  OLD.[UseBreakWater] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] =  OLD.[CategoryType]; ";

            AssertCalculationsWithValidForeshoreProfile(reader, sourceFilePath, calculationEntityName, validForeshoreProfileCriteria);

            const string invalidForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CategoryType] =  OLD.[CategoryType]; ";

            AssertCalculationsWithInvalidForeshoreProfile(reader, sourceFilePath, calculationEntityName, invalidForeshoreProfileCriteria);
        }

        private static void AssertHeightStructuresCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string calculationEntityName = "HeightStructuresCalculationEntity";
            const string validForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HeightStructureEntityId] IS OLD.[HeightStructureEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[LevelCrestStructureMean] IS OLD.[LevelCrestStructureMean] " +
                "AND NEW.[LevelCrestStructureStandardDeviation] IS OLD.[LevelCrestStructureStandardDeviation] " +
                "AND NEW.[DeviationWaveDirection] IS OLD.[DeviationWaveDirection] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithValidForeshoreProfile(reader, sourceFilePath, calculationEntityName, validForeshoreProfileCriteria);

            const string invalidForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HeightStructureEntityId] IS OLD.[HeightStructureEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[LevelCrestStructureMean] IS OLD.[LevelCrestStructureMean] " +
                "AND NEW.[LevelCrestStructureStandardDeviation] IS OLD.[LevelCrestStructureStandardDeviation] " +
                "AND NEW.[DeviationWaveDirection] IS OLD.[DeviationWaveDirection] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithInvalidForeshoreProfile(reader, sourceFilePath, calculationEntityName, invalidForeshoreProfileCriteria);
        }

        private static void AssertClosingStructuresCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string calculationEntityName = "ClosingStructuresCalculationEntity";
            const string validForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[ClosingStructureEntityId] IS OLD.[ClosingStructureEntityId] " +
                "AND NEW.\"Order\" IS OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation]" +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[LevelCrestStructureNotClosingMean] IS OLD.[LevelCrestStructureNotClosingMean] " +
                "AND NEW.[LevelCrestStructureNotClosingStandardDeviation] IS OLD.[LevelCrestStructureNotClosingStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelStandardDeviation] " +
                "AND NEW.[ThresholdHeightOpenWeirMean] IS OLD.[ThresholdHeightOpenWeirMean] " +
                "AND NEW.[ThresholdHeightOpenWeirStandardDeviation] IS OLD.[ThresholdHeightOpenWeirStandardDeviation] " +
                "AND NEW.[AreaFlowAperturesMean] IS OLD.[AreaFlowAperturesMean] " +
                "AND NEW.[AreaFlowAperturesStandardDeviation] IS OLD.[AreaFlowAperturesStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[ProbabilityOpenStructureBeforeFlooding] = OLD.[ProbabilityOpenStructureBeforeFlooding] " +
                "AND NEW.[FailureProbabilityOpenStructure] = OLD.[FailureProbabilityOpenStructure] " +
                "AND NEW.[IdenticalApertures] = OLD.[IdenticalApertures] " +
                "AND NEW.[FailureProbabilityReparation] = OLD.[FailureProbabilityReparation] " +
                "AND NEW.[InflowModelType] = OLD.[InflowModelType] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] = OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[DeviationWaveDirection] IS OLD.[DeviationWaveDirection] " +
                "AND NEW.[DrainCoefficientMean] IS OLD.[DrainCoefficientMean] " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithValidForeshoreProfile(reader, sourceFilePath, calculationEntityName, validForeshoreProfileCriteria);

            const string invalidForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[ClosingStructureEntityId] IS OLD.[ClosingStructureEntityId] " +
                "AND NEW.\"Order\" IS OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation]" +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[LevelCrestStructureNotClosingMean] IS OLD.[LevelCrestStructureNotClosingMean] " +
                "AND NEW.[LevelCrestStructureNotClosingStandardDeviation] IS OLD.[LevelCrestStructureNotClosingStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelStandardDeviation] " +
                "AND NEW.[ThresholdHeightOpenWeirMean] IS OLD.[ThresholdHeightOpenWeirMean] " +
                "AND NEW.[ThresholdHeightOpenWeirStandardDeviation] IS OLD.[ThresholdHeightOpenWeirStandardDeviation] " +
                "AND NEW.[AreaFlowAperturesMean] IS OLD.[AreaFlowAperturesMean] " +
                "AND NEW.[AreaFlowAperturesStandardDeviation] IS OLD.[AreaFlowAperturesStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[ProbabilityOpenStructureBeforeFlooding] = OLD.[ProbabilityOpenStructureBeforeFlooding] " +
                "AND NEW.[FailureProbabilityOpenStructure] = OLD.[FailureProbabilityOpenStructure] " +
                "AND NEW.[IdenticalApertures] = OLD.[IdenticalApertures] " +
                "AND NEW.[FailureProbabilityReparation] = OLD.[FailureProbabilityReparation] " +
                "AND NEW.[InflowModelType] = OLD.[InflowModelType] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] = OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[DeviationWaveDirection] IS OLD.[DeviationWaveDirection] " +
                "AND NEW.[DrainCoefficientMean] IS OLD.[DrainCoefficientMean] " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithInvalidForeshoreProfile(reader, sourceFilePath, calculationEntityName, invalidForeshoreProfileCriteria);
        }

        private static void AssertStabilityPointStructuresCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string calculationEntityName = "StabilityPointStructuresCalculationEntity";
            const string validForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[StabilityPointStructureEntityId] IS OLD.[StabilityPointStructureEntityId] " +
                "AND NEW.\"Order\" IS OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelFailureConstructionMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelFailureConstructionStandardDeviation] " +
                "AND NEW.[ThresholdHeightOpenWeirMean] IS OLD.[ThresholdHeightOpenWeirMean] " +
                "AND NEW.[ThresholdHeightOpenWeirStandardDeviation] IS OLD.[ThresholdHeightOpenWeirStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[ConstructiveStrengthLinearLoadModelMean] IS OLD.[ConstructiveStrengthLinearLoadModelMean] " +
                "AND NEW.[ConstructiveStrengthLinearLoadModelCoefficientOfVariation] IS OLD.[ConstructiveStrengthLinearLoadModelCoefficientOfVariation] " +
                "AND NEW.[ConstructiveStrengthQuadraticLoadModelMean] IS OLD.[ConstructiveStrengthQuadraticLoadModelMean] " +
                "AND NEW.[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation] IS OLD.[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation] " +
                "AND NEW.[BankWidthMean] IS OLD.[BankWidthMean] " +
                "AND NEW.[BankWidthStandardDeviation] IS OLD.[BankWidthStandardDeviation] " +
                "AND NEW.[InsideWaterLevelFailureConstructionMean] IS OLD.[InsideWaterLevelFailureConstructionMean] " +
                "AND NEW.[InsideWaterLevelFailureConstructionStandardDeviation] IS OLD.[InsideWaterLevelFailureConstructionStandardDeviation] " +
                "AND NEW.[EvaluationLevel] IS OLD.[EvaluationLevel] " +
                "AND NEW.[LevelCrestStructureMean] IS OLD.[LevelCrestStructureMean] " +
                "AND NEW.[LevelCrestStructureStandardDeviation] IS OLD.[LevelCrestStructureStandardDeviation] " +
                "AND NEW.[VerticalDistance] IS OLD.[VerticalDistance] " +
                "AND NEW.[FailureProbabilityRepairClosure] = OLD.[FailureProbabilityRepairClosure] " +
                "AND NEW.[FailureCollisionEnergyMean] IS OLD.[FailureCollisionEnergyMean] " +
                "AND NEW.[FailureCollisionEnergyCoefficientOfVariation] IS OLD.[FailureCollisionEnergyCoefficientOfVariation] " +
                "AND NEW.[ShipMassMean] IS OLD.[ShipMassMean] " +
                "AND NEW.[ShipMassCoefficientOfVariation] IS OLD.[ShipMassCoefficientOfVariation] " +
                "AND NEW.[ShipVelocityMean] IS OLD.[ShipVelocityMean] " +
                "AND NEW.[ShipVelocityCoefficientOfVariation] IS OLD.[ShipVelocityCoefficientOfVariation] " +
                "AND NEW.[LevellingCount] = OLD.[LevellingCount] " +
                "AND NEW.[ProbabilityCollisionSecondaryStructure] = OLD.[ProbabilityCollisionSecondaryStructure] " +
                "AND NEW.[FlowVelocityStructureClosableMean] IS OLD.[FlowVelocityStructureClosableMean] " +
                "AND NEW.[StabilityLinearLoadModelMean] IS OLD.[StabilityLinearLoadModelMean] " +
                "AND NEW.[StabilityLinearLoadModelCoefficientOfVariation] IS OLD.[StabilityLinearLoadModelCoefficientOfVariation] " +
                "AND NEW.[StabilityQuadraticLoadModelMean] IS OLD.[StabilityQuadraticLoadModelMean] " +
                "AND NEW.[StabilityQuadraticLoadModelCoefficientOfVariation] IS OLD.[StabilityQuadraticLoadModelCoefficientOfVariation] " +
                "AND NEW.[AreaFlowAperturesMean] IS OLD.[AreaFlowAperturesMean] " +
                "AND NEW.[AreaFlowAperturesStandardDeviation] IS OLD.[AreaFlowAperturesStandardDeviation] " +
                "AND NEW.[InflowModelType] = OLD.[InflowModelType] " +
                "AND NEW.[LoadSchematizationType] = OLD.[LoadSchematizationType] " +
                "AND NEW.[VolumicWeightWater] IS OLD.[VolumicWeightWater] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[DrainCoefficientMean] IS OLD.[DrainCoefficientMean] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithValidForeshoreProfile(reader, sourceFilePath, calculationEntityName, validForeshoreProfileCriteria);

            const string invalidForeshoreProfileCriteria =
                "NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[StabilityPointStructureEntityId] IS OLD.[StabilityPointStructureEntityId] " +
                "AND NEW.\"Order\" IS OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelFailureConstructionMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelFailureConstructionStandardDeviation] " +
                "AND NEW.[ThresholdHeightOpenWeirMean] IS OLD.[ThresholdHeightOpenWeirMean] " +
                "AND NEW.[ThresholdHeightOpenWeirStandardDeviation] IS OLD.[ThresholdHeightOpenWeirStandardDeviation] " +
                "AND NEW.[CriticalOvertoppingDischargeMean] IS OLD.[CriticalOvertoppingDischargeMean] " +
                "AND NEW.[CriticalOvertoppingDischargeCoefficientOfVariation] IS OLD.[CriticalOvertoppingDischargeCoefficientOfVariation] " +
                "AND NEW.[FlowWidthAtBottomProtectionMean] IS OLD.[FlowWidthAtBottomProtectionMean] " +
                "AND NEW.[FlowWidthAtBottomProtectionStandardDeviation] IS OLD.[FlowWidthAtBottomProtectionStandardDeviation] " +
                "AND NEW.[ConstructiveStrengthLinearLoadModelMean] IS OLD.[ConstructiveStrengthLinearLoadModelMean] " +
                "AND NEW.[ConstructiveStrengthLinearLoadModelCoefficientOfVariation] IS OLD.[ConstructiveStrengthLinearLoadModelCoefficientOfVariation] " +
                "AND NEW.[ConstructiveStrengthQuadraticLoadModelMean] IS OLD.[ConstructiveStrengthQuadraticLoadModelMean] " +
                "AND NEW.[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation] IS OLD.[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation] " +
                "AND NEW.[BankWidthMean] IS OLD.[BankWidthMean] " +
                "AND NEW.[BankWidthStandardDeviation] IS OLD.[BankWidthStandardDeviation] " +
                "AND NEW.[InsideWaterLevelFailureConstructionMean] IS OLD.[InsideWaterLevelFailureConstructionMean] " +
                "AND NEW.[InsideWaterLevelFailureConstructionStandardDeviation] IS OLD.[InsideWaterLevelFailureConstructionStandardDeviation] " +
                "AND NEW.[EvaluationLevel] IS OLD.[EvaluationLevel] " +
                "AND NEW.[LevelCrestStructureMean] IS OLD.[LevelCrestStructureMean] " +
                "AND NEW.[LevelCrestStructureStandardDeviation] IS OLD.[LevelCrestStructureStandardDeviation] " +
                "AND NEW.[VerticalDistance] IS OLD.[VerticalDistance] " +
                "AND NEW.[FailureProbabilityRepairClosure] = OLD.[FailureProbabilityRepairClosure] " +
                "AND NEW.[FailureCollisionEnergyMean] IS OLD.[FailureCollisionEnergyMean] " +
                "AND NEW.[FailureCollisionEnergyCoefficientOfVariation] IS OLD.[FailureCollisionEnergyCoefficientOfVariation] " +
                "AND NEW.[ShipMassMean] IS OLD.[ShipMassMean] " +
                "AND NEW.[ShipMassCoefficientOfVariation] IS OLD.[ShipMassCoefficientOfVariation] " +
                "AND NEW.[ShipVelocityMean] IS OLD.[ShipVelocityMean] " +
                "AND NEW.[ShipVelocityCoefficientOfVariation] IS OLD.[ShipVelocityCoefficientOfVariation] " +
                "AND NEW.[LevellingCount] = OLD.[LevellingCount] " +
                "AND NEW.[ProbabilityCollisionSecondaryStructure] = OLD.[ProbabilityCollisionSecondaryStructure] " +
                "AND NEW.[FlowVelocityStructureClosableMean] IS OLD.[FlowVelocityStructureClosableMean] " +
                "AND NEW.[StabilityLinearLoadModelMean] IS OLD.[StabilityLinearLoadModelMean] " +
                "AND NEW.[StabilityLinearLoadModelCoefficientOfVariation] IS OLD.[StabilityLinearLoadModelCoefficientOfVariation] " +
                "AND NEW.[StabilityQuadraticLoadModelMean] IS OLD.[StabilityQuadraticLoadModelMean] " +
                "AND NEW.[StabilityQuadraticLoadModelCoefficientOfVariation] IS OLD.[StabilityQuadraticLoadModelCoefficientOfVariation] " +
                "AND NEW.[AreaFlowAperturesMean] IS OLD.[AreaFlowAperturesMean] " +
                "AND NEW.[AreaFlowAperturesStandardDeviation] IS OLD.[AreaFlowAperturesStandardDeviation] " +
                "AND NEW.[InflowModelType] = OLD.[InflowModelType] " +
                "AND NEW.[LoadSchematizationType] = OLD.[LoadSchematizationType] " +
                "AND NEW.[VolumicWeightWater] IS OLD.[VolumicWeightWater] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[DrainCoefficientMean] IS OLD.[DrainCoefficientMean] " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated]; ";

            AssertCalculationsWithInvalidForeshoreProfile(reader, sourceFilePath, calculationEntityName, invalidForeshoreProfileCriteria);
        }

        private static void AssertCalculationsWithValidForeshoreProfile(MigratedDatabaseReader reader,
                                                                        string sourceFilePath,
                                                                        string calculationEntityName,
                                                                        string criteria)
        {
            string validateCalculationsWithoutForeshoreProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM [SOURCEPROJECT].{calculationEntityName} " +
                "WHERE ForeshoreProfileEntityId IS NULL " +
                ")" +
                $"FROM {calculationEntityName} NEW " +
                $"JOIN [SOURCEPROJECT].{calculationEntityName} OLD USING({calculationEntityName}Id)" +
                "WHERE OLD.ForeshoreProfileEntityId IS NULL " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                $"AND {criteria}" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationsWithoutForeshoreProfiles);

            string validateCalculationsWithValidForeshoreProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM [SOURCEPROJECT].{calculationEntityName} " +
                "JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)" +
                "WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) > 1" +
                ")" +
                $"FROM {calculationEntityName} NEW " +
                $"JOIN [SOURCEPROJECT].{calculationEntityName} OLD USING({calculationEntityName}Id)" +
                "JOIN [SOURCEPROJECT].ForeshoreProfileEntity FP " +
                "WHERE OLD.ForeshoreProfileEntityId = FP.ForeshoreProfileEntityId " +
                "AND (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) > 1 " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                $"AND {criteria}" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationsWithValidForeshoreProfiles);
        }

        private static void AssertCalculationsWithInvalidForeshoreProfile(MigratedDatabaseReader reader,
                                                                          string sourceFilePath,
                                                                          string calculationEntityName,
                                                                          string criteria)
        {
            string validateCalculationsWithInvalidForeshoreProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM [SOURCEPROJECT].{calculationEntityName} " +
                "JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)" +
                "WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1" +
                ")" +
                $"FROM {calculationEntityName} NEW " +
                $"JOIN [SOURCEPROJECT].{calculationEntityName} OLD USING({calculationEntityName}Id)" +
                "JOIN [SOURCEPROJECT].ForeshoreProfileEntity FP " +
                "WHERE OLD.ForeshoreProfileEntityId = FP.ForeshoreProfileEntityId " +
                "AND (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / " +
                "(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1 " +
                "AND NEW.[UseBreakWater] = 0 " +
                "AND NEW.[BreakWaterType] = 3 " +
                "AND NEW.[BreakWaterHeight] IS NULL " +
                "AND NEW.[UseForeshore] = 0 " +
                $"AND {criteria}" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationsWithInvalidForeshoreProfiles);
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
                "GeneralResultFaultTreeIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointStochastEntity",
                "GeneralResultSubMechanismIllustrationPointEntity",
                "GeneralResultSubMechanismIllustrationPointStochastEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsDikeHeightOutputEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsOvertoppingRateOutputEntity",
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
                "PipingCalculationEntity",
                "PipingCalculationOutputEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
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

                Assert.AreEqual(18, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "Gevolgen van de migratie van versie 18.1 naar versie 19.1:"),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'BackgroundData-DefaultValue'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + De waarde voor de transparantie van de achtergrondkaart is aangepast naar 0.60."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'PipingSoilLayer'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '0.0049' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '4.9e-07' voor het gemiddelde van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '4.9e-07' voor het gemiddelde van parameter 'd70' van ondergrondlaag 'DiameterD70Mean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'WithHydraulicDatabase'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'StabilityStoneCoverCalculations'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'GrassCoverErosionOutwardsCalculations'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'InvalidDikeProfiles'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'InvalidForeshoreProfiles'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"19.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader)
        {
            const string validateBelowPhreaticLevel =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [BelowPhreaticLevelMean] < [BelowPhreaticLevelShift] " +
                "OR [BelowPhreaticLevelMean] < 0.005;";
            reader.AssertReturnedDataIsValid(validateBelowPhreaticLevel);

            const string validateDiameter70 =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [DiameterD70Mean] < 0.0000005;";
            reader.AssertReturnedDataIsValid(validateDiameter70);

            const string validatePermeability =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [PermeabilityMean] < 0.0000005;";
            reader.AssertReturnedDataIsValid(validatePermeability);
        }
    }
}