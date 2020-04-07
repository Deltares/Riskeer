using System;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsTestPersistenceFactoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Assert
            Assert.IsInstanceOf<IPersistenceFactory>(persistenceFactory);
            Assert.IsNull(persistenceFactory.PersistableDataModel);
            Assert.IsNull(persistenceFactory.FilePath);
        }

        [Test]
        public void CreateArchivePersister_Always_SetsPropertiesAndReturnsNull()
        {
            // Setup
            var persistableDataModel = new PersistableDataModel();
            const string filePath = "FilePath";

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Call
            Persister persister = persistenceFactory.CreateArchivePersister(filePath, persistableDataModel);

            // Assert
            Assert.AreSame(persistableDataModel, persistenceFactory.PersistableDataModel);
            Assert.AreEqual(filePath, persistenceFactory.FilePath);
            Assert.IsNull(persister);
        }

        [Test]
        public void CreateArchiveReader_Always_ThrowsNotImplementedException()
        {
            // Setup
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Call
            void Call() => persistenceFactory.CreateArchiveReader(null);

            // Assert
            Assert.Throws<NotImplementedException>(Call);
        }
    }
}