using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class PipingFailureMechanismUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(null, new RingtoetsEntities());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoHydraulicBoundaryLocation_EntityNotFoundException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(new UpdateConversionCollector(), new RingtoetsEntities());

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void Update_ContextWithHydraulicBoundaryLocation_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true
            };

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), failureMechanismEntity.IsRelevant);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewStochasticSoilModel_SoilModelsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                StochasticSoilModels =
                {
                    new StochasticSoilModel(-1, string.Empty, string.Empty)
                }
            };

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedStochasticSoilModel_NoNewSoilModelAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                StochasticSoilModels =
                {
                    new StochasticSoilModel(-1, string.Empty, string.Empty)
                    {
                        StorageId = 1
                    }
                }
            };

            var stochasticSoilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                StochasticSoilModelEntities =
                {
                    stochasticSoilModelEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.StochasticSoilModelEntities.Add(stochasticSoilModelEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        } 
    }
}