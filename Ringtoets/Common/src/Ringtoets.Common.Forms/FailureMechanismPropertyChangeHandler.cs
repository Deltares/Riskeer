using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// failure mechanism property.
    /// </summary>
    public class FailureMechanismPropertyChangeHandler : IFailureMechanismPropertyChangeHandler
    {
        public bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(Resources.FailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> PropertyChanged(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            var affected = new List<IObservable>();
            foreach (var calculation in failureMechanism.Calculations.Where(c => c.HasOutput))
            {
                affected.Add(calculation);
                calculation.ClearOutput();
            }
            return affected;
        }
    }
}