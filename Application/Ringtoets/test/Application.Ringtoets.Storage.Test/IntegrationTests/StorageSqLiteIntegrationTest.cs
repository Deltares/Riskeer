// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
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
        public void SaveProjectAs_SaveAsNewFile_ProjectAsEntitiesInBothFiles()
        {
            // Setup
            Project fullProject = RingtoetsProjectHelper.GetFullTestProject();
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

                AssertHydraulicBoundaryDatabase(firstProjectDike[i], secondProjectDike[i]);
                AssertReferenceLine(firstProjectDike[i], secondProjectDike[i]);
            }

            var firstProjectDune = firstProject.Items.OfType<DuneAssessmentSection>().ToList();
            var secondProjectDune = secondProject.Items.OfType<DuneAssessmentSection>().ToList();
            Assert.AreEqual(firstProjectDune.Count, secondProjectDune.Count);
            for (var i = 0; i < firstProjectDune.Count; i++)
            {
                Assert.AreEqual(firstProjectDune[i].StorageId, secondProjectDune[i].StorageId);
                Assert.AreEqual(firstProjectDune[i].Name, secondProjectDune[i].Name);

                AssertHydraulicBoundaryDatabase(firstProjectDune[i], secondProjectDune[i]);
            }
        }

        [Test]
        public void SaveProjectAs_LoadProject_ProjectAsEntitiesInNewStorage()
        {
            // Setup
            StorageSqLite storage = new StorageSqLite();
            Project fullProject = RingtoetsProjectHelper.GetFullTestProject();
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

            var assessmentSection = loadedProject.Items.OfType<DikeAssessmentSection>().FirstOrDefault();
            Assert.IsNotNull(assessmentSection);
            AssertHydraulicBoundaryDatabase(fullProject.Items.OfType<DikeAssessmentSection>().FirstOrDefault(), assessmentSection);
            AssertReferenceLine(fullProject.Items.OfType<DikeAssessmentSection>().FirstOrDefault(), assessmentSection);
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithValidFile_ProjectSet()
        {
            // Setup
            var projectStore = new StorageSqLite();
            Project fullProject = RingtoetsProjectHelper.GetFullTestProject();
            var expectedProjectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);
            var expectedProjectDescritpion = fullProject.Description;

            // Precondition
            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, fullProject);

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                // Call
                Action action = () => gui.Run(tempRingtoetsFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend.",
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 11);
                Assert.AreEqual(tempRingtoetsFile, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(expectedProjectDescritpion, gui.Project.Description);

                var assessmentSection = gui.Project.Items.OfType<DikeAssessmentSection>().FirstOrDefault();
                Assert.IsNotNull(assessmentSection);
                AssertHydraulicBoundaryDatabase(fullProject.Items.OfType<DikeAssessmentSection>().FirstOrDefault(), assessmentSection);
                AssertReferenceLine(fullProject.Items.OfType<DikeAssessmentSection>().FirstOrDefault(), assessmentSection);
            }

            // TearDown
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithInvalidFile_EmptyProjectSet()
        {
            // Setup
            var testFile = "SomeFile";
            var projectStore = new StorageSqLite();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 13);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                // Call
                Action action = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Nieuw project aanmaken..."
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 10);
                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
            }
        }

        private static void AssertHydraulicBoundaryDatabase(AssessmentSectionBase expectedProject, AssessmentSectionBase project)
        {
            Assert.IsNotNull(expectedProject.HydraulicBoundaryDatabase);
            Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Version, project.HydraulicBoundaryDatabase.Version);
            Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.FilePath, project.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations.Count, project.HydraulicBoundaryDatabase.Locations.Count);

            for (int i = 0; i < expectedProject.HydraulicBoundaryDatabase.Locations.Count; i++)
            {
                Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations[i].Id, project.HydraulicBoundaryDatabase.Locations[i].Id);
                Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations[i].Name, project.HydraulicBoundaryDatabase.Locations[i].Name);
                Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations[i].DesignWaterLevel, project.HydraulicBoundaryDatabase.Locations[i].DesignWaterLevel);
                Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations[i].StorageId, project.HydraulicBoundaryDatabase.Locations[i].StorageId);
                Assert.AreEqual(expectedProject.HydraulicBoundaryDatabase.Locations[i].Location, project.HydraulicBoundaryDatabase.Locations[i].Location);
            }
        }


        private static void AssertReferenceLine(AssessmentSectionBase expectedProject, AssessmentSectionBase project)
        {
            Assert.IsNotNull(expectedProject.ReferenceLine);

            for (int i = 0; i < expectedProject.ReferenceLine.Points.Count(); i++)
            {
                var expectedPoint = expectedProject.ReferenceLine.Points.ElementAt(i);
                var resultingPoint = project.ReferenceLine.Points.ElementAt(i);
                Assert.AreEqual(expectedPoint.X, resultingPoint.X);
                Assert.AreEqual(expectedPoint.Y, resultingPoint.Y);
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