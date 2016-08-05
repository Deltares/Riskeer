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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class HydraulicBoundaryLocationUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    hydraulicBoundaryLocation.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoHydraulicBoundaryLocation_ThrowsEntityNotFoundException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    hydraulicBoundaryLocation.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'HydraulicLocationEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoNoHydraulicBoundaryLocationWithId_ThrowsEntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var section = new TestHydraulicBoundaryLocation
            {
                StorageId = storageId
            };

            ringtoetsEntities.HydraulicLocationEntities.Add(new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 2
            });

            // Call
            TestDelegate test = () => section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'HydraulicLocationEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithHydraulicBoundaryLocation_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var random = new Random(21);
            double newX = random.NextDouble() * 10;
            double newY = random.NextDouble() * 10;
            long newId = 2;
            string newName = "newName";
            double newDesignWaterLevel = random.NextDouble() * 10;
            double newWaveHeight = random.NextDouble() * 10;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(newId, newName, newX, newY)
            {
                StorageId = 1,
                DesignWaterLevel = newDesignWaterLevel,
                WaveHeight = newWaveHeight
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 1,
                DesignWaterLevel = 10,
                WaveHeight = 10,
                LocationId = 2,
                LocationX = -3.2,
                LocationY = -3.5
            };

            ringtoetsEntities.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            // Call
            hydraulicBoundaryLocation.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, hydraulicLocationEntity.Name);
            Assert.AreEqual(newId, hydraulicLocationEntity.LocationId);
            Assert.AreEqual(newX, hydraulicLocationEntity.LocationX, 1e-6);
            Assert.AreEqual(newY, hydraulicLocationEntity.LocationY, 1e-6);
            Assert.AreEqual(newDesignWaterLevel, hydraulicLocationEntity.DesignWaterLevel, 1e-6);
            Assert.AreEqual(newWaveHeight, hydraulicLocationEntity.WaveHeight, 1e-6);

            mocks.VerifyAll();
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(-1, string.Empty, 0, 0) {}
        }
    }
}