// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.DuneErosion;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityIsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ((DuneLocationEntity) null).Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_WithValidData_ReturnsDuneLocationWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            const string testName = "testName";

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var random = new Random(21);
            int coastalAreaId = random.Next();
            double offset = random.NextDouble();
            var entity = new DuneLocationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                Name = testName,
                CoastalAreaId = coastalAreaId,
                Offset = offset
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(coastalAreaId, location.CoastalAreaId);
            Assert.AreEqual(offset, location.Offset, location.Offset.GetAccuracy());

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithNullData_ReturnsDuneLocationWithNaNPropertiesSet()
        {
            // Setup
            const string testName = "testName";

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var random = new Random(22);
            int coastalAreaId = random.Next();
            var entity = new DuneLocationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                Name = testName,
                CoastalAreaId = coastalAreaId,
                Offset = null
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(coastalAreaId, location.CoastalAreaId);
            Assert.IsNaN(location.Offset);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_SameDuneLocationEntityTwice_ReturnSameDuneLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var entity = new DuneLocationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                Name = "A"
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocation location1 = entity.Read(collector);
            DuneLocation location2 = entity.Read(collector);

            // Assert
            Assert.AreSame(location1, location2);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_DuneLocationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var entity = new DuneLocationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                Name = string.Empty
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocation duneLocation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, duneLocation.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_DuneLocationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new DuneLocationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                Name = string.Empty
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(hydraulicLocationEntity));
        }
    }
}