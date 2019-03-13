// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PropertyClasses;

namespace Riskeer.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresInputContextPropertiesTest
    {
        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int hydraulicBoundaryLocationPropertyIndex = 2;

        private const int modelFactorSuperCriticalFlowPropertyIndex = 3;

        private const int structureNormalOrientationPropertyIndex = 4;
        private const int levelCrestStructurePropertyIndex = 5;
        private const int widthFlowAperturesPropertyIndex = 6;
        private const int stormDurationPropertyIndex = 7;

        private const int criticalOvertoppingDischargePropertyIndex = 8;
        private const int flowWidthAtBottomProtectionPropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;

        private const int storageStructureAreaPropertyIndex = 11;
        private const int allowedLevelIncreaseStoragePropertyIndex = 12;

        private const int foreshoreProfilePropertyIndex = 13;
        private const int useBreakWaterPropertyIndex = 14;
        private const int useForeshorePropertyIndex = 15;

        private const int calculateIllustrationPointsPropertyIndex = 16;

        private MockRepository mockRepository;
        private IAssessmentSection assessmentSection;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            assessmentSection = mockRepository.Stub<IAssessmentSection>();
        }

        [Test]
        public void Constructor_WithoutData_ThrowsArgumentNullException()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new HeightStructuresInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            TestDelegate test = () => new HeightStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    Structure = new TestHeightStructure(),
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<HeightStructure, HeightStructuresInput, StructuresCalculation<HeightStructuresInput>,
                HeightStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            HeightStructuresInput input = calculation.InputParameters;

            Assert.AreSame(input.Structure, properties.Structure);
            Assert.AreEqual(input.Structure.Location, properties.StructureLocation);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);

            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);

            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);
            Assert.AreSame(input.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.AreSame(input.StormDuration, properties.StormDuration.Data);

            Assert.AreSame(input.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.AreSame(input.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.AreEqual(input.FailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);

            Assert.AreSame(input.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.AreSame(input.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);

            Assert.AreSame(input.ForeshoreProfile, properties.ForeshoreProfile);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.UseBreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.UseForeshore);

            Assert.AreEqual(input.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);

            TestHelper.AssertTypeConverter<HeightStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(HeightStructuresInputContextProperties.FailureProbabilityStructureWithErosion));

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ModelFactorSuperCriticalFlow, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string generalDataCategory = "\t\t\t\t\t\tBasisgegevens";
            const string modelFactorsCategory = "\t\t\t\t\tModelfactoren";
            const string schematizationIncomingFlowCategory = "\t\t\t\tSchematisering instromend debiet/volume";
            const string schematizationGroundErosionCategory = "\t\t\tSchematisering bodembescherming";
            const string schematizationStorageStructureCategory = "\t\tSchematisering komberging";
            const string foreshoreCategory = "\tSchematisering voorland en (haven)dam";
            const string outputSettingsCategory = "Uitvoer";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(17, dynamicProperties.Count);

            PropertyDescriptor structureProperty = dynamicProperties[structurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureProperty,
                                                                            generalDataCategory,
                                                                            "Kunstwerk",
                                                                            "Het kunstwerk dat gebruikt wordt in de berekening.");

            PropertyDescriptor structureLocationProperty = dynamicProperties[structureLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureLocationProperty,
                                                                            generalDataCategory,
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hydraulicBoundaryLocationProperty,
                                                                            generalDataCategory,
                                                                            "Hydraulische belastingenlocatie",
                                                                            "De hydraulische belastingenlocatie.");

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSuperCriticalFlowProperty,
                                                                            modelFactorsCategory,
                                                                            "Modelfactor overloopdebiet volkomen overlaat [-]",
                                                                            "Modelfactor voor het overloopdebiet over een volkomen overlaat.",
                                                                            true);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureNormalOrientationProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.",
                                                                            true);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Kerende hoogte [m+NAP]",
                                                                            "Kerende hoogte van het kunstwerk.",
                                                                            true);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[widthFlowAperturesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthFlowAperturesProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Breedte van doorstroomopening [m]",
                                                                            "Breedte van de doorstroomopening.",
                                                                            true);

            PropertyDescriptor stormDurationProperty = dynamicProperties[stormDurationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stormDurationProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Stormduur [uur]",
                                                                            "Stormduur.",
                                                                            true);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalOvertoppingDischargeProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Kritiek instromend debiet [m³/s/m]",
                                                                            "Kritiek instromend debiet directe invoer per strekkende meter.",
                                                                            true);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(flowWidthAtBottomProtectionProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Stroomvoerende breedte bodembescherming [m]",
                                                                            "Stroomvoerende breedte bodembescherming.",
                                                                            true);

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityStructureWithErosionProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Faalkans gegeven erosie bodem [-]",
                                                                            "Faalkans kunstwerk gegeven erosie bodem.",
                                                                            true);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(storageStructureAreaProperty,
                                                                            schematizationStorageStructureCategory,
                                                                            "Kombergend oppervlak [m²]",
                                                                            "Kombergend oppervlak.",
                                                                            true);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[allowedLevelIncreaseStoragePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(allowedLevelIncreaseStorageProperty,
                                                                            schematizationStorageStructureCategory,
                                                                            "Toegestane peilverhoging komberging [m]",
                                                                            "Toegestane peilverhoging komberging.",
                                                                            true);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[foreshoreProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProfileProperty,
                                                                            foreshoreCategory,
                                                                            "Voorlandprofiel",
                                                                            "De schematisatie van het voorlandprofiel.");

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[useBreakWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            foreshoreCategory,
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[useForeshorePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useForeshoreProperty,
                                                                            foreshoreCategory,
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor shouldIllustrationPointsBeCalculatedProperty = dynamicProperties[calculateIllustrationPointsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(shouldIllustrationPointsBeCalculatedProperty,
                                                                            outputSettingsCategory,
                                                                            "Illustratiepunten inlezen",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithOrWithoutStructure_CorrectReadOnlyForStructureDependentProperties(bool hasStructure)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            if (hasStructure)
            {
                calculation.InputParameters.Structure = new TestHeightStructure();
            }

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            bool expectedReadOnly = !hasStructure;

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex];
            Assert.AreEqual(expectedReadOnly, failureProbabilityStructureWithErosionProperty.IsReadOnly);

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, expectedReadOnly, expectedReadOnly);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, "path");

            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<ForeshoreProfile> availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableStructures_SetInputContextInstanceWithStructures_ReturnStructures()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                new TestHeightStructure()
            }, "some folder");
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<HeightStructure> availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.HeightStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            var newStructure = new TestHeightStructure();
            var handler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            FailureMechanismSection failureMechanismSection =
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(-10.0, -10.0),
                    new Point2D(10.0, 10.0)
                });
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection
            });
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            properties.Structure = newStructure;

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetShouldIllustrationPointsBeCalculated_ValueChanged_UpdateDataAndNotifyObservers()
        {
            // Setup
            var random = new Random(21);
            bool newBoolean = random.NextBoolean();

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            inputContext.Attach(observer);

            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            properties.ShouldIllustrationPointsBeCalculated = newBoolean;

            // Assert
            Assert.AreEqual(newBoolean, calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Structure_Always_InputChangedAndObservablesNotified()
        {
            var structure = new TestHeightStructure();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.Structure = structure);
        }

        [Test]
        public void StructureNormalOrientation_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble orientation = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.StructureNormalOrientation = orientation);
        }

        [Test]
        public void FailureProbabilityStructureWithErosion_Always_InputChangedAndObservablesNotified()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FailureProbabilityStructureWithErosion = random.NextDouble());
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_Always_InputChangedAndObservablesNotified()
        {
            var location = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), null);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.SelectedHydraulicBoundaryLocation = location);
        }

        [Test]
        public void ForeshoreProfile_Always_InputChangedAndObservablesNotified()
        {
            var profile = new TestForeshoreProfile();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ForeshoreProfile = profile);
        }

        [Test]
        public void UseBreakWater_Always_InputChangedAndObservablesNotified()
        {
            bool useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.UseBreakWater.UseBreakWater = useBreakWater);
        }

        [Test]
        public void UseForeshore_Always_InputChangedAndObservablesNotified()
        {
            bool useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.UseForeshore.UseForeshore = useForeshore);
        }

        [Test]
        public void FlowWidthAtBottomProtection_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FlowWidthAtBottomProtection.Mean = newMean);
        }

        [Test]
        public void WidthFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.WidthFlowApertures.Mean = newMean);
        }

        [Test]
        public void StorageStructureArea_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.StorageStructureArea.Mean = newMean);
        }

        [Test]
        public void AllowedLevelIncreaseStorage_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.AllowedLevelIncreaseStorage.Mean = newMean);
        }

        [Test]
        public void CriticalOvertoppingDischarge_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.CriticalOvertoppingDischarge.Mean = newMean);
        }

        [Test]
        public void StormDuration_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.StormDuration.Mean = newMean);
        }

        [Test]
        public void ModelFactorSuperCriticalFlow_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ModelFactorSuperCriticalFlow.Mean = newMean);
        }

        [Test]
        public void LevelCrestStructure_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LevelCrestStructure.Mean = newMean);
        }

        private void SetPropertyAndVerifyNotificationsAndOutput(Action<HeightStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            HeightStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestHeightStructure();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new HeightStructuresInputContext(input,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}