using System;
using NUnit.Framework;
using Ringtoets.Piping.Calculation.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Service.TestUtil.Test
{
    [TestFixture]
    public class PipingCalculationServiceConfigTest
    {
        [Test]
        public void Constructor_NewInstanceCanBeDisposed()
        {
            // Call
            var factory = new PipingCalculationServiceConfig();

            // Assert
            Assert.IsInstanceOf<IDisposable>(factory);
            Assert.DoesNotThrow(() => factory.Dispose());
        }

        [Test]
        public void Constructor_SetsTestFactoryForPipingCalculatorService()
        {
            // Call
            using (new PipingCalculationServiceConfig())
            {
                // Assert
                Assert.IsInstanceOf<TestPipingSubCalculatorFactory>(PipingCalculationService.SubCalculatorFactory);
            }
        }

        [Test]
        public void Dispose_Always_ResetsFactoryToPreviousValue()
        {
            // Setup
            var expectedFactory = PipingCalculationService.SubCalculatorFactory;

            // Call
            using (new PipingCalculationServiceConfig()) { }

            // Assert
            Assert.AreSame(expectedFactory, PipingCalculationService.SubCalculatorFactory);
        }
    }
}