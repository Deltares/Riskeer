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
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Forms.Test.PropertyClasses
{
    public class ClosingStructuresFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int cPropertyIndex = 4;
        private const int n2APropertyIndex = 5;
        private const int nPropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var properties = new ClosingStructuresFailurePathProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresFailureMechanismProperties>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);

            GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;

            Assert.AreEqual(generalInput.C, properties.C);
            Assert.AreEqual(generalInput.N2A, properties.N2A);
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N, properties.N, properties.N.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new ClosingStructuresFailurePathProperties(new ClosingStructuresFailureMechanism());

            // Assert
            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";

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

            PropertyDescriptor groupProperty = dynamicProperties[groupPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(groupProperty,
                                                                            generalCategory,
                                                                            "Groep",
                                                                            "De groep waar het toetsspoor toe behoort.",
                                                                            true);

            PropertyDescriptor contributionProperty = dynamicProperties[contributionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategory,
                                                                            "Faalkansbijdrage [%]",
                                                                            "Procentuele bijdrage van dit toetsspoor aan de totale overstromingskans van het traject.",
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
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-20)]
        [TestCase(41)]
        public void N2A_SetInvalidValue_ThrowsArgumentOutOfRangeException(int newN)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var properties = new ClosingStructuresFailurePathProperties(failureMechanism);

            // Call
            void Call() => properties.N2A = newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N2A' moet in het bereik [0, 40] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N2A_SetValidValue_UpdateDataAndNotifyObservers(int newN)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new ClosingStructuresFailurePathProperties(failureMechanism);

            // Call
            properties.N2A = newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N2A, failureMechanism.GeneralInput.N2A);

            mocks.VerifyAll();
        }
    }
}