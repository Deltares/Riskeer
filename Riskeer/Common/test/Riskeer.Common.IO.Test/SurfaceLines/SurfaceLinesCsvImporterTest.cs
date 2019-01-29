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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.SurfaceLines;

namespace Riskeer.Common.IO.Test.SurfaceLines
{
    [TestFixture]
    public class SurfaceLinesCsvImporterTest
    {
        private const string krpFormat = "{0}.krp.csv";
        private const string surfaceLineFormat = "{0}.csv";
        private readonly string ioTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "SurfaceLines");

        private MockRepository mocks;
        private ISurfaceLineTransformer<IMechanismSurfaceLine> transformer;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            transformer = mocks.Stub<ISurfaceLineTransformer<IMechanismSurfaceLine>>();
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
            var updateStrategy = mocks.Stub<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);

            // Call
            TestDelegate call = () => new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(null,
                                                                                         string.Empty,
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
            var updateStrategy = mocks.Stub<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);

            // Call
            TestDelegate call = () => new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(new TestSurfaceLineCollection(),
                                                                                         string.Empty,
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

            var collection = new TestSurfaceLineCollection();

            // Call
            TestDelegate call = () => new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(collection,
                                                                                         string.Empty,
                                                                                         messageProvider,
                                                                                         null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("configuration", parameter);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);

            var collection = new TestSurfaceLineCollection();

            // Call
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(collection,
                                                                              string.Empty,
                                                                              messageProvider,
                                                                              configuration);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<IMechanismSurfaceLine>>>(importer);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup

            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);

            var readSurfaceLines = new Collection<SurfaceLine>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a => readSurfaceLines.Add(a.Arguments[0] as SurfaceLine));

            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var callCount = 0;
            var progressStarted = false;
            var progressCharacteristicPointsStarted = false;

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged(delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (!progressStarted && callCount == 0)
                {
                    progressStarted = true;
                    Assert.AreEqual("Inlezen van het profielschematisatiesbestand.", currentStepName);
                    return;
                }

                if (!progressCharacteristicPointsStarted && callCount == expectedNumberOfSurfaceLines + 1)
                {
                    progressCharacteristicPointsStarted = true;
                    Assert.AreEqual("Inlezen van het karakteristieke punten-bestand.", currentStepName);
                    return;
                }

                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual($"Inlezen '{twovalidsurfacelinesCsv}'", currentStepName);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + 1 + expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual("Valideren van ingelezen data.", currentStepName);
                    Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + expectedNumberOfSurfaceLines + 2)
                {
                    Assert.AreEqual(expectedAddDataToModelProgressText, currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                callCount++;
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(validFilePath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(expectedNumberOfSurfaceLines, readSurfaceLines.Count);

            SurfaceLine firstSurfaceLine = readSurfaceLines[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.Points.First().Y);

            SurfaceLine secondSurfaceLine = readSurfaceLines[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.Points.Last().X);

            Assert.AreEqual(7, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var readSurfaceLines = new Collection<SurfaceLine>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a => readSurfaceLines.Add(a.Arguments[0] as SurfaceLine));
            mocks.ReplayAll();

            const string twovalidsurfacelinesCsv = "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string[] messages =
            {
                $"Begonnen met het inlezen van profielschematisaties uit bestand '{validFilePath}'.",
                "Profielschematisatie Rotterdam1 bevat aaneengesloten dubbele geometriepunten. Deze dubbele punten worden genegeerd.",
                $"Klaar met het inlezen van profielschematisaties uit bestand '{validFilePath}'.",
                $"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.krp.csv")})"
            };

            TestHelper.AssertLogMessagesAreGenerated(call, messages, 4);
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(validFilePath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(1, readSurfaceLines.Count);

            // Sample some of the imported data:
            SurfaceLine firstSurfaceLine = readSurfaceLines[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Point3D[] geometryPoints = firstSurfaceLine.Points.ToArray();
            Assert.AreEqual(8, geometryPoints.Length);
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[1].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[2].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreEqual(427776.654093, firstSurfaceLine.Points.First().Y);
            CollectionAssert.AllItemsAreUnique(geometryPoints);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_CancelOfImportWhenReadingSurfaceLines_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            var updateStrategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);

            var surfaceLines = new TestSurfaceLineCollection();
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged(delegate(string description, int step, int steps)
            {
                if (description.Contains("Inlezen van het profielschematisatiesbestand."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 3);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingCharacteristicPoints_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            var updateStrategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);

            var surfaceLines = new TestSurfaceLineCollection();
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van het karakteristieke punten-bestand."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 4);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportDuringProcessImportDataToModel_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            var updateStrategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);
            var surfaceLines = new TestSurfaceLineCollection();
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (step < steps
                    && description.Contains("Valideren van ingelezen data."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 4);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ImportCompletedSuccessfullyNonetheless()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            var updateStrategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            updateStrategy.Expect(us => us.UpdateSurfaceLinesWithImportedData(Arg<IMechanismSurfaceLine[]>.List.ContainsAll(surfaceLines), Arg<string>.Is.Equal(validFilePath)));
            mocks.ReplayAll();

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedAddDataToModelProgressText))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLogLevel = Tuple.Create(expectedMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLogLevel, 4);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validFilePath, messageProvider, configuration);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            bool importResult = importer.Import();
            Assert.IsFalse(importResult);

            importer.SetProgressChanged(null);

            // Call
            importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(validFilePath, surfaceLineUpdateStrategy.FilePath);
        }

        [Test]
        public void Import_PathIsInvalid_AbortImportAndLog()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            string corruptPath = validFilePath.Replace('S', Path.GetInvalidPathChars().First());

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);

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
                Assert.IsInstanceOf<ArgumentException>(loggedException);
                Assert.AreEqual(loggedException.Message, expectedLog.Item1);
            });
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_FileDoesNotExist_AbortImportAndLog()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "I_dont_exists.csv");

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(3, loggedMessages.Length);

                Exception loggedException = loggedMessages[1].Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.",
                    loggedException.Message,
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'."
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Error,
                    Level.Info
                }, loggedMessages.Select(m => m.Item2));
            });

            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_FileIsEmpty_AbortImportAndLog()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "empty.csv");

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(3, loggedMessages.Length);

                Exception loggedException = loggedMessages[1].Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.",
                    loggedException.Message,
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'."
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Error,
                    Level.Info
                }, loggedMessages.Select(m => m.Item2));
            });
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_InvalidHeader_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "InvalidHeader_LacksY1.csv");

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(3, loggedMessages.Length);

                Exception loggedException = loggedMessages[1].Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.",
                    loggedException.Message,
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'."
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Error,
                    Level.Info
                }, loggedMessages.Select(m => m.Item2));
            });
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_FileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string copyTargetPath = TestHelper.GetScratchPadPath($"{nameof(Import_FileDeletedDuringRead_AbortImportAndLog)}.csv");
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            File.Copy(validFilePath, copyTargetPath);

            try
            {
                var surfaceLines = new TestSurfaceLineCollection();
                var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
                var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
                var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, copyTargetPath, messageProvider, configuration);
                importer.SetProgressChanged((name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    File.Delete(copyTargetPath);
                });

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import();

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                    Assert.AreEqual(3, loggedMessages.Length);

                    Exception loggedException = loggedMessages[1].Item3;
                    Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                    CollectionAssert.AreEqual(new[]
                    {
                        $"Begonnen met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.",
                        loggedException.Message,
                        $"Klaar met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'."
                    }, loggedMessages.Select(m => m.Item1));

                    CollectionAssert.AreEqual(new[]
                    {
                        Level.Info,
                        Level.Error,
                        Level.Info
                    }, loggedMessages.Select(m => m.Item2));
                });
                AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
            }
            finally
            {
                // Fallback delete in case progress event is not fired:
                if (File.Exists(copyTargetPath))
                {
                    File.Delete(copyTargetPath);
                }
            }
        }

        [Test]
        public void Import_FileHasDuplicateIdentifier_AbortImportAndLog()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a => readSurfaceLines.Add(a.Arguments[0] as SurfaceLine));
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifier";
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string expectedCharacteristicPointsFile = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var duplicateDefinitionMessage = "Meerdere definities gevonden voor profielschematisatie 'Rotterdam1'. Alleen de eerste definitie wordt geïmporteerd.";
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create(duplicateDefinitionMessage, LogLevelConstant.Warn),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {expectedCharacteristicPointsFile})", LogLevelConstant.Info)
            };

            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(corruptPath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(1, readSurfaceLines.Count);
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(3, readSurfaceLines.First(sl => sl.Name == "Rotterdam1").Points.Length, "First line should have been added to the model.");
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a => readSurfaceLines.Add(a.Arguments[0] as SurfaceLine));
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.csv");

            var progressCallCount = 0;
            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, corruptPath, messageProvider, configuration);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                                          .WithLocation("op regel 3")
                                          .WithSubject("profielschematisatie 'InvalidRow'")
                                          .Build("Profielschematisatie heeft een coördinaatwaarde die niet omgezet kan worden naar een getal.");
            string characteristicPointsFilePath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.krp.csv");
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {characteristicPointsFilePath})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(corruptPath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(10, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("4  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each valid surface line checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("10")
                                .ToString());
        }

        [Test]
        public void Import_ImportingToValidTargetWithInvalidFileWithDuplicatePointsCausingRecline_SkipInvalidRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingRecline.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, path, messageProvider, configuration);

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                                          .WithLocation("op regel 2")
                                          .WithSubject("profielschematisatie 'Rotterdam1'")
                                          .Build("Profielschematisatie heeft een teruglopende geometrie (punten behoren een oplopende set L-coördinaten te hebben in het lokale coördinatenstelsel).");
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingRecline.krp.csv")})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(path, surfaceLineUpdateStrategy.FilePath);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        [Test]
        public void Import_ImportingToValidTargetWithInvalidFileWithDuplicatePointsCausingZeroLength_SkipInvalidRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingZeroLength.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, path, messageProvider, configuration);

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                                          .WithLocation("op regel 2")
                                          .WithSubject("profielschematisatie 'Rotterdam1'")
                                          .Build("Profielschematisatie heeft een geometrie die een lijn met lengte 0 beschrijft.");
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingZeroLength.krp.csv")})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(path, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(0, surfaceLineUpdateStrategy.ReadSurfaceLines.Length);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        [Test]
        public void Import_CharacteristicPointsFileDoesNotExist_Log()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var readSurfaceLines = new Collection<SurfaceLine>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a => readSurfaceLines.Add(a.Arguments[0] as SurfaceLine));
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string nonExistingCharacteristicFile = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesFile, messageProvider, configuration);

            var importResult = true;

            // Precondition
            Assert.IsTrue(File.Exists(surfaceLinesFile));
            Assert.IsFalse(File.Exists(nonExistingCharacteristicFile));

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string expectedLogMessage = $"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {nonExistingCharacteristicFile})";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 3);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(surfaceLinesFile, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
        }

        [Test]
        public void Import_CharacteristicPointsFileIsEmpty_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_EmptyCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesFile, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(5, loggedMessages.Length);

                Exception loggedException = loggedMessages[3].Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.",
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.",
                    $"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.",
                    loggedException.Message,
                    $"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'."
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Info,
                    Level.Info,
                    Level.Error,
                    Level.Info
                }, loggedMessages.Select(m => m.Item2));
            });
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CharacteristicPointsFileHasInvalidHeader_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_InvalidHeaderCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesFile, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(5, loggedMessages.Length);

                Exception loggedException = loggedMessages[3].Item3;
                Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.",
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.",
                    $"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.",
                    loggedException.Message,
                    $"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'."
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Info,
                    Level.Info,
                    Level.Error,
                    Level.Info
                }, loggedMessages.Select(m => m.Item2));
            });
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CharacteristicPointsFileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string target = "Import_FileDeletedDuringRead_AbortImportAndLog";
            const string source = "TwoValidSurfaceLines_WithCharacteristicPoints";

            string copyTargetPath = TestHelper.GetScratchPadPath(string.Format(surfaceLineFormat, target));
            string copyCharacteristicPointsTargetFileName = string.Format(krpFormat, target);
            string copyCharacteristicPointsTargetPath = TestHelper.GetScratchPadPath(copyCharacteristicPointsTargetFileName);

            string surfaceLinesPath = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, source));
            string validFilePath = Path.Combine(ioTestDataPath, string.Format(krpFormat, source));

            File.Copy(surfaceLinesPath, copyTargetPath);
            File.Copy(validFilePath, copyCharacteristicPointsTargetPath);

            try
            {
                var surfaceLines = new TestSurfaceLineCollection();
                var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
                var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
                var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, copyTargetPath, messageProvider, configuration);
                importer.SetProgressChanged((name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    if (name == $"Inlezen '{copyCharacteristicPointsTargetFileName}'")
                    {
                        File.Delete(copyCharacteristicPointsTargetPath);
                    }
                });

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import();

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                    Assert.AreEqual(5, loggedMessages.Length);

                    Exception loggedException = loggedMessages[3].Item3;
                    Assert.IsInstanceOf<CriticalFileReadException>(loggedException);
                    CollectionAssert.AreEqual(new[]
                    {
                        $"Begonnen met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.",
                        $"Klaar met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.",
                        $"Begonnen met het inlezen van karakteristieke punten uit bestand '{copyCharacteristicPointsTargetPath}'.",
                        loggedException.Message,
                        $"Klaar met het inlezen van karakteristieke punten uit bestand '{copyCharacteristicPointsTargetPath}'."
                    }, loggedMessages.Select(m => m.Item1));

                    CollectionAssert.AreEqual(new[]
                    {
                        Level.Info,
                        Level.Info,
                        Level.Info,
                        Level.Error,
                        Level.Info
                    }, loggedMessages.Select(m => m.Item2));
                });
                AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
            }
            finally
            {
                // Fallback delete in case progress event is not fired:
                if (File.Exists(copyTargetPath))
                {
                    File.Delete(copyTargetPath);
                }

                // Fallback delete in case progress event is not fired:
                if (File.Exists(copyCharacteristicPointsTargetPath))
                {
                    File.Delete(copyCharacteristicPointsTargetPath);
                }
            }
        }

        [Test]
        public void Import_CharacteristicPointsFileHasDuplicateIdentifier_LogAndSkipsLine()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            var readCharacteristicPoints = new Collection<CharacteristicPoints>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a =>
                       {
                           readSurfaceLines.Add(a.Arguments[0] as SurfaceLine);
                           readCharacteristicPoints.Add(a.Arguments[1] as CharacteristicPoints);
                       });
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifiersCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesFile, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string duplicateDefinitionMessage = "Meerdere karakteristieke punten definities gevonden voor locatie 'Rotterdam1'. Alleen de eerste definitie wordt geïmporteerd.";
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create(duplicateDefinitionMessage, LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(surfaceLinesFile, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
            Assert.AreEqual(2, readCharacteristicPoints.Count);
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(-1.02, readCharacteristicPoints.First(sl => sl.Name == "Rotterdam1").DitchPolderSide.Z, "First characteristic points definition should be added to data model.");
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
        }

        [Test]
        public void Import_TransformerThrowsTransformerException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "This is exceptional";
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything)).Throw(new ImportedDataTransformException(exceptionMessage));
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithCharacteristicPoints";
            string twovalidsurfacelinesCsv = string.Format(surfaceLineFormat, fileName);
            string validSurfaceLinesFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);
            string characteristicPointsFilePath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validSurfaceLinesFilePath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] loggedMessages = messages.ToArray();
                Assert.AreEqual(5, loggedMessages.Length);

                Exception loggedException = loggedMessages[4].Item3;
                Assert.IsInstanceOf<ImportedDataTransformException>(loggedException);
                CollectionAssert.AreEqual(new[]
                {
                    $"Begonnen met het inlezen van profielschematisaties uit bestand '{validSurfaceLinesFilePath}'.",
                    $"Klaar met het inlezen van profielschematisaties uit bestand '{validSurfaceLinesFilePath}'.",
                    $"Begonnen met het inlezen van karakteristieke punten uit bestand '{characteristicPointsFilePath}'.",
                    $"Klaar met het inlezen van karakteristieke punten uit bestand '{characteristicPointsFilePath}'.",
                    loggedException.Message
                }, loggedMessages.Select(m => m.Item1));

                CollectionAssert.AreEqual(new[]
                {
                    Level.Info,
                    Level.Info,
                    Level.Info,
                    Level.Info,
                    Level.Error
                }, loggedMessages.Select(m => m.Item2));
            });

            Assert.IsFalse(importResult);
            Assert.IsFalse(surfaceLineUpdateStrategy.Updated);
            Assert.IsNull(surfaceLineUpdateStrategy.FilePath);
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidCharacteristicPointsDefinitionDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            var readCharacteristicPoints = new Collection<CharacteristicPoints>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a =>
                       {
                           readSurfaceLines.Add(a.Arguments[0] as SurfaceLine);
                           readCharacteristicPoints.Add(a.Arguments[1] as CharacteristicPoints);
                       });
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithOneInvalidCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var progressCallCount = 0;
            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesFile, messageProvider, configuration);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                                          .WithLocation("op regel 2")
                                          .WithSubject("locatie 'Rotterdam1Invalid'")
                                          .Build("Karakteristiek punt heeft een coördinaatwaarde die niet omgezet kan worden naar een getal.");
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze locatie met karakteristieke punten wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create("Er konden geen karakteristieke punten gevonden worden voor locatie 'Rotterdam1Invalid'.", LogLevelConstant.Warn),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 6);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(surfaceLinesFile, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
            Assert.AreEqual(2, readCharacteristicPoints.Count);

            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
            Assert.AreEqual(1, readCharacteristicPoints.Count(cp => cp?.Name == "ArtifcialLocal"));

            Assert.AreEqual(12, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("3  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surface line checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("12")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            var readCharacteristicPoints = new Collection<CharacteristicPoints>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a =>
                       {
                           readSurfaceLines.Add(a.Arguments[0] as SurfaceLine);
                           readCharacteristicPoints.Add(a.Arguments[1] as CharacteristicPoints);
                       });
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithOneCharacteristicPointsLocation";
            string surfaceLinesPath = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var progressCallCount = 0;
            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesPath, messageProvider, configuration);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create("Er konden geen karakteristieke punten gevonden worden voor locatie \'Rotterdam1\'.", LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(surfaceLinesPath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
            Assert.AreEqual(2, readCharacteristicPoints.Count);

            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
            Assert.AreEqual(1, readCharacteristicPoints.Count(cp => cp?.Name == "ArtifcialLocal"));

            Assert.AreEqual(11, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("2  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surface line checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("11")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndThreeCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            var readCharacteristicPoints = new Collection<CharacteristicPoints>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a =>
                       {
                           readSurfaceLines.Add(a.Arguments[0] as SurfaceLine);
                           readCharacteristicPoints.Add(a.Arguments[1] as CharacteristicPoints);
                       });
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithThreeCharacteristicPointsLocations";
            string surfaceLinesPath = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var progressCallCount = 0;
            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, surfaceLinesPath, messageProvider, configuration);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedLogMessagesAndLevel =
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create("Karakteristieke punten gevonden zonder bijbehorende profielschematisatie voor locatie \'Extra\'.", LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(surfaceLinesPath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(2, readSurfaceLines.Count);
            Assert.AreEqual(2, readCharacteristicPoints.Count);

            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, readSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
            Assert.AreEqual(1, readCharacteristicPoints.Count(cp => cp.Name == "Rotterdam1"));
            Assert.AreEqual(1, readCharacteristicPoints.Count(cp => cp.Name == "ArtifcialLocal"));

            Assert.AreEqual(13, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("4  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surface line checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("13")
                                .ToString());
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithCharacteristicPoints_ImportSurfaceLinesToCollection()
        {
            // Setup
            var readSurfaceLines = new Collection<SurfaceLine>();
            var readCharacteristicPoints = new Collection<CharacteristicPoints>();
            transformer.Expect(t => t.Transform(Arg<SurfaceLine>.Is.Anything, Arg<CharacteristicPoints>.Is.Anything))
                       .WhenCalled(a =>
                       {
                           readSurfaceLines.Add(a.Arguments[0] as SurfaceLine);
                           readCharacteristicPoints.Add(a.Arguments[1] as CharacteristicPoints);
                       });
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            const int expectedNumberOfCharacteristicPointsDefinitions = 2;

            const string fileName = "TwoValidSurfaceLines_WithCharacteristicPoints";
            string twovalidsurfacelinesCsv = string.Format(surfaceLineFormat, fileName);
            string twovalidsurfacelinesCharacteristicPointsCsv = string.Format(krpFormat, fileName);

            string validSurfaceLinesFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);
            string validCharacteristicPointsFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var callCount = 0;
            var progressStarted = false;
            var progressCharacteristicPointsStarted = false;

            var surfaceLines = new TestSurfaceLineCollection();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, surfaceLineUpdateStrategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, validSurfaceLinesFilePath, messageProvider, configuration);
            importer.SetProgressChanged(delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (!progressStarted && callCount == 0)
                {
                    progressStarted = true;
                    Assert.AreEqual("Inlezen van het profielschematisatiesbestand.", currentStepName);
                    return;
                }

                if (!progressCharacteristicPointsStarted && callCount == expectedNumberOfSurfaceLines + 1)
                {
                    progressCharacteristicPointsStarted = true;
                    Assert.AreEqual("Inlezen van het karakteristieke punten-bestand.", currentStepName);
                    return;
                }

                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual($"Inlezen '{twovalidsurfacelinesCsv}'", currentStepName);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + expectedNumberOfCharacteristicPointsDefinitions + 1)
                {
                    Assert.AreEqual($"Inlezen '{twovalidsurfacelinesCharacteristicPointsCsv}'", currentStepName);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + expectedNumberOfCharacteristicPointsDefinitions + 2 + expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual("Valideren van ingelezen data.", currentStepName);
                    Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + expectedNumberOfCharacteristicPointsDefinitions + 2 + expectedNumberOfSurfaceLines + 1)
                {
                    Assert.AreEqual(expectedAddDataToModelProgressText, currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                callCount++;
            });

            // Precondition
            CollectionAssert.IsEmpty(surfaceLines);
            Assert.IsTrue(File.Exists(validSurfaceLinesFilePath));
            Assert.IsTrue(File.Exists(validCharacteristicPointsFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.IsTrue(surfaceLineUpdateStrategy.Updated);
            Assert.AreEqual(validSurfaceLinesFilePath, surfaceLineUpdateStrategy.FilePath);

            Assert.AreEqual(expectedNumberOfSurfaceLines, readSurfaceLines.Count);

            // Sample some of the imported data:
            SurfaceLine firstSurfaceLine = readSurfaceLines[0];
            CharacteristicPoints firstCharacteristicPoints = readCharacteristicPoints[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.Points.First().Y);
            Assert.AreEqual(new Point3D(94263.0026213, 427776.654093, -1.02), firstCharacteristicPoints.DitchPolderSide);
            Assert.AreEqual(new Point3D(94275.9126686, 427811.080886, -1.04), firstCharacteristicPoints.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstCharacteristicPoints.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(94294.9380015, 427858.191234, 1.45), firstCharacteristicPoints.DitchDikeSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstCharacteristicPoints.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(94305.3566362, 427889.900123, 1.65), firstCharacteristicPoints.DikeToeAtPolder);

            SurfaceLine secondSurfaceLine = readSurfaceLines[1];
            CharacteristicPoints secondCharacteristicPoints = readCharacteristicPoints[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.Points.Last().X);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondCharacteristicPoints.DitchPolderSide);
            Assert.AreEqual(new Point3D(4.4, 0, 2.0), secondCharacteristicPoints.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondCharacteristicPoints.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondCharacteristicPoints.DitchDikeSide);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondCharacteristicPoints.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondCharacteristicPoints.DikeToeAtPolder);

            Assert.AreEqual(10, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validSurfaceLinesFilePath));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validCharacteristicPointsFilePath));
        }

        [Test]
        public void Import_UpdateSurfaceLinesWithImportedDataThrowsUpdateDataException_ReturnFalseAndLogError()
        {
            // Setup
            var surfaceLines = new TestSurfaceLineCollection();

            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string filePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Profielschematisaties"))
                           .Return("error {0}");

            var strategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>>();
            strategy.Expect(s => s.UpdateSurfaceLinesWithImportedData(Arg<IMechanismSurfaceLine[]>.Is.NotNull,
                                                                      Arg<string>.Is.Same(filePath)
                            )).Throw(new UpdateDataException("Exception message"));
            mocks.ReplayAll();

            var configuration = new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, strategy);
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(surfaceLines, filePath, messageProvider, configuration);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Then
            const string expectedMessage = "error Exception message";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error));
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

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithCharacteristicPoints";
            string twovalidsurfacelinesCsv = string.Format(surfaceLineFormat, fileName);
            string validSurfaceLinesFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var updateStrategy = new TestSurfaceLineUpdateStrategy
            {
                UpdatedInstances = new[]
                {
                    observableA,
                    observableB
                }
            };
            var importer = new SurfaceLinesCsvImporter<IMechanismSurfaceLine>(
                new TestSurfaceLineCollection(),
                validSurfaceLinesFilePath,
                messageProvider,
                new SurfaceLinesCsvImporterConfiguration<IMechanismSurfaceLine>(transformer, updateStrategy));
            importer.Import();

            // Call
            importer.DoPostImport();

            // Asserts done in the TearDown method
        }

        private static void AssertUnsuccessfulImport(bool importResult,
                                                     TestSurfaceLineUpdateStrategy updateStrategy)
        {
            Assert.IsFalse(importResult);
            Assert.IsFalse(updateStrategy.Updated);
        }

        private class TestSurfaceLineCollection : ObservableUniqueItemCollectionWithSourcePath<IMechanismSurfaceLine>
        {
            public TestSurfaceLineCollection()
                : base(sl => sl.ToString(), "something", "something else") {}
        }

        private class TestSurfaceLineUpdateStrategy : ISurfaceLineUpdateDataStrategy<IMechanismSurfaceLine>
        {
            public bool Updated { get; private set; }
            public string FilePath { get; private set; }
            public IMechanismSurfaceLine[] ReadSurfaceLines { get; private set; }
            public IEnumerable<IObservable> UpdatedInstances { private get; set; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(IEnumerable<IMechanismSurfaceLine> surfaceLines, string sourceFilePath)
            {
                Updated = true;
                FilePath = sourceFilePath;
                ReadSurfaceLines = surfaceLines.ToArray();
                return UpdatedInstances;
            }
        }
    }
}