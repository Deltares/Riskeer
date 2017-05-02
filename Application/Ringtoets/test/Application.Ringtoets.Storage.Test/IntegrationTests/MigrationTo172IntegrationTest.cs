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

                    AssertHeightStructuresFailureMechanism(reader);

                    AssertVersions(reader);
                    AssertDatabase(reader);
                }

                AssertLogDatabase(logFilePath);
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
                "MacrostabilityInwardsSectionResultEntity",
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
                    $"ATTACH DATABASE[{sourceFilePath}] AS SOURCEPROJECT; " +
                    $"SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].{table}) " +
                    $"FROM {table};" +
                    "DETACH SOURCEPROJECT;";
                reader.AssertReturnedDataIsValid(validateMigratedTable);
            }
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateSourcePathNull =
                "SELECT COUNT() = 0 FROM ( " +
                "SELECT[HeightStructuresFailureMechanismMetaEntityId] " +
                "FROM HeightStructuresFailureMechanismMetaEntity WHERE [HeightStructureCollectionSourcePath] IS NULL " +
                "EXCEPT " +
                "SELECT[HeightStructuresFailureMechanismMetaEntityId] " +
                "FROM HeightStructuresFailureMechanismMetaEntity HSFM " +
                "WHERE HSFM.[FailureMechanismEntityId] NOT IN (SELECT FailureMechanismEntityId FROM HeightStructureEntity HS) " +
                "); ";
            reader.AssertReturnedDataIsValid(validateSourcePathNull);

            const string validateSourcePathSet =
                "SELECT COUNT() = 0 FROM ( " +
                "SELECT[HeightStructuresFailureMechanismMetaEntityId] " +
                "FROM HeightStructuresFailureMechanismMetaEntity WHERE [HeightStructureCollectionSourcePath] = \"Onbekend\" " +
                "EXCEPT " +
                "SELECT[HeightStructuresFailureMechanismMetaEntityId] " +
                "FROM HeightStructuresFailureMechanismMetaEntity HSFM " +
                "WHERE HSFM.[FailureMechanismEntityId] IN(SELECT FailureMechanismEntityId FROM HeightStructureEntity HS) " +
                "); ";
            reader.AssertReturnedDataIsValid(validateSourcePathSet);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(0, messages.Count);
            }
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