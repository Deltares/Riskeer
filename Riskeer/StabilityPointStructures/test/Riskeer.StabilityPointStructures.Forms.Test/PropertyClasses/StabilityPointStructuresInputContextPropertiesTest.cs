﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses;

namespace Riskeer.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructuresInputContextPropertiesTest
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
            TestDelegate test = () => new StabilityPointStructuresInputContextProperties(null, handler);

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

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure()
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            TestDelegate test = () => new StabilityPointStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<StabilityPointStructure, StabilityPointStructuresInput, StructuresCalculation<StabilityPointStructuresInput>, StabilityPointStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            StabilityPointStructuresInput input = calculation.InputParameters;

            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreEqual(input.VolumicWeightWater, properties.VolumicWeightWater);
            Assert.AreSame(input.InsideWaterLevelFailureConstruction, properties.InsideWaterLevelFailureConstruction.Data);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);
            Assert.AreSame(input.FlowVelocityStructureClosable, properties.FlowVelocityStructureClosable.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreEqual(input.LoadSchematizationType, properties.LoadSchematizationType);
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreSame(input.ConstructiveStrengthLinearLoadModel, properties.ConstructiveStrengthLinearLoadModel.Data);
            Assert.AreSame(input.ConstructiveStrengthQuadraticLoadModel, properties.ConstructiveStrengthQuadraticLoadModel.Data);
            Assert.AreSame(input.StabilityLinearLoadModel, properties.StabilityLinearLoadModel.Data);
            Assert.AreSame(input.StabilityQuadraticLoadModel, properties.StabilityQuadraticLoadModel.Data);
            Assert.AreEqual(input.FailureProbabilityRepairClosure, properties.FailureProbabilityRepairClosure);
            Assert.AreSame(input.FailureCollisionEnergy, properties.FailureCollisionEnergy.Data);
            Assert.AreSame(input.ShipMass, properties.ShipMass.Data);
            Assert.AreSame(input.ShipVelocity, properties.ShipVelocity.Data);
            Assert.AreEqual(input.LevellingCount, properties.LevellingCount);
            Assert.AreEqual(input.ProbabilityCollisionSecondaryStructure, properties.ProbabilityCollisionSecondaryStructure);
            Assert.AreSame(input.BankWidth, properties.BankWidth.Data);
            Assert.AreEqual(input.EvaluationLevel, properties.EvaluationLevel);
            Assert.AreEqual(input.VerticalDistance, properties.VerticalDistance);
            Assert.AreEqual(input.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.FailureProbabilityStructureWithErosion));

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.FailureProbabilityRepairClosure));

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.ProbabilityCollisionSecondaryStructure));

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetGeneralPropertyIndices))]
        public void Constructor_VariousInflowType_GeneralPropertiesHaveExpectedAttributeValues(
            StabilityPointStructureInflowModelType inflowModelType,
            LoadSchematizationType loadSchematizationType,
            GeneralPropertyIndices generalPropertyIndices)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = loadSchematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string generalCategory = "\t\t\t\t\t\t\t\t\tAlgemeen";
            const string schematizationIncomingFlowCategory = "\t\t\t\t\t\tSchematisering instromend debiet/volume";
            const string schematizationGroundErosionCategory = "\t\t\t\t\tSchematisering bodembescherming";
            const string schematizationStorageStructureCategory = "\t\t\t\tSchematisering komberging";
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

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[generalPropertyIndices.StructureNormalOrientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureNormalOrientationProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.");

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
        public void Constructor_LinearLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string hydraulicLoadsCategory = "\t\t\t\t\t\t\t\tHydraulische belastingen op constructie";
            const string constructiveFailureCategory = "\t\t\t\t\t\t\tBezwijken waterkerende constructie-onderdelen";
            const string constructiveInstabilityCategory = "\t\t\tInstabiliteit constructie en grondlichaam";
            const string collisionSecondaryStructureCategory = "\t\tAanvaring tweede keermiddel";
            const string schematizationInflowCategory = "\t\t\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearLowSillVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicLoadsCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicLoadsCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                hydraulicLoadsCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearLowSillVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                hydraulicLoadsCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                constructiveFailureCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                constructiveFailureCategory,
                "Lineaire belastingschematisering constructieve sterkte [kN/m²]",
                "Kritieke sterkte constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearLowSillEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                constructiveFailureCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearLowSillFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                constructiveFailureCategory,
                "Faalkans herstel van gefaalde situatie [-]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationInflowCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationInflowCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                schematizationInflowCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationInflowCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearLowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                schematizationInflowCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearLowSillStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityLinearLoadModel,
                constructiveInstabilityCategory,
                "Lineaire belastingschematisering stabiliteit [kN/m²]",
                "Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                collisionSecondaryStructureCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                collisionSecondaryStructureCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                collisionSecondaryStructureCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearLowSillLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                collisionSecondaryStructureCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                collisionSecondaryStructureCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/nivellering]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                collisionSecondaryStructureCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string hydraulicLoadsCategory = "\t\t\t\t\t\t\t\tHydraulische belastingen op constructie";
            const string constructiveFailureCategory = "\t\t\t\t\t\t\tBezwijken waterkerende constructie-onderdelen";
            const string constructiveInstabilityCategory = "\t\t\tInstabiliteit constructie en grondlichaam";
            const string collisionSecondaryStructureCategory = "\t\tAanvaring tweede keermiddel";
            const string schematizationInflowCategory = "\t\t\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticLowSillVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicLoadsCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicLoadsCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                hydraulicLoadsCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticLowSillVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                hydraulicLoadsCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                constructiveFailureCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                constructiveFailureCategory,
                "Kwadratische belastingschematisering constructieve sterkte [kN/m]",
                "Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticLowSillEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                constructiveFailureCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticLowSillFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                constructiveFailureCategory,
                "Faalkans herstel van gefaalde situatie [-]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationInflowCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationInflowCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                schematizationInflowCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationInflowCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticLowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                schematizationInflowCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor stabilityQuadraticLoadModel = dynamicProperties[quadraticLowSillStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityQuadraticLoadModel,
                constructiveInstabilityCategory,
                "Kwadratische belastingschematisering stabiliteit [kN/m]",
                "Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                collisionSecondaryStructureCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                collisionSecondaryStructureCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                collisionSecondaryStructureCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticLowSillLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                collisionSecondaryStructureCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                collisionSecondaryStructureCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/nivellering]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                collisionSecondaryStructureCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LinearFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string hydraulicLoadsCategory = "\t\t\t\t\t\t\t\tHydraulische belastingen op constructie";
            const string constructiveFailureCategory = "\t\t\t\t\t\t\tBezwijken waterkerende constructie-onderdelen";
            const string constructiveInstabilityCategory = "\t\t\tInstabiliteit constructie en grondlichaam";
            const string collisionSecondaryStructureCategory = "\t\tAanvaring tweede keermiddel";
            const string schematizationInflowCategory = "\t\t\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(36, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearFloodedCulvertVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicLoadsCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicLoadsCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                hydraulicLoadsCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearFloodedCulvertVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                hydraulicLoadsCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                constructiveFailureCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                constructiveFailureCategory,
                "Lineaire belastingschematisering constructieve sterkte [kN/m²]",
                "Kritieke sterkte constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearFloodedCulvertEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                constructiveFailureCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                constructiveFailureCategory,
                "Faalkans herstel van gefaalde situatie [-]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationInflowCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationInflowCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                schematizationInflowCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationInflowCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[linearFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainCoefficientProperty,
                schematizationInflowCategory,
                "Afvoercoëfficiënt [-]",
                "Afvoercoëfficiënt.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.DrainCoefficient, false, false);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[linearFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                areaFlowAperturesProperty,
                schematizationInflowCategory,
                "Doorstroomoppervlak [m²]",
                "Doorstroomoppervlak van doorstroomopeningen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                schematizationInflowCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearFloodedCulvertStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityLinearLoadModel,
                constructiveInstabilityCategory,
                "Lineaire belastingschematisering stabiliteit [kN/m²]",
                "Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                collisionSecondaryStructureCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                collisionSecondaryStructureCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                collisionSecondaryStructureCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearFloodedCulvertLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                collisionSecondaryStructureCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                collisionSecondaryStructureCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/nivellering]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                collisionSecondaryStructureCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string hydraulicLoadsCategory = "\t\t\t\t\t\t\t\tHydraulische belastingen op constructie";
            const string constructiveFailureCategory = "\t\t\t\t\t\t\tBezwijken waterkerende constructie-onderdelen";
            const string constructiveInstabilityCategory = "\t\t\tInstabiliteit constructie en grondlichaam";
            const string collisionSecondaryStructureCategory = "\t\tAanvaring tweede keermiddel";
            const string schematizationInflowCategory = "\t\t\t\t\t\tSchematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(36, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticFloodedCulvertVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicLoadsCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicLoadsCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                hydraulicLoadsCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticFloodedCulvertVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                hydraulicLoadsCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                constructiveFailureCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                constructiveFailureCategory,
                "Kwadratische belastingschematisering constructieve sterkte [kN/m]",
                "Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticFloodedCulvertEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                constructiveFailureCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                constructiveFailureCategory,
                "Faalkans herstel van gefaalde situatie [-]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationInflowCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationInflowCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                schematizationInflowCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationInflowCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[quadraticFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainCoefficientProperty,
                schematizationInflowCategory,
                "Afvoercoëfficiënt [-]",
                "Afvoercoëfficiënt.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.DrainCoefficient, false, false);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[quadraticFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                areaFlowAperturesProperty,
                schematizationInflowCategory,
                "Doorstroomoppervlak [m²]",
                "Doorstroomoppervlak van doorstroomopeningen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                schematizationInflowCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityLinearLoadModel,
                constructiveInstabilityCategory,
                "Kwadratische belastingschematisering stabiliteit [kN/m]",
                "Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                collisionSecondaryStructureCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                collisionSecondaryStructureCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                collisionSecondaryStructureCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticFloodedCulvertLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                collisionSecondaryStructureCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                collisionSecondaryStructureCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/nivellering]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                collisionSecondaryStructureCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutStructure_CorrectReadOnlyForStructureDependentProperties()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            const int structureNormalOrientationPropertyIndex = 10;
            PropertyDescriptor structureNormalOrientation = dynamicProperties[structureNormalOrientationPropertyIndex];
            Assert.IsTrue(structureNormalOrientation.IsReadOnly);

            PropertyDescriptor inflowModelType = dynamicProperties[inflowModelTypePropertyIndex];
            Assert.IsTrue(inflowModelType.IsReadOnly);

            PropertyDescriptor loadSchematizationType = dynamicProperties[loadSchematizationTypePropertyIndex];
            Assert.IsTrue(loadSchematizationType.IsReadOnly);

            PropertyDescriptor levellingCount = dynamicProperties[levellingCountPropertyIndex];
            Assert.IsTrue(levellingCount.IsReadOnly);

            PropertyDescriptor evaluationLevel = dynamicProperties[evaluationLevelPropertyIndex];
            Assert.IsTrue(evaluationLevel.IsReadOnly);

            PropertyDescriptor verticalDistance = dynamicProperties[verticalDistancePropertyIndex];
            Assert.IsTrue(verticalDistance.IsReadOnly);

            PropertyDescriptor failureProbabilityRepairClosure = dynamicProperties[failureProbabilityRepairClosurePropertyIndex];
            Assert.IsTrue(failureProbabilityRepairClosure.IsReadOnly);

            PropertyDescriptor probabilityCollisionSecondaryStructure = dynamicProperties[probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsTrue(probabilityCollisionSecondaryStructure.IsReadOnly);

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthQuadraticLoadModel, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityQuadraticLoadModel, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, true, true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, true, true);
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, "path");
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

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

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                new TestStabilityPointStructure()
            }, "path/to/structures");
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<StabilityPointStructure> availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.StabilityPointStructures, availableStructures);
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

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            inputContext.Attach(observer);

            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call
            properties.ShouldIllustrationPointsBeCalculated = newBoolean;

            // Assert
            Assert.AreEqual(newBoolean, calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            mockRepository.VerifyAll();
        }

        [Test]
        public void VolumicWeightWater_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble height = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.VolumicWeightWater = height);
        }

        [Test]
        public void FactorStormDurationOpenStructure_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble factor = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FactorStormDurationOpenStructure = factor);
        }

        [Test]
        public void InflowModelType_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var type = new Random(21).NextEnumValue<StabilityPointStructureInflowModelType>();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.InflowModelType = type);
        }

        [Test]
        public void FailureProbabilityOpenStructure_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var type = new Random(21).NextEnumValue<LoadSchematizationType>();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LoadSchematizationType = type);
        }

        [Test]
        public void FailureProbabilityRepairClosure_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FailureProbabilityRepairClosure = random.NextDouble());
        }

        [Test]
        public void LevellingCount_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            int count = new Random(21).Next();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LevellingCount = count);
        }

        [Test]
        public void ProbabilityCollisionSecondaryStructure_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var random = new Random(21);
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ProbabilityCollisionSecondaryStructure = random.NextDouble());
        }

        [Test]
        public void EvaluationLevel_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble evaluationLevel = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.EvaluationLevel = evaluationLevel);
        }

        [Test]
        public void VerticalDistance_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble verticalDistance = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.VerticalDistance = verticalDistance);
        }

        [Test]
        public void WidthFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.WidthFlowApertures.Mean = newMean);
        }

        [Test]
        public void InsideWaterLevelFailureConstruction_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.InsideWaterLevelFailureConstruction.Mean = newMean);
        }

        [Test]
        public void InsideWaterLevel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.InsideWaterLevel.Mean = newMean);
        }

        [Test]
        public void DrainCoefficient_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.DrainCoefficient.Mean = newMean);
        }

        [Test]
        public void LevelCrestStructure_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LevelCrestStructure.Mean = newMean);
        }

        [Test]
        public void ThresholdHeightOpenWeir_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ThresholdHeightOpenWeir.Mean = newMean);
        }

        [Test]
        public void FlowVelocityStructureClosable_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FlowVelocityStructureClosable.Mean = newMean);
        }

        [Test]
        public void AreaFlowApertures_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.AreaFlowApertures.Mean = newMean);
        }

        [Test]
        public void ConstructiveStrengthLinearLoadModel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ConstructiveStrengthLinearLoadModel.Mean = newMean);
        }

        [Test]
        public void ConstructiveStrengthQuadraticLoadModel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ConstructiveStrengthQuadraticLoadModel.Mean = newMean);
        }

        [Test]
        public void StabilityLinearLoadModel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.StabilityLinearLoadModel.Mean = newMean);
        }

        [Test]
        public void StabilityQuadraticLoadModel_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.StabilityQuadraticLoadModel.Mean = newMean);
        }

        [Test]
        public void FailureCollisionEnergy_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.FailureCollisionEnergy.Mean = newMean);
        }

        [Test]
        public void ShipMass_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ShipMass.Mean = newMean);
        }

        [Test]
        public void ShipVelocity_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ShipVelocity.Mean = newMean);
        }

        [Test]
        public void BankWidth_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.BankWidth.Mean = newMean);
        }

        [Test]
        public void Structure_Always_InputChangedAndObservablesNotified()
        {
            var structure = new TestStabilityPointStructure();
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
        public void DynamicVisibleValidationMethod_StructureTypeUnknown_ReturnExpectedValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibleValidationMethod_LowSillStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibleValidationMethod_FloodedCulvertStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibleValidationMethod_LinearModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.ConstructiveStrengthLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.StabilityLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibleValidationMethod_QuadraticModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.ConstructiveStrengthLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.StabilityLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(StabilityPointStructuresInputContextProperties.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        private void SetPropertyAndVerifyNotificationsAndOutput(Action<StabilityPointStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            StabilityPointStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestStabilityPointStructure();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetGeneralPropertyIndices()
        {
            yield return new TestCaseData(StabilityPointStructureInflowModelType.LowSill,
                                          LoadSchematizationType.Linear,
                                          new GeneralPropertyIndices(linearLowSillStructurePropertyIndex, linearLowSillStructureLocationPropertyIndex, linearLowSillHydraulicBoundaryLocationPropertyIndex,
                                                                     linearLowSillStructureNormalOrientationPropertyIndex, linearLowSillFlowWidthAtBottomProtectionPropertyIndex, linearLowSillWidthFlowAperturesPropertyIndex,
                                                                     linearLowSillStorageStructureAreaPropertyIndex, linearLowSillAllowedLevelIncreaseStoragePropertyIndex, linearLowSillCriticalOvertoppingDischargePropertyIndex,
                                                                     linearLowSillFailureProbabilityStructureWithErosionPropertyIndex, linearLowSillForeshoreProfilePropertyIndex,
                                                                     linearLowSillUseBreakWaterPropertyIndex, linearLowSillUseForeshorePropertyIndex, linearLowSillStormDurationPropertyIndex,
                                                                     linearLowSillCalculateIllustrationPointsPropertyIndex));

            yield return new TestCaseData(StabilityPointStructureInflowModelType.LowSill,
                                          LoadSchematizationType.Quadratic,
                                          new GeneralPropertyIndices(quadraticLowSillStructurePropertyIndex, quadraticLowSillStructureLocationPropertyIndex, quadraticLowSillHydraulicBoundaryLocationPropertyIndex,
                                                                     quadraticLowSillStructureNormalOrientationPropertyIndex, quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex, quadraticLowSillWidthFlowAperturesPropertyIndex,
                                                                     quadraticLowSillStorageStructureAreaPropertyIndex, quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex, quadraticLowSillCriticalOvertoppingDischargePropertyIndex,
                                                                     quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex, quadraticLowSillForeshoreProfilePropertyIndex,
                                                                     quadraticLowSillUseBreakWaterPropertyIndex, quadraticLowSillUseForeshorePropertyIndex, quadraticLowSillStormDurationPropertyIndex,
                                                                     quadraticLowSillCalculateIllustrationPointsPropertyIndex));

            yield return new TestCaseData(StabilityPointStructureInflowModelType.FloodedCulvert,
                                          LoadSchematizationType.Linear,
                                          new GeneralPropertyIndices(linearFloodedCulvertStructurePropertyIndex, linearFloodedCulvertStructureLocationPropertyIndex, linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex,
                                                                     linearFloodedCulvertStructureNormalOrientationPropertyIndex, linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex, null,
                                                                     linearFloodedCulvertStorageStructureAreaPropertyIndex, linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex, linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex,
                                                                     linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex, linearFloodedCulvertForeshoreProfilePropertyIndex,
                                                                     linearFloodedCulvertUseBreakWaterPropertyIndex, linearFloodedCulvertUseForeshorePropertyIndex, linearFloodedCulvertStormDurationPropertyIndex,
                                                                     linearFloodedCulvertCalculateIllustrationPointsPropertyIndex));

            yield return new TestCaseData(StabilityPointStructureInflowModelType.FloodedCulvert,
                                          LoadSchematizationType.Quadratic,
                                          new GeneralPropertyIndices(quadraticFloodedCulvertStructurePropertyIndex, quadraticFloodedCulvertStructureLocationPropertyIndex, quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex,
                                                                     quadraticFloodedCulvertStructureNormalOrientationPropertyIndex, quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex, null,
                                                                     quadraticFloodedCulvertStorageStructureAreaPropertyIndex, quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex, quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex,
                                                                     quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex, quadraticFloodedCulvertForeshoreProfilePropertyIndex,
                                                                     quadraticFloodedCulvertUseBreakWaterPropertyIndex, quadraticFloodedCulvertUseForeshorePropertyIndex, quadraticFloodedCulvertStormDurationPropertyIndex,
                                                                     quadraticFloodedCulvertCalculateIllustrationPointsPropertyIndex));
        }

        public class GeneralPropertyIndices
        {
            public GeneralPropertyIndices(int structurePropertyIndex, int structureLocationPropertyIndex, int hydraulicBoundaryLocationPropertyIndex,
                                          int structureNormalOrientationPropertyIndex,
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
            public int StructureNormalOrientationPropertyIndex { get; }
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

        #region LowSill + Linear Model property Indices

        private const int linearLowSillStructurePropertyIndex = 0;
        private const int linearLowSillStructureLocationPropertyIndex = 1;
        private const int linearLowSillHydraulicBoundaryLocationPropertyIndex = 2;

        private const int linearLowSillVolumicWeightWaterPropertyIndex = 3;
        private const int linearLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearLowSillBankWidthPropertyIndex = 5;
        private const int linearLowSillVerticalDistancePropertyIndex = 6;

        private const int linearLowSillLoadSchematizationTypePropertyIndex = 7;
        private const int linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex = 8;
        private const int linearLowSillEvaluationLevelPropertyIndex = 9;
        private const int linearLowSillFailureProbabilityRepairClosurePropertyIndex = 10;

        private const int linearLowSillInflowModelTypePropertyIndex = 11;
        private const int linearLowSillStructureNormalOrientationPropertyIndex = 12;
        private const int linearLowSillLevelCrestStructurePropertyIndex = 13;
        private const int linearLowSillInsideWaterLevelPropertyIndex = 14;
        private const int linearLowSillThresholdHeightOpenWeirPropertyIndex = 15;
        private const int linearLowSillWidthFlowAperturesPropertyIndex = 16;
        private const int linearLowSillStormDurationPropertyIndex = 17;
        private const int linearLowSillFactorStormDurationOpenStructurePropertyIndex = 18;

        private const int linearLowSillCriticalOvertoppingDischargePropertyIndex = 19;
        private const int linearLowSillFlowWidthAtBottomProtectionPropertyIndex = 20;
        private const int linearLowSillFailureProbabilityStructureWithErosionPropertyIndex = 21;

        private const int linearLowSillStorageStructureAreaPropertyIndex = 22;
        private const int linearLowSillAllowedLevelIncreaseStoragePropertyIndex = 23;

        private const int linearLowSillStabilityLinearLoadModelPropertyIndex = 24;

        private const int linearLowSillFailureCollisionEnergyPropertyIndex = 25;
        private const int linearLowSillShipMassPropertyIndex = 26;
        private const int linearLowSillShipVelocityPropertyIndex = 27;
        private const int linearLowSillLevellingCountPropertyIndex = 28;
        private const int linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linearLowSillFlowVelocityStructureClosablePropertyIndex = 30;

        private const int linearLowSillForeshoreProfilePropertyIndex = 31;
        private const int linearLowSillUseBreakWaterPropertyIndex = 32;
        private const int linearLowSillUseForeshorePropertyIndex = 33;

        private const int linearLowSillCalculateIllustrationPointsPropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Linear Model property Indices

        private const int linearFloodedCulvertStructurePropertyIndex = 0;
        private const int linearFloodedCulvertStructureLocationPropertyIndex = 1;
        private const int linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 2;

        private const int linearFloodedCulvertVolumicWeightWaterPropertyIndex = 3;
        private const int linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearFloodedCulvertBankWidthPropertyIndex = 5;
        private const int linearFloodedCulvertVerticalDistancePropertyIndex = 6;

        private const int linearFloodedCulvertLoadSchematizationTypePropertyIndex = 7;
        private const int linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex = 8;
        private const int linearFloodedCulvertEvaluationLevelPropertyIndex = 9;
        private const int linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 10;

        private const int linearFloodedCulvertInflowModelTypePropertyIndex = 11;
        private const int linearFloodedCulvertStructureNormalOrientationPropertyIndex = 12;
        private const int linearFloodedCulvertLevelCrestStructurePropertyIndex = 13;
        private const int linearFloodedCulvertInsideWaterLevelPropertyIndex = 14;
        private const int linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 15;
        private const int linearFloodedCulvertAreaFlowAperturesPropertyIndex = 16;
        private const int linearFloodedCulvertDrainCoefficientPropertyIndex = 17;
        private const int linearFloodedCulvertStormDurationPropertyIndex = 18;
        private const int linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 19;

        private const int linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 20;
        private const int linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 21;
        private const int linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 22;

        private const int linearFloodedCulvertStorageStructureAreaPropertyIndex = 23;
        private const int linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 24;

        private const int linearFloodedCulvertStabilityLinearLoadModelPropertyIndex = 25;

        private const int linearFloodedCulvertFailureCollisionEnergyPropertyIndex = 26;
        private const int linearFloodedCulvertShipMassPropertyIndex = 27;
        private const int linearFloodedCulvertShipVelocityPropertyIndex = 28;
        private const int linearFloodedCulvertLevellingCountPropertyIndex = 29;
        private const int linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 30;
        private const int linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 31;

        private const int linearFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int linearFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int linearFloodedCulvertUseForeshorePropertyIndex = 34;

        private const int linearFloodedCulvertCalculateIllustrationPointsPropertyIndex = 35;

        #endregion

        #region LowSill + Quadratic Model property Indices

        private const int quadraticLowSillStructurePropertyIndex = 0;
        private const int quadraticLowSillStructureLocationPropertyIndex = 1;
        private const int quadraticLowSillHydraulicBoundaryLocationPropertyIndex = 2;

        private const int quadraticLowSillVolumicWeightWaterPropertyIndex = 3;
        private const int quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticLowSillBankWidthPropertyIndex = 5;
        private const int quadraticLowSillVerticalDistancePropertyIndex = 6;

        private const int quadraticLowSillLoadSchematizationTypePropertyIndex = 7;
        private const int quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex = 8;
        private const int quadraticLowSillEvaluationLevelPropertyIndex = 9;
        private const int quadraticLowSillFailureProbabilityRepairClosurePropertyIndex = 10;

        private const int quadraticLowSillInflowModelTypePropertyIndex = 11;
        private const int quadraticLowSillStructureNormalOrientationPropertyIndex = 12;
        private const int quadraticLowSillLevelCrestStructurePropertyIndex = 13;
        private const int quadraticLowSillInsideWaterLevelPropertyIndex = 14;
        private const int quadraticLowSillThresholdHeightOpenWeirPropertyIndex = 15;
        private const int quadraticLowSillWidthFlowAperturesPropertyIndex = 16;
        private const int quadraticLowSillStormDurationPropertyIndex = 17;
        private const int quadraticLowSillFactorStormDurationOpenStructurePropertyIndex = 18;

        private const int quadraticLowSillCriticalOvertoppingDischargePropertyIndex = 19;
        private const int quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex = 20;
        private const int quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex = 21;

        private const int quadraticLowSillStorageStructureAreaPropertyIndex = 22;
        private const int quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex = 23;

        private const int quadraticLowSillStabilityQuadraticLoadModelPropertyIndex = 24;

        private const int quadraticLowSillFailureCollisionEnergyPropertyIndex = 25;
        private const int quadraticLowSillShipMassPropertyIndex = 26;
        private const int quadraticLowSillShipVelocityPropertyIndex = 27;
        private const int quadraticLowSillLevellingCountPropertyIndex = 28;
        private const int quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadraticLowSillFlowVelocityStructureClosablePropertyIndex = 30;

        private const int quadraticLowSillForeshoreProfilePropertyIndex = 31;
        private const int quadraticLowSillUseBreakWaterPropertyIndex = 32;
        private const int quadraticLowSillUseForeshorePropertyIndex = 33;

        private const int quadraticLowSillCalculateIllustrationPointsPropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Quadratic Model property Indices

        private const int quadraticFloodedCulvertStructurePropertyIndex = 0;
        private const int quadraticFloodedCulvertStructureLocationPropertyIndex = 1;
        private const int quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 2;

        private const int quadraticFloodedCulvertVolumicWeightWaterPropertyIndex = 3;
        private const int quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticFloodedCulvertBankWidthPropertyIndex = 5;
        private const int quadraticFloodedCulvertVerticalDistancePropertyIndex = 6;

        private const int quadraticFloodedCulvertLoadSchematizationTypePropertyIndex = 7;
        private const int quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex = 8;
        private const int quadraticFloodedCulvertEvaluationLevelPropertyIndex = 9;
        private const int quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 10;

        private const int quadraticFloodedCulvertInflowModelTypePropertyIndex = 11;
        private const int quadraticFloodedCulvertStructureNormalOrientationPropertyIndex = 12;
        private const int quadraticFloodedCulvertLevelCrestStructurePropertyIndex = 13;
        private const int quadraticFloodedCulvertInsideWaterLevelPropertyIndex = 14;
        private const int quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 15;
        private const int quadraticFloodedCulvertAreaFlowAperturesPropertyIndex = 16;
        private const int quadraticFloodedCulvertDrainCoefficientPropertyIndex = 17;
        private const int quadraticFloodedCulvertStormDurationPropertyIndex = 18;
        private const int quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 19;

        private const int quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 20;
        private const int quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 21;
        private const int quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 22;

        private const int quadraticFloodedCulvertStorageStructureAreaPropertyIndex = 23;
        private const int quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 24;

        private const int quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex = 25;

        private const int quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex = 26;
        private const int quadraticFloodedCulvertShipMassPropertyIndex = 27;
        private const int quadraticFloodedCulvertShipVelocityPropertyIndex = 28;
        private const int quadraticFloodedCulvertLevellingCountPropertyIndex = 29;
        private const int quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 30;
        private const int quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 31;

        private const int quadraticFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int quadraticFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int quadraticFloodedCulvertUseForeshorePropertyIndex = 34;

        private const int quadraticFloodedCulvertCalculateIllustrationPointsPropertyIndex = 35;

        #endregion

        #region No structure property Indices

        private const int verticalDistancePropertyIndex = 6;
        private const int loadSchematizationTypePropertyIndex = 7;
        private const int evaluationLevelPropertyIndex = 10;
        private const int failureProbabilityRepairClosurePropertyIndex = 11;
        private const int inflowModelTypePropertyIndex = 12;
        private const int levellingCountPropertyIndex = 32;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 31;

        #endregion
    }
}