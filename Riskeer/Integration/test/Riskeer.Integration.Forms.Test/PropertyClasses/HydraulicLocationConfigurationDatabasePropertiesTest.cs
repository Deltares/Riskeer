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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabasePropertiesTest
    {
        private const int filePathPropertyIndex = 0;
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
        public void Constructor_HydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicLocationConfigurationDatabase", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();

            // Call
            var properties = new HydraulicLocationConfigurationDatabaseProperties(hydraulicLocationConfigurationDatabase);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicLocationConfigurationDatabase>>(properties);
            Assert.AreSame(hydraulicLocationConfigurationDatabase, properties.Data);
        }

        [Test]
        public void GetProperties_WithUnlinkedHydraulicLocationConfigurationDatabase_ReturnsExpectedValues()
        {
            // Setup
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();

            // Call
            var properties = new HydraulicLocationConfigurationDatabaseProperties(hydraulicLocationConfigurationDatabase);

            // Assert
            Assert.IsEmpty(properties.FilePath);
            Assert.IsEmpty(properties.ScenarioName);
            Assert.AreEqual("0", properties.Year);
            Assert.IsEmpty(properties.Scope);
            Assert.IsEmpty(properties.SeaLevel);
            Assert.IsEmpty(properties.RiverDischarge);
            Assert.IsEmpty(properties.LakeLevel);
            Assert.IsEmpty(properties.WindDirection);
            Assert.IsEmpty(properties.WindSpeed);
            Assert.IsEmpty(properties.Comment);
        }

        [Test]
        public void GetProperties_WithLinkedHydraulicLocationConfigurationDatabase_ReturnsExpectedValues()
        {
            // Setup
            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = CreateLinkedHydraulicLocationConfigurationDatabase();

            // Call
            var properties = new HydraulicLocationConfigurationDatabaseProperties(hydraulicLocationConfigurationDatabase);

            // Assert
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.FilePath, properties.FilePath);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.ScenarioName, properties.ScenarioName);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.Year.ToString(), properties.Year);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.Scope, properties.Scope);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.SeaLevel, properties.SeaLevel);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.RiverDischarge, properties.RiverDischarge);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.LakeLevel, properties.LakeLevel);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.WindDirection, properties.WindDirection);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.WindSpeed, properties.WindSpeed);
            Assert.AreEqual(hydraulicLocationConfigurationDatabase.Comment, properties.Comment);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithHydraulicLocationConfigurationDatabase_PropertiesHaveExpectedAttributesValues(bool useLinkedHydraulicLocationConfigurationDatabase)
        {
            // Setup
            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = useLinkedHydraulicLocationConfigurationDatabase
                                                                                                ? CreateLinkedHydraulicLocationConfigurationDatabase()
                                                                                                : new HydraulicLocationConfigurationDatabase();

            // Call
            var properties = new HydraulicLocationConfigurationDatabaseProperties(hydraulicLocationConfigurationDatabase);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor filePathProperty = dynamicProperties[filePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(filePathProperty,
                                                                            expectedCategory,
                                                                            "Bestandslocatie",
                                                                            "Locatie van het bestand.",
                                                                            true);

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

        private static HydraulicLocationConfigurationDatabase CreateLinkedHydraulicLocationConfigurationDatabase()
        {
            return new HydraulicLocationConfigurationDatabase
            {
                FilePath = "hlcd.sqlite",
                ScenarioName = "ScenarioName",
                Year = 1337,
                Scope = "Scope",
                SeaLevel = "SeaLevel",
                RiverDischarge = "RiverDischarge",
                LakeLevel = "LakeLevel",
                WindDirection = "WindDirection",
                WindSpeed = "WindSpeed",
                Comment = "Comment",
                UsePreprocessorClosure = true
            };
        }
    }
}