using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class FailureMechanismPlaceholderUpdateExtensionsTest
    {

        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new FailureMechanismPlaceholder("name");

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
            var failureMechanism = new FailureMechanismPlaceholder("name");

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoFailureMechanism_EntityNotFoundException()
        {
            // Setup
            var failureMechanism = new FailureMechanismPlaceholder("name");

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoFailureMechanismWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var failureMechanism = new FailureMechanismPlaceholder("name")
            {
                StorageId = storageId
            };

            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = 2
            });

            // Call
            TestDelegate test = () => failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new FailureMechanismPlaceholder("name")
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
    }
}