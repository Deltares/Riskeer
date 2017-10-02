﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SubMechanismIllustrationPointPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingScenarioPropertyIndex = 3;
        private const int alphasPropertyIndex = 4;
        private const int durationsPropertyIndex = 5;
        private const int realizationsPropertyIndex = 6;
        private const int resultsPropertyIndex = 7;

        private const string illustrationPointsCategoryName = "Illustratiepunten";

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string windDirection = "N";
            const string closingSituation = "closing situation";
            var illustrationPoint = new SubMechanismIllustrationPoint("Submechanism A",
                                                                      5.4,
                                                                      new[]
                                                                      {
                                                                          new SubMechanismIllustrationPointStochast("Test", 2.0, 4.5, 0.1)
                                                                      },
                                                                      new[]
                                                                      {
                                                                          new IllustrationPointResult("result A", 2.3)
                                                                      });

            // Call
            var properties = new SubMechanismIllustrationPointProperties(illustrationPoint,
                                                                         windDirection,
                                                                         closingSituation);

            // Assert
            Assert.IsInstanceOf<IllustrationPointProperties>(properties);
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor alphasProperty = dynamicProperties[alphasPropertyIndex];
            Assert.NotNull(alphasProperty.Attributes[typeof(KeyValueElementAttribute)]);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);

            PropertyDescriptor realizationsProperty = dynamicProperties[realizationsPropertyIndex];
            Assert.NotNull(realizationsProperty.Attributes[typeof(KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute)]);

            PropertyDescriptor resultsProperty = dynamicProperties[resultsPropertyIndex];
            Assert.NotNull(resultsProperty.Attributes[typeof(KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute)]);

            Assert.AreEqual(windDirection, properties.WindDirection);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(SubMechanismIllustrationPointProperties.Reliability));
            Assert.AreEqual(illustrationPoint.Beta, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(5, properties.Reliability.NumberOfDecimalPlaces);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(SubMechanismIllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(illustrationPoint.Beta), properties.CalculatedProbability);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.AlphaValues));
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.AlphaValues);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.Durations));
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.Durations);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.Realizations));
            CollectionAssert.AreEqual(illustrationPoint.Stochasts, properties.Realizations);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.Results));
            CollectionAssert.AreEqual(illustrationPoint.IllustrationPointResults, properties.Results);
        }

        [Test]
        public void Constructor_WithSubMechanismIllustrationPoint_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

            // Call
            var subMechanismProperties = new SubMechanismIllustrationPointProperties((SubMechanismIllustrationPoint) illustrationPointNode.Data,
                                                                                     "N",
                                                                                     "Regular");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(subMechanismProperties);
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor closingScenarioProperty = dynamicProperties[closingScenarioPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingScenarioProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor alphasProperty = dynamicProperties[alphasPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphasProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Alfa's [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [min]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor realizationsProperty = dynamicProperties[realizationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(realizationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Realisaties in het illustratiepunt",
                                                                            "Realisaties van de stochasten in het illustratiepunt.",
                                                                            true);

            PropertyDescriptor resultsProperty = dynamicProperties[resultsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(resultsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Waarden in het illustratiepunt",
                                                                            "Waarden van variabelen in het illustratiepunt.",
                                                                            true);
        }
    }
}