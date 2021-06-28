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

using System.ComponentModel;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Revetment.Data;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverCalculationsPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int aPropertyIndex = 3;
        private const int bPropertyIndex = 4;
        private const int cPropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            var properties = new WaveImpactAsphaltCoverCalculationsProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverFailureMechanismProperties>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);

            GeneralWaveConditionsInput generalWaveConditionsInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalWaveConditionsInput.A, properties.A);
            Assert.AreEqual(generalWaveConditionsInput.B, properties.B);
            Assert.AreEqual(generalWaveConditionsInput.C, properties.C);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverCalculationsProperties(new WaveImpactAsphaltCoverFailureMechanism());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCategory = "Modelinstellingen";

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

            PropertyDescriptor aProperty = dynamicProperties[aPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            modelSettingsCategory,
                                                                            "a",
                                                                            "De waarde van de parameter 'a' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor bProperty = dynamicProperties[bPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            modelSettingsCategory,
                                                                            "b",
                                                                            "De waarde van de parameter 'b' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[cPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            modelSettingsCategory,
                                                                            "c",
                                                                            "De waarde van de parameter 'c' in de berekening voor golfcondities.",
                                                                            true);
        }
    }
}