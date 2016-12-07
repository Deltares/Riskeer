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
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.HydraRing;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");
        private HydraulicBoundaryDatabaseImporter importer;

        [SetUp]
        public void SetUp()
        {
            importer = new HydraulicBoundaryDatabaseImporter();
        }

        [TearDown]
        public void TearDown()
        {
            importer.Dispose();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call is done in SetUp

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryDatabaseImporter>(importer);
            Assert.IsInstanceOf<IDisposable>(importer);
        }

        [Test]
        public void Import_ExistingFile_DoesNotThrowException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_NonExistingFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "nonexisting.sqlite");
            var expectedExceptionMessage = string.Format("Fout bij het lezen van bestand '{0}': het bestand bestaat niet.", filePath);

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, filePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            var invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, invalidPath);

            // Assert
            var expectedMessage = new FileReaderErrorMessageBuilder(invalidPath)
                .Build(string.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                     string.Join(", ", Path.GetInvalidFileNameChars())));
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileIsDirectory_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "/");
            var expectedExceptionMessage = string.Format("Fout bij het lezen van bestand '{0}': bestandspad mag niet verwijzen naar een lege bestandsnaam.", filePath);

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, filePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutHlcd_ThrowCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath).Build("Het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.");
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutSettings_ThrowCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "withoutSettings", "complete.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath).Build(string.Format(
                "Kon het rekeninstellingen bestand niet openen. Fout bij het lezen van bestand '{0}': het bestand bestaat niet.",
                HydraulicDatabaseHelper.GetHydraulicBoundarySettingsDatabase(validFilePath)));
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithInvalidSettingsDatabaseSchema_ThrowCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath).Build(
                "Kon het rekeninstellingen bestand niet openen. De rekeninstellingen database heeft niet het juiste schema.");
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportHydraulicBoundaryLocationsToCollectionAndAssessmentSectionNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());
            mocks.ReplayAll();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                              "completeWithLocationsToBeFilteredOut.sqlite");

            // Precondition
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(assessmentSection, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                StringAssert.EndsWith("De hydraulische randvoorwaardenlocaties zijn ingelezen.", messageArray[0]);
            });
            Assert.IsTrue(importResult);
            ICollection<HydraulicBoundaryLocation> importedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(9, importedLocations.Count);
            CollectionAssert.AllItemsAreNotNull(importedLocations);
            CollectionAssert.AllItemsAreUnique(importedLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToSameDatabaseOnDifferentPath_FilePathUpdatedAndAssessmentSectionNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Twice();
            mocks.ReplayAll();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                              "completeWithLocationsToBeFilteredOut.sqlite");
            string copyValidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                                  "copyOfCompleteWithLocationsToBeFilteredOut.sqlite");

            // Precondition
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            importer.Import(assessmentSection, validFilePath);

            // Call
            var importResult = importer.Import(assessmentSection, copyValidFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(copyValidFilePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
            ICollection<HydraulicBoundaryLocation> importedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(9, importedLocations.Count);
            CollectionAssert.AllItemsAreNotNull(importedLocations);
            CollectionAssert.AllItemsAreUnique(importedLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToSameDatabaseOnSamePath_AssessmentSectionNotNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());
            mocks.ReplayAll();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                              "completeWithLocationsToBeFilteredOut.sqlite");

            // Precondition
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            importer.Import(assessmentSection, validFilePath);

            // Call
            var importResult = importer.Import(assessmentSection, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(validFilePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
            ICollection<HydraulicBoundaryLocation> importedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(9, importedLocations.Count);
            CollectionAssert.AllItemsAreNotNull(importedLocations);
            CollectionAssert.AllItemsAreUnique(importedLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingFileWithCorruptSchema_AbortAndLog()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var expectedLogMessage = string.Format("Fout bij het lezen van bestand '{0}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database. Het bestand wordt overgeslagen.", corruptPath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(assessmentSection, corruptPath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase, "No HydraulicBoundaryDatabase object should be created when import from corrupt database.");

            mocks.VerifyAll();
        }

        [Test]
        public void Import_CorruptSchemaFile_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers()).Repeat.Never();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(assessmentSection, validFilePath);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath)
                .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database. Het bestand wordt overgeslagen.");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }
    }
}