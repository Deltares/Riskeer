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
        public bool RequiresConfirmation(T failureMechanism)
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

        /// <summary>
        /// Find out whether the property can be updated with or without confirmation. If confirmation is required, 
        /// the confirmation is obtained, after which the property is set if confirmation is given. If no confirmation
        /// was required, then the value will be set for the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value that is set on a property of the failure mechanism.</typeparam>
        /// <param name="failureMechanism">The failure mechanism for which the property is supposed to be set.</param>
        /// <param name="value">The new value of the failure mechanism property.</param>
        /// <param name="setValue">The operation which is performed to set the new property <paramref name="value"/>
        /// on the <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <remarks>Let <paramref name="setValue"/> throw an <see cref="Exception"/> when the 
        /// <see cref="FailureMechanismPropertyChangeHandler{T}"/> should not process the results of that operation.</remarks>
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