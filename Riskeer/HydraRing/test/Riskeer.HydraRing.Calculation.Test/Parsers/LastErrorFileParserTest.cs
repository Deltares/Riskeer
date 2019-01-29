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
using System.Security.AccessControl;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.Parsers;

namespace Riskeer.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class LastErrorFileParserTest
    {
        private readonly string noErrorTestDataDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.Calculation,
                                                                                      Path.Combine("Parsers", "OutputFileParser"));

        private readonly string lastErrorTestDataDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.Calculation,
                                                                                        Path.Combine("Parsers", "LastErrorFileParser"));

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var lastErrorFileParser = new LastErrorFileParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(lastErrorFileParser);
        }

        [Test]
        public void Parse_NotExistingWorkingDirectory_DoesNotThrowException()
        {
            // Setup
            var lastErrorFileParser = new LastErrorFileParser();
            const string nonExistentDirectory = "c:/niet_bestaande_map";

            // Call
            TestDelegate call = () => lastErrorFileParser.Parse(nonExistentDirectory, 1);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Parse_NotExistingLastErrorOutputFile_DoesNotThrowException()
        {
            // Setup
            var lastErrorFileParser = new LastErrorFileParser();

            // Call
            TestDelegate call = () => lastErrorFileParser.Parse(noErrorTestDataDirectory, 1);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Parse_LastErrorFileExists_LastErrorContentSet()
        {
            // Setup
            var lastErrorFileParser = new LastErrorFileParser();

            // Call
            lastErrorFileParser.Parse(lastErrorTestDataDirectory, 1);

            // Assert
            string expectedContent = " File not found: D:\\Repos\\Ringtoets\\Ringtoets\\Integration\\test\\Ringtoets.Integra"
                                     + Environment.NewLine +
                                     " tion.Service.Test\\test-data\\HLCD.sqlite"
                                     + Environment.NewLine;
            Assert.AreEqual(expectedContent, lastErrorFileParser.ErrorFileContent);
        }

        [Test]
        public void Parse_ErrorWhileReadingFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var lastErrorFileParser = new LastErrorFileParser();

            using (new DirectoryPermissionsRevoker(lastErrorTestDataDirectory, FileSystemRights.ReadData))
            {
                // Call
                TestDelegate call = () => lastErrorFileParser.Parse(lastErrorTestDataDirectory, 1);

                // Assert
                var exception = Assert.Throws<HydraRingFileParserException>(call);
                string expectedMessage = $"Kan het Hydra-Ring last_error bestand {HydraRingFileConstants.LastErrorFileName} " +
                                         $"niet lezen uit de map {lastErrorTestDataDirectory}.";
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }
    }
}