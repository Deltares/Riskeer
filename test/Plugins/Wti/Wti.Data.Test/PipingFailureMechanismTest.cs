using NUnit.Framework;

namespace Wti.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var piping = new PipingFailureMechanism();

            // assert
            CollectionAssert.IsEmpty(piping.SurfaceLines);
            Assert.IsNotNull(piping.PipingData);
        }
    }
}