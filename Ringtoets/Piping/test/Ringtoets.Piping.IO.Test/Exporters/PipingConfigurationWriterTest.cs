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
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Integration.TestUtils;
using Ringtoets.Piping.IO.Exporters;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Exporters
{
    [TestFixture]
    public class PipingConfigurationWriterTest
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("calculationWithoutHydraulicLocation",
                                              PipingTestDataGenerator.GetPipingCalculationWithoutHydraulicLocationAndAssessmentLevel())
                    .SetName("calculationWithoutHydraulicLocation");
                yield return new TestCaseData("calculationWithAssessmentLevel",
                                              PipingTestDataGenerator.GetPipingCalculationWithAssessmentLevel())
                    .SetName("calculationWithAssessmentLevel");
                yield return new TestCaseData("calculationWithoutSurfaceLine",
                                              PipingTestDataGenerator.GetPipingCalculationWithoutSurfaceLine())
                    .SetName("calculationWithoutSurfaceLine");
                yield return new TestCaseData("calculationWithoutSoilModel",
                                              PipingTestDataGenerator.GetPipingCalculationWithoutSoilModel())
                    .SetName("calculationWithoutSoilModel");
                yield return new TestCaseData("calculationWithoutSoilProfile",
                                              PipingTestDataGenerator.GetPipingCalculationWithoutSoilProfile())
                    .SetName("calculationWithoutSoilProfile");
                yield return new TestCaseData("calculationWithNaNs",
                                              PipingTestDataGenerator.GetPipingCalculationWithNaNs())
                    .SetName("calculationWithNaNs");
                yield return new TestCaseData("calculationWithInfinities",
                                              PipingTestDataGenerator.GetPipingCalculationWithInfinities())
                    .SetName("calculationWithInfinities");
            }
        }

        [Test]
        public void Write_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingConfigurationWriter.Write(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rootCalculationGroup", exception.ParamName);
        }

        [Test]
        public void Write_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingConfigurationWriter.Write(new CalculationGroup(), null);

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
            TestDelegate call = () => PipingConfigurationWriter.Write(new CalculationGroup(), filePath);

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
            TestDelegate call = () => PipingConfigurationWriter.Write(new CalculationGroup(), filePath);

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
                TestDelegate call = () => PipingConfigurationWriter.Write(new CalculationGroup(), filePath);
                
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
                TestDelegate call = () => PipingConfigurationWriter.Write(new CalculationGroup(), path);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{path}'.", exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void Write_CalculationGroupsAndCalculation_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculation();
            calculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            calculation.InputParameters.ExitPointL = (RoundedDouble) 0.2;

            PipingCalculation calculation2 = PipingTestDataGenerator.GetPipingCalculation();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_SCH_17", 0, 0);
            calculation2.InputParameters.SurfaceLine.Name = "PK001_0002";
            calculation2.InputParameters.EntryPointL = (RoundedDouble) 0.3;
            calculation2.InputParameters.ExitPointL = (RoundedDouble) 0.4;
            calculation2.InputParameters.StochasticSoilModel = new StochasticSoilModel(1, "PK001_0002_Piping", string.Empty);
            calculation2.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile("W1-6_4_1D1", 0, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var calculationGroup2 = new CalculationGroup("PK001_0002", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("PK001_0001", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            var rootGroup = new CalculationGroup("root", false)
            {
                Children =
                {
                    calculationGroup
                }
            };

            try
            {
                // Call
                PipingConfigurationWriter.Write(rootGroup, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                                        Path.Combine("PipingConfigurationWriter",
                                                                                     "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculationCalculation_ValidFile(string expectedFileName, PipingCalculation calculation)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            var rootCalculationGroup = new CalculationGroup("group", false)
            {
                Children =
                {
                    calculation
                }
            };

            try
            {
                // Call
                PipingConfigurationWriter.Write(rootCalculationGroup, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.Combine("PipingConfigurationWriter", $"{expectedFileName}.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}