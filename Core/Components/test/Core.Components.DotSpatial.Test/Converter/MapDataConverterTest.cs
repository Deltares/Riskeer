using System;
using System.Collections.Generic;
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
    public class MapDataConverterTest
    {
        [Test]
        public void TupleToDataPoint_RandomTupleDoubleDouble_ReturnsCoordinate()
        {
            // Setup
            var random = new Random(21);
            var a = random.NextDouble();
            var b = random.NextDouble();
            var tuple = new Tuple<double, double>(a, b);
            var testConverter = new TestMapDataConverter<MapData>();

            // Call
            var coordinate = testConverter.PublicTupleToCoordinate(tuple);

            // Assert
            Assert.AreEqual(a, coordinate.X);
            Assert.AreEqual(b, coordinate.Y);
        }

        [Test]
        public void CanConvertMapData_DifferentInherritingTypes_OnlySupportsExactType()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();

            // Call
            var mapDataResult = testConverter.CanConvertMapData(new TestMapData());
            var classResult = testConverter.CanConvertMapData(new Class());
            var childResult = testConverter.CanConvertMapData(new Child());

            // Assert
            Assert.IsFalse(mapDataResult);
            Assert.IsTrue(classResult);
            Assert.IsFalse(childResult);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();
            var testMapData = new TestMapData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        private class Class : MapData {}

        private class Child : Class {}

        private class TestMapDataConverter<T> : MapDataConverter<T> where T : MapData
        {
            protected override IList<FeatureSet> Convert(T data)
            {
                throw new NotImplementedException();
            }

            public Coordinate PublicTupleToCoordinate(Tuple<double, double> obj)
            {
                return new Coordinate(obj.Item1, obj.Item2);
            }
        }
    }
}
