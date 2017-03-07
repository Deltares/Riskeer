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
using System.IO;
using System.Linq;
using Application.Ringtoets.Migration.Core;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Utils;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsProjectMigratorTest : NUnitFormTest
    {
        private readonly string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

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
        public void ShouldMigrate_SourcePathNull_ThrowsArgumentNullException()
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
        [TestCase("")]
        [TestCase("    ")]
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
            Assert.AreEqual("sourceFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_OutdatedProjectUnsupported_ReturnsFalseAndGeneratesLogMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            const string file = "UnsupportedVersion8.rtd";
            const string fileVersion = "8";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, file);

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            bool shouldMigrate = true;

            // Call
            Action call = () => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            string expectedMessage = $"Het migreren van een projectbestand met versie '{fileVersion}' naar versie '{currentDatabaseVersion}' is niet ondersteund.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsFalse(shouldMigrate);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_OutdatedProjectSupported_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            const string file = "FullTestProject164.rtd";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, file);

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            bool shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            Assert.IsTrue(shouldMigrate);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_LatestProjectVersion_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            const string currentProjectVersionFile = "FullTestProject171.rtd";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, currentProjectVersionFile);

            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            string testProjectVersion = versionedFile.GetVersion();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Pre-condition
            string assertionMessage = $"Database version {testProjectVersion} of the testproject must match with the current database version {currentDatabaseVersion}.";
            Assert.AreEqual(currentDatabaseVersion, testProjectVersion, assertionMessage);

            // Call
            bool shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            Assert.IsFalse(shouldMigrate);
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

            // Call
            TestDelegate call = () => migrator.Migrate(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        public void Migrate_InvalidFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call
            TestDelegate call = () => migrator.Migrate(invalidFilePath);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                call, "Bronprojectpad moet een geldig projectpad zijn.").ParamName;
            Assert.AreEqual("sourceFilePath", paramName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Migrate_Always_DisplaysInquiryMessage(bool confirmMigration)
        {
            // Setup
            string sourceFilePath = "Arbitrary/RingtoetsFile";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen in het formaat van een eerdere " +
                             "versie van Ringtoets. Weet u zeker dat u het bestand wilt migreren naar het formaat van" +
                             $" uw huidige Ringtoetsversie ({currentDatabaseVersion})?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(confirmMigration);
            inquiryHelper.Stub(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(null);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call 
            migrator.Migrate(sourceFilePath);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_Always_ReturnsSuggestedFileNameAndFileFilter()
        {
            // Setup
            const string projectName = "FullTestProject164";
            string sourceFilePath = $"Some/Path/{projectName}.rtd";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);

            string versionSuffix = currentDatabaseVersion.Replace(".", "-");
            string expectedSuggestedFileName = $"{projectName}_{versionSuffix}";
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(new FileFilterGenerator("rtd", "Ringtoets project"),
                                                                        expectedSuggestedFileName))
                         .Return(null);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Call 
            migrator.Migrate(sourceFilePath);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGiven_ThenFileSuccessFullyMigrates()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGiven_ThenFileSuccessFullyMigrates)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(targetFilePath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (new FileDisposeHelper(targetFilePath))
            {
                string actualTargetFilePath = null;

                // When 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Then
                string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' (versie {currentDatabaseVersion}).";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(targetFilePath, actualTargetFilePath);
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(currentDatabaseVersion, toVersionedFile.GetVersion());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenDiscontinuedAfterInquiry_ThenFileMigrationCancelledAndLogsMessage()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(false);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            string actualTargetFilePath = string.Empty;

            // When
            Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

            // Then
            string expectedMessage = $"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsNull(actualTargetFilePath);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryButCancelledSaveFileDialog_ThenFileMigrationCancelledAndLogsMessage()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(null);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            string actualTargetFilePath = string.Empty;

            // When
            Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

            // Then
            string expectedMessage = $"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsNull(actualTargetFilePath);

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            string targetFilePath = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                    $"{nameof(Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError)}.rtd";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(sourceFilePath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Assert
            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                fileDisposeHelper.LockFiles();

                string actualTargetFilePath = string.Empty;

                // Call 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith(string.Format("Het migreren van het projectbestand '{0}' is mislukt: ", sourceFilePath), msgs[0]);
                });
                Assert.IsNull(actualTargetFilePath);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "UnsupportedVersion8.rtd");
            string targetFile = $"{nameof(RingtoetsProjectMigratorTest)}." +
                                $"{nameof(Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError)}";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(targetFilePath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (new FileDisposeHelper(targetFilePath))
            {
                string actualTargetFilePath = string.Empty;

                // Call 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith(string.Format("Het migreren van het projectbestand '{0}' is mislukt: ", sourceFilePath), msgs[0]);
                });
                Assert.IsNull(actualTargetFilePath);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_TargetFileSameAsSourceFile_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(sourceFilePath);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            string actualTargetFilePath = string.Empty;

            // Call 
            Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Het migreren van het projectbestand '{0}' is mislukt: ", sourceFilePath), msgs[0]);
            });
            Assert.IsNull(actualTargetFilePath);

            mocks.VerifyAll();
        }
    }
}