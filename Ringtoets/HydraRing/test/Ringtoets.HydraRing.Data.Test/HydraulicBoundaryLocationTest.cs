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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.HydraRing.Data.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocation(0L, null, 0.0, 0.0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            const long id = 1234L;
            const string name = "<some name>";
            const double x = 567.0;
            const double y = 890.0;

            // Call
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.IsInstanceOf<Observable>(hydraulicBoundaryLocation);

            Assert.AreEqual(id, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocation.Name);
            Point2D location = hydraulicBoundaryLocation.Location;
            Assert.IsInstanceOf<Point2D>(location);
            Assert.AreEqual(x, location.X);
            Assert.AreEqual(y, location.Y);
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.AreEqual(2, hydraulicBoundaryLocation.DesignWaterLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(2, hydraulicBoundaryLocation.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsNull(hydraulicBoundaryLocation.DesignWaterLevelOutput);
            Assert.IsNull(hydraulicBoundaryLocation.WaveHeightOutput);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            var testName = "testName";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0);

            // Call
            var result = hydraulicBoundaryLocation.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}