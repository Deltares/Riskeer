using System;
using System.IO;
using System.Linq;

using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.Gis.IO.Test
{
    [TestFixture]
    public class LineShapeFileReaderTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             "traject_10-1.shp");

            // Call
            using (var reader = new LineShapeFileReader(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ParameteredConstructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new LineShapeFileReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "traject_10-1.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidFileNameChars[0].ToString());

            // Call
            TestDelegate call = () => new LineShapeFileReader(invalidFilePath);

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

            // Call
            TestDelegate call = () => new LineShapeFileReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithOneLineFeature_ReturnOne()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new LineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithOneLineFeature_ReturnShape()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new LineShapeFileReader(shapeWithOneLine))
            {
                // Call
                MapLineData line = reader.ReadLine();

                // Assert
                var points = line.Points.ToArray();
                Assert.AreEqual(1669, points.Length);
                Assert.AreEqual(202714.219, points[457].Item1, 1e-6);
                Assert.AreEqual(507775.781, points[457].Item2, 1e-6);

                Assert.AreEqual(6, line.MetaData.Count);
                Assert.AreEqual("A", line.MetaData["CATEGORIE"]);
                Assert.AreEqual("10", line.MetaData["DIJKRING"]);
                Assert.AreEqual(19190.35000000, line.MetaData["LENGTE_TRJ"]);
                Assert.AreEqual("1:1000", line.MetaData["Ondergrens"]);
                Assert.AreEqual("1:3000", line.MetaData["Signalerin"]);
                Assert.AreEqual("10-1", line.MetaData["TRAJECT_ID"]);
            }
        }
    }
}