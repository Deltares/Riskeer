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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Writers
{
    [TestFixture]
    public class XmlWriterExtensionsTest
    {
        private static readonly string testDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(CalculationConfigurationWriter<ICalculation>));

        public static IEnumerable<TestCaseData> GetDistributions()
        {
            yield return new TestCaseData(
                    new MeanStandardDeviationStochastConfiguration(),
                    "distributionNoMeanNoStandardDeviation.xml")
                .SetName("Distribution with no mean and no standard deviation.");

            yield return new TestCaseData(
                    new MeanStandardDeviationStochastConfiguration
                    {
                        StandardDeviation = 0.1
                    },
                    "distributionNoMean.xml")
                .SetName("Distribution with no mean.");

            yield return new TestCaseData(
                    new MeanStandardDeviationStochastConfiguration
                    {
                        Mean = 0.2
                    },
                    "distributionNoStandardDeviation.xml")
                .SetName("Distribution with no standard deviation.");
            yield return new TestCaseData(
                    new MeanStandardDeviationStochastConfiguration
                    {
                        Mean = 0.2,
                        StandardDeviation = 0.1
                    },
                    "distribution.xml")
                .SetName("Distribution with mean and standard deviation.");
        }

        public static IEnumerable<TestCaseData> GetVariationCoefficientDistributions()
        {
            yield return new TestCaseData(
                    new MeanVariationCoefficientStochastConfiguration(),
                    "variationCoefficientDistributionNoMeanNoVariationCoefficient.xml")
                .SetName("Variation coefficient distribution with no mean and no variation coefficient.");

            yield return new TestCaseData(
                    new MeanVariationCoefficientStochastConfiguration
                    {
                        VariationCoefficient = 0.1
                    },
                    "variationCoefficientDistributionNoMean.xml")
                .SetName("Variation coefficient distribution with no mean.");

            yield return new TestCaseData(
                    new MeanVariationCoefficientStochastConfiguration
                    {
                        Mean = 0.2
                    },
                    "variationCoefficientDistributionNoVariationCoefficient.xml")
                .SetName("Variation coefficient distribution with no variation coefficient.");
            yield return new TestCaseData(
                    new MeanVariationCoefficientStochastConfiguration
                    {
                        Mean = 0.2,
                        VariationCoefficient = 0.1
                    },
                    "variationCoefficientDistribution.xml")
                .SetName("Variation coefficient distribution with mean and variation coefficient.");
        }

        public static IEnumerable<TestCaseData> GetWaveReductions()
        {
            yield return new TestCaseData(
                    new WaveReductionConfiguration(),
                    "waveReductionWithoutParameters.xml")
                .SetName("Wave reduction without any of its paramters set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ReadBreakWaterType.Dam,
                        BreakWaterHeight = 2.33,
                        UseForeshoreProfile = false
                    },
                    "waveReduction.xml")
                .SetName("Wave reduction with all its paramters set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ReadBreakWaterType.Caisson,
                        UseForeshoreProfile = false
                    },
                    "waveReductionWithoutBreakWaterHeight.xml")
                .SetName("Wave reduction without break water height set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = false,
                        BreakWaterHeight = 12.66,
                        UseForeshoreProfile = true
                    },
                    "waveReductionWithoutBreakWaterType.xml")
                .SetName("Wave reduction without break water type set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        BreakWaterType = ReadBreakWaterType.Wall,
                        BreakWaterHeight = 23.4,
                        UseForeshoreProfile = false
                    },
                    "waveReductionWithoutUseBreakWater.xml")
                .SetName("Wave reduction without use break water set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ReadBreakWaterType.Dam,
                        BreakWaterHeight = 0.2,
                    },
                    "waveReductionWithoutUseForeshoreProfile.xml")
                .SetName("Wave reduction without use foreshore profile set.");
        }

        [Test]
        [TestCaseSource(nameof(GetDistributions))]
        public void WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters(MeanStandardDeviationStochastConfiguration distribution, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters));
            const string name = "normal";

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    xmlWriter.WriteDistribution(name, distribution);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent(fileName);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetVariationCoefficientDistributions))]
        public void WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters(MeanVariationCoefficientStochastConfiguration distribution, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters));
            const string name = "variation coefficient normal";

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    xmlWriter.WriteDistribution(name, distribution);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent(fileName);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetWaveReductions))]
        public void WriteWaveReduction_WithoutDifferentSetParameters_WritesStochastWithSetParameters(WaveReductionConfiguration waveReduction, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWaveReduction_WithoutDifferentSetParameters_WritesStochastWithSetParameters));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    xmlWriter.WriteWaveReduction(waveReduction);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent(fileName);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private string GetTestFileContent(string testFile)
        {
            return File.ReadAllText(Path.Combine(testDirectory, testFile));
        }

        private static XmlWriter CreateXmlWriter(string filePath)
        {
            return XmlWriter.Create(filePath, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment
            });
        }
    }
}