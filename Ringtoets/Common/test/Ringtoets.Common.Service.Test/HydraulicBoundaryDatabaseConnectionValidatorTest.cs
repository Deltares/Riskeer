// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseConnectionValidatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [Test]
        public void Validate_DatabaseNull_ReturnErrorMessage()
        {
            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(null);

            // Assert
            const string expectedMessage = "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_DatabaseWithEmptyFilepath_ReturnErrorMessage()
        {
            // Setup
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = string.Empty
            };

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(database);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_DatabaseWithFilepathToNotExistingDatabase_ReturnErrorMessage()
        {
            // Setup
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "I_do_not_exist.db"
            };

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(database);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand 'I_do_not_exist.db': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_ExistingFileWithHlcd_ReturnsNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(database);

            // Assert
            Assert.IsNull(message);
        }

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithoutSettings_ReturnsMessageWithError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = invalidFilePath
            };

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(database);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. De rekeninstellingen database heeft niet het juiste schema.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}