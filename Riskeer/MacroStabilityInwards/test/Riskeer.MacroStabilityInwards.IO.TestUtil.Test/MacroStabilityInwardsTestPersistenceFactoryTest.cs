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
        public void CreateArchivePersister_ThrowExceptionIsFalse_SetsPropertiesAndReturnsTestPersister()
        {
            // Setup
            var persistableDataModel = new PersistableDataModel();
            const string filePath = "FilePath";

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Call
            IPersister persister = persistenceFactory.CreateArchivePersister(filePath, persistableDataModel);

            // Assert
            Assert.AreSame(persistableDataModel, persistenceFactory.PersistableDataModel);
            Assert.AreEqual(filePath, persistenceFactory.FilePath);
            Assert.IsInstanceOf<MacroStabilityInwardsTestPersister>(persister);
        }

        [Test]
        public void CreateArchivePersister_ThrowExceptionIsTrue_ThrowsException()
        {
            // Setup
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true
            };

            // Call
            void Call() => persistenceFactory.CreateArchivePersister("FilePath", new PersistableDataModel());

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreEqual("Exception in persistor.", exception.Message);
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