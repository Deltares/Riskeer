using Deltares.WTIPiping;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingProfileCreatorTest
    {
        [Test]
        public void GivenAPipingProfileCreator_WhenCreatingPipingProfile_ThenAProfileWithALayerIsReturned()
        {
            var pipingProfileCreator = new PipingProfileCreator();
            PipingProfile actual = pipingProfileCreator.Create();
            Assert.That(actual.Layers, Is.Not.Empty);
        }
    }
}