// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class ObserverHelperTest
    {
        [Test]
        public void CreateHydraulicBoundaryLocationCalculationsObserver_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(null, () => {});

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationCalculationsObserver_UpdateObserverActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculations = mocks.Stub<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(calculations, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateObserverAction", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateHydraulicBoundaryLocationCalculationsObserver_WithData_ReturnsRecursiveObserver()
        {
            // Setup
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            // Call
            using (RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> observer =
                ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(calculations, () => {}))
            {
                // Assert
                Assert.AreSame(calculations, observer.Observable);
            }
        }

        [Test]
        public void GivenCreatedHydraulicBoundaryLocationCalculationsObserver_WhenCalculationNotifiesObservers_ThenUpdateObserverActionCalled()
        {
            // Given
            var count = 0;
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            };

            using (ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(calculations, () => { count++; }))
            {
                // When
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, count);
            }
        }
    }
}