﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.PropertyClasses;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingCalculationsPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int waterVolumetricWeightPropertyIndex = 4;
        private const int upLiftModelFactorPropertyIndex = 5;
        private const int sellmeijerModelFactorPropertyIndex = 6;
        private const int aPropertyIndex = 7;
        private const int bPropertyIndex = 8;
        private const int sectionLengthPropertyIndex = 9;
        private const int nPropertyIndex = 10;
        private const int criticalHeaveGradientPropertyIndex = 11;
        private const int sandParticlesVolumetricWeightPropertyIndex = 12;
        private const int whitesDragCoefficientPropertyIndex = 13;
        private const int beddingAnglePropertyIndex = 14;
        private const int waterKinematicViscosityPropertyIndex = 15;
        private const int gravityPropertyIndex = 16;
        private const int meanDiameter70PropertyIndex = 17;
        private const int sellmeijerReductionFactorPropertyIndex = 18;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new PipingCalculationsProperties(new PipingFailureMechanism(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            var properties = new PipingCalculationsProperties(failureMechanism, assessmentSection, handler);

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanismProperties>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);

            GeneralPipingInput generalInput = failureMechanism.GeneralInput;

            Assert.AreEqual(generalInput.UpliftModelFactor.Mean, properties.UpliftModelFactor.Mean);
            Assert.AreEqual(generalInput.UpliftModelFactor.StandardDeviation, properties.UpliftModelFactor.StandardDeviation);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(generalInput).GetDesignValue(),
                            properties.UpliftModelFactor.DesignValue);

            Assert.AreEqual(generalInput.SellmeijerModelFactor.Mean, properties.SellmeijerModelFactor.Mean);
            Assert.AreEqual(generalInput.SellmeijerModelFactor.StandardDeviation, properties.SellmeijerModelFactor.StandardDeviation);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(generalInput).GetDesignValue(),
                            properties.SellmeijerModelFactor.DesignValue);

            Assert.AreEqual(generalInput.WaterVolumetricWeight, properties.WaterVolumetricWeight);

            Assert.AreEqual(generalInput.CriticalHeaveGradient.Mean, properties.CriticalHeaveGradient.Mean);
            Assert.AreEqual(generalInput.CriticalHeaveGradient.StandardDeviation, properties.CriticalHeaveGradient.StandardDeviation);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(generalInput).GetDesignValue(),
                            properties.CriticalHeaveGradient.DesignValue);

            Assert.AreEqual(generalInput.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInput.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(generalInput.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(generalInput.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(generalInput.Gravity, properties.Gravity);
            Assert.AreEqual(generalInput.MeanDiameter70, properties.MeanDiameter70);
            Assert.AreEqual(generalInput.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);

            PipingProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.PipingProbabilityAssessmentInput;
            Assert.AreEqual(probabilityAssessmentInput.A, properties.A);
            Assert.AreEqual(probabilityAssessmentInput.B, properties.B);
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length),
                            properties.N,
                            properties.N.GetAccuracy());
            Assert.AreEqual(2, properties.SectionLength.NumberOfDecimalPlaces);
            Assert.AreEqual(assessmentSection.ReferenceLine.Length,
                            properties.SectionLength,
                            properties.SectionLength.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            var properties = new PipingCalculationsProperties(failureMechanism, assessmentSection, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(19, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string heaveCategory = "Heave";
            const string modelFactorCategory = "Modelinstellingen";
            const string lengthEffectCategory = "Lengte-effect parameters";
            const string sellmeijerCategory = "Terugschrijdende erosie (Sellmeijer)";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor groupProperty = dynamicProperties[groupPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(groupProperty,
                                                                            generalCategory,
                                                                            "Groep",
                                                                            "De groep waar het toetsspoor toe behoort.",
                                                                            true);

            PropertyDescriptor contributionProperty = dynamicProperties[contributionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategory,
                                                                            "Faalkansbijdrage [%]",
                                                                            "Procentuele bijdrage van dit toetsspoor aan de totale overstromingskans van het traject.",
                                                                            true);

            PropertyDescriptor volumicWeightOfWaterProperty = dynamicProperties[waterVolumetricWeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumicWeightOfWaterProperty,
                                                                            generalCategory,
                                                                            "Volumiek gewicht van water [kN/m³]",
                                                                            "Volumiek gewicht van water.");

            PropertyDescriptor upliftModelFactorProperty = dynamicProperties[upLiftModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(upliftModelFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftModelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor opbarsten [-]",
                                                                            "Rekenwaarde om de onzekerheid in het model van opbarsten in rekening te brengen.",
                                                                            true);

            PropertyDescriptor sellmeijerModelFactorProperty = dynamicProperties[sellmeijerModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(sellmeijerModelFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerModelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor piping toegepast op het model van Sellmeijer [-]",
                                                                            "Rekenwaarde om de onzekerheid in het model van Sellmeijer in rekening te brengen.",
                                                                            true);

            PropertyDescriptor aProperty = dynamicProperties[aPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            lengthEffectCategory,
                                                                            "a [-]",
                                                                            "De parameter 'a' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.");

            PropertyDescriptor bProperty = dynamicProperties[bPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            lengthEffectCategory,
                                                                            "b [m]",
                                                                            "De parameter 'b' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.",
                                                                            true);

            PropertyDescriptor sectionLengthProperty = dynamicProperties[sectionLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionLengthProperty,
                                                                            lengthEffectCategory,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N* [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling (afgerond).",
                                                                            true);

            PropertyDescriptor criticalHeaveGradientProperty = dynamicProperties[criticalHeaveGradientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalHeaveGradientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalHeaveGradientProperty,
                                                                            heaveCategory,
                                                                            "Kritiek verhang m.b.t. heave [-]",
                                                                            "Kritiek verhang met betrekking tot heave.",
                                                                            true);

            PropertyDescriptor volumetricWeightSandParticlesProperty = dynamicProperties[sandParticlesVolumetricWeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumetricWeightSandParticlesProperty,
                                                                            sellmeijerCategory,
                                                                            "Volumiek gewicht van de zandkorrels onder water [kN/m³]",
                                                                            "Het (ondergedompelde) volumegewicht van zandkorrelmateriaal van een zandlaag.",
                                                                            true);

            PropertyDescriptor whitesDragCoefficientProperty = dynamicProperties[whitesDragCoefficientPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(whitesDragCoefficientProperty,
                                                                            sellmeijerCategory,
                                                                            "Coëfficiënt van White [-]",
                                                                            "Sleepkrachtfactor volgens White.",
                                                                            true);

            PropertyDescriptor beddingAngleProperty = dynamicProperties[beddingAnglePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(beddingAngleProperty,
                                                                            sellmeijerCategory,
                                                                            "Rolweerstandshoek [°]",
                                                                            "Hoek in het krachtenevenwicht die aangeeft hoeveel weerstand de korrels bieden tegen rollen; ook beddingshoek genoemd.",
                                                                            true);

            PropertyDescriptor waterKinematicViscosityProperty = dynamicProperties[waterKinematicViscosityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waterKinematicViscosityProperty,
                                                                            sellmeijerCategory,
                                                                            "Kinematische viscositeit van water bij 10° C [m²/s]",
                                                                            "Kinematische viscositeit van water bij 10° C.",
                                                                            true);

            PropertyDescriptor gravityProperty = dynamicProperties[gravityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravityProperty,
                                                                            sellmeijerCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor meanDiameter70Property = dynamicProperties[meanDiameter70PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(meanDiameter70Property,
                                                                            sellmeijerCategory,
                                                                            "Referentiewaarde voor 70%-fraktiel in Sellmeijer regel [m]",
                                                                            "Gemiddelde d70 van de in kleine schaalproeven toegepaste zandsoorten, waarop de formule van Sellmeijer is gefit.",
                                                                            true);

            PropertyDescriptor reductionFactorSellmeijerProperty = dynamicProperties[sellmeijerReductionFactorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reductionFactorSellmeijerProperty,
                                                                            sellmeijerCategory,
                                                                            "Reductiefactor Sellmeijer [-]",
                                                                            "Reductiefactor Sellmeijer.",
                                                                            true);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-0.005)]
        [TestCase(20.005)]
        public void WaterVolumetricWeight_SetInvalidValue_ThrowArgumentExceptionAndDoesNotUpdateObservers(double value)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new PipingCalculationsProperties(failureMechanism, assessmentSection, handler);

            // Call            
            void Call() => properties.WaterVolumetricWeight = roundedValue;

            // Assert
            const string expectedMessage = "De waarde moet binnen het bereik [0,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(handler.Called);
        }

        [Test]
        [TestCase(5)]
        [TestCase(-0.004)]
        [TestCase(20.004)]
        public void WaterVolumetricWeight_SetValidValue_SetsValueRoundedAndUpdatesObservers(double value)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new PipingCalculationsProperties(failureMechanism, assessmentSection, handler);

            // Call            
            properties.WaterVolumetricWeight = roundedValue;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.WaterVolumetricWeight,
                            failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());
            Assert.IsTrue(handler.Called);
        }
    }
}