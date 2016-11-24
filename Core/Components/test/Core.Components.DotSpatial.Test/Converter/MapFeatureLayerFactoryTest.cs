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
using System.Linq;
using Core.Common.Base.Geometry;
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
    public class MapFeatureLayerFactoryTest
    {
        [Test]
        public void Create_MapPointData_ReturnMapPointLayer()
        {
            // Setup
            var factory = new MapFeatureLayerFactory();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapPointData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.IsInstanceOf<MapPointLayer>(layer);
        }

        [Test]
        public void Create_MapLineData_ReturnMapLineLayer()
        {
            // Setup
            var factory = new MapFeatureLayerFactory();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapLineData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.IsInstanceOf<MapLineLayer>(layer);
        }

        [Test]
        public void Create_MapPolygonData_ReturnMapPolygonLayer()
        {
            // Setup
            var factory = new MapFeatureLayerFactory();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapPolygonData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.IsInstanceOf<Polygon>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
        }

        [Test]
        public void Create_MapDataCollection_ReturnMapLayersCorrespondingToListItems()
        {
            // Setup
            var factory = new MapFeatureLayerFactory();
            var testData = CreateTestData();
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(new MapPointData("test data")
            {
                Features = testData
            });

            mapDataCollection.Add(new MapLineData("test data")
            {
                Features = testData
            });

            mapDataCollection.Add(new MapPolygonData("test data")
            {
                Features = testData
            });

            var points = testData.First().MapGeometries.First().PointCollections.First().ToArray();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(mapDataCollection);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            Assert.AreEqual(3, layers.Count);

            var layer = layers[0];
            Assert.AreEqual(points.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(testData, layer.DataSet.Features[0].Coordinates);

            layer = layers[1];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.IsInstanceOf<LineString>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);

            layer = layers[2];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.IsInstanceOf<Polygon>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
        }

        [Test]
        public void Create_OtherData_ThrownsNotSupportedException()
        {
            // Setup
            var factory = new MapFeatureLayerFactory();
            var testData = new TestMapData("test data");

            // Call
            TestDelegate test = () => factory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static MapFeature[] CreateTestData()
        {
            return new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.2, 3.4),
                            new Point2D(3.2, 3.4),
                            new Point2D(0.2, 2.4)
                        }
                    })
                })
            };
        }
    }
}