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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.HydraRing.Data.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NullName_DoesNotThrowException()
        {
            // Setup
            long id = 0L;
            double x = 1.0;
            double y = 1.0;

            // Call
            TestDelegate test = () => new HydraulicBoundaryLocation(id, null, x, y);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Constructor_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            long id = 1234L;
            string name = "<some name>";
            double x = 567.0;
            double y = 890.0;

            // Call
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(hydraulicBoundaryLocation);
            Assert.AreEqual(id, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocation.Name);
            Point2D location = hydraulicBoundaryLocation.Location;
            Assert.IsInstanceOf<Point2D>(location);
            Assert.AreEqual(x, location.X);
            Assert.AreEqual(y, location.Y);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            long id = 1234L;
            double x = 567.0;
            double y = 890.0;

            // Call
            var profile = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.AreEqual(name, profile.ToString());
        }
    }
}