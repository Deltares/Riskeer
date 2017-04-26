﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    [TestFixture]
    public class MigrationTo171IntegrationTest
    {
        private const string newVersion = "17.1";

        [Test]
        public void Given164Project_WhenUpgradedTo171_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected), ".log"));
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
                    AssertDuneErosionFailureMechanism(reader);
                    AssertClosingStructuresFailureMechanism(reader);
                    AssertGrassCoverErosionInwardsFailureMechanism(reader);
                    AssertGrassCoverErosionOutwardsFailureMechanism(reader);
                    AssertHeightStructuresFailureMechanism(reader);
                    AssertPipingFailureMechanism(reader);
                    AssertStabilityPointStructuresFailureMechanism(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);

                    AssertHydraulicBoundaryLocations(reader);
                    AssertDikeProfiles(reader);
                    AssertForeshoreProfiles(reader);
                    AssertStochasticSoilModels(reader);
                    AssertSurfaceLines(reader);
                    AssertBackgroundData(reader);

                    AssertVersions(reader);
                    AssertDatabase(reader);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertDikeProfiles(MigratedDatabaseReader reader)
        {
            const string validateDikeProfiles =
                "SELECT " +
                "(SELECT COUNT(DISTINCT(Name)) = COUNT() FROM DikeProfileEntity) " +
                "AND (SELECT COUNT() = 0 FROM DikeProfileEntity WHERE Id != Name);";
            reader.AssertReturnedDataIsValid(validateDikeProfiles);
        }

        private static void AssertForeshoreProfiles(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfiles =
                "SELECT COUNT() = 0 " +
                "FROM ForeshoreProfileEntity " +
                "WHERE Id != Name;";
            reader.AssertReturnedDataIsValid(validateForeshoreProfiles);
        }

        private static void AssertStochasticSoilModels(MigratedDatabaseReader reader)
        {
            const string validateStochasticSoilModels =
                "SELECT COUNT(DISTINCT(Name)) = COUNT() " +
                "FROM StochasticSoilModelEntity;";
            reader.AssertReturnedDataIsValid(validateStochasticSoilModels);

            AssertStochasticSoilProfiles(reader);
        }

        private static void AssertStochasticSoilProfiles(MigratedDatabaseReader reader)
        {
            const string validateStochasticSoilProfiles =
                "SELECT " +
                "(SELECT COUNT() != 0 FROM StochasticSoilProfileEntity WHERE [Type] = 1) " +
                "AND " +
                "(SELECT COUNT() = 0 FROM StochasticSoilProfileEntity WHERE [Probability] NOT BETWEEN 0 AND 1 OR[Probability] IS NULL);";
            reader.AssertReturnedDataIsValid(validateStochasticSoilProfiles);
        }

        private static void AssertSurfaceLines(MigratedDatabaseReader reader)
        {
            const string validateSurfaceLines =
                "SELECT COUNT(DISTINCT(Name)) = COUNT() " +
                "FROM SurfaceLineEntity;";
            reader.AssertReturnedDataIsValid(validateSurfaceLines);
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM VersionEntity " +
                "WHERE [Version] = \"17.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertBackgroundData(MigratedDatabaseReader reader)
        {
            const string validateBackgroundData =
                "SELECT COUNT() = " +
                "(SELECT COUNT() FROM AssessmentSectionEntity) " +
                "FROM BackgroundDataEntity " +
                "WHERE [Name] = \"Bing Maps - Satelliet\" " +
                "AND [IsVisible] = 1 " +
                "AND [Transparency] = 0.0 " +
                "AND [BackgroundDataType] = 2 " +
                "AND [AssessmentSectionEntityId] IN " +
                "(SELECT [AssessmentSectionEntityId] FROM AssessmentSectionEntity);";
            reader.AssertReturnedDataIsValid(validateBackgroundData);

            const string validateBackgroundMetaData =
                "SELECT COUNT() = " +
                "(SELECT COUNT() FROM BackgroundDataEntity) " +
                "FROM BackgroundDataMetaEntity " +
                "WHERE [Key] = \"WellKnownTileSource\" " +
                "AND [Value] = \"2\" " +
                "AND [BackgroundDataEntityId] IN " +
                "(SELECT BackgroundDataEntityId FROM BackgroundDataEntity);";
            reader.AssertReturnedDataIsValid(validateBackgroundMetaData);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(1, messages.Count);
                var expectedMessage = new MigrationLogMessage("5", newVersion, "Alle berekende resultaten zijn verwijderd.");
                AssertMigrationLogMessageEqual(expectedMessage, messages[0]);
            }
        }

        private static void AssertMigrationLogMessageEqual(MigrationLogMessage expected, MigrationLogMessage actual)
        {
            Assert.AreEqual(expected.ToVersion, actual.ToVersion);
            Assert.AreEqual(expected.FromVersion, actual.FromVersion);
            Assert.AreEqual(expected.Message, actual.Message);
        }

        private static void AssertClosingStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM ClosingStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertDuneErosionFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 8) " +
                "FROM DuneErosionFailureMechanismMetaEntity " +
                "WHERE N = 2.0 " +
                "AND FailureMechanismEntityId IN " +
                "(SELECT FailureMechanismEntityId FROM FailureMechanismEntity WHERE FailureMechanismType = 8);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsFailureMechanismMetaEntity] " +
                "WHERE [DikeProfileCollectionSourcePath] != \"Onbekend\";";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsCalculationEntity] " +
                "WHERE [OvertoppingRateCalculationType] != 1;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity] " +
                "JOIN [GrassCoverErosionInwardsOutputEntity];";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                "JOIN GrassCoverErosionOutwardsWaveConditionsOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM HeightStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string validateOutputs =
                "SELECT COUNT() = 0 " +
                "FROM HydraulicLocationOutputEntity";
            reader.AssertReturnedDataIsValid(validateOutputs);
        }

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [PipingFailureMechanismMetaEntity] " +
                "WHERE [StochasticSoilModelSourcePath] != \"Onbekend\"" +
                "OR [SurfacelineSourcePath] != \"Onbekend\";";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM PipingCalculationOutputEntity " +
                "JOIN PipingSemiProbabilisticOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertStabilityPointStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM StabilityPointStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM StabilityStoneCoverWaveConditionsOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM WaveImpactAsphaltCoverWaveConditionsOutputEntity";

            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
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