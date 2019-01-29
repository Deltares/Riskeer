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
            string expectedMessage = $"Fout bij het lezen van bestand '{nonPointShapeFile}': kon geen punten vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_InvalidFilePath_ThrowCriticalFileReadException()
        {
            // Setup
            string nonExistingPointShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                          "NonExistingFile");

            // Call 
            TestDelegate call = () => new PointShapeFileReader(nonExistingPointShapeFile);

            // Assert 
            string expectedMessage = $"Fout bij het lezen van bestand '{nonExistingPointShapeFile}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_ShapeFileIsEmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string emptyPointShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                    "EmptyFile.shp");

            // Call
            TestDelegate call = () => new PointShapeFileReader(emptyPointShapeFile);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{emptyPointShapeFile}': kon geen punten vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_ShapeFileIsCorruptFile_ThrowCriticalFileReadException()
        {
            // Setup
            string corruptPointShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                      "CorruptFile.shp");

            // Call
            TestDelegate call = () => new PointShapeFileReader(corruptPointShapeFile);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{corruptPointShapeFile}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath($"{nameof(PointShapeFileReaderTest)}.{nameof(ParameteredConstructor_ShapeFileIsInUse_ThrowsCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new PointShapeFileReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
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
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

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
                int count = reader.GetNumberOfFeatures();

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
                var pointData = (MapPointData) reader.ReadFeature(name);

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
                var pointData = (MapPointData) reader.ReadFeature(name);

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
                var pointData = reader.ReadFeature() as MapPointData;

                // Assert
                Assert.IsNotNull(pointData);
                Assert.AreEqual(1, pointData.Features.Count());

                MapGeometry[] mapGeometries = pointData.Features.First().MapGeometries.ToArray();
                Assert.AreEqual(1, mapGeometries.Length);

                IEnumerable<Point2D>[] pointCollections = mapGeometries[0].PointCollections.ToArray();
                Assert.AreEqual(1, pointCollections.Length);

                Point2D[] firstPointCollection = pointCollections[0].ToArray();
                Assert.AreEqual(1, firstPointCollection.Length);
                Assert.AreEqual(1.705, firstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.922, firstPointCollection[0].Y, 1e-1);
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
                Assert.AreEqual(6, reader.GetNumberOfFeatures());

                // Call
                var points1 = (MapPointData) reader.ReadFeature();
                var points2 = (MapPointData) reader.ReadFeature();
                var points3 = (MapPointData) reader.ReadFeature();
                var points4 = (MapPointData) reader.ReadFeature();
                var points5 = (MapPointData) reader.ReadFeature();
                var points6 = (MapPointData) reader.ReadFeature();

                // Assert

                #region Assertion for 'point1'

                MapFeature[] features1 = points1.Features.ToArray();
                Assert.AreEqual(1, features1.Length);

                MapFeature point1 = features1[0];
                MapGeometry[] point1Geometry = point1.MapGeometries.ToArray();
                Assert.AreEqual(1, point1Geometry.Length);

                IEnumerable<Point2D>[] point1PointCollections = point1Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point1PointCollections.Length);

                Point2D[] point1FirstPointCpllection = point1PointCollections[0].ToArray();
                Assert.AreEqual(1, point1FirstPointCpllection.Length);
                Assert.AreEqual(-1.750, point1FirstPointCpllection[0].X, 1e-1);
                Assert.AreEqual(-0.488, point1FirstPointCpllection[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point2'

                MapFeature[] features2 = points2.Features.ToArray();
                Assert.AreEqual(1, features2.Length);

                MapFeature point2 = features2[0];
                MapGeometry[] point2Geometry = point2.MapGeometries.ToArray();
                Assert.AreEqual(1, point2Geometry.Length);

                IEnumerable<Point2D>[] point2PointCollections = point2Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point2PointCollections.Length);

                Point2D[] point2FirstPointCollection = point2PointCollections[0].ToArray();
                Assert.AreEqual(1, point2FirstPointCollection.Length);
                Assert.AreEqual(-0.790, point2FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(-0.308, point2FirstPointCollection[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point3'

                MapFeature[] features3 = points3.Features.ToArray();
                Assert.AreEqual(1, features3.Length);

                MapFeature point3 = features3[0];
                MapGeometry[] point3Geometry = point3.MapGeometries.ToArray();
                Assert.AreEqual(1, point3Geometry.Length);

                IEnumerable<Point2D>[] point3PointCollections = point3Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point3PointCollections.Length);

                Point2D[] point3FirstPointCollection = point3PointCollections[0].ToArray();
                Assert.AreEqual(1, point3FirstPointCollection.Length);
                Assert.AreEqual(0.740, point3FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(-0.577, point3FirstPointCollection[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point4'

                MapFeature[] features4 = points4.Features.ToArray();
                Assert.AreEqual(1, features4.Length);

                MapFeature point4 = features4[0];
                MapGeometry[] point4Geometry = point4.MapGeometries.ToArray();
                Assert.AreEqual(1, point4Geometry.Length);

                IEnumerable<Point2D>[] point4PointCollections = point4Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point4PointCollections.Length);

                Point2D[] point4FirstPointCollection = point4PointCollections[0].ToArray();
                Assert.AreEqual(1, point4FirstPointCollection.Length);
                Assert.AreEqual(0.787, point4FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.759, point4FirstPointCollection[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point5'

                MapFeature[] features5 = points5.Features.ToArray();
                Assert.AreEqual(1, features5.Length);

                MapFeature point5 = features5[0];
                MapGeometry[] point5Geometry = point5.MapGeometries.ToArray();
                Assert.AreEqual(1, point5Geometry.Length);

                IEnumerable<Point2D>[] point5PointCollections = point5Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point5PointCollections.Length);

                Point2D[] point5FirstPointCollection = point5PointCollections[0].ToArray();
                Assert.AreEqual(1, point5FirstPointCollection.Length);
                Assert.AreEqual(-0.544, point5FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.283, point5FirstPointCollection[0].Y, 1e-1);

                #endregion

                #region Assertion for 'point6'

                MapFeature[] features6 = points6.Features.ToArray();
                Assert.AreEqual(1, features6.Length);

                MapFeature point6 = features6[0];
                MapGeometry[] point6Geometry = point6.MapGeometries.ToArray();
                Assert.AreEqual(1, point6Geometry.Length);

                IEnumerable<Point2D>[] point6PointCollections = point6Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point6PointCollections.Length);

                Point2D[] point6FirstPointCollection = point6PointCollections[0].ToArray();
                Assert.AreEqual(1, point6FirstPointCollection.Length);
                Assert.AreEqual(-2.066, point6FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.827, point6FirstPointCollection[0].Y, 1e-1);

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
                var pointData = (MapPointData) reader.ReadShapeFile(name);

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
                var pointData = (MapPointData) reader.ReadShapeFile(name);

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
                Assert.AreEqual(6, reader.GetNumberOfFeatures());

                // Call
                var points = (MapPointData) reader.ReadShapeFile();

                // Assert
                MapFeature[] features = points.Features.ToArray();
                Assert.AreEqual(6, features.Length);
                Assert.AreEqual("id", points.SelectedMetaDataAttribute);

                #region Assertion for 'point1'

                MapFeature point1 = features[0];
                MapGeometry[] point1Geometry = point1.MapGeometries.ToArray();
                Assert.AreEqual(1, point1Geometry.Length);

                IEnumerable<Point2D>[] point1PointCollections = point1Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point1PointCollections.Length);

                Point2D[] point1FirstPointCollection = point1PointCollections[0].ToArray();
                Assert.AreEqual(1, point1FirstPointCollection.Length);
                Assert.AreEqual(-1.750, point1FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(-0.488, point1FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point1.MetaData.Count);
                Assert.AreEqual(6, point1.MetaData["id"]);

                #endregion

                #region Assertion for 'point2'

                MapFeature point2 = features[1];
                MapGeometry[] point2Geometry = point2.MapGeometries.ToArray();
                Assert.AreEqual(1, point2Geometry.Length);

                IEnumerable<Point2D>[] point2PointCollections = point2Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point2PointCollections.Length);

                Point2D[] point2FirstPointCollection = point2PointCollections[0].ToArray();
                Assert.AreEqual(1, point2FirstPointCollection.Length);
                Assert.AreEqual(-0.790, point2FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(-0.308, point2FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point2.MetaData.Count);
                Assert.AreEqual(5, point2.MetaData["id"]);

                #endregion

                #region Assertion for 'point3'

                MapFeature point3 = features[2];
                MapGeometry[] point3Geometry = point3.MapGeometries.ToArray();
                Assert.AreEqual(1, point3Geometry.Length);

                IEnumerable<Point2D>[] point3PointCollections = point3Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point3PointCollections.Length);

                Point2D[] point3FirstPointCollection = point3PointCollections[0].ToArray();
                Assert.AreEqual(1, point3FirstPointCollection.Length);
                Assert.AreEqual(0.740, point3FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(-0.577, point3FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point3.MetaData.Count);
                Assert.AreEqual(4, point3.MetaData["id"]);

                #endregion

                #region Assertion for 'point4'

                MapFeature point4 = features[3];
                MapGeometry[] point4Geometry = point4.MapGeometries.ToArray();
                Assert.AreEqual(1, point4Geometry.Length);

                IEnumerable<Point2D>[] point4PointCollections = point4Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point4PointCollections.Length);

                Point2D[] point4FirstPointCollection = point4PointCollections[0].ToArray();
                Assert.AreEqual(1, point4FirstPointCollection.Length);
                Assert.AreEqual(0.787, point4FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.759, point4FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point4.MetaData.Count);
                Assert.AreEqual(3, point4.MetaData["id"]);

                #endregion

                #region Assertion for 'point5'

                MapFeature point5 = features[4];
                MapGeometry[] point5Geometry = point5.MapGeometries.ToArray();
                Assert.AreEqual(1, point5Geometry.Length);

                IEnumerable<Point2D>[] point5PointCollections = point5Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point5PointCollections.Length);

                Point2D[] point5FirstPointCollection = point5PointCollections[0].ToArray();
                Assert.AreEqual(1, point5FirstPointCollection.Length);
                Assert.AreEqual(-0.544, point5FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.283, point5FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point5.MetaData.Count);
                Assert.AreEqual(2, point5.MetaData["id"]);

                #endregion

                #region Assertion for 'point6'

                MapFeature point6 = features[5];
                MapGeometry[] point6Geometry = point6.MapGeometries.ToArray();
                Assert.AreEqual(1, point6Geometry.Length);

                IEnumerable<Point2D>[] point6PointCollections = point6Geometry[0].PointCollections.ToArray();
                Assert.AreEqual(1, point6PointCollections.Length);

                Point2D[] point6FirstPointCollection = point6PointCollections[0].ToArray();
                Assert.AreEqual(1, point6FirstPointCollection.Length);
                Assert.AreEqual(-2.066, point6FirstPointCollection[0].X, 1e-1);
                Assert.AreEqual(0.827, point6FirstPointCollection[0].Y, 1e-1);

                Assert.AreEqual(1, point6.MetaData.Count);
                Assert.AreEqual(1, point6.MetaData["id"]);

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
                for (var i = 0; i < reader.GetNumberOfFeatures(); i++)
                {
                    reader.ReadFeature();
                }

                // Call
                var feature = reader.ReadFeature() as MapPointData;

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