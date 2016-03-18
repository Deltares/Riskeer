using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    public class PiezoHeadCalculatorTest
    {
        [Test]
        public void Constructor_WithInput_PropertiesSet()
        {
            // Call
            var calculator = new PiezoHeadCalculator();

            // Assert
            Assert.IsInstanceOf<IPiezoHeadCalculator>(calculator);
            Assert.AreEqual(0.0, calculator.PhiExit);
        } 
    }
}