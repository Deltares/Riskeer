using System;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class PipingSubCalculatorFactoryConfigTest
    {
        [Test]
        public void Constructor_NewInstanceCanBeDisposed()
        {
            // Call
            var factory = new PipingSubCalculatorFactoryConfig();

            // Assert
            Assert.IsInstanceOf<IDisposable>(factory);
            Assert.DoesNotThrow(() => factory.Dispose());
        }

        [Test]
        public void Constructor_SetsTestFactoryForPipingCalculatorService()
        {
            // Call
            using (new PipingSubCalculatorFactoryConfig())
            {
                // Assert
                Assert.IsInstanceOf<TestPipingSubCalculatorFactory>(PipingSubCalculatorFactory.Instance);
            }
        }

        [Test]
        public void Dispose_Always_ResetsFactoryToPreviousValue()
        {
            // Setup
            var expectedFactory = PipingSubCalculatorFactory.Instance;

            // Call
            using (new PipingSubCalculatorFactoryConfig()) { }

            // Assert
            Assert.AreSame(expectedFactory, PipingSubCalculatorFactory.Instance);
        }
    }
}