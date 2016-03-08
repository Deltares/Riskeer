using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.Test.SubCalculator
{
    [TestFixture]
    public class HeaveCalculatorTest
    {
        [Test]
        public void Constructor_WithInput_PropertiesSet()
        {
            // Call
            var calculator = new HeaveCalculator();

            // Assert
            Assert.IsInstanceOf<IHeaveCalculator>(calculator);
            Assert.AreEqual(0.0, calculator.Zh);
        }
    }
}