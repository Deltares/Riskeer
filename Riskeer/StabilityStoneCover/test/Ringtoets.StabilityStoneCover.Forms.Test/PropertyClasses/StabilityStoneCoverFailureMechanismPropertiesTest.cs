// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int isRelevantPropertyIndex = 4;
        private const int blocksPropertyIndex = 5;
        private const int columnsPropertyIndex = 6;
        private const int nPropertyIndex = 7;

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityStoneCoverFailureMechanismProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
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
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverFailureMechanism>>(properties);
            TestHelper.AssertTypeConverter<StabilityStoneCoverFailureMechanismProperties, ExpandableObjectConverter>(
                nameof(StabilityStoneCoverFailureMechanismProperties.Columns));
            TestHelper.AssertTypeConverter<StabilityStoneCoverFailureMechanismProperties, ExpandableObjectConverter>(
                nameof(StabilityStoneCoverFailureMechanismProperties.Blocks));

            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            GeneralStabilityStoneCoverWaveConditionsInput generalInput = failureMechanism.GeneralInput;
            Assert.AreSame(generalInput.GeneralBlocksWaveConditionsInput, properties.Blocks.Data);
            Assert.AreSame(generalInput.GeneralColumnsWaveConditionsInput, properties.Columns.Data);
            Assert.AreEqual(generalInput.N, properties.N);
        }

        [Test]
        public void N_NewValue_GeneralInputUpdated()
        {
            // Setup
            var random = new Random(39);
            RoundedDouble newN = random.NextRoundedDouble(1.0, 20.0);
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Call
            properties.N = newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCateogry = "Modelinstellingen";
            const string lengthEffectCategory = "Lengte-effect parameters";

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

            PropertyDescriptor contributionProperty = dynamicProperties[contributionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategory,
                                                                            "Faalkansbijdrage [%]",
                                                                            "Procentuele bijdrage van dit toetsspoor aan de totale overstromingskans van het traject.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
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

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

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

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(bool isRelevant)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var properties = new StabilityStoneCoverFailureMechanismProperties(failureMechanism);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Blocks)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Columns)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}