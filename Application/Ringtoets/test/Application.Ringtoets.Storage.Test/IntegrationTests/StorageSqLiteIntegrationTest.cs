using System;
using System.IO;
using Core.Common.Base.Data;
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

        [TestFixtureTearDown]
        [TearDown]
        public void TearDownTempRingtoetsFile()
        {
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProjectLoadProject_CompleteProject_ProjectAsEntitiesInStorage()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectAsFile.rtd");
            Project project = new Project()
            {
                Name = "tempProjectFile",
                Description = "description"
            };
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                Name = "dikeAssessmentSection",
                FailureMechanismContribution =
                {
                    Norm = 8
                }
            };
            project.Items.Add(dikeAssessmentSection);

            StorageSqLite storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, project);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempRingtoetsFile));

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            try
            {
                Assert.DoesNotThrow(test);
            }
            finally
            {
                // TearDown
                TearDownTempRingtoetsFile(tempFile);
            }
        }

        [Test]
        public void SaveAs_LoadProject_ProjectAsEntitiesInNewStorage()
        {
            // Setup
            Project project = new Project()
            {
                Name = "tempProjectFile",
                Description = "description"
            };
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                Name = "dikeAssessmentSection",
                FailureMechanismContribution =
                {
                    Norm = 8
                }
            };
            project.Items.Add(dikeAssessmentSection);

            StorageSqLite storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, project);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempRingtoetsFile));

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, project);

            // Assert
            Assert.DoesNotThrow(test, String.Format("Precondition: failed to save project to file '{0}'.", tempRingtoetsFile));

            // Call
            Project loadedProject = storage.LoadProject(tempRingtoetsFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
            Assert.AreNotSame(project, loadedProject);
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