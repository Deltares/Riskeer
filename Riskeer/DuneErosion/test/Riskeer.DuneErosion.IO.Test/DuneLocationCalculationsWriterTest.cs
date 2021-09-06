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
using System.Security.AccessControl;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationCalculationsWriterTest
    {
        [Test]
        public void WriteDuneLocationCalculations_ExportableDuneLocationCalculationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("exportableDuneLocationCalculations", exception.ParamName);
        }

        [Test]
        public void WriteDuneLocationCalculations_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(Enumerable.Empty<ExportableDuneLocationCalculation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteDuneLocationCalculations_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(Enumerable.Empty<ExportableDuneLocationCalculation>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteDuneLocationCalculations_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(Enumerable.Empty<ExportableDuneLocationCalculation>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteDuneLocationCalculations_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteDuneLocationCalculations_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(WriteDuneLocationCalculations_InvalidDirectoryRights_ThrowCriticalFileWriteException)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(Enumerable.Empty<ExportableDuneLocationCalculation>(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
            }
        }

        [Test]
        public void WriteDuneLocationCalculations_FileInUse_ThrowCriticalFileWriteException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(WriteDuneLocationCalculations_FileInUse_ThrowCriticalFileWriteException));

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                void Call() => DuneLocationCalculationsWriter.WriteDuneLocationCalculations(Enumerable.Empty<ExportableDuneLocationCalculation>(), path);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{path}'.", exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void WriteDuneLocationCalculations_ValidData_ValidFile()
        {
            // Setup
            var calculationWithoutOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(9, 9740, 1.9583e-4)),
                0.5);

            var calculationWithUncalculatedOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(10, 9770.1, 1.9583e-4))
                {
                    Output = CreateDuneLocationCalculationOutputForExport(double.NaN, double.NaN, double.NaN)
                },
                0.25);

            var calculationWithOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(11, 9771.34, 1.337e-4))
                {
                    Output = CreateDuneLocationCalculationOutputForExport(5.89, 14.11, 8.53)
                },
                0.1);

            ExportableDuneLocationCalculation[] exportableDuneLocationCalculations =
            {
                calculationWithoutOutput,
                calculationWithUncalculatedOutput,
                calculationWithOutput
            };

            string directoryPath = TestHelper.GetScratchPadPath("WriteDuneLocationCalculations_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.bnd");

            try
            {
                // Call
                DuneLocationCalculationsWriter.WriteDuneLocationCalculations(exportableDuneLocationCalculations,
                                                                             filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Kv\tNr\tRp\tHs\tTp\tTm-1,0\tD50\t_WBI2023_Waarde{Environment.NewLine}" +
                                      $"*Kustvaknummer\tMetrering\tRekenpeil\tSignificante golfhoogte\tPiekperiode\tSpectrale periode\tKorreldiameter\tPfdsn{Environment.NewLine}" +
                                      $"*[-]\t[dam]\t[m+NAP]\t[m]\t[s]\t[s]\t[m]\t[1/jaar]{Environment.NewLine}" +
                                      $"9\t9740\t*\t*\t*\t*\t0.000196\t0.5{Environment.NewLine}" +
                                      $"10\t9770.1\t*\t*\t*\t*\t0.000196\t0.25{Environment.NewLine}" +
                                      $"11\t9771.3\t5.89\t8.53\t14.11\t*\t0.000134\t0.1{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        private static DuneLocationCalculationOutput CreateDuneLocationCalculationOutputForExport(double waterLevel, double wavePeriod, double waveHeight)
        {
            return new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
            {
                WaterLevel = waterLevel,
                WavePeriod = wavePeriod,
                WaveHeight = waveHeight
            });
        }

        private static DuneLocation CreateDuneLocationForExport(int coastalAreaId, double offset, double d50)
        {
            return new DuneLocation(0, string.Empty, new Point2D(0.0, 0.0), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = coastalAreaId,
                Offset = offset,
                D50 = d50
            });
        }
    }
}