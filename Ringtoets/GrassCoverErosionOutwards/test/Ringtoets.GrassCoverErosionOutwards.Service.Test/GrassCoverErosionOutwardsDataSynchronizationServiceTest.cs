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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearWaveConditionsCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculation_WithCalculation_OutputNull()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null);

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
            bool affected = GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsNaN(location.DesignWaterLevel);
            Assert.IsNaN(location.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);
            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_ReturnsFalse()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            bool affected = GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsFalse(affected);
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
            bool affected = GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsFalse(affected);
        }
    }
}