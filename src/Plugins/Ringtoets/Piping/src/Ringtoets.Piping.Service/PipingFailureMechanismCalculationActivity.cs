using System.Linq;

using DelftTools.Shell.Core.Workflow;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    public class PipingFailureMechanismCalculationActivity : SequentialActivity
    {
        public PipingFailureMechanismCalculationActivity(PipingFailureMechanism pipingFailureMechanism)
        {
            Activities.AddRange(pipingFailureMechanism.Calculations.Select(c => new PipingCalculationActivity(c)));
        }
    }
}