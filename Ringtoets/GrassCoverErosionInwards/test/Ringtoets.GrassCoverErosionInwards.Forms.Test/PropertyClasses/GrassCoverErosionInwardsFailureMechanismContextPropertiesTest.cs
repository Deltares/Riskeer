// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismContextPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int lengthEffectPropertyIndex = 2;
        private const int frunupModelFactorPropertyIndex = 3;
        private const int fbFactorPropertyIndex = 4;
        private const int fnFactorPropertyIndex = 5;
        private const int fshallowModelFactorPropertyIndex = 6;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            IFailureMechanismPropertyChangeHandler<IFailureMechanism> handler = CreateSimpleHandler();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerIsNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            IFailureMechanismPropertyChangeHandler<IFailureMechanism> handler = CreateSimpleHandler();

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                handler);

            // Assert
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, properties.Name);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, properties.Code);
            Assert.AreEqual(2, properties.LengthEffect);
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            Assert.AreEqual(generalInput.FbFactor.Mean, properties.FbFactor.Mean);
            Assert.AreEqual(generalInput.FbFactor.StandardDeviation, properties.FbFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FnFactor.Mean, properties.FnFactor.Mean);
            Assert.AreEqual(generalInput.FnFactor.StandardDeviation, properties.FnFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FrunupModelFactor.Mean, properties.FrunupModelFactor.Mean);
            Assert.AreEqual(generalInput.FrunupModelFactor.StandardDeviation, properties.FrunupModelFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FshallowModelFactor.Mean, properties.FshallowModelFactor.Mean);
            Assert.AreEqual(generalInput.FshallowModelFactor.StandardDeviation, properties.FshallowModelFactor.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            IFailureMechanismPropertyChangeHandler<IFailureMechanism> handler = CreateSimpleHandler();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                handler);

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectParameterCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectParameterCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");

            PropertyDescriptor frunupModelFactorProperty = dynamicProperties[frunupModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(frunupModelFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(frunupModelFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Frunup [-]",
                                                                            "De parameter 'Frunup' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fbModelProperty = dynamicProperties[fbFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fbModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fbModelProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fb [-]",
                                                                            "De parameter 'Fb' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fnFactorProperty = dynamicProperties[fnFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fnFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fnFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fn [-]",
                                                                            "De parameter 'Fn' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fshallowProperty = dynamicProperties[fshallowModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fshallowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fshallowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fondiep [-]",
                                                                            "De parameter 'Fondiep' die gebruikt wordt in de berekening.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-20)]
        public void LengthEffect_SetInvalidValueWithConfirmation_ThrowsArgumentOutOfRangeException(int newLengthEffect)
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            var changeHandler = mockRepository.StrictMock<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(true);

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                changeHandler);

            // Call
            TestDelegate test = () => properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValueWithConfirmation_UpdateDataAndNotifyObservers(int newLengthEffect)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var observableMock = mockRepository.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            var changeHandler = mockRepository.StrictMock<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(true);
            changeHandler.Expect(h => h.PropertyChanged(failureMechanism)).Return(new[] { observableMock });

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();

            mockRepository.ReplayAll();

            failureMechanism.Attach(observerMock);

            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                changeHandler);

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(-10)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValueWithoutConfirmation_NoValueChangeNoUpdates(int newLengthEffect)
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();

            var changeHandler = mockRepository.StrictMock<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(false);

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                changeHandler);
            int oldValue = properties.LengthEffect;

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(oldValue, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        private IFailureMechanismPropertyChangeHandler<IFailureMechanism> CreateSimpleHandler()
        {
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            handler.Stub(h => h.ConfirmPropertyChange()).Return(true);
            handler.Stub(h => h.PropertyChanged(Arg<GrassCoverErosionInwardsFailureMechanism>.Is.NotNull)).Return(Enumerable.Empty<IObservable>());

            return handler;
        }
    }
}