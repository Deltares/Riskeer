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
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class ReliabilityIndexCalculationParserTest
    {
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "ReliabilityIndexCalculationParser");

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new ReliabilityIndexCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_OutputNull()
        {
            // Setup
            var reliabilityIndexCalculationExceptionParser = new ReliabilityIndexCalculationParser();

            // Call
            TestDelegate test = () => reliabilityIndexCalculationExceptionParser.Parse(testDataPath, 1);

            // Assert
            Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsNull(reliabilityIndexCalculationExceptionParser.Output);
        }

        [Test]
        public void Parse_EmptyOutputFile_OutputNull()
        {
            // Setup
            var reliabilityIndexCalculationExceptionParser = new ReliabilityIndexCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "empty");

            // Call
            reliabilityIndexCalculationExceptionParser.Parse(workingDirectory, 1);

            // Assert
            Assert.IsNull(reliabilityIndexCalculationExceptionParser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, HydraRingFileConstants.DesignTablesFileName)));
        }

        [Test]
        [TestCase(1, 1.1, 11.11)]
        [TestCase(3, 3.3, 33.33)]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_ReturnsExpectedReliabilityIndexCalculationResult(int sectionId, double result, double actual)
        {
            // Setup
            var reliabilityIndexCalculationExceptionParser = new ReliabilityIndexCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "exampleOutputTable");

            // Call
            reliabilityIndexCalculationExceptionParser.Parse(workingDirectory, sectionId);

            // Assert
            var reliabilityIndexCalculationOutput = reliabilityIndexCalculationExceptionParser.Output;
            Assert.IsNotNull(reliabilityIndexCalculationOutput);
            Assert.AreEqual(result, reliabilityIndexCalculationOutput.Result);
            Assert.AreEqual(actual, reliabilityIndexCalculationOutput.CalculatedReliabilityIndex);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, HydraRingFileConstants.DesignTablesFileName)));
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileContainingExtraWhiteLine_ReturnsExpectedReliabilityIndexCalculationResult()
        {
            // Setup
            var reliabilityIndexCalculationExceptionParser = new ReliabilityIndexCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "exampleOutputTableWithWhiteLine");

            // Call
            reliabilityIndexCalculationExceptionParser.Parse(workingDirectory, 1);

            // Assert
            var reliabilityIndexCalculationOutput = reliabilityIndexCalculationExceptionParser.Output;
            Assert.IsNotNull(reliabilityIndexCalculationOutput);
            Assert.AreEqual(1.1, reliabilityIndexCalculationOutput.Result);
            Assert.AreEqual(11.11, reliabilityIndexCalculationOutput.CalculatedReliabilityIndex);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, HydraRingFileConstants.DesignTablesFileName)));
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_OutputNull()
        {
            // Setup
            var reliabilityIndexCalculationExceptionParser = new ReliabilityIndexCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "exampleOutputTable");

            // Call
            reliabilityIndexCalculationExceptionParser.Parse(workingDirectory, 2);

            // Assert
            Assert.IsNull(reliabilityIndexCalculationExceptionParser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, HydraRingFileConstants.DesignTablesFileName)));
        }
    }
}