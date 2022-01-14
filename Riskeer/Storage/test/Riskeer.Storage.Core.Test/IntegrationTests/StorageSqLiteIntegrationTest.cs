﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Settings;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.TestUtil;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Test.IntegrationTests
{
    [TestFixture]
    public class StorageSqLiteIntegrationTest
    {
        private const string tempExtension = ".temp";
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        public void SaveProjectAs_SaveAsNewFile_ProjectAsEntitiesInBothFiles()
        {
            // Setup
            RiskeerProject fullProject = RiskeerProjectTestHelper.GetFullTestProject();
            string firstProjectFilePath = GetRandomProjectFilePath();
            string secondProjectFilePath = GetRandomProjectFilePath();

            var storage = new StorageSqLite();
            storage.StageProject(fullProject);
            storage.SaveProjectAs(firstProjectFilePath);

            // Call
            storage.StageProject(fullProject);
            storage.SaveProjectAs(secondProjectFilePath);
            var firstProject = (RiskeerProject) storage.LoadProject(firstProjectFilePath);
            var secondProject = (RiskeerProject) storage.LoadProject(secondProjectFilePath);

            // Assert
            AssertProjectsAreEqual(firstProject, secondProject);
        }

        [Test]
        public void GivenProject_WhenComparingFingerPrintsVariousScenariosUnchangedData_ThenFingerprintUnchanged()
        {
            // Given
            RiskeerProject fullProject = RiskeerProjectTestHelper.GetFullTestProject();
            string projectFilePath = GetRandomProjectFilePath();

            // When
            ProjectEntity entityBeforeSave = fullProject.Create(new PersistenceRegistry());

            byte[] hash1 = FingerprintHelper.Get(entityBeforeSave);

            var storage = new StorageSqLite();
            storage.StageProject(fullProject);
            storage.SaveProjectAs(projectFilePath);

            ProjectEntity entityAfterSave = fullProject.Create(new PersistenceRegistry());
            byte[] hash2 = FingerprintHelper.Get(entityAfterSave);

            var openedProject = (RiskeerProject) storage.LoadProject(projectFilePath);
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
            RiskeerProject fullProject = RiskeerProjectTestHelper.GetFullTestProject();
            storage.StageProject(fullProject);
            string projectFilePath = GetRandomProjectFilePath();
            storage.SaveProjectAs(projectFilePath);

            // Call
            var loadedProject = (RiskeerProject) storage.LoadProject(projectFilePath);

            // Assert
            AssertProjectsAreEqual(fullProject, loadedProject);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGuiWithStorageSqlAndMigrator_WhenRunWithValidFile_ProjectSet()
        {
            // Given
            string projectFilePath = GetRandomProjectFilePath();
            string expectedProjectName = Path.GetFileNameWithoutExtension(projectFilePath);

            var mocks = new MockRepository();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(projectFilePath)).Return(MigrationRequired.No);
            mocks.ReplayAll();

            var projectStore = new StorageSqLite();

            RiskeerProject fullProject = RiskeerProjectTestHelper.GetFullTestProject();
            string expectedProjectDescription = fullProject.Description;

            SqLiteDatabaseHelper.CreateValidProjectDatabase(projectFilePath, fullProject);

            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = SystemIcons.Application
            };

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), guiCoreSettings))
            {
                // When
                void Action() => gui.Run(projectFilePath);

                // Then
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Action, expectedMessages, 3);
                Assert.AreEqual(projectFilePath, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(expectedProjectDescription, gui.Project.Description);

                Assert.IsInstanceOf<RiskeerProject>(gui.Project);
                AssertProjectsAreEqual(fullProject, (RiskeerProject) gui.Project);
            }

            mocks.VerifyAll();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteIntegrationTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directoryDisposeHelper.Dispose();
        }

        private static string GetRandomProjectFilePath()
        {
            return Path.Combine(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteIntegrationTest),
                                string.Concat(Path.GetRandomFileName(), tempExtension));
        }

        private static void AssertProjectsAreEqual(RiskeerProject expectedProject, RiskeerProject actualProject)
        {
            Assert.NotNull(expectedProject);
            Assert.NotNull(actualProject);
            Assert.AreNotSame(expectedProject, actualProject);
            Assert.AreEqual(expectedProject.Description, actualProject.Description);

            AssessmentSection expectedAssessmentSection = expectedProject.AssessmentSection;
            AssessmentSection actualAssessmentSection = actualProject.AssessmentSection;

            Assert.AreEqual(expectedAssessmentSection.Id, actualAssessmentSection.Id);
            Assert.AreEqual(expectedAssessmentSection.Name, actualAssessmentSection.Name);
            AssertComments(expectedAssessmentSection.Comments, actualAssessmentSection.Comments);

            FailureMechanismContribution expectedContribution = expectedAssessmentSection.FailureMechanismContribution;
            FailureMechanismContribution actualContribution = actualAssessmentSection.FailureMechanismContribution;
            Assert.AreEqual(expectedContribution.LowerLimitNorm, actualContribution.LowerLimitNorm);
            Assert.AreEqual(expectedContribution.SignalingNorm, actualContribution.SignalingNorm);
            Assert.AreEqual(expectedContribution.NormativeNorm, actualContribution.NormativeNorm);

            BackgroundDataTestHelper.AssertBackgroundData(expectedAssessmentSection.BackgroundData, actualAssessmentSection.BackgroundData);
            AssertHydraulicBoundaryDatabase(expectedAssessmentSection.HydraulicBoundaryDatabase, actualAssessmentSection.HydraulicBoundaryDatabase);
            AssertHydraulicBoundaryLocationCalculations(expectedAssessmentSection, actualAssessmentSection);
            AssertReferenceLine(expectedAssessmentSection.ReferenceLine, actualAssessmentSection.ReferenceLine);

            IFailureMechanism[] expectedProjectFailureMechanisms = expectedAssessmentSection.GetFailureMechanisms().ToArray();
            IFailureMechanism[] actualProjectFailureMechanisms = actualAssessmentSection.GetFailureMechanisms().ToArray();
            for (var fmi = 0; fmi < expectedProjectFailureMechanisms.Length; fmi++)
            {
                AssertFailureMechanism(expectedProjectFailureMechanisms[fmi], actualProjectFailureMechanisms[fmi]);
            }

            AssertPipingFailureMechanism(expectedAssessmentSection.Piping, actualAssessmentSection.Piping);
            AssertMacroStabilityOutwardsFailureMechanism(expectedAssessmentSection.MacroStabilityOutwards, actualAssessmentSection.MacroStabilityOutwards);
            AssertMacroStabilityInwardsFailureMechanism(expectedAssessmentSection.MacroStabilityInwards, actualAssessmentSection.MacroStabilityInwards);
            AssertGrassCoverErosionInwardsFailureMechanism(expectedAssessmentSection.GrassCoverErosionInwards, actualAssessmentSection.GrassCoverErosionInwards);
            AssertGrassCoverErosionOutwardsFailureMechanism(expectedAssessmentSection.GrassCoverErosionOutwards, actualAssessmentSection.GrassCoverErosionOutwards);
            AssertPipingStructureFailureMechanism(expectedAssessmentSection.PipingStructure, actualAssessmentSection.PipingStructure);
            AssertStabilityStoneCoverFailureMechanism(expectedAssessmentSection.StabilityStoneCover, actualAssessmentSection.StabilityStoneCover);
            AssertWaveImpactAsphaltCoverFailureMechanism(expectedAssessmentSection.WaveImpactAsphaltCover, actualAssessmentSection.WaveImpactAsphaltCover);
            AssertHeightStructuresFailureMechanism(expectedAssessmentSection.HeightStructures, actualAssessmentSection.HeightStructures);
            AssertClosingStructuresFailureMechanism(expectedAssessmentSection.ClosingStructures, actualAssessmentSection.ClosingStructures);
            AssertDuneErosionFailureMechanism(expectedAssessmentSection.DuneErosion, actualAssessmentSection.DuneErosion);
            AssertStabilityPointStructuresFailureMechanism(expectedAssessmentSection.StabilityPointStructures, actualAssessmentSection.StabilityPointStructures);

            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.Piping.SectionResults,
                actualAssessmentSection.Piping.SectionResults);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.GrassCoverErosionInwards.SectionResultsOld,
                actualAssessmentSection.GrassCoverErosionInwards.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.HeightStructures.SectionResults,
                actualAssessmentSection.HeightStructures.SectionResults);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.StrengthStabilityLengthwiseConstruction.SectionResultsOld,
                actualAssessmentSection.StrengthStabilityLengthwiseConstruction.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.TechnicalInnovation.SectionResultsOld,
                actualAssessmentSection.TechnicalInnovation.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.WaterPressureAsphaltCover.SectionResultsOld,
                actualAssessmentSection.WaterPressureAsphaltCover.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.ClosingStructures.SectionResults,
                actualAssessmentSection.ClosingStructures.SectionResults);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.MacroStabilityOutwards.SectionResultsOld,
                actualAssessmentSection.MacroStabilityOutwards.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.MacroStabilityInwards.SectionResults,
                actualAssessmentSection.MacroStabilityInwards.SectionResults);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.WaveImpactAsphaltCover.SectionResultsOld,
                actualAssessmentSection.WaveImpactAsphaltCover.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.GrassCoverErosionOutwards.SectionResultsOld,
                actualAssessmentSection.GrassCoverErosionOutwards.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.GrassCoverSlipOffInwards.SectionResultsOld,
                actualAssessmentSection.GrassCoverSlipOffInwards.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.GrassCoverSlipOffOutwards.SectionResultsOld,
                actualAssessmentSection.GrassCoverSlipOffOutwards.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.Microstability.SectionResultsOld,
                actualAssessmentSection.Microstability.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.PipingStructure.SectionResultsOld,
                actualAssessmentSection.PipingStructure.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.DuneErosion.SectionResultsOld,
                actualAssessmentSection.DuneErosion.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.StabilityStoneCover.SectionResultsOld,
                actualAssessmentSection.StabilityStoneCover.SectionResultsOld);
            AssertFailureMechanismSectionResults(
                expectedAssessmentSection.StabilityPointStructures.SectionResultsOld,
                actualAssessmentSection.StabilityPointStructures.SectionResultsOld);

            AssertCollectionAndItems(expectedAssessmentSection.SpecificFailurePaths.Cast<SpecificFailurePath>(),
                                     actualAssessmentSection.SpecificFailurePaths.Cast<SpecificFailurePath>(),
                                     AssertSpecificFailurePath);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<TechnicalInnovationFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<TechnicalInnovationFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MicrostabilityFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<MicrostabilityFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertFailureMechanism(IFailureMechanism expectedFailureMechanism,
                                                   IFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.Name, actualFailureMechanism.Name);
            Assert.AreEqual(expectedFailureMechanism.Code, actualFailureMechanism.Code);
            Assert.AreEqual(expectedFailureMechanism.InAssembly, actualFailureMechanism.InAssembly);
            AssertComments(expectedFailureMechanism.InAssemblyInputComments, actualFailureMechanism.InAssemblyInputComments);
            AssertComments(expectedFailureMechanism.InAssemblyOutputComments, actualFailureMechanism.InAssemblyOutputComments);
            AssertComments(expectedFailureMechanism.NotInAssemblyComments, actualFailureMechanism.NotInAssemblyComments);
            AssertComments(expectedFailureMechanism.CalculationsInputComments, actualFailureMechanism.CalculationsInputComments);
            AssertFailureMechanismSections(expectedFailureMechanism.Sections, actualFailureMechanism.Sections);
            Assert.AreEqual(expectedFailureMechanism.FailureMechanismSectionSourcePath, actualFailureMechanism.FailureMechanismSectionSourcePath);

            AssertFailurePathAssemblyResult(expectedFailureMechanism.AssemblyResult, actualFailureMechanism.AssemblyResult);
        }

        private static void AssertFailurePathAssemblyResult(FailurePathAssemblyResult expectedResult,
                                                            FailurePathAssemblyResult actualResult)
        {
            Assert.AreEqual(expectedResult.ProbabilityResultType, actualResult.ProbabilityResultType);
            Assert.AreEqual(expectedResult.ManualFailurePathAssemblyProbability, actualResult.ManualFailurePathAssemblyProbability);
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

        private static void AssertSegmentPoints(IEnumerable<Point2D> expectedSoilModelSegmentPoints,
                                                IEnumerable<Point2D> actualSoilModelSegmentPoints)
        {
            Assert.Greater(expectedSoilModelSegmentPoints.Count(), 0);

            CollectionAssert.AreEqual(expectedSoilModelSegmentPoints, actualSoilModelSegmentPoints);
        }

        private static void AssertCalculationGroup(CalculationGroup expectedRootCalculationGroup,
                                                   CalculationGroup actualRootCalculationGroup)
        {
            Assert.AreEqual(expectedRootCalculationGroup.Name, actualRootCalculationGroup.Name);

            Assert.AreEqual(expectedRootCalculationGroup.Children.Count, actualRootCalculationGroup.Children.Count);
            for (var i = 0; i < expectedRootCalculationGroup.Children.Count; i++)
            {
                ICalculationBase expectedChild = expectedRootCalculationGroup.Children[i];
                ICalculationBase actualChild = actualRootCalculationGroup.Children[i];

                Assert.AreEqual(expectedChild.GetType(), actualChild.GetType());

                if (expectedChild is CalculationGroup expectedChildGroup)
                {
                    AssertCalculationGroup(expectedChildGroup, (CalculationGroup) actualChild);
                }

                if (expectedChild is SemiProbabilisticPipingCalculationScenario expectedSemiProbabilisticPipingCalculation)
                {
                    AssertSemiProbabilisticPipingCalculationScenario(expectedSemiProbabilisticPipingCalculation, (SemiProbabilisticPipingCalculationScenario) actualChild);
                }

                if (expectedChild is ProbabilisticPipingCalculationScenario expectedProbabilisticPipingCalculation)
                {
                    AssertProbabilisticPipingCalculationScenario(expectedProbabilisticPipingCalculation, (ProbabilisticPipingCalculationScenario) actualChild);
                }

                if (expectedChild is MacroStabilityInwardsCalculationScenario expectedMacroStabilityInwardsCalculation)
                {
                    AssertMacroStabilityInwardsCalculationScenario(expectedMacroStabilityInwardsCalculation, (MacroStabilityInwardsCalculationScenario) actualChild);
                }

                if (expectedChild is GrassCoverErosionInwardsCalculationScenario expectedGrassCoverErosionInwardsCalculation)
                {
                    AssertGrassCoverErosionInwardsCalculationScenario(expectedGrassCoverErosionInwardsCalculation, (GrassCoverErosionInwardsCalculationScenario) actualChild);
                }

                if (expectedChild is GrassCoverErosionOutwardsWaveConditionsCalculation expectedGrassCoverErosionOutwardsCalculation)
                {
                    AssertGrassCoverErosionOutwardsWaveConditionsCalculation(expectedGrassCoverErosionOutwardsCalculation, (GrassCoverErosionOutwardsWaveConditionsCalculation) actualChild);
                }

                if (expectedChild is StructuresCalculationScenario<HeightStructuresInput> expectedHeightStructuresCalculation)
                {
                    AssertStructuresCalculation(expectedHeightStructuresCalculation, (StructuresCalculationScenario<HeightStructuresInput>) actualChild);
                }

                if (expectedChild is StructuresCalculationScenario<ClosingStructuresInput> expectedClosingStructuresCalculation)
                {
                    AssertStructuresCalculation(expectedClosingStructuresCalculation, (StructuresCalculationScenario<ClosingStructuresInput>) actualChild);
                }

                if (expectedChild is StructuresCalculationScenario<StabilityPointStructuresInput> expectedStabilityPointStructuresCalculation)
                {
                    AssertStructuresCalculation(expectedStabilityPointStructuresCalculation, (StructuresCalculationScenario<StabilityPointStructuresInput>) actualChild);
                }

                if (expectedChild is StabilityStoneCoverWaveConditionsCalculation expectedStabilityStoneCoverWaveConditionsCalculation)
                {
                    AssertStabilityStoneCoverWaveConditionsCalculation(expectedStabilityStoneCoverWaveConditionsCalculation, (StabilityStoneCoverWaveConditionsCalculation) actualChild);
                }

                if (expectedChild is WaveImpactAsphaltCoverWaveConditionsCalculation expectedWaveImpactAsphaltCoverWaveConditionsCalculation)
                {
                    AssertWaveImpactAsphaltCoverWaveConditionsCalculation(expectedWaveImpactAsphaltCoverWaveConditionsCalculation, (WaveImpactAsphaltCoverWaveConditionsCalculation) actualChild);
                }
            }
        }

        private static void AssertWaveConditionsInput(WaveConditionsInput expectedInput, WaveConditionsInput actualInput)
        {
            AssertReferencedObject(() => expectedInput.ForeshoreProfile,
                                   () => actualInput.ForeshoreProfile,
                                   AssertForeshoreProfile);
            AssertReferencedObject(() => expectedInput.HydraulicBoundaryLocation,
                                   () => actualInput.HydraulicBoundaryLocation,
                                   AssertHydraulicBoundaryLocation);
            AssertReferencedObject(() => expectedInput.CalculationsTargetProbability,
                                   () => actualInput.CalculationsTargetProbability,
                                   AssertHydraulicBoundaryLocationCalculationsForTargetProbability);

            AssertBreakWater(expectedInput.BreakWater, actualInput.BreakWater);
            Assert.AreEqual(expectedInput.Orientation, actualInput.Orientation);
            Assert.AreEqual(expectedInput.UseBreakWater, actualInput.UseBreakWater);
            Assert.AreEqual(expectedInput.UseForeshore, actualInput.UseForeshore);
            Assert.AreEqual(expectedInput.UpperBoundaryRevetment, actualInput.UpperBoundaryRevetment);
            Assert.AreEqual(expectedInput.LowerBoundaryRevetment, actualInput.LowerBoundaryRevetment);
            Assert.AreEqual(expectedInput.UpperBoundaryWaterLevels, actualInput.UpperBoundaryWaterLevels);
            Assert.AreEqual(expectedInput.LowerBoundaryWaterLevels, actualInput.LowerBoundaryWaterLevels);
            Assert.AreEqual(expectedInput.StepSize, actualInput.StepSize);
            Assert.AreEqual(expectedInput.WaterLevelType, actualInput.WaterLevelType);
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
            AssertReferencedObject(() => expectedInput.ForeshoreProfile,
                                   () => actualInput.ForeshoreProfile,
                                   AssertForeshoreProfile);
            AssertReferencedObject(() => expectedInput.HydraulicBoundaryLocation,
                                   () => actualInput.HydraulicBoundaryLocation,
                                   AssertHydraulicBoundaryLocation);

            Assert.AreEqual(expectedInput.StructureNormalOrientation, actualInput.StructureNormalOrientation);
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

        /// <summary>
        /// Asserts two referenced objects of <typeparamref name="T"/> and whether
        /// the objects are equal.
        /// </summary>
        /// <typeparam name="T">The type to assert.</typeparam>
        /// <param name="getExpectedReference">The function to perform to retrieve the expected object.</param>
        /// <param name="getActualReference">The function to perform to retrieve the actual object</param>
        /// <param name="assertAction">The action to compare the objects against each other.</param>
        /// <exception cref="AssertionException">Thrown when the items are not equal.</exception>
        private static void AssertReferencedObject<T>(Func<T> getExpectedReference,
                                                      Func<T> getActualReference,
                                                      Action<T, T> assertAction)
        {
            T expectedReferenceValue = getExpectedReference();
            if (expectedReferenceValue == null)
            {
                Assert.IsNull(getActualReference());
            }
            else
            {
                assertAction(expectedReferenceValue, getActualReference());
            }
        }

        #region SpecificFailurePaths

        private static void AssertSpecificFailurePath(SpecificFailurePath expected, SpecificFailurePath actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.InAssembly, actual.InAssembly);

            AssertComments(expected.InAssemblyInputComments, actual.InAssemblyInputComments);
            AssertComments(expected.InAssemblyOutputComments, actual.InAssemblyOutputComments);
            AssertComments(expected.NotInAssemblyComments, actual.NotInAssemblyComments);

            Assert.AreEqual(expected.FailureMechanismSectionSourcePath, actual.FailureMechanismSectionSourcePath);
            AssertFailureMechanismSections(expected.Sections, actual.Sections);

            Assert.AreEqual(expected.Input.N, actual.Input.N);

            AssertFailurePathAssemblyResult(expected.AssemblyResult, actual.AssemblyResult);
        }

        #endregion

        #region StabilityPointStructures FailureMechanism

        private static void AssertStabilityPointStructuresFailureMechanism(StabilityPointStructuresFailureMechanism expectedFailureMechanism,
                                                                           StabilityPointStructuresFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertStabilityPointStructures(expectedFailureMechanism.StabilityPointStructures, actualFailureMechanism.StabilityPointStructures);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<StabilityPointStructuresFailureMechanismSectionResultOld> expectedSectionResults,
                                                                 IEnumerable<StabilityPointStructuresFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentProbability, actualItem.TailorMadeAssessmentProbability, 1e-6);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyProbability, actualItem.ManualAssemblyProbability, 1e-6);
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
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
        }

        private static void AssertStabilityPointStructuresInput(StabilityPointStructuresInput expectedInput,
                                                                StabilityPointStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            AssertReferencedObject(() => expectedInput.Structure,
                                   () => actualInput.Structure,
                                   AssertStabilityPointStructure);

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
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<ClosingStructuresFailureMechanismSectionResultOld> expectedSectionResults,
                                                                 IEnumerable<ClosingStructuresFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentProbability, actualItem.TailorMadeAssessmentProbability, 1e-6);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyProbability, actualItem.ManualAssemblyProbability, 1e-6);
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
            Assert.AreEqual(expectedClosingStructure.ProbabilityOpenStructureBeforeFlooding, actualClosingStructure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(expectedClosingStructure.FailureProbabilityOpenStructure, actualClosingStructure.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedClosingStructure.IdenticalApertures, actualClosingStructure.IdenticalApertures);
            Assert.AreEqual(expectedClosingStructure.FailureProbabilityReparation, actualClosingStructure.FailureProbabilityReparation);
            Assert.AreEqual(expectedClosingStructure.InflowModelType, actualClosingStructure.InflowModelType);
        }

        private static void AssertStructuresCalculation(StructuresCalculationScenario<ClosingStructuresInput> expectedCalculation,
                                                        StructuresCalculationScenario<ClosingStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);
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
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
        }

        private static void AssertClosingStructuresInput(ClosingStructuresInput expectedInput,
                                                         ClosingStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            AssertReferencedObject(() => expectedInput.Structure,
                                   () => actualInput.Structure,
                                   AssertClosingStructure);

            Assert.AreEqual(expectedInput.InflowModelType, actualInput.InflowModelType);
            DistributionAssert.AreEqual(expectedInput.InsideWaterLevel, actualInput.InsideWaterLevel);
            Assert.AreEqual(expectedInput.DeviationWaveDirection, actualInput.DeviationWaveDirection);
            DistributionAssert.AreEqual(expectedInput.ModelFactorSuperCriticalFlow, actualInput.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(expectedInput.DrainCoefficient, actualInput.DrainCoefficient);
            Assert.AreEqual(expectedInput.FactorStormDurationOpenStructure, actualInput.FactorStormDurationOpenStructure);
            DistributionAssert.AreEqual(expectedInput.ThresholdHeightOpenWeir, actualInput.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedInput.AreaFlowApertures, actualInput.AreaFlowApertures);
            Assert.AreEqual(expectedInput.FailureProbabilityOpenStructure, actualInput.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedInput.FailureProbabilityReparation, actualInput.FailureProbabilityReparation);
            Assert.AreEqual(expectedInput.IdenticalApertures, actualInput.IdenticalApertures);
            DistributionAssert.AreEqual(expectedInput.LevelCrestStructureNotClosing, actualInput.LevelCrestStructureNotClosing);
            Assert.AreEqual(expectedInput.ProbabilityOpenStructureBeforeFlooding, actualInput.ProbabilityOpenStructureBeforeFlooding);
        }

        #endregion

        #region DuneErosion FailureMechanism

        private static void AssertDuneErosionFailureMechanism(DuneErosionFailureMechanism expectedFailureMechanism,
                                                              DuneErosionFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertDuneLocations(expectedFailureMechanism.DuneLocations, actualFailureMechanism.DuneLocations);
            AssertDuneLocationCalculations(expectedFailureMechanism, actualFailureMechanism);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<DuneErosionFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<DuneErosionFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedSignalingNorm, actualItem.DetailedAssessmentResultForFactorizedSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForSignalingNorm, actualItem.DetailedAssessmentResultForSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, actualItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForLowerLimitNorm, actualItem.DetailedAssessmentResultForLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedLowerLimitNorm, actualItem.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertDuneLocations(IEnumerable<DuneLocation> expectedDuneLocations,
                                                IEnumerable<DuneLocation> actualDuneLocations)
        {
            AssertCollectionAndItems(expectedDuneLocations,
                                     actualDuneLocations,
                                     AssertDuneBoundaryLocation);
        }

        private static void AssertDuneLocationCalculations(DuneErosionFailureMechanism expectedFailureMechanism,
                                                           DuneErosionFailureMechanism actualFailureMechanism)
        {
            AssertDuneLocationCalculationsForTargetProbabilities(expectedFailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                                                 actualFailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities);
        }

        private static void AssertDuneLocationCalculationsForTargetProbabilities(IEnumerable<DuneLocationCalculationsForTargetProbability> expected,
                                                                                 IEnumerable<DuneLocationCalculationsForTargetProbability> actual)
        {
            AssertCollectionAndItems(expected, actual, AssertDuneLocationCalculationsForTargetProbability);
        }

        private static void AssertDuneLocationCalculationsForTargetProbability(DuneLocationCalculationsForTargetProbability expected, DuneLocationCalculationsForTargetProbability actual)
        {
            Assert.AreEqual(expected.TargetProbability, actual.TargetProbability);
            AssertDuneLocationCalculations(expected.DuneLocationCalculations,
                                           actual.DuneLocationCalculations);
        }

        private static void AssertDuneLocationCalculations(IEnumerable<DuneLocationCalculation> expectedDuneLocationCalculations,
                                                           IEnumerable<DuneLocationCalculation> actualDuneLocationCalculations)
        {
            AssertCollectionAndItems(expectedDuneLocationCalculations,
                                     actualDuneLocationCalculations,
                                     AssertDuneLocationCalculation);
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
        }

        private static void AssertDuneLocationCalculation(DuneLocationCalculation expectedCalculation, DuneLocationCalculation actualCalculation)
        {
            AssertDuneBoundaryLocation(expectedCalculation.DuneLocation, actualCalculation.DuneLocation);
            AssertDuneLocationCalculationOutput(expectedCalculation.Output, actualCalculation.Output);
        }

        private static void AssertDuneLocationCalculationOutput(DuneLocationCalculationOutput expectedOutput, DuneLocationCalculationOutput actualOutput)
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
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<AdoptableFailureMechanismSectionResult> expectedSectionResults,
                                                                 IEnumerable<AdoptableFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.IsRelevant, actualItem.IsRelevant);
                                         Assert.AreEqual(expectedItem.InitialFailureMechanismResult, actualItem.InitialFailureMechanismResult);
                                         Assert.AreEqual(expectedItem.ManualInitialFailureMechanismResultSectionProbability, actualItem.ManualInitialFailureMechanismResultSectionProbability);
                                         Assert.AreEqual(expectedItem.FurtherAnalysisNeeded, actualItem.FurtherAnalysisNeeded);
                                         Assert.AreEqual(expectedItem.RefinedSectionProbability, actualItem.RefinedSectionProbability);
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

        private static void AssertStructuresCalculation(StructuresCalculationScenario<HeightStructuresInput> expectedCalculation,
                                                        StructuresCalculationScenario<HeightStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);
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
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
                expectedOutput.GeneralResult,
                actualOutput.GeneralResult);
        }

        private static void AssertHeightStructuresInput(HeightStructuresInput expectedInput,
                                                        HeightStructuresInput actualInput)
        {
            AssertStructureInputBase(expectedInput, actualInput);

            AssertReferencedObject(() => expectedInput.Structure,
                                   () => actualInput.Structure,
                                   AssertHeightStructure);

            Assert.AreEqual(expectedInput.DeviationWaveDirection, actualInput.DeviationWaveDirection);
            DistributionAssert.AreEqual(expectedInput.ModelFactorSuperCriticalFlow, actualInput.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(expectedInput.LevelCrestStructure, actualInput.LevelCrestStructure);
        }

        #endregion

        #region Piping FailureMechanism

        private static void AssertPipingFailureMechanism(PipingFailureMechanism expectedFailureMechanism,
                                                         PipingFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.ScenarioConfigurationType, actualFailureMechanism.ScenarioConfigurationType);
            AssertPipingProbabilityAssessmentInput(expectedFailureMechanism.PipingProbabilityAssessmentInput, actualFailureMechanism.PipingProbabilityAssessmentInput);
            AssertPipingStochasticSoilModels(expectedFailureMechanism.StochasticSoilModels, actualFailureMechanism.StochasticSoilModels);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);

            Assert.AreEqual(expectedFailureMechanism.SurfaceLines.SourcePath, actualFailureMechanism.SurfaceLines.SourcePath);
            AssertCollectionAndItems(expectedFailureMechanism.SurfaceLines,
                                     actualFailureMechanism.SurfaceLines,
                                     AssertPipingSurfaceLine);
            AssertCollectionAndItems(expectedFailureMechanism.ScenarioConfigurationsPerFailureMechanismSection,
                                     actualFailureMechanism.ScenarioConfigurationsPerFailureMechanismSection,
                                     AssertPipingScenarioConfigurationPerFailureMechanismSection);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.IsRelevant, actualItem.IsRelevant);
                                         Assert.AreEqual(expectedItem.InitialFailureMechanismResult, actualItem.InitialFailureMechanismResult);
                                         Assert.AreEqual(expectedItem.ManualInitialFailureMechanismResultProfileProbability, actualItem.ManualInitialFailureMechanismResultProfileProbability);
                                         Assert.AreEqual(expectedItem.ManualInitialFailureMechanismResultSectionProbability, actualItem.ManualInitialFailureMechanismResultSectionProbability);
                                         Assert.AreEqual(expectedItem.FurtherAnalysisNeeded, actualItem.FurtherAnalysisNeeded);
                                         Assert.AreEqual(expectedItem.ProbabilityRefinementType, actualItem.ProbabilityRefinementType);
                                         Assert.AreEqual(expectedItem.RefinedProfileProbability, actualItem.RefinedProfileProbability);
                                         Assert.AreEqual(expectedItem.RefinedSectionProbability, actualItem.RefinedSectionProbability);
                                     });
        }

        private static void AssertPipingProbabilityAssessmentInput(PipingProbabilityAssessmentInput expectedModel,
                                                                   PipingProbabilityAssessmentInput actualModel)
        {
            Assert.AreEqual(expectedModel.A, actualModel.A);
        }

        private static void AssertSemiProbabilisticPipingCalculationScenario(SemiProbabilisticPipingCalculationScenario expectedPipingCalculation,
                                                                             SemiProbabilisticPipingCalculationScenario actualPipingCalculation)
        {
            AssertPipingCalculationScenario(expectedPipingCalculation, actualPipingCalculation);
            AssertSemiProbabilisticPipingInput(expectedPipingCalculation.InputParameters, actualPipingCalculation.InputParameters);
            AssertSemiProbabilisticPipingOutput(expectedPipingCalculation.Output, actualPipingCalculation.Output);
        }

        private static void AssertProbabilisticPipingCalculationScenario(ProbabilisticPipingCalculationScenario expectedPipingCalculation,
                                                                         ProbabilisticPipingCalculationScenario actualPipingCalculation)
        {
            AssertPipingCalculationScenario(expectedPipingCalculation, actualPipingCalculation);
            AssertProbabilisticPipingInput(expectedPipingCalculation.InputParameters, actualPipingCalculation.InputParameters);
            AssertProbabilisticPipingOutput(expectedPipingCalculation.Output, actualPipingCalculation.Output);
        }

        private static void AssertPipingCalculationScenario(IPipingCalculationScenario<PipingInput> expectedPipingCalculation,
                                                            IPipingCalculationScenario<PipingInput> actualPipingCalculation)
        {
            Assert.AreEqual(expectedPipingCalculation.IsRelevant, actualPipingCalculation.IsRelevant);
            Assert.AreEqual(expectedPipingCalculation.Contribution, actualPipingCalculation.Contribution);
            Assert.AreEqual(expectedPipingCalculation.Name, actualPipingCalculation.Name);
            AssertComments(expectedPipingCalculation.Comments, actualPipingCalculation.Comments);

            AssertPipingInput(expectedPipingCalculation.InputParameters, actualPipingCalculation.InputParameters);
        }

        private static void AssertPipingScenarioConfigurationPerFailureMechanismSection(
            PipingScenarioConfigurationPerFailureMechanismSection expectedConfiguration,
            PipingScenarioConfigurationPerFailureMechanismSection actualConfiguration)
        {
            AssertFailureMechanismSections(new[]
            {
                expectedConfiguration.Section
            }, new[]
            {
                actualConfiguration.Section
            });
            Assert.AreEqual(expectedConfiguration.ScenarioConfigurationType, actualConfiguration.ScenarioConfigurationType);
        }

        private static void AssertPipingInput(PipingInput expectedPipingInput, PipingInput actualPipingInput)
        {
            AssertReferencedObject(() => expectedPipingInput.HydraulicBoundaryLocation,
                                   () => actualPipingInput.HydraulicBoundaryLocation,
                                   AssertHydraulicBoundaryLocation);

            AssertReferencedObject(() => expectedPipingInput.SurfaceLine,
                                   () => actualPipingInput.SurfaceLine,
                                   AssertPipingSurfaceLine);
            AssertReferencedObject(() => expectedPipingInput.StochasticSoilModel,
                                   () => actualPipingInput.StochasticSoilModel,
                                   AssertPipingStochasticSoilModel);
            AssertReferencedObject(() => expectedPipingInput.StochasticSoilProfile,
                                   () => actualPipingInput.StochasticSoilProfile,
                                   AssertPipingStochasticSoilProfile);

            Assert.AreEqual(expectedPipingInput.ExitPointL, actualPipingInput.ExitPointL);
            Assert.AreEqual(expectedPipingInput.EntryPointL, actualPipingInput.EntryPointL);
            Assert.AreEqual(expectedPipingInput.PhreaticLevelExit.Mean, actualPipingInput.PhreaticLevelExit.Mean);
            Assert.AreEqual(expectedPipingInput.PhreaticLevelExit.StandardDeviation, actualPipingInput.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(expectedPipingInput.DampingFactorExit.Mean, actualPipingInput.DampingFactorExit.Mean);
            Assert.AreEqual(expectedPipingInput.DampingFactorExit.StandardDeviation, actualPipingInput.DampingFactorExit.StandardDeviation);
        }

        private static void AssertSemiProbabilisticPipingInput(SemiProbabilisticPipingInput expectedPipingInput,
                                                               SemiProbabilisticPipingInput actualPipingInput)
        {
            Assert.AreEqual(expectedPipingInput.UseAssessmentLevelManualInput, actualPipingInput.UseAssessmentLevelManualInput);
            Assert.AreEqual(expectedPipingInput.AssessmentLevel, actualPipingInput.AssessmentLevel);
        }

        private static void AssertProbabilisticPipingInput(ProbabilisticPipingInput expectedPipingInput,
                                                           ProbabilisticPipingInput actualPipingInput)
        {
            Assert.AreEqual(expectedPipingInput.ShouldProfileSpecificIllustrationPointsBeCalculated,
                            actualPipingInput.ShouldProfileSpecificIllustrationPointsBeCalculated);
            Assert.AreEqual(expectedPipingInput.ShouldSectionSpecificIllustrationPointsBeCalculated,
                            actualPipingInput.ShouldSectionSpecificIllustrationPointsBeCalculated);
        }

        private static void AssertSemiProbabilisticPipingOutput(SemiProbabilisticPipingOutput expectedOutput,
                                                                SemiProbabilisticPipingOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }

            Assert.AreEqual(expectedOutput.HeaveGradient, actualOutput.HeaveGradient);
            Assert.AreEqual(expectedOutput.HeaveFactorOfSafety, actualOutput.HeaveFactorOfSafety);
            Assert.AreEqual(expectedOutput.UpliftEffectiveStress, actualOutput.UpliftEffectiveStress);
            Assert.AreEqual(expectedOutput.UpliftFactorOfSafety, actualOutput.UpliftFactorOfSafety);
            Assert.AreEqual(expectedOutput.SellmeijerCreepCoefficient, actualOutput.SellmeijerCreepCoefficient);
            Assert.AreEqual(expectedOutput.SellmeijerCriticalFall, actualOutput.SellmeijerCriticalFall);
            Assert.AreEqual(expectedOutput.SellmeijerReducedFall, actualOutput.SellmeijerReducedFall);
            Assert.AreEqual(expectedOutput.SellmeijerFactorOfSafety, actualOutput.SellmeijerFactorOfSafety);
        }

        private static void AssertProbabilisticPipingOutput(ProbabilisticPipingOutput expectedOutput,
                                                            ProbabilisticPipingOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
                return;
            }

            AssertPartialProbabilisticPipingOutput((PartialProbabilisticFaultTreePipingOutput) expectedOutput.ProfileSpecificOutput,
                                                   (PartialProbabilisticFaultTreePipingOutput) actualOutput.ProfileSpecificOutput);
            AssertPartialProbabilisticPipingOutput((PartialProbabilisticFaultTreePipingOutput) expectedOutput.SectionSpecificOutput,
                                                   (PartialProbabilisticFaultTreePipingOutput) actualOutput.SectionSpecificOutput);
        }

        private static void AssertPartialProbabilisticPipingOutput(PartialProbabilisticFaultTreePipingOutput expectedOutput,
                                                                   PartialProbabilisticFaultTreePipingOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(expectedOutput.GeneralResult, actualOutput.GeneralResult);
        }

        private static void AssertPipingStochasticSoilModels(PipingStochasticSoilModelCollection expectedModels,
                                                             PipingStochasticSoilModelCollection actualModels)
        {
            Assert.AreEqual(expectedModels.SourcePath, actualModels.SourcePath);
            AssertCollectionAndItems(expectedModels, actualModels, AssertPipingStochasticSoilModel);
        }

        private static void AssertPipingStochasticSoilModel(PipingStochasticSoilModel expectedSoilModel, PipingStochasticSoilModel actualSoilModel)
        {
            Assert.AreEqual(expectedSoilModel.Name, actualSoilModel.Name);
            AssertSegmentPoints(expectedSoilModel.Geometry, actualSoilModel.Geometry);
            AssertCollectionAndItems(expectedSoilModel.StochasticSoilProfiles, actualSoilModel.StochasticSoilProfiles,
                                     AssertPipingStochasticSoilProfile);
        }

        private static void AssertPipingStochasticSoilProfile(PipingStochasticSoilProfile expectedProfile, PipingStochasticSoilProfile actualProfile)
        {
            Assert.AreEqual(expectedProfile.Probability, actualProfile.Probability);
            AssertPipingSoilProfile(expectedProfile.SoilProfile, actualProfile.SoilProfile);
        }

        private static void AssertPipingSoilProfile(PipingSoilProfile expectedProfile, PipingSoilProfile actualProfile)
        {
            Assert.AreEqual(expectedProfile.Bottom, actualProfile.Bottom);
            Assert.AreEqual(expectedProfile.Name, actualProfile.Name);
            Assert.AreEqual(expectedProfile.SoilProfileSourceType, actualProfile.SoilProfileSourceType);
            AssertCollectionAndItems(expectedProfile.Layers, actualProfile.Layers,
                                     AssertPipingSoilLayer);
        }

        private static void AssertPipingSoilLayer(PipingSoilLayer expectedLayer, PipingSoilLayer actualLayer)
        {
            Assert.AreEqual(expectedLayer.Top, actualLayer.Top);
            Assert.AreEqual(expectedLayer.IsAquifer, actualLayer.IsAquifer);
            Assert.AreEqual(expectedLayer.Color.ToArgb(), actualLayer.Color.ToArgb());

            DistributionAssert.AreEqual(expectedLayer.BelowPhreaticLevel, actualLayer.BelowPhreaticLevel);
            DistributionAssert.AreEqual(expectedLayer.DiameterD70, actualLayer.DiameterD70);
            DistributionAssert.AreEqual(expectedLayer.Permeability, actualLayer.Permeability);
        }

        private static void AssertPipingSurfaceLine(PipingSurfaceLine expectedSurfaceLine,
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

        #endregion

        #region MacroStabilityInwards FailureMechanism

        private static void AssertMacroStabilityInwardsFailureMechanism(MacroStabilityInwardsFailureMechanism expectedFailureMechanism,
                                                                        MacroStabilityInwardsFailureMechanism actualFailureMechanism)
        {
            AssertMacroStabilityInwardsProbabilityAssessmentInput(expectedFailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput,
                                                                  actualFailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput);
            AssertMacroStabilityInwardsStochasticSoilModels(expectedFailureMechanism.StochasticSoilModels,
                                                            actualFailureMechanism.StochasticSoilModels);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);

            Assert.AreEqual(expectedFailureMechanism.SurfaceLines.SourcePath, actualFailureMechanism.SurfaceLines.SourcePath);
            AssertCollectionAndItems(expectedFailureMechanism.SurfaceLines,
                                     actualFailureMechanism.SurfaceLines,
                                     AssertMacroStabilityInwardsSurfaceLine);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> expectedSectionResults,
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.IsRelevant, actualItem.IsRelevant);
                                         Assert.AreEqual(expectedItem.InitialFailureMechanismResult, actualItem.InitialFailureMechanismResult);
                                         Assert.AreEqual(expectedItem.ManualInitialFailureMechanismResultProfileProbability, actualItem.ManualInitialFailureMechanismResultProfileProbability);
                                         Assert.AreEqual(expectedItem.ManualInitialFailureMechanismResultSectionProbability, actualItem.ManualInitialFailureMechanismResultSectionProbability);
                                         Assert.AreEqual(expectedItem.FurtherAnalysisNeeded, actualItem.FurtherAnalysisNeeded);
                                         Assert.AreEqual(expectedItem.ProbabilityRefinementType, actualItem.ProbabilityRefinementType);
                                         Assert.AreEqual(expectedItem.RefinedProfileProbability, actualItem.RefinedProfileProbability);
                                         Assert.AreEqual(expectedItem.RefinedSectionProbability, actualItem.RefinedSectionProbability);
                                     });
        }

        private static void AssertMacroStabilityInwardsProbabilityAssessmentInput(MacroStabilityInwardsProbabilityAssessmentInput expectedModel,
                                                                                  MacroStabilityInwardsProbabilityAssessmentInput actualModel)
        {
            Assert.AreEqual(expectedModel.A, actualModel.A);
        }

        private static void AssertMacroStabilityInwardsStochasticSoilModels(MacroStabilityInwardsStochasticSoilModelCollection expectedModels,
                                                                            MacroStabilityInwardsStochasticSoilModelCollection actualModels)
        {
            Assert.AreEqual(expectedModels.SourcePath, actualModels.SourcePath);
            AssertCollectionAndItems(expectedModels, actualModels, AssertMacroStabilityInwardsStochasticSoilModel);
        }

        private static void AssertMacroStabilityInwardsStochasticSoilModel(MacroStabilityInwardsStochasticSoilModel expectedSoilModel,
                                                                           MacroStabilityInwardsStochasticSoilModel actualSoilModel)
        {
            Assert.AreEqual(expectedSoilModel.Name, actualSoilModel.Name);
            AssertSegmentPoints(expectedSoilModel.Geometry, actualSoilModel.Geometry);
            AssertCollectionAndItems(expectedSoilModel.StochasticSoilProfiles,
                                     actualSoilModel.StochasticSoilProfiles,
                                     AssertMacroStabilityInwardsStochasticSoilProfile);
        }

        private static void AssertMacroStabilityInwardsStochasticSoilProfile(MacroStabilityInwardsStochasticSoilProfile expectedProfile,
                                                                             MacroStabilityInwardsStochasticSoilProfile actualProfile)
        {
            Assert.AreEqual(expectedProfile.Probability, actualProfile.Probability);

            if (expectedProfile.SoilProfile is MacroStabilityInwardsSoilProfile1D expectedSoilProfile1D)
            {
                var actualSoilProfile1D = actualProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
                Assert.IsNotNull(actualSoilProfile1D);
                AssertMacroStabilityInwardsSoilProfile(expectedSoilProfile1D, actualSoilProfile1D);
            }

            if (expectedProfile.SoilProfile is MacroStabilityInwardsSoilProfile2D expectedSoilProfile2D)
            {
                var actualSoilProfile2D = actualProfile.SoilProfile as MacroStabilityInwardsSoilProfile2D;
                Assert.IsNotNull(actualSoilProfile2D);
                AssertMacroStabilityInwardsSoilProfile(expectedSoilProfile2D, actualSoilProfile2D);
            }
        }

        private static void AssertMacroStabilityInwardsSoilProfile(MacroStabilityInwardsSoilProfile1D expectedSoilProfile,
                                                                   MacroStabilityInwardsSoilProfile1D actualSoilProfile)
        {
            Assert.AreEqual(expectedSoilProfile.Bottom, actualSoilProfile.Bottom);
            Assert.AreEqual(expectedSoilProfile.Name, actualSoilProfile.Name);
            AssertCollectionAndItems(expectedSoilProfile.Layers,
                                     actualSoilProfile.Layers,
                                     AssertMacroStabilityInwardsSoilLayer);
        }

        private static void AssertMacroStabilityInwardsSoilProfile(MacroStabilityInwardsSoilProfile2D expectedSoilProfile,
                                                                   MacroStabilityInwardsSoilProfile2D actualSoilProfile)
        {
            Assert.AreEqual(expectedSoilProfile.Name, actualSoilProfile.Name);
            AssertCollectionAndItems(expectedSoilProfile.Layers,
                                     actualSoilProfile.Layers,
                                     AssertMacroStabilityInwardsSoilLayer);
        }

        private static void AssertMacroStabilityInwardsSoilLayer(MacroStabilityInwardsSoilLayer1D expectedLayer,
                                                                 MacroStabilityInwardsSoilLayer1D actualLayer)
        {
            Assert.AreEqual(expectedLayer.Top, actualLayer.Top);
            AssertMacroStabilityInwardsSoilLayerData(expectedLayer.Data, actualLayer.Data);
        }

        private static void AssertMacroStabilityInwardsSoilLayer(MacroStabilityInwardsSoilLayer2D expectedLayer,
                                                                 MacroStabilityInwardsSoilLayer2D actualLayer)
        {
            Assert.AreEqual(expectedLayer.OuterRing, actualLayer.OuterRing);
            AssertMacroStabilityInwardsSoilLayerData(expectedLayer.Data, actualLayer.Data);

            AssertCollectionAndItems(expectedLayer.NestedLayers, actualLayer.NestedLayers, AssertMacroStabilityInwardsSoilLayer);
        }

        private static void AssertMacroStabilityInwardsSoilLayerData(MacroStabilityInwardsSoilLayerData expectedData,
                                                                     MacroStabilityInwardsSoilLayerData actualData)
        {
            Assert.AreEqual(expectedData.IsAquifer, actualData.IsAquifer);
            Assert.AreEqual(expectedData.MaterialName, actualData.MaterialName);
            Assert.AreEqual(expectedData.Color.ToArgb(), actualData.Color.ToArgb());
            Assert.AreEqual(expectedData.UsePop, actualData.UsePop);
            Assert.AreEqual(expectedData.ShearStrengthModel, actualData.ShearStrengthModel);
            DistributionAssert.AreEqual(expectedData.AbovePhreaticLevel, actualData.AbovePhreaticLevel);
            DistributionAssert.AreEqual(expectedData.BelowPhreaticLevel, actualData.BelowPhreaticLevel);
            DistributionAssert.AreEqual(expectedData.Cohesion, actualData.Cohesion);
            DistributionAssert.AreEqual(expectedData.FrictionAngle, actualData.FrictionAngle);
            DistributionAssert.AreEqual(expectedData.ShearStrengthRatio, actualData.ShearStrengthRatio);
            DistributionAssert.AreEqual(expectedData.StrengthIncreaseExponent, actualData.StrengthIncreaseExponent);
            DistributionAssert.AreEqual(expectedData.Pop, actualData.Pop);
        }

        private static void AssertMacroStabilityInwardsSurfaceLine(MacroStabilityInwardsSurfaceLine expectedSurfaceLine,
                                                                   MacroStabilityInwardsSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            Assert.AreEqual(expectedSurfaceLine.ReferenceLineIntersectionWorldPoint, actualSurfaceLine.ReferenceLineIntersectionWorldPoint);

            CollectionAssert.AreEqual(expectedSurfaceLine.Points, actualSurfaceLine.Points);

            Assert.AreEqual(expectedSurfaceLine.SurfaceLevelOutside, actualSurfaceLine.SurfaceLevelOutside);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(expectedSurfaceLine.DikeTopAtPolder, actualSurfaceLine.DikeTopAtPolder);
            Assert.AreEqual(expectedSurfaceLine.DikeTopAtRiver, actualSurfaceLine.DikeTopAtRiver);
            Assert.AreEqual(expectedSurfaceLine.ShoulderBaseInside, actualSurfaceLine.ShoulderBaseInside);
            Assert.AreEqual(expectedSurfaceLine.ShoulderTopInside, actualSurfaceLine.ShoulderTopInside);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            Assert.AreEqual(expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.SurfaceLevelInside, actualSurfaceLine.SurfaceLevelInside);
        }

        private static void AssertMacroStabilityInwardsCalculationScenario(MacroStabilityInwardsCalculationScenario expectedMacroStabilityInwardsCalculation,
                                                                           MacroStabilityInwardsCalculationScenario actualMacroStabilityInwardsCalculation)
        {
            Assert.AreEqual(expectedMacroStabilityInwardsCalculation.IsRelevant, actualMacroStabilityInwardsCalculation.IsRelevant);
            Assert.AreEqual(expectedMacroStabilityInwardsCalculation.Contribution, actualMacroStabilityInwardsCalculation.Contribution);
            Assert.AreEqual(expectedMacroStabilityInwardsCalculation.Name, actualMacroStabilityInwardsCalculation.Name);
            AssertComments(expectedMacroStabilityInwardsCalculation.Comments, actualMacroStabilityInwardsCalculation.Comments);

            AssertMacroStabilityInwardsInput(expectedMacroStabilityInwardsCalculation.InputParameters,
                                             actualMacroStabilityInwardsCalculation.InputParameters);

            AssertMacroStabilityInwardsOutput(expectedMacroStabilityInwardsCalculation.Output,
                                              actualMacroStabilityInwardsCalculation.Output);
        }

        private static void AssertMacroStabilityInwardsInput(MacroStabilityInwardsInput expectedInput, MacroStabilityInwardsInput actualInput)
        {
            AssertReferencedObject(() => expectedInput.HydraulicBoundaryLocation,
                                   () => actualInput.HydraulicBoundaryLocation,
                                   AssertHydraulicBoundaryLocation);

            AssertReferencedObject(() => expectedInput.SurfaceLine,
                                   () => actualInput.SurfaceLine,
                                   AssertMacroStabilityInwardsSurfaceLine);
            AssertReferencedObject(() => expectedInput.StochasticSoilModel,
                                   () => actualInput.StochasticSoilModel,
                                   AssertMacroStabilityInwardsStochasticSoilModel);
            AssertReferencedObject(() => expectedInput.StochasticSoilProfile,
                                   () => actualInput.StochasticSoilProfile,
                                   AssertMacroStabilityInwardsStochasticSoilProfile);

            Assert.AreEqual(expectedInput.AssessmentLevel, actualInput.AssessmentLevel);
            Assert.AreEqual(expectedInput.UseAssessmentLevelManualInput, actualInput.UseAssessmentLevelManualInput);
            Assert.AreEqual(expectedInput.SlipPlaneMinimumDepth, actualInput.SlipPlaneMinimumDepth);
            Assert.AreEqual(expectedInput.SlipPlaneMinimumLength, actualInput.SlipPlaneMinimumLength);
            Assert.AreEqual(expectedInput.MaximumSliceWidth, actualInput.MaximumSliceWidth);
            Assert.AreEqual(expectedInput.MoveGrid, actualInput.MoveGrid);
            Assert.AreEqual(expectedInput.DikeSoilScenario, actualInput.DikeSoilScenario);
            Assert.AreEqual(expectedInput.WaterLevelRiverAverage, actualInput.WaterLevelRiverAverage);
            Assert.AreEqual(expectedInput.DrainageConstructionPresent, actualInput.DrainageConstructionPresent);
            Assert.AreEqual(expectedInput.XCoordinateDrainageConstruction, actualInput.XCoordinateDrainageConstruction);
            Assert.AreEqual(expectedInput.ZCoordinateDrainageConstruction, actualInput.ZCoordinateDrainageConstruction);
            Assert.AreEqual(expectedInput.MinimumLevelPhreaticLineAtDikeTopRiver, actualInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(expectedInput.MinimumLevelPhreaticLineAtDikeTopPolder, actualInput.MinimumLevelPhreaticLineAtDikeTopPolder);

            AssertMacroStabilityInwardsLocationInputBase(expectedInput.LocationInputDaily, actualInput.LocationInputDaily);
            AssertMacroStabilityInwardsLocationInput(expectedInput.LocationInputExtreme, actualInput.LocationInputExtreme);

            Assert.AreEqual(expectedInput.AdjustPhreaticLine3And4ForUplift, actualInput.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(expectedInput.LeakageLengthOutwardsPhreaticLine3, actualInput.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(expectedInput.LeakageLengthInwardsPhreaticLine3, actualInput.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(expectedInput.LeakageLengthOutwardsPhreaticLine4, actualInput.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(expectedInput.LeakageLengthInwardsPhreaticLine4, actualInput.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(expectedInput.PiezometricHeadPhreaticLine2Outwards, actualInput.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(expectedInput.PiezometricHeadPhreaticLine2Inwards, actualInput.PiezometricHeadPhreaticLine2Inwards);

            Assert.AreEqual(expectedInput.GridDeterminationType, actualInput.GridDeterminationType);
            Assert.AreEqual(expectedInput.TangentLineDeterminationType, actualInput.TangentLineDeterminationType);

            Assert.AreEqual(expectedInput.TangentLineZTop, actualInput.TangentLineZTop);
            Assert.AreEqual(expectedInput.TangentLineZBottom, actualInput.TangentLineZBottom);
            Assert.AreEqual(expectedInput.TangentLineNumber, actualInput.TangentLineNumber);

            AssertMacroStabilityInwardsGrid(expectedInput.LeftGrid, actualInput.LeftGrid);
            AssertMacroStabilityInwardsGrid(expectedInput.RightGrid, actualInput.RightGrid);

            Assert.AreEqual(expectedInput.CreateZones, actualInput.CreateZones);
        }

        private static void AssertMacroStabilityInwardsGrid(MacroStabilityInwardsGrid expectedGrid, MacroStabilityInwardsGrid actualGrid)
        {
            Assert.AreEqual(expectedGrid.XLeft, actualGrid.XLeft);
            Assert.AreEqual(expectedGrid.XRight, actualGrid.XRight);
            Assert.AreEqual(expectedGrid.NumberOfHorizontalPoints, actualGrid.NumberOfHorizontalPoints);

            Assert.AreEqual(expectedGrid.ZTop, actualGrid.ZTop);
            Assert.AreEqual(expectedGrid.ZBottom, actualGrid.ZBottom);
            Assert.AreEqual(expectedGrid.NumberOfVerticalPoints, actualGrid.NumberOfVerticalPoints);
        }

        private static void AssertMacroStabilityInwardsLocationInputBase(IMacroStabilityInwardsLocationInput expectedLocationInput,
                                                                         IMacroStabilityInwardsLocationInput actualLocationInput)
        {
            Assert.AreEqual(expectedLocationInput.WaterLevelPolder, actualLocationInput.WaterLevelPolder);
            Assert.AreEqual(expectedLocationInput.UseDefaultOffsets, actualLocationInput.UseDefaultOffsets);
            Assert.AreEqual(expectedLocationInput.PhreaticLineOffsetBelowDikeTopAtRiver, actualLocationInput.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(expectedLocationInput.PhreaticLineOffsetBelowDikeTopAtPolder, actualLocationInput.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(expectedLocationInput.PhreaticLineOffsetBelowShoulderBaseInside, actualLocationInput.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(expectedLocationInput.PhreaticLineOffsetBelowDikeToeAtPolder, actualLocationInput.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        private static void AssertMacroStabilityInwardsLocationInput(IMacroStabilityInwardsLocationInputExtreme expectedLocationInput,
                                                                     IMacroStabilityInwardsLocationInputExtreme actualLocationInput)
        {
            AssertMacroStabilityInwardsLocationInputBase(expectedLocationInput, actualLocationInput);
            Assert.AreEqual(expectedLocationInput.PenetrationLength, actualLocationInput.PenetrationLength);
        }

        private static void AssertMacroStabilityInwardsOutput(MacroStabilityInwardsOutput expectedOutput,
                                                              MacroStabilityInwardsOutput actualOutput)
        {
            if (expectedOutput == null)
            {
                Assert.IsNull(actualOutput);
            }
            else
            {
                Assert.AreEqual(expectedOutput.FactorOfStability, actualOutput.FactorOfStability);
                Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMin, actualOutput.ForbiddenZonesXEntryMin);
                Assert.AreEqual(expectedOutput.ForbiddenZonesXEntryMax, actualOutput.ForbiddenZonesXEntryMax);

                AssertMacroStabilityInwardsSlidingCurve(expectedOutput.SlidingCurve, actualOutput.SlidingCurve);
                AssertMacroStabilityInwardsSlipPlaneUpliftVan(expectedOutput.SlipPlane, actualOutput.SlipPlane);
            }
        }

        private static void AssertMacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsSlipPlaneUpliftVan expectedSlipPlane,
                                                                          MacroStabilityInwardsSlipPlaneUpliftVan actualSlipPlane)
        {
            AssertMacroStabilityInwardsGrid(expectedSlipPlane.LeftGrid, actualSlipPlane.LeftGrid);
            AssertMacroStabilityInwardsGrid(expectedSlipPlane.RightGrid, actualSlipPlane.RightGrid);
            CollectionAssert.AreEqual(expectedSlipPlane.TangentLines, actualSlipPlane.TangentLines);
        }

        private static void AssertMacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCurve expectedCurve,
                                                                    MacroStabilityInwardsSlidingCurve actualCurve)
        {
            Assert.AreEqual(expectedCurve.NonIteratedHorizontalForce, actualCurve.NonIteratedHorizontalForce);
            Assert.AreEqual(expectedCurve.IteratedHorizontalForce, actualCurve.IteratedHorizontalForce);

            AssertMacroStabilityInwardsSlidingCircle(expectedCurve.LeftCircle, actualCurve.LeftCircle);
            AssertMacroStabilityInwardsSlidingCircle(expectedCurve.RightCircle, actualCurve.RightCircle);

            AssertCollectionAndItems(expectedCurve.Slices, actualCurve.Slices, AssertMacroStabilityInwardsSlice);
        }

        private static void AssertMacroStabilityInwardsSlidingCircle(MacroStabilityInwardsSlidingCircle expectedCircle,
                                                                     MacroStabilityInwardsSlidingCircle actualCircle)
        {
            Assert.AreEqual(expectedCircle.Center, actualCircle.Center);
            Assert.AreEqual(expectedCircle.Radius, actualCircle.Radius);
            Assert.AreEqual(expectedCircle.IsActive, actualCircle.IsActive);
            Assert.AreEqual(expectedCircle.NonIteratedForce, actualCircle.NonIteratedForce);
            Assert.AreEqual(expectedCircle.IteratedForce, actualCircle.IteratedForce);
            Assert.AreEqual(expectedCircle.DrivingMoment, actualCircle.DrivingMoment);
            Assert.AreEqual(expectedCircle.ResistingMoment, actualCircle.ResistingMoment);
        }

        private static void AssertMacroStabilityInwardsSlice(MacroStabilityInwardsSlice expectedSlice,
                                                             MacroStabilityInwardsSlice actualSlice)
        {
            Assert.AreEqual(expectedSlice.TopLeftPoint, actualSlice.TopLeftPoint);
            Assert.AreEqual(expectedSlice.TopRightPoint, actualSlice.TopRightPoint);
            Assert.AreEqual(expectedSlice.BottomLeftPoint, actualSlice.BottomLeftPoint);
            Assert.AreEqual(expectedSlice.BottomRightPoint, actualSlice.BottomRightPoint);

            Assert.AreEqual(expectedSlice.Cohesion, actualSlice.Cohesion);
            Assert.AreEqual(expectedSlice.FrictionAngle, actualSlice.FrictionAngle);
            Assert.AreEqual(expectedSlice.CriticalPressure, actualSlice.CriticalPressure);
            Assert.AreEqual(expectedSlice.OverConsolidationRatio, actualSlice.OverConsolidationRatio);
            Assert.AreEqual(expectedSlice.Pop, actualSlice.Pop);
            Assert.AreEqual(expectedSlice.DegreeOfConsolidationPorePressureSoil, actualSlice.DegreeOfConsolidationPorePressureSoil);
            Assert.AreEqual(expectedSlice.DegreeOfConsolidationPorePressureLoad, actualSlice.DegreeOfConsolidationPorePressureLoad);
            Assert.AreEqual(expectedSlice.Dilatancy, actualSlice.Dilatancy);
            Assert.AreEqual(expectedSlice.ExternalLoad, actualSlice.ExternalLoad);
            Assert.AreEqual(expectedSlice.HydrostaticPorePressure, actualSlice.HydrostaticPorePressure);
            Assert.AreEqual(expectedSlice.LeftForce, actualSlice.LeftForce);
            Assert.AreEqual(expectedSlice.LeftForceAngle, actualSlice.LeftForceAngle);
            Assert.AreEqual(expectedSlice.LeftForceY, actualSlice.LeftForceY);
            Assert.AreEqual(expectedSlice.RightForce, actualSlice.RightForce);
            Assert.AreEqual(expectedSlice.RightForceAngle, actualSlice.RightForceAngle);
            Assert.AreEqual(expectedSlice.RightForceY, actualSlice.RightForceY);
            Assert.AreEqual(expectedSlice.LoadStress, actualSlice.LoadStress);
            Assert.AreEqual(expectedSlice.NormalStress, actualSlice.NormalStress);
            Assert.AreEqual(expectedSlice.PorePressure, actualSlice.PorePressure);
            Assert.AreEqual(expectedSlice.HorizontalPorePressure, actualSlice.HorizontalPorePressure);
            Assert.AreEqual(expectedSlice.VerticalPorePressure, actualSlice.VerticalPorePressure);
            Assert.AreEqual(expectedSlice.PiezometricPorePressure, actualSlice.PiezometricPorePressure);
            Assert.AreEqual(expectedSlice.EffectiveStress, actualSlice.EffectiveStress);
            Assert.AreEqual(expectedSlice.ExcessPorePressure, actualSlice.ExcessPorePressure);
            Assert.AreEqual(expectedSlice.ShearStress, actualSlice.ShearStress);
            Assert.AreEqual(expectedSlice.SoilStress, actualSlice.SoilStress);
            Assert.AreEqual(expectedSlice.TotalPorePressure, actualSlice.TotalPorePressure);
            Assert.AreEqual(expectedSlice.TotalStress, actualSlice.TotalStress);
            Assert.AreEqual(expectedSlice.Weight, actualSlice.Weight);
        }

        #endregion

        #region GrassCoverErosionInwards FailureMechanism

        private static void AssertGrassCoverErosionInwardsFailureMechanism(GrassCoverErosionInwardsFailureMechanism expectedFailureMechanism,
                                                                           GrassCoverErosionInwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
            AssertDikeProfiles(expectedFailureMechanism.DikeProfiles, actualFailureMechanism.DikeProfiles);
            AssertCalculationGroup(expectedFailureMechanism.CalculationsGroup, actualFailureMechanism.CalculationsGroup);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentProbability, actualItem.TailorMadeAssessmentProbability, 1e-6);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyProbability, actualItem.ManualAssemblyProbability, 1e-6);
                                     });
        }

        private static void AssertGrassCoverErosionInwardsCalculationScenario(GrassCoverErosionInwardsCalculationScenario expectedCalculation,
                                                                              GrassCoverErosionInwardsCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);
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
            AssertReferencedObject(() => expectedInput.DikeProfile,
                                   () => actualInput.DikeProfile,
                                   AssertDikeProfile);
            AssertReferencedObject(() => expectedInput.HydraulicBoundaryLocation,
                                   () => actualInput.HydraulicBoundaryLocation,
                                   AssertHydraulicBoundaryLocation);

            AssertBreakWater(expectedInput.BreakWater, actualInput.BreakWater);
            Assert.AreEqual(expectedInput.Orientation, actualInput.Orientation);
            Assert.AreEqual(expectedInput.UseBreakWater, actualInput.UseBreakWater);
            Assert.AreEqual(expectedInput.UseForeshore, actualInput.UseForeshore);
            Assert.AreEqual(expectedInput.DikeHeight, actualInput.DikeHeight);
            Assert.AreEqual(expectedInput.CriticalFlowRate.Mean, actualInput.CriticalFlowRate.Mean);
            Assert.AreEqual(expectedInput.CriticalFlowRate.StandardDeviation, actualInput.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(expectedInput.ShouldOvertoppingOutputIllustrationPointsBeCalculated, actualInput.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(expectedInput.ShouldDikeHeightBeCalculated, actualInput.ShouldDikeHeightBeCalculated);
            Assert.AreEqual(expectedInput.DikeHeightTargetProbability, actualInput.DikeHeightTargetProbability);
            Assert.AreEqual(expectedInput.ShouldDikeHeightIllustrationPointsBeCalculated, actualInput.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(expectedInput.ShouldOvertoppingRateBeCalculated, actualInput.ShouldOvertoppingRateBeCalculated);
            Assert.AreEqual(expectedInput.OvertoppingRateTargetProbability, actualInput.OvertoppingRateTargetProbability);
            Assert.AreEqual(expectedInput.ShouldOvertoppingRateIllustrationPointsBeCalculated, actualInput.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        private static void AssertGrassCoverErosionInwardsOutput(GrassCoverErosionInwardsOutput expectedOutput,
                                                                 GrassCoverErosionInwardsOutput actualOutput)
        {
            AssertOvertoppingOutput(expectedOutput.OvertoppingOutput, actualOutput.OvertoppingOutput);
            AssertDikeHeightOutput(expectedOutput.DikeHeightOutput, actualOutput.DikeHeightOutput);
            AssertOvertoppingRateOutput(expectedOutput.OvertoppingRateOutput, actualOutput.OvertoppingRateOutput);
        }

        private static void AssertOvertoppingOutput(OvertoppingOutput expectedOutput,
                                                    OvertoppingOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.WaveHeight, actualOutput.WaveHeight);
            Assert.AreEqual(expectedOutput.IsOvertoppingDominant, actualOutput.IsOvertoppingDominant);
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);

            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(expectedOutput.GeneralResult, actualOutput.GeneralResult);
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
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(expectedOutput.GeneralResult, actualOutput.GeneralResult);
        }

        #endregion

        #region GrassCoverErosionOutwards FailureMechanism

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(GrassCoverErosionOutwardsFailureMechanism expectedFailureMechanism,
                                                                            GrassCoverErosionOutwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResultOld> expectedSectionResults,
                                                                 IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedSignalingNorm, actualItem.DetailedAssessmentResultForFactorizedSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForSignalingNorm, actualItem.DetailedAssessmentResultForSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, actualItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForLowerLimitNorm, actualItem.DetailedAssessmentResultForLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedLowerLimitNorm, actualItem.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertGrassCoverErosionOutwardsWaveConditionsCalculation(GrassCoverErosionOutwardsWaveConditionsCalculation expectedCalculation,
                                                                                     GrassCoverErosionOutwardsWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertGrassCoverErosionOutwardsCalculationInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

            if (expectedCalculation.HasOutput)
            {
                AssertWaveConditionsOutputs(expectedCalculation.Output.WaveRunUpOutput.ToArray(), actualCalculation.Output.WaveRunUpOutput.ToArray());
                AssertWaveConditionsOutputs(expectedCalculation.Output.WaveImpactOutput.ToArray(), actualCalculation.Output.WaveImpactOutput.ToArray());
            }
            else
            {
                Assert.IsFalse(actualCalculation.HasOutput);
            }
        }

        private static void AssertGrassCoverErosionOutwardsCalculationInput(GrassCoverErosionOutwardsWaveConditionsInput expectedInput,
                                                                            GrassCoverErosionOutwardsWaveConditionsInput actualInput)
        {
            AssertWaveConditionsInput(expectedInput, actualInput);
            Assert.AreEqual(expectedInput.CalculationType, actualInput.CalculationType);
        }

        #endregion

        #region StabilityStoneCover FailureMechanism

        private static void AssertStabilityStoneCoverFailureMechanism(StabilityStoneCoverFailureMechanism expectedFailureMechanism,
                                                                      StabilityStoneCoverFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<StabilityStoneCoverFailureMechanismSectionResultOld> expectedSectionResults,
                                                                 IEnumerable<StabilityStoneCoverFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedSignalingNorm, actualItem.DetailedAssessmentResultForFactorizedSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForSignalingNorm, actualItem.DetailedAssessmentResultForSignalingNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, actualItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForLowerLimitNorm, actualItem.DetailedAssessmentResultForLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedLowerLimitNorm, actualItem.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        private static void AssertStabilityStoneCoverWaveConditionsCalculation(StabilityStoneCoverWaveConditionsCalculation expectedCalculation,
                                                                               StabilityStoneCoverWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            AssertComments(expectedCalculation.Comments, actualCalculation.Comments);

            AssertStabilityStoneCoverWaveConditionsInput(expectedCalculation.InputParameters, actualCalculation.InputParameters);

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

        private static void AssertStabilityStoneCoverWaveConditionsInput(StabilityStoneCoverWaveConditionsInput expectedInput,
                                                                         StabilityStoneCoverWaveConditionsInput actualInput)
        {
            AssertWaveConditionsInput(expectedInput, actualInput);
            Assert.AreEqual(expectedInput.CalculationType, actualInput.CalculationType);
        }

        #endregion

        #region WaveImpactAsphaltCover FailureMechanism

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(WaveImpactAsphaltCoverFailureMechanism expectedFailureMechanism,
                                                                         WaveImpactAsphaltCoverFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL, actualFailureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL);

            AssertForeshoreProfiles(expectedFailureMechanism.ForeshoreProfiles, actualFailureMechanism.ForeshoreProfiles);
            AssertCalculationGroup(expectedFailureMechanism.WaveConditionsCalculationGroup, actualFailureMechanism.WaveConditionsCalculationGroup);
        }

        private static void AssertFailureMechanismSectionResults(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResultOld> expectedSectionResults,
                                                                 IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults, actualSectionResults, (expectedItem, actualItem) =>
            {
                Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedSignalingNorm, actualItem.DetailedAssessmentResultForFactorizedSignalingNorm);
                Assert.AreEqual(expectedItem.DetailedAssessmentResultForSignalingNorm, actualItem.DetailedAssessmentResultForSignalingNorm);
                Assert.AreEqual(expectedItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, actualItem.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                Assert.AreEqual(expectedItem.DetailedAssessmentResultForLowerLimitNorm, actualItem.DetailedAssessmentResultForLowerLimitNorm);
                Assert.AreEqual(expectedItem.DetailedAssessmentResultForFactorizedLowerLimitNorm, actualItem.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
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

        #region PipingStructure FailureMechanism

        private static void AssertPipingStructureFailureMechanism(PipingStructureFailureMechanism expectedFailureMechanism,
                                                                  PipingStructureFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.GeneralInput.N, actualFailureMechanism.GeneralInput.N);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<PipingStructureFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<PipingStructureFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        #endregion

        #region MacroStabilityOutwards FailureMechanism

        private static void AssertMacroStabilityOutwardsFailureMechanism(MacroStabilityOutwardsFailureMechanism expectedFailureMechanism,
                                                                         MacroStabilityOutwardsFailureMechanism actualFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A, actualFailureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A);
        }

        private static void AssertFailureMechanismSectionResults(
            IEnumerable<MacroStabilityOutwardsFailureMechanismSectionResultOld> expectedSectionResults,
            IEnumerable<MacroStabilityOutwardsFailureMechanismSectionResultOld> actualSectionResults)
        {
            AssertCollectionAndItems(expectedSectionResults,
                                     actualSectionResults,
                                     (expectedItem, actualItem) =>
                                     {
                                         Assert.AreEqual(expectedItem.SimpleAssessmentResult, actualItem.SimpleAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentResult, actualItem.DetailedAssessmentResult);
                                         Assert.AreEqual(expectedItem.DetailedAssessmentProbability, actualItem.DetailedAssessmentProbability);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentResult, actualItem.TailorMadeAssessmentResult);
                                         Assert.AreEqual(expectedItem.TailorMadeAssessmentProbability, actualItem.TailorMadeAssessmentProbability);
                                         Assert.AreEqual(expectedItem.UseManualAssembly, actualItem.UseManualAssembly);
                                         Assert.AreEqual(expectedItem.ManualAssemblyCategoryGroup, actualItem.ManualAssemblyCategoryGroup);
                                     });
        }

        #endregion

        #region Hydraulic Boundary Database

        private static void AssertHydraulicBoundaryDatabase(HydraulicBoundaryDatabase expectedBoundaryDatabase,
                                                            HydraulicBoundaryDatabase actualBoundaryDatabase)
        {
            Assert.IsNotNull(expectedBoundaryDatabase);
            Assert.AreEqual(expectedBoundaryDatabase.Version, actualBoundaryDatabase.Version);
            Assert.AreEqual(expectedBoundaryDatabase.FilePath, actualBoundaryDatabase.FilePath);

            AssertHydraulicLocationConfigurationSettings(expectedBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                         actualBoundaryDatabase.HydraulicLocationConfigurationSettings);

            Assert.AreEqual(expectedBoundaryDatabase.Locations.Count, actualBoundaryDatabase.Locations.Count);

            AssertHydraulicBoundaryLocations(expectedBoundaryDatabase.Locations, actualBoundaryDatabase.Locations);
        }

        private static void AssertHydraulicLocationConfigurationSettings(HydraulicLocationConfigurationSettings expectedSettings,
                                                                         HydraulicLocationConfigurationSettings actualSettings)
        {
            Assert.AreEqual(expectedSettings.FilePath, actualSettings.FilePath);
            Assert.AreEqual(expectedSettings.ScenarioName, actualSettings.ScenarioName);
            Assert.AreEqual(expectedSettings.Year, actualSettings.Year);
            Assert.AreEqual(expectedSettings.Scope, actualSettings.Scope);
            Assert.AreEqual(expectedSettings.SeaLevel, actualSettings.SeaLevel);
            Assert.AreEqual(expectedSettings.RiverDischarge, actualSettings.RiverDischarge);
            Assert.AreEqual(expectedSettings.LakeLevel, actualSettings.LakeLevel);
            Assert.AreEqual(expectedSettings.WindDirection, actualSettings.WindDirection);
            Assert.AreEqual(expectedSettings.WindSpeed, actualSettings.WindSpeed);
            Assert.AreEqual(expectedSettings.Comment, actualSettings.Comment);
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
        }

        #endregion

        #region Hydraulic Boundary Location Calculations

        private static void AssertHydraulicBoundaryLocationCalculations(AssessmentSection expected,
                                                                        AssessmentSection actual)
        {
            AssertHydraulicBoundaryLocationCalculations(expected.WaterLevelCalculationsForSignalingNorm,
                                                        actual.WaterLevelCalculationsForSignalingNorm);
            AssertHydraulicBoundaryLocationCalculations(expected.WaterLevelCalculationsForLowerLimitNorm,
                                                        actual.WaterLevelCalculationsForLowerLimitNorm);

            AssertHydraulicBoundaryLocationCalculationsForTargetProbabilities(expected.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                                                              actual.WaterLevelCalculationsForUserDefinedTargetProbabilities);

            AssertHydraulicBoundaryLocationCalculationsForTargetProbabilities(expected.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                              actual.WaveHeightCalculationsForUserDefinedTargetProbabilities);
        }

        private static void AssertHydraulicBoundaryLocationCalculationsForTargetProbabilities(IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> expected,
                                                                                              IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> actual)
        {
            AssertCollectionAndItems(expected, actual,
                                     AssertHydraulicBoundaryLocationCalculationsForTargetProbability);
        }

        private static void AssertHydraulicBoundaryLocationCalculationsForTargetProbability(HydraulicBoundaryLocationCalculationsForTargetProbability expected,
                                                                                            HydraulicBoundaryLocationCalculationsForTargetProbability actual)
        {
            Assert.AreEqual(expected.TargetProbability, actual.TargetProbability);
            AssertHydraulicBoundaryLocationCalculations(expected.HydraulicBoundaryLocationCalculations,
                                                        actual.HydraulicBoundaryLocationCalculations);
        }

        private static void AssertHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> expected,
                                                                        IEnumerable<HydraulicBoundaryLocationCalculation> actual)
        {
            AssertCollectionAndItems(expected, actual,
                                     AssertHydraulicBoundaryLocationCalculation);
        }

        private static void AssertHydraulicBoundaryLocationCalculation(HydraulicBoundaryLocationCalculation expected, HydraulicBoundaryLocationCalculation actual)
        {
            AssertHydraulicBoundaryLocation(expected.HydraulicBoundaryLocation, actual.HydraulicBoundaryLocation);
            Assert.AreEqual(expected.InputParameters.ShouldIllustrationPointsBeCalculated, actual.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationCalculationOutput(expected.Output, actual.Output);
        }

        private static void AssertHydraulicBoundaryLocationCalculationOutput(HydraulicBoundaryLocationCalculationOutput expectedOutput,
                                                                             HydraulicBoundaryLocationCalculationOutput actualOutput)
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

        private static void AssertGeneralResultTopLevelFaultTreeIllustrationPoint(
            GeneralResult<TopLevelFaultTreeIllustrationPoint> expected,
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

        private static void AssertTopLevelFaultTreeIllustrationPoint(
            TopLevelFaultTreeIllustrationPoint expected,
            TopLevelFaultTreeIllustrationPoint actual)
        {
            AssertWindDirection(expected.WindDirection, actual.WindDirection);

            Assert.AreEqual(expected.ClosingSituation, actual.ClosingSituation);

            AssertIllustrationPointNode(expected.FaultTreeNodeRoot, actual.FaultTreeNodeRoot);
        }

        private static void AssertIllustrationPointNode(IllustrationPointNode expected,
                                                        IllustrationPointNode actual)
        {
            if (expected.Data is FaultTreeIllustrationPoint expectedFaultTreeIllustrationPoint)
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

            if (expected.Data is SubMechanismIllustrationPoint expectedSubMechanismIllustrationPoint)
            {
                var actualSubMechanismIllustrationPoint = actual.Data as SubMechanismIllustrationPoint;
                Assert.IsNotNull(actualSubMechanismIllustrationPoint);

                AssertSubMechanismIllustrationPoint(expectedSubMechanismIllustrationPoint,
                                                    actualSubMechanismIllustrationPoint);
                return;
            }

            Assert.Fail($"Expected data type {expected.Data.GetType()} is not supported.");
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

        private static void AssertWindDirection(WindDirection expectedWindDirection,
                                                WindDirection actualWindDirection)
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

        private static void AssertSubMechanismIllustrationPoint(
            SubMechanismIllustrationPoint expectedSubMechanismIllustrationPoint,
            SubMechanismIllustrationPoint actualSubMechanismIllustrationPoint)
        {
            Assert.AreEqual(expectedSubMechanismIllustrationPoint.Name, actualSubMechanismIllustrationPoint.Name);
            Assert.AreEqual(expectedSubMechanismIllustrationPoint.Beta, actualSubMechanismIllustrationPoint.Beta);

            AssertCollectionAndItems(expectedSubMechanismIllustrationPoint.Stochasts,
                                     actualSubMechanismIllustrationPoint.Stochasts,
                                     AssertSubMechanismIllustrationPointStochast);
            AssertCollectionAndItems(expectedSubMechanismIllustrationPoint.IllustrationPointResults,
                                     actualSubMechanismIllustrationPoint.IllustrationPointResults,
                                     AssertIllustrationPointResult);
        }

        private static void AssertSubMechanismIllustrationPointStochast(
            SubMechanismIllustrationPointStochast expectedStochast,
            SubMechanismIllustrationPointStochast actualStochast)
        {
            AssertStochast(expectedStochast, actualStochast);
            Assert.AreEqual(expectedStochast.Realization, actualStochast.Realization);
        }

        private static void AssertIllustrationPointResult(
            IllustrationPointResult expectedResult,
            IllustrationPointResult actualResult)
        {
            Assert.AreEqual(expectedResult.Description, actualResult.Description);
            Assert.AreEqual(expectedResult.Value, actualResult.Value);
        }

        #endregion
    }
}