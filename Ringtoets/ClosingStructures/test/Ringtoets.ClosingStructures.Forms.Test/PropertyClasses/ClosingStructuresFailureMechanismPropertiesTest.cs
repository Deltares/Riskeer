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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ClosingStructuresFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ClosingStructuresFailureMechanismProperties(
                new ClosingStructuresFailureMechanism(),
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
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ClosingStructuresFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);

            Assert.AreEqual("Kunstwerken - Betrouwbaarheid sluiting kunstwerk",
                            properties.Name);
            Assert.AreEqual("BSKW",
                            properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.GravitationalAcceleration.Value,
                            properties.GravitationalAcceleration.Value);

            Assert.AreEqual(generalInput.C.Value, properties.C.Value);
            Assert.AreEqual(generalInput.N2A, properties.N2A);
            Assert.AreEqual(generalInput.N.Value, properties.LengthEffect.Value);

            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.Mean, properties.ModelFactorOvertoppingFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.StandardDeviation, properties.ModelFactorOvertoppingFlow.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.Mean, properties.ModelFactorSubCriticalFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation, properties.ModelFactorSubCriticalFlow.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorInflowVolume.Value, properties.ModelFactorInflowVolume.Value);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            var properties = new ClosingStructuresFailureMechanismProperties(
                new ClosingStructuresFailureMechanism
                {
                    IsRelevant = true
                },
                changeHandler);

            // Assert
            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
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

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravitationalAccelerationProperty,
                                                                            generalCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            lengthEffectCategory,
                                                                            "C [-]",
                                                                            "De parameter 'C' die gebruikt wordt om het lengte-effect te berekenen.",
                                                                            true);

            PropertyDescriptor n2AProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(n2AProperty,
                                                                            lengthEffectCategory,
                                                                            "2NA [-]",
                                                                            "De parameter '2NA' die gebruikt wordt om het lengte-effect te berekenen.");

            PropertyDescriptor lengthEffectProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.",
                                                                            true);

            PropertyDescriptor modelFactorOvertoppingFlowProperty = dynamicProperties[7];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorOvertoppingFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorOvertoppingFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor overslagdebiet [-]",
                                                                            "Modelfactor voor het overslagdebiet.",
                                                                            true);

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[8];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorStorageVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor kombergend vermogen [-]",
                                                                            "Modelfactor kombergend vermogen.",
                                                                            true);

            PropertyDescriptor modelFactorSubCriticalFlowProperty = dynamicProperties[9];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSubCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSubCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor onvolkomen stroming [-]",
                                                                            "Modelfactor voor onvolkomen stroming.",
                                                                            true);

            PropertyDescriptor modelFactorInflowVolumeProperty = dynamicProperties[10];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorInflowVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor instromend volume [-]",
                                                                            "Modelfactor instromend volume.",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            var properties = new ClosingStructuresFailureMechanismProperties(
                new ClosingStructuresFailureMechanism
                {
                    IsRelevant = false
                },
                changeHandler);

            // Assert
            const string generalCategory = "Algemeen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
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
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(-10)]
        [TestCase(-1)]
        [TestCase(41)]
        [TestCase(141)]
        public void N2A_InvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifcations(int value)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observable= mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<ClosingStructuresFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observable
                });

            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            TestDelegate test = () => properties.N2A = value;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(21)]
        [TestCase(40)]
        public void N2A_SetValidValue_UpdateDataAndNotifyObservers(int value)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observable= mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());

            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<ClosingStructuresFailureMechanism, double>(
                failureMechanism,
                value,
                new[]
                {
                    observable
                });

            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.N2A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.N2A);
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
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new ClosingStructuresFailureMechanismProperties(
                new ClosingStructuresFailureMechanism
                {
                    IsRelevant = isRelevant
                },
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.C)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N2A)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.LengthEffect)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.GravitationalAcceleration)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorOvertoppingFlow)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorStorageVolume)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorSubCriticalFlow)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.ModelFactorInflowVolume)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}