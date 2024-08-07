﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Data.SQLite;
using System.IO;
using System.Security.AccessControl;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.Parsers;

namespace Riskeer.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationParserTest
    {
        private const string validFile = "ValidFile";

        private readonly string testDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.Calculation, "Parsers"),
                                                             nameof(ExceedanceProbabilityCalculationParser));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var parser = new ExceedanceProbabilityCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_WorkingDirectoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(null, 1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("workingDirectory", exception.ParamName);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithoutExpectedFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "EmptyWorkingDirectory");
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithInvalidOutputFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "InvalidFile");
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithEmptyFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "EmptyDatabase");
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor de betrouwbaarheidsindex van de faalkans gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        public void Parse_WithBetaResultOnOtherSection_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "OtherSection");
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor de betrouwbaarheidsindex van de faalkans gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        public void Parse_ValidFileWithBetaResult_OutputSet()
        {
            // Setup
            string path = Path.Combine(testDirectory, validFile);
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            Assert.AreEqual(3.42848, parser.Output);
        }

        [Test]
        public void Parse_BetaNull_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "BetaNull");
            var parser = new ExceedanceProbabilityCalculationParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor de betrouwbaarheidsindex van de faalkans gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
        }

        [Test]
        public void Parse_ErrorWhileReadingFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var parser = new ExceedanceProbabilityCalculationParser();
            string workingDirectory = Path.Combine(testDirectory, validFile);

            using (new DirectoryPermissionsRevoker(testDirectory, FileSystemRights.ReadData))
            {
                // Call
                TestDelegate call = () => parser.Parse(workingDirectory, 1);

                // Assert
                var exception = Assert.Throws<HydraRingFileParserException>(call);
                const string expectedMessage = "Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }
    }
}