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
    public class FailureMechanismPropertyChangeHandler<T> : IFailureMechanismPropertyChangeHandler<T> where T : IFailureMechanism
    {
        /// <summary>
        /// Checks whether a call to <see cref="PropertyChanged"/> would have any effect in the given
        /// <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to check for.</param>
        /// <returns><c>true</c> if <see cref="PropertyChanged"/> would result in changes, 
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        private bool RequiresConfirmation(T failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            return failureMechanism.Calculations.Any(c => c.HasOutput);
        }

        public bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(ConfirmationMessage,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public virtual IEnumerable<IObservable> PropertyChanged(T failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            var affected = new List<IObservable>();
            foreach (var calculation in failureMechanism.Calculations.Where(c => c.HasOutput))
            {
                affected.Add(calculation);
                calculation.ClearOutput();
            }
            return affected;
        }

        /// <summary>
        /// Gets the message that is shown when conformation is inquired.
        /// </summary>
        protected virtual string ConfirmationMessage
        {
            get
            {
                return Resources.FailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data;
            }
        }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(
            T failureMechanism,
            TValue value, SetFailureMechanismPropertyValueDelegate<T, TValue> setValue)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            var changedObjects = new List<IObservable>();

            if (RequiresConfirmation(failureMechanism))
            {
                if (ConfirmPropertyChange())
                {
                    setValue(failureMechanism, value);
                    changedObjects.AddRange(PropertyChanged(failureMechanism));
                    changedObjects.Add(failureMechanism);
                }
            }
            else
            {
                setValue(failureMechanism, value);
                changedObjects.Add(failureMechanism);
            }

            return changedObjects;
        }
    }
}