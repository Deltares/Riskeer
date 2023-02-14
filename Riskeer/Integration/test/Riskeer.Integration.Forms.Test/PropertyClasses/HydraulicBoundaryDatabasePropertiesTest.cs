﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
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
        private const int hrdFilePathPropertyIndex = 0;
        private const int hlcdFilePathPropertyIndex = 1;
        private const int usePreprocessorClosurePropertyIndex = 2;
        private const int scenarioNamePropertyIndex = 3;
        private const int yearPropertyIndex = 4;
        private const int scopePropertyIndex = 5;
        private const int seaLevelPropertyIndex = 6;
        private const int riverDischargePropertyIndex = 7;
        private const int lakeLevelPropertyIndex = 8;
        private const int windDirectionPropertyIndex = 9;
        private const int windSpeedPropertyIndex = 10;
        private const int commentPropertyIndex = 11;
        private const int usePreprocessorPropertyIndex = 12;
        private const int preprocessorDirectoryPropertyIndex = 13;

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseProperties(null, importHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicLocationConfigurationDatabaseImportHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            void Call() => new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, null);

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

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryData>>(properties);
            Assert.AreSame(hydraulicBoundaryData, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithHydraulicBoundaryDataWithPreprocessorData_ReturnExpectedValues()
        {
            // Setup
            const bool usePreprocessor = true;
            const string preprocessorDirectory = @"C:\preprocessor";

            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            Assert.AreEqual(usePreprocessor, properties.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectory);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectoryReadOnly);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithUnlinkedHydraulicBoundaryDatabase_ReturnsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Precondition
            Assert.IsFalse(hydraulicBoundaryData.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            Assert.IsEmpty(properties.HrdFilePath);
            Assert.IsEmpty(properties.HlcdFilePath);
            Assert.IsEmpty(properties.HlcdFilePathReadOnly);
            Assert.IsFalse(properties.UsePreprocessorClosure);
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
        public void GetProperties_WithLinkedHydraulicBoundaryDatabase_ReturnsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryData.IsLinked());

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            Assert.AreEqual(hydraulicBoundaryData.FilePath, properties.HrdFilePath);

            HydraulicLocationConfigurationSettings configurationSettings = hydraulicBoundaryData.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(configurationSettings.FilePath, properties.HlcdFilePath);
            Assert.AreEqual(configurationSettings.FilePath, properties.HlcdFilePathReadOnly);
            Assert.AreEqual(configurationSettings.UsePreprocessorClosure, properties.UsePreprocessorClosure);
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

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = "Preprocessor"
                }
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(14, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD bestandslocatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD bestandslocatie",
                                                                            "Locatie van het HLCD bestand.",
                                                                            true);

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
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

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(12, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD bestandslocatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

            PropertyDescriptor hlcdFilePathProperty = dynamicProperties[hlcdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hlcdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HLCD bestandslocatie",
                                                                            "Locatie van het HLCD bestand.",
                                                                            true);

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
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

            HydraulicBoundaryData hydraulicBoundaryData = isLinked
                                                              ? CreateLinkedHydraulicBoundaryData()
                                                              : new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(12, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "HRD bestandslocatie",
                                                                            "Locatie van het HRD bestand.",
                                                                            true);

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

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
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

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = !usePreprocessor,
                    PreprocessorDirectory = "Preprocessor"
                }
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            hydraulicBoundaryData.Attach(observer);

            // Call
            properties.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.AreEqual(usePreprocessor, hydraulicBoundaryData.HydraulicLocationConfigurationSettings.UsePreprocessor);
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
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = "Preprocessor"
                }
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Call
            properties.PreprocessorDirectory = newPreprocessorDirectory;

            // Assert
            Assert.AreEqual(newPreprocessorDirectory, hydraulicBoundaryData.HydraulicLocationConfigurationSettings.PreprocessorDirectory);
            mocks.VerifyAll();
        }

        [Test]
        public void HlcdFilePath_SetNewValue_CallsHydraulicLocationConfigurationDatabaseImportHandler()
        {
            // Setup
            const string hlcdFilePath = @"C:/path/HlcdFilePath";
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var mocks = new MockRepository();
            var importHandler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseImportHandler>();
            importHandler.Expect(ih => ih.ImportHydraulicLocationConfigurationSettings(hydraulicBoundaryData.HydraulicLocationConfigurationSettings,
                                                                                       hlcdFilePath));
            mocks.ReplayAll();

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

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

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            if (canUsePreprocessor)
            {
                hydraulicBoundaryData.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
                hydraulicBoundaryData.HydraulicLocationConfigurationSettings.UsePreprocessor = usePreprocessor;
                hydraulicBoundaryData.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "Preprocessor";
            }

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

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
        public void DynamicVisibleValidationMethod_DependingOnHydraulicDatabaseLinkStatus_ReturnsExpectedVisibility(bool isHydraulicBoundaryDataLinked)
        {
            // Setup
            var mocks = new MockRepository();
            var importHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseImportHandler>();
            mocks.ReplayAll();

            HydraulicBoundaryData hydraulicBoundaryData = isHydraulicBoundaryDataLinked
                                                              ? CreateLinkedHydraulicBoundaryData()
                                                              : new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryData, importHandler);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.HrdFilePath)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));

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
            mocks.VerifyAll();
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
                                                                                   false,
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