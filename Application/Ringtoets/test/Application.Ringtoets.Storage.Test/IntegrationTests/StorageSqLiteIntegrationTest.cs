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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageSqLiteIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");
        private readonly string tempRingtoetsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles"), "tempProjectFile.rtd");

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

            var firstProjectAssessmentSection = firstProject.Items.OfType<AssessmentSection>().ToList();
            var secondProjectAssessmentSection = secondProject.Items.OfType<AssessmentSection>().ToList();
            Assert.AreEqual(firstProjectAssessmentSection.Count, secondProjectAssessmentSection.Count);
            for (var i = 0; i < firstProjectAssessmentSection.Count; i++)
            {
                Assert.AreEqual(firstProjectAssessmentSection[i].StorageId, secondProjectAssessmentSection[i].StorageId);
                Assert.AreEqual(firstProjectAssessmentSection[i].Name, secondProjectAssessmentSection[i].Name);

                AssertHydraulicBoundaryDatabase(firstProjectAssessmentSection[i], secondProjectAssessmentSection[i]);
                AssertReferenceLine(firstProjectAssessmentSection[i], secondProjectAssessmentSection[i]);

                AssertPipingFailureMechanism(firstProjectAssessmentSection[i].PipingFailureMechanism, secondProjectAssessmentSection[i].PipingFailureMechanism);
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

            var actualAssessmentSection = loadedProject.Items.OfType<AssessmentSection>().FirstOrDefault();
            Assert.IsNotNull(actualAssessmentSection);
            AssessmentSection expectedAssessmentSection = fullProject.Items.OfType<AssessmentSection>().FirstOrDefault();
            AssertHydraulicBoundaryDatabase(expectedAssessmentSection, actualAssessmentSection);
            AssertReferenceLine(expectedAssessmentSection, actualAssessmentSection);
            AssertPipingFailureMechanism(expectedAssessmentSection.PipingFailureMechanism, actualAssessmentSection.PipingFailureMechanism);
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

                var actualAssessmentSection = gui.Project.Items.OfType<AssessmentSection>().FirstOrDefault();
                Assert.IsNotNull(actualAssessmentSection);
                AssessmentSection expectedAssessmentSection = fullProject.Items.OfType<AssessmentSection>().FirstOrDefault();
                AssertHydraulicBoundaryDatabase(expectedAssessmentSection, actualAssessmentSection);
                AssertReferenceLine(expectedAssessmentSection, actualAssessmentSection);
                AssertPipingFailureMechanism(expectedAssessmentSection.PipingFailureMechanism, actualAssessmentSection.PipingFailureMechanism);
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

        private static void AssertHydraulicBoundaryDatabase(IAssessmentSection expectedProject, IAssessmentSection project)
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


        private static void AssertReferenceLine(IAssessmentSection expectedProject, IAssessmentSection project)
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

        private void AssertPipingFailureMechanism(PipingFailureMechanism expectedPipingFailureMechanism, PipingFailureMechanism actualPipingFailureMechanism)
        {
            AssertStochasticSoilModels(expectedPipingFailureMechanism.StochasticSoilModels, actualPipingFailureMechanism.StochasticSoilModels);
            AssertSurfaceLines(expectedPipingFailureMechanism.SurfaceLines, actualPipingFailureMechanism.SurfaceLines);
        }

        private void AssertStochasticSoilModels(ObservableList<StochasticSoilModel> expectedModels, ObservableList<StochasticSoilModel> actualModels)
        {
            // Precondition:
            Assert.Less(0, actualModels.Count);

            Assert.AreEqual(expectedModels.Count, actualModels.Count);
            
            for (int i = 0; i < expectedModels.Count; i++)
            {
                var expectedModel = expectedModels.ElementAt(i);
                var actualModel = actualModels.ElementAt(i);

                Assert.AreEqual(expectedModel.Name, actualModel.Name);
                Assert.AreEqual(expectedModel.SegmentName, actualModel.SegmentName);
                AssertStochasticSoilProfiles(expectedModel.StochasticSoilProfiles, actualModel.StochasticSoilProfiles);
            }
        }

        private void AssertStochasticSoilProfiles(List<StochasticSoilProfile> expectedStochasticSoilProfiles, List<StochasticSoilProfile> actualStochasticSoilProfiles)
        {
            Assert.Less(0, actualStochasticSoilProfiles.Count);
            Assert.AreEqual(expectedStochasticSoilProfiles.Count, actualStochasticSoilProfiles.Count);

            for (int i = 0; i < expectedStochasticSoilProfiles.Count; i++)
            {
                var expectedProfile = expectedStochasticSoilProfiles.ElementAt(i);
                var actualProfile = actualStochasticSoilProfiles.ElementAt(i);

                Assert.AreEqual(expectedProfile.Probability, actualProfile.Probability);
                Assert.AreEqual(expectedProfile.SoilProfile.Bottom, actualProfile.SoilProfile.Bottom);
                Assert.AreEqual(expectedProfile.SoilProfile.Name, actualProfile.SoilProfile.Name);
                AssertSoilLayers(expectedProfile.SoilProfile.Layers, actualProfile.SoilProfile.Layers);
            }
        }

        private void AssertSoilLayers(IEnumerable<PipingSoilLayer> expectedLayers, IEnumerable<PipingSoilLayer> actualLayers)
        {
            var actualLayerArray = actualLayers.ToArray();
            var expectedLayerArray = expectedLayers.ToArray();
            Assert.Less(0, actualLayerArray.Length);
            Assert.AreEqual(expectedLayerArray.Length, actualLayerArray.Length);

            for (int i = 0; i < expectedLayerArray.Length; i++)
            {
                var expectedLayer = actualLayerArray[i];
                var actualLayer = expectedLayerArray[i];

                Assert.AreEqual(expectedLayer.Top, actualLayer.Top);
                Assert.AreEqual(expectedLayer.IsAquifer, actualLayer.IsAquifer);
            }
        }

        private void AssertSurfaceLines(ICollection<RingtoetsPipingSurfaceLine> expectedSurfaceLines, ICollection<RingtoetsPipingSurfaceLine> actualSurfaceLines)
        {
            // Precondition:
            Assert.Greater(expectedSurfaceLines.Count, 0);

            Assert.AreEqual(expectedSurfaceLines.Count, actualSurfaceLines.Count);
            for (int i = 0; i < expectedSurfaceLines.Count; i++)
            {
                RingtoetsPipingSurfaceLine expectedSurfaceLine = expectedSurfaceLines.ElementAt(i);
                RingtoetsPipingSurfaceLine actualSurfaceLine = expectedSurfaceLines.ElementAt(i);
                AssertSurfaceLine(expectedSurfaceLine, actualSurfaceLine);
            }
        }

        private void AssertSurfaceLine(RingtoetsPipingSurfaceLine expectedSurfaceLine, RingtoetsPipingSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            Assert.AreEqual(expectedSurfaceLine.ReferenceLineIntersectionWorldPoint, actualSurfaceLine.ReferenceLineIntersectionWorldPoint);

            CollectionAssert.AreEqual(expectedSurfaceLine.Points, actualSurfaceLine.Points);

            Assert.AreEqual(expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
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