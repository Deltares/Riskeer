// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.TestUtil;

namespace Riskeer.Common.IO.Test.Configurations.Export
{
    [TestFixture]
    public class CalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            TestCalculationConfigurationWriter,
            TestConfigurationItem>
    {
        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void Write_DifferentCalculationAndCalculationGroupConfigurations_ValidFile(IEnumerable<IConfigurationItem> configuration, string expectedFileContentsFileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<IConfigurationItem>), expectedFileContentsFileName));

            try
            {
                // Call
                new TestCalculationConfigurationWriter(filePath).Write(configuration);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

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
        public void WriteDistributionWhenAvailable_MeanStandardDeviationStochastConfigurationWriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                null,
                "some name",
                new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteDistributionWhenAvailable_MeanStandardDeviationStochastConfigurationDistributionNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                null,
                new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distributionName", exception.ParamName);
        }

        [Test]
        public void WriteDistributionWhenAvailable_StochastConfigurationWriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                null,
                "some name",
                new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteDistributionWhenAvailable_StochastConfigurationDistributionNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                null,
                new StochastConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distributionName", exception.ParamName);
        }

        [Test]
        public void WriteDistributionWhenAvailable_StochastConfigurationNull_WriterNotCalled()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                    writer,
                    "name",
                    null);
            }

            string xml = sb.ToString();

            // Assert
            Assert.IsEmpty(xml);
        }

        [Test]
        public void WriteDistributionWhenAvailable_StochastConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            var configuration = new StochastConfiguration();

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                    writer,
                    name,
                    configuration);
            }

            string xml = sb.ToString();

            // Assert
            StringAssert.Contains(ConfigurationSchemaIdentifiers.NameAttribute, xml);
            StringAssert.Contains(ConfigurationSchemaIdentifiers.StochastElement, xml);
            StringAssert.Contains(name, xml);
        }

        [Test]
        public void WriteElementWhenContentAvailable_StringWriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                null,
                "some name",
                "some value");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_StringElementNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                null,
                "some value");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("elementName", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_StringNull_WriterNotCalled()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                    writer,
                    "some name",
                    (string) null);
            }

            string xml = sb.ToString();

            // Assert
            Assert.IsEmpty(xml);
        }

        [Test]
        public void WriteElementWhenContentAvailable_StringSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "someName";
            const string value = "some value";
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                    xmlWriter,
                    name,
                    value);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual("<someName>some value</someName>", xml);
        }

        [Test]
        public void WriteElementWhenContentAvailable_DoubleWriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                null,
                "some name",
                0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_DoubleElementNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                null,
                0.2);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("elementName", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_DoubleNull_WriterNotCalled()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                "some name",
                (double?) null);

            // Assert
            Assert.AreEqual(0, xmlWriter.ReceivedCalls().Count());
        }

        [Test]
        public void WriteElementWhenContentAvailable_DoubleSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "someName";
            const double value = 3.2;
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                    xmlWriter,
                    name,
                    value);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual($"<someName>{XmlConvert.ToString(value)}</someName>", xml);
        }

        [Test]
        public void WriteElementWhenContentAvailable_BoolWriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                null,
                "some name",
                false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_BoolElementNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var xmlWriter = Substitute.For<XmlWriter>();
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                null,
                false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("elementName", exception.ParamName);
        }

        [Test]
        public void WriteElementWhenContentAvailable_BoolNull_WriterNotCalled()
        {
            // Setup
            const string name = "someName";
            const bool value = true;
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                    xmlWriter,
                    name,
                    (bool?) null);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual("", xml);
        }

        [Test]
        public void WriteElementWhenContentAvailable_BoolSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "someName";
            const bool value = true;
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                    xmlWriter,
                    name,
                    value);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual($"<someName>{XmlConvert.ToString(value)}</someName>", xml);
        }

        [Test]
        public void WriteWaveReductionWhenAvailable_WriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteWaveReductionWhenAvailable(
                null,
                new WaveReductionConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteWaveReductionWhenAvailable_WaveReductionConfigurationNull_WriterNotCalled()
        {        
            // Setup
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteWaveReductionWhenAvailable(
                    xmlWriter,
                    null);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual(string.Empty, xml);
        }

        [Test]
        public void WriteScenarioWhenAvailable_WriterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ExposedCalculationConfigurationWriter.PublicWriteScenarioWhenAvailable(
                null,
                new ScenarioConfiguration());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("writer", exception.ParamName);
        }

        [Test]
        public void WriteScenarioWhenAvailable_ScenarioConfigurationNull_WriterNotCalled()
        {
            // Setup
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteScenarioWhenAvailable(
                    xmlWriter,
                    null);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreEqual(string.Empty, xml);
        }

        [Test]
        public void WriteScenarioWhenAvailable_ScenarioConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            var configuration = new ScenarioConfiguration();
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(
                       stringBuilder,
                       new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           ConformanceLevel = ConformanceLevel.Fragment
                       }))
            {
                // Call
                ExposedCalculationConfigurationWriter.PublicWriteScenarioWhenAvailable(
                    xmlWriter,
                    configuration);
            }

            string xml = stringBuilder.ToString();

            // Assert
            Assert.AreNotEqual(string.Empty, xml);
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurations()
        {
            var calculation1 = new TestConfigurationItem
            {
                Name = "calculation1"
            };
            var calculation2 = new TestConfigurationItem
            {
                Name = "calculation2"
            };

            var calculationGroup1 = new CalculationGroupConfiguration("group1", Enumerable.Empty<IConfigurationItem>());
            var calculationGroup2 = new CalculationGroupConfiguration("group2", new IConfigurationItem[]
            {
                calculation2,
                calculationGroup1
            });

            yield return new TestCaseData(
                    new[]
                    {
                        calculationGroup1
                    },
                    "singleGroup.xml")
                .SetName("Single group");
            yield return new TestCaseData(
                    new[]
                    {
                        calculation1
                    },
                    "singleCalculation.xml")
                .SetName("Single calculation");
            yield return new TestCaseData(
                    new IConfigurationItem[]
                    {
                        calculationGroup1,
                        calculation1
                    },
                    "calculationGroupAndCalculation.xml")
                .SetName("Calculation group and calculation");
            yield return new TestCaseData(
                    new IConfigurationItem[]
                    {
                        calculation1,
                        calculationGroup2
                    },
                    "calculationAndGroupWithNesting.xml")
                .SetName("Calculation and group with nesting");
        }

        protected override TestCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new TestCalculationConfigurationWriter(filePath);
        }

        private class ExposedCalculationConfigurationWriter : CalculationConfigurationWriter<TestConfigurationItem>
        {
            public ExposedCalculationConfigurationWriter(string filePath) : base(filePath) {}

            public static void PublicWriteDistributionWhenAvailable(XmlWriter writer, string distributionName, StochastConfiguration configuration)
            {
                WriteDistributionWhenAvailable(writer, distributionName, configuration);
            }

            public static void PublicWriteElementWhenContentAvailable(XmlWriter writer, string elementName, string elementContent)
            {
                WriteElementWhenContentAvailable(writer, elementName, elementContent);
            }

            public static void PublicWriteElementWhenContentAvailable(XmlWriter writer, string elementName, double? elementContent)
            {
                WriteElementWhenContentAvailable(writer, elementName, elementContent);
            }

            public static void PublicWriteElementWhenContentAvailable(XmlWriter writer, string elementName, bool? elementContent)
            {
                WriteElementWhenContentAvailable(writer, elementName, elementContent);
            }

            public static void PublicWriteWaveReductionWhenAvailable(XmlWriter writer, WaveReductionConfiguration configuration)
            {
                WriteWaveReductionWhenAvailable(writer, configuration);
            }

            public static void PublicWriteScenarioWhenAvailable(XmlWriter writer, ScenarioConfiguration configuration)
            {
                WriteScenarioWhenAvailable(writer, configuration);
            }

            protected override int GetConfigurationVersion()
            {
                return 1;
            }

            protected override void WriteCalculation(TestConfigurationItem calculation, XmlWriter writer)
            {
                throw new NotImplementedException();
            }
        }
    }
}