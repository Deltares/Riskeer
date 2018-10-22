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

using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapPolygonDataPropertiesTest
    {
        private const int fillColorPropertyIndex = 6;
        private const int strokeColorPropertyIndex = 7;
        private const int strokeThicknessPropertyIndex = 8;

        private const int strokeColorWithMapThemePropertyIndex = 8;
        private const int strokeThicknessWithMapThemePropertyIndex = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var data = new MapPolygonData("test");

            // Call
            var properties = new MapPolygonDataProperties(data, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapPolygonData>>(properties);
            Assert.AreSame(data, properties.Data);
            Assert.AreEqual("Polygonen", properties.Type);

            TestHelper.AssertTypeConverter<MapPolygonDataProperties, ColorTypeConverter>(
                nameof(MapPolygonDataProperties.FillColor));

            TestHelper.AssertTypeConverter<MapPolygonDataProperties, ColorTypeConverter>(
                nameof(MapPolygonDataProperties.StrokeColor));
        }

        [Test]
        public void Constructor_MapPolygonDataWithoutMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            // Call
            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[fillColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de vlakken waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor styleProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Constructor_MapPolygonDataWithMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test")
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

            // Call
            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorWithMapThemePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor styleProperty = dynamicProperties[strokeThicknessWithMapThemePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);
        }

        [Test]
        public void Constructor_Always_ReturnCorrectPropertyValues()
        {
            // Setup
            Color fillColor = Color.Aqua;
            Color strokeColor = Color.Bisque;
            const int strokeThickness = 4;

            var mapPolygonData = new MapPolygonData("Test", new PolygonStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            });

            // Call
            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual(mapPolygonData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPolygonData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(fillColor, properties.FillColor);
            Assert.AreEqual(strokeColor, properties.StrokeColor);
            Assert.AreEqual(strokeThickness, properties.StrokeThickness);
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

            var mapPolygonData = new MapPolygonData("Test", new PolygonStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.Blue,
                StrokeThickness = 3
            });

            mapPolygonData.Attach(observer);

            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            Color newFillColor = Color.Blue;
            Color newStrokeColor = Color.Red;
            const int newStrokeThickness = 6;

            // Call
            properties.FillColor = newFillColor;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newFillColor, mapPolygonData.Style.FillColor);
            Assert.AreEqual(newStrokeColor, mapPolygonData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, mapPolygonData.Style.StrokeThickness);
            mocks.VerifyAll();
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

            var mapData = new MapPolygonData("Test")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new MapPolygonDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isShowLabelReadOnly = properties.DynamicReadonlyValidator(
                nameof(properties.ShowLabels));
            bool isSelectedMetaDataReadOnly = properties.DynamicReadonlyValidator(
                nameof(properties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreNotEqual(hasMetaData, isShowLabelReadOnly);
            Assert.AreNotEqual(hasMetaData, isSelectedMetaDataReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnlyValidator_MapPolygonDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapData = new MapPolygonData("Test")
            {
                MapTheme = hasMapTheme
                               ? new MapTheme("Attribute", new[]
                               {
                                   CategoryThemeTestFactory.CreateCategoryTheme()
                               })
                               : null
            };

            var properties = new MapPolygonDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isStrokeColorReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPolygonDataProperties.StrokeColor));
            bool isStrokeThicknessReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPolygonDataProperties.StrokeThickness));

            // Assert
            Assert.AreEqual(hasMapTheme, isStrokeColorReadOnly);
            Assert.AreEqual(hasMapTheme, isStrokeThicknessReadOnly);
        }

        [Test]
        public void DynamicReadOnlyValidator_AnyOtherProperty_ReturnsFalse()
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["Key"] = "value";

            var mapPolygonData = new MapPolygonData("Test")
            {
                Features = new[]
                {
                    feature
                },
                MapTheme = new MapTheme("Attribute", new[]
                {
                    CategoryThemeTestFactory.CreateCategoryTheme()
                })
            };

            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

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
            var mapPolygonData = new MapPolygonData("Test")
            {
                ShowLabels = showLabels
            };

            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPolygonDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreEqual(showLabels, isSelectedMetaDataAttributeVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_MapPolygonDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test")
            {
                MapTheme = hasMapTheme
                               ? new MapTheme("Attribute", new[]
                               {
                                   CategoryThemeTestFactory.CreateCategoryTheme()
                               })
                               : null
            };

            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isFillColorVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPolygonDataProperties.FillColor));
            bool isMapThemeAttributeNameVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPolygonDataProperties.MapThemeAttributeName));
            bool isCategoriesVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPolygonDataProperties.Categories));

            // Assert
            Assert.AreNotEqual(hasMapTheme, isFillColorVisible);
            Assert.AreEqual(hasMapTheme, isMapThemeAttributeNameVisible);
            Assert.AreEqual(hasMapTheme, isCategoriesVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            var mapPolygonData = new MapPolygonData("Test")
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

            var properties = new MapPolygonDataProperties(mapPolygonData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }
    }
}