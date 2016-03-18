using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class EffectiveThicknessCalculatorStubTest
    {

        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new EffectiveThicknessCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.ExitPointXCoordinate);
            Assert.AreEqual(0, stub.PhreaticLevel);
            Assert.IsNull(stub.SoilProfile);
            Assert.IsNull(stub.SurfaceLine);
            Assert.AreEqual(0, stub.VolumicWeightOfWater);
            
            Assert.AreEqual(0, stub.EffectiveHeight);
            Assert.AreEqual(0, stub.EffectiveStress);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var stub = new EffectiveThicknessCalculatorStub();

            // Call
            TestDelegate call = () => stub.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }  
    }
}