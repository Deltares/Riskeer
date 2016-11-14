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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class OvertoppingCalculationWaveHeightParserTest
    {
        private const int sectionId = 1;
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "OvertoppingCalculationWaveHeightParser");
        private readonly string outputFileName = sectionId + "-output.txt";

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();

            // Call
            TestDelegate test = () => parser.Parse(testDataPath, 1);

            // Assert
            Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsNull(parser.Output);
        }

        [Test]
        [TestCase("6-3_0-output-no-overtopping")]
        [TestCase("6-3_0-output-no-overflow")]
        [TestCase("6-3_0-output-no-governing-wind-direction")]
        [TestCase("empty")]
        public void Parse_PartiallyOrCompletelyEmptyOutputFile_RetunsWithNullOutput(string testDir)
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();
            var workingDirectory = Path.Combine(testDataPath, testDir);

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        [TestCase("6-3_0-output", 0.91641, true)]
        [TestCase("6-3_0-output-not-dominant", 0.91641, false)]
        [TestCase("304432-fdir-output", 2.78346, true)]
        [TestCase("304432-form-output", 2.78347, true)]
        [TestCase("304432-form-output-not-dominant", 2.78347, false)]
        [TestCase("700003-fdir-output", 1.04899, true)]
        [TestCase("700003-form-output", 1.04899, true)]
        [TestCase("700003-form-output-not-dominant", 1.04899, false)]
        [TestCase("10-1-ZW_HR_L_10_8-NTI", 0.06193, false)]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_OutputSetWithExpectedCalculationResult(string testDir, double expected, bool isOvertoppingDominant)
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();
            var workingDirectory = Path.Combine(testDataPath, testDir);

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.AreEqual(expected, parser.Output.WaveHeight);
            Assert.AreEqual(isOvertoppingDominant, parser.Output.IsOvertoppingDominant);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        [TestCase("6-3_0-output-invalid-hs")]
        [TestCase("6-3_0-output-invalid-closing")]
        [TestCase("6-3_0-output-invalid-wind")]
        [TestCase("6-3_0-output-invalid-beta")]
        [TestCase("6-3_0-output-no-relevant-overflow")]
        public void Parse_InvalidHydraRingOutputFile_ThrowsHydraRingFileParserException(string testDir)
        {
            // Setup
            var parser = new OvertoppingCalculationWaveHeightParser();
            var workingDirectory = Path.Combine(testDataPath, testDir);

            // Call
            TestDelegate test = () => parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }
    }
}