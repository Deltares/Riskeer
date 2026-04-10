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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.ChangeHandlers;
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
                                                                            "De waarde van de parameter 'c' in de berekening voor golfcondities.",
                                                                            false);
        }

        [Test]
        public void Test_InvalidValueParamC()
        {
            // Call
            Assert.Catch<ArgumentOutOfRangeException>(SetToOutOfBounds);
        }

        private void SetToOutOfBounds()
        {
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, new FailureMechanismPropertyChangeHandler<WaveImpactAsphaltCoverFailureMechanism>());
            // C must be in range [0, ... , 2]
            properties.C = new RoundedDouble(2, 3.14);
        }

    }
}