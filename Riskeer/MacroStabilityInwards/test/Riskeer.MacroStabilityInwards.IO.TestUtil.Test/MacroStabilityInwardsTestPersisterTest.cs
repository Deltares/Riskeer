using NUnit.Framework;
using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsTestPersisterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var persister = new MacroStabilityInwardsTestPersister();

            // Assert
            Assert.IsInstanceOf<IPersister>(persister);
            Assert.IsFalse(persister.PersistCalled);
        }

        [Test]
        public void Persist_Always_SetsPersistCalledTrue()
        {
            // Setup
            var persister = new MacroStabilityInwardsTestPersister();

            // Call
            persister.Persist();

            // Assert
            Assert.IsTrue(persister.PersistCalled);
        }
    }
}