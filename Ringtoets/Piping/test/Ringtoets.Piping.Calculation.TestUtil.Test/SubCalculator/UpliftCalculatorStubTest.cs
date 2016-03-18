using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class UpliftCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new UpliftCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.EffectiveStress);
            Assert.AreEqual(0, stub.HExit);
            Assert.AreEqual(0, stub.HRiver);
            Assert.AreEqual(0, stub.ModelFactorUplift);
            Assert.AreEqual(0, stub.PhiExit);
            Assert.AreEqual(0, stub.PhiPolder);
            Assert.AreEqual(0, stub.RExit);
            Assert.AreEqual(0, stub.VolumetricWeightOfWater);

            Assert.AreEqual(0, stub.FoSu);
            Assert.AreEqual(0, stub.Zu);
        }

        [Test]
        public void Validate_Always_EmptyList()
        {
            // Setup
            var stub = new UpliftCalculatorStub();

            // Call
            var result = stub.Validate();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var stub = new UpliftCalculatorStub();

            // Call
            TestDelegate call = () => stub.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }  
    }
}