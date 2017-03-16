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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Utils;

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
                call, "Bronprojectpad moet een geldig projectpad zijn.").ParamName;
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
            MigrationRequired shouldMigrate = MigrationRequired.No;
            Action call = () => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            var expectedLogMessages = new List<Tuple<string, LogLevelConstant>>();
            if (!confirmContinuation)
            {
                expectedLogMessages.Add(Tuple.Create($"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.",
                                                     LogLevelConstant.Warn));
            }
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessages, expectedLogMessages.Count);

            var expectedResult = confirmContinuation ?
                                     MigrationRequired.Yes :
                                     MigrationRequired.Aborted;
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
        public void DetermineMigrationLocation_OriginalFilePathNull_ThrownArgumentNullException()
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
                call, "Bronprojectpad moet een geldig projectpad zijn.").ParamName;
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
        [TestCase(null)]
        [TestCase("")]
        public void DetermineMigrationLocation_TargetFilePathIsEmpty_LogsMessageAndReturnsEmptyTargetPath(string targetPath)
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
                         .Return(targetPath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            string targetFilePath = "arbitraryPath";

            // Call
            Action call = () => targetFilePath = migrator.DetermineMigrationLocation(validFilePath);

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create($"Het migreren van het projectbestand '{validFilePath}' is geannuleerd.",
                                                                               LogLevelConstant.Warn);

            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);

            Assert.AreEqual(targetPath, targetFilePath);
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
                call, "Bronprojectpad moet een geldig projectpad zijn.").ParamName;
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
                call, "Doelprojectpad moet een geldig projectpad zijn.").ParamName;
            Assert.AreEqual("targetFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessFullyMigrates()
        {
            // Given
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessFullyMigrates)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            bool migrationSuccessful = false;

            // When 
            Action call = () => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

            // Then
            string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' (versie {currentDatabaseVersion}).";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);

            Assert.IsTrue(migrationSuccessful);

            var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
            Assert.AreEqual(currentDatabaseVersion, toVersionedFile.GetVersion());

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

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Assert
            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                fileDisposeHelper.LockFiles();

                bool migrationSuccessful = true;

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

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            bool migrationSuccessful = true;

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

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            bool migrationSuccessful = true;

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

            mocks.VerifyAll();
        }
    }
}