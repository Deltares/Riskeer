﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseTest
    {
        [Test]
        public void Constructor_Parameterless_ExpectedValues()
        {
            // Call
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Assert
            Assert.IsInstanceOf<Observable>(hydraulicBoundaryDatabase);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void UsePreprocessor_SetValueWithCanUsePreprocessorTrue_ExpectedValueSet()
        {
            // Setup
            bool usePreprocessor = new Random(11).NextBoolean();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.SetParameters(new List<HydraulicBoundaryLocation>(), "", "", !usePreprocessor, "Preprocessor");

            // Call
            hydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.AreEqual(usePreprocessor, hydraulicBoundaryDatabase.UsePreprocessor);
        }

        [Test]
        public void UsePreprocessor_SetValueWithCanUsePreprocessorFalse_ThrowsInvalidOperationException()
        {
            // Setup
            bool usePreprocessor = new Random(11).NextBoolean();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;

            // Assert
            string message = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual($"{nameof(HydraulicBoundaryDatabase.CanUsePreprocessor)} is false.", message);
        }

        [Test]
        public void PreprocessorDirectory_SetValidValueWithCanUsePreprocessorTrue_ExpectedValueSet()
        {
            // Setup
            const string preprocessorDirectory = "OtherPreprocessor";

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.SetParameters(new List<HydraulicBoundaryLocation>(), "", "", true, "Preprocessor");

            // Call
            hydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            // Assert
            Assert.AreEqual(preprocessorDirectory, hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void PreprocessorDirectory_SetValidValueWithCanUsePreprocessorFalse_ThrowsInvalidOperationException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.PreprocessorDirectory = "Preprocessor";

            // Assert
            string message = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual($"{nameof(HydraulicBoundaryDatabase.CanUsePreprocessor)} is false.", message);
        }

        [Test]
        public void ConstructorWithoutPreprocessorParameters_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.SetParameters(null, "filePath", "version");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void GivenDefaultHydraulicBoundaryDatabase_WhenSettingValidParametersWithoutPreprocessorValues_ThenExpectedValuesSet()
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocations = new List<HydraulicBoundaryLocation>();
            const string filePath = "filePath";
            const string version = "version";

            // When
            hydraulicBoundaryDatabase.SetParameters(hydraulicBoundaryLocations, filePath, version);

            // Then
            Assert.AreSame(hydraulicBoundaryLocations, hydraulicBoundaryDatabase.Locations);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(version, hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void GivenConfiguredHydraulicBoundaryDatabase_WhenSettingValidParametersWithoutPreprocessorValues_ThenExpectedValuesSet()
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocations = new List<HydraulicBoundaryLocation>();
            const string filePath = "filePath";
            const string version = "version";

            hydraulicBoundaryDatabase.SetParameters(new List<HydraulicBoundaryLocation>(),
                                                    "otherFilePath",
                                                    "otherVersion",
                                                    true,
                                                    "preprocessorDirectory");

            // When
            hydraulicBoundaryDatabase.SetParameters(hydraulicBoundaryLocations, filePath, version);

            // Then
            Assert.AreSame(hydraulicBoundaryLocations, hydraulicBoundaryDatabase.Locations);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(version, hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void ConstructorWithPreprocessorParameters_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.SetParameters(null, "filePath", "version", true, "preprocessorDirectory");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GivenDefaultHydraulicBoundaryDatabase_WhenSettingValidParametersWithPreprocessorValues_ThenExpectedValuesSet(bool usePreprocessor)
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocations = new List<HydraulicBoundaryLocation>();
            const string filePath = "filePath";
            const string version = "version";
            const string preprocessorDirectory = "preprocessorDirectory";

            // When
            hydraulicBoundaryDatabase.SetParameters(hydraulicBoundaryLocations, filePath, version, usePreprocessor, preprocessorDirectory);

            // Then
            Assert.AreSame(hydraulicBoundaryLocations, hydraulicBoundaryDatabase.Locations);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(version, hydraulicBoundaryDatabase.Version);
            Assert.IsTrue(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.AreEqual(usePreprocessor, hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GivenConfiguredHydraulicBoundaryDatabase_WhenSettingValidParametersWithPreprocessorValues_ThenExpectedValuesSet(bool usePreprocessor)
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocations = new List<HydraulicBoundaryLocation>();
            const string filePath = "filePath";
            const string version = "version";
            const string preprocessorDirectory = "preprocessorDirectory";

            hydraulicBoundaryDatabase.SetParameters(new List<HydraulicBoundaryLocation>(),
                                                    "otherFilePath",
                                                    "otherVersion");

            // When
            hydraulicBoundaryDatabase.SetParameters(hydraulicBoundaryLocations, filePath, version, usePreprocessor, preprocessorDirectory);

            // Then
            Assert.AreSame(hydraulicBoundaryLocations, hydraulicBoundaryDatabase.Locations);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(version, hydraulicBoundaryDatabase.Version);
            Assert.IsTrue(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.AreEqual(usePreprocessor, hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void PreprocessorDirectory_SetInvalidValueWithCanUsePreprocessorTrue_ThrowsArgumentException(string preprocessorDirectory)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.SetParameters(new List<HydraulicBoundaryLocation>(), "", "", true, "Preprocessor");

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            // Assert
            string message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat moet een waarde hebben.", message);
        }
    }
}