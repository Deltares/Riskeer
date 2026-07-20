// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using log4net.Core;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;
using Riskeer.Common.IO.TestUtil;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "StochasticSoilModelImporter");

        private IStochasticSoilModelTransformer<IMechanismStochasticSoilModel> transformer;

        [SetUp]
        public void Setup()
        {
            transformer = Substitute.For<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
        }

        [TearDown]
        public void TearDown() {}

        [Test]
        public void Constructor_ImportTargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                null,
                filePath,
                messageProvider,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", parameter);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                null,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", parameter);
        }

        [Test]
        public void Constructor_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                messageProvider,
                null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("configuration", parameter);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var collection = new TestStochasticSoilModelCollection();
            string filePath = string.Empty;
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                filePath,
                messageProvider,
                configuration);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>>>(importer);
        }

        [Test]
        public void Import_NonExistingFile_LogErrorReturnFalse()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            const string file = "nonexisting.soil";
            var collection = new TestStochasticSoilModelCollection();
            string validFilePath = Path.Combine(testDataPath, file);
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                validFilePath,
                messageProvider,
                configuration);

            var progress = 0;
            importer.SetProgressChanged((description, step, steps) =>
            {
                progress++;
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Assert.AreEqual(1, messages.Count());
                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(0);

                Assert.AreEqual(Level.Error, expectedLog.Item2);

                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                Assert.AreEqual(loggedException.Message, expectedLog.Item1);
            });

            Assert.AreEqual(1, progress);
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Import_InvalidPath_LogErrorReturnFalse(string fileName)
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var collection = new TestStochasticSoilModelCollection();
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                fileName,
                messageProvider,
                configuration);

            var progress = 0;
            importer.SetProgressChanged((description, step, steps) =>
            {
                progress++;
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Assert.AreEqual(1, messages.Count());
                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(0);

                Assert.AreEqual(Level.Error, expectedLog.Item2);

                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                Assert.AreEqual(loggedException.Message, expectedLog.Item1);
            });

            Assert.AreEqual(1, progress);
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase(FailureMechanismType.Piping, 3)]
        [TestCase(FailureMechanismType.Stability, 3)]
        public void Import_VariousFailureMechanismTypes_ShowProgressAndUpdatesCollection(FailureMechanismType failureMechanismType,
                                                                                         int nrOfFailureMechanismSpecificModelsInDatabase)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            const int totalNrOfStochasticSoilModelInDatabase = 6;

            const string expectedAddDataText = "Adding Data";

            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(callInfo =>
                               callInfo.Arg<StochasticSoilModel>().FailureMechanismType == failureMechanismType
                  );

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddDataText);

            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.When(updateStratcall => updateStratcall.UpdateModelWithImportedData(Arg.Any<IEnumerable<IMechanismStochasticSoilModel>>(), Arg.Any<string>()))
                          .Do(callInfo =>
                          {
                              var soilModels = callInfo.Arg<IEnumerable<IMechanismStochasticSoilModel>>();
                              Assert.AreEqual(nrOfFailureMechanismSpecificModelsInDatabase, soilModels.Count());
                              Assert.AreEqual(validFilePath, callInfo.Arg<string>());
                          });

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var progressChangeNotifications = new List<ProgressNotification>();
            importer.SetProgressChanged((description, step, steps) =>
                                            progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{validFilePath}'.", 1);
            Assert.IsTrue(importResult);

            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1)
            };
            for (var i = 1; i <= totalNrOfStochasticSoilModelInDatabase; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification(
                                                 "Inlezen van de stochastische ondergrondmodellen.", i, totalNrOfStochasticSoilModelInDatabase));
            }

            for (var i = 1; i <= nrOfFailureMechanismSpecificModelsInDatabase; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification(
                                                 "Valideren van ingelezen data.", i, nrOfFailureMechanismSpecificModelsInDatabase));
            }

            expectedProgressMessages.Add(new ProgressNotification(expectedAddDataText, 1, 1));
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
            filter.Received(totalNrOfStochasticSoilModelInDatabase).IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void Import_ImportingFailureMechanismTypeNotInDatabase_ShowProgressAndDoesNotUpdateCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            const int totalNrOfStochasticSoilModelInDatabase = 6;

            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(callInfo =>
                               callInfo.Arg<StochasticSoilModel>().FailureMechanismType == FailureMechanismType.None
                  );

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var progressChangeNotifications = new List<ProgressNotification>();
            importer.SetProgressChanged((description, step, steps) =>
                                            progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsFalse(importResult);

            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1)
            };
            for (var i = 1; i <= totalNrOfStochasticSoilModelInDatabase; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification(
                                                 "Inlezen van de stochastische ondergrondmodellen.", i, totalNrOfStochasticSoilModelInDatabase));
            }

            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);

            filter.Received(totalNrOfStochasticSoilModelInDatabase).IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void Import_CancelWhileReadingSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText("Stochastische ondergrondmodellen")
                           .Returns(cancelledLogMessage);
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

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
            Assert.IsFalse(importResult);
            messageProvider.Received().GetCancelledLogMessageText("Stochastische ondergrondmodellen");
        }

        [Test]
        public void Import_CancelWhenTransformingSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText("Stochastische ondergrondmodellen")
                           .Returns(cancelledLogMessage);
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(true);
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

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
            Assert.IsFalse(importResult);
            filter.Received().IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void Import_WithoutTransformableSoilModels_StopsImportAndLogs()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(false);
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string logMessage = "Er zijn geen stochastische ondergrondmodellen gevonden die horen bij het faalmechanisme.";
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(logMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelWhileAddingDataToModel_ContinuesImportAndLogs()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            const int expectedNrOfStochasticSoilModels = 6;

            var stochasticSoilModelCollection = new TestStochasticSoilModelCollection();

            const string expectedAddDataProgressText = "Adding data...";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddDataProgressText);
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.UpdateModelWithImportedData(
                Arg.Is<IMechanismStochasticSoilModel[]>(arr =>
                                                            stochasticSoilModelCollection.All(soilModel => arr.Contains(soilModel))),
                validFilePath);
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(true);

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                stochasticSoilModelCollection,
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

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
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 2);
            Assert.IsTrue(importResult);
            filter.Received(expectedNrOfStochasticSoilModels).IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void Import_ReadingStochaticSoilModelThrowsException_StopsImportAndLogs()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            string validFilePath = Path.Combine(testDataPath, "invalidSegmentPoint.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, tuples =>
            {
                Tuple<string, Level, Exception>[] tupleArray = tuples.ToArray();
                Assert.AreEqual(1, tupleArray.Length);

                Tuple<string, Level, Exception> actualLog = tupleArray[0];

                const string expectedMessage = "Het stochastische ondergrondmodel 'StochasticSoilModelName' moet een geometrie bevatten.";

                Assert.AreEqual(expectedMessage, actualLog.Item1);
                Assert.AreEqual(Level.Error, actualLog.Item2);
                Assert.IsInstanceOf<StochasticSoilModelException>(actualLog.Item3);
            });

            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_IncorrectProbability_LogAndImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "incorrectProbability.soil");
            const string expectedAddDataProgressText = "Adding data...";

            var stochasticSoilModelCollection = new TestStochasticSoilModelCollection();

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText()
                           .Returns(expectedAddDataProgressText);

            var transformedModel = Substitute.For<IMechanismStochasticSoilModel>();
            transformer.Transform(Arg.Is<StochasticSoilModel>(s => s != null))
                       .Returns(transformedModel);

            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();

            updateStrategy.UpdateModelWithImportedData(
                Arg.Is<IEnumerable<IMechanismStochasticSoilModel>>(arr =>
                                                                       arr.Contains(transformedModel)),
                validFilePath);

            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>())
                  .Returns(true);
            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                stochasticSoilModelCollection,
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedLogMessage = "De som van de kansen van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 2);
            Assert.IsTrue(importResult);
            filter.Received().IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void Import_TransformationThrowsException_StopsImportAndLogs()
        {
            // Setup
            const string exceptionMessage = "Some exception message.";

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>()).Returns(true);
            transformer.Transform(Arg.Any<StochasticSoilModel>()).Returns(_ => throw new ImportedDataTransformException(exceptionMessage));
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Assert.AreEqual(1, messages.Count());
                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(0);

                Assert.AreEqual(Level.Error, expectedLog.Item2);

                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<ImportedDataTransformException>(loggedException);
                Assert.AreEqual(loggedException.Message, expectedLog.Item1);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_UpdateDataExceptionThrownWhenAddingData_StopsImportAndLogs()
        {
            // Setup
            const string exceptionMessage = "Some exception message.";
            const string expectedAddDataProgressText = "Adding data...";
            var updateDataException = new UpdateDataException(exceptionMessage);

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddDataProgressText);
            messageProvider.GetUpdateDataFailedLogMessageText("Stochastische ondergrondmodellen").Returns(exceptionMessage);
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.UpdateModelWithImportedData(Arg.Any<IEnumerable<IMechanismStochasticSoilModel>>(), Arg.Any<string>()).Returns(_ => throw updateDataException);
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>()).Returns(true);
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, tuples =>
            {
                Tuple<string, Level, Exception>[] tupleArray = tuples.ToArray();
                Assert.AreEqual(1, tupleArray.Length);

                Tuple<string, Level, Exception> actualLog = tupleArray[0];

                Assert.AreEqual(updateDataException.Message, actualLog.Item1);
                Assert.AreEqual(Level.Error, actualLog.Item2);
                Assert.AreSame(updateDataException, actualLog.Item3);
            });

            Assert.IsFalse(importResult);
            filter.Received().IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var observableA = Substitute.For<IObservable>();
            var observableB = Substitute.For<IObservable>();

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns("");
            var updateStrategy = Substitute.For<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.UpdateModelWithImportedData(Arg.Any<IEnumerable<IMechanismStochasticSoilModel>>(), Arg.Any<string>())
                          .Returns(new[]
                          {
                              observableA,
                              observableB
                          });
            var filter = Substitute.For<IStochasticSoilModelMechanismFilter>();
            filter.IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>()).Returns(true);
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

            importer.Import();

            // Call
            importer.DoPostImport();

            filter.Received().IsValidForFailureMechanism(Arg.Any<StochasticSoilModel>());
            observableA.Received().NotifyObservers();
            observableB.Received().NotifyObservers();
        }

        private class TestStochasticSoilModelCollection : ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>
        {
            public TestStochasticSoilModelCollection()
                : base(s => s.ToString(), "something", "something else") {}
        }
    }
}