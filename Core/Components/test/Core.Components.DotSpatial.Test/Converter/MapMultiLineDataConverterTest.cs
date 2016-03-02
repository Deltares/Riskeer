using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapMultiLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapMultiLineDataConverter()
        {
            // Call
            var converter = new MapMultiLineDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapMultiLineData>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapLineData_ReturnTrue()
        {
            // Setup
            var converter = new MapMultiLineDataConverter();
            var lineData = new MapMultiLineData(Enumerable.Empty<IEnumerable<Point2D>>(), "test data");

            // Call
            var canConvert = converter.CanConvertMapData(lineData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_TestMapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapMultiLineDataConverter();
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
            var testConverter = new MapMultiLineDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapMultiLineDataConverter();
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
        public void Convert_RandomMultiLineData_ReturnsNewMapLineLayerList()
        {
            // Setup
            var converter = new MapMultiLineDataConverter();
            var random = new Random(21);
            var pointCount = random.Next(5, 10);
            var lineCount = random.Next(5, 10);
            var points = new Collection<Point2D>();
            var lines = new Collection<IEnumerable<Point2D>>();

            for (int li = 0; li < lineCount; li++)
            {
                for (int pi = 0; pi < pointCount; pi++)
                {
                    points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
                }
                lines.Add(points);
            }

            var lineData = new MapMultiLineData(lines, "test data");

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(lineCount, layer.DataSet.Features.Count);
        }
    }
}