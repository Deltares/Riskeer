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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.IO;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneErosionDataSynchronizationServiceTest
    {
        [Test]
        public void SetDuneLocations_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.SetDuneLocations(null,
                                                                                             Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                             Enumerable.Empty<ReadDuneLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                             null,
                                                                                             Enumerable.Empty<ReadDuneLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_ReadDuneLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            
            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                             Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("readDuneLocations", exception.ParamName);
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
            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                   Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                   Enumerable.Empty<ReadDuneLocation>());

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        public void SetDuneLocations_DuneLocationMatchesWithHydraulicBoundaryLocation_DuneLocationAddedToFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("dune location 1", new Point2D(1.0, 5.3), 8, 1.1, 2.2, 3.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "hydraulic location 1", 1.0, 5.3);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism, new [] { hydraulicBoundaryLocation }, new[] { readDuneLocation });

            // Assert
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count);

            DuneLocation duneLocation = failureMechanism.DuneLocations.First();
            Assert.AreEqual(hydraulicBoundaryLocation.Id, duneLocation.Id);
            Assert.AreEqual(readDuneLocation.Name, duneLocation.Name);
            Assert.AreEqual(readDuneLocation.Location, duneLocation.Location);
            Assert.AreEqual(readDuneLocation.Offset, duneLocation.Offset);
            Assert.AreEqual(readDuneLocation.Orientation, duneLocation.Orientation);
            Assert.AreEqual(readDuneLocation.D50, duneLocation.D50);
        }

        [Test]
        public void SetDuneLocation_DuneLocationNoMatchWithHydraulicBoundaryLocation_DuneLocationNotAddedToFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("dune location 1", new Point2D(1.0, 2.0), 0, 0, 0, 0);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "hydraulic location 1", 2.0, 1.0);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism, new[] { hydraulicBoundaryLocation }, new[] { readDuneLocation });

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        public void ClearDuneLocationOutput_locationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.ClearDuneLocationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void ClearDuneLocationOutput_LocationWithOutput_OutputClearedAndAffectedItemReturned()
        {
            // Setup
            var location = new TestDuneLocation
            {
                Output = new DuneLocationOutput(0, 0, 0, 0, 0, 0, 0, CalculationConvergence.NotCalculated)
            };

            // Call
            IEnumerable<IObservable> affected = DuneErosionDataSynchronizationService.ClearDuneLocationOutput(
                new ObservableList<DuneLocation>
                {
                    location
                });

            // Assert
            Assert.IsNull(location.Output);
            CollectionAssert.AreEqual(new[] { location }, affected);
        }
    }
}