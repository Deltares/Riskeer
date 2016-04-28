using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class StochasticSoilProfileUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var soilProfile = new TestStochasticSoilProfile();

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
            var soilProfile = new TestStochasticSoilProfile();

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
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoStochasticSoilProfile_EntityNotFoundException()
        {
            // Setup
            var soilProfile = new TestStochasticSoilProfile();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilProfileEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_WithNoStochasticSoilProfileWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var soilProfile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId,
                SoilProfile = new TestPipingSoilProfile()
            };

            ringtoetsEntities.StochasticSoilProfileEntities.Add(new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 2,
            });

            // Call
            TestDelegate test = () => soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilProfileEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithNewStochasticSoilProfile_PropertiesUpdatedSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newProbability = 0.5;
            var soilProfile = new StochasticSoilProfile(newProbability, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = 1,
                SoilProfile = new TestPipingSoilProfile()
            };

            var soilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 1,
                Probability = 0
            };

            ringtoetsEntities.StochasticSoilProfileEntities.Add(soilProfileEntity);

            // Call
            soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newProbability, soilProfileEntity.Probability);
            Assert.NotNull(soilProfileEntity.SoilProfileEntity);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithUpdatedStochasticSoilProfile_InstanceReferenceNotChanged()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var soilProfile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = 1,
                SoilProfile = new TestPipingSoilProfile
                {
                    StorageId = 1
                }
            };

            var soilProfileEntity = new SoilProfileEntity
            {
                SoilProfileEntityId = 1
            };
            var stochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 1,
                SoilProfileEntity = soilProfileEntity
            };

            ringtoetsEntities.StochasticSoilProfileEntities.Add(stochasticSoilProfileEntity);
            ringtoetsEntities.SoilProfileEntities.Add(soilProfileEntity);

            // Call
            soilProfile.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreSame(soilProfileEntity, stochasticSoilProfileEntity.SoilProfileEntity);

            mocks.VerifyAll();
        }   
    }

    public class TestStochasticSoilProfile : StochasticSoilProfile {
        public TestStochasticSoilProfile() : base(0.5, SoilProfileType.SoilProfile1D, -1) {}
    }
}