// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertDikeProfiles(MigratedDatabaseReader reader)
        {
            const string validateDikeProfiles = "SELECT " +
                                                "(SELECT COUNT(DISTINCT(Name)) = COUNT() FROM DikeProfileEntity) " +
                                                "AND (SELECT COUNT() = 0 FROM DikeProfileEntity WHERE Id != Name);";
            reader.AssertReturnedDataIsValid(validateDikeProfiles);
        }

        private static void AssertForeshoreProfiles(MigratedDatabaseReader reader)
        {
            const string validateDikeProfiles = "SELECT COUNT() = 0 " +
                                                "FROM ForeshoreProfileEntity WHERE Id != Name;";
            reader.AssertReturnedDataIsValid(validateDikeProfiles);
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
            const string getClosingStructuresCalculationOutput =
                "SELECT 'x' " +
                "FROM ClosingStructuresOutputEntity";
            reader.AssertNoDataToBeRead(getClosingStructuresCalculationOutput);
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

            const string getGrassCoverErosionInwardsCalculationOutput =
                "SELECT 'x' " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity] " +
                "JOIN [GrassCoverErosionInwardsOutputEntity];";
            reader.AssertNoDataToBeRead(getGrassCoverErosionInwardsCalculationOutput);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getGrassCoverErosionOutwardsCalculationOutput =
                "SELECT 'x' " +
                "FROM GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                "JOIN GrassCoverErosionOutwardsWaveConditionsOutputEntity";
            reader.AssertNoDataToBeRead(getGrassCoverErosionOutwardsCalculationOutput);
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getHeightStructuresCalculationOutput =
                "SELECT 'x' " +
                "FROM HeightStructuresOutputEntity";
            reader.AssertNoDataToBeRead(getHeightStructuresCalculationOutput);
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string getHydraulicBoundaryLocationsOutput =
                "SELECT 'x' " +
                "FROM HydraulicLocationOutputEntity";
            reader.AssertNoDataToBeRead(getHydraulicBoundaryLocationsOutput);
        }

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [PipingFailureMechanismMetaEntity] " +
                "WHERE [StochasticSoilModelSourcePath] != \"Onbekend\"" +
                "OR [SurfacelineSourcePath] != \"Onbekend\";";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string getPipingCalculationOutput =
                "SELECT 'x' " +
                "FROM PipingCalculationOutputEntity " +
                "JOIN PipingSemiProbabilisticOutputEntity";
            reader.AssertNoDataToBeRead(getPipingCalculationOutput);
        }

        private static void AssertStabilityPointStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getStabilityPointStructuresCalculationOutput =
                "SELECT 'x' " +
                "FROM StabilityPointStructuresOutputEntity";
            reader.AssertNoDataToBeRead(getStabilityPointStructuresCalculationOutput);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getStabilityStoneCoverWaveConditionsCalculationOutput =
                "SELECT 'x' " +
                "FROM StabilityStoneCoverWaveConditionsOutputEntity";
            reader.AssertNoDataToBeRead(getStabilityStoneCoverWaveConditionsCalculationOutput);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getWaveImpactAsphaltCoverCalculationOutput =
                "SELECT 'x' " +
                "FROM WaveImpactAsphaltCoverWaveConditionsOutputEntity";

            reader.AssertNoDataToBeRead(getWaveImpactAsphaltCoverCalculationOutput);
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
            /// Asserts that the <paramref name="queryString"/> result is empty.
            /// </summary>
            /// <param name="queryString">The query to execute.</param>
            /// <exception cref="SQLiteException">The execution of <paramref name="queryString"/> 
            /// failed.</exception>
            public void AssertNoDataToBeRead(string queryString)
            {
                using (IDataReader dataReader = CreateDataReader(queryString))
                {
                    Assert.IsFalse(dataReader.Read());
                }
            }

            public void AssertReturnedDataIsValid(string queryString)
            {
                using (IDataReader dataReader = CreateDataReader(queryString))
                {
                    Assert.IsTrue(dataReader.Read());
                    Assert.AreEqual(1, dataReader.FieldCount);
                    Assert.AreEqual(1, dataReader[0]);
                }
            }
        }
    }
}