using System;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Service.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestPipingSubCalculatorFactory"/> 
    /// for <see cref="PipingCalculationService.SubCalculatorFactory"/> while testing. 
    /// Disposing an instance of this class will revert the 
    /// <see cref="PipingCalculationService.SubCalculatorFactory"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new PipingCalculationServiceConfig()) {
    ///     var testFactory = (TestPipingSubCalculatorFactory) PipingCalculationService.SubCalculatorFactory;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class PipingCalculationServiceConfig : IDisposable
    {
        private readonly IPipingSubCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationServiceConfig"/>.
        /// Sets a <see cref="TestPipingSubCalculatorFactory"/> to 
        /// <see cref="PipingCalculationService.SubCalculatorFactory"/>
        /// </summary>
        public PipingCalculationServiceConfig()
        {
            previousFactory = PipingCalculationService.SubCalculatorFactory;
            PipingCalculationService.SubCalculatorFactory = new TestPipingSubCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="PipingCalculationService.SubCalculatorFactory"/> to the value
        /// it had at time of construction of the <see cref="PipingCalculationServiceConfig"/>.
        /// </summary>
        public void Dispose()
        {
            PipingCalculationService.SubCalculatorFactory = previousFactory;
        }
    }
}