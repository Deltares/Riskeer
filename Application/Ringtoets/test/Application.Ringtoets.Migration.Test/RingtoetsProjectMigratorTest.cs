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
using System.Threading;
using Application.Ringtoets.Migration.Core;
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
        private const string testDirectory = nameof(RingtoetsProjectMigratorTest);

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
            // TODO: introduce the IMigrateProject in Core.Common.Base
            Assert.IsInstanceOf<object>(migrator);

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
            Assert.AreEqual("sourceFilePath", paramName);

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

            var expectedVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            bool shouldMigrate = true;

            // Call
            Action call = () => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            string expectedMessage = $"Het migreren van een projectbestand met versie '{fileVersion}' naar versie '{expectedVersion}' is niet ondersteund.";
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
            string currentVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            // Pre-condition
            var assertionMessage = $"Database version {testProjectVersion} of the testproject must match with the current database version {currentVersion}.";
            Assert.AreEqual(currentVersion, testProjectVersion, assertionMessage);

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
            Assert.AreEqual("sourceFilePath", paramName);
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
        [Apartment(ApartmentState.STA)]
        public void GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGivenWithoutExtension_ThenFileSuccessFullyMigratesAndExtensionAdded()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            string targetFile = $"{nameof(GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGivenWithoutExtension_ThenFileSuccessFullyMigratesAndExtensionAdded)}";
            string targetFilePath = TestHelper.GetScratchPadPath(Path.Combine(testDirectory, targetFile));
            string expectedVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen met een oudere " +
                             "versie van Ringtoets. Wilt u het bestand converteren naar uw " +
                             "huidige Ringtoetsversie?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(true);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), testDirectory))
            {
                string actualTargetFilePath = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new SaveFileDialogTester(wnd);
                    helper.SaveFile(targetFilePath);
                };

                // When 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Then
                string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}.rtd' (versie {expectedVersion}).";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);

                string expectedFilePathWithExtension = $"{targetFilePath}.rtd";
                Assert.AreEqual(expectedFilePathWithExtension, actualTargetFilePath);
                var toVersionedFile = new RingtoetsVersionedFile(expectedFilePathWithExtension);
                Assert.AreEqual(expectedVersion, toVersionedFile.GetVersion());
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGiven_ThenFileSuccessFullyMigrates()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            string targetFile = $"{nameof(GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryAndValidTargetLocationGiven_ThenFileSuccessFullyMigrates)}.rtd";
            string targetFilePath = TestHelper.GetScratchPadPath(Path.Combine(testDirectory, targetFile));
            string expectedVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen met een oudere " +
                             "versie van Ringtoets. Wilt u het bestand converteren naar uw " +
                             "huidige Ringtoetsversie?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(true);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), testDirectory))
            {
                string actualTargetFilePath = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new SaveFileDialogTester(wnd);
                    helper.SaveFile(targetFilePath);
                };

                // When 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Then
                string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' (versie {expectedVersion}).";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);

                Assert.AreEqual(targetFilePath, actualTargetFilePath);
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(expectedVersion, toVersionedFile.GetVersion());
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMigratorAndSupportedFile_WhenDiscontinuedAfterInquiry_ThenFileNotMigrated()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen met een oudere " +
                             "versie van Ringtoets. Wilt u het bestand converteren naar uw " +
                             "huidige Ringtoetsversie?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(false);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            string actualTargetFilePath = string.Empty;

            // Call
            Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

            // Then
            string expectedMessage = $"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsNull(actualTargetFilePath);

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMigratorAndSupportedFile_WhenContinuedAfterInquiryButCancelledSaveFileDialog_ThenFileNotMigrated()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen met een oudere " +
                             "versie van Ringtoets. Wilt u het bestand converteren naar uw " +
                             "huidige Ringtoetsversie?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(true);
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new SaveFileDialogTester(wnd);
                helper.ClickCancel();
            };

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);
            string actualTargetFilePath = string.Empty;

            // Call
            Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

            // Then
            string expectedMessage = $"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsNull(actualTargetFilePath);

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Migrate_UnsupportedSourceFileVersion_ThenLogsError()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "UnsupportedVersion8.rtd");
            string targetFile = $"{nameof(Migrate_UnsupportedSourceFileVersion_ThenLogsError)}";
            string targetFilePath = TestHelper.GetScratchPadPath(Path.Combine(testDirectory, targetFile));

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            string message = "Het project dat u wilt openen is opgeslagen met een oudere " +
                             "versie van Ringtoets. Wilt u het bestand converteren naar uw " +
                             "huidige Ringtoetsversie?";
            inquiryHelper.Expect(helper => helper.InquireContinuation(message)).Return(true);
            mocks.ReplayAll();

            var migrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), testDirectory))
            {
                string actualTargetFilePath = string.Empty;

                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new SaveFileDialogTester(wnd);
                    helper.SaveFile(targetFilePath);
                };

                // When 
                Action call = () => actualTargetFilePath = migrator.Migrate(sourceFilePath);

                // Then
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
    }
}