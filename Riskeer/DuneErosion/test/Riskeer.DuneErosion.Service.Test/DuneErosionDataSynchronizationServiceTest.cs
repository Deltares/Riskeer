﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.IO;

namespace Riskeer.DuneErosion.Service.Test
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
            void Call() => DuneErosionDataSynchronizationService.SetDuneLocations(null,
                                                                                  new HydraulicBoundaryLocation[0],
                                                                                  new ReadDuneLocation[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                  null,
                                                                                  new ReadDuneLocation[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_DuneLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                  new HydraulicBoundaryLocation[0],
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            var locationName = $"Location_{offset}";

            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("dune location 1", new Point2D(1.0, 5.3), 8, offset, 2.2, 3.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 1.0, 5.3);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            void Call() =>
                DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism, new[]
                {
                    hydraulicBoundaryLocation
                }, new[]
                {
                    readDuneLocation
                });

            // Assert
            string expectedMessage = $"Locatie '{locationName}' moet voldoen aan het formaat 'Naam_Vaknummer_Metrering'. " +
                                     "Deze locatie is niet toegevoegd aan de hydraulische belastingen voor het toetsspoor duinen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
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
            void Call() => DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput((DuneErosionFailureMechanism) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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

            var duneLocationCalculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability(0.1);
            var duneLocationCalculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability(0.01);

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    duneLocationCalculationsForTargetProbability1,
                    duneLocationCalculationsForTargetProbability2
                }
            };

            failureMechanism.SetDuneLocations(duneLocations);

            duneLocationCalculationsForTargetProbability1.DuneLocationCalculations.First().Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculationsForTargetProbability2.DuneLocationCalculations.First().Output = new TestDuneLocationCalculationOutput();

            IEnumerable<IObservable> expectedAffectedCalculations =
                DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Call
            IEnumerable<IObservable> affected = DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput(failureMechanism);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affected);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(failureMechanism);
        }

        [Test]
        public void ClearDuneLocationCalculationsOutput_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput((IEnumerable<DuneLocationCalculation>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void ClearDuneLocationCalculationsOutput_CalculationsWithOutput_OutputClearedAndAffectedItemsReturned()
        {
            // Setup
            var duneLocationCalculationWithOutput1 = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = new TestDuneLocationCalculationOutput()
            };

            var duneLocationCalculationWithOutput2 = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = new TestDuneLocationCalculationOutput()
            };

            DuneLocationCalculation[] calculations =
            {
                new DuneLocationCalculation(new TestDuneLocation()),
                duneLocationCalculationWithOutput1,
                new DuneLocationCalculation(new TestDuneLocation()),
                duneLocationCalculationWithOutput2,
                new DuneLocationCalculation(new TestDuneLocation())
            };

            IEnumerable<IObservable> expectedAffectedCalculations = new[]
            {
                duneLocationCalculationWithOutput1,
                duneLocationCalculationWithOutput2
            };

            // Call
            IEnumerable<IObservable> affected = DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput(calculations);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affected);
            Assert.IsNull(duneLocationCalculationWithOutput1.Output);
            Assert.IsNull(duneLocationCalculationWithOutput2.Output);
        }
    }
}