using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
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
                MapPolygonData polygon = (MapPolygonData)reader.ReadLine(name);

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
                MapPolygonData polygon = (MapPolygonData)reader.ReadLine(name);

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
                MapPolygonData polygon = (MapPolygonData)reader.ReadLine();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                var firstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(30, firstPointCollection.Length);
                Assert.AreEqual(-0.264, firstPointCollection[25].X, 1e-1);
                Assert.AreEqual(0.169, firstPointCollection[25].Y, 1e-1);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithSingeFeatureMultiplePolygons_ReturnShapes()
        {
            // Setup
            string shape = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                      "Single_Multi-Polygon_with_ID.shp");

            using (var reader = new PolygonShapeFileReader(shape))
            {
                // Call
                MapPolygonData polygon = (MapPolygonData)reader.ReadLine();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(2, polygonGeometries.Length);

                IEnumerable<Point2D>[] firstGeometryPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, firstGeometryPointCollections.Length);

                Point2D[] firstGeometryFirstPointCollection = firstGeometryPointCollections[0].ToArray();
                Assert.AreEqual(7, firstGeometryFirstPointCollection.Length);
                Assert.AreEqual(-2.257, firstGeometryFirstPointCollection[4].X, 1e-1);
                Assert.AreEqual(0.419, firstGeometryFirstPointCollection[4].Y, 1e-1);
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
                MapPolygonData polygons1 = (MapPolygonData)reader.ReadLine();
                MapPolygonData polygons2 = (MapPolygonData)reader.ReadLine();
                MapPolygonData polygons3 = (MapPolygonData)reader.ReadLine();
                MapPolygonData polygons4 = (MapPolygonData)reader.ReadLine();

                // Assert
                #region Assertsions for 'polygon1'
                
                MapFeature[] features1 = polygons1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                MapFeature polygon1 = features1[0];
                MapGeometry[] polygon1Geometry = polygon1.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon1Geometry.Length);

                IEnumerable<Point2D>[] polygon1PointCollections = polygon1Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon1PointCollections.Length);

                var polygon1PointCollection = polygon1PointCollections[0].ToArray();
                Assert.AreEqual(6, polygon1PointCollection.Length);
                Assert.AreEqual(-1.070, polygon1PointCollection[2].X, 1e-1);
                Assert.AreEqual(0.066, polygon1PointCollection[2].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon2'

                MapFeature[] features2 = polygons2.Features.ToArray();
                Assert.AreEqual(1, features2.Length);

                MapFeature polygon2 = features2[0];
                MapGeometry[] polygon2Geometry = polygon2.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon2Geometry.Length);

                IEnumerable<Point2D>[] polygon2PointCollections = polygon2Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon2PointCollections.Length);

                Point2D[] polygon2PointCollection = polygon2PointCollections[0].ToArray();
                Assert.AreEqual(25, polygon2PointCollection.Length);
                Assert.AreEqual(-2.172, polygon2PointCollection[23].X, 1e-1);
                Assert.AreEqual(0.212, polygon2PointCollection[23].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon3'

                MapFeature[] features3 = polygons3.Features.ToArray();
                Assert.AreEqual(1, features3.Length);

                MapFeature polygon3 = features3[0];
                MapGeometry[] polygon3Geometry = polygon3.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon3Geometry.Length);

                IEnumerable<Point2D>[] polygon3PointCollections = polygon3Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon3PointCollections.Length);

                Point2D[] polygon3PointCollection = polygon3PointCollections[0].ToArray();
                Assert.AreEqual(10, polygon3PointCollection.Length);
                Assert.AreEqual(-1.091, polygon3PointCollection[0].X, 1e-1);
                Assert.AreEqual(0.566, polygon3PointCollection[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon4'

                MapFeature[] features4 = polygons4.Features.ToArray();
                Assert.AreEqual(1, features4.Length);

                MapFeature polygon4 = features4[0];
                MapGeometry[] polygon4Geometry = polygon4.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon4Geometry.Length);

                IEnumerable<Point2D>[] polygon4PointCollections = polygon4Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon4PointCollections.Length);

                var polygon4PointCollection = polygon4PointCollections[0].ToArray();
                Assert.AreEqual(9, polygon4PointCollection.Length);
                Assert.AreEqual(-1.917, polygon4PointCollection[8].X, 1e-1);
                Assert.AreEqual(0.759, polygon4PointCollection[8].Y, 1e-1);

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
                MapPolygonData polygon = (MapPolygonData)reader.ReadShapeFile();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                var polygonFirstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(30, polygonFirstPointCollection.Length);
                Assert.AreEqual(-0.264, polygonFirstPointCollection[25].X, 1e-1);
                Assert.AreEqual(0.169, polygonFirstPointCollection[25].Y, 1e-1);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithSingeFeatureMultiplePolygons_ReturnShapes()
        {
            // Setup
            string shape = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                      "Single_Multi-Polygon_with_ID.shp");

            using (var reader = new PolygonShapeFileReader(shape))
            {
                // Call
                MapPolygonData polygon = (MapPolygonData)reader.ReadShapeFile();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(2, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                var firstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(7, firstPointCollection.Length);
                Assert.AreEqual(-2.257, firstPointCollection[4].X, 1e-1);
                Assert.AreEqual(0.419, firstPointCollection[4].Y, 1e-1);
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
                MapPolygonData polygon = (MapPolygonData)reader.ReadShapeFile(name);

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
                MapPolygonData polygon = (MapPolygonData)reader.ReadShapeFile(name);

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
                MapPolygonData polygons = (MapPolygonData)reader.ReadShapeFile();

                // Assert
                MapFeature[] features = polygons.Features.ToArray();
                Assert.AreEqual(4, features.Length);

                #region Assertsions for 'polygon1'

                MapFeature polygon1 = features[0];
                MapGeometry[] polygon1Geometry = polygon1.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon1Geometry.Length);

                IEnumerable<Point2D>[] polygon1PointCollections = polygon1Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon1PointCollections.Length);

                Point2D[] polygon1FirstPointCollection = polygon1PointCollections[0].ToArray();
                Assert.AreEqual(6, polygon1FirstPointCollection.Length);
                Assert.AreEqual(-1.070, polygon1FirstPointCollection[2].X, 1e-1);
                Assert.AreEqual(0.066, polygon1FirstPointCollection[2].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon2'

                MapFeature polygon2 = features[1];
                MapGeometry[] polygon2Geometry = polygon2.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon2Geometry.Length);

                IEnumerable<Point2D>[] polygon2PointCollections = polygon2Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon2PointCollections.Length);

                Point2D[] polygon2FirstPointCollection = polygon2PointCollections[0].ToArray();
                Assert.AreEqual(25, polygon2FirstPointCollection.Length);
                Assert.AreEqual(-2.172, polygon2FirstPointCollection[23].X, 1e-1);
                Assert.AreEqual(0.212, polygon2FirstPointCollection[23].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon3'

                MapFeature polygon3 = features[2];
                MapGeometry[] polygon3Geometry = polygon3.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon3Geometry.Length);

                IEnumerable<Point2D>[] polygon3PointCollections = polygon3Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon3PointCollections.Length);

                Point2D[] polygon3FirstPointCollection = polygon3PointCollections[0].ToArray();
                Assert.AreEqual(10, polygon3FirstPointCollection.Length);
                Assert.AreEqual(-1.091, polygon3FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.566, polygon3FirstPointCollection[0].Y, 1e-1);

                #endregion

                #region Assertsions for 'polygon4'

                MapFeature polygon4 = features[3];
                MapGeometry[] polygon4Geometry = polygon4.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon4Geometry.Length);

                IEnumerable<Point2D>[] polygon4PointCollections = polygon4Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon4PointCollections.Length);

                Point2D[] polygon4FirstPointCollection = polygon4PointCollections[0].ToArray();
                Assert.AreEqual(9, polygon4FirstPointCollection.Length);
                Assert.AreEqual(-1.917, polygon4FirstPointCollection[8].X, 1e-1);
                Assert.AreEqual(0.759, polygon4FirstPointCollection[8].Y, 1e-1);

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
