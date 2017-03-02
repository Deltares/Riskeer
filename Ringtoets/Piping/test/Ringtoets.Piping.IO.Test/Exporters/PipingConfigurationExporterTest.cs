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
using System.Security.AccessControl;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
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
    public class PipingConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new PipingConfigurationExporter(new CalculationGroup(), "test.xml");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingConfigurationExporter(null, "test.xml");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new PipingConfigurationExporter(new CalculationGroup(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(Export_ValidData_ReturnTrueAndWritesFile)}.xml");

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

            var exporter = new PipingConfigurationExporter(rootGroup, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                                        Path.Combine("PipingConfigurationWriter", "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    PipingTestDataGenerator.GetPipingCalculation()
                }
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");

                var exporter = new PipingConfigurationExporter(calculationGroup, filePath);

                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. "
                                         + "Er is geen configuratie geëxporteerd.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                Assert.IsFalse(isExported);
            }
        }
    }
}