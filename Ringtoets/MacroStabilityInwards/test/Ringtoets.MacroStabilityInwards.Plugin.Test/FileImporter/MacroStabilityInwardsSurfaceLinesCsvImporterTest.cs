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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLinesCsvImporterTest
    {
        private const string surfaceLineFormat = "{0}.csv";
        private readonly string ioTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO, "SurfaceLines");
        private readonly string pluginTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.Plugin, "SurfaceLines");

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
            TestDelegate call = () => new MacroStabilityInwardsSurfaceLinesCsvImporter(null,
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

            var collection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSurfaceLinesCsvImporter(collection,
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
            var collection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSurfaceLinesCsvImporter(collection,
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

            var collection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSurfaceLinesCsvImporter(collection,
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

            var collection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(collection, referenceLine, "", messageProvider, new TestSurfaceLineUpdateStrategy());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<RingtoetsMacroStabilityInwardsSurfaceLineCollection>>(importer);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var callCount = 0;
            var progressStarted = false;

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged(delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (!progressStarted && callCount == 0)
                {
                    progressStarted = true;
                    Assert.AreEqual("Inlezen van het profielschematisatiesbestand.", currentStepName);
                    return;
                }

                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual($"Inlezen \'{twovalidsurfacelinesCsv}\'", currentStepName);
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
            RingtoetsMacroStabilityInwardsSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsMacroStabilityInwardsSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            AssertAreEqualPoint2D(new Point2D(94270.0, 427795.313769642), firstSurfaceLine.ReferenceLineIntersectionWorldPoint);

            RingtoetsMacroStabilityInwardsSurfaceLine secondSurfaceLine = importTargetArray[1];
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var mesages = new[]
            {
                $"Begonnen met het inlezen van profielschematisaties uit bestand \'{validFilePath}\'.",
                "Profielschematisatie Rotterdam1 bevat aaneengesloten dubbele geometriepunten. Deze dubbele punten worden genegeerd.",
                $"Klaar met het inlezen van profielschematisaties uit bestand \'{validFilePath}\'."
            };

            TestHelper.AssertLogMessagesAreGenerated(call, mesages, 3);
            RingtoetsMacroStabilityInwardsSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, 1, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsMacroStabilityInwardsSurfaceLine firstSurfaceLine = importTargetArray[0];
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
        public void Import_CancelOfImportWhenReadingMacroStabilityInwardsSurfaceLines_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Profielschematisaties")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
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

            const string fileName = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(pluginTestDataPath, fileName);

            var referenceLine = new ReferenceLine();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains($"Inlezen '{fileName}'"))
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
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
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 3);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
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
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLogLevel, 3);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build("Het bestand bestaat niet.");
            var expectedLogMessagesAndLevels = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info)
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is leeg.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.", LogLevelConstant.Error),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info)
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
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
                var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), copyTargetPath, messageProvider, surfaceLineUpdateStrategy);
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
                    Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{copyTargetPath}\'.", LogLevelConstant.Info),
                    Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{copyTargetPath}\'.", LogLevelConstant.Info),
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
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath, messageProvider, surfaceLineUpdateStrategy);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string duplicateDefinitionMessage = "Meerdere definities gevonden voor profielschematisatie \'Rotterdam1\'. Alleen de eerste definitie wordt geïmporteerd.";
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create(duplicateDefinitionMessage, LogLevelConstant.Warn)
            };

            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);

            RingtoetsMacroStabilityInwardsSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 1, corruptPath, surfaceLineUpdateStrategy);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var progressCallCount = 0;
            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath, messageProvider, surfaceLineUpdateStrategy);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 3")
                .WithSubject("profielschematisatie 'InvalidRow'")
                .Build("Profielschematisatie heeft een coördinaatwaarde die niet omgezet kan worden naar een getal.");
            var expectedLogMessagesAndLevel = new[]
            {
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{corruptPath}\'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);

            RingtoetsMacroStabilityInwardsSurfaceLine[] importedSurfaceLines = AssertSuccessfulImport(importResult, 2, corruptPath, surfaceLineUpdateStrategy);
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, importedSurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(9, progressCallCount,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path, messageProvider, surfaceLineUpdateStrategy);

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
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{path}\'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{path}\'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path, messageProvider, surfaceLineUpdateStrategy);

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
                Tuple.Create($"Begonnen met het inlezen van profielschematisaties uit bestand \'{path}\'.", LogLevelConstant.Info),
                Tuple.Create($"Klaar met het inlezen van profielschematisaties uit bestand \'{path}\'.", LogLevelConstant.Info),
                Tuple.Create($"{internalErrorMessage} \r\nDeze profielschematisatie wordt overgeslagen.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedLogMessagesAndLevel, 3);
            AssertSuccessfulImport(importResult, 0, path, surfaceLineUpdateStrategy);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

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

            RingtoetsMacroStabilityInwardsSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsMacroStabilityInwardsSurfaceLine firstSurfaceLine = importTargetArray[0];
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLineUpdateStrategy = new TestSurfaceLineUpdateStrategy();
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath, messageProvider, surfaceLineUpdateStrategy);

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

            RingtoetsMacroStabilityInwardsSurfaceLine[] importTargetArray = AssertSuccessfulImport(importResult, expectedNumberOfSurfaceLines, validFilePath, surfaceLineUpdateStrategy);

            // Sample some of the imported data:
            RingtoetsMacroStabilityInwardsSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("ArtifcialLocal", firstSurfaceLine.Name);
            Assert.AreEqual(3, firstSurfaceLine.Points.Length);
            Assert.AreEqual(0.0, firstSurfaceLine.StartingWorldPoint.Y);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_UpdateSurfaceLinesWithImportedDataThrowsUpdateDataException_ReturnFalseAndLogError()
        {
            // Setup
            var importTarget = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();

            const string twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string filePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Profielschematisaties"))
                           .Return("error {0}");

            var strategy = mocks.StrictMock<ISurfaceLineUpdateDataStrategy>();
            strategy.Expect(s => s.UpdateSurfaceLinesWithImportedData(
                                Arg<RingtoetsMacroStabilityInwardsSurfaceLineCollection>.Is.Same(importTarget),
                                Arg<RingtoetsMacroStabilityInwardsSurfaceLine[]>.Is.NotNull,
                                Arg<string>.Is.Same(filePath)
                            )).Throw(new UpdateDataException("Exception message"));
            mocks.ReplayAll();

            var referenceLine = new ReferenceLine();

            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(importTarget, referenceLine, filePath, messageProvider, strategy);

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

            const string fileName = "TwoValidSurfaceLines";
            string twovalidsurfacelinesCsv = string.Format(surfaceLineFormat, fileName);
            string validSurfaceLinesFilePath = Path.Combine(pluginTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var updateStrategy = new TestSurfaceLineUpdateStrategy
            {
                UpdatedInstances = new[]
                {
                    observableA,
                    observableB
                }
            };
            var importer = new MacroStabilityInwardsSurfaceLinesCsvImporter(
                new RingtoetsMacroStabilityInwardsSurfaceLineCollection(),
                referenceLine,
                validSurfaceLinesFilePath,
                messageProvider,
                updateStrategy);
            importer.Import();

            // Call
            importer.DoPostImport();

            // Asserts done in the TearDown method
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine[] AssertSuccessfulImport(bool importResult,
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
            public RingtoetsMacroStabilityInwardsSurfaceLine[] ReadSurfaceLines { get; private set; }
            public IEnumerable<IObservable> UpdatedInstances { private get; set; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(RingtoetsMacroStabilityInwardsSurfaceLineCollection targetDataCollection,
                                                                               IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> readSurfaceLines,
                                                                               string sourceFilePath)
            {
                Updated = true;
                FilePath = sourceFilePath;
                ReadSurfaceLines = readSurfaceLines.ToArray();
                return UpdatedInstances;
            }
        }
    }
}