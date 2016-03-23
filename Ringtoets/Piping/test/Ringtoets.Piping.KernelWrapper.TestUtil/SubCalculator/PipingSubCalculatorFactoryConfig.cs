using System;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestPipingSubCalculatorFactory"/> 
    /// for <see cref="PipingSubCalculatorFactory.Instance"/> while testing. 
    /// Disposing an instance of this class will revert the 
    /// <see cref="PipingSubCalculatorFactory.Instance"/>.
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
    public class PipingSubCalculatorFactoryConfig : IDisposable
    {
        private readonly IPipingSubCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSubCalculatorFactoryConfig"/>.
        /// Sets a <see cref="TestPipingSubCalculatorFactory"/> to 
        /// <see cref="PipingSubCalculatorFactory.Instance"/>
        /// </summary>
        public PipingSubCalculatorFactoryConfig()
        {
            previousFactory = PipingSubCalculatorFactory.Instance;
            PipingSubCalculatorFactory.Instance = new TestPipingSubCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="PipingSubCalculatorFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="PipingSubCalculatorFactoryConfig"/>.
        /// </summary>
        public void Dispose()
        {
            PipingSubCalculatorFactory.Instance = previousFactory;
        }
    }
}