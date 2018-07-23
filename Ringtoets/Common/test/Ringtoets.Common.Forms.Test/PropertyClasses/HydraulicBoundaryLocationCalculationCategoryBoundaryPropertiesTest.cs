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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationCategoryBoundaryPropertiesTest
    {
        private const int categoryBoundaryNamePropertyIndex = 0;
        private const int resultPropertyIndex = 1;
        private const int targetProbabilityPropertyIndex = 2;
        private const int targetReliabilityPropertyIndex = 3;
        private const int calculatedProbabilityPropertyIndex = 4;
        private const int calculatedReliabilityPropertyIndex = 5;
        private const int convergencePropertyIndex = 6;
        private const int shouldCalculateIllustrationPointsIndex = 7;
        private const int governingWindDirectionIndex = 8;
        private const int alphaValuesIndex = 9;
        private const int durationsIndex = 10;
        private const int illustrationPointsIndex = 11;

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(null, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", paramName);
        }

        [Test]
        public void Constructor_CategoryBoundaryNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("categoryBoundaryName", paramName);
        }

        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, "A");

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationBaseProperties>(properties);
        }

        [Test]
        public void Constructor_WithoutGeneralIllustrationPointsResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, "A");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string resultCategory = "Resultaat";
            const string illustrationPointsCategory = "Illustratiepunten";

            PropertyDescriptor categoryBoundaryNameProperty = dynamicProperties[categoryBoundaryNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryBoundaryNameProperty,
                                                                            generalCategory,
                                                                            "Categoriegrens",
                                                                            "De categoriegrens behorende bij het resultaat.",
                                                                            true);

            PropertyDescriptor resultProperty = dynamicProperties[resultPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(resultProperty,
                                                                            resultCategory,
                                                                            nameof(properties.Result),
                                                                            "",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            resultCategory,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[targetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[calculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            resultCategory,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[calculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            resultCategory,
                                                                            "Convergentie",
                                                                            string.Empty,
                                                                            true);

            PropertyDescriptor calculateIllustrationPointsProperty = dynamicProperties[shouldCalculateIllustrationPointsIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculateIllustrationPointsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Illustratiepunten inlezen",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");
        }

        [Test]
        public void Constructor_WithGeneralIllustrationPointsResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(new TestGeneralResultSubMechanismIllustrationPoint())
            };

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, "A");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(12, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string resultCategory = "Resultaat";
            const string illustrationPointsCategory = "Illustratiepunten";

            PropertyDescriptor categoryBoundaryNameProperty = dynamicProperties[categoryBoundaryNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryBoundaryNameProperty,
                                                                            generalCategory,
                                                                            "Categoriegrens",
                                                                            "De categoriegrens behorende bij het resultaat.",
                                                                            true);

            PropertyDescriptor resultProperty = dynamicProperties[resultPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(resultProperty,
                                                                            resultCategory,
                                                                            nameof(properties.Result),
                                                                            "",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            resultCategory,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[targetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[calculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            resultCategory,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[calculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            resultCategory,
                                                                            "Convergentie",
                                                                            string.Empty,
                                                                            true);

            PropertyDescriptor calculateIllustrationPointsProperty = dynamicProperties[shouldCalculateIllustrationPointsIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculateIllustrationPointsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Illustratiepunten inlezen",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[governingWindDirectionIndex],
                                                                            illustrationPointsCategory,
                                                                            "Maatgevende windrichting",
                                                                            "De windrichting waarvoor de berekende betrouwbaarheidsindex het laagst is.",
                                                                            true);

            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesIndex];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            illustrationPointsCategory,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsIndex];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[illustrationPointsIndex],
                                                                            illustrationPointsCategory,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            const string categoryBoundaryName = "A-Z";
            var calculationConvergence = random.NextEnumValue<CalculationConvergence>();

            var hydraulicBoundaryLocationCalculationOutput = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), calculationConvergence);

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                                                                                                    {
                                                                                                        Output = hydraulicBoundaryLocationCalculationOutput
                                                                                                    },
                                                                                                    categoryBoundaryName);

            // Assert
            Assert.AreEqual(categoryBoundaryName, properties.CategoryBoundaryName);
            Assert.AreEqual(calculationConvergence, properties.Convergence);
            Assert.AreEqual(hydraulicBoundaryLocationCalculationOutput.Result, properties.Result);
        }

        [Test]
        public void ToString_Always_ReturnsCategoryBoundaryName()
        {
            // Setup
            const string categoryBoundaryName = "C";

            // Call
            HydraulicBoundaryLocationCalculationCategoryBoundaryProperties properties = new TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                categoryBoundaryName);

            // Assert
            Assert.AreEqual(categoryBoundaryName, properties.ToString());
        }

        private class TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties : HydraulicBoundaryLocationCalculationCategoryBoundaryProperties
        {
            public TestHydraulicBoundaryLocationCalculationCategoryBoundaryProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                                      string categoryBoundaryName)
                : base(hydraulicBoundaryLocationCalculation, categoryBoundaryName) {}
        }
    }
}