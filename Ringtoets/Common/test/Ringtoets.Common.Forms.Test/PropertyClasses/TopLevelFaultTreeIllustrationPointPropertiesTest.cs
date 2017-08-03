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
    public class TopLevelFaultTreeIllustrationPointPropertiesTest
    {
        [Test]
        public void Constructor_Null_ThrowsException()
        {
            // Call
            TestDelegate test = () => new TopLevelFaultTreeIllustrationPointProperties(null);

            // Assert
            const string expectedMessage = "Value cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint(new[]
                {
                    new Stochast("Stochast A", 2.5, 5.5)
                })));

            topLevel.FaultTreeNodeRoot.SetChildren(new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()),
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint())
            });

            // Call
            var faultTree = new TopLevelFaultTreeIllustrationPointProperties(topLevel);

            // Assert
            Assert.AreEqual("N", faultTree.WindDirection);
            Assert.AreEqual(3.14, faultTree.Reliability.Value);
            Assert.AreEqual(5, faultTree.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.14), faultTree.CalculatedProbability);
            Assert.AreEqual("closing situation", faultTree.ClosingSituation);

            Assert.IsNotNull(faultTree.AlphaValues);
            Assert.AreEqual(1, faultTree.AlphaValues.Length);
            Assert.AreEqual(5.5, faultTree.AlphaValues[0].Alpha);

            Assert.IsNotNull(faultTree.Durations);
            Assert.AreEqual(1, faultTree.Durations.Length);
            Assert.AreEqual(2.5, faultTree.Durations[0].Duration);

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(2, faultTree.IllustrationPoints.Length);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(bool showClosingSituations)
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            // Call
            var topLevelFaultTree = new TopLevelFaultTreeIllustrationPointProperties(topLevel, showClosingSituations);

            // Assert
            const string illustrationPointsCategoryName = "Illustratiepunten";
            int closingSituationIndexOffset = showClosingSituations ? 1 : 0;

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(topLevelFaultTree);
            Assert.AreEqual(6 + closingSituationIndexOffset, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Windrichting",
                                                                            "De windrichting waarvoor dit illlustratiepunt is berekend.",
                                                                            true);

            if (showClosingSituations)
            {
                PropertyDescriptor closingSituationProperty = dynamicProperties[3];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingSituationProperty,
                                                                                illustrationPointsCategoryName,
                                                                                "Sluitscenario",
                                                                                "Het sluitscenario waarvoor dit illustratiepunt is berekend.",
                                                                                true);
            }

            PropertyDescriptor alphasProperty = dynamicProperties[3 + closingSituationIndexOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphasProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Alfa's [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[4 + closingSituationIndexOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [min]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[5 + closingSituationIndexOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTreeProperties = new FaultTreeIllustrationPointProperties(
                new IllustrationPointNode(
                    new TestFaultTreeIllustrationPoint("VeryImportant")),
                "NotImportant");

            // Call
            string toString = faultTreeProperties.ToString();

            // Assert
            Assert.AreEqual("VeryImportant", toString);
        }
    }
}