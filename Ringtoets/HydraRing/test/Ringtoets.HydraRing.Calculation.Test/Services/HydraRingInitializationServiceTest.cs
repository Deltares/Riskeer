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
using System.Reflection;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Services;

namespace Riskeer.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingInitializationServiceTest
    {
        private readonly string hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");

        [Test]
        public void Constructor_SettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new HydraRingInitializationService(random.NextEnumValue<HydraRingFailureMechanismType>(),
                                                                         random.Next(),
                                                                         "D:\\work",
                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("settings", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var settings = new HydraRingCalculationSettings("D:\\hlcd\\hlcdFilePath", "D:\\preprocessor");

            // Call
            var hydraRingInitializationService = new HydraRingInitializationService(HydraRingFailureMechanismType.AssessmentLevel,
                                                                                    700001,
                                                                                    "D:\\work",
                                                                                    settings);

            // Assert
            Assert.AreEqual("D:\\work\\700001.ini", hydraRingInitializationService.IniFilePath);
            Assert.AreEqual("D:\\work\\700001.sql", hydraRingInitializationService.DatabaseCreationScriptFilePath);
            Assert.AreEqual(Path.Combine(hydraRingDirectory, "MechanismComputation.exe"), hydraRingInitializationService.MechanismComputationExeFilePath);
            Assert.AreEqual("D:\\work", hydraRingInitializationService.TemporaryWorkingDirectory);
        }

        [Test]
        [TestCase("")]
        [TestCase("D:\\preprocessor")]
        public void GenerateInitializationScript_ReturnsExpectedInitializationScript(string preprocessorDirectory)
        {
            // Setup
            const string hlcdFilePath = "D:\\hlcd\\HlcdFile.sqlite";

            var settings = new HydraRingCalculationSettings(hlcdFilePath, preprocessorDirectory);
            var hydraRingInitializationService = new HydraRingInitializationService(HydraRingFailureMechanismType.StructuresStructuralFailure,
                                                                                    700001,
                                                                                    TestHelper.GetScratchPadPath(),
                                                                                    settings);

            string expectedInitializationScript = "section                 = 700001" + Environment.NewLine +
                                                  "mechanism               = 112" + Environment.NewLine +
                                                  "alternative             = 1" + Environment.NewLine +
                                                  "layer                   = 1" + Environment.NewLine +
                                                  "logfile                 = 700001.log" + Environment.NewLine +
                                                  "outputverbosity         = basic" + Environment.NewLine +
                                                  "outputtofile            = file" + Environment.NewLine +
                                                  "projectdbfilename       = 700001.sql" + Environment.NewLine +
                                                  "outputfilename          = designTable.txt" + Environment.NewLine +
                                                  "configdbfilename        = " + Path.Combine(hydraRingDirectory, "config.sqlite") + Environment.NewLine +
                                                  "hydraulicdbfilename     = " + hlcdFilePath + Environment.NewLine +
                                                  "designpointOutput       = sqlite";

            if (preprocessorDirectory != string.Empty)
            {
                expectedInitializationScript += Environment.NewLine + "preprocessordbdirectory = " + preprocessorDirectory;
            }

            try
            {
                // Call
                hydraRingInitializationService.WriteInitializationScript();

                // Assert
                string initializationScript = File.ReadAllText(hydraRingInitializationService.IniFilePath);
                Assert.AreEqual(expectedInitializationScript, initializationScript);
            }
            finally
            {
                File.Delete(hydraRingInitializationService.IniFilePath);
            }
        }
    }
}