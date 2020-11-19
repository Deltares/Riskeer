﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Util;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Services;

namespace Riskeer.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingInitializationServiceTest
    {
        private static readonly string hydraRingDirectory = Path.Combine(
            AssemblyHelper.GetApplicationDirectory(), "Standalone",
            "Deltares", $"HydraRing-{HydraRingFileConstants.HydraRingVersionNumber}");

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
            var settings = new HydraRingCalculationSettings("D:\\hlcd\\hlcdFilePath", "D:\\preprocessor", false);

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
        public void GenerateInitializationScript_ReturnsExpectedInitializationScript(
            [Values("", "D:\\preprocessor")] string preprocessorDirectory,
            [Values(true, false)] bool usePreprocessorClosure)
        {
            // Setup
            const string hlcdFilePath = "D:\\hlcd\\HlcdFile.sqlite";

            var settings = new HydraRingCalculationSettings(hlcdFilePath, preprocessorDirectory, usePreprocessorClosure);
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

            if (usePreprocessorClosure)
            {
                expectedInitializationScript += Environment.NewLine + "preprocessorclosingdbfilename = D:\\hlcd\\HlcdFile_preprocClosure.sqlite";
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