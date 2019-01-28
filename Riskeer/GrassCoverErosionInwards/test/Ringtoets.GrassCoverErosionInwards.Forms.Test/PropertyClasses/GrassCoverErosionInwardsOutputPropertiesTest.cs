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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputPropertiesTest
    {
        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;
        private const int waveHeightIndex = 5;
        private const int isDominantIndex = 6;

        private const int firstHydraulicLoadsOutputIndex = 7;
        private const int secondHydraulicLoadsOutputIndex = 13;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var grassCoverErosionInwardsOutput = new TestGrassCoverErosionInwardsOutput();

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties(grassCoverErosionInwardsOutput, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsOutput>>(properties);
            Assert.AreSame(grassCoverErosionInwardsOutput, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GrassCoverErosionInwardsOutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsOutputProperties(null, new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("grassCoverErosionInwardsOutput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsOutputProperties(new TestGrassCoverErosionInwardsOutput(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsOutputProperties(new TestGrassCoverErosionInwardsOutput(),
                                                                                   new GrassCoverErosionInwardsFailureMechanism(),
                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var random = new Random(39);
            double waveHeight = random.NextDouble();
            bool isOvertoppingDominant = Convert.ToBoolean(random.Next(0, 2));
            double reliability = random.NextDouble();
            double dikeHeight = random.NextDouble();
            double dikeHeightTargetProbability = random.NextDouble();
            double dikeHeightTargetReliability = random.NextDouble();
            double dikeHeightCalculatedProbability = random.NextDouble();
            double dikeHeightCalculatedReliability = random.NextDouble();
            var dikeHeightConvergence = random.NextEnumValue<CalculationConvergence>();
            double overtoppingRate = random.NextDouble();
            double overtoppingRateTargetProbability = random.NextDouble();
            double overtoppingRateTargetReliability = random.NextDouble();
            double overtoppingRateCalculatedProbability = random.NextDouble();
            double overtoppingRateCalculatedReliability = random.NextDouble();
            var overtoppingRateConvergence = random.NextEnumValue<CalculationConvergence>();

            var resultOutput = new OvertoppingOutput(waveHeight,
                                                     isOvertoppingDominant,
                                                     reliability,
                                                     null);

            var dikeHeightOutput = new DikeHeightOutput(dikeHeight,
                                                        dikeHeightTargetProbability,
                                                        dikeHeightTargetReliability,
                                                        dikeHeightCalculatedProbability,
                                                        dikeHeightCalculatedReliability,
                                                        dikeHeightConvergence,
                                                        null);
            var overtoppingRateOutput = new OvertoppingRateOutput(overtoppingRate,
                                                                  overtoppingRateTargetProbability,
                                                                  overtoppingRateTargetReliability,
                                                                  overtoppingRateCalculatedProbability,
                                                                  overtoppingRateCalculatedReliability,
                                                                  overtoppingRateConvergence,
                                                                  null);
            var output = new GrassCoverErosionInwardsOutput(resultOutput, dikeHeightOutput, overtoppingRateOutput);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(2, properties.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(double.PositiveInfinity, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(3, properties.FactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(0, properties.FactorOfSafety, properties.FactorOfSafety.GetAccuracy());

            Assert.AreEqual(ProbabilityFormattingHelper.Format(0), properties.RequiredProbability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.5), properties.Probability);

            Assert.AreEqual(isOvertoppingDominant, properties.IsOvertoppingDominant);

            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(dikeHeight, properties.DikeHeight, properties.DikeHeight.GetAccuracy());
            Assert.AreEqual(dikeHeightTargetProbability, properties.DikeHeightTargetProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightTargetProbability));
            Assert.AreEqual(dikeHeightTargetReliability, properties.DikeHeightTargetReliability, properties.DikeHeightTargetReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightTargetReliability));
            Assert.AreEqual(dikeHeightCalculatedProbability, properties.DikeHeightCalculatedProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightCalculatedProbability));
            Assert.AreEqual(dikeHeightCalculatedReliability, properties.DikeHeightCalculatedReliability, properties.DikeHeightCalculatedReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightCalculatedReliability));

            string dikeHeightConvergenceValue = new EnumDisplayWrapper<CalculationConvergence>(dikeHeightConvergence).DisplayName;
            Assert.AreEqual(dikeHeightConvergenceValue, properties.DikeHeightConvergence);

            Assert.AreEqual(2, properties.OvertoppingRate.NumberOfDecimalPlaces);
            Assert.AreEqual(overtoppingRate * 1000, properties.OvertoppingRate, properties.OvertoppingRate.GetAccuracy());
            Assert.AreEqual(overtoppingRateTargetProbability, properties.OvertoppingRateTargetProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateTargetProbability));
            Assert.AreEqual(overtoppingRateTargetReliability, properties.OvertoppingRateTargetReliability, properties.OvertoppingRateTargetReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateTargetReliability));
            Assert.AreEqual(overtoppingRateCalculatedProbability, properties.OvertoppingRateCalculatedProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateCalculatedProbability));
            Assert.AreEqual(overtoppingRateCalculatedReliability, properties.OvertoppingRateCalculatedReliability, properties.OvertoppingRateCalculatedReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateCalculatedReliability));

            string overtoppingRateConvergenceValue = new EnumDisplayWrapper<CalculationConvergence>(overtoppingRateConvergence).DisplayName;
            Assert.AreEqual(overtoppingRateConvergenceValue, properties.OvertoppingRateConvergence);
            mocks.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_WithDikeHeightAndOvertoppingRateCalculated_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var resultOutput = new OvertoppingOutput(10,
                                                     true,
                                                     0,
                                                     null);
            var dikeHeightOutput = new TestDikeHeightOutput(double.NaN);
            var overtoppingRateOutput = new TestOvertoppingRateOutput(double.NaN);

            var output = new GrassCoverErosionInwardsOutput(resultOutput,
                                                            dikeHeightOutput,
                                                            overtoppingRateOutput);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(19, dynamicProperties.Count);

            AssertResultOutputProperties(dynamicProperties);
            AssertDikeHeightOutputProperties(dynamicProperties, firstHydraulicLoadsOutputIndex);
            AssertOvertoppingRateOutputProperties(dynamicProperties, secondHydraulicLoadsOutputIndex);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void PropertyAttributes_WithDikeHeightOrOvertoppingRateCalculated_ReturnExpectedValues(bool dikeHeightCalculated,
                                                                                                      bool overtoppingRateCalculated)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            DikeHeightOutput dikeHeightOutput = null;
            OvertoppingRateOutput overtoppingRateOutput = null;

            var resultOutput = new OvertoppingOutput(2,
                                                     true,
                                                     0,
                                                     null);

            if (dikeHeightCalculated)
            {
                dikeHeightOutput = new TestDikeHeightOutput(double.NaN);
            }

            if (overtoppingRateCalculated)
            {
                overtoppingRateOutput = new TestOvertoppingRateOutput(double.NaN);
            }

            var output = new GrassCoverErosionInwardsOutput(resultOutput,
                                                            dikeHeightOutput,
                                                            overtoppingRateOutput);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(13, dynamicProperties.Count);

            AssertResultOutputProperties(dynamicProperties);

            if (dikeHeightCalculated)
            {
                AssertDikeHeightOutputProperties(dynamicProperties, firstHydraulicLoadsOutputIndex);
            }

            if (overtoppingRateCalculated)
            {
                AssertOvertoppingRateOutputProperties(dynamicProperties, firstHydraulicLoadsOutputIndex);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(10)]
        public void PropertyAttributes_WithoutDikeHeightAndOvertoppingRateCalculated_ReturnExpectedValues(double waveHeight)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var resultOutput = new OvertoppingOutput(waveHeight,
                                                     true,
                                                     0,
                                                     null);

            var output = new GrassCoverErosionInwardsOutput(resultOutput, null, null);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            int propertiesCount = double.IsNaN(waveHeight) ? 6 : 7;

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(propertiesCount, dynamicProperties.Count);

            AssertResultOutputProperties(dynamicProperties, !double.IsNaN(waveHeight));
            mocks.VerifyAll();
        }

        private static void AssertResultOutputProperties(PropertyDescriptorCollection dynamicProperties, bool waveHeightCalculated = true)
        {
            const string resultCategory = "\t\tSterkte berekening";
            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            resultCategory,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.",
                                                                            true);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityProperty,
                                                                            resultCategory,
                                                                            "Faalkans [1/jaar]",
                                                                            "De kans dat het toetsspoor optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorOfSafetyProperty,
                                                                            resultCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);

            if (waveHeightCalculated)
            {
                PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                                resultCategory,
                                                                                "Indicatieve golfhoogte (Hs) [m]",
                                                                                "De golfhoogte van de overslag deelberekening.",
                                                                                true);
            }

            int realDominantIndex = waveHeightCalculated
                                        ? isDominantIndex
                                        : isDominantIndex - 1;

            PropertyDescriptor isDominantProperty = dynamicProperties[realDominantIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isDominantProperty,
                                                                            resultCategory,
                                                                            "Overslag dominant [-]",
                                                                            "Is het resultaat van de overslag deelberekening dominant over de overloop deelberekening.",
                                                                            true);
        }

        private static void AssertDikeHeightOutputProperties(PropertyDescriptorCollection dynamicProperties, int dikeHeightIndex)
        {
            const string category = "\tHBN";
            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightProperty,
                                                                            category,
                                                                            "HBN [m+NAP]",
                                                                            "Het berekende Hydraulisch Belasting Niveau (HBN).",
                                                                            true);

            PropertyDescriptor dikeHeightTargetProbability = dynamicProperties[dikeHeightIndex + 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightTargetProbability,
                                                                            category,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor dikeHeightTargetReliability = dynamicProperties[dikeHeightIndex + 2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightTargetReliability,
                                                                            category,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor dikeHeightCalculatedProbability = dynamicProperties[dikeHeightIndex + 3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightCalculatedProbability,
                                                                            category,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor dikeHeightCalculatedReliability = dynamicProperties[dikeHeightIndex + 4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightCalculatedReliability,
                                                                            category,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor dikeHeightCalculationConvergence = dynamicProperties[dikeHeightIndex + 5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightCalculationConvergence,
                                                                            category,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de HBN berekening?",
                                                                            true);
        }

        private static void AssertOvertoppingRateOutputProperties(PropertyDescriptorCollection dynamicProperties, int overtoppingRateIndex)
        {
            const string overtoppingRateCategory = "Overslagdebiet";
            PropertyDescriptor overtoppingRateProperty = dynamicProperties[overtoppingRateIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateProperty,
                                                                            overtoppingRateCategory,
                                                                            "Overslagdebiet [l/m/s]",
                                                                            "Het berekende overslagdebiet.",
                                                                            true);

            PropertyDescriptor overtoppingRateTargetProbability = dynamicProperties[overtoppingRateIndex + 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateTargetProbability,
                                                                            overtoppingRateCategory,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor overtoppingRateTargetReliability = dynamicProperties[overtoppingRateIndex + 2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateTargetReliability,
                                                                            overtoppingRateCategory,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor overtoppingRateCalculatedProbability = dynamicProperties[overtoppingRateIndex + 3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateCalculatedProbability,
                                                                            overtoppingRateCategory,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor overtoppingRateCalculatedReliability = dynamicProperties[overtoppingRateIndex + 4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateCalculatedReliability,
                                                                            overtoppingRateCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor overtoppingRateCalculationConvergence = dynamicProperties[overtoppingRateIndex + 5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateCalculationConvergence,
                                                                            overtoppingRateCategory,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de overslagdebiet berekening?",
                                                                            true);
        }
    }
}