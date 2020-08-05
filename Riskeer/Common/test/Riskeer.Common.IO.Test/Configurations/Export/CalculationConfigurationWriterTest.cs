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
using System.IO;
using System.Linq;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

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
        public void WriteDistributionWhenAvailable_MeanStandardDeviationStochastConfigurationNull_WriterNotCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                "some name",
                null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteDistributionWhenAvailable_MeanStandardDeviationStochastConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            var configuration = new StochastConfiguration();

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteDistribution(name, configuration));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                name,
                configuration);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

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
            // Setup
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                "some name",
                null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteDistributionWhenAvailable_StochastConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            var configuration = new StochastConfiguration();

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteDistribution(name, configuration));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteDistributionWhenAvailable(
                xmlWriter,
                name,
                configuration);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

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
            // Setup
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                "some name",
                (string) null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteElementWhenContentAvailable_StringSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            const string value = "some value";

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteElementString(name, value));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                name,
                value);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                "some name",
                (double?) null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteElementWhenContentAvailable_DoubleSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            const double value = 3.2;

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteElementString(name, XmlConvert.ToString(value)));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                name,
                value);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                "some name",
                (bool?) null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteElementWhenContentAvailable_BoolSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            const string name = "some name";
            const bool value = true;

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteElementString(name, XmlConvert.ToString(value)));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteElementWhenContentAvailable(
                xmlWriter,
                name,
                value);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteWaveReductionWhenAvailable(
                xmlWriter,
                null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteWaveReductionWhenAvailable_WaveReductionConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            var configuration = new WaveReductionConfiguration();

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteWaveReduction(configuration));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteWaveReductionWhenAvailable(
                xmlWriter,
                configuration);

            // Assert
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteScenarioWhenAvailable(
                xmlWriter,
                null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void WriteScenarioWhenAvailable_ScenarioConfigurationSet_WriterCalledWithExpectedParameters()
        {
            // Setup
            var configuration = new ScenarioConfiguration();

            var mocks = new MockRepository();
            var xmlWriter = mocks.StrictMock<XmlWriter>();
            xmlWriter.Expect(w => w.WriteScenario(configuration));
            mocks.ReplayAll();

            // Call
            ExposedCalculationConfigurationWriter.PublicWriteScenarioWhenAvailable(
                xmlWriter,
                configuration);

            // Assert
            mocks.VerifyAll();
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