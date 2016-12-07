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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class DesignWaterLevelLocationRowTest
    {
        [Test]
        public void Constructor_WithoutDesignWaterLevelLocationContext_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DesignWaterLevelLocationRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_WithDesignWaterLevelLocationContext_PropertiesFromHydraulicBoundaryLocation()
        {
            // Setup
            const int id = 1;
            const string locationname = "LocationName";
            const double coordinateX = 1.0;
            const double coordinateY = 2.0;
            RoundedDouble designWaterLevel = (RoundedDouble) 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, locationname, coordinateX, coordinateY)
            {
                DesignWaterLevel = designWaterLevel
            };

            // Call
            var row = new DesignWaterLevelLocationRow(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationRow>(row);
            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(locationname, row.Name);
            Assert.AreEqual(designWaterLevel, row.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
            var expectedPoint2D = new Point2D(coordinateX, coordinateY);
            Assert.AreEqual(expectedPoint2D, row.Location);
            Assert.AreSame(hydraulicBoundaryLocation, row.HydraulicBoundaryLocation);
            Assert.IsFalse(row.ToCalculate);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DesignWaterLevelLocationRow,
                              NoValueRoundedDoubleConverter>(r => r.DesignWaterLevel));
        }

        [Test]
        public void Constructor_Property_SetPropertyAsExpected()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "LocationName", 1.0, 2.0);
            var row = new DesignWaterLevelLocationRow(hydraulicBoundaryLocation);

            // Call
            row.ToCalculate = true;

            // Assert
            Assert.IsTrue(row.ToCalculate);
        }
    }
}