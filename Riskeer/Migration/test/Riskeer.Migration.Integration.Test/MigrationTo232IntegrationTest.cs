// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo232IntegrationTest
    {
        private const string newVersion = "23.2";

        [Test]
        [TestCaseSource(nameof(GetMigrationProjectsWithMessages))]
        public void Given231Project_WhenUpgradedTo232_ThenProjectAsExpected(string fileName, IEnumerable<string> expectedMessages)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               fileName);
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given231Project_WhenUpgradedTo232_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given231Project_WhenUpgradedTo232_ThenProjectAsExpected), ".log"));
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

                    AssertAssessmentSection(reader, sourceFilePath);

                    AssertGrassCoverErosionOutwardsWaveConditionsCalculation(reader, sourceFilePath);
                    AssertStabilityStoneCoverWaveConditionsCalculation(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverWaveConditionsCalculation(reader, sourceFilePath);

                    AssertClosingStructuresFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertDuneErosionFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertHeightStructuresFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertStabilityStoneCoverFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertStabilityPointStructuresFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertSpecificFailureMechanismMetaEntity(reader, sourceFilePath);
                    
                    AssertVersions(reader);
                    AssertDatabase(reader);

                    AssertHydraulicLocationOutput(reader);
                    AssertDuneLocationOutput(reader);

                    AssertGrassCoverErosionInwardsOutput(reader);

                    AssertGrassCoverErosionOutwardsOutput(reader);
                    AssertStabilityStoneCoverOutput(reader);
                    AssertWaveImpactAsphaltCoverOutput(reader);

                    AssertClosingStructuresOutput(reader);
                    AssertHeightStructuresOutput(reader);
                    AssertStabilityPointStructuresOutput(reader);
                    AssertMacroStabilityInwardsOutput(reader, sourceFilePath);
                    AssertPipingOutput(reader, sourceFilePath);

                    AssertIllustrationPointResults(reader);
                }

                AssertLogDatabase(logFilePath, expectedMessages);
            }
        }

        private static IEnumerable<TestCaseData> GetMigrationProjectsWithMessages()
        {
            yield return new TestCaseData("MigrationTestProject231NoOutput.risk", new[]
            {
                "* Geen aanpassingen."
            });

            yield return new TestCaseData("MigrationTestProject231MacroStabilityInwardsNoManualAssessmentLevels.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            yield return new TestCaseData("MigrationTestProject231PipingNoManualAssessmentLevels.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            // This file contains all configured failure mechanisms (except Dunes and MacroStabilityInwards) with output.
            // The mechanisms Dunes and MacroStabilityInwards have different assessment sections, and are therefore put in different test files.
            yield return new TestCaseData("MigrationTestProject231WithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en/of 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
            });

            yield return new TestCaseData("MigrationTestProject231DunesWithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            yield return new TestCaseData("MigrationTestProject231MacroStabilityInwardsWithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en/of 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
            });

            yield return new TestCaseData("MigrationTestProject231RevetmentCalculations.risk", new[]
            {
                "* Geen aanpassingen."
            });
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSection =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AssessmentSectionEntity " +
                ") " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[ProjectEntityId] = OLD.[ProjectEntityId] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity1Id] = OLD.[HydraulicLocationCalculationCollectionEntity1Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity2Id] = OLD.[HydraulicLocationCalculationCollectionEntity2Id] " +
                "AND NEW.[Id] IS OLD.[Id] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[MaximumAllowableFloodingProbability] = OLD.[MaximumAllowableFloodingProbability] " +
                "AND NEW.[SignalFloodingProbability] = OLD.[SignalFloodingProbability] " +
                "AND NEW.[NormativeProbabilityType] = OLD.[NormativeProbabilityType] " +
                "AND NEW.[Composition] = OLD.[Composition] " +
                "AND NEW.[ReferenceLinePointXml] = OLD.[ReferenceLinePointXml]" +
                "AND NEW.[AreFailureMechanismsCorrelated] = 0; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateAssessmentSection);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AdoptableFailureMechanismSectionResultEntity",
                "AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity",
                "AssessmentSectionEntity",
                "BackgroundDataEntity",
                "BackgroundDataMetaEntity",
                "CalculationGroupEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresFailureMechanismMetaEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationForTargetProbabilityCollectionEntity",
                "DuneLocationEntity",
                "FailureMechanismEntity",
                "FailureMechanismFailureMechanismSectionEntity",
                "FailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HydraulicBoundaryDataEntity",
                "HydraulicBoundaryDatabaseEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "NonAdoptableFailureMechanismSectionResultEntity",
                "NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingScenarioConfigurationPerFailureMechanismSectionEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "ProbabilisticPipingCalculationEntity",
                "ProjectEntity",
                "SemiProbabilisticPipingCalculationEntity",
                "SpecificFailureMechanismEntity",
                "SpecificFailureMechanismFailureMechanismSectionEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochastEntity",
                "StochasticSoilModelEntity",
                "SurfaceLineEntity",
                "VersionEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
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

        private static void AssertLogDatabase(string logFilePath, IEnumerable<string> expectedMessages)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(expectedMessages.Count() + 1, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("23.1", newVersion, $"Gevolgen van de migratie van versie 23.1 naar versie {newVersion}:"),
                    messages[i++]);

                foreach (string expectedMessage in expectedMessages)
                {
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("23.1", newVersion, $"{expectedMessage}"),
                        messages[i++]);
                }
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"23.2\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        # region Revetment

        private static void AssertGrassCoverErosionOutwardsWaveConditionsCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                ") " +
                "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS OLD.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] " +
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
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = OLD.[WaterLevelType];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculation);

            AssertWaveConditionsStepSize(reader, sourceFilePath, "GrassCoverErosionOutwardsWaveConditionsCalculationEntity");
        }

        private static void AssertStabilityStoneCoverWaveConditionsCalculation(MigratedDatabaseReader reader,
                                                                               string sourceFilePath)
        {
            string validateCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity " +
                ") " +
                "FROM StabilityStoneCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity OLD USING(StabilityStoneCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS OLD.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] " +
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
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = OLD.[WaterLevelType];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculation);

            AssertWaveConditionsStepSize(reader, sourceFilePath, "StabilityStoneCoverWaveConditionsCalculationEntity");
        }

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculation(MigratedDatabaseReader reader,
                                                                                  string sourceFilePath)
        {
            string validateCalculation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity " +
                ") " +
                "FROM WaveImpactAsphaltCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity OLD USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS OLD.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] " +
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
                "AND NEW.[WaterLevelType] = OLD.[WaterLevelType];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculation);

            AssertWaveConditionsStepSize(reader, sourceFilePath, "WaveImpactAsphaltCoverWaveConditionsCalculationEntity");
        }

        private static void AssertWaveConditionsStepSize(MigratedDatabaseReader reader,
                                                         string sourceFilePath,
                                                         string calculationEntityName)
        {
            string validateStepSize =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCE; " +
                "SELECT " +
                "COALESCE (SUM([IsInvalid]), 0) = 0 " +
                "FROM ( " +
                "SELECT " +
                "CASE " +
                "WHEN OLD.[StepSize] = 1 AND NEW.[StepSize] = 0.5 THEN 0 " +
                "WHEN OLD.[StepSize] = 2 AND NEW.[StepSize] = 1.0 THEN 0 " +
                "WHEN OLD.[StepSize] = 3 AND NEW.[StepSize] = 2.0 THEN 0 " +
                "ELSE 1 " +
                "END AS [IsInvalid] " +
                $"FROM {calculationEntityName} NEW " +
                $"JOIN SOURCE.{calculationEntityName} OLD USING({calculationEntityName}Id)" +
                "); " +
                "DETACH DATABASE SOURCE;";
            reader.AssertReturnedDataIsValid(validateStepSize);
        }

        #endregion

        # region Calculation outputs

        private static void AssertHydraulicLocationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [HydraulicLocationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertDuneLocationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationCalculationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertGrassCoverErosionInwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);

            const string validateDikeHeightOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateDikeHeightOutput);

            const string validateOvertoppingRateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOvertoppingRateOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOvertoppingRateOutput);
        }

        private static void AssertGrassCoverErosionOutwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionOutwardsWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertStabilityStoneCoverOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertWaveImpactAsphaltCoverOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertClosingStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [ClosingStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertHeightStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [HeightStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertStabilityPointStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityPointStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMacroStabilityInwardsOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.MacroStabilityInwardsCalculationOutputEntity " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)" +
                "WHERE UseAssessmentLevelManualInput = 1" +
                ")" +
                "FROM MacroStabilityInwardsCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsCalculationOutputEntity OLD " +
                "USING (MacroStabilityInwardsCalculationOutputEntityId)" +
                "WHERE NEW.[MacroStabilityInwardsCalculationEntityId] = OLD.[MacroStabilityInwardsCalculationEntityId] " +
                "AND NEW.[FactorOfStability] IS OLD.[FactorOfStability] " +
                "AND NEW.[ForbiddenZonesXEntryMin] IS OLD.[ForbiddenZonesXEntryMin] " +
                "AND NEW.[ForbiddenZonesXEntryMax] IS OLD.[ForbiddenZonesXEntryMax] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleCenterX] IS OLD.[SlidingCurveLeftSlidingCircleCenterX] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleCenterY] IS OLD.[SlidingCurveLeftSlidingCircleCenterY] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleRadius] IS OLD.[SlidingCurveLeftSlidingCircleRadius] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleIsActive] = OLD.[SlidingCurveLeftSlidingCircleIsActive] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleNonIteratedForce] IS OLD.[SlidingCurveLeftSlidingCircleNonIteratedForce] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleIteratedForce] IS OLD.[SlidingCurveLeftSlidingCircleIteratedForce] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleDrivingMoment] IS OLD.[SlidingCurveLeftSlidingCircleDrivingMoment] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleResistingMoment] IS OLD.[SlidingCurveLeftSlidingCircleResistingMoment] " +
                "AND NEW.[SlidingCurveRightSlidingCircleCenterX] IS OLD.[SlidingCurveRightSlidingCircleCenterX] " +
                "AND NEW.[SlidingCurveRightSlidingCircleCenterY] IS OLD.[SlidingCurveRightSlidingCircleCenterY] " +
                "AND NEW.[SlidingCurveRightSlidingCircleRadius] IS OLD.[SlidingCurveRightSlidingCircleRadius] " +
                "AND NEW.[SlidingCurveRightSlidingCircleIsActive] = OLD.[SlidingCurveRightSlidingCircleIsActive] " +
                "AND NEW.[SlidingCurveRightSlidingCircleNonIteratedForce] IS OLD.[SlidingCurveRightSlidingCircleNonIteratedForce] " +
                "AND NEW.[SlidingCurveRightSlidingCircleIteratedForce] IS OLD.[SlidingCurveRightSlidingCircleIteratedForce] " +
                "AND NEW.[SlidingCurveRightSlidingCircleDrivingMoment] IS OLD.[SlidingCurveRightSlidingCircleDrivingMoment] " +
                "AND NEW.[SlidingCurveRightSlidingCircleResistingMoment] IS OLD.[SlidingCurveRightSlidingCircleResistingMoment] " +
                "AND NEW.[SlidingCurveNonIteratedHorizontalForce] IS OLD.[SlidingCurveNonIteratedHorizontalForce] " +
                "AND NEW.[SlidingCurveIteratedHorizontalForce] IS OLD.[SlidingCurveIteratedHorizontalForce] " +
                "AND NEW.[SlidingCurveSliceXML] = OLD.[SlidingCurveSliceXML] " +
                "AND NEW.[SlipPlaneLeftGridXLeft] IS OLD.[SlipPlaneLeftGridXLeft] " +
                "AND NEW.[SlipPlaneLeftGridXRight] IS OLD.[SlipPlaneLeftGridXRight] " +
                "AND NEW.[SlipPlaneLeftGridNrOfHorizontalPoints] = OLD.[SlipPlaneLeftGridNrOfHorizontalPoints] " +
                "AND NEW.[SlipPlaneLeftGridZTop] IS OLD.[SlipPlaneLeftGridZTop] " +
                "AND NEW.[SlipPlaneLeftGridZBottom] IS OLD.[SlipPlaneLeftGridZBottom] " +
                "AND NEW.[SlipPlaneLeftGridNrOfVerticalPoints] = OLD.[SlipPlaneLeftGridNrOfVerticalPoints] " +
                "AND NEW.[SlipPlaneRightGridXLeft] IS OLD.[SlipPlaneRightGridXLeft] " +
                "AND NEW.[SlipPlaneRightGridXRight] IS OLD.[SlipPlaneRightGridXRight] " +
                "AND NEW.[SlipPlaneRightGridNrOfHorizontalPoints] = OLD.[SlipPlaneRightGridNrOfHorizontalPoints] " +
                "AND NEW.[SlipPlaneRightGridZTop] IS OLD.[SlipPlaneRightGridZTop] " +
                "AND NEW.[SlipPlaneRightGridZBottom] IS OLD.[SlipPlaneRightGridZBottom] " +
                "AND NEW.[SlipPlaneRightGridNrOfVerticalPoints] = OLD.[SlipPlaneRightGridNrOfVerticalPoints] " +
                "AND NEW.[SlipPlaneTangentLinesXml] = OLD.[SlipPlaneTangentLinesXml]; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMacroStabilityInwardsOutput);
        }

        private static void AssertPipingOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string validateProbabilisticCalculationOutput =
                "SELECT COUNT() = 0 " +
                "FROM ProbabilisticPipingCalculationOutputEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateProbabilisticCalculationOutput);

            string validateSemiProbabilisticOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity " +
                "JOIN SOURCEPROJECT.SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId) " +
                "WHERE UseAssessmentLevelManualInput = 1" +
                ") " +
                "FROM SemiProbabilisticPipingCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity OLD " +
                "USING (SemiProbabilisticPipingCalculationOutputEntityId)" +
                "WHERE NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[SemiProbabilisticPipingCalculationEntityId]" +
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
            reader.AssertReturnedDataIsValid(validateSemiProbabilisticOutput);
        }

        private static void AssertIllustrationPointResults(MigratedDatabaseReader reader)
        {
            const string validateFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeIllustrationPoint);

            const string validateFaultTreeIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeIllustrationPointStochast);

            const string validateFaultTreeSubmechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeSubmechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeSubmechanismIllustrationPoint);

            const string validateGeneralResultFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultFaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultFaultTreeIllustrationPoint);

            const string validateGeneralResultFaultTreeIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultFaultTreeIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultFaultTreeIllustrationPointStochast);

            const string validateGeneralResultSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultSubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultSubMechanismIllustrationPoint);

            const string validateGeneralResultSubMechanismIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultSubMechanismIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultSubMechanismIllustrationPointStochast);

            const string validateIllustrationPointResult =
                "SELECT COUNT() = 0 " +
                "FROM [IllustrationPointResultEntity]; ";
            reader.AssertReturnedDataIsValid(validateIllustrationPointResult);

            const string validateSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [SubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateSubMechanismIllustrationPoint);

            const string validateSubMechanismIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [SubMechanismIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateSubMechanismIllustrationPointStochast);

            const string validateTopLevelFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [TopLevelFaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateTopLevelFaultTreeIllustrationPoint);

            const string validateTopLevelSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [TopLevelSubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateTopLevelSubMechanismIllustrationPoint);
        }

        #endregion

        #region Failure mechanisms meta data

        private static void AssertClosingStructuresFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.ClosingStructuresFailureMechanismMetaEntity " +
                ") " +
                "FROM ClosingStructuresFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.ClosingStructuresFailureMechanismMetaEntity OLD USING(ClosingStructuresFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ClosingStructureCollectionSourcePath] IS OLD.[ClosingStructureCollectionSourcePath] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertDuneErosionFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.DuneErosionFailureMechanismMetaEntity " +
                ") " +
                "FROM DuneErosionFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.DuneErosionFailureMechanismMetaEntity OLD USING(DuneErosionFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsFailureMechanismMetaEntity " +
                ") " +
                "FROM GrassCoverErosionInwardsFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionInwardsFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[DikeProfileCollectionSourcePath] IS OLD.[DikeProfileCollectionSourcePath]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionOutwardsFailureMechanismMetaEntity " +
                ") " +
                "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionOutwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertHeightStructuresFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HeightStructuresFailureMechanismMetaEntity " +
                ") " +
                "FROM HeightStructuresFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.HeightStructuresFailureMechanismMetaEntity OLD USING(HeightStructuresFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[HeightStructureCollectionSourcePath] IS OLD.[HeightStructureCollectionSourcePath] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertStabilityPointStructuresFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityPointStructuresFailureMechanismMetaEntity " +
                ") " +
                "FROM StabilityPointStructuresFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityPointStructuresFailureMechanismMetaEntity OLD USING(StabilityPointStructuresFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[StabilityPointStructureCollectionSourcePath] IS OLD.[StabilityPointStructureCollectionSourcePath] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertStabilityStoneCoverFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityStoneCoverFailureMechanismMetaEntity " +
                ") " +
                "FROM StabilityStoneCoverFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityStoneCoverFailureMechanismMetaEntity OLD USING(StabilityStoneCoverFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.WaveImpactAsphaltCoverFailureMechanismMetaEntity " +
                ") " +
                "FROM WaveImpactAsphaltCoverFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.WaveImpactAsphaltCoverFailureMechanismMetaEntity OLD USING(WaveImpactAsphaltCoverFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }
        
        private static void AssertSpecificFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.SpecificFailureMechanismEntity " +
                ") " +
                "FROM SpecificFailureMechanismEntity NEW " +
                "JOIN SOURCEPROJECT.SpecificFailureMechanismEntity OLD USING(SpecificFailureMechanismEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Code] IS OLD.[Code] " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[InAssembly] = OLD.[InAssembly] " +
                "AND NEW.[FailureMechanismSectionCollectionSourcePath] IS OLD.[FailureMechanismSectionCollectionSourcePath] " +
                "AND NEW.[InAssemblyInputComments] IS OLD.[InAssemblyInputComments] " +
                "AND NEW.[InAssemblyOutputComments] IS OLD.[InAssemblyOutputComments] " +
                "AND NEW.[NotInAssemblyComments] IS OLD.[NotInAssemblyComments] " +
                "AND NEW.[FailureMechanismAssemblyResultProbabilityResultType] = OLD.[FailureMechanismAssemblyResultProbabilityResultType] " +
                "AND NEW.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability] IS OLD.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }


        #endregion
    }
}