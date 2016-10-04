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
    public class HydraRingOutputFileParserTest
    {
        private readonly string testDataDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation,
                                                                               Path.Combine("Parsers", "OutputFileParser"));

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var outputFileParser = new HydraRingOutputFileParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(outputFileParser);
        }

        [Test]
        public void Parse_NotExistingWorkingDirectory_ThrowsHydraRingFileParseException()
        {
            // Setup
            var outputFileParser = new HydraRingOutputFileParser();
            var outputFileNameOnError = "1.log";
            var nonExistentDirectory = "c:/niet_bestaande_map";

            // Call
            TestDelegate call = () => outputFileParser.Parse(nonExistentDirectory, 1);

            // Assert
            var message = Assert.Throws<HydraRingFileParserException>(call).Message;
            var expectedMessage = string.Format("Kan het Hydra-Ring uitvoerbestand {0} noch het logbestand {1} lezen uit de map {2}.",
                                                "1-output.txt", outputFileNameOnError, nonExistentDirectory);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Parse_NotExistingOutputFiles_ThrowsHydraRingFileParseException()
        {
            // Setup
            var outputFileParser = new HydraRingOutputFileParser();

            // Call
            TestDelegate call = () => outputFileParser.Parse(testDataDirectory, 1234567890);

            // Assert
            var message = Assert.Throws<HydraRingFileParserException>(call).Message;
            var outputFileNameOnError = "1234567890.log";
            var expectedMessage = string.Format("Kan het Hydra-Ring uitvoerbestand {0} noch het logbestand {1} lezen uit de map {2}.",
                                                "1234567890-output.txt", outputFileNameOnError, testDataDirectory);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Parse_NotExistingOutputFile_ReadLogFile()
        {
            // Setup
            var outputFileParser = new HydraRingOutputFileParser();

            // Call
            outputFileParser.Parse(testDataDirectory, 123);

            // Assert
            Assert.AreEqual("Dit is het log bestand, niet het output bestand.", outputFileParser.OutputFileContent);
        }

        [Test]
        public void Parse_ValidOutputFile_LogInfo()
        {
            // Setup
            var outputFileParser = new HydraRingOutputFileParser();

            // Call
            outputFileParser.Parse(testDataDirectory, 1);

            // Assert
            var expectedMessage = "In dit bestand staan veschillende berichten, welke door Hydra-Ring gegenereerd zijn.";
            Assert.AreEqual(expectedMessage, outputFileParser.OutputFileContent);
        }
    }
}