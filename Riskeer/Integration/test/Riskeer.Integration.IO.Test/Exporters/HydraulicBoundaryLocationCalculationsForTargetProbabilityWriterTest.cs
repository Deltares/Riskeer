﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilityWriterTest
    {
        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                null, string.Empty, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_InvalidHydraulicBoundaryLocationCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType hydraulicBoundaryLocationCalculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), string.Empty, hydraulicBoundaryLocationCalculationsType);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({hydraulicBoundaryLocationCalculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "ExpectedWaterLevelExport")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "ExpectedWaveHeightExport")]
        public void WriteHydraulicBoundaryLocationCalculations_ValidData_WritesShapeFile(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                         string expectedExportFileName)
        {
            // Setup
            const string fileName = "test";
            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteHydraulicBoundaryLocationCalculations_ValidData_WritesShapeFile));
            string filePath = Path.Combine(directoryPath, $"{fileName}.shp");

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, false);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("location 1"))
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(0.1)
            };

            try
            {
                // Call
                HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(new[]
                {
                    calculation
                }, filePath, calculationsType);

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, fileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter)),
                    expectedExportFileName, 28, 8, 628);
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }
        }
    }
}