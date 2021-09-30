// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Geometry;
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
    public class DuneLocationsReplacementHandlerTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => new DuneLocationsReplacementHandler(null, failureMechanism);

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
            void Call() => new DuneLocationsReplacementHandler(viewCommands, null);

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
            var handler = new DuneLocationsReplacementHandler(viewCommands, new DuneErosionFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IDuneLocationsReplacementHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void Replace_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            // Call
            void Call() => handler.Replace(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Replace_FailureMechanismHasDuneLocations_LocationsAndCalculationsCleared()
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

            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            // Precondition
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count());

            ObservableList<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(2, calculationsForTargetProbabilities[0].DuneLocationCalculations.Count);
            Assert.AreEqual(2, calculationsForTargetProbabilities[1].DuneLocationCalculations.Count);

            // Call
            handler.Replace(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsEmpty(calculationsForTargetProbabilities[0].DuneLocationCalculations);
            CollectionAssert.IsEmpty(calculationsForTargetProbabilities[1].DuneLocationCalculations);
            mocks.VerifyAll();
        }

        [Test]
        public void Replace_NewLocationsIsDune_LocationAndCalculationsAdded()
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
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            });

            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            // Precondition
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count());

            ObservableList<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(2, calculationsForTargetProbabilities[0].DuneLocationCalculations.Count);
            Assert.AreEqual(2, calculationsForTargetProbabilities[1].DuneLocationCalculations.Count);

            // Call
            handler.Replace(new[]
            {
                new HydraulicBoundaryLocation(1, "test_1_100", 205354, 609735)
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            DuneLocation duneLocation = failureMechanism.DuneLocations.First();
            Assert.AreEqual(1, duneLocation.Id);
            Assert.AreEqual(new Point2D(205354, 609735), duneLocation.Location);
            AssertDuneLocationCalculations(duneLocation, failureMechanism);

            mocks.VerifyAll();
        }

        [Test]
        public void DoPostReplacementUpdates_NoDuneLocationsAdded_CloseAllViewsForFailureMechanism()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            });

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            handler.Replace(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);

            // Call
            handler.DoPostReplacementUpdates();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostReplacementUpdates_DuneLocationsAdded_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            });
            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            handler.Replace(new[]
            {
                new HydraulicBoundaryLocation(1, "Locatie_1_100", 205354, 609735)
            });

            // Precondition
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());

            // Call
            handler.DoPostReplacementUpdates();

            // Assert
            mocks.VerifyAll(); // Expect no calls in 'viewCommands'
        }

        private static void AssertDuneLocationCalculations(DuneLocation expectedDuneLocation, DuneErosionFailureMechanism failureMechanism)
        {
            ObservableList<DuneLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
            foreach (DuneLocationCalculationsForTargetProbability userDefinedTargetProbability in userDefinedTargetProbabilities)
            {
                AssertDefaultDuneLocationCalculation(expectedDuneLocation, userDefinedTargetProbability.DuneLocationCalculations.Single());
            }
        }

        private static void AssertDefaultDuneLocationCalculation(DuneLocation expectedDuneLocation, DuneLocationCalculation calculation)
        {
            Assert.AreSame(expectedDuneLocation, calculation.DuneLocation);
            Assert.IsNull(calculation.Output);
        }
    }
}