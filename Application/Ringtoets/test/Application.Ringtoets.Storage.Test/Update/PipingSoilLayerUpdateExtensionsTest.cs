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
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class PipingSoilLayerUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var pipingSoilLayer = new PipingSoilLayer(0.5);

            // Call
            TestDelegate test = () => pipingSoilLayer.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var pipingSoilLayer = new PipingSoilLayer(0.5);

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    pipingSoilLayer.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoPipingSoilLayer_EntityNotFoundException()
        {
            // Setup
            var pipingSoilLayer = new PipingSoilLayer(0.5);

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    pipingSoilLayer.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'SoilLayerEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoPipingSoilLayerWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var random = new Random(21);
            double newTop = random.NextDouble() * 10;
            var storageId = 1;
            var pipingSoilLayer = new PipingSoilLayer(newTop)
            {
                StorageId = storageId
            };

            ringtoetsEntities.SoilLayerEntities.Add(new SoilLayerEntity
            {
                SoilLayerEntityId = 2
            });

            // Call
            TestDelegate test = () => pipingSoilLayer.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'SoilLayerEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            
            mocks.VerifyAll();
        } 

        [Test]
        public void Update_WithPipingSoilLayer_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var random = new Random(21);
            double newTop = random.NextDouble() * 10;
            var pipingSoilLayer = new PipingSoilLayer(newTop)
            {
                StorageId = 1,
                IsAquifer = true
            };

            var soilLayerEntity = new SoilLayerEntity
            {
                SoilLayerEntityId = 1,
                Top = 0,
                IsAquifer = Convert.ToByte(false)
            };

            ringtoetsEntities.SoilLayerEntities.Add(soilLayerEntity);

            // Call
            pipingSoilLayer.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToDouble(newTop), Convert.ToDouble(soilLayerEntity.Top), 1e-6);
            Assert.AreEqual(Convert.ToByte(true), soilLayerEntity.IsAquifer);

            mocks.VerifyAll();
        } 
    }
}