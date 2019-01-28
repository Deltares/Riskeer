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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Base.TestUtil.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.ReferenceLines;

namespace Ringtoets.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineMetaImporterTest
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
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowsArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "SomeFolder");
            string invalidFilePath = validFilePath.Replace('F', invalidPathChars.Last());

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathTooLong_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathTooLong = Path.Combine(testDataPath, new string('A', 260));

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(pathTooLong);

            // Assert
            string expectedExceptionMessage = $"De map met specificaties voor trajecten '{pathTooLong}' is niet gevonden.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void Constructor_FilePathDoesNotExist_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToNonExistingFolder = Path.Combine(testDataPath, "I do not exist");

            // Precondition
            Assert.IsFalse(Directory.Exists(pathToNonExistingFolder));

            // Call
            TestDelegate call = () => new ReferenceLineMetaImporter(pathToNonExistingFolder);

            // Assert
            string expectedExceptionMessage = $"De map met specificaties voor trajecten '{pathToNonExistingFolder}' is niet gevonden.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void Constructor_EmptyFolder_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToEmptyFolder = Path.Combine(TestHelper.GetScratchPadPath(), "EmptyReferenceLineFolder");

            // Call
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), pathToEmptyFolder))
            {
                TestDelegate call = () => new ReferenceLineMetaImporter(pathToEmptyFolder);

                // Assert
                string expectedExceptionMessage = $@"Geen shapebestand om trajecten te specificeren gevonden in de map '{pathToEmptyFolder}'.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedExceptionMessage, exception.Message);
            }
        }

        [Test]
        public void Constructor_FolderWithTwoShapeFiles_LogsWarningAndTakesFirstShapeFile()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "TwoShapeFiles");

            // Call
            Action call = () => new ReferenceLineMetaImporter(pathToFolder);

            // Assert
            string expectedMessage = $@"Meerdere shapebestanden gevonden in '{pathToFolder}'. Het bestand 'NBPW_A.shp' is gebruikt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
        }

        [Test]
        public void GetReferenceLineMetas_InvalidShapeFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string pathToInvalid = Path.Combine(testDataPath, "InvalidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathToInvalid);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            // Assert
            string expectedExceptionMessage = $"Het shapebestand '{Path.Combine(pathToInvalid, "InvalidShapeFile.shp")}' " +
                                              "om trajecten te specificeren moet de attributen 'TRAJECT_ID', 'NORM_SW', en 'NORM_OG' bevatten: " +
                                              @"'TRAJECT_ID', 'NORM_SW', 'NORM_OG' niet gevonden.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_FileWithNonUniqueTrajectIds_ThrowsCriticalFileValidationException()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "NonUniqueTrajectIds");
            var importer = new ReferenceLineMetaImporter(pathToFolder);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            // Assert
            string shapeFile = Path.Combine(pathToFolder, "NonUniqueTrajectIds.shp");
            string expectedMessage = $"Fout bij het lezen van bestand '{shapeFile}': meerdere trajecten met dezelfde identificatiecode " +
                                     "(attribuut 'TRAJECT_ID') gevonden.";
            var exception = Assert.Throws<CriticalFileValidationException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_FileWithEmptyTrajectIds_ThrowsCriticalFileValidationException()
        {
            // Setup
            string pathToFolder = Path.Combine(testDataPath, "EmptyTrackId");
            var importer = new ReferenceLineMetaImporter(pathToFolder);

            // Call
            TestDelegate call = () => importer.GetReferenceLineMetas();

            // Assert
            string shapeFile = Path.Combine(pathToFolder, "EmptyTrackId.shp");
            string expectedMessage = $"Fout bij het lezen van bestand '{shapeFile}': trajecten gevonden zonder een geldige " +
                                     "identificatiecode (attribuut 'TRAJECT_ID').";
            var exception = Assert.Throws<CriticalFileValidationException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetReferenceLineMetas_ValidDirectoryWithOneShapeFile_ReturnsReadReadGetReferenceLineMetas()
        {
            // Setup
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            var importer = new ReferenceLineMetaImporter(pathValidFolder);

            // Call
            ReferenceLineMeta[] referenceLineMetas = importer.GetReferenceLineMetas().ToArray();

            // Assert
            Assert.AreEqual(3, referenceLineMetas.Length);

            var expectedReferenceLineMeta1 = new ReferenceLineMeta
            {
                AssessmentSectionId = "1-2",
                LowerLimitValue = 1000,
                SignalingValue = 3000
            };
            expectedReferenceLineMeta1.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(160679.9250, 475072.583),
                new Point2D(160892.0751, 474315.4917)
            });
            AssertReferenceLineMetas(expectedReferenceLineMeta1, referenceLineMetas[0]);

            var expectedReferenceLineMeta2 = new ReferenceLineMeta
            {
                AssessmentSectionId = "2-1",
                LowerLimitValue = 100,
                SignalingValue = 300
            };
            expectedReferenceLineMeta2.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(155556.9191, 464341.1281),
                new Point2D(155521.4761, 464360.7401)
            });
            AssertReferenceLineMetas(expectedReferenceLineMeta2, referenceLineMetas[1]);

            var expectedReferenceLineMeta3 = new ReferenceLineMeta
            {
                AssessmentSectionId = "3-3",
                LowerLimitValue = 100,
                SignalingValue = 300
            };
            expectedReferenceLineMeta3.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(147367.321899, 476902.9157103),
                new Point2D(147410.0515, 476938.9447)
            });
            AssertReferenceLineMetas(expectedReferenceLineMeta3, referenceLineMetas[2]);
        }

        [Test]
        public void GetReferenceLineMetas_EmptyShapeFile_ReturnsEmptyList()
        {
            // Setup
            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");
            var importer = new ReferenceLineMetaImporter(pathValidFolder);

            // Call
            IEnumerable<ReferenceLineMeta> referenceLineMetas = importer.GetReferenceLineMetas();

            // Assert
            Assert.AreEqual(0, referenceLineMetas.Count());
        }

        private static void AssertReferenceLineMetas(ReferenceLineMeta expectedReferenceLineMeta, ReferenceLineMeta actualReferenceLineMeta)
        {
            Assert.AreEqual(expectedReferenceLineMeta.AssessmentSectionId, actualReferenceLineMeta.AssessmentSectionId);
            Assert.AreEqual(expectedReferenceLineMeta.SignalingValue, actualReferenceLineMeta.SignalingValue);
            Assert.AreEqual(expectedReferenceLineMeta.LowerLimitValue, actualReferenceLineMeta.LowerLimitValue);

            Point2D[] expectedPoints = expectedReferenceLineMeta.ReferenceLine.Points.ToArray();
            Point2D[] actualPoints = actualReferenceLineMeta.ReferenceLine.Points.ToArray();
            CollectionAssert.AreEqual(expectedPoints, actualPoints,
                                      new Point2DComparerWithTolerance(1e-6),
                                      $"Unexpected geometry found in ReferenceLineMeta with id '{actualReferenceLineMeta.AssessmentSectionId}'");
        }
    }
}