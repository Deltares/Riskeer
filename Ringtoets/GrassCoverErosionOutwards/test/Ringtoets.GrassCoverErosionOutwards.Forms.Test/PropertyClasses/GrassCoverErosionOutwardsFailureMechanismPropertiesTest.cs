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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int isRelevantPropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;
        private const int aPropertyIndex = 4;
        private const int bPropertyIndex = 5;
        private const int cPropertyIndex = 6;

        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithoutChangeHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsFailureMechanismProperties(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithFailureMechanism_ExpectedValues(bool isRelevant)
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie buitentalud", properties.Name);
            Assert.AreEqual("GEBU", properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.A, properties.A);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.B, properties.B);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.C, properties.C);
            Assert.AreEqual(2, properties.LengthEffect);
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                },
                changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCategory = "Modelinstellingen";

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
                                                                            "Lengte-effect parameters",
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");

            PropertyDescriptor aProperty = dynamicProperties[aPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            modelSettingsCategory,
                                                                            "a",
                                                                            "De waarde van de parameter 'a' in de berekening voor golf condities.",
                                                                            true);

            PropertyDescriptor bProperty = dynamicProperties[bPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            modelSettingsCategory,
                                                                            "b",
                                                                            "De waarde van de parameter 'b' in de berekening voor golf condities.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[cPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            modelSettingsCategory,
                                                                            "c",
                                                                            "De waarde van de parameter 'c' in de berekening voor golf condities.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                },
                changeHandler);

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

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-20)]
        public void LengthEffect_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(int newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newLengthEffect,
                new[]
                {
                    observableMock
                });

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            TestDelegate test = () => properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValue_UpdateDataAndNotifyObservers(int newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newLengthEffect,
                new[]
                {
                    observableMock
                });

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForRelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                },
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.B)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.C)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForIrrelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                },
                changeHandler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.B)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.C)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}