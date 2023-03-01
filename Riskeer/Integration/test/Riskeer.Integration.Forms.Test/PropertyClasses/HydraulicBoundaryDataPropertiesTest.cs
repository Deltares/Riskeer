// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Drawing.Design;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Editors;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDataPropertiesTest
    {
        private const int hlcdFilePathPropertyIndex = 0;
        private const int scenarioNamePropertyIndex = 1;
        private const int yearPropertyIndex = 2;
        private const int scopePropertyIndex = 3;
        private const int seaLevelPropertyIndex = 4;
        private const int riverDischargePropertyIndex = 5;
        private const int lakeLevelPropertyIndex = 6;
        private const int windDirectionPropertyIndex = 7;
        private const int windSpeedPropertyIndex = 8;
        private const int commentPropertyIndex = 9;

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDataProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryData>>(properties);
            Assert.AreSame(hydraulicBoundaryData, properties.Data);
        }

        [Test]
        public void GetProperties_WithUnlinkedHydraulicBoundaryDatabase_ReturnsExpectedValues()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            Assert.IsEmpty(properties.HlcdFilePath);
            Assert.IsEmpty(properties.HlcdFilePathReadOnly);
            Assert.IsEmpty(properties.ScenarioName);
            Assert.IsEmpty(properties.Year);
            Assert.IsEmpty(properties.Scope);
            Assert.IsEmpty(properties.SeaLevel);
            Assert.IsEmpty(properties.RiverDischarge);
            Assert.IsEmpty(properties.LakeLevel);
            Assert.IsEmpty(properties.WindDirection);
            Assert.IsEmpty(properties.WindSpeed);
            Assert.IsEmpty(properties.Comment);
        }

        [Test]
        public void GetProperties_WithLinkedHydraulicBoundaryDatabase_ReturnsExpectedValues()
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryData.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            HydraulicLocationConfigurationSettings configurationSettings = hydraulicBoundaryData.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(configurationSettings.FilePath, properties.HlcdFilePath);
            Assert.AreEqual(configurationSettings.FilePath, properties.HlcdFilePathReadOnly);
            Assert.AreEqual(configurationSettings.ScenarioName, properties.ScenarioName);
            Assert.AreEqual(configurationSettings.Year.ToString(), properties.Year);
            Assert.AreEqual(configurationSettings.Scope, properties.Scope);
            Assert.AreEqual(configurationSettings.SeaLevel, properties.SeaLevel);
            Assert.AreEqual(configurationSettings.RiverDischarge, properties.RiverDischarge);
            Assert.AreEqual(configurationSettings.LakeLevel, properties.LakeLevel);
            Assert.AreEqual(configurationSettings.WindDirection, properties.WindDirection);
            Assert.AreEqual(configurationSettings.WindSpeed, properties.WindSpeed);
            Assert.AreEqual(configurationSettings.Comment, properties.Comment);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithLinkedDatabaseStatus_PropertiesHaveExpectedAttributesValues(bool isLinked)
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = isLinked
                                                              ? CreateLinkedHydraulicBoundaryData()
                                                              : new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD bestandslocatie",
                                                                            "Locatie van het HLCD bestand.",
                                                                            !isLinked);
            if (isLinked)
            {
                object hlcdFilePathEditor = hlcdFilePathProperty.GetEditor(typeof(UITypeEditor));
                Assert.IsInstanceOf<HlcdFileNameEditor>(hlcdFilePathEditor);
            }

            PropertyDescriptor scenarioNameProperty = dynamicProperties[scenarioNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(scenarioNameProperty,
                                                                            expectedCategory,
                                                                            "Klimaatscenario",
                                                                            "Algemene naam van het klimaatscenario.",
                                                                            true);

            PropertyDescriptor yearProperty = dynamicProperties[yearPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(yearProperty,
                                                                            expectedCategory,
                                                                            "Zichtjaar",
                                                                            "Jaartal van het jaar waarop de statistiek van toepassing is.",
                                                                            true);

            PropertyDescriptor scopeProperty = dynamicProperties[scopePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(scopeProperty,
                                                                            expectedCategory,
                                                                            "Toepassingskader",
                                                                            "Projectkader waarin de statistiek bedoeld is te gebruiken.",
                                                                            true);

            PropertyDescriptor seaLevelProperty = dynamicProperties[seaLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(seaLevelProperty,
                                                                            expectedCategory,
                                                                            "Zeewaterstand",
                                                                            "Klimaatinformatie met betrekking tot de zeewaterstand.",
                                                                            true);

            PropertyDescriptor riverDischargeProperty = dynamicProperties[riverDischargePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(riverDischargeProperty,
                                                                            expectedCategory,
                                                                            "Rivierafvoer",
                                                                            "Klimaatinformatie met betrekking tot de rivierafvoer.",
                                                                            true);

            PropertyDescriptor lakeLevelProperty = dynamicProperties[lakeLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lakeLevelProperty,
                                                                            expectedCategory,
                                                                            "Meerpeil",
                                                                            "Klimaatinformatie met betrekking tot het meerpeil/de meerpeilen.",
                                                                            true);

            PropertyDescriptor windDirectionProperty = dynamicProperties[windDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windDirectionProperty,
                                                                            expectedCategory,
                                                                            "Windrichting",
                                                                            "Klimaatinformatie met betrekking tot de windrichting.",
                                                                            true);

            PropertyDescriptor windSpeedProperty = dynamicProperties[windSpeedPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(windSpeedProperty,
                                                                            expectedCategory,
                                                                            "Windsnelheid",
                                                                            "Klimaatinformatie met betrekking tot de windsnelheid.",
                                                                            true);

            PropertyDescriptor commentProperty = dynamicProperties[commentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(commentProperty,
                                                                            expectedCategory,
                                                                            "Overig",
                                                                            "Overige informatie.",
                                                                            true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnHydraulicBoundaryDataLinkStatus_ReturnsExpectedVisibility(bool isHydraulicBoundaryDataLinked)
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = isHydraulicBoundaryDataLinked
                                                              ? CreateLinkedHydraulicBoundaryData()
                                                              : new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(isHydraulicBoundaryDataLinked, properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePath)));
            Assert.AreEqual(!isHydraulicBoundaryDataLinked, properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePathReadOnly)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ScenarioName)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Year)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Scope)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.SeaLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.RiverDischarge)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.LakeLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.WindDirection)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.WindSpeed)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Comment)));
        }

        private static HydraulicBoundaryData CreateLinkedHydraulicBoundaryData()
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = @"C:\file.sqlite"
            };

            hydraulicBoundaryData.HydraulicLocationConfigurationSettings.SetValues("FilePath",
                                                                                   "ScenarioName",
                                                                                   10,
                                                                                   "Scope",
                                                                                   true,
                                                                                   "SeaLevel",
                                                                                   "RiverDischarge",
                                                                                   "LakeLevel",
                                                                                   "WindDirection",
                                                                                   "WindSpeed",
                                                                                   "Comment");

            return hydraulicBoundaryData;
        }
    }
}