// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses.RegistrationState;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses.RegistrationState
{
    public class StabilityStoneCoverFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int inAssemblyPropertyIndex = 2;

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
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverFailureMechanismPropertiesBase>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
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
    }
}