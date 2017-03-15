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
using System.Linq;
using System.Security.AccessControl;
using System.Xml;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Exporters
{
    [TestFixture]
    public class CalculationConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new SimpleCalculationConfigurationExporter(Enumerable.Empty<ICalculationBase>(), "test.xml");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_ConfigurationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SimpleCalculationConfigurationExporter(null, "test.xml");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new SimpleCalculationConfigurationExporter(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            string targetFilePath = TestHelper.GetScratchPadPath($"{nameof(Export_ValidData_ReturnTrueAndWritesFile)}.xml");

            string testFileSubPath = Path.Combine(
                nameof(CalculationConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>),
                "folderWithSubfolderAndCalculation.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                testFileSubPath);

            var calculation = new TestCalculation("Calculation A");
            var calculation2 = new TestCalculation("Calculation B");

            var calculationGroup2 = new CalculationGroup("Group B", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("Group A", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            var exporter = new SimpleCalculationConfigurationExporter(new[]
            {
                calculationGroup
            }, targetFilePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(targetFilePath));

                string actualXml = File.ReadAllText(targetFilePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(targetFilePath);
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

                var exporter = new SimpleCalculationConfigurationExporter(new[]
                {
                    new TestCalculation("Calculation A")
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

    public class SimpleCalculationConfigurationExporter : CalculationConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>
    {
        public SimpleCalculationConfigurationExporter(IEnumerable<ICalculationBase> configuration, string targetFilePath) : base(configuration, targetFilePath) {}
    }

    public class SimpleCalculationConfigurationWriter : CalculationConfigurationWriter<TestCalculation>
    {
        protected override void WriteCalculation(TestCalculation calculation, XmlWriter writer)
        {
            writer.WriteElementString("calculation", calculation.Name);
        }
    }
}