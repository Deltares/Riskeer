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
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataSynchronizationServiceTest
    {
        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        [TestCase(3.4, 5.3)]
        [TestCase(3.4, double.NaN)]
        [TestCase(double.NaN, 8.5)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithData_ClearsDataAndReturnsTrue(double designWaterLevel, double waveHeight)
        {
            // Setup
            var location = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) designWaterLevel,
                WaveHeight = (RoundedDouble) waveHeight
            };

            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsNaN(location.DesignWaterLevel);
            Assert.IsNaN(location.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);

            CollectionAssert.AreEqual(new[]
            {
                location
            }, affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_ReturnsFalse()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutWaveHeightAndDesignWaterLevel_ReturnsFalse()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            StructuresCalculation<TestInput> calculation = null;

            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var calculation = new StructuresCalculation<TestInput>
            {
                Output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            };

            // Call
            IEnumerable<IObservable> changedObjects = RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsNull(calculation.Output);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new StructuresCalculation<TestInput>
            {
                Output = null
            };

            // Call
            IEnumerable<IObservable> changedObjects = RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        private class TestInput : ICalculationInput
        {
            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }
        }
    }
}