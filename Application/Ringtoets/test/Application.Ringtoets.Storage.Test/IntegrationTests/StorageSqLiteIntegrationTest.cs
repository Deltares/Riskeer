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
using System.Threading;
using System.Windows.Threading;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageSqLiteIntegrationTest
    {
        private const string tempExtension = ".temp";

        [Test]
        public void SaveProjectAs_SaveAsNewFile_ProjectAsEntitiesInBothFiles()
        {
            // Setup
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            string firstRingtoetsFile = GetRandomRingtoetsFile();
            string secondRingtoetsFile = GetRandomRingtoetsFile();

            StorageSqLite storage = new StorageSqLite();
            storage.StageProject(fullProject);
            storage.SaveProjectAs(firstRingtoetsFile);

            // Call
            RingtoetsProject firstProject = null;
            RingtoetsProject secondProject = null;
            try
            {
                storage.StageProject(fullProject);
                storage.SaveProjectAs(secondRingtoetsFile);
                firstProject = (RingtoetsProject) storage.LoadProject(firstRingtoetsFile);
                secondProject = (RingtoetsProject) storage.LoadProject(secondRingtoetsFile);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
            finally
            {
                // TearDown
                TearDownTempRingtoetsFile(secondRingtoetsFile);
            }

            // Assert
            AssertProjectsAreEqual(firstProject, secondProject);
        }

        [Test]
        public void GivenRingtoetsProject_WhenComparingFingerPrintsVariousScenariosUnchangedData_ThenFingerprintUnchanged()
        {
            // Given
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            string tempRingtoetsFile = GetRandomRingtoetsFile();

            // When
            var entityBeforeSave = fullProject.Create(new PersistenceRegistry());

            byte[] hash1 = FingerprintHelper.Get(entityBeforeSave);

            StorageSqLite storage = new StorageSqLite();
            storage.StageProject(fullProject);
            storage.SaveProjectAs(tempRingtoetsFile);

            ProjectEntity entityAfterSave = fullProject.Create(new PersistenceRegistry());
            byte[] hash2 = FingerprintHelper.Get(entityAfterSave);

            var openedProject = (RingtoetsProject) storage.LoadProject(tempRingtoetsFile);
            ProjectEntity entityAfterOpening = openedProject.Create(new PersistenceRegistry());

            byte[] hash3 = FingerprintHelper.Get(entityAfterOpening);

            // Then
            CollectionAssert.AreEqual(hash1, hash2);
            CollectionAssert.AreEqual(hash1, hash3);
        }

        [Test]
        public void LoadProject_FullTestProjectSaved_ProjectAsEntitiesInNewStorage()
        {
            // Setup
            StorageSqLite storage = new StorageSqLite();
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            storage.StageProject(fullProject);
            string tempRingtoetsFile = GetRandomRingtoetsFile();
            storage.SaveProjectAs(tempRingtoetsFile);

            // Call
            RingtoetsProject loadedProject = (RingtoetsProject) storage.LoadProject(tempRingtoetsFile);

            // Assert
            AssertProjectsAreEqual(fullProject, loadedProject);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GivenRingtoetsGuiWithStorageSqlAndMigrator_WhenRunWithEmptyFile_DefaultProjectStillSet(string testFile)
        {
            // Given
            var mocks = new MockRepository();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            var projectStore = new StorageSqLite();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(testFile);

                // Then
                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
                Assert.IsInstanceOf<RingtoetsProject>(gui.Project);
                CollectionAssert.IsEmpty(((RingtoetsProject) gui.Project).AssessmentSections);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenRingtoetsGuiWithStorageSqlAndMigrator_WhenRunWithValidFile_ProjectSet()
        {
            // Given
            string tempRingtoetsFile = GetRandomRingtoetsFile();
            var expectedProjectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);

            var mocks = new MockRepository();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(tempRingtoetsFile)).Return(MigrationNeeded.No);
            mocks.ReplayAll();

            var projectStore = new StorageSqLite();

            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            var expectedProjectDescription = fullProject.Description;

            // Precondition
            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, fullProject);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                // When
                Action action = () => gui.Run(tempRingtoetsFile);

                // Then
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject...",
                    "Bestaand Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(action, expectedMessages, 3);
                Assert.AreEqual(tempRingtoetsFile, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(expectedProjectDescription, gui.Project.Description);

                Assert.IsInstanceOf<RingtoetsProject>(gui.Project);
                AssertProjectsAreEqual(fullProject, (RingtoetsProject) gui.Project);
            }

            mocks.VerifyAll();
        }

        [OneTimeTearDown]
        public void TearDownTempRingtoetsFile()
        {
            IEnumerable<string> filesToDelete = Directory.EnumerateFiles(TestHelper.GetScratchPadPath(), $"*{tempExtension}");

            foreach (string fileToDelete in filesToDelete)
            {
                TearDownTempRingtoetsFile(fileToDelete);
            }
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private static string GetRandomRingtoetsFile()
        {
            return Path.Combine(TestHelper.GetScratchPadPath(),
                                string.Concat(Path.GetRandomFileName(), tempExtension));
        }

        private static void AssertProjectsAreEqual(RingtoetsProject expectedProject, RingtoetsProject actualProject)
        {
            Assert.NotNull(expectedProject);
            Assert.NotNull(actualProject);
            Assert.AreNotSame(expectedProject, actualProject);

            IList<AssessmentSection> expectedProjectAssessmentSections = expectedProject.AssessmentSections;
            IList<AssessmentSection> actualProjectAssessmentSections = actualProject.AssessmentSections;
            Assert.AreEqual(expectedProjectAssessmentSections.Count, actualProjectAssessmentSections.Count);
            for (var i = 0; i < expectedProjectAssessmentSections.Count; i++)
            {
                AssessmentSection expectedAssessmentSection = expectedProjectAssessmentSections[i];
                AssessmentSection actualAssessmentSection = actualProjectAssessmentSections[i];

                Assert.AreEqual(expectedAssessmentSection.Id, actualAssessmentSection.Id);
                Assert.AreEqual(expectedAssessmentSection.Name, actualAssessmentSection.Name);
                AssertComments(expectedAssessmentSection.Comments, actualAssessmentSection.Comments);

                AssertBackgroundMapDataContainer(expectedAssessmentSection.BackgroundMapData, actualAssessmentSection.BackgroundMapData);
                AssertHydraulicBoundaryDatabase(expectedAssessmentSection.HydraulicBoundaryDatabase, actualAssessmentSection.HydraulicBoundaryDatabase);
                AssertReferenceLine(expectedAssessmentSection.ReferenceLine, actualAssessmentSection.ReferenceLine);
                AssertPipingFailureMechanism(expectedAssessmentSection.PipingFailureMechanism, actualAssessmentSection.PipingFailureMechanism);
                AssertGrassCoverErosionInwardsFailureMechanism(expectedAssessmentSection.GrassCoverErosionInwards, actualAssessmentSection.GrassCoverErosionInwards);
                AssertGrassCoverErosionOutwardsFailureMechanism(expectedAssessmentSection.GrassCoverErosionOutwards, actualAssessmentSection.GrassCoverErosionOutwards);
                AssertStabilityStoneCoverFailureMechanism(expectedAssessmentSection.StabilityStoneCover, actualAssessmentSection.StabilityStoneCover);
                AssertWaveImpactAsphaltCoverFailureMechanism(expectedAssessmentSection.WaveImpactAsphaltCover, actualAssessmentSection.WaveImpactAsphaltCover);
                AssertHeightStructuresFailureMechanism(expectedAssessmentSection.HeightStructures, actualAssessmentSection.HeightStructures);
                AssertClosingStructuresFailureMechanism(expectedAssessmentSection.ClosingStructures, actualAssessmentSection.ClosingStructures);
                AssertDuneErosionFailureMechanism(expectedAssessmentSection.DuneErosion, actualAssessmentSection.DuneErosion);
                AssertStabilityPointStructuresFailureMechanism(expectedAssessmentSection.StabilityPointStructures, actualAssessmentSection.StabilityPointStructures);

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
                    expectedAssessmentSection.ClosingStructures.SectionResults,
                    actualAssessmentSection.ClosingStructures.SectionResults);
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
                    expectedAssessmentSection.StabilityPointStructures.SectionResults,
                    actualAssessmentSection.StabilityPointStructures.SectionResults);
            }
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<TechnicalInnovationFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<TechnicalInnovationFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MicrostabilityFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MicrostabilityFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<PipingStructureFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<PipingStructureFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertFailureMechanism(IFailureMechanism expectedFailureMechanism,
                                                   IFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.Name, actualFailureMechanism.Name);
            Assert.AreEqual(expectedFailureMechanism.Code, actualFailureMechanism.Code);
            Assert.AreEqual(expectedFailureMechanism.IsRelevant, actualFailureMechanism.IsRelevant);
            AssertFailureMechanismSections(expectedFailureMechanism.Sections, actualFailureMechanism.Sections);
        }

        private static void AssertFailureMechanismSections(IEnumerable<FailureMechanismSection> expectedSections,
                                                           IEnumerable<FailureMechanismSection> actualSections)
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

        private static void AssertReferenceLine(ReferenceLine expectedReferenceLine, ReferenceLine actualReferenceLine)
        {
            Assert.IsNotNull(expectedReferenceLine);

            CollectionAssert.AreEqual(expectedReferenceLine.Points, actualReferenceLine.Points);
        }

        private static void AssertStochasticSoilModels(StochasticSoilModelCollection expectedModels,
                                                       StochasticSoilModelCollection actualModels)
        {
            // Precondition:
            Assert.Less(0, actualModels.Count);

            Assert.AreEqual(expectedModels.SourcePath, actualModels.SourcePath);
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

        private static void AssertSegmentPoints(List<Point2D> expectedSoilModelSegmentPoints,
                                                List<Point2D> actualSoilModelSegmentPoints)
        {
            Assert.Greater(expectedSoilModelSegmentPoints.Count, 0);
            CollectionAssert.AreEqual(expectedSoilModelSegmentPoints, actualSoilModelSegmentPoints);
        }

        private static void AssertStochasticSoilProfiles(List<StochasticSoilProfile> expectedStochasticSoilProfiles,
                                                         List<StochasticSoilProfile> actualStochasticSoilProfiles)
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

        private static void AssertSoilLayers(IEnumerable<PipingSoilLayer> expectedLayers, IEnumerable<PipingSoilLayer> actualLayers)
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
                Assert.AreEqual(expectedLayer.BelowPhreaticLevelDeviation, actualLayer.BelowPhreaticLevelDeviation);
                Assert.AreEqual(expectedLayer.BelowPhreaticLevelShift, actualLayer.BelowPhreaticLevelShift);
                Assert.AreEqual(expectedLayer.DiameterD70Mean, actualLayer.DiameterD70Mean);
                Assert.AreEqual(expectedLayer.DiameterD70Deviation, actualLayer.DiameterD70Deviation);
                Assert.AreEqual(expectedLayer.PermeabilityMean, actualLayer.PermeabilityMean);
                Assert.AreEqual(expectedLayer.PermeabilityDeviation, actualLayer.PermeabilityDeviation);
            }
        }

        private static void AssertSurfaceLines(RingtoetsPipingSurfaceLineCollection expectedSurfaceLines,
                                               RingtoetsPipingSurfaceLineCollection actualSurfaceLines)
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

        private static void AssertSurfaceLine(RingtoetsPipingSurfaceLine expectedSurfaceLine,
                                              RingtoetsPipingSurfaceLine actualSurfaceLine)
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

        private static void AssertCalculationGroup(CalculationGroup expectedRootCalculationGroup,
                                                   CalculationGroup actualRootCalculationGroup)
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
                var expectedGrassCoverErosionInwardsCalculation = expectedChild as GrassCoverErosionInwardsCalculation;
                if (expectedGrassCoverErosionInwardsCalculation != null)
                {
                    AssertGrassCoverErosionInwardsCalculation(expectedGrassCoverErosionInwardsCalculation, (GrassCoverErosionInwardsCalculation) actualChild);
                }
                var expectedGrassCoverErosionOutwardsCalculation = expectedChild as GrassCoverErosionOutwardsWaveConditionsCalculation;
                if (expectedGrassCoverErosionOutwardsCalculation != null)
                {
                    AssertGrassCoverErosionOutwardsWaveConditionsCalculation(expectedGrassCoverErosionOutwardsCalculation, (GrassCoverErosionOutwardsWaveConditionsCalculation) actualChild);
                }
                var expectedHeightStructuresCalculation = expectedChild as StructuresCalculation<HeightStructuresInput>;
                if (expectedHeightStructuresCalculation != null)
                {
                    AssertStructuresCalculation(expectedHeightStructuresCalculation, (StructuresCalculation<HeightStructuresInput>) actualChild);
                }
                var expectedClosingStructuresCalculation = expectedChild as StructuresCalculation<ClosingStructuresInput>;
                if (expectedClosingStructuresCalculation != null)
                {
                    AssertStructuresCalculation(expectedClosingStructuresCalculation, (StructuresCalculation<ClosingStructuresInput>) actualChild);
                }
                var expectedStabilityPointStructuresCalculation = expectedChild as StructuresCalculation<StabilityPointStructuresInput>;
                if (expectedStabilityPointStructuresCalculation != null)
                {
                    AssertStructuresCalculation(expectedStabilityPointStructuresCalculation, (StructuresCalculation<StabilityPointStructuresInput>) actualChild);
                }
                var expectedStabilityStoneCoverWaveConditionsCalculation = expectedChild as StabilityStoneCoverWaveConditionsCalculation;
                if (expectedStabilityStoneCoverWaveConditionsCalculation != null)
                {
                    AssertStabilityStoneCoverWaveConditionsCalculation(expectedStabilityStoneCoverWaveConditionsCalculation, (StabilityStoneCoverWaveConditionsCalculation) actualChild);
                }
                var expectedWaveImpactAsphaltCoverWaveConditionsCalculation = expectedChild as WaveImpactAsphaltCoverWaveConditionsCalculation;
                if (expectedWaveImpactAsphaltCoverWaveConditionsCalculation != null)
                {
                    AssertWaveImpactAsphaltCoverWaveConditionsCalculation(expectedWaveImpactAsphaltCoverWaveConditionsCalculation, (WaveImpactAsphaltCoverWaveConditionsCalculation) actualChild);
                }
            }
        }

        private static void AssertProbabilityAssessmentOutput(ProbabilityAssessmentOutput expectedOutput,
                                                              ProbabilityAssessmentOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.FactorOfSafety, actualOutput.FactorOfSafety);
            Assert.AreEqual(expectedOutput.Probability, actualOutput.Probability);
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);
            Assert.AreEqual(expectedOutput.RequiredProbability, actualOutput.RequiredProbability);
            Assert.AreEqual(expectedOutput.RequiredReliability, actualOutput.RequiredReliability);
        }

        private static void AssertWaveConditionsInput(WaveConditionsInput expectedInput, WaveConditionsInput actualInput)
        {
            if (expectedInput.ForeshoreProfile == null)
            {
                Assert.IsNull(actualInput.ForeshoreProfile);
            }
            else
            {
                AssertForeshoreProfile(expectedInput.ForeshoreProfile, actualInput.ForeshoreProfile);
            }
            if (expectedInput.HydraulicBoundaryLocation == null)
            {
                Assert.IsNull(actualInput.HydraulicBoundaryLocation);
            }
            else
            {
                AssertHydraulicBoundaryLocation(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
            }
            AssertBreakWater(expectedInput.BreakWater, actualInput.BreakWater);
            Assert.AreEqual(expectedInput.Orientation, actualInput.Orientation);
            Assert.AreEqual(expectedInput.UseBreakWater, actualInput.UseBreakWater);
            Assert.AreEqual(expectedInput.UseForeshore, actualInput.UseForeshore);
            Assert.AreEqual(expectedInput.UpperBoundaryRevetment, actualInput.UpperBoundaryRevetment);
            Assert.AreEqual(expectedInput.LowerBoundaryRevetment, actualInput.LowerBoundaryRevetment);
            Assert.AreEqual(expectedInput.UpperBoundaryWaterLevels, actualInput.UpperBoundaryWaterLevels);
            Assert.AreEqual(expectedInput.LowerBoundaryWaterLevels, actualInput.LowerBoundaryWaterLevels);
            Assert.AreEqual(expectedInput.StepSize, actualInput.StepSize);
        }

        private static void AssertWaveConditionsOutputs(WaveConditionsOutput[] expectedOutputs,
                                                        WaveConditionsOutput[] actualOutputs)
        {
            Assert.AreEqual(expectedOutputs.Length, actualOutputs.Length);
            for (var i = 0; i < expectedOutputs.Length; i++)
            {
                WaveConditionsOutput expectedOutput = expectedOutputs[i];
                WaveConditionsOutput actualOutput = actualOutputs[i];

                AssertWaveConditionsOutput(expectedOutput, actualOutput);
            }
        }

        private static void AssertWaveConditionsOutput(WaveConditionsOutput expectedOutput,
                                                       WaveConditionsOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.WaterLevel, actualOutput.WaterLevel, expectedOutput.WaterLevel.GetAccuracy());
            Assert.AreEqual(expectedOutput.WaveHeight, actualOutput.WaveHeight, expectedOutput.WaveHeight.GetAccuracy());
            Assert.AreEqual(expectedOutput.WavePeakPeriod, actualOutput.WavePeakPeriod, expectedOutput.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(expectedOutput.WaveAngle, actualOutput.WaveAngle, expectedOutput.WaveAngle.GetAccuracy());
            Assert.AreEqual(expectedOutput.WaveDirection, actualOutput.WaveDirection, expectedOutput.WaveDirection.GetAccuracy());
            Assert.AreEqual(expectedOutput.TargetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(expectedOutput.TargetReliability, actualOutput.TargetReliability, expectedOutput.TargetReliability.GetAccuracy());
            Assert.AreEqual(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability, expectedOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(expectedOutput.CalculationConvergence, actualOutput.CalculationConvergence);
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

        private static void AssertForeshoreProfiles(IList<ForeshoreProfile> expectedForeshoreProfiles,
                                                    IList<ForeshoreProfile> actualDikeProfiles)
        {
            Assert.AreEqual(expectedForeshoreProfiles.Count, actualDikeProfiles.Count);
            for (int i = 0; i < expectedForeshoreProfiles.Count; i++)
            {
                AssertForeshoreProfile(expectedForeshoreProfiles[i], actualDikeProfiles[i]);
            }
        }

        private static void AssertForeshoreProfile(ForeshoreProfile expectedDikeProfile, ForeshoreProfile actualDikeProfile)
        {
            Assert.AreEqual(expectedDikeProfile.Name, actualDikeProfile.Name);
            Assert.AreEqual(expectedDikeProfile.WorldReferencePoint, actualDikeProfile.WorldReferencePoint);
            Assert.AreEqual(expectedDikeProfile.X0, actualDikeProfile.X0);
            Assert.AreEqual(expectedDikeProfile.Orientation, actualDikeProfile.Orientation);
            AssertBreakWater(expectedDikeProfile.BreakWater, actualDikeProfile.BreakWater);
            CollectionAssert.AreEqual(expectedDikeProfile.Geometry, actualDikeProfile.Geometry);
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

        private static void AssertRoughnessPoints(RoughnessPoint[] expectedRoughnessPoints,
                                                  RoughnessPoint[] actualRoughnessPoints)
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

        private static void AssertComments(Comment expectedComments, Comment actualComments)
        {
            Assert.AreEqual(expectedComments.Body, actualComments.Body);
        }

        private static void AssertStructureInputBase<T>(StructuresInputBase<T> expectedInput,
                                                        StructuresInputBase<T> actualInput) where T : StructureBase
        {
            Assert.AreEqual(expectedInput.UseForeshore, actualInput.UseForeshore);
            if (expectedInput.ForeshoreProfile == null)
            {
                Assert.IsNull(actualInput.ForeshoreProfile);
            }
            else
            {
                AssertForeshoreProfile(expectedInput.ForeshoreProfile, actualInput.ForeshoreProfile);
            }

            if (expectedInput.HydraulicBoundaryLocation == null)
            {
                Assert.IsNull(actualInput.HydraulicBoundaryLocation);
            }
            else
            {
                AssertHydraulicBoundaryLocation(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
            }

            Assert.AreEqual(expectedInput.StructureNormalOrientation, actualInput.StructureNormalOrientation);
            DistributionAssert.AreEqual(expectedInput.ModelFactorSuperCriticalFlow, actualInput.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(expectedInput.AllowedLevelIncreaseStorage, actualInput.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedInput.StorageStructureArea, actualInput.StorageStructureArea);
            DistributionAssert.AreEqual(expectedInput.FlowWidthAtBottomProtection, actualInput.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedInput.CriticalOvertoppingDischarge, actualInput.CriticalOvertoppingDischarge);
            Assert.AreEqual(expectedInput.FailureProbabilityStructureWithErosion, actualInput.FailureProbabilityStructureWithErosion);
            DistributionAssert.AreEqual(expectedInput.WidthFlowApertures, actualInput.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedInput.StormDuration, actualInput.StormDuration);
            Assert.AreEqual(expectedInput.UseBreakWater, actualInput.UseBreakWater);
            AssertBreakWater(expectedInput.BreakWater, actualInput.BreakWater);
        }

        private static void TearDownTempRingtoetsFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        #region StabilityPointStructures FailureMechanism

        private static void AssertStabilityPointStructuresFailureMechanism(StabilityPointStructuresFailureMechanism expectedFailureMechanism,
                                                                           StabilityPointStructuresFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertStabilityPointStructures(expectedFailureMechanism.StabilityPointStructures, actualFailureMechanism.StabilityPointStructures);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                StabilityPointStructuresFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                StabilityPointStructuresFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
                if (expectedSection.Calculation == null)
                {
                    Assert.IsNull(actualSection.Calculation);
                }
                else
                {
                    AssertStructuresCalculation(expectedSection.Calculation, actualSection.Calculation);
                }
            }
        }

        private static void AssertStabilityPointStructures(ObservableList<StabilityPointStructure> expectedStabilityPointStructures,
                                                           ObservableList<StabilityPointStructure> actualStabilityPointStructures)
        {
            Assert.AreEqual(expectedStabilityPointStructures.Count, actualStabilityPointStructures.Count);
            for (int i = 0; i < expectedStabilityPointStructures.Count; i++)
            {
                AssertStabilityPointStructure(expectedStabilityPointStructures[i], actualStabilityPointStructures[i]);
            }
        }

        private static void AssertStabilityPointStructure(StabilityPointStructure expectedStabilityPointStructure,
                                                          StabilityPointStructure actualStabilityPointStructure)
        {
            Assert.AreEqual(expectedStabilityPointStructure.Name, actualStabilityPointStructure.Name);
            Assert.AreEqual(expectedStabilityPointStructure.Id, actualStabilityPointStructure.Id);
            Assert.AreEqual(expectedStabilityPointStructure.Location, actualStabilityPointStructure.Location);
            Assert.AreEqual(expectedStabilityPointStructure.StructureNormalOrientation, actualStabilityPointStructure.StructureNormalOrientation);

            DistributionAssert.AreEqual(expectedStabilityPointStructure.StorageStructureArea, actualStabilityPointStructure.StorageStructureArea);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.AllowedLevelIncreaseStorage, actualStabilityPointStructure.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.WidthFlowApertures, actualStabilityPointStructure.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.InsideWaterLevel, actualStabilityPointStructure.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ThresholdHeightOpenWeir, actualStabilityPointStructure.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.CriticalOvertoppingDischarge, actualStabilityPointStructure.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FlowWidthAtBottomProtection, actualStabilityPointStructure.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ConstructiveStrengthLinearLoadModel, actualStabilityPointStructure.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ConstructiveStrengthQuadraticLoadModel, actualStabilityPointStructure.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.BankWidth, actualStabilityPointStructure.BankWidth);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.InsideWaterLevelFailureConstruction, actualStabilityPointStructure.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(expectedStabilityPointStructure.EvaluationLevel, actualStabilityPointStructure.EvaluationLevel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.LevelCrestStructure, actualStabilityPointStructure.LevelCrestStructure);
            Assert.AreEqual(expectedStabilityPointStructure.VerticalDistance, actualStabilityPointStructure.VerticalDistance);
            Assert.AreEqual(expectedStabilityPointStructure.FailureProbabilityRepairClosure, actualStabilityPointStructure.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FailureCollisionEnergy, actualStabilityPointStructure.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ShipMass, actualStabilityPointStructure.ShipMass);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ShipVelocity, actualStabilityPointStructure.ShipVelocity);
            Assert.AreEqual(expectedStabilityPointStructure.LevellingCount, actualStabilityPointStructure.LevellingCount);
            Assert.AreEqual(expectedStabilityPointStructure.ProbabilityCollisionSecondaryStructure, actualStabilityPointStructure.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FlowVelocityStructureClosable, actualStabilityPointStructure.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.StabilityLinearLoadModel, actualStabilityPointStructure.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.StabilityQuadraticLoadModel, actualStabilityPointStructure.StabilityQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.AreaFlowApertures, actualStabilityPointStructure.AreaFlowApertures);
            Assert.AreEqual(expectedStabilityPointStructure.InflowModelType, actualStabilityPointStructure.InflowModelType);
        }

        private static void AssertStructuresCalculation(StructuresCalculation<StabilityPointStructuresInput> expectedCalculation,
                                                        StructuresCalculation<StabilityPointStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertStabilityPointStructuresInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertProbabilityAssessmentOutput(expectedCalculation.Output, actualCalculation.Output);
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        private static void AssertStabilityPointStructuresInput(StabilityPointStructuresInput expectedInput,
                                                                StabilityPointStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            if (expectedInput.Structure == null)
            {
                Assert.IsNull(actualInput.Structure);
            }
            else
            {
                AssertStabilityPointStructure(expectedInput.Structure, actualInput.Structure);
            }

            DistributionAssert.AreEqual(expectedInput.InsideWaterLevel, actualInput.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedInput.ThresholdHeightOpenWeir, actualInput.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedInput.ConstructiveStrengthLinearLoadModel, actualInput.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedInput.ConstructiveStrengthQuadraticLoadModel, actualInput.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedInput.BankWidth, actualInput.BankWidth);
            DistributionAssert.AreEqual(expectedInput.InsideWaterLevelFailureConstruction, actualInput.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(expectedInput.EvaluationLevel, actualInput.EvaluationLevel);
            DistributionAssert.AreEqual(expectedInput.LevelCrestStructure, actualInput.LevelCrestStructure);
            Assert.AreEqual(expectedInput.VerticalDistance, actualInput.VerticalDistance);
            Assert.AreEqual(expectedInput.FailureProbabilityRepairClosure, actualInput.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(expectedInput.FailureCollisionEnergy, actualInput.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedInput.ShipMass, actualInput.ShipMass);
            DistributionAssert.AreEqual(expectedInput.ShipVelocity, actualInput.ShipVelocity);
            Assert.AreEqual(expectedInput.LevellingCount, actualInput.LevellingCount);
            Assert.AreEqual(expectedInput.ProbabilityCollisionSecondaryStructure, actualInput.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(expectedInput.FlowVelocityStructureClosable, actualInput.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedInput.StabilityLinearLoadModel, actualInput.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedInput.StabilityQuadraticLoadModel, actualInput.StabilityQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedInput.AreaFlowApertures, actualInput.AreaFlowApertures);
            Assert.AreEqual(expectedInput.InflowModelType, actualInput.InflowModelType);
            Assert.AreEqual(expectedInput.LoadSchematizationType, actualInput.LoadSchematizationType);
            Assert.AreEqual(expectedInput.VolumicWeightWater, actualInput.VolumicWeightWater);
            Assert.AreEqual(expectedInput.FactorStormDurationOpenStructure, actualInput.FactorStormDurationOpenStructure);
            DistributionAssert.AreEqual(expectedInput.DrainCoefficient, actualInput.DrainCoefficient);
        }

        #endregion

        #region ClosingStructures FailureMechanism

        private static void AssertClosingStructuresFailureMechanism(ClosingStructuresFailureMechanism expectedFailureMechanism,
                                                                    ClosingStructuresFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N2A, actualFailureMechanism.GeneralInput.N2A);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertClosingStructures(expectedFailureMechanism.ClosingStructures, actualFailureMechanism.ClosingStructures);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<ClosingStructuresFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<ClosingStructuresFailureMechanismSectionResult> actualSectionResults)
        {
            var expectedSectionResultsArray = expectedSectionResults.ToArray();
            var actualSectionResultsArray = actualSectionResults.ToArray();

            Assert.AreEqual(expectedSectionResultsArray.Length, actualSectionResultsArray.Length);

            for (var i = 0; i < expectedSectionResultsArray.Length; i++)
            {
                ClosingStructuresFailureMechanismSectionResult expectedSection = expectedSectionResultsArray[i];
                ClosingStructuresFailureMechanismSectionResult actualSection = actualSectionResultsArray[i];

                Assert.AreEqual(expectedSection.AssessmentLayerOne, actualSection.AssessmentLayerOne);
                Assert.AreEqual(expectedSection.AssessmentLayerTwoA, actualSection.AssessmentLayerTwoA);
                Assert.AreEqual(expectedSection.AssessmentLayerThree, actualSection.AssessmentLayerThree);
            }
        }

        private static void AssertClosingStructures(ObservableList<ClosingStructure> expectedClosingStructures,
                                                    ObservableList<ClosingStructure> actualClosingStructures)
        {
            Assert.AreEqual(expectedClosingStructures.Count, actualClosingStructures.Count);
            for (int i = 0; i < expectedClosingStructures.Count; i++)
            {
                AssertClosingStructure(expectedClosingStructures[i], actualClosingStructures[i]);
            }
        }

        private static void AssertClosingStructure(ClosingStructure expectedClosingStructure,
                                                   ClosingStructure actualClosingStructure)
        {
            Assert.AreEqual(expectedClosingStructure.Name, actualClosingStructure.Name);
            Assert.AreEqual(expectedClosingStructure.Id, actualClosingStructure.Id);
            Assert.AreEqual(expectedClosingStructure.Location, actualClosingStructure.Location);
            Assert.AreEqual(expectedClosingStructure.StructureNormalOrientation, actualClosingStructure.StructureNormalOrientation);

            DistributionAssert.AreEqual(expectedClosingStructure.StorageStructureArea, actualClosingStructure.StorageStructureArea);
            DistributionAssert.AreEqual(expectedClosingStructure.AllowedLevelIncreaseStorage, actualClosingStructure.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedClosingStructure.WidthFlowApertures, actualClosingStructure.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedClosingStructure.LevelCrestStructureNotClosing, actualClosingStructure.LevelCrestStructureNotClosing);
            DistributionAssert.AreEqual(expectedClosingStructure.InsideWaterLevel, actualClosingStructure.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedClosingStructure.ThresholdHeightOpenWeir, actualClosingStructure.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedClosingStructure.AreaFlowApertures, actualClosingStructure.AreaFlowApertures);
            DistributionAssert.AreEqual(expectedClosingStructure.CriticalOvertoppingDischarge, actualClosingStructure.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedClosingStructure.FlowWidthAtBottomProtection, actualClosingStructure.FlowWidthAtBottomProtection);
            Assert.AreEqual(expectedClosingStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding, actualClosingStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            Assert.AreEqual(expectedClosingStructure.FailureProbabilityOpenStructure, actualClosingStructure.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedClosingStructure.IdenticalApertures, actualClosingStructure.IdenticalApertures);
            Assert.AreEqual(expectedClosingStructure.FailureProbabilityReparation, actualClosingStructure.FailureProbabilityReparation);
            Assert.AreEqual(expectedClosingStructure.InflowModelType, actualClosingStructure.InflowModelType);
        }

        private static void AssertStructuresCalculation(StructuresCalculation<ClosingStructuresInput> expectedCalculation,
                                                        StructuresCalculation<ClosingStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertClosingStructuresInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertProbabilityAssessmentOutput(expectedCalculation.Output, actualCalculation.Output);
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        private static void AssertClosingStructuresInput(ClosingStructuresInput expectedInput,
                                                         ClosingStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            if (expectedInput.Structure == null)
            {
                Assert.IsNull(actualInput.Structure);
            }
            else
            {
                AssertClosingStructure(expectedInput.Structure, actualInput.Structure);
            }

            Assert.AreEqual(expectedInput.InflowModelType, actualInput.InflowModelType);
            DistributionAssert.AreEqual(expectedInput.InsideWaterLevel, actualInput.InsideWaterLevel);
            Assert.AreEqual(expectedInput.DeviationWaveDirection, actualInput.DeviationWaveDirection);
            DistributionAssert.AreEqual(expectedInput.DrainCoefficient, actualInput.DrainCoefficient);
            Assert.AreEqual(expectedInput.FactorStormDurationOpenStructure, actualInput.FactorStormDurationOpenStructure);
            DistributionAssert.AreEqual(expectedInput.ThresholdHeightOpenWeir, actualInput.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedInput.AreaFlowApertures, actualInput.AreaFlowApertures);
            Assert.AreEqual(expectedInput.FailureProbabilityOpenStructure, actualInput.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedInput.FailureProbabilityReparation, actualInput.FailureProbabilityReparation);
            Assert.AreEqual(expectedInput.IdenticalApertures, actualInput.IdenticalApertures);
            DistributionAssert.AreEqual(expectedInput.LevelCrestStructureNotClosing, actualInput.LevelCrestStructureNotClosing);
            Assert.AreEqual(expectedInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding, actualInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
        }

        #endregion

        #region DuneErosion FailureMechanism

        private static void AssertDuneErosionFailureMechanism(DuneErosionFailureMechanism expectedFailureMechanism,
                                                              DuneErosionFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertDuneLocations(expectedFailureMechanism.DuneLocations, actualFailureMechanism.DuneLocations);

            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<DuneErosionFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<DuneErosionFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertDuneLocations(List<DuneLocation> expectedDuneLocations,
                                                List<DuneLocation> actualDuneLocations)
        {
            Assert.AreEqual(expectedDuneLocations.Count, actualDuneLocations.Count);
            for (var i = 0; i < expectedDuneLocations.Count; i++)
            {
                DuneLocation expectedLocation = expectedDuneLocations[i];
                DuneLocation actualLocation = actualDuneLocations[i];

                AssertDuneBoundaryLocation(expectedLocation, actualLocation);
            }
        }

        private static void AssertDuneBoundaryLocation(DuneLocation expectedLocation, DuneLocation actualLocation)
        {
            Assert.AreEqual(expectedLocation.Id, actualLocation.Id);
            Assert.AreEqual(expectedLocation.Name, actualLocation.Name);
            Assert.AreEqual(expectedLocation.Location, actualLocation.Location);
            Assert.AreEqual(expectedLocation.CoastalAreaId, actualLocation.CoastalAreaId);
            Assert.AreEqual(expectedLocation.Offset, actualLocation.Offset);
            Assert.AreEqual(expectedLocation.Orientation, actualLocation.Orientation);
            Assert.AreEqual(expectedLocation.D50, actualLocation.D50);
            AssertDuneBoundaryLocationOutput(expectedLocation.Output, actualLocation.Output);
        }

        private static void AssertDuneBoundaryLocationOutput(DuneLocationOutput expectedOutput, DuneLocationOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }
            Assert.AreEqual(expectedOutput.WaterLevel, actualOutput.WaterLevel);
            Assert.AreEqual(expectedOutput.WaveHeight, actualOutput.WaveHeight);
            Assert.AreEqual(expectedOutput.WavePeriod, actualOutput.WavePeriod);
            Assert.AreEqual(expectedOutput.TargetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(expectedOutput.TargetReliability, actualOutput.TargetReliability);
            Assert.AreEqual(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability);
            Assert.AreEqual(expectedOutput.CalculationConvergence, actualOutput.CalculationConvergence);
        }

        #endregion

        #region HeightStructures FailureMechanism

        private static void AssertHeightStructuresFailureMechanism(HeightStructuresFailureMechanism expectedFailureMechanism,
                                                                   HeightStructuresFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertHeightStructures(expectedFailureMechanism.HeightStructures, actualFailureMechanism.HeightStructures);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<HeightStructuresFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<HeightStructuresFailureMechanismSectionResult> actualSectionResults)
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
                if (expectedSection.Calculation == null)
                {
                    Assert.IsNull(actualSection.Calculation);
                }
                else
                {
                    AssertStructuresCalculation(expectedSection.Calculation, actualSection.Calculation);
                }
            }
        }

        private static void AssertHeightStructures(ObservableList<HeightStructure> expectedHeightStructures,
                                                   ObservableList<HeightStructure> actualHeightStructures)
        {
            Assert.AreEqual(expectedHeightStructures.Count, actualHeightStructures.Count);
            for (int i = 0; i < expectedHeightStructures.Count; i++)
            {
                AssertHeightStructure(expectedHeightStructures[i], actualHeightStructures[i]);
            }
        }

        private static void AssertHeightStructure(HeightStructure expectedHeightStructure,
                                                  HeightStructure actualHeightStructure)
        {
            Assert.AreEqual(expectedHeightStructure.Name, actualHeightStructure.Name);
            Assert.AreEqual(expectedHeightStructure.Id, actualHeightStructure.Id);
            Assert.AreEqual(expectedHeightStructure.Location, actualHeightStructure.Location);
            Assert.AreEqual(expectedHeightStructure.StructureNormalOrientation, actualHeightStructure.StructureNormalOrientation);

            DistributionAssert.AreEqual(expectedHeightStructure.LevelCrestStructure, actualHeightStructure.LevelCrestStructure);
            DistributionAssert.AreEqual(expectedHeightStructure.FlowWidthAtBottomProtection, actualHeightStructure.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedHeightStructure.CriticalOvertoppingDischarge, actualHeightStructure.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedHeightStructure.WidthFlowApertures, actualHeightStructure.WidthFlowApertures);
            Assert.AreEqual(expectedHeightStructure.FailureProbabilityStructureWithErosion, actualHeightStructure.FailureProbabilityStructureWithErosion);
            DistributionAssert.AreEqual(expectedHeightStructure.StorageStructureArea, actualHeightStructure.StorageStructureArea);
            DistributionAssert.AreEqual(expectedHeightStructure.AllowedLevelIncreaseStorage, actualHeightStructure.AllowedLevelIncreaseStorage);
        }

        private static void AssertStructuresCalculation(StructuresCalculation<HeightStructuresInput> expectedCalculation,
                                                        StructuresCalculation<HeightStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertHeightStructuresInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertProbabilityAssessmentOutput(expectedCalculation.Output, actualCalculation.Output);
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        private static void AssertHeightStructuresInput(HeightStructuresInput expectedInput,
                                                        HeightStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            if (expectedInput.Structure == null)
            {
                Assert.IsNull(actualInput.Structure);
            }
            else
            {
                AssertHeightStructure(expectedInput.Structure, actualInput.Structure);
            }

            Assert.AreEqual(expectedInput.DeviationWaveDirection, actualInput.DeviationWaveDirection);
            DistributionAssert.AreEqual(expectedInput.LevelCrestStructure, actualInput.LevelCrestStructure);
        }

        #endregion

        #region Piping FailureMechanism

        private static void AssertPipingFailureMechanism(PipingFailureMechanism expectedPipingFailureMechanism,
                                                         PipingFailureMechanism actualPipingFailureMechanism)
        {
            AssertProbabilityAssessmentInput(expectedPipingFailureMechanism.PipingProbabilityAssessmentInput, actualPipingFailureMechanism.PipingProbabilityAssessmentInput);
            AssertStochasticSoilModels(expectedPipingFailureMechanism.StochasticSoilModels, actualPipingFailureMechanism.StochasticSoilModels);
            AssertSurfaceLines(expectedPipingFailureMechanism.SurfaceLines, actualPipingFailureMechanism.SurfaceLines);
            AssertCalculationGroup(expectedPipingFailureMechanism.CalculationsGroup, actualPipingFailureMechanism.CalculationsGroup);
            AssertComments(expectedPipingFailureMechanism.InputComments, actualPipingFailureMechanism.InputComments);
            AssertComments(expectedPipingFailureMechanism.OutputComments, actualPipingFailureMechanism.OutputComments);
            AssertComments(expectedPipingFailureMechanism.NotRelevantComments, actualPipingFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(
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

        private static void AssertProbabilityAssessmentInput(PipingProbabilityAssessmentInput expectedModel,
                                                             PipingProbabilityAssessmentInput actualModel)
        {
            Assert.AreEqual(expectedModel.A, actualModel.A);
        }

        private static void AssertPipingCalculationScenario(PipingCalculationScenario expectedPipingCalculation,
                                                            PipingCalculationScenario actualPipingCalculation)
        {
            Assert.AreEqual(expectedPipingCalculation.IsRelevant, actualPipingCalculation.IsRelevant);
            Assert.AreEqual(expectedPipingCalculation.Contribution, actualPipingCalculation.Contribution);
            Assert.AreEqual(expectedPipingCalculation.Name, actualPipingCalculation.Name);
            AssertComments(expectedPipingCalculation.Comments, actualPipingCalculation.Comments);

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
            Assert.AreEqual(expectedPipingInput.UseAssessmentLevelManualInput, actualPipingInput.UseAssessmentLevelManualInput);
            Assert.AreEqual(expectedPipingInput.AssessmentLevel, actualPipingInput.AssessmentLevel);
        }

        private static void AssertPipingOutput(PipingOutput expectedOutput, PipingOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
            }
            else
            {
                Assert.AreEqual(expectedOutput.HeaveGradient, actualOutput.HeaveGradient);
                Assert.AreEqual(expectedOutput.HeaveFactorOfSafety, actualOutput.HeaveFactorOfSafety);
                Assert.AreEqual(expectedOutput.HeaveZValue, actualOutput.HeaveZValue);
                Assert.AreEqual(expectedOutput.UpliftEffectiveStress, actualOutput.UpliftEffectiveStress);
                Assert.AreEqual(expectedOutput.UpliftFactorOfSafety, actualOutput.UpliftFactorOfSafety);
                Assert.AreEqual(expectedOutput.UpliftZValue, actualOutput.UpliftZValue);
                Assert.AreEqual(expectedOutput.SellmeijerCreepCoefficient, actualOutput.SellmeijerCreepCoefficient);
                Assert.AreEqual(expectedOutput.SellmeijerCriticalFall, actualOutput.SellmeijerCriticalFall);
                Assert.AreEqual(expectedOutput.SellmeijerReducedFall, actualOutput.SellmeijerReducedFall);
                Assert.AreEqual(expectedOutput.SellmeijerFactorOfSafety, actualOutput.SellmeijerFactorOfSafety);
                Assert.AreEqual(expectedOutput.SellmeijerZValue, actualOutput.SellmeijerZValue);
            }
        }

        private static void AssertPipingSemiProbabilisticOutput(PipingSemiProbabilisticOutput expectedOutput,
                                                                PipingSemiProbabilisticOutput actualOutput)
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

        #endregion

        #region GrassCoverErosionInwards FailureMechanism

        private static void AssertGrassCoverErosionInwardsFailureMechanism(GrassCoverErosionInwardsFailureMechanism expectedFailureMechanism,
                                                                           GrassCoverErosionInwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
            AssertDikeProfiles(expectedFailureMechanism.DikeProfiles, actualFailureMechanism.DikeProfiles);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> actualSectionResults)
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
                if (expectedSection.Calculation == null)
                {
                    Assert.IsNull(actualSection.Calculation);
                }
                else
                {
                    AssertGrassCoverErosionInwardsCalculation(expectedSection.Calculation, actualSection.Calculation);
                }
            }
        }

        private static void AssertGrassCoverErosionInwardsCalculation(GrassCoverErosionInwardsCalculation expectedCalculation,
                                                                      GrassCoverErosionInwardsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertGrassCoverErosionInwardsInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertGrassCoverErosionInwardsOutput(expectedCalculation.Output, actualCalculation.Output);
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        private static void AssertGrassCoverErosionInwardsInput(GrassCoverErosionInwardsInput expectedInput,
                                                                GrassCoverErosionInwardsInput actualInput)
        {
            if (expectedInput.DikeProfile == null)
            {
                Assert.IsNull(actualInput.DikeProfile);
            }
            else
            {
                AssertDikeProfile(expectedInput.DikeProfile, actualInput.DikeProfile);
            }
            if (expectedInput.HydraulicBoundaryLocation == null)
            {
                Assert.IsNull(actualInput.HydraulicBoundaryLocation);
            }
            else
            {
                AssertHydraulicBoundaryLocation(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
            }
            AssertBreakWater(expectedInput.BreakWater, actualInput.BreakWater);
            Assert.AreEqual(expectedInput.Orientation, actualInput.Orientation);
            Assert.AreEqual(expectedInput.UseBreakWater, actualInput.UseBreakWater);
            Assert.AreEqual(expectedInput.UseForeshore, actualInput.UseForeshore);
            Assert.AreEqual(expectedInput.DikeHeight, actualInput.DikeHeight);
            Assert.AreEqual(expectedInput.CriticalFlowRate.Mean, actualInput.CriticalFlowRate.Mean);
            Assert.AreEqual(expectedInput.CriticalFlowRate.StandardDeviation, actualInput.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(expectedInput.DikeHeightCalculationType, actualInput.DikeHeightCalculationType);
        }

        private static void AssertGrassCoverErosionInwardsOutput(GrassCoverErosionInwardsOutput expectedOutput,
                                                                 GrassCoverErosionInwardsOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.WaveHeight, actualOutput.WaveHeight);
            Assert.AreEqual(expectedOutput.IsOvertoppingDominant, actualOutput.IsOvertoppingDominant);
            AssertProbabilityAssessmentOutput(expectedOutput.ProbabilityAssessmentOutput, actualOutput.ProbabilityAssessmentOutput);
            AssertDikeHeightAssessmentOutput(expectedOutput.DikeHeightAssessmentOutput, actualOutput.DikeHeightAssessmentOutput);
        }

        private static void AssertDikeHeightAssessmentOutput(DikeHeightAssessmentOutput expectedOutput,
                                                             DikeHeightAssessmentOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }
            Assert.AreEqual(expectedOutput.DikeHeight, actualOutput.DikeHeight);
            Assert.AreEqual(expectedOutput.TargetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(expectedOutput.TargetReliability, actualOutput.TargetReliability);
            Assert.AreEqual(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability);
        }

        #endregion

        #region GrassCoverErosionOutwards FailureMechanism

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(GrassCoverErosionOutwardsFailureMechanism expectedFailureMechanism,
                                                                            GrassCoverErosionOutwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertHydraulicBoundaryLocations(expectedFailureMechanism.HydraulicBoundaryLocations, actualFailureMechanism.HydraulicBoundaryLocations);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertGrassCoverErosionOutwardsWaveConditionsCalculation(GrassCoverErosionOutwardsWaveConditionsCalculation expectedCalculation,
                                                                                     GrassCoverErosionOutwardsWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertWaveConditionsInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertWaveConditionsOutputs(expectedCalculation.Output.Items.ToArray(), actualCalculation.Output.Items.ToArray());
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        #endregion

        #region StabilityStoneCover FailureMechanism

        private static void AssertStabilityStoneCoverFailureMechanism(StabilityStoneCoverFailureMechanism expectedFailureMechanism,
                                                                      StabilityStoneCoverFailureMechanism actualFailureMechanism)
        {
            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertStabilityStoneCoverWaveConditionsCalculation(StabilityStoneCoverWaveConditionsCalculation expectedCalculation,
                                                                               StabilityStoneCoverWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertWaveConditionsInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertWaveConditionsOutputs(expectedCalculation.Output.BlocksOutput.ToArray(), actualCalculation.Output.BlocksOutput.ToArray());
                AssertWaveConditionsOutputs(expectedCalculation.Output.ColumnsOutput.ToArray(), actualCalculation.Output.ColumnsOutput.ToArray());
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        #endregion

        #region WaveImpactAsphaltCover FailureMechanism

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(WaveImpactAsphaltCoverFailureMechanism expectedFailureMechanism,
                                                                         WaveImpactAsphaltCoverFailureMechanism actualFailureMechanism)
        {
            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
            AssertComments(expectedFailureMechanism.InputComments, actualFailureMechanism.InputComments);
            AssertComments(expectedFailureMechanism.OutputComments, actualFailureMechanism.OutputComments);
            AssertComments(expectedFailureMechanism.NotRelevantComments, actualFailureMechanism.NotRelevantComments);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> actualSectionResults)
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

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculation(WaveImpactAsphaltCoverWaveConditionsCalculation expectedCalculation,
                                                                                  WaveImpactAsphaltCoverWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertWaveConditionsInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertWaveConditionsOutputs(expectedCalculation.Output.Items.ToArray(), actualCalculation.Output.Items.ToArray());
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        #endregion

        #region Hydraulic Boundary Database

        private static void AssertHydraulicBoundaryDatabase(HydraulicBoundaryDatabase expectedBoundaryDatabase,
                                                            HydraulicBoundaryDatabase actualBoundaryDatabase)
        {
            Assert.IsNotNull(expectedBoundaryDatabase);
            Assert.AreEqual(expectedBoundaryDatabase.Version, actualBoundaryDatabase.Version);
            Assert.AreEqual(expectedBoundaryDatabase.FilePath, actualBoundaryDatabase.FilePath);
            Assert.AreEqual(expectedBoundaryDatabase.Locations.Count, actualBoundaryDatabase.Locations.Count);

            AssertHydraulicBoundaryLocations(expectedBoundaryDatabase.Locations, actualBoundaryDatabase.Locations);
        }

        private static void AssertHydraulicBoundaryLocations(List<HydraulicBoundaryLocation> expectedHydraulicBoundaryLocations,
                                                             List<HydraulicBoundaryLocation> actualHydraulicBoundaryLocations)
        {
            Assert.AreEqual(expectedHydraulicBoundaryLocations.Count, actualHydraulicBoundaryLocations.Count);
            for (int i = 0; i < expectedHydraulicBoundaryLocations.Count; i++)
            {
                HydraulicBoundaryLocation expectedBoundaryLocation = expectedHydraulicBoundaryLocations[i];
                HydraulicBoundaryLocation actualBoundaryLocation = actualHydraulicBoundaryLocations[i];

                AssertHydraulicBoundaryLocation(expectedBoundaryLocation, actualBoundaryLocation);
            }
        }

        private static void AssertHydraulicBoundaryLocation(HydraulicBoundaryLocation expectedBoundaryLocation,
                                                            HydraulicBoundaryLocation actualBoundaryLocation)
        {
            Assert.AreEqual(expectedBoundaryLocation.Id, actualBoundaryLocation.Id);
            Assert.AreEqual(expectedBoundaryLocation.Name, actualBoundaryLocation.Name);
            Assert.AreEqual(expectedBoundaryLocation.Location, actualBoundaryLocation.Location);

            AssertHydraulicBoundaryLocationOutput(expectedBoundaryLocation.DesignWaterLevelOutput,
                                                  actualBoundaryLocation.DesignWaterLevelOutput);
            AssertHydraulicBoundaryLocationOutput(expectedBoundaryLocation.WaveHeightOutput,
                                                  actualBoundaryLocation.WaveHeightOutput);
        }

        private static void AssertHydraulicBoundaryLocationOutput(HydraulicBoundaryLocationOutput expectedOutput,
                                                                  HydraulicBoundaryLocationOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }
            Assert.AreEqual(expectedOutput.Result, actualOutput.Result);
            Assert.AreEqual(expectedOutput.TargetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(expectedOutput.TargetReliability, actualOutput.TargetReliability);
            Assert.AreEqual(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, actualOutput.CalculationConvergence);
        }

        #endregion

        #region BackgroundMapDataContainer

        private static void AssertBackgroundMapDataContainer(BackgroundMapDataContainer expectedContainer, BackgroundMapDataContainer actualContainer)
        {
            Assert.AreEqual(expectedContainer.IsVisible, actualContainer.IsVisible);
            Assert.AreEqual(expectedContainer.Transparency, actualContainer.Transparency);

            var expectedWmtsMapData = (WmtsMapData) expectedContainer.MapData;
            var actualWmtsMapData = (WmtsMapData) actualContainer.MapData;
            AssertWmtsMapData(expectedWmtsMapData, actualWmtsMapData);
        }

        private static void AssertWmtsMapData(WmtsMapData expectedWmtsMapData, WmtsMapData actualWmtsMapData)
        {
            Assert.AreEqual(expectedWmtsMapData.Name, actualWmtsMapData.Name);
            Assert.AreEqual(expectedWmtsMapData.IsVisible, actualWmtsMapData.IsVisible);
            Assert.AreEqual(expectedWmtsMapData.Transparency, actualWmtsMapData.Transparency);

            Assert.AreEqual(expectedWmtsMapData.IsConfigured, actualWmtsMapData.IsConfigured);
            Assert.AreEqual(expectedWmtsMapData.SourceCapabilitiesUrl, actualWmtsMapData.SourceCapabilitiesUrl);
            Assert.AreEqual(expectedWmtsMapData.SelectedCapabilityIdentifier, actualWmtsMapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(expectedWmtsMapData.PreferredFormat, actualWmtsMapData.PreferredFormat);
        }

        #endregion
    }
}