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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.Properties;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;

namespace Ringtoets.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresFailureMechanismContextPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int gravitationalAccelerationPropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;
        private const int modelFactorOvertoppingFlowPropertyIndex = 4;
        private const int modelFactorStorageVolumePropertyIndex = 5;
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
            var properties = new HeightStructuresFailureMechanismContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HeightStructuresFailureMechanismContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var properties = new HeightStructuresFailureMechanismContextProperties();

            // Call
            properties.Data = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Assert
            Assert.AreEqual(Resources.HeightStructuresFailureMechanism_DisplayName, properties.Name);
            Assert.AreEqual(Resources.HeightStructuresFailureMechanism_Code, properties.Code);
            Assert.AreEqual(failureMechanism.GeneralInput.GravitationalAcceleration, properties.GravitationalAcceleration);
            Assert.AreEqual(2, properties.LengthEffect);

            var generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.Mean, properties.ModelFactorOvertoppingFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.StandardDeviation, properties.ModelFactorOvertoppingFlow.StandardDeviation);

            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 1;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.Attach(observerMock);
            var properties = new HeightStructuresFailureMechanismContextProperties
            {
                Data = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock)
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

            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var properties = new HeightStructuresFailureMechanismContextProperties
            {
                Data = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock)
            };

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het toetsspoor.", nameProperty.Description);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            Assert.IsNotNull(codeProperty);
            Assert.IsTrue(codeProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Label", codeProperty.DisplayName);
            Assert.AreEqual("Het label van het toetsspoor.", codeProperty.Description);

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[gravitationalAccelerationPropertyIndex];
            Assert.IsNotNull(gravitationalAccelerationProperty);
            Assert.IsTrue(gravitationalAccelerationProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, gravitationalAccelerationProperty.Category);
            Assert.AreEqual("Valversnelling [m/s²]", gravitationalAccelerationProperty.DisplayName);
            Assert.AreEqual("Valversnelling.", gravitationalAccelerationProperty.Description);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            Assert.IsNotNull(lengthEffectProperty);
            Assert.IsFalse(lengthEffectProperty.IsReadOnly);
            Assert.AreEqual(lengthEffectCategory, lengthEffectProperty.Category);
            Assert.AreEqual("N [-]", lengthEffectProperty.DisplayName);
            Assert.AreEqual("De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.", lengthEffectProperty.Description);

            PropertyDescriptor modelFactorOvertoppingFlowProperty = dynamicProperties[modelFactorOvertoppingFlowPropertyIndex];
            Assert.IsNotNull(modelFactorOvertoppingFlowProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorOvertoppingFlowProperty.Converter);
            Assert.IsTrue(modelFactorOvertoppingFlowProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, modelFactorOvertoppingFlowProperty.Category);
            Assert.AreEqual("Modelfactor overslagdebiet [-]", modelFactorOvertoppingFlowProperty.DisplayName);
            Assert.AreEqual("Modelfactor voor het overslagdebiet.", modelFactorOvertoppingFlowProperty.Description);

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[modelFactorStorageVolumePropertyIndex];
            Assert.IsNotNull(modelFactorStorageVolumeProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            Assert.IsTrue(modelFactorStorageVolumeProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, modelFactorStorageVolumeProperty.Category);
            Assert.AreEqual("Modelfactor kombergend vermogen [-]", modelFactorStorageVolumeProperty.DisplayName);
            Assert.AreEqual("Modelfactor kombergend vermogen.", modelFactorStorageVolumeProperty.Description);

            mockRepository.VerifyAll();
        }
    }
}