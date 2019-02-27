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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.IO.TestUtil;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Importers;

namespace Riskeer.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterTest
    {
        private const int totalNumberOfSteps = 4;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO, nameof(HydraulicBoundaryDatabaseImporter));
        private static readonly string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

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

            string path = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO, "I_dont_exist");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand bestaat niet.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "corruptschema.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "empty.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kon geen locaties verkrijgen van de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutHlcd_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutHLCD", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithEmptyHlcdSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "EmptyHLCDSchema", "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bevragen van de database is mislukt.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithInvalidHlcdSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "InvalidHLCDSchema", "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("hlcdWithScenarioInformationNoEntries")]
        [TestCase("hlcdWithScenarioInformationMultipleEntries")]
        public void Import_ExistingFileAndHlcdWithInvalidNumberOfScenarioInformationEntries_CancelImportWithErrorMessage(string testFolder)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, testFolder, "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': de tabel 'ScenarioInformation' in het HLCD bestand moet exact 1 rij bevatten.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutSettings_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutSettings", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kon het rekeninstellingen bestand niet openen. " +
                                     $"Fout bij het lezen van bestand '{HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(path)}': het bestand bestaat niet.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithInvalidSettings_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "invalidSettings", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': de rekeninstellingen database heeft niet het juiste schema.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileAndHlcdWithUsePreprocessorClosureTrueAndWithoutPreprocessorClosure_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "missingPreprocessorClosure", "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bijbehorende preprocessor closure bestand is niet gevonden in dezelfde map als het HLCD bestand.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Import_WithValidFileAndConfirmationRequired_InquiresAndUpdatesHydraulicBoundaryDatabase(bool confirmationRequired)
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.IsConfirmationRequired(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       AssertReadHydraulicBoundaryDatabase((ReadHydraulicBoundaryDatabase) invocation.Arguments[1]);
                   })
                   .Return(confirmationRequired);

            if (confirmationRequired)
            {
                handler.Expect(h => h.InquireConfirmation()).Return(true);
            }

            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabase>.Is.NotNull,
                                         Arg<IEnumerable<long>>.Is.NotNull,
                                         Arg<string>.Is.NotNull,
                                         Arg<string>.Is.NotNull))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, validFilePath);

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{validFilePath}'.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_WithValidFileAndHlcdWithoutScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.IsConfirmationRequired(null, null))
                   .IgnoreArguments()
                   .Return(false);

            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabase>.Is.NotNull,
                                         Arg<IEnumerable<long>>.Is.NotNull,
                                         Arg<string>.Is.Equal(validFilePath),
                                         Arg<string>.Is.Equal(hlcdFilePath)))
                   .WhenCalled(invocation =>
                   {
                       AssertReadHydraulicBoundaryDatabase((ReadHydraulicBoundaryDatabase) invocation.Arguments[1]);

                       var readHydraulicLocationConfigurationDatabase = (ReadHydraulicLocationConfigurationDatabase) invocation.Arguments[2];
                       Assert.AreEqual(18, readHydraulicLocationConfigurationDatabase.LocationIdMappings.Count());
                       Assert.IsNull(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings);

                       var excludedLocationIds = (IEnumerable<long>) invocation.Arguments[3];
                       Assert.AreEqual(1, excludedLocationIds.Count());
                   })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, validFilePath);

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{validFilePath}'.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_WithValidFileAndHlcdWithValidScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            string hydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation", "complete.sqlite");
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation", "hlcd.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.IsConfirmationRequired(null, null))
                   .IgnoreArguments()
                   .Return(false);

            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabase>.Is.NotNull,
                                         Arg<IEnumerable<long>>.Is.NotNull,
                                         Arg<string>.Is.Equal(hydraulicBoundaryDatabaseFilePath),
                                         Arg<string>.Is.Equal(hlcdFilePath)))
                   .WhenCalled(invocation =>
                   {
                       AssertReadHydraulicBoundaryDatabase((ReadHydraulicBoundaryDatabase) invocation.Arguments[1]);

                       var readHydraulicLocationConfigurationDatabase = (ReadHydraulicLocationConfigurationDatabase) invocation.Arguments[2];
                       Assert.AreEqual(18, readHydraulicLocationConfigurationDatabase.LocationIdMappings.Count());
                       Assert.AreEqual(1, readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings.Count());

                       var excludedLocationIds = (IEnumerable<long>) invocation.Arguments[3];
                       Assert.AreEqual(0, excludedLocationIds.Count());
                   })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, hydraulicBoundaryDatabaseFilePath);

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{hydraulicBoundaryDatabaseFilePath}'.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetValidFiles))]
        public void Import_ValidFiles_ExpectedProgressNotifications(string filePath)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            handler.Stub(h => h.Update(null, null, null, null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, filePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressNotifications = new[]
            {
                new ProgressNotification("Inlezen van het hydraulische belastingen bestand.", 1, totalNumberOfSteps),
                new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 2, totalNumberOfSteps),
                new ProgressNotification("Inlezen van het rekeninstellingen bestand.", 3, totalNumberOfSteps),
                new ProgressNotification("Geïmporteerde data toevoegen aan het traject.", 4, totalNumberOfSteps)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringDialogInteraction_GenerateCanceledLogMessageAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.IsConfirmationRequired(Arg<HydraulicBoundaryDatabase>.Is.NotNull,
                                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       AssertReadHydraulicBoundaryDatabase((ReadHydraulicBoundaryDatabase) invocation.Arguments[1]);
                   })
                   .Return(true);
            handler.Expect(h => h.InquireConfirmation()).Return(false);
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, validFilePath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs(int stepNumber)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, validFilePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                if (currentStep == stepNumber)
                {
                    importer.Cancel();
                }
            });

            // Precondition
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = true;
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringAddReadDataToDataModel_ContinuesImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Stub(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            handler.Stub(h => h.Update(null, null, null, null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, validFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (step == totalNumberOfSteps)
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            importer.Import();
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 2);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImportUpdates_HydraulicBoundaryDatabaseIsSetAndAnswerDialogToContinue_NotifyObserversOfTargetAndClearedObjects()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var hydraulicBoundaryDatabaseObserver = mocks.Stub<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(o => o.UpdateObserver());

            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.IsConfirmationRequired(null, null)).IgnoreArguments().Return(false);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabase>.Is.NotNull,
                                         Arg<IEnumerable<long>>.Is.NotNull,
                                         Arg<string>.Is.NotNull,
                                         Arg<string>.Is.NotNull))
                   .Return(new[]
                   {
                       observable1,
                       observable2
                   });
            handler.Expect(h => h.DoPostUpdateActions());
            mocks.ReplayAll();

            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, validFilePath);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on updated observables
        }

        [Test]
        public void DoPostImportUpdates_CancelingImport_DoNotNotifyObserversAndNotDoPostReplacementUpdates()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, validFilePath);
            handler.Expect(h => h.IsConfirmationRequired(null, null)).IgnoreArguments()
                   .WhenCalled(invocation => importer.Cancel())
                   .Return(false);

            mocks.ReplayAll();

            hydraulicBoundaryDatabase.Attach(observer);

            // Precondition
            Assert.IsFalse(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect no NotifyObserver calls
        }

        private static IEnumerable<TestCaseData> GetValidFiles()
        {
            return new[]
            {
                new TestCaseData(validFilePath)
                    .SetName("validFilePath"),
                new TestCaseData(Path.Combine(testDataPath, "withoutPreprocessorClosure", "complete.sqlite"))
                    .SetName("withoutPreprocessorClosure")
            };
        }

        private static void AssertImportFailed(Action call, string errorMessage, ref bool importSuccessful)
        {
            string expectedMessage = $"{errorMessage}" +
                                     $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importSuccessful);
        }

        private static void AssertReadHydraulicBoundaryDatabase(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            Assert.AreEqual("Dutch coast South19-11-2015 12:0013", readHydraulicBoundaryDatabase.Version);
            Assert.AreEqual((long) 13, readHydraulicBoundaryDatabase.TrackId);
            Assert.AreEqual(18, readHydraulicBoundaryDatabase.Locations.Count());

            ReadHydraulicBoundaryLocation location = readHydraulicBoundaryDatabase.Locations.First();

            Assert.AreEqual(1, location.Id);
            Assert.AreEqual("punt_flw_ 1", location.Name);
            Assert.AreEqual(52697.5, location.CoordinateX);
            Assert.AreEqual(427567.0, location.CoordinateY);
        }
    }
}