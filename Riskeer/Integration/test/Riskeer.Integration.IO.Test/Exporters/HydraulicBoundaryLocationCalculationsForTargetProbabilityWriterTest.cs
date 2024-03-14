// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
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
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                null, assessmentSection, string.Empty, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, string.Empty, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), assessmentSection, null, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void WriteHydraulicBoundaryLocationCalculations_InvalidHydraulicBoundaryLocationCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType hydraulicBoundaryLocationCalculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), assessmentSection, string.Empty, hydraulicBoundaryLocationCalculationsType);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({hydraulicBoundaryLocationCalculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);

            mocks.VerifyAll();
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

            var location = new TestHydraulicBoundaryLocation("location 1");
            var calculation = new HydraulicBoundaryLocationCalculation(location)
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(0.1)
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = "Just/A/HRD/File",
                        Locations =
                        {
                            location
                        }
                    }
                }
            });
            mocks.ReplayAll();

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, false);

            try
            {
                // Call
                HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter.WriteHydraulicBoundaryLocationCalculations(new[]
                {
                    calculation
                }, assessmentSection, filePath, calculationsType);

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, fileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter)),
                    expectedExportFileName, 28, 8, 915);
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }

            mocks.VerifyAll();
        }
    }
}