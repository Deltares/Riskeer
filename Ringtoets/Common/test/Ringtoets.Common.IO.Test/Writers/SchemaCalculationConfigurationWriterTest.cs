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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Writers
{
    [TestFixture]
    public class SchemaCalculationConfigurationWriterTest
        : CustomSchemaCalculationConfigurationWriterDesignGuidelinesTestFixture<
            SimpleSchemaCalculationConfigurationWriter,
            TestConfigurationItem>
    {
        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void Write_DifferentCalculationAndCalculationGroupConfigurations_ValidFile(IEnumerable<IConfigurationItem> configuration, string expectedFileContentsFileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<ICalculation>), expectedFileContentsFileName));

            try
            {
                // Call
                new SimpleSchemaCalculationConfigurationWriter(filePath).Write(configuration);

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

            var calculationGroup1 = new CalculationConfigurationGroup("group1", Enumerable.Empty<IConfigurationItem>());
            var calculationGroup2 = new CalculationConfigurationGroup("group2", new IConfigurationItem[]
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

        protected override SimpleSchemaCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new SimpleSchemaCalculationConfigurationWriter(filePath);
        }
    }

    public class SimpleSchemaCalculationConfigurationWriter : SchemaCalculationConfigurationWriter<TestConfigurationItem>
    {
        public const string CalculationElementTag = "calculation";

        public SimpleSchemaCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(TestConfigurationItem calculation, XmlWriter writer)
        {
            writer.WriteElementString(CalculationElementTag, calculation.Name);
        }
    }

    public class TestConfigurationItem : IConfigurationItem
    {
        public string Name { get; set; }
    }
}