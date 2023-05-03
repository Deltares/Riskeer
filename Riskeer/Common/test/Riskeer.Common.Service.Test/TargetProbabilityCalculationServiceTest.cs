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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class TargetProbabilityCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";
        private static readonly TargetProbabilityCalculationService calculationService = new TestTargetProbabilityCalculationService();

        [Test]
        public void Validate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => calculationService.Validate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Validate_WithCalculationSettings_ReturnsTrue()
        {
            // Setup
            var valid = false;

            // Call
            void Call() => valid = calculationService.Validate(CreateValidCalculationSettings());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabasePath_LogsErrorAndReturnsFalse()
        {
            // Setup
            string invalidHrdFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHlcdFilePath, invalidHrdFilePath, validHrdFileVersion, false);
            var valid = true;

            // Call
            void Call() => valid = calculationService.Validate(calculationSettings);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_MismatchingHydraulicBoundaryDatabaseVersion_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string invalidHrdFileVersion = "Dutch coast South19-11-2015 12:0113";
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath, invalidHrdFileVersion, false);
            var valid = true;

            // Call
            void Call() => valid = calculationService.Validate(calculationSettings);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De versie van de corresponderende hydraulische belastingendatabase wijkt af van de versie zoals gevonden in het bestand '{validHrdFilePath}'.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            string invalidHrdFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHlcdFilePath, invalidHrdFilePath, validHrdFileVersion, false);

            // Call
            void Call() => valid = calculationService.Validate(calculationSettings);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_UsePreprocessorClosureTrueAndWithoutPreprocessorClosureDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var valid = true;
            string folderWithoutPreprocessorClosureDatabase = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "HydraulicBoundaryData"),
                                                                           "withoutPreprocessorClosure");
            var calculationSettings = new HydraulicBoundaryCalculationSettings(Path.Combine(folderWithoutPreprocessorClosureDatabase, "hlcd.sqlite"),
                                                                               Path.Combine(folderWithoutPreprocessorClosureDatabase, "complete.sqlite"),
                                                                               validHrdFileVersion,
                                                                               true);

            // Call
            void Call() => valid = calculationService.Validate(calculationSettings);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);

                string preprocessorClosureFilePath = Path.Combine(folderWithoutPreprocessorClosureDatabase, "hlcd_preprocClosure.sqlite");
                var expectedMessage = $"Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.";
                Assert.AreEqual(expectedMessage, msgs[1]);

                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        private static HydraulicBoundaryCalculationSettings CreateValidCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath, validHrdFileVersion, false);
        }

        private class TestTargetProbabilityCalculationService : TargetProbabilityCalculationService {}
    }
}