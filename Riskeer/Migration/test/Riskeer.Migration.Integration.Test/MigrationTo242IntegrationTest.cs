﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
    public class MigrationTo242IntegrationTest
    {
        private const string newVersion = "24.2";

        [Test]
        [TestCaseSource(nameof(GetMigrationProjectsWithMessages))]
        public void Given241Project_WhenUpgradedTo242_ThenProjectAsExpected(string fileName, IEnumerable<string> expectedMessages)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               fileName);
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given241Project_WhenUpgradedTo242_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given241Project_WhenUpgradedTo242_ThenProjectAsExpected), ".log"));
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
                }

                AssertLogDatabase(logFilePath, expectedMessages);
            }
        }

        private static IEnumerable<TestCaseData> GetMigrationProjectsWithMessages()
        {
            const string allCalculatedResultsRemovedMessage = "* Alle berekende resultaten zijn verwijderd.";
            string fixedMigrationMessage =
                $"* Omdat alleen faalkansen op vakniveau een rol spelen in de assemblage, zijn de assemblageresultaten voor de faalmechanismen aangepast:{Environment.NewLine}" +
                $"  + De initiële faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Handmatig invullen'.{Environment.NewLine}" +
                $"  + De aangescherpte faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Per doorsnede' of 'Beide'.{Environment.NewLine}" +
                $"  + De assemblagemethode 'Automatisch berekenen o.b.v. slechtste doorsnede of vak' is vervangen door 'Automatisch berekenen o.b.v. slechtste vak'.{Environment.NewLine}" +
                "* Voor HLCD bestanden waarbij geen tabel 'ScenarioInformation' aanwezig is, worden niet langer standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie.";

            yield return new TestCaseData("MigrationTestProject231NoOutput.risk", new[]
            {
                fixedMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231VariousFailureMechanismSectionResultConfigurations.risk", new[]
            {
                fixedMigrationMessage
            });

            // This file contains all configured failure mechanisms (except Dunes and MacroStabilityInwards) with output.
            // The mechanisms Dunes and MacroStabilityInwards have different assessment sections, and are therefore put in different test files.
            yield return new TestCaseData("MigrationTestProject231WithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                fixedMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231DunesWithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                fixedMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231MacroStabilityInwardsWithOutput.risk", new[]
            {
                allCalculatedResultsRemovedMessage,
                fixedMigrationMessage
            });

            yield return new TestCaseData("MigrationTestProject231RevetmentCalculations.risk", new[]
            {
                fixedMigrationMessage
            });
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
                    new MigrationLogMessage("24.1", newVersion, $"Gevolgen van de migratie van versie 24.1 naar versie {newVersion}:"),
                    messages[i++]);

                foreach (string expectedMessage in expectedMessages)
                {
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("24.1", newVersion, $"{expectedMessage}"),
                        messages[i++]);
                }
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"24.2\";";
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