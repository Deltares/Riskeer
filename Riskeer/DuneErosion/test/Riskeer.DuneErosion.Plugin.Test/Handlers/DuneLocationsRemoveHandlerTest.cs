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
using Core.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Plugin.Handlers;

namespace Riskeer.DuneErosion.Plugin.Test.Handlers
{
    [TestFixture]
    public class DuneLocationsRemoveHandlerTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationsRemoveHandler(null, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationsRemoveHandler(new DuneErosionFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new DuneLocationsRemoveHandler(new DuneErosionFailureMechanism(), viewCommands);

            // Assert
            Assert.IsInstanceOf<IDuneLocationsRemoveHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new DuneLocationsRemoveHandler(new DuneErosionFailureMechanism(), viewCommands);

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
            var handler = new DuneLocationsRemoveHandler(failureMechanism, viewCommands);
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
        public void DoPostRemoveActions_FailureMechanismWithDuneLocations_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });

            var handler = new DuneLocationsRemoveHandler(failureMechanism, viewCommands);

            // Call
            handler.DoPostRemoveActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostRemoveActions_FailureMechanismWithoutDuneLocations_CloseViews()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            var handler = new DuneLocationsRemoveHandler(failureMechanism, viewCommands);

            // Call
            handler.DoPostRemoveActions();

            // Assert
            mocks.VerifyAll();
        }
    }
}