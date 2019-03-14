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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresInputContextPropertiesTest
    {
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
            TestDelegate test = () => new ClosingStructuresInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            TestDelegate test = () => new ClosingStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall),
                    ForeshoreProfile = new TestForeshoreProfile(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput,
                StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            ClosingStructuresInput input = calculation.InputParameters;

            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreEqual(input.IdenticalApertures, properties.IdenticalApertures);
            Assert.AreSame(input.LevelCrestStructureNotClosing, properties.LevelCrestStructureNotClosing.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreEqual(input.ProbabilityOpenStructureBeforeFlooding, properties.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(input.FailureProbabilityOpenStructure, properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(input.FailureProbabilityReparation, properties.FailureProbabilityReparation);
            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);

            Assert.AreSame(input.Structure, properties.Structure);
            Assert.AreEqual(input.Structure.Location.X, properties.StructureLocation.X, 1);
            Assert.AreEqual(input.Structure.Location.Y, properties.StructureLocation.Y, 1);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);

            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.AreSame(input.StormDuration, properties.StormDuration.Data);

            Assert.AreSame(input.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.AreSame(input.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.AreEqual(input.FailureProbabilityStructureWithErosion, properties.FailureProbabilityStructureWithErosion);

            Assert.AreSame(input.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.AreSame(input.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);
            Assert.AreEqual(input.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);

            Assert.AreSame(input.ForeshoreProfile, properties.ForeshoreProfile);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.UseBreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.UseForeshore);

            Assert.AreEqual(input.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);

            TestHelper.AssertTypeConverter<ClosingStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(ClosingStructuresInputContextProperties.FailureProbabilityStructureWithErosion));

            TestHelper.AssertTypeConverter<ClosingStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(ClosingStructuresInputContextProperties.FailureProbabilityOpenStructure));

            TestHelper.AssertTypeConverter<ClosingStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(ClosingStructuresInputContextProperties.ProbabilityOpenStructureBeforeFlooding));

            TestHelper.AssertTypeConverter<ClosingStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(ClosingStructuresInputContextProperties.FailureProbabilityReparation));

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ModelFactorSuperCriticalFlow, false, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.DrainCoefficient, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetGeneralPropertyIndices))]
        public void Constructor_VariousInflowType_GeneralPropertiesHaveExpectedAttributeValues(
            ClosingStructureInflowModelType inflowModelType,
            GeneralPropertyIndices generalPropertyIndices)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(inflowModelType)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string generalCategory = "\t\t\t\t\t\t\tAlgemeen";
            const string schematizationIncomingFlowCategory = "\t\t\t\tSchematisering instromend debiet/volume";
            const string schematizationGroundErosionCategory = "\t\t\tSchematisering bodembescherming";
            const string schematizationStorageStructureCategory = "\t\tSchematisering komberging";
            const string foreshoreCategory = "\tVoorland en (haven)dam";
            const string outputSettingsCategory = "Uitvoer";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor structureProperty = dynamicProperties[generalPropertyIndices.StructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureProperty,
                                                                            generalCategory,
                                                                            "Kunstwerk",
                                                                            "Het kunstwerk dat gebruikt wordt in de berekening.");

            PropertyDescriptor structureLocationProperty = dynamicProperties[generalPropertyIndices.StructureLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureLocationProperty,
                                                                            generalCategory,
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[generalPropertyIndices.HydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hydraulicBoundaryLocationProperty,
                                                                            generalCategory,
                                                                            "Hydraulische belastingenlocatie",
                                                                            "De hydraulische belastingenlocatie.");

            if (generalPropertyIndices.StructureNormalOrientationPropertyIndex.HasValue)
            {
                PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[generalPropertyIndices.StructureNormalOrientationPropertyIndex.Value];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureNormalOrientationProperty,
                                                                                schematizationIncomingFlowCategory,
                                                                                "Oriëntatie [°]",
                                                                                "Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.");
            }

            if (generalPropertyIndices.WidthFlowAperturesPropertyIndex.HasValue)
            {
                PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[generalPropertyIndices.WidthFlowAperturesPropertyIndex.Value];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthFlowAperturesProperty,
                                                                                schematizationIncomingFlowCategory,
                                                                                "Breedte van doorstroomopening [m]",
                                                                                "Breedte van de doorstroomopening.",
                                                                                true);
            }

            PropertyDescriptor stormDurationProperty = dynamicProperties[generalPropertyIndices.StormDurationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stormDurationProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Stormduur [uur]",
                                                                            "Stormduur.",
                                                                            true);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[generalPropertyIndices.CriticalOverToppingDischargePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalOvertoppingDischargeProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Kritiek instromend debiet [m³/s/m]",
                                                                            "Kritiek instromend debiet directe invoer per strekkende meter.",
                                                                            true);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[generalPropertyIndices.FlowWidthAtBottomProtectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(flowWidthAtBottomProtectionProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Stroomvoerende breedte bodembescherming [m]",
                                                                            "Stroomvoerende breedte bodembescherming.",
                                                                            true);

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[generalPropertyIndices.FailureProbabilityStructureWithErosionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityStructureWithErosionProperty,
                                                                            schematizationGroundErosionCategory,
                                                                            "Faalkans gegeven erosie bodem [-]",
                                                                            "Faalkans kunstwerk gegeven erosie bodem.");

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[generalPropertyIndices.StorageStructureAreaPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(storageStructureAreaProperty,
                                                                            schematizationStorageStructureCategory,
                                                                            "Kombergend oppervlak [m²]",
                                                                            "Kombergend oppervlak.",
                                                                            true);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[generalPropertyIndices.AllowedLevelIncreaseStoragePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(allowedLevelIncreaseStorageProperty,
                                                                            schematizationStorageStructureCategory,
                                                                            "Toegestane peilverhoging komberging [m]",
                                                                            "Toegestane peilverhoging komberging.",
                                                                            true);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[generalPropertyIndices.ForeShoreProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProfileProperty,
                                                                            foreshoreCategory,
                                                                            "Voorlandprofiel",
                                                                            "De schematisatie van het voorlandprofiel.");

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[generalPropertyIndices.UseBreakWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            foreshoreCategory,
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[generalPropertyIndices.UseForeshorePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useForeshoreProperty,
                                                                            foreshoreCategory,
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor shouldIllustrationPointsBeCalculatedProperty = dynamicProperties[generalPropertyIndices.ShouldIllustrationPointsBeCalculatedPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(shouldIllustrationPointsBeCalculatedProperty,
                                                                            outputSettingsCategory,
                                                                            "Illustratiepunten inlezen",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");
        }

        [Test]
        public void Constructor_VerticalWallStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string modelSettingsCategory = "\t\t\t\t\t\tModelinstellingen";

            const string schematizationClosureCategory = "\t\t\t\t\tSchematisering sluitproces";
            const string schematizationIncomingFlowCategory = "\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(23, dynamicProperties.Count);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[verticalWallModelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSuperCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor overloopdebiet volkomen overlaat [-]",
                                                                            "Modelfactor voor het overloopdebiet over een volkomen overlaat.",
                                                                            true);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[verticalWallIdenticalAperturesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(identicalAperturesProperty,
                                                                            schematizationClosureCategory,
                                                                            "Aantal identieke doorstroomopeningen [-]",
                                                                            "Aantal identieke doorstroomopeningen.");

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[verticalWallFailureProbabilityOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityOpenStructureProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans mislukken sluiting [-]",
                                                                            "Kans op mislukken sluiting van geopend kunstwerk.");

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[verticalWallProbabilityOpenStructureBeforeFloodingPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityOpenStructureBeforeFloodingProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans op open staan bij naderend hoogwater [-]",
                                                                            "Kans op open staan bij naderend hoogwater.");

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[verticalWallFailureProbabilityReparationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityReparationProperty,
                                                                            schematizationClosureCategory,
                                                                            "Faalkans herstel van gefaalde situatie [-]",
                                                                            "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[verticalWallInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inflowModelTypeProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureNotClosingProperty = dynamicProperties[verticalWallLevelCrestStructureNotClosingPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureNotClosingProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureNotClosingProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Kruinhoogte niet gesloten kering [m+NAP]",
                                                                            "Niveau kruin bij niet gesloten maximaal kerende keermiddelen.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructureNotClosing, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[verticalWallFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_FloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.FloodedCulvert)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationClosureCategory = "\t\t\t\t\tSchematisering sluitproces";
            const string schematizationIncomingFlowCategory = "\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(22, dynamicProperties.Count);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[floodedCulvertIdenticalAperturesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(identicalAperturesProperty,
                                                                            schematizationClosureCategory,
                                                                            "Aantal identieke doorstroomopeningen [-]",
                                                                            "Aantal identieke doorstroomopeningen.");

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[floodedCulvertFailureProbabilityOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityOpenStructureProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans mislukken sluiting [-]",
                                                                            "Kans op mislukken sluiting van geopend kunstwerk.");

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[floodedCulvertProbabilityOpenStructureBeforeFloodingPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityOpenStructureBeforeFloodingProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans op open staan bij naderend hoogwater [-]",
                                                                            "Kans op open staan bij naderend hoogwater.");

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[floodedCulvertFailureProbabilityReparationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityReparationProperty,
                                                                            schematizationClosureCategory,
                                                                            "Faalkans herstel van gefaalde situatie [-]",
                                                                            "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[floodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inflowModelTypeProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[floodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[floodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(areaFlowAperturesProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Doorstroomoppervlak [m²]",
                                                                            "Doorstroomoppervlak van doorstroomopeningen.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[floodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(drainCoefficientProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Afvoercoëfficiënt [-]",
                                                                            "Afvoercoëfficiënt.",
                                                                            true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[floodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.LowSill)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationClosureCategory = "\t\t\t\t\tSchematisering sluitproces";
            const string schematizationIncomingFlowCategory = "\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(22, dynamicProperties.Count);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[lowSillIdenticalAperturesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(identicalAperturesProperty,
                                                                            schematizationClosureCategory,
                                                                            "Aantal identieke doorstroomopeningen [-]",
                                                                            "Aantal identieke doorstroomopeningen.");

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[lowSillFailureProbabilityOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityOpenStructureProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans mislukken sluiting [-]",
                                                                            "Kans op mislukken sluiting van geopend kunstwerk.");

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[lowSillProbabilityOpenStructureBeforeFloodingPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityOpenStructureBeforeFloodingProperty,
                                                                            schematizationClosureCategory,
                                                                            "Kans op open staan bij naderend hoogwater [-]",
                                                                            "Kans op open staan bij naderend hoogwater.");

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[lowSillFailureProbabilityReparationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityReparationProperty,
                                                                            schematizationClosureCategory,
                                                                            "Faalkans herstel van gefaalde situatie [-]",
                                                                            "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[lowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inflowModelTypeProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[lowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[lowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(thresholdHeightOpenWeirProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Drempelhoogte [m+NAP]",
                                                                            "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[lowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutStructure_CorrectReadOnlyForStructureDependentProperties()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor inflowModelType = dynamicProperties[inflowModelTypePropertyIndex];
            Assert.IsTrue(inflowModelType.IsReadOnly);

            PropertyDescriptor identicalApertures = dynamicProperties[identicalAperturesPropertyIndex];
            Assert.IsTrue(identicalApertures.IsReadOnly);

            PropertyDescriptor probabilityOpenStructureBeforeFlooding = dynamicProperties[probabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsTrue(probabilityOpenStructureBeforeFlooding.IsReadOnly);

            PropertyDescriptor failureProbabilityOpenStructure = dynamicProperties[failureProbabilityOpenStructurePropertyIndex];
            Assert.IsTrue(failureProbabilityOpenStructure.IsReadOnly);

            PropertyDescriptor failureProbabilityReparation = dynamicProperties[failureProbabilityReparationPropertyIndex];
            Assert.IsTrue(failureProbabilityReparation.IsReadOnly);

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructureNotClosing, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, true, true);
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, "path");

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                new TestClosingStructure()
            }, "some path");
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<ClosingStructure> availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.ClosingStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void FactorStormDurationOpenStructure_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble factor = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FactorStormDurationOpenStructure = factor);
        }

        [Test]
        public void InflowModelType_Always_InputChangedAndObservablesNotified()
        {
            var inflowModelType = new Random(21).NextEnumValue<ClosingStructureInflowModelType>();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.InflowModelType = inflowModelType);
        }

        [Test]
        public void ProbabilityOpenStructureBeforeFlooding_Always_InputChangedAndObservablesNotified()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ProbabilityOpenStructureBeforeFlooding = random.NextDouble());
        }

        [Test]
        public void FailureProbabilityOpenStructure_Always_InputChangedAndObservablesNotified()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FailureProbabilityOpenStructure = random.NextDouble());
        }

        [Test]
        public void FailureProbabilityReparation_Always_InputChangedAndObservablesNotified()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FailureProbabilityReparation = random.NextDouble());
        }

        [Test]
        public void IdenticalApertures_Always_InputChangedAndObservablesNotified()
        {
            int propertiesIdenticalApertures = new Random(21).Next();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.IdenticalApertures = propertiesIdenticalApertures);
        }

        [Test]
        public void InsideWaterLevel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.InsideWaterLevel.Mean = newMean);
        }

        [Test]
        public void WidthFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.WidthFlowApertures.Mean = newMean);
        }

        [Test]
        public void ThresholdHeightOpenWeir_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ThresholdHeightOpenWeir.Mean = newMean);
        }

        [Test]
        public void AreaFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.AreaFlowApertures.Mean = newMean);
        }

        [Test]
        public void ModelFactorSuperCriticalFlow_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ModelFactorSuperCriticalFlow.Mean = newMean);
        }

        [Test]
        public void DrainCoefficient_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.DrainCoefficient.Mean = newMean);
        }

        [Test]
        public void LevelCrestStructureNotClosing_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LevelCrestStructureNotClosing.Mean = newMean);
        }

        [Test]
        public void Structure_Always_InputChangedAndObservablesNotified()
        {
            var structure = new TestClosingStructure();
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
        public void Structure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);

            var newStructure = new TestClosingStructure();
            var handler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section", new[]
                {
                    new Point2D(-10.0, -10.0),
                    new Point2D(10.0, 10.0)
                })
            });
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            properties.Structure = newStructure;

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsVerticalWall_ReturnExpectedVisibility()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsLowSill_ReturnExpectedVisibility()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.LowSill)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsFloodedCulvert_ReturnExpectedVisibility()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.FloodedCulvert)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureTypeUnknown_ReturnExpectedVisibility()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        private void SetPropertyAndVerifyNotificationsAndOutput(Action<ClosingStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestClosingStructure();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetGeneralPropertyIndices()
        {
            yield return new TestCaseData(ClosingStructureInflowModelType.LowSill, new GeneralPropertyIndices(lowSillStructurePropertyIndex, lowSillStructureLocationPropertyIndex, lowSillHydraulicBoundaryLocationPropertyIndex,
                                                                                                              null, lowSillFlowWidthAtBottomProtectionPropertyIndex, lowSillWidthFlowAperturesPropertyIndex,
                                                                                                              lowSillStorageStructureAreaPropertyIndex, lowSillAllowedLevelIncreaseStoragePropertyIndex, lowSillCriticalOvertoppingDischargePropertyIndex,
                                                                                                              lowSillFailureProbabilityStructureWithErosionPropertyIndex, lowSillForeshoreProfilePropertyIndex,
                                                                                                              lowSillUseBreakWaterPropertyIndex, lowSillUseForeshorePropertyIndex, lowSillStormDurationPropertyIndex,
                                                                                                              lowSillCalculateIllustrationPointsPropertyIndex));
            yield return new TestCaseData(ClosingStructureInflowModelType.FloodedCulvert, new GeneralPropertyIndices(floodedCulvertStructurePropertyIndex, floodedCulvertStructureLocationPropertyIndex, floodedCulvertHydraulicBoundaryLocationPropertyIndex,
                                                                                                                     null, floodedCulvertFlowWidthAtBottomProtectionPropertyIndex, null,
                                                                                                                     floodedCulvertStorageStructureAreaPropertyIndex, floodedCulvertAllowedLevelIncreaseStoragePropertyIndex, floodedCulvertCriticalOvertoppingDischargePropertyIndex,
                                                                                                                     floodedCulvertFailureProbabilityStructureWithErosionPropertyIndex, floodedCulvertForeshoreProfilePropertyIndex,
                                                                                                                     floodedCulvertUseBreakWaterPropertyIndex, floodedCulvertUseForeshorePropertyIndex, floodedCulvertStormDurationPropertyIndex,
                                                                                                                     floodedCulvertCalculateIllustrationPointsPropertyIndex));
            yield return new TestCaseData(ClosingStructureInflowModelType.VerticalWall, new GeneralPropertyIndices(verticalWallStructurePropertyIndex, verticalWallStructureLocationPropertyIndex, verticalWallHydraulicBoundaryLocationPropertyIndex,
                                                                                                                   verticalWallStructureNormalOrientationPropertyIndex, verticalWallFlowWidthAtBottomProtectionPropertyIndex, verticalWallWidthFlowAperturesPropertyIndex,
                                                                                                                   verticalWallStorageStructureAreaPropertyIndex, verticalWallAllowedLevelIncreaseStoragePropertyIndex, verticalWallCriticalOvertoppingDischargePropertyIndex,
                                                                                                                   verticalWallFailureProbabilityStructureWithErosionPropertyIndex, verticalWallForeshoreProfilePropertyIndex,
                                                                                                                   verticalWallUseBreakWaterPropertyIndex, verticalWallUseForeshorePropertyIndex, verticalWallStormDurationPropertyIndex,
                                                                                                                   verticalWallCalculateIllustrationPointsPropertyIndex));
        }

        public class GeneralPropertyIndices
        {
            public GeneralPropertyIndices(int structurePropertyIndex, int structureLocationPropertyIndex, int hydraulicBoundaryLocationPropertyIndex,
                                          int? structureNormalOrientationPropertyIndex,
                                          int flowWidthAtBottomProtectionPropertyIndex, int? widthFlowAperturesPropertyIndex,
                                          int storageStructureAreaPropertyIndex, int allowedLevelIncreaseStoragePropertyIndex,
                                          int criticalOverToppingDischargePropertyIndex, int failureProbabilityStructureWithErosionPropertyIndex,
                                          int foreShoreProfilePropertyIndex, int useBreakWaterPropertyIndex, int useForeshorePropertyIndex,
                                          int stormDurationPropertyIndex, int shouldIllustrationPointsBeCalculatedPropertyIndex)
            {
                StructurePropertyIndex = structurePropertyIndex;
                StructureLocationPropertyIndex = structureLocationPropertyIndex;
                HydraulicBoundaryLocationPropertyIndex = hydraulicBoundaryLocationPropertyIndex;
                StructureNormalOrientationPropertyIndex = structureNormalOrientationPropertyIndex;
                FlowWidthAtBottomProtectionPropertyIndex = flowWidthAtBottomProtectionPropertyIndex;
                WidthFlowAperturesPropertyIndex = widthFlowAperturesPropertyIndex;
                StorageStructureAreaPropertyIndex = storageStructureAreaPropertyIndex;
                AllowedLevelIncreaseStoragePropertyIndex = allowedLevelIncreaseStoragePropertyIndex;
                CriticalOverToppingDischargePropertyIndex = criticalOverToppingDischargePropertyIndex;
                FailureProbabilityStructureWithErosionPropertyIndex = failureProbabilityStructureWithErosionPropertyIndex;
                ForeShoreProfilePropertyIndex = foreShoreProfilePropertyIndex;
                UseBreakWaterPropertyIndex = useBreakWaterPropertyIndex;
                UseForeshorePropertyIndex = useForeshorePropertyIndex;
                StormDurationPropertyIndex = stormDurationPropertyIndex;
                ShouldIllustrationPointsBeCalculatedPropertyIndex = shouldIllustrationPointsBeCalculatedPropertyIndex;
            }

            public int StructurePropertyIndex { get; }
            public int StructureLocationPropertyIndex { get; }
            public int HydraulicBoundaryLocationPropertyIndex { get; }
            public int? StructureNormalOrientationPropertyIndex { get; }
            public int FlowWidthAtBottomProtectionPropertyIndex { get; }
            public int? WidthFlowAperturesPropertyIndex { get; }
            public int StorageStructureAreaPropertyIndex { get; }
            public int AllowedLevelIncreaseStoragePropertyIndex { get; }
            public int CriticalOverToppingDischargePropertyIndex { get; }
            public int FailureProbabilityStructureWithErosionPropertyIndex { get; }
            public int ForeShoreProfilePropertyIndex { get; }
            public int UseBreakWaterPropertyIndex { get; }
            public int UseForeshorePropertyIndex { get; }
            public int StormDurationPropertyIndex { get; }
            public int ShouldIllustrationPointsBeCalculatedPropertyIndex { get; }
        }

        #region Property indices

        #region VerticalWall structures indices

        private const int verticalWallStructurePropertyIndex = 0;
        private const int verticalWallStructureLocationPropertyIndex = 1;
        private const int verticalWallHydraulicBoundaryLocationPropertyIndex = 2;

        private const int verticalWallModelFactorSuperCriticalFlowPropertyIndex = 3;

        private const int verticalWallIdenticalAperturesPropertyIndex = 4;
        private const int verticalWallFailureProbabilityOpenStructurePropertyIndex = 5;
        private const int verticalWallProbabilityOpenStructureBeforeFloodingPropertyIndex = 6;
        private const int verticalWallFailureProbabilityReparationPropertyIndex = 7;

        private const int verticalWallInflowModelTypePropertyIndex = 8;
        private const int verticalWallStructureNormalOrientationPropertyIndex = 9;
        private const int verticalWallLevelCrestStructureNotClosingPropertyIndex = 10;
        private const int verticalWallWidthFlowAperturesPropertyIndex = 11;
        private const int verticalWallStormDurationPropertyIndex = 12;
        private const int verticalWallFactorStormDurationOpenStructurePropertyIndex = 13;

        private const int verticalWallCriticalOvertoppingDischargePropertyIndex = 14;
        private const int verticalWallFlowWidthAtBottomProtectionPropertyIndex = 15;
        private const int verticalWallFailureProbabilityStructureWithErosionPropertyIndex = 16;

        private const int verticalWallStorageStructureAreaPropertyIndex = 17;
        private const int verticalWallAllowedLevelIncreaseStoragePropertyIndex = 18;

        private const int verticalWallForeshoreProfilePropertyIndex = 19;
        private const int verticalWallUseBreakWaterPropertyIndex = 20;
        private const int verticalWallUseForeshorePropertyIndex = 21;

        private const int verticalWallCalculateIllustrationPointsPropertyIndex = 22;

        #endregion

        #region LowSill structures indices

        private const int lowSillStructurePropertyIndex = 0;
        private const int lowSillStructureLocationPropertyIndex = 1;
        private const int lowSillHydraulicBoundaryLocationPropertyIndex = 2;

        private const int lowSillIdenticalAperturesPropertyIndex = 3;
        private const int lowSillFailureProbabilityOpenStructurePropertyIndex = 4;
        private const int lowSillProbabilityOpenStructureBeforeFloodingPropertyIndex = 5;
        private const int lowSillFailureProbabilityReparationPropertyIndex = 6;

        private const int lowSillInflowModelTypePropertyIndex = 7;
        private const int lowSillInsideWaterLevelPropertyIndex = 8;
        private const int lowSillThresholdHeightOpenWeirPropertyIndex = 9;
        private const int lowSillWidthFlowAperturesPropertyIndex = 10;
        private const int lowSillStormDurationPropertyIndex = 11;
        private const int lowSillFactorStormDurationOpenStructurePropertyIndex = 12;

        private const int lowSillCriticalOvertoppingDischargePropertyIndex = 13;
        private const int lowSillFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int lowSillFailureProbabilityStructureWithErosionPropertyIndex = 15;

        private const int lowSillStorageStructureAreaPropertyIndex = 16;
        private const int lowSillAllowedLevelIncreaseStoragePropertyIndex = 17;

        private const int lowSillForeshoreProfilePropertyIndex = 18;
        private const int lowSillUseBreakWaterPropertyIndex = 19;
        private const int lowSillUseForeshorePropertyIndex = 20;

        private const int lowSillCalculateIllustrationPointsPropertyIndex = 21;

        #endregion

        #region FloodedCulvert structures indices

        private const int floodedCulvertStructurePropertyIndex = 0;
        private const int floodedCulvertStructureLocationPropertyIndex = 1;
        private const int floodedCulvertHydraulicBoundaryLocationPropertyIndex = 2;

        private const int floodedCulvertIdenticalAperturesPropertyIndex = 3;
        private const int floodedCulvertFailureProbabilityOpenStructurePropertyIndex = 4;
        private const int floodedCulvertProbabilityOpenStructureBeforeFloodingPropertyIndex = 5;
        private const int floodedCulvertFailureProbabilityReparationPropertyIndex = 6;

        private const int floodedCulvertInflowModelTypePropertyIndex = 7;
        private const int floodedCulvertInsideWaterLevelPropertyIndex = 8;
        private const int floodedCulvertAreaFlowAperturesPropertyIndex = 9;
        private const int floodedCulvertDrainCoefficientPropertyIndex = 10;
        private const int floodedCulvertStormDurationPropertyIndex = 11;
        private const int floodedCulvertFactorStormDurationOpenStructurePropertyIndex = 12;

        private const int floodedCulvertCriticalOvertoppingDischargePropertyIndex = 13;
        private const int floodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int floodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 15;

        private const int floodedCulvertStorageStructureAreaPropertyIndex = 16;
        private const int floodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 17;

        private const int floodedCulvertForeshoreProfilePropertyIndex = 18;
        private const int floodedCulvertUseBreakWaterPropertyIndex = 19;
        private const int floodedCulvertUseForeshorePropertyIndex = 20;

        private const int floodedCulvertCalculateIllustrationPointsPropertyIndex = 21;

        #endregion

        #region No structure property indices

        private const int identicalAperturesPropertyIndex = 4;
        private const int failureProbabilityOpenStructurePropertyIndex = 5;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 6;
        private const int failureProbabilityReparationPropertyIndex = 7;
        private const int inflowModelTypePropertyIndex = 8;

        #endregion

        #endregion
    }
}