﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class OvertoppingOutputPropertiesTest
    {
        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;
        private const int waveHeightIndex = 5;
        private const int isDominantIndex = 6;
        private const int windDirectionPropertyIndex = 7;
        private const int alphaValuesPropertyIndex = 8;
        private const int durationsPropertyIndex = 9;
        private const int illustrationPointsPropertyIndex = 10;

        private const string resultCategoryName = "\tSterkte berekening";
        private const string illustrationPointsCategoryName = "Illustratiepunten";

        [Test]
        public void Constructor_OvertoppingOutput_ExpectedValues()
        {
            // Setup
            var overtoppingOutput = new TestOvertoppingOutput(0.5);

            // Call
            var properties = new OvertoppingOutputProperties(overtoppingOutput);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<OvertoppingOutput>>(properties);
            Assert.AreSame(overtoppingOutput, properties.Data);
        }

        [Test]
        public void Constructor_OvertoppingOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new OvertoppingOutputProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("overtoppingOutput", exception.ParamName);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random();
            double waveHeight = random.NextDouble();
            bool isOvertoppingDominant = random.NextBoolean();
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

            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            var overtoppingOutput = new OvertoppingOutput(waveHeight,
                                                          isOvertoppingDominant,
                                                          probabilityAssessmentOutput,
                                                          generalResult);

            // Call
            var properties = new OvertoppingOutputProperties(overtoppingOutput);

            // Assert
            Assert.AreEqual(2, properties.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(3, properties.FactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(factorOfSafety, properties.FactorOfSafety, properties.FactorOfSafety.GetAccuracy());

            Assert.AreEqual(ProbabilityFormattingHelper.Format(requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), properties.Probability);

            Assert.AreEqual(isOvertoppingDominant, properties.IsOvertoppingDominant);

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
            var overtoppingOutput = new TestOvertoppingOutput(0.5);
            var properties = new OvertoppingOutputProperties(overtoppingOutput);

            // Call
            TopLevelFaultTreeIllustrationPointProperties[] illustrationPoints = properties.IllustrationPoints;

            // Assert
            Assert.IsEmpty(illustrationPoints);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(10)]
        public void PropertyAttributes_NoGeneralResult_ReturnExpectedValues(double waveHeight)
        {
            // Setup
            var probabilityAssessmentOutput = new TestProbabilityAssessmentOutput();
            var overtoppingOutput = new OvertoppingOutput(waveHeight,
                                                          true,
                                                          probabilityAssessmentOutput,
                                                          null);

            // Call
            var properties = new OvertoppingOutputProperties(overtoppingOutput);

            // Assert
            int propertiesCount = overtoppingOutput.HasWaveHeight ? 7 : 6;

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(propertiesCount, dynamicProperties.Count);

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

            if (overtoppingOutput.HasWaveHeight)
            {
                PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                                resultCategoryName,
                                                                                "Indicatieve golfhoogte (Hs) [m]",
                                                                                "De golfhoogte van de overslag deelberekening.",
                                                                                true);
            }

            int waveHeightNotPresentOffset = overtoppingOutput.HasWaveHeight
                                                 ? 0
                                                 : 1;

            PropertyDescriptor isDominantProperty = dynamicProperties[isDominantIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isDominantProperty,
                                                                            resultCategoryName,
                                                                            "Overslag dominant [-]",
                                                                            "Is het resultaat van de overslag deelberekening dominant over de overloop deelberekening.",
                                                                            true);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(10)]
        public void PropertyAttributes_HasGeneralResult_ReturnExpectedValues(double waveHeight)
        {
            // Setup
            var probabilityAssessmentOutput = new TestProbabilityAssessmentOutput();
            var overtoppingOutput = new OvertoppingOutput(waveHeight,
                                                          true,
                                                          probabilityAssessmentOutput,
                                                          new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            var properties = new OvertoppingOutputProperties(overtoppingOutput);

            // Assert
            int propertiesCount = overtoppingOutput.HasWaveHeight ? 11 : 10;

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(propertiesCount, dynamicProperties.Count);

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

            if (overtoppingOutput.HasWaveHeight)
            {
                PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                                resultCategoryName,
                                                                                "Indicatieve golfhoogte (Hs) [m]",
                                                                                "De golfhoogte van de overslag deelberekening.",
                                                                                true);
            }

            int waveHeightNotPresentOffset = overtoppingOutput.HasWaveHeight
                                                 ? 0
                                                 : 1;

            PropertyDescriptor isDominantProperty = dynamicProperties[isDominantIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isDominantProperty,
                                                                            resultCategoryName,
                                                                            "Overslag dominant [-]",
                                                                            "Is het resultaat van de overslag deelberekening dominant over de overloop deelberekening.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Maatgevende windrichting",
                                                                            "De windrichting waarvoor de berekende betrouwbaarheidsindex het laagst is.",
                                                                            true);

            PropertyDescriptor alphaValuesProperty = dynamicProperties[alphaValuesPropertyIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            PropertyDescriptor durationsProperty = dynamicProperties[durationsPropertyIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Tijdsduren [uur]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            PropertyDescriptor illustrationPointProperty = dynamicProperties[illustrationPointsPropertyIndex - waveHeightNotPresentOffset];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(illustrationPointProperty,
                                                                            illustrationPointsCategoryName,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }
    }
}