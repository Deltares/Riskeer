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
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneErosionFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int isRelevantPropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DuneErosionFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismProperties(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectParameterCategory = "Lengte-effect parameters";

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

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectParameterCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

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

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Data_SetNewFailureMechanismContext_ReturnCorrectPropertyValues(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var originalFailureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = !isRelevant,
                GeneralInput =
                {
                    N = (RoundedDouble) 1.1
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var properties = new DuneErosionFailureMechanismProperties(originalFailureMechanism, changeHandler);

            // Call
            properties.Data = failureMechanism;

            // Assert
            Assert.AreEqual("Duinwaterkering - Duinafslag", properties.Name);
            Assert.AreEqual("DA", properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);
            Assert.AreEqual(failureMechanism.GeneralInput.N, properties.LengthEffect);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0, TestName = "LenghtEffect_InvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(0)")]
        [TestCase(-1, TestName = "LenghtEffect_InvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(-1)")]
        [TestCase(-20, TestName = "LenghtEffect_InvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(-20)")]
        public void LengthEffect_SetInvalidValue_ThrowsThrowsArgumentOutOfRangeExceptionNoNotifcations(double newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<DuneErosionFailureMechanism, RoundedDouble>(
                failureMechanism,
                (RoundedDouble) newLengthEffect,
                new[]
                {
                    observableMock
                });

            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            TestDelegate test = () => properties.LengthEffect = (RoundedDouble) newLengthEffect;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValue_UpdateDataAndNotifyObservers(double newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<DuneErosionFailureMechanism, RoundedDouble>(
                failureMechanism,
                (RoundedDouble) newLengthEffect,
                new[]
                {
                    observableMock
                });

            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.LengthEffect = (RoundedDouble) newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N.Value);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new DuneErosionFailureMechanismProperties(
                new DuneErosionFailureMechanism
                {
                    IsRelevant = isRelevant
                },
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}