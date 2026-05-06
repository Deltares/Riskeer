// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses.HydraulicLoadsState;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.PropertyClasses.HydraulicLoadsState
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int aPropertyIndex = 2;
        private const int bPropertyIndex = 3;
        private const int cPropertyIndex = 4;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, new FailureMechanismPropertyChangeHandler<WaveImpactAsphaltCoverFailureMechanism>());

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverFailureMechanismPropertiesBase>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);

            GeneralWaveImpactAsphaltCoverWaveConditionsInput generalWaveConditionsInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalWaveConditionsInput.A, properties.A);
            Assert.AreEqual(generalWaveConditionsInput.B, properties.B);
            Assert.AreEqual(generalWaveConditionsInput.C, properties.C);
        }

        [Test]
        public void Constructor_ChangeHandlerNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new WaveImpactAsphaltCoverFailureMechanismProperties(new WaveImpactAsphaltCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(new WaveImpactAsphaltCoverFailureMechanism(), new FailureMechanismPropertyChangeHandler<WaveImpactAsphaltCoverFailureMechanism>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCategory = "Modelinstellingen";

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

            PropertyDescriptor aProperty = dynamicProperties[aPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            modelSettingsCategory,
                                                                            "a [-]",
                                                                            "De waarde van de parameter 'a' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor bProperty = dynamicProperties[bPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            modelSettingsCategory,
                                                                            "b [-]",
                                                                            "De waarde van de parameter 'b' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[cPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            modelSettingsCategory,
                                                                            "c [-]",
                                                                            "De waarde van de parameter 'c' in de berekening voor golfcondities.");
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-0.005)]
        [TestCase(2.005)]
        public void C_SetInvalidValue_ThrowArgumentExceptionAndDoesNotUpdateObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<WaveImpactAsphaltCoverFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, handler);

            // Call
            void Call() => properties.C = roundedValue;

            // Assert
            const string expectedMessage = "De waarde van parameter 'c' moet binnen het bereik [0,00, 2,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.5)]
        [TestCase(-0.004)]
        [TestCase(2.004)]
        public void C_SetValidValue_SetsValueRoundedAndUpdatesObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<WaveImpactAsphaltCoverFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, handler);

            // Call
            properties.C = roundedValue;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.C,
                            failureMechanism.GeneralInput.C.GetAccuracy());
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }
    }
}