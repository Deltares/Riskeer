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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StructuresInputBasePropertiesTest
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
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsInstanceOf<IHasStructureProperty<SimpleStructure>>(properties);
            Assert.IsInstanceOf<IHasForeshoreProfileProperty>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Constructor_ConstructionPropertiesIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleStructuresInputProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            SimpleStructureInput input = calculation.InputParameters;
            var expectedFailureProbabilityStructureWithErosion = ProbabilityFormattingHelper.Format(input.FailureProbabilityStructureWithErosion);

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

            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndNoStructure_ReturnsLocationsSortedById()
        {
            // Setup
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
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Call
            var availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl =>
                                                 new SelectableHydraulicBoundaryLocation(hbl, null))
                                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndStructure_ReturnsLocationsSortByDistanceThenById()
        {
            // Setup
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
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>()
            {
                InputParameters =
                {
                    Structure = new SimpleStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Call
            var availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl =>
                                                 new SelectableHydraulicBoundaryLocation(
                                                     hbl, calculation.InputParameters.Structure.Location))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingStructure_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
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
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>()
            {
                InputParameters =
                {
                    Structure = new SimpleStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

            // When
            properties.Structure = new SimpleStructure(new Point2D(0, 190));

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl =>
                                                 new SelectableHydraulicBoundaryLocation(hbl, properties.StructureLocation))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 5;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>
            {
                InputParameters =
                {
                    Structure = new SimpleStructure()
                }
            };
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            inputContext.Attach(observerMock);

            var random = new Random(100);
            double newStructureNormalOrientation = random.NextDouble();
            var newStructure = new SimpleStructure();
            ForeshoreProfile newForeshoreProfile = new TestForeshoreProfile();
            HydraulicBoundaryLocation newHydraulicBoundaryLocation = CreateHydraulicBoundaryLocation();
            var newSelectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newHydraulicBoundaryLocation, null);

            // Call
            properties.Structure = newStructure;
            properties.StructureNormalOrientation = (RoundedDouble) newStructureNormalOrientation;
            properties.FailureProbabilityStructureWithErosion = "1e-2";
            properties.SelectedHydraulicBoundaryLocation = newSelectableHydraulicBoundaryLocation;
            properties.ForeshoreProfile = newForeshoreProfile;

            // Assert
            Assert.AreSame(newStructure, properties.Structure);
            Assert.AreEqual(newStructureNormalOrientation, properties.StructureNormalOrientation, properties.StructureNormalOrientation.GetAccuracy());
            Assert.AreEqual(0.01, inputContext.WrappedData.FailureProbabilityStructureWithErosion);
            Assert.AreEqual("1/100", properties.FailureProbabilityStructureWithErosion);
            Assert.AreSame(newHydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreSame(newForeshoreProfile, properties.ForeshoreProfile);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_NullValue_AfterSettingStructureCalled()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Precondition
            Assert.IsFalse(properties.AfterSettingStructureCalled);

            // Call
            properties.Structure = null;

            // Assert
            Assert.IsTrue(properties.AfterSettingStructureCalled);
        }

        [Test]
        public void SetStructure_ValidValue_AfterSettingStructureCalled()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Precondition
            Assert.IsFalse(properties.AfterSettingStructureCalled);

            // Call
            properties.Structure = new SimpleStructure();

            // Assert
            Assert.IsTrue(properties.AfterSettingStructureCalled);
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityStructureWithErosion_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityStructureWithErosion_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityStructureWithErosion_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties())
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityStructureWithErosion = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructureInput>();
            var inputContext = new SimpleInputContext(calculation.InputParameters,
                                                      calculation,
                                                      failureMechanismStub,
                                                      assessmentSectionStub);
            var constructionProperties = GetRandomConstructionProperties();

            // Call
            var properties = new SimpleStructuresInputProperties(constructionProperties)
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(15, dynamicProperties.Count);

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
            Assert.IsFalse(structureNormalOrientationProperty.IsReadOnly);
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

            mockRepository.VerifyAll();
        }

        private StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties GetRandomConstructionProperties()
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
            var randomObjectLookup = new[]
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

            return new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties
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

        private class SimpleStructure : StructureBase
        {
            public SimpleStructure() : base("Name", "Id", new Point2D(0, 0), 0.0) {}

            public SimpleStructure(Point2D location) : base("Name", "Id", location, 0.0) {}
        }

        private class SimpleStructureInput : StructuresInputBase<SimpleStructure>
        {
            protected override void UpdateStructureParameters() {}
        }

        private class SimpleStructuresInputProperties : StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleStructuresInputProperties(ConstructionProperties constructionProperties) : base(constructionProperties) {}

            [Browsable(false)]
            public bool AfterSettingStructureCalled { get; private set; }

            public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
            {
                yield return new TestForeshoreProfile();
            }

            public override IEnumerable<SimpleStructure> GetAvailableStructures()
            {
                yield return new SimpleStructure();
            }

            protected override void AfterSettingStructure()
            {
                AfterSettingStructureCalled = true;
            }
        }

        private class SimpleInputContext : InputContextBase<SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleInputContext(SimpleStructureInput wrappedData, StructuresCalculation<SimpleStructureInput> calculation, IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, failureMechanism, assessmentSection) {}
        }
    }
}