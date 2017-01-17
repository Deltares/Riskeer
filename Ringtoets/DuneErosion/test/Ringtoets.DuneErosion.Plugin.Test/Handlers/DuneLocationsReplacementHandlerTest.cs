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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Plugin.Handlers;

namespace Ringtoets.DuneErosion.Plugin.Test.Handlers
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
            TestDelegate test = () => new DuneLocationsReplacementHandler(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
            TestDelegate test = () => new DuneLocationsReplacementHandler(viewCommands, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
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
            TestDelegate test = () => handler.Replace(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Replace_FailureMechanismHasDuneLocations_LocationsCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocations =
                {
                    new TestDuneLocation(),
                    new TestDuneLocation()
                }
            };
            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            // Precondition
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count);

            // Call
            handler.Replace(new HydraulicBoundaryLocation[]
                            {
                                new TestHydraulicBoundaryLocation()
                            });

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Replace_NewLocationsIsDune_LocationAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocations =
                {
                    new TestDuneLocation(),
                    new TestDuneLocation()
                }
            };
            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            // Precondition
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count);

            // Call
            handler.Replace(new []
                            {
                                new HydraulicBoundaryLocation(1, "test", 205354, 609735)
                            });

            // Assert
            Assert.AreEqual(7, failureMechanism.DuneLocations.Count);

            foreach (DuneLocation duneLocation in failureMechanism.DuneLocations)
            {
                Assert.AreEqual(1, duneLocation.Id);
                Assert.AreEqual(new Point2D(205354, 609735), duneLocation.Location);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void DoPostReplacementUpdates_NoDuneLocationsAdded_DoNothing()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocations =
                {
                    new TestDuneLocation(),
                    new TestDuneLocation()
                }
            };

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(failureMechanism.DuneLocations));
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

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocations =
                {
                    new TestDuneLocation(),
                    new TestDuneLocation()
                }
            };
            var handler = new DuneLocationsReplacementHandler(viewCommands, failureMechanism);

            handler.Replace(new[]
                            {
                                new HydraulicBoundaryLocation(1, "test", 205354, 609735)
                            });

            // Precondition
            Assert.AreEqual(7, failureMechanism.DuneLocations.Count);

            // Call
            handler.DoPostReplacementUpdates();

            // Assert
            mocks.VerifyAll(); // Expect not calls in 'viewCommands'
        }
    }
}