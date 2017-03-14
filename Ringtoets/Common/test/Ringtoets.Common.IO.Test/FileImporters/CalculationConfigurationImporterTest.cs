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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class CalculationConfigurationImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new TestCalculationConfigurationImporter("",
                                                                    new CalculationGroup());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<CalculationGroup>>(importer);
        }

        [Test]
        public void Import_CancelingImport_CancelImportAndLog()
        {
            // Setup
            var importer = new TestCalculationConfigurationImporter("",
                                                                    new CalculationGroup());

            importer.SetProgressChanged((description, step, steps) => { importer.Cancel(); });

            // Call
            Action call = () => importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Berekeningenconfiguratie importeren afgebroken. Geen data ingelezen.", 1);
        }

        private class TestCalculationConfigurationImporter : CalculationConfigurationImporter<TestConfigurationReader, TestReadConfigurationItem>
        {
            public TestCalculationConfigurationImporter(string filePath, CalculationGroup importTarget)
                : base(filePath, importTarget) {}

            protected override bool OnImport()
            {
                NotifyProgress("Progress", 1, 1);

                if (Canceled)
                {
                    return false;
                }

                return true;
            }

            protected override TestConfigurationReader CreateConfigurationReader(string filePath)
            {
                return new TestConfigurationReader(filePath);
            }

            protected override ICalculationBase ProcessCalculation(TestReadConfigurationItem readCalculation)
            {
                throw new NotImplementedException();
            }
        }

        private class TestConfigurationReader : ConfigurationReader<TestReadConfigurationItem>
        {
            private static readonly string mainSchemaDefinition =
                File.ReadAllText(Path.Combine(TestHelper.GetTestDataPath(
                                                  TestDataPath.Ringtoets.Common.IO,
                                                  "ConfigurationReader"),
                                              "validConfigurationSchema.xsd"));

            public TestConfigurationReader(string xmlFilePath)
                : base(xmlFilePath, mainSchemaDefinition, new Dictionary<string, string>()) {}

            protected override TestReadConfigurationItem ParseCalculationElement(XElement calculationElement)
            {
                throw new NotImplementedException();
            }
        }

        private class TestReadConfigurationItem : IReadConfigurationItem
        {
            public string Name { get; }
        }
    }
}