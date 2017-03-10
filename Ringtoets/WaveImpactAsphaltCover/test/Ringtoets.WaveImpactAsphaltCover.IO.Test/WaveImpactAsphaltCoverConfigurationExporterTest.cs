﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.IO.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new WaveImpactAsphaltCoverConfigurationExporter(Enumerable.Empty<ICalculationBase>(), "test.xml");

            // Assert
            Assert.IsInstanceOf<
                ConfigurationExporter<
                    WaveImpactAsphaltCoverConfigurationWriter,
                    WaveImpactAsphaltCoverWaveConditionsCalculation>>(exporter);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(Export_ValidData_ReturnTrueAndWritesFile)}.xml");

            var calculation1 = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "Calculation A",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("ForeshoreA")
                }
            };

            var calculation2 = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "PK001_0002 W1-6_4_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HydraulicLocationA", 0, 0),
                    UseBreakWater = true
                }
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
                    calculation1,
                    calculationGroup2
                }
            };

            var exporter = new WaveImpactAsphaltCoverConfigurationExporter(new []
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
                string testDirSubPath = Path.Combine(nameof(WaveImpactAsphaltCoverConfigurationExporter), "fullValidConfiguration.xml");
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.WaveImpactAsphaltCover.IO,
                                                                        testDirSubPath);
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