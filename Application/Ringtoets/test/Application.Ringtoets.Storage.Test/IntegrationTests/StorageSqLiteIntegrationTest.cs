// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using StochasticSoilModelCollection = Ringtoets.Piping.Data.StochasticSoilModelCollection;
using StochasticSoilProfile = Ringtoets.Piping.Data.StochasticSoilProfile;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageSqLiteIntegrationTest
    {
        private const string tempExtension = ".temp";
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        public void SaveProjectAs_DuplicateItemsInProjectSaveAsNewFile_ProjectAsEntitiesInFile()
        {
            // Setup
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            RingtoetsProject duplicateProject = RingtoetsProjectTestHelper.GetFullTestProject();

            AssessmentSection duplicateAssessmentSection = duplicateProject.AssessmentSections.First();
            fullProject.AssessmentSections.Add(duplicateAssessmentSection);

            string ringtoetsFile = GetRandomRingtoetsFile();

            var storage = new StorageSqLite();
            storage.StageProject(fullProject);

            // Call
            storage.SaveProjectAs(ringtoetsFile);
            var firstProject = (RingtoetsProject) storage.LoadProject(ringtoetsFile);

            // Assert
            AssertProjectsAreEqual(fullProject, firstProject);
        }

        [Test]
        public void SaveProjectAs_SaveAsNewFile_ProjectAsEntitiesInBothFiles()
        {
            // Setup
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            string firstRingtoetsFile = GetRandomRingtoetsFile();
            string secondRingtoetsFile = GetRandomRingtoetsFile();

            var storage = new StorageSqLite();
            storage.StageProject(fullProject);
            storage.SaveProjectAs(firstRingtoetsFile);

            // Call
            storage.StageProject(fullProject);
            storage.SaveProjectAs(secondRingtoetsFile);
            var firstProject = (RingtoetsProject) storage.LoadProject(firstRingtoetsFile);
            var secondProject = (RingtoetsProject) storage.LoadProject(secondRingtoetsFile);

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
            ProjectEntity entityBeforeSave = fullProject.Create(new PersistenceRegistry());

            byte[] hash1 = FingerprintHelper.Get(entityBeforeSave);

            var storage = new StorageSqLite();
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
            var storage = new StorageSqLite();
            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            storage.StageProject(fullProject);
            string tempRingtoetsFile = GetRandomRingtoetsFile();
            storage.SaveProjectAs(tempRingtoetsFile);

            // Call
            var loadedProject = (RingtoetsProject) storage.LoadProject(tempRingtoetsFile);

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
            string expectedProjectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);

            var mocks = new MockRepository();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(tempRingtoetsFile)).Return(MigrationRequired.No);
            mocks.ReplayAll();

            var projectStore = new StorageSqLite();

            RingtoetsProject fullProject = RingtoetsProjectTestHelper.GetFullTestProject();
            string expectedProjectDescription = fullProject.Description;

            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, fullProject);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                // When
                Action action = () => gui.Run(tempRingtoetsFile);

                // Then
                var expectedMessages = new[]
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(action, expectedMessages, 3);
                Assert.AreEqual(tempRingtoetsFile, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(expectedProjectDescription, gui.Project.Description);

                Assert.IsInstanceOf<RingtoetsProject>(gui.Project);
                AssertProjectsAreEqual(fullProject, (RingtoetsProject) gui.Project);
            }

            mocks.VerifyAll();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteIntegrationTest));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directoryDisposeHelper.Dispose();
        }

        private static string GetRandomRingtoetsFile()
        {
            return Path.Combine(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteIntegrationTest),
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

                BackgroundDataTestHelper.AssertBackgroundData(expectedAssessmentSection.BackgroundData, actualAssessmentSection.BackgroundData);
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
                    expectedAssessmentSection.MacroStabilityInwards.SectionResults,
                    actualAssessmentSection.MacroStabilityInwards.SectionResults);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<TechnicalInnovationFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<TechnicalInnovationFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MicrostabilityFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MicrostabilityFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<PipingStructureFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<PipingStructureFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults, (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
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
            AssertCollectionAndItems(expectedSections,
                                     actualSections,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.Name, actualItem.Name);
                                         CollectionAssert.AreEqual(expectedItem.Points, actualItem.Points);
                                     });
        }

        private static void AssertReferenceLine(ReferenceLine expectedReferenceLine, ReferenceLine actualReferenceLine)
        {
            Assert.IsNotNull(expectedReferenceLine);

            CollectionAssert.AreEqual(expectedReferenceLine.Points, actualReferenceLine.Points);
        }

        private static void AssertStochasticSoilModels(StochasticSoilModelCollection expectedModels,
                                                       StochasticSoilModelCollection actualModels)
        {
            Assert.Less(0, actualModels.Count);

            Assert.AreEqual(expectedModels.SourcePath, actualModels.SourcePath);
            AssertCollectionAndItems(expectedModels, actualModels, (expectedItem, actualItem) =>
            {
                Assert.AreEqual(expectedItem.Name, actualItem.Name);
                AssertSegmentPoints(expectedItem.Geometry, actualItem.Geometry);
                AssertStochasticSoilProfiles(expectedItem.StochasticSoilProfiles, actualItem.StochasticSoilProfiles);
            });
        }

        private static void AssertSegmentPoints(IEnumerable<Point2D> expectedSoilModelSegmentPoints,
                                                IEnumerable<Point2D> actualSoilModelSegmentPoints)
        {
            Assert.Greater(expectedSoilModelSegmentPoints.Count(), 0);

            CollectionAssert.AreEqual(expectedSoilModelSegmentPoints, actualSoilModelSegmentPoints);
        }

        private static void AssertStochasticSoilProfiles(IEnumerable<StochasticSoilProfile> expectedStochasticSoilProfiles,
                                                         IEnumerable<StochasticSoilProfile> actualStochasticSoilProfiles)
        {
            Assert.Less(0, actualStochasticSoilProfiles.Count());

            AssertCollectionAndItems(expectedStochasticSoilProfiles,
                                     actualStochasticSoilProfiles,
                                     AssertPipingSoilProfile);
        }

        private static void AssertPipingSoilProfile(StochasticSoilProfile expectedProfile, StochasticSoilProfile actualProfile)
        {
            Assert.AreEqual(expectedProfile.Probability, actualProfile.Probability);
            Assert.AreEqual(expectedProfile.SoilProfile.Bottom, actualProfile.SoilProfile.Bottom);
            Assert.AreEqual(expectedProfile.SoilProfile.Name, actualProfile.SoilProfile.Name);
            AssertSoilLayers(expectedProfile.SoilProfile.Layers, actualProfile.SoilProfile.Layers);
        }

        private static void AssertSoilLayers(IEnumerable<PipingSoilLayer> expectedLayers, IEnumerable<PipingSoilLayer> actualLayers)
        {
            AssertCollectionAndItems(expectedLayers,
                                     actualLayers,
                                     AssertPipingSoilLayer);
        }

        private static void AssertPipingSoilLayer(PipingSoilLayer expectedLayer, PipingSoilLayer actualLayer)
        {
            Assert.AreEqual(expectedLayer.Top, actualLayer.Top);
            Assert.AreEqual(expectedLayer.IsAquifer, actualLayer.IsAquifer);
            Assert.AreEqual(expectedLayer.BelowPhreaticLevelMean, actualLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(expectedLayer.BelowPhreaticLevelDeviation, actualLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(expectedLayer.BelowPhreaticLevelShift, actualLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(expectedLayer.DiameterD70Mean, actualLayer.DiameterD70Mean);
            Assert.AreEqual(expectedLayer.DiameterD70CoefficientOfVariation, actualLayer.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(expectedLayer.PermeabilityMean, actualLayer.PermeabilityMean);
            Assert.AreEqual(expectedLayer.PermeabilityCoefficientOfVariation, actualLayer.PermeabilityCoefficientOfVariation);
        }

        private static void AssertSurfaceLines(PipingSurfaceLineCollection expectedSurfaceLines,
                                               PipingSurfaceLineCollection actualSurfaceLines)
        {
            Assert.Greater(expectedSurfaceLines.Count, 0);

            Assert.AreEqual(expectedSurfaceLines.SourcePath, actualSurfaceLines.SourcePath);
            AssertCollectionAndItems(expectedSurfaceLines,
                                     actualSurfaceLines,
                                     AssertSurfaceLine);
        }

        private static void AssertSurfaceLine(PipingSurfaceLine expectedSurfaceLine,
                                              PipingSurfaceLine actualSurfaceLine)
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
            for (var i = 0; i < expectedRootCalculationGroup.Children.Count; i++)
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

        private static void AssertWaveConditionsOutputs(IEnumerable<WaveConditionsOutput> expectedOutputs,
                                                        IEnumerable<WaveConditionsOutput> actualOutputs)
        {
            AssertCollectionAndItems(expectedOutputs,
                                     actualOutputs,
                                     AssertWaveConditionsOutput);
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

        private static void AssertDikeProfiles(DikeProfileCollection expectedDikeProfiles, DikeProfileCollection actualDikeProfiles)
        {
            Assert.AreEqual(expectedDikeProfiles.SourcePath, actualDikeProfiles.SourcePath);
            AssertCollectionAndItems(expectedDikeProfiles,
                                     actualDikeProfiles,
                                     AssertDikeProfile);
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

        private static void AssertForeshoreProfiles(ForeshoreProfileCollection expectedForeshoreProfiles,
                                                    ForeshoreProfileCollection actualForeshoreProfiles)
        {
            Assert.AreEqual(expectedForeshoreProfiles.SourcePath, actualForeshoreProfiles.SourcePath);
            AssertCollectionAndItems(expectedForeshoreProfiles,
                                     actualForeshoreProfiles,
                                     AssertForeshoreProfile);
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

        private static void AssertRoughnessPoints(IEnumerable<RoughnessPoint> expectedRoughnessPoints,
                                                  IEnumerable<RoughnessPoint> actualRoughnessPoints)
        {
            AssertCollectionAndItems(expectedRoughnessPoints,
                                     actualRoughnessPoints,
                                     AssertRoughnessPoint);
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

        /// <summary>
        /// Asserts two collections with items of <typeparamref name="T"/> on the
        /// length of the collections and whether the items are equal according to 
        /// <paramref name="assertAction"/>.
        /// </summary>
        /// <typeparam name="T">The item type to assert.</typeparam>
        /// <param name="expectedCollection">The expected collection to assert
        /// against to.</param>
        /// <param name="actualCollection">The actual collection to assert.</param>
        /// <param name="assertAction">The action to compare items in the collection
        /// against each other.</param>
        /// <exception cref="AssertionException">Thrown when the collections are
        /// not of equal length or when the items within the collection are not equal.</exception>
        private static void AssertCollectionAndItems<T>(IEnumerable<T> expectedCollection,
                                                        IEnumerable<T> actualCollection,
                                                        Action<T, T> assertAction)
        {
            T[] expectedArray = expectedCollection.ToArray();
            T[] actualArray = actualCollection.ToArray();
            int expectedNrOfItems = expectedArray.Length;
            Assert.AreEqual(expectedNrOfItems, actualArray.Length);

            for (var i = 0; i < expectedNrOfItems; i++)
            {
                assertAction(expectedArray[i], actualArray[i]);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                         if (expectedItem.Calculation == null)
                                         {
                                             Assert.IsNull(actualItem.Calculation);
                                         }
                                         else
                                         {
                                             AssertStructuresCalculation(expectedItem.Calculation, actualItem.Calculation);
                                         }
                                     });
        }

        private static void AssertStabilityPointStructures(StructureCollection<StabilityPointStructure> expectedStabilityPointStructures,
                                                           StructureCollection<StabilityPointStructure> actualStabilityPointStructures)
        {
            Assert.AreEqual(expectedStabilityPointStructures.Count, actualStabilityPointStructures.Count);
            for (var i = 0; i < expectedStabilityPointStructures.Count; i++)
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

            if (!expectedCalculation.HasOutput)
            {
                Assert.IsFalse(actualCalculation.HasOutput);
                return;
            }

            StructuresOutput expectedOutput = expectedCalculation.Output;
            StructuresOutput actualOutput = actualCalculation.Output;
            Assert.IsNotNull(actualOutput);
            AssertProbabilityAssessmentOutput(expectedOutput.ProbabilityAssessmentOutput,
                                              actualOutput.ProbabilityAssessmentOutput);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                         if (expectedItem.Calculation == null)
                                         {
                                             Assert.IsNull(actualItem.Calculation);
                                         }
                                         else
                                         {
                                             AssertStructuresCalculation(expectedItem.Calculation, actualItem.Calculation);
                                         }
                                     });
        }

        private static void AssertClosingStructures(StructureCollection<ClosingStructure> expectedClosingStructures,
                                                    StructureCollection<ClosingStructure> actualClosingStructures)
        {
            Assert.AreEqual(expectedClosingStructures.SourcePath, actualClosingStructures.SourcePath);
            AssertCollectionAndItems(expectedClosingStructures,
                                     actualClosingStructures,
                                     AssertClosingStructure);
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

            if (!expectedCalculation.HasOutput)
            {
                Assert.IsFalse(actualCalculation.HasOutput);
                return;
            }

            StructuresOutput expectedOutput = expectedCalculation.Output;
            StructuresOutput actualOutput = actualCalculation.Output;
            Assert.IsNotNull(actualOutput);
            AssertProbabilityAssessmentOutput(expectedOutput.ProbabilityAssessmentOutput,
                                              actualOutput.ProbabilityAssessmentOutput);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
        }

        private static void AssertDuneLocations(IEnumerable<DuneLocation> expectedDuneLocations,
                                                IEnumerable<DuneLocation> actualDuneLocations)
        {
            AssertCollectionAndItems(expectedDuneLocations,
                                     actualDuneLocations,
                                     AssertDuneBoundaryLocation);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                         if (expectedItem.Calculation == null)
                                         {
                                             Assert.IsNull(actualItem.Calculation);
                                         }
                                         else
                                         {
                                             AssertStructuresCalculation(expectedItem.Calculation, actualItem.Calculation);
                                         }
                                     });
        }

        private static void AssertHeightStructures(StructureCollection<HeightStructure> expectedHeightStructures,
                                                   StructureCollection<HeightStructure> actualHeightStructures)
        {
            Assert.AreEqual(expectedHeightStructures.SourcePath, actualHeightStructures.SourcePath);
            AssertCollectionAndItems(expectedHeightStructures,
                                     actualHeightStructures,
                                     AssertHeightStructure);
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

            if (!expectedCalculation.HasOutput)
            {
                Assert.IsFalse(actualCalculation.HasOutput);
                return;
            }

            StructuresOutput expectedOutput = expectedCalculation.Output;
            StructuresOutput actualOutput = actualCalculation.Output;
            Assert.IsNotNull(actualOutput);
            AssertProbabilityAssessmentOutput(expectedOutput.ProbabilityAssessmentOutput,
                                              actualOutput.ProbabilityAssessmentOutput);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
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
            Assert.AreEqual(expectedPipingInput.Diameter70.CoefficientOfVariation, actualPipingInput.Diameter70.CoefficientOfVariation);
            Assert.AreEqual(expectedPipingInput.DarcyPermeability.Mean, actualPipingInput.DarcyPermeability.Mean);
            Assert.AreEqual(expectedPipingInput.DarcyPermeability.CoefficientOfVariation, actualPipingInput.DarcyPermeability.CoefficientOfVariation);
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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                         if (expectedItem.Calculation == null)
                                         {
                                             Assert.IsNull(expectedItem.Calculation);
                                         }
                                         else
                                         {
                                             AssertGrassCoverErosionInwardsCalculation(expectedItem.Calculation, actualItem.Calculation);
                                         }
                                     });
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
            Assert.AreEqual(expectedInput.OvertoppingRateCalculationType, actualInput.OvertoppingRateCalculationType);
        }

        private static void AssertGrassCoverErosionInwardsOutput(GrassCoverErosionInwardsOutput expectedOutput,
                                                                 GrassCoverErosionInwardsOutput actualOutput)
        {
            OvertoppingOutput expectedOvertoppingOutput = expectedOutput.OvertoppingOutput;
            OvertoppingOutput actualOvertoppingOutput = actualOutput.OvertoppingOutput;

            Assert.AreEqual(expectedOvertoppingOutput.WaveHeight, actualOvertoppingOutput.WaveHeight);
            Assert.AreEqual(expectedOvertoppingOutput.IsOvertoppingDominant, actualOvertoppingOutput.IsOvertoppingDominant);
            AssertProbabilityAssessmentOutput(expectedOvertoppingOutput.ProbabilityAssessmentOutput, actualOvertoppingOutput.ProbabilityAssessmentOutput);
            AssertDikeHeightOutput(expectedOutput.DikeHeightOutput, actualOutput.DikeHeightOutput);
            AssertOvertoppingRateOutput(expectedOutput.OvertoppingRateOutput, actualOutput.OvertoppingRateOutput);
        }

        private static void AssertDikeHeightOutput(DikeHeightOutput expectedOutput,
                                                   DikeHeightOutput actualOutput)
        {
            AssertHydraulicLoadsOutput(expectedOutput, actualOutput);

            Assert.AreEqual(expectedOutput.DikeHeight, actualOutput.DikeHeight);
        }

        private static void AssertOvertoppingRateOutput(OvertoppingRateOutput expectedOutput,
                                                        OvertoppingRateOutput actualOutput)
        {
            AssertHydraulicLoadsOutput(expectedOutput, actualOutput);

            Assert.AreEqual(expectedOutput.OvertoppingRate, actualOutput.OvertoppingRate);
        }

        private static void AssertHydraulicLoadsOutput(HydraulicLoadsOutput expectedOutput,
                                                       HydraulicLoadsOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }

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
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                                         Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                                         Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
                                     });
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
            StabilityStoneCoverFailureMechanismSectionResult[] expectedSectionResultsArray = expectedSectionResults.ToArray();
            StabilityStoneCoverFailureMechanismSectionResult[] actualSectionResultsArray = actualSectionResults.ToArray();

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
            AssertCollectionAndItems(expectedSectionResults, actualSectionResults, (expectedItem, actualItem) =>
            {
                Assert.AreEqual(expectedItem.AssessmentLayerOne, actualItem.AssessmentLayerOne);
                Assert.AreEqual(expectedItem.AssessmentLayerTwoA, actualItem.AssessmentLayerTwoA);
                Assert.AreEqual(expectedItem.AssessmentLayerThree, actualItem.AssessmentLayerThree);
            });
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

        private static void AssertHydraulicBoundaryLocations(IEnumerable<HydraulicBoundaryLocation> expectedHydraulicBoundaryLocations,
                                                             IEnumerable<HydraulicBoundaryLocation> actualHydraulicBoundaryLocations)
        {
            AssertCollectionAndItems(expectedHydraulicBoundaryLocations,
                                     actualHydraulicBoundaryLocations,
                                     AssertHydraulicBoundaryLocation);
        }

        private static void AssertHydraulicBoundaryLocation(HydraulicBoundaryLocation expectedBoundaryLocation,
                                                            HydraulicBoundaryLocation actualBoundaryLocation)
        {
            Assert.AreEqual(expectedBoundaryLocation.Id, actualBoundaryLocation.Id);
            Assert.AreEqual(expectedBoundaryLocation.Name, actualBoundaryLocation.Name);
            Assert.AreEqual(expectedBoundaryLocation.Location, actualBoundaryLocation.Location);

            HydraulicBoundaryLocationCalculation expectedDesignWaterLevelCalculation = expectedBoundaryLocation.DesignWaterLevelCalculation;
            HydraulicBoundaryLocationCalculation actualDesignWaterLevelCalculation = actualBoundaryLocation.DesignWaterLevelCalculation;
            Assert.AreEqual(expectedDesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated,
                            actualDesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationOutput(expectedDesignWaterLevelCalculation.Output,
                                                  actualDesignWaterLevelCalculation.Output);

            HydraulicBoundaryLocationCalculation expectedWaveHeightCalculation = expectedBoundaryLocation.WaveHeightCalculation;
            HydraulicBoundaryLocationCalculation actualWaveHeightCalculation = actualBoundaryLocation.WaveHeightCalculation;
            Assert.AreEqual(expectedWaveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated,
                            actualWaveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationOutput(expectedWaveHeightCalculation.Output,
                                                  actualWaveHeightCalculation.Output);
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
            Assert.AreEqual(expectedOutput.CalculationConvergence, actualOutput.CalculationConvergence);

            AssertGeneralResultTopLevelSubMechanismIllustrationPoint(expectedOutput.GeneralResult,
                                                                     actualOutput.GeneralResult);
        }

        #endregion

        #region IllustrationPoints

        private static void AssertGeneralResultTopLevelFaultTreeIllustrationPoint(GeneralResult<TopLevelFaultTreeIllustrationPoint> expected,
                                                                                  GeneralResult<TopLevelFaultTreeIllustrationPoint> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AssertWindDirection(expected.GoverningWindDirection,
                                actual.GoverningWindDirection);

            AssertCollectionAndItems(expected.Stochasts,
                                     actual.Stochasts,
                                     AssertStochast);

            AssertCollectionAndItems(expected.TopLevelIllustrationPoints,
                                     actual.TopLevelIllustrationPoints,
                                     AssertTopLevelFaultTreeIllustrationPoint);
        }

        private static void AssertTopLevelFaultTreeIllustrationPoint(TopLevelFaultTreeIllustrationPoint expected,
                                                                     TopLevelFaultTreeIllustrationPoint actual)
        {
            AssertWindDirection(expected.WindDirection, actual.WindDirection);

            Assert.AreEqual(expected.ClosingSituation, actual.ClosingSituation);

            AssertIllustrationPointNode(expected.FaultTreeNodeRoot, actual.FaultTreeNodeRoot);
        }

        private static void AssertIllustrationPointNode(IllustrationPointNode expected, IllustrationPointNode actual)
        {
            var expectedFaultTreeIllustrationPoint = expected.Data as FaultTreeIllustrationPoint;
            if (expectedFaultTreeIllustrationPoint != null)
            {
                var actualFaultTreeIllustrationPoint = actual.Data as FaultTreeIllustrationPoint;
                Assert.IsNotNull(actualFaultTreeIllustrationPoint);

                AssertFaultTreeIllustrationPoint(expectedFaultTreeIllustrationPoint,
                                                 actualFaultTreeIllustrationPoint);

                AssertCollectionAndItems(expected.Children,
                                         actual.Children,
                                         AssertIllustrationPointNode);
                return;
            }

            var expectedSubMechanismIllustrationPoint = expected.Data as SubMechanismIllustrationPoint;
            if (expectedSubMechanismIllustrationPoint != null)
            {
                var actualSubMechanismIllustrationPoint = actual.Data as SubMechanismIllustrationPoint;
                Assert.IsNotNull(actualSubMechanismIllustrationPoint);

                AssertSubMechanismIllustrationPoint(expectedSubMechanismIllustrationPoint,
                                                    actualSubMechanismIllustrationPoint);
            }
        }

        private static void AssertFaultTreeIllustrationPoint(FaultTreeIllustrationPoint expected,
                                                             FaultTreeIllustrationPoint actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Beta, actual.Beta);
            Assert.AreEqual(expected.CombinationType, actual.CombinationType);

            AssertCollectionAndItems(expected.Stochasts, actual.Stochasts, AssertStochast);
        }

        private static void AssertGeneralResultTopLevelSubMechanismIllustrationPoint(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> expectedGeneralResult,
            GeneralResult<TopLevelSubMechanismIllustrationPoint> actualGeneralResult)
        {
            if (expectedGeneralResult == null)
            {
                Assert.IsNull(actualGeneralResult);
                return;
            }

            AssertWindDirection(expectedGeneralResult.GoverningWindDirection,
                                actualGeneralResult.GoverningWindDirection);

            AssertCollectionAndItems(expectedGeneralResult.Stochasts,
                                     actualGeneralResult.Stochasts,
                                     AssertStochast);

            AssertCollectionAndItems(expectedGeneralResult.TopLevelIllustrationPoints,
                                     actualGeneralResult.TopLevelIllustrationPoints,
                                     AssertTopLevelSubMechanismIllustrationPoint);
        }

        private static void AssertWindDirection(WindDirection expectedWindDirection, WindDirection actualWindDirection)
        {
            Assert.AreEqual(expectedWindDirection.Name, actualWindDirection.Name);
            Assert.AreEqual(expectedWindDirection.Angle, actualWindDirection.Angle);
        }

        private static void AssertStochast(Stochast expectedStochast,
                                           Stochast actualStochast)
        {
            Assert.AreEqual(expectedStochast.Name, actualStochast.Name);
            Assert.AreEqual(expectedStochast.Alpha, actualStochast.Alpha);
            Assert.AreEqual(expectedStochast.Duration, actualStochast.Duration);
        }

        private static void AssertTopLevelSubMechanismIllustrationPoint(
            TopLevelSubMechanismIllustrationPoint expectedTopLevelSubMechanismIllustrationPoint,
            TopLevelSubMechanismIllustrationPoint actualTopLevelSubMechanismIllustrationPoint)
        {
            Assert.AreEqual(expectedTopLevelSubMechanismIllustrationPoint.ClosingSituation,
                            actualTopLevelSubMechanismIllustrationPoint.ClosingSituation);

            WindDirection expectedWindDirection = expectedTopLevelSubMechanismIllustrationPoint.WindDirection;
            WindDirection actualWindDirection = actualTopLevelSubMechanismIllustrationPoint.WindDirection;
            AssertWindDirection(expectedWindDirection, actualWindDirection);

            AssertSubMechanismIllustrationPoint(expectedTopLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint,
                                                actualTopLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint);
        }

        private static void AssertSubMechanismIllustrationPoint(SubMechanismIllustrationPoint expectedSubMechanismIllustrationPoint,
                                                                SubMechanismIllustrationPoint actualSubMechanismIllustrationPoint)
        {
            Assert.AreEqual(expectedSubMechanismIllustrationPoint.Name, actualSubMechanismIllustrationPoint.Name);
            Assert.AreEqual(expectedSubMechanismIllustrationPoint.Beta, actualSubMechanismIllustrationPoint.Beta);

            AssertCollectionAndItems(expectedSubMechanismIllustrationPoint.Stochasts, actualSubMechanismIllustrationPoint.Stochasts, AssertSubMechanismIllustrationPointStochast);
            AssertCollectionAndItems(expectedSubMechanismIllustrationPoint.IllustrationPointResults, actualSubMechanismIllustrationPoint.IllustrationPointResults, AssertIllustrationPointResult);
        }

        private static void AssertSubMechanismIllustrationPointStochast(SubMechanismIllustrationPointStochast expectedStochast,
                                                                        SubMechanismIllustrationPointStochast actualStochast)
        {
            AssertStochast(expectedStochast, actualStochast);
            Assert.AreEqual(expectedStochast.Realization, actualStochast.Realization);
        }

        private static void AssertIllustrationPointResult(IllustrationPointResult expectedResult,
                                                          IllustrationPointResult actualResult)
        {
            Assert.AreEqual(expectedResult.Description, actualResult.Description);
            Assert.AreEqual(expectedResult.Value, actualResult.Value);
        }

        #endregion
    }
}