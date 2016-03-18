using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class SellmeijerCalculatorStubTest
    {

        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new SellmeijerCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.BeddingAngle);
            Assert.AreEqual(0, stub.D70);
            Assert.AreEqual(0, stub.D70Mean);
            Assert.AreEqual(0, stub.DAquifer);
            Assert.AreEqual(0, stub.DTotal);
            Assert.AreEqual(0, stub.DarcyPermeability);
            Assert.AreEqual(0, stub.GammaSubParticles);
            Assert.AreEqual(0, stub.Gravity);
            Assert.AreEqual(0, stub.HExit);
            Assert.AreEqual(0, stub.HRiver);
            Assert.AreEqual(0, stub.KinematicViscosityWater);
            Assert.AreEqual(0, stub.ModelFactorPiping);
            Assert.AreEqual(0, stub.Rc);
            Assert.AreEqual(0, stub.SeepageLength);
            Assert.AreEqual(0, stub.VolumetricWeightOfWater);
            Assert.AreEqual(0, stub.WhitesDragCoefficient);

            Assert.AreEqual(0, stub.FoSp);
            Assert.AreEqual(0, stub.Zp);
        }

        [Test]
        public void Validate_Always_EmptyList()
        {
            // Setup
            var stub = new SellmeijerCalculatorStub();

            // Call
            var result = stub.Validate();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var stub = new SellmeijerCalculatorStub();

            // Call
            TestDelegate call = () => stub.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}