// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
        public void SetDuneLocations_ReadDuneLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                                  new HydraulicBoundaryLocation[0],
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readDuneLocations", exception.ParamName);
        }

        [Test]
        public void SetDuneLocations_HydraulicBoundaryLocationNameMatchesReadDuneLocationName_DuneLocationAddedToFailureMechanism()
        {
            // Setup
            const string name = "001-01_0001_SCHR_02_jr001000";

            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation(name, new Point2D(random.NextDouble(), random.NextDouble()), random.Next(), random.Next());
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(random.Next(), name, random.NextDouble(), random.NextDouble());

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
            Assert.AreSame(hydraulicBoundaryLocation, duneLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(name, duneLocation.Name);
            Assert.AreEqual(readDuneLocation.CoastalAreaId, duneLocation.CoastalAreaId);
            Assert.AreEqual(readDuneLocation.Offset, duneLocation.Offset);
        }

        [Test]
        public void SetDuneLocations_HydraulicBoundaryLocationNameNotMatchingReadDuneLocationName_DuneLocationNotAddedToFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism();
            var readDuneLocation = new ReadDuneLocation("Location 1", new Point2D(random.NextDouble(), random.NextDouble()), random.Next(), random.NextDouble());
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(random.Next(), "Location 2", random.NextDouble(), random.NextDouble());

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