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

using System;
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
        private readonly TestDataPath testPath = TestDataPath.Application.Ringtoets.Migration.Core;

        [Test]
        public void Given164Project_WhenUpgradedTo171_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(testPath, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

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
                }
            }
        }

        private static void AssertClosingStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getClosingStructuresCalculationOutput = "SELECT 'x' " +
                                                                 "FROM ClosingStructuresOutputEntity";
            reader.AssertData(getClosingStructuresCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getGrassCoverErosionInwardsCalculationOutput = "SELECT 'x' " +
                                                                        "FROM GrassCoverErosionInwardsDikeHeightOutputEntity " +
                                                                        "JOIN GrassCoverErosionInwardsOutputEntity";
            reader.AssertData(getGrassCoverErosionInwardsCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getGrassCoverErosionOutwardsCalculationOutput = "SELECT 'x' " +
                                                                         "FROM GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                                                                         "JOIN GrassCoverErosionOutwardsWaveConditionsOutputEntity";
            reader.AssertData(getGrassCoverErosionOutwardsCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getHeightStructuresCalculationOutput = "SELECT 'x' " +
                                                                "FROM HeightStructuresOutputEntity";
            reader.AssertData(getHeightStructuresCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string getHydraulicBoundaryLocationsOutput = "SELECT 'x' " +
                                                               "FROM HydraulicLocationOutputEntity";
            reader.AssertData(getHydraulicBoundaryLocationsOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getPipingCalculationOutput = "SELECT 'x' " +
                                                      "FROM PipingCalculationOutputEntity " +
                                                      "JOIN PipingSemiProbabilisticOutputEntity";
            reader.AssertData(getPipingCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertStabilityPointStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getStabilityPointStructuresCalculationOutput = "SELECT 'x' " +
                                                                        "FROM StabilityPointStructuresOutputEntity";
            reader.AssertData(getStabilityPointStructuresCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getStabilityStoneCoverWaveConditionsCalculationOutput = "SELECT 'x' " +
                                                                                 "FROM StabilityStoneCoverWaveConditionsOutputEntity";
            reader.AssertData(getStabilityStoneCoverWaveConditionsCalculationOutput, r => Assert.IsFalse(r.Read()));
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string getWaveImpactAsphaltCoverCalculationOutput = "SELECT 'x' " +
                                                                      "FROM WaveImpactAsphaltCoverWaveConditionsOutputEntity";

            reader.AssertData(getWaveImpactAsphaltCoverCalculationOutput, r => Assert.IsFalse(r.Read()));
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
            /// Asserts the data from the <paramref name="queryString"/> result.
            /// </summary>
            /// <param name="queryString">The query to execute.</param>
            /// <param name="assert">The assert to perform.</param>
            /// <exception cref="SQLiteException">The execution of queryString failed.</exception>
            public void AssertData(string queryString, Action<IDataReader> assert)
            {
                using (IDataReader dataReader = CreateDataReader(queryString))
                {
                    assert(dataReader);
                }
            }
        }
    }
}