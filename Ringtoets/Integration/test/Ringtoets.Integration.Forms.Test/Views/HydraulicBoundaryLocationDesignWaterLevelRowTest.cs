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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationDesignWaterLevelRowTest
    {
        [Test]
        public void Constructor_WithoutHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationDesignWaterLevelRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_WithHydraulicBoundaryLocation_PropertiesFromHydraulicBoundaryLocation()
        {
            // Setup
            const int id = 1;
            const string locationname = "LocationName";
            const double coordinateX = 1.0;
            const double coordinateY = 2.0;
            const double designWaterLevel = 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, locationname, coordinateX, coordinateY)
            {
                DesignWaterLevel = designWaterLevel
            };

            // Call
            var row = new HydraulicBoundaryLocationDesignWaterLevelRow(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(locationname, row.Name);
            var expectedDesignWaterLevel = new RoundedDouble(2, designWaterLevel);
            Assert.AreEqual(expectedDesignWaterLevel, row.DesignWaterLevel);
            var expectedPoint2D = new Point2D(coordinateX, coordinateY);
            Assert.AreEqual(expectedPoint2D, row.Location);
            Assert.AreEqual(hydraulicBoundaryLocation, row.HydraulicBoundaryLocation);
        }
    }
}