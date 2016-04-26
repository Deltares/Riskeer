using System;
using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
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
            TestDelegate test = () => soilProfile.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Update(null, new RingtoetsEntities());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoStochasticSoilModel_EntityNotFoundException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Update(new UpdateConversionCollector(), new RingtoetsEntities());

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void Update_NewSoilLayer_PropertiesUpdatedAndLayerAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

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
            soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

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
            soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new [] { soilLayerEntity }, profileEntity.SoilLayerEntities);

            mocks.VerifyAll();
        }
    }
}