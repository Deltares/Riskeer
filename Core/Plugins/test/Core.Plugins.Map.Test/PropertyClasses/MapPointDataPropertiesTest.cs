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
using Core.Plugins.Map.TestUtil;
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

        private const int categoryThemesPropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var data = new MapPointData("test");

            // Call
            var properties = new MapPointDataProperties(data, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapPointData>>(properties);
            Assert.AreSame(data, properties.Data);
            Assert.AreEqual("Punten", properties.Type);

            TestHelper.AssertTypeConverter<MapPointDataProperties, ColorTypeConverter>(
                nameof(MapPointDataProperties.Color));
            TestHelper.AssertTypeConverter<MapPointDataProperties, ColorTypeConverter>(
                nameof(MapPointDataProperties.StrokeColor));
            TestHelper.AssertTypeConverter<MapPointDataProperties, EnumTypeConverter>(
                nameof(MapPointDataProperties.Symbol));

            TestHelper.AssertTypeConverter<MapPointDataProperties, ExpandableArrayConverter>(
                nameof(MapPointDataProperties.CategoryThemes));
        }

        [Test]
        public void Constructor_MapPointDataWithoutMapTheme_PropertiesHaveExpectedAttributesValues()
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
            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
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
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            styleCategory,
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Constructor_MapPointDataWithMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true,
                Theme = CreateMapTheme()
            };

            // Call
            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor categoryThemesProperty = dynamicProperties[categoryThemesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryThemesProperty,
                                                                            styleCategory,
                                                                            "Categorieën",
                                                                            string.Empty,
                                                                            true);
        }

        [Test]
        public void Constructor_WithoutMapTheme_ReturnCorrectPropertyValues()
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

            // Call
            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual("Enkel symbool", properties.StyleType);

            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(color, properties.StrokeColor);
            Assert.AreEqual(1, properties.StrokeThickness);
            Assert.AreEqual(size, properties.Size);
            Assert.AreEqual(symbol, properties.Symbol);

            CollectionAssert.IsEmpty(properties.CategoryThemes);
        }

        [Test]
        public void Constructor_WithMapTheme_ReturnCorrectPropertyValues()
        {
            // Setup
            const string attributeName = "Attribute";
            var categoryTheme = new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(), new PointStyle());
            var mapPointData = new MapPointData("Test", new PointStyle())
            {
                Theme = new MapTheme<PointCategoryTheme>(attributeName, new[]
                {
                    categoryTheme
                })
            };

            // Call
            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual("Categorie", properties.StyleType);

            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(1, properties.CategoryThemes.Length);
            PointCategoryThemeProperties pointCategoryThemeProperties = properties.CategoryThemes.First();
            Assert.AreSame(categoryTheme, pointCategoryThemeProperties.Data);
            ValueCriterionTestHelper.AssertValueCriterionFormatExpression(attributeName,
                                                                          categoryTheme.Criterion,
                                                                          pointCategoryThemeProperties.Criterion);
        }

        [Test]
        public void GivenMapPointDataPropertiesWithMapTheme_WhenCategoryThemePropertySet_ThenMapDataNotified()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var mapPointData = new MapPointData("Test", new PointStyle())
            {
                Theme = new MapTheme<PointCategoryTheme>("Attribute", new[]
                {
                    new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(), new PointStyle())
                })
            };
            mapPointData.Attach(observer);

            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // When
            PointCategoryThemeProperties categoryThemeProperties = properties.CategoryThemes.First();
            categoryThemeProperties.Size = random.Next(1, 48);

            // Then
            mocks.VerifyAll();
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

            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

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

            var mapData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new MapPointDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

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
        public void DynamicVisibleValidationMethod_ShowLabels_ReturnsExpectedValuesForRelevantProperties(bool showLabels)
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                ShowLabels = showLabels
            };

            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreEqual(showLabels, isSelectedMetaDataAttributeVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_MapPointDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                Theme = hasMapTheme
                            ? CreateMapTheme()
                            : null
            };

            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isCategoryThemesVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.CategoryThemes));
            bool isColorVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.Color));
            bool isStrokeColorVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.StrokeColor));
            bool isStrokeThicknessVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.StrokeThickness));
            bool isSizeVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.Size));
            bool isSymbolVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapPointDataProperties.Symbol));

            // Assert
            Assert.AreEqual(hasMapTheme, isCategoryThemesVisible);
            Assert.AreNotEqual(hasMapTheme, isColorVisible);
            Assert.AreNotEqual(hasMapTheme, isStrokeColorVisible);
            Assert.AreNotEqual(hasMapTheme, isStrokeThicknessVisible);
            Assert.AreNotEqual(hasMapTheme, isSizeVisible);
            Assert.AreNotEqual(hasMapTheme, isSymbolVisible);
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
                Theme = CreateMapTheme()
            };

            var properties = new MapPointDataProperties(mapPointData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }

        private static MapTheme<PointCategoryTheme> CreateMapTheme()
        {
            return new MapTheme<PointCategoryTheme>("Attribute", new[]
            {
                new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(), new PointStyle())
            });
        }
    }
}