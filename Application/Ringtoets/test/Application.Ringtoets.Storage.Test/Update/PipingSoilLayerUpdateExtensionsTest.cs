using System;
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
            TestDelegate test = () => pipingSoilLayer.Update(new UpdateConversionCollector(), null);

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
            TestDelegate test = () => pipingSoilLayer.Update(null, new RingtoetsEntities());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoHydraulicBoundaryLocation_EntityNotFoundException()
        {
            // Setup
            var pipingSoilLayer = new PipingSoilLayer(0.5);

            // Call
            TestDelegate test = () => pipingSoilLayer.Update(new UpdateConversionCollector(), new RingtoetsEntities());

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void Update_WithHydraulicBoundaryLocation_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var random = new Random(21);
            double newTop = random.NextDouble() * 10;
            var hydraulicBoundaryLocation = new PipingSoilLayer(newTop)
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
            hydraulicBoundaryLocation.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToDouble(newTop), Convert.ToDouble(soilLayerEntity.Top), 1e-6);
            Assert.AreEqual(Convert.ToByte(true), soilLayerEntity.IsAquifer);

            mocks.VerifyAll();
        } 
    }
}