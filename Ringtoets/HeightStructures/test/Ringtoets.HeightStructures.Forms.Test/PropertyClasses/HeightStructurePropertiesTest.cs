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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PropertyClasses;

namespace Ringtoets.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructurePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int locationPropertyIndex = 1;
        private const int orientationOfTheNormalOfTheStructure = 2;
        private const int levelOfCrestOfStructure = 3;
        private const int allowableIncreaseOfLevelForStorage = 4;
        private const int storageStructureArea = 5;
        private const int flowWidthAtBottomProtection = 6;
        private const int widthOfFlowApertures = 7;
        private const int criticalOvertoppingDischarge = 8;
        private const int failureProbabilityOfStructureGivenErosion = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new HeightStructureProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HeightStructure>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewHeightStructureInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var structure = new HeightStructure("A", "B", new Point2D(1, 2),
                                                1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9,
                                                10.10, 11.11, 12.12, 13.13, 14.14);
            var properties = new HeightStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
            Assert.AreEqual(structure.Name, properties.Name);
            Assert.AreEqual(structure.Location, properties.Location);
            Assert.AreEqual(structure.OrientationOfTheNormalOfTheStructure, properties.OrientationOfTheNormalOfTheStructure);

            Assert.AreEqual("Normaal", properties.LevelOfCrestOfStructure.DistributionType);
            Assert.AreEqual(structure.LevelOfCrestOfStructure, properties.LevelOfCrestOfStructure.Data);
            Assert.IsTrue(properties.LevelOfCrestOfStructure.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.LevelOfCrestOfStructure.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.AllowableIncreaseOfLevelForStorage.DistributionType);
            Assert.AreEqual(structure.AllowableIncreaseOfLevelForStorage, properties.AllowableIncreaseOfLevelForStorage.Data);
            Assert.IsTrue(properties.AllowableIncreaseOfLevelForStorage.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.AllowableIncreaseOfLevelForStorage.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.StorageStructureArea.DistributionType);
            Assert.AreEqual(structure.StorageStructureArea, properties.StorageStructureArea.Data);
            Assert.IsTrue(properties.StorageStructureArea.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.StorageStructureArea.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.FlowWidthAtBottomProtection.DistributionType);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection, properties.FlowWidthAtBottomProtection.Data);
            Assert.IsTrue(properties.FlowWidthAtBottomProtection.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.FlowWidthAtBottomProtection.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Normaal", properties.WidthOfFlowApertures.DistributionType);
            Assert.AreEqual(structure.WidthOfFlowApertures, properties.WidthOfFlowApertures.Data);
            Assert.IsTrue(properties.WidthOfFlowApertures.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.WidthOfFlowApertures.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual("Lognormaal", properties.CriticalOvertoppingDischarge.DistributionType);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge, properties.CriticalOvertoppingDischarge.Data);
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.CriticalOvertoppingDischarge.DynamicReadOnlyValidationMethod("StandardDeviation"));

            Assert.AreEqual(structure.FailureProbabilityOfStructureGivenErosion, properties.FailureProbabilityOfStructureGivenErosion);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var structure = new HeightStructure("A", "B", new Point2D(1, 2),
                                                1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9,
                                                10.10, 11.11, 12.12, 13.13, 14.14);

            // Call
            var properties = new HeightStructureProperties
            {
                Data = structure
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]{BrowsableAttribute.Yes});
            Assert.AreEqual(10, dynamicProperties.Count);

            const string schematizationCategory = "Schematisatie";
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


            PropertyDescriptor orientationOfTheNormalOfTheStructureProperty = dynamicProperties[orientationOfTheNormalOfTheStructure];
            Assert.IsTrue(orientationOfTheNormalOfTheStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, orientationOfTheNormalOfTheStructureProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", orientationOfTheNormalOfTheStructureProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.", orientationOfTheNormalOfTheStructureProperty.Description);

            PropertyDescriptor levelOfCrestOfStructureProperty = dynamicProperties[levelOfCrestOfStructure];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelOfCrestOfStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelOfCrestOfStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m]", levelOfCrestOfStructureProperty.DisplayName);
            Assert.AreEqual("De kerende hoogte van het kunstwerk.", levelOfCrestOfStructureProperty.Description);

            PropertyDescriptor allowableIncreaseOfLevelForStorageProperty = dynamicProperties[allowableIncreaseOfLevelForStorage];
            Assert.IsInstanceOf<ExpandableObjectConverter>(allowableIncreaseOfLevelForStorageProperty.Converter);
            Assert.AreEqual(schematizationCategory, allowableIncreaseOfLevelForStorageProperty.Category);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", allowableIncreaseOfLevelForStorageProperty.DisplayName);
            Assert.AreEqual("De toegestane peilverhoging op het kombergend oppervlak.", allowableIncreaseOfLevelForStorageProperty.Description);

            PropertyDescriptor storageStructureAreaProperty = dynamicProperties[storageStructureArea];
            Assert.IsInstanceOf<ExpandableObjectConverter>(storageStructureAreaProperty.Converter);
            Assert.AreEqual(schematizationCategory, storageStructureAreaProperty.Category);
            Assert.AreEqual("Kombergend oppervlak [m²]", storageStructureAreaProperty.DisplayName);
            Assert.AreEqual("Het kombergend oppervlak.", storageStructureAreaProperty.Description);

            PropertyDescriptor flowWidthAtBottomProtectionProperty = dynamicProperties[flowWidthAtBottomProtection];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowWidthAtBottomProtectionProperty.Converter);
            Assert.AreEqual(schematizationCategory, flowWidthAtBottomProtectionProperty.Category);
            Assert.AreEqual("Stroomvoerende breedte bij bodembescherming [m]", flowWidthAtBottomProtectionProperty.DisplayName);
            Assert.AreEqual("De stroomvoerende breedte bij bodembescherming.", flowWidthAtBottomProtectionProperty.Description);

            PropertyDescriptor widthOfFlowAperturesProperty = dynamicProperties[widthOfFlowApertures];
            Assert.IsInstanceOf<ExpandableObjectConverter>(widthOfFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, widthOfFlowAperturesProperty.Category);
            Assert.AreEqual("Breedte van de kruin van het kunstwerk [m]", widthOfFlowAperturesProperty.DisplayName);
            Assert.AreEqual("De breedte van de kruin van het kunstwerk.", widthOfFlowAperturesProperty.Description);

            PropertyDescriptor criticalOvertoppingDischargeProperty = dynamicProperties[criticalOvertoppingDischarge];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalOvertoppingDischargeProperty.Converter);
            Assert.AreEqual(schematizationCategory, criticalOvertoppingDischargeProperty.Category);
            Assert.AreEqual("Kritiek overslagdebiet [m³/s/m]", criticalOvertoppingDischargeProperty.DisplayName);
            Assert.AreEqual("Het kritieke overslagdebiet per strekkende meter.", criticalOvertoppingDischargeProperty.Description);

            PropertyDescriptor failureProbabilityOfStructureGivenErosionProperty = dynamicProperties[failureProbabilityOfStructureGivenErosion];
            Assert.IsTrue(failureProbabilityOfStructureGivenErosionProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOfStructureGivenErosionProperty.Category);
            Assert.AreEqual("Faalkans kunstwerk gegeven erosie bodem [-]", failureProbabilityOfStructureGivenErosionProperty.DisplayName);
            Assert.AreEqual("De faalkans van het kunstwerk gegeven de erosie in de bodem.", failureProbabilityOfStructureGivenErosionProperty.Description);
        }
    }
}