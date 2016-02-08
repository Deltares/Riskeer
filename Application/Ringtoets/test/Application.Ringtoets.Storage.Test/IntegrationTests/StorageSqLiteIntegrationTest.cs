using System;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageSqLiteIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");
        private readonly string tempRingtoetsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles"), "tempProjectFile.rtd");
        private Project fullProject;

        [SetUp]
        public void Setup()
        {
            fullProject = new Project()
            {
                Name = "tempProjectFile",
                Description = "description",
                Items =
                {
                    new DikeAssessmentSection
                    {
                        Name = "dikeAssessmentSection"
                    },
                    new DuneAssessmentSection
                    {
                        Name = "duneAssessmentSection"
                    }
                }
            };
        }

        [TestFixtureTearDown]
        [TearDown]
        public void TearDownTempRingtoetsFile()
        {
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProjectAs_SaveAsNewFile_ProjectAsEntitiesInBothFiles()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectAsFile.rtd");

            StorageSqLite storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, fullProject);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempRingtoetsFile));

            // Call
            Project firstProject = null;
            Project secondProject = null;
            try
            {
                storage.SaveProjectAs(tempFile, fullProject);
                firstProject = storage.LoadProject(tempRingtoetsFile);
                secondProject = storage.LoadProject(tempFile);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
            finally
            {
                // TearDown
                TearDownTempRingtoetsFile(tempFile);
            }

            // Assert
            Assert.IsInstanceOf<Project>(firstProject);
            Assert.IsInstanceOf<Project>(secondProject);

            var firstProjectDike = firstProject.Items.OfType<DikeAssessmentSection>().ToList();
            var secondProjectDike = secondProject.Items.OfType<DikeAssessmentSection>().ToList();
            Assert.AreEqual(firstProjectDike.Count, secondProjectDike.Count);
            for (var i = 0; i < firstProjectDike.Count; i++)
            {
                Assert.AreEqual(firstProjectDike[i].StorageId, secondProjectDike[i].StorageId);
                Assert.AreEqual(firstProjectDike[i].Name, secondProjectDike[i].Name);
            }

            var firstProjectDune = firstProject.Items.OfType<DuneAssessmentSection>().ToList();
            var secondProjectDune = secondProject.Items.OfType<DuneAssessmentSection>().ToList();
            Assert.AreEqual(firstProjectDune.Count, secondProjectDune.Count);
            for (var i = 0; i < firstProjectDune.Count; i++)
            {
                Assert.AreEqual(firstProjectDune[i].StorageId, secondProjectDune[i].StorageId);
                Assert.AreEqual(firstProjectDune[i].Name, secondProjectDune[i].Name);
            }
        }

        [Test]
        public void SaveProjectAs_LoadProject_ProjectAsEntitiesInNewStorage()
        {
            // Setup
            StorageSqLite storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, fullProject);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempRingtoetsFile));

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, fullProject);

            // Assert
            Assert.DoesNotThrow(test, String.Format("Precondition: failed to save project to file '{0}'.", tempRingtoetsFile));

            // Call
            Project loadedProject = storage.LoadProject(tempRingtoetsFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
            Assert.AreNotSame(fullProject, loadedProject);
            Assert.AreEqual(fullProject.Items.Count, loadedProject.Items.Count);
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithValidFile_ProjectSet()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var projectStore = new StorageSqLite();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                // Call
                Action action = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend.",
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 13);
                Assert.AreEqual(testFile, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("ValidRingtoetsDatabase", gui.Project.Name);
                Assert.AreEqual("Test description", gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
            }
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithInvalidFile_EmptyProjectSet()
        {
            // Setup
            var testFile = "SomeFile";
            var projectStore = new StorageSqLite();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                // Call
                Action action = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    string.Format("Fout bij het lezen van bestand '{0}': ", testFile),
                    "Het is niet gelukt om het Ringtoetsproject te laden.",
                    "Nieuw project aanmaken..."
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 15);
                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
            }
        }

        [Test]
        [STAThread]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithEmptyFile_EmptyProjectSet(string testFile)
        {
            // Setup
            var projectStore = new StorageSqLite();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                // Call
                Action action = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Nieuw project aanmaken..."
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 12);
                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
            }
        }

        private void TearDownTempRingtoetsFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}