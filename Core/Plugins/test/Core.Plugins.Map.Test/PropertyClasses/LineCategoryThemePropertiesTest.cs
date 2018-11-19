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
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class LineCategoryThemePropertiesTest
    {
        private const int colorPropertyIndex = 1;
        private const int lineWidthPropertyIndex = 2;
        private const int lineStylePropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var categoryLineTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                          new LineStyle());

            // Call
            var properties = new LineCategoryThemeProperties(string.Empty, categoryLineTheme, new MapLineData("Name"));

            // Assert
            Assert.IsInstanceOf<CategoryThemeProperties<LineCategoryTheme>>(properties);

            TestHelper.AssertTypeConverter<LineCategoryThemeProperties, ColorTypeConverter>(
                nameof(LineCategoryThemeProperties.Color));
            TestHelper.AssertTypeConverter<LineCategoryThemeProperties, EnumTypeConverter>(
                nameof(LineCategoryThemeProperties.DashStyle));
        }

        [Test]
        public void Constructor_Always_ReturnExpectedPropertyValues()
        {
            // Setup
            const string attributeName = "AttributeName";
            var mapLineData = new MapLineData("Name");
            ValueCriterion valueCriterion = ValueCriterionTestFactory.CreateValueCriterion();
            var categoryLineTheme = new LineCategoryTheme(valueCriterion, new LineStyle());

            var properties = new LineCategoryThemeProperties(attributeName, categoryLineTheme, mapLineData);

            // Assert
            Assert.AreSame(categoryLineTheme, properties.Data);

            Assert.AreEqual(categoryLineTheme.Style.Color, properties.Color);
            Assert.AreEqual(categoryLineTheme.Style.Width, properties.Width);
            Assert.AreEqual(categoryLineTheme.Style.DashStyle, properties.DashStyle);

            string expectedValue = GetExpectedFormatExpression(valueCriterion, attributeName);
            Assert.AreEqual(expectedValue, properties.Criterion);
        }

        [Test]
        public void Constructor_WithValidArguments_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var categoryLineTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                          new LineStyle());

            // Call
            var properties = new LineCategoryThemeProperties(string.Empty, categoryLineTheme, new MapLineData("Name"));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de lijnen waarmee deze categorie wordt weergegeven.",
                                                                            true);

            PropertyDescriptor lineWidthProperty = dynamicProperties[lineWidthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lineWidthProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijnen waarmee deze categorie wordt weergegeven.",
                                                                            true);

            PropertyDescriptor lineStyleProperty = dynamicProperties[lineStylePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lineStyleProperty,
                                                                            styleCategory,
                                                                            "Lijnstijl",
                                                                            "De stijl van de lijnen waarmee deze categorie wordt weergegeven.",
                                                                            true);
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

            var mapData = new MapLineData("Name");
            mapData.Attach(observer);

            var categoryLineTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                          new LineStyle());

            var properties = new LineCategoryThemeProperties(string.Empty, categoryLineTheme, mapData);

            int width = random.Next(1, 48);
            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            var dashStyle = random.NextEnumValue<LineDashStyle>();

            // Call
            properties.Width = width;
            properties.Color = color;
            properties.DashStyle = dashStyle;

            // Assert
            LineStyle actualLineStyle = categoryLineTheme.Style;
            Assert.AreEqual(width, actualLineStyle.Width);
            Assert.AreEqual(color, actualLineStyle.Color);
            Assert.AreEqual(dashStyle, actualLineStyle.DashStyle);

            mocks.VerifyAll();
        }

        private static string GetExpectedFormatExpression(ValueCriterion valueCriterion, string attributeName)
        {
            string valueCriterionValue = valueCriterion.Value;
            switch (valueCriterion.ValueOperator)
            {
                case ValueCriterionOperator.EqualValue:
                    return $"{attributeName} = {valueCriterionValue}";
                case ValueCriterionOperator.UnequalValue:
                    return $"{attributeName} ≠ {valueCriterionValue}";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}