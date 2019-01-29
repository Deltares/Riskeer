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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Readers;
using DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Readers
{
    [TestFixture]
    public class ShapeFileReaderBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             "traject_10-1.shp");

            // Call
            using (var reader = new TestShapeFileReaderBase(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
                Assert.AreEqual(testFilePath, reader.GetFilePath);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ParameteredConstructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FileDoesNotExist_ThrowArgumentException()
        {
            // Call
            string pathToNotExistingShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                           "I_do_not_exist.shp");
            TestDelegate call = () => new TestShapeFileReaderBase(pathToNotExistingShapeFile);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{pathToNotExistingShapeFile}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "traject_10-1.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidPathChars[0].ToString());

            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void CopyMetaDataIntoFeature_AllValidValues_MetaDataAddedToFeature()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "traject_10-1.shp");

            var reader = new TestShapeFileReaderBase(validFilePath);
            var targetFeature = new MapFeature(new MapGeometry[0]);

            // Call
            reader.PublicCopyMetaDataIntoFeature(targetFeature, 0);

            // Assert
            Assert.AreEqual("A", targetFeature.MetaData["CATEGORIE"]);
            Assert.AreEqual("10", targetFeature.MetaData["DIJKRING"]);
            Assert.AreEqual(19190.35, targetFeature.MetaData["LENGTE_TRJ"]);
            Assert.AreEqual("1:1000", targetFeature.MetaData["Ondergrens"]);
            Assert.AreEqual("1:3000", targetFeature.MetaData["Signalerin"]);
            Assert.AreEqual("10-1", targetFeature.MetaData["TRAJECT_ID"]);
        }

        [Test]
        public void CopyMetaDataIntoFeature_MultipleFeaturesAllValidValues_MetaDataAddedToFeature()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Multiple_Polygon_with_ID.shp");

            var reader = new TestShapeFileReaderBase(validFilePath);
            var features = new List<MapFeature>();

            for (var i = 0; i < 4; i++)
            {
                var targetFeature = new MapFeature(new MapGeometry[0]);

                // Call
                reader.PublicCopyMetaDataIntoFeature(targetFeature, i);

                features.Add(targetFeature);
            }

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                4,
                3,
                2,
                1
            }, features.Select(f => f.MetaData["id"]));
        }

        [Test]
        public void CopyMetaDataIntoFeature_DBNullValuesForColumn_MetaDataWithNullValueForAttribute()
        {
            // Setup
            string fileWithDbNullValues = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Shape_DBNullSignalerinValue.shp");

            var reader = new TestShapeFileReaderBase(fileWithDbNullValues);
            var targetFeature = new MapFeature(new MapGeometry[0]);

            // Call
            reader.PublicCopyMetaDataIntoFeature(targetFeature, 0);

            // Assert
            Assert.IsNull(targetFeature.MetaData["Signalerin"]);
        }

        [Test]
        public void CopyMetaDataIntoFeature_DBNullValuesForEachType_MetaDataWithNullValues()
        {
            // Setup
            string fileWithDbNullValues = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Attribute_EachTypeNullValue_QGis.shp");

            var reader = new TestShapeFileReaderBase(fileWithDbNullValues);
            var targetFeature = new MapFeature(new MapGeometry[0]);

            // Call
            reader.PublicCopyMetaDataIntoFeature(targetFeature, 3);

            // Assert
            Assert.IsNull(targetFeature.MetaData["attInteg2"]);
            Assert.IsNull(targetFeature.MetaData["attInteg64"]);
            Assert.IsNull(targetFeature.MetaData["attDecimal"]);
            Assert.IsNull(targetFeature.MetaData["attString"]);
            Assert.IsNull(targetFeature.MetaData["attDate"]);
        }

        private class TestShapeFileReaderBase : ShapeFileReaderBase
        {
            public TestShapeFileReaderBase(string filePath) : base(filePath)
            {
                ShapeFile = Shapefile.OpenFile(filePath);
            }

            public string GetFilePath
            {
                get
                {
                    return FilePath;
                }
            }

            public Shapefile GetShapeFile
            {
                get
                {
                    return ShapeFile;
                }
            }

            public void PublicCopyMetaDataIntoFeature(MapFeature targetFeature, int sourceFeatureIndex)
            {
                CopyMetaDataIntoFeature(targetFeature, sourceFeatureIndex);
            }

            public override FeatureBasedMapData ReadFeature(string name = null)
            {
                return null;
            }

            public override FeatureBasedMapData ReadShapeFile(string name = null)
            {
                return null;
            }

            public override IFeature GetFeature(int index)
            {
                return null;
            }
        }
    }
}