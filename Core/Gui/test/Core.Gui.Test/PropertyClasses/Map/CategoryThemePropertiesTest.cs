﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using Core.Gui.PropertyBag;
using Core.Gui.PropertyClasses.Map;
using Core.Gui.TestUtil;
using NUnit.Framework;

namespace Core.Gui.Test.PropertyClasses.Map
{
    [TestFixture]
    public class CategoryThemePropertiesTest
    {
        private const int criterionPropertyIndex = 0;

        [Test]
        public void Constructor_AttributeNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var categoryTheme = new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion());

            // Call
            TestDelegate call = () => new TestCategoryThemeProperties(null, categoryTheme, new TestFeatureBasedMapData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("attributeName", exception.ParamName);
        }

        [Test]
        public void Constructor_CategoryThemeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestCategoryThemeProperties(string.Empty, null, new TestFeatureBasedMapData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categoryTheme", exception.ParamName);
        }

        [Test]
        public void Constructor_MapDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var categoryTheme = new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion());

            // Call
            TestDelegate call = () => new TestCategoryThemeProperties(string.Empty, categoryTheme, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var theme = new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion());

            // Call
            var properties = new TestCategoryThemeProperties(string.Empty, theme, new TestFeatureBasedMapData());

            // Assert
            Assert.AreSame(theme, properties.Data);
            Assert.IsInstanceOf<ObjectProperties<TestCategoryTheme>>(properties);
            TestHelper.AssertTypeConverter<CategoryThemeProperties<CategoryTheme>, ExpandableObjectConverter>();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var theme = new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion());

            // Call
            var properties = new TestCategoryThemeProperties(string.Empty, theme, new TestFeatureBasedMapData());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string themeCategory = "Stijl";
            PropertyDescriptor criterionProperty = dynamicProperties[criterionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criterionProperty,
                                                                            themeCategory,
                                                                            "Criterium",
                                                                            "Het criterium van deze categorie.",
                                                                            true);
        }

        [Test]
        [TestCase(ValueCriterionOperator.EqualValue, "{0} = {1}")]
        [TestCase(ValueCriterionOperator.UnequalValue, "{0} ≠ {1}")]
        public void Constructor_Always_ReturnExpectedValues(ValueCriterionOperator valueOperator,
                                                            string formatExpression)
        {
            // Setup
            const string attributeName = "AttributeName";
            const string value = "random value 123";

            var criterion = new ValueCriterion(valueOperator, value);
            var theme = new TestCategoryTheme(criterion);

            // Call
            var properties = new TestCategoryThemeProperties(attributeName, theme, new TestFeatureBasedMapData());

            // Assert
            string expectedValue = string.Format(formatExpression, attributeName, value);
            Assert.AreEqual(expectedValue, properties.Criterion);
        }

        [Test]
        public void ToString_Always_ReturnsEmptyString()
        {
            // Setup
            var theme = new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion());
            var properties = new TestCategoryThemeProperties(string.Empty, theme, new TestFeatureBasedMapData());

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.IsEmpty(toString);
        }

        private class TestCategoryThemeProperties : CategoryThemeProperties<TestCategoryTheme>
        {
            public TestCategoryThemeProperties(string attributeName, TestCategoryTheme categoryTheme, FeatureBasedMapData mapData)
                : base(categoryTheme, attributeName, mapData) {}
        }

        private class TestCategoryTheme : CategoryTheme
        {
            public TestCategoryTheme(ValueCriterion criterion) : base(criterion) {}
        }
    }
}