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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PropertyClasses;
using Riskeer.DuneErosion.Forms.PropertyClasses.HydraulicLoadsState;

namespace Riskeer.DuneErosion.Forms.Test.PropertyClasses.HydraulicLoadsState
{
    [TestFixture]
    public class DuneErosionFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<DuneErosionFailureMechanismPropertiesBase>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

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
        }
    }
}