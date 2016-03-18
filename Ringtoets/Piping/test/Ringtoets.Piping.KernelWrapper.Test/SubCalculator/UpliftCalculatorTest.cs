using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
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
            Assert.AreEqual(double.NaN, calculator.FoSu);
        }
    }
}