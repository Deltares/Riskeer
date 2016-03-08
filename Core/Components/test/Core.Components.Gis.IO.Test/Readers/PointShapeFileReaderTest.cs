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
        [TestCase("Test points")]
        [TestCase("Another test with points")]
        public void ReadLine_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                MapPointData pointData = reader.ReadLine(name) as MapPointData;

                // Assert
                Assert.AreEqual(name, pointData.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadLine_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                MapPointData pointData = reader.ReadLine(name) as MapPointData;

                // Assert
                Assert.AreEqual("Punten", pointData.Name);
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
                MapPointData pointData = reader.ReadLine() as MapPointData;

                // Assert
                Assert.IsNotNull(pointData);                
                Assert.AreEqual(1, pointData.Features.Count());
                var points = pointData.Features.First().MapGeometries.First().Points.ToArray();
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
                MapPointData points1 = reader.ReadLine() as MapPointData;
                MapPointData points2 = reader.ReadLine() as MapPointData;
                MapPointData points3 = reader.ReadLine() as MapPointData;
                MapPointData points4 = reader.ReadLine() as MapPointData;
                MapPointData points5 = reader.ReadLine() as MapPointData;
                MapPointData points6 = reader.ReadLine() as MapPointData;

                // Assert

                #region Assertion for 'point1'
                
                var features1 = points1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                var point1 = features1[0];
                var point1Geometry = point1.MapGeometries.ToArray();
                Assert.AreEqual(1, point1Geometry.Length);
                var point1Points = point1Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point1Points.Length);
                Assert.AreEqual(-1.750, point1Points[0].X, 1e-1);
                Assert.AreEqual(-0.488, point1Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point2'

                var features2 = points2.Features.ToArray();
                Assert.AreEqual(1, features2.Length);

                var point2 = features2[0];
                var point2Geometry = point2.MapGeometries.ToArray();
                Assert.AreEqual(1, point2Geometry.Length);
                var point2Points = point2Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point2Points.Length);
                Assert.AreEqual(-0.790, point2Points[0].X, 1e-1);
                Assert.AreEqual(-0.308, point2Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point3'
                var features3 = points3.Features.ToArray();
                Assert.AreEqual(1, features3.Length);

                var point3 = features3[0];
                var point3Geometry = point3.MapGeometries.ToArray();
                Assert.AreEqual(1, point3Geometry.Length);
                var point3Points = point3Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point3Points.Length);
                Assert.AreEqual(0.740, point3Points[0].X, 1e-1);
                Assert.AreEqual(-0.577, point3Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point4'
                var features4 = points4.Features.ToArray();
                Assert.AreEqual(1, features4.Length);

                var point4 = features4[0];
                var point4Geometry = point4.MapGeometries.ToArray();
                Assert.AreEqual(1, point4Geometry.Length);
                var point4Points = point4Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point4Points.Length);
                Assert.AreEqual(0.787, point4Points[0].X, 1e-1);
                Assert.AreEqual(0.759, point4Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point5'
                var features5 = points5.Features.ToArray();
                Assert.AreEqual(1, features5.Length);

                var point5 = features5[0];
                var point5Geometry = point5.MapGeometries.ToArray();
                Assert.AreEqual(1, point5Geometry.Length);
                var point5Points = point5Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point5Points.Length);
                Assert.AreEqual(-0.544, point5Points[0].X, 1e-1);
                Assert.AreEqual(0.283, point5Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point6'
                var features6 = points6.Features.ToArray();
                Assert.AreEqual(1, features6.Length);

                var point6 = features6[0];
                var point6Geometry = point6.MapGeometries.ToArray();
                Assert.AreEqual(1, point6Geometry.Length);
                var point6Points = point6Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point6Points.Length);
                Assert.AreEqual(-2.066, point6Points[0].X, 1e-1);
                Assert.AreEqual(0.827, point6Points[0].Y, 1e-1);

                #endregion
            }
        }

        [Test]
        [TestCase("Test points")]
        [TestCase("Another test with points")]
        public void ReadShapeFile_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                MapPointData pointData = reader.ReadShapeFile(name) as MapPointData;

                // Assert
                Assert.AreEqual(name, pointData.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadShapeFile_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOnePoint = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithOnePoint))
            {
                // Call
                MapPointData pointData = reader.ReadShapeFile(name) as MapPointData;

                // Assert
                Assert.AreEqual("Punten", pointData.Name);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithMultiplePointFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultiplePoints = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_Point_with_ID.shp");
            using (var reader = new PointShapeFileReader(shapeWithMultiplePoints))
            {
                // Precondition
                Assert.AreEqual(6, reader.GetNumberOfLines());

                // Call
                MapPointData points = reader.ReadShapeFile() as MapPointData;

                // Assert
                var features = points.Features.ToArray();
                Assert.AreEqual(6, features.Length);

                #region Assertion for 'point1'

                var point1 = features[0];                
                var point1Geometry = point1.MapGeometries.ToArray();
                Assert.AreEqual(1, point1Geometry.Length);
                var point1Points = point1Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point1Points.Length);
                Assert.AreEqual(-1.750, point1Points[0].X, 1e-1);
                Assert.AreEqual(-0.488, point1Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point2'

                var point2 = features[1];
                var point2Geometry = point2.MapGeometries.ToArray();
                Assert.AreEqual(1, point2Geometry.Length);
                var point2Points = point2Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point2Points.Length);
                Assert.AreEqual(-0.790, point2Points[0].X, 1e-1);
                Assert.AreEqual(-0.308, point2Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point3'

                var point3 = features[2];
                var point3Geometry = point3.MapGeometries.ToArray();
                Assert.AreEqual(1, point3Geometry.Length);
                var point3Points = point3Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point3Points.Length);
                Assert.AreEqual(0.740, point3Points[0].X, 1e-1);
                Assert.AreEqual(-0.577, point3Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point4'

                var point4 = features[3];
                var point4Geometry = point4.MapGeometries.ToArray();
                Assert.AreEqual(1, point4Geometry.Length);
                var point4Points = point4Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point4Points.Length);
                Assert.AreEqual(0.787, point4Points[0].X, 1e-1);
                Assert.AreEqual(0.759, point4Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point5'

                var point5 = features[4];
                var point5Geometry = point5.MapGeometries.ToArray();
                Assert.AreEqual(1, point5Geometry.Length);
                var point5Points = point5Geometry[0].Points.ToArray();
                Assert.AreEqual(1, point5Points.Length);
                Assert.AreEqual(-0.544, point5Points[0].X, 1e-1);
                Assert.AreEqual(0.283, point5Points[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point6'

                var point6 = features[5];
                var point6Geometry = point6.MapGeometries.ToArray();
                Assert.AreEqual(1, point6Geometry.Length);
                var point6Points = point6Geometry[0].Points.ToArray();
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
                MapPointData feature = reader.ReadLine() as MapPointData;

                // Assert
                Assert.IsNull(feature);
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