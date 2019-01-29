// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Base.TestUtil.Geometry;
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
            string expectedMessage = $"Fout bij het lezen van bestand '{nonPolygonShapeFile}': kon geen polygonen vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath($"{nameof(PolygonShapeFileReaderTest)}.{nameof(ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new PolygonShapeFileReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
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
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void GetNumberOfLines_ShapeFileWithOnePolygonWithHoles_ReturnOne()
        {
            // Setup
            string shapeWithOnePolygonWithHoles = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                             "Single_Polygon_with_two_holes_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygonWithHoles))
            {
                // Call
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

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
                var polygon = (MapPolygonData) reader.ReadFeature(name);

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
                var polygon = (MapPolygonData) reader.ReadFeature(name);

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
                var polygon = (MapPolygonData) reader.ReadFeature();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                Point2D[] firstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(30, firstPointCollection.Length);
                Assert.AreEqual(-0.264, firstPointCollection[25].X, 1e-1);
                Assert.AreEqual(0.169, firstPointCollection[25].Y, 1e-1);
            }
        }

        [Test]
        public void ReadLine_ShapeFileWithOnePolygonWithTwoHolesFeature_ReturnShape()
        {
            // Setup
            string shapeWithOnePolygonWithHoles = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                             "Single_Polygon_with_two_holes_with_ID.shp");
            using (var reader = new PolygonShapeFileReader(shapeWithOnePolygonWithHoles))
            {
                // Call
                var polygon = (MapPolygonData) reader.ReadFeature();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(3, polygonPointCollections.Length);

                var pointComparer = new Point2DComparerWithTolerance(1e-6);

                Point2D[] outerRingPoints = polygonPointCollections[0].ToArray();
                var expectedOuterRingPoints = new[]
                {
                    new Point2D(-866522.534211655, -5517886.97470326),
                    new Point2D(-569923.527795405, -5517539.26191731),
                    new Point2D(-565403.261578042, -5759199.6481533),
                    new Point2D(-865479.395853802, -5759199.6481533),
                    new Point2D(-866522.534211655, -5517886.97470326)
                };
                CollectionAssert.AreEqual(expectedOuterRingPoints, outerRingPoints,
                                          pointComparer);

                Point2D[] innerRing1Points = polygonPointCollections[1].ToArray();
                var expectedInnerRing1Points = new[]
                {
                    new Point2D(-829317.266114892, -5539445.16743223),
                    new Point2D(-831055.830044648, -5604119.74561913),
                    new Point2D(-746213.91027259, -5604815.17119103),
                    new Point2D(-747257.048630444, -5538749.74186033),
                    new Point2D(-829317.266114892, -5539445.16743223)
                };
                CollectionAssert.AreEqual(expectedInnerRing1Points, innerRing1Points,
                                          pointComparer);

                Point2D[] innerRing2Points = polygonPointCollections[2].ToArray();
                var expectedInnerRing2Points = new[]
                {
                    new Point2D(-715615.185108898, -5673314.59002339),
                    new Point2D(-657547.149855071, -5731730.33806316),
                    new Point2D(-591829.433310322, -5686875.38867548),
                    new Point2D(-648506.617420344, -5624634.79999024),
                    new Point2D(-715615.185108898, -5673314.59002339)
                };
                CollectionAssert.AreEqual(expectedInnerRing2Points, innerRing2Points,
                                          pointComparer);
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
                var polygon = (MapPolygonData) reader.ReadFeature();

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
                Assert.AreEqual(4, reader.GetNumberOfFeatures());

                // Call
                var polygons1 = (MapPolygonData) reader.ReadFeature();
                var polygons2 = (MapPolygonData) reader.ReadFeature();
                var polygons3 = (MapPolygonData) reader.ReadFeature();
                var polygons4 = (MapPolygonData) reader.ReadFeature();

                // Assert

                #region Assertsions for 'polygon1'

                MapFeature[] features1 = polygons1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                MapFeature polygon1 = features1[0];
                MapGeometry[] polygon1Geometry = polygon1.MapGeometries.ToArray();
                Assert.AreEqual(1, polygon1Geometry.Length);

                IEnumerable<Point2D>[] polygon1PointCollections = polygon1Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygon1PointCollections.Length);

                Point2D[] polygon1PointCollection = polygon1PointCollections[0].ToArray();
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

                Point2D[] polygon4PointCollection = polygon4PointCollections[0].ToArray();
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
                var polygon = (MapPolygonData) reader.ReadShapeFile();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(1, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                Point2D[] polygonFirstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(30, polygonFirstPointCollection.Length);
                Assert.AreEqual(-0.264, polygonFirstPointCollection[25].X, 1e-1);
                Assert.AreEqual(0.169, polygonFirstPointCollection[25].Y, 1e-1);

                Assert.AreEqual(1, polygonFeatures[0].MetaData.Count);
                Assert.AreEqual(76, polygonFeatures[0].MetaData["id"]);
                Assert.AreEqual("id", polygon.SelectedMetaDataAttribute);
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
                var polygon = (MapPolygonData) reader.ReadShapeFile();

                // Assert
                Assert.IsNotNull(polygon);
                MapFeature[] polygonFeatures = polygon.Features.ToArray();
                Assert.AreEqual(1, polygonFeatures.Length);

                MapGeometry[] polygonGeometries = polygonFeatures[0].MapGeometries.ToArray();
                Assert.AreEqual(2, polygonGeometries.Length);

                IEnumerable<Point2D>[] polygonPointCollections = polygonGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, polygonPointCollections.Length);

                Point2D[] firstPointCollection = polygonPointCollections[0].ToArray();
                Assert.AreEqual(7, firstPointCollection.Length);
                Assert.AreEqual(-2.257, firstPointCollection[4].X, 1e-1);
                Assert.AreEqual(0.419, firstPointCollection[4].Y, 1e-1);

                Assert.AreEqual(1, polygonFeatures[0].MetaData.Count);
                Assert.AreEqual(1, polygonFeatures[0].MetaData["id"]);
                Assert.AreEqual("id", polygon.SelectedMetaDataAttribute);
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
                var polygon = (MapPolygonData) reader.ReadShapeFile(name);

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
                var polygon = (MapPolygonData) reader.ReadShapeFile(name);

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
                Assert.AreEqual(4, reader.GetNumberOfFeatures());

                // Call
                var polygons = (MapPolygonData) reader.ReadShapeFile();

                // Assert
                MapFeature[] features = polygons.Features.ToArray();
                Assert.AreEqual(4, features.Length);
                Assert.AreEqual(1, polygons.MetaData.Count());
                Assert.AreEqual("id", polygons.SelectedMetaDataAttribute);

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

                Assert.AreEqual(1, polygon1.MetaData.Count);
                Assert.AreEqual(4, polygon1.MetaData["id"]);

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

                Assert.AreEqual(1, polygon2.MetaData.Count);
                Assert.AreEqual(3, polygon2.MetaData["id"]);

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

                Assert.AreEqual(1, polygon3.MetaData.Count);
                Assert.AreEqual(2, polygon3.MetaData["id"]);

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

                Assert.AreEqual(1, polygon4.MetaData.Count);
                Assert.AreEqual(1, polygon4.MetaData["id"]);

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
                for (var i = 0; i < reader.GetNumberOfFeatures(); i++)
                {
                    reader.ReadFeature();
                }

                // Call
                var polygon = reader.ReadFeature() as MapPolygonData;

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