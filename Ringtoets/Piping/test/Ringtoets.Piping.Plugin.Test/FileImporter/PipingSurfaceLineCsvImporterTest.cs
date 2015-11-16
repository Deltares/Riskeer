using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
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
            CollectionAssert.AreEqual(new[] { typeof(ICollection<RingtoetsPipingSurfaceLine>) }, importer.SupportedItemTypes);
            Assert.IsFalse(importer.CanImportOnRootLevel);
            var expectedFileFilter = String.Format("{0} {1} (*.csv)|*.csv", 
                PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, ApplicationResources.Csv_file_name);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsNull(importer.TargetDataDirectory);
            Assert.IsFalse(importer.ShouldCancel);
            Assert.IsNull(importer.ProgressChanged);
            Assert.IsFalse(importer.OpenViewAfterImport);
        }

        [Test]
        public void CanImportOn_TargetIsCollectionOfPipingSurfaceLines_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var validTarget = mocks.StrictMock<ICollection<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            var importAllowed = importer.CanImportOn(validTarget);

            // Assert
            Assert.IsTrue(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanImportOn_InvalidTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var invalidTarget = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            var importAllowed = importer.CanImportOn(invalidTarget);

            // Assert
            Assert.IsFalse(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
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
            Assert.IsTrue(importer.CanImportOn(observableSurfaceLinesList));
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importedItem = importer.ImportItem(validFilePath, observableSurfaceLinesList);

            // Assert
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Count());
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);

            var secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtificalLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Count());
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);

            Assert.AreEqual(4, callCount);

            Assert.IsTrue(FileHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll();
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFileWithConsecutiveDuplicatePoints_ImportSurfaceLineWithDuplicatesRemovedToCollection()
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
            Assert.IsTrue(importer.CanImportOn(observableSurfaceLinesList));
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            object importedItem = null;
            Action call = () => importedItem = importer.ImportItem(validFilePath, observableSurfaceLinesList);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Dwarsdoorsnede Rotterdam1 bevat aaneengesloten dubbele geometrie punten, welke zijn genegeerd.", 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
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
        public void ImportItem_ImportingToValidTargetWithValidFileWhileShouldCancelTrue_CancelImportAndLog()
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
            Assert.IsTrue(importer.CanImportOn(observableSurfaceLinesList));
            Assert.IsTrue(File.Exists(validFilePath));

            importer.ShouldCancel = true;

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(validFilePath, observableSurfaceLinesList);
            
            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_ImportItem_Import_cancelled, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void ImportItem_PathIsInvalid_AbortImportAndLog()
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

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.Error_Path_cannot_contain_characters_0_,
                                                     String.Join(", ", Path.GetInvalidFileNameChars()));
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_FileDoesNotExist_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "I_dont_exists.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.Error_File_0_does_not_exist,
                                                     corruptPath);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_FileIsEmpty_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.Error_File_0_empty,
                                                     corruptPath);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_InvalidHeader_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "InvalidHeader_LacksY1.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            var observableSurfaceLinesList = new ObservableList<RingtoetsPipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.PipingSurfaceLinesCsvReader_File_0_invalid_header,
                                                     corruptPath);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList,
                "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_FileDeletedDuringRead_AbortImportAndLog()
        {
            // Setup
            var copyTargetPath = "ImportItem_FileDeletedDuringRead_AbortImportAndLog.csv";
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

                object importedItem = null;

                // Call
                Action call = () => importedItem = importer.ImportItem(copyTargetPath, observableSurfaceLinesList);

                // Assert
                var internalErrorMessage = String.Format(PipingIOResources.Error_File_0_does_not_exist,
                                                         copyTargetPath);
                var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                                       copyTargetPath, internalErrorMessage);
                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
                Assert.AreSame(observableSurfaceLinesList, importedItem);
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
        public void ImportItem_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
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

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.Error_File_0_has_not_double_SurfaceLineName_1_,
                                                     corruptPath, "InvalidRow");
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_Parse_error_File_0_SurfaceLinesNumber_1_Message_2_,
                                                   corruptPath, 2, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);

            Assert.AreEqual(2, observableSurfaceLinesList.Count,
                "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, observableSurfaceLinesList.Count(sl => sl.Name == "ArtificalLocal"));

            Assert.AreEqual(5, progressCallCount,
                "Expect 1 call for each surfaceline (3 in total) +1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll();
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithInValidFileWithDuplicatePointsCausingRecline_SkipInvalidRowAndLog()
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
            Assert.IsTrue(importer.CanImportOn(observableSurfaceLinesList));
            Assert.IsTrue(File.Exists(path));

            // Call
            object importedItem = null;
            Action call = () => importedItem = importer.ImportItem(path, observableSurfaceLinesList);

            // Assert
            var internalErrorMessage = String.Format(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_File_0_SurfaceLineName_1_has_reclining_geometry,
                                                     path, "Rotterdam1");
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_Parse_error_File_0_SurfaceLinesNumber_1_Message_2_,
                                                   path, 1, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            var importTargetArray = observableSurfaceLinesList.ToArray();
            Assert.AreEqual(0, importTargetArray.Length);

            Assert.IsTrue(FileHelper.CanOpenFileForWrite(path));

            mocks.VerifyAll();
        }
    }
}