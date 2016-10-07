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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructurePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int locationPropertyIndex = 1;
        private const int structureNormalOrientationPropertyIndex = 2;
        private const int closingStructureTypePropertyIndex = 3;
        private const int widthFlowAperturesPropertyIndex = 4;
        private const int areaFlowAperturesPropertyIndex = 5;
        private const int identicalAperturesPropertyIndex = 6;
        private const int flowWidthAtBottomProtectionPropertyIndex = 7;
        private const int storageStructureAreaPropertyIndex = 8;
        private const int allowedLevelIncreaseStoragePropertyIndex = 9;
        private const int levelCrestStructureNotClosingPropertyIndex = 10;
        private const int thresholdHeightOpenWeirPropertyIndex = 11;
        private const int insideWaterLevelPropertyIndex = 12;
        private const int criticalOvertoppingDischargePropertyIndex = 13;
        private const int probabilityOpenStructureBeforeFloodingPropertyIndex = 14;
        private const int failureProbabilityOpenStructurePropertyIndex = 15;
        private const int failureProbabilityReparationPropertyIndex = 16;

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
            var structure = new ClosingStructure("A", "B", new Point2D(1, 2),
                                                 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9,
                                                 10.10, 11.11, 12.12, 13.13, 14.14, 15.15, 16.16,
                                                 17.17, 18.18, 19.19, 20.20, 21.21, 22, 23.23,
                                                 ClosingStructureType.VerticalWall);
            var properties = new ClosingStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
            Assert.AreEqual(structure.Name, properties.Name);
            Assert.AreEqual(structure.Location, properties.Location);
            Assert.AreEqual(structure.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreEqual(structure.InflowModel, properties.InflowModel);

            Assert.AreEqual("Normaal", properties.WidthFlowApertures.DistributionType);
            Assert.AreEqual(structure.WidthFlowApertures, properties.WidthFlowApertures.Data);
            Assert.IsTrue(properties.WidthFlowApertures.DynamicReadOnlyValidationMethod("Mean"));
            Assert.IsTrue(properties.WidthFlowApertures.DynamicReadOnlyValidationMethod("CoefficientOfVariation"));

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
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var structure = new ClosingStructure("A", "B", new Point2D(1, 2),
                                                 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9,
                                                 10.10, 11.11, 12.12, 13.13, 14.14, 15.15, 16.16,
                                                 17.17, 18.18, 19.19, 20.20, 21.21, 22, 23.23,
                                                 ClosingStructureType.VerticalWall);

            // Call
            var properties = new ClosingStructureProperties
            {
                Data = structure
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(17, dynamicProperties.Count);

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

            PropertyDescriptor structureNormalOrientationProperty = dynamicProperties[structureNormalOrientationPropertyIndex];
            Assert.IsTrue(structureNormalOrientationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, structureNormalOrientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", structureNormalOrientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de normaal van het kunstwerk ten opzichte van het noorden.", structureNormalOrientationProperty.Description);
        }
    }
}