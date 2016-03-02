using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataConverterTest
    {
        [Test]
        public void CanConvertMapData_DifferentInherritingTypes_OnlySupportsExactType()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();

            // Call
            var mapDataResult = testConverter.CanConvertMapData(new TestMapData("test data"));
            var classResult = testConverter.CanConvertMapData(new Class("test data"));
            var childResult = testConverter.CanConvertMapData(new Child("test data"));

            // Assert
            Assert.IsFalse(mapDataResult);
            Assert.IsTrue(classResult);
            Assert.IsTrue(childResult);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestMapDataConverter<Class>();
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
        public void Convert_DataCanBeConverted_ReturnsResult()
        {
            // Setup
            var testConverter = new TestMapDataConverter<TestMapData>();
            var testMapData = new TestMapData("test data");

            // Precondition
            Assert.IsTrue(testConverter.CanConvertMapData(testMapData));

            // Call
            var result = testConverter.Convert(testMapData);

            // Assert
            Assert.IsNotNull(result);
        }

        private class Class : MapData {
            public Class(string name) : base(name) {}
        }

        private class Child : Class {
            public Child(string name) : base(name) {}
        }

        private class TestMapDataConverter<T> : MapDataConverter<T> where T : MapData
        {
            protected override IList<IMapFeatureLayer> Convert(T data)
            {
                return new List<IMapFeatureLayer>(); // Dummy implementation
            }
        }
    }
}