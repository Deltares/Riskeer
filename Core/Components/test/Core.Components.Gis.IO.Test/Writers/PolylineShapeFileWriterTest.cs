// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Writers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Writers
{
    [TestFixture]
    public class PolylineShapeFileWriterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var writer = new PolylineShapeFileWriter())
            {
                // Assert
                Assert.IsInstanceOf<ShapeFileWriterBase<MapLineData>>(writer);
            }
        }

        [Test]
        public void AddFeature_InconsistentMetaDataBetweenMapLineDatas_ThrowArgumentException()
        {
            // Setup
            MapFeature[] features1 = CreateFeatures(0.0);
            var mapLineData1 = new MapLineData("test data 1")
            {
                Features = features1
            };
            mapLineData1.Features.First().MetaData["firstKey"] = 123;
            mapLineData1.Features.First().MetaData["secondKey"] = "aValue";

            MapFeature[] features2 = CreateFeatures(10.0);
            var mapLineData2 = new MapLineData("test data 2")
            {
                Features = features2
            };
            mapLineData2.Features.First().MetaData["firstKey"] = 123;
            mapLineData2.Features.First().MetaData["anotherKey"] = "anotherValue";

            using (var writer = new PolylineShapeFileWriter())
            {
                writer.AddFeature(mapLineData1);

                // Call
                TestDelegate call = () => writer.AddFeature(mapLineData2);

                // Assert
                Assert.Throws<ArgumentException>(call);
            }
        }

        [Test]
        public void SaveAs_ValidMapLineData_WritesShapeFile()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "SaveAs_ValidMapLineData_WritesShapeFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            var baseName = "test";

            MapFeature[] features = CreateFeatures(0.0);

            var mapLineData = new MapLineData("test data")
            {
                Features = features
            };

            mapLineData.Features.First().MetaData["<some key>"] = 123;

            try
            {
                using (var writer = new PolylineShapeFileWriter())
                {
                    writer.AddFeature(mapLineData);

                    // Precondition
                    AssertEssentialShapefileExists(directoryPath, baseName, false);

                    // Call
                    writer.SaveAs(filePath);

                    // Assert
                    AssertEssentialShapefileExists(directoryPath, baseName, true);
                    AssertEssentialShapefileMd5Hashes(directoryPath, baseName);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        private void AssertEssentialShapefileMd5Hashes(string directoryPath, string baseName)
        {
            string refPathName = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO), "md5_reference");
            string pathName = Path.Combine(directoryPath, baseName);

            Assert.AreEqual(MD5.Create().ComputeHash(File.ReadAllBytes(refPathName + ".shp")),
                            MD5.Create().ComputeHash(File.ReadAllBytes(pathName + ".shp")));
            Assert.AreEqual(MD5.Create().ComputeHash(File.ReadAllBytes(refPathName + ".shx")),
                            MD5.Create().ComputeHash(File.ReadAllBytes(pathName + ".shx")));

            int headerLength = 32;
            int bodyLength = 45;
            var refContent = File.ReadAllBytes(refPathName + ".dbf");
            var content = File.ReadAllBytes(pathName + ".dbf");
            Assert.AreEqual(headerLength + bodyLength, content.Length);
            Assert.AreEqual(refContent.Skip(headerLength).Take(45),
                            content.Skip(headerLength).Take(45));
        }

        private static MapFeature[] CreateFeatures(double seed)
        {
            return new[]
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(seed, seed),
                            new Point2D(seed + 1, seed + 1),
                            new Point2D(seed + 2, seed + 2)
                        }
                    })
                })
            };
        }

        private static void AssertEssentialShapefileExists(string directoryPath, string baseName, bool shouldExist)
        {
            string pathName = Path.Combine(directoryPath, baseName);
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shp"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shx"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".dbf"));
        }
    }
}