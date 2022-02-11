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
using Riskeer.Common.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses
{
    public class StabilityStoneCoverFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;
        private const int nPropertyIndex = 4;
        private const int applyLengthEffectInSectionPropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };

            // Call
            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverFailureMechanismProperties>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);

            GeneralStabilityStoneCoverWaveConditionsInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N,
                            properties.N,
                            properties.N.GetAccuracy());
            Assert.AreEqual(generalInput.ApplyLengthEffectInSection, properties.ApplyLengthEffectInSection);
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect";

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

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");

            PropertyDescriptor applySectionLengthInSectionProperty = dynamicProperties[applyLengthEffectInSectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(applySectionLengthInSectionProperty,
                                                                            lengthEffectCategory,
                                                                            "Toepassen lengte-effect binnen vak",
                                                                            "Geeft aan of het lengte-effect binnen een vak toegepast wordt.");
        }

        [Test]
        public void Constructor_InAssemblyFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                InAssembly = false
            };

            // Call
            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

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

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(-20.0)]
        public void N_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double newN)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Call
            void Call() => properties.N = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(20.0)]
        public void N_SetValidValue_UpdateDataAndNotifyObservers(double newN)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void ApplyLengthEffectInSection_SetNewValue_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            failureMechanism.Attach(observer);

            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Call
            properties.ApplyLengthEffectInSection = true;

            // Assert
            Assert.IsTrue(failureMechanism.GeneralInput.ApplyLengthEffectInSection);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnInAssembly_ReturnExpectedVisibility(bool inAssembly)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new StabilityStoneCoverFailurePathProperties(failureMechanism);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));

            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.ApplyLengthEffectInSection)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}