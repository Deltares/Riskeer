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
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationBasePropertiesTest
    {
        private const int resultPropertyIndex = 0;
        private const int targetProbabilityPropertyIndex = 1;
        private const int targetReliabilityPropertyIndex = 2;
        private const int calculatedProbabilityPropertyIndex = 3;
        private const int calculatedReliabilityPropertyIndex = 4;
        private const int convergencePropertyIndex = 5;
        private const int shouldCalculateIllustrationPointsIndex = 6;
        private const int governingWindDirectionIndex = 7;
        private const int alphaValuesIndex = 8;
        private const int durationsIndex = 9;
        private const int illustrationPointsIndex = 10;

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationCalculationBaseProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", paramName);
        }

        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationBaseProperties(hydraulicBoundaryLocationCalculation);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryLocationCalculation>>(properties);
            Assert.IsInstanceOf<ExpandableObjectConverter>(TypeDescriptor.GetConverter(properties, true));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, NoValueRoundedDoubleConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.Result));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, NoProbabilityValueDoubleConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.TargetProbability));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, NoValueRoundedDoubleConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.TargetReliability));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, NoProbabilityValueDoubleConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.CalculatedProbability));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, NoValueRoundedDoubleConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.CalculatedReliability));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, EnumTypeConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.Convergence));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, KeyValueExpandableArrayConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.AlphaValues));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, KeyValueExpandableArrayConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.Durations));
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationProperties, ExpandableArrayConverter>(
                nameof(HydraulicBoundaryLocationCalculationProperties.IllustrationPoints));
        }

        [Test]
        public void Constructor_WithoutGeneralIllustrationPointsResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationBaseProperties(hydraulicBoundaryLocationCalculation);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string resultCategory = "Resultaat";
            const string illustrationPointsCategory = "Illustratiepunten";

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
            var properties = new TestHydraulicBoundaryLocationCalculationBaseProperties(hydraulicBoundaryLocationCalculation);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);

            const string resultCategory = "Resultaat";
            const string illustrationPointsCategory = "Illustratiepunten";

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
        [TestCase(true)]
        [TestCase(false)]
        public void GetProperties_ValidData_ReturnsExpectedValues(bool withIllustrationPoints)
        {
            // Setup
            var random = new Random();

            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            double result = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            var illustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(new WindDirection("WEST", 4), "sluit", new TestSubMechanismIllustrationPoint())
            };
            var stochasts = new[]
            {
                new Stochast("a", 2, 3)
            };
            const string governingWindDirection = "EAST";
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult =
                withIllustrationPoints
                    ? new GeneralResult<TopLevelSubMechanismIllustrationPoint>(new WindDirection(governingWindDirection, 2),
                                                                               stochasts,
                                                                               illustrationPoints)
                    : null;

            var hydraulicBoundaryLocationCalculationOutput = new HydraulicBoundaryLocationCalculationOutput(result,
                                                                                                            targetProbability,
                                                                                                            targetReliability,
                                                                                                            calculatedProbability,
                                                                                                            calculatedReliability,
                                                                                                            convergence,
                                                                                                            generalResult);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = hydraulicBoundaryLocationCalculationOutput
            };

            // Call
            var properties = new TestHydraulicBoundaryLocationCalculationBaseProperties(calculation);

            // Assert
            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, properties.Convergence);
            Assert.AreEqual(calculation.InputParameters.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);

            if (withIllustrationPoints)
            {
                GeneralResult<TopLevelSubMechanismIllustrationPoint> expectedGeneralResult = hydraulicBoundaryLocationCalculationOutput.GeneralResult;
                CollectionAssert.AreEqual(expectedGeneralResult.Stochasts, properties.AlphaValues);
                CollectionAssert.AreEqual(expectedGeneralResult.Stochasts, properties.Durations);
                CollectionAssert.AreEqual(expectedGeneralResult.TopLevelIllustrationPoints, properties.IllustrationPoints.Select(ip => ip.Data));
                Assert.AreEqual(expectedGeneralResult.GoverningWindDirection.Name, properties.GoverningWindDirection);
            }
        }

        [Test]
        public void ShouldIllustrationPointsBeCalculated_SetNewValue_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = false
                }
            };

            hydraulicBoundaryLocationCalculation.Attach(observer);

            var properties = new TestHydraulicBoundaryLocationCalculationBaseProperties(hydraulicBoundaryLocationCalculation);

            // Call
            properties.ShouldIllustrationPointsBeCalculated = true;

            // Assert
            Assert.IsTrue(hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            mocks.VerifyAll();
        }

        private class TestHydraulicBoundaryLocationCalculationBaseProperties : HydraulicBoundaryLocationCalculationBaseProperties
        {
            public TestHydraulicBoundaryLocationCalculationBaseProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
                : base(hydraulicBoundaryLocationCalculation) {}
        }
    }
}