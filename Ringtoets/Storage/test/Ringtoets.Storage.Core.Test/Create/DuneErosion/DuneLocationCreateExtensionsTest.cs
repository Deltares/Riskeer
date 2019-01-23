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
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
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
            TestDelegate call = () => ((DuneLocation) null).Create(new PersistenceRegistry(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var location = new TestDuneLocation();

            // Call
            TestDelegate test = () => location.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithPersistenceRegistry_ReturnsDuneLocationEntityWithPropertiesSet()
        {
            // Setup
            const string testName = "testName";
            var random = new Random(21);
            double coordinateX = random.NextDouble();
            double coordinateY = random.NextDouble();
            int id = random.Next(0, 150);
            int order = random.Next();
            var registry = new PersistenceRegistry();

            var location = new DuneLocation(id, testName, new Point2D(coordinateX, coordinateY),
                                            new DuneLocation.ConstructionProperties
                                            {
                                                CoastalAreaId = random.Next(),
                                                Offset = random.NextDouble(),
                                                Orientation = random.NextDouble(),
                                                D50 = random.NextDouble()
                                            });

            // Call
            DuneLocationEntity entity = location.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(id, entity.LocationId);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(coordinateX, entity.LocationX);
            Assert.AreEqual(coordinateY, entity.LocationY);
            Assert.AreEqual(location.CoastalAreaId, entity.CoastalAreaId);
            Assert.AreEqual(location.Offset, entity.Offset, location.Offset.GetAccuracy());
            Assert.AreEqual(location.Orientation, entity.Orientation, location.Orientation.GetAccuracy());
            Assert.AreEqual(location.D50, entity.D50, location.D50.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNValues_ReturnsDuneLocationEntityWithNullPropertiesSet()
        {
            // Setup
            var random = new Random(28);
            int id = random.Next(0, 150);
            int order = random.Next();
            var registry = new PersistenceRegistry();

            var location = new DuneLocation(id, string.Empty, new Point2D(double.NaN, double.NaN),
                                            new DuneLocation.ConstructionProperties
                                            {
                                                Offset = double.NaN,
                                                Orientation = double.NaN,
                                                D50 = double.NaN
                                            });

            // Call
            DuneLocationEntity entity = location.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsEmpty(entity.Name);
            Assert.IsNull(entity.LocationX);
            Assert.IsNull(entity.LocationY);
            Assert.IsNull(entity.Offset);
            Assert.IsNull(entity.Orientation);
            Assert.IsNull(entity.D50);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReferences()
        {
            // Setup
            const string testName = "original name";
            var location = new DuneLocation(1, testName, new Point2D(0, 0), new DuneLocation.ConstructionProperties());
            var registry = new PersistenceRegistry();

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
            var location = new TestDuneLocation();

            var registry = new PersistenceRegistry();

            // Call
            DuneLocationEntity entity1 = location.Create(registry, 0);
            DuneLocationEntity entity2 = location.Create(registry, 1);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}