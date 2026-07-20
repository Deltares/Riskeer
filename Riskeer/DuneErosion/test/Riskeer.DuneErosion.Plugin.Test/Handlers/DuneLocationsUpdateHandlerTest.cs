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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Commands;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Plugin.Handlers;

namespace Riskeer.DuneErosion.Plugin.Test.Handlers
{
    [TestFixture]
    public class DuneLocationsUpdateHandlerTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => new DuneLocationsUpdateHandler(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new DuneLocationsUpdateHandler(viewCommands, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            // Call
            var handler = new DuneLocationsUpdateHandler(viewCommands, new DuneErosionFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IDuneLocationsUpdateHandler>(handler);
        }

        [Test]
        public void AddLocations_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = new DuneErosionFailureMechanism();
            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Call
            void Call() => handler.AddLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void AddLocations_AddedLocationIsDuneLocation_LocationAndCalculationsAdded()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.01)),
                    new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.01))
                }
            };

            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            ObservableList<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
            CollectionAssert.IsEmpty(calculationsForTargetProbabilities[0].DuneLocationCalculations);
            CollectionAssert.IsEmpty(calculationsForTargetProbabilities[1].DuneLocationCalculations);

            // Call
            HydraulicBoundaryLocation hydraulicBoundaryLocation = CreateLocationThatIsDuneLocation(random);
            handler.AddLocations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            DuneLocation duneLocation = failureMechanism.DuneLocations.Single();
            Assert.AreSame(hydraulicBoundaryLocation, duneLocation.HydraulicBoundaryLocation);
            AssertDuneLocationCalculations(duneLocation, failureMechanism);
        }

        [Test]
        public void AddLocations_FailureMechanismHasDuneLocations_LocationsAndCalculationsAdded()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.01)),
                    new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.01))
                }
            };

            var duneLocations = new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            };
            failureMechanism.SetDuneLocations(duneLocations);

            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Precondition
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count());

            ObservableList<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(2, calculationsForTargetProbabilities[0].DuneLocationCalculations.Count);
            Assert.AreEqual(2, calculationsForTargetProbabilities[1].DuneLocationCalculations.Count);

            // Call
            handler.AddLocations(new[]
            {
                CreateLocationThatIsDuneLocation(random)
            });

            // Assert
            Assert.AreEqual(3, failureMechanism.DuneLocations.Count());

            foreach (DuneLocation duneLocation in failureMechanism.DuneLocations)
            {
                AssertDuneLocationCalculations(duneLocation, failureMechanism);
            }
        }

        [Test]
        public void RemoveLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            var handler = new DuneLocationsUpdateHandler(viewCommands, new DuneErosionFailureMechanism());

            // Call
            void Call() => handler.RemoveLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void GivenFailureMechanismWithLocationsAndCalculations_WhenRemoveLocations_ThenExpectedLocationsRemoved()
        {
            // Given
            var viewCommands = Substitute.For<IViewCommands>();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(0.1)
                }
            };

            failureMechanism.SetDuneLocations(new[]
            {
                new DuneLocation(string.Empty, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties())
            });

            DuneLocationCalculationsForTargetProbability duneLocationCalculationsForTargetProbability =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.First();

            // Precondition
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());
            Assert.AreEqual(1, duneLocationCalculationsForTargetProbability.DuneLocationCalculations.Count);

            // When
            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);
            handler.RemoveLocations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Then
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsEmpty(duneLocationCalculationsForTargetProbability.DuneLocationCalculations);
        }

        [Test]
        public void DoPostUpdateActions_FailureMechanismHasNoDuneLocations_CloseAllViewsForFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var viewCommands = Substitute.For<IViewCommands>();
            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            viewCommands.Received().RemoveAllViewsForItem(failureMechanism);
        }

        [Test]
        public void DoPostUpdateActions_FailureMechanismHasDuneLocations_DoNothing()
        {
            // Setup
            var viewCommands = Substitute.For<IViewCommands>();
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism();

            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);
            handler.AddLocations(new[]
            {
                CreateLocationThatIsDuneLocation(random)
            });

            // Precondition
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            // Call
            handler.DoPostUpdateActions();

            // Assert
            Assert.AreEqual(0, viewCommands.ReceivedCalls().Count());
        }

        private static HydraulicBoundaryLocation CreateLocationThatIsDuneLocation(Random random)
        {
            return new HydraulicBoundaryLocation(random.Next(), "001-01_0001_SCHR_02_jr001000", random.NextDouble(), random.NextDouble());
        }

        private static void AssertDuneLocationCalculations(DuneLocation expectedDuneLocation, DuneErosionFailureMechanism failureMechanism)
        {
            ObservableList<DuneLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;

            foreach (DuneLocationCalculation duneLocationCalculation in userDefinedTargetProbabilities.Select(userDefinedTargetProbability => userDefinedTargetProbability.DuneLocationCalculations.SingleOrDefault(c => c.DuneLocation == expectedDuneLocation)))
            {
                Assert.IsNotNull(duneLocationCalculation);
                Assert.IsNull(duneLocationCalculation.Output);
            }
        }
    }
}