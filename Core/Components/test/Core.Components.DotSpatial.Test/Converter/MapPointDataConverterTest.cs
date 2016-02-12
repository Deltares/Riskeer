using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.TestUtil;
using DotSpatial.Data;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapPointDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapPointDataConverter()
        {
            // Call
            var converter = new MapPointDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapPointData>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapPointData_ReturnsTrue()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var pointData = new MapPointData(new Collection<Tuple<double, double>>());

            // Call
            var canConvert = converter.CanConvertMapData(pointData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_TestMapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapData = new TestMapData();

            // Call
            var canConvert = converter.CanConvertMapData(mapData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewFeatureSetList()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var pointData = new MapPointData(points);

            // Call
            var featureSets = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<FeatureSet>>(featureSets);
            var featureSet = featureSets[0];
            Assert.AreEqual(pointData.Points.ToArray().Length, featureSet.Features.Count);
            Assert.IsInstanceOf<FeatureSet>(featureSet);
            Assert.AreEqual(FeatureType.Point, featureSet.FeatureType);
            CollectionAssert.AreNotEqual(pointData.Points, featureSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new MapPointDataConverter();

            TestDelegate test = () => testConverter.Convert(null);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("Null data cannot be converted into feature sets.", message);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapPointDataConverter();
            var testMapData = new TestMapData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}