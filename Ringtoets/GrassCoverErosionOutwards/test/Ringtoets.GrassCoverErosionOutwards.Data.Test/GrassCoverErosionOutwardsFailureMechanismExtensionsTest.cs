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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismExtensionsTest
    {
        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_Always_PreviousLocationsCleared()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);

            // Precondition
            CollectionAssert.AreEqual(new[] { hydraulicBoundaryLocation }, failureMechanism.HydraulicBoundaryLocations);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(null);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundaryLocations);
        }

        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_HydraulicBoundaryDatabaseWithLocations_SetLocations()
        {
            // Setup
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 2),
                new HydraulicBoundaryLocation(2, "location 2", 3, 4)
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.AddRange(locations);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundaryLocations);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(2, failureMechanism.HydraulicBoundaryLocations.Count);

            for (var i = 0; i < locations.Length; i++)
            {
                Assert.AreEqual(locations[i].Id, failureMechanism.HydraulicBoundaryLocations[i].Id);
                Assert.AreEqual(locations[i].Name, failureMechanism.HydraulicBoundaryLocations[i].Name);
                Assert.AreEqual(locations[i].Location.X, failureMechanism.HydraulicBoundaryLocations[i].Location.X);
                Assert.AreEqual(locations[i].Location.Y, failureMechanism.HydraulicBoundaryLocations[i].Location.Y);
            }
        }

        [Test]
        public void CalculationBeta_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.CalculationBeta(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CalculationBeta_WithAssessmentSection_ReturnBeta()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 300));
            mocks.ReplayAll();

            // Call
            double beta = failureMechanism.CalculationBeta(assessmentSection);

            // Assert

            double expectedBeta = assessmentSection.FailureMechanismContribution.Norm/
                                  (failureMechanism.Contribution/100)*
                                  failureMechanism.GeneralInput.N;

            Assert.AreEqual(expectedBeta, beta);
        }
    }
}