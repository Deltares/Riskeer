﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "StochasticSoilModelReader");
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
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var filter = mocks.Stub<IStochasticSoilModelMechanismFilter>();
            var updateStrategy = mocks.Stub<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();
            mocks.ReplayAll();

            var collection = new TestStochasticSoilModelCollection();
            var configuration = new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(transformer, filter, updateStrategy);

            // Call
            TestDelegate call = () => new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                collection,
                null,
                messageProvider,
                configuration);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", parameter);
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

                StringAssert.EndsWith($"{string.Empty} {Environment.NewLine}Het bestand wordt overgeslagen.",
                                      expectedLog.Item1);
                Assert.AreEqual(Level.Error, expectedLog.Item2);
                Assert.IsInstanceOf<CriticalFileReadException>(expectedLog.Item3);
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

                StringAssert.EndsWith($"{string.Empty} {Environment.NewLine}Het bestand wordt overgeslagen.",
                                      expectedLog.Item1);
                Assert.AreEqual(Level.Error, expectedLog.Item2);
                Assert.IsInstanceOf<CriticalFileReadException>(expectedLog.Item3);
            });

            Assert.AreEqual(1, progress);
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase(FailureMechanismType.Piping, 3)]
        [TestCase(FailureMechanismType.Stability, 3)]
        [TestCase(FailureMechanismType.None, 0)]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilModelToCollection(
            FailureMechanismType failureMechanismType,
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
            var updateStrategy = mocks.StrictMock<IStochasticSoilModelUpdateModelStrategy<IMechanismStochasticSoilModel>>();

            if (nrOfFailureMechanismSpecificModelsInDatabase > 0)
            {
                messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                               .Return(expectedAddDataText);

                updateStrategy.Expect(u => u.UpdateModelWithImportedData(null, null)).IgnoreArguments().WhenCalled(invocation =>
                {
                    var soilModels = (IEnumerable<IMechanismStochasticSoilModel>) invocation.Arguments[0];
                    var filePath = (string) invocation.Arguments[1];

                    Assert.AreEqual(nrOfFailureMechanismSpecificModelsInDatabase, soilModels.Count());
                    Assert.AreEqual(validFilePath, filePath);
                });
            }

            mocks.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var importer = new StochasticSoilModelImporter<IMechanismStochasticSoilModel>(
                new TestStochasticSoilModelCollection(),
                validFilePath,
                messageProvider,
                new StochasticSoilModelImporterConfiguration<IMechanismStochasticSoilModel>(
                    transformer,
                    filter,
                    updateStrategy));

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

            if (nrOfFailureMechanismSpecificModelsInDatabase > 0)
            {
                expectedProgressMessages.Add(new ProgressNotification(expectedAddDataText, 1, 1));
            }
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

        private static void FilterFailureMechanismSpecificModel(MethodInvocation invocation, FailureMechanismType failureMechanismType)
        {
            invocation.ReturnValue = failureMechanismType == ((StochasticSoilModel) invocation.Arguments[0]).FailureMechanismType;
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

        private class TestStochasticSoilModelCollection : ObservableUniqueItemCollectionWithSourcePath<IMechanismStochasticSoilModel>
        {
            public TestStochasticSoilModelCollection()
                : base(s => s.ToString(), "something", "something else") {}
        }
    }
}