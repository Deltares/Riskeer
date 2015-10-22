using NUnit.Framework;

using Wti.Forms.PresentationObjects;

namespace Wti.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationInputsTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var presentationObject = new PipingCalculationInputs();

            // Assert
            Assert.IsNull(presentationObject.PipingData);
            CollectionAssert.IsEmpty(presentationObject.AvailablePipingSurfaceLines);
        }
    }
}