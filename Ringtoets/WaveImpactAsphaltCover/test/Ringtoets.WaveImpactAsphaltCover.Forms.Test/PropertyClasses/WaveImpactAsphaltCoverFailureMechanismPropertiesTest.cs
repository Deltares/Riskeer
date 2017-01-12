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
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveImpactAsphaltCoverFailureMechanism>>(properties);
        }

        [Test]
        public void Data_SetNewWaveImpactAsphaltCoverFailureMechanismContext_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties();

            // Call
            properties.Data = failureMechanism;

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.GeneralInput.A, properties.A);
            Assert.AreEqual(failureMechanism.GeneralInput.B, properties.B);
            Assert.AreEqual(failureMechanism.GeneralInput.C, properties.C);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverFailureMechanismProperties
            {
                Data = new WaveImpactAsphaltCoverFailureMechanism()
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

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

            PropertyDescriptor aProperty = dynamicProperties[2];
            Assert.IsNotNull(aProperty);
            Assert.IsTrue(aProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", aProperty.Category);
            Assert.AreEqual("a", aProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'a' in de berekening voor golf condities.", aProperty.Description);

            PropertyDescriptor bProperty = dynamicProperties[3];
            Assert.IsNotNull(bProperty);
            Assert.IsTrue(bProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", bProperty.Category);
            Assert.AreEqual("b", bProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'b' in de berekening voor golf condities.", bProperty.Description);

            PropertyDescriptor cProperty = dynamicProperties[4];
            Assert.IsNotNull(cProperty);
            Assert.IsTrue(cProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", cProperty.Category);
            Assert.AreEqual("c", cProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'c' in de berekening voor golf condities.", cProperty.Description);
        }
    }
}