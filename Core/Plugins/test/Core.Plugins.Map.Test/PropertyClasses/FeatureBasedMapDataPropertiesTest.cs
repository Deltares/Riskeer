// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Plugins.Map.PropertyClasses;
using Core.Plugins.Map.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class FeatureBasedMapDataPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int isVisiblePropertyIndex = 2;
        private const int showLabelsPropertyIndex = 3;
        private const int selectedMetaDataAttributePropertyIndex = 4;
        private const int stylePropertyIndex = 5;

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFeatureBasedMapDataProperties(null, Enumerable.Empty<MapDataCollection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ParentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFeatureBasedMapDataProperties(new TestFeatureBasedMapData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parents", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var data = new TestFeatureBasedMapData();

            // Call
            var properties = new TestFeatureBasedMapDataProperties(data, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<TestFeatureBasedMapData>>(properties);
            Assert.IsInstanceOf<IHasMetaData>(properties);
            Assert.AreSame(data, properties.Data);
        }

        [Test]
        public void Constructor_ReturnCorrectPropertyValues()
        {
            // Setup
            var data = new TestFeatureBasedMapData();

            // Call
            var properties = new TestFeatureBasedMapDataProperties(data, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual(data.Name, properties.Name);
            Assert.AreEqual("Test feature based map data", properties.Type);
            Assert.AreEqual(data.IsVisible, properties.IsVisible);
            Assert.AreEqual(data.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(data.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual("Enkel symbool", properties.StyleType);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["key"] = "value";

            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = true
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string layerCategory = "Kaartlaag";
            const string labelCategory = "Labels";
            const string styleCategory = "Stijl";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            layerCategory,
                                                                            "Naam",
                                                                            "De naam van deze kaartlaag.",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[typePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            layerCategory,
                                                                            "Type",
                                                                            "Het type van de data die wordt weergegeven op deze kaartlaag.",
                                                                            true);

            PropertyDescriptor isVisibleProperty = dynamicProperties[isVisiblePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isVisibleProperty,
                                                                            layerCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of deze kaartlaag wordt weergegeven.");

            PropertyDescriptor showlabelsProperty = dynamicProperties[showLabelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(showlabelsProperty,
                                                                            labelCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of labels worden weergegeven op deze kaartlaag.");

            PropertyDescriptor selectedMetaDataAttributeProperty = dynamicProperties[selectedMetaDataAttributePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(selectedMetaDataAttributeProperty,
                                                                            labelCategory,
                                                                            "Op basis van",
                                                                            "Toont de eigenschap op basis waarvan labels worden weergegeven op deze kaartlaag.");

            PropertyDescriptor styleTypeProperty = dynamicProperties[stylePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleTypeProperty,
                                                                            styleCategory,
                                                                            "Type",
                                                                            "Het type van de stijl die wordt toegepast voor het weergeven van deze kaartlaag.",
                                                                            true);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 2;
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapData = new TestFeatureBasedMapData
            {
                ShowLabels = true
            };

            mapData.Attach(observer);

            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            properties.ShowLabels = false;
            properties.SelectedMetaDataAttribute = new SelectableMetaDataAttribute("ID");

            // Assert
            Assert.IsFalse(mapData.ShowLabels);
            Assert.AreEqual("ID", mapData.SelectedMetaDataAttribute);
            mocks.VerifyAll();
        }

        [Test]
        public void IsVisible_SetNewValue_UpdateDataAndNotifyObserversOfDataAndParents()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(4);
            mocks.ReplayAll();

            var mapData = new TestFeatureBasedMapData();
            var parents = new[]
            {
                new MapDataCollection("test 1"),
                new MapDataCollection("test 2"),
                new MapDataCollection("test 3")
            };

            mapData.Attach(observer);
            parents.ForEachElementDo(parent => parent.Attach(observer));

            var properties = new TestFeatureBasedMapDataProperties(mapData, parents);

            // Call
            properties.IsVisible = false;

            // Assert
            Assert.IsFalse(properties.IsVisible);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShowLabels_MapDataHasMetaData_ShowLabelsAndSelectedMetaDataAttributeShouldNotBeReadOnly(bool hasMetaData)
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            if (hasMetaData)
            {
                feature.MetaData["key"] = "value";
            }

            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = true
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor showlabelsProperty = dynamicProperties[showLabelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(showlabelsProperty,
                                                                            "Labels",
                                                                            "Weergeven",
                                                                            "Geeft aan of labels worden weergegeven op deze kaartlaag.",
                                                                            !hasMetaData);

            PropertyDescriptor selectedMetaDataAttributeProperty = dynamicProperties[selectedMetaDataAttributePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(selectedMetaDataAttributeProperty,
                                                                            "Labels",
                                                                            "Op basis van",
                                                                            "Toont de eigenschap op basis waarvan labels worden weergegeven op deze kaartlaag.",
                                                                            !hasMetaData);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SelectedMetaDataAttribute_ShowLabelsTrue_SelectedMetaDataAttributeShouldBeVisible(bool showLabels)
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["key"] = "value";

            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = showLabels
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(showLabels ? 6 : 5, dynamicProperties.Count);

            if (showLabels)
            {
                PropertyDescriptor selectedMetaDataAttributeProperty = dynamicProperties[selectedMetaDataAttributePropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(selectedMetaDataAttributeProperty,
                                                                                "Labels",
                                                                                "Op basis van",
                                                                                "Toont de eigenschap op basis waarvan labels worden weergegeven op deze kaartlaag.");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnlyValidator_MapHasMetaData_ReturnsExpectedValuesForRelevantProperties(bool hasMetaData)
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            if (hasMetaData)
            {
                feature.MetaData["key"] = "value";
            }

            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isShowLabelReadOnly = properties.DynamicReadonlyValidator(
                nameof(TestFeatureBasedMapDataProperties.ShowLabels));
            bool isSelectedMetaDataReadOnly = properties.DynamicReadonlyValidator(
                nameof(TestFeatureBasedMapDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreNotEqual(hasMetaData, isShowLabelReadOnly);
            Assert.AreNotEqual(hasMetaData, isSelectedMetaDataReadOnly);
        }

        [Test]
        public void DynamicReadOnlyValidator_AnyOtherProperty_ReturnsFalse()
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["Key"] = "value";

            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isOtherPropertyReadOnly = properties.DynamicReadonlyValidator(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_ShowLabels_ReturnsExpectedValuesForRelevantProperties(bool showLabels)
        {
            // Setup
            var mapData = new TestFeatureBasedMapData
            {
                ShowLabels = showLabels
            };

            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(TestFeatureBasedMapDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreEqual(showLabels, isSelectedMetaDataAttributeVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            var mapData = new TestFeatureBasedMapData
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            var properties = new TestFeatureBasedMapDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }

        private class TestFeatureBasedMapDataProperties : FeatureBasedMapDataProperties<TestFeatureBasedMapData>
        {
            public TestFeatureBasedMapDataProperties(TestFeatureBasedMapData data, IEnumerable<MapDataCollection> parents)
                : base(data, parents) {}

            public override string Type
            {
                get
                {
                    return "Test feature based map data";
                }
            }
        }
    }
}