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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using ClosingStructuresDataResources = Ringtoets.ClosingStructures.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructureFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var properties = new ClosingStructureFailureMechanismProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ClosingStructuresFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewFailureMechanismContext_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var properties = new ClosingStructureFailureMechanismProperties();

            // Call
            properties.Data = failureMechanism;

            // Assert
            Assert.AreEqual(ClosingStructuresDataResources.ClosingStructuresFailureMechanism_DisplayName,
                            properties.Name);
            Assert.AreEqual(ClosingStructuresDataResources.ClosingStructuresFailureMechanism_Code,
                            properties.Code);

            GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(generalInput.GravitationalAcceleration.Value,
                            properties.GravitationalAcceleration.Value);

            Assert.AreEqual(generalInput.C.Value, properties.C.Value);
            Assert.AreEqual(generalInput.N2A, properties.N2A);
            Assert.AreEqual(generalInput.N.Value, properties.LengthEffect.Value);

            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.Mean, properties.ModelFactorOvertoppingFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorOvertoppingFlow.StandardDeviation, properties.ModelFactorOvertoppingFlow.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.Mean, properties.ModelFactorStorageVolume.Mean);
            Assert.AreEqual(generalInput.ModelFactorStorageVolume.StandardDeviation, properties.ModelFactorStorageVolume.StandardDeviation);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.Mean, properties.ModelFactorSubCriticalFlow.Mean);
            Assert.AreEqual(generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation, properties.ModelFactorSubCriticalFlow.CoefficientOfVariation);
            Assert.AreEqual(generalInput.ModelFactorInflowVolume.Value, properties.ModelFactorInflowVolume.Value);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberProperties = 2;
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.Attach(observerMock);
            var properties = new ClosingStructureFailureMechanismProperties
            {
                Data = failureMechanism
            };

            RoundedDouble newC = (RoundedDouble) 2.1;
            const int newN2A = 10;

            // Call
            properties.C = newC;
            properties.N2A = newN2A;

            // Assert
            Assert.AreEqual(newC, failureMechanism.GeneralInput.C);
            Assert.AreEqual(newN2A, failureMechanism.GeneralInput.N2A);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var properties = new ClosingStructureFailureMechanismProperties
            {
                Data = failureMechanism
            };

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

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

            PropertyDescriptor gravitationalAccelerationProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(gravitationalAccelerationProperty,
                                                                            generalCategory,
                                                                            "Valversnelling [m/s²]",
                                                                            "Valversnelling.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            lengthEffectCategory,
                                                                            "C [-]",
                                                                            "De parameter 'C' die gebruikt wordt om het lengte-effect te berekenen.");

            PropertyDescriptor n2aProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(n2aProperty,
                                                                            lengthEffectCategory,
                                                                            "2NA [-]",
                                                                            "De parameter '2NA' die gebruikt wordt om het lengte-effect te berekenen.");

            PropertyDescriptor lengthEffectProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.",
                                                                            true);

            PropertyDescriptor modelFactorOvertoppingFlowProperty = dynamicProperties[6];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorOvertoppingFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorOvertoppingFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor overslagdebiet [-]",
                                                                            "Modelfactor voor het overslagdebiet.",
                                                                            true);

            PropertyDescriptor modelFactorStorageVolumeProperty = dynamicProperties[7];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorStorageVolumeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorStorageVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor kombergend vermogen [-]",
                                                                            "Modelfactor kombergend vermogen.",
                                                                            true);

            PropertyDescriptor modelFactorSubCriticalFlowProperty = dynamicProperties[8];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSubCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSubCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor voor onvolkomen stroming [-]",
                                                                            "Modelfactor voor onvolkomen stroming.",
                                                                            true);

            PropertyDescriptor modelFactorInflowVolumeProperty = dynamicProperties[9];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorInflowVolumeProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor instromend volume [-]",
                                                                            "Modelfactor instromend volume.",
                                                                            true);
        }
    }
}