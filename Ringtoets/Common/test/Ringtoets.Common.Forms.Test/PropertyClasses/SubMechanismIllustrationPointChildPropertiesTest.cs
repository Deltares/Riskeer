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
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SubMechanismIllustrationPointChildPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingScenarioPropertyIndex = 3;
        private const int subMechanismStochastPropertyIndex = 4;

        private const string illustrationPointsCategoryName = "Illustratiepunten";

        [Test]
        public void Constructor_InvalidIllustrationPointType_ThrowsException()
        {
            // Setup
            const string expectedMessage = "illustrationPointNode data type has to be SubMechanismIllustrationPoint";

            // Call
            TestDelegate test = () => new SubMechanismIllustrationPointChildProperties(new IllustrationPointNode(
                                                                                           new FaultTreeIllustrationPoint("N",
                                                                                                                          1.5,
                                                                                                                          new Stochast[0],
                                                                                                                          CombinationType.And)),
                                                                                       "N");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup

            // Call
            var faultTree = new SubMechanismIllustrationPointChildProperties(new IllustrationPointNode(
                                                                                 new SubMechanismIllustrationPoint("N",
                                                                                                                   1.5,
                                                                                                                   new SubMechanismIllustrationPointStochast[0],
                                                                                                                   new IllustrationPointResult[0])),
                                                                             "N");

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 1.5);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(1.5));
            Assert.AreEqual(faultTree.Name, "N");

            Assert.IsNotNull(faultTree.SubMechanismStochasts);
            Assert.AreEqual(faultTree.SubMechanismStochasts.Length, 0);

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 0);
        }

        [Test]
        public void Constructor_WithSubMechanismIllustrationPoint_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var faultTree = new SubMechanismIllustrationPointChildProperties(new IllustrationPointNode(
                                                                                 new SubMechanismIllustrationPoint("N",
                                                                                                                   1.5,
                                                                                                                   new SubMechanismIllustrationPointStochast[0],
                                                                                                                   new IllustrationPointResult[0])),
                                                                             "N");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(faultTree);
            Assert.AreEqual(5, dynamicProperties.Count);

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
            var faultTree = new SubMechanismIllustrationPointChildProperties(new IllustrationPointNode(
                                                                                 new SubMechanismIllustrationPoint("N",
                                                                                                                   1.5,
                                                                                                                   new SubMechanismIllustrationPointStochast[0],
                                                                                                                   new IllustrationPointResult[0])),
                                                                             "N");

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual(toString, "N");
        }
    }
}