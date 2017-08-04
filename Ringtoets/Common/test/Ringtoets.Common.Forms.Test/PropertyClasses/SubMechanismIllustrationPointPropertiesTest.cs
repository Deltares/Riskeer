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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
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
        private const int subMechanismStochastPropertyIndex = 6;

        private const string illustrationPointsCategoryName = "Illustratiepunten";

        [Test]
        public void Constructor_InvalidIllustrationPointType_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new SubMechanismIllustrationPointProperties(new IllustrationPointNode(
                                                                                      new TestIllustrationPoint()),
                                                                                  "N");

            // Assert
            const string expectedMessage = "illustrationPointNode type has to be SubMechanismIllustrationPoint";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Call
            var subMechanismProperties = new SubMechanismIllustrationPointProperties(new IllustrationPointNode(
                                                                                         new TestSubMechanismIllustrationPoint(new[]
                                                                                         {
                                                                                             new SubMechanismIllustrationPointStochast("Test", 2.0, 4.5, 0.1)
                                                                                         })),
                                                                                     "N");

            // Assert
            Assert.AreEqual("N", subMechanismProperties.WindDirection);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(SubMechanismIllustrationPointProperties.Reliability));
            Assert.AreEqual(3.14, subMechanismProperties.Reliability.Value);
            Assert.AreEqual(5, subMechanismProperties.Reliability.NumberOfDecimalPlaces);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(SubMechanismIllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.14), subMechanismProperties.CalculatedProbability);
            Assert.AreEqual("Illustration Point", subMechanismProperties.Name);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.AlphaValues));
            Assert.IsNotEmpty(subMechanismProperties.AlphaValues);
            Assert.AreEqual(1, subMechanismProperties.AlphaValues.Length);
            Assert.AreEqual(4.5, subMechanismProperties.AlphaValues.First().Alpha);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.Durations));
            Assert.IsNotEmpty(subMechanismProperties.Durations);
            Assert.AreEqual(1, subMechanismProperties.Durations.Length);
            Assert.AreEqual(2.0, subMechanismProperties.Durations.First().Duration);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.SubMechanismStochasts));
            Assert.IsNotEmpty(subMechanismProperties.SubMechanismStochasts);
            Assert.AreEqual(1, subMechanismProperties.SubMechanismStochasts.Length);
            Assert.AreEqual(0.1, subMechanismProperties.SubMechanismStochasts.First().Realization);

            TestHelper.AssertTypeConverter<SubMechanismIllustrationPointProperties, ExpandableArrayConverter>(
                nameof(SubMechanismIllustrationPointProperties.IllustrationPoints));
            Assert.IsEmpty(subMechanismProperties.IllustrationPoints);
            Assert.AreEqual(0, subMechanismProperties.IllustrationPoints.Length);
        }

        [Test]
        public void Constructor_WithSubMechanismIllustrationPoint_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var subMechanismProperties = new SubMechanismIllustrationPointProperties(new IllustrationPointNode(new TestSubMechanismIllustrationPoint()), "N");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(subMechanismProperties);
            Assert.AreEqual(7, dynamicProperties.Count);

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

            PropertyDescriptor subMechanismStochastProperty = dynamicProperties[subMechanismStochastPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(subMechanismStochastProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Waarden in het illustratiepunt",
                                                                            "Realisaties van de stochasten in het illustratiepunt",
                                                                            true);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var subMechanismProperties = new SubMechanismIllustrationPointProperties(
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("Relevant")),
                "NotRelevant");

            // Call
            string toString = subMechanismProperties.ToString();

            // Assert
            Assert.AreEqual(toString, "Relevant");
        }
    }
}