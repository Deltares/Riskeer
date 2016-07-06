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
    public class TargetProbabilityCalculationParserTest
    {
        private const string workingDirectory = "tempDir";
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "TargetProbabilityCalculationParser");

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new TargetProbabilityCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_OutputNull()
        {
            // Setup
            var targetProbabilityCalculationParser = new TargetProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                // Call
                targetProbabilityCalculationParser.Parse(workingDirectory, 1);
            }

            // Assert
            Assert.IsNull(targetProbabilityCalculationParser.Output);
        }

        [Test]
        public void Parse_EmptyOutputFile_OutputNull()
        {
            // Setup
            var targetProbabilityCalculationParser = new TargetProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                CopyTestInputToTemporaryOutput("empty.txt");

                // Call
                targetProbabilityCalculationParser.Parse(workingDirectory, 1);
            }

            // Assert
            Assert.IsNull(targetProbabilityCalculationParser.Output);
        }

        [Test]
        [TestCase(1, 1.1, 11.11)]
        [TestCase(3, 3.3, 33.33)]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_ReturnsExpectedTargetProbabilityCalculationResult(int sectionId, double result, double actual)
        {
            // Setup
            var targetProbabilityCalculationParser = new TargetProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                CopyTestInputToTemporaryOutput("exampleOutputTable.txt");

                // Call
                targetProbabilityCalculationParser.Parse(workingDirectory, sectionId);
            }

            // Assert
            var targetProbabilityCalculationOutput = targetProbabilityCalculationParser.Output;
            Assert.IsNotNull(targetProbabilityCalculationOutput);
            Assert.AreEqual(result, targetProbabilityCalculationOutput.Result);
            Assert.AreEqual(actual, targetProbabilityCalculationOutput.ActualTargetProbability);
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_OutputNull()
        {
            // Setup
            var targetProbabilityCalculationParser = new TargetProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                CopyTestInputToTemporaryOutput("exampleOutputTable.txt");

                // Call
                targetProbabilityCalculationParser.Parse(workingDirectory, 2);
            }

            // Assert
            Assert.IsNull(targetProbabilityCalculationParser.Output);
        }

        /// <summary>
        /// Copies the testfile from the test directory to the working directory.
        /// </summary>
        /// <param name="testFile">The name of the test's input file.</param>
        /// <remarks>The copied file is removed from the working directory by using the <see cref="DirectoryDisposeHelper"/>,
        /// which recursively removes all files in the directory.</remarks>
        private void CopyTestInputToTemporaryOutput(string testFile)
        {
            var inputFilePath = Path.Combine(testDataPath, testFile);
            var outputFilePath = Path.Combine(workingDirectory, HydraRingFileName.DesignTablesFileName);
            File.Copy(inputFilePath, outputFilePath);
        }
    }
}