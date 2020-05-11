// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseConnectionValidatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));

        [Test]
        public void Validate_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicBoundaryDatabaseConnectionValidator.Validate(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryDatabase", paramName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseNotLinked_ReturnErrorMessage()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);

            // Assert
            const string expectedMessage = "Er is geen hydraulische belastingendatabase geïmporteerd.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseLinkedToNotExistingDatabaseFile_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "I_do_not_exist.db"
            };

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met één van de benodigde hydraulische belastingendatabases is mislukt. Fout bij het lezen van bestand 'I_do_not_exist.db': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseLinkedToExistingDatabaseFileWithoutSettings_ReturnsErrorMessage()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = invalidFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met één van de benodigde hydraulische belastingendatabases is mislukt. De rekeninstellingen database heeft niet het juiste schema.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_UsePreprocessorClosureTrueWithoutPreprocessorClosureFile_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "complete.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase, true);

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "hlcd_preprocClosure.sqlite");
            string expectedMessage = $"Herstellen van de verbinding met één van de benodigde hydraulische belastingendatabases is mislukt. Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_HydraulicBoundaryDatabaseLinkedToValidDatabaseFile_ReturnsNull(bool usePreprocessorClosure)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase, usePreprocessorClosure);

            // Call
            string message = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);

            // Assert
            Assert.IsNull(message);
        }
    }
}