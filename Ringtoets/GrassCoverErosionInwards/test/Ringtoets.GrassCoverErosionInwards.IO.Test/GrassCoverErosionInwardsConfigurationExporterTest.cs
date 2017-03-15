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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new GrassCoverErosionInwardsConfigurationExporter(Enumerable.Empty<ICalculationBase>(), "test.xml");

            // Assert
            Assert.IsInstanceOf<
                ConfigurationExporter<
                    GrassCoverErosionInwardsCalculationConfigurationWriter,
                    GrassCoverErosionInwardsCalculation>>(exporter);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(Export_ValidData_ReturnTrueAndWritesFile)}.xml");

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 1"
            };
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

            var exporter = new GrassCoverErosionInwardsConfigurationExporter(new[]
            {
                calculationGroup
            }, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(
                    TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                    Path.Combine(nameof(GrassCoverErosionInwardsConfigurationExporter), "folderWithSubfolderAndCalculation.xml"));

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
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");

                var exporter = new GrassCoverErosionInwardsConfigurationExporter(new[]
                {
                    new GrassCoverErosionInwardsCalculation()
                }, filePath);

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