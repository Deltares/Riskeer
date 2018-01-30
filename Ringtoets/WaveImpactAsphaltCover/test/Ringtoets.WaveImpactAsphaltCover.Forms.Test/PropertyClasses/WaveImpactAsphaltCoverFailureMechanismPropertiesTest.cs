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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverFailureMechanismProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithWaveImpactAsphaltCoverFailureMechanism_ReturnsCorrectPropertyValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);
            Assert.AreEqual(failureMechanism.GeneralInput.A, properties.A);
            Assert.AreEqual(failureMechanism.GeneralInput.B, properties.B);
            Assert.AreEqual(failureMechanism.GeneralInput.C, properties.C);
            Assert.AreEqual(failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL, properties.DeltaL);

            Assert.AreEqual(2, properties.SectionLength.NumberOfDecimalPlaces);
            Assert.AreEqual(failureMechanism.GeneralWaveImpactAsphaltCoverInput.SectionLength,
                            properties.SectionLength,
                            properties.SectionLength.GetAccuracy());

            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(failureMechanism.GeneralWaveImpactAsphaltCoverInput.N,
                            properties.N,
                            properties.N.GetAccuracy());
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(
                new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = true
                });

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";
            const string modelSettingsCategory = "Modelinstellingen";

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

            PropertyDescriptor sectionLength = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionLength,
                                                                            lengthEffectCategory,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor deltaLProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(deltaLProperty,
                                                                            lengthEffectCategory,
                                                                            "ΔL [m]",
                                                                            "Lengte van onafhankelijke dijkstrekkingen voor dit toetsspoor.");

            PropertyDescriptor nProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N* [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect " +
                                                                            "mee te nemen in de beoordeling (afgerond).",
                                                                            true);

            PropertyDescriptor aProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            modelSettingsCategory,
                                                                            "a",
                                                                            "De waarde van de parameter 'a' in de berekening voor golf condities.",
                                                                            true);

            PropertyDescriptor bProperty = dynamicProperties[7];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            modelSettingsCategory,
                                                                            "b",
                                                                            "De waarde van de parameter 'b' in de berekening voor golf condities.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[8];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            modelSettingsCategory,
                                                                            "c",
                                                                            "De waarde van de parameter 'c' in de berekening voor golf condities.",
                                                                            true);
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(
                new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = false
                });

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
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.005)]
        [TestCase(-1000)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        public void DeltaL_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism);

            // Call
            TestDelegate call = () => properties.DeltaL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'ΔL' moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0.005)]
        [TestCase(1)]
        [TestCase(1000)]
        [TestCase(double.PositiveInfinity)]
        public void DeltaL_SetValidValue_SetsValueAndUpdatesObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism);

            // Call
            properties.DeltaL = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(value,
                            failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL,
                            failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnsExpectedVisibility(bool isRelevant)
        {
            // Setup
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(
                new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = isRelevant
                });

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.SectionLength)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.DeltaL)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.A)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.B)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.C)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}