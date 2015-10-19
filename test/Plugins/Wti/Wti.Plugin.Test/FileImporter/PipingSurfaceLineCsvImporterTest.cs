using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DelftTools.Shell.Core;
using DelftTools.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Wti.Data;
using Wti.Plugin.FileImporter;
using WtiFormsResources = Wti.Forms.Properties.Resources;
using ApplicationResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLineCsvImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSurfaceLinesCsvReader");

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new PipingSurfaceLinesCsvImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(WtiFormsResources.PipingSurfaceLinesCollectionName, importer.Name);
            Assert.AreEqual(ApplicationResources.WtiApplicationName, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            CollectionAssert.AreEqual(new[]{typeof(IEnumerable<PipingSurfaceLine>)}, importer.SupportedItemTypes);
            Assert.IsFalse(importer.CanImportOnRootLevel);
            var expectedFileFilter = String.Format("{0} {1} (*.csv)|*.csv", 
                WtiFormsResources.PipingSurfaceLinesCollectionName, ApplicationResources.CsvFileName);
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
            var validTarget = mocks.StrictMock<ICollection<PipingSurfaceLine>>();
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
            var invalidTarget = mocks.StrictMock<IEnumerable<PipingSurfaceLine>>();
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

            var observableSurfaceLinesList = new ObservableList<PipingSurfaceLine>();
            observableSurfaceLinesList.Attach(observer);

            var importer = new PipingSurfaceLinesCsvImporter();
            int callCount = 0;
            importer.ProgressChanged += delegate(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                if (callCount <= expectedNumberOfSurfaceLines)
                {
                    Assert.AreEqual(String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_0_, twovalidsurfacelinesCsv), currentStepName);
                }
                else if (callCount == expectedNumberOfSurfaceLines+1)
                {
                    Assert.AreEqual(ApplicationResources.PipingSurfaceLinesCsvImporter_AddingImportedDataToModel, currentStepName);
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
            Assert.AreEqual(4.4, secondSurfaceLine.EndingWorldPoint.X);

            Assert.AreEqual(4, callCount);

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

            var observableSurfaceLinesList = new ObservableList<PipingSurfaceLine>();
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
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSurfaceLinesCsvImporter_ImportItem_ImportCancelled, 1);
            Assert.AreSame(observableSurfaceLinesList, importedItem);
            CollectionAssert.IsEmpty(observableSurfaceLinesList);

            mocks.VerifyAll(); // 'observer' should not be notified
        }
    }
}