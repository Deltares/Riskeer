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
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Ringtoets.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismExtensionsTest
    {
        [Test]
        public void SetDuneLocations_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneErosionFailureMechanismExtensions.SetDuneLocations(null, new HydraulicBoundaryDatabase(), Enumerable.Empty<DuneLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_Always_PreviousDuneLocationsCleared()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(duneLocation);

            // Precondition
            CollectionAssert.AreEqual(new[]
                                      {
                                          duneLocation
                                      }, failureMechanism.DuneLocations);

            // call
            failureMechanism.SetDuneLocations(null, Enumerable.Empty<DuneLocation>());

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        public void SetDuneLocations_DuneLocationMatchesWithHydraulicBoundaryLocation_DuneLocationAddedToFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocation = new DuneLocation("dune location 1", 1.0, 5.3, 0, 0, 0, 0);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "hydraulic location 1", 1.0, 5.3);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            
            // Call
            failureMechanism.SetDuneLocations(hydraulicBoundaryDatabase, new [] { duneLocation });

            // Assert
            CollectionAssert.AreEqual(new[]
                                      {
                                          duneLocation
                                      }, failureMechanism.DuneLocations);
        }

        [Test]
        public void SetDuneLocation_DuneLocationNoMatchWithHydraulicBoundaryLocation_DuneLocationNotAddedToFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocation = new DuneLocation("dune location 1", 1.0, 2.0, 0, 0, 0, 0);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "hydraulic location 1", 2.0, 1.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            failureMechanism.SetDuneLocations(hydraulicBoundaryDatabase, new[] { duneLocation });

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }
    }
}