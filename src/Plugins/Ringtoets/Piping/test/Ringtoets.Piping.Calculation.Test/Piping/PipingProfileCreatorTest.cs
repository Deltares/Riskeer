using Deltares.WTIPiping;
using NUnit.Framework;

using Ringtoets.Piping.Calculation.Piping;

namespace Ringtoets.Piping.Calculation.Test.Piping
{
    public class PipingProfileCreatorTest
    {
        [Test]
        public void Create_Always_ReturnsProfileWithSingleAquiferLayer()
        {
            // Call
            PipingProfile actual = PipingProfileCreator.Create();

            // Assert
            Assert.IsNotNull(actual.Layers);
            Assert.AreEqual(1, actual.Layers.Count);
            Assert.IsTrue(actual.Layers[0].IsAquifer);
        }
    }
}