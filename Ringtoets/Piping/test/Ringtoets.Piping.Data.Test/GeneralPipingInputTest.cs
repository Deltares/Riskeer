using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class GeneralPipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralPipingInput();

            // Assert
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor);

            Assert.AreEqual(10.0, inputParameters.WaterVolumetricWeight);

            Assert.AreEqual(0.3, inputParameters.CriticalHeaveGradient);

            Assert.AreEqual(16.5, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(37, inputParameters.BeddingAngle);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(9.81, inputParameters.Gravity);
            Assert.AreEqual(2.08e-4, inputParameters.MeanDiameter70);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor);

            Assert.AreEqual(1.0, inputParameters.A);
            Assert.AreEqual(350.0, inputParameters.B);
            Assert.IsNaN(inputParameters.SectionLength);
        }
    }
}