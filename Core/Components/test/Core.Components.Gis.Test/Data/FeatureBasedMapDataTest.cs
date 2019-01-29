// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class FeatureBasedMapDataTest
    {
        [Test]
        public void Constructor_ValidName_ExpectedValues()
        {
            // Setup
            const string name = "test data";

            // Call
            var data = new TestFeatureBasedMapData(name);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.IsFalse(data.ShowLabels);
            Assert.IsNull(data.SelectedMetaDataAttribute);
            CollectionAssert.IsEmpty(data.MetaData);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new TestFeatureBasedMapData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Features_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");
            var features = new[]
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            // Call
            data.Features = features;

            // Assert
            Assert.AreSame(features, data.Features);
        }

        [Test]
        public void Features_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => data.Features = null;

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Features_SetCollectionWithNullElement_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => data.Features = new MapFeature[]
            {
                null
            };

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void MetaData_Always_ReturnAllUniqueAvailableMetaDataAttributesFromFeatures()
        {
            // Setup
            var feature1 = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature1.MetaData["Attribute1"] = new object();
            var feature2 = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature2.MetaData["Attribute1"] = new object();
            feature2.MetaData["Attribute2"] = new object();
            var feature3 = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature3.MetaData["Attribute3"] = new object();

            var data = new TestFeatureBasedMapData("test data")
            {
                Features = new[]
                {
                    feature1,
                    feature2,
                    feature3
                }
            };

            // Call
            IEnumerable<string> metaData = data.MetaData;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Attribute1",
                "Attribute2",
                "Attribute3"
            }, metaData);
        }

        [Test]
        public void TypedConstructor_ThemeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TypedTestFeatureBasedMapData("name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("theme", exception.ParamName);
        }

        [Test]
        public void TypedConstructor_WitName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var data = new TypedTestFeatureBasedMapData(name);

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapData>(data);
            Assert.AreEqual(name, data.Name);
            Assert.IsNull(data.Theme);
        }

        [Test]
        public void TypedConstructor_WithNameAndCategoryThemes_ExpectedValues()
        {
            // Setup
            const string name = "name";
            var mapTheme = new MapTheme<TestCategoryTheme>("test", new[]
            {
                new TestCategoryTheme()
            });

            // Call
            var data = new TypedTestFeatureBasedMapData(name, mapTheme);

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapData>(data);
            Assert.AreEqual(name, data.Name);
            Assert.AreSame(mapTheme, data.Theme);
        }

        private class TestFeatureBasedMapData : FeatureBasedMapData
        {
            public TestFeatureBasedMapData(string name) : base(name) {}
        }

        private class TypedTestFeatureBasedMapData : FeatureBasedMapData<TestCategoryTheme>
        {
            public TypedTestFeatureBasedMapData(string name, MapTheme<TestCategoryTheme> theme)
                : base(name, theme) {}

            public TypedTestFeatureBasedMapData(string name) : base(name) {}
        }

        private class TestCategoryTheme : CategoryTheme
        {
            public TestCategoryTheme() : base(ValueCriterionTestFactory.CreateValueCriterion()) {}
        }
    }
}