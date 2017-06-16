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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.TestUtil.Test
{
    [TestFixture]
    public class RingtoetsProjectTestHelperTest
    {
        [Test]
        public void RingtoetsProjectHelper_Always_ReturnsFullProject()
        {
            // Setup
            const string expectedProjectName = "tempProjectFile";
            const string expectedDescription = "description";
            const string expectedAssessmentSectionName = "assessmentSection";

            // Call
            RingtoetsProject project = RingtoetsProjectTestHelper.GetFullTestProject();

            // Assert
            Assert.AreEqual(expectedProjectName, project.Name);
            Assert.AreEqual(expectedDescription, project.Description);

            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.NotNull(assessmentSection);
            Assert.AreEqual(expectedAssessmentSectionName, assessmentSection.Name);

            AssertHydraulicBoundaryDatabase(assessmentSection.HydraulicBoundaryDatabase);

            AssertPipingFailureMechanism(assessmentSection);

            AssertGrassCoverErosionInwardsFailureMechanism(assessmentSection);

            AssertGrassCoverErosionOutwardsFailureMechanism(assessmentSection);

            AssertStabilityStoneCoverFailureMechanism(assessmentSection);

            AssertWaveImpactAsphaltCoverFailureMechanism(assessmentSection);

            AssertHeightStructuresFailureMechanism(assessmentSection);

            AssertClosingStructuresFailureMechanism(assessmentSection);

            AssertStabilityPointStructuresFailureMechanism(assessmentSection);

            AssertDuneErosionFailureMechanism(assessmentSection);
        }

        private static void AssertPipingFailureMechanism(AssessmentSection assessmentSection)
        {
            PipingFailureMechanism failureMechanism = assessmentSection.PipingFailureMechanism;
            Assert.AreEqual("some/path/to/stochasticSoilModelFile", failureMechanism.StochasticSoilModels.SourcePath);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual("modelName", soilModel.Name);
            Assert.AreEqual(2, soilModel.StochasticSoilProfiles.Count);
            StochasticSoilProfile stochasticSoilProfile1 = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(0.2, stochasticSoilProfile1.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, stochasticSoilProfile1.SoilProfileType);
            Assert.AreEqual(-1, stochasticSoilProfile1.SoilProfileId);
            StochasticSoilProfile stochasticSoilProfile2 = soilModel.StochasticSoilProfiles[1];
            Assert.AreEqual(0.8, stochasticSoilProfile2.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticSoilProfile2.SoilProfileType);
            Assert.AreEqual(-1, stochasticSoilProfile2.SoilProfileId);

            Assert.AreEqual("some/path/to/surfaceLineFile", failureMechanism.SurfaceLines.SourcePath);
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count);
            RingtoetsPipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines.First();
            Assert.AreEqual("Surfaceline", surfaceLine.Name);
            Assert.AreEqual(new Point2D(4.0, 6.0), surfaceLine.ReferenceLineIntersectionWorldPoint);
            var geometryPoints = new[]
            {
                new Point3D(6.0, 6.0, -2.3),
                new Point3D(5.8, 6.0, -2.3),
                new Point3D(5.6, 6.0, 3.4),
                new Point3D(4.2, 6.0, 3.5),
                new Point3D(4.0, 6.0, 0.5),
                new Point3D(3.8, 6.0, 0.5),
                new Point3D(3.6, 6.0, 0.2),
                new Point3D(3.4, 6.0, 0.25),
                new Point3D(3.2, 6.0, 0.5),
                new Point3D(3.0, 6.0, 0.5)
            };
            CollectionAssert.AreEqual(geometryPoints, surfaceLine.Points);
            Assert.AreSame(surfaceLine.Points[1], surfaceLine.DikeToeAtRiver);
            Assert.AreSame(surfaceLine.Points[4], surfaceLine.DikeToeAtPolder);
            Assert.AreSame(surfaceLine.Points[5], surfaceLine.DitchDikeSide);
            Assert.AreSame(surfaceLine.Points[6], surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(surfaceLine.Points[7], surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(surfaceLine.Points[8], surfaceLine.DitchPolderSide);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (PipingCalculationScenario) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);

            var emptyCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, emptyCalculationGroup.Children.Count);

            var calculationWithoutOutput = (PipingCalculationScenario) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(AssessmentSection assessmentSection)
        {
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (GrassCoverErosionInwardsCalculation) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);

            var emptyCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, emptyCalculationGroup.Children.Count);

            var calculationWithoutOutput = (GrassCoverErosionInwardsCalculation) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            GrassCoverErosionInwardsFailureMechanismSectionResult firstSectionResult = failureMechanism.SectionResults.First();
            Assert.AreSame(calculationWithOutput, firstSectionResult.Calculation);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(AssessmentSection assessmentSection)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            Assert.AreEqual(2, failureMechanism.HydraulicBoundaryLocations.Count);
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithoutOutput = (GrassCoverErosionOutwardsWaveConditionsCalculation) firstCalculationGroup.Children[0];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            var emptyCalculationGroup = (CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[1];
            Assert.AreEqual(0, emptyCalculationGroup.Children.Count);

            var calculationWithOutput = (GrassCoverErosionOutwardsWaveConditionsCalculation) failureMechanism.WaveConditionsCalculationGroup.Children[2];
            Assert.IsTrue(calculationWithOutput.HasOutput);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(AssessmentSection assessmentSection)
        {
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.NotNull(failureMechanism.WaveConditionsCalculationGroup);
            Assert.AreEqual(3, failureMechanism.WaveConditionsCalculationGroup.Children.Count);

            var calculationGroup = (CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual(1, calculationGroup.Children.Count);

            var stabilityStoneCoverCalculationWithoutOutput = (StabilityStoneCoverWaveConditionsCalculation) calculationGroup.Children[0];
            Assert.IsFalse(stabilityStoneCoverCalculationWithoutOutput.HasOutput);

            Assert.AreEqual(0, ((CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[1]).Children.Count);

            var stabilityStoneCoverCalculationWithOutput = (StabilityStoneCoverWaveConditionsCalculation) failureMechanism.WaveConditionsCalculationGroup.Children[2];
            Assert.IsTrue(stabilityStoneCoverCalculationWithOutput.HasOutput);
            Assert.AreEqual(2, stabilityStoneCoverCalculationWithOutput.Output.BlocksOutput.Count());
            Assert.AreEqual(2, stabilityStoneCoverCalculationWithOutput.Output.ColumnsOutput.Count());
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(AssessmentSection assessmentSection)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.NotNull(failureMechanism.WaveConditionsCalculationGroup);
            Assert.AreEqual(3, failureMechanism.WaveConditionsCalculationGroup.Children.Count);

            var calculationGroup = (CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual(1, calculationGroup.Children.Count);

            var waveImpactAsphaltCoverCalculationWithoutOutput = (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationGroup.Children[0];
            Assert.IsFalse(waveImpactAsphaltCoverCalculationWithoutOutput.HasOutput);

            Assert.AreEqual(0, ((CalculationGroup) failureMechanism.WaveConditionsCalculationGroup.Children[1]).Children.Count);

            var waveImpactAsphaltCoverCalculationWithOutput = (WaveImpactAsphaltCoverWaveConditionsCalculation) failureMechanism.WaveConditionsCalculationGroup.Children[2];
            Assert.IsTrue(waveImpactAsphaltCoverCalculationWithOutput.HasOutput);
            Assert.AreEqual(2, waveImpactAsphaltCoverCalculationWithOutput.Output.Items.Count());
        }

        private static void AssertHeightStructuresFailureMechanism(AssessmentSection assessmentSection)
        {
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.HeightStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<HeightStructuresInput>) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);

            var secondCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, secondCalculationGroup.Children.Count);
            var calculationWithoutOutput = (StructuresCalculation<HeightStructuresInput>) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            HeightStructuresFailureMechanismSectionResult firstSectionResult = failureMechanism.SectionResults.First();
            Assert.AreSame(calculationWithOutput, firstSectionResult.Calculation);
        }

        private static void AssertClosingStructuresFailureMechanism(AssessmentSection assessmentSection)
        {
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.ClosingStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<ClosingStructuresInput>) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);

            var secondCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, secondCalculationGroup.Children.Count);
            var calculationWithoutOutput = (StructuresCalculation<ClosingStructuresInput>) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            ClosingStructuresFailureMechanismSectionResult firstSectionResult = failureMechanism.SectionResults.First();
            Assert.AreSame(calculationWithOutput, firstSectionResult.Calculation);
        }

        private static void AssertDuneErosionFailureMechanism(AssessmentSection assessmentSection)
        {
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;

            Assert.IsEmpty(failureMechanism.Calculations);

            List<DuneLocation> duneLocations = failureMechanism.DuneLocations;

            Assert.AreEqual(3, duneLocations.Count);
            Assert.IsNull(duneLocations[0].Output);

            Assert.IsNotNull(duneLocations[1].Output);
            Assert.AreEqual(CalculationConvergence.NotCalculated, duneLocations[1].Output.CalculationConvergence);
            Assert.IsNotNull(duneLocations[2].Output);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, duneLocations[2].Output.CalculationConvergence);

            Assert.AreEqual(3, failureMechanism.SectionResults.Count());
        }

        private static void AssertStabilityPointStructuresFailureMechanism(AssessmentSection assessmentSection)
        {
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.StabilityPointStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(1, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<StabilityPointStructuresInput>) firstCalculationGroup.Children[0];
            Assert.AreEqual("Calculation 1", calculationWithOutput.Name);
            Assert.IsTrue(calculationWithOutput.HasOutput);

            var secondCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, secondCalculationGroup.Children.Count);
            var calculationWithoutOutput = (StructuresCalculation<StabilityPointStructuresInput>) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            Assert.AreSame(calculationWithOutput, sectionResult.Calculation);
        }

        #region Hydraulic Boundary Database

        private static void AssertHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            Assert.NotNull(hydraulicBoundaryDatabase);
            Assert.AreEqual("1.0", hydraulicBoundaryDatabase.Version);
            Assert.AreEqual("/temp/test", hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(1, hydraulicBoundaryDatabase.Locations.Count);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations.First();
            AssertHydraulicBoundaryLocation(hydraulicBoundaryLocation);
        }

        private static void AssertHydraulicBoundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            Assert.AreEqual(13001, hydraulicBoundaryLocation.Id);
            Assert.AreEqual("test", hydraulicBoundaryLocation.Name);
            Assert.AreEqual(152.3, hydraulicBoundaryLocation.Location.X);
            Assert.AreEqual(2938.5, hydraulicBoundaryLocation.Location.Y);

            AssertHydraulicBoundaryLocationDesignWaterLevelOutput(hydraulicBoundaryLocation.DesignWaterLevelOutput);
            AssertHydraulicBoundaryLocationWaveHeightOutputOutput(hydraulicBoundaryLocation.WaveHeightOutput);
        }

        private static void AssertHydraulicBoundaryLocationDesignWaterLevelOutput(HydraulicBoundaryLocationOutput output)
        {
            Assert.AreEqual(12.4, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        private static void AssertHydraulicBoundaryLocationWaveHeightOutputOutput(HydraulicBoundaryLocationOutput output)
        {
            Assert.AreEqual(2.4, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(0, output.TargetProbability);
            Assert.AreEqual(0, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(0, output.CalculatedProbability);
            Assert.AreEqual(0, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        #endregion
    }
}