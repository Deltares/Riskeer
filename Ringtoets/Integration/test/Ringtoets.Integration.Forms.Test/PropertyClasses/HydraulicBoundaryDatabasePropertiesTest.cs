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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabase>>(properties);
            Assert.AreSame(hydraulicBoundaryDatabase, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string filePath = @"C:\file.sqlite";
            const bool usePreprocessor = true;
            const string preprocessorDirectory = @"C:\preprocessor";

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = preprocessorDirectory
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(filePath, properties.FilePath);
            Assert.AreEqual(usePreprocessor, properties.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectory);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectoryReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_CanUsePreprocessorTrue_PropertiesHaveExpectedAttributesValues(bool usePreprocessor)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(13, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "Hydraulische belastingendatabase",
                                                                            "Locatie van het hydraulische belastingendatabase bestand.",
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
        }

        [Test]
        public void Constructor_CanUsePreprocessorFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[hrdFilePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "Hydraulische belastingendatabase",
                                                                            "Locatie van het hydraulische belastingendatabase bestand.",
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
        }

        [Test]
        public void UsePreprocessor_SetNewValue_ValueSetToHydraulicBoundaryDatabaseAndObserversNotified([Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = !usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

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
            const string newPreprocessorDirectory = @"C:/path";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Call
            properties.PreprocessorDirectory = newPreprocessorDirectory;

            // Assert
            Assert.AreEqual(newPreprocessorDirectory, hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void DynamicVisibleValidationMethod_DependingOnCanUsePreprocessorAndUsePreprocessor_ReturnExpectedVisibility(
            [Values(true, false)] bool canUsePreprocessor,
            [Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            if (canUsePreprocessor)
            {
                hydraulicBoundaryDatabase.CanUsePreprocessor = true;
                hydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;
                hydraulicBoundaryDatabase.PreprocessorDirectory = "Preprocessor";
            }

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.FilePath)));
            Assert.AreEqual(canUsePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.AreEqual(canUsePreprocessor && usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.AreEqual(canUsePreprocessor && !usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));
        }
    }
}