﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
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

        private const int mapThemeAttributeNamePropertyIndex = 5;
        private const int mapThemeCategoriesPropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new TestFeatureBasedMapDataProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapPointData>>(properties);
            Assert.IsInstanceOf<IHasMetaData>(properties);
            Assert.IsNull(properties.Data);

            TestHelper.AssertTypeConverter<TestFeatureBasedMapDataProperties, ExpandableArrayConverter>(
                nameof(properties.Categories));
        }

        [Test]
        public void Data_SetNewMapPointDataInstanceWithoutMapTheme_ReturnCorrectPropertyValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test");

            var properties = new TestFeatureBasedMapDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.Name, properties.Name);
            Assert.AreEqual("Test feature based map data", properties.Type);
            Assert.AreEqual(mapPointData.IsVisible, properties.IsVisible);
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual("Enkel symbool", properties.StyleType);
            Assert.IsEmpty(properties.MapThemeAttributeName);
            CollectionAssert.IsEmpty(properties.Categories);
        }

        [Test]
        public void Data_SetNewMapPointDataInstanceWithMapTheme_ReturnCorrectPropertyValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                MapTheme = new MapTheme("Attribute", new[]
                {
                    CategoryThemeTestFactory.CreateCategoryTheme()
                })
            };

            var properties = new TestFeatureBasedMapDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.Name, properties.Name);
            Assert.AreEqual("Test feature based map data", properties.Type);
            Assert.AreEqual(mapPointData.IsVisible, properties.IsVisible);
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual("Categorie", properties.StyleType);
            Assert.AreEqual(mapPointData.MapTheme.AttributeName, properties.MapThemeAttributeName);

            IEnumerable<CategoryTheme> categoryThemes = mapPointData.MapTheme.CategoryThemes;
            Assert.AreEqual(categoryThemes.Count(), properties.Categories.Length);

            CategoryThemeProperties categoryThemeProperties = properties.Categories.First();
            CategoryTheme categoryTheme = categoryThemes.First();
            Assert.AreSame(categoryTheme, categoryThemeProperties.Data);
            StringAssert.StartsWith(mapPointData.MapTheme.AttributeName, categoryThemeProperties.Criterion);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["key"] = "value";

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = true
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

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
            const int numberOfChangedProperties = 3;
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapPointData = new MapPointData("Test")
            {
                ShowLabels = true
            };

            mapPointData.Attach(observer);

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Call
            properties.IsVisible = false;
            properties.ShowLabels = false;
            properties.SelectedMetaDataAttribute = new SelectableMetaDataAttribute("ID");

            // Assert
            Assert.IsFalse(mapPointData.IsVisible);
            Assert.IsFalse(mapPointData.ShowLabels);
            Assert.AreEqual("ID", mapPointData.SelectedMetaDataAttribute);
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

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = true
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

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

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = showLabels
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

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
        public void MapThemeRelatedProperties_MapDataWithMapThemeConfiguration_PropertiesShouldBeVisible(bool hasMapTheme)
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                MapTheme = hasMapTheme
                               ? new MapTheme("Attribute", new[]
                               {
                                   CategoryThemeTestFactory.CreateCategoryTheme()
                               })
                               : null
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Assert
            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(hasMapTheme ? 7 : 5, dynamicProperties.Count);

            if (hasMapTheme)
            {
                PropertyDescriptor mapThemeAttributeNameProperty = dynamicProperties[mapThemeAttributeNamePropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(mapThemeAttributeNameProperty,
                                                                                "Stijl",
                                                                                "Op basis van",
                                                                                "Toont de eigenschap op basis waarvan de kaartlaag is gecategoriseerd.",
                                                                                true);

                PropertyDescriptor categoriesAttribute = dynamicProperties[mapThemeCategoriesPropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoriesAttribute,
                                                                                "Stijl",
                                                                                "Categorieën",
                                                                                string.Empty,
                                                                                true);
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

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

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

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

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
            var mapPointData = new MapPointData("Test")
            {
                ShowLabels = showLabels
            };

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(TestFeatureBasedMapDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreEqual(showLabels, isSelectedMetaDataAttributeVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_MapDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                MapTheme = hasMapTheme
                               ? new MapTheme("Attribute", new[]
                               {
                                   CategoryThemeTestFactory.CreateCategoryTheme()
                               })
                               : null
            };

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Call
            bool isMapThemeAttributeNameVisible = properties.DynamicVisibleValidationMethod(
                nameof(TestFeatureBasedMapDataProperties.MapThemeAttributeName));
            bool isMapThemeCategoriesVisible = properties.DynamicVisibleValidationMethod(
                nameof(TestFeatureBasedMapDataProperties.Categories));

            // Assert
            Assert.AreEqual(hasMapTheme, isMapThemeAttributeNameVisible);
            Assert.AreEqual(hasMapTheme, isMapThemeCategoriesVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true,
                MapTheme = new MapTheme("Attribute", new[]
                {
                    CategoryThemeTestFactory.CreateCategoryTheme()
                })
            };

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }

        private class TestFeatureBasedMapDataProperties : FeatureBasedMapDataProperties<MapPointData>
        {
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