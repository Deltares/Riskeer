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
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class FeatureBasedMapDataConverterTest
    {
        [Test]
        public void CanConvertMapData_DifferentInheritingTypes_OnlySupportsExactType()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            var featureBasedMapDataResult = testConverter.CanConvertMapData(new TestFeatureBasedMapData("test data"));
            var classResult = testConverter.CanConvertMapData(new Class("test data"));
            var childResult = testConverter.CanConvertMapData(new Child("test data"));

            // Assert
            Assert.IsFalse(featureBasedMapDataResult);
            Assert.IsTrue(classResult);
            Assert.IsTrue(childResult);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            const string expectedMessage = "Null data cannot be converted into a feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testFeatureBasedMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testFeatureBasedMapData);

            // Assert
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testFeatureBasedMapData.GetType());
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Convert_DataCanBeConverted_ReturnsResult()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<TestFeatureBasedMapData>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Precondition
            Assert.IsTrue(testConverter.CanConvertMapData(testFeatureBasedMapData));

            // Call
            var result = testConverter.Convert(testFeatureBasedMapData);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ConvertLayerFeatures_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(null, new MapPointLayer());

            // Assert
            const string expectedMessage = "Null data cannot be converted into a feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerFeatures_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testFeatureBasedMapData));

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(testFeatureBasedMapData, new MapPointLayer());

            // Assert
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testFeatureBasedMapData.GetType());
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerFeatures_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<TestFeatureBasedMapData>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(testFeatureBasedMapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(null, new MapPointLayer());

            // Assert
            const string expectedMessage = "Null data cannot be converted into a feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testFeatureBasedMapData));

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(testFeatureBasedMapData, new MapPointLayer());

            // Assert
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testFeatureBasedMapData.GetType());
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<TestFeatureBasedMapData>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(testFeatureBasedMapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        private class Class : FeatureBasedMapData
        {
            public Class(string name) : base(name) {}
        }

        private class Child : Class
        {
            public Child(string name) : base(name) {}
        }

        private class TestFeatureBasedMapDataConverter<TFeatureBasedMapData> : FeatureBasedMapDataConverter<TFeatureBasedMapData, MapPointLayer>
            where TFeatureBasedMapData : FeatureBasedMapData
        {
            protected override IFeatureSymbolizer CreateSymbolizer(TFeatureBasedMapData mapData)
            {
                return new PointSymbolizer();
            }

            protected override IMapFeatureLayer CreateLayer()
            {
                return new MapPointLayer();
            }

            protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
            {
                throw new NotImplementedException();
            }
        }
    }
}