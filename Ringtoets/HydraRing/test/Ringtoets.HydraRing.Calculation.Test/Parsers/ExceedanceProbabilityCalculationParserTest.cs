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

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationParserTest
    {
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "ExceedanceProbabilityCalculationParser");

        [Test]
        public void Parse_NotExistingOutputFile_ReturnsNull()
        {
            // Setup
            ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = null;
            var filePath = Path.Combine(testDataPath, "notExisting.sqlite");

            // Call
            TestDelegate test = () => exceedanceProbabilityCalculationOutput = ExceedanceProbabilityCalculationParser.Parse(filePath, 1);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsNull(exceedanceProbabilityCalculationOutput);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Parse_EmptyOutputFile_ReturnsNull()
        {
            // Setup
            ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = null;
            var filePath = Path.Combine(testDataPath, "empty.sqlite");

            // Call
            TestDelegate test = () => exceedanceProbabilityCalculationOutput = ExceedanceProbabilityCalculationParser.Parse(filePath, 1);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsNull(exceedanceProbabilityCalculationOutput);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Parse_ExampleCompleteOutputFile_ReturnsExpecteExceedanceProbabilityCalculationOutput()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "complete.sqlite");
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
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, -0.414848705277957),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, -0.499651535355214),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, -0.580660162401853),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0.463049288940854),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0.0434055709671213),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0.150241106274945),
                new ExceedanceProbabilityCalculationAlphaOutput(0, 1, 101, 0, 101, 35, 1, 1, 0.0470275786268176)
            };

            // Call
            ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = ExceedanceProbabilityCalculationParser.Parse(filePath, sectionId);

            // Assert
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
                Assert.AreEqual(expectedAlpha.Alpha, actualAlpha.Alpha);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Parse_FileWithoutTableAlphaResults_ReturnsNull()
        {
            // Setup
            ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = null;
            var filePath = Path.Combine(testDataPath, "withoutAlphaResults.sqlite");

            // Call
            TestDelegate test = () => exceedanceProbabilityCalculationOutput = ExceedanceProbabilityCalculationParser.Parse(filePath, 1);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsNull(exceedanceProbabilityCalculationOutput);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Parse_ExampleHydraRingOutputFileNotContainingSectionId_ReturnsNull()
        {
            // Setup
            var sectionId = 1;
            var filePath = Path.Combine(testDataPath, "complete.sqlite");
            ExceedanceProbabilityCalculationOutput exceedanceProbabilityCalculationOutput = null;

            // Call
            TestDelegate test = () => exceedanceProbabilityCalculationOutput = ExceedanceProbabilityCalculationParser.Parse(filePath, sectionId);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsNull(exceedanceProbabilityCalculationOutput);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }
    }
}