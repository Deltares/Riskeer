// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Contribution;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo172IntegrationTest
    {
        private const string newVersion = "17.2";

        [Test]
        public void Given171Project_WhenUpgradedTo172_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core,
                                                               "MigrationTestProject171.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given171Project_WhenUpgradedTo172_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given171Project_WhenUpgradedTo172_ThenProjectAsExpected), ".log"));
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

                    AssertCalculationGroup(reader, sourceFilePath);

                    AssertClosingStructuresFailureMechanism(reader);
                    AssertClosingStructures(reader);
                    AssertDuneErosionFailureMechanism(reader);
                    AssertForeshoreProfiles(reader);

                    AssertGrassCoverErosionInwardsFailureMechanism(reader);
                    AssertGrassCoverErosionOutwardsFailureMechanism(reader);

                    AssertHeightStructuresFailureMechanism(reader);
                    AssertHeightStructures(reader);
                    AssertHydraulicBoundaryLocations(reader);
                    AssertMacroStabilityInwardsFailureMechanism(reader);

                    AssertPipingFailureMechanism(reader, sourceFilePath);
                    AssertPipingCharacteristicPoints(reader, sourceFilePath);
                    AssertPipingStochasticSoilProfiles(reader, sourceFilePath);
                    AssertPipingSoilProfiles(reader, sourceFilePath);
                    AssertPipingSoilLayers(reader, sourceFilePath);

                    AssertStabilityPointStructuresFailureMechanism(reader);
                    AssertStabilityPointStructures(reader);

                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);

                    AssertFailureMechanismSectionResults(reader, "ClosingStructuresSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "GrassCoverErosionInwardsSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "HeightStructuresSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "MacroStabilityInwardsSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "PipingSectionResultEntity");
                    AssertFailureMechanismSectionResults(reader, "StabilityPointStructuresSectionResultEntity");

                    AssertVersions(reader);
                    AssertDatabase(reader);
                }

                AssertLogDatabase(logFilePath);
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

            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(sourceFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                File.Copy(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core,
                                                     "Empty valid Release 17.1.rtd"),
                          sourceFilePath, true
                );

                var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
                using (var databaseFile = new ProjectDatabaseFile(sourceFilePath))
                {
                    databaseFile.OpenDatabaseConnection();

                    double originalNorm = 1.0 / (setNormType == NormType.Signaling
                                                     ? signalingReturnPeriod
                                                     : lowerLimitReturnPeriod);
                    databaseFile.ExecuteQuery("INSERT INTO ProjectEntity ([ProjectEntityId]) VALUES (1);");
                    databaseFile.ExecuteQuery("INSERT INTO AssessmentSectionEntity ([ProjectEntityId], [Composition], [Order], [Name], [Id], [Norm]) " +
                                              $"VALUES (1, 1, 0, \"{trajectId}\", \"{trajectId}\", {originalNorm});");
                }

                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                NormType expectedNormType = lowerLimitReturnPeriod == signalingReturnPeriod
                                                ? NormType.Signaling
                                                : setNormType;

                string expectedAssessmentSectionQuery = GetExpectedAssessmentSectionQuery(trajectId,
                                                                                          lowerLimitReturnPeriod,
                                                                                          signalingReturnPeriod,
                                                                                          expectedNormType);

                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    reader.AssertReturnedDataIsValid(expectedAssessmentSectionQuery);
                }

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(5, messages.Count);

                    var i = 0;
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[i++]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", $"* Traject: '{trajectId}'"),
                        messages[i++]);

                    AssertAssessmentSectionNormMigrationMessage(new[]
                    {
                        messages[i++],
                        messages[i++],
                        messages[i]
                    }, lowerLimitReturnPeriod, signalingReturnPeriod, expectedNormType);
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
            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(sourceFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                File.Copy(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core,
                                                     "Empty valid Release 17.1.rtd"),
                          sourceFilePath, true
                );

                var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
                using (var databaseFile = new ProjectDatabaseFile(sourceFilePath))
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
                int expectedLowerLimitReturnPeriod = expectedNormType == NormType.LowerLimit
                                                         ? originalReturnPeriod
                                                         : lowerLimitReturnPeriod;

                int expectedSignalingReturnPeriod = expectedNormType == NormType.Signaling
                                                        ? originalReturnPeriod
                                                        : signalingReturnPeriod;

                string expectedAssessmentSectionQuery = GetExpectedAssessmentSectionQuery(trajectId,
                                                                                          expectedLowerLimitReturnPeriod,
                                                                                          expectedSignalingReturnPeriod,
                                                                                          expectedNormType);

                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    reader.AssertReturnedDataIsValid(expectedAssessmentSectionQuery);
                }

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(5, messages.Count);

                    var i = 0;
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[i++]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "* Traject: '2-1'"),
                        messages[i++]);

                    AssertAssessmentSectionNormMigrationMessage(new[]
                    {
                        messages[i++],
                        messages[i++],
                        messages[i]
                    }, expectedLowerLimitReturnPeriod, expectedSignalingReturnPeriod, expectedNormType);
                }
            }
        }

        private static string GetExpectedAssessmentSectionQuery(string trajectId,
                                                                int lowerLimitReturnPeriod,
                                                                int signalingReturnPeriod,
                                                                NormType normType)
        {
            return "SELECT COUNT() = 1 " +
                   "FROM AssessmentSectionEntity " +
                   $"WHERE [Id] = \"{trajectId}\" " +
                   $"AND CAST(1.0 / [LowerLimitNorm] AS FLOAT) BETWEEN ({lowerLimitReturnPeriod} - 0.1) AND ({lowerLimitReturnPeriod} + 0.1) " +
                   $"AND CAST(1.0 / [SignalingNorm] AS FLOAT) BETWEEN ({signalingReturnPeriod} - 0.1) AND ({signalingReturnPeriod} + 0.1) " +
                   $"AND [NormativeNormType] = {Convert.ToByte(normType)}";
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

            IEnumerable<AssessmentSectionReturnPeriod> uniqueTrajectPeriods = GetAllTrajectTestCaseData()
                                                                              .GroupBy(t => Tuple.Create(t.SignalingReturnPeriod, t.LowerLimitPeriod))
                                                                              .Select(t => t.First());
            foreach (AssessmentSectionReturnPeriod data in uniqueTrajectPeriods)
            {
                foreach (NormType normType in normTypes)
                {
                    yield return new TestCaseData(normType,
                                                  data.Id,
                                                  data.SignalingReturnPeriod,
                                                  data.LowerLimitPeriod)
                        .SetName("Given171ProjectWithNorm{1}OfTraject{0}_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase");
                }
            }

            yield return new TestCaseData(NormType.Signaling,
                                          "NoValidTrajectId",
                                          30000,
                                          30000)
                .SetName("Given171ProjectWithNorm{1}OfUnknownTraject_WhenMigrated_ThenDatabaseUpdatedAndExpectedLogDatabase");
        }

        private static IEnumerable<AssessmentSectionReturnPeriod> GetAllTrajectTestCaseData()
        {
            yield return new AssessmentSectionReturnPeriod("1-1", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("1-2", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("2-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("2-2", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("3-1", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("3-2", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("4-1", 300, 300);
            yield return new AssessmentSectionReturnPeriod("4-2", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("5-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("5-2", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("6-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-4", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-5", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-6", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("6-7", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("7-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("7-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("8-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("8-2", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("8-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("8-4", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("8-5", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("8-6", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("8-7", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("9-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("9-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("10-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("10-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("10-3", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("11-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("11-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("11-3", 300, 100);
            yield return new AssessmentSectionReturnPeriod("12-1", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("12-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-2", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("13-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-4", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-5", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-6", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-7", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-8", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13-9", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("13a-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("13b-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("14-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-10", 30000, 30000);
            yield return new AssessmentSectionReturnPeriod("14-2", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("14-3", 10000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-4", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("14-5", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-6", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-7", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-8", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("14-9", 30000, 30000);
            yield return new AssessmentSectionReturnPeriod("15-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("15-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("15-3", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("16-1", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("16-2", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("16-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("16-4", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("16-5", 10, 10); // Signaling norm set to LowerLimit
            yield return new AssessmentSectionReturnPeriod("17-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("17-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("17-3", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("18-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("19-1", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("20-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("20-2", 10000, 10000);
            yield return new AssessmentSectionReturnPeriod("20-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("20-4", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("21-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("21-2", 300, 100);
            yield return new AssessmentSectionReturnPeriod("22-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("22-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("23-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("24-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("24-2", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("24-3", 10000, 10000);
            yield return new AssessmentSectionReturnPeriod("25-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("25-2", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("25-3", 300, 100);
            yield return new AssessmentSectionReturnPeriod("25-4", 300, 300);
            yield return new AssessmentSectionReturnPeriod("26-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("26-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("26-3", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("26-4", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("27-1", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("27-2", 10000, 10000);
            yield return new AssessmentSectionReturnPeriod("27-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("27-4", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("28-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("29-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("29-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("29-3", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("29-4", 1000, 1000);
            yield return new AssessmentSectionReturnPeriod("30-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("30-2", 100000, 100000);
            yield return new AssessmentSectionReturnPeriod("30-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("30-4", 1000000, 1000000);
            yield return new AssessmentSectionReturnPeriod("31-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("31-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("31-3", 300, 100);
            yield return new AssessmentSectionReturnPeriod("32-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("32-2", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("32-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("32-4", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("33-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("34-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("34-2", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("34-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("34-4", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("34-5", 300, 100);
            yield return new AssessmentSectionReturnPeriod("34a-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("35-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("35-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("36-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("36-2", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("36-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("36-4", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("36-5", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("36a-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("37-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("38-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("38-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("39-1", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("40-1", 30000, 30000);
            yield return new AssessmentSectionReturnPeriod("40-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("41-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("41-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("41-3", 3000, 3000);
            yield return new AssessmentSectionReturnPeriod("41-4", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("42-1", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("43-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("43-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("43-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("43-4", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("43-5", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("43-6", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("44-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("44-2", 300, 100);
            yield return new AssessmentSectionReturnPeriod("44-3", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("45-1", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("45-2", 300, 100);
            yield return new AssessmentSectionReturnPeriod("45-3", 300, 100);
            yield return new AssessmentSectionReturnPeriod("46-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("47-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("48-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("48-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("48-3", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("49-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("49-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("50-1", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("50-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("51-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("52-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("52-2", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("52-3", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("52-4", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("52a-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("53-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("53-2", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("53-3", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("54-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("55-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("56-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("57-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("58-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("59-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("60-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("61-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("63-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("64-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("65-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("66-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("67-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("68-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("68-2", 300, 100);
            yield return new AssessmentSectionReturnPeriod("69-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("70-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("71-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("72-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("73-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("74-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("75-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("76-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("76-2", 300, 100);
            yield return new AssessmentSectionReturnPeriod("76a-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("77-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("78-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("78a-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("79-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("80-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("81-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("82-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("83-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("85-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("86-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("87-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("88-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("89-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("90-1", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("91-1", 300, 300);
            yield return new AssessmentSectionReturnPeriod("92-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("93-1", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("94-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("95-1", 300, 100);
            yield return new AssessmentSectionReturnPeriod("201", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("202", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("204a", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("204b", 1000, 300);
            yield return new AssessmentSectionReturnPeriod("205", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("206", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("208", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("209", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("210", 100000, 30000);
            yield return new AssessmentSectionReturnPeriod("211", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("212", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("213", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("214", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("215", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("216", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("217", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("218", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("219", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("221", 10000, 3000);
            yield return new AssessmentSectionReturnPeriod("222", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("223", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("224", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("225", 30000, 10000);
            yield return new AssessmentSectionReturnPeriod("226", 3000, 1000);
            yield return new AssessmentSectionReturnPeriod("227", 3000, 1000);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AssessmentSectionEntity",
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

        private static void AssertCalculationGroup(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string expectedNrOfCalculationGroupsQuery =
                "SELECT " +
                "COUNT() + " +
                "(" +
                "SELECT " +
                "COUNT() " +
                "FROM [SOURCEPROJECT].FailureMechanismEntity " +
                "WHERE [FailureMechanismType] = 2" +
                ") " +
                "FROM [SOURCEPROJECT].CalculationGroupEntity ";

            string validateMigratedTable =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                $"SELECT COUNT() = ({expectedNrOfCalculationGroupsQuery})" +
                "FROM CalculationGroupEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedTable);
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
                "LEFT JOIN [ClosingStructureEntity] USING([FailureMechanismEntityId]) " +
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
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [ClosingStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [ClosingStructuresCalculationEntity] " +
                "WHERE [ShouldIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [ClosingStructuresOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertClosingStructures(MigratedDatabaseReader reader)
        {
            const string validateClosingStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM [ClosingStructureEntity] " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateClosingStructures);
        }

        private static void AssertDuneErosionFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertForeshoreProfiles(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfiles =
                "SELECT COUNT(DISTINCT([Id])) IS COUNT() " +
                "FROM [ForeshoreProfileEntity] " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateForeshoreProfiles);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsCalculationEntity] " +
                "WHERE [ShouldDikeHeightIllustrationPointsBeCalculated] != 0 AND " +
                "[ShouldOvertoppingRateIllustrationPointsBeCalculated] != 0 AND " +
                "[ShouldOvertoppingOutputIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity] " +
                "JOIN [GrassCoverErosionInwardsOutputEntity] " +
                "JOIN [GrassCoverErosionInwardsOvertoppingRateOutputEntity];";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [GrassCoverErosionOutwardsFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionOutwardsHydraulicLocationEntity] " +
                "WHERE [ShouldWaveHeightIllustrationPointsBeCalculated] != 0 " +
                "|| [ShouldDesignWaterLevelIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionOutwardsHydraulicLocationOutputEntity] " +
                "JOIN [GrassCoverErosionOutwardsWaveConditionsOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
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
                "LEFT JOIN [HeightStructureEntity] USING([FailureMechanismEntityId]) " +
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
                "FROM [HeightStructuresCalculationEntity] " +
                "WHERE [ShouldIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [HeightStructuresOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertHeightStructures(MigratedDatabaseReader reader)
        {
            const string validateHeightStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM [HeightStructureEntity] " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateHeightStructures);
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [HydraulicLocationEntity] " +
                "WHERE [ShouldWaveHeightIllustrationPointsBeCalculated] != 0 " +
                "|| [ShouldDesignWaterLevelIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [HydraulicLocationOutputEntity]";
            reader.AssertReturnedDataIsValid(validateOutputs);
        }

        private static void AssertMacroStabilityInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 2) " +
                "FROM [MacroStabilityInwardsFailureMechanismMetaEntity] " +
                "WHERE [A] = 0.033 " +
                "AND [StochasticSoilModelCollectionSourcePath] IS NULL " +
                "AND [SurfaceLineCollectionSourcePath] IS NULL " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 2);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateMacroStabilityInwardsFailureMechanismCalculationGroup =
                "SELECT COUNT() = 0 " +
                "FROM [FailureMechanismEntity] " +
                "WHERE [FailureMechanismType] = 2 " +
                "AND [CalculationGroupEntityId] IS NULL";
            reader.AssertReturnedDataIsValid(validateMacroStabilityInwardsFailureMechanismCalculationGroup);
        }

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [PipingCalculationOutputEntity] " +
                "LEFT JOIN [PipingSemiProbabilisticOutputEntity] USING(PipingCalculationEntityId) " +
                "WHERE [PipingCalculationEntityId] IN( " +
                "SELECT [PipingCalculationEntityId] " +
                "FROM [PipingCalculationEntity] " +
                "WHERE [UseAssessmentLevelManualInput] IS 0);";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);

            string validateCalculationOutputsWithManualInput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = ( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].PipingCalculationOutputEntity " +
                "LEFT JOIN [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity USING(PipingCalculationEntityId) " +
                "WHERE [PipingCalculationEntityId] IN( " +
                "SELECT [PipingCalculationEntityId] " +
                "FROM [SOURCEPROJECT].PipingCalculationEntity " +
                "WHERE [UseAssessmentLevelManualInput] IS 1 " +
                ") " +
                ") " +
                "FROM [PipingCalculationOutputEntity] " +
                "LEFT JOIN [PipingSemiProbabilisticOutputEntity] USING(PipingCalculationEntityId) " +
                "WHERE [PipingCalculationEntityId] IN( " +
                "SELECT [PipingCalculationEntityId] " +
                "FROM [PipingCalculationEntity] " +
                "WHERE [UseAssessmentLevelManualInput] IS 1);" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationOutputsWithManualInput);
        }

        private static void AssertPipingCharacteristicPoints(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].CharacteristicPointEntity) " +
                "FROM [PipingCharacteristicPointEntity]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingStochasticSoilProfiles(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StochasticSoilProfileEntity) " +
                "FROM [PipingStochasticSoilProfileEntity]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingSoilProfiles(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilProfiles =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilProfileEntity) " +
                "FROM [PipingSoilProfileEntity] " +
                "WHERE [SourceType] IN (1,2);" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilProfiles);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilLayers =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilLayerEntity) " +
                "FROM [PipingSoilLayerEntity]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilLayers);

            string validateSoilLayerColors =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilLayerEntity WHERE [Color] = 0) " +
                "FROM [PipingSoilLayerEntity] " +
                "WHERE [Color] IS NULL;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilLayerColors);
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
                "LEFT JOIN [StabilityPointStructureEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateStructuresCollectionSourcePath);

            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS [IsInvalid] " +
                "FROM [StabilityPointStructuresFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityPointStructuresCalculationEntity] " +
                "WHERE [ShouldIllustrationPointsBeCalculated] != 0;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityPointStructuresOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertStabilityPointStructures(MigratedDatabaseReader reader)
        {
            const string validateStabilityPointStructures =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM [StabilityPointStructureEntity] " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateStabilityPointStructures);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [StabilityStoneCoverFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverWaveConditionsOutputEntity]";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfileCollectionSourcePath =
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM (" +
                "SELECT " +
                "CASE WHEN " +
                "COUNT([ForeshoreProfileEntityId]) AND [ForeshoreProfileCollectionSourcePath] IS NULL " +
                "OR " +
                "[ForeshoreProfileCollectionSourcePath] IS NOT NULL AND NOT COUNT([ForeshoreProfileEntityId]) " +
                "THEN 1 ELSE 0 END AS[IsInvalid] " +
                "FROM [WaveImpactAsphaltCoverFailureMechanismMetaEntity] " +
                "LEFT JOIN [ForeshoreProfileEntity] USING([FailureMechanismEntityId]) " +
                "GROUP BY [FailureMechanismEntityId]);";
            reader.AssertReturnedDataIsValid(validateForeshoreProfileCollectionSourcePath);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverWaveConditionsOutputEntity]";

            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertAssessmentSectionNormMigrationMessage(MigrationLogMessage[] messages,
                                                                        int lowerLimitReturnPeriod,
                                                                        int signalingReturnPeriod,
                                                                        NormType normType)
        {
            Assert.AreEqual(3, messages.Length);

            string lowerLimitLogSuffix = normType == NormType.LowerLimit
                                             ? " (voorheen de waarde van de norm)"
                                             : "";
            MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, $"  + De ondergrens is gelijk gesteld aan 1/{lowerLimitReturnPeriod}{lowerLimitLogSuffix}."),
                messages[0]);
            string signalingLogSuffix = normType == NormType.Signaling
                                            ? " (voorheen de waarde van de norm)"
                                            : "";
            MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, $"  + De signaleringswaarde is gelijk gesteld aan 1/{signalingReturnPeriod}{signalingLogSuffix}."),
                messages[1]);
            MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, $"  + De norm van het dijktraject is gelijk gesteld aan de {GetNormTypeString(normType)}."),
                messages[2]);
        }

        private static void AssertFailureMechanismSectionResults(MigratedDatabaseReader reader, string sectionResultTable)
        {
            string validateFailureMechanismSectionResults =
                "SELECT COUNT() = 0 " +
                $"FROM {sectionResultTable} " +
                "WHERE [LayerThree] < 0 OR [LayerThree] > 1";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        private static void AssertLayerThreeMigrationMessage(MigrationLogMessage[] messages)
        {
            Assert.AreEqual(2, messages.Length);

            MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, "    - Het geregistreerde resultaat voor de toets op maat in 'Vak 1' ('5.0') kon niet worden geconverteerd naar een geldige kans en is verwijderd."),
                messages[0]);
            MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                new MigrationLogMessage("17.1", newVersion, "    - Het geregistreerde resultaat voor de toets op maat in 'Vak 2' ('-10.0') kon niet worden geconverteerd naar een geldige kans en is verwijderd."),
                messages[1]);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(50, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Alle berekende resultaten zijn verwijderd, behalve die van het toetsspoor 'Piping' waarbij de waterstand handmatig is ingevuld."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSection'"),
                    messages[i++]);
                AssertAssessmentSectionNormMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++],
                    messages[i++]
                }, 30000, 30000, NormType.Signaling);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '102'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Golfklappen op asfaltbekleding'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '10' is veranderd naar '10000000000000000000004'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Stabiliteit steenzetting'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van voorlandprofiel '100' is veranderd naar '10000000000000000000006'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '1' is veranderd naar '103'."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'Demo traject'"),
                    messages[i++]);
                AssertAssessmentSectionNormMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++],
                    messages[i++]
                }, 1000, 30000, NormType.Signaling);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "    - Het ID van kunstwerk '10' is veranderd naar '104'."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'Empty'"),
                    messages[i++]);
                AssertAssessmentSectionNormMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++],
                    messages[i++]
                }, 1000, 1000, NormType.Signaling);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "* Traject: 'assessmentSectionResults'"),
                    messages[i++]);
                AssertAssessmentSectionNormMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++],
                    messages[i++]
                }, 1000, 3000, NormType.Signaling);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Macrostabiliteit binnenwaarts'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i++]
                });
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.1", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'"),
                    messages[i++]);
                AssertLayerThreeMigrationMessage(new[]
                {
                    messages[i++],
                    messages[i]
                });
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
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
        /// Class for combination of an assessment section id with signaling and lower limit return period.
        /// </summary>
        private class AssessmentSectionReturnPeriod
        {
            /// <summary>
            /// Creates a new instance of <see cref="AssessmentSectionReturnPeriod"/>.
            /// </summary>
            /// <param name="id">The assessment section identifier.</param>
            /// <param name="signalingReturnPeriod">The signaling return period.</param>
            /// <param name="lowerLimitPeriod">The lower limit return period.</param>
            public AssessmentSectionReturnPeriod(string id, int signalingReturnPeriod, int lowerLimitPeriod)
            {
                Id = id;
                SignalingReturnPeriod = signalingReturnPeriod;
                LowerLimitPeriod = lowerLimitPeriod;
            }

            /// <summary>
            /// Gets the identifier of the assessment section.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Gets the signaling return period which has been defined on the assessment section.
            /// </summary>
            public int SignalingReturnPeriod { get; }

            /// <summary>
            /// Gets the lower limit return period which has been defined on the assessment section.
            /// </summary>
            public int LowerLimitPeriod { get; }
        }
    }
}