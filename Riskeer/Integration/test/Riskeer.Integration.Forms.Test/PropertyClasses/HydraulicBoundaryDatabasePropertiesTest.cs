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
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Editors;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
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
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseProperties(null, importHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicLocationConfigurationDatabaseImportHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            void Call() => new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicLocationConfigurationDatabaseImportHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabase>>(properties);
            Assert.AreSame(hydraulicBoundaryDatabase, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithUnlinkedDatabase_ReturnsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Precondition
            Assert.IsFalse(hydraulicBoundaryDatabase.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

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
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithLinkedDatabase_ReturnsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateLinkedHydraulicBoundaryDatabase();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            HydraulicLocationConfigurationSettings configurationSettings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
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
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithLinkedDatabaseStatus_HlcdFilePathHaveExpectedAttributeValue(bool isLinked)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = isLinked
                                                                      ? CreateLinkedHydraulicBoundaryDatabase()
                                                                      : new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

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
        public void HlcdFilePath_SetNewValue_CallsHydraulicLocationConfigurationDatabaseImportHandler()
        {
            // Setup
            const string hlcdFilePath = @"C:/path/HlcdFilePath";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var importHandler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseImportHandler>();
            importHandler.Expect(ih => ih.ImportHydraulicLocationConfigurationSettings(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                                                       hlcdFilePath));
            mocks.ReplayAll();

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Call
            properties.HlcdFilePath = hlcdFilePath;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnHydraulicDatabaseLinkStatus_ReturnsExpectedVisibility(bool isHydraulicBoundaryDatabaseLinked)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = isHydraulicBoundaryDatabaseLinked
                                                                      ? CreateLinkedHydraulicBoundaryDatabase()
                                                                      : new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            Assert.AreEqual(isHydraulicBoundaryDatabaseLinked, properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePath)));
            Assert.AreEqual(!isHydraulicBoundaryDatabaseLinked, properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePathReadOnly)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.ScenarioName)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Year)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Scope)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.SeaLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.RiverDischarge)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.LakeLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.WindDirection)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.WindSpeed)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Comment)));
            mocks.VerifyAll();
        }

        private static HydraulicBoundaryDatabase CreateLinkedHydraulicBoundaryDatabase()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                HrdFiles =
                {
                    new HrdFile
                    {
                        FilePath = @"C:\file.sqlite"
                    }
                }
            };

            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues("FilePath",
                                                                                       "ScenarioName",
                                                                                       10,
                                                                                       "Scope",
                                                                                       false,
                                                                                       "SeaLevel",
                                                                                       "RiverDischarge",
                                                                                       "LakeLevel",
                                                                                       "WindDirection",
                                                                                       "WindSpeed",
                                                                                       "Comment");

            return hydraulicBoundaryDatabase;
        }
    }
}