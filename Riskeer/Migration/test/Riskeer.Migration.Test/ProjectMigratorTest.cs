﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.TestUtil.Settings;
using Core.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Util;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Test
{
    [TestFixture]
    public class ProjectMigratorTest
    {
        private const string testDirectory = nameof(ProjectMigratorTest);
        private readonly string currentDatabaseVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
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
            void Call() => new ProjectMigrator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_ReturnsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var migrator = new ProjectMigrator(inquiryHelper);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            // Call
            void Call() => migrator.ShouldMigrate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            // Call
            void Call() => migrator.ShouldMigrate(invalidFilePath);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                Call, "Bronprojectpad moet een geldig projectpad zijn.");
            Assert.AreEqual("filePath", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldMigrate_OutdatedProjectUnsupported_ReturnsNotSupportedAndGeneratesLogMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            var versionedFile = new ProjectVersionedFile(sourceFilePath);
            string fileVersion = versionedFile.GetVersion();

            var migrator = new ProjectMigrator(inquiryHelper);
            var shouldMigrate = MigrationRequired.Yes;

            // Call
            void Call() => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            var expectedMessage = $"Het migreren van een projectbestand met versie '{fileVersion}' naar versie '{currentDatabaseVersion}' is niet ondersteund.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
            Assert.AreEqual(MigrationRequired.NotSupported, shouldMigrate);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldMigrate_OutdatedProjectSupported_AskMigrationConfirmationAndReturnBasedOnConfirmation(bool confirmContinuation)
        {
            // Setup
            string question = "Het project dat u wilt openen is opgeslagen in het formaat van een eerdere versie van Riskeer of Ringtoets." +
                              $"{Environment.NewLine}{Environment.NewLine}" +
                              $"Weet u zeker dat u het bestand wilt migreren naar het formaat van uw huidige Riskeerversie ({currentDatabaseVersion})?";
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(question)).Return(confirmContinuation);
            mocks.ReplayAll();

            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            var migrator = new ProjectMigrator(inquiryHelper);

            // Call
            var shouldMigrate = MigrationRequired.No;
            void Call() => shouldMigrate = migrator.ShouldMigrate(sourceFilePath);

            // Assert
            var expectedLogMessages = new List<Tuple<string, LogLevelConstant>>();
            if (!confirmContinuation)
            {
                expectedLogMessages.Add(Tuple.Create($"Het migreren van het projectbestand '{sourceFilePath}' is geannuleerd.",
                                                     LogLevelConstant.Warn));
            }

            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedLogMessages, expectedLogMessages.Count);

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

            string sourceFilePath = ProjectMigrationTestHelper.GetLatestProjectFilePath();

            var migrator = new ProjectMigrator(inquiryHelper);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            // Call
            void Call() => migrator.DetermineMigrationLocation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("originalFilePath", exception.ParamName);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            // Call
            void Call() => migrator.DetermineMigrationLocation(invalidFilePath);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                Call, "Bronprojectpad moet een geldig projectpad zijn.");
            Assert.AreEqual("originalFilePath", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void DetermineMigrationLocation_ValidPath_AsksUserForTargetPathAndReturnsIt()
        {
            // Setup
            const string originalFileName = "Im_a_valid_file_path";
            const string expectedFileExtension = "risk";

            string validFilePath = TestHelper.GetScratchPadPath($"{originalFileName}.{expectedFileExtension}");

            string versionWithDashes = ProjectVersionHelper.GetCurrentDatabaseVersion().Replace('.', '-');
            var expectedFileFilter = new FileFilterGenerator(expectedFileExtension, "Riskeer project");
            string expectedSuggestedFileName = $"{originalFileName}_{versionWithDashes}";

            string expectedReturnPath = TestHelper.GetScratchPadPath("Im_a_file_path_to_the_migrated_file.risk");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.GetTargetFileLocation(expectedFileFilter.Filter, expectedSuggestedFileName))
                         .Return(expectedReturnPath);
            mocks.ReplayAll();

            var migrator = new ProjectMigrator(inquiryHelper);

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
            const string expectedFileExtension = "risk";

            string validFilePath = TestHelper.GetScratchPadPath($"{originalFileName}.{expectedFileExtension}");

            var expectedFileFilter = new FileFilterGenerator(expectedFileExtension, "Riskeer project");
            string versionWithDashes = ProjectVersionHelper.GetCurrentDatabaseVersion().Replace('.', '-');
            var expectedSuggestedFileName = $"{originalFileName}_{versionWithDashes}";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.GetTargetFileLocation(expectedFileFilter.Filter, expectedSuggestedFileName))
                         .Return(null);
            mocks.ReplayAll();

            var migrator = new ProjectMigrator(inquiryHelper);
            var targetFilePath = "arbitraryPath";

            // Call
            void Call() => targetFilePath = migrator.DetermineMigrationLocation(validFilePath);

            // Assert
            var expectedLogMessage = Tuple.Create($"Het migreren van het projectbestand '{validFilePath}' is geannuleerd.",
                                                  LogLevelConstant.Warn);

            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, expectedLogMessage, 1);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            string targetFileName = $"{nameof(ProjectMigratorTest)}." +
                                    $"{nameof(Migrate_SourcePathNull_ThrowsArgumentNullException)}.rtd";
            string targetFilePath = TestHelper.GetScratchPadPath(targetFileName);

            // Call
            void Call() => migrator.Migrate(null, targetFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void Migrate_TargetPathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new ProjectMigrator(inquiryHelper);

            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            // Call
            void Call() => migrator.Migrate(sourceFilePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("targetFilePath", exception.ParamName);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Migrate_InvalidSourceFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var migrator = new ProjectMigrator(inquiryHelper);

            string targetFileName = $"{nameof(ProjectMigratorTest)}." +
                                    $"{nameof(Migrate_InvalidSourceFilePath_ThrowsArgumentException)}.rtd";
            string targetFilePath = TestHelper.GetScratchPadPath(targetFileName);

            // Call
            void Call() => migrator.Migrate(invalidFilePath, targetFilePath);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                Call, "Bronprojectpad moet een geldig projectpad zijn.");
            Assert.AreEqual("sourceFilePath", exception.ParamName);

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

            var migrator = new ProjectMigrator(inquiryHelper);

            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            // Call
            void Call() => migrator.Migrate(sourceFilePath, invalidFilePath);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                Call, "Doelprojectpad moet een geldig projectpad zijn.");
            Assert.AreEqual("targetFilePath", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates()
        {
            // Given
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            string targetFile = $"{nameof(ProjectMigratorTest)}." +
                                $"{nameof(GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var logDirectory = $"{nameof(GivenMigratorAndSupportedFile_WhenValidTargetLocationGiven_ThenFileSuccessfullyMigrates)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new ProjectMigrator(inquiryHelper);

                var migrationSuccessful = false;

                // When 
                void Call() => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Then
                string expectedMessage = $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' " +
                                         $"(versie {currentDatabaseVersion}).";
                var migrationLog = new StringBuilder();
                migrationLog.AppendLine("Door de migratie is het project aangepast. Bekijk het migratierapport door op details te klikken.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 16.4 naar versie 17.1:");
                migrationLog.AppendLine("* Alle berekende resultaten zijn verwijderd.");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'");
                migrationLog.AppendLine("    - De naam van dijkprofiel '1' is veranderd naar '102' en wordt ook gebruikt als ID.");
                migrationLog.AppendLine("    - De naam van dijkprofiel '10' is veranderd naar '104' en wordt ook gebruikt als ID.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 17.1 naar versie 17.2:");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + De omgevingswaarde is gelijk gesteld aan 1/30000.");
                migrationLog.AppendLine("  + De signaleringsparameter is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm).");
                migrationLog.AppendLine("  + De norm van het traject is gelijk gesteld aan de signaleringsparameter.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Hoogte kunstwerk'");
                migrationLog.AppendLine("    - Het ID van kunstwerk 'Id' is veranderd naar 'Id00003'.");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP7'.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Betrouwbaarheid sluiting kunstwerk'");
                migrationLog.AppendLine("    - Het ID van kunstwerk 'id' is veranderd naar 'id00002'.");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP8'.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Golfklappen op asfaltbekleding'");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP9'.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie buitentalud'");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP10'.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Stabiliteit steenzetting'");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP11'.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit puntconstructies'");
                migrationLog.AppendLine("    - Het ID van kunstwerk 'anId' is veranderd naar 'anId000000002'.");
                migrationLog.AppendLine("    - Het ID van voorlandprofiel 'FP' is veranderd naar 'FP12'.");
                migrationLog.AppendLine("* Traject: 'Demo traject'");
                migrationLog.AppendLine("  + De omgevingswaarde is gelijk gesteld aan 1/1000.");
                migrationLog.AppendLine("  + De signaleringsparameter is gelijk gesteld aan 1/30000 (voorheen de waarde van de norm).");
                migrationLog.AppendLine("  + De norm van het traject is gelijk gesteld aan de signaleringsparameter.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 17.2 naar versie 17.3:");
                migrationLog.AppendLine("* Geen aanpassingen.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 17.3 naar versie 18.1:");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping'");
                migrationLog.AppendLine("    - De waarde '3.2' voor de verschuiving van parameter 'Verzadigd gewicht' van ondergrondlaag 'HotPinkLayer' is ongeldig en is veranderd naar NaN.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit langsconstructies'");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Technische innovaties'");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Wateroverdruk bij asfaltbekleding'");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Betrouwbaarheid sluiting kunstwerk'");
                migrationLog.AppendLine("    - De waarde van '0' van parameter 'Aantal identieke doorstroomopeningen' van berekening 'Nieuwe berekening' is ongeldig en is veranderd naar 1.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Macrostabiliteit buitenwaarts'");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Golfklappen op asfaltbekleding'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie buitentalud'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven binnentalud'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven buitentalud'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Microstabiliteit'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping bij kunstwerk'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Stabiliteit steenzetting'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Duinafslag'");
                migrationLog.AppendLine("    - Alle resultaten voor de gedetailleerde toets van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("    - Alle resultaten voor de toets op maat van dit faalmechanisme konden niet worden omgezet naar een geldig resultaat en zijn verwijderd.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 18.1 naar versie 19.1:");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie.");
                migrationLog.AppendLine("  + De waarde voor de transparantie van de achtergrondkaart is aangepast naar 0.60.");
                migrationLog.AppendLine("* Traject: 'Demo traject'");
                migrationLog.AppendLine("  + Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie.");
                migrationLog.AppendLine("  + De waarde voor de transparantie van de achtergrondkaart is aangepast naar 0.60.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 19.1 naar versie 21.1:");
                migrationLog.AppendLine("* Geen aanpassingen.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 21.1 naar versie 22.1:");
                migrationLog.AppendLine("* De oorspronkelijke faalmechanismen zijn omgezet naar het nieuwe formaat.\r\n* Alle toetsoordelen zijn verwijderd.");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'");
                migrationLog.AppendLine("    - De waarden van de doelkans voor HBN en overslagdebiet zijn veranderd naar de trajectnorm.");
                migrationLog.AppendLine("* Traject: 'Demo traject'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'");
                migrationLog.AppendLine("    - De waarden van de doelkans voor HBN en overslagdebiet zijn veranderd naar de trajectnorm.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 22.1 naar versie 23.1:");
                migrationLog.AppendLine("* Traject: 'assessmentSection'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Hoogte kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Wateroverdruk bij asfaltbekleding'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Betrouwbaarheid sluiting kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Macrostabiliteit binnenwaarts'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Golfklappen op asfaltbekleding'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie buitentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven binnentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven buitentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Microstabiliteit'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping bij kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Stabiliteit steenzetting'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Duinafslag'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit puntconstructies'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Macrostabiliteit buitenwaarts'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit langsconstructies'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Technische innovaties'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("* Traject: 'Demo traject'");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Hoogte kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Wateroverdruk bij asfaltbekleding'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Betrouwbaarheid sluiting kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Macrostabiliteit binnenwaarts'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Golfklappen op asfaltbekleding'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding erosie buitentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven binnentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Grasbekleding afschuiven buitentalud'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Microstabiliteit'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Piping bij kunstwerk'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Stabiliteit steenzetting'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Duinafslag'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit puntconstructies'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Macrostabiliteit buitenwaarts'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Sterkte en stabiliteit langsconstructies'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("  + Faalmechanisme: 'Technische innovaties'");
                migrationLog.AppendLine("    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 23.1 naar versie 24.1:");
                migrationLog.AppendLine("* Omdat alleen faalkansen op vakniveau een rol spelen in de assemblage, zijn de assemblageresultaten voor de faalmechanismen aangepast:");
                migrationLog.AppendLine("  + De initiële faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Handmatig invullen'.");
                migrationLog.AppendLine("  + De aangescherpte faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Per doorsnede' of 'Beide'.");
                migrationLog.AppendLine("  + De assemblagemethode 'Automatisch berekenen o.b.v. slechtste doorsnede of vak' is vervangen door 'Automatisch berekenen o.b.v. slechtste vak'.");
                migrationLog.AppendLine("* Voor HLCD bestanden waarbij geen tabel 'ScenarioInformation' aanwezig is, worden niet langer standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie.");
                migrationLog.AppendLine("Gevolgen van de migratie van versie 24.1 naar versie 24.2:");
                migrationLog.AppendLine("* Geen aanpassingen.");

                Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
                {
                    Tuple.Create(expectedMessage, LogLevelConstant.Info),
                    Tuple.Create(migrationLog.ToString(), LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedLogMessagesAndLevel, 2);

                Assert.IsTrue(migrationSuccessful);

                var toVersionedFile = new ProjectVersionedFile(targetFilePath);
                Assert.AreEqual(currentDatabaseVersion, toVersionedFile.GetVersion());
            }

            string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RiskeerMigrationLog.sqlite");
            Assert.IsFalse(File.Exists(logPath));

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFile = $"{nameof(ProjectMigratorTest)}." +
                                $"{nameof(Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var logDirectory = $"{nameof(Migrate_MigrationLogDatabaseInUse_MigrationFailsAndLogsError)}_log";

            string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RiskeerMigrationLog.sqlite");

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            using (var fileDisposeHelper = new FileDisposeHelper(logPath))
            {
                var migrator = new ProjectMigrator(inquiryHelper);
                fileDisposeHelper.LockFiles();

                var migrationSuccessful = true;

                // Call 
                void Call() => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                var logMessage = Tuple.Create(
                    $"Het is niet mogelijk om het Riskeer logbestand '{logPath}' aan te maken.",
                    LogLevelConstant.Error);
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, logMessage);
                Assert.IsFalse(migrationSuccessful);

                Assert.IsTrue(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFile = $"{nameof(ProjectMigratorTest)}." +
                                $"{nameof(Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError)}.rtd";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var logDirectory = $"{nameof(Migrate_UnableToSaveAtTargetFilePath_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                var migrator = new ProjectMigrator(inquiryHelper);

                fileDisposeHelper.LockFiles();

                var migrationSuccessful = true;

                // Call 
                void Call() => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RiskeerMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            string targetFile = $"{nameof(ProjectMigratorTest)}." +
                                $"{nameof(Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError)}";
            string targetFilePath = Path.Combine(TestHelper.GetScratchPadPath(), testDirectory, targetFile);

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var logDirectory = $"{nameof(Migrate_UnsupportedSourceFileVersion_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new ProjectMigrator(inquiryHelper);

                var migrationSuccessful = true;

                // Call 
                void Call() => migrationSuccessful = migrator.Migrate(sourceFilePath, targetFilePath);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(logDirectory), "RiskeerMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Migrate_TargetFileSameAsSourceFile_MigrationFailsAndLogsError()
        {
            // Setup
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var logDirectory = $"{nameof(Migrate_TargetFileSameAsSourceFile_MigrationFailsAndLogsError)}_log";
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), logDirectory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                TempPath = TestHelper.GetScratchPadPath(logDirectory)
            }))
            {
                var migrator = new ProjectMigrator(inquiryHelper);

                var migrationSuccessful = true;

                // Call 
                void Call() => migrationSuccessful = migrator.Migrate(sourceFilePath, sourceFilePath);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    StringAssert.StartsWith($"Het migreren van het projectbestand '{sourceFilePath}' is mislukt: ", msgs[0]);
                });
                Assert.IsFalse(migrationSuccessful);

                string logPath = Path.Combine(TestHelper.GetScratchPadPath(), logDirectory, "RiskeerMigrationLog.sqlite");
                Assert.IsFalse(File.Exists(logPath));
            }

            mocks.VerifyAll();
        }
    }
}