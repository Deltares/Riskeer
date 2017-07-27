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
    public class IllustrationPointChildPropertyTest
    {
        [Test]
        public void Constructor_IllustrationPointNodeNull_ThrowsException()
        {
            // Setup
            const string expectedMessage = "Value cannot be null.";

            // Call
            TestDelegate test = () => new IllustrationPointChildProperty(null, "N");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WindDirectionNull_ThrowsException()
        {
            // Setup
            const string expectedMessage = "Value cannot be null.";

            // Call
            TestDelegate test = () => new IllustrationPointChildProperty(new IllustrationPointNode(new TestIllustrationPoint()), null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithoutChildren_CorrectValues()
        {
            // Setup

            // Call
            var faultTree = new IllustrationPointChildProperty(new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 1.5, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0])), "N");

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 1.5);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(1.5));
            Assert.AreEqual(faultTree.Name, "N");

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 0);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithChildren_CorrectValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new SubMechanismIllustrationPoint("N",
                                                                                                    1.5,
                                                                                                    new SubMechanismIllustrationPointStochast[0],
                                                                                                    new IllustrationPointResult[0]));
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("N", 7.2, new Stochast[0], CombinationType.And)),
                new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 7.2, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0]))
            });

            // Call
            var faultTree = new IllustrationPointChildProperty(illustrationPointNode, "N");

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 1.5);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(1.5));
            Assert.AreEqual(faultTree.Name, "N");

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 2);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTree = new IllustrationPointChildProperty(new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 1.5, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0])), "N");

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual(toString, "N");

        }

        [Test]
        public void Constructor_WithChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new SubMechanismIllustrationPoint("N",
                                                                                                    1.5,
                                                                                                    new SubMechanismIllustrationPointStochast[0],
                                                                                                    new IllustrationPointResult[0]));
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("N", 7.2, new Stochast[0], CombinationType.And)),
                new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 7.2, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0]))
            });

            // Call
            var faultTree = new IllustrationPointChildProperty(illustrationPointNode, "N");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            "Misc",
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            "Misc",
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            "Misc",
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor closingScenarioProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingScenarioProperty,
                                                                            "Misc",
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            "Misc",
                                                                            "Illustratiepunten",
                                                                            "",
                                                                            true);
        }

        [Test]
        public void Constructor_WithoutChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new SubMechanismIllustrationPoint("N",
                                                                                                    1.5,
                                                                                                    new SubMechanismIllustrationPointStochast[0],
                                                                                                    new IllustrationPointResult[0]));
            // Call
            var faultTree = new IllustrationPointChildProperty(illustrationPointNode, "N");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            "Misc",
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            "Misc",
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            "Misc",
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor closingScenarioProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingScenarioProperty,
                                                                            "Misc",
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);
        }

    }
}