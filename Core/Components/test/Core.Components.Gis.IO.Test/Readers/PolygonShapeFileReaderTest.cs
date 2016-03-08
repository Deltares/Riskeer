using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Readers
{
    [TestFixture]
    public class PolygonShapeFileReaderTest
    {
        [Test]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        public void ParameteredConstructor_ExpectedValues(string shapeFileName)
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             shapeFileName);

            // Call
            using (var reader = new PolygonShapeFileReader(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<ShapeFileReaderBase>(reader);
            }
        }

        [Test]
        [TestCase("traject_10-1.shp")]
        [TestCase("Empty_Point_with_ID.shp")]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Multiple_Point_with_ID.shp")]
        [TestCase("Empty_PolyLine_with_ID.shp")]
        [TestCase("Single_Multi-PolyLine_with_ID.shp")]
        [TestCase("Multiple_PolyLine_with_ID.shp")]
        public void ParameteredConstructor_ShapeFileIsNotPolygonShapesfile_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string nonPolygonShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 shapeFileName);

            // Call
            TestDelegate call = () => new PolygonShapeFileReader(nonPolygonShapeFile);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestand bevat geometrieën die geen polygonen zijn.",
                                                nonPolygonShapeFile);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetNumberOfLines_EmptyPolygonShapeFile_ReturnZero()
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Empty_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithOnePolygonFeature_ReturnOne()
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
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
            string shapeWithMultiplePolygons = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       "Multiple_Polygon_with_ID.shp");

            using (var reader = new PolygonShapeFileReader(shapeWithMultiplePolygons))
            {
                // Call
                var count = reader.GetNumberOfLines();

                // Assert
                Assert.AreEqual(4, count);
            }
        }

        [Test]
        [TestCase("Test polygons")]
        [TestCase("Another test with polygons")]
        public void ReadLine_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadLine(name) as MapPolygonData;

                // Assert
                Assert.AreEqual(name, polygon.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadLine_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadLine(name) as MapPolygonData;

                // Assert
                Assert.AreEqual("Polygoon", polygon.Name);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithOnePolygonFeature_ReturnShape()
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadLine() as MapPolygonData;

                // Assert
                Assert.IsNotNull(polygon);
                var polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);
                var polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);
                var polygonPoints = polygonGeometries[0].Points.ToArray();
                Assert.AreEqual(30, polygonPoints.Length);
                Assert.AreEqual(-0.264, polygonPoints[25].X, 1e-1);
                Assert.AreEqual(0.169, polygonPoints[25].Y, 1e-1);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithSingeFeatureMultiplePolygons_ReturnShapes()
        {
            // Setup
            string shape = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Single_Multi-Polygon_with_ID.shp");

            using (var reader = new PolygonShapeFileReader(shape))
            {
                // Call
                MapPolygonData polygon = reader.ReadLine() as MapPolygonData;

                // Assert
                Assert.IsNotNull(polygon);
                var polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);
                var polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(2, polygonGeometries.Length);
                var polygonPoints = polygonGeometries[0].Points.ToArray();
                Assert.AreEqual(7, polygonPoints.Length);
                Assert.AreEqual(-2.257, polygonPoints[4].X, 1e-1);
                Assert.AreEqual(0.419, polygonPoints[4].Y, 1e-1);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithMultiplePolygonFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultiplePolygons = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithMultiplePolygons))
            {
                // Precondition
                Assert.AreEqual(4, reader.GetNumberOfLines());

                // Call
                MapPolygonData polygons1 = reader.ReadLine() as MapPolygonData;
                MapPolygonData polygons2 = reader.ReadLine() as MapPolygonData;
                MapPolygonData polygons3 = reader.ReadLine() as MapPolygonData;
                MapPolygonData polygons4 = reader.ReadLine() as MapPolygonData;

                // Assert
                #region Assertsions for 'polygon1'
                
                var features1 = polygons1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                var polygon1 = features1[0];
                var polygon1Geometry = polygon1.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon1Geometry.Length);
                var polygon1Points = polygon1Geometry[0].Points.ToArray();
                Assert.AreEqual(6, polygon1Points.Length);
                Assert.AreEqual(-1.070, polygon1Points[2].X, 1e-1);
                Assert.AreEqual(0.066, polygon1Points[2].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon2'

                var features2 = polygons2.Features.ToArray();
                Assert.AreEqual(1, features2.Length);

                var polygon2 = features2[0];
                var polygon2Geometry = polygon2.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon2Geometry.Length);
                var polygon2Points = polygon2Geometry[0].Points.ToArray();
                Assert.AreEqual(25, polygon2Points.Length);
                Assert.AreEqual(-2.172, polygon2Points[23].X, 1e-1);
                Assert.AreEqual(0.212, polygon2Points[23].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon3'

                var features3 = polygons3.Features.ToArray();
                Assert.AreEqual(1, features3.Length);

                var polygon3 = features3[0];
                var polygon3Geometry = polygon3.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon3Geometry.Length);
                var polygon3Points = polygon3Geometry[0].Points.ToArray();
                Assert.AreEqual(10, polygon3Points.Length);
                Assert.AreEqual(-1.091, polygon3Points[0].X, 1e-1);
                Assert.AreEqual(0.566, polygon3Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon4'

                var features4 = polygons4.Features.ToArray();
                Assert.AreEqual(1, features4.Length);

                var polygon4 = features4[0];
                var polygon4Geometry = polygon4.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon4Geometry.Length);
                var polygon4Points = polygon4Geometry[0].Points.ToArray();
                Assert.AreEqual(9, polygon4Points.Length);
                Assert.AreEqual(-1.917, polygon4Points[8].X, 1e-1);
                Assert.AreEqual(0.759, polygon4Points[8].Y, 1e-1);

                #endregion
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithOnePolygonFeature_ReturnShape()
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadShapeFile() as MapPolygonData;

                // Assert
                Assert.IsNotNull(polygon);
                var polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);
                var polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);
                var polygonPoints = polygonGeometries[0].Points.ToArray();
                Assert.AreEqual(30, polygonPoints.Length);
                Assert.AreEqual(-0.264, polygonPoints[25].X, 1e-1);
                Assert.AreEqual(0.169, polygonPoints[25].Y, 1e-1);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithSingeFeatureMultiplePolygons_ReturnShapes()
        {
            // Setup
            string shape = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Single_Multi-Polygon_with_ID.shp");

            using (var reader = new PolygonShapeFileReader(shape))
            {
                // Call
                MapPolygonData polygon = reader.ReadShapeFile() as MapPolygonData;

                // Assert
                Assert.IsNotNull(polygon);
                var polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);
                var polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(2, polygonGeometries.Length);
                var polygonPoints = polygonGeometries[0].Points.ToArray();
                Assert.AreEqual(7, polygonPoints.Length);
                Assert.AreEqual(-2.257, polygonPoints[4].X, 1e-1);
                Assert.AreEqual(0.419, polygonPoints[4].Y, 1e-1);
            }
        }

        [Test]
        [TestCase("Test polygons")]
        [TestCase("Another test with polygons")]
        public void ReadShapeFile_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadShapeFile(name) as MapPolygonData;

                // Assert
                Assert.AreEqual(name, polygon.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadShapeFile_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOnePolygon = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygon))
            {
                // Call
                MapPolygonData polygon = reader.ReadShapeFile(name) as MapPolygonData;

                // Assert
                Assert.AreEqual("Polygoon", polygon.Name);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithMultiplePolygonFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultiplePolygons = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Multiple_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithMultiplePolygons))
            {
                // Precondition
                Assert.AreEqual(4, reader.GetNumberOfLines());
                
                // Call
                MapPolygonData polygons = reader.ReadShapeFile() as MapPolygonData;

                // Assert
                var features = polygons.Features.ToArray();
                Assert.AreEqual(4, features.Length);

                #region Assertsions for 'polygon1'

                var polygon1 = features[0];
                var polygon1Geometry = polygon1.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon1Geometry.Length);
                var polygon1Points = polygon1Geometry[0].Points.ToArray();
                Assert.AreEqual(6, polygon1Points.Length);
                Assert.AreEqual(-1.070, polygon1Points[2].X, 1e-1);
                Assert.AreEqual(0.066, polygon1Points[2].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon2'

                var polygon2 = features[1];
                var polygon2Geometry = polygon2.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon2Geometry.Length);
                var polygon2Points = polygon2Geometry[0].Points.ToArray();
                Assert.AreEqual(25, polygon2Points.Length);
                Assert.AreEqual(-2.172, polygon2Points[23].X, 1e-1);
                Assert.AreEqual(0.212, polygon2Points[23].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon3'

                var polygon3 = features[2];
                var polygon3Geometry = polygon3.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon3Geometry.Length);
                var polygon3Points = polygon3Geometry[0].Points.ToArray();
                Assert.AreEqual(10, polygon3Points.Length);
                Assert.AreEqual(-1.091, polygon3Points[0].X, 1e-1);
                Assert.AreEqual(0.566, polygon3Points[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon4'

                var polygon4 = features[3];
                var polygon4Geometry = polygon4.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon4Geometry.Length);
                var polygon4Points = polygon4Geometry[0].Points.ToArray();
                Assert.AreEqual(9, polygon4Points.Length);
                Assert.AreEqual(-1.917, polygon4Points[8].X, 1e-1);
                Assert.AreEqual(0.759, polygon4Points[8].Y, 1e-1);

                #endregion
            }
        }

        [Test]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        public void ReadLine_WhenAtEndOfShapeFile_ReturnNull(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       fileName);
            using (var reader = new PolygonShapeFileReader(filePath))
            {
                for (int i = 0; i < reader.GetNumberOfLines(); i++)
                {
                    reader.ReadLine();
                }

                // Call
                MapPolygonData polygon = reader.ReadLine() as MapPolygonData;

                // Assert
                Assert.IsNull(polygon);
            }
        }

        [Test]
        public void HasAttribute_AttributeInShapefile_ReturnTrue()
        {
            // Setup
            string shapefileFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapefileFilePath))
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
                                                                 "Single_Polygon_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapefileFilePath))
            {
                // Call
                bool result = reader.HasAttribute(attributeName);

                // Assert
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}
