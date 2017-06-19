// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointsParserTest
    {
        private readonly string testDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"),
                                                             nameof(IllustrationPointsParser));

        [Test]
        public void DefaultConstructor_CreatesNewParserInstance()
        {
            // Call
            var parser = new IllustrationPointsParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
        }

        [Test]
        public void Parse_WorkingDirectoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithoutExpectedFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "EmptyWorkingDirectory");
            var parser = new IllustrationPointsParser();

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
            var parser = new IllustrationPointsParser();

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
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_ValidDataForOtherSection_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidStructuresStabilityOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 2);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_ValidStructuresStabilityData_SetsOutputAsExpected()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidStructuresStabilityOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            GeneralResult generalResult = parser.Output;
            Assert.NotNull(generalResult);
            Assert.NotNull(generalResult.GoverningWind);
            Assert.AreEqual(1.19513, generalResult.Beta);
            Assert.AreEqual(46, generalResult.Stochasts.Count());
            Assert.AreEqual(12, generalResult.IllustrationPoints.Count());
        }

        [Test]
        public void Parse_ValidDesignWaterLevelData_SetsOutputAsExpected()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidDesignWaterLevelOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            GeneralResult generalResult = parser.Output;
            Assert.NotNull(generalResult);
            Assert.NotNull(generalResult.GoverningWind);
            Assert.AreEqual(4.45304, generalResult.Beta);
            Assert.AreEqual(6, generalResult.Stochasts.Count());
            Assert.AreEqual(16, generalResult.IllustrationPoints.Count());
        }
    }
}