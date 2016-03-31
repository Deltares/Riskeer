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
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.TestUtil.Test
{
    [TestFixture]
    public class RingtoetsProjectHelperTest
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
            Project project = RingtoetsProjectHelper.GetFullTestProject();

            // Assert
            Assert.AreEqual(expectedProjectName, project.Name);
            Assert.AreEqual(expectedDescription, project.Description);

            AssessmentSection assessmentSection = project.Items.OfType<AssessmentSection>().FirstOrDefault();
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
        }
    }
}