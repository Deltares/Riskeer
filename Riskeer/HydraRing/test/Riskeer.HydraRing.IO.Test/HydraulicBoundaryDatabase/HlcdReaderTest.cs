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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class HlcdReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.IO, "HydraulicLocationConfigurationDatabase");

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithScenarioInformation.sqlite");

            // Call
            using (var reader = new HlcdReader(hlcdFilePath, string.Empty))
            {
                // Assert
                Assert.AreEqual(hlcdFilePath, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }
        }

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "doesNotExist.sqlite");

            // Call
            void Call()
            {
                using (new HlcdReader(hlcdFilePath, string.Empty)) {}
            }

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FilePathNullOrEmpty_ThrowsCriticalFileReadException(string hlcdFilePath)
        {
            // Call
            void Call()
            {
                using (new HlcdReader(hlcdFilePath, string.Empty)) {}
            }

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}