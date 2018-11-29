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
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
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
    public class PolygonCategoryThemePropertiesTest
    {
        private const int fillColorPropertyIndex = 1;
        private const int strokeColorPropertyIndex = 2;
        private const int strokeThicknessPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var categoryTheme = new PolygonCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                         new PolygonStyle());

            // Call
            var properties = new PolygonCategoryThemeProperties(categoryTheme, string.Empty, new MapPolygonData("Name"));

            // Assert
            Assert.IsInstanceOf<CategoryThemeProperties<PolygonCategoryTheme>>(properties);

            TestHelper.AssertTypeConverter<PolygonCategoryThemeProperties, ColorTypeConverter>(
                nameof(PolygonCategoryThemeProperties.FillColor));
            TestHelper.AssertTypeConverter<PolygonCategoryThemeProperties, ColorTypeConverter>(
                nameof(PolygonCategoryThemeProperties.StrokeColor));
        }

        [Test]
        public void Constructor_Always_ReturnExpectedPropertyValues()
        {
            // Setup
            const string attributeName = "AttributeName";
            ValueCriterion valueCriterion = ValueCriterionTestFactory.CreateValueCriterion();
            var categoryTheme = new PolygonCategoryTheme(valueCriterion, new PolygonStyle());

            var properties = new PolygonCategoryThemeProperties(categoryTheme, attributeName, new MapPolygonData("Name"));

            // Assert
            Assert.AreSame(categoryTheme, properties.Data);

            Assert.AreEqual(categoryTheme.Style.FillColor, properties.FillColor);
            Assert.AreEqual(categoryTheme.Style.StrokeColor, properties.StrokeColor);
            Assert.AreEqual(categoryTheme.Style.StrokeThickness, properties.StrokeThickness);

            ValueCriterionTestHelper.AssertValueCriterionFormatExpression(attributeName,
                                                                          valueCriterion,
                                                                          properties.Criterion);
        }

        [Test]
        public void Constructor_WithValidArguments_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var categoryTheme = new PolygonCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                         new PolygonStyle());

            // Call
            var properties = new PolygonCategoryThemeProperties(categoryTheme, string.Empty, new MapPolygonData("Name"));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor fillColorProperty = dynamicProperties[fillColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fillColorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de vlakken waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de vlakken waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de vlakken waarmee deze categorie wordt weergegeven.");
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(3);
            mocks.ReplayAll();

            var mapData = new MapPolygonData("Name");
            mapData.Attach(observer);

            var categoryTheme = new PolygonCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                         new PolygonStyle());

            var properties = new PolygonCategoryThemeProperties(categoryTheme, string.Empty, mapData);

            Color fillColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            Color strokeColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            int strokeThickness = random.Next(1, 48);

            // Call
            properties.FillColor = fillColor;
            properties.StrokeColor = strokeColor;
            properties.StrokeThickness = strokeThickness;

            // Assert
            PolygonStyle actualStyle = categoryTheme.Style;
            Assert.AreEqual(fillColor, actualStyle.FillColor);
            Assert.AreEqual(strokeColor, actualStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, actualStyle.StrokeThickness);

            mocks.VerifyAll();
        }
    }
}