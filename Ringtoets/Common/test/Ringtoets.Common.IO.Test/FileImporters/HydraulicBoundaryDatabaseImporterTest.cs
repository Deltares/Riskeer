// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.HydraRing;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(HydraulicBoundaryDatabaseImporter));
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
            // Assert
            Assert.IsInstanceOf<IDisposable>(importer);
        }

        [Test]
        public void Import_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => importer.Import(null, validFilePath);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Import_NonExistingFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "doesNotExist.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedExceptionMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet.";
            Assert.AreEqual(expectedExceptionMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            string invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, invalidPath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(invalidPath)
                .Build("Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileIsDirectory_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "/");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedExceptionMessage = $"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutHlcd_ThrowCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath).Build("Het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutSettings_ThrowCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "withoutSettings", "complete.sqlite");

            // Call
            TestDelegate test = () => importer.Import(assessmentSection, validFilePath);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(validFilePath).Build($"Kon het rekeninstellingen bestand niet openen. Fout bij het lezen van bestand '{HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(validFilePath)}': het bestand bestaat niet.");
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFile_DataImported()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(assessmentSection, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                StringAssert.EndsWith("De hydraulische belastingenlocaties zijn ingelezen.", messageArray[0]);
            });
            Assert.IsTrue(importResult);
            IEnumerable<HydraulicBoundaryLocation> importedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(18, importedLocations.Count());
            CollectionAssert.AllItemsAreNotNull(importedLocations);
            CollectionAssert.AllItemsAreUnique(importedLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingFileWithCorruptSchema_ReturnsFalseAndLogsErrorMessages()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            string corruptPath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(assessmentSection, corruptPath);

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Assert.AreEqual(1, messages.Count());
                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(0);

                Assert.AreEqual(Level.Error, expectedLog.Item2);

                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<LineParseException>(loggedException);
                Assert.AreEqual(loggedException.Message, expectedLog.Item1);
            });
            Assert.IsFalse(importResult);
            Assert.IsFalse(assessmentSection.HydraulicBoundaryDatabase.IsLinked(), "No HydraulicBoundaryDatabase object should be created when import from corrupt database.");

            mocks.VerifyAll();
        }
    }
}