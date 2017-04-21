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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismExtensionsTest
    {
        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsFailureMechanismExtensions.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(
                null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_LocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_Always_PreviousLocationsCleared()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);

            // Precondition
            CollectionAssert.AreEqual(new[]
            {
                hydraulicBoundaryLocation
            }, failureMechanism.HydraulicBoundaryLocations);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(Enumerable.Empty<HydraulicBoundaryLocation>());

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundaryLocations);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(locations);

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
    }
}