using NUnit.Framework;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Test.Contribution
{
    [TestFixture]
    public class OtherFailureMechanismTest
    {
        [Test]
        public void Constructor_Always_NameSet()
        {
            // Call
            var result = new OtherFailureMechanism();

            // Assert
            Assert.AreEqual(Resources.OtherFailureMechanism_DisplayName, result.Name);
        }
    }
}