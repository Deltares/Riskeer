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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
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
            var targetProbabilityCalculationInput = new AssessmentLevelCalculationInput(999, 10000); // Unknown station id

            // Call
            var targetProbabilityCalculationOutput = hydraRingCalculationService.PerformCalculation(hlcdDirectory, "dummyRingId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput);

            // Assert
            Assert.IsNull(targetProbabilityCalculationOutput);

            var outputFolder = Path.Combine(Path.GetTempPath(), "999");
            Assert.IsTrue(Directory.Exists(outputFolder));
            Assert.IsTrue(File.Exists(Path.Combine(outputFolder, "999.ini")));
            Assert.IsTrue(File.Exists(Path.Combine(outputFolder, "999.sql")));
            Assert.IsTrue(File.Exists(Path.Combine(outputFolder, "temp.sqlite")));
            Assert.IsTrue(File.Exists(Path.Combine(outputFolder, "999.log")));
        }
    }
}
