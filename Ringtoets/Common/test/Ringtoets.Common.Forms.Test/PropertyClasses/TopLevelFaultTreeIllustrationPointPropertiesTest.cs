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
    public class TopLevelFaultTreeIllustrationPointPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int closingSituationPropertyIndex = 3;
        private const int alphasPropertyIndex = 4;
        private const int durationsPropertyIndex = 5;
        private const int illustrationPointPropertyIndex = 6;

        [Test]
        public void Constructor_FaultTreeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TopLevelFaultTreeIllustrationPointProperties(null, new[]
            {
                "closing situation"
            });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("faultTreeData", paramName);
        }

        [Test]
        public void Constructor_ClosingSituationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            // Call
            TestDelegate test = () => new TopLevelFaultTreeIllustrationPointProperties(topLevel, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("allClosingSituations", paramName);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault Tree A",
                                                                         3.14,
                                                                         new[]
                                                                         {
                                                                             new Stochast("Stochast A", 2.5, 5.5)
                                                                         },
                                                                         CombinationType.And)));

            topLevel.FaultTreeNodeRoot.SetChildren(new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
            });

            // Call
            var faultTree = new TopLevelFaultTreeIllustrationPointProperties(topLevel, new[]
            {
                "closing situation"
            });

            // Assert
            Assert.AreEqual("N", faultTree.WindDirection);

            TestHelper.AssertTypeConverter<TopLevelFaultTreeIllustrationPointProperties, NoValueRoundedDoubleConverter>(
                nameof(TopLevelFaultTreeIllustrationPointProperties.Reliability));
            Assert.AreEqual(3.14, faultTree.Reliability.Value);
            Assert.AreEqual(5, faultTree.Reliability.NumberOfDecimalPlaces);

            TestHelper.AssertTypeConverter<TopLevelFaultTreeIllustrationPointProperties, NoProbabilityValueDoubleConverter>(
                nameof(TopLevelFaultTreeIllustrationPointProperties.CalculatedProbability));
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(3.14), faultTree.CalculatedProbability);

            Assert.AreEqual("closing situation", faultTree.ClosingSituation);

            TestHelper.AssertTypeConverter<TopLevelFaultTreeIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(TopLevelFaultTreeIllustrationPointProperties.AlphaValues));
            Assert.IsNotNull(faultTree.AlphaValues);
            Assert.AreEqual(1, faultTree.AlphaValues.Length);
            foreach (Stochast alpha in faultTree.AlphaValues)
            {
                Assert.AreEqual(5.5, alpha.Alpha);
            }

            TestHelper.AssertTypeConverter<TopLevelFaultTreeIllustrationPointProperties, KeyValueExpandableArrayConverter>(
                nameof(TopLevelFaultTreeIllustrationPointProperties.Durations));
            Assert.IsNotNull(faultTree.Durations);
            Assert.AreEqual(1, faultTree.Durations.Length);
            foreach (Stochast duration in faultTree.Durations)
            {
                Assert.AreEqual(2.5, duration.Duration);
            }

            TestHelper.AssertTypeConverter<TopLevelFaultTreeIllustrationPointProperties, ExpandableArrayConverter>(
                nameof(TopLevelFaultTreeIllustrationPointProperties.IllustrationPoints));
            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(2, faultTree.IllustrationPoints.Length);
        }

        [Test]
        public void Constructor_UniqueClosingSituations_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            // Call
            var topLevelFaultTree = new TopLevelFaultTreeIllustrationPointProperties(topLevel, new[]
            {
                "closing situation"
            });

            // Assert
            const string illustrationPointsCategoryName = "Illustratiepunten";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(topLevelFaultTree);
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
        public void Constructor_NoUniqueClosingSituations_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            // Call
            var topLevelFaultTree = new TopLevelFaultTreeIllustrationPointProperties(topLevel, new[]
            {
                "closing situation",
                "closing situation 2"
            });

            // Assert
            const string illustrationPointsCategoryName = "Illustratiepunten";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(topLevelFaultTree);
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

            PropertyDescriptor closingSituationProperty = dynamicProperties[closingSituationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(closingSituationProperty,
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
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            var topLevelFaultTreeProperties = new TopLevelFaultTreeIllustrationPointProperties(topLevelFaultTreeIllustrationPoint, new string[0]);

            // Call
            string toString = topLevelFaultTreeProperties.ToString();

            // Assert
            Assert.AreEqual("N", toString);
        }
    }
}