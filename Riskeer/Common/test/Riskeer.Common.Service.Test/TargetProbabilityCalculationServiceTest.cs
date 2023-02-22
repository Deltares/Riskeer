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
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
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
            var calculationSettings = new HydraulicBoundaryCalculationSettings(invalidHrdFilePath, validHlcdFilePath, false);
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
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            string invalidHrdFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(invalidHrdFilePath, validHlcdFilePath, false);

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
        public void Validate_UsePreprocessorClosureTrueAndWithoutPreprocessorClosure_LogsErrorAndReturnsFalse()
        {
            // Setup
            var valid = true;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHrdFilePath, validHlcdFilePath, true);

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

        private static HydraulicBoundaryCalculationSettings CreateValidCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHrdFilePath, validHlcdFilePath, false);
        }

        private class TestTargetProbabilityCalculationService : TargetProbabilityCalculationService {}
    }
}