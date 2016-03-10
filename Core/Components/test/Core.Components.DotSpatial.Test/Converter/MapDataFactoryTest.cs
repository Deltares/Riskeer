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
    public class MapDataFactoryTest
    {
        [Test]
        public void Create_MapPointData_ReturnMapPointLayer()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapPointData(testData, "test data"));

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.IsInstanceOf<MapPointLayer>(layer);
        }

        [Test]
        public void Create_MapLineData_ReturnMapLineLayer()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapLineData(testData, "test data"));

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.IsInstanceOf<MapLineLayer>(layer);
        }

        [Test]
        public void Create_MapPolygonData_ReturnMapPolygonLayer()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<IMapFeatureLayer> layers = factory.Create(new MapPolygonData(testData, "test data"));

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            var layer = layers[0];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.IsInstanceOf<MultiPolygon>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
        }

        [Test]
        public void Create_MapDataCollection_ReturnMapLayersCorrespondingToListItems()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();
            var mapDataCollection = new MapDataCollection(new List<MapData>
            {
                new MapPointData(testData, "test data"),
                new MapLineData(testData, "test data"),
                new MapPolygonData(testData, "test data")
            }, "test data");

            var points = testData.First().MapGeometries.First().Points.ToArray();

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
            Assert.IsInstanceOf<MultiLineString>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);

            layer = layers[2];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.IsInstanceOf<MultiPolygon>(layer.DataSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
        }

        [Test]
        public void Create_OtherData_ThrownsNotSupportedException()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = new TestMapData("test data");

            // Call
            TestDelegate test = () => factory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static List<MapFeature> CreateTestData()
        {
            return new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(1.2, 3.4),
                        new Point2D(3.2, 3.4),
                        new Point2D(0.2, 2.4)
                    })
                })
            };
        }
    }
}