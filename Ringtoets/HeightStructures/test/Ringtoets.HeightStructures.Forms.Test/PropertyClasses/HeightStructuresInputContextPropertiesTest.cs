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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.Properties;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;
using CoreCommonBasePropertiesResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresInputContextPropertiesTest
    {
        private const int structureNormalOrientationPropertyIndex = 0;
        private const int levelCrestStructurePropertyIndex = 1;
        private const int allowedLevelIncreaseStoragePropertyIndex = 2;
        private const int storageStructureAreaPropertyIndex = 3;
        private const int flowWidthAtBottomProtectionPropertyIndex = 4;
        private const int widthOfFlowAperturesPropertyIndex = 5;
        private const int criticalOvertoppingDischargePropertyIndex = 6;
        private const int failureProbabilityOfStructureGivenErosionPropertyIndex = 7;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 8;
        private const int hydraulicBoundaryLocationPropertyIndex = 9;
        private const int stormDurationPropertyIndex = 10;
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
            var properties = new HeightStructuresInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HeightStructuresInputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var properties = new HeightStructuresInputContextProperties();

            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            // Call
            properties.Data = inputContext;

            // Assert
            var modelFactorSuperCriticalFlowProperties = new NormalDistributionProperties
            {
                Data = input.ModelFactorSuperCriticalFlow
            };
            AssertDistributionProperties(modelFactorSuperCriticalFlowProperties, properties.ModelFactorSuperCriticalFlow);

            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);

            var levelCrestStructureProperties = new NormalDistributionProperties
            {
                Data = input.LevelCrestStructure
            };
            AssertDistributionProperties(levelCrestStructureProperties, properties.LevelCrestStructure);

            var allowedLevelIncreaseStorageProperties = new LogNormalDistributionProperties
            {
                Data = input.AllowedLevelIncreaseStorage
            };
            AssertDistributionProperties(allowedLevelIncreaseStorageProperties, properties.AllowedLevelIncreaseStorage);

            var storageStructureAreaProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StorageStructureArea
            };
            AssertLogNormalDistributionVariationProperties(storageStructureAreaProperties, properties.StorageStructureArea);

            var flowWidthAtBottomProtectionProperties = new LogNormalDistributionProperties
            {
                Data = input.FlowWidthAtBottomProtection
            };
            AssertDistributionProperties(flowWidthAtBottomProtectionProperties, properties.FlowWidthAtBottomProtection);

            var widthOfFlowAperturesProperties = new NormalDistributionVariationProperties
            {
                Data = input.WidthOfFlowApertures
            };
            AssertDistributionProperties(widthOfFlowAperturesProperties, properties.WidthOfFlowApertures);

            var criticalOvertoppingDischargeProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.CriticalOvertoppingDischarge
            };
            AssertLogNormalDistributionVariationProperties(criticalOvertoppingDischargeProperties, properties.CriticalOvertoppingDischarge);

            var expectedFailureProbabilityOfStructureGivenErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityOfStructureGivenErosion);
            Assert.AreEqual(expectedFailureProbabilityOfStructureGivenErosion, properties.FailureProbabilityOfStructureGivenErosion);

            Assert.AreEqual(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            var stormDurationProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StormDuration
            };
            AssertLogNormalDistributionVariationProperties(stormDurationProperties, properties.StormDuration);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 3;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new HeightStructuresCalculation();
            var input = calculation.InputParameters;
            input.Attach(observerMock);
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            var random = new Random(100);
            var newStructureNormalOrientation = new RoundedDouble(2, random.NextDouble());

            // Call
            properties.StructureNormalOrientation = newStructureNormalOrientation;
            properties.FailureProbabilityOfStructureGivenErosion = "1e-2";
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(newStructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreEqual(0.01, input.FailureProbabilityOfStructureGivenErosion);
            Assert.AreEqual("1/100", properties.FailureProbabilityOfStructureGivenErosion);
            Assert.AreSame(hydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityOfStructureGivenErosion_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOfStructureGivenErosion = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = Resources.FailureProbabilityOfStructureGivenErosion_Value_too_large;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityOfStructureGivenErosion_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOfStructureGivenErosion = newValue;

            // Assert
            var expectedMessage = Resources.FailureProbabilityOfStructureGivenErosion_Could_not_parse_string_to_double_value;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityOfStructureGivenErosion_NullValue_ThrowsArgumentException()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOfStructureGivenErosion = null;

            // Assert
            var expectedMessage = Resources.FailureProbabilityOfStructureGivenErosion_Value_cannot_be_null;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            // Call
            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            var schematizationCategory = "Schematisatie";
            var modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(12, dynamicProperties.Count);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            Assert.IsFalse(structureNormalOrientationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureNormalOrientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", structureNormalOrientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.", structureNormalOrientationProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("De kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[allowedLevelIncreaseStoragePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(allowedLevelIncreaseStorageProperty.Converter);
            Assert.AreEqual(schematizationCategory, allowedLevelIncreaseStorageProperty.Category);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", allowedLevelIncreaseStorageProperty.DisplayName);
            Assert.AreEqual("De toegestane peilverhoging op het kombergend oppervlak.", allowedLevelIncreaseStorageProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            Assert.AreEqual(schematizationCategory, storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Het kombergend oppervlak.", storageStructureAreaProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bij bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("De stroomvoerende breedte bij bodembescherming.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor widthOfFlowAperturesProperty = dynamicProperties[widthOfFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthOfFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, widthOfFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van de kruin van het kunstwerk [m]", widthOfFlowAperturesProperty.DisplayName);
            Assert.AreEqual("De breedte van de kruin van het kunstwerk.", widthOfFlowAperturesProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            Assert.AreEqual(schematizationCategory, criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek overslagdebiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Het kritieke overslagdebiet per strekkende meter.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor failureProbabilityOfStructureGivenErosionProperty = dynamicProperties[failureProbabilityOfStructureGivenErosionPropertyIndex];
            Assert.IsFalse(failureProbabilityOfStructureGivenErosionProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOfStructureGivenErosionProperty.Category);
            Assert.AreEqual("Faalkans kunstwerk gegeven erosie bodem [-]", failureProbabilityOfStructureGivenErosionProperty.DisplayName);
            Assert.AreEqual("De faalkans van het kunstwerk gegeven de erosie in de bodem.", failureProbabilityOfStructureGivenErosionProperty.Description);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, modelFactorSuperCriticalFlowProperty.Category);
            Assert.AreEqual("Modelfactor van overloopdebiet bij superkritische stroming [-]", modelFactorSuperCriticalFlowProperty.DisplayName);
            Assert.AreEqual("Het modelfactor van overloopdebiet bij superkritische stroming.", modelFactorSuperCriticalFlowProperty.Description);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual("Hydraulische gegevens", hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden.", hydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor stormDurationProperty = dynamicProperties[stormDurationPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stormDurationProperty.Converter);
            Assert.AreEqual("Hydraulische gegevens", stormDurationProperty.Category);
            Assert.AreEqual("Stormduur [uur]", stormDurationProperty.DisplayName);
            Assert.AreEqual("De duur van de storm.", stormDurationProperty.Description);

            mockRepository.VerifyAll();
        }

        private static void AssertDistributionProperties(DistributionProperties expected, DistributionProperties actual)
        {
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.Data, actual.Data);
        }

        private static void AssertLogNormalDistributionVariationProperties(LogNormalDistributionVariationProperties expected,
                                                                           LogNormalDistributionVariationProperties actual)
        {
            Assert.AreEqual(expected.Data, actual.Data);
        }
    }
}