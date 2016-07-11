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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ReferenceLineMetaImporterTest : NUnitFormsAssertTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowsArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowsArgumentException()
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
            var expectedExceptionMessage = string.Format("Fout bij het lezen van bestand '{0}': De folder locatie is ongeldig.",
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
            Action call = () => new ReferenceLineMetaImporter(pathToFolder);

            // Assert
            var expectedMessage = string.Format("Er zijn meerdere shape files gevonden in '{0}'. 'NBPW_A.shp' is gekozen.", pathToFolder);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
        }

        [Test]
        public void GetReferenceLineMetas_InvalidShapeFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToEmptyFolder = Path.Combine(testDataPath, "InvalidShapeFile");
            ReferenceLineMetaImporter importer = new ReferenceLineMetaImporter(pathToEmptyFolder);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            // Assert
            var expectedExceptionMessage = "Het bestand heeft de attributen 'TRAJECT_ID', 'NORM_SW', 'NORM_OG' niet. Deze attributen zijn vereist.";
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_FileWithNonUniqueTrajectIds_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "NonUniqueTrajectIds");
            ReferenceLineMetaImporter importer = new ReferenceLineMetaImporter(pathToFolder);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Assert
            var shapeFile = Path.Combine(pathToFolder, "NonUniqueTrajectIds.shp");
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': De trajectid's zijn niet uniek.", shapeFile);
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_FileWithEmptyTrajectIds_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "EmptyTrackId");
            ReferenceLineMetaImporter importer = new ReferenceLineMetaImporter(pathToFolder);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Assert
            var shapeFile = Path.Combine(pathToFolder, "EmptyTrackId.shp");
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': De trajectid's zijn niet allemaal ingevuld.", shapeFile);
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_ValidDirectoryWithOneShapeFile_ReturnsReadReadGetReferenceLineMetas()
        {
            // Setup
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathValidFolder);

            // Call
            var referenceLineMetas = importer.GetReferenceLineMetas().ToArray();

            // Assert
            Assert.AreEqual(10, referenceLineMetas.Length);
            var expectedAssessmentSectionIds = new[]
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
            var assessmentSectionIds = referenceLineMetas.Select(rlm => rlm.AssessmentSectionId);
            Assert.AreEqual(expectedAssessmentSectionIds, assessmentSectionIds);
        }

        [Test]
        public void GetReferenceLineMetas_EmptyShapeFile_ReturnsEmptyList()
        {
            // Setup
            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");
            var importer = new ReferenceLineMetaImporter(pathValidFolder);

            // Call
            IEnumerable<ReferenceLineMeta> referenceIds = importer.GetReferenceLineMetas();

            // Assert
            Assert.AreEqual(0, referenceIds.Count());
        }
    }
}