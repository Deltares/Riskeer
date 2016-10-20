﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismContextPropertiesTest
    {

        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int gravitationalAccelerationPropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;
        private const int modelFactorStorageVolumePropertyIndex = 4;
        private const int modelFactorSubCriticalFlowPropertyIndex = 5;
        private const int modelFactorCollisionLoadPropertyIndex = 6;
        private const int modelFactorLoadEffectPropertyIndex = 7;
        private const int modelFactorInflowVolumePropertyIndex = 8;
        private const int modificationFactorWavesSlowlyVaryingPressureComponentPropertyIndex = 9;
        private const int modificationFactorDynamicOrImpulsivePressureComponentPropertyIndex = 10;
        private const int waveRatioMaxHNPropertyIndex = 11;
        private const int waveRatioMaxHStandardDeviationPropertyIndex = 12;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityPointStructuresFailureMechanismContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityPointStructuresFailureMechanismContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var properties = new StabilityPointStructuresFailureMechanismContextProperties();

            // Call
            properties.Data = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual("Kunstwerken - Sterkte en stabiliteit puntconstructies", properties.Name);
            Assert.AreEqual("STKWp", properties.Code);
            Assert.AreEqual(3, properties.LengthEffect);

            GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.GravitationalAcceleration, properties.GravitationalAcceleration);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.Mean, properties.ModelFactorSubCriticalFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation, properties.ModelFactorSubCriticalFlow.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.Mean, properties.ModelFactorCollisionLoad.Mean);
            Assert.AreEqual(generalInput.ModelFactorCollisionLoad.CoefficientOfVariation, properties.ModelFactorCollisionLoad.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.Mean, properties.ModelFactorLoadEffect.Mean);
            Assert.AreEqual(generalInput.ModelFactorLoadEffect.StandardDeviation, properties.ModelFactorLoadEffect.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorInflowVolume, properties.ModelFactorInflowVolume);
            Assert.AreEqual(generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent, properties.ModificationFactorWavesSlowlyVaryingPressureComponent);
            Assert.AreEqual(generalInput.ModificationFactorDynamicOrImpulsivePressureComponent, properties.ModificationFactorDynamicOrImpulsivePressureComponent);
            Assert.AreEqual(generalInput.WaveRatioMaxHN, properties.WaveRatioMaxHN);
            Assert.AreEqual(generalInput.WaveRatioMaxHStandardDeviation, properties.WaveRatioMaxHStandardDeviation);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.Attach(observerMock);
            var properties = new StabilityPointStructuresFailureMechanismContextProperties
            {
                Data = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock)
            };
            const int newLengthEffect = 10;

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            var properties = new StabilityPointStructuresFailureMechanismContextProperties
            {
                Data = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock)
            };

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(13, dynamicProperties.Count);

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

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[gravitationalAccelerationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravitationalAccelerationProperty,
                                                                            generalCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[modelFactorStorageVolumePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorStorageVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor kombergend vermogen [-]",
                                                                            "Modelfactor kombergend vermogen.",
                                                                            true);

            PropertyDescriptor modelFactorSubCriticalFlowProperty = dynamicProperties[modelFactorSubCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSubCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSubCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor onvolkomen stroming [-]",
                                                                            "Modelfactor voor onvolkomen stroming.",
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

            PropertyDescriptor modelFactorInflowVolumeProperty = dynamicProperties[modelFactorInflowVolumePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorInflowVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor instromend volume [-]",
                                                                            "Modelfactor instromend volume.",
                                                                            true);

            PropertyDescriptor modificationFactorWavesSlowlyVaryingPressureComponentProperty = dynamicProperties[modificationFactorWavesSlowlyVaryingPressureComponentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modificationFactorWavesSlowlyVaryingPressureComponentProperty,
                                                                            modelSettingsCategory,
                                                                            "Modificatiefactor geometrie van de wand [-]",
                                                                            "Modificatiefactor voor de geometrie van de wand.",
                                                                            true);

            PropertyDescriptor modificationFactorDynamicOrImpulsivePressureComponentProperty = dynamicProperties[modificationFactorDynamicOrImpulsivePressureComponentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modificationFactorDynamicOrImpulsivePressureComponentProperty,
                                                                            modelSettingsCategory,
                                                                            "Modificatiefactor aard van de wand [-]",
                                                                            "Modificatiefactor voor de aard van de wand.",
                                                                            true);

            PropertyDescriptor waveRatioMaxHNProperty = dynamicProperties[waveRatioMaxHNPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveRatioMaxHNProperty,
                                                                            modelSettingsCategory,
                                                                            "Rayleigh-N N parameter [-]",
                                                                            "N parameter van de Rayleigh-N verdeling voor het quotiënt van Hmax en Hs voor N golven.",
                                                                            true);

            PropertyDescriptor waveRatioMaxHStandardDeviationProperty = dynamicProperties[waveRatioMaxHStandardDeviationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveRatioMaxHStandardDeviationProperty,
                                                                            modelSettingsCategory,
                                                                            "Rayleigh-N schaal parameter [-]",
                                                                            "Schaal parameter van de Rayleigh-N verdeling voor het quotiënt van Hmax en Hs voor N golven.",
                                                                            true);

            mockRepository.VerifyAll();
        }
    }
}