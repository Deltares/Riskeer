﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismPropertiesBaseTest
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 0;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new TestStabilityStoneCoverFailureMechanismProperties(null, new StabilityStoneCoverFailureMechanismPropertiesBase.ConstructionProperties());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestStabilityStoneCoverFailureMechanismProperties(new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new TestStabilityStoneCoverFailureMechanismProperties(
                failureMechanism, new StabilityStoneCoverFailureMechanismPropertiesBase.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverFailureMechanism>>(properties);

            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var properties = new TestStabilityStoneCoverFailureMechanismProperties(
                failureMechanism, new StabilityStoneCoverFailureMechanismPropertiesBase.ConstructionProperties
                {
                    NamePropertyIndex = namePropertyIndex,
                    CodePropertyIndex = codePropertyIndex
                });

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

        private class TestStabilityStoneCoverFailureMechanismProperties : StabilityStoneCoverFailureMechanismPropertiesBase
        {
            public TestStabilityStoneCoverFailureMechanismProperties(StabilityStoneCoverFailureMechanism data,
                                                                     ConstructionProperties constructionProperties)
                : base(data, constructionProperties) {}
        }
    }
}