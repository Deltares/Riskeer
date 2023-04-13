// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
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
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationsUpdateHandler(viewCommands, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new DuneLocationsUpdateHandler(viewCommands, new DuneErosionFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IDuneLocationsUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void AddLocations_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Call
            void Call() => handler.AddLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AddLocations_NewLocationsIsDune_LocationAndCalculationsAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test_1_100", 205354, 609735);
            handler.AddLocations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            DuneLocation duneLocation = failureMechanism.DuneLocations.Single();
            Assert.AreSame(hydraulicBoundaryLocation, duneLocation.HydraulicBoundaryLocation);
            AssertDuneLocationCalculations(duneLocation, failureMechanism);

            mocks.VerifyAll();
        }

        [Test]
        public void AddLocations_FailureMechanismHasDuneLocations_LocationsAndCalculationsAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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
                new HydraulicBoundaryLocation(1, "test_1_100", 205354, 609735)
            });

            // Assert
            Assert.AreEqual(3, failureMechanism.DuneLocations.Count());

            foreach (DuneLocation duneLocation in failureMechanism.DuneLocations)
            {
                AssertDuneLocationCalculations(duneLocation, failureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void RemoveLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_FailureMechanismHasNoDuneLocations_CloseAllViewsForFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_FailureMechanismHasNoDuneLocations_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var handler = new DuneLocationsUpdateHandler(viewCommands, failureMechanism);
            handler.AddLocations(new[]
            {
                new HydraulicBoundaryLocation(1, "Locatie_1_100", 205354, 609735)
            });

            // Precondition
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll(); // Expect no calls in 'viewCommands'
        }

        private static void AssertDuneLocationCalculations(DuneLocation expectedDuneLocation, DuneErosionFailureMechanism failureMechanism)
        {
            ObservableList<DuneLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;

            foreach (DuneLocationCalculation duneLocationCalculation in userDefinedTargetProbabilities.Select(
                         userDefinedTargetProbability => userDefinedTargetProbability.DuneLocationCalculations.SingleOrDefault(
                             c => c.DuneLocation == expectedDuneLocation)))
            {
                Assert.IsNotNull(duneLocationCalculation);
                Assert.IsNull(duneLocationCalculation.Output);
            }
        }
    }
}