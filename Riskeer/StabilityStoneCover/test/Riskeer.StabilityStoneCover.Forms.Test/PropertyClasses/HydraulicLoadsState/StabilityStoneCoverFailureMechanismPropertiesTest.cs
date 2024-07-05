﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses.HydraulicLoadsState;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses.HydraulicLoadsState
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int blocksPropertyIndex = 2;
        private const int columnsPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverFailureMechanismPropertiesBase>(properties);
            TestHelper.AssertTypeConverter<StabilityStoneCoverFailureMechanismProperties, ExpandableObjectConverter>(
                nameof(StabilityStoneCoverFailureMechanismProperties.Columns));
            TestHelper.AssertTypeConverter<StabilityStoneCoverFailureMechanismProperties, ExpandableObjectConverter>(
                nameof(StabilityStoneCoverFailureMechanismProperties.Blocks));

            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);

            GeneralStabilityStoneCoverWaveConditionsInput generalInput = failureMechanism.GeneralInput;
            Assert.AreSame(generalInput.GeneralBlocksWaveConditionsInput, properties.Blocks.Data);
            Assert.AreSame(generalInput.GeneralColumnsWaveConditionsInput, properties.Columns.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCateogry = "Modelinstellingen";

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

            PropertyDescriptor blocksProperty = dynamicProperties[blocksPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(blocksProperty,
                                                                            modelSettingsCateogry,
                                                                            "Blokken",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor blokken.",
                                                                            true);

            PropertyDescriptor columnsProperty = dynamicProperties[columnsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(columnsProperty,
                                                                            modelSettingsCateogry,
                                                                            "Zuilen",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor zuilen.",
                                                                            true);
        }
    }
}