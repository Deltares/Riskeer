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
using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Persistors;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class HydraulicLocationEntityPersistorTest
    {
        private static IEnumerable<Func<HydraulicBoundaryLocation>> TestCases
        {
            get
            {
                yield return () => null;
                yield return null;
            }
        }

        [Test]
        public void Constructor_NullDataSet_ThrowsAgrumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicLocationEntityPersistor(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("ringtoetsContext", exception.ParamName);
        }

        [Test]
        public void Constructor_EmtpyDataSet_NewInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.StrictMock<IRingtoetsEntities>();
            mocks.ReplayAll();

            // Call
            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<IPersistor<HydraulicLocationEntity, HydraulicBoundaryLocation>>(persistor);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntities);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModel(null, () => new HydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void LoadModel_ValidEntityNullModel_ThrowsArgumentNullException(Func<HydraulicBoundaryLocation> modelFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntities);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModel(new HydraulicLocationEntity(), modelFunc);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("model", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntityValidModel_EntityAsModel()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntities);
            mocks.ReplayAll();

            const string name = "test";
            const double designWaterLevel = 15.6;
            const long locationId = 1300001;
            const long storageId = 1234L;
            const decimal locationX = 253;
            const decimal locationY = 123;
            var entity = new HydraulicLocationEntity()
            {
                LocationId = locationId,
                Name = name,
                DesignWaterLevel = designWaterLevel,
                HydraulicLocationEntityId = storageId,
                LocationX = locationX,
                LocationY = locationY
            };

            // Call
            HydraulicBoundaryLocation location = persistor.LoadModel(entity, () => new HydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(name, location.Name);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel);
            Assert.AreEqual(locationX, location.Location.X);
            Assert.AreEqual(locationY, location.Location.Y);
            Assert.AreEqual(storageId, location.StorageId);

            mocks.VerifyAll();
        }
    }
}
