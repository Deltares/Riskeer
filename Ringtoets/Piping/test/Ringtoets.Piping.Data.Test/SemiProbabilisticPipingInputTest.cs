using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class SemiProbabilisticPipingInputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Call
            var inputParameters = new SemiProbabilisticPipingInput();

            // Assert

            Assert.AreEqual(1.0, inputParameters.A);
            Assert.AreEqual(350.0, inputParameters.B);

            Assert.IsNaN(inputParameters.SectionLength);
            Assert.AreEqual(0, inputParameters.Norm);
            Assert.IsNaN(inputParameters.Contribution);

        }
    }
}