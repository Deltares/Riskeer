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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationParserTest
    {
        private const string workingDirectory = "tempDir";
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "ExceedanceProbabilityCalculationParser");

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new ExceedanceProbabilityCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_OutputNull()
        {
            // Setup
            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                // Call
                exceedanceProbabilityCalculationParser.Parse(workingDirectory, 1);
            }

            // Assert
            Assert.IsNull(exceedanceProbabilityCalculationParser.Output);
        }

        [Test]
        public void Parse_EmptyOutputFile_OutputNull()
        {
            // Setup
            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();
            var testFile = "empty.sqlite";

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                CopyTestInputToTemporaryOutput(testFile);

                // Call
                exceedanceProbabilityCalculationParser.Parse(workingDirectory, 1);

                // Assert
                Assert.IsNull(exceedanceProbabilityCalculationParser.Output);
                Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, testFile)));
            }
        }

        [Test]
        public void Parse_ExampleCompleteOutputFile_ExpectedExceedanceProbabilityCalculationOutputSet()
        {
            // Setup
            var ringCombinMethod = 0;
            var presentationSectionId = 1;
            var mainMechanismId = 101;
            var mainMechanismCombinMethod = 0;
            var mechanismId = 101;
            var sectionId = 35;
            var layerId = 1;
            var alternativeId = 1;
            var beta = 2.74030893482198;
            var alphaValues = new List<ExceedanceProbabilityCalculationAlphaOutput>
            {
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0, 1, -0.414848705277957),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0, 23, -0.499651535355214),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0, 51, -0.580660162401853),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 1, 0, 0.463049288940854),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 10, 0, 0.0434055709671213),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 11, 0, 0.150241106274945),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 17, 0, 0.0470275786268176)
            };

            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();
            var testFile = "complete.sqlite";

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                CopyTestInputToTemporaryOutput(testFile);

                // Call
                exceedanceProbabilityCalculationParser.Parse(workingDirectory, sectionId);

                // Assert
                ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = exceedanceProbabilityCalculationParser.Output;
                Assert.IsNotNull(exceedanceProbabilityCalculationOutput);
                Assert.AreEqual(ringCombinMethod, exceedanceProbabilityCalculationOutput.RingCombinMethod);
                Assert.AreEqual(presentationSectionId, exceedanceProbabilityCalculationOutput.PresentationSectionId);
                Assert.AreEqual(mainMechanismId, exceedanceProbabilityCalculationOutput.MainMechanismId);
                Assert.AreEqual(mainMechanismCombinMethod, exceedanceProbabilityCalculationOutput.MainMechanismCombinMethod);
                Assert.AreEqual(mechanismId, exceedanceProbabilityCalculationOutput.MechanismId);
                Assert.AreEqual(layerId, exceedanceProbabilityCalculationOutput.LayerId);
                Assert.AreEqual(alternativeId, exceedanceProbabilityCalculationOutput.AlternativeId);
                Assert.AreEqual(beta, exceedanceProbabilityCalculationOutput.Beta);

                Assert.AreEqual(alphaValues.Count, exceedanceProbabilityCalculationOutput.Alphas.Count);
                for (var i = 0; i < alphaValues.Count; i++)
                {
                    var expectedAlpha = alphaValues[i];
                    var actualAlpha = exceedanceProbabilityCalculationOutput.Alphas[i];

                    Assert.AreEqual(expectedAlpha.RingCombinMethod, actualAlpha.RingCombinMethod);
                    Assert.AreEqual(expectedAlpha.PresentationSectionId, actualAlpha.PresentationSectionId);
                    Assert.AreEqual(expectedAlpha.MainMechanismId, actualAlpha.MainMechanismId);
                    Assert.AreEqual(expectedAlpha.MainMechanismCombinMethod, actualAlpha.MainMechanismCombinMethod);
                    Assert.AreEqual(expectedAlpha.MechanismId, actualAlpha.MechanismId);
                    Assert.AreEqual(expectedAlpha.LayerId, actualAlpha.LayerId);
                    Assert.AreEqual(expectedAlpha.AlternativeId, actualAlpha.AlternativeId);
                    Assert.AreEqual(expectedAlpha.VariableId, actualAlpha.VariableId);
                    Assert.AreEqual(expectedAlpha.LoadVariableId, actualAlpha.LoadVariableId);
                    Assert.AreEqual(expectedAlpha.Alpha, actualAlpha.Alpha);
                }

                Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, testFile)));
            }
        }

        [Test]
        public void Parse_FileWithoutTableAlphaResults_OutputNull()
        {
            // Setup
            var testFile = "withoutAlphaResults.sqlite";
            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                // Call
                exceedanceProbabilityCalculationParser.Parse(workingDirectory, 1);

                // Assert
                Assert.IsNull(exceedanceProbabilityCalculationParser.Output);
                Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, testFile)));
            }
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_OutputNull()
        {
            // Setup
            var sectionId = 1;
            var testFile = "complete.sqlite";
            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();

            using (new DirectoryDisposeHelper(workingDirectory))
            {
                // Call
                exceedanceProbabilityCalculationParser.Parse(workingDirectory, sectionId);

                // Assert
                Assert.IsNull(exceedanceProbabilityCalculationParser.Output);
                Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, testFile)));
            }
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
            var outputFilePath = Path.Combine(workingDirectory, HydraRingFileName.OutputDatabaseFileName);
            File.Copy(inputFilePath, outputFilePath);
        }
    }
}