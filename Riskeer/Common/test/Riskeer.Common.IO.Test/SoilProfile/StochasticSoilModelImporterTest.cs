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
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "StochasticSoilModelImporter");
        private MockRepository mocks;
        private IStochasticSoilModelTransformer<IMechanismStochasticSoilModel> transformer;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            transformer = mocks.Stub<IStochasticSoilModelTransformer<IMechanismStochasticSoilModel>>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

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
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

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
            importer.SetProgressChanged((description, step, steps) => { progress++; });

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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                fileName,
                messageProvider,
                configuration);

            var progress = 0;
            importer.SetProgressChanged((description, step, steps) => { progress++; });

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

            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(false)
                  .WhenCalled(invocation => { FilterFailureMechanismSpecificModel(invocation, failureMechanismType); })
                  .Repeat
                  .Times(totalNrOfStochasticSoilModelInDatabase);

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataText);

            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.Expect(u => u.UpdateModelWithImportedData(null, null))
                          .IgnoreArguments()
                          .WhenCalled(invocation =>
                          {
                              var soilModels = (IEnumerable<IMechanismStochasticSoilModel>) invocation.Arguments[0];
                              var filePath = (string) invocation.Arguments[1];

                              Assert.AreEqual(nrOfFailureMechanismSpecificModelsInDatabase, soilModels.Count());
                              Assert.AreEqual(validFilePath, filePath);
                          });

            mocks.ReplayAll();

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
        }

        [Test]
        public void Import_ImportingFailureMechanismTypeNotInDatabase_ShowProgressAndDoesNotUpdateCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            const int totalNrOfStochasticSoilModelInDatabase = 6;

            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(false)
                  .WhenCalled(invocation => { FilterFailureMechanismSpecificModel(invocation, FailureMechanismType.None); })
                  .Repeat
                  .Times(totalNrOfStochasticSoilModelInDatabase);
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

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
        }

        [Test]
        public void Import_CancelWhileReadingSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen"))
                           .Return(cancelledLogMessage);
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            mocks.ReplayAll();

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
        }

        [Test]
        public void Import_CancelWhenTransformingSoilModels_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Stochastische ondergrondmodellen"))
                           .Return(cancelledLogMessage);
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .AtLeastOnce();
            mocks.ReplayAll();

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
        }

        [Test]
        public void Import_WithoutTransformableSoilModels_StopsImportAndLogs()
        {
            // Setup
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(false)
                  .Repeat
                  .AtLeastOnce();
            mocks.ReplayAll();

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
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataProgressText);
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.Expect(u => u.UpdateModelWithImportedData(Arg<IMechanismStochasticSoilModel[]>.List.ContainsAll(stochasticSoilModelCollection),
                                                                     Arg<string>.Is.Equal(validFilePath)));
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .Times(expectedNrOfStochasticSoilModels);
            mocks.ReplayAll();

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
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_ReadingStochaticSoilModelThrowsException_StopsImportAndLogs()
        {
            // Setup
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            mocks.ReplayAll();

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

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataProgressText);

            var transformedModel = mocks.Stub<IMechanismStochasticSoilModel>();

            transformer.Expect(t => t.Transform(Arg<StochasticSoilModel>.Is.NotNull))
                       .Return(transformedModel);

            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.Expect(u => u.UpdateModelWithImportedData(Arg<IMechanismStochasticSoilModel[]>.List.ContainsAll(new[]
                                                                     {
                                                                         transformedModel
                                                                     }),
                                                                     Arg<string>.Is.Equal(validFilePath)));
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .AtLeastOnce();
            mocks.ReplayAll();

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
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_TransformationThrowsException_StopsImportAndLogs()
        {
            // Setup
            const string exceptionMessage = "Some exception message.";

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .AtLeastOnce();
            transformer.Expect(t => t.Transform(null))
                       .IgnoreArguments()
                       .Throw(new ImportedDataTransformException(exceptionMessage));
            mocks.ReplayAll();

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

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return(expectedAddDataProgressText);
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Stochastische ondergrondmodellen"))
                           .Return(exceptionMessage);
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.Expect(u => u.UpdateModelWithImportedData(null, null))
                          .IgnoreArguments()
                          .Throw(updateDataException);
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .AtLeastOnce();
            mocks.ReplayAll();

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
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return("");
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            updateStrategy.Expect(u => u.UpdateModelWithImportedData(null, null))
                          .IgnoreArguments()
                          .Return(new[]
                          {
                              observableA,
                              observableB
                          });
            var filter = mocks.StrictMock<IStochasticSoilModelMechanismFilter>();
            filter.Expect(f => f.IsValidForFailureMechanism(null))
                  .IgnoreArguments()
                  .Return(true)
                  .Repeat
                  .AtLeastOnce();
            mocks.ReplayAll();

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

            // Asserts done in the TearDown method
        }

        private static void FilterFailureMechanismSpecificModel(MethodInvocation invocation, FailureMechanismType failureMechanismType)
        {
            invocation.ReturnValue = failureMechanismType == ((StochasticSoilModel) invocation.Arguments[0]).FailureMechanismType;
        }

        private class TestStochasticSoilModelCollection : ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>
        {
            public TestStochasticSoilModelCollection()
                : base(s => s.ToString(), "something", "something else") {}
        }
    }
}