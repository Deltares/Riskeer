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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructurePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int locationPropertyIndex = 1;

        private const int insideWaterLevelPropertyIndex = 2;
        private const int insideWaterLevelFailureConstructionPropertyIndex = 3;

        private const int structureNormalOrientationPropertyIndex = 4;
        private const int stabilityPointStructureInflowModelTypePropertyIndex = 5;
        private const int widthFlowAperturesPropertyIndex = 6;
        private const int areaFlowAperturesPropertyIndex = 7;
        private const int flowWidthAtBottomProtectionPropertyIndex = 8;
        private const int storageStructureAreaPropertyIndex = 9;
        private const int allowedLevelIncreaseStoragePropertyIndex = 10;
        private const int levelCrestStructurePropertyIndex = 11;
        private const int thresholdHeightOpenWeirPropertyIndex = 12;
        private const int criticalOvertoppingDischargePropertyIndex = 13;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 14;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 15;
        private const int bankWidthPropertyIndex = 16;
        private const int evaluationLevelPropertyIndex = 17;
        private const int verticalDistancePropertyIndex = 18;
        private const int failureProbabilityRepairClosurePropertyIndex = 19;
        private const int failureCollisionEnergyPropertyIndex = 20;
        private const int shipMassPropertyIndex = 21;
        private const int shipVelocityPropertyIndex = 22;
        private const int levellingCountPropertyIndex = 23;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 24;
        private const int flowVelocityStructureClosablePropertyIndex = 25;
        private const int stabilityLinearLoadModelPropertyIndex = 26;
        private const int stabilityQuadraticLoadModelPropertyIndex = 27;

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
            StabilityPointStructure structure = CreateSimpleStabilityPointStructure();
            var properties = new StabilityPointStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
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
            Assert.IsTrue(properties.WidthFlowApertures.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

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

            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.LevellingCount), properties.LevellingCount);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.ProbabilityCollisionSecondaryStructure), properties.ProbabilityCollisionSecondaryStructure);

            Assert.AreEqual("Normaal", properties.FlowVelocityStructureClosable.DistributionType);
            Assert.AreEqual(structure.FlowVelocityStructureClosable, properties.FlowVelocityStructureClosable.Data);
            Assert.IsTrue(properties.FlowVelocityStructureClosable.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.FlowVelocityStructureClosable.DynamicReadOnlyValidationMethod("StandardDeviation"));

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
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            StabilityPointStructure structure = CreateSimpleStabilityPointStructure();

            // Call
            var properties = new StabilityPointStructureProperties
            {
                Data = structure
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(28, dynamicProperties.Count);

            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het kunstwerk.", nameProperty.Description);

            PropertyDescriptor locationProperty = dynamicProperties[locationPropertyIndex];
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, locationProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", locationProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.", locationProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            Assert.IsTrue(structureNormalOrientationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureNormalOrientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", structureNormalOrientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.", structureNormalOrientationProperty.Description);

            PropertyDescriptor stabilityPointStructureTypeProperty = dynamicProperties[stabilityPointStructureInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(stabilityPointStructureTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityPointStructureTypeProperty.Category);
            Assert.AreEqual("Instroommodel", stabilityPointStructureTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", stabilityPointStructureTypeProperty.Description);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[widthFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, widthFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van doorstroomopening [m]", widthFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Breedte van de doorstroomopening.", widthFlowAperturesProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtectionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureAreaPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            Assert.AreEqual(schematizationCategory, storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Kombergend oppervlak.", storageStructureAreaProperty.Description);

            PropertyDescriptor allowedLevelIncreaseStorageProperty = dynamicProperties[allowedLevelIncreaseStoragePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(allowedLevelIncreaseStorageProperty.Converter);
            Assert.AreEqual(schematizationCategory, allowedLevelIncreaseStorageProperty.Category);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", allowedLevelIncreaseStorageProperty.DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging.", allowedLevelIncreaseStorageProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischargePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            Assert.AreEqual(schematizationCategory, criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Kritiek instromend debiet directe invoer.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[constructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthLinearLoadModelProperty.Category);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[constructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthQuadraticLoadModelProperty.Category);

            PropertyDescriptor bankWidthProperty = dynamicProperties[bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[evaluationLevelPropertyIndex];
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[verticalDistancePropertyIndex];
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[failureProbabilityRepairClosurePropertyIndex];
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);

            PropertyDescriptor shipMassProperty = dynamicProperties[shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);

            PropertyDescriptor levellingCountProperty = dynamicProperties[levellingCountPropertyIndex];
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowVelocityStructureClosableProperty.Category);

            PropertyDescriptor stabilityLinearLoadModelProperty = dynamicProperties[stabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityLinearLoadModelProperty.Category);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[stabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityQuadraticLoadModelProperty.Category);
        }

        private static StabilityPointStructure CreateSimpleStabilityPointStructure()
        {
            return new StabilityPointStructure("Name", "Id", new Point2D(1.234, 2.3456),
                                               123.456,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               456.789, 0.456,
                                               567.890, 0.567,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               123.546, 0.123,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               555.555,
                                               456.789, 0.456,
                                               555.55,
                                               0.55,
                                               567.890, 0.567,
                                               7777777.777, 0.777,
                                               567.890, 0.567,
                                               42,
                                               0.55,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               StabilityPointStructureInflowModelType.FloodedCulvert
                );
        }
    }
}