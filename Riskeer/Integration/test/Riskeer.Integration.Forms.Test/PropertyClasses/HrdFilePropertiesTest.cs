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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HrdFilePropertiesTest
    {
        private const int filePathPropertyIndex = 0;
        private const int usePreprocessorClosurePropertyIndex = 1;
        private const int usePreprocessorPropertyIndex = 2;
        private const int preprocessorDirectoryPropertyIndex = 3;

        [Test]
        public void Constructor_HrdFileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HrdFileProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hrdFile", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hrdFile = new HrdFile();

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HrdFile>>(properties);
            Assert.AreSame(hrdFile, properties.Data);
        }

        [Test]
        public void GetProperties_WithHrdFileWithPreprocessorData_ReturnExpectedValues()
        {
            // Setup
            const bool usePreprocessor = true;
            const string preprocessorDirectory = @"C:\preprocessor";

            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = preprocessorDirectory
            };

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            Assert.AreEqual(usePreprocessor, properties.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectory);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectoryReadOnly);
        }

        [Test]
        public void GetProperties_WithHrdFile_ReturnsExpectedValues()
        {
            // Setup
            var hrdFile = new HrdFile
            {
                FilePath = "random",
                UsePreprocessorClosure = new Random().NextBoolean()
            };

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            Assert.AreEqual(hrdFile.FilePath, properties.FilePath);
            Assert.AreEqual(hrdFile.UsePreprocessorClosure, properties.UsePreprocessorClosure);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_CanUsePreprocessorTrue_PropertiesHaveExpectedAttributesValues(bool usePreprocessor)
        {
            // Setup
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true,
                UsePreprocessor = usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[filePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "Bestandslocatie",
                                                                            "Locatie van het bestand.",
                                                                            true);

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
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
            var hrdFile = new HrdFile();

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor hrdFilePathProperty = dynamicProperties[filePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hrdFilePathProperty,
                                                                            expectedCategory,
                                                                            "Bestandslocatie",
                                                                            "Locatie van het bestand.",
                                                                            true);

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
                                                                            true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UsePreprocessor_SetNewValue_ValueSetToHrdFileAndObserversNotified(bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true,
                UsePreprocessor = !usePreprocessor,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HrdFileProperties(hrdFile);

            hrdFile.Attach(observer);

            // Call
            properties.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.AreEqual(usePreprocessor, hrdFile.UsePreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void PreprocessorDirectory_SetNewValue_ValueSetToHrdFile()
        {
            // Setup
            const string newPreprocessorDirectory = @"C:/path";
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = "Preprocessor"
            };

            var properties = new HrdFileProperties(hrdFile);

            // Call
            properties.PreprocessorDirectory = newPreprocessorDirectory;

            // Assert
            Assert.AreEqual(newPreprocessorDirectory, hrdFile.PreprocessorDirectory);
        }

        [Test]
        [Combinatorial]
        public void DynamicVisibleValidationMethod_DependingOnCanUsePreprocessorAndUsePreprocessor_ReturnExpectedVisibility(
            [Values(true, false)] bool canUsePreprocessor,
            [Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var hrdFile = new HrdFile();

            if (canUsePreprocessor)
            {
                hrdFile.CanUsePreprocessor = true;
                hrdFile.UsePreprocessor = usePreprocessor;
                hrdFile.PreprocessorDirectory = "Preprocessor";
            }

            // Call
            var properties = new HrdFileProperties(hrdFile);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.FilePath)));
            Assert.AreEqual(canUsePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.AreEqual(canUsePreprocessor && usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.AreEqual(canUsePreprocessor && !usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));
        }
    }
}