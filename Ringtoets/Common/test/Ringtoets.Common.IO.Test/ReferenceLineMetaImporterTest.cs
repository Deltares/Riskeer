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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ReferenceLineMetaImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "SomeFolder");
            string invalidFilePath = validFilePath.Replace("F", invalidFileNameChars[1].ToString());

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathTooLong_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToEmptyFolder = Path.Combine(testDataPath, new string('A', 260));

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(pathToEmptyFolder);

            // Assert
            var expectedExceptionMessage = string.Format("Fout bij het lezen van bestand '{0}': Ongeldig pad.",
                                                         pathToEmptyFolder);
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void Constructor_EmptyFolder_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToEmptyFolder = Path.Combine(testDataPath, "EmptyFolder");

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(pathToEmptyFolder);

            // Assert
            var expectedExceptionMessage = string.Format("Fout bij het lezen van bestand '{0}\\*.shp': Er is geen shape file gevonden.",
                                                         pathToEmptyFolder);
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void Constructor_FolderWithTwoShapeFiles_LogsWarningAndTakesFirstShapeFile()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "TwoShapeFiles");

            // Call
            var referenceIds = new List<string>();
            Action call = () =>
            {
                var importer = new ReferenceLineMetaImporter(pathToFolder);
                referenceIds.AddRange(importer.GetAssessmentSectionIds());
            };

            // Assert
            var expectedMessage = string.Format("Er zijn meerdere shape files gevonden in '{0}'. 'NBPW_A.shp' is gekozen.", pathToFolder);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.AreEqual(1, referenceIds.Count);
            Assert.AreEqual("A", referenceIds.First());
        }

        [Test]
        public void GetReferenceLineIds_ValidDirectoryWithOneShapeFile_ReturnsReadReferenceIds()
        {
            // Setup
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathValidFolder);

            // Call
            var referenceIds = importer.GetAssessmentSectionIds().ToArray();

            // Assert
            Assert.AreEqual(10, referenceIds.Length);
            var expectedReferenceIds = new[]
            {
                "1-1",
                "2-2",
                "3-3",
                "4-4",
                "5-5",
                "6-6",
                "7-7",
                "8-8",
                "9-9",
                "10-10",
            };
            Assert.AreEqual(expectedReferenceIds, referenceIds);
        }

        [Test]
        public void Import_IncorrectReferenceId_LogsWarningAndReturnsFalse()
        {
            // Setup
            MockRepository mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            string pathToValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathToValidFolder);
            const string invalidReferenceId = "B";
            var referecenLineContext = new ReferenceLineContext(assessmentSectionMock);

            // Call
            Action call = () => importer.Import(referecenLineContext, invalidReferenceId);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': De geselecteerde referentielijn '{1}' is niet gevonden.",
                                                Path.Combine(pathToValidFolder, "validShapeFile.shp")
                                                , invalidReferenceId);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ValidReferenceId_ImportsReferenceLineAndRetunsTrue()
        {
            // Setup
            MockRepository mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            string pathToValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathToValidFolder);
            const string validReferenceId = "1-1";
            var referecenLineContext = new ReferenceLineContext(assessmentSectionMock);

            // Call
            bool importsuccesful = importer.Import(referecenLineContext, validReferenceId);

            // Assert
            Assert.True(importsuccesful);
            Assert.AreEqual(validReferenceId, assessmentSectionMock.Id);
            mockRepository.VerifyAll();
        }
    }
}