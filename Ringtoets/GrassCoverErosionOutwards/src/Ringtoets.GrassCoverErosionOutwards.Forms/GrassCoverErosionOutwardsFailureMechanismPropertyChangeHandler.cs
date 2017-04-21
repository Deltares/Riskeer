using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionOutwards.Forms
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// grass cover erosion outwards failure mechanism property.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler : FailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>
    {
        protected override string ConfirmationMessage
        {
            get
            {
                return Resources.GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data;
            }
        }

        protected override bool RequiresConfirmation(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return base.RequiresConfirmation(failureMechanism) ||
                   failureMechanism.HydraulicBoundaryLocations.Any(c => c.WaveHeightOutput != null || c.DesignWaterLevelOutput != null);
        }

        protected override IEnumerable<IObservable> PropertyChanged(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            var affectedObjects = new List<IObservable>(base.PropertyChanged(failureMechanism));

            IEnumerable<IObservable> affectedLocations = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                failureMechanism.HydraulicBoundaryLocations);

            if (affectedLocations.Any())
            {
                affectedObjects.Add(failureMechanism.HydraulicBoundaryLocations);
            }
            return affectedObjects;
        }
    }
}