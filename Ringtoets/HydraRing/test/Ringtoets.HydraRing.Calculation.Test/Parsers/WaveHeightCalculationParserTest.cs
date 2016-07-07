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
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class WaveHeightCalculationParserTest
    {
        private string workingDirectory;
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "WaveHeightCalculationParser");

        [SetUp]
        public void SetUp()
        {
            workingDirectory = Path.GetRandomFileName();
        }

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new WaveHeightCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_OutputNull()
        {
            // Setup
            var parser = new WaveHeightCalculationParser();

            using (new TestDataCopyHelper(testDataPath, workingDirectory))
            {
                // Call
                parser.Parse(workingDirectory, 1);
            }

            // Assert
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_EmptyOutputFile_OutputNull()
        {
            // Setup
            var parser = new WaveHeightCalculationParser();
            var sectionId = 1;

            using (var copyHelper = new TestDataCopyHelper(testDataPath, workingDirectory))
            {
                copyHelper.CopyToTemporaryOutput("empty.txt", GetOutputFileName(sectionId));

                // Call
                parser.Parse(workingDirectory, sectionId);
            }

            // Assert
            Assert.IsNull(parser.Output);
        }

        [Test]
        [TestCase("6-3_0-output.txt", 0.91641, true)]
        [TestCase("6-3_0-output-not-dominant.txt", 0.91641, false)]
        [TestCase("304432-fdir-output.txt", 2.78346, true)]
        [TestCase("304432-form-output.txt", 2.78347, true)]
        [TestCase("304432-form-output-not-dominant.txt", 2.78347, false)]
        [TestCase("700003-fdir-output.txt", 1.04899, true)]
        [TestCase("700003-form-output.txt", 1.04899, true)]
        [TestCase("700003-form-output-not-dominant.txt", 1.04899, false)]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_OutputSetWithExpectedCalculationResult(string testFile, double expected, bool isOvertoppingDominant)
        {
            // Setup
            var parser = new WaveHeightCalculationParser();
            var sectionId = 1;

            using (var copyHelper = new TestDataCopyHelper(testDataPath, workingDirectory))
            {
                copyHelper.CopyToTemporaryOutput(testFile, GetOutputFileName(sectionId));

                // Call
                parser.Parse(workingDirectory, sectionId);
            }

            // Assert
            Assert.AreEqual(expected, parser.Output.WaveHeight);
            Assert.AreEqual(isOvertoppingDominant, parser.Output.IsOvertoppingDominant);
        }

        [Test]
        [TestCase("6-3_0-output-no-overtopping.txt")]
        [TestCase("6-3_0-output-no-overflow.txt")]
        [TestCase("6-3_0-output-invalid-hs.txt")]
        [TestCase("6-3_0-output-invalid-closing.txt")]
        [TestCase("6-3_0-output-invalid-wind.txt")]
        [TestCase("6-3_0-output-invalid-beta.txt")]
        [TestCase("6-3_0-output-no-relevant-overflow.txt")]
        public void Parse_InvalidHydraRingOutputFile_OutputNull(string testFile)
        {
            // Setup
            var parser = new WaveHeightCalculationParser();
            var sectionId = 1;

            using (var copyHelper = new TestDataCopyHelper(testDataPath, workingDirectory))
            {
                copyHelper.CopyToTemporaryOutput(testFile, GetOutputFileName(sectionId));

                // Call
                parser.Parse(workingDirectory, sectionId);
            }

            // Assert
            Assert.IsNull(parser.Output);
        }

        private string GetOutputFileName(int sectionId)
        {
            return string.Format("{0}{1}", sectionId, HydraRingFileName.OutputFileSuffix);
        }
    }
}