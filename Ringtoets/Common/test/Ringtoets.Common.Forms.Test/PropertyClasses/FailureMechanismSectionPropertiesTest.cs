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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionPropertiesTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var properties = new FailureMechanismSectionProperties(section);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismSection>>(properties);
            Assert.AreSame(section, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismSectionProperties, ExpandableObjectConverter>();

            Assert.AreEqual(section.Name, properties.Name);
            Assert.AreEqual(section.Length, properties.Length);
            Assert.AreEqual(section.StartPoint, properties.StartPoint);
            Assert.AreEqual(section.EndPoint, properties.EndPoint);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var properties = new FailureMechanismSectionProperties(section);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Vaknaam",
                                                                            "De naam van het vak.",
                                                                            true);
            PropertyDescriptor lengthProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthProperty,
                                                                            "Algemeen",
                                                                            "Lengte [m]",
                                                                            "De totale lengte van het vak in meters.",
                                                                            true);
            PropertyDescriptor startPointProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(startPointProperty,
                                                                            "Algemeen",
                                                                            "Beginpunt",
                                                                            "Beginpunt van het vak (X-coördinaat, Y-coördinaat).",
                                                                            true);
            PropertyDescriptor endPointProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(endPointProperty,
                                                                            "Algemeen",
                                                                            "Eindpunt",
                                                                            "Eindpunt van het vak (X-coördinaat, Y-coördinaat).",
                                                                            true);
        }

        [Test]
        public void ToString_ValidData_ReturnSectionName()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var properties = new FailureMechanismSectionProperties(section);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(section.Name, toString);
        }
    }
}