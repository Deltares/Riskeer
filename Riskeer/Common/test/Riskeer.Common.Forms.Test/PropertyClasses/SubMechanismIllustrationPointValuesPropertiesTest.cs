// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SubMechanismIllustrationPointValuesPropertiesTest
    {
        private const int realizationsPropertyIndex = 0;
        private const int resultsPropertyIndex = 1;

        [Test]
        public void Constructor_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SubMechanismIllustrationPointValuesProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArgument_ReturnsExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var illustrationPoint = new SubMechanismIllustrationPoint(string.Empty,
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());

            // Call
            var properties = new SubMechanismIllustrationPointValuesProperties(illustrationPoint);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<SubMechanismIllustrationPoint>>(properties);
            Assert.AreSame(illustrationPoint, properties.Data);

            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointValuesProperties, KeyValueExpandableArrayConverter>(nameof(SubMechanismIllustrationPointValuesProperties.Realizations));
            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointValuesProperties, KeyValueExpandableArrayConverter>(nameof(SubMechanismIllustrationPointValuesProperties.Results));
        }

        [Test]
        public void GetProperties_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var stochasts = new[]
            {
                new SubMechanismIllustrationPointStochast("some name", random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            var illustrationPointResults = new[]
            {
                new IllustrationPointResult("some description", random.NextDouble())
            };

            var illustrationPoint = new SubMechanismIllustrationPoint("name",
                                                                      random.NextDouble(),
                                                                      stochasts,
                                                                      illustrationPointResults);

            // Call
            var properties = new SubMechanismIllustrationPointValuesProperties(illustrationPoint);

            // Assert
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.Realizations);
            CollectionAssert.AreEqual(illustrationPoint.IllustrationPointResults, properties.Results);
        }

        [Test]
        public void ToString_WithValidData_ReturnsEmptyString()
        {
            // Setup
            var random = new Random(21);
            var illustrationPoint = new SubMechanismIllustrationPoint("Not an empty string",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());

            var properties = new SubMechanismIllustrationPointValuesProperties(illustrationPoint);

            // Call
            string toStringValue = properties.ToString();

            // Assert
            Assert.IsEmpty(toStringValue);
        }

        [Test]
        public void GetProperties_WithValidData_ReturnsExpectedAttributeValues()
        {
            // Setup
            var random = new Random(21);
            var illustrationPoint = new SubMechanismIllustrationPoint("name",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());

            // Call
            var properties = new SubMechanismIllustrationPointValuesProperties(illustrationPoint);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string category = "Illustratiepunten";

            PropertyDescriptor realizationsProperty = dynamicProperties[realizationsPropertyIndex];
            Assert.NotNull(realizationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(realizationsProperty,
                                                                            category,
                                                                            "Stochastwaarden",
                                                                            "Realisaties van de stochasten in het illustratiepunt.",
                                                                            true);

            PropertyDescriptor resultsProperty = dynamicProperties[resultsPropertyIndex];
            Assert.NotNull(resultsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(resultsProperty,
                                                                            category,
                                                                            "Afgeleide variabelen",
                                                                            "Waarden van afgeleide variabelen in het illustratiepunt.",
                                                                            true);
        }
    }
}