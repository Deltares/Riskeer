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
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FaultTreeIllustrationPointPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingScenarioPropertyIndex = 3;
        private const int alphasPropertyIndex = 4;
        private const int durationsPropertyIndex = 5;
        private const int illustrationPointPropertyIndex = 6;

        private const string illustrationPointsCategoryName = "Illustratiepunten";

        [Test]
        public void Constructor_IllustrationPointNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointProperties(null, "Point name A", "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("illustrationPointNode", paramName);
        }

        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointProperties(new IllustrationPointNode(new TestFaultTreeIllustrationPoint()), null, "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("windDirection", paramName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsException()
        {
            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointProperties(new IllustrationPointNode(new TestFaultTreeIllustrationPoint()), "SE", null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("closingSituation", paramName);
        }

        [Test]
        public void Constructor_InvalidIllustrationPointType_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointProperties(new IllustrationPointNode(new TestIllustrationPoint()), "N", "Regular");

            // Assert
            const string expectedMessage = "illustrationPointNode type has to be FaultTreeIllustrationPoint";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_ReturnsExpectedValues()
        {
            // Setup
            var stochast = new Stochast("Stochast A", 10.0, 2.5);
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault tree Test",
                                                                                                 1.5,
                                                                                                 new[]
                                                                                                 {
                                                                                                     stochast
                                                                                                 },
                                                                                                 CombinationType.And));
            var illustrationPointNodeChild = new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault tree child",
                                                                                                      3.5,
                                                                                                      new Stochast[0],
                                                                                                      CombinationType.Or));
            var illustrationPointNodeChildren = new[]
            {
                illustrationPointNodeChild,
                illustrationPointNodeChild
            };
            illustrationPointNode.SetChildren(illustrationPointNodeChildren);

            // Call
            var faultTree = new FaultTreeIllustrationPointProperties(illustrationPointNode, "NNE", "closing situation");

            // Assert
            Assert.AreEqual("NNE", faultTree.WindDirection);
            Assert.AreEqual(1.5, faultTree.Reliability.Value);
            Assert.AreEqual(5, faultTree.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(1.5), faultTree.CalculatedProbability);
            Assert.AreEqual("closing situation", faultTree.ClosingSituation);

            CollectionAssert.IsNotEmpty(faultTree.AlphaValues);
            Assert.AreEqual(1, faultTree.AlphaValues.Length);
            foreach (Stochast alphaValue in faultTree.AlphaValues)
            {
                Assert.AreEqual(2.5, alphaValue.Alpha);
            }

            CollectionAssert.IsNotEmpty(faultTree.Durations);
            Assert.AreEqual(1, faultTree.Durations.Length);
            foreach (Stochast duration in faultTree.Durations)
            {
                Assert.AreEqual(10.0, duration.Duration);
            }

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(2, faultTree.IllustrationPoints.Length);
            foreach (IllustrationPointProperties illustrationPointProperties in faultTree.IllustrationPoints)
            {
                Assert.AreEqual("NNE", illustrationPointProperties.WindDirection);
                Assert.AreEqual("closing situation", illustrationPointProperties.ClosingSituation);
                Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.5), illustrationPointProperties.CalculatedProbability);
                Assert.AreEqual(3.5, illustrationPointProperties.Reliability);
                Assert.AreEqual(0, illustrationPointProperties.IllustrationPoints.Length);
            }
        }

        [Test]
        public void VisibleProperties_WithChildIllustrationPointNodes_ExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint()),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
            });

            // Call
            var faultTree = new FaultTreeIllustrationPointProperties(illustrationPointNode, "N", "Regular");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
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

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void VisibleProperties_HiddenClosingSituation_ExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint()),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
            });

            // Call
            var faultTree = new FaultTreeIllustrationPointProperties(illustrationPointNode, "N", string.Empty);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(6, dynamicProperties.Count);

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

            PropertyDescriptor alphasProperty = dynamicProperties[alphasPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphasProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Alfa's [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [min]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void VisibleProperties_WithoutChildIllustrationPointNodes_ExpectedAttributesValues()
        {
            // Setup
            var faultTree = new FaultTreeIllustrationPointProperties(
                new IllustrationPointNode(
                    new TestFaultTreeIllustrationPoint()), "N", "Regular");

            // Call
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);

            // Assert
            Assert.AreEqual(6, dynamicProperties.Count);

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
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTreeProperties = new FaultTreeIllustrationPointProperties(
                new IllustrationPointNode(
                    new TestFaultTreeIllustrationPoint("Point Name")),
                "Wind", "ClosingSit");

            // Call
            string toString = faultTreeProperties.ToString();

            // Assert
            Assert.AreEqual(toString, "Point Name");
        }
    }
}