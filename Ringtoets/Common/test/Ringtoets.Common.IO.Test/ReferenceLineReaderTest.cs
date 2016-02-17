﻿using System;
using System.IO;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

using NUnit.Framework;

using Ringtoets.Common.Data;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ReferenceLineReaderTest
    {
        [Test]
        public void ReadReferenceLine_FileHasOneValidLineInShape_ReturnReferenceLine()
        {
            // Setup
            var validReferenceLineShapeFile = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");
            var reader =  new ReferenceLineReader();

            // Call
            ReferenceLine referenceLine = reader.ReadReferenceLine(validReferenceLineShapeFile);

            // Assert
            Point2D[] point2Ds = referenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(193527.156, point2Ds[465].X, 1e-6);
            Assert.AreEqual(511438.281, point2Ds[465].Y, 1e-6);
            Assert.AreEqual(191144.375, point2Ds[800].X, 1e-6);
            Assert.AreEqual(508840.469, point2Ds[800].Y, 1e-6);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ReadReferenceLine_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Setup
            var reader = new ReferenceLineReader();

            // Call
            TestDelegate call = () => reader.ReadReferenceLine(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "traject_10-1.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidFileNameChars[3].ToString());

            var reader = new ReferenceLineReader();

            // Call
            TestDelegate call = () => reader.ReadReferenceLine(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            var reader = new ReferenceLineReader();

            // Call
            TestDelegate call = () => reader.ReadReferenceLine(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }
    }
}