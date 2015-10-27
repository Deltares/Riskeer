using DelftTools.Shell.Core.Workflow;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// <see cref="IActivity"/> for running a piping calculation.
    /// </summary>
    public class PipingCalculationActivity : Activity
    {
        private readonly PipingData pipingData;

        public PipingCalculationActivity(PipingData pipingData)
        {
            this.pipingData = pipingData;
        }

        public override string Name
        {
            get
            {
                return pipingData.Name;
            }
        }

        protected override void OnInitialize()
        {
            if (!PipingCalculationService.Validate(pipingData))
            {
                Status = ActivityStatus.Failed;
            }
            else
            {
                pipingData.Output = null;
            }
        }

        protected override void OnExecute()
        {
            PipingCalculationService.Calculate(pipingData);
        }

        protected override void OnCancel()
        {
            // Unable to cancel a running kernel, so nothing can be done.
        }

        protected override void OnCleanUp()
        {
            // Nothing to clean up.
        }

        protected override void OnFinish()
        {
            pipingData.NotifyObservers();
        }
    }
}