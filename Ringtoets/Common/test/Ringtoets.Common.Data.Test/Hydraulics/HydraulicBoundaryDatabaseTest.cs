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
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true
            };

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
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true
            };

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
        public void GivenDatabaseWithPreprocessorSettings_WhenSettingCanUsePreprocessorFalse_ThenPreprocessorSettingsReset()
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = "PreprocessorDirectory"
            };

            // When
            hydraulicBoundaryDatabase.CanUsePreprocessor = false;

            // Then
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanUsePreprocessor_Always_ExpectedValuesSet(bool canUsePreprocessor)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            hydraulicBoundaryDatabase.CanUsePreprocessor = canUsePreprocessor;

            // Assert
            Assert.AreEqual(canUsePreprocessor, hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void PreprocessorDirectory_SetInvalidValueWithCanUsePreprocessorTrue_ThrowsArgumentException(string preprocessorDirectory)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                CanUsePreprocessor = true
            };

            // Call
            TestDelegate test = () => hydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            // Assert
            string message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat moet een waarde hebben.", message);
        }
    }
}