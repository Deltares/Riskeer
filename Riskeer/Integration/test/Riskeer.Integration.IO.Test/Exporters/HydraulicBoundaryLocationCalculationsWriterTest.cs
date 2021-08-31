// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsWriterTest
    {
        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsWriter.WriteHydraulicBoundaryLocationCalculations(
                null, string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_MetaDataHeaderNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("metaDataHeader", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void WriteHydraulicBoundaryLocationCalculations_ValidData_WritesShapeFile()
        {
            // Setup
            const string fileName = "test";
            const string metaDataHeader = "header";
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
                HydraulicBoundaryLocationCalculationsWriter.WriteHydraulicBoundaryLocationCalculations(new[]
                {
                    calculation
                }, filePath, metaDataHeader);

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, fileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsWriter)),
                    "ExpectedExport", 28, 8, 628);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}