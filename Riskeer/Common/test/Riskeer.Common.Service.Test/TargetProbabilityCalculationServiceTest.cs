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
        private const double validTargetProbability = 0.005;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();
        private static readonly TargetProbabilityCalculationService calculationService = new TestTargetProbabilityCalculationService();

        [Test]
        public void Validate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => calculationService.Validate(null, validTargetProbability);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Validate_ValidParameters_ReturnsTrue()
        {
            // Setup
            var valid = false;

            // Call
            Action call = () => valid = calculationService.Validate(CreateValidCalculationSettings(), validTargetProbability);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            string invalidHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var calculationSettings = new HydraulicBoundaryCalculationSettings(invalidHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               string.Empty);
            var valid = true;

            // Call
            Action call = () => valid = calculationService.Validate(calculationSettings, validTargetProbability);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            string invalidHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(invalidHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               string.Empty);

            // Call
            Action call = () => valid = calculationService.Validate(calculationSettings, validTargetProbability);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_InvalidPreprocessorDirectory_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string invalidPreprocessorDirectory = "NonExistingPreprocessorDirectory";
            var valid = true;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               invalidPreprocessorDirectory);

            // Call
            Action call = () => valid = calculationService.Validate(calculationSettings, validTargetProbability);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_TargetProbabilityInvalid_LogsErrorAndReturnsFalse()
        {
            // Setup
            var valid = true;

            // Call
            Action call = () => valid = calculationService.Validate(CreateValidCalculationSettings(), double.NaN);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kon geen doelkans bepalen voor deze berekening.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_TargetProbabilityInvalidTooBig_LogsErrorAndReturnsFalse()
        {
            // Setup
            var valid = true;

            // Call
            Action call = () => valid = calculationService.Validate(CreateValidCalculationSettings(), 1.0);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_TargetProbabilityInvalidTooSmall_LogsErrorAndReturnsFalse()
        {
            // Setup
            var valid = true;

            // Call
            Action call = () => valid = calculationService.Validate(CreateValidCalculationSettings(), 0.0);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Doelkans is te klein om een berekening uit te kunnen voeren.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        private static HydraulicBoundaryCalculationSettings CreateValidCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                            validHlcdFilePath,
                                                            validPreprocessorDirectory);
        }

        private class TestTargetProbabilityCalculationService : TargetProbabilityCalculationService {}
    }
}