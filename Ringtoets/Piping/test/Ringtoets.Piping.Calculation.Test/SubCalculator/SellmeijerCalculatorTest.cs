using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class SellmeijerCalculatorTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var calculator = new SellmeijerCalculator();

            // Assert
            Assert.IsInstanceOf<ISellmeijerCalculator>(calculator);
            Assert.AreEqual(0.0, calculator.Zp);
        } 
    }
}