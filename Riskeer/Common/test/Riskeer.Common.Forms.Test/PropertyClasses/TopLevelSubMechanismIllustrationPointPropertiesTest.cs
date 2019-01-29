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
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class TopLevelSubMechanismIllustrationPointPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int calculatedProbabilityPropertyIndex = 1;
        private const int calculatedReliabilityPropertyIndex = 2;
        private const int windDirectionNamePropertyIndex = 3;
        private const int alphaValuesPropertyIndex = 4;
        private const int durationsPropertyIndex = 5;
        private const int illustrationPointValuesPropertyIndex = 6;

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TopLevelSubMechanismIllustrationPointProperties(null, Enumerable.Empty<string>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                 string.Empty,
                                                                 new TestSubMechanismIllustrationPoint());

            // Call
            TestDelegate call = () => new TopLevelSubMechanismIllustrationPointProperties(data, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituations", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArgumentValues_DoesNotThrowException()
        {
            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          string.Empty,
                                                          new TestSubMechanismIllustrationPoint()),
                Enumerable.Empty<string>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<TopLevelSubMechanismIllustrationPoint>>(properties);
        }

        [Test]
        public void ToString_DifferentClosingSituations_ReturnsCombinationOfWindDirectionAndClosingSituation()
        {
            // Setup
            string illustrationPointName = string.Empty;
            var subMechanismIllustrationPoint =
                new SubMechanismIllustrationPoint(illustrationPointName,
                                                  3,
                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                  Enumerable.Empty<IllustrationPointResult>());

            const string closingSituation = "direction";
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var context = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                    closingSituation,
                                                                    subMechanismIllustrationPoint);

            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(
                context,
                new[]
                {
                    closingSituation,
                    closingSituation,
                    "Different situation"
                });

            // Assert
            string expectedStringValue = $"{windDirection.Name} ({closingSituation})";
            Assert.AreEqual(expectedStringValue, properties.ToString());
        }

        [Test]
        public void ToString_SameClosingSituations_ReturnsWindDirectionName()
        {
            // Setup
            string illustrationPointName = string.Empty;
            var subMechanismIllustrationPoint =
                new SubMechanismIllustrationPoint(illustrationPointName,
                                                  3,
                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                  Enumerable.Empty<IllustrationPointResult>());

            const string closingSituation = "direction";
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var context = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                    closingSituation,
                                                                    subMechanismIllustrationPoint);

            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(
                context,
                new[]
                {
                    closingSituation,
                    closingSituation
                });

            // Assert
            string expectedStringValue = $"{windDirection.Name}";
            Assert.AreEqual(expectedStringValue, properties.ToString());
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            var random = new Random(21);
            double beta = random.NextDouble();
            var stochasts = new[]
            {
                new SubMechanismIllustrationPointStochast("some name", random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            var illustrationPointResults = new[]
            {
                new IllustrationPointResult("some description", random.NextDouble())
            };

            const string illustrationPointName = "name";
            var subMechanismIllustrationPoint = new SubMechanismIllustrationPoint(illustrationPointName,
                                                                                  beta,
                                                                                  stochasts,
                                                                                  illustrationPointResults);

            const string closingSituation = "closingSituation";
            const string windDirectionName = "windDirection";
            var illustrationPoint = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(windDirectionName),
                                                                              closingSituation,
                                                                              subMechanismIllustrationPoint);

            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(illustrationPoint, Enumerable.Empty<string>());

            // Assert
            Assert.AreEqual(illustrationPointName, properties.Name);
            Assert.AreEqual(windDirectionName, properties.WindDirection);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);
            CollectionAssert.AreEqual(subMechanismIllustrationPoint.Stochasts, properties.AlphaValues);
            CollectionAssert.AreEqual(subMechanismIllustrationPoint.Stochasts, properties.Durations);

            Assert.AreSame(illustrationPoint.SubMechanismIllustrationPoint,
                           properties.SubMechanismIllustrationPointValues.Data);
        }

        [Test]
        public void GetProperties_DifferentClosingSituations_ReturnsExpectedAttributeValues()
        {
            // Setup
            var subMechanismIllustrationPoint =
                new SubMechanismIllustrationPoint(string.Empty,
                                                  3,
                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                  Enumerable.Empty<IllustrationPointResult>());

            const string closingSituation = "Closing Situation";
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var data = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                 closingSituation,
                                                                 subMechanismIllustrationPoint);

            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(
                data,
                new[]
                {
                    closingSituation,
                    closingSituation,
                    "Different closing situation"
                });

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[namePropertyIndex],
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van dit berekende illustratiepunt.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[calculatedProbabilityPropertyIndex],
                                                                            generalCategory,
                                                                            "Berekende kans [-]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[calculatedReliabilityPropertyIndex],
                                                                            generalCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[windDirectionNamePropertyIndex],
                                                                            generalCategory,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[4],
                                                                            generalCategory,
                                                                            "Keringsituatie",
                                                                            "De keringsituatie waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            TestHelper.AssertTypeConverter<TopLevelSubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(TopLevelSubMechanismIllustrationPointProperties.AlphaValues));
            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesPropertyIndex + 1];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            generalCategory,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<TopLevelSubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(TopLevelSubMechanismIllustrationPointProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex + 1];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            generalCategory,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointValuesProperties = dynamicProperties[illustrationPointValuesPropertyIndex + 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointValuesProperties,
                                                                            generalCategory,
                                                                            "Waarden in het illustratiepunt",
                                                                            "",
                                                                            true);
        }

        [Test]
        public void GetProperties_SameClosingSituations_ReturnsExpectedAttributeValues()
        {
            // Setup
            var context = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                    "closingSituation",
                                                                    new TestSubMechanismIllustrationPoint());
            const string closingSituation = "Closing Situation";

            // Call
            var properties = new TopLevelSubMechanismIllustrationPointProperties(
                context,
                new[]
                {
                    closingSituation,
                    closingSituation
                });

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[namePropertyIndex],
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van dit berekende illustratiepunt.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[calculatedProbabilityPropertyIndex],
                                                                            generalCategory,
                                                                            "Berekende kans [-]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[calculatedReliabilityPropertyIndex],
                                                                            generalCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[windDirectionNamePropertyIndex],
                                                                            generalCategory,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            TestHelper.AssertTypeConverter<TopLevelSubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(TopLevelSubMechanismIllustrationPointProperties.AlphaValues));
            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesPropertyIndex];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            generalCategory,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<TopLevelSubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(TopLevelSubMechanismIllustrationPointProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            generalCategory,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointValuesProperties = dynamicProperties[illustrationPointValuesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointValuesProperties,
                                                                            generalCategory,
                                                                            "Waarden in het illustratiepunt",
                                                                            "",
                                                                            true);
        }
    }
}