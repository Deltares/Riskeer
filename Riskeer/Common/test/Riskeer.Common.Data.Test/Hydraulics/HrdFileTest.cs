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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HrdFileTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hrdFile = new HrdFile();

            // Assert
            Assert.IsNull(hrdFile.FilePath);
            Assert.IsNull(hrdFile.Version);
            Assert.IsFalse(hrdFile.UsePreprocessorClosure);
            Assert.IsFalse(hrdFile.CanUsePreprocessor);
            Assert.IsFalse(hrdFile.UsePreprocessor);
            Assert.IsNull(hrdFile.PreprocessorDirectory);
        }

        [Test]
        public void UsePreprocessor_SetValueWithCanUsePreprocessorTrue_ExpectedValueSet()
        {
            // Setup
            bool usePreprocessor = new Random(11).NextBoolean();
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true
            };

            // Call
            hrdFile.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.IsInstanceOf<Observable>(hrdFile);
            Assert.AreEqual(usePreprocessor, hrdFile.UsePreprocessor);
        }

        [Test]
        public void UsePreprocessor_SetValueWithCanUsePreprocessorFalse_ThrowsInvalidOperationException()
        {
            // Setup
            bool usePreprocessor = new Random(11).NextBoolean();
            var hrdFile = new HrdFile();

            // Call
            void Call() => hrdFile.UsePreprocessor = usePreprocessor;

            // Assert
            string message = Assert.Throws<InvalidOperationException>(Call).Message;
            Assert.AreEqual($"{nameof(HrdFile.CanUsePreprocessor)} is false.", message);
        }

        [Test]
        public void PreprocessorDirectory_SetValidValueWithCanUsePreprocessorTrue_ExpectedValueSet()
        {
            // Setup
            const string preprocessorDirectory = "OtherPreprocessor";
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true
            };

            // Call
            hrdFile.PreprocessorDirectory = preprocessorDirectory;

            // Assert
            Assert.AreEqual(preprocessorDirectory, hrdFile.PreprocessorDirectory);
        }

        [Test]
        public void PreprocessorDirectory_SetValidValueWithCanUsePreprocessorFalse_ThrowsInvalidOperationException()
        {
            // Setup
            var hrdFile = new HrdFile();

            // Call
            void Call() => hrdFile.PreprocessorDirectory = "Preprocessor";

            // Assert
            string message = Assert.Throws<InvalidOperationException>(Call).Message;
            Assert.AreEqual($"{nameof(HrdFile.CanUsePreprocessor)} is false.", message);
        }

        [Test]
        public void GivenHrdFileWithPreprocessorValues_WhenSettingCanUsePreprocessorFalse_ThenPreprocessorValuesReset()
        {
            // Given
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = "PreprocessorDirectory"
            };

            // When
            hrdFile.CanUsePreprocessor = false;

            // Then
            Assert.IsFalse(hrdFile.CanUsePreprocessor);
            Assert.IsFalse(hrdFile.UsePreprocessor);
            Assert.IsNull(hrdFile.PreprocessorDirectory);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanUsePreprocessor_Always_ExpectedValuesSet(bool canUsePreprocessor)
        {
            // Setup
            var hrdFile = new HrdFile();

            // Call
            hrdFile.CanUsePreprocessor = canUsePreprocessor;

            // Assert
            Assert.AreEqual(canUsePreprocessor, hrdFile.CanUsePreprocessor);
            Assert.IsFalse(hrdFile.UsePreprocessor);
            Assert.IsNull(hrdFile.PreprocessorDirectory);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void PreprocessorDirectory_SetInvalidValueWithCanUsePreprocessorTrue_ThrowsArgumentException(string preprocessorDirectory)
        {
            // Setup
            var hrdFile = new HrdFile
            {
                CanUsePreprocessor = true
            };

            // Call
            void Call() => hrdFile.PreprocessorDirectory = preprocessorDirectory;

            // Assert
            string message = Assert.Throws<ArgumentException>(Call).Message;
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat moet een waarde hebben.", message);
        }
    }
}