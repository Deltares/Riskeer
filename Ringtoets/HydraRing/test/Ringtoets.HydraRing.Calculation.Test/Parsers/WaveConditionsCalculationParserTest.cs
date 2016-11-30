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
    public class WaveConditionsCalculationParserTest
    {
        private const int sectionId = 1;
        private readonly string testDataPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"), "WaveConditionsCalculationParser");
        private readonly string outputFileName = sectionId + "-output.txt";

        [Test]
        public void DefaultConstructor_SetDefaultValues()
        {
            // Call
            var parser = new WaveConditionsCalculationParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_NotExistingOutputFile_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();

            // Call
            parser.Parse(testDataPath, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
        }

        [Test]
        public void Parse_EmptyOutputFile_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "empty");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        public void Parse_ValidHydraRingOutputFile_OutputSetWithExpectedCalculationResult()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "valid");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.AreEqual(22.01300, parser.Output.WaveAngle);
            Assert.AreEqual(1.87957, parser.Output.WaveHeight);
            Assert.AreEqual(11.15140, parser.Output.WavePeakPeriod);
            Assert.AreEqual(337.98700, parser.Output.WaveDirection);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        public void Parse_InvalidHydraRingOutputFileWaveAngleMissing_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "output-no-waveAngle");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }


        [Test]
        public void Parse_InvalidHydraRingOutputFileWaveDirectionMissing_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "output-no-waveDirection");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        public void Parse_InvalidHydraRingOutputFileWaveHeightMissing_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "output-no-waveHeight");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }

        [Test]
        public void Parse_InvalidHydraRingOutputFileWavePeakPeriodMissing_OutputNull()
        {
            // Setup
            var parser = new WaveConditionsCalculationParser();
            var workingDirectory = Path.Combine(testDataPath, "output-no-wavePeakPeriod");

            // Call
            parser.Parse(workingDirectory, sectionId);

            // Assert
            Assert.IsNull(parser.Output);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(Path.Combine(workingDirectory, outputFileName)));
        }
    }
}