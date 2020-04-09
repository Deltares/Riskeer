using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class IdFactoryTest
    {
        [Test]
        public void Create_Always_ReturnsUniqueId()
        {
            // Setup
            var idFactory = new IdFactory();
            
            // Call
            string firstId = idFactory.Create();
            string secondId = idFactory.Create();

            // Assert
            Assert.AreNotEqual(firstId, secondId);
        }
    }
}