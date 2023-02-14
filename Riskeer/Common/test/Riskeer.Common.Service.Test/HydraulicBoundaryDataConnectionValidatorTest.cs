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
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraulicBoundaryDataConnectionValidatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));

        [Test]
        public void Validate_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataConnectionValidator.Validate(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDataNotLinked_ReturnErrorMessage()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData);

            // Assert
            const string expectedMessage = "Er is geen hydraulische belastingendatabase geïmporteerd.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDataLinkedToNotExistingDatabaseFile_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = "I_do_not_exist.sqlite"
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand 'I_do_not_exist.sqlite': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDataLinkedToExistingDatabaseFileWithoutSettings_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite")
            };
            
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryData);

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. De rekeninstellingen database heeft niet het juiste schema.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_UsePreprocessorClosureTrueWithoutPreprocessorClosureFile_ReturnsMessageWithError()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "complete.sqlite")
            };
            
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryData, true);

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "hlcd_preprocClosure.sqlite");
            string expectedMessage = $"Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_HydraulicBoundaryDataLinkedToValidDatabaseFile_ReturnsNull(bool usePreprocessorClosure)
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            };
            
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryData, usePreprocessorClosure);

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData);

            // Assert
            Assert.IsNull(message);
        }
    }
}