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
using Core.Common.TestUtil;
using Core.Common.Util;
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

        [Test]
        public void Constructor_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(null,
                                                                      "Point name A",
                                                                      "Closing Situation");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("illustrationPoint", paramName);
        }

        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointProperties(new TestIllustrationPoint(),
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
            TestDelegate test = () => new IllustrationPointProperties(new TestIllustrationPoint(),
                                                                      "SE",
                                                                      null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("closingSituation", paramName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var random = new Random(15);

            const string windDirection = "N";
            const string closingSituation = "closing situation";
            var illustrationPoint = new SubMechanismIllustrationPoint("NNE",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());

            // Call
            var properties = new IllustrationPointProperties(illustrationPoint, windDirection, closingSituation);

            // Assert
            Assert.AreEqual(windDirection, properties.WindDirection);

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointProperties.Reliability));
            Assert.AreEqual(illustrationPoint.Beta, properties.Reliability, properties.Reliability.GetAccuracy());

            TestHelper.AssertTypeConverter<IllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(illustrationPoint.Beta), properties.CalculatedProbability);
            Assert.AreEqual(closingSituation, properties.ClosingSituation);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

            // Call
            var properties = new IllustrationPointProperties(illustrationPointNode.Data, "N", "Closing Situation");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Berekende kans [-]",
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
                                                                            "Keringsituatie",
                                                                            "De keringsituatie waarvoor dit illustratiepunt is berekend.",
                                                                            true);
        }

        [Test]
        public void Constructor_NoClosingSituation_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();

            // Call
            var properties = new IllustrationPointProperties(illustrationPoint, "N", string.Empty);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointCategoryName,
                                                                            "Berekende kans [-]",
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
        }

        [Test]
        public void ToString_Always_ReturnIllustrationPointName()
        {
            // Setup
            const string name = "ImportantName";
            var illustrationPoint = new TestFaultTreeIllustrationPoint(name);
            var properties = new IllustrationPointProperties(illustrationPoint,
                                                             "NotImportant",
                                                             "Closing Situation");

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(name, toString);
        }

        [Test]
        public void IsDynamicVisible_WithClosingSituation_ReturnsTrue()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new IllustrationPointProperties(illustrationPoint,
                                                             "NotImportant",
                                                             "Closing Situation");

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
            var properties = new IllustrationPointProperties(illustrationPoint,
                                                             "NotImportant",
                                                             string.Empty);

            // Call
            bool isVisible = properties.IsDynamicVisible(nameof(IllustrationPointProperties.ClosingSituation));

            // Assert
            Assert.IsFalse(isVisible);
        }

        [Test]
        [TestCase(nameof(IllustrationPointProperties.WindDirection))]
        [TestCase(nameof(IllustrationPointProperties.CalculatedProbability))]
        [TestCase(nameof(IllustrationPointProperties.Reliability))]
        public void IsDynamicVisible_WithOtherProperties_ReturnsFalse(string property)
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();
            var properties = new IllustrationPointProperties(illustrationPoint,
                                                             "NotImportant",
                                                             string.Empty);

            // Call
            bool isVisible = properties.IsDynamicVisible(property);

            // Assert
            Assert.IsFalse(isVisible);
        }
    }
}