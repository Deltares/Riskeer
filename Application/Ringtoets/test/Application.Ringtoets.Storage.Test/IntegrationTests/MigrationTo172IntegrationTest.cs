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

using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using Application.Ringtoets.Migration.Core;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    public class MigrationTo172IntegrationTest
    {
        private const string newVersion = "17.2";

        [Test]
        public void Given171Project_WhenUpgradedTo172_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               "FullTestProject171.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given171Project_WhenUpgradedTo172_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given171Project_WhenUpgradedTo172_ThenProjectAsExpected), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
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

                    AssertGrassCoverErosionOutwardsFailureMechanism(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);
                    AssertHeightStructuresFailureMechanism(reader);
                    AssertClosingStructuresFailureMechanism(reader);
                    AssertStabilityPointStructuresFailureMechanism(reader);
                    AssertGrassCoverErosionInwardsFailureMechanism(reader);
                    AssertHydraulicBoundaryLocations(reader);

                    AssertClosingStructures(reader);
                    AssertHeightStructures(reader);
                    AssertStabilityPointStructures(reader);
                    AssertForeshoreProfiles(reader);

                    AssertSoilProfiles(reader, sourceFilePath);

                    AssertFailureMechanismSectionResults(reader, "ClosingStructuresSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "StabilityPointStructuresSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "HeightStructuresSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "PipingSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "GrassCoverErosionInwardsSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "MacroStabilityInwardsSectionResultEntity");

                    AssertVersions(reader);
                    AssertDatabase(reader);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        [Test]
        public void GivenEmpty164Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               "Empty valid Release 16.4.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(
                nameof(GivenEmpty164Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade));
            string logFilePath = TestHelper.GetScratchPadPath(
                string.Concat(nameof(GivenEmpty164Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(4, messages.Count);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("5", "17.1", "Gevolgen van de migratie van versie 16.4 naar versie 17.1:"),
                        messages[0]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("5", "17.1", "* Geen aanpassingen."),
                        messages[1]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[2]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "* Geen aanpassingen."),
                        messages[3]);
                }
            }
        }

        [Test]
        public void GivenEmpty171Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               "Empty valid Release 17.1.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(
                nameof(GivenEmpty171Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade));
            string logFilePath = TestHelper.GetScratchPadPath(
                string.Concat(nameof(GivenEmpty171Project_WhenNoChangesMade_ThenLogDatabaseContainsMessagesSayingNoChangesMade), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(2, messages.Count);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[0]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "* Geen aanpassingen."),
                        messages[1]);
                }
            }
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var tables = new[]
            {
                "AssessmentSectionEntity",
                "CalculationGroupEntity",
                "CharacteristicPointEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionSectionResultEntity",
                "FailureMechanismEntity",
                "FailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsHydraulicLocationEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicLocationEntity",
                "MacroStabilityInwardsSectionResultEntity",
                "MacrostabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCalculationEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
                "SoilLayerEntity",
                "SoilProfileEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochasticSoilModelEntity",
                "StochasticSoilProfileEntity",
                "StrengthStabilityLengthwiseConstructionSectionResultEntity",
                "SurfaceLineEntity",
                "TechnicalInnovationSectionResultEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
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

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM[GrassCoverErosionOutwardsFailureMechanismMetaEntity] " +
                "LEFT JOIN[ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY[FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM GrassCoverErosionOutwardsHydraulicLocationEntity " +
                "WHERE ShouldWaveHeightIllustrationPointsBeCalculated != 0 " +
                "|| ShouldDesignWaterLevelIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM[StabilityStoneCoverFailureMechanismMetaEntity] " +
                "LEFT JOIN[ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY[FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM[WaveImpactAsphaltCoverFailureMechanismMetaEntity] " +
                "LEFT JOIN[ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY[FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateStructuresCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (SELECT " +
                "CASE WHEN " +
                "COUNT([HeightStructureEntityId]) AND [HeightStructureCollectionSourcePath] IS NULL " +
                "OR " +
                "[HeightStructureCollectionSourcePath] IS NOT NULL AND NOT COUNT([HeightStructureEntityId]) " +
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [HeightStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [HeightStructureEntity] USING ([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateStructuresCollectionSourcePath);

            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [HeightStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM HeightStructuresCalculationEntity " +
                "WHERE ShouldIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertClosingStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateStructuresCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (SELECT " +
                "CASE WHEN " +
                "COUNT([ClosingStructureEntityId]) AND [ClosingStructureCollectionSourcePath] IS NULL " +
                "OR " +
                "[ClosingStructureCollectionSourcePath] IS NOT NULL AND NOT COUNT([ClosingStructureEntityId]) " +
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [ClosingStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ClosingStructureEntity] USING ([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateStructuresCollectionSourcePath);

            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [ClosingStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM ClosingStructuresCalculationEntity " +
                "WHERE ShouldIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertStabilityPointStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateStructuresCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (SELECT " +
                "CASE WHEN " +
                "COUNT([StabilityPointStructureEntityId]) AND [StabilityPointStructureCollectionSourcePath] IS NULL " +
                "OR " +
                "[StabilityPointStructureCollectionSourcePath] IS NOT NULL AND NOT COUNT([StabilityPointStructureEntityId]) " +
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [StabilityPointStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [StabilityPointStructureEntity] USING ([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateStructuresCollectionSourcePath);

            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM(" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [StabilityPointStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM StabilityPointStructuresCalculationEntity " +
                "WHERE ShouldIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM GrassCoverErosionInwardsCalculationEntity " +
                "WHERE ShouldDikeHeightIllustrationPointsBeCalculated != 0 AND " +
                "ShouldOvertoppingRateIllustrationPointsBeCalculated != 0 AND " +
                "ShouldOvertoppingOutputIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM HydraulicLocationEntity " +
                "WHERE ShouldWaveHeightIllustrationPointsBeCalculated != 0 " +
                "|| ShouldDesignWaterLevelIllustrationPointsBeCalculated != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(36, messages.Count);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                    messages[0]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSection'"),
                    messages[1]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[2]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[3]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[4]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '102'."),
                    messages[5]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Golfklappen op asfaltbekleding'"),
                    messages[6]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '10' is veranderd naar '10004'."),
                    messages[7]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Stabiliteit steenzetting'"),
                    messages[8]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '100' is veranderd naar '10006'."),
                    messages[9]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[10]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '102'."),
                    messages[11]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'Demo traject'"),
                    messages[12]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[13]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[14]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[15]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[16]);

                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSectionResults'"),
                    messages[17]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[18]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[19],
                    messages[20]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[21]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[22],
                    messages[23]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[24]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[25],
                    messages[26]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[27]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[28],
                    messages[29]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Macrostabiliteit binnenwaarts'"),
                    messages[30]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[31],
                    messages[32]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[33]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[34],
                    messages[35]
                });
            }
        }

        private static void AssertLayerThreeMigrationMessage(MigrationLogMessage[] messages)
        {
            Assert.AreEqual(2, messages.Length);

            AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, "    - Het geregistreerde resultaat voor toetslaag 3 in 'Vak 1' ('5.0') kon niet worden geconverteerd naar een geldige kans en is verwijderd."),
                messages[0]);
            AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, "    - Het geregistreerde resultaat voor toetslaag 3 in 'Vak 2' ('-10.0') kon niet worden geconverteerd naar een geldige kans en is verwijderd."),
                messages[1]);
        }

        private static void AssertMigrationLogMessageEqual(MigrationLogMessage expected, MigrationLogMessage actual)
        {
            Assert.AreEqual(expected.ToVersion, actual.ToVersion);
            Assert.AreEqual(expected.FromVersion, actual.FromVersion);
            Assert.AreEqual(expected.Message, actual.Message);
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM VersionEntity " +
                "WHERE [Version] = \"17.2\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertForeshoreProfiles(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfiles =
                "SELECT COUNT(DISTINCT([Id])) IS COUNT() " +
                "FROM ForeshoreProfileEntity " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateForeshoreProfiles);
        }

        private static void AssertHeightStructures(MigratedDatabaseReader reader)
        {
            const string validateHeightStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM HeightStructureEntity " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateHeightStructures);
        }

        private static void AssertClosingStructures(MigratedDatabaseReader reader)
        {
            const string validateClosingStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM ClosingStructureEntity " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateClosingStructures);
        }

        private static void AssertStabilityPointStructures(MigratedDatabaseReader reader)
        {
            const string validateStabilityPointStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM StabilityPointStructureEntity " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateStabilityPointStructures);
        }

        private static void AssertSoilProfiles(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilProfileEntity) " +
                "FROM SoilProfileEntity " +
                "WHERE [SourceType] IN (1,2);" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertFailureMechanismSectionResults(MigratedDatabaseReader reader, string sectionResultTable)
        {
            string validateFailureMechanismSectionResults =
                "SELECT COUNT() = 0 " +
                $"FROM {sectionResultTable} " +
                "WHERE LayerThree < 0 OR LayerThree > 1";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        /// <summary>
        /// Database reader for migrated database.
        /// </summary>
        private class MigratedDatabaseReader : SqLiteDatabaseReaderBase
        {
            /// <summary>
            /// Creates a new instance of <see cref="MigratedDatabaseReader"/>.
            /// </summary>
            /// <param name="databaseFilePath">The path of the database file to open.</param>
            /// <exception cref="CriticalFileReadException">Thrown when:
            /// <list type="bullet">
            /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
            /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
            /// <item>Unable to open database file.</item>
            /// </list>
            /// </exception>
            public MigratedDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

            /// <summary>
            /// Asserts that the <paramref name="queryString"/> results in one field with the value <c>true</c>.
            /// </summary>
            /// <param name="queryString">The query to execute.</param>
            /// <exception cref="SQLiteException">The execution of <paramref name="queryString"/> 
            /// failed.</exception>
            public void AssertReturnedDataIsValid(string queryString)
            {
                using (IDataReader dataReader = CreateDataReader(queryString))
                {
                    Assert.IsTrue(dataReader.Read(), "No data can be read from the data reader " +
                                                     $"when using query '{queryString}'.");
                    Assert.AreEqual(1, dataReader.FieldCount, $"Expected one field, was {dataReader.FieldCount} " +
                                                              $"fields when using query '{queryString}'.");
                    Assert.AreEqual(1, dataReader[0], $"Result should be 1 when using query '{queryString}'.");
                }
            }
        }
    }
}