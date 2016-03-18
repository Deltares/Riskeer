using System;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.InputParameterCalculation.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestPipingSubCalculatorFactory"/> 
    /// for <see cref="InputParameterCalculationService.SubCalculatorFactory"/> while testing. 
    /// Disposing an instance of this class will revert the 
    /// <see cref="PipingCalculationSerInputParameterCalculationServicevice.SubCalculatorFactory"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new InputParameterCalculationServiceConfig()) {
    ///     var testFactory = (TestPipingSubCalculatorFactory) InputParameterCalculationService.SubCalculatorFactory;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class InputParameterCalculationServiceConfig : IDisposable
    {
        private readonly IPipingSubCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="InputParameterCalculationServiceConfig"/>.
        /// Sets a <see cref="TestPipingSubCalculatorFactory"/> to 
        /// <see cref="InputParameterCalculationService.SubCalculatorFactory"/>
        /// </summary>
        public InputParameterCalculationServiceConfig()
        {
            previousFactory = InputParameterCalculationService.SubCalculatorFactory;
            InputParameterCalculationService.SubCalculatorFactory = new TestPipingSubCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="InputParameterCalculationService.SubCalculatorFactory"/> to the value
        /// it had at time of construction of the <see cref="InputParameterCalculationServiceConfig"/>.
        /// </summary>
        public void Dispose()
        {
            InputParameterCalculationService.SubCalculatorFactory = previousFactory;
        }
    }
}