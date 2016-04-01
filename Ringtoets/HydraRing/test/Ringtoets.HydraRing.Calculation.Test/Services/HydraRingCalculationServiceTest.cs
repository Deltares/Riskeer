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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingCalculationServiceTest
    {
        [Test]
        public void PerformCalculation_InvalidCalculationInput_ExpectedFilesAreWrittenToFileSystemAndCalculationFailsWithExpectedTargetProbabilityCalculationOutput()
        {
            // Setup
            var hlcdDirectory = "hlcdDirectory"; // Invalid HLCD directory
            var hydraRingCalculationService = new HydraRingCalculationService();
            var incorrectStationId = 999;
            var targetProbabilityCalculationInput = new AssessmentLevelCalculationInput(incorrectStationId, 10000); // Unknown station id
            var outputFolder = Path.Combine(Path.GetTempPath(), "" + incorrectStationId);
            var outputFiles = new[]
            {
                Path.Combine(outputFolder, incorrectStationId + ".ini"),
                Path.Combine(outputFolder, incorrectStationId + ".sql"),
                Path.Combine(outputFolder, "temp.sqlite"),
                Path.Combine(outputFolder, incorrectStationId + ".log")
            };

            using (new FileDisposeHelper(outputFiles))
            {
                // Call
                var targetProbabilityCalculationOutput = hydraRingCalculationService.PerformCalculation(
                    hlcdDirectory, "dummyRingId", HydraRingTimeIntegrationSchemeType.FBC,
                    HydraRingUncertaintiesType.All, targetProbabilityCalculationInput);

                // Assert
                Assert.IsNull(targetProbabilityCalculationOutput);

                Assert.IsTrue(Directory.Exists(outputFolder));
                foreach (var outputFile in outputFiles)
                {
                    Assert.IsTrue(File.Exists(outputFile), outputFile + " does not exist.");
                }
            }
        }

        [Test]
        public void PerformCalculation_InvalidCalculationInput_ExpectedFilesAreWrittenToFileSystemAndCalculationFailsWithExpectedExceedanceProbabilityCalculationOutput()
        {
            // Setup
            var hlcdDirectory = "Invalid HLCD directory";
            var hydraRingCalculationService = new HydraRingCalculationService();
            var incorrectStationId = 999;
            var hydraRingSection = new HydraRingSection(incorrectStationId, "999", 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            var targetProbabilityCalculationInput = new OvertoppingCalculationInput(incorrectStationId, hydraRingSection,
                                                                                    new List<HydraRingProfilePoint>(),
                                                                                    new List<HydraRingForelandPoint>());
            var outputFolder = Path.Combine(Path.GetTempPath(), "" + incorrectStationId);
            var outputFiles = new[]
            {
                Path.Combine(outputFolder, incorrectStationId + ".ini"),
                Path.Combine(outputFolder, incorrectStationId + ".sql"),
                Path.Combine(outputFolder, "temp.sqlite"),
                Path.Combine(outputFolder, incorrectStationId + ".log")
            };

            using (new FileDisposeHelper(outputFiles))
            {
                // Call
                var targetProbabilityCalculationOutput = hydraRingCalculationService.PerformCalculation(
                    hlcdDirectory, "dummyRingId", HydraRingTimeIntegrationSchemeType.FBC,
                    HydraRingUncertaintiesType.All, targetProbabilityCalculationInput);

                // Assert
                Assert.IsNull(targetProbabilityCalculationOutput);
                Assert.IsTrue(Directory.Exists(outputFolder));
                foreach (var outputFile in outputFiles)
                {
                    Assert.IsTrue(File.Exists(outputFile), outputFile + " does not exist.");
                }
            }
        }
    }
}