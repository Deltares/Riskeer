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
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers");

        [Test]
        public void Parse_NotExistingOutputFile_ReturnsNull()
        {
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "notExisting.txt"), 1);

            Assert.IsNull(targetProbabilityCalculationOutput);
        }

        [Test]
        public void Parse_EmptyOutputFile_ReturnsNull()
        {
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "empty.txt"), 1);

            Assert.IsNull(targetProbabilityCalculationOutput);
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileContainingSectionIds_ReturnsExpectedTargetProbabilityCalculationResult()
        {
            var outputFilePath = Path.Combine(testDataPath, "exampleOutputTable.txt");

            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(outputFilePath, 1);
            Assert.IsNotNull(targetProbabilityCalculationOutput);
            Assert.AreEqual(1.1, targetProbabilityCalculationOutput.Result);
            Assert.AreEqual(11.11, targetProbabilityCalculationOutput.ActualTargetProbability);

            targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(outputFilePath, 3);
            Assert.IsNotNull(targetProbabilityCalculationOutput);
            Assert.AreEqual(3.3, targetProbabilityCalculationOutput.Result);
            Assert.AreEqual(33.33, targetProbabilityCalculationOutput.ActualTargetProbability);
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_ReturnsNull()
        {
            var targetProbabilityCalculationOutput = TargetProbabilityCalculationParser.Parse(Path.Combine(testDataPath, "exampleOutputTable.txt"), 2);

            Assert.IsNull(targetProbabilityCalculationOutput);
        }
    }
}
