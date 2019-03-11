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
            const string modelFactorsCategory = "Modelfactoren";

            const string schematizationClosureCategory = "Schematisering sluitproces";
            const string schematizationInflowCategory = "Schematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(23, dynamicProperties.Count);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[verticalWallModelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSuperCriticalFlowProperty,
                                                                            modelFactorsCategory,
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
                                                                            schematizationInflowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureNotClosingProperty = dynamicProperties[verticalWallLevelCrestStructureNotClosingPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureNotClosingProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureNotClosingProperty,
                                                                            schematizationInflowCategory,
                                                                            "Kruinhoogte niet gesloten kering [m+NAP]",
                                                                            "Niveau kruin bij niet gesloten maximaal kerende keermiddelen.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructureNotClosing, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[verticalWallFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationInflowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[verticalWallStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[verticalWallStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[verticalWallStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[verticalWallFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[verticalWallWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[verticalWallStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[verticalWallAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[verticalWallCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [-]", dynamicProperties[verticalWallFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[verticalWallForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[verticalWallUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[verticalWallUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[verticalWallHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[verticalWallStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[verticalWallCalculateIllustrationPointsPropertyIndex].DisplayName);

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
            const string schematizationClosureCategory = "Schematisering sluitproces";
            const string schematizationInflowCategory = "Schematisering instromend debiet/volume";

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
                                                                            schematizationInflowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[floodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationInflowCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[floodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(areaFlowAperturesProperty,
                                                                            schematizationInflowCategory,
                                                                            "Doorstroomoppervlak [m²]",
                                                                            "Doorstroomoppervlak van doorstroomopeningen.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[floodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(drainCoefficientProperty,
                                                                            schematizationInflowCategory,
                                                                            "Afvoercoëfficiënt [-]",
                                                                            "Afvoercoëfficiënt.",
                                                                            true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[floodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationInflowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[floodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[floodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[floodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[floodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[floodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[floodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [-]", dynamicProperties[floodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[floodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[floodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[floodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[floodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[floodedCulvertStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[floodedCulvertCalculateIllustrationPointsPropertyIndex].DisplayName);

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
            const string schematizationClosureCategory = "Schematisering sluitproces";
            const string schematizationInflowCategory = "Schematisering instromend debiet/volume";

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
                                                                            schematizationInflowCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[lowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationInflowCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[lowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(thresholdHeightOpenWeirProperty,
                                                                            schematizationInflowCategory,
                                                                            "Drempelhoogte [m+NAP]",
                                                                            "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                                                                            true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[lowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(factorStormDurationOpenStructureProperty,
                                                                            schematizationInflowCategory,
                                                                            "Factor voor stormduur hoogwater [-]",
                                                                            "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[lowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[lowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[lowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[lowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[lowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[lowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[lowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [-]", dynamicProperties[lowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[lowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[lowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[lowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[lowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[lowSillStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[lowSillCalculateIllustrationPointsPropertyIndex].DisplayName);

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

        #region Property indices

        #region VerticalWall structures indices

        private const int verticalWallHydraulicBoundaryLocationPropertyIndex = 0;
        private const int verticalWallStructurePropertyIndex = 1;
        private const int verticalWallStructureLocationPropertyIndex = 2;

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

        private const int lowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int lowSillStructurePropertyIndex = 1;
        private const int lowSillStructureLocationPropertyIndex = 2;

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

        private const int floodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int floodedCulvertStructurePropertyIndex = 1;
        private const int floodedCulvertStructureLocationPropertyIndex = 2;

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