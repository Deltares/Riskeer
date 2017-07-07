﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StructuresInputBasePropertiesTest
    {
        private MockRepository mockRepository;
        private IObservablePropertyChangeHandler handler;
        private IAssessmentSection assessmentSection;
        private IFailureMechanism failureMechanism;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            assessmentSection = mockRepository.Stub<IAssessmentSection>();
            failureMechanism = mockRepository.Stub<IFailureMechanism>();
        }

        [Test]
        public void Constructor_ConstructionPropertiesIsNull_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);
            // Call
            TestDelegate call = () => new SimpleStructuresInputProperties(inputContext, null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_DataIsNull_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            StructuresInputBaseProperties<
                TestStructure, 
                SimpleStructureInput, 
                StructuresCalculation<SimpleStructureInput>, 
                IFailureMechanism>
                .ConstructionProperties constructionProperties = GetRandomConstructionProperties();

            // Call
            TestDelegate call = () => new SimpleStructuresInputProperties(null, constructionProperties, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("data", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerIsNull_ThrowsArgumentNullException()
        {
            // Setup
            StructuresInputBaseProperties<
                TestStructure, 
                SimpleStructureInput, 
                StructuresCalculation<SimpleStructureInput>, 
                IFailureMechanism>
                .ConstructionProperties constructionProperties = GetRandomConstructionProperties();
            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            // Call
            TestDelegate call = () => new SimpleStructuresInputProperties(inputContext, constructionProperties, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            mockRepository.ReplayAll();

            StructuresInputBaseProperties<
                TestStructure, 
                SimpleStructureInput, 
                StructuresCalculation<SimpleStructureInput>, 
                IFailureMechanism>
                .ConstructionProperties constructionProperties = GetRandomConstructionProperties();
            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            // Call
            var properties = new SimpleStructuresInputProperties(
                inputContext,
                constructionProperties,
                handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsInstanceOf<IHasStructureProperty<TestStructure>>(properties);
            Assert.IsInstanceOf<IHasForeshoreProfileProperty>(properties);
            Assert.AreSame(inputContext, properties.Data);

            SimpleStructureInput input = calculation.InputParameters;
            string expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);

            Assert.IsNull(properties.Structure);
            Assert.IsNull(properties.StructureLocation);
            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);
            Assert.AreSame(input.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.AreSame(input.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.AreSame(input.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.AreSame(input.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.IsNull(properties.ForeshoreProfile);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.UseBreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.UseForeshore);
            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);
            Assert.IsNull(properties.SelectedHydraulicBoundaryLocation);
            Assert.AreSame(input.StormDuration, properties.StormDuration.Data);

            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string outputSettingsCategory = "Uitvoer";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            PropertyDescriptor structureProperty = dynamicProperties[constructionProperties.StructurePropertyIndex];
            Assert.IsFalse(structureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureProperty.Category);
            Assert.AreEqual("Kunstwerk", structureProperty.DisplayName);
            Assert.AreEqual("Het kunstwerk dat gebruikt wordt in de berekening.", structureProperty.Description);

            PropertyDescriptor structureLocationProperty = dynamicProperties[constructionProperties.StructureLocationPropertyIndex];
            Assert.IsTrue(structureLocationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureLocationProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", structureLocationProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.", structureLocationProperty.Description);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[constructionProperties.StructureNormalOrientationPropertyIndex];
            Assert.IsTrue(structureNormalOrientationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureNormalOrientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", structureNormalOrientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.", structureNormalOrientationProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[constructionProperties.FlowWidthAtBottomProtectionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[constructionProperties.WidthFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, widthFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van doorstroomopening [m]", widthFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Breedte van de doorstroomopening.", widthFlowAperturesProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[constructionProperties.StorageStructureAreaPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            Assert.AreEqual(schematizationCategory, storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Kombergend oppervlak.", storageStructureAreaProperty.Description);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[constructionProperties.AllowedLevelIncreaseStoragePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(allowedLevelIncreaseStorageProperty.Converter);
            Assert.AreEqual(schematizationCategory, allowedLevelIncreaseStorageProperty.Category);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", allowedLevelIncreaseStorageProperty.DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging.", allowedLevelIncreaseStorageProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[constructionProperties.CriticalOvertoppingDischargePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            Assert.AreEqual(schematizationCategory, criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Kritiek instromend debiet directe invoer per strekkende meter.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[constructionProperties.FailureProbabilityStructureWithErosionPropertyIndex];
            Assert.IsFalse(failureProbabilityStructureWithErosionProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityStructureWithErosionProperty.Category);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", failureProbabilityStructureWithErosionProperty.DisplayName);
            Assert.AreEqual("Faalkans kunstwerk gegeven erosie bodem.", failureProbabilityStructureWithErosionProperty.Description);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[constructionProperties.ModelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, modelFactorSuperCriticalFlowProperty.Category);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", modelFactorSuperCriticalFlowProperty.DisplayName);
            Assert.AreEqual("Modelfactor voor het overloopdebiet over een volkomen overlaat.", modelFactorSuperCriticalFlowProperty.Description);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[constructionProperties.ForeshoreProfilePropertyIndex];
            Assert.IsFalse(foreshoreProfileProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, foreshoreProfileProperty.Category);
            Assert.AreEqual("Voorlandprofiel", foreshoreProfileProperty.DisplayName);
            Assert.AreEqual("De schematisatie van het voorlandprofiel.", foreshoreProfileProperty.Description);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[constructionProperties.UseBreakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(useBreakWaterProperty.Converter);
            Assert.IsTrue(useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, useBreakWaterProperty.Category);
            Assert.AreEqual("Dam", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", useBreakWaterProperty.Description);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[constructionProperties.UseForeshorePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(useForeshoreProperty.Converter);
            Assert.IsTrue(useForeshoreProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, useForeshoreProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", useForeshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", useForeshoreProperty.Description);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[constructionProperties.HydraulicBoundaryLocationPropertyIndex];
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden.", hydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor stormDurationProperty = dynamicProperties[constructionProperties.StormDurationPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stormDurationProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, stormDurationProperty.Category);
            Assert.AreEqual("Stormduur [uur]", stormDurationProperty.DisplayName);
            Assert.AreEqual("Stormduur.", stormDurationProperty.Description);

            PropertyDescriptor shouldIllustrationPointsBeCalculatedProperty = dynamicProperties[dynamicProperties.Count - 1];
            Assert.AreEqual(outputSettingsCategory, shouldIllustrationPointsBeCalculatedProperty.Category);
            Assert.AreEqual("Illustratiepunten inlezen", shouldIllustrationPointsBeCalculatedProperty.DisplayName);
            Assert.AreEqual("Neem de informatie over de illustratiepunten op in het berekeningsresultaat.", shouldIllustrationPointsBeCalculatedProperty.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithOrWithoutStructure_CorrectReadOnlyForStructureDependentProperties(bool hasStructure)
        {
            // Setup
            mockRepository.ReplayAll();

            StructuresInputBaseProperties<
                TestStructure, 
                SimpleStructureInput, 
                StructuresCalculation<SimpleStructureInput>, 
                IFailureMechanism>
                .ConstructionProperties constructionProperties = GetRandomConstructionProperties();
            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            if (hasStructure)
            {
                calculation.InputParameters.Structure = new TestStructure();
            }

            // Call
            var properties = new SimpleStructuresInputProperties(
                inputContext,
                constructionProperties,
                handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            bool expectedReadOnly = !hasStructure;

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[constructionProperties.StructureNormalOrientationPropertyIndex];
            Assert.AreEqual(expectedReadOnly, structureNormalOrientationProperty.IsReadOnly);

            AssertPropertiesInState(properties.FlowWidthAtBottomProtection, expectedReadOnly);
            AssertPropertiesInState(properties.WidthFlowApertures, expectedReadOnly);
            AssertPropertiesInState(properties.StorageStructureArea, expectedReadOnly);
            AssertPropertiesInState(properties.AllowedLevelIncreaseStorage, expectedReadOnly);
            AssertPropertiesInState(properties.CriticalOvertoppingDischarge, expectedReadOnly);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);
            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsStructure_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(new Point2D(200620.173572981, 503401.652985217))
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            // Call 
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            double distanceToPropertiesStructureLocation =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.StructureLocation);
            double distanceToInputStructureLocation =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(calculation.InputParameters.Structure.Location);
            Assert.AreEqual(59, distanceToPropertiesStructureLocation, 1);
            Assert.AreEqual(60, distanceToInputStructureLocation, 1);

            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            RoundedDouble itemDistance = hydraulicBoundaryLocationItem.Distance;
            Assert.AreEqual(distanceToInputStructureLocation, itemDistance, itemDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputWithLocationsStructure_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    Structure = new TestStructure(new Point2D(200620.173572981, 503401.652985217))
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            // Call 
            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            double distanceToPropertiesStructureLocation =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.StructureLocation);
            double distanceToInputStructureLocation =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(calculation.InputParameters.Structure.Location);
            Assert.AreEqual(59, distanceToPropertiesStructureLocation, 1);
            Assert.AreEqual(60, distanceToInputStructureLocation, 1);

            RoundedDouble selectedLocationDistance = selectedHydraulicBoundaryLocation.Distance;
            Assert.AreEqual(distanceToInputStructureLocation, selectedLocationDistance, selectedLocationDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithStructureAndLocations_WhenSelectingLocation_ThenSelectedLocationDistanceSameAsLocationItem()
        {
            // Given
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    Structure = new TestStructure(new Point2D(200620.173572981, 503401.652985217))
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            // When
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Then
            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            Assert.AreEqual(selectedLocation.Distance, hydraulicBoundaryLocationItem.Distance,
                            hydraulicBoundaryLocationItem.Distance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndNoStructure_ReturnsLocationsSortedById()
        {
            // Setup
            mockRepository.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 1),
                    new HydraulicBoundaryLocation(4, "C", 0, 2),
                    new HydraulicBoundaryLocation(3, "D", 0, 3),
                    new HydraulicBoundaryLocation(2, "B", 0, 4)
                }
            };
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);
            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null))
                                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndStructure_ReturnsLocationsSortByDistanceThenById()
        {
            // Setup
            mockRepository.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);
            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl => new SelectableHydraulicBoundaryLocation(
                                                     hbl,
                                                     calculation.InputParameters.Structure.Location))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingStructure_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
            mockRepository.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var newStructure = new TestStructure(new Point2D(0, 190));

            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

            // When
            properties.Structure = newStructure;

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl => new SelectableHydraulicBoundaryLocation(
                                                     hbl,
                                                     properties.StructureLocation))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 6;
            var observerMock = mockRepository.StrictMock<IObserver>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                new ObservablePropertyChangeHandler(inputContext.Calculation, calculation.InputParameters));

            inputContext.Attach(observerMock);

            var random = new Random(100);
            double newStructureNormalOrientation = random.NextDouble();
            const bool newShouldIllustrationPointsBeCalculated = true;
            var newStructure = new TestStructure();
            ForeshoreProfile newForeshoreProfile = new TestForeshoreProfile();
            HydraulicBoundaryLocation newHydraulicBoundaryLocation = CreateHydraulicBoundaryLocation();
            var newSelectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newHydraulicBoundaryLocation, null);

            // Call
            properties.Structure = newStructure;
            properties.StructureNormalOrientation = (RoundedDouble) newStructureNormalOrientation;
            properties.FailureProbabilityStructureWithErosion = "1e-2";
            properties.SelectedHydraulicBoundaryLocation = newSelectableHydraulicBoundaryLocation;
            properties.ForeshoreProfile = newForeshoreProfile;
            properties.ShouldIllustrationPointsBeCalculated = newShouldIllustrationPointsBeCalculated;

            // Assert
            Assert.AreSame(newStructure, properties.Structure);
            Assert.AreEqual(newStructureNormalOrientation, properties.StructureNormalOrientation, properties.StructureNormalOrientation.GetAccuracy());
            Assert.AreEqual(0.01, inputContext.WrappedData.FailureProbabilityStructureWithErosion);
            Assert.AreEqual("1/100", properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(newHydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreSame(newForeshoreProfile, properties.ForeshoreProfile);
            Assert.AreEqual(newShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Structure_Always_InputChangedAndObservablesNotified()
        {
            var structure = new TestStructure();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.Structure = structure);
        }

        [Test]
        public void StructureNormalOrientation_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble orientation = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.StructureNormalOrientation = orientation);
        }

        [Test]
        public void FailureProbabilityStructureWithErosion_Always_InputChangedAndObservablesNotified()
        {
            string failureProbability = new Random(21).NextDouble().ToString(CultureInfo.CurrentCulture);
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.FailureProbabilityStructureWithErosion = failureProbability);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_Always_InputChangedAndObservablesNotified()
        {
            var location = new SelectableHydraulicBoundaryLocation(CreateHydraulicBoundaryLocation(), null);
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.SelectedHydraulicBoundaryLocation = location);
        }

        [Test]
        public void ForeshoreProfile_Always_InputChangedAndObservablesNotified()
        {
            var profile = new TestForeshoreProfile();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.ForeshoreProfile = profile);
        }

        [Test]
        public void UseBreakWater_Always_InputChangedAndObservablesNotified()
        {
            bool useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.UseBreakWater.UseBreakWater = useBreakWater);
        }

        [Test]
        public void UseForeshore_Always_InputChangedAndObservablesNotified()
        {
            bool useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.UseForeshore.UseForeshore = useForeshore);
        }

        [Test]
        public void ModelFactorSuperCriticalFlow_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.ModelFactorSuperCriticalFlow.Mean = newMean);
        }

        [Test]
        public void FlowWidthAtBottomProtection_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.FlowWidthAtBottomProtection.Mean = newMean);
        }

        [Test]
        public void WidthFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.WidthFlowApertures.Mean = newMean);
        }

        [Test]
        public void StorageStructureArea_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.StorageStructureArea.Mean = newMean);
        }

        [Test]
        public void AllowedLevelIncreaseStorage_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.AllowedLevelIncreaseStorage.Mean = newMean);
        }

        [Test]
        public void CriticalOvertoppingDischarge_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.CriticalOvertoppingDischarge.Mean = newMean);
        }

        [Test]
        public void StormDuration_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.StormDuration.Mean = newMean);
        }

        [Test]
        public void Structure_NullValue_AfterSettingStructureCalled()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>,
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            // Precondition
            Assert.IsFalse(properties.AfterSettingStructureCalled);

            // Call
            properties.Structure = null;

            // Assert
            Assert.IsTrue(properties.AfterSettingStructureCalled);
        }

        [Test]
        public void Structure_ValidValue_AfterSettingStructureCalled()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            var newStructure = new TestStructure();
            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            // Precondition
            Assert.IsFalse(properties.AfterSettingStructureCalled);

            // Call
            properties.Structure = newStructure;

            // Assert
            Assert.IsTrue(properties.AfterSettingStructureCalled);
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void FailureProbabilityStructureWithErosion_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            string newStringValue = newValue.ToString(CultureInfo.InvariantCulture);
            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newStringValue;

            // Assert
            const string expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void FailureProbabilityStructureWithErosion_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newValue;

            // Assert
            const string expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureProbabilityStructureWithErosion_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);

            CalculationInputSetPropertyValueAfterConfirmationParameterTester customHandler =
                CreateCustomHandlerForCalculationReturningNoObservables();

            var properties = new SimpleStructuresInputProperties(
                inputContext,
                new StructuresInputBaseProperties<
                    TestStructure, 
                    SimpleStructureInput, 
                    StructuresCalculation<SimpleStructureInput>, 
                    IFailureMechanism>
                    .ConstructionProperties(),
                customHandler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = null;

            // Assert
            const string expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        private static CalculationInputSetPropertyValueAfterConfirmationParameterTester CreateCustomHandlerForCalculationReturningNoObservables()
        {
            return new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
        }

        private static StructuresInputBaseProperties<
            TestStructure, 
            SimpleStructureInput, 
            StructuresCalculation<SimpleStructureInput>, 
            IFailureMechanism>
            .ConstructionProperties
            GetRandomConstructionProperties()
        {
            var structureObject = new object();
            var structureLocationObject = new object();
            var structureNormalOrientationObject = new object();
            var flowWidthAtBottomProtectionObject = new object();
            var widthFlowAperturesObject = new object();
            var storageStructureAreaObject = new object();
            var allowedLevelIncreaseStorageObject = new object();
            var criticalOvertoppingDischargeObject = new object();
            var failureProbabilityStructureWithErosionObject = new object();
            var foreshoreProfileObject = new object();
            var useBreakWaterObject = new object();
            var useForeshoreObject = new object();
            var modelFactorSuperCriticalFlowObject = new object();
            var hydraulicBoundaryLocationObject = new object();
            var stormDurationObject = new object();

            var random = new Random();
            List<object> randomObjectLookup = new[]
                {
                    structureObject,
                    structureLocationObject,
                    structureNormalOrientationObject,
                    flowWidthAtBottomProtectionObject,
                    widthFlowAperturesObject,
                    storageStructureAreaObject,
                    allowedLevelIncreaseStorageObject,
                    criticalOvertoppingDischargeObject,
                    failureProbabilityStructureWithErosionObject,
                    foreshoreProfileObject,
                    useBreakWaterObject,
                    useForeshoreObject,
                    modelFactorSuperCriticalFlowObject,
                    hydraulicBoundaryLocationObject,
                    stormDurationObject
                }.OrderBy(p => random.Next())
                 .ToList();

            return new StructuresInputBaseProperties<
                TestStructure, 
                SimpleStructureInput, 
                StructuresCalculation<SimpleStructureInput>, 
                IFailureMechanism>
                .ConstructionProperties
            {
                StructurePropertyIndex = randomObjectLookup.IndexOf(structureObject),
                StructureLocationPropertyIndex = randomObjectLookup.IndexOf(structureLocationObject),
                StructureNormalOrientationPropertyIndex = randomObjectLookup.IndexOf(structureNormalOrientationObject),
                FlowWidthAtBottomProtectionPropertyIndex = randomObjectLookup.IndexOf(flowWidthAtBottomProtectionObject),
                WidthFlowAperturesPropertyIndex = randomObjectLookup.IndexOf(widthFlowAperturesObject),
                StorageStructureAreaPropertyIndex = randomObjectLookup.IndexOf(storageStructureAreaObject),
                AllowedLevelIncreaseStoragePropertyIndex = randomObjectLookup.IndexOf(allowedLevelIncreaseStorageObject),
                CriticalOvertoppingDischargePropertyIndex = randomObjectLookup.IndexOf(criticalOvertoppingDischargeObject),
                FailureProbabilityStructureWithErosionPropertyIndex = randomObjectLookup.IndexOf(failureProbabilityStructureWithErosionObject),
                ForeshoreProfilePropertyIndex = randomObjectLookup.IndexOf(foreshoreProfileObject),
                UseBreakWaterPropertyIndex = randomObjectLookup.IndexOf(useBreakWaterObject),
                UseForeshorePropertyIndex = randomObjectLookup.IndexOf(useForeshoreObject),
                ModelFactorSuperCriticalFlowPropertyIndex = randomObjectLookup.IndexOf(modelFactorSuperCriticalFlowObject),
                HydraulicBoundaryLocationPropertyIndex = randomObjectLookup.IndexOf(hydraulicBoundaryLocationObject),
                StormDurationPropertyIndex = randomObjectLookup.IndexOf(stormDurationObject)
            };
        }

        private static HydraulicBoundaryLocation CreateHydraulicBoundaryLocation()
        {
            return new HydraulicBoundaryLocation(0, "", 0, 0);
        }

        private class SimpleStructureInput : StructuresInputBase<TestStructure>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }

        private class SimpleStructuresInputProperties : StructuresInputBaseProperties<
            TestStructure,
            SimpleStructureInput,
            StructuresCalculation<SimpleStructureInput>,
            IFailureMechanism>
        {
            public SimpleStructuresInputProperties(
                SimpleInputContext context,
                ConstructionProperties constructionProperties,
                IObservablePropertyChangeHandler handler)
                : base(context, constructionProperties, handler) {}

            [Browsable(false)]
            public bool AfterSettingStructureCalled { get; private set; }

            public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
            {
                yield return new TestForeshoreProfile();
            }

            public override IEnumerable<TestStructure> GetAvailableStructures()
            {
                yield return new TestStructure();
            }

            protected override void AfterSettingStructure()
            {
                AfterSettingStructureCalled = true;
            }
        }

        private class SimpleInputContext
            : InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleInputContext(
                SimpleStructureInput wrappedData,
                StructuresCalculation<SimpleStructureInput> calculation,
                IFailureMechanism failureMechanism,
                IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, failureMechanism, assessmentSection) {}
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(Action<SimpleStructuresInputProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            SimpleStructureInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestStructure();

            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new SimpleInputContext(input,
                                                      calculation,
                                                      failureMechanism,
                                                      assessmentSection);
            var properties = new SimpleStructuresInputProperties(inputContext, GetRandomConstructionProperties(), customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private static void AssertPropertiesInState(object properties, bool expectedReadOnly)
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(expectedReadOnly, dynamicProperties[1].IsReadOnly);
            Assert.AreEqual(expectedReadOnly, dynamicProperties[2].IsReadOnly);
        }
    }
}