﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new DuneLocation(null, new TestHydraulicBoundaryLocation(), new DuneLocation.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocation(string.Empty, null, new DuneLocation.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_PropertiesNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new DuneLocation(string.Empty, new TestHydraulicBoundaryLocation(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Dune location";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hydraulic boundary location", 10.0, 12.0);
            const int coastalAreaId = 3;
            const double offset = 4.2;
            const double orientation = 4.2;
            const double d50 = 0.123456;

            // Call
            var duneLocation = new DuneLocation(name, hydraulicBoundaryLocation,
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = coastalAreaId,
                                                    Offset = offset,
                                                    Orientation = orientation,
                                                    D50 = d50
                                                });

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, duneLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(hydraulicBoundaryLocation.Id, duneLocation.Id);
            Assert.AreEqual(name, duneLocation.Name);
            Assert.AreSame(hydraulicBoundaryLocation.Location, duneLocation.Location);
            Assert.AreEqual(coastalAreaId, duneLocation.CoastalAreaId);
            Assert.AreEqual(offset, duneLocation.Offset.Value);
            Assert.AreEqual(orientation, duneLocation.Orientation.Value);
            Assert.AreEqual(d50, duneLocation.D50.Value);
        }

        [Test]
        public void Constructor_WithOffset_OffsetRounded()
        {
            // Call
            var duneLocation = new DuneLocation("dune", new TestHydraulicBoundaryLocation(),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    Offset = 4.298
                                                });

            // Assert
            Assert.AreEqual(1, duneLocation.Offset.NumberOfDecimalPlaces);
            Assert.AreEqual(4.3, duneLocation.Offset, duneLocation.Offset.GetAccuracy());
        }

        [Test]
        public void Constructor_WithOrientation_OrientationRounded()
        {
            // Call
            var duneLocation = new DuneLocation("dune", new TestHydraulicBoundaryLocation(),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    Orientation = 8.214
                                                });

            // Assert
            Assert.AreEqual(1, duneLocation.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(8.2, duneLocation.Orientation, duneLocation.Orientation.GetAccuracy());
        }

        [Test]
        public void Constructor_WithD50_D50Rounded()
        {
            // Call
            var duneLocation = new DuneLocation("dune", new TestHydraulicBoundaryLocation(),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    D50 = 0.1234567
                                                });

            // Assert
            Assert.AreEqual(6, duneLocation.D50.NumberOfDecimalPlaces);
            Assert.AreEqual(0.123457, duneLocation.D50, duneLocation.D50.GetAccuracy());
        }
    }
}