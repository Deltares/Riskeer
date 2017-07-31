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
    public class FaultTreeIllustrationPointChildPropertiesTest
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
        public void Constructor_InvalidIllustrationPointType_ThrowsException()
        {
            // Setup
            const string expectedMessage = "illustrationPointNode type has to be FaultTreeIllustrationPoint";

            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointChildProperties(new IllustrationPointNode(new TestSubMechanismIllustrationPoint()), "N");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
                                                                                                 1.5,
                                                                                                 new Stochast[0],
                                                                                                 CombinationType.And));
            var illustrationPointNodeChild = new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
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
            var faultTree = new FaultTreeIllustrationPointChildProperties(illustrationPointNode, "N");

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 1.5);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(1.5));
            Assert.AreEqual(faultTree.Name, "N");

            Assert.IsNotNull(faultTree.AlphaValues);
            Assert.AreEqual(faultTree.AlphaValues.Length, 0);

            Assert.IsNotNull(faultTree.Durations);
            Assert.AreEqual(faultTree.Durations.Length, 0);

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 2);
            Assert.AreEqual(faultTree.IllustrationPoints[0].WindDirection, "N");
            Assert.AreEqual(faultTree.IllustrationPoints[0].Name, "N");
            Assert.AreEqual(faultTree.IllustrationPoints[0].CalculatedProbability, StatisticsConverter.ReliabilityToProbability(3.5));
            Assert.AreEqual(faultTree.IllustrationPoints[0].Reliability, 3.5);
            Assert.AreEqual(faultTree.IllustrationPoints[0].IllustrationPoints.Length, 0);
        }

        [Test]
        public void Constructor_WithChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
                                                                                                 1.5,
                                                                                                 new Stochast[0],
                                                                                                 CombinationType.And));
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
                                                                         7.2,
                                                                         new Stochast[0],
                                                                         CombinationType.And)),
                new IllustrationPointNode(new SubMechanismIllustrationPoint("N",
                                                                            7.2,
                                                                            new SubMechanismIllustrationPointStochast[0],
                                                                            new IllustrationPointResult[0]))
            });

            // Call
            var faultTree = new FaultTreeIllustrationPointChildProperties(illustrationPointNode, "N");

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
                                                                            "",
                                                                            true);
        }

        [Test]
        public void Constructor_WithoutChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
                                                                                                 1.5,
                                                                                                 new Stochast[0],
                                                                                                 CombinationType.And));

            // Call
            var faultTree = new FaultTreeIllustrationPointChildProperties(illustrationPointNode, "N");

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
            var faultTree = new FaultTreeIllustrationPointChildProperties(new IllustrationPointNode(new FaultTreeIllustrationPoint("N",
                                                                                                                                   1.5,
                                                                                                                                   new Stochast[0],
                                                                                                                                   CombinationType.And)),
                                                                          "N");

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual(toString, "N");
        }
    }
}