using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.Test.SubCalculator
{
    [TestFixture]
    public class PipingSubCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var factory = new PipingSubCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IPipingSubCalculatorFactory>(factory);
        }

        [Test]
        public void CreateHeaveCalculator_WithInput_NewHeaveCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.IsInstanceOf<IHeaveCalculator>(calculator);
        }

        [Test]
        public void CreateUpliftCalculator_WithInput_NewUpliftCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.IsInstanceOf<IUpliftCalculator>(calculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_WithInput_NewSellmeijerCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.IsInstanceOf<ISellmeijerCalculator>(calculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_WithInput_NewSellmeijerCalculator()
        {
            // Setup
            var factory = new PipingSubCalculatorFactory();

            // Call
            var calculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.IsInstanceOf<IEffectiveThicknessCalculator>(calculator);
        }
    }
}