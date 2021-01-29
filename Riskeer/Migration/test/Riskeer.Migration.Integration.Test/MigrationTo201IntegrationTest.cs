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
    [TestFixture]
    public class MigrationTo201IntegrationTest
    {
        private const string newVersion = "20.1";

        [Test]
        public void Given191Project_WhenUpgradedTo201_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject191.risk");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given191Project_WhenUpgradedTo201_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given191Project_WhenUpgradedTo201_ThenProjectAsExpected), ".log"));
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
                    AssertGrassCoverErosionInwardsSectionResult(reader, sourceFilePath);

                    AssertClosingStructuresCalculation(reader, sourceFilePath);
                    AssertClosingStructuresSectionResult(reader, sourceFilePath);

                    AssertHeightStructuresCalculation(reader, sourceFilePath);
                    AssertHeightStructuresSectionResult(reader, sourceFilePath);

                    AssertStabilityPointStructuresCalculation(reader, sourceFilePath);
                    AssertStabilityPointStructuresSectionResult(reader, sourceFilePath);

                    AssertMacroStabilityInwardsCalculation(reader, sourceFilePath);
                    AssertMacroStabilityInwardsOutput(reader);

                    AssertPipingCalculation(reader, sourceFilePath);
                    AssertPipingOutput(reader, sourceFilePath);

                    AssertIllustrationPointResult(reader, sourceFilePath);
                    AssertSubMechanismIllustrationPointStochast(reader, sourceFilePath);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertMacroStabilityInwardsCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity " +
                ")" +
                "FROM MacroStabilityInwardsCalculationEntity NEW " +
                "JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity OLD USING(MacroStabilityInwardsCalculationEntityId) " +
                "WHERE NEW.[MacroStabilityInwardsCalculationEntityId] = OLD.[MacroStabilityInwardsCalculationEntityId] " +
                "AND NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[SurfaceLineEntityId] IS OLD.[SurfaceLineEntityId] " +
                "AND NEW.[MacroStabilityInwardsStochasticSoilProfileEntityId] IS OLD.[MacroStabilityInwardsStochasticSoilProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[Order] = OLD.[Order] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comment] IS OLD.[Comment] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[AssessmentLevel] IS OLD.[AssessmentLevel] " +
                "AND NEW.[UseAssessmentLevelManualInput] = OLD.[UseAssessmentLevelManualInput] " +
                "AND NEW.[SlipPlaneMinimumDepth] IS OLD.[SlipPlaneMinimumDepth] " +
                "AND NEW.[SlipPlaneMinimumLength] IS OLD.[SlipPlaneMinimumLength] " +
                "AND NEW.[MaximumSliceWidth] IS OLD.[MaximumSliceWidth] " +
                "AND NEW.[MoveGrid] = OLD.[MoveGrid] " +
                "AND NEW.[GridDeterminationType] = OLD.[GridDeterminationType] " +
                "AND NEW.[TangentLineDeterminationType] = OLD.[TangentLineDeterminationType] " +
                "AND NEW.[TangentLineZTop] IS OLD.[TangentLineZTop] " +
                "AND NEW.[TangentLineZBottom] IS OLD.[TangentLineZBottom] " +
                "AND NEW.[TangentLineNumber] = OLD.[TangentLineNumber] " +
                "AND NEW.[LeftGridXLeft] IS OLD.[LeftGridXLeft] " +
                "AND NEW.[LeftGridXRight] IS OLD.[LeftGridXRight] " +
                "AND NEW.[LeftGridNrOfHorizontalPoints] = OLD.[LeftGridNrOfHorizontalPoints] " +
                "AND NEW.[LeftGridZTop] IS OLD.[LeftGridZTop] " +
                "AND NEW.[LeftGridZBottom] IS OLD.[LeftGridZBottom] " +
                "AND NEW.[LeftGridNrOfVerticalPoints] = OLD.[LeftGridNrOfVerticalPoints] " +
                "AND NEW.[RightGridXLeft] IS OLD.[RightGridXLeft] " +
                "AND NEW.[RightGridXRight] IS OLD.[RightGridXRight] " +
                "AND NEW.[RightGridNrOfHorizontalPoints] = OLD.[RightGridNrOfHorizontalPoints] " +
                "AND NEW.[RightGridZTop] IS OLD.[RightGridZTop] " +
                "AND NEW.[RightGridZBottom] IS OLD.[RightGridZBottom] " +
                "AND NEW.[RightGridNrOfVerticalPoints] = OLD.[RightGridNrOfVerticalPoints] " +
                "AND NEW.[DikeSoilScenario] = OLD.[DikeSoilScenario] " +
                "AND NEW.[WaterLevelRiverAverage] IS OLD.[WaterLevelRiverAverage] " +
                "AND NEW.[DrainageConstructionPresent] = OLD.[DrainageConstructionPresent] " +
                "AND NEW.[DrainageConstructionCoordinateX] IS OLD.[DrainageConstructionCoordinateX] " +
                "AND NEW.[DrainageConstructionCoordinateZ] IS OLD.[DrainageConstructionCoordinateZ] " +
                "AND NEW.[MinimumLevelPhreaticLineAtDikeTopRiver] IS OLD.[MinimumLevelPhreaticLineAtDikeTopRiver] " +
                "AND NEW.[MinimumLevelPhreaticLineAtDikeTopPolder] IS OLD.[MinimumLevelPhreaticLineAtDikeTopPolder] " +
                "AND NEW.[AdjustPhreaticLine3And4ForUplift] = OLD.[AdjustPhreaticLine3And4ForUplift] " +
                "AND NEW.[LeakageLengthOutwardsPhreaticLine3] IS OLD.[LeakageLengthOutwardsPhreaticLine3] " +
                "AND NEW.[LeakageLengthInwardsPhreaticLine3] IS OLD.[LeakageLengthInwardsPhreaticLine3] " +
                "AND NEW.[LeakageLengthOutwardsPhreaticLine4] IS OLD.[LeakageLengthOutwardsPhreaticLine4] " +
                "AND NEW.[LeakageLengthInwardsPhreaticLine4] IS OLD.[LeakageLengthInwardsPhreaticLine4] " +
                "AND NEW.[PiezometricHeadPhreaticLine2Outwards] IS OLD.[PiezometricHeadPhreaticLine2Outwards] " +
                "AND NEW.[PiezometricHeadPhreaticLine2Inwards] IS OLD.[PiezometricHeadPhreaticLine2Inwards] " +
                "AND NEW.[LocationInputExtremeWaterLevelPolder] IS OLD.[LocationInputExtremeWaterLevelPolder] " +
                "AND NEW.[LocationInputExtremeUseDefaultOffsets] = OLD.[LocationInputExtremeUseDefaultOffsets] " +
                "AND NEW.[LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver] IS OLD.[LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver] " +
                "AND NEW.[LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder] IS OLD.[LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder] " +
                "AND NEW.[LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside] IS OLD.[LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside] " +
                "AND NEW.[LocationInputExtremePhreaticLineOffsetDikeToeAtPolder] IS OLD.[LocationInputExtremePhreaticLineOffsetDikeToeAtPolder] " +
                "AND NEW.[LocationInputExtremePenetrationLength] IS OLD.[LocationInputExtremePenetrationLength] " +
                "AND NEW.[LocationInputDailyWaterLevelPolder] IS OLD.[LocationInputDailyWaterLevelPolder] " +
                "AND NEW.[LocationInputDailyUseDefaultOffsets] = OLD.[LocationInputDailyUseDefaultOffsets] " +
                "AND NEW.[LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver] IS OLD.[LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver] " +
                "AND NEW.[LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder] IS OLD.[LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder] " +
                "AND NEW.[LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside] IS OLD.[LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside] " +
                "AND NEW.[LocationInputDailyPhreaticLineOffsetDikeToeAtPolder] IS OLD.[LocationInputDailyPhreaticLineOffsetDikeToeAtPolder] " +
                "AND NEW.[CreateZones] = OLD.[CreateZones] " +
                "AND NEW.[ZoningBoundariesDeterminationType] = OLD.[ZoningBoundariesDeterminationType] " +
                "AND NEW.[ZoneBoundaryLeft] IS OLD.[ZoneBoundaryLeft] " +
                "AND NEW.[ZoneBoundaryRight] IS OLD.[ZoneBoundaryRight]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculation);
            
            string validateScenarioContribution = 
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM " +
                "(" +
                "SELECT " +
                "CASE WHEN OLD.[ScenarioContribution] IS NULL OR OLD.[ScenarioContribution] < 0 OR OLD.[ScenarioContribution] > 1 " +
                "THEN 0 " +
                "ELSE OLD.[ScenarioContribution]" +
                "END AS [IsInvalid] " +
                "FROM MacroStabilityInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsCalculationEntity OLD " +
                "ON NEW.[MacroStabilityInwardsCalculationEntityId] = OLD.[MacroStabilityInwardsCalculationEntityId] " +
                "); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateScenarioContribution);
        }

        private static void AssertGrassCoverErosionInwardsCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculationLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsSectionResultEntity USING(GrassCoverErosionInwardsCalculationEntityId) " +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeightCalculationType] = OLD.[DikeHeightCalculationType] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[OvertoppingRateCalculationType] = OLD.[OvertoppingRateCalculationType] " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 1 " +
                "AND NEW.[ScenarioContribution] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationLinkedToSectionResult);

            string validateCalculationNotLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "LEFT JOIN SOURCEPROJECT.GrassCoverErosionInwardsSectionResultEntity " +
                "USING(GrassCoverErosionInwardsCalculationEntityId) " +
                "WHERE GrassCoverErosionInwardsSectionResultEntityId IS NULL" +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeightCalculationType] = OLD.[DikeHeightCalculationType] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[OvertoppingRateCalculationType] = OLD.[OvertoppingRateCalculationType] " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 0 " +
                "AND NEW.[ScenarioContribution] = 0; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationNotLinkedToSectionResult);
        }

        private static void AssertGrassCoverErosionInwardsSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsSectionResultEntity" +
                ") " +
                "FROM GrassCoverErosionInwardsSectionResultEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsSectionResultEntity OLD USING(GrassCoverErosionInwardsSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[SimpleAssessmentResult] = OLD.[SimpleAssessmentResult] " +
                "AND NEW.[DetailedAssessmentResult] = OLD.[DetailedAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentResult] = OLD.[TailorMadeAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentProbability] IS OLD.[TailorMadeAssessmentProbability] " +
                "AND NEW.[UseManualAssembly] = OLD.[UseManualAssembly] " +
                "AND NEW.[ManualAssemblyProbability] IS OLD.[ManualAssemblyProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
        }

        private static void AssertClosingStructuresCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculationLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.ClosingStructuresCalculationEntity " +
                "JOIN SOURCEPROJECT.ClosingStructuresSectionResultEntity USING(ClosingStructuresCalculationEntityId) " +
                ") " +
                "FROM ClosingStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.ClosingStructuresCalculationEntity OLD USING(ClosingStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[ClosingStructureEntityId] IS OLD.[ClosingStructureEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
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
                "AND NEW.[DrainCoefficientStandardDeviation] IS 0.2 " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 1 " +
                "AND NEW.[ScenarioContribution] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationLinkedToSectionResult);

            string validateCalculationNotLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.ClosingStructuresCalculationEntity " +
                "LEFT JOIN SOURCEPROJECT.ClosingStructuresSectionResultEntity " +
                "USING(ClosingStructuresCalculationEntityId) " +
                "WHERE ClosingStructuresSectionResultEntityId IS NULL" +
                ") " +
                "FROM ClosingStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.ClosingStructuresCalculationEntity OLD USING(ClosingStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[ClosingStructureEntityId] IS OLD.[ClosingStructureEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
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
                "AND NEW.[DrainCoefficientStandardDeviation] IS 0.2 " +
                "AND NEW.[ModelFactorSuperCriticalFlowMean] IS OLD.[ModelFactorSuperCriticalFlowMean] " +
                "AND NEW.[StormDurationMean] IS OLD.[StormDurationMean] " +
                "AND NEW.[FactorStormDurationOpenStructure] IS OLD.[FactorStormDurationOpenStructure] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 0 " +
                "AND NEW.[ScenarioContribution] = 0; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationNotLinkedToSectionResult);
        }

        private static void AssertClosingStructuresSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.ClosingStructuresSectionResultEntity" +
                ") " +
                "FROM ClosingStructuresSectionResultEntity NEW " +
                "JOIN SOURCEPROJECT.ClosingStructuresSectionResultEntity OLD USING(ClosingStructuresSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[SimpleAssessmentResult] = OLD.[SimpleAssessmentResult] " +
                "AND NEW.[DetailedAssessmentResult] = OLD.[DetailedAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentResult] = OLD.[TailorMadeAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentProbability] IS OLD.[TailorMadeAssessmentProbability] " +
                "AND NEW.[UseManualAssembly] = OLD.[UseManualAssembly] " +
                "AND NEW.[ManualAssemblyProbability] IS OLD.[ManualAssemblyProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
        }

        private static void AssertHeightStructuresCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculationLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HeightStructuresCalculationEntity " +
                "JOIN SOURCEPROJECT.HeightStructuresSectionResultEntity USING(HeightStructuresCalculationEntityId) " +
                ") " +
                "FROM HeightStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.HeightStructuresCalculationEntity OLD USING(HeightStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
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
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 1 " +
                "AND NEW.[ScenarioContribution] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationLinkedToSectionResult);

            string validateCalculationNotLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HeightStructuresCalculationEntity " +
                "LEFT JOIN SOURCEPROJECT.HeightStructuresSectionResultEntity " +
                "USING(HeightStructuresCalculationEntityId) " +
                "WHERE HeightStructuresSectionResultEntityId IS NULL" +
                ") " +
                "FROM HeightStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.HeightStructuresCalculationEntity OLD USING(HeightStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
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
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 0 " +
                "AND NEW.[ScenarioContribution] = 0; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationNotLinkedToSectionResult);
        }

        private static void AssertHeightStructuresSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HeightStructuresSectionResultEntity" +
                ") " +
                "FROM HeightStructuresSectionResultEntity NEW " +
                "JOIN SOURCEPROJECT.HeightStructuresSectionResultEntity OLD USING(HeightStructuresSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[SimpleAssessmentResult] = OLD.[SimpleAssessmentResult] " +
                "AND NEW.[DetailedAssessmentResult] = OLD.[DetailedAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentResult] = OLD.[TailorMadeAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentProbability] IS OLD.[TailorMadeAssessmentProbability] " +
                "AND NEW.[UseManualAssembly] = OLD.[UseManualAssembly] " +
                "AND NEW.[ManualAssemblyProbability] IS OLD.[ManualAssemblyProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
        }

        private static void AssertStabilityPointStructuresCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculationLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityPointStructuresCalculationEntity " +
                "JOIN SOURCEPROJECT.StabilityPointStructuresSectionResultEntity USING(StabilityPointStructuresCalculationEntityId) " +
                ") " +
                "FROM StabilityPointStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityPointStructuresCalculationEntity OLD USING(StabilityPointStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[StabilityPointStructureEntityId] IS OLD.[StabilityPointStructureEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelStandardDeviation] " +
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
                "AND NEW.[DrainCoefficientStandardDeviation] IS 0.2 " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 1 " +
                "AND NEW.[ScenarioContribution] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationLinkedToSectionResult);

            string validateCalculationNotLinkedToSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityPointStructuresCalculationEntity " +
                "LEFT JOIN SOURCEPROJECT.StabilityPointStructuresSectionResultEntity " +
                "USING(StabilityPointStructuresCalculationEntityId) " +
                "WHERE StabilityPointStructuresSectionResultEntityId IS NULL" +
                ") " +
                "FROM StabilityPointStructuresCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityPointStructuresCalculationEntity OLD USING(StabilityPointStructuresCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[StabilityPointStructureEntityId] IS OLD.[StabilityPointStructureEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[StructureNormalOrientation] IS OLD.[StructureNormalOrientation] " +
                "AND NEW.[StorageStructureAreaMean] IS OLD.[StorageStructureAreaMean] " +
                "AND NEW.[StorageStructureAreaCoefficientOfVariation] IS OLD.[StorageStructureAreaCoefficientOfVariation] " +
                "AND NEW.[AllowedLevelIncreaseStorageMean] IS OLD.[AllowedLevelIncreaseStorageMean] " +
                "AND NEW.[AllowedLevelIncreaseStorageStandardDeviation] IS OLD.[AllowedLevelIncreaseStorageStandardDeviation] " +
                "AND NEW.[WidthFlowAperturesMean] IS OLD.[WidthFlowAperturesMean] " +
                "AND NEW.[WidthFlowAperturesStandardDeviation] IS OLD.[WidthFlowAperturesStandardDeviation] " +
                "AND NEW.[InsideWaterLevelMean] IS OLD.[InsideWaterLevelMean] " +
                "AND NEW.[InsideWaterLevelStandardDeviation] IS OLD.[InsideWaterLevelStandardDeviation] " +
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
                "AND NEW.[DrainCoefficientStandardDeviation] IS 0.2 " +
                "AND NEW.[FailureProbabilityStructureWithErosion] IS OLD.[FailureProbabilityStructureWithErosion] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = 0 " +
                "AND NEW.[ScenarioContribution] = 0; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationNotLinkedToSectionResult);
        }

        private static void AssertStabilityPointStructuresSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityPointStructuresSectionResultEntity" +
                ") " +
                "FROM StabilityPointStructuresSectionResultEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityPointStructuresSectionResultEntity OLD USING(StabilityPointStructuresSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[SimpleAssessmentResult] = OLD.[SimpleAssessmentResult] " +
                "AND NEW.[DetailedAssessmentResult] = OLD.[DetailedAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentResult] = OLD.[TailorMadeAssessmentResult] " +
                "AND NEW.[TailorMadeAssessmentProbability] IS OLD.[TailorMadeAssessmentProbability] " +
                "AND NEW.[UseManualAssembly] = OLD.[UseManualAssembly] " +
                "AND NEW.[ManualAssemblyProbability] IS OLD.[ManualAssemblyProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
        }

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader)
        {
            const string macroStabilityInwardsCalculationOutputEntityTable =
                "SELECT COUNT() = 0 " +
                "FROM MacroStabilityInwardsCalculationOutputEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(macroStabilityInwardsCalculationOutputEntityTable);
        }

        private static void AssertPipingCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.PipingCalculationEntity" +
                ") " +
                "FROM SemiProbabilisticPipingCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.PipingCalculationEntity OLD " +
                "ON NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[PipingCalculationEntityId]" +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[SurfaceLineEntityId] IS OLD.[SurfaceLineEntityId] " +
                "AND NEW.[PipingStochasticSoilProfileEntityId] IS OLD.[PipingStochasticSoilProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[EntryPointL] IS OLD.[EntryPointL] " +
                "AND NEW.[ExitPointL] IS OLD.[ExitPointL] " +
                "AND NEW.[PhreaticLevelExitMean] IS OLD.[PhreaticLevelExitMean] " +
                "AND NEW.[PhreaticLevelExitStandardDeviation] IS OLD.[PhreaticLevelExitStandardDeviation] " +
                "AND NEW.[DampingFactorExitMean] IS OLD.[DampingFactorExitMean] " +
                "AND NEW.[DampingFactorExitStandardDeviation] IS OLD.[DampingFactorExitStandardDeviation] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[AssessmentLevel] IS OLD.[AssessmentLevel] " +
                "AND NEW.[UseAssessmentLevelManualInput] = OLD.[UseAssessmentLevelManualInput]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculation);
            
            string validateScenarioContribution = 
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM " +
                "(" +
                "SELECT " +
                "CASE WHEN OLD.[ScenarioContribution] IS NULL OR OLD.[ScenarioContribution] < 0 OR OLD.[ScenarioContribution] > 1 " +
                "THEN 0 " +
                "ELSE OLD.[ScenarioContribution]" +
                "END AS [IsInvalid] " +
                "FROM SemiProbabilisticPipingCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.PipingCalculationEntity OLD " +
                "ON NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[PipingCalculationEntityId] " +
                "); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateScenarioContribution);
        }

        private static void AssertPipingOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.PipingCalculationOutputEntity" +
                ") " +
                "FROM SemiProbabilisticPipingCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.PipingCalculationOutputEntity OLD " +
                "ON NEW.[SemiProbabilisticPipingCalculationOutputEntityId] = OLD.[PipingCalculationOutputEntityId]" +
                "WHERE NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[PipingCalculationEntityId] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[HeaveFactorOfSafety] IS OLD.[HeaveFactorOfSafety] " +
                "AND NEW.[UpliftFactorOfSafety] IS OLD.[UpliftFactorOfSafety] " +
                "AND NEW.[SellmeijerFactorOfSafety] IS OLD.[SellmeijerFactorOfSafety] " +
                "AND NEW.[UpliftEffectiveStress] IS OLD.[UpliftEffectiveStress] " +
                "AND NEW.[HeaveGradient] IS OLD.[HeaveGradient] " +
                "AND NEW.[SellmeijerCreepCoefficient] IS OLD.[SellmeijerCreepCoefficient] " +
                "AND NEW.[SellmeijerCriticalFall] IS OLD.[SellmeijerCriticalFall] " +
                "AND NEW.[SellmeijerReducedFall] IS OLD.[SellmeijerReducedFall]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateOutput);
        }
        
        private static void AssertIllustrationPointResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.IllustrationPointResultEntity" +
                ") " +
                "FROM IllustrationPointResultEntity NEW " +
                "JOIN SOURCEPROJECT.IllustrationPointResultEntity OLD USING(IllustrationPointResultEntityId) " +
                "WHERE NEW.[IllustrationPointResultEntityId] = OLD.[IllustrationPointResultEntityId] " +
                "AND NEW.[SubMechanismIllustrationPointEntityId] = OLD.[SubMechanismIllustrationPointEntityId] " +
                "AND NEW.[Description] = OLD.[Description] " +
                "AND NEW.[Unit] = \"-\" " +
                "AND NEW.[Value] IS OLD.[Value] " +
                "AND NEW.[Order] = OLD.[Order]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
        }
        
        private static void AssertSubMechanismIllustrationPointStochast(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.SubMechanismIllustrationPointStochastEntity" +
                ") " +
                "FROM SubMechanismIllustrationPointStochastEntity NEW " +
                "JOIN SOURCEPROJECT.SubMechanismIllustrationPointStochastEntity OLD USING(SubMechanismIllustrationPointStochastEntityId) " +
                "WHERE NEW.[SubMechanismIllustrationPointStochastEntityId] = OLD.[SubMechanismIllustrationPointStochastEntityId] " +
                "AND NEW.[SubMechanismIllustrationPointEntityId] = OLD.[SubMechanismIllustrationPointEntityId] " +
                "AND NEW.[Name] = OLD.[Name] " +
                "AND NEW.[Unit] = \"-\" " +
                "AND NEW.[Duration] IS OLD.[Duration] " +
                "AND NEW.[Alpha] IS OLD.[Alpha] " +
                "AND NEW.[Realization] IS OLD.[Realization] " +
                "AND NEW.[Order] = OLD.[Order]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResults);
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
                "ClosingStructuresFailureMechanismMetaEntity",
                "ClosingStructuresOutputEntity",
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
                "GrassCoverErosionInwardsDikeHeightOutputEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsOvertoppingRateOutputEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresOutputEntity",
                "HydraulicBoundaryDatabaseEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationEntity",
                "HydraulicLocationOutputEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
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
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresOutputEntity",
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

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"20.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(5, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("19.1", newVersion, "Gevolgen van de migratie van versie 19.1 naar versie 20.1:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("19.1", newVersion, "* Alle berekende resultaten van het toetsspoor 'Macrostabiliteit binnenwaarts' zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("19.1", newVersion, "* Alle berekende resultaten van het toetsspoor 'Piping' waarbij de waterstand handmatig is ingevuld zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("19.1", newVersion, "* Alle scenario bijdragen van het toetsspoor 'Macrostabiliteit Binnenwaarts' waarbij de bijdrage groter is dan 100% of kleiner dan 0% zijn aangepast naar respectievelijk 100% en 0%."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("19.1", newVersion, "* Alle scenario bijdragen van van het toetsspoor 'Piping' waarbij de bijdrage groter is dan 100% of kleiner dan 0% zijn aangepast naar respectievelijk 100% en 0%."),
                    messages[i]);
            }
        }
    }
}