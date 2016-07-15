﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Threading;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
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
            Dispatcher.CurrentDispatcher.InvokeShutdown();
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
            AssertProjectsAreEqual(firstProject, secondProject);
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
            AssertProjectsAreEqual(fullProject, loadedProject);
        }

        [Test]
        public void SaveProject_SaveWithoutChanges_Return0()
        {
            // Setup
            Project fullProject = RingtoetsProjectHelper.GetFullTestProject();

            var storage = new StorageSqLite();
            storage.SaveProjectAs(tempRingtoetsFile, fullProject);

            // Call
            int numberOfChanges = storage.SaveProject(tempRingtoetsFile, fullProject);

            // Assert
            Assert.AreEqual(0, numberOfChanges);
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithValidFile_ProjectSet()
        {
            // Setup
            var projectStore = new StorageSqLite();
            Project fullProject = RingtoetsProjectHelper.GetFullTestProject();
            var expectedProjectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);
            var expectedProjectDescription = fullProject.Description;

            // Precondition
            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, fullProject);

            using (var gui = new GuiCore(new MainWindow(), projectStore, new GuiCoreSettings()))
            {
                // Call
                Action action = () => gui.Run(tempRingtoetsFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend.",
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 3);
                Assert.AreEqual(tempRingtoetsFile, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(expectedProjectDescription, gui.Project.Description);

                AssertProjectsAreEqual(gui.Project, fullProject);
            }
        }

        [Test]
        [STAThread]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithInvalidFile_EmptyProjectSet()
        {
            // Setup
            var testFile = "SomeFile";
            var projectStore = new StorageSqLite();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new GuiCoreSettings()))
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
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 5);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, new GuiCoreSettings()))
            {
                // Call
                Action action = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Nieuw project aanmaken..."
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 2);
                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
            }
        }

        private void AssertProjectsAreEqual(Project expectedProject, Project actualProject)
        {
            Assert.NotNull(expectedProject);
            Assert.NotNull(actualProject);
            Assert.AreNotSame(expectedProject, actualProject);

            AssessmentSection[] expectedProjectAssessmentSections = expectedProject.Items.OfType<AssessmentSection>().ToArray();
            AssessmentSection[] actualProjectAssessmentSections = actualProject.Items.OfType<AssessmentSection>().ToArray();
            Assert.AreEqual(expectedProjectAssessmentSections.Length, actualProjectAssessmentSections.Length);
            for (var i = 0; i < expectedProjectAssessmentSections.Length; i++)
            {
                AssessmentSection expectedAssessmentSection = expectedProjectAssessmentSections[i];
                AssessmentSection actualAssessmentSection = actualProjectAssessmentSections[i];

                Assert.AreEqual(expectedAssessmentSection.StorageId, actualAssessmentSection.StorageId);
                Assert.AreEqual(expectedAssessmentSection.Name, actualAssessmentSection.Name);

                AssertHydraulicBoundaryDatabase(expectedAssessmentSection.HydraulicBoundaryDatabase, actualAssessmentSection.HydraulicBoundaryDatabase);
                AssertReferenceLine(expectedAssessmentSection.ReferenceLine, actualAssessmentSection.ReferenceLine);
                AssertPipingFailureMechanism(expectedAssessmentSection.PipingFailureMechanism, actualAssessmentSection.PipingFailureMechanism);
                AssertGrassCoverErosionInwardsFailureMechanism(expectedAssessmentSection.GrassCoverErosionInwards, actualAssessmentSection.GrassCoverErosionInwards);

                IFailureMechanism[] expectedProjectFailureMechanisms = expectedAssessmentSection.GetFailureMechanisms().ToArray();
                IFailureMechanism[] actualProjectFailureMechanisms = actualAssessmentSection.GetFailureMechanisms().ToArray();
                for (var fmi = 0; fmi < expectedProjectFailureMechanisms.Length; fmi++)
                {
                    AssertFailureMechanism(expectedProjectFailureMechanisms[fmi], actualProjectFailureMechanisms[fmi]);
                }

                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.PipingFailureMechanism.SectionResults,
                    actualAssessmentSection.PipingFailureMechanism.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.GrassCoverErosionInwards.SectionResults,
                    actualAssessmentSection.GrassCoverErosionInwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.HeightStructures.SectionResults,
                    actualAssessmentSection.HeightStructures.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults,
                    actualAssessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.TechnicalInnovation.SectionResults,
                    actualAssessmentSection.TechnicalInnovation.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.WaterPressureAsphaltCover.SectionResults,
                    actualAssessmentSection.WaterPressureAsphaltCover.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.ClosingStructure.SectionResults,
                    actualAssessmentSection.ClosingStructure.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.MacrostabilityOutwards.SectionResults,
                    actualAssessmentSection.MacrostabilityOutwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.MacrostabilityInwards.SectionResults,
                    actualAssessmentSection.MacrostabilityInwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.WaveImpactAsphaltCover.SectionResults,
                    actualAssessmentSection.WaveImpactAsphaltCover.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.GrassCoverErosionOutwards.SectionResults,
                    actualAssessmentSection.GrassCoverErosionOutwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.GrassCoverSlipOffInwards.SectionResults,
                    actualAssessmentSection.GrassCoverSlipOffInwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.GrassCoverSlipOffOutwards.SectionResults,
                    actualAssessmentSection.GrassCoverSlipOffOutwards.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.Microstability.SectionResults,
                    actualAssessmentSection.Microstability.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.PipingStructure.SectionResults,
                    actualAssessmentSection.PipingStructure.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.DuneErosion.SectionResults,
                    actualAssessmentSection.DuneErosion.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.StabilityStoneCover.SectionResults,
                    actualAssessmentSection.StabilityStoneCover.SectionResults);
                AssertFailureMechanismSectionResults(
                    expectedAssessmentSection.StrengthStabilityPointConstruction.SectionResults,
                    actualAssessmentSection.StrengthStabilityPointConstruction.SectionResults);
            }
        }

        private void AssertFailureMechanismSectionResults(
            IEnumerable<PipingFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<PipingFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                PipingFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                PipingFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                GrassCoverErosionInwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                GrassCoverErosionInwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<HeightStructuresFailureMechanismSectionResult> expectedSectionResults, IEnumerable<HeightStructuresFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                HeightStructuresFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                HeightStructuresFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> expectedSectionResults, IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<TechnicalInnovationFailureMechanismSectionResult> expectedSectionResults, IEnumerable<TechnicalInnovationFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                TechnicalInnovationFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                TechnicalInnovationFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> expectedSectionResults, IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                WaterPressureAsphaltCoverFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                WaterPressureAsphaltCoverFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<ClosingStructureFailureMechanismSectionResult> expectedSectionResults, IEnumerable<ClosingStructureFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                ClosingStructureFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                ClosingStructureFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                MacrostabilityInwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                MacrostabilityInwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                MacrostabilityOutwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                MacrostabilityOutwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> expectedSectionResults, IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                WaveImpactAsphaltCoverFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                WaveImpactAsphaltCoverFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                GrassCoverErosionOutwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                GrassCoverErosionOutwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                GrassCoverSlipOffInwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                GrassCoverSlipOffInwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> expectedSectionResults, IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<MicrostabilityFailureMechanismSectionResult> expectedSectionResults, IEnumerable<MicrostabilityFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                MicrostabilityFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                MicrostabilityFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<PipingStructureFailureMechanismSectionResult> expectedSectionResults, IEnumerable<PipingStructureFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                PipingStructureFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                PipingStructureFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<DuneErosionFailureMechanismSectionResult> expectedSectionResults, IEnumerable<DuneErosionFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                DuneErosionFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                DuneErosionFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> expectedSectionResults, IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                StabilityStoneCoverFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                StabilityStoneCoverFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanismSectionResults(IEnumerable<StrengthStabilityPointConstructionFailureMechanismSectionResult> expectedSectionResults, IEnumerable<StrengthStabilityPointConstructionFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                StrengthStabilityPointConstructionFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                StrengthStabilityPointConstructionFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private void AssertFailureMechanism(IFailureMechanism expectedFailureMechanism, IFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.Name, actualFailureMechanism.Name);
            Assert.AreEqual(expectedFailureMechanism.Code, actualFailureMechanism.Code);
            Assert.AreEqual(expectedFailureMechanism.IsRelevant, actualFailureMechanism.IsRelevant);
            AssertFailureMechanismSections(expectedFailureMechanism.Sections, actualFailureMechanism.Sections);
        }

        private void AssertFailureMechanismSections(IEnumerable<FailureMechanismSection> expectedSections, IEnumerable<FailureMechanismSection> actualSections)
        {
            var expectedSectionsArray = expectedSections.ToArray();
            var actualSectionsArray = actualSections.ToArray();

            Assert.AreEqual(expectedSectionsArray.Length, actualSectionsArray.Length);

            for (var i = 0; i < expectedSectionsArray.Length; i++)
            {
                FailureMechanismSection expectedSection = expectedSectionsArray[i];
                FailureMechanismSection actualSection = actualSectionsArray[i];

                Assert.AreEqual(expectedSection.Name, actualSection.Name);
                CollectionAssert.AreEqual(expectedSection.Points, actualSection.Points);
            }
        }

        private static void AssertHydraulicBoundaryDatabase(HydraulicBoundaryDatabase expectedBoundaryDatabase, HydraulicBoundaryDatabase actualBoundaryDatabase)
        {
            Assert.IsNotNull(expectedBoundaryDatabase);
            Assert.AreEqual(expectedBoundaryDatabase.Version, actualBoundaryDatabase.Version);
            Assert.AreEqual(expectedBoundaryDatabase.FilePath, actualBoundaryDatabase.FilePath);
            Assert.AreEqual(expectedBoundaryDatabase.Locations.Count, actualBoundaryDatabase.Locations.Count);

            for (int i = 0; i < expectedBoundaryDatabase.Locations.Count; i++)
            {
                HydraulicBoundaryLocation expectedBoundaryLocation = expectedBoundaryDatabase.Locations[i];
                HydraulicBoundaryLocation actualBoundaryLocation = actualBoundaryDatabase.Locations[i];

                Assert.AreEqual(expectedBoundaryLocation.Id, actualBoundaryLocation.Id);
                Assert.AreEqual(expectedBoundaryLocation.Name, actualBoundaryLocation.Name);
                Assert.AreEqual(expectedBoundaryLocation.DesignWaterLevel, actualBoundaryLocation.DesignWaterLevel);
                Assert.AreEqual(expectedBoundaryLocation.StorageId, actualBoundaryLocation.StorageId);
                Assert.AreEqual(expectedBoundaryLocation.Location, actualBoundaryLocation.Location);
            }
        }

        private static void AssertReferenceLine(ReferenceLine expectedReferenceLine, ReferenceLine actualReferenceLine)
        {
            Assert.IsNotNull(expectedReferenceLine);

            CollectionAssert.AreEqual(expectedReferenceLine.Points, actualReferenceLine.Points);
        }

        private void AssertPipingFailureMechanism(PipingFailureMechanism expectedPipingFailureMechanism, PipingFailureMechanism actualPipingFailureMechanism)
        {
            AssertProbabilityAssesmentInput(expectedPipingFailureMechanism.PipingProbabilityAssessmentInput, actualPipingFailureMechanism.PipingProbabilityAssessmentInput);
            AssertStochasticSoilModels(expectedPipingFailureMechanism.StochasticSoilModels, actualPipingFailureMechanism.StochasticSoilModels);
            AssertSurfaceLines(expectedPipingFailureMechanism.SurfaceLines, actualPipingFailureMechanism.SurfaceLines);
            AssertCalculationGroup(expectedPipingFailureMechanism.CalculationsGroup, actualPipingFailureMechanism.CalculationsGroup);
        }

        private void AssertProbabilityAssesmentInput(PipingProbabilityAssessmentInput expectedModel, PipingProbabilityAssessmentInput actualModel)
        {
            Assert.AreEqual(expectedModel.A, actualModel.A);
        }

        private void AssertStochasticSoilModels(ObservableList<StochasticSoilModel> expectedModels, ObservableList<StochasticSoilModel> actualModels)
        {
            // Precondition:
            Assert.Less(0, actualModels.Count);

            Assert.AreEqual(expectedModels.Count, actualModels.Count);

            for (int i = 0; i < expectedModels.Count; i++)
            {
                StochasticSoilModel expectedModel = expectedModels[i];
                StochasticSoilModel actualModel = actualModels[i];

                Assert.AreEqual(expectedModel.Name, actualModel.Name);
                Assert.AreEqual(expectedModel.SegmentName, actualModel.SegmentName);
                AssertSegmentPoints(expectedModel.Geometry, actualModel.Geometry);
                AssertStochasticSoilProfiles(expectedModel.StochasticSoilProfiles, actualModel.StochasticSoilProfiles);
            }
        }

        private void AssertSegmentPoints(List<Point2D> expectedSoilModelSegmentPoints, List<Point2D> actualSoilModelSegmentPoints)
        {
            Assert.Greater(expectedSoilModelSegmentPoints.Count, 0);
            CollectionAssert.AreEqual(expectedSoilModelSegmentPoints, actualSoilModelSegmentPoints);
        }

        private void AssertStochasticSoilProfiles(List<StochasticSoilProfile> expectedStochasticSoilProfiles, List<StochasticSoilProfile> actualStochasticSoilProfiles)
        {
            Assert.Less(0, actualStochasticSoilProfiles.Count);
            Assert.AreEqual(expectedStochasticSoilProfiles.Count, actualStochasticSoilProfiles.Count);

            for (int i = 0; i < expectedStochasticSoilProfiles.Count; i++)
            {
                StochasticSoilProfile expectedProfile = expectedStochasticSoilProfiles[i];
                StochasticSoilProfile actualProfile = actualStochasticSoilProfiles[i];

                Assert.AreEqual(expectedProfile.Probability, actualProfile.Probability);
                Assert.AreEqual(expectedProfile.SoilProfile.Bottom, actualProfile.SoilProfile.Bottom);
                Assert.AreEqual(expectedProfile.SoilProfile.Name, actualProfile.SoilProfile.Name);
                AssertSoilLayers(expectedProfile.SoilProfile.Layers, actualProfile.SoilProfile.Layers);
            }
        }

        private void AssertSoilLayers(IEnumerable<PipingSoilLayer> expectedLayers, IEnumerable<PipingSoilLayer> actualLayers)
        {
            PipingSoilLayer[] actualLayerArray = actualLayers.ToArray();
            PipingSoilLayer[] expectedLayerArray = expectedLayers.ToArray();
            Assert.Less(0, actualLayerArray.Length);
            Assert.AreEqual(expectedLayerArray.Length, actualLayerArray.Length);

            for (int i = 0; i < expectedLayerArray.Length; i++)
            {
                PipingSoilLayer expectedLayer = actualLayerArray[i];
                PipingSoilLayer actualLayer = expectedLayerArray[i];

                Assert.AreEqual(expectedLayer.Top, actualLayer.Top);
                Assert.AreEqual(expectedLayer.IsAquifer, actualLayer.IsAquifer);
                Assert.AreEqual(expectedLayer.BelowPhreaticLevelMean, actualLayer.BelowPhreaticLevelMean);
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

        private void AssertCalculationGroup(CalculationGroup expectedRootCalculationGroup, CalculationGroup actualRootCalculationGroup)
        {
            Assert.AreEqual(expectedRootCalculationGroup.Name, actualRootCalculationGroup.Name);
            Assert.AreEqual(expectedRootCalculationGroup.IsNameEditable, actualRootCalculationGroup.IsNameEditable);

            Assert.AreEqual(expectedRootCalculationGroup.Children.Count, actualRootCalculationGroup.Children.Count);
            for (int i = 0; i < expectedRootCalculationGroup.Children.Count; i++)
            {
                ICalculationBase expectedChild = expectedRootCalculationGroup.Children[i];
                ICalculationBase actualChild = actualRootCalculationGroup.Children[i];

                Assert.AreEqual(expectedChild.GetType(), actualChild.GetType());

                var expectedChildGroup = expectedChild as CalculationGroup;
                if (expectedChildGroup != null)
                {
                    AssertCalculationGroup(expectedChildGroup, (CalculationGroup) actualChild);
                }
                var expectedPipingCalculation = expectedChild as PipingCalculationScenario;
                if (expectedPipingCalculation != null)
                {
                    AssertPipingCalculationScenario(expectedPipingCalculation, (PipingCalculationScenario) actualChild);
                }
                // TODO GEBK Berekening
            }
        }

        private void AssertPipingCalculationScenario(PipingCalculationScenario expectedPipingCalculation, PipingCalculationScenario actualPipingCalculation)
        {
            Assert.AreEqual(expectedPipingCalculation.IsRelevant, actualPipingCalculation.IsRelevant);
            Assert.AreEqual(expectedPipingCalculation.Contribution, actualPipingCalculation.Contribution);
            Assert.AreEqual(expectedPipingCalculation.Name, actualPipingCalculation.Name);
            Assert.AreEqual(expectedPipingCalculation.Comments, actualPipingCalculation.Comments);

            AssertPipingInput(expectedPipingCalculation.InputParameters, actualPipingCalculation.InputParameters);
            AssertPipingOutput(expectedPipingCalculation.Output, actualPipingCalculation.Output);
            AssertPipingSemiProbabilisticOutput(expectedPipingCalculation.SemiProbabilisticOutput, actualPipingCalculation.SemiProbabilisticOutput);
        }

        private static void AssertPipingInput(PipingInput expectedPipingInput, PipingInput actualPipingInput)
        {
            Assert.AreEqual(expectedPipingInput.ExitPointL, actualPipingInput.ExitPointL);
            Assert.AreEqual(expectedPipingInput.EntryPointL, actualPipingInput.EntryPointL);
            Assert.AreEqual(expectedPipingInput.PhreaticLevelExit.Mean, actualPipingInput.PhreaticLevelExit.Mean);
            Assert.AreEqual(expectedPipingInput.PhreaticLevelExit.StandardDeviation, actualPipingInput.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(expectedPipingInput.DampingFactorExit.Mean, actualPipingInput.DampingFactorExit.Mean);
            Assert.AreEqual(expectedPipingInput.DampingFactorExit.StandardDeviation, actualPipingInput.DampingFactorExit.StandardDeviation);
            Assert.AreEqual(expectedPipingInput.SaturatedVolumicWeightOfCoverageLayer.Mean, actualPipingInput.SaturatedVolumicWeightOfCoverageLayer.Mean);
            Assert.AreEqual(expectedPipingInput.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation, actualPipingInput.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation);
            Assert.AreEqual(expectedPipingInput.SaturatedVolumicWeightOfCoverageLayer.Shift, actualPipingInput.SaturatedVolumicWeightOfCoverageLayer.Shift);
            Assert.AreEqual(expectedPipingInput.Diameter70.Mean, actualPipingInput.Diameter70.Mean);
            Assert.AreEqual(expectedPipingInput.Diameter70.StandardDeviation, actualPipingInput.Diameter70.StandardDeviation);
            Assert.AreEqual(expectedPipingInput.DarcyPermeability.Mean, actualPipingInput.DarcyPermeability.Mean);
            Assert.AreEqual(expectedPipingInput.DarcyPermeability.StandardDeviation, actualPipingInput.DarcyPermeability.StandardDeviation);
        }

        private void AssertPipingOutput(PipingOutput expectedOutput, PipingOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
            }
            else
            {
                Assert.AreEqual(expectedOutput.HeaveFactorOfSafety, actualOutput.HeaveFactorOfSafety);
                Assert.AreEqual(expectedOutput.HeaveZValue, actualOutput.HeaveZValue);
                Assert.AreEqual(expectedOutput.UpliftFactorOfSafety, actualOutput.UpliftFactorOfSafety);
                Assert.AreEqual(expectedOutput.UpliftZValue, actualOutput.UpliftZValue);
                Assert.AreEqual(expectedOutput.SellmeijerFactorOfSafety, actualOutput.SellmeijerFactorOfSafety);
                Assert.AreEqual(expectedOutput.SellmeijerZValue, actualOutput.SellmeijerZValue);
            }
        }

        private void AssertPipingSemiProbabilisticOutput(PipingSemiProbabilisticOutput expectedOutput, PipingSemiProbabilisticOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
            }
            else
            {
                Assert.AreEqual(expectedOutput.HeaveFactorOfSafety, actualOutput.HeaveFactorOfSafety);
                Assert.AreEqual(expectedOutput.HeaveProbability, actualOutput.HeaveProbability);
                Assert.AreEqual(expectedOutput.HeaveReliability, actualOutput.HeaveReliability);

                Assert.AreEqual(expectedOutput.SellmeijerFactorOfSafety, actualOutput.SellmeijerFactorOfSafety);
                Assert.AreEqual(expectedOutput.SellmeijerProbability, actualOutput.SellmeijerProbability);
                Assert.AreEqual(expectedOutput.SellmeijerReliability, actualOutput.SellmeijerReliability);

                Assert.AreEqual(expectedOutput.UpliftFactorOfSafety, actualOutput.UpliftFactorOfSafety);
                Assert.AreEqual(expectedOutput.UpliftProbability, actualOutput.UpliftProbability);
                Assert.AreEqual(expectedOutput.UpliftReliability, actualOutput.UpliftReliability);

                Assert.AreEqual(expectedOutput.RequiredReliability, actualOutput.RequiredReliability);
                Assert.AreEqual(expectedOutput.RequiredProbability, actualOutput.RequiredProbability);

                Assert.AreEqual(expectedOutput.PipingFactorOfSafety, actualOutput.PipingFactorOfSafety);
                Assert.AreEqual(expectedOutput.PipingReliability, actualOutput.PipingReliability);
                Assert.AreEqual(expectedOutput.PipingProbability, actualOutput.PipingProbability);
            }
        }

        private void AssertGrassCoverErosionInwardsFailureMechanism(GrassCoverErosionInwardsFailureMechanism expectedFailureMechanism,
                                                                           GrassCoverErosionInwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
            AssertDikeProfiles(expectedFailureMechanism.DikeProfiles, actualFailureMechanism.DikeProfiles);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
        }

        private static void AssertDikeProfiles(IList<DikeProfile> expectedDikeProfiles, IList<DikeProfile> actualDikeProfiles)
        {
            Assert.AreEqual(expectedDikeProfiles.Count, actualDikeProfiles.Count);
            for (int i = 0; i < expectedDikeProfiles.Count; i++)
            {
                AssertDikeProfile(expectedDikeProfiles[i], actualDikeProfiles[i]);
            }
        }

        private static void AssertDikeProfile(DikeProfile expectedDikeProfile, DikeProfile actualDikeProfile)
        {
            Assert.AreEqual(expectedDikeProfile.Name, actualDikeProfile.Name);
            Assert.AreEqual(expectedDikeProfile.WorldReferencePoint, actualDikeProfile.WorldReferencePoint);
            Assert.AreEqual(expectedDikeProfile.X0, actualDikeProfile.X0);
            Assert.AreEqual(expectedDikeProfile.Orientation, actualDikeProfile.Orientation);
            AssertBreakWater(expectedDikeProfile.BreakWater, actualDikeProfile.BreakWater);
            CollectionAssert.AreEqual(expectedDikeProfile.ForeshoreGeometry, actualDikeProfile.ForeshoreGeometry);
            AssertRoughnessPoints(expectedDikeProfile.DikeGeometry, actualDikeProfile.DikeGeometry);
            Assert.AreEqual(expectedDikeProfile.DikeHeight, actualDikeProfile.DikeHeight);
        }

        private static void AssertBreakWater(BreakWater expectedBreakWater, BreakWater actualBreakWater)
        {
            if (expectedBreakWater == null)
            {
                Assert.IsNull(actualBreakWater);
            }
            else
            {
                Assert.AreEqual(expectedBreakWater.Height, actualBreakWater.Height);
                Assert.AreEqual(expectedBreakWater.Type, actualBreakWater.Type);
            }
        }

        private static void AssertRoughnessPoints(RoughnessPoint[] expectedRoughnessPoints, RoughnessPoint[] actualRoughnessPoints)
        {
            Assert.AreEqual(expectedRoughnessPoints.Length, actualRoughnessPoints.Length);
            for (int i = 0; i < expectedRoughnessPoints.Length; i++)
            {
                AssertRoughnessPoint(expectedRoughnessPoints[i], actualRoughnessPoints[i]);
            }
        }

        private static void AssertRoughnessPoint(RoughnessPoint expectedRoughnessPoint, RoughnessPoint actualRoughnessPoint)
        {
            Assert.AreEqual(expectedRoughnessPoint.Point, actualRoughnessPoint.Point);
            Assert.AreEqual(expectedRoughnessPoint.Roughness, actualRoughnessPoint.Roughness);
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