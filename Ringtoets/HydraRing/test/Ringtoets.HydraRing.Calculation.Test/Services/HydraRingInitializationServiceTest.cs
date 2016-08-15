﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingInitializationServiceTest
    {
        private readonly string hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Call
            var hydraRingInitializationService = new HydraRingInitializationService(HydraRingFailureMechanismType.DikesPiping, 700001, "D:\\hlcd", "D:\\work");

            // Assert
            Assert.AreEqual("D:\\work\\700001.ini", hydraRingInitializationService.IniFilePath);
            Assert.AreEqual("D:\\work\\700001.sql", hydraRingInitializationService.DatabaseCreationScriptFilePath);
            Assert.AreEqual("D:\\work\\700001.log", hydraRingInitializationService.LogFilePath);
            Assert.AreEqual("D:\\work\\designTable.txt", hydraRingInitializationService.OutputFilePath);
            Assert.AreEqual("D:\\work\\temp.sqlite", hydraRingInitializationService.OutputDataBasePath);
            Assert.AreEqual("D:\\hlcd\\HLCD.sqlite", hydraRingInitializationService.HlcdFilePath);

            Assert.AreEqual(Path.Combine(hydraRingDirectory, "MechanismComputation.exe"), hydraRingInitializationService.MechanismComputationExeFilePath);
        }

        [Test]
        public void GenerateInitializationScript_ReturnsExpectedInitializationScript()
        {
            // Setup
            var hydraRingInitializationService = new HydraRingInitializationService(HydraRingFailureMechanismType.DikesPiping, 700001, "D:\\hlcd", "");
            var configurationDatabaseFilePath = Path.Combine(hydraRingDirectory, "config.sqlite");

            var expectedInitializationScript = "section             = 700001" + Environment.NewLine +
                                               "mechanism           = 103" + Environment.NewLine +
                                               "alternative         = 1" + Environment.NewLine +
                                               "layer               = 1" + Environment.NewLine +
                                               "logfile             = 700001.log" + Environment.NewLine +
                                               "outputverbosity     = basic" + Environment.NewLine +
                                               "outputtofile        = file" + Environment.NewLine +
                                               "projectdbfilename   = 700001.sql" + Environment.NewLine +
                                               "outputfilename      = designTable.txt" + Environment.NewLine +
                                               "configdbfilename    = " + configurationDatabaseFilePath + Environment.NewLine +
                                               "hydraulicdbfilename = D:\\hlcd\\HLCD.sqlite";

            // Call
            hydraRingInitializationService.WriteInitializationScript();

            // Assert
            var initializationScript = File.ReadAllText(hydraRingInitializationService.IniFilePath);
            Assert.AreEqual(expectedInitializationScript, initializationScript);
            File.Delete(hydraRingInitializationService.IniFilePath);
        }
    }
}