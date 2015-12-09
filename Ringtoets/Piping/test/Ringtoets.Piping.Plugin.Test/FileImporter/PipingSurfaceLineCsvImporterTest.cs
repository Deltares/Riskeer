using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Test.TestHelpers;
using Ringtoets.Piping.Plugin.FileImporter;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using PipingIOResources = Ringtoets.Piping.IO.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLineCsvImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines");

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
            Assert.IsNull(importer.ProgressChanged);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 2;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(testDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            importer.ProgressChanged += delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual(String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_, twovalidsurfacelinesCsv), currentStepName);
                }
                else if (callCount == expectedNumberOfSurfaceLines+1)
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

            Assert.IsTrue(FileHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
        {
            // Setup
            var twovalidsurfacelinesCsv = "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv";
            string validFilePath = Path.Combine(testDataPath, twovalidsurfacelinesCsv);

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

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(observableSurfaceLinesList, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Profielmeting Rotterdam1 bevat aaneengesloten dubbele geometrie punten, welke zijn genegeerd.", 1);
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

            Assert.IsTrue(FileHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

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
            string validFilePath = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

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
            string validFilePath = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");
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
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(String.Format(PipingIOResources.Error_Path_cannot_contain_Characters_0_,
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
            string corruptPath = Path.Combine(testDataPath, "I_dont_exists.csv");

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
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(PipingIOResources.Error_File_does_not_exist);
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
            string corruptPath = Path.Combine(testDataPath, "empty.csv");

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
                .Build(PipingIOResources.Error_File_empty);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
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
            string corruptPath = Path.Combine(testDataPath, "InvalidHeader_LacksY1.csv");

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
            string validFilePath = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");
            File.Copy(validFilePath, copyTargetPath);

            try
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                var importer = new PipingSurfaceLinesCsvImporter();
                importer.ProgressChanged += (name, step, steps) =>
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
                var internalErrorMessage = new FileReaderErrorMessageBuilder(copyTargetPath).Build(PipingIOResources.Error_File_does_not_exist);
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
            string corruptPath = Path.Combine(testDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged += (name, step, steps) =>
            {
                progressCallCount++;
            };

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
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(5, progressCallCount,
                "Expect 1 call for each surfaceline (3 in total) +1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToValidTargetWithInValidFileWithDuplicatePointsCausingRecline_SkipInvalidRowAndLog()
        {
            // Setup
            var twovalidsurfacelinesCsv = "InvalidRow_DuplicatePointsCausingRecline.csv";
            string path = Path.Combine(testDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
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
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(FileHelper.CanOpenFileForWrite(path));

            mocks.VerifyAll();
        }
    }
}