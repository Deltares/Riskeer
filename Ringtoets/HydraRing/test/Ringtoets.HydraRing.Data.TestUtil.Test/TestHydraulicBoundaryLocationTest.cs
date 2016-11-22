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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.HydraRing.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var testLocation = new TestHydraulicBoundaryLocation();

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name); 
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);
            Assert.IsNaN(testLocation.DesignWaterLevel);
            Assert.AreEqual(CalculationConvergence.NotCalculated, testLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, testLocation.WaveHeightCalculationConvergence);
        }

        [Test]
        public void Constructor_WithDesignWaterLevel_ExpectedValues()
        {
            // Setup
            var designWaterLevel = new RoundedDouble(4, 0.2);

            // Call
            var testLocation = new TestHydraulicBoundaryLocation(designWaterLevel);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name); 
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);
            Assert.AreEqual(designWaterLevel, testLocation.DesignWaterLevel);
            Assert.AreEqual(CalculationConvergence.NotCalculated, testLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, testLocation.WaveHeightCalculationConvergence);
        }
         
    }
}