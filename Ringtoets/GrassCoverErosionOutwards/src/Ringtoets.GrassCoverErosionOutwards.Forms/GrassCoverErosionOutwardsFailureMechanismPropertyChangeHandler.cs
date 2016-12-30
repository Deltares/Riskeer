using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionOutwards.Forms
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// grass cover erosion outwards failure mechanism property.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler : FailureMechanismPropertyChangeHandler, IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>
    {
        protected override string ConfirmationMessage
        {
            get
            {
                return Resources.GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data;
            }
        }

        public IEnumerable<IObservable> PropertyChanged(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                failureMechanism.HydraulicBoundaryLocations);
            return affectedObjects.Concat(base.PropertyChanged(failureMechanism));
        }
    }
}