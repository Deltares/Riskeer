// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;

namespace Riskeer.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class XmlWriterExtensionsTest
    {
        private static readonly string testDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(XmlWriterExtensions));

        [Test]
        public void WriteStartFolder_WithNameAndWriter_WritesFolderStart()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteStartFolder_WithNameAndWriter_WritesFolderStart));
            const string name = "folder";

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    xmlWriter.WriteStartFolder(name);

                    // Assert
                    Assert.AreEqual(WriteState.Element, xmlWriter.WriteState);
                }

                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent("simpleFolder.xml");
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteStartFolder_WithoutWriter_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate testDelegate = () => ((XmlWriter) null).WriteStartFolder("name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(testDelegate);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteStartFolder_WithoutName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteStartFolder_WithoutName_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteStartFolder(null);

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("name", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteStartFolder_WriterClosed_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteStartFolder_WriterClosed_ThrowsInvalidOperationException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    xmlWriter.Close();

                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteStartFolder("name");

                    // Assert
                    Assert.Throws<InvalidOperationException>(testDelegate);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_StandardDeviationDistributionWithoutWriter_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate testDelegate = () => ((XmlWriter) null).WriteDistribution("name", new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(testDelegate);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteDistribution_StandardDeviationDistributionWithoutName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_StandardDeviationDistributionWithoutName_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteDistribution(null, new StochastConfiguration());

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("name", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_WithoutStandardDeviationDistribution_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithoutStandardDeviationDistribution_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteDistribution("name", null);

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("distribution", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetDistributions))]
        public void WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters(StochastConfiguration distribution, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithDifferentSetParameters_WritesStochastWithSetParameters));
            const string name = "distribution";

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
        public void WriteDistribution_VariationCoefficientDistributionWithoutWriter_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate testDelegate = () => ((XmlWriter) null).WriteDistribution("name", new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(testDelegate);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteDistribution_VariationCoefficientDistributionWithoutName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_VariationCoefficientDistributionWithoutName_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteDistribution(null, new StochastConfiguration());

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("name", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_WithoutVariationCoefficientDistribution_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithoutVariationCoefficientDistribution_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteDistribution("name", null);

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("distribution", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_WriterClosed_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WriterClosed_ThrowsInvalidOperationException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    xmlWriter.Close();

                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteDistribution("name", new StochastConfiguration());

                    // Assert
                    Assert.Throws<InvalidOperationException>(testDelegate);
                }
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
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteWaveReduction_WithoutDifferentSetParameters_WritesStochastWithSetParameters)}.{fileName}");

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

        [Test]
        public void WriteWaveReduction_WithoutWriter_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate testDelegate = () => ((XmlWriter) null).WriteWaveReduction(new WaveReductionConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(testDelegate);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteWaveReduction_WithoutWaveReduction_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWaveReduction_WithoutWaveReduction_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteWaveReduction(null);

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("waveReduction", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteWaveReduction_InvalidBreakWaterType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWaveReduction_WithoutWaveReduction_ThrowsArgumentNullException));
            var configuration = new WaveReductionConfiguration
            {
                BreakWaterType = (ConfigurationBreakWaterType?) 9000
            };

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteWaveReduction(configuration);

                    // Assert
                    Assert.Throws<InvalidEnumArgumentException>(testDelegate);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteWaveReduction_WriterClosed_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWaveReduction_WriterClosed_ThrowsInvalidOperationException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    xmlWriter.Close();

                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteWaveReduction(new WaveReductionConfiguration());

                    // Assert
                    Assert.Throws<InvalidOperationException>(testDelegate);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetScenarioConfigurations))]
        public void WriteScenario_WithoutDifferentSetParameters_WritesExpectedParameters(ScenarioConfiguration configuration, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteScenario_WithoutDifferentSetParameters_WritesExpectedParameters)}.{fileName}");

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    xmlWriter.WriteScenario(configuration);
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
        public void WriteScenario_WithoutWriter_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate testDelegate = () => ((XmlWriter) null).WriteScenario(new ScenarioConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(testDelegate);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteScenario_WithoutScenarioConfiguration_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteScenario_WithoutScenarioConfiguration_ThrowsArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteScenario(null);

                    // Assert
                    var exception = Assert.Throws<ArgumentNullException>(testDelegate);
                    Assert.AreEqual("scenarioConfiguration", exception.ParamName);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteScenario_WriterClosed_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteScenario_WriterClosed_ThrowsInvalidOperationException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    xmlWriter.Close();

                    // Call
                    TestDelegate testDelegate = () => xmlWriter.WriteScenario(new ScenarioConfiguration());

                    // Assert
                    Assert.Throws<InvalidOperationException>(testDelegate);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        public static IEnumerable<TestCaseData> GetDistributions()
        {
            yield return new TestCaseData(
                    new StochastConfiguration(),
                    "distributionEmpty.xml")
                .SetName("Distribution with no parameters.");

            yield return new TestCaseData(
                    new StochastConfiguration
                    {
                        Mean = 0.2
                    },
                    "distributionMean.xml")
                .SetName("Distribution with mean.");

            yield return new TestCaseData(
                    new StochastConfiguration
                    {
                        StandardDeviation = 0.1
                    },
                    "distributionStandardDeviation.xml")
                .SetName("Distribution with standard deviation.");

            yield return new TestCaseData(
                    new StochastConfiguration
                    {
                        Mean = 0.2,
                        StandardDeviation = 0.1
                    },
                    "distributionMeanStandardDeviation.xml")
                .SetName("Distribution with mean and standard deviation.");

            yield return new TestCaseData(
                    new StochastConfiguration
                    {
                        VariationCoefficient = 0.1
                    },
                    "distributionVariationCoefficient.xml")
                .SetName("Distribution with variation coefficient.");

            yield return new TestCaseData(
                    new StochastConfiguration
                    {
                        Mean = 0.2,
                        VariationCoefficient = 0.1
                    },
                    "distributionMeanVariationCoefficient.xml")
                .SetName("Distribution with mean and variation coefficient.");
        }

        public static IEnumerable<TestCaseData> GetWaveReductions()
        {
            yield return new TestCaseData(
                    new WaveReductionConfiguration(),
                    "waveReductionWithoutParameters.xml")
                .SetName("Wave reduction without any of its parameters set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ConfigurationBreakWaterType.Dam,
                        BreakWaterHeight = 2.33,
                        UseForeshoreProfile = false
                    },
                    "waveReduction.xml")
                .SetName("Wave reduction with all its parameters set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ConfigurationBreakWaterType.Caisson,
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
                        BreakWaterType = ConfigurationBreakWaterType.Wall,
                        BreakWaterHeight = 23.4,
                        UseForeshoreProfile = false
                    },
                    "waveReductionWithoutUseBreakWater.xml")
                .SetName("Wave reduction without use break water set.");

            yield return new TestCaseData(
                    new WaveReductionConfiguration
                    {
                        UseBreakWater = true,
                        BreakWaterType = ConfigurationBreakWaterType.Dam,
                        BreakWaterHeight = 0.2
                    },
                    "waveReductionWithoutUseForeshoreProfile.xml")
                .SetName("Wave reduction without use foreshore profile set.");
        }

        public static IEnumerable<TestCaseData> GetScenarioConfigurations()
        {
            const string testNameFormat = "{m}({1:40})";

            yield return new TestCaseData(
                    new ScenarioConfiguration(),
                    "scenarioConfigurationWithoutParameters.xml")
                .SetName(testNameFormat);

            yield return new TestCaseData(
                    new ScenarioConfiguration
                    {
                        IsRelevant = true,
                        Contribution = 0.8
                    },
                    "scenarioConfiguration.xml")
                .SetName(testNameFormat);

            yield return new TestCaseData(
                    new ScenarioConfiguration
                    {
                        IsRelevant = true
                    },
                    "scenarioConfigurationWithoutContribution.xml")
                .SetName(testNameFormat);

            yield return new TestCaseData(
                    new ScenarioConfiguration
                    {
                        Contribution = 0.8
                    },
                    "scenarioConfigurationWithoutIsRelevant.xml")
                .SetName(testNameFormat);
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