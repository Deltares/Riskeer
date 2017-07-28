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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StructuresOutputPropertiesTest
    {
        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;
        private const int windDirectionPropertyIndex = 5;
        private const int alphaValuesPropertyIndex = 6;
        private const int durationsPropertyIndex = 7;
        private const int illustrationPointsPropertyIndex = 8;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var structuresOutput = new StructuresOutput(new TestProbabilityAssessmentOutput());

            // Call
            var properties = new StructuresOutputProperties(structuresOutput);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StructuresOutput>>(properties);
            Assert.AreSame(structuresOutput, properties.Data);
        }

        [Test]
        public void Constructor_StructuresOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StructuresOutputProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structuresOutput", exception.ParamName);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability,
                                                                              requiredReliability,
                                                                              probability,
                                                                              reliability,
                                                                              factorOfSafety);
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(new WindDirection("SSE", 5.0), "closing situation", new IllustrationPointNode(new TestIllustrationPoint()));

            var expectedFaultTreeIllustrationPointBaseProperty = new[]
            {
                new FaultTreeIllustrationPointBaseProperty(topLevelFaultTreeIllustrationPoint)
            };
            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(new WindDirection("SSE", 5.0), new Stochast[0], new List<TopLevelFaultTreeIllustrationPoint> { topLevelFaultTreeIllustrationPoint }); var structuresOutput = new StructuresOutput(probabilityAssessmentOutput);
            structuresOutput.SetGeneralResult(generalResult);

            // Call
            var properties = new StructuresOutputProperties(structuresOutput);

            // Assert
            Assert.AreEqual(ProbabilityFormattingHelper.Format(requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), properties.Probability);
            Assert.AreEqual(reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(factorOfSafety, properties.FactorOfSafety, properties.FactorOfSafety.GetAccuracy());
            Assert.AreEqual("SSE", properties.WindDirection);
            Assert.IsEmpty(properties.AlphaValues);
            Assert.IsEmpty(properties.Durations);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].WindDirection, properties.IllustrationPoints[0].WindDirection);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].Reliability, properties.IllustrationPoints[0].Reliability);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].CalculatedProbability, properties.IllustrationPoints[0].CalculatedProbability);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].ClosingSituation, properties.IllustrationPoints[0].ClosingSituation);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].IllustrationPoints, properties.IllustrationPoints[0].IllustrationPoints);
        }

        [Test]
        public void Constructor_HasGeneralResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            const string illustrationPointsCategoryName = "Illustratiepunten";
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var structuresOutput = new StructuresOutput(probabilityAssessmentOutput);
            structuresOutput.SetGeneralResult(generalResult);

            // Call
            var properties = new StructuresOutputProperties(structuresOutput);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            "Resultaat",
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            "Resultaat",
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Maatgevende windrichting",
                                                                            "De windrichting waarvoor de berekende betrouwbaarheidsindex het laagst is.",
                                                                            true);

            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
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

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "",
                                                                            true);
        }

        [Test]
        public void Constructor_NoGeneralResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var structuresOutput = new StructuresOutput(probabilityAssessmentOutput);

            // Call
            var properties = new StructuresOutputProperties(structuresOutput);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            "Resultaat",
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            "Resultaat",
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
        }
    }
}