// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo241IntegrationTest
    {
        private const string newVersion = "24.1";

        [Test]
        [TestCaseSource(nameof(GetMigrationProjectsWithMessages))]
        public void Given231Project_WhenUpgradedTo241_ThenProjectAsExpected(string fileName, IEnumerable<string> expectedMessages)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               fileName);
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given231Project_WhenUpgradedTo241_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given231Project_WhenUpgradedTo241_ThenProjectAsExpected), ".log"));
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

                    AssertAdoptableFailureMechanismSectionResult(reader, sourceFilePath);
                    AssertNonAdoptableFailureMechanismSectionResult(reader, sourceFilePath);

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

                    AssertMacroStabilityInwardsFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertMacroStabilityInwardsFailureMechanismSectionConfiguration(reader, sourceFilePath);
                    AssertMacroStabilityInwardsOutput(reader);

                    AssertPipingFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertPipingFailureMechanismSectionConfiguration(reader, sourceFilePath);
                    AssertPipingOutput(reader);

                    AssertIllustrationPointResults(reader);
                }

                AssertLogDatabase(logFilePath, expectedMessages);
            }
        }

        private static IEnumerable<TestCaseData> GetMigrationProjectsWithMessages()
        {
            const string allCalculatedResultsRemovedMessage = "* Alle berekende resultaten zijn verwijderd.";
            string adjustedAssemblyResultsMigrationMessage =
                $"* Omdat alleen faalkansen op vakniveau een rol spelen in de assemblage, zijn de assemblageresultaten voor de faalmechanismen aangepast:{Environment.NewLine}" +
                $"  + De initiële faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Handmatig invullen'.{Environment.NewLine}" +
                $"  + De aangescherpte faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Per doorsnede' of 'Beide'.{Environment.NewLine}" +
                "  + De assemblagemethode 'Automatisch berekenen o.b.v. slechtste doorsnede of vak' is vervangen door 'Automatisch berekenen o.b.v. slechtste vak'.";
            yield return new TestCaseData("MigrationTestProject231NoOutput.risk", new[]
            {
                adjustedAssemblyResultsMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231VariousFailureMechanismSectionResultConfigurations.risk", new[]
            {
                adjustedAssemblyResultsMigrationMessage
            });

            // This file contains all configured failure mechanisms (except Dunes and MacroStabilityInwards) with output.
            // The mechanisms Dunes and MacroStabilityInwards have different assessment sections, and are therefore put in different test files.
            yield return new TestCaseData("MigrationTestProject231WithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                adjustedAssemblyResultsMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231DunesWithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                adjustedAssemblyResultsMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231MacroStabilityInwardsWithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                adjustedAssemblyResultsMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231RevetmentCalculations.risk", new[]
            {
                adjustedAssemblyResultsMigrationMessage
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
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
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
                "WHERE [Version] = \"24.1\";";
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

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateMacroStabilityInwardsOutput =
                "SELECT COUNT() = 0 " +
                "FROM MacroStabilityInwardsCalculationOutputEntity; ";

            reader.AssertReturnedDataIsValid(validateMacroStabilityInwardsOutput);
        }

        private static void AssertPipingOutput(MigratedDatabaseReader reader)
        {
            const string validateProbabilisticCalculationOutput =
                "SELECT COUNT() = 0 " +
                "FROM ProbabilisticPipingCalculationOutputEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateProbabilisticCalculationOutput);

            const string validateSemiProbabilisticOutput =
                "SELECT COUNT() = 0 " +
                "FROM SemiProbabilisticPipingCalculationOutputEntity; ";
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

        #region Failure mechanism section results

        private static void AssertAdoptableFailureMechanismSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateNrOfSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT SUM(count) " +
                "FROM " +
                "( " +
                "SELECT COUNT() as count " +
                "FROM SOURCEPROJECT.AdoptableFailureMechanismSectionResultEntity " +
                "UNION " +
                "SELECT COUNT() as count " +
                "FROM SOURCEPROJECT.AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity " +
                ")" +
                ")" +
                "FROM AdoptableFailureMechanismSectionResultEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfSectionResults);

            string validateExistingFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AdoptableFailureMechanismSectionResultEntity " +
                ")" +
                "FROM AdoptableFailureMechanismSectionResultEntity NEW " +
                "JOIN  SOURCEPROJECT.AdoptableFailureMechanismSectionResultEntity OLD USING(AdoptableFailureMechanismSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = OLD.[IsRelevant] " +
                "AND NEW.[InitialFailureMechanismResultType] = OLD.[InitialFailureMechanismResultType] " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS OLD.[ManualInitialFailureMechanismResultSectionProbability] " +
                "AND NEW.[FurtherAnalysisType] = OLD.[FurtherAnalysisType] " +
                "AND NEW.[RefinedSectionProbability] IS OLD.[RefinedSectionProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateExistingFailureMechanismSectionResults);

            string validateMigratedAdoptableWithProfileProbabilityFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity " +
                ")" +
                "FROM AdoptableFailureMechanismSectionResultEntity NEW " +
                "JOIN  SOURCEPROJECT.AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity OLD USING(FailureMechanismSectionEntityId) " +
                "JOIN ( " +
                "SELECT " +
                "FailureMechanismSectionEntityId, " +
                "CASE " +
                "WHEN [ProbabilityRefinementType] = 2 OR [ProbabilityRefinementType] = 3 " +
                "THEN [RefinedSectionProbability] " +
                "WHEN [ProbabilityRefinementType] = 1 " +
                "THEN NULL " +
                "END AS ExpectedRefinedSectionProbability " +
                "FROM SOURCEPROJECT.AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity" +
                ") AS ofmsr USING(FailureMechanismSectionEntityId)" +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = OLD.[IsRelevant] " +
                "AND NEW.[InitialFailureMechanismResultType] = OLD.[InitialFailureMechanismResultType] " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS OLD.[ManualInitialFailureMechanismResultSectionProbability] " +
                "AND NEW.[FurtherAnalysisType] = OLD.[FurtherAnalysisType] " +
                "AND NEW.[RefinedSectionProbability] IS ofmsr.[ExpectedRefinedSectionProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedAdoptableWithProfileProbabilityFailureMechanismSectionResults);
        }

        private static void AssertNonAdoptableFailureMechanismSectionResult(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateNrOfSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT SUM(count) " +
                "FROM " +
                "( " +
                "SELECT COUNT() as count " +
                "FROM SOURCEPROJECT.NonAdoptableFailureMechanismSectionResultEntity " +
                "UNION " +
                "SELECT COUNT() as count " +
                "FROM SOURCEPROJECT.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity " +
                ")" +
                ")" +
                "FROM NonAdoptableFailureMechanismSectionResultEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfSectionResults);

            string validateExistingFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.NonAdoptableFailureMechanismSectionResultEntity " +
                ")" +
                "FROM NonAdoptableFailureMechanismSectionResultEntity NEW " +
                "JOIN  SOURCEPROJECT.NonAdoptableFailureMechanismSectionResultEntity OLD USING(NonAdoptableFailureMechanismSectionResultEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = OLD.[IsRelevant] " +
                "AND NEW.[InitialFailureMechanismResultType] = OLD.[InitialFailureMechanismResultType] " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS OLD.[ManualInitialFailureMechanismResultSectionProbability] " +
                "AND NEW.[FurtherAnalysisType] = OLD.[FurtherAnalysisType] " +
                "AND NEW.[RefinedSectionProbability] IS OLD.[RefinedSectionProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateExistingFailureMechanismSectionResults);

            string validateMigratedNonAdoptableWithProfileProbabilityFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity " +
                ")" +
                "FROM NonAdoptableFailureMechanismSectionResultEntity NEW " +
                "JOIN  SOURCEPROJECT.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity OLD USING(FailureMechanismSectionEntityId) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = OLD.[IsRelevant] " +
                "AND NEW.[InitialFailureMechanismResultType] = OLD.[InitialFailureMechanismResultType] " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS OLD.[ManualInitialFailureMechanismResultSectionProbability] " +
                "AND NEW.[FurtherAnalysisType] = OLD.[FurtherAnalysisType] " +
                "AND NEW.[RefinedSectionProbability] IS OLD.[RefinedSectionProbability]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedNonAdoptableWithProfileProbabilityFailureMechanismSectionResults);
        }

        #endregion

        #region Piping

        private static void AssertPipingFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.PipingFailureMechanismMetaEntity " +
                ") " +
                "FROM PipingFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.PipingFailureMechanismMetaEntity OLD USING(PipingFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[WaterVolumetricWeight] = OLD.[WaterVolumetricWeight] " +
                "AND NEW.[StochasticSoilModelCollectionSourcePath] IS OLD.[StochasticSoilModelCollectionSourcePath] " +
                "AND NEW.[SurfaceLineCollectionSourcePath] IS OLD.[SurfaceLineCollectionSourcePath] " +
                "AND NEW.[ScenarioConfigurationType] = OLD.[PipingScenarioConfigurationType]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertPipingFailureMechanismSectionConfiguration(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.PipingScenarioConfigurationPerFailureMechanismSectionEntity " +
                ") " +
                "FROM PipingFailureMechanismSectionConfigurationEntity NEW " +
                "JOIN SOURCEPROJECT.PipingScenarioConfigurationPerFailureMechanismSectionEntity OLD " +
                "WHERE NEW.[PipingFailureMechanismSectionConfigurationEntityId] = OLD.[PipingScenarioConfigurationPerFailureMechanismSectionEntityId] " +
                "AND NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[ScenarioConfigurationType] = OLD.[PipingScenarioConfigurationPerFailureMechanismSectionType] " +
                "AND NEW.[A] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        #endregion

        #region MacroStabilityInwards

        private static void AssertMacroStabilityInwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.MacroStabilityInwardsFailureMechanismMetaEntity " +
                ") " +
                "FROM MacroStabilityInwardsFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsFailureMechanismMetaEntity OLD USING(MacroStabilityInwardsFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[StochasticSoilModelCollectionSourcePath] IS OLD.[StochasticSoilModelCollectionSourcePath] " +
                "AND NEW.[SurfaceLineCollectionSourcePath] IS OLD.[SurfaceLineCollectionSourcePath]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertMacroStabilityInwardsFailureMechanismSectionConfiguration(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "JOIN SOURCEPROJECT.FailureMechanismFailureMechanismSectionEntity USING(FailureMechanismEntityId) " +
                "WHERE FailureMechanismType = 2 " +
                ") " +
                "FROM FailureMechanismEntity " +
                "JOIN FailureMechanismFailureMechanismSectionEntity USING(FailureMechanismEntityId) " +
                "JOIN MacroStabilityInwardsFailureMechanismSectionConfigurationEntity NEW USING(FailureMechanismSectionEntityId) " +
                "WHERE FailureMechanismType = 2 " +
                "AND NEW.[A] = 1; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        #endregion
    }
}