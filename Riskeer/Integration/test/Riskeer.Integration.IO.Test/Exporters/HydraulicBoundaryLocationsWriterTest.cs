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
using Ringtoets.Common.Util;
using Ringtoets.Common.Util.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationsWriterTest
    {
        [Test]
        public void WriteHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("WriteHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException",
                                                                        "test.shp"));
            // Call
            TestDelegate call = () => HydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocations_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(
                new AggregatedHydraulicBoundaryLocation[0], null);

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

            var waterLevelCalculationForFactorizedSignalingNorm = (RoundedDouble) 0.1;
            var waterLevelCalculationForSignalingNorm = (RoundedDouble) 0.2;
            var waterLevelCalculationForLowerLimitNorm = (RoundedDouble) 0.4;
            var waterLevelCalculationForFactorizedLowerLimitNorm = (RoundedDouble) 0.5;
            var waveHeightCalculationForFactorizedSignalingNorm = (RoundedDouble) 0.6;
            var waveHeightCalculationForSignalingNorm = (RoundedDouble) 0.7;
            var waveHeightCalculationForLowerLimitNorm = (RoundedDouble) 0.9;
            var waveHeightCalculationForFactorizedLowerLimitNorm = (RoundedDouble) 1.0;

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, false);

            AggregatedHydraulicBoundaryLocation location = AggregatedHydraulicBoundaryLocationTestHelper.Create(
                waterLevelCalculationForFactorizedSignalingNorm,
                waterLevelCalculationForSignalingNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForFactorizedSignalingNorm,
                waveHeightCalculationForSignalingNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm);

            try
            {
                // Call
                HydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(new[]
                {
                    location
                }, filePath);

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(directoryPath,
                                                                 baseName,
                                                                 Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO),
                                                                              nameof(HydraulicBoundaryLocationsWriter)),
                                                                 "ExpectedExport",
                                                                 28,
                                                                 8,
                                                                 2637);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}