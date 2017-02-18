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
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.IO.Importers;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingConfigurationImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new PipingConfigurationImporter("", new CalculationGroup());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<CalculationGroup>>(importer);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new PipingConfigurationImporter(path, new CalculationGroup());

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam. " + Environment.NewLine +
                                  "Er is geen berekening configuratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "I_dont_exist");
            var importer = new PipingConfigurationImporter(path, new CalculationGroup());

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand bestaat niet. " + Environment.NewLine +
                                  "Er is geen berekening configuratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        public void Import_CancelingImport_CancelImportAndLog()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.Combine("PipingConfigurationReader", "validEmptyRoot.xml"));
            var importer = new PipingConfigurationImporter(path, calculationGroup);
            importer.SetProgressChanged((description, step, steps) =>
            {
                importer.Cancel();
            });

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Berekening configuratie importeren afgebroken. Geen data ingelezen.", 1);
            CollectionAssert.IsEmpty(calculationGroup.Children);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        public void GivenImport_WhenImporting_ThenExpectedProgressMessagesGenerated()
        {
            // Given
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.Combine("PipingConfigurationReader", "validConfiguration.xml"));
            var importer = new PipingConfigurationImporter(path, new CalculationGroup());

            var expectedProgressMessages = new[]
           {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen berekening configuratie.", CurrentStep = 1, MaxNrOfSteps = 3
                }
            };

            var progressChangedCallCount = 0;
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                progressChangedCallCount++;
            });

            // When
            importer.Import();

            // Then
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int MaxNrOfSteps { get; set; }
        }
    }
}