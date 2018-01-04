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
using System.IO;
using System.Linq;
using System.Text;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.TestUtil.Settings;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Util;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsProjectMigratorTest
    {
        private const string testDirectory = nameof(RingtoetsProjectMigratorTest);
        private readonly string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
        private DirectoryDisposeHelper directoryDisposeHelper;

        [SetUp]
        public void Setup()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            directoryDisposeHelper.Dispose();
        }

        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsProjectMigrator(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("inquiryHelper", paramName);
        }

        [Test]
        public void Constructor_ReturnsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IMigrateProject>(migrator);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            TestDelegate call = () => migrator.ShouldMigrate(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void ShouldMigrate_InvalidFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            TestDelegate call = () => migrator.ShouldMigrate(invalidFilePath);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             call, "Bronprojectpad moet een geldig projectpad zijn.")
                                         .ParamName;
            Assert.AreEqual("filePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_OutdatedProjectUnsupported_ReturnsAbortedAndGeneratesLogMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            string fileVersion = versionedFile.GetVersion();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            var shouldMigrate = MigrationRequired.Yes;

            // Call
            Action call = () => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            string expectedMessage = $"Het migreren van een projectbestand met versie '{fileVersion}' naar versie '{currentDatabaseVersion}' is niet ondersteund.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.AreEqual(MigrationRequired.Aborted, shouldMigrate);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldMigrate_OutdatedProjectSupported_AskMigrationConfirmationAndReturnBasedOnConfirmation(bool confirmContinuation)
        {
            // Setup
            string question = "Het project dat u wilt openen is opgeslagen in het formaat van een eerdere versie van Ringtoets." +
                              $"{Environment.NewLine}{Environment.NewLine}" +
                              $"Weet u zeker dat u het bestand wilt migreren naar het formaat van uw huidige Ringtoetsversie ({currentDatabaseVersion})?";
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(question)).Return(confirmContinuation);
            mocks.ReplayAll();

            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            var shouldMigrate = MigrationRequired.No;
            Action call = () => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            var expectedLogMessages = new List<Tuple<string, LogLevelConstant>>();
            if (!confirmContinuation)
            {
                expectedLogMessages.Add(Tuple.Create($"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.",
                                                     LogLevelConstant.Warn));
            }

            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessages, expectedLogMessages.Count);

            MigrationRequired expectedResult = confirmContinuation ? MigrationRequired.Yes : MigrationRequired.Aborted;
            Assert.AreEqual(expectedResult, shouldMigrate);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_LatestProjectVersion_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetLatestProjectFilePath();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            MigrationRequired shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            Assert.AreEqual(MigrationRequired.No, shouldMigrate);
            mocks.VerifyAll();
        }

        [Test]
        public void DetermineMigrationLocation_OriginalFilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            TestDelegate call = () => migrator.DetermineMigrationLocation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("originalFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void DetermineMigrationLocation_InvalidOriginalFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            TestDelegate call = () => migrator.DetermineMigrationLocation(invalidFilePath);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             call, "Bronprojectpad moet een geldig projectpad zijn.")
                                         .ParamName;
            Assert.AreEqual("originalFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void DetermineMigrationLocation_ValidPath_AsksUserForTargetPathAndReturnsIt()
        {
            // Setup
            const string originalFileName = "Im_a_valid_file_path";
            const string expectedFileExtension = "rtd";

            string validFilePath = TestHelper.GetScratchPadPath($"{originalFileName}.{expectedFileExtension}");

            string versionWithDashes = RingtoetsVersionHelper.GetCurrentDatabaseVersion().Replace('.', '-');
            var expectedFileFilter = new FileFilterGenerator(expectedFileExtension, "Ringtoets project");
            string expectedSuggestedFileName = $"{originalFileName}_{versionWithDashes}";

            string expectedReturnPath = TestHelper.GetScratchPadPath("Im_a_file_path_to_the_migrated_file.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.GetTargetFileLocation(expectedFileFilter.Filter, expectedSuggestedFileName))
                         .Return(expectedReturnPath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            string targetFilePath = migrator.DetermineMigrationLocation(validFilePath);

            // Assert
            Assert.AreEqual(expectedReturnPath, targetFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void DetermineMigrationLocation_TargetFilePathIsEmpty_LogsMessageAndReturnsEmptyTargetPath()
        {
            // Setup
            const string originalFileName = "Im_a_valid_file_path";
            const string expectedFileExtension = "rtd";

            string validFilePath = TestHelper.GetScratchPadPath($"{originalFileName}.{expectedFileExtension}");

            var expectedFileFilter = new FileFilterGenerator(expectedFileExtension, "Ringtoets project");
            string versionWithDashes = RingtoetsVersionHelper.GetCurrentDatabaseVersion().Replace('.', '-');
            string expectedSuggestedFileName = $"{originalFileName}_{versionWithDashes}";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.GetTargetFileLocation(expectedFileFilter.Filter, expectedSuggestedFileName))
                         .Return(null);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            var targetFilePath = "arbitraryPath";

            // Call
            Action call = () => targetFilePath = migrator.DetermineMigrationLocation(validFilePath);

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create($"Het migreren van het projectbestand '{validFilePath}' is geannuleerd.",
                                                                              LogLevelConstant.Warn);

            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);

            Assert.IsNull(targetFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            string targetFileName = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                    $"{nameof(Migrate_SourcePathNull_ThrowsArgumentNullException)}.rtd";
            string targetFilePath = TestHelper.GetScratchPadPath(targetFileName);

            // Call
            TestDelegate call = () => migrator.Migrate(null, targetFilePath);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void Migrate_TargetPathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            // Call
            TestDelegate call = () => migrator.Migrate(sourceFilePath, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("targetFilePath", paramName);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Migrate_InvalidSourceFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            string targetFileName = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                    $"{nameof(Migrate_InvalidSourceFilePath_ThrowsArgumentException)}.rtd";
            string targetFilePath = TestHelper.GetScratchPadPath(targetFileName);

            // Call
            TestDelegate call = () => migrator.Migrate(invalidFilePath, targetFilePath);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             call, "Bronprojectpad moet een geldig projectpad zijn.")
                                         .ParamName;
            Assert.AreEqual("sourceFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Migrate_InvalidTargetFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            // Call
            TestDelegate call = () => migrator.Migrate(sourceFilePath, invalidFilePath);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             call, "Doelprojectpad moet een geldig projectpad zijn.")
                                         .ParamName;
            Assert.AreEqual("targetFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates()
        {
            // Given
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string logDirectory = $"{nameof(GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new RingtoetsProjectMigrator(inquiryHelper);

                var migrationSuccessful = false;

                // When 
                Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Then
                string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' " +
                                         $"(versie {currentDatabaseVersion}).";
                var migrationLog = new StringBuilder();
                migrationLog.AppendLine(@"Door de migratie is het project aangepast. Bekijk het migratierapport door op details te klikken.");
                migrationLog.AppendLine(@"Gevolgen van de migratie van versie 16.4 naar versie 17.1:");
                migrationLog.AppendLine($@"* Alle berekende resultaten zijn verwijderd.{Environment.NewLine}" +
                                        $@"* Traject: 'assessmentSection'{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Grasbekleding erosie kruin en binnentalud'{Environment.NewLine}" +
                                        $@"    - De naam van dijkprofiel '1' is veranderd naar '102' en wordt ook gebruikt als ID.{Environment.NewLine}" +
                                        $@"    - De naam van dijkprofiel '10' is veranderd naar '104' en wordt ook gebruikt als ID.{Environment.NewLine}" +
                                        $@"Gevolgen van de migratie van versie 17.1 naar versie 17.2:{Environment.NewLine}" +
                                        $@"* Traject: 'assessmentSection'{Environment.NewLine}" +
                                        $@"  + De ondergrens is gelijk gesteld aan 1/30000.{Environment.NewLine}" +
                                        $@"  + De signaleringswaarde is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm).{Environment.NewLine}" +
                                        $@"  + De norm van het dijktraject is gelijk gesteld aan de signaleringswaarde.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Hoogte kunstwerk'{Environment.NewLine}" +
                                        $@"    - Het ID van kunstwerk 'Id' is veranderd naar 'Id00003'.{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP7'.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'{Environment.NewLine}" +
                                        $@"    - Het ID van kunstwerk 'id' is veranderd naar 'id00002'.{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP8'.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Golfklappen op asfaltbekleding'{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP9'.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Grasbekleding erosie buitentalud'{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP10'.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Stabiliteit steenzetting'{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP11'.{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Sterkte en stabiliteit puntconstructies'{Environment.NewLine}" +
                                        $@"    - Het ID van kunstwerk 'anId' is veranderd naar 'anId000000002'.{Environment.NewLine}" +
                                        $@"    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP12'.{Environment.NewLine}" +
                                        $@"* Traject: 'Demo traject'{Environment.NewLine}" +
                                        $@"  + De ondergrens is gelijk gesteld aan 1/1000.{Environment.NewLine}" +
                                        $@"  + De signaleringswaarde is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm).{Environment.NewLine}" +
                                        $@"  + De norm van het dijktraject is gelijk gesteld aan de signaleringswaarde.{Environment.NewLine}" +
                                        $@"Gevolgen van de migratie van versie 17.2 naar versie 17.3:{Environment.NewLine}" +
                                        $@"* Geen aanpassingen.{Environment.NewLine}" +
                                        $@"Gevolgen van de migratie van versie 17.3 naar versie 18.1:{Environment.NewLine}" +
                                        $@"* Traject: 'assessmentSection'{Environment.NewLine}" +
                                        $@"  + Toetsspoor: 'Piping'{Environment.NewLine}" +
                                        $@"    - De waarde '3.2' voor de verschuiving van parameter 'Verzadigd gewicht' van ondergrondlaag 'HotPinkLayer' is ongeldig en is veranderd naar NaN.");

                var expectedLogMessagesAndLevel = new[]
                {
                    Tuple.Create(expectedMessage, LogLevelConstant.Info),
                    Tuple.Create(migrationLog.ToString(), LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 2);

                Assert.IsTrue(migrationSuccessful);

                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(currentDatabaseVersion, toVersionedFile.GetVersion());
            }

            string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RingtoetsMigrationLog.sqlite");
            Assert.IsFalse(File.Exists(logPath));

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string logDirectory = $"{nameof(Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError)}_log";

            string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RingtoetsMigrationLog.sqlite");

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            using (var fileDisposeHelper = new FileDisposeHelper(logPath))
            {
                var migrator = new RingtoetsProjectMigrator(inquiryHelper);
                fileDisposeHelper.LockFiles();

                var migrationSuccessful = true;

                // Call 
                Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                Tuple<string, LogLevelConstant> logMessage = Tuple.Create(
                    $"Het is niet mogelijk om het Ringtoets logbestand '{logPath}' aan te maken.",
                    LogLevelConstant.Error);
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, logMessage);
                Assert.IsFalse(migrationSuccessful);

                Assert.IsTrue(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string logDirectory = $"{nameof(Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                var migrator = new RingtoetsProjectMigrator(inquiryHelper);

                fileDisposeHelper.LockFiles();

                var migrationSuccessful = true;

                // Call 
                Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RingtoetsMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError)}";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string logDirectory = $"{nameof(Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new RingtoetsProjectMigrator(inquiryHelper);

                var migrationSuccessful = true;

                // Call 
                Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(logDirectory), "RingtoetsMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_TargetFileSameAsSourceFile_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string logDirectory = $"{nameof(Migrate_TargetFileSameAsSourceFile_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new RingtoetsProjectMigrator(inquiryHelper);

                var migrationSuccessful = true;

                // Call 
                Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, sourceFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RingtoetsMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }
    }
}