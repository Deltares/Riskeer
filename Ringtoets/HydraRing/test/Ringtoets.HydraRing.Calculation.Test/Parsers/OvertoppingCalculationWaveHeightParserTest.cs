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
using System.Data.SQLite;
using System.IO;
using System.Security.AccessControl;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class OvertoppingCalculationWaveHeightParserTest
    {
        private const string validFileOvertoppingDominant = "ValidFileOvertoppingDominant";

        private static readonly string testDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"),
                                                                    nameof(OvertoppingCalculationWaveHeightParser));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_WorkingDirectoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();

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
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        [TestCase("InvalidFile")]
        [TestCase("MissingTableDesignBeta")]
        [TestCase("MissingTableDesignPointResults")]
        [TestCase("MissingTableGoverningWind")]
        public void Parse_WithWorkingDirectoryWithInvalidOutputFile_ThrowsHydraRingFileParserException(string subFolder)
        {
            // Setup
            string path = Path.Combine(testDirectory, subFolder);
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        [TestCase("EmptyDatabase")]
        [TestCase("EmptyTableDesignBeta")]
        [TestCase("EmptyTableGoverningWind")]
        public void Parse_WithDataNotComplete_ThrowsHydraRingFileParserException(string subFolder)
        {
            // Setup
            string path = Path.Combine(testDirectory, subFolder);
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor overslag en overloop gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
        }

        [Test]
        public void Parse_ValidDataForOtherSection_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "OtherSection");
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er is geen resultaat voor overslag en overloop gevonden in de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
        }

        [Test]
        public void Parse_ErrorWhileReadingFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();
            string workingDirectory = Path.Combine(testDirectory, validFileOvertoppingDominant);

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

        [Test]
        [TestCase(validFileOvertoppingDominant, 0.265866, true)]
        [TestCase("ValidFileOvertoppingNotDominant", 0.000355406, false)]
        public void Parse_ValidData_OutputSet(string file, double expectedWaveHeight, bool expectedOvertoppingDominant)
        {
            // Setup
            string path = Path.Combine(testDirectory, file);
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            Assert.AreEqual(expectedWaveHeight, parser.Output.WaveHeight, 1e-11);
            Assert.AreEqual(expectedOvertoppingDominant, parser.Output.IsOvertoppingDominant);
        }

        [Test]
        public void Parse_WaveHeightNull_OutputSet()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidFileWaveHeightNull");
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            Assert.IsNaN(parser.Output.WaveHeight);
            Assert.IsFalse(parser.Output.IsOvertoppingDominant);
        }
    }
}