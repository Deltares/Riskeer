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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
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
        private static IEnumerable<TestCaseData> Locations
        {
            get
            {
                yield return new TestCaseData(
                    new ReadDuneLocation("dune location 1", new Point2D(1.0, 2.0), 0, 0, 0, 0),
                    new HydraulicBoundaryLocation(1, "hydraulic location 1", 2.0, 1.0)).SetName("DifferentCoordinates");

                yield return new TestCaseData(
                    new ReadDuneLocation("dune location 1", new Point2D(1.0, 2.0), 0, 2.2, 0, 0),
                    new HydraulicBoundaryLocation(1, "hydraulic_location_1.1", 1.0, 2.0)).SetName("DifferentOffset");
            }
        }

        [Test]
        public void SetDuneLocations_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.SetDuneLocations(null,
                                                                                             new HydraulicBoundaryLocation[0],
                                                                                             new ReadDuneLocation[0]);

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
                                                                                             new ReadDuneLocation[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_DuneLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                             new HydraulicBoundaryLocation[0],
                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocations", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_Always_PreviousDuneLocationsCleared()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation
            });

            // Precondition
            CollectionAssert.AreEqual(
                new[]
                {
                    duneLocation
                }, failureMechanism.DuneLocations);

            // Call
            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                   new HydraulicBoundaryLocation[0],
                                                                   new ReadDuneLocation[0]);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        public void SetDuneLocations_DuneLocationOffsetMatchesWithHydraulicBoundaryLocationName_DuneLocationAddedToFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("dune location 1", new Point2D(1.0, 5.3), 8, 1.1, 2.2, 3.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Location_2_1.1", 1.0, 5.3);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            DuneErosionDataSynchronizationService.SetDuneLocations(
                failureMechanism,
                new[]
                {
                    hydraulicBoundaryLocation
                }, new[]
                {
                    readDuneLocation
                });

            // Assert
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            DuneLocation duneLocation = failureMechanism.DuneLocations.First();
            Assert.AreEqual(hydraulicBoundaryLocation.Id, duneLocation.Id);
            Assert.AreEqual(readDuneLocation.Name, duneLocation.Name);
            Assert.AreEqual(readDuneLocation.Location, duneLocation.Location);
            Assert.AreEqual(readDuneLocation.Offset, duneLocation.Offset);
            Assert.AreEqual(readDuneLocation.Orientation, duneLocation.Orientation);
            Assert.AreEqual(readDuneLocation.D50, duneLocation.D50);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-0.123)]
        [TestCase(1)]
        [TestCase(123.456789)]
        public void SetDuneLocations_DuneLocationsMatchNameNotAccordingFormat_DuneLocationNotAddedLogMessage(double offset)
        {
            // Setup
            string locationName = $"Location_{offset}";

            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("dune location 1", new Point2D(1.0, 5.3), 8, offset, 2.2, 3.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 1.0, 5.3);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            Action test = () => DuneErosionDataSynchronizationService.SetDuneLocations(
                failureMechanism,
                new[]
                {
                    hydraulicBoundaryLocation
                }, new[]
                {
                    readDuneLocation
                });

            // Assert
            string expectedMessage = $"Locatie '{locationName}' moet voldoen aan het formaat 'Naam_Vaknummer_Metrering'. " +
                                     "Deze locatie is niet toegevoegd aan de hydraulische belastingen voor toetsspoor duinen.";
            TestHelper.AssertLogMessageIsGenerated(test, expectedMessage, 1);
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        [TestCaseSource(nameof(Locations))]
        public void SetDuneLocation_DuneLocationNoMatchWithHydraulicBoundaryLocation_DuneLocationNotAddedToFailureMechanism(ReadDuneLocation readDuneLocation,
                                                                                                                            HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism, new[]
            {
                hydraulicBoundaryLocation
            }, new[]
            {
                readDuneLocation
            });

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
        }

        [Test]
        public void ClearDuneLocationCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneErosionDataSynchronizationService.ClearDuneLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearDuneLocationCalculationOutput_CalculationsWithOutput_OutputClearedAndAffectedItemsReturned()
        {
            // Setup
            var duneLocations = new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);

            failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForMechanismSpecificSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForFactorizedLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();

            IEnumerable<IObservable> expectedAffectedCalculations =
                DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Call
            IEnumerable<IObservable> affected = DuneErosionDataSynchronizationService.ClearDuneLocationCalculationOutput(failureMechanism);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affected);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(failureMechanism);
        }
    }
}