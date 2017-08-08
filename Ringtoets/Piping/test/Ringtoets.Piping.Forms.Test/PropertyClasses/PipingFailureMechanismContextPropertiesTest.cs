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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingFailureMechanismContextPropertiesTest
    {
        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingFailureMechanismContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection),
                null);

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

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingFailureMechanismContext>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            Assert.AreEqual(failureMechanism.GeneralInput.UpliftModelFactor, properties.UpliftModelFactor);

            Assert.AreEqual(failureMechanism.GeneralInput.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(failureMechanism.GeneralInput.WaterVolumetricWeight, properties.WaterVolumetricWeight);

            Assert.AreEqual(failureMechanism.GeneralInput.CriticalHeaveGradient, properties.CriticalHeaveGradient);

            Assert.AreEqual(failureMechanism.GeneralInput.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(failureMechanism.GeneralInput.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(failureMechanism.GeneralInput.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(failureMechanism.GeneralInput.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(failureMechanism.GeneralInput.Gravity, properties.Gravity);
            Assert.AreEqual(failureMechanism.GeneralInput.MeanDiameter70, properties.MeanDiameter70);
            Assert.AreEqual(failureMechanism.GeneralInput.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);

            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, properties.A);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.B, properties.B);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = true
            };

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();

            mockRepository.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string heaveCategory = "Heave";
            const string modelFactorCategory = "Modelinstellingen";
            const string semiProbabilisticCategory = "Semi-probabilistische parameters";
            const string sellmeijerCategory = "Terugschrijdende erosie (Sellmeijer)";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            PropertyDescriptor volumicWeightOfWaterProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumicWeightOfWaterProperty,
                                                                            generalCategory,
                                                                            "Volumiek gewicht van water [kN/m³]",
                                                                            "Volumiek gewicht van water.");

            PropertyDescriptor upliftModelFactorProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftModelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor opbarsten [-]",
                                                                            "Rekenwaarde om de onzekerheid in het model van opbarsten in rekening te brengen.",
                                                                            true);

            PropertyDescriptor sellmeijerModelFactorProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerModelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor piping toegepast op het model van Sellmeijer [-]",
                                                                            "Rekenwaarde om de onzekerheid in het model van Sellmeijer in rekening te brengen.",
                                                                            true);

            PropertyDescriptor aProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            semiProbabilisticCategory,
                                                                            "a [-]",
                                                                            "De parameter 'a' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.");

            PropertyDescriptor bProperty = dynamicProperties[7];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            semiProbabilisticCategory,
                                                                            "b [m]",
                                                                            "De parameter 'b' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.",
                                                                            true);

            PropertyDescriptor criticalHeaveGradientProperty = dynamicProperties[8];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalHeaveGradientProperty,
                                                                            heaveCategory,
                                                                            "Kritiek verhang m.b.t. heave [-]",
                                                                            "Kritiek verhang met betrekking tot heave.",
                                                                            true);

            PropertyDescriptor volumetricWeightSandParticlesProperty = dynamicProperties[9];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumetricWeightSandParticlesProperty,
                                                                            sellmeijerCategory,
                                                                            "Volumiek gewicht van de zandkorrels onder water [kN/m³]",
                                                                            "Het (ondergedompelde) volumegewicht van zandkorrelmateriaal van een zandlaag.",
                                                                            true);

            PropertyDescriptor whitesDragCoefficientProperty = dynamicProperties[10];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(whitesDragCoefficientProperty,
                                                                            sellmeijerCategory,
                                                                            "Coëfficiënt van White [-]",
                                                                            "Sleepkrachtfactor volgens White.",
                                                                            true);

            PropertyDescriptor beddingAngleProperty = dynamicProperties[11];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(beddingAngleProperty,
                                                                            sellmeijerCategory,
                                                                            "Rolweerstandshoek [°]",
                                                                            "Hoek in het krachtenevenwicht die aangeeft hoeveel weerstand de korrels bieden tegen rollen; ook beddingshoek genoemd.",
                                                                            true);

            PropertyDescriptor waterKinematicViscosityProperty = dynamicProperties[12];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waterKinematicViscosityProperty,
                                                                            sellmeijerCategory,
                                                                            "Kinematische viscositeit van water bij 10° C [m²/s]",
                                                                            "Kinematische viscositeit van water bij 10° C.",
                                                                            true);

            PropertyDescriptor gravityProperty = dynamicProperties[13];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravityProperty,
                                                                            sellmeijerCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor meanDiameter70Property = dynamicProperties[14];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(meanDiameter70Property,
                                                                            sellmeijerCategory,
                                                                            "Referentiewaarde voor 70%-fraktiel in Sellmeijer regel [m]",
                                                                            "Gemiddelde d70 van de in kleine schaalproeven toegepaste zandsoorten, waarop de formule van Sellmeijer is gefit.",
                                                                            true);

            PropertyDescriptor reductionFactorSellmeijerProperty = dynamicProperties[15];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(reductionFactorSellmeijerProperty,
                                                                            sellmeijerCategory,
                                                                            "Reductiefactor Sellmeijer [-]",
                                                                            "Reductiefactor Sellmeijer.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();

            mockRepository.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(double value)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var observableMock = mocks.StrictMock<IObservable>();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observableMock
                });

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                changeHandler);

            // Call
            TestDelegate call = () => properties.A = value;

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("De waarde moet in het bereik [0,0, 1,0] liggen.", exception.Message);
            Assert.IsTrue(changeHandler.Called);
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
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var observableMock = mocks.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observableMock
                });

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                changeHandler);

            // Call
            properties.A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.PipingProbabilityAssessmentInput.A);
            Assert.IsTrue(changeHandler.Called);
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
            var observableMock = mocks.StrictMock<IObservable>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observableMock
                });

            mocks.ReplayAll();

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                changeHandler);

            // Call            
            TestDelegate test = () => properties.WaterVolumetricWeight = roundedValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, "De waarde moet binnen het bereik [0,00, 20,00] liggen.");
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

            var observableMock = mocks.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<PipingFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observableMock
                });

            mocks.ReplayAll();

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                changeHandler);

            // Call            
            properties.WaterVolumetricWeight = roundedValue;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.WaterVolumetricWeight.Value, failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());
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
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<PipingFailureMechanism>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection),
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.CriticalHeaveGradient)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.WaterVolumetricWeight)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.B)));
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