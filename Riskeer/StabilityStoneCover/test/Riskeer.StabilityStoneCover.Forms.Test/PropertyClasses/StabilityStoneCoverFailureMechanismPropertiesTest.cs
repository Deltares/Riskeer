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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 2;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 0;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityStoneCoverFailureMechanismProperties(null, new StabilityStoneCoverFailureMechanismProperties.ConstructionProperties());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StabilityStoneCoverFailureMechanismProperties(new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            bool isRelevant = random.NextBoolean();
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = isRelevant,
                GeneralInput =
                {
                    N = random.NextRoundedDouble(1.0, 20.0)
                }
            };

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism, new StabilityStoneCoverFailureMechanismProperties.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverFailureMechanism>>(properties);

            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism, new StabilityStoneCoverFailureMechanismProperties.ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex,
                GroupPropertyIndex = groupPropertyIndex
            });

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

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
        }
    }
}