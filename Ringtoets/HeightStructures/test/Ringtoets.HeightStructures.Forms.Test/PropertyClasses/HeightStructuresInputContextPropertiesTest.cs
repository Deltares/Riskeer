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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using CoreCommonBasePropertiesResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresInputContextPropertiesTest
    {
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
            var calculationMock = mockRepository.StrictMock<ICalculation>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var properties = new HeightStructuresInputContextProperties();

            var inputContext = new HeightStructuresInputContext(input, calculationMock, failureMechanism, assessmentSectionMock);

            // Call
            properties.Data = inputContext;

            // Assert
            var modelFactorOvertoppingSuperCriticalFlowProperties = new NormalDistributionProperties
            {
                Data = input.ModelFactorOvertoppingSuperCriticalFlow
            };
            AssertDistributionProperties(modelFactorOvertoppingSuperCriticalFlowProperties, properties.ModelFactorOvertoppingSuperCriticalFlow);

            Assert.AreEqual(input.OrientationOfTheNormalOfTheStructure, properties.OrientationOfTheNormalOfTheStructure);

            var levelOfCrestOfStructureProperties = new NormalDistributionProperties
            {
                Data = input.LevelOfCrestOfStructure
            };
            AssertDistributionProperties(levelOfCrestOfStructureProperties, properties.LevelOfCrestOfStructure);

            var allowableIncreaseOfLevelForStorageProperties = new LogNormalDistributionProperties
            {
                Data = input.AllowableIncreaseOfLevelForStorage
            };
            AssertDistributionProperties(allowableIncreaseOfLevelForStorageProperties, properties.AllowableIncreaseOfLevelForStorage);

            var storageStructureAreaProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StorageStructureArea
            };
            AssertDistributionProperties(storageStructureAreaProperties, properties.StorageStructureArea);

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
            AssertDistributionProperties(criticalOvertoppingDischargeProperties, properties.CriticalOvertoppingDischarge);

            var expectedFailureProbabilityOfStructureGivenErosion = string.Format(CoreCommonBasePropertiesResources.ProbabilityPerYearFormat,
                                                                                  input.FailureProbabilityOfStructureGivenErosion);
            Assert.AreEqual(expectedFailureProbabilityOfStructureGivenErosion, properties.FailureProbabilityOfStructureGivenErosion);

            Assert.AreEqual(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            var stormDurationProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StormDuration
            };
            AssertDistributionProperties(stormDurationProperties, properties.StormDuration);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 2;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var calculationMock = mockRepository.StrictMock<ICalculation>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            input.Attach(observerMock);
            var inputContext = new HeightStructuresInputContext(input, calculationMock, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            var random = new Random(100);
            var newModelFactorOvertoppingSuperCriticalFlowMean = new RoundedDouble(2, random.NextDouble());
            var newOrientationOfTheNormalOfTheStructure = new RoundedDouble(2, random.NextDouble());

            // Call
            properties.ModelFactorOvertoppingSuperCriticalFlow.Mean = newModelFactorOvertoppingSuperCriticalFlowMean;
            properties.OrientationOfTheNormalOfTheStructure = newOrientationOfTheNormalOfTheStructure;

            // Assert
            Assert.AreEqual(newModelFactorOvertoppingSuperCriticalFlowMean, properties.ModelFactorOvertoppingSuperCriticalFlow.Mean);
            Assert.AreEqual(newOrientationOfTheNormalOfTheStructure, properties.OrientationOfTheNormalOfTheStructure);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var calculationMock = mockRepository.StrictMock<ICalculation>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, calculationMock, failureMechanism, assessmentSectionMock);

            // Call
            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(12, dynamicProperties.Count);

            PropertyDescriptor orientationOfTheNormalOfTheStructureProperty = dynamicProperties[orientationOfTheNormalOfTheStructurePropertyIndex];
            Assert.IsNotNull(orientationOfTheNormalOfTheStructureProperty);
            Assert.IsFalse(orientationOfTheNormalOfTheStructureProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", orientationOfTheNormalOfTheStructureProperty.Category);
            Assert.AreEqual("Oriëntatie [º]", orientationOfTheNormalOfTheStructureProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van het kunstwerk.", orientationOfTheNormalOfTheStructureProperty.Description);

            PropertyDescriptor levelOfCrestOfStructureProperty = dynamicProperties[levelOfCrestOfStructurePropertyIndex];
            Assert.IsNotNull(levelOfCrestOfStructureProperty);
            Assert.AreEqual("Schematisatie", levelOfCrestOfStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m]", levelOfCrestOfStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk dat gebruikt wordt tijdens de berekening.", levelOfCrestOfStructureProperty.Description);

            PropertyDescriptor allowableIncreaseOfLevelForStorageProperty = dynamicProperties[allowableIncreaseOfLevelForStoragePropertyIndex];
            Assert.IsNotNull(allowableIncreaseOfLevelForStorageProperty);
            Assert.AreEqual("Schematisatie", allowableIncreaseOfLevelForStorageProperty.Category);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", allowableIncreaseOfLevelForStorageProperty.DisplayName);
            Assert.AreEqual("De toegestane peilverhoging komberging dat gebruikt wordt tijdens de berekening.", allowableIncreaseOfLevelForStorageProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            Assert.IsNotNull(storageStructureAreaProperty);
            Assert.AreEqual("Schematisatie", storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Het kombergend oppervlak dat gebruikt wordt tijdens de berekening.", storageStructureAreaProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            Assert.IsNotNull(flowWidthAtBottomProtectionProperty);
            Assert.AreEqual("Schematisatie", flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bij bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("De stroomvoerende breedte bij bodembescherming die gebruikt wordt tijdens de berekening.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor widthOfFlowAperturesProperty = dynamicProperties[widthOfFlowAperturesPropertyIndex];
            Assert.IsNotNull(widthOfFlowAperturesProperty);
            Assert.AreEqual("Schematisatie", widthOfFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van de kruin van het kunstwerk [m]", widthOfFlowAperturesProperty.DisplayName);
            Assert.AreEqual("De breedte van de kruin van het kunstwerk die gebruikt wordt tijdens de berekening.", widthOfFlowAperturesProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            Assert.IsNotNull(criticalOvertoppingDischargeProperty);
            Assert.AreEqual("Schematisatie", criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek overslagdebiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Het kritieke overslagdebiet per strekkende meter.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor failureProbabilityOfStructureGivenErosionProperty = dynamicProperties[failureProbabilityOfStructureGivenErosionPropertyIndex];
            Assert.IsNotNull(failureProbabilityOfStructureGivenErosionProperty);
            Assert.IsFalse(failureProbabilityOfStructureGivenErosionProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", failureProbabilityOfStructureGivenErosionProperty.Category);
            Assert.AreEqual("Faalkans kunstwerk gegeven erosie bodem [-]", failureProbabilityOfStructureGivenErosionProperty.DisplayName);
            Assert.AreEqual("De faalkans kunstwerk gegeven erosie bodem.", failureProbabilityOfStructureGivenErosionProperty.Description);

            PropertyDescriptor modelFactorOvertoppingSuperCriticalFlowProperty = dynamicProperties[modelFactorOvertoppingSuperCriticalFlowPropertyIndex];
            Assert.IsNotNull(modelFactorOvertoppingSuperCriticalFlowProperty);
            Assert.AreEqual("Modelfactoren", modelFactorOvertoppingSuperCriticalFlowProperty.Category);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", modelFactorOvertoppingSuperCriticalFlowProperty.DisplayName);
            Assert.AreEqual("Het modelfactor overloopdebiet volkomen overlaat dat gebruikt wordt tijdens de berekening.", modelFactorOvertoppingSuperCriticalFlowProperty.Description);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(modelFactorOvertoppingSuperCriticalFlowProperty);
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual("Hydraulische gegevens", hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden die gebruikt wordt tijdens de berekening.", hydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor stormDurationProperty = dynamicProperties[stormDurationPropertyIndex];
            Assert.IsNotNull(stormDurationProperty);
            Assert.AreEqual("Hydraulische gegevens", stormDurationProperty.Category);
            Assert.AreEqual("Stormduur [uren]", stormDurationProperty.DisplayName);
            Assert.AreEqual("De duur van de storm dat gebruikt wordt tijdens de berekening.", stormDurationProperty.Description);

            mockRepository.VerifyAll();
        }

        private static void AssertDistributionProperties(DistributionProperties expected, DistributionProperties actual)
        {
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.Data, actual.Data);
        }

        private static void AssertDistributionProperties(LogNormalDistributionVariationProperties expected, LogNormalDistributionVariationProperties actual)
        {
            Assert.AreEqual(expected.Data, actual.Data);
        }

        private const int orientationOfTheNormalOfTheStructurePropertyIndex = 0;
        private const int levelOfCrestOfStructurePropertyIndex = 1;
        private const int allowableIncreaseOfLevelForStoragePropertyIndex = 2;
        private const int storageStructureAreaPropertyIndex = 3;
        private const int flowWidthAtBottomProtectionPropertyIndex = 4;
        private const int widthOfFlowAperturesPropertyIndex = 5;
        private const int criticalOvertoppingDischargePropertyIndex = 6;
        private const int failureProbabilityOfStructureGivenErosionPropertyIndex = 7;
        private const int modelFactorOvertoppingSuperCriticalFlowPropertyIndex = 8;
        private const int hydraulicBoundaryLocationPropertyIndex = 9;
        private const int stormDurationPropertyIndex = 10;
    }
}