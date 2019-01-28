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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.DuneErosion.IO.Test
{
    [TestFixture]
    public class ReadDuneLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReadDuneLocation(null, new Point2D(0.0, 0.0), 0, 0.0, 0.0, 0.0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Dune location";
            var location = new Point2D(10.0, 12.0);
            const int coastalAreaId = 3;
            const double offset = 4.29;
            const double orientation = 4.2;
            const double d50 = 0.123456;

            // Call
            var duneLocation = new ReadDuneLocation(name, location, coastalAreaId, offset, orientation, d50);

            // Assert
            Assert.AreEqual(name, duneLocation.Name);
            Assert.AreSame(location, duneLocation.Location);
            Assert.AreEqual(coastalAreaId, duneLocation.CoastalAreaId);
            Assert.AreEqual(offset, duneLocation.Offset.Value);
            Assert.AreEqual(orientation, duneLocation.Orientation.Value);
            Assert.AreEqual(d50, duneLocation.D50.Value);
        }

        [Test]
        public void Constructor_WithOffset_OffsetRounded()
        {
            // Call
            var duneLocation = new ReadDuneLocation("dune", new Point2D(0.0, 0.0), 0, 4.298, 0.0, 0.0);

            // Assert
            Assert.AreEqual(2, duneLocation.Offset.NumberOfDecimalPlaces);
            Assert.AreEqual(4.30, duneLocation.Offset, duneLocation.Offset.GetAccuracy());
        }

        [Test]
        public void Constructor_WithOrientation_OrientationRounded()
        {
            // Call
            var duneLocation = new ReadDuneLocation("dune", new Point2D(0.0, 0.0), 0, 0.0, 8.214, 0.0);

            // Assert
            Assert.AreEqual(1, duneLocation.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(8.2, duneLocation.Orientation, duneLocation.Orientation.GetAccuracy());
        }

        [Test]
        public void Constructor_WithD50_D50Rounded()
        {
            // Call
            var duneLocation = new ReadDuneLocation("dune", new Point2D(0.0, 0.0), 0, 0.0, 0.0, 0.1234567);

            // Assert
            Assert.AreEqual(6, duneLocation.D50.NumberOfDecimalPlaces);
            Assert.AreEqual(0.123457, duneLocation.D50, duneLocation.D50.GetAccuracy());
        }
    }
}