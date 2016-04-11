using NUnit.Framework;

using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.Contribution
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
            Assert.AreEqual("Overig", result.Name);
            Assert.IsInstanceOf<FailureMechanismBase>(result);
            CollectionAssert.IsEmpty(result.CalculationItems);
        }
    }
}