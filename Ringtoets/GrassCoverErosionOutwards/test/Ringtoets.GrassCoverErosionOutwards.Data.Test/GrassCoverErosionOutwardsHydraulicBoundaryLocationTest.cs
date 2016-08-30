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
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new GrassCoverErosionOutwardsHydraulicBoundaryLocation(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_PropertiesAsExpected()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 0.0, 0.0);

            // Call
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<IStorable>(grassCoverErosionOutwardsHydraulicBoundaryLocation);
            Assert.AreSame(hydraulicBoundaryLocation, grassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevel);
            Assert.AreEqual(2, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevelCalculationConvergence);
        }

        [Test]
        public void SetProperties_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 0.0, 0.0);
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation);

            // Call
            var waterLevel = (RoundedDouble) 1.23456;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevel = waterLevel;

            // Assert
            Assert.AreEqual(waterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevel,
                            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaterLevel.GetAccuracy());
        }
    }
}