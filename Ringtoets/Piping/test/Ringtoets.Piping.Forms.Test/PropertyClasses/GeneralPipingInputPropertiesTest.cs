using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GeneralPipingInputPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var properties = new GeneralPipingInputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GeneralPipingInput>>(properties);
        }

        [Test]
        public void Data_SetNewGeneralPipingInputInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var inputParameters = new GeneralPipingInput();
            var properties = new GeneralPipingInputProperties();

            // Call
            properties.Data = inputParameters;

            // Assert
            Assert.AreEqual(inputParameters.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(inputParameters.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeight);

            Assert.AreEqual(inputParameters.CriticalHeaveGradient, properties.CriticalHeaveGradient);

            Assert.AreEqual(inputParameters.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(inputParameters.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(inputParameters.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(inputParameters.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(inputParameters.Gravity, properties.Gravity);
            Assert.AreEqual(inputParameters.MeanDiameter70, properties.MeanDiameter70);
            Assert.AreEqual(inputParameters.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);

            Assert.AreEqual(inputParameters.A, properties.A);
            Assert.AreEqual(inputParameters.B, properties.B);
        }
    }
}