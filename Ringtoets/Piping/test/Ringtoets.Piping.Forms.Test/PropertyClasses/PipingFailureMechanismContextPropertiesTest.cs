﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
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
            var handler = CreateSimpleHandler(new MockRepository());
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = CreateSimpleHandler(new MockRepository());
            mockRepository.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSectionStub),
                handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingFailureMechanismContext>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);

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
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = CreateSimpleHandler(new MockRepository());

            mockRepository.ReplayAll();

            // Call
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSectionStub),
                handler);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(16, dynamicProperties.Count);

            var generalCategory = "Algemeen";
            var heaveCategory = "Heave";
            var modelFactorCategory = "Modelinstellingen";
            var semiProbabilisticCategory = "Semi-probabilistische parameters";
            var sellmeijerCategory = "Terugschrijdende erosie (Sellmeijer)";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het toetsspoor.", nameProperty.Description);

            PropertyDescriptor labelProperty = dynamicProperties[1];
            Assert.IsNotNull(labelProperty);
            Assert.IsTrue(labelProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, labelProperty.Category);
            Assert.AreEqual("Label", labelProperty.DisplayName);
            Assert.AreEqual("Het label van het toetsspoor.", labelProperty.Description);

            PropertyDescriptor volumicWeightOfWaterProperty = dynamicProperties[2];
            Assert.IsNotNull(volumicWeightOfWaterProperty);
            Assert.IsFalse(volumicWeightOfWaterProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, volumicWeightOfWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightOfWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightOfWaterProperty.Description);

            PropertyDescriptor upliftModelFactorProperty = dynamicProperties[3];
            Assert.IsNotNull(upliftModelFactorProperty);
            Assert.IsTrue(upliftModelFactorProperty.IsReadOnly);
            Assert.AreEqual(modelFactorCategory, upliftModelFactorProperty.Category);
            Assert.AreEqual("Modelfactor opbarsten [-]", upliftModelFactorProperty.DisplayName);
            Assert.AreEqual("Rekenwaarde om de onzekerheid in het model van opbarsten in rekening te brengen.", upliftModelFactorProperty.Description);

            PropertyDescriptor sellmeijerModelFactorProperty = dynamicProperties[4];
            Assert.IsNotNull(sellmeijerModelFactorProperty);
            Assert.IsTrue(sellmeijerModelFactorProperty.IsReadOnly);
            Assert.AreEqual(modelFactorCategory, sellmeijerModelFactorProperty.Category);
            Assert.AreEqual("Modelfactor piping toegepast op het model van Sellmeijer [-]", sellmeijerModelFactorProperty.DisplayName);
            Assert.AreEqual("Rekenwaarde om de onzekerheid in het model van Sellmeijer in rekening te brengen.", sellmeijerModelFactorProperty.Description);

            PropertyDescriptor aProperty = dynamicProperties[5];
            Assert.IsNotNull(aProperty);
            Assert.IsFalse(aProperty.IsReadOnly);
            Assert.AreEqual(semiProbabilisticCategory, aProperty.Category);
            Assert.AreEqual("a [-]", aProperty.DisplayName);
            Assert.AreEqual("De parameter 'a' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.", aProperty.Description);

            PropertyDescriptor bProperty = dynamicProperties[6];
            Assert.IsNotNull(bProperty);
            Assert.IsTrue(bProperty.IsReadOnly);
            Assert.AreEqual(semiProbabilisticCategory, bProperty.Category);
            Assert.AreEqual("b [m]", bProperty.DisplayName);
            Assert.AreEqual("De parameter 'b' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.", bProperty.Description);

            PropertyDescriptor criticalHeaveGradientProperty = dynamicProperties[7];
            Assert.IsNotNull(criticalHeaveGradientProperty);
            Assert.IsTrue(criticalHeaveGradientProperty.IsReadOnly);
            Assert.AreEqual(heaveCategory, criticalHeaveGradientProperty.Category);
            Assert.AreEqual("Kritiek verhang m.b.t. heave [-]", criticalHeaveGradientProperty.DisplayName);
            Assert.AreEqual("Kritiek verhang met betrekking tot heave.", criticalHeaveGradientProperty.Description);

            PropertyDescriptor volumetricWeightSandParticlesProperty = dynamicProperties[8];
            Assert.IsNotNull(volumetricWeightSandParticlesProperty);
            Assert.IsTrue(volumetricWeightSandParticlesProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, volumetricWeightSandParticlesProperty.Category);
            Assert.AreEqual("Volumiek gewicht van de zandkorrels onder water [kN/m³]", volumetricWeightSandParticlesProperty.DisplayName);
            Assert.AreEqual("Het (ondergedompelde) volumegewicht van zandkorrelmateriaal van een zandlaag.", volumetricWeightSandParticlesProperty.Description);

            PropertyDescriptor whitesDragCoefficientProperty = dynamicProperties[9];
            Assert.IsNotNull(whitesDragCoefficientProperty);
            Assert.IsTrue(whitesDragCoefficientProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, whitesDragCoefficientProperty.Category);
            Assert.AreEqual("Coëfficiënt van White [-]", whitesDragCoefficientProperty.DisplayName);
            Assert.AreEqual("Sleepkrachtfactor volgens White.", whitesDragCoefficientProperty.Description);

            PropertyDescriptor beddingAngleProperty = dynamicProperties[10];
            Assert.IsNotNull(beddingAngleProperty);
            Assert.IsTrue(beddingAngleProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, beddingAngleProperty.Category);
            Assert.AreEqual("Rolweerstandshoek [°]", beddingAngleProperty.DisplayName);
            Assert.AreEqual("Hoek in het krachtenevenwicht die aangeeft hoeveel weerstand de korrels bieden tegen rollen; ook beddingshoek genoemd.", beddingAngleProperty.Description);

            PropertyDescriptor waterKinematicViscosityProperty = dynamicProperties[11];
            Assert.IsNotNull(waterKinematicViscosityProperty);
            Assert.IsTrue(waterKinematicViscosityProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, waterKinematicViscosityProperty.Category);
            Assert.AreEqual("Kinematische viscositeit van water bij 10° C [m²/s]", waterKinematicViscosityProperty.DisplayName);
            Assert.AreEqual("Kinematische viscositeit van water bij 10° C.", waterKinematicViscosityProperty.Description);

            PropertyDescriptor gravityProperty = dynamicProperties[12];
            Assert.IsNotNull(gravityProperty);
            Assert.IsTrue(gravityProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, gravityProperty.Category);
            Assert.AreEqual("Valversnelling [m/s²]", gravityProperty.DisplayName);
            Assert.AreEqual("Valversnelling.", gravityProperty.Description);

            PropertyDescriptor meanDiameter70Property = dynamicProperties[13];
            Assert.IsNotNull(meanDiameter70Property);
            Assert.IsTrue(meanDiameter70Property.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, meanDiameter70Property.Category);
            Assert.AreEqual("Referentiewaarde voor 70%-fraktiel in Sellmeijer regel [m]", meanDiameter70Property.DisplayName);
            Assert.AreEqual("Gemiddelde d70 van de in kleine schaalproeven toegepaste zandsoorten, waarop de formule van Sellmeijer is gefit.", meanDiameter70Property.Description);

            PropertyDescriptor reductionFactorSellmeijerProperty = dynamicProperties[14];
            Assert.IsNotNull(reductionFactorSellmeijerProperty);
            Assert.IsTrue(reductionFactorSellmeijerProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, reductionFactorSellmeijerProperty.Category);
            Assert.AreEqual("Reductiefactor Sellmeijer [-]", reductionFactorSellmeijerProperty.DisplayName);
            Assert.AreEqual("Reductiefactor Sellmeijer.", reductionFactorSellmeijerProperty.Description);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_SetInvalidValue_ThrowsArgumentException(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();

            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, mocks.Stub<IAssessmentSection>()),
                CreateSimpleHandler(mocks));
            mocks.ReplayAll();

            failureMechanism.Attach(observerMock);

            // Call
            TestDelegate call = () => properties.A = value;

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("De waarde moet in het bereik [0, 1] liggen.", exception.Message);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, properties.A);
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
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var observableMock = mocks.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler>();
            handler.Expect(h => h.ConfirmPropertyChange()).Return(true);
            handler.Expect(h => h.PropertyChanged(Arg<PipingFailureMechanism>.Is.NotNull)).Return(new [] { observableMock });

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                handler);

            // Call
            properties.A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.PipingProbabilityAssessmentInput.A);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void A_SetValidValueNoConfirmation_NoValueChangeNoUpdates(int newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();

            var changeHandler = mockRepository.StrictMock<IFailureMechanismPropertyChangeHandler>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(false);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();

            mockRepository.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, assessmentSection),
                changeHandler);
            var oldValue = properties.A;

            // Call
            properties.A = newLengthEffect;

            // Assert
            Assert.AreEqual(oldValue, failureMechanism.PipingProbabilityAssessmentInput.A);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(5)]
        [TestCase(-0.004)]
        [TestCase(20.004)]
        public void WaterVolumetricWeight_SetValidValue_SetsValueRoundedAndUpdatesObservers(double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, mocks.Stub<IAssessmentSection>()),
                CreateSimpleHandler(mocks));
            mocks.ReplayAll();

            failureMechanism.Attach(observerMock);

            // Call            
            properties.WaterVolumetricWeight = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanism.GeneralInput.WaterVolumetricWeight.Value, failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-0.005)]
        [TestCase(20.005)]
        public void WaterVolumetricWeight_SetValidValue_ThrowArgumentExceptionAndDoesNotUpdateObservers(double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();

            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties(
                new PipingFailureMechanismContext(failureMechanism, mocks.Stub<IAssessmentSection>()),
                CreateSimpleHandler(mocks));
            mocks.ReplayAll();

            failureMechanism.Attach(observerMock);

            // Call            
            TestDelegate test = () => properties.WaterVolumetricWeight = (RoundedDouble) newValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, "De waarde moet binnen het bereik [0, 20] liggen.");
            mocks.VerifyAll(); // Does not expect notify observers.
        }

        private IFailureMechanismPropertyChangeHandler CreateSimpleHandler(MockRepository mockRepository)
        {
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler>();
            handler.Stub(h => h.ConfirmPropertyChange()).Return(true);
            handler.Stub(h => h.PropertyChanged(Arg<PipingFailureMechanism>.Is.NotNull)).Return(Enumerable.Empty<IObservable>());

            return handler;
        }
    }
}