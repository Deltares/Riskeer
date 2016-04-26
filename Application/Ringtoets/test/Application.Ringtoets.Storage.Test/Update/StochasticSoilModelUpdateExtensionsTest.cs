using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class StochasticSoilModelUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => soilModel.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => soilModel.Update(null, new RingtoetsEntities());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoStochasticSoilModel_EntityNotFoundException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => soilModel.Update(new UpdateConversionCollector(), new RingtoetsEntities());

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void Update_ContextWithStochasticSoilModel_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            string newName = "new name";
            string newSegmentName = "new segment name";
            var soilModel = new StochasticSoilModel(-1, newName, newSegmentName)
            {
                StorageId = 1,
            };

            var modelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1,
                Name = string.Empty,
                SegmentName = string.Empty
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(modelEntity);

            // Call
            soilModel.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, modelEntity.Name);
            Assert.AreEqual(newSegmentName, modelEntity.SegmentName);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithNewStochasticSoilProfile_StochasticSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();
            
            var soilModel = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = 1,
                StochasticSoilProfiles = 
                {
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestPipingSoilProfile()
                    }
                }
            };

            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(soilModelEntity);

            // Call
            soilModel.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, soilModelEntity.StochasticSoilProfileEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithUpdatedStochasticSoilProfile_NoNewStochasticSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var soilModel = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = 1,
                StochasticSoilProfiles = 
                {
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
                    {
                        StorageId = 1,
                        SoilProfile = new TestPipingSoilProfile()
                    }
                }
            };

            var soilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 1
            };
            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1,
                StochasticSoilProfileEntities = 
                {
                    soilProfileEntity
                }
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(soilModelEntity);
            ringtoetsEntities.StochasticSoilProfileEntities.Add(soilProfileEntity);

            // Call
            soilModel.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new [] {soilProfileEntity}, soilModelEntity.StochasticSoilProfileEntities);

            mocks.VerifyAll();
        }  
    }
}