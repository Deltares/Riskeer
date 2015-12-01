using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtils;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
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
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(PipingFormsResources.PipingSoilProfilesCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            CollectionAssert.AreEqual(new[] { typeof(ICollection<PipingSoilProfile>) }, importer.SupportedItemTypes);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsFalse(importer.ShouldCancel);
            Assert.IsNull(importer.ProgressChanged);
        }

        [Test]
        public void CanImportOn_TargetIsCollectionOfPipingSoilProfile_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var validTarget = mocks.StrictMock<ObservableList<PipingSoilProfile>>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

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
            var invalidTarget = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            // Call
            var importAllowed = importer.CanImportOn(invalidTarget);

            // Assert
            Assert.IsFalse(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void ImportItem_FromNonExistingFile_LogError()
        {
            // Setup
            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableList = new ObservableList<PipingSoilProfile>();
            observableList.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);
            Assert.IsTrue(importer.CanImportOn(observableList));

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(validFilePath, observableList);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.AreSame(observableList, importedItem);
            CollectionAssert.IsEmpty(observableList);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void ImportItem_FromInvalidFileName_LogError()
        {
            // Setup
            var file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableList = new ObservableList<PipingSoilProfile>();
            observableList.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);
            Assert.IsTrue(importer.CanImportOn(observableList));

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(invalidFilePath, observableList);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.AreSame(observableList, importedItem);
            CollectionAssert.IsEmpty(observableList);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFile_ImportSoilProfilesToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            var piping = new PipingFailureMechanism();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var importTarget = piping.SoilProfiles;

            // Precondition
            Assert.IsTrue(importer.CanImportOn(importTarget));

            // Call
            var importedItem = importer.ImportItem(validFilePath, importTarget);

            // Assert
            Assert.AreSame(importTarget, importedItem);
            Assert.AreEqual(28, progress);
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFileWhileShouldCancelTrue_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableList = new ObservableList<PipingSoilProfile>();
            observableList.Attach(observer);

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);
            Assert.IsTrue(importer.CanImportOn(observableList));
            Assert.IsTrue(File.Exists(validFilePath));

            importer.ShouldCancel = true;

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(validFilePath, observableList);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSoilProfilesImporter_ImportItem_Import_cancelled, 1);
            Assert.AreSame(observableList, importedItem);
            CollectionAssert.IsEmpty(observableList);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var observableSoilProfileList = new ObservableList<PipingSoilProfile>();
            observableSoilProfileList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSoilProfileList);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(RingtoetsIOResources.Error_SoilProfile_read_from_database);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSoilProfileList, importedItem);
            CollectionAssert.IsEmpty(observableSoilProfileList,
                "No items should be added to collection when import is aborted.");
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var observableSoilProfileList = new ObservableList<PipingSoilProfile>();
            observableSoilProfileList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSoilProfileList);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithSubject("ondergrondschematisering 'Profile'")
                .Build(string.Format(RingtoetsIOResources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_,
                                     "IntersectionX"));
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);

            Assert.AreSame(observableSoilProfileList, importedItem);
            Assert.AreEqual(1, observableSoilProfileList.Count);
            Assert.AreEqual(4, progress);

            mocks.VerifyAll();
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter
            {
                ProgressChanged = IncrementProgress
            };

            var observableSoilProfileList = new ObservableList<PipingSoilProfile>();
            observableSoilProfileList.Attach(observer);

            // Call
            var importedItem = importer.ImportItem(corruptPath, observableSoilProfileList);

            Assert.AreSame(observableSoilProfileList, importedItem);
            Assert.AreEqual(0, observableSoilProfileList.Count);

            mocks.VerifyAll();
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }
    }
}