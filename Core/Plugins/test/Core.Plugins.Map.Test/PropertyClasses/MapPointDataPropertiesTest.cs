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

using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Util;
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
    public class MapPointDataPropertiesTest
    {
        private const int colorPropertyIndex = 6;
        private const int strokeColorPropertyIndex = 7;
        private const int strokeThicknessPropertyIndex = 8;
        private const int sizePropertyIndex = 9;
        private const int symbolPropertyIndex = 10;

        private const int strokeColorWithMapThemePropertyIndex = 8;
        private const int strokeThicknessWithMapThemePropertyIndex = 9;
        private const int sizeWithMapThemePropertyIndex = 10;
        private const int symbolWithMapThemePropertyIndex = 11;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new MapPointDataProperties();

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapPointData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Punten", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            // Call
            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(strokeColorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor sizeProperty = dynamicProperties[sizePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            styleCategory,
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor symbolProperty = dynamicProperties[symbolPropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(symbolProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            styleCategory,
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Constructor_MapLineDataWithMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
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

            // Call
            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(12, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorWithMapThemePropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(strokeColorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessWithMapThemePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor sizeProperty = dynamicProperties[sizeWithMapThemePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            styleCategory,
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor symbolProperty = dynamicProperties[symbolWithMapThemePropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(symbolProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            styleCategory,
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);
        }

        [Test]
        public void Data_SetNewMapPointDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int size = 4;
            const PointSymbol symbol = PointSymbol.Circle;

            var mapPointData = new MapPointData("Test", new PointStyle
            {
                Color = color,
                Size = size,
                Symbol = symbol,
                StrokeColor = color,
                StrokeThickness = 1
            });
            var properties = new MapPointDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(color, properties.StrokeColor);
            Assert.AreEqual(1, properties.StrokeThickness);
            Assert.AreEqual(size, properties.Size);
            Assert.AreEqual(symbol, properties.Symbol);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 5;
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            Color color = Color.AliceBlue;
            var mapPointData = new MapPointData("Test", new PointStyle
            {
                Color = color,
                Size = 3,
                Symbol = PointSymbol.Circle,
                StrokeColor = color,
                StrokeThickness = 1
            });

            mapPointData.Attach(observer);

            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            Color newColor = Color.Blue;
            Color newStrokeColor = Color.Aquamarine;
            const int newSize = 6;
            const PointSymbol newSymbol = PointSymbol.Diamond;
            const int newStrokeThickness = 4;

            // Call
            properties.Color = newColor;
            properties.Size = newSize;
            properties.Symbol = newSymbol;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newColor, mapPointData.Style.Color);
            Assert.AreEqual(newSize, mapPointData.Style.Size);
            Assert.AreEqual(newSymbol, mapPointData.Style.Symbol);
            Assert.AreEqual(newStrokeColor, mapPointData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, mapPointData.Style.StrokeThickness);
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

            var mapData = new MapLineData("Test")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new MapLineDataProperties
            {
                Data = mapData
            };

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
        public void DynamicReadOnlyValidator_MapLineDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapData = new MapPointData("Test")
            {
                MapTheme = hasMapTheme
                               ? new MapTheme("Attribute", new[]
                               {
                                   CategoryThemeTestFactory.CreateCategoryTheme()
                               })
                               : null
            };

            var properties = new MapPointDataProperties
            {
                Data = mapData
            };

            // Call
            bool isWidthReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPointDataProperties.Size));
            bool isStrokeColorReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPointDataProperties.StrokeColor));
            bool isStrokeThicknessReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPointDataProperties.StrokeThickness));
            bool isDashStyleReadOnly = properties.DynamicReadonlyValidator(
                nameof(MapPointDataProperties.Symbol));

            // Assert
            Assert.AreEqual(hasMapTheme, isWidthReadOnly);
            Assert.AreEqual(hasMapTheme, isStrokeColorReadOnly);
            Assert.AreEqual(hasMapTheme, isStrokeThicknessReadOnly);
            Assert.AreEqual(hasMapTheme, isDashStyleReadOnly);
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
                },
                MapTheme = new MapTheme("Attribute", new[]
                {
                    CategoryThemeTestFactory.CreateCategoryTheme()
                })
            };

            var properties = new MapPointDataProperties
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
            var mapLineData = new MapLineData("Test")
            {
                ShowLabels = showLabels
            };

            var properties = new MapLineDataProperties
            {
                Data = mapLineData
            };

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.SelectedMetaDataAttribute));

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

            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            // Call
            bool isMapThemeAttributeNameVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.MapThemeAttributeName));
            bool isMapThemeCategoriesVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.Categories));
            bool isLineColorStyleVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.Color));

            // Assert
            Assert.AreEqual(hasMapTheme, isMapThemeAttributeNameVisible);
            Assert.AreEqual(hasMapTheme, isMapThemeCategoriesVisible);
            Assert.AreNotEqual(hasMapTheme, isLineColorStyleVisible);
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

            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }
    }
}