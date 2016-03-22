using System;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
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
        private readonly string pluginSurfaceLinesTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.Plugin, "SurfaceLines");

        private readonly string krpFormat = "{0}.krp.csv";
        private readonly string surfaceLineFormat = "{0}.csv";

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new PipingSurfaceLinesCsvImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase<RingtoetsPipingSurfaceLinesContext>>(importer);
            Assert.AreEqual(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            var expectedFileFilter = String.Format("{0} {1} (*.csv)|*.csv",
                                                   PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, ApplicationResources.Csv_file_name);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
        }

        [Test]
        public void CanImportOn_ValidContextWithReferenceLine_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var targetContext = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            var canImport = importer.CanImportOn(targetContext);

            // Assert
            Assert.IsTrue(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_ValidContextWithoutReferenceLine_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = null;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var targetContext = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            var canImport = importer.CanImportOn(targetContext);

            // Assert
            Assert.IsFalse(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_NoReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(context, validFilePath);

            // Assert
            var expectedMessage = "Er is geen referentielijn beschikbaar om profielmetingen voor te definiëren. Er zijn geen profielmetingen geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            mocks.VerifyAll();
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            bool progressStarted = false;
            bool progressCharacteristicPointsStarted = false;
            importer.ProgressChanged = delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (!progressStarted && callCount == 0)
                {
                    progressStarted = true;
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Reading_surface_line_file, currentStepName);
                    return;
                }
                if (!progressCharacteristicPointsStarted && callCount == expectedNumberOfSurfaceLines + 1)
                {
                    progressCharacteristicPointsStarted = true;
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Reading_characteristic_points_file, currentStepName);
                    return;
                }

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
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            AssertAreEqualPoint2D(new Point2D(94270.0, 427795.313769642), firstSurfaceLine.ReferenceLineIntersectionWorldPoint);

            var secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);
            AssertAreEqualPoint2D(new Point2D(3.3, 0), secondSurfaceLine.ReferenceLineIntersectionWorldPoint);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427850),
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();

            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            var mesages = new[]
            {
                "Profielmeting Rotterdam1 bevat aaneengesloten dubbele geometrie punten, welke zijn genegeerd.",
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.krp.csv"))
            };

            TestHelper.AssertLogMessagesAreGenerated(call, mesages, 2);
            Assert.IsTrue(importResult);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
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
            AssertAreEqualPoint2D(new Point2D(94270.0, 427795.313769642), firstSurfaceLine.ReferenceLineIntersectionWorldPoint);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(context, validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_PathIsInvalid_AbortImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(ioTestDataPath, "TwoValidSurfaceLines.csv");
            string corruptPath = validFilePath.Replace('S', Path.GetInvalidPathChars().First());

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(String.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                                                                          String.Join(", ", Path.GetInvalidFileNameChars())));
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(UtilsResources.Error_File_does_not_exist);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(UtilsResources.Error_File_empty);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithLocation("op regel 1")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
                var assessmentSection = mocks.Stub<AssessmentSectionBase>();
                assessmentSection.ReferenceLine = new ReferenceLine();
                var failureMechanism = new PipingFailureMechanism();
                mocks.ReplayAll();

                var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
                context.Attach(observer);

                var importer = new PipingSurfaceLinesCsvImporter();
                importer.ProgressChanged = (name, step, steps) =>
                {
                    // Delete the file being read by the import during the import itself:
                    File.Delete(copyTargetPath);
                };

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import(context, copyTargetPath);

                // Assert
                var internalErrorMessage = new FileReaderErrorMessageBuilder(copyTargetPath).Build(UtilsResources.Error_File_does_not_exist);
                var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                                       internalErrorMessage);
                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
                Assert.IsFalse(importResult);
                CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
        public void Import_FileHasDuplicateIdentifier_AbortImportAndLog()
        {
            // Setup
            var fileName = "TwoValidSurfaceLines_DuplicateIdentifier";
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string expectedCharacteristicPointsFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900)
            });
            assessmentSection.ReferenceLine = referenceLine;

            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var duplicateDefinitionMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_, "Rotterdam1");
            var expectedLogMessages = new[]
            {
                duplicateDefinitionMessage,
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, expectedCharacteristicPointsFile)
            };

            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the one valid surfaceline has been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(3, context.FailureMechanism.SurfaceLines.First(sl => sl.Name == "Rotterdam1").Points.Length, "First line should have been added to the model.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(ioTestDataPath, "TwoValidAndOneInvalidNumberRowSurfaceLines.csv");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427700),
                new Point2D(94270, 427900),
                new Point2D(9.8, -1),
                new Point2D(9.8, 1)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

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

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(7, progressCallCount,
                            "Expect 1 call for start reading surface lines file" +
                            ", 1 call for start reading characteristic points file" +
                            ", 1 call for each surfaceline (3 in total) +1 for 0/N progress, and 1 for putting data in model.");
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(context, path);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(path)
                .WithLocation("op regel 2")
                .WithSubject("profielmeting 'Rotterdam1'")
                .Build(PipingIOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                              internalErrorMessage),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                              Path.Combine(ioTestDataPath, "InvalidRow_DuplicatePointsCausingRecline.krp.csv"))
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(path));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(context, path);

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
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Precondition
            Assert.IsTrue(File.Exists(surfaceLineFile));
            Assert.IsFalse(File.Exists(nonExistingCharacteristicFile));

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLineFile);

            // Assert
            var expectedLogMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_,
                                                   nonExistingCharacteristicFile);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);
            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count);
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_CharacteristicPointsFileIsEmpty_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_EmptyCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLineFile);

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
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
                                     "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_CharacteristicPointsFileHasInvalidHeader_AbortImportAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_InvalidHeaderCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLineFile);

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
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
            string surfaceLines = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, source));
            string validFilePath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, source));
            File.Copy(surfaceLines, copyTargetPath);
            File.Copy(validFilePath, copyCharacteristicPointsTargetPath);

            try
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                var assessmentSection = mocks.Stub<AssessmentSectionBase>();
                assessmentSection.ReferenceLine = new ReferenceLine();
                var failureMechanism = new PipingFailureMechanism();
                mocks.ReplayAll();

                var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
                context.Attach(observer);

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

                var importResult = true;

                // Call
                Action call = () => importResult = importer.Import(context, copyTargetPath);

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
                CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines,
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
        public void Import_CharacteristicPointsFileHasDuplicateIdentifier_LogAndSkipsLine()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_DuplicateIdentifiersCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLineFile);

            // Assert
            var duplicateDefinitionMessage = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
                                                           "Rotterdam1");
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, corruptPath),
                duplicateDefinitionMessage
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(-1.02, context.FailureMechanism.SurfaceLines.First(sl => sl.Name == "Rotterdam1").DitchPolderSide.Z, "First characteristic points definition should be added to data model.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneInvalidCharacteristicPointsDefinitionDueToUnparsableNumber_SkipInvalidRowAndLog()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithOneInvalidCharacteristicPoints";
            string surfaceLineFile = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLineFile);

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

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(9, progressCallCount,
                            "Expect 1 call for start reading surface lines file" +
                            ", 1 call for start reading characteristic points file" +
                            ", 1 call for each surfaceline (2 in total) +1 for 0/N progress and for each characteristic point location (2 in total) " +
                            "+1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndOneCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithOneCharacteristicPointsLocation";
            string surfaceLines = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLines);

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

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(8, progressCallCount,
                            "Expect 1 call for start reading surface lines file" +
                            ", 1 call for start reading characteristic points file" +
                            ", 1 call for each surfaceline (2 in total) +1 for 0/N progress and for each characteristic point location (1 in total) " +
                            "+1 for 0/N progress, and 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_FileWithTwoValidLinesAndThreeCharacteristicPointsDefinition_LogMissingDefinition()
        {
            // Setup
            const string fileName = "TwoValidSurfaceLines_WithThreeCharacteristicPointsLocations";
            string surfaceLines = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLines);

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

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1"));
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(10, progressCallCount,
                            "Expect 1 call for start reading surface lines file" +
                            ", 1 call for start reading characteristic points file" +
                            ", 1 call for each surfaceline (2 in total) +1 for 0/N progress and for " +
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
            string surfaceLines = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(surfaceLineFormat, fileName));
            string corruptPath = Path.Combine(pluginSurfaceLinesTestDataPath, string.Format(krpFormat, fileName));

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int progressCallCount = 0;
            importer.ProgressChanged = (name, step, steps) => { progressCallCount++; };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, surfaceLines);

            // Assert
            var pointFormat = string.Format(PipingDataResources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                            new Point3D(0, 1, 2),
                                            characteristicPointName);
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_,
                              corruptPath),
                string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                              "Rotterdam1Invalid",
                              pointFormat)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, context.FailureMechanism.SurfaceLines.Count,
                            "Ensure only the two valid surfacelines have been imported.");
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "Rotterdam1Invalid"));
            Assert.AreEqual(1, context.FailureMechanism.SurfaceLines.Count(sl => sl.Name == "ArtifcialLocal"));

            Assert.AreEqual(9, progressCallCount,
                            "Expect 1 call for start reading surface lines file" +
                            ", 1 call for start reading characteristic points file" +
                            ", 1 call for each surfaceline (2 in total) +1 for 0/N progress and for " +
                            "each characteristic point location (2 in total) +1 for 0/N progress, " +
                            ", 1 for putting data in model.");
            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
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

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            bool progressStarted = false;
            bool progressCharacteristicPointsStarted = false;
            importer.ProgressChanged = delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (!progressStarted && callCount == 0)
                {
                    progressStarted = true;
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Reading_surface_line_file, currentStepName);
                    return;
                }
                if (!progressCharacteristicPointsStarted && callCount == expectedNumberOfSurfaceLines + 1)
                {
                    progressCharacteristicPointsStarted = true;
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_Reading_characteristic_points_file, currentStepName);
                    return;
                }

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
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validSurfaceLinesFilePath));
            Assert.IsTrue(File.Exists(validCharacteristicPointsFilePath));

            // Call
            var importResult = importer.Import(context, validSurfaceLinesFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);
            Assert.AreEqual(new Point3D(94263.0026213, 427776.654093, -1.02), firstSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D(94275.9126686, 427811.080886, -1.04), firstSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(94294.9380015, 427858.191234, 1.45), firstSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D(94284.0663827, 427831.918156, 1.25), firstSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(94305.3566362, 427889.900123, 1.65), firstSurfaceLine.DikeToeAtPolder);

            var secondSurfaceLine = importTargetArray[1];
            Assert.AreEqual("ArtifcialLocal", secondSurfaceLine.Name);
            Assert.AreEqual(3, secondSurfaceLine.Points.Length);
            Assert.AreEqual(5.7, secondSurfaceLine.EndingWorldPoint.X);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondSurfaceLine.DitchPolderSide);
            Assert.AreEqual(new Point3D(4.4, 0, 2.0), secondSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.DitchDikeSide);
            Assert.AreEqual(new Point3D(2.3, 0, 1.0), secondSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(new Point3D(5.7, 0, 1.1), secondSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(7, callCount);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validSurfaceLinesFilePath));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validCharacteristicPointsFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_DoesNotInterSectReferenceLine_SkipRowAndLog()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 1;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var mesagge = "Profielmeting ArtifcialLocal doorkruist de huidige referentielijn niet of op meer dan 1 punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielmeting een lokaal coordinaat systeem heeft.";

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, mesagge);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
            Assert.IsTrue(importResult);
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("Rotterdam1", firstSurfaceLine.Name);
            Assert.AreEqual(8, firstSurfaceLine.Points.Length);
            Assert.AreEqual(427776.654093, firstSurfaceLine.StartingWorldPoint.Y);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_DoesInterSectReferenceLineMultipleTimes_SkipRowAndLog()
        {
            // Setup
            const int expectedNumberOfSurfaceLines = 1;
            var twovalidsurfacelinesCsv = "TwoValidSurfaceLines.csv";
            string validFilePath = Path.Combine(ioTestDataPath, twovalidsurfacelinesCsv);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
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
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var mesagge = "Profielmeting Rotterdam1 doorkruist de huidige referentielijn niet of op meer dan 1 punt en kan niet worden geïmporteerd.";

            var context = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.SurfaceLines);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, mesagge);
            var importTargetArray = context.FailureMechanism.SurfaceLines.ToArray();
            Assert.IsTrue(importResult);
            Assert.AreEqual(expectedNumberOfSurfaceLines, importTargetArray.Length);

            // Sample some of the imported data:
            var firstSurfaceLine = importTargetArray[0];
            Assert.AreEqual("ArtifcialLocal", firstSurfaceLine.Name);
            Assert.AreEqual(3, firstSurfaceLine.Points.Length);
            Assert.AreEqual(0.0, firstSurfaceLine.StartingWorldPoint.Y);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(validFilePath));

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        private static void AssertAreEqualPoint2D(Point2D expectedPoint, Point2D actualPoint)
        {
            Assert.IsTrue(Math2D.AreEqualPoints(expectedPoint, actualPoint),
                          String.Format("Expected point: {0}" + Environment.NewLine + "Actual point: {1}",
                                        expectedPoint, actualPoint));
        }
    }
}