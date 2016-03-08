using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.Test.SubCalculator
{
    [TestFixture]
    public class UpliftCalculatorTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var calculator = new UpliftCalculator();

            // Assert
            Assert.IsInstanceOf<IUpliftCalculator>(calculator);
            Assert.AreEqual(double.NaN, calculator.Zu);
        }  
    }
}