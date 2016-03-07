using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Readers
{
    [TestFixture]
    public class PointShapeFileReaderTest
    {
        [Test]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Multiple_Point_with_ID.shp")]
        public void ParameteredConstructor_ExpectedValues(string shapeFileName)
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             shapeFileName);

            // Call
            using (var reader = new PointShapeFileReader(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<ShapeFileReaderBase>(reader);
            }
        }

        [Test]
        [TestCase("traject_10-1.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        public void ParameteredConstructor_ShapeFileIsNotPointShapesfile_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string nonPointShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 shapeFileName);

            // Call
            TestDelegate call = () => new PointShapeFileReader(nonPointShapeFile);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestand bevat geometrieën die geen punt zijn.",
                                                nonPointShapeFile);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetNumberOfLines_EmptyPointShapeFile_ReturnZero()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Empty_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOneLine))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithOnePointFeature_ReturnOne()
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithMultiplePointFeatures_ReturnThatNumberOfFeatures()
        {
            // Setup
            string shapeWithMultiplePoints = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       "Multiple_Point_with_ID.shp");

            using (var reader = new PointShapeFileReader(shapeWithMultiplePoints))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(6, count);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithOnePointFeature_ReturnShape()
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                MapPointData point = reader.ReadLine() as MapPointData;

                // Assert
                Assert.IsNotNull(point);
                var points = point.Points.ToArray();
                Assert.AreEqual(1, points.Length);
                Assert.AreEqual(1.705, points[0].X, 1e-1);
                Assert.AreEqual(0.922, points[0].Y, 1e-1);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithMultiplePointFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultiplePoints = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithMultiplePoints))
            {
                // Precondition
                Assert.AreEqual(6, reader.GetNumberOfLines());

                // Call
                MapPointData point1 = reader.ReadLine() as MapPointData;
                MapPointData point2 = reader.ReadLine() as MapPointData;
                MapPointData point3 = reader.ReadLine() as MapPointData;
                MapPointData point4 = reader.ReadLine() as MapPointData;
                MapPointData point5 = reader.ReadLine() as MapPointData;
                MapPointData point6 = reader.ReadLine() as MapPointData;

                // Assert

                #region Assertsions for 'point1'

                var point1Points = point1.Points.ToArray();
                Assert.AreEqual(1, point1Points.Length);
                Assert.AreEqual(-1.750, point1Points[0].X, 1e-1);
                Assert.AreEqual(-0.488, point1Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'point2'

                var point2Points = point2.Points.ToArray();
                Assert.AreEqual(1, point2Points.Length);
                Assert.AreEqual(-0.790, point2Points[0].X, 1e-1);
                Assert.AreEqual(-0.308, point2Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'point3'

                var point3Points = point3.Points.ToArray();
                Assert.AreEqual(1, point3Points.Length);
                Assert.AreEqual(0.740, point3Points[0].X, 1e-1);
                Assert.AreEqual(-0.577, point3Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'point4'

                var point4Points = point4.Points.ToArray();
                Assert.AreEqual(1, point4Points.Length);
                Assert.AreEqual(0.787, point4Points[0].X, 1e-1);
                Assert.AreEqual(0.759, point4Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'point5'

                var point5Points = point5.Points.ToArray();
                Assert.AreEqual(1, point5Points.Length);
                Assert.AreEqual(-0.544, point5Points[0].X, 1e-1);
                Assert.AreEqual(0.283, point5Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'point6'

                var point6Points = point6.Points.ToArray();
                Assert.AreEqual(1, point6Points.Length);
                Assert.AreEqual(-2.066, point6Points[0].X, 1e-1);
                Assert.AreEqual(0.827, point6Points[0].Y, 1e-1);

                #endregion
            }            
        }

        [Test]
        [TestCase("Empty_Point_with_ID.shp")]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Multiple_Point_with_ID.shp")]
        public void ReadLine_WhenAtEndOfShapeFile_ReturnNull(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       fileName);
            using (var reader = new PointShapeFileReader(filePath))
            {
                for (int i = 0; i < reader.GetNumberOfLines(); i++)
                {
                    reader.ReadLine();
                }

                // Call
                MapPointData point = reader.ReadLine() as MapPointData;

                // Assert
                Assert.IsNull(point);
            }
        }

        [Test]
        public void HasAttribute_AttributeInShapefile_ReturnTrue()
        {
            // Setup
            string shapefileFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapefileFilePath))
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
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapefileFilePath))
            {
                // Call
                bool result = reader.HasAttribute(attributeName);

                // Assert
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}