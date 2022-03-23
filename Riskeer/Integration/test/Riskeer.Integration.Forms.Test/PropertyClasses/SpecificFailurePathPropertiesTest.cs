// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SpecificFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int inAssemblyPropertyIndex = 2;
        private const int nPropertyIndex = 3;
        private const int applyLengthEffectInSectionPropertyIndex = 4;

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SpecificFailurePathProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failurePath = new SpecificFailureMechanism
            {
                InAssembly = random.NextBoolean(),
                GeneralInput =
                {
                    ApplyLengthEffectInSection = random.NextBoolean()
                }
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<SpecificFailureMechanism>>(properties);
            Assert.AreEqual(failurePath.Name, properties.Name);
            Assert.AreEqual(failurePath.Code, properties.Code);
            Assert.AreEqual(failurePath.InAssembly, properties.InAssembly);

            GeneralInput input = failurePath.GeneralInput;
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(input.N, properties.N, properties.N.GetAccuracy());
            Assert.AreEqual(input.ApplyLengthEffectInSection, properties.ApplyLengthEffectInSection);
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failurePath = new SpecificFailureMechanism
            {
                InAssembly = true
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.");

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het faalmechanisme.");

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
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
            var failurePath = new SpecificFailureMechanism
            {
                InAssembly = false
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.");

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het faalmechanisme.");

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                                            true);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            const int numberOfChangedProperties = 3;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var random = new Random(21);
            var failurePath = new SpecificFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            failurePath.Attach(projectObserver);

            // Call
            const string newName = "Some new cool pretty name";
            const string newCode = "Nieuw Faalmechanisme";
            RoundedDouble newN = random.NextRoundedDouble(1.0, 20.0);
            properties.Name = newName;
            properties.Code = newCode;
            properties.N = newN;

            // Assert
            Assert.AreEqual(newName, failurePath.Name);
            Assert.AreEqual(newCode, failurePath.Code);

            GeneralInput input = failurePath.GeneralInput;
            Assert.AreEqual(newN, input.N, input.N.GetAccuracy());

            mocks.VerifyAll();
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

            var failurePath = new SpecificFailureMechanism();
            failurePath.Attach(observer);

            var properties = new SpecificFailurePathProperties(failurePath);

            // Call
            void Call() => properties.N = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);

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

            var failureMechanism = new SpecificFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new SpecificFailurePathProperties(failureMechanism);

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
            var failurePath = new SpecificFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new SpecificFailurePathProperties(failurePath);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));

            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.ApplyLengthEffectInSection)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}