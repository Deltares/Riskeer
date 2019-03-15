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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses;

namespace Riskeer.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructurePropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;

        private const int insideWaterLevelFailureConstructionPropertyIndex = 3;
        private const int bankWidthPropertyIndex = 4;
        private const int verticalDistancePropertyIndex = 5;

        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 6;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 7;
        private const int evaluationLevelPropertyIndex = 8;
        private const int failureProbabilityRepairClosurePropertyIndex = 9;

        private const int stabilityPointStructureInflowModelTypePropertyIndex = 10;
        private const int structureNormalOrientationPropertyIndex = 11;
        private const int levelCrestStructurePropertyIndex = 12;
        private const int insideWaterLevelPropertyIndex = 13;
        private const int thresholdHeightOpenWeirPropertyIndex = 14;
        private const int widthFlowAperturesPropertyIndex = 15;
        private const int areaFlowAperturesPropertyIndex = 16;

        private const int criticalOvertoppingDischargePropertyIndex = 17;
        private const int flowWidthAtBottomProtectionPropertyIndex = 18;

        private const int storageStructureAreaPropertyIndex = 19;
        private const int allowedLevelIncreaseStoragePropertyIndex = 20;

        private const int stabilityQuadraticLoadModelPropertyIndex = 21;
        private const int stabilityLinearLoadModelPropertyIndex = 22;

        private const int failureCollisionEnergyPropertyIndex = 23;
        private const int shipMassPropertyIndex = 24;
        private const int shipVelocityPropertyIndex = 25;
        private const int levellingCountPropertyIndex = 26;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 27;
        private const int flowVelocityStructureClosablePropertyIndex = 28;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityPointStructureProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityPointStructure>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewStabilityPointStructureInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();
            var properties = new StabilityPointStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
            Assert.AreEqual(structure.Id, properties.Id);
            Assert.AreEqual(structure.Name, properties.Name);
            var expectedLocation = new Point2D(new RoundedDouble(0, structure.Location.X),
                                               new RoundedDouble(0, structure.Location.Y));
            Assert.AreEqual(expectedLocation, properties.Location);

            Assert.AreEqual("Normaal", properties.InsideWaterLevel.DistributionType);
            Assert.AreEqual(structure.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.IsTrue(properties.InsideWaterLevel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.InsideWaterLevel.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.InsideWaterLevelFailureConstruction.DistributionType);
            Assert.AreEqual(structure.InsideWaterLevelFailureConstruction, properties.InsideWaterLevelFailureConstruction.Data);
            Assert.IsTrue(properties.InsideWaterLevelFailureConstruction.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.InsideWaterLevelFailureConstruction.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual(structure.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreEqual(structure.InflowModelType, properties.InflowModelType);

            Assert.AreEqual("Normaal", properties.WidthFlowApertures.DistributionType);
            Assert.AreEqual(structure.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.IsTrue(properties.WidthFlowApertures.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.WidthFlowApertures.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.AreaFlowApertures.DistributionType);
            Assert.AreEqual(structure.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.IsTrue(properties.AreaFlowApertures.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.AreaFlowApertures.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.FlowWidthAtBottomProtection.DistributionType);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.IsTrue(properties.FlowWidthAtBottomProtection.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.FlowWidthAtBottomProtection.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.StorageStructureArea.DistributionType);
            Assert.AreEqual(structure.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.IsTrue(properties.StorageStructureArea.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.StorageStructureArea.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Lognormaal", properties.AllowedLevelIncreaseStorage.DistributionType);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage, properties.AllowedLevelIncreaseStorage.Data);
            Assert.IsTrue(properties.AllowedLevelIncreaseStorage.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.AllowedLevelIncreaseStorage.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.LevelCrestStructure.DistributionType);
            Assert.AreEqual(structure.LevelCrestStructure, properties.LevelCrestStructure.Data);
            Assert.IsTrue(properties.LevelCrestStructure.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.LevelCrestStructure.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.ThresholdHeightOpenWeir.DistributionType);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.IsTrue(properties.ThresholdHeightOpenWeir.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ThresholdHeightOpenWeir.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.CriticalOvertoppingDischarge.DistributionType);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Lognormaal", properties.ConstructiveStrengthLinearLoadModel.DistributionType);
            Assert.AreEqual(structure.ConstructiveStrengthLinearLoadModel, properties.ConstructiveStrengthLinearLoadModel.Data);
            Assert.IsTrue(properties.ConstructiveStrengthLinearLoadModel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ConstructiveStrengthLinearLoadModel.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Lognormaal", properties.ConstructiveStrengthQuadraticLoadModel.DistributionType);
            Assert.AreEqual(structure.ConstructiveStrengthQuadraticLoadModel, properties.ConstructiveStrengthQuadraticLoadModel.Data);
            Assert.IsTrue(properties.ConstructiveStrengthQuadraticLoadModel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ConstructiveStrengthQuadraticLoadModel.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Normaal", properties.BankWidth.DistributionType);
            Assert.AreEqual(structure.BankWidth, properties.BankWidth.Data);
            Assert.IsTrue(properties.BankWidth.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.BankWidth.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual(structure.EvaluationLevel, properties.EvaluationLevel);

            Assert.AreEqual(structure.VerticalDistance, properties.VerticalDistance);

            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.FailureProbabilityRepairClosure), properties.FailureProbabilityRepairClosure);

            Assert.AreEqual("Lognormaal", properties.FailureCollisionEnergy.DistributionType);
            Assert.AreEqual(structure.FailureCollisionEnergy, properties.FailureCollisionEnergy.Data);
            Assert.IsTrue(properties.FailureCollisionEnergy.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.FailureCollisionEnergy.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Normaal", properties.ShipMass.DistributionType);
            Assert.AreEqual(structure.ShipMass, properties.ShipMass.Data);
            Assert.IsTrue(properties.ShipMass.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ShipMass.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Normaal", properties.ShipVelocity.DistributionType);
            Assert.AreEqual(structure.ShipVelocity, properties.ShipVelocity.Data);
            Assert.IsTrue(properties.ShipVelocity.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ShipVelocity.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual(structure.LevellingCount, properties.LevellingCount);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.ProbabilityCollisionSecondaryStructure), properties.ProbabilityCollisionSecondaryStructure);

            Assert.AreEqual("Normaal", properties.FlowVelocityStructureClosable.DistributionType);
            Assert.AreEqual(structure.FlowVelocityStructureClosable, properties.FlowVelocityStructureClosable.Data);
            Assert.IsTrue(properties.FlowVelocityStructureClosable.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.FlowVelocityStructureClosable.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Lognormaal", properties.StabilityLinearLoadModel.DistributionType);
            Assert.AreEqual(structure.StabilityLinearLoadModel, properties.StabilityLinearLoadModel.Data);
            Assert.IsTrue(properties.StabilityLinearLoadModel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.StabilityLinearLoadModel.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual("Lognormaal", properties.StabilityQuadraticLoadModel.DistributionType);
            Assert.AreEqual(structure.StabilityQuadraticLoadModel, properties.StabilityQuadraticLoadModel.Data);
            Assert.IsTrue(properties.StabilityQuadraticLoadModel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.StabilityQuadraticLoadModel.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            // Call
            var properties = new StabilityPointStructureProperties
            {
                Data = structure
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(29, dynamicProperties.Count);

            const string schematizationCategory = "Schematisatie";
            const string generalCategory = "Algemeen";

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategory,
                                                                            "ID",
                                                                            "ID van het kunstwerk.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het kunstwerk.",
                                                                            true);

            PropertyDescriptor locationProperty = dynamicProperties[locationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationProperty,
                                                                            generalCategory,
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelFailureConstructionProperty,
                                                                            schematizationCategory,
                                                                            "Binnenwaterstand bij constructief falen [m+NAP]",
                                                                            "Binnenwaterstand bij constructief falen.",
                                                                            true);

            PropertyDescriptor bankWidthProperty = dynamicProperties[bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bankWidthProperty,
                                                                            schematizationCategory,
                                                                            "Bermbreedte [m]",
                                                                            "Bermbreedte.",
                                                                            true);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[verticalDistancePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(verticalDistanceProperty,
                                                                            schematizationCategory,
                                                                            "Afstand onderkant wand en teen van de dijk/berm [m]",
                                                                            "Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.",
                                                                            true);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[constructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(constructiveStrengthQuadraticLoadModelProperty,
                                                                            schematizationCategory,
                                                                            "Kwadratische belastingschematisering constructieve sterkte [kN/m]",
                                                                            "Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.",
                                                                            true);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[constructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(constructiveStrengthLinearLoadModelProperty,
                                                                            schematizationCategory,
                                                                            "Lineaire belastingschematisering constructieve sterkte [kN/m²]",
                                                                            "Kritieke sterkte constructie volgens de lineaire belastingschematisatie.",
                                                                            true);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[evaluationLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(evaluationLevelProperty,
                                                                            schematizationCategory,
                                                                            "Analysehoogte [m+NAP]",
                                                                            "Hoogte waarop de constructieve sterkte wordt beoordeeld.",
                                                                            true);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[failureProbabilityRepairClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityRepairClosureProperty,
                                                                            schematizationCategory,
                                                                            "Faalkans herstel van gefaalde situatie [-]",
                                                                            "Faalkans herstel van gefaalde situatie.",
                                                                            true);

            PropertyDescriptor stabilityPointStructureTypeProperty = dynamicProperties[stabilityPointStructureInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(stabilityPointStructureTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stabilityPointStructureTypeProperty,
                                                                            schematizationCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.",
                                                                            true);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureNormalOrientationProperty,
                                                                            schematizationCategory,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.",
                                                                            true);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureProperty,
                                                                            schematizationCategory,
                                                                            "Kerende hoogte [m+NAP]",
                                                                            "Kerende hoogte van het kunstwerk.",
                                                                            true);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(thresholdHeightOpenWeirProperty,
                                                                            schematizationCategory,
                                                                            "Drempelhoogte [m+NAP]",
                                                                            "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                                                                            true);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[widthFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthFlowAperturesProperty,
                                                                            schematizationCategory,
                                                                            "Breedte van doorstroomopening [m]",
                                                                            "Breedte van de doorstroomopening.",
                                                                            true);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(areaFlowAperturesProperty,
                                                                            schematizationCategory,
                                                                            "Doorstroomoppervlak [m²]",
                                                                            "Doorstroomoppervlak van doorstroomopeningen.",
                                                                            true);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalOvertoppingDischargeProperty,
                                                                            schematizationCategory,
                                                                            "Kritiek instromend debiet [m³/s/m]",
                                                                            "Kritiek instromend debiet directe invoer per strekkende meter.",
                                                                            true);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(flowWidthAtBottomProtectionProperty,
                                                                            schematizationCategory,
                                                                            "Stroomvoerende breedte bodembescherming [m]",
                                                                            "Stroomvoerende breedte bodembescherming.",
                                                                            true);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(storageStructureAreaProperty,
                                                                            schematizationCategory,
                                                                            "Kombergend oppervlak [m²]",
                                                                            "Kombergend oppervlak.",
                                                                            true);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[allowedLevelIncreaseStoragePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(allowedLevelIncreaseStorageProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(allowedLevelIncreaseStorageProperty,
                                                                            schematizationCategory,
                                                                            "Toegestane peilverhoging komberging [m]",
                                                                            "Toegestane peilverhoging komberging.",
                                                                            true);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[stabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stabilityQuadraticLoadModelProperty,
                                                                            schematizationCategory,
                                                                            "Kwadratische belastingschematisering stabiliteit [kN/m]",
                                                                            "Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.",
                                                                            true);

            PropertyDescriptor stabilityLinearLoadModelProperty = dynamicProperties[stabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stabilityLinearLoadModelProperty,
                                                                            schematizationCategory,
                                                                            "Lineaire belastingschematisering stabiliteit [kN/m²]",
                                                                            "Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.",
                                                                            true);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureCollisionEnergyProperty,
                                                                            schematizationCategory,
                                                                            "Bezwijkwaarde aanvaarenergie [kN m]",
                                                                            "Bezwijkwaarde aanvaarenergie.",
                                                                            true);

            PropertyDescriptor shipMassProperty = dynamicProperties[shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(shipMassProperty,
                                                                            schematizationCategory,
                                                                            "Massa van het schip [ton]",
                                                                            "Massa van het schip.",
                                                                            true);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(shipVelocityProperty,
                                                                            schematizationCategory,
                                                                            "Aanvaarsnelheid [m/s]",
                                                                            "Aanvaarsnelheid.",
                                                                            true);

            PropertyDescriptor levellingCountProperty = dynamicProperties[levellingCountPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levellingCountProperty,
                                                                            schematizationCategory,
                                                                            "Aantal nivelleringen per jaar [1/jaar]",
                                                                            "Aantal nivelleringen per jaar.",
                                                                            true);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[probabilityCollisionSecondaryStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityCollisionSecondaryStructureProperty,
                                                                            schematizationCategory,
                                                                            "Kans op aanvaring tweede keermiddel per nivellering [1/nivellering]",
                                                                            "Kans op aanvaring tweede keermiddel per nivellering.",
                                                                            true);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(flowVelocityStructureClosableProperty,
                                                                            schematizationCategory,
                                                                            "Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]",
                                                                            "Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.",
                                                                            true);
        }
    }
}