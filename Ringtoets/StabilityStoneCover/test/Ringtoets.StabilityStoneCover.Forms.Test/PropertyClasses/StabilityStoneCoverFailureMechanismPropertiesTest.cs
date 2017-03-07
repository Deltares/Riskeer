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
        [TestCase(true)]
        [TestCase(false)]
        public void Data_SetNewStabilityStoneCoverFailureMechanismContext_ReturnCorrectPropertyValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var properties = new StabilityStoneCoverFailureMechanismProperties();

            // Call
            properties.Data = failureMechanism;

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);
            Assert.AreSame(failureMechanism.GeneralInput.GeneralBlocksWaveConditionsInput, properties.Blocks.Data);
            Assert.AreSame(failureMechanism.GeneralInput.GeneralColumnsWaveConditionsInput, properties.Columns.Data);
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties
            {
                Data = new StabilityStoneCoverFailureMechanism
                {
                    IsRelevant = true
                }
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCateogry = "Modelinstellingen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            PropertyDescriptor blocksProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(blocksProperty,
                                                                            modelSettingsCateogry,
                                                                            "Blokken",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor blokken.",
                                                                            true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(blocksProperty.Converter);

            PropertyDescriptor columnsProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(columnsProperty,
                                                                            modelSettingsCateogry,
                                                                            "Zuilen",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor zuilen.",
                                                                            true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(columnsProperty.Converter);
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties
            {
                Data = new StabilityStoneCoverFailureMechanism
                {
                    IsRelevant = false
                }
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForRelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var properties = new StabilityStoneCoverFailureMechanismProperties
            {
                Data = new StabilityStoneCoverFailureMechanism
                {
                    IsRelevant = true
                }
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Blocks)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Columns)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_ForIrrelevantFailureMechanism_ReturnExpectedVisibility()
        {
            // Setup
            var properties = new StabilityStoneCoverFailureMechanismProperties
            {
                Data = new StabilityStoneCoverFailureMechanism
                {
                    IsRelevant = false
                }
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.Blocks)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.Columns)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}