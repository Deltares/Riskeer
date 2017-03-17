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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.IO.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
        private int progress;

        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            progress = 0;
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ObservableListNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter(
                null,
                "",
                messageProvider,
                new TestStochasticSoilModelUpdateModelStrategy());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StochasticSoilModelImporter(
                new StochasticSoilModelCollection(),
                "",
                null,
                new TestStochasticSoilModelUpdateModelStrategy());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", paramName);
        }

        [Test]
        public void Constructor_ModelUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter(
                new StochasticSoilModelCollection(),
                "",
                messageProvider,
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("modelUpdateStrategy", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var list = new StochasticSoilModelCollection();

            // Call
            var importer = new StochasticSoilModelImporter(
                list,
                "",
                messageProvider,
                new TestStochasticSoilModelUpdateModelStrategy());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<StochasticSoilModelCollection>>(importer);
        }

        [Test]
        public void Import_FromNonExistingFile_LogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call,
                                         messages =>
                                         {
                                             string[] messageArray = messages.ToArray();
                                             string message = $"{string.Empty} \r\nHet bestand wordt overgeslagen.";
                                             StringAssert.EndsWith(message, messageArray[0]);
                                         });
            Assert.AreEqual(1, progress);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_FromInvalidFilePath_LogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                invalidFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call,
                                         messages =>
                                         {
                                             string[] messageArray = messages.ToArray();
                                             string message = $"{string.Empty} \r\nHet bestand wordt overgeslagen.";
                                             StringAssert.EndsWith(message, messageArray[0]);
                                         });
            Assert.AreEqual(1, progress);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilModelToCollectionAndSourcePathSet()
        {
            // Setup
            const string expectedAddDataText = "Adding Data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataText);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();

            var progressChangeNotifications = new List<ProgressNotification>();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
                                            progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            var expectedProfiles = 26;
            var expectedModels = 3;

            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1)
            };
            for (var i = 1; i <= expectedProfiles; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de ondergrondschematisatie uit de D-Soil Model database.", i, expectedProfiles));
            }
            expectedProgressMessages.Add(new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de stochastische ondergrondmodellen.", i, expectedModels));
            }
            expectedProgressMessages.Add(new ProgressNotification("Controleren van ondergrondschematisaties.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Valideren van ingelezen data.", i, expectedModels));
            }
            expectedProgressMessages.Add(new ProgressNotification(expectedAddDataText, 1, 1));
            Assert.AreEqual(expectedProgressMessages.Count, progressChangeNotifications.Count);
            for (var i = 0; i < expectedProgressMessages.Count; i++)
            {
                ProgressNotification notification = expectedProgressMessages[i];
                ProgressNotification actualNotification = progressChangeNotifications[i];
                Assert.AreEqual(notification.Text, actualNotification.Text);
                Assert.AreEqual(notification.CurrentStep, actualNotification.CurrentStep);
                Assert.AreEqual(notification.TotalSteps, actualNotification.TotalSteps);
            }
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileTwice_ReadAnotherTime()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;
            importer.Import();

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(36 * 2, progress);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            Assert.AreEqual(3, readModels.Length);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingSoilProfiles_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van de D-Soil Model database."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingStochasticSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van de stochastische ondergrondmodellen."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingAndCheckingSoilProfiles_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Controleren van ondergrondschematisaties."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenValidatingStochasticSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Valideren van ingelezen data."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            AssertUnsuccessfulImport(importResult, updateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ImportCompletedSuccessfullyNonetheless()
        {
            // Setup
            const string expectedAddDataProgressText = "Adding data...";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataProgressText);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedAddDataProgressText))
                {
                    importer.Cancel();
                }
            });

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            Assert.AreEqual(3, readModels.Length);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            bool importResult = importer.Import();
            Assert.IsFalse(importResult);

            importer.SetProgressChanged(null);

            // Call
            importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            Assert.AreEqual(3, readModels.Length);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToCorruptFile = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToCorruptFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(pathToCorruptFile)
                .WithSubject("ondergrondschematisatie 'Profile'")
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom \'IntersectionX\'.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"{internalErrorMessage} \r\nDeze ondergrondschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create("Het stochastische ondergrondmodel \'Name\' heeft een ongespecificeerde ondergrondschematisatie. Dit model wordt overgeslagen.", LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 2);
            Assert.AreEqual(8, progress);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToCorruptFile, importResult, updateStrategy);
            CollectionAssert.IsEmpty(readModels);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToCorruptFile = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToCorruptFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            // Call
            bool importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToCorruptFile, importResult, updateStrategy);
            CollectionAssert.IsEmpty(readModels);
        }

        [Test]
        public void Import_IncorrectProfiles_SkipModelAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToCorruptFile = Path.Combine(testDataPath, "invalidStochasticSoilProfiles.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToCorruptFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string expectedLogMessage =
                $"Fout bij het lezen van bestand '{pathToCorruptFile}': de ondergrondschematisatie verwijst naar een ongeldige waarde." +
                " Dit stochastische ondergrondmodel wordt overgeslagen.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToCorruptFile, importResult, updateStrategy);
            CollectionAssert.IsEmpty(readModels);
        }

        [Test]
        public void Import_IncorrectProbability_LogAndImportSoilModelToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToCorruptFile = Path.Combine(testDataPath, "incorrectProbability.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToCorruptFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessage = "De som van de kansen van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToCorruptFile, importResult, updateStrategy);
            Assert.AreEqual(1, readModels.Length);
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile1D_ImportSoilModelsToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile1D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);

            // Call
            bool importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);

            Assert.AreEqual(2, readModels.Length);
            StochasticSoilModel model1 = readModels[0];
            StochasticSoilModel model2 = readModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile1.SoilProfileType,
                            "Expected database to have 1D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile2.SoilProfileType,
                            "Expected database to have 1D profiles.");
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile2D_ImportSoilModelsToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile2D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                pipingFailureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);

            // Call
            bool importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);

            Assert.AreEqual(2, readModels.Length);
            StochasticSoilModel model1 = readModels[0];
            StochasticSoilModel model2 = readModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile1.SoilProfileType,
                            "Expected database to have 2D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile2.SoilProfileType,
                            "Expected database to have 2D profiles.");
        }

        [Test]
        public void Import_ModelWithOneInvalidStochasticSoilProfileDueToMissingProfile_SkipModelAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "EmptySoilModel.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string expectedLogMessage = "Er zijn geen ondergrondschematisaties gevonden in het stochastische " +
                                        "ondergrondmodel 'Model'. Dit model wordt overgeslagen.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            CollectionAssert.IsEmpty(readModels);
        }

        [Test]
        public void Import_ModelWithTwoStochasticSoilProfileForSameProfile_ProbabilitiesAddedAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToFile = Path.Combine(testDataPath, "multipleStochasticSoilProfileForSameProfile.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action importAction = () => importResult = importer.Import();

            // Assert
            string expectedMessage = "Ondergrondschematisatie 'Profile' is meerdere keren gevonden in ondergrondmodel " +
                                     "'StochasticSoilModelName'. Kansen van voorkomen worden opgeteld.";
            TestHelper.AssertLogMessageIsGenerated(importAction, expectedMessage, 1);

            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToFile, importResult, updateStrategy);
            Assert.AreEqual(1, readModels.Length);
            StochasticSoilModel firstModel = readModels.First();
            Assert.AreEqual(1, firstModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(1.0, firstModel.StochasticSoilProfiles[0].Probability);
        }

        [Test]
        public void Import_ModelWithTwoStochasticSoilProfileForProfilesWithSameNameButDifferentTypes_ProbabilitiesNotAdded()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string pathToFile = Path.Combine(testDataPath, "combined1d2d.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                pathToFile,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action importAction = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesCount(importAction, 0);
            StochasticSoilModel[] readModels = AssertSuccessfulImport(pathToFile, importResult, updateStrategy);
            Assert.AreEqual(1, readModels.Length);
            StochasticSoilModel firstModel = readModels.First();
            Assert.AreEqual(2, firstModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(firstModel.StochasticSoilProfiles[0].SoilProfile.Name, firstModel.StochasticSoilProfiles[1].SoilProfile.Name);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithoutLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_noLayerProperties.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            // Call
            bool importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            Assert.AreEqual(1, readModels.Length);

            StochasticSoilModel soilModel = readModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(1.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("AD647M30_Segment_36005_1D1", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            Assert.AreEqual(9, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                4.8899864439741778,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            int expectedNumberOfLayers = expectedLayerTops.Length;
            CollectionAssert.AreEqual(Enumerable.Repeat(false, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(Enumerable.Repeat(double.NaN, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.BelowPhreaticLevelMean));

            Assert.AreEqual(7, progress);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_withLayerProperties.soil");

            var failureMechanism = new PipingFailureMechanism();
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            var importer = new StochasticSoilModelImporter(
                failureMechanism.StochasticSoilModels,
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.SetProgressChanged(IncrementProgress);

            // Call
            bool importResult = importer.Import();

            // Assert
            StochasticSoilModel[] readModels = AssertSuccessfulImport(validFilePath, importResult, updateStrategy);
            Assert.AreEqual(1, readModels.Length);

            StochasticSoilModel soilModel = readModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(1.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("Test 2d profile", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            const int expectedNumberOfLayers = 9;
            Assert.AreEqual(expectedNumberOfLayers, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                5.0571018353300463,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            var expectedIsAquiferValues = new[]
            {
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false
            };
            CollectionAssert.AreEqual(expectedIsAquiferValues,
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            var expectedBelowPhreaticLevelValues = new[]
            {
                27.27,
                28.28,
                29.29,
                30.3,
                33.33,
                35.35,
                37.37,
                39.39,
                40.4
            };
            CollectionAssert.AreEqual(expectedBelowPhreaticLevelValues,
                                      profile.Layers.Select(l => l.BelowPhreaticLevelMean));

            Assert.AreEqual(7, progress);
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            var updateStrategy = new TestStochasticSoilModelUpdateModelStrategy();
            updateStrategy.UpdatedInstances = new[]
            {
                observableA,
                observableB
            };
            var importer = new StochasticSoilModelImporter(
                new StochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                updateStrategy);
            importer.Import();

            // Call
            importer.DoPostImport();

            // Assert
        }

        private static StochasticSoilModel[] AssertSuccessfulImport(
            string expectedPath,
            bool actualImportResult,
            TestStochasticSoilModelUpdateModelStrategy updateStrategy)
        {
            Assert.IsTrue(actualImportResult);
            Assert.IsTrue(updateStrategy.Updated);
            Assert.AreEqual(expectedPath, updateStrategy.FilePath);
            return updateStrategy.ReadModels;
        }

        private static void AssertUnsuccessfulImport(
            bool actualImportResult,
            TestStochasticSoilModelUpdateModelStrategy updateStrategy)
        {
            Assert.IsFalse(updateStrategy.Updated);
            Assert.IsFalse(actualImportResult);
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }

        private class ProgressNotification
        {
            public ProgressNotification(string description, int currentStep, int totalSteps)
            {
                Text = description;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
            }

            public string Text { get; }
            public int CurrentStep { get; }
            public int TotalSteps { get; }
        }
    }
}