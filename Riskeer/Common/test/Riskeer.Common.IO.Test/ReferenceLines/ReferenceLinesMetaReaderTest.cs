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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.ReferenceLines;

namespace Riskeer.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLinesMetaReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "NBPW");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ReadReferenceLinesMetas_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLinesMetas_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = Path.Combine(testDataPath, "NBPW.shp");
            string invalidFilePath = validFilePath.Replace("P", invalidPathChars[1].ToString());

            // Call
            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLinesMetas_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLinesMetas_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Multiple_Point_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        public void ReadReferenceLinesMetas_ShapefileDoesNotHaveSinglePolyline_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, shapeFileName);

            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert .
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': kon geen lijnen vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("NBPW_MultiPolyLines.shp")]
        public void ReadReferenceLinesMeta_ShapefileHasMultiplePolylines_ReturnsEmptyReferenceLines(string shapeFileName)
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, shapeFileName);

            // Call
            IEnumerable<ReferenceLineMeta> referenceLineMetas = ReferenceLinesMetaReader.ReadReferenceLinesMetas(invalidFilePath);

            // Assert
            Assert.AreEqual(2, referenceLineMetas.Count());
            CollectionAssert.IsEmpty(referenceLineMetas.ElementAt(0).ReferenceLine.Points);
            CollectionAssert.IsEmpty(referenceLineMetas.ElementAt(1).ReferenceLine.Points);
        }

        [Test]
        [TestCase("NBPW_missingTrajectId.shp", "TRAJECT_ID")]
        [TestCase("NBPW_missingNORM_SW.shp", "NORM_SW")]
        [TestCase("NBPW_missingNORM_OG.shp", "NORM_OG")]
        public void ReadReferenceLinesMetas_FileLacksAttribute_ThrowCriticalFileReadException(string shapeFileName, string missingAttribute)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, shapeFileName);

            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(validFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = $"Het shapebestand '{validFilePath}' om trajecten te specificeren moet de attributen 'TRAJECT_ID', 'NORM_SW', en 'NORM_OG' bevatten: '{missingAttribute}' niet gevonden.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("NBPW_missingAllAttributes.shp", "TRAJECT_ID', 'NORM_SW', 'NORM_OG")]
        [TestCase("NBPW_missingTrajectIdAndNORM_SW.shp", "TRAJECT_ID', 'NORM_SW")]
        [TestCase("NBPW_missingTrajectIdAndNORM_OG.shp", "TRAJECT_ID', 'NORM_OG")]
        [TestCase("NBPW_missingNORM_SWAndNORM_OG.shp", "NORM_SW', 'NORM_OG")]
        public void ReadReferenceLinesMetas_FileLacksAttributes_ThrowCriticalFileReadException(string shapeFileName, string missingAttributes)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, shapeFileName);

            TestDelegate call = () => ReferenceLinesMetaReader.ReadReferenceLinesMetas(validFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = $"Het shapebestand '{validFilePath}' om trajecten te specificeren moet de attributen 'TRAJECT_ID', 'NORM_SW', en 'NORM_OG' bevatten: '{missingAttributes}' niet gevonden.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadReferenceLinesMeta_ValidFilePath_ReturnsElement()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW.shp");

            // Call
            IEnumerable<ReferenceLineMeta> referenceLineMetas = ReferenceLinesMetaReader.ReadReferenceLinesMetas(validFilePath);

            // Assert
            Assert.AreEqual(3, referenceLineMetas.Count());

            var expectedReferenceLineMeta1 = new ReferenceLineMeta
            {
                AssessmentSectionId = "1-1",
                LowerLimitValue = 1000,
                SignalingValue = 3000
            };
            expectedReferenceLineMeta1.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(160679.9250, 475072.583),
                new Point2D(160892.0751, 474315.4917)
            });
            AssertReferenceLineMetas(expectedReferenceLineMeta1, referenceLineMetas.ElementAt(0));

            var expectedReferenceLineMeta2 = new ReferenceLineMeta
            {
                AssessmentSectionId = "2-2",
                LowerLimitValue = 100,
                SignalingValue = 300
            };
            expectedReferenceLineMeta2.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(155556.9191, 464341.1281),
                new Point2D(155521.4761, 464360.7401)
            });
            AssertReferenceLineMetas(expectedReferenceLineMeta2, referenceLineMetas.ElementAt(1));

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
            AssertReferenceLineMetas(expectedReferenceLineMeta3, referenceLineMetas.ElementAt(2));
        }

        [Test]
        public void ReadReferenceLinesMeta_EmptyNormOgAndNormSw_ReturnsElement()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW_EmptyNormOGAndNormSW.shp");

            // Call
            IEnumerable<ReferenceLineMeta> referenceLineMetas = ReferenceLinesMetaReader.ReadReferenceLinesMetas(validFilePath);

            // Assert
            ReferenceLineMeta referenceLineMeta = referenceLineMetas.Single();
            Assert.AreEqual("46-1", referenceLineMeta.AssessmentSectionId);
            Assert.IsNull(referenceLineMeta.SignalingValue);
            Assert.AreEqual(0, referenceLineMeta.LowerLimitValue);
        }

        [Test]
        public void ReadReferenceLinesMeta_EmptyTrackId_ReturnsElement()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "NBPW_EmptyTrackId.shp");

            // Call
            IEnumerable<ReferenceLineMeta> referenceLineMetas = ReferenceLinesMetaReader.ReadReferenceLinesMetas(validFilePath);

            // Assert
            ReferenceLineMeta referenceLineMeta = referenceLineMetas.Single();
            Assert.IsEmpty(referenceLineMeta.AssessmentSectionId);
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