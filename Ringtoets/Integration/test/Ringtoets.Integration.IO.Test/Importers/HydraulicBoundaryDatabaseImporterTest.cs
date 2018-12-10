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
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Importers;

namespace Ringtoets.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, nameof(HydraulicBoundaryDatabaseImporter));

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var updateHandler = mocks.Stub<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), updateHandler, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicBoundaryDatabase>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, "I_dont_exist");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': het bestand bestaat niet."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutHlcd_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutSettings_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutSettings", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kon het rekeninstellingen bestand niet openen. " +
                                     $"Fout bij het lezen van bestand '{HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(path)}': het bestand bestaat niet."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFiles_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var totalSteps = 0;

            string filePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, filePath);
            importer.SetProgressChanged((currentStepName, currentStep, totalNumberOfSteps) =>
            {
                totalSteps = totalNumberOfSteps;
                if (currentStep == 1)
                {
                    Assert.AreEqual("Inlezen van het hydraulische belastingen bestand.", currentStepName);
                }

                if (currentStep == 2)
                {
                    Assert.AreEqual("Inlezen van het hydraulische locatie configuratie bestand.", currentStepName);
                }

                if (currentStep == 3)
                {
                    Assert.AreEqual("Controleren van het rekeninstellingen bestand.", currentStepName);
                }
            });

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(3, totalSteps);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs(int stepNumber)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, filePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                if (currentStep == stepNumber)
                {
                    importer.Cancel();
                }
            });

            // Precondition
            Assert.IsTrue(File.Exists(filePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create("Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd.", LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
        }
    }
}