// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
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

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.FailureProbabilityStructureWithErosion));

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.FailureProbabilityRepairClosure));

            TestHelper.AssertTypeConverter<StabilityPointStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresInputContextProperties.ProbabilityCollisionSecondaryStructure));

            mockRepository.VerifyAll();
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
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearLowSillVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicDataCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                hydraulicDataCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicDataCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                schematizationCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearLowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                modelSettingsCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                schematizationCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                schematizationCategory,
                "Lineaire belastingschematisering constructieve sterkte [kN/m²]",
                "Kritieke sterkte constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                schematizationCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearLowSillEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                schematizationCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearLowSillVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                schematizationCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearLowSillFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                schematizationCategory,
                "Faalkans herstel van gefaalde situatie [1/jaar]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                schematizationCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                schematizationCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                schematizationCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearLowSillLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                schematizationCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                schematizationCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearLowSillStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityLinearLoadModel,
                schematizationCategory,
                "Lineaire belastingschematisering stabiliteit [kN/m²]",
                "Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linearLowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linearLowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linearLowSillStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linearLowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[linearLowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linearLowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linearLowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linearLowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linearLowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linearLowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linearLowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linearLowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[linearLowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linearLowSillStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[linearLowSillCalculateIllustrationPointsPropertyIndex].DisplayName);

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
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticLowSillVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicDataCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                hydraulicDataCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicDataCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                schematizationCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticLowSillFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                modelSettingsCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                schematizationCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthQuadraticLoadModelProperty,
                schematizationCategory,
                "Kwadratische belastingschematisering constructieve sterkte [kN/m]",
                "Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthQuadraticLoadModel, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                schematizationCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticLowSillEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                schematizationCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticLowSillVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                schematizationCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticLowSillFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                schematizationCategory,
                "Faalkans herstel van gefaalde situatie [1/jaar]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                schematizationCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                schematizationCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                schematizationCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticLowSillLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                schematizationCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                schematizationCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadraticLowSillStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityQuadraticLoadModelProperty,
                schematizationCategory,
                "Kwadratische belastingschematisering stabiliteit [kN/m]",
                "Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityQuadraticLoadModel, false, false);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadraticLowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadraticLowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadraticLowSillStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[quadraticLowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadraticLowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadraticLowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadraticLowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadraticLowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadraticLowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[quadraticLowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadraticLowSillStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[quadraticLowSillCalculateIllustrationPointsPropertyIndex].DisplayName);

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
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(36, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearFloodedCulvertVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicDataCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                hydraulicDataCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicDataCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                schematizationCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[linearFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainCoefficientProperty,
                modelSettingsCategory,
                "Afvoercoëfficiënt [-]",
                "Afvoercoëfficiënt.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.DrainCoefficient, false, true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                modelSettingsCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                schematizationCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[linearFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                areaFlowAperturesProperty,
                schematizationCategory,
                "Doorstroomoppervlak [m²]",
                "Doorstroomoppervlak van doorstroomopeningen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthLinearLoadModelProperty,
                schematizationCategory,
                "Lineaire belastingschematisering constructieve sterkte [kN/m²]",
                "Kritieke sterkte constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthLinearLoadModel, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                schematizationCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearFloodedCulvertEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                schematizationCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearFloodedCulvertVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                schematizationCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                schematizationCategory,
                "Faalkans herstel van gefaalde situatie [1/jaar]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                schematizationCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                schematizationCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                schematizationCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearFloodedCulvertLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                schematizationCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                schematizationCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearFloodedCulvertStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityLinearLoadModel,
                schematizationCategory,
                "Lineaire belastingschematisering stabiliteit [kN/m²]",
                "Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityLinearLoadModel, false, false);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linearFloodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linearFloodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linearFloodedCulvertStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linearFloodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linearFloodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linearFloodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linearFloodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linearFloodedCulvertStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[linearFloodedCulvertCalculateIllustrationPointsPropertyIndex].DisplayName);

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
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(36, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticFloodedCulvertVolumicWeightWaterPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                volumicWeightWaterProperty,
                hydraulicDataCategory,
                "Volumiek gewicht van water [kN/m³]",
                "Volumiek gewicht van water.");

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelProperty,
                hydraulicDataCategory,
                "Binnenwaterstand [m+NAP]",
                "Binnenwaterstand.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevel, false, false);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                insideWaterLevelFailureConstructionProperty,
                hydraulicDataCategory,
                "Binnenwaterstand bij constructief falen [m+NAP]",
                "Binnenwaterstand bij constructief falen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.InsideWaterLevelFailureConstruction, false, false);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                flowVelocityStructureClosableProperty,
                schematizationCategory,
                "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FlowVelocityStructureClosable, false, true);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[quadraticFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainCoefficientProperty,
                modelSettingsCategory,
                "Afvoercoëfficiënt [-]",
                "Afvoercoëfficiënt.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.DrainCoefficient, false, true);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                factorStormDurationOpenStructureProperty,
                modelSettingsCategory,
                "Factor voor stormduur hoogwater [-]",
                "Factor voor stormduur hoogwater gegeven geopend kunstwerk.");

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                inflowModelTypeProperty,
                schematizationCategory,
                "Instroommodel",
                "Instroommodel van het kunstwerk.");

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                loadSchematizationTypeProperty,
                schematizationCategory,
                "Belastingschematisering",
                "Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.");

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[quadraticFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                areaFlowAperturesProperty,
                schematizationCategory,
                "Doorstroomoppervlak [m²]",
                "Doorstroomoppervlak van doorstroomopeningen.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.AreaFlowApertures, false, false);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levelCrestStructureProperty,
                schematizationCategory,
                "Kerende hoogte [m+NAP]",
                "Kerende hoogte van het kunstwerk.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, false, false);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thresholdHeightOpenWeirProperty,
                schematizationCategory,
                "Drempelhoogte [m+NAP]",
                "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ThresholdHeightOpenWeir, false, false);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                constructiveStrengthQuadraticLoadModelProperty,
                schematizationCategory,
                "Kwadratische belastingschematisering constructieve sterkte [kN/m]",
                "Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ConstructiveStrengthQuadraticLoadModel, false, false);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                bankWidthProperty,
                schematizationCategory,
                "Bermbreedte [m]",
                "Bermbreedte.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.BankWidth, false, false);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticFloodedCulvertEvaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                evaluationLevelProperty,
                schematizationCategory,
                "Analysehoogte [m+NAP]",
                "Hoogte waarop de constructieve sterkte wordt beoordeeld.");

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticFloodedCulvertVerticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                verticalDistanceProperty,
                schematizationCategory,
                "Afstand onderkant wand en teen van de dijk/berm [m]",
                "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.");

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureProbabilityRepairClosureProperty,
                schematizationCategory,
                "Faalkans herstel van gefaalde situatie [1/jaar]",
                "Faalkans herstel van gefaalde situatie.");

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                failureCollisionEnergyProperty,
                schematizationCategory,
                "Bezwijkwaarde aanvaarenergie [kN m]",
                "Bezwijkwaarde aanvaarenergie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.FailureCollisionEnergy, false, false);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipMassProperty,
                schematizationCategory,
                "Massa van het schip [ton]",
                "Massa van het schip.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipMass, false, false);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                shipVelocityProperty,
                schematizationCategory,
                "Aanvaarsnelheid [m/s]",
                "Aanvaarsnelheid.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ShipVelocity, false, false);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticFloodedCulvertLevellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                levellingCountProperty,
                schematizationCategory,
                "Aantal nivelleringen per jaar [1/jaar]",
                "Aantal nivelleringen per jaar.");

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                probabilityCollisionSecondaryStructureProperty,
                schematizationCategory,
                "Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]",
                "Kans op aanvaring tweede keermiddel per nivellering.");

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stabilityQuadraticLoadModelProperty,
                schematizationCategory,
                "Kwadratische belastingschematisering stabiliteit [kN/m]",
                "Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.",
                true);
            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.StabilityQuadraticLoadModel, false, false);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadraticFloodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadraticFloodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadraticFloodedCulvertStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadraticFloodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadraticFloodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadraticFloodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadraticFloodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadraticFloodedCulvertStormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[quadraticFloodedCulvertCalculateIllustrationPointsPropertyIndex].DisplayName);

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
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var newStructure = new TestStabilityPointStructure();
            var handler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new StabilityPointStructuresInputContextProperties(inputContext, handler);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
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

        #region LowSill + Linear Model property Indices

        private const int linearLowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int linearLowSillVolumicWeightWaterPropertyIndex = 1;
        private const int linearLowSillStormDurationPropertyIndex = 2;
        private const int linearLowSillInsideWaterLevelPropertyIndex = 3;
        private const int linearLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearLowSillFactorStormDurationOpenStructurePropertyIndex = 5;
        private const int linearLowSillStructurePropertyIndex = 6;
        private const int linearLowSillStructureLocationPropertyIndex = 7;
        private const int linearLowSillStructureNormalOrientationPropertyIndex = 8;
        private const int linearLowSillInflowModelTypePropertyIndex = 9;
        private const int linearLowSillLoadSchematizationTypePropertyIndex = 10;
        private const int linearLowSillWidthFlowAperturesPropertyIndex = 11;
        private const int linearLowSillFlowWidthAtBottomProtectionPropertyIndex = 12;
        private const int linearLowSillStorageStructureAreaPropertyIndex = 13;
        private const int linearLowSillAllowedLevelIncreaseStoragePropertyIndex = 14;
        private const int linearLowSillLevelCrestStructurePropertyIndex = 15;
        private const int linearLowSillThresholdHeightOpenWeirPropertyIndex = 16;
        private const int linearLowSillCriticalOvertoppingDischargePropertyIndex = 17;
        private const int linearLowSillFlowVelocityStructureClosablePropertyIndex = 18;
        private const int linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex = 19;
        private const int linearLowSillBankWidthPropertyIndex = 20;
        private const int linearLowSillEvaluationLevelPropertyIndex = 21;
        private const int linearLowSillVerticalDistancePropertyIndex = 22;
        private const int linearLowSillFailureProbabilityRepairClosurePropertyIndex = 23;
        private const int linearLowSillFailureCollisionEnergyPropertyIndex = 24;
        private const int linearLowSillShipMassPropertyIndex = 25;
        private const int linearLowSillShipVelocityPropertyIndex = 26;
        private const int linearLowSillLevellingCountPropertyIndex = 27;
        private const int linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 28;
        private const int linearLowSillStabilityLinearLoadModelPropertyIndex = 29;
        private const int linearLowSillFailureProbabilityStructureWithErosionPropertyIndex = 30;
        private const int linearLowSillForeshoreProfilePropertyIndex = 31;
        private const int linearLowSillUseBreakWaterPropertyIndex = 32;
        private const int linearLowSillUseForeshorePropertyIndex = 33;
        private const int linearLowSillCalculateIllustrationPointsPropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Linear Model property Indices

        private const int linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int linearFloodedCulvertVolumicWeightWaterPropertyIndex = 1;
        private const int linearFloodedCulvertStormDurationPropertyIndex = 2;
        private const int linearFloodedCulvertInsideWaterLevelPropertyIndex = 3;
        private const int linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearFloodedCulvertDrainCoefficientPropertyIndex = 5;
        private const int linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 6;
        private const int linearFloodedCulvertStructurePropertyIndex = 7;
        private const int linearFloodedCulvertStructureLocationPropertyIndex = 8;
        private const int linearFloodedCulvertStructureNormalOrientationPropertyIndex = 9;
        private const int linearFloodedCulvertInflowModelTypePropertyIndex = 10;
        private const int linearFloodedCulvertLoadSchematizationTypePropertyIndex = 11;
        private const int linearFloodedCulvertAreaFlowAperturesPropertyIndex = 12;
        private const int linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 13;
        private const int linearFloodedCulvertStorageStructureAreaPropertyIndex = 14;
        private const int linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 15;
        private const int linearFloodedCulvertLevelCrestStructurePropertyIndex = 16;
        private const int linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 17;
        private const int linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 18;
        private const int linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 19;
        private const int linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex = 20;
        private const int linearFloodedCulvertBankWidthPropertyIndex = 21;
        private const int linearFloodedCulvertEvaluationLevelPropertyIndex = 22;
        private const int linearFloodedCulvertVerticalDistancePropertyIndex = 23;
        private const int linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int linearFloodedCulvertFailureCollisionEnergyPropertyIndex = 25;
        private const int linearFloodedCulvertShipMassPropertyIndex = 26;
        private const int linearFloodedCulvertShipVelocityPropertyIndex = 27;
        private const int linearFloodedCulvertLevellingCountPropertyIndex = 28;
        private const int linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linearFloodedCulvertStabilityLinearLoadModelPropertyIndex = 30;
        private const int linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int linearFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int linearFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int linearFloodedCulvertUseForeshorePropertyIndex = 34;
        private const int linearFloodedCulvertCalculateIllustrationPointsPropertyIndex = 35;

        #endregion

        #region LowSill + Quadratic Model property Indices

        private const int quadraticLowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadraticLowSillVolumicWeightWaterPropertyIndex = 1;
        private const int quadraticLowSillStormDurationPropertyIndex = 2;
        private const int quadraticLowSillInsideWaterLevelPropertyIndex = 3;
        private const int quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticLowSillFactorStormDurationOpenStructurePropertyIndex = 5;
        private const int quadraticLowSillStructurePropertyIndex = 6;
        private const int quadraticLowSillStructureLocationPropertyIndex = 7;
        private const int quadraticLowSillStructureNormalOrientationPropertyIndex = 8;
        private const int quadraticLowSillInflowModelTypePropertyIndex = 9;
        private const int quadraticLowSillLoadSchematizationTypePropertyIndex = 10;
        private const int quadraticLowSillWidthFlowAperturesPropertyIndex = 11;
        private const int quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex = 12;
        private const int quadraticLowSillStorageStructureAreaPropertyIndex = 13;
        private const int quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex = 14;
        private const int quadraticLowSillLevelCrestStructurePropertyIndex = 15;
        private const int quadraticLowSillThresholdHeightOpenWeirPropertyIndex = 16;
        private const int quadraticLowSillCriticalOvertoppingDischargePropertyIndex = 17;
        private const int quadraticLowSillFlowVelocityStructureClosablePropertyIndex = 18;
        private const int quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex = 19;
        private const int quadraticLowSillBankWidthPropertyIndex = 20;
        private const int quadraticLowSillEvaluationLevelPropertyIndex = 21;
        private const int quadraticLowSillVerticalDistancePropertyIndex = 22;
        private const int quadraticLowSillFailureProbabilityRepairClosurePropertyIndex = 23;
        private const int quadraticLowSillFailureCollisionEnergyPropertyIndex = 24;
        private const int quadraticLowSillShipMassPropertyIndex = 25;
        private const int quadraticLowSillShipVelocityPropertyIndex = 26;
        private const int quadraticLowSillLevellingCountPropertyIndex = 27;
        private const int quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 28;
        private const int quadraticLowSillStabilityQuadraticLoadModelPropertyIndex = 29;
        private const int quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex = 30;
        private const int quadraticLowSillForeshoreProfilePropertyIndex = 31;
        private const int quadraticLowSillUseBreakWaterPropertyIndex = 32;
        private const int quadraticLowSillUseForeshorePropertyIndex = 33;
        private const int quadraticLowSillCalculateIllustrationPointsPropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Quadratic Model property Indices

        private const int quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadraticFloodedCulvertVolumicWeightWaterPropertyIndex = 1;
        private const int quadraticFloodedCulvertStormDurationPropertyIndex = 2;
        private const int quadraticFloodedCulvertInsideWaterLevelPropertyIndex = 3;
        private const int quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticFloodedCulvertDrainCoefficientPropertyIndex = 5;
        private const int quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 6;
        private const int quadraticFloodedCulvertStructurePropertyIndex = 7;
        private const int quadraticFloodedCulvertStructureLocationPropertyIndex = 8;
        private const int quadraticFloodedCulvertStructureNormalOrientationPropertyIndex = 9;
        private const int quadraticFloodedCulvertInflowModelTypePropertyIndex = 10;
        private const int quadraticFloodedCulvertLoadSchematizationTypePropertyIndex = 11;
        private const int quadraticFloodedCulvertAreaFlowAperturesPropertyIndex = 12;
        private const int quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 13;
        private const int quadraticFloodedCulvertStorageStructureAreaPropertyIndex = 14;
        private const int quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 15;
        private const int quadraticFloodedCulvertLevelCrestStructurePropertyIndex = 16;
        private const int quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 17;
        private const int quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 18;
        private const int quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 19;
        private const int quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex = 20;
        private const int quadraticFloodedCulvertBankWidthPropertyIndex = 21;
        private const int quadraticFloodedCulvertEvaluationLevelPropertyIndex = 22;
        private const int quadraticFloodedCulvertVerticalDistancePropertyIndex = 23;
        private const int quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex = 25;
        private const int quadraticFloodedCulvertShipMassPropertyIndex = 26;
        private const int quadraticFloodedCulvertShipVelocityPropertyIndex = 27;
        private const int quadraticFloodedCulvertLevellingCountPropertyIndex = 28;
        private const int quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex = 30;
        private const int quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int quadraticFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int quadraticFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int quadraticFloodedCulvertUseForeshorePropertyIndex = 34;
        private const int quadraticFloodedCulvertCalculateIllustrationPointsPropertyIndex = 35;

        #endregion

        #region No structure property Indices

        private const int inflowModelTypePropertyIndex = 11;
        private const int loadSchematizationTypePropertyIndex = 12;
        private const int levellingCountPropertyIndex = 18;
        private const int evaluationLevelPropertyIndex = 25;
        private const int verticalDistancePropertyIndex = 26;
        private const int failureProbabilityRepairClosurePropertyIndex = 27;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 31;

        #endregion
    }
}