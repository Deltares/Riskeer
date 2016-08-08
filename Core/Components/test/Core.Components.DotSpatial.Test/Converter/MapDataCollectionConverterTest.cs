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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataCollectionConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapDataConverter()
        {
            // Call
            var converter = new MapDataCollectionConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapDataCollection>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapDataCollection_ReturnsTrue()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
            var collectionData = new MapDataCollection("test data");

            // Call
            var canConvert = converter.CanConvertMapData(collectionData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_MapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
            var mapData = new TestMapData("test data");

            // Call
            var canConvert = converter.CanConvertMapData(mapData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new MapDataCollectionConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapDataCollectionConverter();
            var testMapData = new TestMapData("test data");
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Convert_CollectionOfPointAndLineData_ReturnsTwoNewIMapFeatureLayers()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new List<Point2D>();
            var linePoints = new List<Point2D>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
                linePoints.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var pointFeatures = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        points
                    })
                })
            };

            var lineFeatures = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        linePoints
                    })
                })
            };

            var collectionData = new MapDataCollection("test data");
            var pointData = new MapPointData("test data")
            {
                Features = pointFeatures
            };
            var lineData = new MapLineData("test data")
            {
                Features = lineFeatures
            };
            collectionData.Add(pointData);
            collectionData.Add(lineData);

            // Call
            var layers = converter.Convert(collectionData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            Assert.AreEqual(2, layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(layers[0]);
            Assert.AreEqual(FeatureType.Point, layers[0].DataSet.FeatureType);
            Assert.IsInstanceOf<MapLineLayer>(layers[1]);
            Assert.AreEqual(FeatureType.Line, layers[1].DataSet.FeatureType);
        }
    }
}