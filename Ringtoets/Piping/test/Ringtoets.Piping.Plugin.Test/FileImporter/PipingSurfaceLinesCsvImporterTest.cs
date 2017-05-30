// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Text;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLinesCsvImporterTest
    {
        private const string krpFormat = "{0}.krp.csv";
        private const string surfaceLineFormat = "{0}.csv";
        private readonly string ioTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines");
        private readonly string pluginSurfaceLinesTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.Plugin, "SurfaceLines");

        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
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

            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvImporter(null,
                                                                        referenceLine,
                                                                        string.Empty,
                                                                        messageProvider,
                                                                        new TestSurfaceLineUpdateStrategy());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", parameter);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var collection = new RingtoetsPipingSurfaceLineCollection();

            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvImporter(collection,
                                                                        null,
                                                                        string.Empty,
                                                                        messageProvider,
                                                                        new TestSurfaceLineUpdateStrategy());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("referenceLine", parameter);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var collection = new RingtoetsPipingSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvImporter(collection,
                                                                        referenceLine,
                                                                        string.Empty,
                                                                        null,
                                                                        new TestSurfaceLineUpdateStrategy());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", parameter);
        }

        [Test]
        public void Constructor_ModelUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var collection = new RingtoetsPipingSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvImporter(collection,
                                                                        referenceLine,
                                                                        string.Empty,
                                                                        messageProvider,
                                                                        null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("surfaceLineUpdateStrategy", parameter);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var collection = new RingtoetsPipingSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new PipingSurfaceLinesCsvImporter(collection, referenceLine, "", messageProvider, new TestSurfaceLineUpdateStrategy());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<RingtoetsPipingSurfaceLineCollection>>(importer);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var callCount = 0;
            var progressStarted = false;
            var progressCharacteristicPointsStarted = false;

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
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
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            AssertAreEqualPoint2D(new Point2D(94270.0, 427795.313769642), firstSurfaceLine.ReferenceLineIntersectionWorldPoint);

            RingtoetsPipingSurfaceLine secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);
            AssertAreEqualPoint2D(new Point2D(3.3, 0), secondSurfaceLine.ReferenceLineIntersectionWorldPoint);

            Assert.AreEqual(7, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string twovalidsurfacelinesCsv = "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427850)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var mesages = new[]
            {
                $"Begonnen met het inlezen van profielschematisaties uit bestand '{validFilePath}'.",
                "Profielschematisatie Rotterdam1 bevat aaneengesloten dubbele geometriepunten. Deze dubbele punten worden genegeerd.",
                $"Klaar met het inlezen van profielschematisaties uit bestand '{validFilePath}'.",
                $"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.krp.csv")})"
            };

            TestHelper.AssertLogMessagesAreGenerated(call, mesages, 4);
            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, 1, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Point3D[] geometryPoints = firstSurfaceLine.Points.ToArray();
            Assert.AreEqual(8, geometryPoints.Length);
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[1].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[2].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            CollectionAssert.AllItemsAreUnique(geometryPoints);
            AssertAreEqualPoint2D(new Point2D(94270.0, 427795.313769642), firstSurfaceLine.ReferenceLineIntersectionWorldPoint);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_CancelOfImportWhenReadingPipingSurfaceLines_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged(delegate(string description, int step, int steps)
            {
                if (description.Contains("Inlezen van het profielschematisatiesbestand."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 3);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingCharacteristicPoints_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van het karakteristieke punten-bestand."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 4);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CancelOfImportDuringProcessImportDataToModel_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (step < steps
                    && description.Contains("Valideren van ingelezen data."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 4);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ImportCompletedSuccessfullyNonetheless()
        {
            // Setup
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedAddDataToModelProgressText))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLogLevel = Tuple.Create(expectedMessage, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLogLevel, 4);
            AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            bool importResult = importer.Import();
            Assert.IsFalse(importResult);

            importer.SetProgressChanged(null);

            // Call
            importResult = importer.Import();

            // Assert
            AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_PathIsInvalid_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            string corruptPath = validFilePath.Replace('S', Path.GetInvalidPathChars().First());

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .Build("Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.");
            string expectedLogMessage = $"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel, 1);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_FileDoesNotExist_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "I_dont_exists.csv");

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build("Het bestand bestaat niet.");
            var expectedLogMessagesAndLevels = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevels, 3);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_FileIsEmpty_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "empty.csv");

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is leeg.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_InvalidHeader_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "InvalidHeader_LacksY1.csv");

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);
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
                var failureMechanism = new PipingFailureMechanism();
                var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
                var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), copyTargetPath, messageProvider, surfaceLineUpdateStrategy);
                importer.SetProgressChanged((name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    File.Delete(copyTargetPath);
                });

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import();

                // Assert
                string internalErrorMessage = new FileReaderErrorMessageBuilder(copyTargetPath).Build("Het bestand bestaat niet.");
                var expectedLogMessagesAndLevel = new[]
                {
                    Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);
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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifier";
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string expectedCharacteristicPointsFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string duplicateDefinitionMessage = "Meerdere definities gevonden voor profielschematisatie 'Rotterdam1'. Alleen de eerste definitie wordt geïmporteerd.";
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create(duplicateDefinitionMessage, LogLevelConstant.Warn),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {expectedCharacteristicPointsFile})", LogLevelConstant.Info)
            };

            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 1, corruptPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(3, importedSurfaceLines.First(sl => sl.Name == "Rotterdam1").Points.Length, "First line should have been added to the model.");
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900),
                new Point2D(9.8, -1),
                new Point2D(9.8, 1)
            });

            var failureMechanism = new PipingFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath, messageProvider, surfaceLineUpdateStrategy);
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
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {characteristicPointsFilePath})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, corruptPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(10, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("4  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each valid surfaceline checked against reference line.")
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

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                .WithLocation("op regel 2")
                .WithSubject("profielschematisatie 'Rotterdam1'")
                .Build("Profielschematisatie heeft een teruglopende geometrie (punten behoren een oplopende set L-coördinaten te hebben in het lokale coördinatenstelsel).");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingRecline.krp.csv")})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);
            AssertSuccessfulImport(importResult, 0, path, surfaceLineUpdateStrategy);

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

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                .WithLocation("op regel 2")
                .WithSubject("profielschematisatie 'Rotterdam1'")
                .Build("Profielschematisatie heeft een geometrie die een lijn met lengte 0 beschrijft.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{path}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Geen karakteristieke punten-bestand gevonden naast het profielschematisatiesbestand. (Verwacht bestand: {Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingZeroLength.krp.csv")})", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 4);
            AssertSuccessfulImport(importResult, 0, path, surfaceLineUpdateStrategy);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        [Test]
        public void Import_CharacteristicPointsFileDoesNotExist_Log()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines";
            string surfaceLinesFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string nonExistingCharacteristicFile = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile, messageProvider, surfaceLineUpdateStrategy);

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

            AssertSuccessfulImport(importResult, 2, surfaceLinesFile, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CharacteristicPointsFileIsEmpty_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_EmptyCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), surfaceLinesFile, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is leeg.");

            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);
            AssertUnsuccessfulImport(importResult, surfaceLineUpdateStrategy);
        }

        [Test]
        public void Import_CharacteristicPointsFileHasInvalidHeader_AbortImportAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_InvalidHeaderCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), surfaceLinesFile, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is niet geschikt om karakteristieke punten uit te lezen: koptekst komt niet overeen met wat verwacht wordt.");

            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error)
            };

            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);
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

            string surfaceLinesPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, source));
            string validFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, source));

            File.Copy(surfaceLinesPath, copyTargetPath);
            File.Copy(validFilePath, copyCharacteristicPointsTargetPath);

            try
            {
                var failureMechanism = new PipingFailureMechanism();
                var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
                var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), copyTargetPath, messageProvider, surfaceLineUpdateStrategy);
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
                string internalErrorMessage = new FileReaderErrorMessageBuilder(copyCharacteristicPointsTargetPath).Build("Het bestand bestaat niet.");
                var expectedLogMessagesAndLevel = new[]
                {
                    Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{copyTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{copyCharacteristicPointsTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{copyCharacteristicPointsTargetPath}'.", LogLevelConstant.Info),
                    Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);
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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifiersCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string duplicateDefinitionMessage = "Meerdere karakteristieke punten definities gevonden voor locatie 'Rotterdam1'. Alleen de eerste definitie wordt geïmporteerd.";
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create(duplicateDefinitionMessage, LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, surfaceLinesFile, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(-1.02, importedSurfaceLines.First(sl => sl.Name == "Rotterdam1").DitchPolderSide.Z, "First characteristic points definition should be added to data model.");
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidCharacteristicPointsDefinitionDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithOneInvalidCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 2")
                .WithSubject("locatie 'Rotterdam1Invalid'")
                .Build("Karakteristiek punt heeft een coördinaatwaarde die niet omgezet kan worden naar een getal.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesFile}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze locatie met karakteristieke punten wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create("Er konden geen karakteristieke punten gevonden worden voor locatie 'Rotterdam1Invalid'.", LogLevelConstant.Warn),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 6);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, surfaceLinesFile, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(12, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("3  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("12")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithOneCharacteristicPointsLocation";
            string surfaceLinesPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Er konden geen karakteristieke punten gevonden worden voor locatie 'Rotterdam1'.", LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, surfaceLinesPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(11, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("2  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("11")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndThreeCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const string fileName = "TwoValidSurfaceLines_WithThreeCharacteristicPointsLocations";
            string surfaceLinesPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Karakteristieke punten gevonden zonder bijbehorende profielschematisatie voor locatie 'Extra'.", LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 5);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, surfaceLinesPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(13, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("4  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("13")
                                .ToString());
        }

        [Test]
        [TestCase("Slootbodem dijkzijde", "TwoValidSurfaceLines_CharacteristicPointsInvalidBottomDitchDikeSide")]
        [TestCase("Slootbodem polderzijde", "TwoValidSurfaceLines_CharacteristicPointsInvalidBottomDitchPolderSide")]
        [TestCase("Teen dijk binnenwaarts", "TwoValidSurfaceLines_CharacteristicPointsInvalidDikeToeAtPolder")]
        [TestCase("Teen dijk buitenwaarts", "TwoValidSurfaceLines_CharacteristicPointsInvalidDikeToeAtRiver")]
        [TestCase("Insteek sloot dijkzijde", "TwoValidSurfaceLines_CharacteristicPointsInvalidDitchDikeSide")]
        [TestCase("Insteek sloot polderzijde", "TwoValidSurfaceLines_CharacteristicPointsInvalidDitchPolderSide")]
        public void Import_FileWithTwoValidLinesAndCharacteristicPointNotOnGeometry_LogInvalidPointDefinition(string characteristicPointName, string fileName)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string surfaceLinesPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string pointFormat = $"De geometrie bevat geen punt op locatie {new Point3D(0, 1, 2)} om als '{characteristicPointName}' in te stellen.";
            var expectedLogMessages = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand '{surfaceLinesPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Begonnen met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van karakteristieke punten uit bestand '{corruptPath}'.", LogLevelConstant.Info),
                Tuple.Create($"Karakteristiek punt van profielschematisatie 'Rotterdam1Invalid' is overgeslagen. {pointFormat}", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessages);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, surfaceLinesPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(12, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("3  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : 1 call to process the imported surface lines.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("12")
                                .ToString());
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithCharacteristicPoints_ImportSurfaceLinesToCollection()
        {
            // Setup
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 2;
            const int expectedNumberOfCharacteristicPointsDefinitions = 2;

            const string fileName = "TwoValidSurfaceLines_WithCharacteristicPoints";
            string twovalidsurfacelinesCsv = string.Format(surfaceLineFormat, fileName);
            string twovalidsurfacelinesCharacteristicPointsCsv = string.Format(krpFormat, fileName);

            string validSurfaceLinesFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, twovalidsurfacelinesCsv);
            string validCharacteristicPointsFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var callCount = 0;
            var progressStarted = false;
            var progressCharacteristicPointsStarted = false;

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validSurfaceLinesFilePath, messageProvider, surfaceLineUpdateStrategy);
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
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validSurfaceLinesFilePath));
            Assert.IsTrue(File.Exists(validCharacteristicPointsFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validSurfaceLinesFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            Assert.AreEqual(new Point3D(94263.0026213, 427776.654093, -1.02), firstSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D(94275.9126686, 427811.080886, -1.04), firstSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(94294.9380015, 427858.191234, 1.45), firstSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(94305.3566362, 427889.900123, 1.65), firstSurfaceLine.DikeToeAtPolder);

            RingtoetsPipingSurfaceLine secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D(4.4, 0, 2.0), secondSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(10, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validSurfaceLinesFilePath));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validCharacteristicPointsFilePath));
        }

        [Test]
        public void Import_DoesNotInterSectReferenceLine_SkipRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 1;
            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            const string message = "Profielschematisatie ArtifcialLocal doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielschematisatie een lokaal coördinaatsysteem heeft.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(message, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel);

            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_DoesInterSectReferenceLineMultipleTimes_SkipRowAndLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            const int expectedNumberOfSurfaceLines = 1;
            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775),
                new Point2D(94270, 427812),
                new Point2D(94271, 427776),
                new Point2D(94271, 427813)
            });

            var failureMechanism = new PipingFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            const string message = "Profielschematisatie Rotterdam1 doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(message, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel);

            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("ArtifcialLocal", firstSurfaceLine.Name);
            Assert.AreEqual(3, firstSurfaceLine.Points.Length);
            Assert.AreEqual(0.0, firstSurfaceLine.StartingWorldPoint.Y);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidDikeToeAtRiverAndDikeToeAtPolder")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsDikeToeAtRiverAndDikeToeAtPolderSame")]
        public void Import_ExitPointGreaterOrEqualToEntryPoint_SkipRowAndLog(string fileName)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            const string message = "Het uittredepunt moet landwaarts van het intredepunt liggen voor locatie ArtifcialLocal.";
            Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(message, LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel);

            RingtoetsPipingSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, 1, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
        }

        [Test]
        public void Import_UpdateSurfaceLinesWithImportedDataThrowsUpdateDataException_ReturnFalseAndLogError()
        {
            // Setup
            var importTarget = new RingtoetsPipingSurfaceLineCollection();

            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string filePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Profielschematisaties"))
                           .Return("error {0}");

            var strategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy>();
            strategy.Expect(s => s.UpdateSurfaceLinesWithImportedData(
                                Arg<RingtoetsPipingSurfaceLineCollection>.Is.Same(importTarget),
                                Arg<RingtoetsPipingSurfaceLine[]>.Is.NotNull,
                                Arg<string>.Is.Same(filePath)
                            )).Throw(new UpdateDataException("Exception message"));
            mocks.ReplayAll();

            var referenceLine = new ReferenceLine();

            var importer = new PipingSurfaceLinesCsvImporter(importTarget, referenceLine, filePath, messageProvider, strategy);

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
            string validSurfaceLinesFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var updateStrategy = new TestSurfaceLineUpdateStrategy();
            updateStrategy.UpdatedInstances = new[]
            {
                observableA,
                observableB
            };
            var importer = new PipingSurfaceLinesCsvImporter(
                new RingtoetsPipingSurfaceLineCollection(),
                referenceLine,
                validSurfaceLinesFilePath,
                messageProvider,
                updateStrategy);
            importer.Import();

            // Call
            importer.DoPostImport();

            // Asserts done in the TearDown method
        }

        private static RingtoetsPipingSurfaceLine[] AssertSuccessfulImport(bool importResult,
                                                                           int expectedSurfaceLineCount,
                                                                           string expectedFilePath,
                                                                           TestSurfaceLineUpdateStrategy updateStrategy)
        {
            Assert.IsTrue(importResult);
            Assert.IsTrue(updateStrategy.Updated);
            Assert.AreEqual(expectedFilePath, updateStrategy.FilePath);

            string message = $"Ensure only {expectedSurfaceLineCount} valid surfacelines are imported.";
            Assert.AreEqual(expectedSurfaceLineCount, updateStrategy.ReadSurfaceLines.Length, message);

            return updateStrategy.ReadSurfaceLines;
        }

        private static void AssertUnsuccessfulImport(bool importResult,
                                                     TestSurfaceLineUpdateStrategy updateStrategy)
        {
            Assert.IsFalse(importResult);
            Assert.IsFalse(updateStrategy.Updated);
        }

        private static void AssertAreEqualPoint2D(Point2D expectedPoint, Point2D actualPoint)
        {
            Assert.IsTrue(Math2D.AreEqualPoints(expectedPoint, actualPoint),
                          string.Format("Expected point: {0}" + Environment.NewLine + "Actual point: {1}",
                                        expectedPoint, actualPoint));
        }

        private class TestSurfaceLineUpdateStrategy : ISurfaceLineUpdateDataStrategy
        {
            public bool Updated { get; private set; }
            public string FilePath { get; private set; }
            public RingtoetsPipingSurfaceLine[] ReadSurfaceLines { get; private set; }
            public IEnumerable<IObservable> UpdatedInstances { private get; set; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(RingtoetsPipingSurfaceLineCollection targetDataCollection,
                                                                               IEnumerable<RingtoetsPipingSurfaceLine> readRingtoetsPipingSurfaceLines,
                                                                               string sourceFilePath)
            {
                Updated = true;
                FilePath = sourceFilePath;
                ReadSurfaceLines = readRingtoetsPipingSurfaceLines.ToArray();
                return UpdatedInstances;
            }
        }
    }
}