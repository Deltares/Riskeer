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

using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationWriterTest
    {
        [Test]
        public void Write_ConfigurationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        public void Write_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(Enumerable.Empty<ICalculationBase>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Write_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void Write_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void Write_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate call = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(Enumerable.Empty<ICalculationBase>(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
            }
        }

        [Test]
        public void Write_FileInUse_ThrowCriticalFileWriteException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(Write_FileInUse_ThrowCriticalFileWriteException));

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(Enumerable.Empty<ICalculationBase>(), path);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{path}'.", exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void WriteConfiguration_SparseCalculation_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteConfiguration_SparseCalculation_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationWriter), "sparseConfiguration.xml"));

            var calculation = new GrassCoverErosionInwardsCalculation()
            {
                Name = "Berekening 1"
            };

            try
            {
                var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter();

                // Call
                writer.Write(new[]
                {
                    calculation
                }, filePath);

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteConfiguration_CompleteCalculation_WritesCompleteConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteConfiguration_CompleteCalculation_WritesCompleteConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationWriter), "completeConfiguration.xml"));

            GrassCoverErosionInwardsCalculation calculation = CreateCompleteCalculation();

            try
            {
                var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter();

                // Call
                writer.Write(new[]
                {
                    calculation
                }, filePath);

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Write_NestedConfiguration_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            GrassCoverErosionInwardsCalculation calculation = CreateCompleteCalculation();
            GrassCoverErosionInwardsCalculation calculation2 = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 2"
            };
            var calculationGroup2 = new CalculationGroup("Nested", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("Testmap", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            try
            {
                // Call
                new GrassCoverErosionInwardsCalculationConfigurationWriter().Write(new[]
                {
                    calculationGroup
                }, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                        Path.Combine("GrassCoverErosionInwardsCalculationConfigurationWriter",
                                                                                     "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static GrassCoverErosionInwardsCalculation CreateCompleteCalculation()
        {
            return new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    DikeProfile = new TestDikeProfile("dijkProfiel"),
                    Orientation = (RoundedDouble) 67.1,
                    UseForeshore = true,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 1.23,
                        Type = BreakWaterType.Caisson
                    },
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) 0.1,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };
        }
    }
}