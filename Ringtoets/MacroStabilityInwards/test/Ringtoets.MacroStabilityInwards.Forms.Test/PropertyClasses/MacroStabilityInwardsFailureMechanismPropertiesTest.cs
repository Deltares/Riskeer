﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsFailureMechanismProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsFailureMechanism>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput;
            Assert.AreEqual(probabilityAssessmentInput.A, properties.A);
            Assert.AreEqual(probabilityAssessmentInput.B, properties.B);
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(probabilityAssessmentInput.GetSectionSpecificN(
                                probabilityAssessmentInput.SectionLength),
                            properties.N,
                            properties.N.GetAccuracy());
            Assert.AreEqual(2, properties.SectionLength.NumberOfDecimalPlaces);
            Assert.AreEqual(probabilityAssessmentInput.SectionLength,
                            properties.SectionLength,
                            properties.SectionLength.GetAccuracy());
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";

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

            PropertyDescriptor aProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            lengthEffectCategory,
                                                                            "a [-]",
                                                                            "De parameter 'a' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.");

            PropertyDescriptor bProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            lengthEffectCategory,
                                                                            "b [m]",
                                                                            "De parameter 'b' die gebruikt wordt voor het lengte-effect in berekening van de maximaal toelaatbare faalkans.",
                                                                            true);

            PropertyDescriptor sectionLengthProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionLengthProperty,
                                                                            lengthEffectCategory,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor nProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N* [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling (afgerond).",
                                                                            true);
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

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
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Call
            TestDelegate call = () => properties.A = value;

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
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
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Call
            properties.A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(bool isRelevant)
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.B)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SectionLength)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}