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
using Core.Common.Util;
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
    public class PointCategoryThemePropertiesTest
    {
        private const int colorPropertyIndex = 1;
        private const int strokeColorPropertyIndex = 2;
        private const int strokeThicknessPropertyIndex = 3;
        private const int sizePropertyIndex = 4;
        private const int symbolPropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var categoryTheme = new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                       new PointStyle());

            // Call
            var properties = new PointCategoryThemeProperties(string.Empty, categoryTheme, new MapPointData("Name"));

            // Assert
            Assert.IsInstanceOf<CategoryThemeProperties<PointCategoryTheme>>(properties);

            TestHelper.AssertTypeConverter<PointCategoryThemeProperties, ColorTypeConverter>(
                nameof(PointCategoryThemeProperties.Color));

            TestHelper.AssertTypeConverter<PointCategoryThemeProperties, ColorTypeConverter>(
                nameof(PointCategoryThemeProperties.StrokeColor));

            TestHelper.AssertTypeConverter<PointCategoryThemeProperties, EnumTypeConverter>(
                nameof(PointCategoryThemeProperties.Symbol));
        }

        [Test]
        public void Constructor_WithValidArguments_ReturnExpectedPropertyValues()
        {
            // Setup
            const string attributeName = "AttributeName";
            ValueCriterion valueCriterion = ValueCriterionTestFactory.CreateValueCriterion();
            var categoryTheme = new PointCategoryTheme(valueCriterion, new PointStyle());

            var properties = new PointCategoryThemeProperties(attributeName, categoryTheme, new MapPointData("Name"));

            // Assert
            Assert.AreSame(categoryTheme, properties.Data);

            Assert.AreEqual(categoryTheme.Style.Color, properties.Color);
            Assert.AreEqual(categoryTheme.Style.StrokeColor, properties.StrokeColor);
            Assert.AreEqual(categoryTheme.Style.StrokeThickness, properties.StrokeThickness);
            Assert.AreEqual(categoryTheme.Style.Symbol, properties.Symbol);
            Assert.AreEqual(categoryTheme.Style.Size, properties.Size);

            ValueCriterionTestHelper.AssertValueCriterionFormatExpression(attributeName,
                                                                          valueCriterion,
                                                                          properties.Criterion);
        }

        [Test]
        public void Constructor_WithValidArguments_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var categoryTheme = new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                       new PointStyle());

            // Call
            var properties = new PointCategoryThemeProperties(string.Empty, categoryTheme, new MapPointData("Name"));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de symbolen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor sizeProperty = dynamicProperties[sizePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            styleCategory,
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor symbolProperty = dynamicProperties[symbolPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            styleCategory,
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze categorie wordt weergegeven.");
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(5);
            mocks.ReplayAll();

            var mapData = new MapPointData("Name");
            mapData.Attach(observer);

            var categoryTheme = new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                       new PointStyle());

            var properties = new PointCategoryThemeProperties(string.Empty, categoryTheme, mapData);

            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            Color strokeColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            int strokeThickness = random.Next(1, 48);
            int size = random.Next(1, 48);
            var symbol = random.NextEnumValue<PointSymbol>();

            // Call
            properties.Color = color;
            properties.StrokeColor = strokeColor;
            properties.StrokeThickness = strokeThickness;
            properties.Size = size;
            properties.Symbol = symbol;

            // Assert
            PointStyle actualStyle = categoryTheme.Style;
            Assert.AreEqual(color, actualStyle.Color);
            Assert.AreEqual(strokeColor, actualStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, actualStyle.StrokeThickness);
            Assert.AreEqual(size, actualStyle.Size);
            Assert.AreEqual(symbol, actualStyle.Symbol);

            mocks.VerifyAll();
        }
    }
}