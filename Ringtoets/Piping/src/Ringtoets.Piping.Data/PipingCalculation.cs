using Core.Common.Base;

using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information which can be made visible in the graphical interface of the application.
    /// </summary>
    public class PipingCalculation : Observable, IPipingCalculationItem
    {
        /// <summary>
        /// Constructs a new instance of <see cref="PipingCalculation"/> with default values set for some of the parameters.
        /// </summary>
        public PipingCalculation()
        {
            Name = Resources.PipingCalculationData_DefaultName;

            Comments = new InputPlaceholder(Resources.Comments_DisplayName);
            InputParameters = new PipingInput();
            CalculationReport = new PlaceholderWithReadonlyName(Resources.CalculationReport_DisplayName);
        }

        /// <summary>
        /// Gets the user notes for this calculation.
        /// </summary>
        public PlaceholderWithReadonlyName Comments { get; private set; }

        /// <summary>
        /// Gets the input parameters to perform a piping calculation with.
        /// </summary>
        public PipingInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a Piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

        /// <summary>
        /// Gets the calculation report for the calculation that generated <see cref="Output"/>.
        /// </summary>
        public PlaceholderWithReadonlyName CalculationReport { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PipingCalculation"/> has <see cref="Output"/>.
        /// </summary>
        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        /// <summary>
        /// Clears the <see cref="Output"/>.
        /// </summary>
        public void ClearOutput()
        {
            Output = null;
        }
    }
}