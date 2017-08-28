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
using System.Linq;
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
    public class IllustrationPointPropertiesTest
    {
        private const string illustrationPointCategoryName = "Illustratiepunten";

        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingScenarioPropertyIndex = 3;
        private const int illustrationPointPropertyIndex = 4;

        [Test]
        public void Constructor_IllustrationPointNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(null, "Point name A", "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("illustrationPointNode", paramName);
        }

        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(new IllustrationPointNode(new TestIllustrationPoint()),
                                                                      null,
                                                                      "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("windDirection", paramName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(new IllustrationPointNode(new TestIllustrationPoint()),
                                                                      "SE",
                                                                      null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("closingSituation", paramName);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithoutChildren_CorrectValues()
        {
            // Setup
            const string windDirection = "NNE";
            const string closingSituation = "closing situation";
            var random = new Random(15);

            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault Tree AAA",
                                                                                                 random.NextDouble(),
                                                                                                 Enumerable.Empty<Stochast>(),
                                                                                                 random.NextEnumValue<CombinationType>()));

            // Call
            var properties = new IllustrationPointProperties(illustrationPointNode,
                                                             windDirection,
                                                             closingSituation);

            // Assert
            Assert.AreEqual(windDirection, properties.WindDirection);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointProperties.Reliability));
            Assert.AreEqual(illustrationPointNode.Data.Beta, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(5, properties.Reliability.NumberOfDecimalPlaces);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(illustrationPointNode.Data.Beta),
                            properties.CalculatedProbability);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, ExpandableArrayConverter>(
                nameof(IllustrationPointProperties.IllustrationPoints));
            Assert.AreEqual(illustrationPointNode.Children.Count(), properties.IllustrationPoints.Length);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPointWithChildren_CorrectValues()
        {
            // Setup
            var random = new Random(15);

            const string windDirection = "N";
            const string closingSituation = "closing situation";
            var illustrationPointNode = new IllustrationPointNode(new SubMechanismIllustrationPoint("NNE",
                                                                                                    random.NextDouble(),
                                                                                                    Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                                                    Enumerable.Empty<IllustrationPointResult>()));
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault Tree A",
                                                                         random.NextDouble(),
                                                                         Enumerable.Empty<Stochast>(),
                                                                         CombinationType.And)),
                new IllustrationPointNode(new SubMechanismIllustrationPoint("Sub Mechanism B",
                                                                            4.2,
                                                                            Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                            Enumerable.Empty<IllustrationPointResult>()))
            });

            // Call
            var properties = new IllustrationPointProperties(illustrationPointNode, windDirection, closingSituation);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointPropertyIndex];
            Assert.NotNull(illustrationPointProperty.Attributes[typeof(KeyValueElementAttribute)]);

            Assert.AreEqual(windDirection, properties.WindDirection);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointProperties.Reliability));
            Assert.AreEqual(illustrationPointNode.Data.Beta, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(5, properties.Reliability.NumberOfDecimalPlaces);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(illustrationPointNode.Data.Beta), properties.CalculatedProbability);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, ExpandableArrayConverter>(
                nameof(IllustrationPointProperties.IllustrationPoints));
            Assert.AreEqual(illustrationPointNode.Children.Count(), properties.IllustrationPoints.Length);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            const string name = "ImportantName";
            var properties = new IllustrationPointProperties(new IllustrationPointNode(new TestFaultTreeIllustrationPoint(name)),
                                                             "NotImportant",
                                                             "Closing Situation");

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(name, toString);
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
            var properties = new IllustrationPointProperties(illustrationPointNode, "N", "Closing Situation");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
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
            var properties = new IllustrationPointProperties(illustrationPointNode, "N", "Closing Situation");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
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
            var properties = new IllustrationPointProperties(illustrationPointNode, "N", string.Empty);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
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