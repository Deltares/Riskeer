// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
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

        [Test]
        public void Constructor_ChildNodesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointProperties(new TestFaultTreeIllustrationPoint(),
                                                                               null,
                                                                               "NNE",
                                                                               "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("childNodes", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var stochast = new Stochast("Stochast A", 10.0, 2.5);
            var illustrationPoint = new FaultTreeIllustrationPoint("Fault tree Test",
                                                                   1.5,
                                                                   new[]
                                                                   {
                                                                       stochast
                                                                   },
                                                                   CombinationType.And);
            var illustrationPointNodeChild1 = new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault tree child",
                                                                                                       3.5,
                                                                                                       new[]
                                                                                                       {
                                                                                                           stochast
                                                                                                       },
                                                                                                       CombinationType.Or));
            var illustrationPointNodeChild2 = new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault tree child 2",
                                                                                                       3.5,
                                                                                                       new Stochast[0],
                                                                                                       CombinationType.Or));
            IllustrationPointNode[] illustrationPointNodeChildren =
            {
                illustrationPointNodeChild1,
                illustrationPointNodeChild2
            };

            // Call
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, illustrationPointNodeChildren, "NNE", "closing situation");

            // Assert
            Assert.IsInstanceOf<IllustrationPointProperties>(properties);
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor alphasProperty = dynamicProperties[alphasPropertyIndex];
            Assert.NotNull(alphasProperty.Attributes[typeof(KeyValueElementAttribute)]);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);

            Assert.AreEqual("NNE", properties.WindDirection);
            Assert.AreEqual(illustrationPoint.Beta, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(5, properties.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(illustrationPoint.Beta), properties.CalculatedProbability);
            Assert.AreEqual("closing situation", properties.ClosingSituation);

            TestHelper.AssertTypeConverter<FaultTreeIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(FaultTreeIllustrationPointProperties.AlphaValues));
            CollectionAssert.IsNotEmpty(properties.AlphaValues);
            CollectionAssert.AreEqual(new[]
            {
                stochast
            }, properties.AlphaValues);

            TestHelper.AssertTypeConverter<FaultTreeIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(FaultTreeIllustrationPointProperties.Durations));
            CollectionAssert.AreEqual(new[]
            {
                stochast
            }, properties.Durations);

            TestHelper.AssertTypeConverter<FaultTreeIllustrationPointProperties, ExpandableArrayConverter>(
                nameof(FaultTreeIllustrationPointProperties.IllustrationPoints));
            Assert.AreEqual(2, properties.IllustrationPoints.Length);
            Assert.AreSame(illustrationPointNodeChild1.Data, properties.IllustrationPoints.ElementAt(0).Data);
            Assert.AreSame(illustrationPointNodeChild2.Data, properties.IllustrationPoints.ElementAt(1).Data);
        }

        [Test]
        public void VisibleProperties_WithChildIllustrationPointNodes_ExpectedAttributesValues()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var childNodes = new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
            };

            // Call
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, childNodes, "N", "Regular");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string illustrationPointsCategoryName = "Illustratiepunten";
            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Berekende kans [-]",
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
                                                                            "Keringsituatie",
                                                                            "De keringsituatie waarvoor dit illustratiepunt is berekend.",
                                                                            true);

            PropertyDescriptor alphasProperty = dynamicProperties[alphasPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphasProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [uur]",
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
        public void IsDynamicVisible_WithChildren_ReturnsTrue()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var childNodes = new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
            };
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, childNodes, "N", "Regular");

            // Call
            bool isVisible = properties.IsDynamicVisible(nameof(properties.IllustrationPoints));

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        public void IsDynamicVisible_WithoutChildren_ReturnsFalse()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, new IllustrationPointNode[0], "N", "Regular");

            // Call
            bool isVisible = properties.IsDynamicVisible(nameof(properties.IllustrationPoints));

            // Assert
            Assert.IsFalse(isVisible);
        }

        [Test]
        public void IsDynamicVisible_WithClosingSituation_ReturnsTrue()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, new IllustrationPointNode[0], "N", "Regular");

            // Call
            bool isVisible = properties.IsDynamicVisible(nameof(IllustrationPointProperties.ClosingSituation));

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        public void IsDynamicVisible_WithEmptyClosingSituation_ReturnsFalse()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, new IllustrationPointNode[0], "N", string.Empty);

            // Call
            bool isVisible = properties.IsDynamicVisible(nameof(IllustrationPointProperties.ClosingSituation));

            // Assert
            Assert.IsFalse(isVisible);
        }

        [Test]
        [TestCase(nameof(FaultTreeIllustrationPointProperties.WindDirection))]
        [TestCase(nameof(FaultTreeIllustrationPointProperties.CalculatedProbability))]
        [TestCase(nameof(FaultTreeIllustrationPointProperties.Reliability))]
        [TestCase(nameof(FaultTreeIllustrationPointProperties.AlphaValues))]
        [TestCase(nameof(FaultTreeIllustrationPointProperties.Durations))]
        public void IsDynamicVisible_WithOtherProperties_ReturnsFalse(string property)
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new FaultTreeIllustrationPointProperties(illustrationPoint, new IllustrationPointNode[0], "N", string.Empty);

            // Call
            bool isVisible = properties.IsDynamicVisible(property);

            // Assert
            Assert.IsFalse(isVisible);
        }
    }
}