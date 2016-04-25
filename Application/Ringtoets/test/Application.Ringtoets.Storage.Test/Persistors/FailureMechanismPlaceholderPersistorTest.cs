using System.Collections.Generic;

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Persistors;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class FailureMechanismPlaceholderPersistorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var storageContext = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            // Call
            var persistor = new FailureMechanismPlaceholderPersistor(storageContext, FailureMechanismType.MacrostabilityInwards);

            // Assert
            Assert.IsInstanceOf<FailureMechanismPersistorBase<FailureMechanismPlaceholder>>(persistor);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismType.Microstability, 12345678, true)]
        [TestCase(FailureMechanismType.ReliabilityClosingOfStructure, 546, false)]
        public void LoadModel_ValidEntity_ProperlyInitializePlaceholderFailureMechanism(
            FailureMechanismType type, long id, bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var storageContext = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var persistor = new FailureMechanismPlaceholderPersistor(storageContext, type);

            var failureMechanism = new FailureMechanismPlaceholder("A");
            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)type,
                FailureMechanismEntityId = id,
                IsRelevant = isRelevant ? (byte)1 : (byte)0
            };

            // Call
            persistor.LoadModel(entity, failureMechanism);

            // Assert
            Assert.AreEqual(id, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
        }

        [Test]
        [TestCase(FailureMechanismType.DuneErosion, true)]
        [TestCase(FailureMechanismType.StabilityStoneRevetment, false)]
        public void UpdateModel_FailureMechanismNotPersisted_FailureMechanismAddedToPersistedCollection(
            FailureMechanismType type, bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var storageContext = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var parentNavigationProperty = new List<FailureMechanismEntity>();

            var persistor = new FailureMechanismPlaceholderPersistor(storageContext, type);

            var failureMechanism = new FailureMechanismPlaceholder("A")
            {
                IsRelevant = isRelevant
            };

            // Precondition
            Assert.AreEqual(0, failureMechanism.StorageId,
                "StorageId should be 0 to denote the failure mechanism as unsaved.");

            // Call
            persistor.UpdateModel(parentNavigationProperty, failureMechanism);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            FailureMechanismEntity entity = parentNavigationProperty[0];
            Assert.AreEqual((short)type, entity.FailureMechanismType);
            var expectedIsRelevantValue = isRelevant ? (byte)1 : (byte)0;
            Assert.AreEqual(expectedIsRelevantValue, entity.IsRelevant);
        }

        [Test]
        [TestCase(FailureMechanismType.DuneErosion, true)]
        [TestCase(FailureMechanismType.StabilityStoneRevetment, false)]
        public void UpdateModel_FailureMechanismHasBeenPersistedAlready_EntityUpdateWithNewValues(
            FailureMechanismType type, bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var storageContext = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            const int id = 234567;
            FailureMechanismEntity alreadyExistingEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = id,
                FailureMechanismType = (short)type
            };
            var parentNavigationProperty = new List<FailureMechanismEntity>
            {
                alreadyExistingEntity,
            };

            var persistor = new FailureMechanismPlaceholderPersistor(storageContext, type);

            var failureMechanism = new FailureMechanismPlaceholder("A")
            {
                IsRelevant = isRelevant,
                StorageId = id
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, failureMechanism);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            Assert.AreEqual((short)type, alreadyExistingEntity.FailureMechanismType);
            var expectedIsRelevantValue = isRelevant ? (byte)1 : (byte)0;
            Assert.AreEqual(expectedIsRelevantValue, alreadyExistingEntity.IsRelevant);
        }

        [Test]
        public void PerformPostSaveActions_PlaceholderRecentlyInserted_SetIdOfPlaceholder()
        {
            // Setup
            var mocks = new MockRepository();
            var storageContext = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var type = FailureMechanismType.PipingAtStructure;

            var parentNavigationProperty = new List<FailureMechanismEntity>();

            var persistor = new FailureMechanismPlaceholderPersistor(storageContext, type);

            var failureMechanism = new FailureMechanismPlaceholder("A");

            // Precondition
            Assert.AreEqual(0, failureMechanism.StorageId,
                            "StorageId should be 0 to denote the failure mechanism as unsaved.");

            const long id = 63547234L;
            persistor.UpdateModel(parentNavigationProperty, failureMechanism);
            parentNavigationProperty[0].FailureMechanismEntityId = id;

            // Call
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            Assert.AreEqual(id, failureMechanism.StorageId);
        }
    }
}