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
using System.Data.SQLite;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class ConvergenceParserTest
    {
        private const string emptyWorkingDirectory = "EmptyWorkingDirectory";
        private const string invalidFileInDirectory = "InvalidFile";
        private const string emptyFileInDirectory = "EmptyDatabase";
        private const string convergenceOnBetaForSection1 = "ConvergenceOnBetaSection1";
        private const string convergenceOnValueForSection1 = "ConvergenceOnValueSection1";
        private const string convergenceOnBothForSection1 = "ConvergenceOnBothSection1";
        private const string noConvergenceForSection1 = "NoConvergenceSection1";
        private const string convergenceOnAllButLastIterationForSection1 = "ConvergenceOnAllButLastIteration";
        private static readonly string testDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, Path.Combine("Parsers", "ConvergenceParser"));

        [Test]
        public void DefaultConstrutor_Always_CreatesNewHydraRingFileParser()
        {
            // Call
            var parser = new ConvergenceParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_WithoutWorkingDirectory_ThrowsArgumentNullException()
        {
            // Setup
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("workingDirectory", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(@"C:\>")]
        public void Parse_WithInvalidWorkingDirectoryPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(invalidPath, 0);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithoutExpectedFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var path = Path.Combine(testDirectory, emptyWorkingDirectory);
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 0);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithInvalidOutputFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var path = Path.Combine(testDirectory, invalidFileInDirectory);
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 0);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            Assert.AreEqual("Er kon geen resultaat voor convergentie gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithEmptyFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var path = Path.Combine(testDirectory, emptyFileInDirectory);
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 0);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor convergentie gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithFileWithTrueResultForOtherSection_ThrowsHydraRingFileParserException()
        {
            // Setup
            var path = Path.Combine(testDirectory, convergenceOnBetaForSection1);
            var parser = new ConvergenceParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 0);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor convergentie gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        [TestCase(noConvergenceForSection1)]
        [TestCase(convergenceOnAllButLastIterationForSection1)]
        public void Parse_WithWorkingDirectoryWithFileWithFalseResult_SetOutputFalse(string testSubDirectory)
        {
            // Setup
            var path = Path.Combine(testDirectory, testSubDirectory);
            var parser = new ConvergenceParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            Assert.IsFalse(parser.Output.Value);
        }

        [Test]
        [TestCase(convergenceOnBetaForSection1)]
        [TestCase(convergenceOnValueForSection1)]
        [TestCase(convergenceOnBothForSection1)]
        public void Parse_WithWorkingDirectoryWithFileWithTrueResult_SetOutputTrue(string testSubDirectory)
        {
            // Setup
            var path = Path.Combine(testDirectory, testSubDirectory);
            var parser = new ConvergenceParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            Assert.IsTrue(parser.Output.Value);
        }
    }
}