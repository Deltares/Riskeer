﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.ComponentModel;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructurePropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;

        private const int identicalAperturesPropertyIndex = 3;
        private const int failureProbabilityOpenStructurePropertyIndex = 4;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 5;
        private const int failureProbabilityReparationPropertyIndex = 6;

        private const int inflowModelTypePropertyIndex = 7;
        private const int insideWaterLevelPropertyIndex = 8;
        private const int structureNormalOrientationPropertyIndex = 9;
        private const int areaFlowAperturesPropertyIndex = 10;
        private const int thresholdHeightOpenWeirPropertyIndex = 11;
        private const int levelCrestStructureNotClosingPropertyIndex = 12;
        private const int widthFlowAperturesPropertyIndex = 13;

        private const int criticalOvertoppingDischargePropertyIndex = 14;
        private const int flowWidthAtBottomProtectionPropertyIndex = 15;

        private const int storageStructureAreaPropertyIndex = 16;
        private const int allowedLevelIncreaseStoragePropertyIndex = 17;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ClosingStructureProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ClosingStructure>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewClosingStructureInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();
            var properties = new ClosingStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
            Assert.AreEqual(structure.Id, properties.Id);
            Assert.AreEqual(structure.Name, properties.Name);
            Assert.AreEqual(Math.Round(structure.Location.X), properties.Location.X);
            Assert.AreEqual(Math.Round(structure.Location.Y), properties.Location.Y);
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

            Assert.AreEqual(structure.IdenticalApertures, properties.IdenticalApertures);

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

            Assert.AreEqual("Normaal", properties.LevelCrestStructureNotClosing.DistributionType);
            Assert.AreEqual(structure.LevelCrestStructureNotClosing, properties.LevelCrestStructureNotClosing.Data);
            Assert.IsTrue(properties.LevelCrestStructureNotClosing.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.LevelCrestStructureNotClosing.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.ThresholdHeightOpenWeir.DistributionType);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.IsTrue(properties.ThresholdHeightOpenWeir.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.ThresholdHeightOpenWeir.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.InsideWaterLevel.DistributionType);
            Assert.AreEqual(structure.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.IsTrue(properties.InsideWaterLevel.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.InsideWaterLevel.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.CriticalOvertoppingDischarge.DistributionType);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.ProbabilityOpenStructureBeforeFlooding), properties.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.FailureProbabilityOpenStructure), properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(structure.FailureProbabilityReparation), properties.FailureProbabilityReparation);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();

            // Call
            var properties = new ClosingStructureProperties
            {
                Data = structure
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(18, dynamicProperties.Count);

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

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[identicalAperturesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(identicalAperturesProperty,
                                                                            schematizationCategory,
                                                                            "Aantal identieke doorstroomopeningen [-]",
                                                                            "Aantal identieke doorstroomopeningen.",
                                                                            true);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[failureProbabilityOpenStructurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityOpenStructureProperty,
                                                                            schematizationCategory,
                                                                            "Kans mislukken sluiting [-]",
                                                                            "Kans op mislukken sluiting van geopend kunstwerk.",
                                                                            true);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[probabilityOpenStructureBeforeFloodingPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(probabilityOpenStructureBeforeFloodingProperty,
                                                                            schematizationCategory,
                                                                            "Kans op open staan bij naderend hoogwater [-]",
                                                                            "Kans op open staan bij naderend hoogwater.",
                                                                            true);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[failureProbabilityReparationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureProbabilityReparationProperty,
                                                                            schematizationCategory,
                                                                            "Faalkans herstel van gefaalde situatie [-]",
                                                                            "Faalkans herstel van gefaalde situatie.",
                                                                            true);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inflowModelTypeProperty,
                                                                            schematizationCategory,
                                                                            "Instroommodel",
                                                                            "Instroommodel van het kunstwerk.",
                                                                            true);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(insideWaterLevelProperty,
                                                                            schematizationCategory,
                                                                            "Binnenwaterstand [m+NAP]",
                                                                            "Binnenwaterstand.",
                                                                            true);

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(structureNormalOrientationProperty,
                                                                            schematizationCategory,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.",
                                                                            true);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(areaFlowAperturesProperty,
                                                                            schematizationCategory,
                                                                            "Doorstroomoppervlak [m²]",
                                                                            "Doorstroomoppervlak van doorstroomopeningen.",
                                                                            true);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(thresholdHeightOpenWeirProperty,
                                                                            schematizationCategory,
                                                                            "Drempelhoogte [m+NAP]",
                                                                            "Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.",
                                                                            true);

            PropertyDescriptor levelCrestStructureNotClosingProperty = dynamicProperties[levelCrestStructureNotClosingPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureNotClosingProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureNotClosingProperty,
                                                                            schematizationCategory,
                                                                            "Kruinhoogte niet gesloten kering [m+NAP]",
                                                                            "Niveau kruin bij niet gesloten maximaal kerende keermiddelen.",
                                                                            true);

            PropertyDescriptor widthFlowAperturesProperty = dynamicProperties[widthFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthFlowAperturesProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthFlowAperturesProperty,
                                                                            schematizationCategory,
                                                                            "Breedte van doorstroomopening [m]",
                                                                            "Breedte van de doorstroomopening.",
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
        }
    }
}