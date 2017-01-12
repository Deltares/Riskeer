// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverFailureMechanism>>(properties);
        }

        [Test]
        public void Data_SetNewStabilityStoneCoverFailureMechanismContext_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var properties = new StabilityStoneCoverFailureMechanismProperties();

            // Call
            properties.Data = failureMechanism;

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreSame(failureMechanism.GeneralInput.GeneralBlocksWaveConditionsInput, properties.Blocks.Data);
            Assert.AreSame(failureMechanism.GeneralInput.GeneralColumnsWaveConditionsInput, properties.Columns.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties
            {
                Data = new StabilityStoneCoverFailureMechanism()
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het toetsspoor.", nameProperty.Description);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            Assert.IsTrue(codeProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", codeProperty.Category);
            Assert.AreEqual("Label", codeProperty.DisplayName);
            Assert.AreEqual("Het label van het toetsspoor.", codeProperty.Description);

            PropertyDescriptor blocksProperty = dynamicProperties[2];
            Assert.IsTrue(blocksProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableObjectConverter>(blocksProperty.Converter);
            Assert.AreEqual("Modelinstellingen", blocksProperty.Category);
            Assert.AreEqual("Blokken", blocksProperty.DisplayName);
            Assert.AreEqual("De modelinstellingen voor het berekenen van golfcondities voor blokken.", blocksProperty.Description);

            PropertyDescriptor columnsProperty = dynamicProperties[3];
            Assert.IsTrue(columnsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableObjectConverter>(columnsProperty.Converter);
            Assert.AreEqual("Modelinstellingen", columnsProperty.Category);
            Assert.AreEqual("Zuilen", columnsProperty.DisplayName);
            Assert.AreEqual("De modelinstellingen voor het berekenen van golfcondities voor zuilen.", columnsProperty.Description);
        }
    }
}