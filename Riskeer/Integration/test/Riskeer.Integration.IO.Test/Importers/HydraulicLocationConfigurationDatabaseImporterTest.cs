// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImporterTest
    {
        private const int totalNumberOfSteps = 2;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO,
                                                                                 nameof(HydraulicLocationConfigurationDatabaseImporter));

        private readonly string validHrdFilePath = Path.Combine(testDataPath, "completeHrd.sqlite");
        private readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(),
                                                                              null, new HydraulicBoundaryData(),
                                                                              validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(),
                                                                              handler, null, validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              new HydraulicBoundaryData(), validHlcdFilePath);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicLocationConfigurationDatabase>>(importer);
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

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            var importResult = true;

            // Call
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_HlcdInDifferentDirectoryThanHydraulicBoundaryDatabases_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            string hlcdFilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO,
                                                                          nameof(HydraulicBoundaryDatabaseImporter)), "HLCD.sqlite");

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het HLCD bestand moet zich in dezelfde map bevinden als de toegevoegde HRD bestanden.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string hlcdFilePath = Path.Combine(testDataPath, "empty.sqlite");

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bevragen van de database is mislukt.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("scenarioInformationNoEntries")]
        [TestCase("scenarioInformationMultipleEntries")]
        public void Import_InvalidNumberOfScenarioInformationEntries_CancelImportWithErrorMessage(string hlcdFileName)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string hlcdFilePath = Path.Combine(testDataPath, $"{hlcdFileName}.sqlite");

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': de tabel 'ScenarioInformation' moet exact 1 rij bevatten.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ReadHlcdDoesNotContainCurrentLocationId_CancelImportWithErrorMessage()
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            hydraulicBoundaryData.Locations.Add(new TestHydraulicBoundaryLocation());

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{validHlcdFilePath}': 1 of meerdere locaties komen niet voor in de HLCD.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Import_ValidHlcdFile_ExpectedProgressNotifications(bool hydraulicBoundaryDataLinked)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            handler.Stub(h => h.Update(null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();

            HydraulicBoundaryData hydraulicBoundaryData = hydraulicBoundaryDataLinked
                                                              ? CreateLinkedHydraulicBoundaryData()
                                                              : new HydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressNotifications = new[]
            {
                new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 1, totalNumberOfSteps),
                new ProgressNotification("Geïmporteerde data toevoegen aan het traject.", 2, totalNumberOfSteps)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithoutScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryData>.Is.Same(hydraulicBoundaryData),
                                         Arg<ReadHydraulicLocationConfigurationSettings>.Is.Null,
                                         Arg<string>.Is.Equal(hlcdFilePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importResult = false;
            void Call() => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{hlcdFilePath}'.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            string filePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryData>.Is.Same(hydraulicBoundaryData),
                                         Arg<ReadHydraulicLocationConfigurationSettings>.Is.NotNull,
                                         Arg<string>.Is.Equal(filePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, filePath);

            // Call
            var importResult = false;
            void Call() => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImportUpdates_WhenImportSuccessful_NotifyObserversOfReturnedObjects()
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryData>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationSettings>.Is.Null,
                                         Arg<string>.Is.NotNull))
                   .Return(new[]
                   {
                       observable1,
                       observable2
                   });
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on updated observables
        }

        [Test]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                importer.Cancel();
            });

            // Call
            var importResult = true;
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringAddReadDataToDataModel_ContinuesImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            handler.Stub(h => h.Update(null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
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
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 2);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        private HydraulicBoundaryData CreateLinkedHydraulicBoundaryData()
        {
            return new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath,
                    ScenarioName = "ScenarioName",
                    Year = 2022,
                    Scope = "Scope",
                    SeaLevel = "SeaLevel",
                    RiverDischarge = "RiverDischarge",
                    LakeLevel = "LakeLevel",
                    WindDirection = "WindDirection",
                    WindSpeed = "WindSpeed",
                    Comment = "Comment"
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        UsePreprocessorClosure = false
                    }
                }
            };
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