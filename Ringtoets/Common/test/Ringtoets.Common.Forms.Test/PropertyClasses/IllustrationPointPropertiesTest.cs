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
    public class IllustrationPointPropertiesTest
    {
        private const string illustrationPointCategoryName = "Illustratiepunten";

        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingScenarioPropertyIndex = 3;
        private const int illustrationPointPropertyIndex = 4;

        [Test]
        public void Constructor_IllustrationPointNodeNull_ThrowsException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(null, "Point name A", "Closing Situation");

            // Assert
            const string expectedMessage = "Value cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WindDirectionNull_ThrowsException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(new IllustrationPointNode(new TestIllustrationPoint()), null, "Closing Situation");

            // Assert
            const string expectedMessage = "Value cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(new IllustrationPointNode(new TestIllustrationPoint()), "SE", null);

            // Assert
            const string expectedMessage = "Value cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithoutChildren_CorrectValues()
        {
            // Call
            var faultTree = new IllustrationPointProperties(new IllustrationPointNode(new TestFaultTreeIllustrationPoint()), "NNE", "closing situation");

            // Assert
            Assert.AreEqual("NNE", faultTree.WindDirection);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointProperties.Reliability));
            Assert.AreEqual(3.14, faultTree.Reliability.Value);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(5, faultTree.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.14), faultTree.CalculatedProbability);
            Assert.AreEqual("closing situation", faultTree.ClosingSituation);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, ExpandableArrayConverter>(
                nameof(IllustrationPointProperties.IllustrationPoints));
            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(0, faultTree.IllustrationPoints.Length);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithChildren_CorrectValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
            });

            // Call
            var faultTree = new IllustrationPointProperties(illustrationPointNode, "N", "closing situation");

            // Assert
            Assert.AreEqual("N", faultTree.WindDirection);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointProperties.Reliability));
            Assert.AreEqual(3.14, faultTree.Reliability.Value);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(5, faultTree.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.14), faultTree.CalculatedProbability);
            Assert.AreEqual("closing situation", faultTree.ClosingSituation);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, ExpandableArrayConverter>(
                nameof(IllustrationPointProperties.IllustrationPoints));
            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(2, faultTree.IllustrationPoints.Length);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTree = new IllustrationPointProperties(new IllustrationPointNode(new TestFaultTreeIllustrationPoint("ImportantName")),
                                                            "NotImportant", "Closing Situation");

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual("ImportantName", toString);
        }

        [Test]
        public void Constructor_WithChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()),
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint())
            });

            // Call
            var faultTree = new IllustrationPointProperties(illustrationPointNode, "N", "Closing Situation");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor closingScenarioProperty = dynamicProperties[closingScenarioPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingScenarioProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void Constructor_WithoutChildIllustrationPointNodes_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());
            // Call
            var faultTree = new IllustrationPointProperties(illustrationPointNode, "N", "Closing Situation");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor closingScenarioProperty = dynamicProperties[closingScenarioPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingScenarioProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Sluitscenario",
                                                                            "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                            true);
        }

        [Test]
        public void Constructor_HiddenClosingSituation_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()),
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint())
            });

            // Call
            var faultTree = new IllustrationPointProperties(illustrationPointNode, "N", string.Empty);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }
    }
}