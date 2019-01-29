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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
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

        private const string illustrationPointsCategoryName = "Illustratiepunten";
        private const string resultCategoryName = "\tResultaat";

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var structuresOutput = new StructuresOutput(0, null);

            // Call
            var properties = new SimpleStructuresOutputProperties(structuresOutput);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StructuresOutput>>(properties);
            Assert.AreSame(structuresOutput, properties.Data);
        }

        [Test]
        public void Constructor_StructuresOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SimpleStructuresOutputProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structuresOutput", exception.ParamName);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var structuresOutput = new TestStructuresOutput(generalResult);

            // Call
            var properties = new SimpleStructuresOutputProperties(structuresOutput);

            // Assert
            Assert.AreEqual(generalResult.GoverningWindDirection.Name, properties.WindDirection);

            TestHelper.AssertTypeConverter<StructuresOutputProperties, KeyValueExpandableArrayConverter>(
                nameof(StructuresOutputProperties.AlphaValues));
            TestHelper.AssertTypeConverter<StructuresOutputProperties, KeyValueExpandableArrayConverter>(
                nameof(StructuresOutputProperties.Durations));

            int nrOfExpectedStochasts = generalResult.Stochasts.Count();
            Assert.AreEqual(nrOfExpectedStochasts, properties.AlphaValues.Length);
            Assert.AreEqual(nrOfExpectedStochasts, properties.Durations.Length);
            Stochast expectedStochast = generalResult.Stochasts.First();
            Assert.AreEqual(expectedStochast.Alpha, properties.AlphaValues[0].Alpha);
            Assert.AreEqual(expectedStochast.Duration, properties.Durations[0].Duration);

            TestHelper.AssertTypeConverter<StructuresOutputProperties, ExpandableArrayConverter>(
                nameof(StructuresOutputProperties.IllustrationPoints));

            int nrOfExpectedTopLevelIllustrationPoints = generalResult.TopLevelIllustrationPoints.Count();
            Assert.AreEqual(nrOfExpectedTopLevelIllustrationPoints, properties.IllustrationPoints.Length);

            CollectionAssert.AreEqual(generalResult.TopLevelIllustrationPoints, properties.IllustrationPoints.Select(i => i.Data));
        }

        [Test]
        public void IllustrationPoints_WithoutGeneralResult_ReturnsEmptyTopLevelFaultTreeIllustrationPointPropertiesArray()
        {
            // Setup
            var structuresOutput = new StructuresOutput(0, null);
            var properties = new SimpleStructuresOutputProperties(structuresOutput);

            // Call
            TopLevelFaultTreeIllustrationPointProperties[] illustrationPoints = properties.IllustrationPoints;

            // Assert
            Assert.IsEmpty(illustrationPoints);
        }

        [Test]
        public void Constructor_HasGeneralResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var structuresOutput = new StructuresOutput(0, generalResult);

            // Call
            var properties = new SimpleStructuresOutputProperties(structuresOutput);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

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

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            resultCategoryName,
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
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointsPropertyIndex];
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
            var structuresOutput = new StructuresOutput(0, null);

            // Call
            var properties = new SimpleStructuresOutputProperties(structuresOutput);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            resultCategoryName,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            resultCategoryName,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

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

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            resultCategoryName,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
        }

        private class SimpleStructuresOutputProperties : StructuresOutputProperties
        {
            public SimpleStructuresOutputProperties(StructuresOutput structuresOutput)
                : base(structuresOutput) {}

            protected override ProbabilityAssessmentOutput CreateDerivedOutput()
            {
                return null;
            }
        }
    }
}