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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update.Piping;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update.Piping
{
    [TestFixture]
    public class PipingSoilProfileUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    soilProfile.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoPipingSoilProfile_EntityNotFoundException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    soilProfile.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'SoilProfileEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoPipingSoilProfileWithId_PropertiesUpdatedAndLayerAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            IEnumerable<PipingSoilLayer> newLayers = new[]
            {
                new PipingSoilLayer(5.0)
            };
            var storageId = 1;
            var soilProfile = new PipingSoilProfile("new name", 0.5, newLayers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId
            };

            ringtoetsEntities.SoilProfileEntities.Add(new SoilProfileEntity
            {
                SoilProfileEntityId = 2,
                Name = string.Empty,
                Bottom = 0
            });

            // Call
            TestDelegate test = () => soilProfile.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'SoilProfileEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_NewSoilLayer_PropertiesUpdatedAndLayerAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            string newName = "new name";
            double newBottom = 0.5;
            IEnumerable<PipingSoilLayer> newLayers = new[]
            {
                new PipingSoilLayer(5.0)
            };
            var soilProfile = new PipingSoilProfile(newName, newBottom, newLayers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = 1
            };

            var profileEntity = new SoilProfileEntity
            {
                SoilProfileEntityId = 1,
                Name = string.Empty,
                Bottom = 0
            };

            ringtoetsEntities.SoilProfileEntities.Add(profileEntity);

            // Call
            soilProfile.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, profileEntity.Name);
            Assert.AreEqual(newBottom, profileEntity.Bottom);
            Assert.AreEqual(1, profileEntity.SoilLayerEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_UpdatedSoilLayer_StochasticSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            IEnumerable<PipingSoilLayer> newLayers = new[]
            {
                new PipingSoilLayer(5.0)
                {
                    StorageId = 1
                }
            };
            var soilProfile = new PipingSoilProfile("new name", 0.5, newLayers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = 1
            };

            SoilLayerEntity soilLayerEntity = new SoilLayerEntity
            {
                SoilLayerEntityId = 1
            };
            var profileEntity = new SoilProfileEntity
            {
                SoilProfileEntityId = 1,
                SoilLayerEntities =
                {
                    soilLayerEntity
                }
            };

            ringtoetsEntities.SoilProfileEntities.Add(profileEntity);
            ringtoetsEntities.SoilLayerEntities.Add(soilLayerEntity);

            // Call
            soilProfile.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new [] { soilLayerEntity }, profileEntity.SoilLayerEntities);

            mocks.VerifyAll();
        }
    }
}