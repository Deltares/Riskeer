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
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Export
{
    [TestFixture]
    public class StructureCalculationConfigurationWriterTest
    {
        private static readonly string testDirectory = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            nameof(StructureCalculationConfigurationWriter<StructuresCalculationConfiguration>));

        [Test]
        public void Write_WithoutConfigurations_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            var writer = new SimpleStructureCalculationConfigurationWriter(filePath, false);
            {
                // Call
                TestDelegate test = () => writer.Write(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("configurations", exception.ParamName);
            }
        }

        [Test]
        public void Write_WithAllParametersSet_WritesCalculationWithAllParametersAndStochasts()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Write_WithAllParametersSet_WritesCalculationWithAllParametersAndStochasts));
            try
            {
                var writer = new SimpleStructureCalculationConfigurationWriter(filePath, true);

                // Call
                writer.Write(new[]
                {
                    CreateStructureWithAllParametersSet("some other name")
                });

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent("structureCalculationWithAllParametersSet.xml");
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Write_WithoutParametersSet_WritesCalculationWithOnlyEmptyStochasts()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Write_WithoutParametersSet_WritesCalculationWithOnlyEmptyStochasts));
            try
            {
                var writer = new SimpleStructureCalculationConfigurationWriter(filePath, false);

                // Call
                writer.Write(new[]
                {
                    new SimpleStructuresCalculationConfiguration("some name")
                });

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent("structureCalculationWithoutParametersSet.xml");
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static SimpleStructuresCalculationConfiguration CreateStructureWithAllParametersSet(string name)
        {
            return new SimpleStructuresCalculationConfiguration(name)
            {
                AllowedLevelIncreaseStorage = new StochastConfiguration
                {
                    Mean = 1.2,
                    StandardDeviation = 3.4
                },
                CriticalOvertoppingDischarge = new StochastConfiguration
                {
                    Mean = 22.2,
                    VariationCoefficient = 2.1
                },
                FailureProbabilityStructureWithErosion = 2.1,
                FlowWidthAtBottomProtection = new StochastConfiguration
                {
                    Mean = 5.4,
                    StandardDeviation = 1.1
                },
                ForeshoreProfileId = "Voorland",
                HydraulicBoundaryLocationName = "Belastingenlocatie",
                StorageStructureArea = new StochastConfiguration
                {
                    Mean = 11.122,
                    VariationCoefficient = 32.111
                },
                StormDuration = new StochastConfiguration
                {
                    Mean = 21.22,
                    VariationCoefficient = 1.2
                },
                StructureNormalOrientation = 5.6,
                StructureId = "Kunstwerk",
                WaveReduction = new WaveReductionConfiguration
                {
                    BreakWaterType = ConfigurationBreakWaterType.Caisson,
                    UseBreakWater = false,
                    BreakWaterHeight = 111111.2,
                    UseForeshoreProfile = true
                },
                WidthFlowApertures = new StochastConfiguration
                {
                    Mean = 121.3,
                    StandardDeviation = 222.1
                },
                ShouldIllustrationPointsBeCalculated = true
            };
        }

        private string GetTestFileContent(string testFile)
        {
            return File.ReadAllText(Path.Combine(testDirectory, testFile));
        }

        private class SimpleStructuresCalculationConfiguration : StructuresCalculationConfiguration
        {
            public SimpleStructuresCalculationConfiguration(string name) : base(name) {}
        }

        private class SimpleStructureCalculationConfigurationWriter : StructureCalculationConfigurationWriter<SimpleStructuresCalculationConfiguration>
        {
            private readonly bool writeExtraParameterAndStochast;

            public SimpleStructureCalculationConfigurationWriter(string filePath, bool writeExtraParameterAndStochast) : base(filePath)
            {
                this.writeExtraParameterAndStochast = writeExtraParameterAndStochast;
            }

            protected override void WriteSpecificStructureParameters(SimpleStructuresCalculationConfiguration configuration, XmlWriter writer)
            {
                if (writeExtraParameterAndStochast)
                {
                    writer.WriteElementString("testName", "testValue");
                }
            }

            protected override void WriteSpecificStochasts(SimpleStructuresCalculationConfiguration configuration, XmlWriter writer)
            {
                if (writeExtraParameterAndStochast)
                {
                    writer.WriteDistribution("testStochastName", new StochastConfiguration());
                }
            }
        }
    }
}