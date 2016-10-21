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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
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
            string expectedProjectName = "tempProjectFile";
            string expectedDescription = "description";
            string expectedAssessmentSectionName = "assessmentSection";

            string hydraulicDatabaseFilePath = "/temp/test";
            string hydraulicDatabaseVersion = "1.0";

            long locationId = 13001;
            string locationName = "test";
            double locationX = 152.3;
            double locationY = 2938.5;
            RoundedDouble designWaterLevel = (RoundedDouble) 12.4;
            RoundedDouble waveHeight = (RoundedDouble) 2.4;

            // Call
            RingtoetsProject project = RingtoetsProjectTestHelper.GetFullTestProject();

            // Assert
            Assert.AreEqual(expectedProjectName, project.Name);
            Assert.AreEqual(expectedDescription, project.Description);

            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.NotNull(assessmentSection);
            Assert.AreEqual(expectedAssessmentSectionName, assessmentSection.Name);

            Assert.NotNull(assessmentSection.HydraulicBoundaryDatabase);
            Assert.AreEqual(hydraulicDatabaseVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
            Assert.AreEqual(hydraulicDatabaseFilePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(1, assessmentSection.HydraulicBoundaryDatabase.Locations.Count);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(locationId, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(locationName, hydraulicBoundaryLocation.Name);
            Assert.AreEqual(locationX, hydraulicBoundaryLocation.Location.X);
            Assert.AreEqual(locationY, hydraulicBoundaryLocation.Location.Y);
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            Assert.AreEqual(1, pipingFailureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel soilModel = pipingFailureMechanism.StochasticSoilModels[0];
            Assert.AreEqual(-1, soilModel.Id);
            Assert.AreEqual("modelName", soilModel.Name);
            Assert.AreEqual("modelSegmentName", soilModel.SegmentName);
            Assert.AreEqual(2, soilModel.StochasticSoilProfiles.Count);
            StochasticSoilProfile stochasticSoilProfile1 = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(0.2, stochasticSoilProfile1.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, stochasticSoilProfile1.SoilProfileType);
            Assert.AreEqual(-1, stochasticSoilProfile1.SoilProfileId);
            StochasticSoilProfile stochasticSoilProfile2 = soilModel.StochasticSoilProfiles[1];
            Assert.AreEqual(0.8, stochasticSoilProfile2.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, stochasticSoilProfile2.SoilProfileType);
            Assert.AreEqual(-1, stochasticSoilProfile2.SoilProfileId);

            Assert.AreEqual(1, pipingFailureMechanism.SurfaceLines.Count);
            RingtoetsPipingSurfaceLine surfaceLine = pipingFailureMechanism.SurfaceLines.First();
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

            Assert.NotNull(pipingFailureMechanism.CalculationsGroup);
            Assert.AreEqual(3, pipingFailureMechanism.CalculationsGroup.Children.Count);
            Assert.AreEqual(1, ((CalculationGroup) pipingFailureMechanism.CalculationsGroup.Children[0]).Children.Count);
            Assert.IsInstanceOf<PipingCalculationScenario>(((CalculationGroup) pipingFailureMechanism.CalculationsGroup.Children[0]).Children[0]);
            Assert.AreEqual(0, ((CalculationGroup) pipingFailureMechanism.CalculationsGroup.Children[1]).Children.Count);
            Assert.IsInstanceOf<PipingCalculationScenario>(pipingFailureMechanism.CalculationsGroup.Children[2]);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            Assert.NotNull(grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            Assert.AreEqual(3, grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children.Count);

            Assert.AreEqual(1, ((CalculationGroup) grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children[0]).Children.Count);
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(((CalculationGroup) grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children[0]).Children[0]);
            Assert.AreEqual(0, ((CalculationGroup) grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children[1]).Children.Count);
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children[2]);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            Assert.AreEqual(2, grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations.Count);
            Assert.AreEqual(2, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles.Count);
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculation>(
                ((CalculationGroup) grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children[0]).Children[0]);
            Assert.AreEqual(0, ((CalculationGroup) grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children[1]).Children.Count);
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculation>(
                grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children[2]);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            Assert.AreEqual(2, stabilityStoneCoverFailureMechanism.ForeshoreProfiles.Count);
            Assert.NotNull(stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            Assert.AreEqual(3, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children.Count);

            Assert.AreEqual(1, ((CalculationGroup) stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children[0]).Children.Count);
            Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsCalculation>(
                ((CalculationGroup) stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children[0]).Children[0]);

            var stabilityStoneCoverCalculationWithoutOutput = ((CalculationGroup) stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children[0])
                                                                  .Children[0] as StabilityStoneCoverWaveConditionsCalculation;
            Assert.NotNull(stabilityStoneCoverCalculationWithoutOutput);
            Assert.IsFalse(stabilityStoneCoverCalculationWithoutOutput.HasOutput);

            Assert.AreEqual(0, ((CalculationGroup) stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children[1]).Children.Count);

            var stabilityStoneCoverCalculationWithOutput = stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children[2]
                                                           as StabilityStoneCoverWaveConditionsCalculation;
            Assert.NotNull(stabilityStoneCoverCalculationWithOutput);
            Assert.IsTrue(stabilityStoneCoverCalculationWithOutput.HasOutput);
            Assert.AreEqual(2, stabilityStoneCoverCalculationWithOutput.Output.BlocksOutput.Count());
            Assert.AreEqual(2, stabilityStoneCoverCalculationWithOutput.Output.ColumnsOutput.Count());

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            Assert.AreEqual(2, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles.Count);
            Assert.NotNull(waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            Assert.AreEqual(3, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children.Count);

            Assert.AreEqual(1, ((CalculationGroup) waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children[0]).Children.Count);

            var waveImpactAsphaltCoverCalculationWithoutOutput = ((CalculationGroup) waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children[0])
                                                                     .Children[0] as WaveImpactAsphaltCoverWaveConditionsCalculation;
            Assert.NotNull(waveImpactAsphaltCoverCalculationWithoutOutput);
            Assert.IsFalse(waveImpactAsphaltCoverCalculationWithoutOutput.HasOutput);

            Assert.AreEqual(0, ((CalculationGroup) waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children[1]).Children.Count);

            var waveImpactAsphaltCoverCalculationWithOutput = waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children[2]
                                                              as WaveImpactAsphaltCoverWaveConditionsCalculation;
            Assert.NotNull(waveImpactAsphaltCoverCalculationWithOutput);
            Assert.IsTrue(waveImpactAsphaltCoverCalculationWithOutput.HasOutput);
            Assert.AreEqual(2, waveImpactAsphaltCoverCalculationWithOutput.Output.Items.Count());
            Assert.AreEqual(2, waveImpactAsphaltCoverCalculationWithOutput.Output.Items.Count());

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            Assert.AreEqual(2, heightStructuresFailureMechanism.ForeshoreProfiles.Count);
            Assert.AreEqual(2, heightStructuresFailureMechanism.HeightStructures.Count);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            Assert.AreEqual(2, closingStructuresFailureMechanism.ForeshoreProfiles.Count);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            Assert.AreEqual(2, stabilityPointStructuresFailureMechanism.ForeshoreProfiles.Count);
        }
    }
}