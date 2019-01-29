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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocation(0, null, 0.0, 0.0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            const long id = 1234;
            const string name = "<some name>";
            const double x = 567.0;
            const double y = 890.0;

            // Call
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocation.Name);
            Point2D location = hydraulicBoundaryLocation.Location;
            Assert.AreEqual(x, location.X);
            Assert.AreEqual(y, location.Y);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            const string testName = "testName";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0);

            // Call
            string result = hydraulicBoundaryLocation.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}