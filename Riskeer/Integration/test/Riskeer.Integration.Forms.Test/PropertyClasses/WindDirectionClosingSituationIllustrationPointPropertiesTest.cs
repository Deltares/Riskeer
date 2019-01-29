// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Integration.Forms.PropertyClasses;
using System;
using System.Linq;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WindDirectionClosingSituationIllustrationPointPropertiesTest
    {

        [Test]
        public void Constructor_DefaultArgumentValues_DoesNotThrowException()
        {
            // Call
            var properties = new WindDirectionClosingSituationIllustrationPointProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WindDirectionClosingSituationIllustrationPoint>>(properties);
        }

        [Test]
        [TestCase("north")]
        [TestCase("some random name")]
        public void ToString_WithName_ReturnsCombinationOfWindDirectionAndClosingSituation(string name)
        {
            // Setup
            var illustrationPoint = new IllustrationPoint(name, Enumerable.Empty<SubmechanismIllustrationPointStochast>(), Enumerable.Empty<IllustrationPointResult>(), 3);
            var context = new WindDirectionClosingSituationIllustrationPoint(new TestWindDirection(), "direction", illustrationPoint);

            // Call
            var hydraulicBoundaryLocationProperties = new WindDirectionClosingSituationIllustrationPointProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.ToString());
        }


        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();
            var stochasts = new[]
            {
                new SubmechanismIllustrationPointStochast("some name", random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            var illustrationPointResults = new[]
            {
                new IllustrationPointResult("some description", random.NextDouble())
            };
            var illustrationPoint = new IllustrationPoint("name", stochasts, illustrationPointResults, beta);

            const string closingSituation = "closingSituation";
            const string windDirectionName = "windDirection";
            var windDirection = new WindDirection(windDirectionName, 123);

            var context = new WindDirectionClosingSituationIllustrationPoint(windDirection, closingSituation, illustrationPoint);

            // Call
            var properties = new WindDirectionClosingSituationIllustrationPointProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(windDirectionName, properties.WindDirection);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.AlphaValues);
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.Durations);
            CollectionAssert.AreEqual(illustrationPoint.IllustrationPointResults, properties.IllustrationPointResults);

            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);
            const string miscCategory = "Misc";
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[0],
                                                                            miscCategory,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[1],
                                                                            miscCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[2],
                                                                            miscCategory,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[3],
                                                                            miscCategory,
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            TestHelper.AssertTypeConverter<WindDirectionClosingSituationIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(WindDirectionClosingSituationIllustrationPointProperties.AlphaValues));
            PropertyDescriptor alphaValuesProperty = dynamicProperties[4];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            miscCategory,
                                                                            "Alfa's",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<WindDirectionClosingSituationIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(WindDirectionClosingSituationIllustrationPointProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[5];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            miscCategory,
                                                                            "Tijdsduren",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            TestHelper.AssertTypeConverter<WindDirectionClosingSituationIllustrationPointProperties, KeyValueExpandableArrayConverter>(nameof(WindDirectionClosingSituationIllustrationPointProperties.IllustrationPointResults));
            PropertyDescriptor illustrationPointResultsProperty = dynamicProperties[6];
            Assert.NotNull(illustrationPointResultsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointResultsProperty,
                                                                            miscCategory,
                                                                            "Waarden in het illustratiepunt",
                                                                            "Realisaties van de stochasten in het illustratiepunt.",
                                                                            true);
        }
    }
}