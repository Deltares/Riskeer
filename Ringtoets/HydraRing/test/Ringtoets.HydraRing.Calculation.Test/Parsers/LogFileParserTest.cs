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

using System;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class LogFileParserTest
    {
        private readonly string testDataDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "LogFileParser");

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new HydraRingLogFileParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
        }

        [Test]
        public void Parse_NotExistingWorkingDirectory_LogError()
        {
            // Setup
            var logFileParser = new HydraRingLogFileParser();
            var logFileName = "1.log";
            var nonExistentDirectory = "c:/niet_bestaande_map";

            // Call
            Action call = () => logFileParser.Parse(nonExistentDirectory, 1);

            // Assert
            var expectedMessage = string.Format("Kan het Hydra-Ring logbestand {0} niet lezen uit de map {1}.", logFileName, nonExistentDirectory);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
        }

        [Test]
        public void Parse_NotExistingLogFile_LogError()
        {
            // Setup
            var logFileParser = new HydraRingLogFileParser();
            var logFileName = "1234567890.log";

            // Call
            Action call = () => logFileParser.Parse(testDataDirectory, 1234567890);

            // Assert
            var expectedMessage = string.Format("Kan het Hydra-Ring logbestand {0} niet lezen uit de map {1}.", new[] { logFileName, testDataDirectory });
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
        }

        [Test]
        public void Parse_ValidLogFile_LogInfo()
        {
            // Setup
            var logFileParser = new HydraRingLogFileParser();

            // Call
           logFileParser.Parse(testDataDirectory, 1);

            // Assert
            var expectedMessage = "In dit bestand staan veschillende log berichten, welke door Hydra-Ring gegenereerd zijn.";
            Assert.AreEqual(expectedMessage, logFileParser.LogFileContent);
        }
    }
}