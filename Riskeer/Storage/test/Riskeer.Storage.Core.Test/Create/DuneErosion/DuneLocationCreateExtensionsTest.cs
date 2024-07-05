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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCreateExtensionsTest
    {
        [Test]
        public void Create_DuneLocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((DuneLocation) null).Create(new PersistenceRegistry(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var location = new TestDuneLocation();

            // Call
            void Call() => location.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithPersistenceRegistry_ReturnsDuneLocationEntityWithPropertiesSet()
        {
            // Setup
            const string testName = "testName";
            var random = new Random(21);
            int id = random.Next(0, 150);
            double coordinateX = random.NextDouble();
            double coordinateY = random.NextDouble();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, string.Empty, coordinateX, coordinateY);

            int order = random.Next();

            var location = new DuneLocation(testName, hydraulicBoundaryLocation,
                                            new DuneLocation.ConstructionProperties
                                            {
                                                CoastalAreaId = random.Next(),
                                                Offset = random.NextDouble()
                                            });

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocationEntity entity = location.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(location.CoastalAreaId, entity.CoastalAreaId);
            Assert.AreEqual(location.Offset, entity.Offset, location.Offset.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNValues_ReturnsDuneLocationEntityWithNullPropertiesSet()
        {
            // Setup
            var random = new Random(28);
            int id = random.Next(0, 150);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, string.Empty, double.NaN, double.NaN);

            int order = random.Next();

            var location = new DuneLocation(string.Empty, hydraulicBoundaryLocation,
                                            new DuneLocation.ConstructionProperties
                                            {
                                                Offset = double.NaN
                                            });

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocationEntity entity = location.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsEmpty(entity.Name);
            Assert.IsNull(entity.Offset);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_DuneLocationWithAlreadyRegisteredHydraulicLocation_ReturnsEntityWithHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var duneLocation = new DuneLocation(string.Empty, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties());

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocationEntity entity = duneLocation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReferences()
        {
            // Setup
            const string testName = "original name";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);
            var location = new DuneLocation(testName, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties());

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocationEntity entity = location.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }

        [Test]
        public void Create_DuneLocationSavedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var location = new DuneLocation(string.Empty, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties());

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            DuneLocationEntity entity1 = location.Create(registry, 0);
            DuneLocationEntity entity2 = location.Create(registry, 1);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}