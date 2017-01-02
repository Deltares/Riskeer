using NUnit.Framework;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStructuresOutputTest
    {
        [Test]
        public void Constructor_SetExpectedValues()
        {
            // Call
            var output = new TestStructuresOutput();

            // Assert
            Assert.AreEqual(0, output.FactorOfSafety.Value);
            Assert.AreEqual(0, output.RequiredProbability);
            Assert.AreEqual(0, output.Probability);
            Assert.AreEqual(0, output.RequiredReliability.Value);
            Assert.AreEqual(0, output.Reliability.Value);
        }
    }
}