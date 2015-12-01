using Core.Common.Base.Workflow;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a piping calculation.
    /// </summary>
    public class PipingCalculationActivity : Activity
    {
        private readonly PipingCalculation calculation;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationActivity"/> class.
        /// </summary>
        /// <param name="calculation">The piping data used for the calculation.</param>
        public PipingCalculationActivity(PipingCalculation calculation)
        {
            this.calculation = calculation;
        }

        public override string Name
        {
            get
            {
                return calculation.Name;
            }
        }

        protected override void OnInitialize()
        {
            if (!PipingCalculationService.Validate(calculation))
            {
                Status = ActivityStatus.Failed;
            }
            else
            {
                calculation.Output = null;
            }
        }

        protected override void OnExecute()
        {
            PipingCalculationService.Calculate(calculation);
            Status = ActivityStatus.Done;
        }

        protected override void OnCancel()
        {
            // Unable to cancel a running kernel, so nothing can be done.
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }
    }
}