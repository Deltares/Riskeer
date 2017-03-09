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

using System;
using System.IO;
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
    public class ConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new SimpleConfigurationExporter(new CalculationGroup(), "test.xml");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SimpleConfigurationExporter(null, "test.xml");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new SimpleConfigurationExporter(new CalculationGroup(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            string targetFilePath = TestHelper.GetScratchPadPath($"{nameof(Export_ValidData_ReturnTrueAndWritesFile)}.xml");

            string testFileSubPath = Path.Combine(
                nameof(ConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>),
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

            var rootGroup = new CalculationGroup("root", false)
            {
                Children =
                {
                    calculationGroup
                }
            };

            var exporter = new SimpleConfigurationExporter(rootGroup, targetFilePath);

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
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    new TestCalculation("Calculation A")
                }
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");

                var exporter = new SimpleConfigurationExporter(calculationGroup, filePath);

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

    public class SimpleConfigurationExporter : ConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>
    {
        public SimpleConfigurationExporter(CalculationGroup calculationGroup, string targetFilePath) : base(calculationGroup, targetFilePath) {}
    }

    public class SimpleCalculationConfigurationWriter : CalculationConfigurationWriter<TestCalculation>
    {
        protected override void WriteCalculation(TestCalculation calculation, XmlWriter writer)
        {
            writer.WriteElementString("calculation", calculation.Name);
        }
    }
}