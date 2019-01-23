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
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.DuneErosion;

namespace Riskeer.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((DuneLocationEntity) null).Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_WithValidData_ReturnsDuneLocationWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            const string testName = "testName";
            var random = new Random(21);
            long locationId = random.Next(0, 400);
            double x = random.NextDouble();
            double y = random.NextDouble();
            int coastalAreaId = random.Next();
            double offset = random.NextDouble();
            double orientation = random.NextDouble();
            double d50 = random.NextDouble();
            var entity = new DuneLocationEntity
            {
                LocationId = locationId,
                Name = testName,
                LocationX = x,
                LocationY = y,
                CoastalAreaId = coastalAreaId,
                Offset = offset,
                Orientation = orientation,
                D50 = d50
            };

            var collector = new ReadConversionCollector();

            // Call
            DuneLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(x, location.Location.X, 1e-6);
            Assert.AreEqual(y, location.Location.Y, 1e-6);
            Assert.AreEqual(coastalAreaId, location.CoastalAreaId);
            Assert.AreEqual(offset, location.Offset, location.Offset.GetAccuracy());
            Assert.AreEqual(orientation, location.Orientation, location.Orientation.GetAccuracy());
            Assert.AreEqual(d50, location.D50, location.D50.GetAccuracy());

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithNullData_ReturnsDuneLocationWithNaNPropertiesSet()
        {
            // Setup
            const string testName = "testName";
            var random = new Random(22);
            long locationId = random.Next(0, 400);
            int coastalAreaId = random.Next();
            var entity = new DuneLocationEntity
            {
                LocationId = locationId,
                Name = testName,
                LocationX = null,
                LocationY = null,
                CoastalAreaId = coastalAreaId,
                Offset = null,
                Orientation = null,
                D50 = null
            };

            var collector = new ReadConversionCollector();

            // Call
            DuneLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(testName, location.Name);
            Assert.IsNaN(location.Location.X);
            Assert.IsNaN(location.Location.Y);
            Assert.AreEqual(coastalAreaId, location.CoastalAreaId);
            Assert.IsNaN(location.Offset);
            Assert.IsNaN(location.Orientation);
            Assert.IsNaN(location.D50);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_SameDuneLocationEntityTwice_ReturnSameDuneLocation()
        {
            // Setup
            var entity = new DuneLocationEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            DuneLocation location1 = entity.Read(collector);
            DuneLocation location2 = entity.Read(collector);

            // Assert
            Assert.AreSame(location1, location2);
        }
    }
}