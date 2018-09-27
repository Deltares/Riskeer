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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;

namespace Ringtoets.Common.IO.TestUtil
{
    [TestFixture]
    public abstract class CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
        TCalculationConfigurationExporter, TWriter, TCalculation, TConfiguration>
        where TCalculation : class, ICalculation
        where TWriter : CalculationConfigurationWriter<TConfiguration>
        where TConfiguration : class, IConfigurationItem
        where TCalculationConfigurationExporter : CalculationConfigurationExporter<
            TWriter, TCalculation, TConfiguration>
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CallConfigurationFilePathConstructor(null, "test.xml");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            AssertNullCalculations(exception);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => CallConfigurationFilePathConstructor(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            TCalculationConfigurationExporter exporter = CallConfigurationFilePathConstructor(Enumerable.Empty<ICalculationBase>(), "test.xml");

            // Assert
            AssertDefaultConstructedInstance(exporter);
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            const string folderName = nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse);
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");

                TCalculationConfigurationExporter exporter = CallConfigurationFilePathConstructor(new[]
                {
                    CreateCalculation()
                }, filePath);

                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                IEnumerable<Tuple<string, LogLevelConstant>> logMessages = GetExpectedExportFailedLogMessages(filePath);
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, logMessages);
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        public void Export_PathTooLong_LogErrorAndReturnFalse()
        {
            // Setup
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"C:\");
            for (var i = 0; i < 300; i++)
            {
                stringBuilder.Append("A");
            }

            string filePath = Path.Combine(stringBuilder.ToString(), "test.xml");

            TCalculationConfigurationExporter exporter = CallConfigurationFilePathConstructor(new[]
            {
                CreateCalculation()
            }, filePath);

            // Call
            var isExported = true;
            Action call = () => isExported = exporter.Export();

            // Assert
            IEnumerable<Tuple<string, LogLevelConstant>> logMessages = GetExpectedExportFailedLogMessages(filePath);
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, logMessages);
            Assert.IsFalse(isExported);
        }

        protected virtual IEnumerable<Tuple<string, LogLevelConstant>> GetExpectedExportFailedLogMessages(string filePath)
        {
            return new[]
            {
                Tuple.Create(
                    $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. "
                    + "Er is geen configuratie geëxporteerd.", LogLevelConstant.Error)
            };
        }

        protected virtual void AssertNullCalculations(ArgumentNullException exception)
        {
            Assert.IsNotNull(exception);
            Assert.IsInstanceOf<ArgumentNullException>(exception);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        protected virtual void AssertDefaultConstructedInstance(TCalculationConfigurationExporter exporter)
        {
            Assert.IsInstanceOf<CalculationConfigurationExporter<TWriter, TCalculation, TConfiguration>>(exporter);
        }

        protected void WriteAndValidate(IEnumerable<ICalculationBase> configuration, string expectedXmlFilePath)
        {
            string filePath = TestHelper.GetScratchPadPath($"{nameof(TCalculationConfigurationExporter)}.{nameof(WriteAndValidate)}.xml");
            TCalculationConfigurationExporter exporter = CallConfigurationFilePathConstructor(configuration, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
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

        protected abstract TCalculation CreateCalculation();

        protected abstract TCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath);
    }
}