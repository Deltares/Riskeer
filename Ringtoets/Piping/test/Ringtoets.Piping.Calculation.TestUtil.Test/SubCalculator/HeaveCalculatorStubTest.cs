using NUnit.Framework;
using Ringtoets.Piping.Calculation.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Calculation.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class HeaveCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new HeaveCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.DTotal);
            Assert.AreEqual(0, stub.HExit);
            Assert.AreEqual(0, stub.Ich);
            Assert.AreEqual(0, stub.PhiExit);
            Assert.AreEqual(0, stub.PhiPolder);
            Assert.AreEqual(0, stub.RExit);

            Assert.AreEqual(0, stub.FoSh);
            Assert.AreEqual(0, stub.Zh);
        }

        [Test]
        public void Validate_Always_EmptyList()
        {
            // Setup
            var stub = new HeaveCalculatorStub();

            // Call
            var result = stub.Validate();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var stub = new HeaveCalculatorStub();

            // Call
            TestDelegate call = () => stub.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }  
    }
}