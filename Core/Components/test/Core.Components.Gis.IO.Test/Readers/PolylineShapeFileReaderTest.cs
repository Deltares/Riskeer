using System;
using System.IO;
using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Readers
{
    [TestFixture]
    public class PolylineShapeFileReaderTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             "traject_10-1.shp");

            // Call
            using (var reader = new PolylineShapeFileReader(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<ShapeFileReaderBase>(reader);
            }
        }

        [Test]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Multiple_Point_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        public void ParameteredConstructor_ShapeFileIsNotLinesShapesfile_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string nonLineShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 shapeFileName);

            // Call
            TestDelegate call = () => new PolylineShapeFileReader(nonLineShapeFile);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestand bevat geometrieën die geen lijn zijn.",
                                                nonLineShapeFile);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetNumberOfLines_EmptyLineShapeFile_ReturnZero()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Empty_PolyLine_with_ID.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithOneLineFeature_ReturnOne()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithMultipleLineFeatures_ReturnThatNumberOfFeatures()
        {
            // Setup
            string shapeWithMultipleLines = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       "Multiple_PolyLine_with_ID.shp");

            using (var reader = new PolylineShapeFileReader(shapeWithMultipleLines))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(4, count);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithOneLineFeature_ReturnShape()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                MapLineData line = reader.ReadLine() as MapLineData;

                // Assert
                Assert.IsNotNull(line);
                var points = line.Points.ToArray();
                Assert.AreEqual(1669, points.Length);
                Assert.AreEqual(202714.219, points[457].X, 1e-6);
                Assert.AreEqual(507775.781, points[457].Y, 1e-6);

                Assert.AreEqual(6, line.MetaData.Count);
                Assert.AreEqual("A", line.MetaData["CATEGORIE"]);
                Assert.AreEqual("10", line.MetaData["DIJKRING"]);
                Assert.AreEqual(19190.35000000, line.MetaData["LENGTE_TRJ"]);
                Assert.AreEqual("1:1000", line.MetaData["Ondergrens"]);
                Assert.AreEqual("1:3000", line.MetaData["Signalerin"]);
                Assert.AreEqual("10-1", line.MetaData["TRAJECT_ID"]);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithMultipleLineFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultipleLines = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_PolyLine_with_ID.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithMultipleLines))
            {
                // Precondition
                Assert.AreEqual(4, reader.GetNumberOfLines());

                // Call
                MapLineData line1 = reader.ReadLine() as MapLineData;
                MapLineData line2 = reader.ReadLine() as MapLineData;
                MapLineData line3 = reader.ReadLine() as MapLineData;
                MapLineData line4 = reader.ReadLine() as MapLineData;

                // Assert

                #region Assertions for 'line1'

                var line1Points = line1.Points.ToArray();
                Assert.AreEqual(15, line1Points.Length);
                Assert.AreEqual(-1.514151, line1Points[2].X, 1e-6);
                Assert.AreEqual(-0.879717, line1Points[2].Y, 1e-6);

                Assert.AreEqual(1, line1.MetaData.Count);
                Assert.AreEqual(4, line1.MetaData["id"]);

                #endregion

                #region Assertions for 'line2'

                var line2Points = line2.Points.ToArray();
                Assert.AreEqual(6, line2Points.Length);
                Assert.AreEqual(-2.028302, line2Points[3].X, 1e-6);
                Assert.AreEqual(-0.382075, line2Points[3].Y, 1e-6);

                Assert.AreEqual(1, line2.MetaData.Count);
                Assert.AreEqual(3, line2.MetaData["id"]);

                #endregion

                #region Assertions for 'line3'

                var line3Points = line3.Points.ToArray();
                Assert.AreEqual(13, line3Points.Length);
                Assert.AreEqual(0.891509, line3Points[12].X, 1e-6);
                Assert.AreEqual(-0.122641, line3Points[12].Y, 1e-6);

                Assert.AreEqual(1, line3.MetaData.Count);
                Assert.AreEqual(2, line3.MetaData["id"]);

                #endregion

                #region Assertions for 'line4'

                var line4Points = line4.Points.ToArray();
                Assert.AreEqual(6, line4Points.Length);
                Assert.AreEqual(-2.070754, line4Points[0].X, 1e-6);
                Assert.AreEqual(0.73584906, line4Points[0].Y, 1e-6);

                Assert.AreEqual(1, line4.MetaData.Count);
                Assert.AreEqual(1, line4.MetaData["id"]);

                #endregion
            }
        }

        [Test]
        [TestCase("Empty_PolyLine_with_ID.shp")]
        [TestCase("traject_10-1.shp")]
        [TestCase("Multiple_PolyLine_with_ID.shp")]
        public void ReadLine_WhenAtEndOfShapeFile_ReturnNull(string shapeFileName)
        {
            // Setup
            string linesShapefileFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       shapeFileName);
            using (var reader = new PolylineShapeFileReader(linesShapefileFilePath))
            {
                for (int i = 0; i < reader.GetNumberOfLines(); i++)
                {
                    reader.ReadLine();
                }

                // Call
                MapLineData line = reader.ReadLine() as MapLineData;

                // Assert
                Assert.IsNull(line);
            }
        }

        [Test]
        public void ReadLine_ShapeFileIsIsMultiLine_ThrowCriticalFileReadException()
        {
            // Setup
            string nonLineShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Multi-PolyLine_with_ID.shp");

            var reader = new PolylineShapeFileReader(nonLineShapeFile);

            // Call
            TestDelegate call = () => reader.ReadLine();

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op index 0: Ingelezen element is een 'multi-lijn'; alleen enkelvoudige lijn elementen worden ondersteund.",
                                                nonLineShapeFile);
            var message = Assert.Throws<ElementReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void HasAttribute_AttributeInShapefile_ReturnTrue()
        {
            // Setup
            string shapefileFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_PolyLine_with_ID.shp");
            using (var reader = new PolylineShapeFileReader(shapefileFilePath))
            {
                // Call
                bool result = reader.HasAttribute("id");

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        [TestCase("id", true)]
        [TestCase("ID", false)]
        [TestCase("Id", false)]
        [TestCase("iD", false)]
        [TestCase("Im_not_in_file", false)]
        public void HasAttribute_VariousCases_ReturnTrueIfMatchesInProperCaseHasBeenFound(string attributeName, bool expectedResult)
        {
            // Setup
            string shapefileFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_PolyLine_with_ID.shp");
            using (var reader = new PolylineShapeFileReader(shapefileFilePath))
            {
                // Call
                bool result = reader.HasAttribute(attributeName);

                // Assert
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}