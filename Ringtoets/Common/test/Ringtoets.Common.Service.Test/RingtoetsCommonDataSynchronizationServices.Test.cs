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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataSynchronizationServiceTest
    {
        [Test]
        public void ClearDesignWaterLevel_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearDesignWaterLevel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void ClearDesignWaterLevel_WithHydraulicBoundaryLocation_OutputNaN()
        {
            // Setup
            var calculation = new HydraulicBoundaryLocation(0, "Location 1", 0.5, 0.5)
            {
                DesignWaterLevel = (RoundedDouble) 5.0
            };

            // Call
            RingtoetsCommonDataSynchronizationService.ClearDesignWaterLevel(calculation);

            // Assert
            Assert.IsNaN(calculation.DesignWaterLevel);
        }

        [Test]
        public void ClearWaveHeight_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearWaveHeight(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void ClearWaveHeight_WithHydraulicBoundaryLocation_OutputNaN()
        {
            // Setup
            var calculation = new HydraulicBoundaryLocation(0, "Location 1", 0.5, 0.5)
            {
                WaveHeight = (RoundedDouble) 5.0
            };

            // Call
            RingtoetsCommonDataSynchronizationService.ClearWaveHeight(calculation);

            // Assert
            Assert.IsNaN(calculation.WaveHeight);
        }
    }
}