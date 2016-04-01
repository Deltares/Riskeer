using System;
using System.Collections.Generic;
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
        private MockRepository mockRepository;
        private int progress;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            progress = 0;
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Prepare
            var expectedFileFilter = String.Format("{0} {1} (*.soil)|*.soil",
                                                   PipingFormsResources.StochasticSoilProfileCollection_DisplayName, ApplicationResources.Soil_file_name);

            // Call
            var importer = new PipingSoilProfilesImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase<StochasticSoilModelContext>>(importer);
            Assert.AreEqual(PipingFormsResources.StochasticSoilProfileCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
        }

        [Test]
        public void CanImportOn_ValidContextWithReferenceLine_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var targetContext = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            var importer = new PipingSoilProfilesImporter();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var targetContext = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            var importer = new PipingSoilProfilesImporter();

            // Call
            var canImport = importer.CanImportOn(targetContext);

            // Assert
            Assert.IsFalse(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FromNonExistingFile_LogError()
        {
            // Setup
            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_FromInvalidFileName_LogError()
        {
            // Setup
            var file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_NoReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var file = "file";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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
            var expectedMessage = "Er is geen referentielijn beschikbaar. Geen data ingelezen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(context.FailureMechanism.StochasticSoilModels);
            Assert.AreEqual(0, progress);

            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);

            // Call
            var importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            var expectedProfiles = 24;
            var expectedModels = 3;

            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1)
            };
            for (var i = 1; i <= expectedProfiles; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de ondergrondschematisering uit de D-Soil Model database.", i, expectedProfiles));
            }
            expectedProgressMessages.Add(new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de stochastische ondergrondmodellen.", i, expectedModels));
            }
            expectedProgressMessages.Add(new ProgressNotification("Controleren van ondergrondprofielen.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Geïmporteerde data toevoegen aan faalmechanisme.", i, expectedModels));
            }
            Assert.AreEqual(expectedProgressMessages.Count, progressChangeNotifications.Count);
            for (var i = 0; i < expectedProgressMessages.Count; i++)
            {
                var notification = expectedProgressMessages[i];
                var actualNotification = progressChangeNotifications[i];
                Assert.AreEqual(notification.Text, actualNotification.Text);
                Assert.AreEqual(notification.CurrentStep, actualNotification.CurrentStep);
                Assert.AreEqual(notification.TotalSteps, actualNotification.TotalSteps);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFileTwice_AddsSoilModelToCollectionLogWarning()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);
            context.Attach(observer);
            var importResult = false;

            // Precondition
            importer.Import(context, validFilePath);
            var alreadyImportedSoilModelNames = context.FailureMechanism.StochasticSoilModels.Select(ssm => ssm.Name);

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            var expectedLogMessages = alreadyImportedSoilModelNames.Select(name => string.Format("Het stochastisch ondergrondmodel '{0}' bestaat al in het faalmechanisme.", name)).ToArray();
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, expectedLogMessages.Length);

            Assert.IsTrue(importResult);
            Assert.AreEqual(33*2, progress);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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
            Assert.AreEqual(34, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, corruptPath);

            // Assert
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
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);

            Assert.IsTrue(importResult);
            Assert.AreEqual(0, context.FailureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(7, progress);

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_IncorrectProfiles_SkipModelAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "invalidStochasticSoilProfiles.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            var expectedLogMessage = String.Format("Fout bij het lezen van bestand '{0}': Het stochastisch ondergrondprofiel bevat geen geldige waarde." +
                                                   " Dit stochastisch ondergrondmodel wordt overgeslagen.", validFilePath);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_IncorrectProbability_LogAndImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "incorrectProbability.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            var expectedLogMessages = "De som van de kans van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessages, 1);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }

        private class ProgressNotification
        {
            public ProgressNotification(string description, int currentStep, int totalSteps)
            {
                Text = description;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
            }

            public string Text { get; private set; }
            public int CurrentStep { get; private set; }
            public int TotalSteps { get; private set; }
        }
    }
}