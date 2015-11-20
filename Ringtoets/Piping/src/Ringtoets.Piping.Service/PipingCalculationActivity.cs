﻿using Core.Common.Base.Workflow;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// <see cref="IActivity"/> for running a piping calculation.
    /// </summary>
    public class PipingCalculationActivity : Activity
    {
        private readonly PipingCalculationData pipingData;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationActivity"/> class.
        /// </summary>
        /// <param name="pipingData">The piping data used for the calculation.</param>
        public PipingCalculationActivity(PipingCalculationData pipingData)
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
            Status = ActivityStatus.Done;
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