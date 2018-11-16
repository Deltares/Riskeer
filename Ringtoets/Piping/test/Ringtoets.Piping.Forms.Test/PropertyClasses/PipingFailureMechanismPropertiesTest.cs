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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int isRelevantPropertyIndex = 4;
        private const int waterVolumetricWeightPropertyIndex = 5;
        private const int upLiftModelFactorPropertyIndex = 6;
        private const int sellMeijerModelFactorPropertyIndex = 7;
        private const int aPropertyIndex = 8;
        private const int bPropertyIndex = 9;
        private const int sectionLengthPropertyIndex = 10;
        private const int nPropertyIndex = 11;
        private const int criticalHeaveGradientPropertyIndex = 12;
        private const int sandParticlesVolumetricWeightPropertyIndex = 13;
        private const int whitesDragCoefficientPropertyIndex = 14;
        private const int beddingAnglePropertyIndex = 15;
        private const int waterKinematicViscosityPropertyIndex = 16;
        private const int gravityPropertyIndex = 17;
        private const int meanDiameter70PropertyIndex = 18;
        private const int sellMeijerReductionFactorPropertyIndex = 19;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingFailureMechanismProperties(null, assessmentSection, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingFailureMechanismProperties(new PipingFailureMechanism(), assessmentSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            // Call
            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingFailureMechanism>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            GeneralPipingInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.UpliftModelFactor, properties.UpliftModelFactor);

            Assert.AreEqual(generalInput.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(generalInput.WaterVolumetricWeight, properties.WaterVolumetricWeight);

            Assert.AreEqual(generalInput.CriticalHeaveGradient, properties.CriticalHeaveGradient);

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
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = true
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(20, dynamicProperties.Count);

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

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            PropertyDescriptor volumicWeightOfWaterProperty = dynamicProperties[waterVolumetricWeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumicWeightOfWaterProperty,
                                                                            generalCategory,
                                                                            "Volumiek gewicht van water [kN/m³]",
                                                                            "Volumiek gewicht van water.");

            PropertyDescriptor upliftModelFactorProperty = dynamicProperties[upLiftModelFactorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftModelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor opbarsten [-]",
                                                                            "Rekenwaarde om de onzekerheid in het model van opbarsten in rekening te brengen.",
                                                                            true);

            PropertyDescriptor sellmeijerModelFactorProperty = dynamicProperties[sellMeijerModelFactorPropertyIndex];
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

            PropertyDescriptor reductionFactorSellmeijerProperty = dynamicProperties[sellMeijerReductionFactorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reductionFactorSellmeijerProperty,
                                                                            sellmeijerCategory,
                                                                            "Reductiefactor Sellmeijer [-]",
                                                                            "Reductiefactor Sellmeijer.",
                                                                            true);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

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

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, changeHandler);

            // Call
            TestDelegate call = () => properties.A = value;

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_SetValidValue_SetsValueAndUpdatesObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, changeHandler);

            // Call
            properties.A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.PipingProbabilityAssessmentInput.A);
            mocks.VerifyAll();
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
            var failureMechanism = new PipingFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, changeHandler);

            // Call            
            TestDelegate test = () => properties.WaterVolumetricWeight = roundedValue;

            // Assert

            const string expectedMessage = "De waarde moet binnen het bereik [0,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            Assert.IsTrue(changeHandler.Called);
            mocks.VerifyAll(); // Does not expect notify observers.
        }

        [Test]
        [TestCase(5)]
        [TestCase(-0.004)]
        [TestCase(20.004)]
        public void WaterVolumetricWeight_SetValidValue_SetsValueRoundedAndUpdatesObservers(double value)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new PipingFailureMechanismProperties(failureMechanism, assessmentSection, changeHandler);

            // Call            
            properties.WaterVolumetricWeight = roundedValue;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.WaterVolumetricWeight,
                            failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());
            Assert.IsTrue(changeHandler.Called);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var properties = new PipingFailureMechanismProperties(pipingFailureMechanism, assessmentSection, changeHandler);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.CriticalHeaveGradient)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.WaterVolumetricWeight)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.B)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SectionLength)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SandParticlesVolumicWeight)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.WhitesDragCoefficient)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.BeddingAngle)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.WaterKinematicViscosity)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Gravity)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.MeanDiameter70)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SellmeijerReductionFactor)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.UpliftModelFactor)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SellmeijerModelFactor)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}