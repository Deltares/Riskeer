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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
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
            string expectedMessage = $"Fout bij het lezen van bestand '{nonLineShapeFile}': kon geen lijnen vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath($"{nameof(PolylineShapeFileReaderTest)}.{nameof(ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new PolylineShapeFileReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
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
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

                // Assert
                Assert.AreEqual(4, count);
            }
        }

        [Test]
        [TestCase("Test lines")]
        [TestCase("Another test with lines")]
        public void ReadLine_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var line = (MapLineData) reader.ReadFeature(name);

                // Assert
                Assert.AreEqual(name, line.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadLine_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var line = (MapLineData) reader.ReadFeature(name);

                // Assert
                Assert.AreEqual("Lijn", line.Name);
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
                var line = (MapLineData) reader.ReadFeature();

                // Assert
                Assert.IsNotNull(line);

                MapFeature[] features = line.Features.ToArray();
                Assert.AreEqual(1, features.Length);

                MapGeometry[] geometries = features[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                IEnumerable<Point2D>[] pointCollections = geometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, pointCollections.Length);

                Point2D[] firstPointCollection = pointCollections[0].ToArray();
                Assert.AreEqual(1669, firstPointCollection.Length);
                Assert.AreEqual(202714.219, firstPointCollection[457].X, 1e-6);
                Assert.AreEqual(507775.781, firstPointCollection[457].Y, 1e-6);

                Assert.AreEqual(6, features[0].MetaData.Count);
                Assert.AreEqual("A", features[0].MetaData["CATEGORIE"]);
                Assert.AreEqual("10", features[0].MetaData["DIJKRING"]);
                Assert.AreEqual(19190.35000000, features[0].MetaData["LENGTE_TRJ"]);
                Assert.AreEqual("1:1000", features[0].MetaData["Ondergrens"]);
                Assert.AreEqual("1:3000", features[0].MetaData["Signalerin"]);
                Assert.AreEqual("10-1", features[0].MetaData["TRAJECT_ID"]);
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
                Assert.AreEqual(4, reader.GetNumberOfFeatures());

                // Call
                var line1 = (MapLineData) reader.ReadFeature();
                var line2 = (MapLineData) reader.ReadFeature();
                var line3 = (MapLineData) reader.ReadFeature();
                var line4 = (MapLineData) reader.ReadFeature();

                // Assert

                #region Assertions for 'line1'

                MapFeature[] features1 = line1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                MapGeometry[] geometries1 = features1[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries1.Length);

                IEnumerable<Point2D>[] line1PointCollections = geometries1[0].PointCollections.ToArray();
                Assert.AreEqual(1, line1PointCollections.Length);

                Point2D[] line1FirstPointCollection = line1PointCollections[0].ToArray();
                Assert.AreEqual(15, line1FirstPointCollection.Length);
                Assert.AreEqual(-1.514151, line1FirstPointCollection[2].X, 1e-6);
                Assert.AreEqual(-0.879717, line1FirstPointCollection[2].Y, 1e-6);

                Assert.AreEqual(1, features1[0].MetaData.Count);
                Assert.AreEqual(4, features1[0].MetaData["id"]);

                #endregion

                #region Assertions for 'line2'

                MapFeature[] features2 = line2.Features.ToArray();
                Assert.AreEqual(1, features2.Length);

                MapGeometry[] geometries2 = features2[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries2.Length);

                IEnumerable<Point2D>[] line2PointCollections = geometries2[0].PointCollections.ToArray();
                Assert.AreEqual(1, line2PointCollections.Length);

                Point2D[] line2FirstPointCollection = line2PointCollections[0].ToArray();
                Assert.AreEqual(6, line2FirstPointCollection.Length);
                Assert.AreEqual(-2.028302, line2FirstPointCollection[3].X, 1e-6);
                Assert.AreEqual(-0.382075, line2FirstPointCollection[3].Y, 1e-6);

                Assert.AreEqual(1, features2[0].MetaData.Count);
                Assert.AreEqual(3, features2[0].MetaData["id"]);

                #endregion

                #region Assertions for 'line3'

                MapFeature[] features3 = line3.Features.ToArray();
                Assert.AreEqual(1, features3.Length);

                MapGeometry[] geometries3 = features3[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries3.Length);

                IEnumerable<Point2D>[] line3PointCollections = geometries3[0].PointCollections.ToArray();
                Assert.AreEqual(1, line3PointCollections.Length);

                Point2D[] line3FirstPointCollection = line3PointCollections[0].ToArray();
                Assert.AreEqual(13, line3FirstPointCollection.Length);
                Assert.AreEqual(0.891509, line3FirstPointCollection[12].X, 1e-6);
                Assert.AreEqual(-0.122641, line3FirstPointCollection[12].Y, 1e-6);

                Assert.AreEqual(1, features3[0].MetaData.Count);
                Assert.AreEqual(2, features3[0].MetaData["id"]);

                #endregion

                #region Assertions for 'line4'

                MapFeature[] features4 = line4.Features.ToArray();
                Assert.AreEqual(1, features4.Length);

                MapGeometry[] geometries4 = features4[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries4.Length);

                IEnumerable<Point2D>[] line4PointCollections = geometries4[0].PointCollections.ToArray();
                Assert.AreEqual(1, line4PointCollections.Length);

                Point2D[] line4FirstPointCollection = line4PointCollections[0].ToArray();
                Assert.AreEqual(6, line4FirstPointCollection.Length);
                Assert.AreEqual(-2.070754, line4FirstPointCollection[0].X, 1e-6);
                Assert.AreEqual(0.73584906, line4FirstPointCollection[0].Y, 1e-6);

                Assert.AreEqual(1, features4[0].MetaData.Count);
                Assert.AreEqual(1, features4[0].MetaData["id"]);

                #endregion
            }
        }

        [Test]
        [TestCase("Test lines")]
        [TestCase("Another test with lines")]
        public void ReadShapeFile_WithName_ApplyNameToMapData(string name)
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var line = (MapLineData) reader.ReadShapeFile(name);

                // Assert
                Assert.AreEqual(name, line.Name);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReadShapeFile_WithoutName_MapDataHasDefaultName(string name)
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var line = (MapLineData) reader.ReadShapeFile(name);

                // Assert
                Assert.AreEqual("Lijn", line.Name);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithOneLineFeature_ReturnShape()
        {
            // Setup
            string shapeWithOneLine = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                 "traject_10-1.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithOneLine))
            {
                // Call
                var line = (MapLineData) reader.ReadShapeFile();

                // Assert
                Assert.IsNotNull(line);

                MapFeature[] features = line.Features.ToArray();
                Assert.AreEqual(1, features.Length);

                MapGeometry[] geometries = features[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                IEnumerable<Point2D>[] pointCollections = geometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, pointCollections.Length);

                Point2D[] firstPointCollection = pointCollections[0].ToArray();
                Assert.AreEqual(1669, firstPointCollection.Length);
                Assert.AreEqual(202714.219, firstPointCollection[457].X, 1e-6);
                Assert.AreEqual(507775.781, firstPointCollection[457].Y, 1e-6);

                Assert.AreEqual(6, features[0].MetaData.Count);
                Assert.AreEqual("A", features[0].MetaData["CATEGORIE"]);
                Assert.AreEqual("10", features[0].MetaData["DIJKRING"]);
                Assert.AreEqual(19190.35000000, features[0].MetaData["LENGTE_TRJ"]);
                Assert.AreEqual("1:1000", features[0].MetaData["Ondergrens"]);
                Assert.AreEqual("1:3000", features[0].MetaData["Signalerin"]);
                Assert.AreEqual("10-1", features[0].MetaData["TRAJECT_ID"]);
                Assert.AreEqual("CATEGORIE", line.SelectedMetaDataAttribute);
            }
        }

        [Test]
        public void ReadShapeFile_ShapeFileWithMultipleLineFeatures_ReturnShapes()
        {
            // Setup
            string shapeWithMultipleLines = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                       "Multiple_PolyLine_with_ID.shp");
            using (var reader = new PolylineShapeFileReader(shapeWithMultipleLines))
            {
                // Precondition
                Assert.AreEqual(4, reader.GetNumberOfFeatures());

                // Call
                var lines = (MapLineData) reader.ReadShapeFile();

                // Assert
                MapFeature[] features = lines.Features.ToArray();
                Assert.AreEqual(4, features.Length);
                Assert.AreEqual(1, lines.MetaData.Count());
                Assert.AreEqual("id", lines.SelectedMetaDataAttribute);

                #region Assertions for 'line1'

                MapGeometry[] geometries1 = features[0].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries1.Length);

                IEnumerable<Point2D>[] line1PointCollections = geometries1[0].PointCollections.ToArray();
                Assert.AreEqual(1, line1PointCollections.Length);

                Point2D[] line1FirstPointCollection = line1PointCollections[0].ToArray();
                Assert.AreEqual(15, line1FirstPointCollection.Length);
                Assert.AreEqual(-1.514151, line1FirstPointCollection[2].X, 1e-6);
                Assert.AreEqual(-0.879717, line1FirstPointCollection[2].Y, 1e-6);

                Assert.AreEqual(1, features[0].MetaData.Count);
                Assert.AreEqual(4, features[0].MetaData["id"]);

                #endregion

                #region Assertions for 'line2'

                MapGeometry[] geometries2 = features[1].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries2.Length);

                IEnumerable<Point2D>[] line2PointCollections = geometries2[0].PointCollections.ToArray();
                Assert.AreEqual(1, line2PointCollections.Length);

                Point2D[] line2FirstPointCollection = line2PointCollections[0].ToArray();
                Assert.AreEqual(6, line2FirstPointCollection.Length);
                Assert.AreEqual(-2.028302, line2FirstPointCollection[3].X, 1e-6);
                Assert.AreEqual(-0.382075, line2FirstPointCollection[3].Y, 1e-6);

                Assert.AreEqual(1, features[1].MetaData.Count);
                Assert.AreEqual(3, features[1].MetaData["id"]);

                #endregion

                #region Assertions for 'line3'

                MapGeometry[] geometries3 = features[2].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries3.Length);

                IEnumerable<Point2D>[] line3PointCollections = geometries3[0].PointCollections.ToArray();
                Assert.AreEqual(1, line3PointCollections.Length);

                Point2D[] line3FirstPointCollection = line3PointCollections[0].ToArray();
                Assert.AreEqual(13, line3FirstPointCollection.Length);
                Assert.AreEqual(0.891509, line3FirstPointCollection[12].X, 1e-6);
                Assert.AreEqual(-0.122641, line3FirstPointCollection[12].Y, 1e-6);

                Assert.AreEqual(1, features[2].MetaData.Count);
                Assert.AreEqual(2, features[2].MetaData["id"]);

                #endregion

                #region Assertions for 'line4'

                MapGeometry[] geometries4 = features[3].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries4.Length);

                IEnumerable<Point2D>[] line4PointCollections = geometries4[0].PointCollections.ToArray();
                Assert.AreEqual(1, line4PointCollections.Length);

                Point2D[] line4FirstPointCollection = line4PointCollections[0].ToArray();
                Assert.AreEqual(6, line4FirstPointCollection.Length);
                Assert.AreEqual(-2.070754, line4FirstPointCollection[0].X, 1e-6);
                Assert.AreEqual(0.73584906, line4FirstPointCollection[0].Y, 1e-6);

                Assert.AreEqual(1, features[3].MetaData.Count);
                Assert.AreEqual(1, features[3].MetaData["id"]);

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
                for (var i = 0; i < reader.GetNumberOfFeatures(); i++)
                {
                    reader.ReadFeature();
                }

                // Call
                var line = reader.ReadFeature() as MapLineData;

                // Assert
                Assert.IsNull(line);
            }
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