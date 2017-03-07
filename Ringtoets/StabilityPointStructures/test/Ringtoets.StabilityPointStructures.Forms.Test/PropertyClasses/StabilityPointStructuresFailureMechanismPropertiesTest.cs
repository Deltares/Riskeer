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
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int isRelevantPropertyIndex = 2;
        private const int gravitationalAccelerationPropertyIndex = 3;
        private const int lengthEffectPropertyIndex = 4;
        private const int modelFactorStorageVolumePropertyIndex = 5;
        private const int modelFactorSubCriticalFlowPropertyIndex = 6;
        private const int modelFactorCollisionLoadPropertyIndex = 7;
        private const int modelFactorLoadEffectPropertyIndex = 8;

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism> changeHandler =
                mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new StabilityPointStructuresFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityPointStructuresFailureMechanismProperties(
                new StabilityPointStructuresFailureMechanism(),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidValues_ExpectedValues(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityPointStructuresFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);

            Assert.AreEqual("Kunstwerken - Sterkte en stabiliteit puntconstructies", properties.Name);
            Assert.AreEqual("STKWp", properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.N, properties.LengthEffect);
            Assert.AreEqual(generalInput.GravitationalAcceleration, properties.GravitationalAcceleration);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.Mean, properties.ModelFactorSubCriticalFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation, properties.ModelFactorSubCriticalFlow.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.Mean, properties.ModelFactorCollisionLoad.Mean);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.CoefficientOfVariation, properties.ModelFactorCollisionLoad.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.Mean, properties.ModelFactorLoadEffect.Mean);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.StandardDeviation, properties.ModelFactorLoadEffect.StandardDeviation);
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);

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

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[gravitationalAccelerationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravitationalAccelerationProperty,
                                                                            generalCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[modelFactorStorageVolumePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorStorageVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor kombergend vermogen [-]",
                                                                            "Modelfactor kombergend vermogen.",
                                                                            true);

            PropertyDescriptor modelFactorSubCriticalFlowProperty = dynamicProperties[modelFactorSubCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSubCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSubCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor onvolkomen stroming [-]",
                                                                            "Modelfactor voor onvolkomen stroming.",
                                                                            true);

            PropertyDescriptor modelFactorCollisionLoadProperty = dynamicProperties[modelFactorCollisionLoadPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorCollisionLoadProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorCollisionLoadProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor aanvaarbelasting [-]",
                                                                            "Modelfactor voor aanvaarbelasting.",
                                                                            true);

            PropertyDescriptor modelFactorLoadEffectProperty = dynamicProperties[modelFactorLoadEffectPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorLoadEffectProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorLoadEffectProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor belastingeffect [-]",
                                                                            "Modelfactor belastingeffect.",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            var generalCategory = "Algemeen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

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
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-20)]
        public void LengthEffect_InvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(int value)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<StabilityPointStructuresFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observableMock
                });

            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            TestDelegate test = () => properties.LengthEffect = value;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValue_UpdateDataAndNotifyObservers(int value)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableMock = mockRepository.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<StabilityPointStructuresFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observableMock
                });

            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.LengthEffect = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.N);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForRelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new StabilityPointStructuresFailureMechanismProperties(
                new StabilityPointStructuresFailureMechanism
                {
                    IsRelevant = true
                },
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.GravitationalAcceleration)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorStorageVolume)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorSubCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorCollisionLoad)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorLoadEffect)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForIrrelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new StabilityPointStructuresFailureMechanismProperties(
                new StabilityPointStructuresFailureMechanism
                {
                    IsRelevant = false
                },
                changeHandler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.GravitationalAcceleration)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorStorageVolume)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorSubCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorCollisionLoad)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorLoadEffect)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}