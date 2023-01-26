// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.ClosingStructures.Forms.PropertyClasses.RegistrationState;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Forms.Test.PropertyClasses.RegistrationState
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int inAssemblyPropertyIndex = 2;
        private const int cPropertyIndex = 3;
        private const int n2APropertyIndex = 4;
        private const int nPropertyIndex = 5;
        private const int applyLengthEffectInSectionPropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };

            // Call
            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresFailureMechanismPropertiesBase>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);

            GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;

            Assert.AreEqual(generalInput.C, properties.C);
            Assert.AreEqual(generalInput.N2A, properties.N2A);
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N, properties.N, properties.N.GetAccuracy());
            Assert.AreEqual(generalInput.ApplyLengthEffectInSection, properties.ApplyLengthEffectInSection);
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new ClosingStructuresFailureMechanismProperties(new ClosingStructuresFailureMechanism());

            // Assert
            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[cPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            lengthEffectCategory,
                                                                            "C [-]",
                                                                            "De parameter 'C' die gebruikt wordt om het lengte-effect te berekenen.",
                                                                            true);

            PropertyDescriptor n2AProperty = dynamicProperties[n2APropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(n2AProperty,
                                                                            lengthEffectCategory,
                                                                            "2NA [-]",
                                                                            "De parameter '2NA' die gebruikt wordt om het lengte-effect te berekenen.");

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N* [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling (afgerond).",
                                                                            true);

            PropertyDescriptor applySectionLengthInSectionProperty = dynamicProperties[applyLengthEffectInSectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(applySectionLengthInSectionProperty,
                                                                            lengthEffectCategory,
                                                                            "Toepassen lengte-effect binnen vak",
                                                                            "Geeft aan of het lengte-effect binnen een vak toegepast wordt.",
                                                                            true);
        }

        [Test]
        public void Constructor_InAssemblyFalse_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                InAssembly = false
            };
            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism);

            // Assert
            const string generalCategory = "Algemeen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                                            true);
        }

        [Test]
        [Culture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-20)]
        [TestCase(41)]
        public void N2A_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(int newN2A)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism);

            // Call
            void Call() => properties.N2A = newN2A;

            // Assert
            const string expectedMessage = "De waarde voor 'N2A' moet in het bereik [0, 40] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N2A_SetValidValue_UpdateDataAndNotifyObservers(int newN2A)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism);

            // Call
            properties.N2A = newN2A;

            // Assert
            Assert.AreEqual(newN2A, failureMechanism.GeneralInput.N2A, failureMechanism.GeneralInput.N2A);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnInAssembly_ReturnExpectedVisibility(bool inAssembly)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new ClosingStructuresFailureMechanismProperties(failureMechanism);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));

            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.C)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N2A)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.ApplyLengthEffectInSection)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}