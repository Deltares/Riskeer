﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses.CalculationsState;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses.CalculationsState
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int waterVolumetricWeightPropertyIndex = 2;
        private const int modelFactorPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsFailureMechanismPropertiesBase>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);

            GeneralMacroStabilityInwardsInput generalInput = failureMechanism.GeneralInput;

            Assert.AreEqual(generalInput.WaterVolumetricWeight, properties.WaterVolumetricWeight);
            Assert.AreEqual(generalInput.ModelFactor, properties.ModelFactor);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var properties = new MacroStabilityInwardsFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelFactorCategory = "Modelinstellingen";

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

            PropertyDescriptor volumicWeightOfWaterProperty = dynamicProperties[waterVolumetricWeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(volumicWeightOfWaterProperty,
                                                                            generalCategory,
                                                                            "Volumiek gewicht van water [kN/m³]",
                                                                            "Volumiek gewicht van water.",
                                                                            true);

            PropertyDescriptor modelFactorProperty = dynamicProperties[modelFactorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorProperty,
                                                                            modelFactorCategory,
                                                                            "Modelfactor [-]",
                                                                            "Modelfactor die wordt gebruikt bij de berekening van de benaderde faalkans op basis van de berekende stabiliteitsfactor.",
                                                                            true);
        }
    }
}