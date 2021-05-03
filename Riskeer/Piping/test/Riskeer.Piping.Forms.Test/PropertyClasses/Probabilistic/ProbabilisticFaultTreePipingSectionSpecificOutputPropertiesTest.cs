// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Converters;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.Probabilistic
{
    [TestFixture]
    public class ProbabilisticFaultTreePipingSectionSpecificOutputPropertiesTest
    {
        private const int probabilityPropertyIndex = 0;
        private const int reliabilityPropertyIndex = 1;
        private const int windDirectionPropertyIndex = 2;
        private const int alphaValuesPropertyIndex = 3;
        private const int durationsPropertyIndex = 4;
        private const int illustrationPointsPropertyIndex = 5;

        private const string illustrationPointsCategoryName = "Illustratiepunten";
        private const string resultCategoryName = "\tResultaat";

        [Test]
        public void Constructor_OutputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            PartialProbabilisticFaultTreePipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput();

            // Call
            var properties = new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(output);

            // Assert
            Assert.IsInstanceOf<ProbabilisticFaultTreePipingSectionSpecificOutputProperties>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void Constructor_HasGeneralResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            PartialProbabilisticFaultTreePipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput();

            // Call
            var properties = new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(output);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Maatgevende windrichting",
                                                                            "De windrichting waarvoor de berekende betrouwbaarheidsindex het laagst is.",
                                                                            true);

            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesPropertyIndex];
            TestHelper.AssertTypeConverter<ProbabilisticFaultTreePipingSectionSpecificOutputProperties, KeyValueExpandableArrayConverter>(
                nameof(ProbabilisticFaultTreePipingSectionSpecificOutputProperties.AlphaValues));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            TestHelper.AssertTypeConverter<ProbabilisticFaultTreePipingSectionSpecificOutputProperties, KeyValueExpandableArrayConverter>(
                nameof(ProbabilisticFaultTreePipingSectionSpecificOutputProperties.Durations));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointsPropertyIndex];
            TestHelper.AssertTypeConverter<ProbabilisticFaultTreePipingSectionSpecificOutputProperties, ExpandableArrayConverter>(
                nameof(ProbabilisticFaultTreePipingSectionSpecificOutputProperties.IllustrationPoints));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void Constructor_NoGeneralResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            PartialProbabilisticFaultTreePipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(null);

            // Call
            var properties = new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(output);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            PartialProbabilisticFaultTreePipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput();

            // Call
            var properties = new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(output);

            // Assert
            Assert.AreEqual(ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(output.Reliability)), properties.Probability);
            Assert.AreEqual(output.Reliability, properties.Reliability, properties.Reliability.GetAccuracy());

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = output.GeneralResult;

            Assert.AreEqual(generalResult.GoverningWindDirection.Name, properties.WindDirection);

            int nrOfExpectedStochasts = generalResult.Stochasts.Count();
            Assert.AreEqual(nrOfExpectedStochasts, properties.AlphaValues.Length);
            Assert.AreEqual(nrOfExpectedStochasts, properties.Durations.Length);
            Stochast expectedStochast = generalResult.Stochasts.First();
            Assert.AreEqual(expectedStochast.Alpha, properties.AlphaValues[0].Alpha);
            Assert.AreEqual(expectedStochast.Duration, properties.Durations[0].Duration);

            int nrOfExpectedTopLevelIllustrationPoints = generalResult.TopLevelIllustrationPoints.Count();
            Assert.AreEqual(nrOfExpectedTopLevelIllustrationPoints, properties.IllustrationPoints.Length);

            CollectionAssert.AreEqual(generalResult.TopLevelIllustrationPoints, properties.IllustrationPoints.Select(i => i.Data));
        }

        [Test]
        public void IllustrationPoints_WithoutGeneralResult_ReturnsEmptyTopLevelFaultTreeIllustrationPointPropertiesArray()
        {
            // Setup
            PartialProbabilisticFaultTreePipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(null);
            var properties = new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(output);

            // Call
            TopLevelFaultTreeIllustrationPointProperties[] illustrationPoints = properties.IllustrationPoints;

            // Assert
            CollectionAssert.IsEmpty(illustrationPoints);
        }
    }
}