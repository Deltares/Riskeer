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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
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
        private const int heightStructurePropertyIndex = 0;
        private const int heightStructureLocationPropertyIndex = 1;
        private const int structureNormalOrientationPropertyIndex = 2;
        private const int levelCrestStructurePropertyIndex = 3;
        private const int allowedLevelIncreaseStoragePropertyIndex = 4;
        private const int storageStructureAreaPropertyIndex = 5;
        private const int flowWidthAtBottomProtectionPropertyIndex = 6;
        private const int widthFlowAperturesPropertyIndex = 7;
        private const int criticalOvertoppingDischargePropertyIndex = 8;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 9;
        private const int foreshoreProfilePropertyIndex = 10;
        private const int breakWaterPropertyIndex = 11;
        private const int foreshoreGeometryPropertyIndex = 12;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 13;
        private const int hydraulicBoundaryLocationPropertyIndex = 14;
        private const int stormDurationPropertyIndex = 15;
        private const int deviationWaveDirectionPropertyIndex = 16;

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
            Assert.IsNull(properties.HeightStructure);

            Assert.IsNull(properties.HeightStructureLocation);

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

            var widthFlowAperturesProperties = new NormalDistributionVariationProperties
            {
                Data = input.WidthFlowApertures
            };
            AssertDistributionProperties(widthFlowAperturesProperties, properties.WidthFlowApertures);

            var criticalOvertoppingDischargeProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.CriticalOvertoppingDischarge
            };
            AssertLogNormalDistributionVariationProperties(criticalOvertoppingDischargeProperties, properties.CriticalOvertoppingDischarge);

            Assert.IsNull(properties.ForeshoreProfile);

            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);

            Assert.IsInstanceOf<UseForeshoreProperties>(properties.ForeshoreGeometry);

            var expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);

            Assert.AreEqual(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            var stormDurationProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StormDuration
            };
            AssertLogNormalDistributionVariationProperties(stormDurationProperties, properties.StormDuration);

            Assert.AreEqual(input.DeviationWaveDirection, properties.DeviationWaveDirection);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewInputContextInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var input = new HeightStructuresInput
            {
                HeightStructure = CreateValidHeightStructure(),
                HydraulicBoundaryLocation = CreateValidHydraulicBoundaryLocation(),
                ForeshoreProfile = CreateValidForeshoreProfile()
            };
            var inputContext = new HeightStructuresInputContext(input,
                                                                new HeightStructuresFailureMechanism(),
                                                                assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties();

            // Call
            properties.Data = inputContext;

            // Assert
            Assert.AreSame(input.HeightStructure, properties.HeightStructure);

            var expectedHeightStructureLocation = new Point2D(new RoundedDouble(0, input.HeightStructure.Location.X), new RoundedDouble(0, input.HeightStructure.Location.Y));
            Assert.AreEqual(expectedHeightStructureLocation, properties.HeightStructureLocation);

            Assert.AreEqual(input.HeightStructure.StructureNormalOrientation, properties.StructureNormalOrientation);

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

            var widthFlowAperturesProperties = new NormalDistributionVariationProperties
            {
                Data = input.WidthFlowApertures
            };
            AssertDistributionProperties(widthFlowAperturesProperties, properties.WidthFlowApertures);

            var criticalOvertoppingDischargeProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.CriticalOvertoppingDischarge
            };
            AssertLogNormalDistributionVariationProperties(criticalOvertoppingDischargeProperties, properties.CriticalOvertoppingDischarge);

            var expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);

            Assert.AreSame(input.ForeshoreProfile, properties.ForeshoreProfile);

            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);

            Assert.IsInstanceOf<UseForeshoreProperties>(properties.ForeshoreGeometry);

            var modelFactorSuperCriticalFlowProperties = new NormalDistributionProperties
            {
                Data = input.ModelFactorSuperCriticalFlow
            };
            AssertDistributionProperties(modelFactorSuperCriticalFlowProperties, properties.ModelFactorSuperCriticalFlow);

            Assert.AreSame(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            var stormDurationProperties = new LogNormalDistributionVariationProperties
            {
                Data = input.StormDuration
            };
            AssertLogNormalDistributionVariationProperties(stormDurationProperties, properties.StormDuration);

            Assert.AreEqual(input.DeviationWaveDirection, properties.DeviationWaveDirection);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 6;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var hydraulicBoundaryLocation = CreateValidHydraulicBoundaryLocation();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
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
            double newStructureNormalOrientation = random.NextDouble();
            HeightStructure newHeightStructure = CreateValidHeightStructure();
            ForeshoreProfile newForeshoreProfile = CreateValidForeshoreProfile();
            double newDeviationWaveDirection = random.NextDouble();

            // Call
            properties.HeightStructure = newHeightStructure;
            properties.StructureNormalOrientation = (RoundedDouble) newStructureNormalOrientation;
            properties.FailureProbabilityStructureWithErosion = "1e-2";
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;
            properties.ForeshoreProfile = newForeshoreProfile;
            properties.DeviationWaveDirection = (RoundedDouble) newDeviationWaveDirection;

            // Assert
            Assert.AreSame(newHeightStructure, properties.HeightStructure);
            Assert.AreEqual(newStructureNormalOrientation, properties.StructureNormalOrientation,
                            properties.StructureNormalOrientation.GetAccuracy());
            Assert.AreEqual(0.01, input.FailureProbabilityStructureWithErosion);
            Assert.AreEqual("1/100", properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(hydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            Assert.AreSame(newForeshoreProfile, properties.ForeshoreProfile);
            Assert.AreEqual(newDeviationWaveDirection, properties.DeviationWaveDirection,
                            properties.DeviationWaveDirection.GetAccuracy());
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityStructureWithErosion_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = Resources.FailureProbabilityStructureWithErosion_Value_too_large;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityStructureWithErosion_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newValue;

            // Assert
            var expectedMessage = Resources.FailureProbabilityStructureWithErosion_Could_not_parse_string_to_double_value;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityStructureWithErosion_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var input = new HeightStructuresInput();
            var inputContext = new HeightStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = null;

            // Assert
            var expectedMessage = Resources.FailureProbabilityStructureWithErosion_Value_cannot_be_null;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
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
            const string schematizationCategory = "Schematisatie";
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(17, dynamicProperties.Count);

            PropertyDescriptor heightStructureProperty = dynamicProperties[heightStructurePropertyIndex];
            Assert.IsFalse(heightStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, heightStructureProperty.Category);
            Assert.AreEqual("Kunstwerk", heightStructureProperty.DisplayName);
            Assert.AreEqual("Het kunstwerk dat gebruikt wordt in de berekening.", heightStructureProperty.Description);

            PropertyDescriptor heightStructureLocationProperty = dynamicProperties[heightStructureLocationPropertyIndex];
            Assert.IsTrue(heightStructureLocationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, heightStructureLocationProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", heightStructureLocationProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.", heightStructureLocationProperty.Description);

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
            Assert.AreEqual("Toegestane peilverhoging komberging.", allowedLevelIncreaseStorageProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            Assert.AreEqual(schematizationCategory, storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Kombergend oppervlak.", storageStructureAreaProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[widthFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, widthFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van doorstroomopening [m]", widthFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Breedte van de doorstroomopening.", widthFlowAperturesProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            Assert.AreEqual(schematizationCategory, criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Kritiek instromend debiet directe invoer.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex];
            Assert.IsFalse(failureProbabilityStructureWithErosionProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityStructureWithErosionProperty.Category);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", failureProbabilityStructureWithErosionProperty.DisplayName);
            Assert.AreEqual("De faalkans van het kunstwerk gegeven de erosie in de bodem.", failureProbabilityStructureWithErosionProperty.Description);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, modelFactorSuperCriticalFlowProperty.Category);
            Assert.AreEqual("Modelfactor van overloopdebiet bij superkritische stroming [-]", modelFactorSuperCriticalFlowProperty.DisplayName);
            Assert.AreEqual("Het modelfactor van overloopdebiet bij superkritische stroming.", modelFactorSuperCriticalFlowProperty.Description);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[foreshoreProfilePropertyIndex];
            Assert.IsFalse(foreshoreProfileProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, foreshoreProfileProperty.Category);
            Assert.AreEqual("Voorlandprofiel", foreshoreProfileProperty.DisplayName);
            Assert.AreEqual("De schematisatie van het voorlandprofiel.", foreshoreProfileProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, breakWaterProperty.Category);
            Assert.AreEqual("Dam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", breakWaterProperty.Description);

            PropertyDescriptor foreshoreGeometryProperty = dynamicProperties[foreshoreGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreGeometryProperty.Converter);
            Assert.IsTrue(foreshoreGeometryProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, foreshoreGeometryProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", foreshoreGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", foreshoreGeometryProperty.Description);

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

            PropertyDescriptor deviationWaveDirectionProperty = dynamicProperties[deviationWaveDirectionPropertyIndex];
            Assert.AreEqual("Hydraulische gegevens", deviationWaveDirectionProperty.Category);
            Assert.AreEqual("Afwijking van de golfrichting [°]", deviationWaveDirectionProperty.DisplayName);
            Assert.AreEqual("De afwijking van de golfrichting.", deviationWaveDirectionProperty.Description);

            mockRepository.VerifyAll();
        }

        private static ForeshoreProfile CreateValidForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties());
        }

        private static HydraulicBoundaryLocation CreateValidHydraulicBoundaryLocation()
        {
            return new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);
        }

        private static HeightStructure CreateValidHeightStructure()
        {
            return new HeightStructure("aName", "anId", new Point2D(1, 1),
                                       0.12345,
                                       234.567, 0.23456,
                                       345.678, 0.34567,
                                       456.789, 0.45678,
                                       567.890, 0.56789,
                                       0.67890,
                                       112.223, 0.11222,
                                       225.336, 0.22533);
        }

        private static void AssertDistributionProperties<T>(DistributionPropertiesBase<T> expected, DistributionPropertiesBase<T> actual) where T : IDistribution
        {
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.Data, actual.Data);
        }

        private static void AssertDistributionProperties<T>(VariationCoefficientDistributionPropertiesBase<T> expected, VariationCoefficientDistributionPropertiesBase<T> actual) where T : IVariationCoefficientDistribution
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