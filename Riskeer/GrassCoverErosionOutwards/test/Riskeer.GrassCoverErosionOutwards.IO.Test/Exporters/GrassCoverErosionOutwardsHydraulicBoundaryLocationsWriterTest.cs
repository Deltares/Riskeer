// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.IO.Exporters;
using Ringtoets.GrassCoverErosionOutwards.Util;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.IO.Test.Exporters
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriterTest
    {
        [Test]
        public void WriteHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("WriteHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException",
                                                                        "test.shp"));
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocations_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(
                new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void WriteHydraulicBoundaryLocations_ValidData_WritesShapeFile()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath("WriteHydraulicBoundaryLocations_ValidData_WritesShapeFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            const string baseName = "test";

            var waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm = (RoundedDouble) 0.1;
            var waterLevelCalculationForMechanismSpecificSignalingNorm = (RoundedDouble) 0.2;
            var waterLevelCalculationForMechanismSpecificLowerLimitNorm = (RoundedDouble) 0.3;
            var waterLevelCalculationForLowerLimitNorm = (RoundedDouble) 0.4;
            var waterLevelCalculationForFactorizedLowerLimitNorm = (RoundedDouble) 0.5;
            var waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm = (RoundedDouble) 0.6;
            var waveHeightCalculationForMechanismSpecificSignalingNorm = (RoundedDouble) 0.7;
            var waveHeightCalculationForMechanismSpecificLowerLimitNorm = (RoundedDouble) 0.8;
            var waveHeightCalculationForLowerLimitNorm = (RoundedDouble) 0.9;
            var waveHeightCalculationForFactorizedLowerLimitNorm = (RoundedDouble) 1.0;

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, false);

            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location = GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper.Create(
                waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm,
                waterLevelCalculationForMechanismSpecificSignalingNorm,
                waterLevelCalculationForMechanismSpecificLowerLimitNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
                waveHeightCalculationForMechanismSpecificSignalingNorm,
                waveHeightCalculationForMechanismSpecificLowerLimitNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm);

            try
            {
                // Call
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(new[]
                {
                    location
                }, filePath);

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(directoryPath,
                                                                 baseName,
                                                                 Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionOutwards.IO),
                                                                              nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriter)),
                                                                 "ExpectedExport",
                                                                 28,
                                                                 8,
                                                                 3211);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}