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
    public class LineCategoryThemePropertiesTest
    {
        private const int colorPropertyIndex = 1;
        private const int lineWidthPropertyIndex = 2;
        private const int lineStylePropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var categoryTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                      new LineStyle());

            // Call
            var properties = new LineCategoryThemeProperties(categoryTheme, string.Empty, new MapLineData("Name"));

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
            ValueCriterion valueCriterion = ValueCriterionTestFactory.CreateValueCriterion();
            var categoryTheme = new LineCategoryTheme(valueCriterion, new LineStyle());

            var properties = new LineCategoryThemeProperties(categoryTheme, attributeName, new MapLineData("Name"));

            // Assert
            Assert.AreSame(categoryTheme, properties.Data);

            Assert.AreEqual(categoryTheme.Style.Color, properties.Color);
            Assert.AreEqual(categoryTheme.Style.Width, properties.Width);
            Assert.AreEqual(categoryTheme.Style.DashStyle, properties.DashStyle);

            ValueCriterionTestHelper.AssertValueCriterionFormatExpression(attributeName,
                                                                          valueCriterion,
                                                                          properties.Criterion);
        }

        [Test]
        public void Constructor_WithValidArguments_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var categoryTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                      new LineStyle());

            // Call
            var properties = new LineCategoryThemeProperties(categoryTheme, string.Empty, new MapLineData("Name"));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de lijnen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor lineWidthProperty = dynamicProperties[lineWidthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lineWidthProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijnen waarmee deze categorie wordt weergegeven.");

            PropertyDescriptor lineStyleProperty = dynamicProperties[lineStylePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lineStyleProperty,
                                                                            styleCategory,
                                                                            "Lijnstijl",
                                                                            "De stijl van de lijnen waarmee deze categorie wordt weergegeven.");
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

            var categoryTheme = new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                      new LineStyle());

            var properties = new LineCategoryThemeProperties(categoryTheme, string.Empty, mapData);

            int width = random.Next(1, 48);
            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            var dashStyle = random.NextEnumValue<LineDashStyle>();

            // Call
            properties.Width = width;
            properties.Color = color;
            properties.DashStyle = dashStyle;

            // Assert
            LineStyle actualStyle = categoryTheme.Style;
            Assert.AreEqual(width, actualStyle.Width);
            Assert.AreEqual(color, actualStyle.Color);
            Assert.AreEqual(dashStyle, actualStyle.DashStyle);

            mocks.VerifyAll();
        }
    }
}