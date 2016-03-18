using System;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.InputParameterCalculation.TestUtil.Test
{
    [TestFixture]
    public class InputParameterCalculationServiceConfigTest
    {
        [Test]
        public void Constructor_NewInstanceCanBeDisposed()
        {
            // Call
            var factory = new InputParameterCalculationServiceConfig();

            // Assert
            Assert.IsInstanceOf<IDisposable>(factory);
            Assert.DoesNotThrow(() => factory.Dispose());
        }

        [Test]
        public void Constructor_SetsTestFactoryForPipingCalculatorService()
        {
            // Call
            using (new InputParameterCalculationServiceConfig())
            {
                // Assert
                Assert.IsInstanceOf<TestPipingSubCalculatorFactory>(InputParameterCalculationService.SubCalculatorFactory);
            }
        }

        [Test]
        public void Dispose_Always_ResetsFactoryToPreviousValue()
        {
            // Setup
            var expectedFactory = InputParameterCalculationService.SubCalculatorFactory;

            // Call
            using (new InputParameterCalculationServiceConfig()) { }

            // Assert
            Assert.AreSame(expectedFactory, InputParameterCalculationService.SubCalculatorFactory);
        }
    }
}