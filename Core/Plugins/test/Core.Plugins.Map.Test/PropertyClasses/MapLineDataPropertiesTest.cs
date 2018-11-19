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
    public class MapLineDataPropertiesTest
    {
        private const int colorPropertyIndex = 6;
        private const int widthPropertyIndex = 7;
        private const int stylePropertyIndex = 8;

        private const int mapThemeCategoriesPropertyIndex = 8;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var data = new MapLineData("test");

            // Call
            var properties = new MapLineDataProperties(data, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapLineData>>(properties);
            Assert.AreSame(data, properties.Data);
            Assert.AreEqual("Lijnen", properties.Type);

            TestHelper.AssertTypeConverter<MapLineDataProperties, ColorTypeConverter>(
                nameof(MapLineDataProperties.Color));
            TestHelper.AssertTypeConverter<MapLineDataProperties, EnumTypeConverter>(
                nameof(MapLineDataProperties.DashStyle));
        }

        [Test]
        public void Constructor_MapLineDataWithoutMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapLineData = new MapLineData("Test")
            {
                ShowLabels = true
            };

            // Call
            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de lijnen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor widthProperty = dynamicProperties[widthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijnen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor styleProperty = dynamicProperties[stylePropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(styleProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            styleCategory,
                                                                            "Lijnstijl",
                                                                            "De stijl van de lijnen waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Constructor_MapLineDataWithMapTheme_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapLineData = new MapLineData("Test")
            {
                ShowLabels = true,
                Theme = CreateMapTheme()
            };

            // Call
            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor widthProperty = dynamicProperties[mapThemeCategoriesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthProperty,
                                                                            styleCategory,
                                                                            "Categorieën",
                                                                            "De dikte van de lijnen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);
        }

        [Test]
        public void Constructor_WithoutMapTheme_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int width = 4;
            const LineDashStyle dashStyle = LineDashStyle.DashDot;

            var mapLineData = new MapLineData("Test", new LineStyle
            {
                Color = color,
                Width = width,
                DashStyle = dashStyle
            });

            // Call
            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual(mapLineData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapLineData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(width, properties.Width);
            Assert.AreEqual(dashStyle, properties.DashStyle);

            CollectionAssert.IsEmpty(properties.LineCategoryThemes);
        }

        [Test]
        public void Constructor_WithMapTheme_ReturnCorrectPropertyValues()
        {
            // Setup
            var lineCategoryTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(), new LineStyle());
            var mapLineData = new MapLineData("Test", new LineStyle())
            {
                Theme = new MapTheme<LineCategoryTheme>("Attribute", new[]
                {
                    lineCategoryTheme
                })
            };

            // Call
            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.AreEqual(mapLineData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapLineData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(1, properties.LineCategoryThemes);
            Assert.AreEqual(lineCategoryTheme, properties.LineCategoryThemes.First().Data);
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

            var mapLineData = new MapLineData("Test", new LineStyle
            {
                Color = Color.AliceBlue,
                Width = 3,
                DashStyle = LineDashStyle.Solid
            });

            mapLineData.Attach(observer);

            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            Color newColor = Color.Blue;
            const int newWidth = 6;
            const LineDashStyle newDashStyle = LineDashStyle.DashDot;

            // Call
            properties.Color = newColor;
            properties.Width = newWidth;
            properties.DashStyle = newDashStyle;

            // Assert
            Assert.AreEqual(newColor, mapLineData.Style.Color);
            Assert.AreEqual(newWidth, mapLineData.Style.Width);
            Assert.AreEqual(newDashStyle, mapLineData.Style.DashStyle);
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

            var properties = new MapLineDataProperties(mapData, Enumerable.Empty<MapDataCollection>());

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
            var mapLineData = new MapLineData("Test")
            {
                ShowLabels = showLabels
            };

            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isSelectedMetaDataAttributeVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.SelectedMetaDataAttribute));

            // Assert
            Assert.AreEqual(showLabels, isSelectedMetaDataAttributeVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_MapLineDataWithMapTheme_ReturnsExpectedValuesForRelevantProperties(bool hasMapTheme)
        {
            // Setup
            var mapLineData = new MapLineData("Test")
            {
                Theme = hasMapTheme
                            ? CreateMapTheme()
                            : null
            };

            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isMapThemeCategoriesVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.LineCategoryThemes));
            bool isLineColorStyleVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.Color));
            bool isLineWidthVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.Color));
            bool isLineDashStyleVisible = properties.DynamicVisibleValidationMethod(
                nameof(MapLineDataProperties.Color));

            // Assert
            Assert.AreEqual(hasMapTheme, isMapThemeCategoriesVisible);
            Assert.AreNotEqual(hasMapTheme, isLineColorStyleVisible);
            Assert.AreNotEqual(hasMapTheme, isLineWidthVisible);
            Assert.AreNotEqual(hasMapTheme, isLineDashStyleVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            var mapLineData = new MapLineData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true,
                Theme = CreateMapTheme()
            };

            var properties = new MapLineDataProperties(mapLineData, Enumerable.Empty<MapDataCollection>());

            // Call
            bool isOtherPropertyVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(isOtherPropertyVisible);
        }

        private static MapTheme<LineCategoryTheme> CreateMapTheme()
        {
            return new MapTheme<LineCategoryTheme>("Attribute", new[]
            {
                new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(), new LineStyle())
            });
        }
    }
}