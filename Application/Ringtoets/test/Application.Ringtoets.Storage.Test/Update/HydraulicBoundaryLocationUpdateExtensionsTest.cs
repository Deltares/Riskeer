using System;
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
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
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
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoHydraulicBoundaryLocation_EntityNotFoundException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    hydraulicBoundaryLocation.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'HydraulicLocationEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoNoHydraulicBoundaryLocationWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

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
            TestDelegate test = () => section.Update(new UpdateConversionCollector(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var random = new Random(21);
            double newX = random.NextDouble() * 10;
            double newY = random.NextDouble() * 10;
            long newId = 2;
            string newName = "newName";
            double newDesignWaterLevel = random.NextDouble() * 10;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(newId, newName, newX, newY)
            {
                StorageId = 1,
                DesignWaterLevel = newDesignWaterLevel
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 1,
                DesignWaterLevel = 10,
                LocationId = 2,
                LocationX = Convert.ToDecimal(-3.2),
                LocationY = Convert.ToDecimal(-3.5)
            };

            ringtoetsEntities.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            // Call
            hydraulicBoundaryLocation.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, hydraulicLocationEntity.Name);
            Assert.AreEqual(newId, hydraulicLocationEntity.LocationId);
            Assert.AreEqual(newX, Convert.ToDouble(hydraulicLocationEntity.LocationX), 1e-6);
            Assert.AreEqual(newY, Convert.ToDouble(hydraulicLocationEntity.LocationY), 1e-6);
            Assert.AreEqual(newDesignWaterLevel, hydraulicLocationEntity.DesignWaterLevel, 1e-6);

            mocks.VerifyAll();
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(-1, string.Empty, 0, 0) {}
        }
    }
}