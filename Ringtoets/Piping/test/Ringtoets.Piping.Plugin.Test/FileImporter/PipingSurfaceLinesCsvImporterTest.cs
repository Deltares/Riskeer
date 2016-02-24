using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using PipingIOResources = Ringtoets.Piping.IO.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLinesCsvImporterTest
    {
        private readonly string ioTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines");
        private readonly string pluginTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.Plugin, "SurfaceLines");

        private string krpFormat = ("{0}.krp.csv");
        private string surfaceLineFormat = ("{0}.csv");

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new PipingSurfaceLinesCsvImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            Assert.AreEqual(typeof(ICollection<RingtoetsPipingSurfaceLine>), importer.SupportedItemType);
            var expectedFileFilter = String.Format("{0} {1} (*.csv)|*.csv",
                                                   PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, ApplicationResources.Csv_file_name);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 2;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            importer.ProgressChanged = delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual(String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_, twovalidsurfacelinesCsv), currentStepName);
                }
                else if (callCount == expectedNumberOfSurfaceLines + 1)
                {
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                callCount++;
            };

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Count());
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);

            var secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Count());
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);

            Assert.AreEqual(4, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
        {
            // Setup
            var twovalidsurfacelinesCsv = "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            var mesages = new[]
            {
                "Profielmeting Rotterdam1 bevat aaneengesloten dubbele geometrie punten, welke zijn genegeerd.",
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.krp.csv"))
            };

            TestHelper.AssertLogMessagesAreGenerated(call, mesages, 2);
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(1, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            var geometryPoints = firstSurfaceLine.Points.ToArray();
            Assert.AreEqual(8, geometryPoints.Length);
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[1].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreNotEqual(geometryPoints[0].X, geometryPoints[2].X,
                               "Originally duplicate points at the start have been removed.");
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            CollectionAssert.AllItemsAreUnique(geometryPoints);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 2;
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(observableSurfaceLinesList, validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);
        }

        [Test]
        public void Import_PathIsInvalid_AbortImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            string corruptPath = validFilePath.Replace('S', Path.GetInvalidPathChars().First());

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(String.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                                                                          String.Join(", ", Path.GetInvalidFileNameChars())));
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileDoesNotExist_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "I_dont_exists.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(UtilsResources.Error_File_does_not_exist);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileIsEmpty_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "empty.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(UtilsResources.Error_File_empty);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileHasDuplicateIdentifier_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(pluginTestDataPath, "TwoValidSurfaceLines_DuplicateIdentifier.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_, "Rotterdam1");
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_InvalidHeader_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "InvalidHeader_LacksY1.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
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
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                var importer = new PipingSurfaceLinesCsvImporter();
                importer.ProgressChanged = (name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    File.Delete(copyTargetPath);
                };

                var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
                observableSurfaceLinesList.Attach(observer);

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import(observableSurfaceLinesList, copyTargetPath);

                // Assert
                var internalErrorMessage = new FileReaderErrorMessageBuilder(copyTargetPath).Build(UtilsResources.Error_File_does_not_exist);
                var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                       internalErrorMessage);
                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
                Assert.IsFalse(importResult);
                CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                         "No items should be added to collection when import is aborted.");
                mocks.VerifyAll(); // Expect no calls on 'observer'
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
        public void Import_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 3")
                .WithSubject("profielmeting 'InvalidRow'")
                .Build(PipingIOResources.Error_SurfaceLine_has_not_double);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(5, progressCallCount,
                            "Expect 1 call for each surfaceline (3 in total) +1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithInValidFileWithDuplicatePointsCausingRecline_SkipInvalidRowAndLog()
        {
            // Setup
            var twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingRecline.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, path);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                .WithLocation("op regel 2")
                .WithSubject("profielmeting 'Rotterdam1'")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
            var expectedLogMessages = new []
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingRecline.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithInValidFileWithDuplicatePointsCausingZeroLength_SkipInvalidRowAndLog()
        {
            // Setup
            var twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingZeroLength.csv";
            string path = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, path);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                .WithLocation("op regel 2")
                .WithSubject("profielmeting 'Rotterdam1'")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingZeroLength.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_CancelOfImportWithCharacteristicPointsAfterSurfaceLinesRead_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_CharacteristicPointsFileDoesNotExist_Log()
        {
            // Setup
            var fileName = "TwoValidSurfaceLines";
            string surfaceLineFile = Path.Combine(ioTestDataPath, string.Format(surfaceLineFormat, fileName));
            string nonExistingCharacteristicFile = Path.Combine(ioTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Precondition
            Assert.IsTrue(File.Exists(surfaceLineFile));
            Assert.IsFalse(File.Exists(nonExistingCharacteristicFile));

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLineFile);

            // Assert
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                                                   nonExistingCharacteristicFile);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);
            Assert.AreEqual(2, observableSurfaceLinesList.Count);
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_CharacteristicPointsFileIsEmpty_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_EmptyCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLineFile);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(UtilsResources.Error_File_empty);

            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_,
                              corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_CharacteristicPointsFileHasInvalidHeader_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_InvalidHeaderCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLineFile);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(PipingIOResources.CharacteristicPointsCsvReader_File_invalid_header);

            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_,
                              corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                              internalErrorMessage)
            };

            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_CharacteristicPointsFileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            const string target = "Import_FileDeletedDuringRead_AbortImportAndLog";
            const string source = "TwoValidSurfaceLines_WithCharacteristicPoints";

            var copyTargetPath = string.Format(surfaceLineFormat, target);
            var copyCharacteristicPointsTargetPath = string.Format(krpFormat, target);
            string surfaceLines = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, source));
            string validFilePath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, source));
            File.Copy(surfaceLines, copyTargetPath);
            File.Copy(validFilePath, copyCharacteristicPointsTargetPath);

            try
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                var importer = new PipingSurfaceLinesCsvImporter();
                importer.ProgressChanged = (name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    if (name == string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_,
                                              copyCharacteristicPointsTargetPath))
                    {
                        File.Delete(copyCharacteristicPointsTargetPath);
                    }
                };

                var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
                observableSurfaceLinesList.Attach(observer);

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import(observableSurfaceLinesList, copyTargetPath);

                // Assert
                var internalErrorMessage = new FileReaderErrorMessageBuilder(copyCharacteristicPointsTargetPath).Build(UtilsResources.Error_File_does_not_exist);
                var expectedLogMessages = new[]
                {
                    string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_,
                                  copyCharacteristicPointsTargetPath),
                    string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                  internalErrorMessage)
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
                Assert.IsFalse(importResult);
                CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                         "No items should be added to collection when import is aborted.");
                mocks.VerifyAll(); // Expect no calls on 'observer'
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
        public void Import_CharacteristicPointsFileHasDuplicateIdentifier_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifiersCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLineFile);

            // Assert
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_, "Rotterdam1")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidCharacteristicPointsDefinitionDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithOneInvalidCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLineFile);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 2")
                .WithSubject("locatie 'Rotterdam1Invalid'")
                .Build(PipingIOResources.Error_CharacteristicPoint_has_not_double);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, 
                    corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                    internalErrorMessage),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_, 
                    "Rotterdam1Invalid")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 3);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(7, progressCallCount,
                            "Expect 1 call for each surfaceline (2 in total) +1 for 0/N progress and for each characteristic point location (2 in total) +1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithOneCharacteristicPointsLocation";
            string surfaceLines = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLines);

            // Assert
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, 
                    corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_, 
                    "Rotterdam1")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(6, progressCallCount,
                            "Expect 1 call for each surfaceline (2 in total) +1 for 0/N progress and for each characteristic point location (1 in total) +1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndThreeCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithThreeCharacteristicPointsLocations";
            string surfaceLines = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLines);

            // Assert
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, 
                    corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Characteristic_points_found_for_unknown_SurfaceLine_0_, 
                    "Extra")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(8, progressCallCount,
                            "Expect 1 call for each surfaceline (2 in total) +1 for 0/N progress and for " +
                            "each characteristic point location (2 in total) +1 for 0/N progress, " +
                            "one for the 'Extra' characteristic point definition " + 
                            "and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidBottomDitchDikeSide", "Slootbodem dijkzijde")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidBottomDitchPolderSide", "Slootbodem polderzijde")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidDikeToeAtPolder", "Teen dijk binnenwaarts")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidDikeToeAtRiver", "Teen dijk buitenwaarts")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidDitchDikeSide", "Insteek sloot dijkzijde")]
        [TestCase("TwoValidSurfaceLines_CharacteristicPointsInvalidDitchPolderSide", "Insteek sloot polderzijde")]
        public void Import_FileWithTwoValidLinesAndCharacteristicPointNotOnGeometry_LogInvalidPointDefinition(string fileName, string characteristicPointName)
        {
            // Setup
            string surfaceLines = Path.Combine(pluginTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, surfaceLines);

            // Assert
            var pointFormat = string.Format(PipingDataResources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point,0,1,2);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, 
                    corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CharacteristicPoint_0_of_SurfaceLine_1_skipped_cause_2_,
                    characteristicPointName,
                    "Rotterdam1Invalid",
                    pointFormat)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(7, progressCallCount,
                            "Expect 1 call for each surfaceline (2 in total) +1 for 0/N progress and for " +
                            "each characteristic point location (2 in total) +1 for 0/N progress, " +
                            "and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithCharacteristicPoints_ImportSurfaceLinesToCollection()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 2;
            const int expectedNumberOfCharacteristicPointsDefinitions = 2;

            const string fileName = "TwoValidSurfaceLines_WithCharacteristicPoints";
            string twovalidsurfacelinesCsv =string.Format(surfaceLineFormat, fileName);
            string twovalidsurfacelinesCharacteristicPointsCsv = string.Format(krpFormat, fileName);

            string validSurfaceLinesFilePath = Path.Combine(pluginTestDataPath, twovalidsurfacelinesCsv);
            string validCharacteristicPointsFilePath = Path.Combine(pluginTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            importer.ProgressChanged = delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual(string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_, twovalidsurfacelinesCsv), currentStepName);
                }
                else if (callCount <= expectedNumberOfSurfaceLines + expectedNumberOfCharacteristicPointsDefinitions + 1)
                {
                    Assert.AreEqual(string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_, twovalidsurfacelinesCharacteristicPointsCsv), currentStepName);
                }
                else if (callCount == expectedNumberOfSurfaceLines + expectedNumberOfCharacteristicPointsDefinitions + 2)
                {
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, currentStepName);
                }
                else
                {
                    Assert.Fail("Not expecting progress: \"{0}: {1} out of {2}\".", currentStepName, currentStep, totalNumberOfSteps);
                }

                Assert.AreEqual(expectedNumberOfSurfaceLines, totalNumberOfSteps);
                callCount++;
            };

            // Precondition
            CollectionAssert.IsEmpty(observableSurfaceLinesList);
            Assert.IsTrue(File.Exists(validSurfaceLinesFilePath));
            Assert.IsTrue(File.Exists(validCharacteristicPointsFilePath));

            // Call
            var importResult = importer.Import(observableSurfaceLinesList, validSurfaceLinesFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Count());
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            Assert.AreEqual(new Point3D 
            {
                X = 94263.0026213,
                Y = 427776.654093,
                Z = -1.02
            }, firstSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D
            {
                X = 94275.9126686,
                Y = 427811.080886,
                Z = -1.04
            }, firstSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D
            {
                X = 94284.0663827,
                Y = 427831.918156,
                Z = 1.25
            }, firstSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D
            {
                X = 94294.9380015,
                Y = 427858.191234,
                Z = 1.45
            }, firstSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D
            {
                X = 94284.0663827,
                Y = 427831.918156,
                Z = 1.25
            }, firstSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D
            {
                X = 94305.3566362,
                Y = 427889.900123,
                Z = 1.65
            }, firstSurfaceLine.DikeToeAtPolder);

            var secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Count());
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);
            Assert.AreEqual(new Point3D
            {
                X = 2.3,
                Y = 0,
                Z = 1.0
            }, secondSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D
            {
                X = 4.4,
                Y = 0,
                Z = 2.0
            }, secondSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D
            {
                X = 5.7,
                Y = 0,
                Z = 1.1
            }, secondSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D
            {
                X = 5.7,
                Y = 0,
                Z = 1.1
            }, secondSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D
            {
                X = 2.3,
                Y = 0,
                Z = 1.0
            }, secondSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D
            {
                X = 5.7,
                Y = 0,
                Z = 1.1
            }, secondSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(7, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validSurfaceLinesFilePath));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validCharacteristicPointsFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }
    }
}