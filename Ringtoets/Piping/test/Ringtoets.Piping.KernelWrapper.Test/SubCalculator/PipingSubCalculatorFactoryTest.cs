using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class PipingSubCalculatorFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            var factory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.IsInstanceOf<IPipingSubCalculatorFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsANewInstance()
        {
            var firstFactory = PipingSubCalculatorFactory.Instance;
            PipingSubCalculatorFactory.Instance = null;

            // Call
            var secondFactory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestPipingSubCalculatorFactory();
            PipingSubCalculatorFactory.Instance = firstFactory;

            // Call
            var secondFactory = PipingSubCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateHeaveCalculator_Always_NewHeaveCalculator()
        {
            // Setup
            var factory = PipingSubCalculatorFactory.Instance;

            // Call
            var calculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.IsInstanceOf<IHeaveCalculator>(calculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_NewUpliftCalculator()
        {
            // Setup
            var factory = PipingSubCalculatorFactory.Instance;

            // Call
            var calculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.IsInstanceOf<IUpliftCalculator>(calculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_NewSellmeijerCalculator()
        {
            // Setup
            var factory = PipingSubCalculatorFactory.Instance;

            // Call
            var calculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.IsInstanceOf<ISellmeijerCalculator>(calculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_NewSellmeijerCalculator()
        {
            // Setup
            var factory = PipingSubCalculatorFactory.Instance;

            // Call
            var calculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.IsInstanceOf<IEffectiveThicknessCalculator>(calculator);
        }

        [Test]
        public void CreatPiezometricHeadAtExitCalculator_Always_NewPizometricHeadAtExitCalculator()
        {
            // Setup
            var factory = PipingSubCalculatorFactory.Instance;

            // Call
            var calculator = factory.CreatePiezometricHeadAtExitCalculator();

            // Assert
            Assert.IsInstanceOf<IPiezoHeadCalculator>(calculator);
        }
    }
}