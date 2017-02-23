﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Properties;
using Core.Components.Gis.IO.Writers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Writers
{
    [TestFixture]
    public class PointShapeFileWriterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var writer = new PointShapeFileWriter())
            {
                // Assert
                Assert.IsInstanceOf<ShapeFileWriterBase>(writer);
            }
        }

        [Test]
        public void CopyToFeature_FeatureContainsNoGeometries_ThrowsArgumentException()
        {
            // Setup
            MapFeature feature = new MapFeature(new MapGeometry[0]);

            MapPointData mapData = new MapPointData("test")
            {
                Features = new[]
                {
                    feature
                }
            };

            using (var writer = new PointShapeFileWriter())
            {
                // Call
                TestDelegate call = () => writer.CopyToFeature(mapData);

                // Assert
                var expectedMessage = Resources.PointShapeFileWriter_CreatePointFromMapFeature_A_feature_can_only_contain_one_geometry;
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        [Test]
        public void CopyToFeature_FeatureContainsMultipleGeometries_ThrowsArgumentException()
        {
            // Setup
            MapFeature feature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    Enumerable.Empty<Point2D>()
                }),
                new MapGeometry(new[]
                {
                    Enumerable.Empty<Point2D>()
                })
            });

            MapPointData mapData = new MapPointData("test")
            {
                Features = new[]
                {
                    feature
                }
            };

            using (var writer = new PointShapeFileWriter())
            {
                // Call
                TestDelegate call = () => writer.CopyToFeature(mapData);

                // Assert
                var expectedMessage = Resources.PointShapeFileWriter_CreatePointFromMapFeature_A_feature_can_only_contain_one_geometry;
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        [Test]
        public void CopyToFeature_InconsistentMetaDataBetweenMapPointDatas_ThrowArgumentException()
        {
            // Setup
            MapFeature[] features1 = CreateFeatures(0.0);
            var mapPointData1 = new MapPointData("test data 1")
            {
                Features = features1
            };
            mapPointData1.Features.First().MetaData["firstKey"] = 123;
            mapPointData1.Features.First().MetaData["secondKey"] = "aValue";

            MapFeature[] features2 = CreateFeatures(10.0);
            var mapPointData2 = new MapPointData("test data 2")
            {
                Features = features2
            };
            mapPointData2.Features.First().MetaData["firstKey"] = 123;
            mapPointData2.Features.First().MetaData["anotherKey"] = "anotherValue";

            using (var writer = new PointShapeFileWriter())
            {
                writer.CopyToFeature(mapPointData1);

                // Call
                TestDelegate call = () => writer.CopyToFeature(mapPointData2);

                // Assert
                var message = "Column 'anotherKey' does not belong to table .";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            }
        }

        [Test]
        public void SaveAs_ValidMapPointData_WritesShapeFile()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(SaveAs_ValidMapPointData_WritesShapeFile));
            string filePath = Path.Combine(directoryPath, "test.shp");
            var baseName = "test";

            MapFeature[] features = CreateFeatures(0.0);

            var mapPointData = new MapPointData("test data")
            {
                Features = features
            };

            mapPointData.Features.First().MetaData["<some key>"] = 123;

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(SaveAs_ValidMapPointData_WritesShapeFile)))
            using (var writer = new PointShapeFileWriter())
            {
                writer.CopyToFeature(mapPointData);

                // Call
                writer.SaveAs(filePath);

                // Assert
                string pathName = Path.Combine(directoryPath, baseName);
                Assert.IsTrue(File.Exists(pathName + ".shp"));
                Assert.IsTrue(File.Exists(pathName + ".shx"));
                Assert.IsTrue(File.Exists(pathName + ".dbf"));
            }
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
                            new Point2D(seed, seed)
                        }
                    })
                })
            };
        }
    }
}