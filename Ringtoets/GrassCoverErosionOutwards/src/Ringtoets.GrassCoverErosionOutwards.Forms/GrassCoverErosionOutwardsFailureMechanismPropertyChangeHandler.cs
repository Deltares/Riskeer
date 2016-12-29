using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// grass cover erosion outwards failure mechanism property.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler : IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler
    {
        private readonly FailureMechanismPropertyChangeHandler failureMechanismPropertyChangeHandler = new FailureMechanismPropertyChangeHandler();

        public bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(Resources.GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> PropertyChanged(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                failureMechanism.HydraulicBoundaryLocations);
            return affectedObjects.Concat(failureMechanismPropertyChangeHandler.PropertyChanged(failureMechanism));
        }
    }
}