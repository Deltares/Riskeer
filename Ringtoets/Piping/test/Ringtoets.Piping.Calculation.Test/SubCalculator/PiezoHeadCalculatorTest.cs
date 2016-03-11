using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.Test.SubCalculator
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