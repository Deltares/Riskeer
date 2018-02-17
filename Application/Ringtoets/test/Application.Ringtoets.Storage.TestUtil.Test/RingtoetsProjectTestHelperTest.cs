﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
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
        public void GetFullTestProject_Always_ReturnsFullProject()
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
            Assert.AreEqual("12-2", assessmentSection.Id);

            FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
            Assert.AreEqual(1.0 / 10, contribution.LowerLimitNorm);
            Assert.AreEqual(1.0 / 1000000, contribution.SignalingNorm);
            Assert.AreEqual(NormType.Signaling, contribution.NormativeNorm);

            AssertHydraulicBoundaryDatabase(assessmentSection.HydraulicBoundaryDatabase);

            AssertPipingFailureMechanism(assessmentSection);

            AssertMacroStabilityInwardsFailureMechanism(assessmentSection);

            AssertMacroStabilityOutwardsFailureMechanism(assessmentSection);

            AssertGrassCoverErosionInwardsFailureMechanism(assessmentSection);

            AssertGrassCoverErosionOutwardsFailureMechanism(assessmentSection);

            AssertStabilityStoneCoverFailureMechanism(assessmentSection);

            AssertWaveImpactAsphaltCoverFailureMechanism(assessmentSection);

            AssertHeightStructuresFailureMechanism(assessmentSection);

            AssertClosingStructuresFailureMechanism(assessmentSection);

            AssertStabilityPointStructuresFailureMechanism(assessmentSection);

            AssertPipingStructureFailureMechanism(assessmentSection);

            AssertDuneErosionFailureMechanism(assessmentSection);
        }

        private static void AssertPipingFailureMechanism(AssessmentSection assessmentSection)
        {
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            Assert.AreEqual(0.9, failureMechanism.PipingProbabilityAssessmentInput.A);
            Assert.AreEqual("some/path/to/stochasticSoilModelFile", failureMechanism.StochasticSoilModels.SourcePath);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            PipingStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual("modelName", soilModel.Name);

            PipingStochasticSoilProfile[] stochasticSoilProfiles = soilModel.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(2, stochasticSoilProfiles.Length);
            PipingStochasticSoilProfile stochasticSoilProfile1 = stochasticSoilProfiles[0];
            Assert.AreEqual(0.2, stochasticSoilProfile1.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, stochasticSoilProfile1.SoilProfile.SoilProfileSourceType);
            PipingStochasticSoilProfile stochasticSoilProfile2 = stochasticSoilProfiles[1];
            Assert.AreEqual(0.8, stochasticSoilProfile2.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticSoilProfile2.SoilProfile.SoilProfileSourceType);

            Assert.AreEqual("some/path/to/surfaceLineFile", failureMechanism.SurfaceLines.SourcePath);
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count);
            PipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines.First();
            Assert.AreEqual("Surface line", surfaceLine.Name);
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
            Assert.AreSame(surfaceLine.Points.ElementAt(1), surfaceLine.DikeToeAtRiver);
            Assert.AreSame(surfaceLine.Points.ElementAt(4), surfaceLine.DikeToeAtPolder);
            Assert.AreSame(surfaceLine.Points.ElementAt(5), surfaceLine.DitchDikeSide);
            Assert.AreSame(surfaceLine.Points.ElementAt(6), surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(surfaceLine.Points.ElementAt(7), surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(surfaceLine.Points.ElementAt(8), surfaceLine.DitchPolderSide);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (PipingCalculationScenario) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            Assert.IsFalse(calculationWithOutput.InputParameters.UseAssessmentLevelManualInput);

            var calculationWithAssessmentLevelAndOutput = (PipingCalculationScenario) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithAssessmentLevelAndOutput.HasOutput);
            Assert.IsTrue(calculationWithAssessmentLevelAndOutput.InputParameters.UseAssessmentLevelManualInput);

            var emptyCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, emptyCalculationGroup.Children.Count);

            var calculationWithoutOutput = (PipingCalculationScenario) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);
        }

        private static void AssertMacroStabilityInwardsFailureMechanism(AssessmentSection assessmentSection)
        {
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            Assert.AreEqual(0.9, failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A);
            Assert.AreEqual("some/path/to/stochasticSoilModelFile", failureMechanism.StochasticSoilModels.SourcePath);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            MacroStabilityInwardsStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual("MacroStabilityInwards model name", soilModel.Name);

            MacroStabilityInwardsStochasticSoilProfile[] stochasticSoilProfiles = soilModel.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(2, stochasticSoilProfiles.Length);
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile1 = stochasticSoilProfiles[0];
            Assert.AreEqual(0.3, stochasticSoilProfile1.Probability);
            Assert.IsInstanceOf<MacroStabilityInwardsSoilProfile1D>(stochasticSoilProfile1.SoilProfile);
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile2 = stochasticSoilProfiles[1];
            Assert.AreEqual(0.7, stochasticSoilProfile2.Probability);
            Assert.IsInstanceOf<MacroStabilityInwardsSoilProfile2D>(stochasticSoilProfile2.SoilProfile);
            CollectionAssert.IsNotEmpty(((MacroStabilityInwardsSoilProfile2D) stochasticSoilProfile2.SoilProfile).PreconsolidationStresses);

            Assert.AreEqual("some/path/to/surfaceLineFile", failureMechanism.SurfaceLines.SourcePath);
            Assert.AreEqual(1, failureMechanism.SurfaceLines.Count);
            MacroStabilityInwardsSurfaceLine surfaceLine = failureMechanism.SurfaceLines.First();
            Assert.AreEqual("MacroStabilityInwards surface line", surfaceLine.Name);
            Assert.AreEqual(new Point2D(4.4, 6.6), surfaceLine.ReferenceLineIntersectionWorldPoint);
            var geometryPoints = new[]
            {
                new Point3D(1.0, 6.0, -2.3),
                new Point3D(2.8, 6.0, -2.3),
                new Point3D(3.6, 6.0, 3.4),
                new Point3D(4.2, 6.0, 3.5),
                new Point3D(5.0, 6.0, 0.5),
                new Point3D(6.8, 6.0, 0.5),
                new Point3D(7.6, 6.0, 0.2),
                new Point3D(8.4, 6.0, 0.25),
                new Point3D(9.2, 6.0, 0.5),
                new Point3D(10.0, 6.0, 0.5),
                new Point3D(11.0, 6.0, -2.3),
                new Point3D(12.8, 6.0, -2.3),
                new Point3D(13.6, 6.0, 3.4)
            };
            CollectionAssert.AreEqual(geometryPoints, surfaceLine.Points);

            Assert.AreEqual(surfaceLine.Points.ElementAt(12), surfaceLine.SurfaceLevelOutside);
            Assert.AreEqual(surfaceLine.Points.ElementAt(11), surfaceLine.DikeToeAtRiver);
            Assert.AreEqual(surfaceLine.Points.ElementAt(10), surfaceLine.DikeTopAtPolder);
            Assert.AreEqual(surfaceLine.Points.ElementAt(9), surfaceLine.DikeTopAtRiver);
            Assert.AreEqual(surfaceLine.Points.ElementAt(8), surfaceLine.ShoulderBaseInside);
            Assert.AreEqual(surfaceLine.Points.ElementAt(7), surfaceLine.ShoulderTopInside);
            Assert.AreEqual(surfaceLine.Points.ElementAt(6), surfaceLine.DikeToeAtPolder);
            Assert.AreEqual(surfaceLine.Points.ElementAt(5), surfaceLine.DitchDikeSide);
            Assert.AreEqual(surfaceLine.Points.ElementAt(4), surfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(surfaceLine.Points.ElementAt(3), surfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(surfaceLine.Points.ElementAt(2), surfaceLine.DitchPolderSide);
            Assert.AreEqual(surfaceLine.Points.ElementAt(1), surfaceLine.SurfaceLevelInside);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (MacroStabilityInwardsCalculationScenario) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            Assert.IsFalse(calculationWithOutput.InputParameters.UseAssessmentLevelManualInput);

            var calculationWithAssessmentLevelAndOutput = (MacroStabilityInwardsCalculationScenario) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithAssessmentLevelAndOutput.HasOutput);
            Assert.IsTrue(calculationWithAssessmentLevelAndOutput.InputParameters.UseAssessmentLevelManualInput);

            var emptyCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, emptyCalculationGroup.Children.Count);

            var calculationWithoutOutput = (MacroStabilityInwardsCalculationScenario) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);
        }

        private static void AssertMacroStabilityOutwardsFailureMechanism(AssessmentSection assessmentSection)
        {
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            Assert.AreEqual(0.6, failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(AssessmentSection assessmentSection)
        {
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            Assert.AreEqual(15.0, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (GrassCoverErosionInwardsCalculation) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            GrassCoverErosionInwardsOutput calculationOutput = calculationWithOutput.Output;
            Assert.IsFalse(calculationOutput.OvertoppingOutput.HasGeneralResult);
            Assert.IsFalse(calculationOutput.DikeHeightOutput.HasGeneralResult);
            Assert.IsFalse(calculationOutput.OvertoppingRateOutput.HasGeneralResult);

            var calculationWithOutputAndGeneralResult = (GrassCoverErosionInwardsCalculation) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithOutputAndGeneralResult.HasOutput);
            GrassCoverErosionInwardsOutput outputCalculationWithGeneralResult = calculationWithOutputAndGeneralResult.Output;
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(outputCalculationWithGeneralResult.OvertoppingOutput.GeneralResult);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(outputCalculationWithGeneralResult.DikeHeightOutput.GeneralResult);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(outputCalculationWithGeneralResult.OvertoppingRateOutput.GeneralResult);

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
            Assert.AreEqual(15.0, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

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
            Assert.AreEqual(15.0, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

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
            Assert.AreEqual(1337.0,
                            failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL,
                            failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL.GetAccuracy());
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
            Assert.AreEqual(5.0, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.HeightStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<HeightStructuresInput>) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            Assert.IsFalse(calculationWithOutput.Output.HasGeneralResult);

            var calculationWithOutputAndGeneralResult = (StructuresCalculation<HeightStructuresInput>) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithOutputAndGeneralResult.HasOutput);
            Assert.IsTrue(calculationWithOutputAndGeneralResult.Output.HasGeneralResult);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(calculationWithOutputAndGeneralResult.Output.GeneralResult);

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
            Assert.AreEqual(6, failureMechanism.GeneralInput.N2A);

            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.ClosingStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<ClosingStructuresInput>) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            Assert.IsFalse(calculationWithOutput.Output.HasGeneralResult);

            var calculationWithOutputAndGeneralResult = (StructuresCalculation<ClosingStructuresInput>) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithOutputAndGeneralResult.HasOutput);
            Assert.IsTrue(calculationWithOutputAndGeneralResult.Output.HasGeneralResult);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(calculationWithOutputAndGeneralResult.Output.GeneralResult);

            ClosingStructuresFailureMechanismSectionResult firstSectionResult = failureMechanism.SectionResults.First();
            Assert.AreSame(calculationWithOutput, firstSectionResult.Calculation);

            var secondCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, secondCalculationGroup.Children.Count);
            var calculationWithoutOutput = (StructuresCalculation<ClosingStructuresInput>) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);
        }

        private static void AssertDuneErosionFailureMechanism(AssessmentSection assessmentSection)
        {
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            Assert.AreEqual(5.5, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            CollectionAssert.IsEmpty(failureMechanism.Calculations);

            List<DuneLocation> duneLocations = failureMechanism.DuneLocations;

            Assert.AreEqual(3, duneLocations.Count);
            Assert.IsNull(duneLocations[0].Calculation.Output);

            Assert.IsNotNull(duneLocations[1].Calculation.Output);
            Assert.AreEqual(CalculationConvergence.NotCalculated, duneLocations[1].Calculation.Output.CalculationConvergence);
            Assert.IsNotNull(duneLocations[2].Calculation.Output);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, duneLocations[2].Calculation.Output.CalculationConvergence);

            Assert.AreEqual(3, failureMechanism.SectionResults.Count());
        }

        private static void AssertStabilityPointStructuresFailureMechanism(AssessmentSection assessmentSection)
        {
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            Assert.AreEqual(8.0, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, failureMechanism.StabilityPointStructures.Count);

            Assert.NotNull(failureMechanism.CalculationsGroup);
            Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);

            var firstCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual(2, firstCalculationGroup.Children.Count);

            var calculationWithOutput = (StructuresCalculation<StabilityPointStructuresInput>) firstCalculationGroup.Children[0];
            Assert.IsTrue(calculationWithOutput.HasOutput);
            Assert.IsFalse(calculationWithOutput.Output.HasGeneralResult);

            var calculationWithOutputAndGeneralResult = (StructuresCalculation<StabilityPointStructuresInput>) firstCalculationGroup.Children[1];
            Assert.IsTrue(calculationWithOutputAndGeneralResult.HasOutput);
            Assert.IsTrue(calculationWithOutputAndGeneralResult.Output.HasGeneralResult);
            AssertGeneralResultTopLevelFaultTreeIllustrationPoint(calculationWithOutputAndGeneralResult.Output.GeneralResult);

            var secondCalculationGroup = (CalculationGroup) failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual(0, secondCalculationGroup.Children.Count);
            var calculationWithoutOutput = (StructuresCalculation<StabilityPointStructuresInput>) failureMechanism.CalculationsGroup.Children[2];
            Assert.IsFalse(calculationWithoutOutput.HasOutput);

            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults2.First();
            Assert.AreSame(calculationWithOutput, sectionResult.Calculation);
        }

        private static void AssertPipingStructureFailureMechanism(AssessmentSection assessmentSection)
        {
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            Assert.AreEqual(12.5, failureMechanism.N, failureMechanism.N.GetAccuracy());
        }

        private static void AssertGeneralResultTopLevelFaultTreeIllustrationPoint(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            WindDirection actualGoverningWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual("GoverningWindDirection", actualGoverningWindDirection.Name);
            Assert.AreEqual(180, actualGoverningWindDirection.Angle, actualGoverningWindDirection.Angle.GetAccuracy());

            Stochast stochast = generalResult.Stochasts.Single();
            Assert.AreEqual("Stochast", stochast.Name);
            Assert.AreEqual(0.9, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(0.1, stochast.Duration, stochast.Duration.GetAccuracy());

            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint =
                generalResult.TopLevelIllustrationPoints.Single();
            Assert.AreEqual("ClosingSituation", topLevelFaultTreeIllustrationPoint.ClosingSituation);
            Assert.AreEqual("WindDirection", topLevelFaultTreeIllustrationPoint.WindDirection.Name);
            Assert.AreEqual(120, topLevelFaultTreeIllustrationPoint.WindDirection.Angle,
                            topLevelFaultTreeIllustrationPoint.WindDirection.Angle.GetAccuracy());

            IllustrationPointNode node = topLevelFaultTreeIllustrationPoint.FaultTreeNodeRoot;
            Assert.AreEqual(0, node.Children.Count());

            var faultTreeIllustrationPoint = node.Data as FaultTreeIllustrationPoint;
            Assert.IsNotNull(faultTreeIllustrationPoint);
            Assert.AreEqual("FaultTreeIllustrationPoint", faultTreeIllustrationPoint.Name);
            Assert.AreEqual(0.5, faultTreeIllustrationPoint.Beta, faultTreeIllustrationPoint.Beta.GetAccuracy());

            Stochast innerStochast = faultTreeIllustrationPoint.Stochasts.Single();
            Assert.AreEqual("Stochast", innerStochast.Name);
            Assert.AreEqual(0.9, innerStochast.Alpha, innerStochast.Alpha.GetAccuracy());
            Assert.AreEqual(0.1, innerStochast.Duration, innerStochast.Duration.GetAccuracy());
        }

        #region Hydraulic Boundary Database

        private static void AssertHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            Assert.NotNull(hydraulicBoundaryDatabase);
            Assert.AreEqual("1.0", hydraulicBoundaryDatabase.Version);
            Assert.AreEqual("/temp/test", hydraulicBoundaryDatabase.FilePath);
            Assert.IsTrue(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsTrue(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.AreEqual("/temp/preprocessor", hydraulicBoundaryDatabase.PreprocessorDirectory);
            Assert.AreEqual(2, hydraulicBoundaryDatabase.Locations.Count);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations[0];
            Assert.AreEqual(13001, hydraulicBoundaryLocation.Id);
            Assert.AreEqual("test", hydraulicBoundaryLocation.Name);
            Assert.AreEqual(152.3, hydraulicBoundaryLocation.Location.X);
            Assert.AreEqual(2938.5, hydraulicBoundaryLocation.Location.Y);

            HydraulicBoundaryLocationCalculation designWaterLevelCalculation = hydraulicBoundaryLocation.DesignWaterLevelCalculation1;
            Assert.IsFalse(designWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationDesignWaterLevelCalculation(designWaterLevelCalculation);

            designWaterLevelCalculation = hydraulicBoundaryLocation.DesignWaterLevelCalculation3;
            Assert.IsFalse(designWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationDesignWaterLevelCalculation(designWaterLevelCalculation);

            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation.DesignWaterLevelCalculation2);
            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation.DesignWaterLevelCalculation4);

            HydraulicBoundaryLocationCalculation waveHeightCalculation = hydraulicBoundaryLocation.WaveHeightCalculation1;
            Assert.IsFalse(waveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationWaveHeightCalculation(waveHeightCalculation);

            waveHeightCalculation = hydraulicBoundaryLocation.WaveHeightCalculation3;
            Assert.IsFalse(waveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationWaveHeightCalculation(waveHeightCalculation);

            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation.WaveHeightCalculation2);
            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation.WaveHeightCalculation4);

            HydraulicBoundaryLocation hydraulicBoundaryLocationWithIllustrationPoints = hydraulicBoundaryDatabase.Locations[1];
            Assert.AreEqual(13002, hydraulicBoundaryLocationWithIllustrationPoints.Id);
            Assert.AreEqual("test2", hydraulicBoundaryLocationWithIllustrationPoints.Name);
            Assert.AreEqual(135.2, hydraulicBoundaryLocationWithIllustrationPoints.Location.X);
            Assert.AreEqual(5293.8, hydraulicBoundaryLocationWithIllustrationPoints.Location.Y);

            HydraulicBoundaryLocationCalculation designWaterLevelCalculationWithIllustrationPoints =
                hydraulicBoundaryLocationWithIllustrationPoints.DesignWaterLevelCalculation1;
            Assert.IsTrue(designWaterLevelCalculationWithIllustrationPoints.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationDesignWaterLevelCalculation(designWaterLevelCalculationWithIllustrationPoints);
            AssertGeneralResultTopLevelSubMechanismIllustrationPoint(designWaterLevelCalculationWithIllustrationPoints.Output.GeneralResult);

            designWaterLevelCalculationWithIllustrationPoints =
                hydraulicBoundaryLocationWithIllustrationPoints.DesignWaterLevelCalculation3;
            Assert.IsTrue(designWaterLevelCalculationWithIllustrationPoints.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationDesignWaterLevelCalculation(designWaterLevelCalculationWithIllustrationPoints);
            AssertGeneralResultTopLevelSubMechanismIllustrationPoint(designWaterLevelCalculationWithIllustrationPoints.Output.GeneralResult);

            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationWithIllustrationPoints.DesignWaterLevelCalculation2);
            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationWithIllustrationPoints.DesignWaterLevelCalculation4);

            HydraulicBoundaryLocationCalculation waveHeightCalculationWithIllustrationPoints =
                hydraulicBoundaryLocationWithIllustrationPoints.WaveHeightCalculation1;
            Assert.IsTrue(waveHeightCalculationWithIllustrationPoints.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationWaveHeightCalculation(waveHeightCalculationWithIllustrationPoints);
            AssertGeneralResultTopLevelSubMechanismIllustrationPoint(waveHeightCalculationWithIllustrationPoints.Output.GeneralResult);

            waveHeightCalculationWithIllustrationPoints =
                hydraulicBoundaryLocationWithIllustrationPoints.WaveHeightCalculation3;
            Assert.IsTrue(waveHeightCalculationWithIllustrationPoints.InputParameters.ShouldIllustrationPointsBeCalculated);
            AssertHydraulicBoundaryLocationWaveHeightCalculation(waveHeightCalculationWithIllustrationPoints);
            AssertGeneralResultTopLevelSubMechanismIllustrationPoint(waveHeightCalculationWithIllustrationPoints.Output.GeneralResult);

            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationWithIllustrationPoints.WaveHeightCalculation2);
            AssertSimpleHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationWithIllustrationPoints.WaveHeightCalculation4);
        }

        private static void AssertSimpleHydraulicBoundaryLocationCalculation(HydraulicBoundaryLocationCalculation calculation)
        {
            Assert.IsFalse(calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.IsFalse(calculation.HasOutput);
        }

        private static void AssertHydraulicBoundaryLocationDesignWaterLevelCalculation(HydraulicBoundaryLocationCalculation calculation)
        {
            HydraulicBoundaryLocationOutput output = calculation.Output;
            Assert.AreEqual(12.4, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }

        private static void AssertHydraulicBoundaryLocationWaveHeightCalculation(HydraulicBoundaryLocationCalculation calculation)
        {
            HydraulicBoundaryLocationOutput output = calculation.Output;
            Assert.AreEqual(2.4, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(0, output.TargetProbability);
            Assert.AreEqual(0, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(0, output.CalculatedProbability);
            Assert.AreEqual(0, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
        }

        private static void AssertGeneralResultTopLevelSubMechanismIllustrationPoint(GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            WindDirection actualGoverningWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual("SSE", actualGoverningWindDirection.Name);
            Assert.AreEqual(120, actualGoverningWindDirection.Angle, actualGoverningWindDirection.Angle.GetAccuracy());

            Stochast stochast = generalResult.Stochasts.Single();
            Assert.AreEqual("Name of a stochast", stochast.Name);
            Assert.AreEqual(37, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(13, stochast.Duration, stochast.Duration.GetAccuracy());

            TopLevelSubMechanismIllustrationPoint actualTopLevelSubMechanismIllustrationPoint =
                generalResult.TopLevelIllustrationPoints.Single();
            Assert.AreEqual("Closing situation", actualTopLevelSubMechanismIllustrationPoint.ClosingSituation);
            Assert.AreEqual("60", actualTopLevelSubMechanismIllustrationPoint.WindDirection.Name);
            Assert.AreEqual(60, actualTopLevelSubMechanismIllustrationPoint.WindDirection.Angle,
                            actualTopLevelSubMechanismIllustrationPoint.WindDirection.Angle.GetAccuracy());

            SubMechanismIllustrationPoint illustrationPoint = actualTopLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
            Assert.AreEqual("Name of illustrationPoint", illustrationPoint.Name);
            Assert.AreEqual(3, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());

            SubMechanismIllustrationPointStochast illustrationPointStochast = illustrationPoint.Stochasts.Single();
            Assert.AreEqual("Name of a stochast", illustrationPointStochast.Name);
            Assert.AreEqual(10, illustrationPointStochast.Duration, illustrationPointStochast.Duration.GetAccuracy());
            Assert.AreEqual(9, illustrationPointStochast.Alpha, illustrationPointStochast.Alpha.GetAccuracy());
            Assert.AreEqual(8, illustrationPointStochast.Realization, illustrationPointStochast.Realization.GetAccuracy());

            IllustrationPointResult illustrationPointResult = illustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual("Description of result", illustrationPointResult.Description);
            Assert.AreEqual(5, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());
        }

        #endregion
    }
}