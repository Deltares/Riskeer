﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;
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
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de ondergrondschematisatie uit de D-Soil Model database.", i, expectedProfiles));
            }
            expectedProgressMessages.Add(new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de stochastische ondergrondmodellen.", i, expectedModels));
            }
            expectedProgressMessages.Add(new ProgressNotification("Controleren van ondergrondschematisaties.", 1, 1));
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
                .WithSubject("ondergrondschematisatie 'Profile'")
                .Build(string.Format(RingtoetsIOResources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_,
                                     "IntersectionX"));
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                              internalErrorMessage),
                string.Format("Het stochastisch ondergrondmodel '{0}' heeft een stochastisch profiel zonder grondprofiel, deze wordt overgeslagen.", "Name")
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
            var expectedLogMessages = "De som van de kansen van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessages, 1);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile1D_ImportSoilModelsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile1D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);

            // Call
            var importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, pipingFailureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel model1 = pipingFailureMechanism.StochasticSoilModels[0];
            StochasticSoilModel model2 = pipingFailureMechanism.StochasticSoilModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile1.SoilProfileType,
                            "Expected database to have 1D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile2.SoilProfileType,
                            "Expected database to have 1D profiles.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile2D_ImportSoilModelsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile2D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            var context = new StochasticSoilModelContext(pipingFailureMechanism, assessmentSection);

            // Call
            var importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, pipingFailureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel model1 = pipingFailureMechanism.StochasticSoilModels[0];
            StochasticSoilModel model2 = pipingFailureMechanism.StochasticSoilModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile1.SoilProfileType,
                            "Expected database to have 2D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile2.SoilProfileType,
                            "Expected database to have 2D profiles.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ModelWithOneInvalidStochasticSoilProfileDueToMissingProfile_SkipModelAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "EmptySoilModel.soil");

            var observer = mockRepository.StrictMock<IObserver>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            context.Attach(observer);

            var importer = new PipingSoilProfilesImporter();

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(context, validFilePath);

            // Assert
            var expectedLogMessage = @"Er zijn geen profielen gevonden in het stochastisch ondergrondmodel 'Model', deze wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);

            mockRepository.VerifyAll(); // Ensure there are no calls to UpdateObserver
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithoutLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_noLayerProperties.soil");

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

            // Call
            bool importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);

            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(100.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("AD647M30_Segment_36005_1D1", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            Assert.AreEqual(9, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                4.8899864439741778,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            int expectedNumberOfLayers = expectedLayerTops.Length;
            CollectionAssert.AreEqual(Enumerable.Repeat(false, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(Enumerable.Repeat<double?>(null, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.AbovePhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat<double?>(null, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat<double?>(null, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.DryUnitWeight));

            Assert.AreEqual(6, progress);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_withLayerProperties.soil");

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

            // Call
            bool importResult = importer.Import(context, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);

            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(100.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("Test 2d profile", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            const int expectedNumberOfLayers = 9;
            Assert.AreEqual(expectedNumberOfLayers, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                5.0571018353300463,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            var expectedIsAquiferValues = new[]
            {
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false
            };
            CollectionAssert.AreEqual(expectedIsAquiferValues,
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            var expectedAbovePhreaticLevelValues = new[]
            {
                1.1,
                2.2,
                3.3,
                4.4,
                7.7,
                9.9,
                11.11,
                13.13,
                14.14
            };
            CollectionAssert.AreEqual(expectedAbovePhreaticLevelValues,
                                      profile.Layers.Select(l => l.AbovePhreaticLevel));
            var expectedBelowPhreaticLevelValues = new[]
            {
                27.27,
                28.28,
                29.29,
                30.3,
                33.33,
                35.35,
                37.37,
                39.39,
                40.4
            };
            CollectionAssert.AreEqual(expectedBelowPhreaticLevelValues,
                                      profile.Layers.Select(l => l.BelowPhreaticLevel));
            CollectionAssert.AreEqual(Enumerable.Repeat<double?>(null, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.DryUnitWeight));

            Assert.AreEqual(6, progress);
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