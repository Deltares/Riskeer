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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

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
            double designWaterLevel = 12.4;

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
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

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
        }
    }
}