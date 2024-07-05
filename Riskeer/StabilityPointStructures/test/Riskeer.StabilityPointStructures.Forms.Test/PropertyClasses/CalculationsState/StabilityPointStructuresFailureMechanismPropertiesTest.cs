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

using System;
using System.ComponentModel;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses.CalculationsState;

namespace Riskeer.StabilityPointStructures.Forms.Test.PropertyClasses.CalculationsState
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int gravitationalAccelerationPropertyIndex = 2;
        private const int modelFactorStorageVolumePropertyIndex = 3;
        private const int modelFactorCollisionLoadPropertyIndex = 4;
        private const int modelFactorLoadEffectPropertyIndex = 5;
        private const int modelFactorLongThresholdPropertyIndex = 6;

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityPointStructuresFailureMechanismProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructuresFailureMechanismPropertiesBase>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);

            GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.GravitationalAcceleration, properties.GravitationalAcceleration);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.Mean, properties.ModelFactorCollisionLoad.Mean);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.CoefficientOfVariation, properties.ModelFactorCollisionLoad.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.Mean, properties.ModelFactorLoadEffect.Mean);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.StandardDeviation, properties.ModelFactorLoadEffect.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorLongThreshold.Mean, properties.ModelFactorLongThreshold.Mean);
            Assert.AreEqual(generalInput.ModelFactorLongThreshold.StandardDeviation, properties.ModelFactorLongThreshold.StandardDeviation);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            var properties = new StabilityPointStructuresFailureMechanismProperties(failureMechanism);

            // Assert
            const string generalCategory = "Algemeen";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

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

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[gravitationalAccelerationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravitationalAccelerationProperty,
                                                                            generalCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[modelFactorStorageVolumePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorStorageVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor kombergend vermogen [-]",
                                                                            "Modelfactor kombergend vermogen.",
                                                                            true);

            PropertyDescriptor modelFactorCollisionLoadProperty = dynamicProperties[modelFactorCollisionLoadPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorCollisionLoadProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorCollisionLoadProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor aanvaarbelasting [-]",
                                                                            "Modelfactor voor aanvaarbelasting.",
                                                                            true);

            PropertyDescriptor modelFactorLoadEffectProperty = dynamicProperties[modelFactorLoadEffectPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorLoadEffectProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorLoadEffectProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor belastingeffect [-]",
                                                                            "Modelfactor belastingeffect.",
                                                                            true);

            PropertyDescriptor modelFactorLongThresholdProperty = dynamicProperties[modelFactorLongThresholdPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorLongThresholdProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorLongThresholdProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor lange overlaat [-]",
                                                                            "Modelfactor voor een lange overlaat.",
                                                                            true);
        }
    }
}