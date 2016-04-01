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

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class TargetProbabilityCalculationParserTest
    {
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "TargetProbabilityCalculationParser");

        [Test]
        public void Parse_NotExistingOutputFile_ReturnsNull()
        {
            // Call
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "notExisting.txt"), 1);

            // Assert
            Assert.IsNull(targetProbabilityCalculationOutput);
        }

        [Test]
        public void Parse_EmptyOutputFile_ReturnsNull()
        {
            // Call
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "empty.txt"), 1);

            // Assert
            Assert.IsNull(targetProbabilityCalculationOutput);
        }

        [Test]
        [TestCase(1, 1.1, 11.11)]
        [TestCase(3, 3.3, 33.33)]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_ReturnsExpectedTargetProbabilityCalculationResult(int sectionId, double result, double actual)
        {
            // Setup
            var outputFilePath = Path.Combine(testDataPath, "exampleOutputTable.txt");

            // Call
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(outputFilePath, sectionId);

            // Assert
            Assert.IsNotNull(targetProbabilityCalculationOutput);
            Assert.AreEqual(result, targetProbabilityCalculationOutput.Result);
            Assert.AreEqual(actual, targetProbabilityCalculationOutput.ActualTargetProbability);
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_ReturnsNull()
        {
            // Call
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "exampleOutputTable.txt"), 2);

            // Assert
            Assert.IsNull(targetProbabilityCalculationOutput);
        }
    }
}