﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Application.Ringtoets.Migration.Core;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Contribution;

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

                    AssertMacroStabilityInwardsFailureMechanism(reader);
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

                    AssertPipingSoilProfiles(reader, sourceFilePath);
                    AssertPipingSoilLayers(reader, sourceFilePath);
                    AssertPipingStochasticSoilProfiles(reader, sourceFilePath);
                    AssertPipingCharacteristicPoints(reader, sourceFilePath);

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

        [Test]
        [SetCulture("en-US")]
        [TestCaseSource(nameof(GetTrajectCombinations))]
        public void Given171ProjectOfTrajectWithNorm_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase(NormType setNormType,
                                                                                                            string trajectId,
                                                                                                            int signalingReturnPeriod,
                                                                                                            int lowerLimitReturnPeriod)
        {
            // Given
            string testName = TestContext.CurrentContext.Test.Name.Replace("\"", string.Empty);

            string sourceFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, "Source", ".sql"));
            string targetFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, "Target", ".sql"));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, ".log"));

            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(sourceFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                File.Copy(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                     "Empty valid Release 17.1.rtd"),
                          sourceFilePath, true
                );

                var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
                using (var databaseFile = new RingtoetsDatabaseFile(sourceFilePath))
                {
                    databaseFile.OpenDatabaseConnection();

                    double fromNorm = 1.0 / (setNormType == NormType.Signaling
                                                 ? signalingReturnPeriod
                                                 : lowerLimitReturnPeriod);
                    databaseFile.ExecuteQuery("INSERT INTO ProjectEntity ([ProjectEntityId]) VALUES (1);");
                    databaseFile.ExecuteQuery("INSERT INTO AssessmentSectionEntity ([ProjectEntityId], [Composition], [Order], [Name], [Id], [Norm]) " +
                                              $"VALUES (1, 1, 0, \"{trajectId}\", \"{trajectId}\", {fromNorm});");
                }

                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                NormType expectedNormType = lowerLimitReturnPeriod == signalingReturnPeriod
                                                ? NormType.Signaling
                                                : setNormType;
                byte normTypeByte = Convert.ToByte(expectedNormType);

                string expectedAssessmentSectionQuery =
                    "SELECT COUNT() = 1 " +
                    "FROM AssessmentSectionEntity " +
                    $"WHERE [Id] = \"{trajectId}\" " +
                    $"AND CAST(1.0 / [LowerLimitNorm] AS FLOAT) BETWEEN ({lowerLimitReturnPeriod} - 0.1) AND ({lowerLimitReturnPeriod} + 0.1) " +
                    $"AND CAST(1.0 / [SignalingNorm] AS FLOAT) BETWEEN ({signalingReturnPeriod} - 0.1) AND ({signalingReturnPeriod} + 0.1) " +
                    $"AND [NormativeNorm] = {normTypeByte}";

                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    reader.AssertReturnedDataIsValid(expectedAssessmentSectionQuery);
                }

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(5, messages.Count);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[0]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"* Traject: '{trajectId}'"),
                        messages[1]);
                    string lowerLimitLogSuffix = expectedNormType == NormType.LowerLimit
                                                     ? " (voorheen de waarde van de norm)"
                                                     : "";
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De ondergrens is gelijk gesteld aan 1/{lowerLimitReturnPeriod}{lowerLimitLogSuffix}."),
                        messages[2]);
                    string signalingLogSuffix = expectedNormType == NormType.Signaling
                                                    ? " (voorheen de waarde van de norm)"
                                                    : "";
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De signaleringswaarde is gelijk gesteld aan 1/{signalingReturnPeriod}{signalingLogSuffix}."),
                        messages[3]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(expectedNormType)}."),
                        messages[4]);
                }
            }
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(2, NormType.LowerLimit, TestName = "Given171ProjectWithNormLargerThenLowerLimit_WhenMigrated_ThenLowerLimitNormSetToNormAndExpectedLogDatabase")]
        [TestCase(600, NormType.Signaling, TestName = "Given171ProjectWithNormLessThenLowerLimit_WhenMigrated_ThenSignalingNormSetToNormAndExpectedLogDatabase")]
        public void Given171ProjectWithNormNotInList_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase(int originalReturnPeriod,
                                                                                                            NormType expectedNormType)
        {
            // Given
            const string trajectId = "2-1";
            const int signalingReturnPeriod = 1000;
            const int lowerLimitReturnPeriod = 300;

            string testName = TestContext.CurrentContext.Test.Name;

            string sourceFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, "Source", ".sql"));
            string targetFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, "Target", ".sql"));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(testName, ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(sourceFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                File.Copy(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                     "Empty valid Release 17.1.rtd"),
                          sourceFilePath, true
                );

                var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
                using (var databaseFile = new RingtoetsDatabaseFile(sourceFilePath))
                {
                    databaseFile.OpenDatabaseConnection();

                    double fromNorm = 1.0 / originalReturnPeriod;
                    databaseFile.ExecuteQuery("INSERT INTO ProjectEntity ([ProjectEntityId]) VALUES (1);");
                    databaseFile.ExecuteQuery("INSERT INTO AssessmentSectionEntity ([ProjectEntityId], [Composition], [Order], [Name], [Id], [Norm]) " +
                                              $"VALUES (1, 1, 0, \"{trajectId}\", \"{trajectId}\", {fromNorm});");
                }

                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                byte normTypeByte = Convert.ToByte(expectedNormType);

                int actualLowerLimitReturnPeriod = expectedNormType == NormType.LowerLimit
                                                       ? originalReturnPeriod
                                                       : lowerLimitReturnPeriod;

                int actualSignalingReturnPeriod = expectedNormType == NormType.Signaling
                                                      ? originalReturnPeriod
                                                      : signalingReturnPeriod;

                string expectedAssessmentSectionQuery =
                    "SELECT COUNT() = 1 " +
                    "FROM AssessmentSectionEntity " +
                    $"WHERE [Id] = \"{trajectId}\" " +
                    $"AND CAST(1.0 / [LowerLimitNorm] AS FLOAT) BETWEEN ({actualLowerLimitReturnPeriod} - 0.1) AND ({actualLowerLimitReturnPeriod} + 0.1) " +
                    $"AND CAST(1.0 / [SignalingNorm] AS FLOAT) BETWEEN ({actualSignalingReturnPeriod} - 0.1) AND ({actualSignalingReturnPeriod} + 0.1) " +
                    $"AND [NormativeNorm] = {normTypeByte}";

                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    reader.AssertReturnedDataIsValid(expectedAssessmentSectionQuery);
                }

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(5, messages.Count);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[0]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "* Traject: '2-1'"),
                        messages[1]);
                    string lowerLimitLogSuffix = expectedNormType == NormType.LowerLimit
                                                     ? " (voorheen de waarde van de norm)"
                                                     : "";
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De ondergrens is gelijk gesteld aan 1/{actualLowerLimitReturnPeriod}{lowerLimitLogSuffix}."),
                        messages[2]);
                    string signalingLogSuffix = expectedNormType == NormType.Signaling
                                                    ? " (voorheen de waarde van de norm)"
                                                    : "";
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De signaleringswaarde is gelijk gesteld aan 1/{actualSignalingReturnPeriod}{signalingLogSuffix}."),
                        messages[3]);
                    AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(expectedNormType)}."),
                        messages[4]);
                }
            }
        }

        private static string GetNormTypeString(NormType normType)
        {
            return normType == NormType.LowerLimit
                       ? "ondergrens"
                       : "signaleringswaarde";
        }

        private static IEnumerable<TestCaseData> GetTrajectCombinations()
        {
            Array normTypes = Enum.GetValues(typeof(NormType));

            IEnumerable<TestCaseData> uniqueTrajectNorms = GetAllTrajectTestCaseData()
                .GroupBy(t => Tuple.Create(t.Arguments[1], t.Arguments[2]))
                .Select(t => t.First());
            foreach (TestCaseData data in uniqueTrajectNorms)
            {
                foreach (NormType normType in normTypes)
                {
                    var trajectId = (string) data.Arguments[0];
                    yield return new TestCaseData(normType,
                                                  trajectId,
                                                  data.Arguments[1],
                                                  data.Arguments[2])
                        .SetName("Given171ProjectWithNorm{1}OfTraject{0}_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase");
                }
            }

            yield return new TestCaseData(NormType.Signaling,
                                          "NoValidTrajectId",
                                          30000,
                                          30000)
                .SetName("Given171ProjectWithNorm{1}OfUnknownTraject_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase");
        }

        private static IEnumerable<TestCaseData> GetAllTrajectTestCaseData()
        {
            yield return new TestCaseData("1-1", 1000, 1000);
            yield return new TestCaseData("1-2", 1000, 1000);
            yield return new TestCaseData("2-1", 1000, 300);
            yield return new TestCaseData("2-2", 1000, 1000);
            yield return new TestCaseData("3-1", 3000, 3000);
            yield return new TestCaseData("3-2", 1000, 1000);
            yield return new TestCaseData("4-1", 300, 300);
            yield return new TestCaseData("4-2", 1000, 300);
            yield return new TestCaseData("5-1", 3000, 1000);
            yield return new TestCaseData("5-2", 3000, 3000);
            yield return new TestCaseData("6-1", 3000, 1000);
            yield return new TestCaseData("6-2", 3000, 1000);
            yield return new TestCaseData("6-3", 3000, 1000);
            yield return new TestCaseData("6-4", 3000, 1000);
            yield return new TestCaseData("6-5", 3000, 1000);
            yield return new TestCaseData("6-6", 3000, 1000);
            yield return new TestCaseData("6-7", 10000, 3000);
            yield return new TestCaseData("7-1", 3000, 1000);
            yield return new TestCaseData("7-2", 3000, 1000);
            yield return new TestCaseData("8-1", 30000, 10000);
            yield return new TestCaseData("8-2", 30000, 10000);
            yield return new TestCaseData("8-3", 30000, 10000);
            yield return new TestCaseData("8-4", 30000, 10000);
            yield return new TestCaseData("8-5", 3000, 1000);
            yield return new TestCaseData("8-6", 3000, 1000);
            yield return new TestCaseData("8-7", 3000, 1000);
            yield return new TestCaseData("9-1", 1000, 300);
            yield return new TestCaseData("9-2", 3000, 1000);
            yield return new TestCaseData("10-1", 3000, 1000);
            yield return new TestCaseData("10-2", 3000, 1000);
            yield return new TestCaseData("10-3", 10000, 3000);
            yield return new TestCaseData("11-1", 3000, 1000);
            yield return new TestCaseData("11-2", 3000, 1000);
            yield return new TestCaseData("11-3", 300, 100);
            yield return new TestCaseData("12-1", 1000, 1000);
            yield return new TestCaseData("12-2", 3000, 1000);
            yield return new TestCaseData("13-1", 3000, 1000);
            yield return new TestCaseData("13-2", 3000, 3000);
            yield return new TestCaseData("13-3", 3000, 1000);
            yield return new TestCaseData("13-4", 3000, 1000);
            yield return new TestCaseData("13-5", 3000, 1000);
            yield return new TestCaseData("13-6", 3000, 1000);
            yield return new TestCaseData("13-7", 3000, 1000);
            yield return new TestCaseData("13-8", 3000, 1000);
            yield return new TestCaseData("13-9", 3000, 1000);
            yield return new TestCaseData("13a-1", 300, 100);
            yield return new TestCaseData("13b-1", 300, 100);
            yield return new TestCaseData("14-1", 30000, 10000);
            yield return new TestCaseData("14-10", 30000, 30000);
            yield return new TestCaseData("14-2", 100000, 30000);
            yield return new TestCaseData("14-3", 10000, 10000);
            yield return new TestCaseData("14-4", 10000, 3000);
            yield return new TestCaseData("14-5", 30000, 10000);
            yield return new TestCaseData("14-6", 30000, 10000);
            yield return new TestCaseData("14-7", 30000, 10000);
            yield return new TestCaseData("14-8", 30000, 10000);
            yield return new TestCaseData("14-9", 30000, 30000);
            yield return new TestCaseData("15-1", 30000, 10000);
            yield return new TestCaseData("15-2", 10000, 3000);
            yield return new TestCaseData("15-3", 10000, 3000);
            yield return new TestCaseData("16-1", 100000, 30000);
            yield return new TestCaseData("16-2", 30000, 10000);
            yield return new TestCaseData("16-3", 30000, 10000);
            yield return new TestCaseData("16-4", 30000, 10000);
            yield return new TestCaseData("16-5", 10, 10); // Signaling norm set to LowerLimit
            yield return new TestCaseData("17-1", 3000, 1000);
            yield return new TestCaseData("17-2", 3000, 1000);
            yield return new TestCaseData("17-3", 100000, 30000);
            yield return new TestCaseData("18-1", 10000, 3000);
            yield return new TestCaseData("19-1", 100000, 30000);
            yield return new TestCaseData("20-1", 30000, 10000);
            yield return new TestCaseData("20-2", 10000, 10000);
            yield return new TestCaseData("20-3", 30000, 10000);
            yield return new TestCaseData("20-4", 1000, 300);
            yield return new TestCaseData("21-1", 3000, 1000);
            yield return new TestCaseData("21-2", 300, 100);
            yield return new TestCaseData("22-1", 3000, 1000);
            yield return new TestCaseData("22-2", 10000, 3000);
            yield return new TestCaseData("23-1", 3000, 1000);
            yield return new TestCaseData("24-1", 10000, 3000);
            yield return new TestCaseData("24-2", 1000, 300);
            yield return new TestCaseData("24-3", 10000, 10000);
            yield return new TestCaseData("25-1", 3000, 1000);
            yield return new TestCaseData("25-2", 1000, 300);
            yield return new TestCaseData("25-3", 300, 100);
            yield return new TestCaseData("25-4", 300, 300);
            yield return new TestCaseData("26-1", 3000, 1000);
            yield return new TestCaseData("26-2", 3000, 1000);
            yield return new TestCaseData("26-3", 10000, 3000);
            yield return new TestCaseData("26-4", 1000, 1000);
            yield return new TestCaseData("27-1", 3000, 3000);
            yield return new TestCaseData("27-2", 10000, 10000);
            yield return new TestCaseData("27-3", 3000, 1000);
            yield return new TestCaseData("27-4", 1000, 300);
            yield return new TestCaseData("28-1", 1000, 300);
            yield return new TestCaseData("29-1", 3000, 1000);
            yield return new TestCaseData("29-2", 10000, 3000);
            yield return new TestCaseData("29-3", 100000, 30000);
            yield return new TestCaseData("29-4", 1000, 1000);
            yield return new TestCaseData("30-1", 3000, 1000);
            yield return new TestCaseData("30-2", 100000, 100000);
            yield return new TestCaseData("30-3", 3000, 1000);
            yield return new TestCaseData("30-4", 1000000, 1000000);
            yield return new TestCaseData("31-1", 30000, 10000);
            yield return new TestCaseData("31-2", 10000, 3000);
            yield return new TestCaseData("31-3", 300, 100);
            yield return new TestCaseData("32-1", 1000, 300);
            yield return new TestCaseData("32-2", 1000, 300);
            yield return new TestCaseData("32-3", 3000, 1000);
            yield return new TestCaseData("32-4", 3000, 1000);
            yield return new TestCaseData("33-1", 300, 100);
            yield return new TestCaseData("34-1", 1000, 300);
            yield return new TestCaseData("34-2", 1000, 300);
            yield return new TestCaseData("34-3", 3000, 1000);
            yield return new TestCaseData("34-4", 1000, 300);
            yield return new TestCaseData("34-5", 300, 100);
            yield return new TestCaseData("34a-1", 3000, 1000);
            yield return new TestCaseData("35-1", 10000, 3000);
            yield return new TestCaseData("35-2", 3000, 1000);
            yield return new TestCaseData("36-1", 10000, 3000);
            yield return new TestCaseData("36-2", 30000, 10000);
            yield return new TestCaseData("36-3", 30000, 10000);
            yield return new TestCaseData("36-4", 10000, 3000);
            yield return new TestCaseData("36-5", 10000, 3000);
            yield return new TestCaseData("36a-1", 3000, 1000);
            yield return new TestCaseData("37-1", 10000, 3000);
            yield return new TestCaseData("38-1", 30000, 10000);
            yield return new TestCaseData("38-2", 10000, 3000);
            yield return new TestCaseData("39-1", 3000, 3000);
            yield return new TestCaseData("40-1", 30000, 30000);
            yield return new TestCaseData("40-2", 10000, 3000);
            yield return new TestCaseData("41-1", 30000, 10000);
            yield return new TestCaseData("41-2", 10000, 3000);
            yield return new TestCaseData("41-3", 3000, 3000);
            yield return new TestCaseData("41-4", 10000, 3000);
            yield return new TestCaseData("42-1", 10000, 3000);
            yield return new TestCaseData("43-1", 30000, 10000);
            yield return new TestCaseData("43-2", 10000, 3000);
            yield return new TestCaseData("43-3", 30000, 10000);
            yield return new TestCaseData("43-4", 30000, 10000);
            yield return new TestCaseData("43-5", 30000, 10000);
            yield return new TestCaseData("43-6", 30000, 10000);
            yield return new TestCaseData("44-1", 30000, 10000);
            yield return new TestCaseData("44-2", 300, 100);
            yield return new TestCaseData("44-3", 30000, 10000);
            yield return new TestCaseData("45-1", 100000, 30000);
            yield return new TestCaseData("45-2", 300, 100);
            yield return new TestCaseData("45-3", 300, 100);
            yield return new TestCaseData("46-1", 300, 100);
            yield return new TestCaseData("47-1", 3000, 1000);
            yield return new TestCaseData("48-1", 30000, 10000);
            yield return new TestCaseData("48-2", 10000, 3000);
            yield return new TestCaseData("48-3", 10000, 3000);
            yield return new TestCaseData("49-1", 300, 100);
            yield return new TestCaseData("49-2", 10000, 3000);
            yield return new TestCaseData("50-1", 30000, 10000);
            yield return new TestCaseData("50-2", 3000, 1000);
            yield return new TestCaseData("51-1", 1000, 300);
            yield return new TestCaseData("52-1", 3000, 1000);
            yield return new TestCaseData("52-2", 3000, 1000);
            yield return new TestCaseData("52-3", 3000, 1000);
            yield return new TestCaseData("52-4", 3000, 1000);
            yield return new TestCaseData("52a-1", 3000, 1000);
            yield return new TestCaseData("53-1", 3000, 1000);
            yield return new TestCaseData("53-2", 10000, 3000);
            yield return new TestCaseData("53-3", 10000, 3000);
            yield return new TestCaseData("54-1", 1000, 300);
            yield return new TestCaseData("55-1", 1000, 300);
            yield return new TestCaseData("56-1", 300, 100);
            yield return new TestCaseData("57-1", 300, 100);
            yield return new TestCaseData("58-1", 300, 100);
            yield return new TestCaseData("59-1", 300, 100);
            yield return new TestCaseData("60-1", 300, 100);
            yield return new TestCaseData("61-1", 300, 100);
            yield return new TestCaseData("63-1", 300, 100);
            yield return new TestCaseData("64-1", 300, 100);
            yield return new TestCaseData("65-1", 300, 100);
            yield return new TestCaseData("66-1", 300, 100);
            yield return new TestCaseData("67-1", 300, 100);
            yield return new TestCaseData("68-1", 1000, 300);
            yield return new TestCaseData("68-2", 300, 100);
            yield return new TestCaseData("69-1", 1000, 300);
            yield return new TestCaseData("70-1", 300, 100);
            yield return new TestCaseData("71-1", 300, 100);
            yield return new TestCaseData("72-1", 300, 100);
            yield return new TestCaseData("73-1", 300, 100);
            yield return new TestCaseData("74-1", 300, 100);
            yield return new TestCaseData("75-1", 300, 100);
            yield return new TestCaseData("76-1", 300, 100);
            yield return new TestCaseData("76-2", 300, 100);
            yield return new TestCaseData("76a-1", 300, 100);
            yield return new TestCaseData("77-1", 300, 100);
            yield return new TestCaseData("78-1", 300, 100);
            yield return new TestCaseData("78a-1", 300, 100);
            yield return new TestCaseData("79-1", 300, 100);
            yield return new TestCaseData("80-1", 300, 100);
            yield return new TestCaseData("81-1", 300, 100);
            yield return new TestCaseData("82-1", 300, 100);
            yield return new TestCaseData("83-1", 300, 100);
            yield return new TestCaseData("85-1", 300, 100);
            yield return new TestCaseData("86-1", 300, 100);
            yield return new TestCaseData("87-1", 1000, 300);
            yield return new TestCaseData("88-1", 300, 100);
            yield return new TestCaseData("89-1", 300, 100);
            yield return new TestCaseData("90-1", 3000, 1000);
            yield return new TestCaseData("91-1", 300, 300);
            yield return new TestCaseData("92-1", 300, 100);
            yield return new TestCaseData("93-1", 1000, 300);
            yield return new TestCaseData("94-1", 300, 100);
            yield return new TestCaseData("95-1", 300, 100);
            yield return new TestCaseData("201", 10000, 3000);
            yield return new TestCaseData("202", 10000, 3000);
            yield return new TestCaseData("204a", 10000, 3000);
            yield return new TestCaseData("204b", 1000, 300);
            yield return new TestCaseData("205", 3000, 1000);
            yield return new TestCaseData("206", 10000, 3000);
            yield return new TestCaseData("208", 100000, 30000);
            yield return new TestCaseData("209", 100000, 30000);
            yield return new TestCaseData("210", 100000, 30000);
            yield return new TestCaseData("211", 3000, 1000);
            yield return new TestCaseData("212", 10000, 3000);
            yield return new TestCaseData("213", 10000, 3000);
            yield return new TestCaseData("214", 3000, 1000);
            yield return new TestCaseData("215", 30000, 10000);
            yield return new TestCaseData("216", 3000, 1000);
            yield return new TestCaseData("217", 30000, 10000);
            yield return new TestCaseData("218", 30000, 10000);
            yield return new TestCaseData("219", 30000, 10000);
            yield return new TestCaseData("221", 10000, 3000);
            yield return new TestCaseData("222", 30000, 10000);
            yield return new TestCaseData("223", 30000, 10000);
            yield return new TestCaseData("224", 30000, 10000);
            yield return new TestCaseData("225", 30000, 10000);
            yield return new TestCaseData("226", 3000, 1000);
            yield return new TestCaseData("227", 3000, 1000);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var tables = new[]
            {
                "AssessmentSectionEntity",
                "CalculationGroupEntity",
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
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochasticSoilModelEntity",
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

        private static void AssertMacroStabilityInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 2) " +
                "FROM MacroStabilityInwardsFailureMechanismMetaEntity " +
                "WHERE A = 0.033 " +
                "AND FailureMechanismEntityId IN " +
                "(SELECT FailureMechanismEntityId FROM FailureMechanismEntity WHERE FailureMechanismType = 2);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
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

                Assert.AreEqual(49, messages.Count);
                var i = 0;
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSection'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De ondergrens is gelijk gesteld aan 1/30000."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De signaleringswaarde is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm)."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(NormType.Signaling)}."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '102'."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Golfklappen op asfaltbekleding'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '10' is veranderd naar '10004'."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Stabiliteit steenzetting'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '100' is veranderd naar '10006'."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '102'."),
                    messages[i++]);

                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'Demo traject'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De ondergrens is gelijk gesteld aan 1/1000."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De signaleringswaarde is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm)."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(NormType.Signaling)}."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);

                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'Empty'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De ondergrens is gelijk gesteld aan 1/1000."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De signaleringswaarde is gelijk gesteld aan 1/1000 (voorheen de waarde van de norm)."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(NormType.Signaling)}."),
                    messages[i++]);

                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSectionResults'"),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De ondergrens is gelijk gesteld aan 1/1000."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + De signaleringswaarde is gelijk gesteld aan 1/3000 (voorheen de waarde van de norm)."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(NormType.Signaling)}."),
                    messages[i++]);
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Macrostabiliteit binnenwaarts'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i]
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

        private static void AssertPipingSoilProfiles(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilProfileEntity) " +
                "FROM PipingSoilProfileEntity " +
                "WHERE [SourceType] IN (1,2);" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilLayerEntity) " +
                "FROM PipingSoilLayerEntity; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingStochasticSoilProfiles(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StochasticSoilProfileEntity) " +
                "FROM PipingStochasticSoilProfileEntity; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingCharacteristicPoints(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].CharacteristicPointEntity) " +
                "FROM PipingCharacteristicPointEntity; " +
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