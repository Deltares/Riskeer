using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;
using Ringtoets.Piping.Calculation.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Calculation.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class TestPipingSubCalculatorFactoryTest
    {
        [Test]
        public void DefaultConstructor_SetDefaultProperties()
        {
            // Call
            var factory = new TestPipingSubCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IPipingSubCalculatorFactory>(factory);
            Assert.IsNull(factory.LastCreatedEffectiveThicknessCalculator);
            Assert.IsNull(factory.LastCreatedUpliftCalculator);
            Assert.IsNull(factory.LastCreatedSellmeijerCalculator);
            Assert.IsNull(factory.LastCreatedHeaveCalculator);
        }

        [Test]
        public void CreateEffectiveThicknessCalculator_Always_LastCreatedEffectiveThicknessCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateEffectiveThicknessCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedEffectiveThicknessCalculator, subCalculator);
        }

        [Test]
        public void CreateUpliftCalculator_Always_LastCreatedUpliftCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateUpliftCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftCalculator, subCalculator);
        }

        [Test]
        public void CreateHeaveCalculator_Always_LastCreatedHeaveCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateHeaveCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedHeaveCalculator, subCalculator);
        }

        [Test]
        public void CreateSellmeijerCalculator_Always_LastCreatedSellmeijerCalculatorSetToReturnValue()
        {
            // Setup
            var factory = new TestPipingSubCalculatorFactory();

            // Call
            var subCalculator = factory.CreateSellmeijerCalculator();

            // Assert
            Assert.AreSame(factory.LastCreatedSellmeijerCalculator, subCalculator);
        }
    }
}