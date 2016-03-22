using System;
using System.IO;
using System.Linq;
using Core.Common.Base;
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
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;
using RingtoetsIOResources = Ringtoets.Piping.IO.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSoilProfilesImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
        private int progress;

        [SetUp]
        public void SetUp()
        {
            progress = 0;
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Prepare
            var expectedFileFilter = String.Format("{0} {1} (*.soil)|*.soil",
                                                   PipingFormsResources.PipingSoilProfilesCollection_DisplayName, ApplicationResources.Soil_file_name);

            // Call
            var importer = new PipingSoilProfilesImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase<StochasticSoilModelContext>>(importer);
            Assert.AreEqual(PipingFormsResources.PipingSoilProfilesCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
        }

        [Test]
        public void Import_FromNonExistingFile_LogError()
        {
            // Setup
            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_FromInvalidFileName_LogError()
        {
            // Setup
            var file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, invalidFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_NoReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var file = "file";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var expectedMessage = "Er is geen referentielijn beschikbaar. Geen data ingelezen.";
            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, invalidFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.AreEqual(0, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilProfilesToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);

            // Call
            var importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(34, progress);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileTwice_AddsSoilProfilesToCollectionLogWarning()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);
            var importResult = false;

            // Precondition
            importer.Import(context, validFilePath);
            var names = context.FailureMechanism.StochasticSoilModels.Select(ssm => ssm.Name);
            var expectedLogMessages = names.Select(name => string.Format("Het stochastisch ondergrondmodel '{0}' bestaat al in het faalmechanisme.", name)).ToList();
            expectedLogMessages.Add(String.Format("Het uitgelezen profiel '{0}' niet wordt niet gebruikt in een van de stochastische ondergrondmodellen.", "Segment_36005_1D2"));
            expectedLogMessages.Add(String.Format("Het uitgelezen profiel '{0}' niet wordt niet gebruikt in een van de stochastische ondergrondmodellen.", "Segment_36005_1D3"));

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, expectedLogMessages.Count);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(34*2, progress);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSoilProfilesImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(context, validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(35, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(RingtoetsIOResources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels,
                                     "No items should be added to collection when import is aborted.");
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithSubject("ondergrondschematisering 'Profile'")
                .Build(string.Format(RingtoetsIOResources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_,
                                     "IntersectionX"));
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                              internalErrorMessage),
                string.Format("Er zijn geen profielen gevonden in het stochastich ondersgrondmodel '{0}', deze wordt overgeslagen.", "Name")
            };

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);

            Assert.IsTrue(importResult);
            Assert.AreEqual(0, context.FailureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(6, progress);

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Call
            var importResult = importer.Import(context, corruptPath);

            Assert.IsTrue(importResult);
            Assert.AreEqual(0, context.FailureMechanism.StochasticSoilModels.Count);

            mocks.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }
    }
}