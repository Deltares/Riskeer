// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.HydraRing;

namespace Riskeer.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraRingSettingsDatabaseValidatorTest
    {
        private const string testDataSubDirectory = "HydraRingSettingsDatabaseValidator";

        private static readonly string directoryPath = TestHelper.GetTestDataPath(
            TestDataPath.Riskeer.Common.IO, testDataSubDirectory);

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string completeDatabasePath = Path.Combine(directoryPath, "validSettings.config.sqlite");

            // Call
            using (var validator = new HydraRingSettingsDatabaseValidator(completeDatabasePath))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(validator);
            }
        }

        [Test]
        public void ValidateSchema_ValidDatabase_ReturnTrue()
        {
            // Setup
            string completeDatabasePath = Path.Combine(directoryPath, "validSettings.config.sqlite");

            using (var validator = new HydraRingSettingsDatabaseValidator(completeDatabasePath))
            {
                // Call
                bool valid = validator.ValidateSchema();

                // Assert
                Assert.IsTrue(valid);
            }
        }

        [Test]
        [TestCase("invalidSettings_MissingColumn")]
        [TestCase("invalidSettings_MissingTable")]
        [TestCase("invalidSettings_WrongColumnName")]
        [TestCase("invalidSettings_WrongTableName")]
        public void ValidateSchema_InvalidDatabase_ReturnFalse(string databaseName)
        {
            // Setup
            string completeDatabasePath = Path.Combine(directoryPath, $"{databaseName}.config.sqlite");

            using (var validator = new HydraRingSettingsDatabaseValidator(completeDatabasePath))
            {
                // Call
                bool valid = validator.ValidateSchema();

                // Assert
                Assert.IsFalse(valid);
            }
        }
    }
}