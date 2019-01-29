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
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
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
        private const int hrdFilePathPropertyIndex = 0;
        private const int hlcdFilePathPropertyIndex = 1;
        private const int scenarioNamePropertyIndex = 2;
        private const int yearPropertyIndex = 3;
        private const int scopePropertyIndex = 4;
        private const int seaLevelPropertyIndex = 5;
        private const int riverDischargePropertyIndex = 6;
        private const int lakeLevelPropertyIndex = 7;
        private const int windDirectionPropertyIndex = 8;
        private const int windSpeedPropertyIndex = 9;
        private const int commentPropertyIndex = 10;
        private const int usePreprocessorPropertyIndex = 11;
        private const int preprocessorDirectoryPropertyIndex = 12;

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseProperties(null, importHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicLocationConfigurationDatabaseImportHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
        public void GetProperties_WithHydraulicBoundaryDatabaseWithPreprocessorData_ReturnExpectedValues()
        {
            // Setup
            const bool usePreprocessor = true;
            const string preprocessorDirectory = @"C:\preprocessor";

            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = preprocessorDirectory
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            Assert.AreEqual(usePreprocessor, properties.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectory);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectoryReadOnly);
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
            Assert.IsEmpty(properties.HrdFilePath);
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
            Assert.AreEqual(hydraulicBoundaryDatabase.FilePath, properties.HrdFilePath);

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
        public void Constructor_CanUsePreprocessorTrue_PropertiesHaveExpectedAttributesValues(bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(13, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD database locatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD database locatie",
                                                                            "Locatie van het HLCD bestand.",
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

            PropertyDescriptor usePreprocessorProperty = dynamicProperties[usePreprocessorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor",
                                                                            "Gebruik de preprocessor bij het uitvoeren van een berekening.");

            PropertyDescriptor preprocessorDirectoryProperty = dynamicProperties[preprocessorDirectoryPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(preprocessorDirectoryProperty,
                                                                            expectedCategory,
                                                                            "Locatie preprocessor bestanden",
                                                                            "Locatie waar de preprocessor bestanden opslaat.",
                                                                            !usePreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CanUsePreprocessorFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD database locatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD database locatie",
                                                                            "Locatie van het HLCD bestand.",
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
            Assert.AreEqual(11, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD database locatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD database locatie",
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
        public void UsePreprocessor_SetNewValue_ValueSetToHydraulicBoundaryDatabaseAndObserversNotified(bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = !usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            hydraulicBoundaryDatabase.Attach(observer);

            // Call
            properties.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.AreEqual(usePreprocessor, hydraulicBoundaryDatabase.UsePreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void PreprocessorDirectory_SetNewValue_ValueSetToHydraulicBoundaryDatabase()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            const string newPreprocessorDirectory = @"C:/path";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Call
            properties.PreprocessorDirectory = newPreprocessorDirectory;

            // Assert
            Assert.AreEqual(newPreprocessorDirectory, hydraulicBoundaryDatabase.PreprocessorDirectory);
            mocks.VerifyAll();
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
        [Combinatorial]
        public void DynamicVisibleValidationMethod_DependingOnCanUsePreprocessorAndUsePreprocessor_ReturnExpectedVisibility(
            [Values(true, false)] bool canUsePreprocessor,
            [Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            if (canUsePreprocessor)
            {
                hydraulicBoundaryDatabase.CanUsePreprocessor = true;
                hydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;
                hydraulicBoundaryDatabase.PreprocessorDirectory = "Preprocessor";
            }

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase, importHandler);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.HrdFilePath)));
            Assert.AreEqual(canUsePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.AreEqual(canUsePreprocessor && usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.AreEqual(canUsePreprocessor && !usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));

            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePath)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.HlcdFilePathReadOnly)));
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
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.HrdFilePath)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));

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
            const string filePath = @"C:\file.sqlite";

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath
            };

            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues("FilePath",
                                                                                       "ScenarioName",
                                                                                       10,
                                                                                       "Scope",
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