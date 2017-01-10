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
using System.IO;
using System.Linq;
using System.Text;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;
using PipingIOResources = Ringtoets.Piping.IO.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLinesCsvImporterTest
    {
        private readonly string ioTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines");
        private readonly string pluginSurfaceLinesTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.Plugin, "SurfaceLines");

        private readonly string krpFormat = "{0}.krp.csv";
        private readonly string surfaceLineFormat = "{0}.csv";

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            var list = new ObservableList<RingtoetsPipingSurfaceLine>();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new PipingSurfaceLinesCsvImporter(list, referenceLine, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ObservableList<RingtoetsPipingSurfaceLine>>>(importer);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 2;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
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

            int callCount = 0;
            bool progressStarted = false;
            bool progressCharacteristicPointsStarted = false;

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);
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
                    Assert.AreEqual(string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_, twovalidsurfacelinesCsv), currentStepName);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + 1 + expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual("Geïmporteerde data toevoegen aan het toetsspoor.", currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                callCount++;
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

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

            Assert.AreEqual(6, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
        {
            // Setup
            var twovalidsurfacelinesCsv = "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427850)
            });

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var mesages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              validFilePath),
                "Profielschematisatie Rotterdam1 bevat aaneengesloten dubbele geometriepunten. Deze dubbele punten worden genegeerd.",
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              validFilePath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.krp.csv"))
            };

            TestHelper.AssertLogMessagesAreGenerated(call, mesages, 4);
            Assert.IsTrue(importResult);
            var importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(1, importTargetArray.Length);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            var geometryPoints = firstSurfaceLine.Points.ToArray();
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
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);
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

            bool importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Profielschematisaties importeren afgebroken. Geen data ingelezen.", 3);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingCharacteristicPoints_CancelsImportAndLogs()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);
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

            bool importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Profielschematisaties importeren afgebroken. Geen data ingelezen.", 4);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
        }

        [Test]
        public void Import_CancelOfImportDuringAddingDataToModel_CancelsImportAndLogs()
        {
            // Setup
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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (step < steps
                    && description.Contains("Geïmporteerde data toevoegen aan het toetsspoor."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            bool importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Profielschematisaties importeren afgebroken. Geen data ingelezen.", 4);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            var importResult = importer.Import();
            Assert.IsFalse(importResult);

            importer.SetProgressChanged(null);

            // Call
            importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);
        }

        [Test]
        public void Import_PathIsInvalid_AbortImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            string corruptPath = validFilePath.Replace('S', Path.GetInvalidPathChars().First());

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath);

            bool importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(string.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                                                                             string.Join(", ", Path.GetInvalidFileNameChars())));
            var expectedLogMessage = string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_FileDoesNotExist_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "I_dont_exists.csv");

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(UtilsResources.Error_File_does_not_exist);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              corruptPath)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 3);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_FileIsEmpty_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "empty.csv");

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(UtilsResources.Error_File_empty);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 3);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_InvalidHeader_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "InvalidHeader_LacksY1.csv");

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), corruptPath);

            bool importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              corruptPath)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 3);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_FileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            var copyTargetPath = "Import_FileDeletedDuringRead_AbortImportAndLog.csv";
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            File.Copy(validFilePath, copyTargetPath);

            try
            {
                var failureMechanism = new PipingFailureMechanism();

                var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), copyTargetPath);
                importer.SetProgressChanged((name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    File.Delete(copyTargetPath);
                });

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import();

                // Assert
                string internalErrorMessage = new FileReaderErrorMessageBuilder(copyTargetPath).Build(UtilsResources.Error_File_does_not_exist);
                var expectedLogMessages = new[]
                {
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                                  copyTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                                  copyTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                  internalErrorMessage)
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 3);
                Assert.IsFalse(importResult);
                CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                         "No items should be added to collection when import is aborted.");
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
            var fileName = "TwoValidSurfaceLines_DuplicateIdentifier";
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string expectedCharacteristicPointsFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900)
            });

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string duplicateDefinitionMessage = string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_,
                                                              "Rotterdam1");
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              corruptPath),
                duplicateDefinitionMessage,
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, expectedCharacteristicPointsFile)
            };

            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 4);
            Assert.IsTrue(importResult);

            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count,
                            "Ensure only the one valid surfaceline has been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(3, failureMechanism.SurfaceLines.First(sl => sl.Name == "Rotterdam1").Points.Length, "First line should have been added to the model.");
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
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

            int progressCallCount = 0;
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, corruptPath);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 3")
                .WithSubject("profielschematisatie 'InvalidRow'")
                .Build(PipingIOResources.Error_SurfaceLine_has_not_double);
            string characteristicPointsFilePath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.krp.csv");
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              characteristicPointsFilePath)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 4);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(9, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("4  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each valid surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("9")
                                .ToString());
        }

        [Test]
        public void Import_ImportingToValidTargetWithInvalidFileWithDuplicatePointsCausingRecline_SkipInvalidRowAndLog()
        {
            // Setup
            var twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingRecline.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path);

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
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              path),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              path),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingRecline.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 4);
            Assert.IsTrue(importResult);
            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        [Test]
        public void Import_ImportingToValidTargetWithInvalidFileWithDuplicatePointsCausingZeroLength_SkipInvalidRowAndLog()
        {
            // Setup
            var twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingZeroLength.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), path);

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
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              path),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              path),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingZeroLength.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 4);
            Assert.IsTrue(importResult);
            var importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        [Test]
        public void Import_CharacteristicPointsFileDoesNotExist_Log()
        {
            // Setup
            var fileName = "TwoValidSurfaceLines";
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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile);

            var importResult = true;

            // Precondition
            Assert.IsTrue(File.Exists(surfaceLinesFile));
            Assert.IsFalse(File.Exists(nonExistingCharacteristicFile));

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessage = string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                                                   nonExistingCharacteristicFile);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 3);
            Assert.IsTrue(importResult);
            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count);
        }

        [Test]
        public void Import_CharacteristicPointsFileIsEmpty_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_EmptyCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), surfaceLinesFile);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(UtilsResources.Error_File_empty);

            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_CharacteristicPointsFileHasInvalidHeader_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_InvalidHeaderCharacteristicPoints";
            string surfaceLinesFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), surfaceLinesFile);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(PipingIOResources.CharacteristicPointsCsvReader_File_invalid_header);

            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage)
            };

            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
        }

        [Test]
        public void Import_CharacteristicPointsFileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            const string target = "Import_FileDeletedDuringRead_AbortImportAndLog";
            const string source = "TwoValidSurfaceLines_WithCharacteristicPoints";

            var copyTargetPath = string.Format(surfaceLineFormat, target);
            var copyCharacteristicPointsTargetPath = string.Format(krpFormat, target);
            string surfaceLinesPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, source));
            string validFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, source));
            File.Copy(surfaceLinesPath, copyTargetPath);
            File.Copy(validFilePath, copyCharacteristicPointsTargetPath);

            try
            {
                var failureMechanism = new PipingFailureMechanism();

                var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, new ReferenceLine(), copyTargetPath);
                importer.SetProgressChanged((name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    if (name == string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_,
                                              copyCharacteristicPointsTargetPath))
                    {
                        File.Delete(copyCharacteristicPointsTargetPath);
                    }
                });

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import();

                // Assert
                string internalErrorMessage = new FileReaderErrorMessageBuilder(copyCharacteristicPointsTargetPath).Build(UtilsResources.Error_File_does_not_exist);
                var expectedLogMessages = new[]
                {
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                                  copyTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                                  copyTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                                  copyCharacteristicPointsTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                                  copyCharacteristicPointsTargetPath),
                    string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                  internalErrorMessage)
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
                Assert.IsFalse(importResult);
                CollectionAssert.IsEmpty(failureMechanism.SurfaceLines,
                                         "No items should be added to collection when import is aborted.");
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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string duplicateDefinitionMessage = string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
                                                              "Rotterdam1");
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                duplicateDefinitionMessage
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(-1.02, failureMechanism.SurfaceLines.First(sl => sl.Name == "Rotterdam1").DitchPolderSide.Z, "First characteristic points definition should be added to data model.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidCharacteristicPointsDefinitionDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
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

            int progressCallCount = 0;
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesFile);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 2")
                .WithSubject("locatie 'Rotterdam1Invalid'")
                .Build(PipingIOResources.Error_CharacteristicPoint_has_not_double);
            var expectedLogMessages = new[]
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesFile),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                              internalErrorMessage),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_,
                              "Rotterdam1Invalid"),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 6);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(11, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("3  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("11")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
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

            int progressCallCount = 0;
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string[] expectedLogMessages =
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_,
                              "Rotterdam1")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(10, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("2  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("10")
                                .ToString());
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndThreeCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
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

            int progressCallCount = 0;
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string[] expectedLogMessages =
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Characteristic_points_found_for_unknown_SurfaceLine_0_,
                              "Extra")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 5);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(12, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("4  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("12")
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

            int progressCallCount = 0;
            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, surfaceLinesPath);
            importer.SetProgressChanged((name, step, steps) => progressCallCount++);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var pointFormat = string.Format(PipingDataResources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                            new Point3D(0, 1, 2),
                                            characteristicPointName);
            string[] expectedLogMessages =
            {
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                              surfaceLinesPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                              corruptPath),
                string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                              "Rotterdam1Invalid",
                              pointFormat)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(11, progressCallCount,
                            new StringBuilder()
                                .AppendLine("Expected number of calls:")
                                .AppendLine("1  : Start reading surface lines file.")
                                .AppendLine("3  : 1 call for each read surface line, +1 for index 0.")
                                .AppendLine("1  : Start reading characteristic points file.")
                                .AppendLine("3  : 1 call for each set of characteristic points for a locations being read, +1 for index 0.")
                                .AppendLine("1  : Start adding data to failure mechanism.")
                                .AppendLine("2  : 1 call for each surfaceline checked against reference line.")
                                .AppendLine("-- +")
                                .AppendLine("11")
                                .ToString());
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithCharacteristicPoints_ImportSurfaceLinesToCollection()
        {
            // Setup
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

            int callCount = 0;
            bool progressStarted = false;
            bool progressCharacteristicPointsStarted = false;

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validSurfaceLinesFilePath);
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
                    Assert.AreEqual("Geïmporteerde data toevoegen aan het toetsspoor.", currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                callCount++;
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validSurfaceLinesFilePath));
            Assert.IsTrue(File.Exists(validCharacteristicPointsFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

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

            Assert.AreEqual(9, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validSurfaceLinesFilePath));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validCharacteristicPointsFilePath));
        }

        [Test]
        public void Import_DoesNotInterSectReferenceLine_SkipRowAndLog()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 1;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var failureMechanism = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var mesagge = "Profielschematisatie ArtifcialLocal doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielschematisatie een lokaal coördinaatsysteem heeft.";
            TestHelper.AssertLogMessageIsGenerated(call, mesagge);
            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.IsTrue(importResult);
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

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
            const int expectedNumberOfSurfaceLines = 1;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var mesagge = "Profielschematisatie Rotterdam1 doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, mesagge);
            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.IsTrue(importResult);
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

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

            var importer = new PipingSurfaceLinesCsvImporter(failureMechanism.SurfaceLines, referenceLine, validFilePath);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            var message = string.Format(PipingPluginResources.PipingSurfaceLinesCsvImporter_CheckCharacteristicPoints_EntryPointL_greater_or_equal_to_ExitPointL_for_0_,
                                        "ArtifcialLocal");
            TestHelper.AssertLogMessageIsGenerated(call, message);
            Assert.IsTrue(importResult);

            RingtoetsPipingSurfaceLine[] importTargetArray = failureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(1, importTargetArray.Length);

            // Sample some of the imported data:
            RingtoetsPipingSurfaceLine firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
        }

        private static void AssertAreEqualPoint2D(Point2D expectedPoint, Point2D actualPoint)
        {
            Assert.IsTrue(Math2D.AreEqualPoints(expectedPoint, actualPoint),
                          string.Format("Expected point: {0}" + Environment.NewLine + "Actual point: {1}",
                                        expectedPoint, actualPoint));
        }
    }
}