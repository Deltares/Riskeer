﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ReferenceLinesMetaReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "NBPW");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW.shp");

            // Call
            using (var reader = new ReferenceLinesMetaReader(validFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }

            Assert.True(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new ReferenceLinesMetaReader(invalidFilePath);

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

            string validFilePath = Path.Combine(testDataPath, "NBPW.shp");
            string invalidFilePath = validFilePath.Replace("P", invalidFileNameChars[1].ToString());

            // Call
            TestDelegate call = () => new ReferenceLinesMetaReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new ReferenceLinesMetaReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet verwijzen naar een lege bestandsnaam.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => new ReferenceLinesMetaReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Multiple_Point_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        public void Constructor_ShapefileDoesNotHaveSinglePolyline_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, shapeFileName);

            TestDelegate call = () => new ReferenceLinesMetaReader(invalidFilePath);

            // Assert .
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestand bevat geometrieën die geen lijn zijn.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("NBPW_MultiPolyLines.shp")]
        public void ReadReferenceLinesMeta_ShapefileHasMultiplePolylines_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, shapeFileName);

            using (var reader = new ReferenceLinesMetaReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.ReadReferenceLinesMeta();

                // Assert
                var expectedMessage = "Het bestand bevat een multi-polylijn. Multi-polylijnen worden niet ondersteund.";
                var message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("NBPW_missingTrajectId.shp", "TRAJECT_ID")]
        [TestCase("NBPW_missingNORM_SW.shp", "NORM_SW")]
        [TestCase("NBPW_missingNORM_OG.shp", "NORM_OG")]
        public void Constructor_FileLacksAttribute_ThrowCriticalFileReadException(string shapeFileName, string missingAttribute)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, shapeFileName);

            TestDelegate call = () => new ReferenceLinesMetaReader(validFilePath);

            // Assert
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            var expectedMessage = String.Format("Het bestand heeft geen attribuut '{0}'. Dit attribuut is vereist.",
                                                missingAttribute);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("NBPW_missingAllAttributes.shp", "TRAJECT_ID', 'NORM_SW', 'NORM_OG")]
        [TestCase("NBPW_missingTrajectIdAndNORM_SW.shp", "TRAJECT_ID', 'NORM_SW")]
        [TestCase("NBPW_missingTrajectIdAndNORM_OG.shp", "TRAJECT_ID', 'NORM_OG")]
        [TestCase("NBPW_missingNORM_SWAndNORM_OG.shp", "NORM_SW', 'NORM_OG")]
        public void Constructor_FileLacksAttributes_ThrowCriticalFileReadException(string shapeFileName, string missingAttributes)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, shapeFileName);

            TestDelegate call = () => new ReferenceLinesMetaReader(validFilePath);

            // Assert
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            var expectedMessage = String.Format("Het bestand heeft de attributen '{0}' niet. Deze attributen zijn vereist.",
                                                missingAttributes);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadReferenceLinesMeta_ValidFilePath_ReturnsElement()
        {
            // Setup
            var validFilePath = Path.Combine(testDataPath, "NBPW.shp");

            using (var reader = new ReferenceLinesMetaReader(validFilePath))
            {
                // Call
                ReferenceLineMeta referenceLineMeta = reader.ReadReferenceLinesMeta();

                // Assert
                Assert.AreEqual("205", referenceLineMeta.AssessmentSectionId);
                Assert.AreEqual(3000, referenceLineMeta.SignalingValue);
                Assert.AreEqual(1000, referenceLineMeta.LowerLimitValue);
                Point2D[] geometryPoints = referenceLineMeta.ReferenceLine.Points.ToArray();
                Assert.AreEqual(2, geometryPoints.Length);
                Assert.AreEqual(475072.583000, geometryPoints[0].Y, 1e-6);
                Assert.AreEqual(160892.075100, geometryPoints[1].X, 1e-6);

                ReferenceLineMeta referenceLineMeta2 = reader.ReadReferenceLinesMeta();
                Assert.AreEqual("11-1", referenceLineMeta2.AssessmentSectionId);
            }
        }

        [Test]
        public void ReadReferenceLinesMeta_EmptyNormOgAndNormSw_ReturnsElement()
        {
            // Setup
            var validFilePath = Path.Combine(testDataPath, "NBPW_EmptyNormOGAndNormSW.shp");

            using (var reader = new ReferenceLinesMetaReader(validFilePath))
            {
                // Call
                ReferenceLineMeta referenceLineMeta = reader.ReadReferenceLinesMeta();

                // Assert
                Assert.AreEqual("46-1", referenceLineMeta.AssessmentSectionId);
                Assert.IsNull(referenceLineMeta.SignalingValue);
                Assert.IsNull(referenceLineMeta.LowerLimitValue);
            }
        }

        [Test]
        public void ReadReferenceLinesMeta_EmptyTrackId_ReturnsElement()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW_EmptyTrackId.shp");

            using (var reader = new ReferenceLinesMetaReader(validFilePath))
            {
                // Call
                ReferenceLineMeta referenceLineMeta = reader.ReadReferenceLinesMeta();

                // Assert
                Assert.AreEqual(string.Empty, referenceLineMeta.AssessmentSectionId);
            }
        }

        [Test]
        public void ReadReferenceLinesMeta_ReadingToEndOfFile_ReturnNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW.shp");

            using (var reader = new ReferenceLinesMetaReader(validFilePath))
            {
                ReferenceLineMeta line;
                do
                {
                    line = reader.ReadReferenceLinesMeta();
                } while (line != null);

                // Call
                var resultBeyondEndOfFile = reader.ReadReferenceLinesMeta();

                // Assert
                Assert.IsNull(resultBeyondEndOfFile);
            }
        }
    }
}