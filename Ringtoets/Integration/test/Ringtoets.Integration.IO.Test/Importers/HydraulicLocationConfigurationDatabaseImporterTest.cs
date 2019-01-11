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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Importers;

namespace Ringtoets.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImporterTest
    {
        private const int totalNumberOfSteps = 2;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                 nameof(HydraulicLocationConfigurationDatabaseImporter));

        private readonly string validHrdFilePath = Path.Combine(testDataPath, "completeHrd.sqlite");
        private readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), null,
                                                                                         new HydraulicBoundaryDatabase(), validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                                         null, validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              new HydraulicBoundaryDatabase(), validHlcdFilePath);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicLocationConfigurationSettings>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDifferentFromHydraulicBoundaryDatabasesFilePath_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string hydraulicBoundaryDatabasePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                           nameof(HydraulicBoundaryDatabaseImporter)),"complete.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabasePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{validHlcdFilePath}': het HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }
        
        [Test]
        public void Import_HrdInvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "CorruptHrd");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(path, "corruptschema.sqlite")
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, Path.Combine(path, "HLCD.sqlite"));

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabase.FilePath}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_HrdEmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "EmptyHrd");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(path, "empty.sqlite")
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, Path.Combine(path, "HLCD.sqlite"));

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabase.FilePath}': kon geen locaties verkrijgen van de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "empty.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bevragen van de database is mislukt.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "invalid.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("scenarioInformationNoEntries")]
        [TestCase("scenarioInformationMultipleEntries")]
        public void Import_InvalidNumberOfScenarioInformationEntries_CancelImportWithErrorMessage(string fileName)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, $"{fileName}.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);
            
            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': de tabel 'ScenarioInformation' moet exact 1 rij bevatten.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFiles_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            handler.Stub(h => h.Update(null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressNotifications = new[]
            {
                new ProgressNotification("Inlezen van het hydraulische belastingen bestand.", 1, totalNumberOfSteps),
                new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 2, totalNumberOfSteps),
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs(int stepNumber)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                if (currentStep == stepNumber)
                {
                    importer.Cancel();
                }
            });

            // Call
            var importResult = true;
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringDialogInteraction_GenerateCanceledLogMessageAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(false);
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithoutScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            string filePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicLocationConfigurationSettings>.Is.Same(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings),
                                         Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.Null,
                                         Arg<string>.Is.Equal(filePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, filePath);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath
            };

            string filePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicLocationConfigurationSettings>.Is.Same(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings),
                                         Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.NotNull,
                                         Arg<string>.Is.Equal(filePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, filePath);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        private static void AssertImportFailed(Action call, string errorMessage, ref bool importSuccessful)
        {
            string expectedMessage = $"{errorMessage}" +
                                     $"{Environment.NewLine}Er is geen HLCD geïmporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importSuccessful);
        }
    }
}